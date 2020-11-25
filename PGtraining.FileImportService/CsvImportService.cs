using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PGtraining.FileImportService
{
    public partial class CsvImportService : ServiceBase
    {
        public CsvImportService()
        {
            InitializeComponent();
        }

        public void onDebug()
        {
            var csvFile = new CsvFile();
            csvFile.Import(@"C:\Users\Yoshikuni\source\PGtraining\sample\20200601140000.csv");
        }


        protected override void OnStart(string[] args)
        {
            var csvFile = new CsvFile();
            csvFile.Import(@"C:\Users\Yoshikuni\source\PGtraining\sample\20200601140000.csv");
        }

        protected override void OnStop()
        {
        }
    }
}
