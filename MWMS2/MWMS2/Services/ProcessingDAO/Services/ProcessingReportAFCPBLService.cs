using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.Services;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingReportAFCPBLService: ProcessingReportBLService
    {
        //ProcessingSRDAOService
        private ProcessingReportAFCPDAOService DAOService;
        protected ProcessingReportAFCPDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingReportAFCPDAOService()); }
        }


        public void loadDefault(Fn10RPT_AFCModel model){
            model.DateFrom = new DateTime(DateTime.Now.Year, 1, 1);
            model.DateTo = DateTime.Now;

        }

       

        public FileStreamResult ExportToExcel(Fn10RPT_AFCModel searchModel)
        {
            string fileName = "AFCProgressReport";

            string handlingOfficer = CommonUtil.getDisplay(searchModel.HandlingOfficer);
            string sortBy = CommonUtil.getDisplay(searchModel.SortBy);
            DateTime fromDate = searchModel.DateFrom;
            DateTime toDate = searchModel.DateTo;

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '|';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };

            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("AFC");
            int rowStart = 1;
            int colStart = 1;

            sheet.Cells[1,1].LoadFromText("AFC Progress Report (known as \"Submission Progress Report\" before 2014)");

            sheet.Cells[3,1].LoadFromText("From Date: "+fromDate + " To Date: "+ toDate +
                "Handling Officer: "+ handlingOfficer + "Sort By: " + sortBy);

            String[] header = new String[]{"Received Date" ,"Submission No.","Form No.",
                "Status", "Handling Officer", "PO Handling Officer", "SPO Handling Officer","Acknowledgement Date" };

            for (int i = 0; i < header.Length; i++)
            {   sheet.Cells[5, i+1].LoadFromText(header[i]);
            }

            


            //Dictionary<String, Dictionary<String, int>> receivedData = DA.getReceivedData(fromDate, toDate);
            //Dictionary<String, Dictionary<String, int>> processingData = DA.getProcessingCountData(fromDate, toDate);
            //Dictionary<String, Dictionary<String, int>> ackData = DA.getAckData(fromDate, toDate);
            //Dictionary<String, Dictionary<String, int>> countData = DA.getCountData(fromDate, toDate);


            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            return fsr;
        }
    }

    
}