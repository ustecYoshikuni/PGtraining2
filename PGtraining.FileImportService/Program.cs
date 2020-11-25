using System.ServiceProcess;

namespace PGtraining.FileImportService
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        private static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CsvImportService()
            };

            CsvImportService myService = new CsvImportService();
            myService.onDebug();

            //switch (args[0])
            //{
            //    case "install":
            //        string[] args2 = { System.Reflection.Assembly.GetExecutingAssembly().Location };
            //        ManagedInstallerClass.InstallHelper(args2);
            //        break;

            //    case "uninstall":
            //        string[] uninstallargs = {"/u", System.Reflection.Assembly.GetExecutingAssembly().Location };
            //        ManagedInstallerClass.InstallHelper(uninstallargs);
            //        break;

            //    case "start":
            //        ServiceBase.Run(ServicesToRun);
            //        break;

            //    default:
            //        ServiceBase.Run(ServicesToRun);
            //        break;
            //}
        }
    }
}