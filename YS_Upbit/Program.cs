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

                SetLog($"바라볼 폴더명 : {folderPath} 파일명 : {filePath} , Path : {folderPath}{filePath}");

                if (!string.IsNullOrEmpty(access_key) && !string.IsNullOrEmpty(secret_key))
                {
                    UpBitProcess.Instance.Init(access_key, secret_key);
                }

                _marketListDic = UpBitProcess.Instance.GetMarketList();

                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = folderPath;
                watcher.Filter = filePath;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                watcher.Created += YSFile_Changed;
                watcher.Changed += YSFile_Changed;
                watcher.EnableRaisingEvents = true;
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
                                UpBitBuyAndSelling(beforeLastLine, lastLine);
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

        /// <summary>
        /// YS 데이터 기반 매매 처리 함수
        /// </summary>
        /// <param name="beforeLastLine"></param>
        /// <param name="lastLine"></param>
        private static void UpBitBuyAndSelling(string beforeLastLine, string lastLine)
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

        private static void SetLog(string message)
        {
            Console.WriteLine(message);
            Trace.WriteLine(message);
        }
    }
}
