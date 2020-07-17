using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace NewarePassPort.Common
{
    public static class Http
    {


        public static string Get(string url, Dictionary<string, string> data = null, Encoding encoding = null, int timeout = 600, bool isRedirect = true, string proxyAddress = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (url == null || url.Length == 0)
                return string.Empty;
            if (data != null && data.Count > 0)
            {
                url += "?" + Convert.DictionaryToString(data, "&", "=");
            }
            string result = string.Empty;
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = isRedirect;
                request.Timeout = timeout * 1000;
                if (proxyAddress != null && proxyAddress.Length > 0)
                    request.Proxy = new WebProxy(proxyAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public static void GetAsync(string url, Dictionary<string, string> data = null, Encoding encoding = null, int timeout = 60, bool isRedirect = true, string proxyAddress = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (url == null || url.Length == 0)
                return;
            if (data != null && data.Count > 0)
                url += "?" + Convert.DictionaryToString(data, "&", "=");
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = isRedirect;
                request.Timeout = timeout * 1000;
                if (proxyAddress != null && proxyAddress.Length > 0)
                    request.Proxy = new WebProxy(proxyAddress);
                ThreadPool.QueueUserWorkItem(m => { request.GetResponse(); });
            }
            catch
            {
                return;
            }
        }
        public static string Post(string url, Dictionary<string, string> data, Encoding encoding = null, int timeout = 600, string account = "", string password = "")
        {
            if (url == null || url.Length == 0)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            string result = string.Empty;
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] b = encoding.GetBytes(Convert.DictionaryToString(data, "&", "="));
                request.ContentLength = b.Length;
                request.Timeout = 1000 * timeout;
                if (account != null && account.Length > 0)
                    request.Headers["Authorization"] = "Basic " + System.Convert.ToBase64String(Encoding.Default.GetBytes(account + ":" + password));
                using (Stream s = request.GetRequestStream())
                {
                    s.Write(b, 0, b.Length);
                    s.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
    }
}
