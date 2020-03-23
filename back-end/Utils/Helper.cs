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

    public class LoggingTxt
    {
        private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

        /// <summary>
        /// InsertLog - pathType: 1/2 = lưu theo ngày/giờ
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathType">1/2 = lưu theo ngày/giờ</param>
        /// <param name="data"></param>
        public static void InsertLog(string _data ,string _path = "", int _pathType = 1)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                string sPhysicPath = Environment.CurrentDirectory + "/ErrorLogs";
                string strDirectory = "";
                string strFile = "";

                if (_pathType == 1)
                    strDirectory = sPhysicPath + "/" + _path + "/" + DateTime.Now.ToString("yyyyMM");
                else if (_pathType == 2)
                    strDirectory = sPhysicPath + "/" + _path + "/" + DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");

                if (_pathType == 1)
                    strFile = DateTime.Now.ToString("yyyy-MM-dd");
                else if (_pathType == 2)
                    strFile = DateTime.Now.ToString("HH-00");

                if (!Directory.Exists(strDirectory))
                    Directory.CreateDirectory(strDirectory);

                FileStream fs = new FileStream((strDirectory + ("\\" + (strFile + ".txt"))), FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(("---------------------------------------------------------------\r\n" + System.DateTime.Now + ("\r\n" + (_data + "\r\n" + "\r\n"))));
                sw.Close();
                sw = null;
                fs = null;
                GC.Collect();
                readWriteLock.ExitWriteLock();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}