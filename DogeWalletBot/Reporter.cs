using DogeWalletBot.Model;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace DogeWalletBot
{
    public class Reporter
    {
        public Report Report { get; set; }

        public MemoryStream GetReceivedTransactionsPdf(string address, List<ReceivedTransaction> transactions, string afterTXID = null)
        {
            string appData = HostingEnvironment.MapPath("~/App_Data/");
            Config.WebMode = true;
            Report = new Report();
            Report.Load(appData + "TransactionsReport.frx");
            Report.RegisterData(transactions, "txs");
            Report.GetDataSource("txs").Enabled = true;
            (Report.FindObject("Data1") as DataBand).DataSource = Report.GetDataSource("txs");
            Report.Prepare();
            PDFExport pdf = new PDFExport();
            MemoryStream exportStream = new MemoryStream();
            Report.Export(pdf, exportStream);
            exportStream.Position = 0;
            return exportStream;
        }
    }
}