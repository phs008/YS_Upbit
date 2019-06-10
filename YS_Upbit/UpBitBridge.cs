using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UpbitApi;
using YS_TelegramBot;
using YS_Upbit;

namespace UpBit_Bridge
{
    public class UpBitBridge
    {
        private static UpBitBridge _instance;

        public static UpBitBridge Instance
        {
            get => _instance ?? (_instance = new UpBitBridge());
        }

        public static Dictionary<string, string> Markets = new Dictionary<string, string>();

        public void Init(string access_key, string secret_key)
        {
            UpbitAPI.Instance.Init(access_key, secret_key);
            var allMarkets = UpbitAPI.Instance.GetMarkets();
            var markets = JArray.Parse(allMarkets);
            foreach (var market in markets)
            {
                var marketName = market["korean_name"].Value<string>();
                var marketCode = market["market"].Value<string>();
                Program.SetLog($"marketName : {marketName} , marketCode : {marketCode}");
                if (!Markets.ContainsKey(marketName))
                    Markets.Add(marketName, marketCode);
            }
        }

        public void Order(string orderMessage)
        {
            var orderMessages = orderMessage.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var orderType = orderMessages[2];
            var orderSymbol = orderMessages[4];
            var orderPosition = orderMessages[6];
            var orderPrice = orderMessages[8];
            TelegramBotApi.Instance.SendMessage($"[{orderType}] : Price {orderPrice} ");
            Program.SetLog($"OrderType : {orderType} , OrderSymbol : {orderSymbol} , OrderPosition : {orderPosition} , OrderPrice : {orderPrice}");
        }
        
    }
}
