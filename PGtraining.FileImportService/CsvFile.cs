using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PGtraining.FileImportService
{
    public class CsvFile
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Import(string path)
        {
            FileInfo info = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository(), info);

            _logger.Info("Import Start");

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

                    _logger.Info($"{ line }");

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

                    var study = new Study(values);

                    if (study.StudyValidation())
                    {
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            _logger.Info("Import End");
        }

        private bool CheckDoubleQuotes(string value)
        {
            return Regex.IsMatch(value, "^\".*\"");
        }
    }
}