using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InternetBanking.Utils
{
    public class CallAPIHelper
    {
        public static T CallAPI<T>(string url, string method, object request, Dictionary<string, string> headers = null) where T : class
        {
            string json = "";
            T response = null;

            try
            {
                if (method.ToUpper() == "GET" && request != null)
                {
                    var jsonrequest = JsonConvert.SerializeObject(request);
                    Dictionary<string, string> ht = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonrequest);
                    string Params = "";
                    for (int i = 0; i < ht.Count; i++)
                    {
                        var param = string.Format("{0}={1}&", ht.ElementAt(i).Key, HttpUtility.UrlEncode(ht.ElementAt(i).Value));
                        Params += param;
                    }

                    url += string.Format("?{0}", Params);
                }

                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                Request.Method = method.ToString();
                Request.KeepAlive = false;
                Request.ContentType = "application/json; charset=UTF-8";

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> entry in headers)
                    {
                        Request.Headers.Add(entry.Key, entry.Value);
                    }
                }

                //khoi tao tham so
                if (method.ToUpper() != "GET")
                {
                    Byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                    Request.ContentLength = byteArray.Length;
                    Stream dataStream = Request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                /*Kiểm tra kết quả trả về */
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.OK))
                {
                    StreamReader Reader = new StreamReader(Response.GetResponseStream());
                    json = Reader.ReadToEnd();
                    Reader.Close();
                    if (typeof(System.String) == typeof(T))
                    {
                        response = (T)Convert.ChangeType(json, typeof(T));
                    }
                    else
                    {
                        response = JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                response = null;
            }
            return response;
        }
    }
}
