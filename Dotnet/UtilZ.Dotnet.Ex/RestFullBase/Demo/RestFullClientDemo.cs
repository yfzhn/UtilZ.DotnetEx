using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.RestFullBase.Demo
{
    internal class RestFullClientDemo
    {
        public static void Test()
        {
            try
            {

                var bassAddress = new Uri("http://127.0.0.1:7789/");
                var str = bassAddress.ToString();
                //var str = HttpRequest(@"http://127.0.0.1:7789/PersonInfoQuery/", "xcvzxcv");
                //var user = SerializeEx.RuntimeJsonDeserializeObject<User>(str);



                User addUser = new User() { Id = "5", Name = "起赵鼎", Age = 19, Birthday = DateTime.Now.AddDays(-1547) };
                var jsonAdd = RestFullClientDemo.Post(@"http://127.0.0.1:7789/PersonInfoQuery/user", addUser);

                var jsonGet = RestFullClientDemo.Get(@"http://127.0.0.1:7789/PersonInfoQuery/5");
                var userGet = SerializeEx.RuntimeJsonDeserializeObject<User>(jsonGet);

                userGet.Name = "mm";
                userGet.Age = 25;
                var jsonPut = RestFullClientDemo.Put(@"http://127.0.0.1:7789/PersonInfoQuery/user", userGet);

                var jsonGet2 = RestFullClientDemo.Get(@"http://127.0.0.1:7789/PersonInfoQuery/5");
                var userGet2 = SerializeEx.RuntimeJsonDeserializeObject<User>(jsonGet2);

                var jsonDel = RestFullClientDemo.Delete(@"http://127.0.0.1:7789/PersonInfoQuery/5");

                var jsonGet3 = RestFullClientDemo.Get(@"http://127.0.0.1:7789/PersonInfoQuery/5");
                //var userGet3 = SerializeEx.RuntimeJsonDeserializeObject<User>(jsonGet3);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// http请求(带参数)
        /// </summary>
        /// <param name="urlStr"></param>
        /// <param name="parameters">parameters例如：?name=LiLei</param>
        /// <returns></returns>
        public static string HttpRequest(string urlStr, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(urlStr + parameters);

            request.Method = WebRequestMethods.Http.Get;
            request.ContentLength = 0;
            request.ContentType = "application/json";

            //if (!string.IsNullOrEmpty(PostData) && Method == EnumHttpVerb.POST)
            //{
            //    var bytes = Encoding.UTF8.GetBytes(PostData);
            //    request.ContentLength = bytes.Length;

            //    using (var writeStream = request.GetRequestStream())
            //    {
            //        writeStream.Write(bytes, 0, bytes.Length);
            //    }
            //}

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = string.Format("请求数据失败. 返回的 HTTP 状态码：{0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

        private static HttpWebRequest CreateRequest(string url, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentLength = 0;
            request.ContentType = "application/json";
            return request;
        }

        private static string Request(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = string.Format("请求数据失败. 返回的 HTTP 状态码：{0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

        public static string Get(string url)
        {
            var request = CreateRequest(url, WebRequestMethods.Http.Get);
            return Request(request);
        }

        public static string Post(string url, object obj)
        {
            var request = CreateRequest(url, WebRequestMethods.Http.Post);
            string json = SerializeEx.RuntimeJsonSerializerObject(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            return Request(request);
        }

        public static string Delete(string url)
        {
            var request = CreateRequest(url, "DELETE");
            return Request(request);
        }

        public static string Put(string url, object obj)
        {
            var request = CreateRequest(url, WebRequestMethods.Http.Put);
            string json = SerializeEx.RuntimeJsonSerializerObject(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            return Request(request);
        }
    }
}
