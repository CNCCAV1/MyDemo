using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Helper
{
    public class HttpHelper
    {
        public static T GetMethodToObject<T>(string url) where T : new()
        {
            T t = new T();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json;charset=UTF-8";
                HttpWebResponse response = null;

                response = (HttpWebResponse)request.GetResponse();

                string json = GetResponseString(response);
                //LogHelper.WriteLog("接口：" + url + "：：" + json);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog("请求接口" + url + ",错误：" + e.Message);
                var result = t.GetType().GetProperty("result");
                var error = t.GetType().GetProperty("error");
                if (result != null && error != null)
                {
                    result.SetValue(t, "FAIL", null);
                    error.SetValue(t, e.Message, null);
                }
                return t;
            }
        }
        // 将 HttpWebResponse 返回结果转换成 string
        private static string GetResponseString(HttpWebResponse response)
        {
            string json = null;
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8")))
            {
                json = reader.ReadToEnd();
            }
            return json;
        }

    }
}
