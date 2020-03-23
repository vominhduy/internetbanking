using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using InternetBanking.Models.Request;
using InternetBanking.Models.ViewModels;

namespace InternetBanking.Utils
{
    public class Middlewares
    {
        private readonly RequestDelegate _next;
        private static IEncrypt _encrypt;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public Middlewares(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, IEncrypt iencrypt)
        {
            StringBuilder log = new StringBuilder();
            _encrypt = iencrypt;
            bool result = false;
            try
            {
                try
                {
                    log.AppendLine(httpContext.Request.Path.Value);
                    log.AppendLine(JsonConvert.SerializeObject(httpContext.Request.Headers));
                    log.AppendLine(ReadRequestBody(httpContext.Request));
                }
                catch (Exception)
                {
                    log.AppendLine("null body");
                }

                var request = httpContext.Request;

                // Test only
                string admin = request.Headers["admin_key"];
                if (!string.IsNullOrWhiteSpace(admin)
                    && admin.ToLower().Equals("09411a3942454ec9b36e3bcaf1d69f22".ToLower()))
                {
                    result = true;
                    await _next(httpContext);
                    return;
                }
                // End Test only

                if (!CheckBasicAuthen(request))
                {
                    log.Append("CheckBasicAuthen: false");
                    return;
                }

                // Nếu là controller partners thì check thêm mã hóa bất đối xứng
                if (request.Path.Value.ToLower().Contains("api/transactions/receive_external".ToLower()))
                {
                    string keyReq = request.Headers["partner_code"];
                    string encrypt = request.Headers["signature"];
                    long timestampReq = long.Parse(request.Headers["timestamp"]);
                    string checksumReq = request.Headers["hash"];

                    if (!string.IsNullOrWhiteSpace(encrypt))
                    {
                        var temp = ReadRequestBody(request);
                        var obj = JsonConvert.DeserializeObject<TransferMoneyRequest>(temp);
                        string input = $"{keyReq}|{timestampReq}|{obj.from_account_number}|{obj.to_account_number}|{(int)obj.amount}|{obj.message}";
                        string hash = Encrypting.HMD5Hash(input, keyReq);

                        _encrypt.SetKey(keyReq);
                        if (_encrypt.DecryptData(encrypt, hash))
                        {
                            result = true;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.Message);
                return;
            }
            finally
            {
                if (httpContext.Request.Path.Value.ToLower().Contains("api/transactions".ToLower()))
                {
                    LoggingTxt.InsertLog(log.ToString());
                }

                if (!result)
                {
                    var response = httpContext.Response;
                    response.ContentType = "application/json";
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                }               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool CheckBasicAuthen(HttpRequest request)
        {
            try
            {
                var ignores = new[] { "WeatherForecast" };
                var keys = new[] {
                    "75836f6ded2047c4b1f5770c3229fc02", // key for front-end
                    "a2660f0f7e3b44cb8a08bf79ac7e94ae",
                    "26dee8c166394501810905fee8a992ba",
                    "e44be7e772364f048523508bbcf08cc3",
                    "098fb55748ad4e4aacc64ea16a07998c",
                    "35d4baf7ea9843a99870eaaac90382ad",
                    "a9030ad3fb5943dd90392480f451e18e",
                    "f936792f71344a6eabf773f18e2694e4",
                    "99793bb9137042a3a7f15950f1215950", // khuê
                    "09411a3942454ec9b36e3bcaf1d69f22" // Da dung
                };

                if (ignores.Any(x => request.Path.Value.ToLower().Contains(x.ToLower())))
                {
                    return true;
                }

                long timestampReq = long.Parse(request.Headers["timestamp"]);
                string keyReq = request.Headers["partner_code"];
                string checksumReq = request.Headers["hash"];

                // A kiểm tra lời gọi api có phải xuất phát từ B (đã đăng ký liên kết từ trước) hay không
                if (!keys.Any(x => x.Equals(keyReq)))
                {
                    return false;
                }

                // A kiểm tra xem lời gọi này là mới hay là thông tin cũ đã quá hạn
                long timestamp = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(-50)).ToUnixTimeSeconds();
                if (timestamp > timestampReq)
                {
                    return false;
                }

                // Không checksum với method GET
                if (request.Method.Equals("POST")
                    || request.Method.Equals("PUT"))
                {
                    if (request.Path.Value.ToLower().Contains("api/transactions/receive_external".ToLower()))
                    {
                        var temp = ReadRequestBody(request);
                        var obj = JsonConvert.DeserializeObject<TransferMoneyRequest>(temp);
                        string secretKey = "99793bb9137042a3a7f15950f1215950";
                        string input = $"{keyReq}|{timestampReq}|{obj.from_account_number}|{obj.to_account_number}|{(int)obj.amount}|{obj.message}";

                        if (!Encrypting.HMD5Verify(input,checksumReq, secretKey))
                        {
                            return false;
                        }
                    }
                    else if (request.Path.Value.ToLower().Contains("api/transactions/query_info".ToLower()))
                    {
                        var temp = ReadRequestBody(request);
                        var obj = JsonConvert.DeserializeObject<InfoUserRequest>(temp);
                        string secretKey = "99793bb9137042a3a7f15950f1215950";
                        string hash = $"{keyReq}|{timestampReq}|{obj.account_number}";

                        if (!Encrypting.HMD5Verify(hash, checksumReq, secretKey))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // A kiểm tra xem gói tin B gửi qua là gói tin nguyên bản hay gói tin đã bị chỉnh sửa
                        if (!Encrypting.MD5Verify(string.Concat(ReadRequestBody(request), keyReq, timestamp), checksumReq))
                        {
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            var temp = JsonConvert.DeserializeObject(bodyAsText);
            bodyAsText = JsonConvert.SerializeObject(temp);
            return bodyAsText;
        }
    }
}
