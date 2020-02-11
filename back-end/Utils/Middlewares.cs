using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace InternetBanking.Utils
{
    public class Middlewares
    {
        private readonly RequestDelegate _next;

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
        public async Task Invoke(HttpContext httpContext)
        {
            bool result = false;
            try
            {
                var request = httpContext.Request;
                if (!CheckBasicAuthen(request))
                {
                    return;
                }
                result = true;
                await _next(httpContext);
            }
            catch
            {
                return;
            }
            finally
            {
                if (!result)
                {
                    var response = httpContext.Response;
                    response.ContentType = "application/json";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
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
                var ignores = new[] { "controller1" };
                var keys = new[] { "key" };
                if (ignores.Any(x => request.Path.Value.Contains(x)))
                {
                    return true;
                }

                long timestampReq = long.Parse(request.Headers["timestamp"]);
                string keyReq = request.Headers["key"];
                string checksumReq = request.Headers["checksum"];

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

                // A kiểm tra xem gói tin B gửi qua là gói tin nguyên bản hay gói tin đã bị chỉnh sửa
                if (!Encrypting.MD5Verify(ReadRequestBody(request), checksumReq))
                {
                    return false;
                }
            }
            catch
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
            request.Body.Read(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);
            return bodyAsText;
        }
    }
}
