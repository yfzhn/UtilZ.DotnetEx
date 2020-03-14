using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// RestFull客户端辅助类
    /// </summary>
    public class RestFullClientHelper
    {
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
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }

                return responseValue;
            }
        }

        /// <summary>
        /// 调用RestFull服务的Get方法
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>返回字符串</returns>
        public static string Get(string url)
        {
            var request = CreateRequest(url, WebRequestMethods.Http.Get);
            return Request(request);
        }

        /// <summary>
        /// 调用RestFull服务的Get方法
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="obj">参数对象</param>
        /// <returns>返回字符串</returns>
        public static string Get(string url, object obj)
        {
            var request = CreateRequest(url, WebRequestMethods.Http.Get);
            string json = SerializeEx.RuntimeJsonSerializerObject(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            return Request(request);
        }

        /// <summary>
        /// 调用RestFull服务的Post方法
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="obj">参数对象</param>
        /// <returns>返回字符串</returns>
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

        /// <summary>
        /// 调用RestFull服务的Delete方法
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>返回字符串</returns>
        public static string Delete(string url)
        {
            var request = CreateRequest(url, "DELETE");
            return Request(request);
        }

        /// <summary>
        /// 调用RestFull服务的Put方法
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="obj">参数对象</param>
        /// <returns>返回字符串</returns>
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
