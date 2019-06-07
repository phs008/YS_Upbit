using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS_Upbit
{
    class Program
    {
        private static Dictionary<string, string> _marketListDic;
        static void Main(string[] args)
        {
            StreamWriter writer = new StreamWriter(File.Open(DateTime.Now.ToString("yyyyMMdd_HHMMss") + ".log", FileMode.CreateNew, FileAccess.ReadWrite));
            TextWriterTraceListener listener = new TextWriterTraceListener(writer);
            Trace.Listeners.Add(listener);
            try
            {
                SetLog("YS to restAPI !");


                if (ConfigurationManager.AppSettings.Count == 0 || ConfigurationManager.AppSettings.Count == 1)
                {
                    throw new Exception("폴더위치와 파일명을 argument 에 넣어주세요");
                }

                string folderPath = ConfigurationManager.AppSettings["folderpath"];
                string filePath = ConfigurationManager.AppSettings["filepath"];
                string access_key = ConfigurationManager.AppSettings["access_key"];
                string secret_key = ConfigurationManager.AppSettings["secret_key"];
                string telegram_token = ConfigurationManager.AppSettings["telegram_token"];

                SetLog($"바라볼 폴더명 : {folderPath} 파일명 : {filePath} , Path : {folderPath}{filePath}");
                InitFileWatcherEvent(folderPath, filePath);


                if (!string.IsNullOrEmpty(access_key) && !string.IsNullOrEmpty(secret_key))
                {
                    UpbitAPI.Instance.Init(access_key, secret_key);
                    var markets = UpbitAPI.Instance.GetMarkets();
                    var account = UpbitAPI.Instance.GetAccount();
                }

                if (!string.IsNullOrEmpty(telegram_token))
                {
                    TelegramBotApi.Instance.Init(telegram_token);
                    TelegramBotApi.Instance.SendMessage("test");
                }

                

                while (true)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                SetLog(e.Message);
                SetLog("아무키나 누르면 종료됩니다");
                Console.ReadLine();
            }
            finally
            {
                Trace.Flush();
                writer.Close();
            }
        }

        private static void InitFileWatcherEvent(string folderPath , string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = folderPath;
            watcher.Filter = filePath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Created += YSFile_Changed;
            watcher.Changed += YSFile_Changed;
            watcher.EnableRaisingEvents = true;
        }
        private static void YSFile_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var encoding = Encoding.UTF8;
                int charsize = encoding.GetByteCount("\n");
                using (var fs = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        string beforeLastLine = "";
                        string lastLine = "";
                        while (!sr.EndOfStream)
                        {
                            lastLine = sr.ReadLine();
                            if (sr.Peek() == -1)
                            {
                                SetLog($"마지막 -1 : {beforeLastLine} , 마지막 : {lastLine}");

                            }
                            else
                            {
                                beforeLastLine = lastLine;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public static void SetLog(string message)
        {
            Console.WriteLine(message);
            Trace.WriteLine(message);
        }
    }
}
