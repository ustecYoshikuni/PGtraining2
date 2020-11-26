using System.Configuration.Install;
using System.ServiceProcess;

namespace PGtraining.FileImportService
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                        new CsvImportService()
                };
                ServiceBase.Run(ServicesToRun);
            }

            switch (args[0])
            {
                case "install":
                    string[] args2 = { System.Reflection.Assembly.GetExecutingAssembly().Location };
                    ManagedInstallerClass.InstallHelper(args2);

                    break;

                case "uninstall":
                    string[] uninstallargs = { "/u", System.Reflection.Assembly.GetExecutingAssembly().Location };
                    ManagedInstallerClass.InstallHelper(uninstallargs);

                    break;

                default:
                    break;
            }
        }
    }
}