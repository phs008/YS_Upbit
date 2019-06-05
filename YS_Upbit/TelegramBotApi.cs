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

        public static TelegramBotApi Instance
        {
            get => _telegramBotApi ?? (_telegramBotApi = new TelegramBotApi());
        }

        private string _telegramToken;
        private string _telegramId;

        TelegramBotApi()
        {
            Task.Factory.StartNew(() =>
            {
                bool isGetTelegramId = false;
                while (!isGetTelegramId)
                {
                    Thread.Sleep(1500);
                    if (string.IsNullOrEmpty(_telegramToken))
                        continue;

                    string uri = $"https://api.telegram.org/bot{_telegramToken}/getUpdates";
                    var response = HttpUtils.HttpGetRequest(uri, null);
                    if (string.IsNullOrEmpty(response))
                        continue;
                    var resultObject = JObject.Parse(response);
                    string responseResult = resultObject["ok"].Value<string>();
                    if (responseResult == "True")
                    {
                        var result = resultObject["result"].Value<JArray>();
                        if (result.Count == 0)
                        {
                            Program.SetLog("기입한 토근에 대한 텔레그램 봇을 활성해 해주세요");
                            continue;
                        }

                        Program.SetLog("토근이 활성화 되었습니다");
                    }
                }
            });

        }

        public void Init(string token)
        {
            _telegramToken = token;
        }
    }
}
