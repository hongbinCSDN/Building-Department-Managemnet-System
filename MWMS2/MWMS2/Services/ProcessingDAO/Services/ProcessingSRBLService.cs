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
    public class ProcessingSRBLService: ProcessingReportBLService
    {
        //ProcessingSRDAOService
        private ProcessingSRDAOService DAOService;
        protected ProcessingSRDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingSRDAOService()); }
        }


        public void loadDefault(Fn10RPT_SRSearchModel model)
        {
            string currentMonth =  DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month).Substring(0, 2);
            string currentYear = DateTime.Now.Year.ToString();
            DateTime today = DateTime.Now;
            DateTime BefY = today.AddYears(-1);
            int BefM = 12;
            int BefD = DateTime.DaysInMonth(BefY.Year, BefM);
            model.DateFrom = new DateTime(BefY.Year, BefM, BefD);
            model.DateTo = DateTime.Now;

        }

       

        public FileStreamResult ExportSubmissionRecordToExcel(Fn10RPT_SRSearchModel searchModel)
        {
            string fileName = "SubmissionRecord";
            //Fn04RPT_MRSearchModel exportModel = ViewMRRecord(searchModel);

            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Submission Record");

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '|';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };


            //  sheet.Range["A3:E3"].Style.Font.FontName = "Comic Sans MS";
            //  sheet.Range["A4:E14"].Style.Font.FontName = "Corbel";

            string reportType = CommonUtil.getDisplay(searchModel.ReportType);
            string classCode = CommonUtil.getDisplay(searchModel.WorkClass);
            DateTime fromDate = searchModel.DateFrom;
            DateTime toDate = searchModel.DateTo;

            bool showMw = "".Equals(reportType) || ProcessingConstant.MW.Equals(reportType);
            bool showClassAll = "".Equals(classCode);
            bool showClass1 = ("".Equals(classCode) || ProcessingConstant.DB_CLASS_I.Equals(classCode)) && showMw;
            bool showClass2 = ("".Equals(classCode) || ProcessingConstant.DB_CLASS_II.Equals(classCode)) && showMw;
            bool showClass3 = ("".Equals(classCode) || ProcessingConstant.DB_CLASS_III.Equals(classCode)) && showMw;
            bool showVs = ("".Equals(reportType) || ProcessingConstant.VS.Equals(reportType)) && showClassAll;
            bool showEnq = ("".Equals(reportType) || ProcessingConstant.ENQ.Equals(reportType)) && showClassAll;
            bool showCom = ("".Equals(reportType) || ProcessingConstant.COM.Equals(reportType)) && showClassAll;
            int class1Span = showClass1 ? 0 : 15;
            int class2Span = showClass2 ? 0 : 15;
            int class3Span = showClass3 ? 0 : 10;
            int classSpan = class1Span + class2Span + class3Span;
            classSpan = 0;
            //int mwSpan = showMw ? 0:41;
            int mwSpan = showMw ? 0 : 46;
            int vsSpan = showVs ? 0 : 11;
            int enqSpan = showEnq ? 0 : 3;
            int comSpan = showCom ? 0 : 3;

            int totalRow = 0;
            if (showClass1 && showClass2 && showClass3) totalRow += 1;
            if (showVs) totalRow += 11;
            if (showEnq) totalRow += 3;
            if (showCom) totalRow += 3;
            if (showClass1) totalRow += 15;
            if (showClass2) totalRow += 15;
            if (showClass3) totalRow += 15;


            int rowStart = 1;
            int colStart = 1;

            sheet.Cells[rowStart+0,0+colStart].LoadFromText("Submission Records");

            sheet.Cells[rowStart + 0, 1 + colStart].LoadFromText("From Date: "+fromDate + " To Date: "+ toDate +
                "Report Type: "+ reportType+ "Class: " + classCode);
            if (showClassAll && showMw)
            {
                sheet.Cells[rowStart+2, 0+colStart].LoadFromText(getExcelString("Ratio of 3 Classes Submissions"));
                sheet.Cells[rowStart+2, 3+colStart].LoadFromText(getExcelString("XX"));
                sheet.Cells[rowStart+2, 4+colStart].LoadFromText(getExcelString(":"));
                sheet.Cells[rowStart+2, 5+colStart].LoadFromText(getExcelString("XX"));
                sheet.Cells[rowStart+2, 6+colStart].LoadFromText(getExcelString(":"));
                sheet.Cells[rowStart+2, 7+colStart].LoadFromText(getExcelString("XX"));
                sheet.Cells[rowStart+3, 3+colStart].LoadFromText(getExcelString("Class I"));
                sheet.Cells[rowStart+3, 4+colStart].LoadFromText(getExcelString(":"));
                sheet.Cells[rowStart+3, 5+colStart].LoadFromText(getExcelString("Class II"));
                sheet.Cells[rowStart+3, 6+colStart].LoadFromText(getExcelString(":"));
                sheet.Cells[rowStart+3, 7+colStart].LoadFromText(getExcelString("Class III"));
            }
            sheet.Cells[rowStart+5, 0+colStart].LoadFromText(getExcelString("Type"));
            sheet.Cells[rowStart+5, 1+colStart].LoadFromText(getExcelString("Forms"));
            sheet.Cells[rowStart+5, 2+colStart].LoadFromText(getExcelString("Status"));
            if (showMw)
            {
                if (showClass1) sheet.Cells[rowStart+6, 0+colStart].LoadFromText(getExcelString("MW w/ PBP Class I"));
                if (showClass2) sheet.Cells[rowStart+21 - class1Span, 0+colStart].LoadFromText(getExcelString("MW w/o PBP Class II"));
                if (showClass3) sheet.Cells[rowStart+36 - class1Span - class2Span, 0+colStart].LoadFromText(getExcelString("MW Class III"));
                if (showClassAll) sheet.Cells[rowStart+51, 0+colStart].LoadFromText(getExcelString("No. of MW Submissions Received (Class I, Class II and Class III)"), format);
            }
            if (showVs)
            {
                sheet.Cells[rowStart+52 - mwSpan - classSpan, 0+colStart].LoadFromText(getExcelString("Validation Scheme"));
                sheet.Cells[rowStart+62 - mwSpan - classSpan, 0+colStart].LoadFromText(getExcelString("No. of VS Submissions Received"));
            }
            if (showEnq)
            {
                sheet.Cells[rowStart+63 - mwSpan - classSpan - vsSpan, 0+colStart].LoadFromText(getExcelString("Enquiry"));
            }
            if (showCom)
            {
                sheet.Cells[rowStart+66 - mwSpan - classSpan - vsSpan - enqSpan, 0+colStart].LoadFromText(getExcelString("Complaint"));
            }

            if (showMw)
            {
             
                if (showClass1)
                {
                    sheet.Cells[rowStart+6, 1+colStart].LoadFromText(getExcelString("Notice of Commencement (Form MW01 and MW11)"));
                    sheet.Cells[rowStart+11, 1+colStart].LoadFromText(getExcelString("Certificate of Completion (Form MW02)"));
                    sheet.Cells[rowStart+16, 1+colStart].LoadFromText(getExcelString("Other Forms (MW07, MW08, MW09, MW10, MW31, MW33) Related to Form MW01 and MW02"),format);
                }
                if (showClass2)
                {
                    sheet.Cells[rowStart+21 - class1Span, 1+colStart].LoadFromText(getExcelString("Notice of Commencement (Form MW03 & MW12)"));
                    sheet.Cells[rowStart+26 - class1Span, 1+colStart].LoadFromText(getExcelString("Certificate of Completion (Form MW04)"));
                    sheet.Cells[rowStart+31 - class1Span, 1+colStart].LoadFromText(getExcelString("Other Forms (MW07, MW10, MW33) Related to Form MW03 and MW04"), format);
                }
                if (showClass3)
                {
                    sheet.Cells[rowStart+36 - class1Span - class2Span, 1+colStart].LoadFromText(getExcelString("Notice and Certificate of Completion (Form MW05)"));
                    sheet.Cells[rowStart+41 - class1Span - class2Span, 1+colStart].LoadFromText(getExcelString("Notice and Certificate of Completion (Form MW05(Item3.6))"));
                    sheet.Cells[rowStart+46 - class1Span - class2Span, 1+colStart].LoadFromText(getExcelString("Other Forms (MW07, MW10, MW33) Related to Form MW05"), format);
                }
            }

            if (showVs)
            {
                sheet.Cells[rowStart+52 - mwSpan - classSpan, 1+colStart].LoadFromText(getExcelString("Notice of Completion (Form MW06)"));
                sheet.Cells[rowStart+57 - mwSpan - classSpan, 1+colStart].LoadFromText(getExcelString("Other Forms (MW07, MW10, MW33) Related to Form MW06"), format);
            }



            if (showMw)
            {
                int j = 0;

                if (showClass1) for (int i = 0; i < 3; i++, j++)
                    {
                        sheet.Cells[rowStart+(j * 5) + 6, 2+colStart].LoadFromText(getExcelString("Received"));
                        sheet.Cells[rowStart+(j * 5) + 7, 2+colStart].LoadFromText(getExcelString("Processing"));
                        sheet.Cells[rowStart+(j * 5) + 8, 2+colStart].LoadFromText(getExcelString("Acknowledged"));
                        sheet.Cells[rowStart+(j * 5) + 9, 2+colStart].LoadFromText(getExcelString("Conditional"));
                        sheet.Cells[rowStart+(j * 5) + 10, 2+colStart].LoadFromText(getExcelString("Refused"));
                    }

                if (showClass2) for (int i = 0; i < 3; i++, j++)
                    {
                        sheet.Cells[rowStart+(j * 5) + 6, 2+colStart].LoadFromText(getExcelString("Received"));
                        sheet.Cells[rowStart+(j * 5) + 7, 2+colStart].LoadFromText(getExcelString("Processing"));
                        sheet.Cells[rowStart+(j * 5) + 8, 2+colStart].LoadFromText(getExcelString("Acknowledged"));
                        sheet.Cells[rowStart+(j * 5) + 9, 2+colStart].LoadFromText(getExcelString("Conditional"));
                        sheet.Cells[rowStart+(j * 5) + 10, 2+colStart].LoadFromText(getExcelString("Refused"));
                    }

                if (showClass3) for (int i = 0; i < 3; i++, j++)
                    {
                        sheet.Cells[rowStart+(j * 5) + 6, 2+colStart].LoadFromText(getExcelString("Received"));
                        sheet.Cells[rowStart+(j * 5) + 7, 2+colStart].LoadFromText(getExcelString("Processing"));
                        sheet.Cells[rowStart+(j * 5) + 8, 2+colStart].LoadFromText(getExcelString("Acknowledged"));
                        sheet.Cells[rowStart+(j * 5) + 9, 2+colStart].LoadFromText(getExcelString("Conditional"));
                        sheet.Cells[rowStart+(j * 5) + 10, 2+colStart].LoadFromText(getExcelString("Refused"));
                    }
                sheet.Cells[rowStart+51, 2+colStart].LoadFromText(getExcelString("Counting by DSN"));
            }
            if (showVs)
            {
                for (int i = 0; i < 2; i++)
                {
                    sheet.Cells[rowStart+(i * 5) + 52 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Received"));
                    sheet.Cells[rowStart+(i * 5) + 53 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Processing"));
                    sheet.Cells[rowStart+(i * 5) + 54 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Acknowledged"));
                    sheet.Cells[rowStart+(i * 5) + 55 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Conditional"));
                    sheet.Cells[rowStart+(i * 5) + 56 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Refused"));
                }
                sheet.Cells[rowStart+62 - mwSpan - classSpan, 2+colStart].LoadFromText(getExcelString("Counting by DSN"));
            }
            if (showEnq)
            {
                sheet.Cells[rowStart+63 - mwSpan - classSpan - vsSpan, 2+colStart].LoadFromText(getExcelString("Received"));
                sheet.Cells[rowStart+64 - mwSpan - classSpan - vsSpan, 2+colStart].LoadFromText(getExcelString("Processing"));
                sheet.Cells[rowStart+65 - mwSpan - classSpan - vsSpan, 2+colStart].LoadFromText(getExcelString("Completed"));
            }
            if (showCom)
            {
                sheet.Cells[rowStart+66 - mwSpan - classSpan - vsSpan - enqSpan, 2+colStart].LoadFromText(getExcelString("Received"));
                sheet.Cells[rowStart+67 - mwSpan - classSpan - vsSpan - enqSpan, 2+colStart].LoadFromText(getExcelString("Processing"));
                sheet.Cells[rowStart+68 - mwSpan - classSpan - vsSpan - enqSpan, 2+colStart].LoadFromText(getExcelString("Completed"));
            }
            //reportType
            //classCode


            List<DateTime> dateList = DateUtil.getMonthList(fromDate, toDate);
            for (int i = 0; i < dateList.Count; i++)
            {
                try
                {
                    sheet.Cells[rowStart+5, 3 + i+colStart].LoadFromText(
                        getExcelString(DateUtil.getEnglishMMMMYYYYFormat(dateList[i])+" "), format);
                }
                catch (Exception e) {
                }
            }
            sheet.Cells[rowStart+5, 3 + dateList.Count+colStart].LoadFromText(getExcelString("Total"));
            int[] totals = new int[69];
            int sumClass1 = 0;
            int sumClass2 = 0;
            int sumClass3 = 0;
            if (showMw || showVs)
            {
                Dictionary<String, Dictionary<String, int>> receivedData = DA.getReceivedData(fromDate, toDate);
                Dictionary<String, Dictionary<String, int>> processingData = DA.getProcessingCountData(fromDate, toDate);
                Dictionary<String, Dictionary<String, int>> ackData = DA.getAckData(fromDate, toDate);
                Dictionary<String, Dictionary<String, int>> countData = DA.getCountData(fromDate, toDate);


                for (int i = 0; i < dateList.Count; i++)
                {   String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);

                    if (!receivedData.ContainsKey(mmmYYYDate)) {
                        receivedData.Add(mmmYYYDate , new Dictionary<String, int>());
                     }
                    Dictionary<String, int> dateItem = receivedData[mmmYYYDate];
                    if (showMw)
                    {
                        if (showClass1)
                        {
                            sheet.Cells[rowStart+6, 3 + i+colStart].LoadFromText(getExcelString ( (dateItem.ContainsKey("1") ? dateItem["1"] : 0 )+""));

                            sheet.Cells[rowStart+11, 3 + i+colStart].LoadFromText(getExcelString((dateItem.ContainsKey("2") ? dateItem["2"] : 0) + ""));
                            sheet.Cells[rowStart+16, 3 + i+colStart].LoadFromText(getExcelString((dateItem.ContainsKey("3") ? dateItem["3"] : 0) + ""));
                            totals[6] += dateItem.ContainsKey("1") ? dateItem["1"] : 0;
                            totals[11] += dateItem.ContainsKey("2") ? dateItem["2"] : 0;
                            totals[16] += dateItem.ContainsKey("3") ? dateItem["3"] : 0;
                        }
                        if (showClass2)
                        {
                            sheet.Cells[rowStart+21 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("4") ? dateItem["4"] : 0));
                            sheet.Cells[rowStart+26 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("5") ? dateItem["5"] : 0));
                            sheet.Cells[rowStart+31 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("6") ? dateItem["6"] : 0));
                            totals[21 - class1Span] += dateItem.ContainsKey("4") ? dateItem["4"] : 0;
                            totals[26 - class1Span] += dateItem.ContainsKey("5") ? dateItem["5"] : 0;
                            totals[31 - class1Span] += dateItem.ContainsKey("6") ? dateItem["6"] : 0;
            }
                        if (showClass3)
                        {




                            sheet.Cells[rowStart+36 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("7") ? dateItem["7"] : 0));
                            sheet.Cells[rowStart+41 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0));
                            sheet.Cells[rowStart+46 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("8") ? dateItem["8"] : 0));
                            totals[36 - class1Span - class2Span] += dateItem.ContainsKey("7") ? dateItem["7"] : 0;
                            totals[41 - class1Span - class2Span] += dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0;
                            totals[46 - class1Span - class2Span] += dateItem.ContainsKey("8") ? dateItem["8"] : 0;
                        }
                        if (showClass1)
                        {
                            sumClass1 += dateItem.ContainsKey("1") ? dateItem["1"] : 0;
                            sumClass1 += dateItem.ContainsKey("2") ? dateItem["2"] : 0;
                            sumClass1 += dateItem.ContainsKey("3") ? dateItem["3"] : 0;
                        }
                        if (showClass2)
                        {
                            sumClass2 += dateItem.ContainsKey("4") ? dateItem["4"] : 0;
                            sumClass2 += dateItem.ContainsKey("5") ? dateItem["5"] : 0;
                            sumClass2 += dateItem.ContainsKey("6") ? dateItem["6"] : 0;
                        }
                        if (showClass3)
                        {
                            sumClass3 += dateItem.ContainsKey("7") ? dateItem["7"] : 0;
                            sumClass3 += dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0;
                            sumClass3 += dateItem.ContainsKey("8") ? dateItem["8"] : 0;
                        }
                    }
                    //CLASS_VS
                    if (showVs)
                    {
                        sheet.Cells[rowStart+52 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("9") ? dateItem["9"] : 0));
                        sheet.Cells[rowStart+57 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("10") ? dateItem["10"] : 0));
                        totals[52 - mwSpan - classSpan] += dateItem.ContainsKey("9") ? dateItem["9"] : 0;
                        totals[57 - mwSpan - classSpan] += dateItem.ContainsKey("10") ? dateItem["10"] : 0;
                    }
                }
                if (showClassAll && showMw)
                {
                    sheet.Cells[rowStart+2, 3+colStart].LoadFromText(getExcelString(100 * (1.0 * sumClass1) / (sumClass1 + sumClass2 + sumClass3)));
                    sheet.Cells[rowStart+2, 5+colStart].LoadFromText(getExcelString(100 * (1.0 * sumClass2) / (sumClass1 + sumClass2 + sumClass3)));
                    sheet.Cells[rowStart+2, 7+colStart].LoadFromText(getExcelString(100 * (1.0 * sumClass3) / (sumClass1 + sumClass2 + sumClass3)));
                }

                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);

                    if (!processingData.ContainsKey(mmmYYYDate)) processingData.Add(mmmYYYDate, new Dictionary<String, int>());
                    Dictionary<String, int> dateItem = processingData[mmmYYYDate];
                    if (showMw)
                    {
                        if (showClass1)
                        {
                            sheet.Cells[rowStart+6 + 1, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("1") ? dateItem["1"] : 0));
                            sheet.Cells[rowStart+11 + 1, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("2") ? dateItem["2"] : 0));
                            sheet.Cells[rowStart+16 + 1, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3") ? dateItem["3"] : 0));

                            //Calculate processing total
                            //totals[6+1] = dateItem.ContainsKey("1") ? dateItem["1"] : 0;
                            //totals[11+1] = dateItem.ContainsKey("2") ? dateItem["2"] : 0;
                            //totals[16+1] = dateItem.ContainsKey("3") ? dateItem["3"] : 0;
                            totals[6 + 1] += dateItem.ContainsKey("1") ? dateItem["1"] : 0;
                            totals[11 + 1] += dateItem.ContainsKey("2") ? dateItem["2"] : 0;
                            totals[16 + 1] += dateItem.ContainsKey("3") ? dateItem["3"] : 0;
                        }

                        if (showClass2)
                        {
                            sheet.Cells[rowStart+21 + 1 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("4") ? dateItem["4"] : 0));
                            sheet.Cells[rowStart+26 + 1 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("5") ? dateItem["5"] : 0));
                            sheet.Cells[rowStart+31 + 1 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("6") ? dateItem["6"] : 0));

                            //Calculate processing total
                            //totals[21+1-class1Span] = dateItem.ContainsKey("4") ? dateItem["4"] : 0;
                            //totals[26+1-class1Span] = dateItem.ContainsKey("5") ? dateItem["5"] : 0;
                            //totals[31+1-class1Span] = dateItem.ContainsKey("6") ? dateItem["6"] : 0;
                            totals[21 + 1 - class1Span] += dateItem.ContainsKey("4") ? dateItem["4"] : 0;
                            totals[26 + 1 - class1Span] += dateItem.ContainsKey("5") ? dateItem["5"] : 0;
                            totals[31 + 1 - class1Span] += dateItem.ContainsKey("6") ? dateItem["6"] : 0;
                        }

                        if (showClass3)
                        {
                            sheet.Cells[rowStart+36 + 1 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("7") ? dateItem["7"] : 0));
                            sheet.Cells[rowStart+41 + 1 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0));
                            sheet.Cells[rowStart+46 + 1 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("8") ? dateItem["8"] : 0));

                            //Calculate processing total
                            //totals[36+1-class1Span-class2Span] = dateItem.ContainsKey("7") ? dateItem["7"] : 0;
                            //totals[41+1-class1Span-class2Span] = dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0;
                            //totals[46+1-class1Span-class2Span] = dateItem.ContainsKey("8") ? dateItem["8"] : 0;
                            totals[36 + 1 - class1Span - class2Span] += dateItem.ContainsKey("7") ? dateItem["7"] : 0;
                            totals[41 + 1 - class1Span - class2Span] += dateItem.ContainsKey("3.6") ? dateItem["3.6"] : 0;
                            totals[46 + 1 - class1Span - class2Span] += dateItem.ContainsKey("8") ? dateItem["8"] : 0;
                        }
                    }
                    if (showVs)
                    {
                        //CLASS_VS
                        sheet.Cells[rowStart+52 + 1 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("9") ? dateItem["9"] : 0));
                        sheet.Cells[rowStart+57 + 1 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("10") ? dateItem["10"] : 0));

                        //Calculate processing total
                        //totals[52+1 - mwSpan-classSpan] = dateItem.ContainsKey("9") ? dateItem["9"] : 0;
                        //totals[57+1 - mwSpan-classSpan] = dateItem.ContainsKey("10") ? dateItem["10"] : 0;
                        totals[52 + 1 - mwSpan - classSpan] += dateItem.ContainsKey("9") ? dateItem["9"] : 0;
                        totals[57 + 1 - mwSpan - classSpan] += dateItem.ContainsKey("10") ? dateItem["10"] : 0;
                    }
                }


                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);

                    if (!ackData.ContainsKey(mmmYYYDate)) {
                        ackData.Add(mmmYYYDate, new Dictionary<String, int>());
                    }
                    Dictionary<String, int> dateItem = ackData[mmmYYYDate];
                    if (showMw)
                    {
                        if (showClass1)
                        {
                            sheet.Cells[rowStart+6 + 2, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("1O") ? dateItem["1O"] : 0));
                            sheet.Cells[rowStart+11 + 2, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("2O") ? dateItem["2O"] : 0));
                            sheet.Cells[rowStart+16 + 2, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3O") ? dateItem["3O"] : 0));
                            totals[6 + 2] += dateItem.ContainsKey("1O") ? dateItem["1O"] : 0;
                            totals[11 + 2] += dateItem.ContainsKey("2O") ? dateItem["2O"] : 0;
                            totals[16 + 2] += dateItem.ContainsKey("3O") ? dateItem["3O"] : 0;
                        }

                        if (showClass2)
                        {
                            sheet.Cells[rowStart+21 + 2 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("4O") ? dateItem["4O"] : 0));
                            sheet.Cells[rowStart+26 + 2 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("5O") ? dateItem["5O"] : 0));
                            sheet.Cells[rowStart+31 + 2 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("6O") ? dateItem["6O"] : 0));
                            totals[21 + 2 - class1Span] += dateItem.ContainsKey("4O") ? dateItem["4O"] : 0;
                            totals[26 + 2 - class1Span] += dateItem.ContainsKey("5O") ? dateItem["5O"] : 0;
                            totals[31 + 2 - class1Span] += dateItem.ContainsKey("6O") ? dateItem["6O"] : 0;
                        }

                        if (showClass3)
                        {
                            sheet.Cells[rowStart+36 + 2 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("7O") ? dateItem["7O"] : 0));
                            sheet.Cells[rowStart+41 + 2 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3.6O") ? dateItem["3.6O"] : 0));
                            sheet.Cells[rowStart+46 + 2 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("8O") ? dateItem["8O"] : 0));
                            totals[36 + 2 - class1Span - class2Span] += dateItem.ContainsKey("7O") ? dateItem["7O"] : 0;
                            totals[41 + 2 - class1Span - class2Span] += dateItem.ContainsKey("3.6O") ? dateItem["3.6O"] : 0;
                            totals[46 + 2 - class1Span - class2Span] += dateItem.ContainsKey("8O") ? dateItem["8O"] : 0;
                        }
                    }
                    if (showVs)
                    {
                        sheet.Cells[rowStart+52 + 2 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("9O") ? dateItem["9O"] : 0));
                        sheet.Cells[rowStart+57 + 2 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("10O") ? dateItem["10O"] : 0));
                        totals[52 + 2 - mwSpan - classSpan] += dateItem.ContainsKey("9O") ? dateItem["9O"] : 0;
                        totals[57 + 2 - mwSpan - classSpan] += dateItem.ContainsKey("10O") ? dateItem["10O"] : 0;
                    }
                    if (showMw)
                    {
                        if (showClass1)
                        {
                            sheet.Cells[rowStart+6 + 3, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("1C") ? dateItem["1C"] : 0));
                            sheet.Cells[rowStart+11 + 3, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("2C") ? dateItem["2C"] : 0));
                            sheet.Cells[rowStart+16 + 3, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3C") ? dateItem["3C"] : 0));
                            totals[6 + 3] += dateItem.ContainsKey("1C") ? dateItem["1C"] : 0;
                            totals[11 + 3] += dateItem.ContainsKey("2C") ? dateItem["2C"] : 0;
                            totals[16 + 3] += dateItem.ContainsKey("3C") ? dateItem["3C"] : 0;
                        }

                        if (showClass2)
                        {
                            sheet.Cells[rowStart+21 + 3 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("4C") ? dateItem["4C"] : 0));
                            sheet.Cells[rowStart+26 + 3 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("5C") ? dateItem["5C"] : 0));
                            sheet.Cells[rowStart+31 + 3 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("6C") ? dateItem["6C"] : 0));
                            totals[21 + 3 - class1Span] += dateItem.ContainsKey("4C") ? dateItem["4C"] : 0;
                            totals[26 + 3 - class1Span] += dateItem.ContainsKey("5C") ? dateItem["5C"] : 0;
                            totals[31 + 3 - class1Span] += dateItem.ContainsKey("6C") ? dateItem["6C"] : 0;
                        }

                        if (showClass3)
                        {
                            sheet.Cells[rowStart+36 + 3 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("7C") ? dateItem["7C"] : 0));
                            sheet.Cells[rowStart+41 + 3 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3.6C") ? dateItem["3.6C"] : 0));
                            sheet.Cells[rowStart+46 + 3 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("8C") ? dateItem["8C"] : 0));
                            totals[36 + 3 - class1Span - class2Span] += dateItem.ContainsKey("7C") ? dateItem["7C"] : 0;
                            totals[41 + 3 - class1Span - class2Span] += dateItem.ContainsKey("3.6C") ? dateItem["3.6C"] : 0;
                            totals[46 + 3 - class1Span - class2Span] += dateItem.ContainsKey("8C") ? dateItem["8C"] : 0;
                        }
                    }
                    if (showVs)
                    {
                        sheet.Cells[rowStart+52 + 3 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("9C") ? dateItem["9C"] : 0));
                        sheet.Cells[rowStart+57 + 3 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("10C") ? dateItem["10C"] : 0));
                        totals[52 + 3 - mwSpan - classSpan] += dateItem.ContainsKey("9C") ? dateItem["9C"] : 0;
                        totals[57 + 3 - mwSpan - classSpan] += dateItem.ContainsKey("10C") ? dateItem["10C"] : 0;
                    }
                    if (showMw)
                    {
                        if (showClass1)
                        {
                            sheet.Cells[rowStart+6 + 4, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("1N") ? dateItem["1N"] : 0));
                            sheet.Cells[rowStart+11 + 4, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("2N") ? dateItem["2N"] : 0));
                            sheet.Cells[rowStart+16 + 4, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3N") ? dateItem["3N"] : 0));
                            totals[6 + 4] += dateItem.ContainsKey("1N") ? dateItem["1N"] : 0;
                            totals[11 + 4] += dateItem.ContainsKey("2N") ? dateItem["2N"] : 0;
                            totals[16 + 4] += dateItem.ContainsKey("3N") ? dateItem["3N"] : 0;
                        }

                        if (showClass2)
                        {
                            sheet.Cells[rowStart+21 + 4 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("4N") ? dateItem["4N"] : 0));
                            sheet.Cells[rowStart+26 + 4 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("5N") ? dateItem["5N"] : 0));
                            sheet.Cells[rowStart+31 + 4 - class1Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("6N") ? dateItem["6N"] : 0));
                            totals[21 + 4 - class1Span] += dateItem.ContainsKey("4N") ? dateItem["4N"] : 0;
                            totals[26 + 4 - class1Span] += dateItem.ContainsKey("5N") ? dateItem["5N"] : 0;
                            totals[31 + 4 - class1Span] += dateItem.ContainsKey("6N") ? dateItem["6N"] : 0;
                        }

                        if (showClass3)
                        {
                            sheet.Cells[rowStart+36 + 4 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("7N") ? dateItem["7N"] : 0));
                            sheet.Cells[rowStart+41 + 4 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("3.6N") ? dateItem["3.6N"] : 0));
                            sheet.Cells[rowStart+46 + 4 - class1Span - class2Span, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("8N") ? dateItem["8N"] : 0));
                            totals[36 + 4 - class1Span - class2Span] += dateItem.ContainsKey("7N") ? dateItem["7N"] : 0;
                            totals[41 + 4 - class1Span - class2Span] += dateItem.ContainsKey("3.6N") ? dateItem["3.6N"] : 0;
                            totals[46 + 4 - class1Span - class2Span] += dateItem.ContainsKey("8N") ? dateItem["8N"] : 0;
                        }
                    }
                    if (showVs)
                    {
                        sheet.Cells[rowStart+52 + 4 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("9N") ? dateItem["9N"] : 0));
                        sheet.Cells[rowStart+57 + 4 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("10N") ? dateItem["10N"] : 0));
                        totals[52 + 4 - mwSpan - classSpan] += dateItem.ContainsKey("9N") ? dateItem["9N"] : 0;
                        totals[57 + 4 - mwSpan - classSpan] += dateItem.ContainsKey("10N") ? dateItem["10N"] : 0;
                    }
                }
                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);

                    if (!countData.ContainsKey(mmmYYYDate)) countData.Add(mmmYYYDate, new Dictionary<String, int>());
                    Dictionary<String, int> dateItem = countData[mmmYYYDate];
                    if (showMw)
                    {
                        sheet.Cells[rowStart+51, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("MW") ? dateItem["MW"] : 0));
                        totals[51] += dateItem.ContainsKey("MW") ? dateItem["MW"] : 0;
                    }
                    if (showVs)
                    {
                        sheet.Cells[rowStart+62 - mwSpan - classSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("VS") ? dateItem["VS"] : 0));
                        totals[62 - mwSpan - classSpan] += dateItem.ContainsKey("VS") ? dateItem["VS"] : 0;
                    }
                }
            }



            if (showEnq || showCom)
            {
                Dictionary<String, Dictionary<String, int>> generalReceivedData = DA.getGeneralReceivedCountData(fromDate, toDate);
                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);

                    if (!generalReceivedData.ContainsKey(mmmYYYDate)){
                        generalReceivedData.Add(mmmYYYDate, new Dictionary<String, int>());
                    }

                    Dictionary<String, int> dateItem = generalReceivedData[mmmYYYDate];
                    if (showEnq)
                    {
                        sheet.Cells[rowStart+63 - mwSpan - classSpan - vsSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0));
                        totals[63 - mwSpan - classSpan - vsSpan] += dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0;
                    }
                    if (showCom)
                    {
                        sheet.Cells[rowStart+66 - mwSpan - classSpan - vsSpan - enqSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Com") ? dateItem["Com"] : 0));
                        totals[66 - mwSpan - classSpan - vsSpan - enqSpan] += dateItem.ContainsKey("Com") ? dateItem["Com"] : 0;
                    }
                }

                Dictionary<String, Dictionary<String, int>> generalProcessingData = DA.getGeneralProcessingCountData(fromDate, toDate);
                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);
                    if (!generalProcessingData.ContainsKey(mmmYYYDate)) {
                        generalProcessingData.Add(mmmYYYDate, new Dictionary<String, int>());
                    }
                    Dictionary<String, int> dateItem = generalProcessingData[mmmYYYDate];
                    if (showEnq)
                    {
                        sheet.Cells[rowStart+64 - mwSpan - classSpan - vsSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0));
                        //Calculate processing total
                        //totals[64 - mwSpan-classSpan - vsSpan] = dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0;
                        totals[64 - mwSpan - classSpan - vsSpan] += dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0;
                    }
                    if (showCom)
                    {
                        sheet.Cells[rowStart+67 - mwSpan - classSpan - vsSpan - enqSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Com") ? dateItem["Com"] : 0));

                        //Calculate processing total
                        //totals[67 - mwSpan-classSpan - vsSpan - enqSpan] = dateItem.ContainsKey("Com") ? dateItem["Com"] : 0;
                        totals[67 - mwSpan - classSpan - vsSpan - enqSpan] += dateItem.ContainsKey("Com") ? dateItem["Com"] : 0;
                    }
                }

                Dictionary<String, Dictionary<String, int>> generalCompletedData = DA.getGeneralProcessingCountData(fromDate, toDate);
                for (int i = 0; i < dateList.Count; i++)
                {
                    String mmmYYYDate = DateUtil.getYYYYMMFormat(dateList[i]);
                    if (!generalCompletedData.ContainsKey(mmmYYYDate)) {
                        generalCompletedData.Add(mmmYYYDate, new Dictionary<String, int>());
                    }
                    Dictionary<String, int> dateItem = generalCompletedData[mmmYYYDate];
                    if (showEnq)
                    {
                        sheet.Cells[rowStart+65 - mwSpan - classSpan - vsSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0));
                        totals[65 - mwSpan - classSpan - vsSpan] += dateItem.ContainsKey("Enq") ? dateItem["Enq"] : 0;
                    }
                    if (showCom)
                    {
                        sheet.Cells[rowStart+68 - mwSpan - classSpan - vsSpan - enqSpan, 3 + i+colStart].LoadFromText(getExcelString(dateItem.ContainsKey("Com") ? dateItem["Com"] : 0));
                        totals[68 - mwSpan - classSpan - vsSpan - enqSpan] += dateItem.ContainsKey("Com") ? dateItem["Com"] : 0;
                    }
                }
            }
        
            for (int i = 0; i < totalRow; i++)
            {
                sheet.Cells[rowStart+6 + i, 3 + dateList.Count()+colStart].LoadFromText(getExcelString(totals[6 + i]));
            }


            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            return fsr;
        }
    }

    
}