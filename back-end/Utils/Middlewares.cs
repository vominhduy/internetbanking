using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using InternetBanking.Models.Request;
using InternetBanking.Models.ViewModels;
using InternetBanking.Services;

namespace InternetBanking.Utils
{
    public class Middlewares
    {
        private readonly RequestDelegate _next;
        private static IEncrypt _encrypt;
        private static ILinkingBankService _linkingBank;

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
        public async Task Invoke(HttpContext httpContext, IEncrypt iencrypt, ILinkingBankService linkingBank)
        {
            _encrypt = iencrypt;
            _linkingBank = linkingBank;

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
                    var resultCheck = CheckBasicAuthenForPartner(request);
                    if (resultCheck.Item1 != 1)
                    {
                        var obj = new
                        {
                            code = -1,
                            message = resultCheck.Item2,
                            data = (string)null
                        };
                        var response = httpContext.Response;
                        response.ContentType = "application/json";
                        response.StatusCode = resultCheck.Item1 == 500 ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest;
                        response.WriteAsync(JsonConvert.SerializeObject(obj)).Wait();
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
                    var task = Task.Run(() => ReadRequestBody(request)).GetAwaiter();
                    string body = task.GetResult();
                    if (!Encrypting.MD5Verify(string.Concat(body, keyReq, timestamp), checksumReq))
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
        private Tuple<int, string> CheckBasicAuthenForPartner(HttpRequest request)
        {
            StringBuilder log = new StringBuilder();
            Tuple<int, string> result = new Tuple<int, string>(1, "success");
            try
            {
                try
                {
                    log.AppendLine(request.Path.Value);
                    log.AppendLine(JsonConvert.SerializeObject(request.Headers));
                    var task = Task.Run(() => ReadRequestBody(request)).GetAwaiter();
                    log.AppendLine(task.GetResult());
                }
                catch (Exception)
                {
                    log.AppendLine("null body");
                }

                var keys = new[] {
                    "f936792f71344a6eabf773f18e2694e4",
                    "99793bb9137042a3a7f15950f1215950",// khuê
                    "bkt.partner"
                };

                long timestampReq = long.Parse(request.Query["timestamp"].ToString());
                string keyReq = request.Query["partner_code"].ToString();
                string checksumReq = request.Query["hash"].ToString();

                // A kiểm tra lời gọi api có phải xuất phát từ B (đã đăng ký liên kết từ trước) hay không
                if (!keys.Any(x => x.Equals(keyReq)))
                {
                    return new Tuple<int, string>(400, "partner_code invalid");
                }

                // A kiểm tra xem lời gọi này là mới hay là thông tin cũ đã quá hạn
                long timestamp = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(-180)).ToUnixTimeSeconds();
                if (timestamp > timestampReq)
                {
                    return new Tuple<int, string>(400, "timestamp expired");
                }

                // Check toàn vẹn dữ liệu
                if (request.Method.Equals("POST"))
                {
                    if (request.Path.Value.ToLower().Contains("api/transactions/receive_external".ToLower()))
                    {
                        var infoPartner = _linkingBank.GetLinkingBankById(new Models.Filters.LinkingBankFilter() { Code = keyReq });
                        if (infoPartner == null)
                        {
                            return new Tuple<int, string>(500, "internal server error");
                        }
                        var task = Task.Run(() => ReadRequestBody(request)).GetAwaiter();
                        var temp = task.GetResult();
                        var obj = JsonConvert.DeserializeObject<TransferMoneyRequest>(temp);
                        string secretKey = infoPartner.SecretKey;
                        string input = $"{keyReq}|{timestampReq}|{obj.from_account_number}|{obj.to_account_number}|{(int)obj.amount}|{obj.message}";

                        if (!Encrypting.HMD5Verify(input, checksumReq, secretKey))
                        {
                            log.Append("Hash: false");
                            return new Tuple<int, string>(400, "hash invalid");
                        }

                        // Nếu là controller partners thì check thêm mã hóa bất đối xứng
                        string encrypt = request.Query["signature"].ToString();
                        if (!string.IsNullOrWhiteSpace(encrypt))
                        {
                            string hash = Encrypting.HMD5Hash(input, secretKey);
                            _encrypt.SetKey(keyReq);
                            if (_encrypt.DecryptData(encrypt, hash))
                            {
                                return result;
                            }
                            else
                            {
                                log.Append("DecryptData: false");
                                return new Tuple<int, string>(400, "signature invalid");
                            }
                        }
                        else
                        {
                            log.Append("DecryptData: false");
                            return new Tuple<int, string>(400, "signature invalid");
                        }
                    }
                    else if (request.Path.Value.ToLower().Contains("api/transactions/query_info".ToLower()))
                    {
                        var infoPartner = _linkingBank.GetLinkingBankById(new Models.Filters.LinkingBankFilter() { Code = keyReq });
                        if (infoPartner == null)
                        {
                            return new Tuple<int, string>(500, "internal server error");
                        }

                        var task = Task.Run(() => ReadRequestBody(request)).GetAwaiter();
                        var temp =task.GetResult();
                        var obj = JsonConvert.DeserializeObject<InfoUserRequest>(temp);
                        string secretKey = infoPartner.SecretKey;
                        string hash = $"{keyReq}|{timestampReq}|{obj.account_number}";

                        if (!Encrypting.HMD5Verify(hash, checksumReq, secretKey))
                        {
                            log.Append("Hash: false");
                            return new Tuple<int, string>(400, "hash invalid");
                        }
                    }
                    else
                    {
                        return new Tuple<int, string>(400, "invalid url");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Append(ex.Message);
                return new Tuple<int, string>(500, "internal server error");
            }
            finally
            {
                LoggingTxt.InsertLog(log.ToString());
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            var temp = JsonConvert.DeserializeObject(bodyAsText);
            bodyAsText = JsonConvert.SerializeObject(temp);
            return await Task.FromResult(bodyAsText);
        }
    }
}
