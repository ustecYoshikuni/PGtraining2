using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace PGtraining.FileImportService
{
    public partial class CsvImportService : ServiceBase
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Thread Worker;
        private AutoResetEvent StopRequest = new AutoResetEvent(false);

        private string TargetFolderPath = "";
        private string FileNamePattern = ".*";
        private int ProcessingInterval = 5;
        private int ReprocessingTimes = 0;
        private string ErrorFolderPath = $"{System.Environment.CurrentDirectory}\\error";
        private string SuccessFolderPath = $"{System.Environment.CurrentDirectory}\\success";

        private List<string> FileNemes = new List<string>();

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

        private void DoWork(object arg)
        {
            this.CheckSetting();

            for (; ; )
            {
                this.CheckFolder();

                foreach (var file in this.FileNemes)
                {
                    var import = this.ImportFile(file);

                    if (import)
                    {
                        var move = this.MoveFile(file, this.SuccessFolderPath);
                    }
                    else
                    {
                        var move = this.MoveFile(file, this.ErrorFolderPath);
                    }
                }

                if (StopRequest.WaitOne(ProcessingInterval * 1000)) return;
            }
        }

        private bool MoveFile(string sourceFilePath, string destFolderPath)
        {
            try
            {
                var fileName = Path.GetFileName(sourceFilePath);
                var destFilePath = $"{destFolderPath}\\{fileName}";

                _logger.Info($"ファイルを{destFilePath}に移動させます。");

                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }
                File.Move(sourceFilePath, destFilePath);

                _logger.Info($"ファイルを移動しました。");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"ファイルの移動に失敗しました：{ex.ToString()}");
                return false;
            }
        }

        private bool ImportFile(string filePath)
        {
            var result = true;

            for (var i = 0; i < this.ReprocessingTimes; i++)
            {
                var csvFile = new CsvFile();
                try
                {
                    result = csvFile.Import(filePath);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error($"ファイルの読込に失敗しました：{ex.ToString()}");
                    result = false;
                }
            }
            return result;
        }

        #region 設定値のチェック

        private void CheckSetting()
        {
            _logger.Info($"以下の設定になっています。" + Environment.NewLine +
            $"******************************************************************" + Environment.NewLine +
            $"監視対象フォルダのパス：{Properties.Settings.Default.TargetFolderPath}" + Environment.NewLine +
            $"ファイル名のパターン：{Properties.Settings.Default.FileNamePattern}" + Environment.NewLine +
            $"処理間隔期間：{Properties.Settings.Default.ProcessingInterval}" + Environment.NewLine +
            $"再処理回数：{Properties.Settings.Default.ReprocessingTimes}" + Environment.NewLine +
            $"エラーフォルダへのパス：{Properties.Settings.Default.ErrorFolderPath}" + Environment.NewLine +
            $"成功フォルダへのパス：{Properties.Settings.Default.SuccessFolderPath}" + Environment.NewLine +
            $"DB接続文字列：{Properties.Settings.Default.ConnectionString}" + Environment.NewLine +
            $"******************************************************************"
           );

            this.CheckTargetFolderPath();
            this.CheckFileNamePattern();
            this.CheckProcessingInterval();
            this.CheckReprocessingTimes();
            this.CheckErrorFolderPath();
            this.CheckSuccessFolderPath();

            _logger.Info($"以下の設定で処理します。" + Environment.NewLine +
            $"******************************************************************" + Environment.NewLine +
            $"監視対象フォルダのパス：{this.TargetFolderPath}" + Environment.NewLine +
            $"ファイル名のパターン：{this.FileNamePattern}" + Environment.NewLine +
            $"処理間隔期間：{this.ProcessingInterval}" + Environment.NewLine +
            $"再処理回数：{this.ReprocessingTimes}" + Environment.NewLine +
            $"エラーフォルダへのパス：{this.ErrorFolderPath}" + Environment.NewLine +
            $"成功フォルダへのパス：{this.SuccessFolderPath}" + Environment.NewLine +
            $"DB接続文字列：{Properties.Settings.Default.ConnectionString}" + Environment.NewLine +
            $"******************************************************************"
           );
        }

        #region 設定の各項目のチェック

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
                _logger.Info($"監視対象フォルダのパス【問題なし】");
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
                _logger.Info($"ファイル名のパターン【問題なし】");
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
                _logger.Info($"処理間隔期間【問題なし】");
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
                _logger.Info($"再処理回数【問題なし】");
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
                _logger.Info($"エラーフォルダパス【問題なし】");
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
                _logger.Info($"成功フォルダパス【問題なし】");
            }
        }

        #endregion 設定の各項目のチェック

        #endregion 設定値のチェック

        private void CheckFolder()
        {
            _logger.Info($"CheckFolder() 対象フォルダ内のファイルを取得します。");
            this.FileNemes.Clear();

            try
            {
                this.FileNemes = Directory.GetFiles(this.TargetFolderPath, this.FileNamePattern).ToList(); ;
                foreach (string name in this.FileNemes)
                {
                    Console.WriteLine(name);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ファイル名取得に失敗しました：{ex.ToString()}");
            }

            _logger.Info($"CheckFolder() ファイル名を取得しました。【{this.FileNemes.Count()}ファイル】" + Environment.NewLine +
                $"{string.Join(Environment.NewLine, this.FileNemes)}");
        }
    }
}