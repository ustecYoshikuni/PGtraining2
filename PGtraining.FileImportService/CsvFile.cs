using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PGtraining.FileImportService
{
    public class CsvFile : IDisposable
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Dispose関連処理

        private IntPtr _handle;
        private Stream _stream;
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }

                MyCloseHandle(_handle);
                _handle = IntPtr.Zero;

                _disposed = true;
            }
        }

        protected static void MyCloseHandle(IntPtr handle)
        {
        }

        #endregion Dispose関連処理

        public CsvFile()
        {
            Dispose(false);
        }

        public void Import(string path)
        {
            FileInfo info = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository(), info);

            _logger.Info($"Import Start {path}【読込開始】");

            StreamReader sr = new StreamReader(@path, System.Text.Encoding.GetEncoding("shift_jis"));
            {
                var row = 0;
                while (!sr.EndOfStream)
                {
                    row++;

                    string line = sr.ReadLine();

                    //ヘッダーを飛ばす
                    if (row == 1)
                    {
                        continue;
                    }

                    string[] values = line.Split(',');

                    //項目数を確認
                    if (values.Length < 12)
                    {
                        _logger.Error($"検査の項目数は12項目以上必要です。項目数が{ values.Length }です。");
                        continue;
                    }
                    if (values.Length % 2 != 0)
                    {
                        _logger.Error($"検査の項目数は偶数必要です。項目数が{ values.Length }です。");
                        continue;
                    }

                    _logger.Info($"{row}行目読込：{ line }");

                    //ダブルクォーテーションがあるか確認
                    var doubleQuotesError = false;
                    for (var i = 0; i < values.Length; i++)
                    {
                        var result = this.CheckDoubleQuotes(values[i]);

                        if (result)
                        {
                            values[i] = values[i].Substring(1, values[i].Length - 2);
                        }
                        else
                        {
                            _logger.Error($"ダブルクォーテーションがありません。{ values[i] }です。");
                            doubleQuotesError = true;
                        }
                    }

                    if (doubleQuotesError)
                    {
                        continue;
                    }

                    //検査の読込
                    _logger.Info($"{ string.Join(",", values) }");

                    using (var order = new Order(values))
                    {
                        if (order.OrderValidation())
                        {
                            _logger.Info($"{row}行目読込完了");
                        }
                        else
                        {
                            _logger.Error($"{row}行目読込エラー");
                            continue;
                        }

                        //登録済みなら、上書き、未登録なら追加
                        if (order.IsRegistered())
                        {
                            var update = order.UpdateOrder();
                        }
                        else
                        {
                            var insert = order.InsertOrder();
                        }
                    }
                }
            }
            sr.Close();

            _logger.Info($"Import End {path}【読込終了】");
        }

        private bool CheckDoubleQuotes(string value)
        {
            return Regex.IsMatch(value, "^\".*\"");
        }
    }
}