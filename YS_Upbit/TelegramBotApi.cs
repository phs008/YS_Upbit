using System.Collections.Generic;
using YS_Upbit;

namespace YS_TelegramBot
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
        private string _telegramChatId;

        public void Init(string token,string chatId)
        {
            _baseUri = $"https://api.telegram.org/bot{token}/";
            _telegramToken = token;
            _telegramChatId = chatId;

            Program.SetLog("토근이 활성화 되었습니다");
        }

        public void SendMessage(string message)
        {
            var api = _baseUri + "sendMessage";
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                {"chat_id", _telegramChatId},
                {"text", message}
            };
            var result = HttpUtils.HttpGetRequest(api, param);
            
        }
    }
}
