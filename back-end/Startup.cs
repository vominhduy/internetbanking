using System;
using System.Net;
using System.Text;
using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.DataCollections.Implementations;
using InternetBanking.Services;
using InternetBanking.Services.Implementations;
using InternetBanking.Settings;
using InternetBanking.Settings.Implementations;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InternetBanking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            AppSetting appSettings = new AppSetting(new Message());
            var appSettingsSection = Configuration.GetSection("Settings");

            appSettingsSection.Bind(appSettings);

            services.AddSingleton<MongoDBClient>(new MongoDBClient(appSettings.DBEndpoint, appSettings.DBName));

            services.AddSingleton<ISetting>(appSettings);

            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);

            // jwt token authorize
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.ApplicationToken)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());



            /// Add collections
            services.AddSingleton<IEmployeeCollection, MongoEmployeeCollection>();
            services.AddSingleton<IUserCollection, MongoUserCollection>();
            services.AddSingleton<IDeptReminderCollection, MongoDeptReminderCollection>();
            services.AddSingleton<ITransferCollection, MongoTransferCollection>();
            ///
            /// Add services
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IDeptReminderService, DeptReminderService>();
            ///
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            System.Reflection.Assembly ass = System.Reflection.Assembly.GetEntryAssembly();
            string serviceName = ass.GetName().Name;
            var _logger = loggerFactory.CreateLogger(serviceName);

            app.ConfigureExceptionHandler(_logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Server error: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
