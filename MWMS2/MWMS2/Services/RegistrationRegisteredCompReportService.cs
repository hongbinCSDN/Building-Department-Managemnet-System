using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class RegistrationRegisteredCompReportService: RegistrationCommonService
    {
        public FileStreamResult exportRegisteredCompanyReport(Fn01Search_RCRSearchModel model,string ExportType)
        {

            bool haveMWC = false;
            string logic = model.Logic;
            string orderBy = model.rc_orderBy;
            string[] appTypes = model.getRc_applcationTypes();
            string[] subTypes = model.getRc_subTypes();
            string[] groupTypes = model.getRc_groupTypes();
            DateTime? specifiedDate = model.specifiedDate;
            if (orderBy == "101")
            {
                orderBy = "101";
            }
            else if (orderBy == "102")
            {
                orderBy = "102";
            }
            else if (orderBy == "")
            {
                orderBy = "1";
            }
            else
            {
                orderBy = ((orderBy) + 1) + "";
            }
            List<List<object>> list = GetCompReportData(appTypes, subTypes, groupTypes, logic, orderBy, haveMWC, specifiedDate);
            //Header
            List<string> columnHeaders = new List<string>();
            columnHeaders.Add("Name");
            columnHeaders.Add("BR Number");
            if (haveMWC)
            {
                columnHeaders.Add("District Area");
            }
            string[] SCType = new string[] { "SC(D)", "SC(F)", "SC(GI)", "SC(SF)", "SC(V)" };

            for (int i = 0; i < appTypes.Length; i++)
            {
                if (appTypes[i] != (""))
                {
                    if (appTypes[i] == "GBC")
                    {
                        columnHeaders.Add("GBC");
                    }
                    else if (appTypes[i] == "SC")
                    {
                        for (int j = 0; j < groupTypes[i].Count(); j++)
                        {
                            columnHeaders.Add("SC(" + SCType[int.Parse(groupTypes[i][j]+" ")- 1] + ")");
                        }
                    }
                    else if (appTypes[i] != null)
                    {
                        if (appTypes[i].IndexOf("MWC") >= 0)
                        {
                            for (int j = 0; j < groupTypes[i].Count(); j++)
                            {
                                columnHeaders.Add(appTypes[i] + " (" + groupTypes[i][j] + ")");
                            }
                        }
                    }
                }
            }
            if (ExportType == "Exc") { 
                return exportCompExcelFile("RegisteredCompanyReport", columnHeaders, list, haveMWC);
            }
            else
            {
                return exportCSVFile("RegisteredCompanyReport", columnHeaders, list);
            }
        }
        public FileStreamResult exportCompExcelFile(string fileName, List<string> Columns, List<List<object>> Data, bool haveMWC)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            int perAdd = haveMWC ? 0 : 1;
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
                sheet.Cells[1, i + 1].Style.Font.Size = 14;
                sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < Columns.Count; j++)
                    {
                        if (j > 1)
                        {
                            sheet.Cells[i + 2, j + 1].LoadFromText(eachRow[j + perAdd].ToString());
                        }
                        else
                        {
                            sheet.Cells[i + 2, j + 1].LoadFromText(eachRow[j].ToString());
                        }
                    }
                }
            }
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            return fsr;
        }
        public List<List<object>> GetCompReportData(string[] appTypes,
            string[] subTypes, string[] groupTypes, string logic, string orderBy, bool haveMWC, DateTime? specifiedDate)
        {
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            DateTime? currentDate= DateTime.Now.Date;
            int a = 0;
            for (int i = 0; i < appTypes.Length; i++)
            {
                if (appTypes[i] != null && appTypes[i] != "")
                {
                    QueryParameters.Add("appType" + a, appTypes[i]);
                    if (subTypes[i]!=null) {
                        QueryParameters.Add("subType" + a, subTypes[i].ToLower());
                    }else{
                        QueryParameters.Add("subType" + a, subTypes[i]);
                    }
                    if (subTypes[i] != null){
                        QueryParameters.Add("groupType" + a, groupTypes[i].ToUpper());
                    }else{
                        QueryParameters.Add("groupType" + a, groupTypes[i]);
                    }
                    a++;
                }

            }
            for (int i = a; i < 7; i++)
            {

                QueryParameters.Add("appType" + i, null);
                QueryParameters.Add("subType" + i, null);
                QueryParameters.Add("groupType" + i, null);

            }
            QueryParameters.Add("logic", logic);
            QueryParameters.Add("orderBy", orderBy);

            for (int i = 0; i < appTypes.Length; i++)
            {
                if (appTypes[i] != null)
                {
                    if (appTypes[i].IndexOf("MWC") >= 0)
                    {
                        haveMWC = true;
                        break;
                    }
                }
            }
            if (haveMWC)
            {
                QueryParameters.Add("haveMw", "2");
            }
            else
            {
                QueryParameters.Add("haveMw", "2");
            }
            if (specifiedDate!=null)
            {
                QueryParameters.Add("specDate", specifiedDate.Value);
            }
            else
            {
                QueryParameters.Add("specDate", currentDate.Value);
            }
            string queryStr = "";
            queryStr = " SELECT * FROM TABLE(";
            queryStr += " C_GET_REG_COMP(";
            queryStr += " :appType0, :subType0, :groupType0,";
            queryStr += " :appType1, :subType1, :groupType1,";
            queryStr += " :appType2, :subType2, :groupType2,";
            queryStr += " :appType3, :subType3, :groupType3,";
            queryStr += " :appType4, :subType4, :groupType4,";
            queryStr += " :appType5, :subType5, :groupType5,";
            queryStr += " :appType6, :subType6, :groupType6,";
            queryStr += " :logic, :orderBy, :haveMw,:specDate))";

            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, QueryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }
    }
}