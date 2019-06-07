using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace YS_Upbit
{
    public class UpbitAPI
    {
        public MyHttpClient httpClient;
        
        private static bool IsSetInit = false;

        private static UpbitAPI _instance;
        public static UpbitAPI Instance
        {
            get => _instance ?? (_instance = new UpbitAPI());
        }

        public void Init(string accessKey, string secretKey)
        {
            this.httpClient = new MyHttpClient(accessKey, secretKey);
            IsSetInit = true;
        }

        public class MyHttpClient : HttpClient
        {
            private string _accessKey;
            public string AccessKey
            {
                get { return _accessKey; }
                set { _accessKey = value; }
            }
            private string _secretKey;
            public string SecretKey
            {
                get { return _secretKey; }
                set { _secretKey = value; }
            }
            public MyHttpClient(string accessKey, string secretKey)
            {
                if (string.IsNullOrWhiteSpace(accessKey)) { throw new ArgumentNullException("accessKey"); }
                if (string.IsNullOrWhiteSpace(secretKey)) { throw new ArgumentNullException("secretKey"); }
                _accessKey = accessKey;
                _secretKey = secretKey;
            }
        }

        public string GetAccount()
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/accounts";
            return CallAPI_NoParam(url, HttpMethod.Get);
        }
        public string GetTicker(string markets)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/ticker";
            return CallAPI_WithParam(url, new NameValueCollection { { "markets", markets } }, HttpMethod.Get);
        }
        public string GetMarkets()
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/market/all";
            return CallAPI_NoParam(url, HttpMethod.Get);
        }
        public string GetAllOrder()
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/orders";
            return CallAPI_NoParam(url, HttpMethod.Get);
        }
        public string GetOrder(string uuid)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/order";
            return CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid } }, HttpMethod.Get);
        }
        public string MakeOrder(string market, UpbitOrderSide side, decimal volume, decimal price, UpbitOrderType ord_type = UpbitOrderType.limit)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/orders";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "side", side.ToString() }, { "volume", volume.ToString() }, { "price", price.ToString() }, { "ord_type", ord_type.ToString() } }, HttpMethod.Post);
        }
        public string CancelOrder(string uuid)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/order";
            return CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid } }, HttpMethod.Delete);
        }
        public string GetOrderbook(string markets)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/orderbook";
            return CallAPI_WithParam(url, new NameValueCollection { { "markets", markets } }, HttpMethod.Get);
        }
        public string GetOrderChance(string market)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/orders/chance";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market } }, HttpMethod.Get);
        }

        public string GetCandles_Minute(string market, UpbitMinuteCandleType unit, DateTime to = default(DateTime), int count = 1)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/candles/minutes/" + (int)unit;
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        }
        public string GetCandles_Day(string market, DateTime to = default(DateTime), int count = 1)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/candles/days";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        }
        public string GetCandles_Week(string market, DateTime to = default(DateTime), int count = 1)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/candles/weeks";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        }
        public string GetCandles_Month(string market, DateTime to = default(DateTime), int count = 1)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/candles/months";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        }
        public string GetTicks(string market, int count = 1)
        {
            if (!IsSetInit)
                throw new InvalidOperationException("Init 함수를 먼저 선언하세요");
            string url = "https://api.upbit.com/v1/trades/ticks";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "count", count.ToString() } }, HttpMethod.Get);
        }

        private string CallAPI_NoParam(string url, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_NoParameter());
            var response = httpClient.SendAsync(requestMessage).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            return contents;
        }
        private string CallAPI_WithParam(string url, NameValueCollection nvc, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url + "?" + ToQueryString(nvc, true)));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_WithParameter(nvc));
            var response = httpClient.SendAsync(requestMessage).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            return contents;
        }

        private string JWT_NoParameter()
        {
            var payload = new JwtPayload
            {
                {"accsee_key", httpClient.AccessKey},
                {"nonce", Guid.NewGuid().ToString()},
                {"query_hash", ""},
                {"query_hash_alg", "SHA512"}
            };

            //////TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
            //////var nonce = Convert.ToInt64(diff.TotalMilliseconds);
            //////var payload = new JwtPayload { { "access_key", httpClient.AccessKey }, { "nonce", nonce } };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.SecretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }
        private string JWT_WithParameter(NameValueCollection nvc)
        {
            string queryString = ToQueryString(nvc, false);
            var payload = new JwtPayload
            {
                {"access_key", httpClient.AccessKey},
                {"nonce", Guid.NewGuid().ToString()},
                {"query", queryString},
                {"query_hash_alg", "SHA512"}
            };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.SecretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }
        private string ToQueryString(NameValueCollection nvc, bool isURI)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", (isURI) ? HttpUtility.UrlEncode(key) : key, (isURI) ? HttpUtility.UrlEncode(value) : value))
                .ToArray();
            return string.Join("&", array);
        }
        private string DateTime2String(DateTime to)
        {
            return to.ToString("s") + "+09:00";
        }
        public enum UpbitMinuteCandleType { _1 = 1, _3 = 3, _5 = 5, _10 = 10, _15 = 15, _30 = 30, _60 = 60, _240 = 240 }
        public enum UpbitOrderSide { ask, bid }
        public enum UpbitOrderType { limit }
    }
}
