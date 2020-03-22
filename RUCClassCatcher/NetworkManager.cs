using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using RUCClassCatcher.Model;

namespace RUCClassCatcher
{
    class NetworkManager
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";


        private static readonly NetworkManager single = new NetworkManager();
        private NetworkManager()
        {

        }

        public static NetworkManager getSinglton()
        {
            return single;
        }

        /// <summary>
        /// 发送一个HTTP的Get请求，并返回全部response流
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="timeout">超时时间，可以为Null</param>
        /// <param name="cookies">CookieCollection</param>
        /// <returns>字符串</returns>
        public string sendGETRequestWithCallback(string url, int? timeout, CookieCollection cookies)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("没有url");
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                Stream responseStream = request.GetResponse().GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream, Encoding.UTF8);
                return responseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                RootRequestModel errorMsg = new RootRequestModel();
                errorMsg.code = 400;
                errorMsg.message = ex.Message;
                return Newtonsoft.Json.JsonConvert.SerializeObject(errorMsg);
            }
        }
    }

}
