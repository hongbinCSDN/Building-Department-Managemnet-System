
using MWMS2Interface.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
namespace MWMS2Interface
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

         
    
        public void AppSort(string column)
        {
            Data = Data.OrderBy(o => o[column]).ToList(); ;
        }

        public void Search()
        {
            Page = Page <= 0 ? defaultPage : Page;
            using (EntitiesProcessing db = new EntitiesProcessing())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    DbDataReader dr = CommonUtil.GetDataReader(conn
                        , Query
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
     
        public Dictionary<string, string> CreateExcelColumn(string displayName, string columnName)
        {
            Dictionary<string, string> col = new Dictionary<string, string>();
            col.Add("displayName", displayName);
            col.Add("columnName", columnName);
            return col;
        }

    
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void addMemory(DateTime k, FileStreamResult fsr)
        //{
        //    DateTime nowDt = DateTime.Now;
        //    foreach (KeyValuePair<DateTime, FileStreamResult> entry in preloadMemory.ToList())
        //    {
        //        TimeSpan ts = (nowDt - entry.Key);
        //        if (ts.TotalSeconds > 60) preloadMemory.Remove(entry.Key);
        //    }
        //    preloadMemory.Add(k, fsr);
        //}

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static FileStreamResult getMemory(string key)
        //{
        //    DateTime? dt = CommonUtil.Ticks2Datetime(key);
        //    FileStreamResult fsr = null;
        //    if (dt != null)
        //    {
        //        fsr = preloadMemory[dt.Value];
        //        preloadMemory.Remove(dt.Value);
        //    }
        //    return fsr;
        //}



    }
}