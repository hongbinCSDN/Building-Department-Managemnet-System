using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services;
using MWMS2.Utility;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class DisplayGrid
    {
      
        public string doSearch { get; set; } 
        private int _rpp = 0;
        private const int defaultPage = 1;
        private const int defaultRpp = 100;
        private const string defaultSort = "1";
        private const int defaultSortType = 0;
        public string Key { get; set; } = "";
        public int Page { get; set; } = defaultPage;
        public int Rpp { get { return _rpp == 0 ? defaultRpp : _rpp; } set { _rpp = value; } }
        public int Total { get; set; }
        public string Sort { get; set; } = defaultSort;
        public int SortType { get; set; } = defaultSortType;
        public Dictionary<string, string>[] Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public string Query { private get; set; } = "";
        public string QueryWhere { private get; set; } = "";
        public Dictionary<string, object> QueryParameters { get; set; } = new Dictionary<string, object>();
        private static Dictionary<DateTime, FileStreamResult> preloadMemory = new Dictionary<DateTime, FileStreamResult>();
        public string ExportKey { get; set; }

        public Dictionary<string, object> Const
        {
            get
            {
                Dictionary<string, object> o = new Dictionary<string, object>();
                o.Add("DISPLAY_DATE_FORMAT", ApplicationConstant.DISPLAY_DATE_FORMAT);
                o.Add("DISPLAY_DATE_COLUMN_SUFFIX", ApplicationConstant.DISPLAY_DATE_COLUMN_SUFFIX);
                return o;
            }
        }
        public FormCollection Parameters
        {
            set
            {
                if (value == null) return;
                Page = value["Page"] == null || "null".Equals(value["Page"]) ? defaultPage : int.Parse(value["Page"]);
                Rpp = value["Rpp"] == null || "null".Equals(value["Rpp"]) ? defaultRpp : int.Parse(value["Rpp"]);
                Sort = value["Sort"] == null || "null".Equals(value["Sort"]) ? defaultSort : value["Sort"];
                SortType = value["SortType"] == null || "null".Equals(value["SortType"]) ? defaultSortType : int.Parse(value["SortType"]);
            }
        }
        public void AppSort(string column)
        {
            Data = Data.OrderBy(o => o[column]).ToList(); ;
        }

        public void Search()
        {
            Page = Page <= 0 ? defaultPage : Page;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    DbDataReader dr = CommonUtil.GetDataReader(conn
                        , "SELECT * FROM (SELECT a.*, rownum r__ FROM ("
                        + Query + "\r\n\r\n"
                        + QueryWhere + "\r\n\r\n"
                        + " ORDER BY " + (Sort == null ? "1" : Sort) + (SortType == 1 ? " DESC " : " ASC") + " ) a " +
                        (Rpp > 0 ? "WHERE rownum <" + (Rpp * Page + 1) : "") +

                        ") " + (Rpp > 0 ? "WHERE r__ >= " + ((Page - 1) * Rpp + 1) : "")
                        , QueryParameters);

                    if (dr != null && Columns != null)
                    {
                        for (int i = 0; i < Columns.Length; i++)
                        {
                            if (!Columns[i].ContainsKey("columnName")) continue;
                            if (Columns[i].ContainsKey("format")) continue;
                            for (int j = 0; j < dr.FieldCount; j++)
                            {
                                if (dr.GetName(j).Equals(Columns[i]["columnName"]))
                                {
                                    Columns[i]["format"] = dr.GetFieldType(j).ToString();
                                    break;
                                }
                            }
                        }
                    }
                    else if (Columns == null)
                    {
                        List<Dictionary<string, string>> columnList = new List<Dictionary<string, string>>();
                        for (int j = 0; j < dr.FieldCount; j++)
                        {
                            columnList.Add(new Dictionary<string, string>()
                            {
                                ["format"] = dr.GetFieldType(j).ToString(),
                                ["columnName"] = dr.GetName(j)
                            });
                        }
                        Columns = columnList.ToArray();
                    }
                    Data = CommonUtil.LoadDbData(dr);

                    Total = CommonUtil.LoadDbCount(CommonUtil.GetDataReader(conn
                        , "SELECT COUNT(*) FROM (" + Query + "\r\n\r\n" + QueryWhere + ")"
                        , QueryParameters));
                    conn.Close();
                }
            }
        }
        public string Export(string fileName)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    Data = CommonUtil.LoadDbData(CommonUtil.GetDataReader(conn
                        , Query + "\r\n\r\n" + QueryWhere + "\r\n\r\n" + " ORDER BY " + (Sort == null ? "1" : Sort) + (SortType == 1 ? " DESC " : " ASC")
                        , QueryParameters));
                    conn.Close();
                    return preloadExcel(fileName);
                }
            }
        }

        public string ExportCurrentData(string fileName)
        {
            return preloadExcel(fileName);
        }

        public string ExportWithCriteria(ExcelPackage ep, ExcelWorksheet sheet, int rowIdx, string fileName)
        {
            preloadExcel(sheet, rowIdx);
            return GetExportKey(ep, fileName);
        }

        public string ExcelColumnFromNumber(int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }


        private int NumberFromExcelColumn(string column)
        {
            int retVal = 0;
            string col = column.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }
        private string preloadExcel(string fileName)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            //string headerRange = "A1:" + Char.ConvertFromUtf32(Columns.Length + 64) + "1";

            string headerRange = "A1:" + ExcelColumnFromNumber(Columns.Length) + "1";
            var q = new List<object[]>() { Columns.Select(o => o["displayName"]).ToArray<object>() };



            sheet.Cells[headerRange].LoadFromArrays(new List<object[]>() { Columns.Select(o => o["displayName"]).ToArray<object>() });
            sheet.Cells[headerRange].Style.Font.Bold = true;
            sheet.Cells[headerRange].Style.Font.Size = 14;
            sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    Dictionary<string, object> data = Data[i];
                    for (int j = 0; j < Columns.Length; j++)
                    {
                        if (Columns[j].ContainsKey("columnName") && !string.IsNullOrWhiteSpace(Columns[j]["columnName"]))
                        {
                            sheet.Cells[i + 2, j + 1].Style.Numberformat.Format = "@";
                            sheet.Cells[i + 2, j + 1].Value = (data[Columns[j]["columnName"].Trim().ToString()])?.ToString();
                            //sheet.Cells[i + 2, j + 1].LoadFromText(data[Columns[j]["columnName"].Trim().ToString()]?.ToString());
                        }
                    }
                }
            }

            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            DateTime nowDt = DateTime.Now;
            addMemory(nowDt, fsr);
            ExportKey = nowDt.Ticks.ToString();
            return ExportKey;
        }

        public int preloadExcel(ExcelWorksheet sheet, int rowIdx)
        {

            string headerRange = "A" + rowIdx + ":" + ExcelColumnFromNumber(Columns.Length) + rowIdx.ToString();
            var q = new List<object[]>() { Columns.Select(o => o["displayName"]).ToArray<object>() };

            sheet.Cells[headerRange].LoadFromArrays(new List<object[]>() { Columns.Select(o => o["displayName"]).ToArray<object>() });
            sheet.Cells[headerRange].Style.Font.Bold = true;
            sheet.Cells[headerRange].Style.Font.Size = 14;
            sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();
            rowIdx++;

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    Dictionary<string, object> data = Data[i];
                    for (int j = 0; j < Columns.Length; j++)
                    {

                        if (Columns[j].ContainsKey("columnName") && !string.IsNullOrWhiteSpace(Columns[j]["columnName"]))
                        {
                            sheet.Cells[rowIdx, j + 1].Style.Numberformat.Format = "@";
                            sheet.Cells[rowIdx, j + 1].Value = (data[Columns[j]["columnName"].Trim().ToString()])?.ToString();
                            //if ((data[Columns[j]["columnName"].Trim().ToString()]) is DateTime)
                            //{
                            //    sheet.Cells[rowIdx, j + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                            //    sheet.Cells[rowIdx, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //    sheet.Cells[rowIdx, j + 1].LoadFromText(Convert.ToDateTime(data[Columns[j]["columnName"].Trim().ToString()]).ToString("dd/MM/yyyy"));
                            //}
                            //else
                            //{
                            //    sheet.Cells[rowIdx, j + 1].LoadFromText(data[Columns[j]["columnName"].Trim().ToString()]?.ToString());
                            //}
                        }
                    }
                    rowIdx++;
                }
            }
            return rowIdx;
        }

        public Dictionary<string, string> CreateExcelColumn(string displayName, string columnName)
        {
            Dictionary<string, string> col = new Dictionary<string, string>();
            col.Add("displayName", displayName);
            col.Add("columnName", columnName);
            return col;
        }

        private string GetExportKey(ExcelPackage ep, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            DateTime nowDt = DateTime.Now;
            addMemory(nowDt, fsr);
            ExportKey = nowDt.Ticks.ToString();
            return ExportKey;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void addMemory(DateTime k, FileStreamResult fsr)
        {
            DateTime nowDt = DateTime.Now;
            foreach (KeyValuePair<DateTime, FileStreamResult> entry in preloadMemory.ToList())
            {
                TimeSpan ts = (nowDt - entry.Key);
                if (ts.TotalSeconds > 60) preloadMemory.Remove(entry.Key);
            }
            preloadMemory.Add(k, fsr);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static FileStreamResult getMemory(string key)
        {
            DateTime? dt = CommonUtil.Ticks2Datetime(key);
            FileStreamResult fsr = null;
            if (dt != null)
            {
                fsr = preloadMemory[dt.Value];
                preloadMemory.Remove(dt.Value);
            }
            return fsr;
        }



    }
}