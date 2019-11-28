
using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using OfficeOpenXml;
using System.Globalization;
using System.Reflection;
using NPOI.XWPF.UserModel;
using Microsoft.Office.Interop.Word;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Text.RegularExpressions;
using Spire.Doc;

namespace MWMS2.Services
{
    public class BaseCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public string getString(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return "";




            }
            else
            {
                var si = new System.Globalization.StringInfo(txt);
                var l = si.LengthInTextElements; // length is equal to 6.
                                                 ///Label3.Text = si.SubstringByTextElements(0, l-1); //no exception!


                return si.SubstringByTextElements(0, l);
            }



        }



        public FileStreamResult exportExcelFile(string fileName,
                List<string> Columns, List<List<object>> Data,string contentTitle = null)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");


            //string headerRange = "A1:" + Char.ConvertFromUtf32(Columns.Count + 64) + "1";
            //sheet.Cells[headerRange].LoadFromArrays(
            //    new List<object[]>() { Columns.ToArray() });
            //sheet.Cells[headerRange].Style.Font.Bold = true;
            //sheet.Cells[headerRange].Style.Font.Size = 14;
            //sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

            //Add the content title
            int startRow = 1;
            if (!string.IsNullOrEmpty(contentTitle))
            {
                string endColumnName = string.Empty;
                var dividend = Columns.Count;
                while (dividend > 0)
                {
                    var module = (dividend - 1) % 26;
                    endColumnName = Convert.ToChar(65 + module) + endColumnName;
                    dividend = (dividend - module) / 26;
                }
                string headerRange = "A1:" + endColumnName + "1";
                sheet.Cells[headerRange].LoadFromText(contentTitle);
                sheet.Cells[headerRange].Merge = true;
                sheet.Cells[headerRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[headerRange].Style.Font.Bold = true;
                sheet.Cells[headerRange].Style.Font.Size = 38;
                sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            //for (int i = 0; i < Columns.Count; i++)
            //{
            //    sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
            //    sheet.Cells[1, i + 1].Style.Font.Bold = true;
            //    sheet.Cells[1, i + 1].Style.Font.Size = 14;
            //    sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            //}

            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[startRow, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                sheet.Cells[startRow, i + 1].Style.Font.Size = 14;
                sheet.Cells[startRow, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            sheet.Cells.AutoFitColumns();



            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        //sheet.Cells[i + 2, j + 1].Value =
                        //    (getString(eachRow[j].ToString()));
                        sheet.Cells[i + startRow + 1, j + 1].Value =
                        (getString(eachRow[j].ToString()));


                        //sheet.Cells[i + 2, j + 1].LoadFromText(getString(eachRow[j].ToString()));
                    }
                }
            }
            /**
            string path = @"C:\MWMS2\test.xlsx "; 
            Stream streamDDD = File.Create(path);
            ep.SaveAs(streamDDD);
            streamDDD.Close();
               **/
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            //DateTime nowDt = DateTime.Now;
            // addMemory(nowDt, fsr);
            //return nowDt.Ticks.ToString();
            return fsr;
        }

        public string appendDoubleQuote(String str)
        {
            str = String.IsNullOrEmpty(str) ? "\"" + "\"" : "\"" + str.Trim() + "\"";
            return str;
        }

        public string getEnglishFormatDate(DateTime? date)
        {
            String engDate = date.Value.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            return engDate;
        }

        public string appendCSVLine(String dataContent, string appendStr)
        {
            appendStr = appendDoubleQuote(appendStr);

            dataContent += String.IsNullOrEmpty(dataContent) ? appendStr : "," + appendStr;
            return dataContent;
        }
        public string appendNewLine(String dataContent)
        {
            dataContent += Environment.NewLine;
            return dataContent;
        }

        /*
        public FileStreamResult exportCSVFile2(string fileName, List<string> Columns,
                List<List<object>> Data)
        {
            FileStreamResult result = exportFile(fileName, Columns, Data);
            result.FileDownloadName = fileName + ".csv";
            return result;
        }
        */
        public FileStreamResult exportCSVFile(string fileName, List<string> Columns,
         List<List<object>> Data)
        {
            FileStreamResult result = exportCSVFile(fileName, "csv", Columns, Data);
            //result.FileDownloadName = fileName + "");
            return result;
        }
        public FileStreamResult exportCSVFile(string fileName, string ext, List<string> Columns,
                List<List<object>> Data)
        {

            var sb = new StringBuilder();
            String eachLine = "";
            for (int i = 0; i < Columns.Count; i++)
            {
                eachLine = appendCSVLine(eachLine, Columns[i]);
            }
            eachLine = appendNewLine(eachLine);
            sb.Append(eachLine);

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    eachLine = "";
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        object dataObejct = eachRow[j];
                        String dataValue = "";
                        if (dataObejct is DateTime)
                        {
                            dataValue = ((DateTime)dataObejct).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                        }
                        else
                        {
                            dataValue = dataObejct.ToString();

                        }
                        eachLine = appendCSVLine(eachLine, dataValue);
                    }
                    eachLine = appendNewLine(eachLine);
                    sb.Append(eachLine);
                }
            }
            //var byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var byteArray = Encoding.GetEncoding("Big5").GetBytes(sb.ToString());

            //var byteArray = Encoding.Default.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var mimeType = "text/csv";
            FileStreamResult result = new FileStreamResult(stream, mimeType);
            result.FileDownloadName = fileName + (ext == null ? "" : ("." + ext));


            return result;
        }

        public FileStreamResult exporTxtFile(string fileName, List<string> Columns,
                List<List<object>> Data)
        {

            var sb = new StringBuilder();
            String eachLine = "";
            //header
            for (int i = 0; i < Columns.Count; i++)
            {
                eachLine = appendCSVLine(eachLine, Columns[i]);
            }
            eachLine = appendNewLine(eachLine);
            sb.Append(eachLine);

            //data
            // if (Data != null){

            for (int i = 0; i < 1; i++)
            {
                List<object> eachRow = Data[i];
                eachLine = "";
                for (int j = 0; j < 4; j++)
                {

                    object dataObejct = eachRow[j];

                    String dataValue = "";
                    if (dataObejct is DateTime)
                    {
                        dataValue = ((DateTime)dataObejct).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                    }
                    else
                    {
                        dataValue = dataObejct.ToString();

                    }
                    eachLine = appendCSVLine(eachLine, dataValue);
                }

                //eachLine = appendNewLine(eachLine);
                //sb.Append(eachLine);
            }

            for (int i = 0; i < Data.Count; i++)
            {
                List<object> eachRow = Data[i];

                //eachLine = "";
                for (int j = 4; j < 6; j++)
                {

                    object dataObejct = eachRow[j];
                    String dataValue = "";
                    if (dataObejct is DateTime)
                    {
                        dataValue = ((DateTime)dataObejct).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                    }
                    else
                    {
                        dataValue = dataObejct.ToString();

                    }
                    eachLine = appendCSVLine(eachLine, dataValue);
                }

                //eachLine = appendNewLine(eachLine);

            }
            sb.Append(eachLine);
            //}
            var byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var mimeType = "text/plain";
            FileStreamResult result = new FileStreamResult(stream, mimeType);
            //result.FileDownloadName = fileName + ".csv";
            result.FileDownloadName = fileName + ".txt";
            return result;

        }

        public FileStreamResult ExportFile(string filePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath + "/" + fileName);
            var stream = new MemoryStream(fileBytes);
            //var mimeType = "text/plain";
            string mimeType = System.Web.MimeMapping.GetMimeMapping(fileName);

            FileStreamResult result = new FileStreamResult(stream, mimeType);
            //result.FileDownloadName = fileName + ".docx";
            return result;





        }
        public static string getChineseFormatDate(DateTime? date)
        {
            if (date == null)
            {
                return "date is null";
            }
            int year = date.Value.Year;
            int month = date.Value.Month;
            int dates = date.Value.Day;
            return year + "年" + month + "月" + dates + "日";
        }


        // Begin add by chester 2019-07-19
        private static void ReplaceKey<T>(T entity, XWPFParagraph paragraph)
        {
            Type entityType = typeof(T);
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            string entityName = entityType.Name;
            string paragraphText = paragraph.ParagraphText;
            string styleId = paragraph.Style;
            string text = paragraph.ParagraphText;
            foreach (var p in propertyInfos)
            {
                string replaceName = "${" + p.Name + "}";
                object value = p.GetValue(entity);
                if (value == null)
                {
                    value = "";
                }
                if (text.Contains(replaceName))
                {
                    //paragraph.ReplaceText(replaceName, value.ToString());
                    if (paragraph.ParagraphText.Contains(replaceName))
                    {
                        paragraph.ReplaceText(replaceName, value.ToString());
                    }
                }

            }
        }
        private static void SetFontFamily(XWPFParagraph paragraph)
        {
            foreach (var run in paragraph.Runs)
            {
                Regex regex = new Regex(@"^[A-Za-z0-9]+$");
                string strInput = run.Text.Replace(" ", "");
                strInput = Regex.Replace(strInput, "[ \\[ \\] \\^ \\/ \\-_ *×――(^)（^）$% ~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
                strInput = Regex.Replace(strInput, "[^\u4e00-\u9fa5a-zA-Z0-9]", "");
                CT_RPr rpr = run.GetCTR().AddNewRPr();
                CT_Fonts fonts = rpr.AddNewRFonts();
                if (regex.IsMatch(strInput))
                {
                    fonts.ascii = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY;
                    fonts.eastAsia = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY;
                    run.FontFamily = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY;
                }
                else
                {
                    fonts.ascii = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY;
                    fonts.eastAsia = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY;
                    run.FontFamily = ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY;
                }
            }
        }
        public static XWPFDocument GetWordDocument<T>(string tempPath, T model, CT_SectPr sectPr, string fontFamily)
        {

            if (Directory.Exists(tempPath) || File.Exists(tempPath))

            //if (Directory.Exists(tempPath))
            {
                using (FileStream fs = File.OpenRead(tempPath))
                {
                    XWPFDocument doc = new XWPFDocument(fs);

                    if (sectPr != null)
                        doc.Document.body.sectPr = sectPr;

                    // Begin word header
                    if (doc.HeaderList != null)
                    {
                        foreach (XWPFHeader header in doc.HeaderList)
                        {
                            foreach (var table in header.Tables)
                            {
                                ReplaceTable(table, model);
                            }
                            foreach (var headerPara in header.Paragraphs)
                            {
                                ReplaceKey(model, headerPara);
                            }
                        }
                    }
                    // End word header

                    // Begin word footer
                    if (doc.FooterList != null)
                    {
                        foreach (XWPFFooter footer in doc.FooterList)
                        {
                            foreach (var table in footer.Tables)
                            {
                                ReplaceTable(table, model);
                            }
                            foreach (var footerPara in footer.Paragraphs)
                            {
                                ReplaceKey(model, footerPara);
                            }
                        }
                    }
                    // End word footer

                    // Begin word table
                    if (doc.Tables != null)
                    {
                        foreach (var table in doc.Tables)
                        {
                            foreach (var row in table.Rows)
                            {
                                foreach (var cell in row.GetTableCells())
                                {
                                    if (cell.Tables != null)
                                    {
                                        foreach (var cellTable in cell.Tables)
                                        {
                                            foreach (var cellRow in cellTable.Rows)
                                            {
                                                foreach (var cellCol in cellRow.GetTableCells())
                                                {
                                                    foreach (var cellPara in cellCol.Paragraphs)
                                                    {
                                                        ReplaceKey(model, cellPara);
                                                        SetFontFamily(cellPara);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    foreach (var cellPara in cell.Paragraphs)
                                    {
                                        ReplaceKey(model, cellPara);
                                        SetFontFamily(cellPara);
                                    }
                                }
                            }
                        }
                    }

                    //End word table

                    //Begin word body
                    if (doc.Paragraphs != null)
                    {
                        foreach (XWPFParagraph paragraph in doc.Paragraphs)
                        {
                            ReplaceKey(model, paragraph);
                        }
                    }
                    //End word body

                    return doc;
                }
            }
            else
                return null;

        }

        public static void ReplaceTable<T>(XWPFTable table, T model)
        {
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.GetTableCells())
                {
                    if (cell.Tables != null)
                    {
                        foreach (var cellPara in cell.Paragraphs)
                        {
                            ReplaceKey(model, cellPara);
                            SetFontFamily(cellPara);
                        }
                    }
                }
            }
        }

        public static XWPFDocument GetWordDocument<T>(string tempPath, XWPFDocument doc, T model, CT_SectPr sectPr, string fontFamily)
        {

            if (Directory.Exists(tempPath) || File.Exists(tempPath))
            {
                using (FileStream fs = File.OpenRead(tempPath))
                {
                    if (sectPr != null)
                        doc.Document.body.sectPr = sectPr;

                    // Begin word header
                    if (doc.HeaderList != null)
                    {
                        foreach (XWPFHeader header in doc.HeaderList)
                        {
                            foreach (var table in header.Tables)
                            {
                                ReplaceTable(table, model);
                            }
                            foreach (var headerPara in header.Paragraphs)
                            {
                                ReplaceKey(model, headerPara);
                            }
                        }
                    }
                    // End word header

                    // Begin word footer
                    if (doc.FooterList != null)
                    {
                        foreach (XWPFFooter footer in doc.FooterList)
                        {
                            foreach (var table in footer.Tables)
                            {
                                ReplaceTable(table, model);
                            }
                            foreach (var footerPara in footer.Paragraphs)
                            {
                                ReplaceKey(model, footerPara);
                            }
                        }
                    }
                    // End word footer

                    // Begin word table
                    if (doc.Tables != null)
                    {
                        foreach (var table in doc.Tables)
                        {
                            foreach (var row in table.Rows)
                            {
                                foreach (var cell in row.GetTableCells())
                                {
                                    if (cell.Tables != null)
                                    {
                                        foreach (var cellTable in cell.Tables)
                                        {
                                            foreach (var cellRow in cellTable.Rows)
                                            {
                                                foreach (var cellCol in cellRow.GetTableCells())
                                                {
                                                    foreach (var cellPara in cellCol.Paragraphs)
                                                    {
                                                        ReplaceKey(model, cellPara);
                                                        SetFontFamily(cellPara);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    foreach (var cellPara in cell.Paragraphs)
                                    {
                                        ReplaceKey(model, cellPara);
                                        SetFontFamily(cellPara);
                                    }
                                }
                            }
                        }
                    }

                    //End word table

                    //Begin word body
                    if (doc.Paragraphs != null)
                    {
                        foreach (XWPFParagraph paragraph in doc.Paragraphs)
                        {
                            ReplaceKey(model, paragraph);
                        }
                    }
                    //End word body

                    return doc;
                }
            }
            else
                return null;

        }

        public static byte[] WordToPDF<T>(string tempPath, object tmpPDFPath, string tmpWordPath, T entity, CT_SectPr sectPr, string fontFamily)
        {
            string[] tempDirectories = tmpWordPath.Split('\\');
            string tmpWordDirectory = "";
            for (int i = 0; i < tempDirectories.Length - 1; i++)
            {
                tmpWordDirectory = tmpWordDirectory + tempDirectories[i] + "\\";
            }
            if (!Directory.Exists(tmpWordDirectory))
            {
                Directory.CreateDirectory(tmpWordDirectory);
            }
            using (FileStream fs = File.Create(tmpWordPath))
            {
                XWPFDocument resultWord = GetWordDocument(tempPath, entity, sectPr, fontFamily);
                resultWord.Write(fs);
            }

            //Spire.Doc.Document document = new Spire.Doc.Document();
            //document.LoadFromFile(tmpWordPath);
            //document.SaveToFile(tmpPDFPath.ToString(), FileFormat.PDF);

            Application app = new Application();
            object nullobj = Missing.Value;
            object file = tmpWordPath;
            Microsoft.Office.Interop.Word.Document doc = app.Documents.Open(
            ref file, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            doc.Activate();

            object fileFormat = WdSaveFormat.wdFormatPDF;
            doc.SaveAs(ref tmpPDFPath,
                        ref fileFormat, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            object missingValue = Type.Missing;
            object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(ref nullobj, ref nullobj, ref nullobj);
            app.Quit(ref nullobj, ref nullobj, ref nullobj);

            byte[] bytes = File.ReadAllBytes(tmpPDFPath.ToString());
            File.Delete(tmpWordPath);
            File.Delete(tmpPDFPath.ToString());
            return bytes;
        }
        public static byte[] WordToPDF<T>(string tempPath, XWPFDocument resultWord, object tmpPDFPath, string tmpWordPath, T entity, CT_SectPr sectPr, string fontFamily)
        {
            string[] tempDirectories = tmpWordPath.Split('\\');
            string tmpWordDirectory = "";
            for (int i = 0; i < tempDirectories.Length - 1; i++)
            {
                tmpWordDirectory = tmpWordDirectory + tempDirectories[i] + "\\";
            }
            if (!Directory.Exists(tmpWordDirectory))
            {
                Directory.CreateDirectory(tmpWordDirectory);
            }
            using (FileStream fs = File.Create(tmpWordPath))
            {
                resultWord = GetWordDocument(tempPath, resultWord, entity, sectPr, fontFamily);
                resultWord.Write(fs);
            }

            Application app = new Application();
            object nullobj = Missing.Value;
            object file = tmpWordPath;
            Microsoft.Office.Interop.Word.Document doc = app.Documents.Open(
            ref file, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            doc.Activate();

            object fileFormat = WdSaveFormat.wdFormatPDF;
            doc.SaveAs(ref tmpPDFPath,
                        ref fileFormat, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            object missingValue = Type.Missing;
            object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(ref nullobj, ref nullobj, ref nullobj);
            app.Quit(ref nullobj, ref nullobj, ref nullobj);

            byte[] bytes = File.ReadAllBytes(tmpPDFPath.ToString());
            File.Delete(tmpWordPath);
            File.Delete(tmpPDFPath.ToString());
            return bytes;
        }

        public static T DeepCopy<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
        // End add by chester 2019-07-19
    }
}
