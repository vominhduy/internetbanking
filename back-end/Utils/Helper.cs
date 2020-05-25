using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace InternetBanking.Utils
{
    public class Helper
    {
        public static T CallAPI<T>(string api, string method, object obj, Dictionary<string, string> headers = null, bool addQueryParams = false) where T : class
        {
            string json = "";
            T result = null;

            try
            {
                if (addQueryParams && headers != null)
                {
                    if (addQueryParams)
                    {
                        string Params = "";
                        foreach (var h in headers)
                        {
                            Params += string.Format("{0}={1}&", h.Key, h.Value); ;
                        }
                        api += string.Format("?{0}", Params);
                    }
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
                request.Method = method;
                request.KeepAlive = false;
                request.ContentType = "application/json; charset=UTF-8";

                if (headers != null && !addQueryParams)
                {
                    foreach (KeyValuePair<string, string> entry in headers)
                    {
                        request.Headers.Add(entry.Key, entry.Value);
                    }
                }

                if (method.ToUpper() == "POST")
                {
                    Byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                WebResponse response = request.GetResponse();
                HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                if (statusCode.Equals(HttpStatusCode.OK) || statusCode.Equals(HttpStatusCode.Created))
                {
                    StreamReader Reader = new StreamReader(response.GetResponseStream());
                    json = Reader.ReadToEnd();
                    Reader.Close();
                    if (typeof(System.String) == typeof(T))
                    {
                        result = (T)Convert.ChangeType(json, typeof(T));
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

    }

    /// <summary>
    /// 
    /// </summary>

    public class LogTxt
    {
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static void WritetLog(string content)
        {
            try
            {
                _lock.EnterWriteLock();
                string basePath = Environment.CurrentDirectory + "/MyLogs";
                string directoryPath = basePath + "/" + DateTime.Now.ToString("yyyyMM");
                string filePath = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var fs = new FileStream(Path.Combine(directoryPath ,(filePath  + ".txt")), FileMode.Append, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fs, Encoding.UTF8);
                streamWriter.Write(("******************************\r\n" + DateTime.Now + ("\r\n" + (content + "\r\n" + "\r\n"))));
                streamWriter.Close();
                _lock.ExitWriteLock();
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}