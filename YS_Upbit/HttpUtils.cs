using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YS_Upbit
{
    public class HttpUtils
    {

        private static string HttpRequest(string requestUrl, string postData = "", bool isPost = false, string contentType = "appplication/json", Encoding encoding = null)
        {
            HttpStatusCode responseStatusCode = HttpStatusCode.BadRequest;
            HttpWebResponse response = null;
            string resultString = string.Empty;
            try
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.44 Safari/534.7";
                request.Timeout = 5000;
                request.ContentType = $"{contentType}; charset{encoding.WebName}";
                request.Method = "GET";
                if (isPost)
                {
                    request.Method = "POST";
                    byte[] byteArray = encoding.GetBytes(postData);
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                response = (HttpWebResponse)request.GetResponse();
                responseStatusCode = response.StatusCode;
                using (Stream readData = response.GetResponseStream())
                {
                    if (readData != null)
                    {
                        using (StreamReader srReadData = new StreamReader(readData, encoding ?? Encoding.UTF8))
                        {
                            resultString = srReadData.ReadToEnd();
                        }
                    }
                }

                if (responseStatusCode != HttpStatusCode.OK)
                    resultString = responseStatusCode.ToString();
                return resultString;

            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string HttpGetRequest(string requestUri, bool isPost = false, string postData = "")
        {
            string resultString = "";
            resultString = HttpRequest(requestUri, postData, isPost);
            return resultString;
        }


        public static string HttpGetRequest(string requestUri, Dictionary<string, string> queryData = null)
        {
            string queryString = GetHttpNameValuePairString(queryData);
            queryString = string.IsNullOrWhiteSpace(queryString) ? "" : "?" + queryString;

            return HttpRequest($"{requestUri}{queryString}");
        }

        public static string HttpGetRequest(string requestUri, string siteId, string postData)
        {
            string queryString = GetHttpQuery(siteId, postData);
            return HttpRequest(requestUri + queryString);
        }

        public static string HttpPostRequest(string requestUri, Dictionary<string, string> postData = null)
        {
            string postString = GetHttpNameValuePairString(postData);

            return HttpRequest(requestUri, postString, true);
        }

        public static string HttpPostRequest(string requestUri, string body)
        {
            return HttpRequest(requestUri, body, true);
        }

        //public static string HttpRequestEucKr(string requestUri)
        //{
        //    return HttpRequest(requestUri, false, "", "", Encoding.GetEncoding("euc-kr"));
        //}

        private static string GetHttpNameValuePairString(Dictionary<string, string> queryData)
        {
            if (queryData == null)
            {
                return "";
            }

            List<string> queryDataList = new List<string>();
            foreach (KeyValuePair<string, string> keyValuePair in queryData)
            {
                string key = "";
                string value = "";

                // todo : 한시적으로 servicekey 일때만, URL 인코딩 사용 안함.
                switch (keyValuePair.Key)
                {
                    default:
                        key = HttpUtility.UrlEncode(keyValuePair.Key);
                        value = HttpUtility.UrlEncode(keyValuePair.Value);
                        break;
                }

                queryDataList.Add($"{key}={value}");

            }

            return string.Join("&", queryDataList);
        }

        private static string GetHttpQuery(string siteId, string queryJson)
        {
            string urlEncodingText = HttpUtility.UrlEncode(queryJson);
            return $"?siteId={siteId}&Query={urlEncodingText}";
        }
    }
}

