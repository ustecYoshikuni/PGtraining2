using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace PGtraining.FileImportService
{
    public partial class CsvImportService : ServiceBase
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Thread Worker;
        private AutoResetEvent StopRequest = new AutoResetEvent(false);

        public CsvImportService()
        {
            FileInfo info = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository(), info);

            InitializeComponent();
        }

        public void onDebug()
        {
            Worker = new Thread(DoWork);
            Worker.Start();
        }

        protected override void OnStart(string[] args)
        {
            Worker = new Thread(DoWork);
            Worker.Start();
        }

        protected override void OnStop()
        {
            StopRequest.Set();
            Worker.Join();
        }

        private string TargetFolderPath = "";
        private string FileNamePattern = ".*";
        private int ProcessingInterval = 5;
        private int ReprocessingTimes = 0;
        private string ErrorFolderPath = $"{System.Environment.CurrentDirectory}\\error";
        private string SuccessFolderPath = $"{System.Environment.CurrentDirectory}\\success";

        private void DoWork(object arg)
        {
            this.CheckSetting();

            for (; ; )
            {
                this.CheckFolder();

                if (StopRequest.WaitOne(ProcessingInterval * 1000)) return;
            }
        }

        private void CheckSetting()
        {
            _logger.Info($"監視対象フォルダのパス：{Properties.Settings.Default.TargetFolderPath}" + Environment.NewLine +
            $"ファイル名のパターン：{Properties.Settings.Default.FileNamePattern}" + Environment.NewLine +
            $"処理間隔期間：{Properties.Settings.Default.ProcessingInterval}" + Environment.NewLine +
            $"再処理回数：{Properties.Settings.Default.ReprocessingTimes}" + Environment.NewLine +
            $"エラーフォルダへのパス：{Properties.Settings.Default.ErrorFolderPath}" + Environment.NewLine +
            $"成功フォルダへのパス：{Properties.Settings.Default.SuccessFolderPath}" + Environment.NewLine +
            $"DB接続文字列：{Properties.Settings.Default.ConnectionString}" + Environment.NewLine
           );

            this.CheckTargetFolderPath();
            this.CheckFileNamePattern();
            this.CheckProcessingInterval();
            this.CheckReprocessingTimes();
            this.CheckErrorFolderPath();
            this.CheckSuccessFolderPath();

            _logger.Info($"以下の設定で処理します。" + Environment.NewLine +
            $"監視対象フォルダのパス：{this.TargetFolderPath}" + Environment.NewLine +
            $"ファイル名のパターン：{this.FileNamePattern}" + Environment.NewLine +
            $"処理間隔期間：{this.ProcessingInterval}" + Environment.NewLine +
            $"再処理回数：{this.ReprocessingTimes}" + Environment.NewLine +
            $"エラーフォルダへのパス：{this.ErrorFolderPath}" + Environment.NewLine +
            $"成功フォルダへのパス：{this.SuccessFolderPath}" + Environment.NewLine +
            $"DB接続文字列：{Properties.Settings.Default.ConnectionString}" + Environment.NewLine
           );
        }

        #region 設定値のチェック

        private void CheckTargetFolderPath()
        {
            var result = true;

            this.TargetFolderPath = Properties.Settings.Default.TargetFolderPath;
            var trim = this.TargetFolderPath.Trim();
            if (trim != this.TargetFolderPath)
            {
                result = false;
                this.TargetFolderPath = trim;
                _logger.Info($"監視対象フォルダのパスに不要なスペースがあるので Trim() します。：{this.TargetFolderPath}");
            }

            try
            {
                if (Directory.Exists(this.TargetFolderPath) == false)
                {
                    result = false;
                    _logger.Error($"監視対象フォルダが存在しません。");
                }
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error($"※※※監視対象フォルダエラーです※※※:{ex.ToString()}");
            }

            if (result)
            {
                _logger.Info($"監視対象フォルダのパス問題なし");
            }
        }

        private void CheckFileNamePattern()
        {
            var result = true;

            this.FileNamePattern = Properties.Settings.Default.FileNamePattern;
            var trim = this.FileNamePattern.Trim();
            if (trim != this.FileNamePattern)
            {
                result = false;
                this.FileNamePattern = trim;
                _logger.Info($"ファイル名のパターンに不要なスペースがあるので Trim() します。：{this.FileNamePattern}");
            }

            if (result)
            {
                _logger.Info($"ファイル名のパターン問題なし");
            }
        }

        private void CheckProcessingInterval()
        {
            var result = true;

            if (int.TryParse(Properties.Settings.Default.ProcessingInterval, out var i))
            {
                ProcessingInterval = i;
            }
            else
            {
                result = false;
                _logger.Info($"処理間隔期間が数値となっていないため5秒とします。");
            }
            if (ProcessingInterval < 0)
            {
                result = false;
                ProcessingInterval = 5;
                _logger.Info($"処理間隔期間がマイナスとなってるため5秒とします。");
            }

            if (result)
            {
                _logger.Info($"処理間隔期間問題なし");
            }
        }

        private void CheckReprocessingTimes()
        {
            var result = true;

            if (int.TryParse(Properties.Settings.Default.ReprocessingTimes, out var r))
            {
                ReprocessingTimes = r;
            }
            else
            {
                result = false;
                _logger.Info($"再処理回数が数値となっていないため0回（再処理なし）とします。");
            }
            if (ReprocessingTimes < 0)
            {
                result = false;
                ReprocessingTimes = 0;
                _logger.Info($"再処理回数がマイナスとなっているため0回（再処理なし）とします。");
            }

            if (result)
            {
                _logger.Info($"再処理回数問題なし");
            }
        }

        private void CheckErrorFolderPath()
        {
            var result = true;

            if (Directory.Exists(Properties.Settings.Default.ErrorFolderPath))
            {
                this.ErrorFolderPath = Properties.Settings.Default.ErrorFolderPath;
            }
            else
            {
                result = false;
                _logger.Error($"エラーフォルダが存在しません。フォルダ:[{this.ErrorFolderPath}]を作成します。");

                try
                {
                    Directory.CreateDirectory(this.ErrorFolderPath);
                    _logger.Info($"エラーフォルダを作成しました。");
                }
                catch (Exception ex)
                {
                    _logger.Info($"エラーフォルダの作成に失敗しました。:{ex.ToString()}");
                }
            }

            if (result)
            {
                _logger.Info($"エラーフォルダパス問題なし");
            }
        }

        private void CheckSuccessFolderPath()
        {
            var result = true;

            if (Directory.Exists(Properties.Settings.Default.SuccessFolderPath))
            {
                this.SuccessFolderPath = Properties.Settings.Default.SuccessFolderPath;
            }
            else
            {
                result = false;
                _logger.Error($"成功フォルダが存在しません。フォルダ:[{this.SuccessFolderPath}]を作成します。");

                try
                {
                    Directory.CreateDirectory(this.SuccessFolderPath);
                    _logger.Info($"成功フォルダを作成しました。");
                }
                catch (Exception ex)
                {
                    _logger.Info($"成功フォルダの作成に失敗しました。:{ex.ToString()}");
                }
            }

            if (result)
            {
                _logger.Info($"成功フォルダパス問題なし");
            }
        }

        #endregion 設定値のチェック

        private void CheckFolder()
        {
            _logger.Info($"CheckFolder() Start");

            var csvFile = new CsvFile();
            //csvFile.Import(@"C:\Users\Yoshikuni\source\PGtraining\sample\20200601140000.csv");
            _logger.Info($"CheckFolder() End");
        }
    }
}