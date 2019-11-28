using ExcelReader.DA;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace ExcelReader.BL
{
    public class Excel_BL
    {
        private Excecl_DA _DA;

        protected Excecl_DA DA
        {
            get { return _DA ?? (_DA = new Excecl_DA()); }
        }

        public void ExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook = null;

            using (FileStream fileStream = new FileStream(filePath,FileMode.Open,FileAccess.Read) )  // File.OpenRead(filePath)
            {
                if (Path.GetExtension(filePath).ToLower() == ".xls".ToLower())
                {
                    workbook = new HSSFWorkbook(fileStream);
                    string sheetName = workbook.GetSheetName(workbook.ActiveSheetIndex);
                    ISheet sheet = workbook.GetSheetAt(workbook.ActiveSheetIndex);
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                    HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
                    for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
                    {
                        HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                        dt.Columns.Add(cell.ToString());
                    }
                    while (rows.MoveNext())
                    {
                        IRow row = (HSSFRow)rows.Current;
                        DataRow dr = dt.NewRow();
                        if (row.RowNum == 0) continue;

                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            if (i >= dt.Columns.Count)
                            {
                                break;
                            }

                            ICell cell = row.GetCell(i);

                            if ((i == 0) && (string.IsNullOrEmpty(cell.ToString()) == true))
                            {
                                break;
                            }

                            if (cell == null)
                            {
                                dr[i] = null;
                            }
                            else
                            {
                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dr[i] = cell.DateCellValue.ToString("yyyy-MM-dd hh:mm:ss"); 
                                }
                                else
                                {
                                    dr[i] = row.GetCell(i).ToString();
                                }
                            }
                        }

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    workbook = new XSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(workbook.ActiveSheetIndex);
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                    XSSFRow headerRow = (XSSFRow)sheet.GetRow(0);


                    for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
                    {
                        //SET EVERY COLUMN NAME
                        XSSFCell cell = (XSSFCell)headerRow.GetCell(j);

                        dt.Columns.Add(cell.ToString());

                    }
                    while (rows.MoveNext())
                    {
                        IRow row = (XSSFRow)rows.Current;
                        DataRow dr = dt.NewRow();

                        if (row.RowNum == 0) continue;

                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            if (i >= dt.Columns.Count)
                            {
                                break;
                            }

                            ICell cell = row.GetCell(i);

                            if ((i == 0) && (string.IsNullOrEmpty(cell.ToString()) == true))
                            {
                                break;
                            }

                            if (cell == null)
                            {
                                dr[i] = null;
                            }
                            else
                            {
                                //dr[i] = cell.ToString();
                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dr[i] = cell.DateCellValue.ToString("yyyy-MM-dd hh:mm:ss");
                                }
                                else
                                {
                                    dr[i] = row.GetCell(i).ToString();
                                }
                            }
                        }
                        dt.Rows.Add(dr);
                    }

                }
                //return dt;
                DA.InsertExcelDataToDB(dt, workbook.GetSheetName(workbook.ActiveSheetIndex));
            }

        }

        public DataSet GetSysFunc()
        {
            return DA.GetSysFunc();
        }
    }
}