using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace YS_Upbit
{
    public class UpBitProcess
    {
        private string _access_key;
        private JwtHeader _secret_key;
        private static UpBitProcess _instnce;

        private static bool _isLoaded = false;
        public static bool IsLoaded
        {
            get => _isLoaded;
        }
        public static UpBitProcess Instance
        {
            get => _instnce ?? (_instnce = new UpBitProcess());
        }
        public void Init(string accessKey , string secretKey)
        {
            _access_key = accessKey;
            byte[] keyBytes = Encoding.Default.GetBytes(secretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            _secret_key = new JwtHeader(credentials);
            _isLoaded = true;
        }

        public string PayLoad(Dictionary<string, string> param)
        {
            if (!IsLoaded)
                return "";
            StringBuilder builder = new StringBuilder();
            foreach (var v in param)
            {
                builder.Append(v.Key).Append("=").Append(v.Value).Append("&");
            }

            string queryString = builder.ToString().TrimEnd('&');
            SHA512 sHA512 = SHA512.Create();
            byte[] queryHashByteArray = sHA512.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            string queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();

            var payLoad = new JwtPayload
            {
                {"access_key", _access_key},
                {"nonce", Guid.NewGuid().ToString()},
                {"query_hash", queryHash},
                {"query_hash_alg", "SHA512"}
            };

            var secToken = new JwtSecurityToken(_secret_key, payLoad);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            return "Bearer " + jwtToken;
        }

        /// <summary>
        /// Get Upbit 상품 리스트 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetMarketList()
        {
            string uri = "https://api.upbit.com/v1/market/all";
            var response = HttpUtils.HttpGetRequest(uri, null);
            Dictionary<string, string> marketDic = new Dictionary<string, string>();
            JArray marketJArray = JArray.Parse(response);
            foreach (var marketValue in marketJArray)
            {
                
                var market = marketValue["market"].Value<String>();
                var name = marketValue["korean_name"].Value<String>();
                marketDic.Add(market, name);
            }

            return marketDic;
        }

        /// <summary>
        /// YS 데이터 기반 매매 처리 함수
        /// </summary>
        /// <param name="beforeLastLine"></param>
        /// <param name="lastLine"></param>
        public static void UpBitBuyAndSelling(string beforeLastLine, string lastLine)
        {
            /// beforeLastLine 매매 신호 를 파싱해서 처리하고

            Dictionary<string, string> bodyParam = new Dictionary<string, string>()
            {
                {"market", ""},
                { "side" , "" },
                { "volume" , "" },
                { "price", "" },
                { "ord_type" , "" }
            };

            var query = UpBitProcess.Instance.PayLoad(bodyParam);

            /// lastLine 매매 신호를 파싱해서 처리한다.

            bodyParam = new Dictionary<string, string>()
            {
                {"market", ""},
                { "side" , "" },
                { "volume" , "" },
                { "price", "" },
                { "ord_type" , "" }
            };

            query = UpBitProcess.Instance.PayLoad(bodyParam);

        }
    }
}
