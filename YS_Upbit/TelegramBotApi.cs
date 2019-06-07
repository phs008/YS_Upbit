using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YS_Upbit
{
    public class TelegramBotApi
    {
        private static TelegramBotApi _telegramBotApi;
        private static string _baseUri = string.Empty;

        public static TelegramBotApi Instance
        {
            get => _telegramBotApi ?? (_telegramBotApi = new TelegramBotApi());
        }

        private string _telegramToken;
        private string _telegramId;

        public void Init(string token)
        {
            _telegramToken = token;
            _baseUri = $"https://api.telegram.org/bot{_telegramToken}/";


            bool isChatIdUpdated = false;
            var api = _baseUri + "getUpdates";
            var response = HttpUtils.HttpGetRequest(api, null);
            var resultObject = JObject.Parse(response);

            var result = resultObject["result"].Value<JArray>();
            if (result.Count == 0)
            {
                throw new InvalidOperationException("기입한 토근에 대한 텔레그램 봇에 메세지를 한번 기입해 주세요");
            }

            var chatId = result["message"]["from"]["id"].Value<int>();
            Program.SetLog("토근이 활성화 되었습니다");
        }

        public void SendMessage(string message)
        {
            var api = _baseUri + "sendMessage";
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                {"chat_id", _telegramId},
                {"text", message}
            };
            var result = HttpUtils.HttpGetRequest(api, param);
            
        }
    }
}
