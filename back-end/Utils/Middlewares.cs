using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
            _encrypt = iencrypt;
            try
            {
                var request = httpContext.Request;

                // Test only
                string admin = request.Headers["admin_key"];
                if (!string.IsNullOrWhiteSpace(admin)
                    && admin.ToLower().Equals("09411a3942454ec9b36e3bcaf1d69f22".ToLower()))
                {
                    await _next(httpContext);
                    return;
                }
                // End Test only

                if (request.Path.Value.ToLower().Contains("api/transactions".ToLower()))
                {
                    if (!CheckBasicAuthenForPartner(request))
                    {
                        var response = httpContext.Response;
                        response.ContentType = "application/json";
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        return;
                    }
                    await _next(httpContext);
                }
                else
                {
                    if (!CheckBasicAuthen(request))
                    {
                        var response = httpContext.Response;
                        response.ContentType = "application/json";
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
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
                long timestamp = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(-5)).ToUnixTimeSeconds();
                if (timestamp > timestampReq)
                {
                    return false;
                }

                // Không checksum với method GET
                if (request.Method.Equals("POST")
                    || request.Method.Equals("PUT"))
                {
                    // A kiểm tra xem gói tin B gửi qua là gói tin nguyên bản hay gói tin đã bị chỉnh sửa
                    if (!Encrypting.MD5Verify(string.Concat(ReadRequestBody(request), keyReq, timestamp), checksumReq))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
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
        private bool CheckBasicAuthenForPartner(HttpRequest request)
        {
            StringBuilder log = new StringBuilder();
            try
            {
                try
                {
                    log.AppendLine(request.Path.Value);
                    log.AppendLine(JsonConvert.SerializeObject(request.Headers));
                    log.AppendLine(ReadRequestBody(request));
                }
                catch (Exception)
                {
                    log.AppendLine("null body");
                }

                var keys = new[] {
                    "99793bb9137042a3a7f15950f1215950", // khuê
                };

                long timestampReq = long.Parse(request.Query["timestamp"].ToString());
                string keyReq = request.Query["partner_code"].ToString();
                string checksumReq = request.Query["hash"].ToString();

                // A kiểm tra lời gọi api có phải xuất phát từ B (đã đăng ký liên kết từ trước) hay không
                if (!keys.Any(x => x.Equals(keyReq)))
                {
                    return false;
                }

                // A kiểm tra xem lời gọi này là mới hay là thông tin cũ đã quá hạn
                long timestamp = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(-180)).ToUnixTimeSeconds();
                if (timestamp > timestampReq)
                {
                    return false;
                }

                // Check toàn vẹn dữ liệu
                if (request.Method.Equals("POST"))
                {
                    if (request.Path.Value.ToLower().Contains("api/transactions/receive_external".ToLower()))
                    {
                        var temp = ReadRequestBody(request);
                        var obj = JsonConvert.DeserializeObject<TransferMoneyRequest>(temp);
                        string secretKey = request.Query["partner_code"].ToString().Trim();
                        string input = $"{keyReq}|{timestampReq}|{obj.from_account_number}|{obj.to_account_number}|{(int)obj.amount}|{obj.message}";

                        if (!Encrypting.HMD5Verify(input, checksumReq, secretKey))
                        {
                            log.Append("Hash: false");
                            return false;
                        }

                        // Nếu là controller partners thì check thêm mã hóa bất đối xứng
                        string encrypt = request.Headers["signature"];
                        if (!string.IsNullOrWhiteSpace(encrypt))
                        {
                            string hash = Encrypting.HMD5Hash(input, keyReq);
                            _encrypt.SetKey(keyReq);
                            if (_encrypt.DecryptData(encrypt, hash))
                            {
                                return true;
                            }
                            else
                            {
                                log.Append("DecryptData: false");
                                return false;
                            }
                        }
                        else
                        {
                            log.Append("DecryptData: false");
                            return false;
                        }
                    }
                    else if (request.Path.Value.ToLower().Contains("api/transactions/query_info".ToLower()))
                    {
                        var temp = ReadRequestBody(request);
                        var obj = JsonConvert.DeserializeObject<InfoUserRequest>(temp);
                        string secretKey = request.Query["partner_code"].ToString().Trim();
                        string hash = $"{keyReq}|{timestampReq}|{obj.account_number}";

                        if (!Encrypting.HMD5Verify(hash, checksumReq, secretKey))
                        {
                            log.Append("Hash: false");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Append(ex.Message);
                return false;
            }
            finally
            {
                LoggingTxt.InsertLog(log.ToString());
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
