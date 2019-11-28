using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class DisplayGridMWP
    {
        private const int defaultPage = 1;
        private const int defaultRpp = 10;
        private const string defaultSort = "1";
        private const int defaultSortType = 0;
        public string Key { get; set; } = "";
        public int Page { get; set; } = defaultPage;
        public int Rpp { get; set; } = defaultRpp;
        public int Total { get; set; }
        public string Sort { get; set; } = defaultSort;
        public int SortType { get; set; } = defaultSortType;
        public Dictionary<string, string>[] Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public string Query { private get; set; } = "";
        public string QueryWhere { private get; set; } = "";
        public Dictionary<string, object> QueryParameters { get; set; } = new Dictionary<string, object>();
        private static Dictionary<DateTime, FileStreamResult> preloadMemory = new Dictionary<DateTime, FileStreamResult>();


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
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    DbDataReader dr = CommonUtil.GetDataReader(conn
                        , "SELECT * FROM (SELECT a.*, rownum r__ FROM (" + Query + "\r\n\r\n" + QueryWhere + "\r\n\r\n" + " ORDER BY " + (Sort == null ? "1" : Sort) + (SortType == 1 ? " DESC " : " ASC") + " ) a WHERE rownum <" + (Rpp * Page + 1) + ") WHERE r__ >= " + ((Page - 1) * Rpp + 1)
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

        private string preloadExcel(string fileName)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            string headerRange = "A1:" + Char.ConvertFromUtf32(Columns.Length + 64) + "1";
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
                        sheet.Cells[i + 2, j + 1].LoadFromText(data[Columns[j]["columnName"].ToString()].ToString());
                    }
                }
            }

            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            DateTime nowDt = DateTime.Now;
            addMemory(nowDt, fsr);
            return nowDt.Ticks.ToString();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void addMemory(DateTime k, FileStreamResult fsr)
        {
            DateTime nowDt = DateTime.Now;
            foreach (KeyValuePair<DateTime, FileStreamResult> entry in preloadMemory)
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