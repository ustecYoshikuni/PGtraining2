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
        }
    }
}