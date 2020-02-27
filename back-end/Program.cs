using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;


namespace InternetBanking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // use this to allow command line parameters in the config
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("log.json")
                .AddCommandLine(args)
                .Build();

            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                return Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(configuration))
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            config
                                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                                .AddConfiguration(configuration)
                                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                                .AddEnvironmentVariables();
                        })
                        .UseStartup<Startup>();
                    });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
