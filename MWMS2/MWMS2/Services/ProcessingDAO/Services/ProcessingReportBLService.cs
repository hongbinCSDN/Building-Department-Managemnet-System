using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingReportBLService
    {
        private ProcessingReportDAOService _DA;

        public ProcessingReportDAOService DA
        {
            get { return _DA ?? (_DA = new ProcessingReportDAOService()); }
        }

        string SearchRPT_PPJL_q = " SELECT ap.ENGLISH_NAME || ', ' || ap.CHINESE_NAME AS NAME, "
            + " \r\n\t ap.CERTIFICATION_NO AS CERT_NO, refno.REFERENCE_NO AS MW_NO, "
            + " \r\n\t CASE WHEN (record.COMMENCEMENT_DATE <= (SELECT SYSDATE FROM dual) AND record.COMPLETION_DATE IS NULL) THEN 'In Progress' "
            + " \r\n\t WHEN record.COMPLETION_DATE <= (SELECT SYSDATE FROM dual) THEN 'Completed' END AS STATUS, "
            + " \r\n\t CASE WHEN record.LANGUAGE_CODE = 'EN' THEN(record.LOCATION_OF_MINOR_WORK || ', ' || addr.ENGLISH_DISPLAY) "
            + " \r\n\t WHEN record.LANGUAGE_CODE = 'ZH' THEN (record.LOCATION_OF_MINOR_WORK || ', ' || addr.CHINESE_DISPLAY) END AS ADDRESS "
            + " \r\n\t FROM P_MW_RECORD record "
            + " \r\n\t LEFT JOIN P_MW_APPOINTED_PROFESSIONAL ap ON ap.MW_RECORD_ID = record.UUID "
            + " \r\n\t LEFT JOIN P_MW_REFERENCE_NO REFNO ON refno.UUID = record.REFERENCE_NUMBER "
            + " \r\n\t LEFT JOIN P_MW_ADDRESS ADDR ON addr.UUID = record.LOCATION_ADDRESS_ID "
            + " \r\n\t WHERE 1=1 "
            //+ " \r\n\t AND ROWNUM <= 5"
            ;

        public Fn10RPT_PPJLModel SearchRPT_PPJL(Fn10RPT_PPJLModel model)
        {
            model.Query = SearchRPT_PPJL_q;
            model.QueryWhere = SearchRPT_PPJL_whereQ(model);
            model.Search();
            return model;
        }

        public string SearchRPT_PPJL_whereQ(Fn10RPT_PPJLModel model)
        {
            string whereQ = "";

            whereQ += " \r\n\t AND IS_DATA_ENTRY = 'N' ";

            if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Submission_Type))
            {
                whereQ += " \r\n\t AND refno.REFERENCE_NO LIKE :PREFIX";
                model.QueryParameters.Add("PREFIX", model.Fn10RPT_PPJLSearchModel.Submission_Type + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Status))
            {
                if (ProcessingConstant.RPT_STATUS_IN_PROGRESS.Equals(model.Status))
                {
                    whereQ += " \r\n\t AND record.COMMENCEMENT_DATE <= (SELECT SYSDATE FROM dual) ";
                    whereQ += " \r\n\t AND record.COMPLETION_DATE IS NULL ";
                }
                else if (ProcessingConstant.RPT_STATUS_COMPLETED.Equals(model.Status))
                {
                    whereQ += " \r\n\t AND record.COMPLETION_DATE <= (SELECT SYSDATE FROM dual) ";
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PBP)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PBP)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PBP)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PRC))
            {
                whereQ += " \r\n\t AND (( ";
                if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PBP)
                    || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PBP)
                    || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PBP))
                {
                    whereQ += " \r\n\t ap.IDENTIFY_FLAG in ('AP', 'RSE' ,'RGE', 'RI') ";
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PBP))
                    {
                        whereQ += " \r\n\t AND ap.CERTIFICATION_NO LIKE :PBP_REG_NO ";
                        model.QueryParameters.Add("PBP_REG_NO", "%" + model.Fn10RPT_PPJLSearchModel.Registration_No_of_PBP + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PBP))
                    {
                        whereQ += " \r\n\t AND lower(ap.ENGLISH_NAME) LIKE :PBP_ENG_NAME";
                        model.QueryParameters.Add("PBP_ENG_NAME", "%" + model.Fn10RPT_PPJLSearchModel.English_Name_of_PBP.ToLower() + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PBP))
                    {
                        whereQ += " \r\n\t AND lower(ap.CHINESE_NAME) LIKE :PBP_CHIN_NAME";
                        model.QueryParameters.Add("PBP_CHIN_NAME", "%" + model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PBP.ToLower() + "%");
                    }
                }
                if ((!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PBP)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PBP)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PBP))
                && (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PRC)))
                {
                    whereQ += " \r\n\t ) OR ( ";
                }
                if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PRC)
                || !string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PRC))
                {
                    whereQ += " \r\n\t ap.IDENTIFY_FLAG = 'PRC' ";
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Registration_No_of_PRC))
                    {
                        whereQ += "\r\n\t AND ap.CERTIFICATION_NO LIKE :PRC_REG_NO";
                        model.QueryParameters.Add("PRC_REG_NO", "%" + model.Fn10RPT_PPJLSearchModel.Registration_No_of_PRC + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.English_Name_of_PRC))
                    {
                        whereQ += " \r\n\t AND lower(ap.ENGLISH_NAME) LIKE :PRC_ENG_NAME ";
                        model.QueryParameters.Add("PRC_ENG_NAME", "%" + model.Fn10RPT_PPJLSearchModel.English_Name_of_PRC.ToLower() + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PRC))
                    {
                        whereQ += " \r\n\t AND lower(ap.CHINESE_NAME) LIKE :PRC_CHIN_NAME ";
                        model.QueryParameters.Add("PRC_CHIN_NAME", "%" + model.Fn10RPT_PPJLSearchModel.Chinese_Name_of_PRC.ToLower() + "%");
                    }
                }
                whereQ += " \r\n\t ))";
            }
            // order by
            return whereQ;
        }

        public string ExportRPT_PPJL(Fn10RPT_PPJLModel model)
        {
            model.Query = SearchRPT_PPJL_q;
            model.QueryWhere = SearchRPT_PPJL_whereQ(model);

            Dictionary<string, string> col1 = model.CreateExcelColumn("NAME", "NAME");
            Dictionary<string, string> col2 = model.CreateExcelColumn("CERT NO", "CERT_NO");
            Dictionary<string, string> col3 = model.CreateExcelColumn("MW NO", "MW_NO");
            Dictionary<string, string> col4 = model.CreateExcelColumn("STATUS", "STATUS");
            Dictionary<string, string> col5 = model.CreateExcelColumn("ADDRESS", "ADDRESS");
            model.Columns = new Dictionary<string, string>[] { col1, col2, col3, col4, col5 };
            model.Rpp = 1048575;
            model.Search();
            //return model.Export("Summary_of_PBP_and_PRC_Job_List");
            return model.ExportCurrentData("Summary_of_PBP_and_PRC_Job_List");
        }

        private string GetIncompletedRecord_Whereq(Fn10RPT_MWMSWCCModel model, List<string> status, List<string> mw0103CodeList)
        {
            StringBuilder whereq = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.ReceivedFromDate) || !string.IsNullOrWhiteSpace(model.ReceivedToDate))
            {
                whereq.Append(@" INNER JOIN P_MW_FORM t4 ON t0.UUID = t4.MW_RECORD_ID ");
            }
            else
            {
                whereq.Append(@" LEFT JOIN P_MW_FORM t4 ON t0.UUID = t4.MW_RECORD_ID ");
            }
            whereq.Append(@" WHERE t0.COMMENCEMENT_DATE IS NOT NULL ");

            if (!string.IsNullOrWhiteSpace(model.ReceivedFromDate))
            {
                whereq.Append(@" AND TO_DATE(TO_CHAR(t4.RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') >= TO_DATE(:dateReceivedFrom,'dd/MM/yyyy') ");
                model.QueryParameters.Add("dateReceivedFrom", model.ReceivedFromDate);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedToDate))
            {
                whereq.Append(@" AND TO_DATE(TO_CHAR(t4.RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') <= TO_DATE(:dateReceivedTo,'dd/MM/yyyy') ");
                model.QueryParameters.Add("dateReceivedTo", model.ReceivedToDate);
            }
            whereq.Append(@" AND t2.RECOMMEDATION_APPLICATION IN (:status) ");
            //model.QueryParameters.Add("status", string.Join(",", status.ToArray()));
            model.QueryParameters.Add("status", status);

            if (model.PRCReg != null && !string.IsNullOrWhiteSpace(model.PRCReg.Trim()))
            {
                whereq.Append(@" AND t3PRC.IDENTIFY_FLAG = 'PRC' AND UPPER(t3PRC.CERTIFICATION_NO) LIKE :prc ");
                model.QueryParameters.Add("prc", model.PRCReg);
            }
            if (model.PBPReg != null && !string.IsNullOrWhiteSpace(model.PBPReg.Trim()))
            {
                whereq.Append(@" AND t3PBP.IDENTIFY_FLAG IN ('RSE', 'RGE', 'AP', 'RI') AND UPPER(t3PBP.CERTIFICATION_NO) LIKE :pbp ");
                model.QueryParameters.Add("pbp", model.PBPReg);
            }
            if (!string.IsNullOrWhiteSpace(model.CommFromDate))
            {
                whereq.Append(@" AND TO_DATE(TO_CHAR(t0.COMMENCEMENT_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') >= TO_DATE(:dateFrom,'dd/MM/yyyy') ");
                model.QueryParameters.Add("dateFrom", model.CommFromDate);
            }
            if (!string.IsNullOrWhiteSpace(model.CommToDate))
            {
                whereq.Append(@" AND TO_DATE(TO_CHAR(t0.COMMENCEMENT_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') <= TO_DATE(:dateTo,'dd/MM/yyyy') ");
                model.QueryParameters.Add("dateTo", model.CommToDate);
            }
            whereq.Append(@" AND 
                             (SELECT COUNT(R.UUID)
                             FROM P_MW_RECORD R WHERE IS_DATA_ENTRY = 'N' and R.completion_Date is null
                             and R.REFERENCE_NUMBER= t0.REFERENCE_NUMBER ) > 0 ");
            whereq.Append(@" and t0.S_FORM_TYPE_CODE in ( :MW0103CodeList ) ");
            //model.QueryParameters.Add("MW0103CodeList", string.Join(",", mw0103CodeList.ToArray()));
            model.QueryParameters.Add("MW0103CodeList", mw0103CodeList);
            //whereq.Append(@" ORDER BY 4 DESC ,1,3,2 ");
            return whereq.ToString();
        }
        public Fn10RPT_MWMSWCCModel GetIncompletedRecord(Fn10RPT_MWMSWCCModel model)
        {
            List<string> status = new List<string>();
            if (((model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_ACKNOWLEDGED && m.IsChecked == false).Count() > 0)
                && (model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_REFUSED && m.IsChecked == false).Count() > 0))
                || model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_ACKNOWLEDGED && m.IsChecked == true).Count() > 0)
            {
                status.Add(ProcessingConstant.NOTIFICATION_VALID);
            }

            if (((model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_ACKNOWLEDGED && m.IsChecked == false).Count() > 0)
                && (model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_REFUSED && m.IsChecked == false).Count() > 0))
                || model.StatusFormMW0204.Where(m => m.Code == ProcessingConstant.CHECKBOX_REFUSED && m.IsChecked == true).Count() > 0)
            {
                status.Add(ProcessingConstant.NOTIFICATION_CONDITIONAL);
                status.Add(ProcessingConstant.NOTIFICATION_REFUSAL);
            }

            List<string> mw0103CodeList = new List<string>();
            if (!model.FormTypeMW01 && !model.FormTypeMW03)
            {
                mw0103CodeList.Add(ProcessingConstant.FORM_01);
                mw0103CodeList.Add(ProcessingConstant.FORM_03);
            }
            else
            {
                if (model.FormTypeMW01)
                {
                    mw0103CodeList.Add(ProcessingConstant.FORM_01);
                }
                if (model.FormTypeMW03)
                {
                    mw0103CodeList.Add(ProcessingConstant.FORM_03);
                }
            }

            List<string> mw0204CodeList = new List<string>();
            mw0204CodeList.Add(ProcessingConstant.FORM_02);
            mw0204CodeList.Add(ProcessingConstant.FORM_04);
            //model.QueryParameters.Add("MW0204CodeList", string.Join(",", mw0204CodeList.ToArray()));
            model.QueryParameters.Add("MW0204CodeList", mw0204CodeList);
            model.QueryParameters.Add("ResultAck", ProcessingConstant.NOTIFICATION_VALID);

            model.QueryWhere = GetIncompletedRecord_Whereq(model, status, mw0103CodeList);
            model.Sort = "4";
            model.SortType = 1;
            DA.GetIncompletedRecord(model);

            return model;
        }

        #region Fn10RPT_MWMSWCC
        public IWorkbook Fn10RPT_MWMSWCC_Workbook(Fn10RPT_MWMSWCCModel model)
        {
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet("Report");
            SetColWidth(sheet);
            int datatablePosition = 0;
            datatablePosition = CreateFn10RPT_MWMSWCCExcelSeachCriteria(model, wb, sheet, datatablePosition);


            //List<Dictionary<string, object>> dataRows = FakeFn10RPT_MWSWCC.GetIncompletedRecord_Fake();
            List<Dictionary<string, object>> dataRows = GetIncompletedRecord(model).Data;

            int lastGroup = 12, interval = 3, per = 0, current = lastGroup;
            while (current >= 0)
            {
                int groupCount = 0;
                IRow rowTimeElapsed1 = sheet.CreateRow(datatablePosition);
                SetCellMergedRegion(sheet, datatablePosition);
                datatablePosition++;

                GetDataHeader(sheet.CreateRow(datatablePosition));
                datatablePosition++;

                foreach (var item in dataRows)
                {
                    int M = Convert.ToInt32(item["MONTH_DIFF"]);
                    if ((per == 0 || M <= per) && M >= current && GetMW0204Status(item, model) != "")
                    {
                        IRow dataRow = sheet.CreateRow(datatablePosition);

                        CreateExcelDataCell(dataRow, 0, GetCellStyle(wb), item["S_FORM_TYPE_CODE"].ToString());
                        CreateExcelDataCell(dataRow, 1, GetCellStyle(wb), item["REFERENCE_NO"].ToString());
                        CreateExcelDataCell(dataRow, 2, GetCellStyle(wb), Convert.ToDateTime(item["RECEIVED_DATE"]).ToString("dd/MM/yyyy"));
                        CreateExcelDataCell(dataRow, 3, GetCellStyle(wb), Convert.ToDateTime(item["COMMENCEMENT_DATE"]).ToString("dd/MM/yyyy"));
                        CreateExcelDataCell(dataRow, 4, GetCellStyle(wb), GetMW0204Status(item, model));

                        datatablePosition++;
                        groupCount++;
                    }
                }

                CreateExcelDataCell(rowTimeElapsed1, 0, GetCellStyle(wb), GetHeaderTimeElapsed(per, current) + GetGroupCount(groupCount));

                per = current;
                current = current - interval;
                datatablePosition++;
            }
            return wb;
        }
        public Fn10RPT_MWMSWCCModel Fn10RPT_MWMSWCC_Search(Fn10RPT_MWMSWCCModel model)
        {
            model = GetIncompletedRecord(model);


            int cnt = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("({\"rows\":[");
            foreach (var item in model.Data)
            {
                int mw0204Counter = Convert.ToInt32(item["MW0204_COUNTER"]);
                int mw0204OkCounter = Convert.ToInt32(item["MW0204_OK_COUNTER"]);
                int mw0204NotOkCounter = Convert.ToInt32(item["MW0204_NOT_OK_COUNTER"]);

                string mw0204Status = "";
                if (mw0204Counter > 0)
                {
                    if (mw0204OkCounter > 0)
                    {
                        if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_ACKNOWLEDGED).IsChecked)
                        {
                            continue;
                        }
                        mw0204Status = ProcessingConstant.CHECKBOX_ACKNOWLEDGED;
                    }
                    else if (mw0204NotOkCounter > 0)
                    {
                        if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_REFUSED).IsChecked)
                        {
                            continue;
                        }
                        mw0204Status = ProcessingConstant.CHECKBOX_REFUSED;
                    }
                    else
                    {
                        if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_PROCESSING).IsChecked)
                        {
                            continue;
                        }
                        mw0204Status = ProcessingConstant.CHECKBOX_PROCESSING;
                    }
                }
                else
                {
                    if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_NOSUBMISSION).IsChecked)
                    {
                        continue;
                    }
                    mw0204Status = ProcessingConstant.CHECKBOX_NOSUBMISSION;
                }

                if (cnt > 0)
                {
                    sb.Append(",");
                }
                sb.Append("{");
                sb.Append("\"F\":\""); sb.Append(item["S_FORM_TYPE_CODE"]); sb.Append("\"");
                sb.Append(",");
                sb.Append("\"R\":\""); sb.Append(item["REFERENCE_NO"]); sb.Append("\"");
                sb.Append(",");
                sb.Append("\"C\":\""); sb.Append(item["COMMENCEMENT_DATE"]); sb.Append("\"");
                sb.Append(",");
                sb.Append("\"M\":\""); sb.Append(item["MONTH_DIFF"]); sb.Append("\"");
                sb.Append(",");
                sb.Append("\"RD\":\""); sb.Append(item["RECEIVED_DATE"]); sb.Append("\"");
                sb.Append(",");
                sb.Append("\"MW0204\":\""); sb.Append(mw0204Status); sb.Append("\"");
                sb.Append("}");

                cnt++;
            }
            sb.Append("]})");

            model.Data = new List<Dictionary<string, object>>();
            Dictionary<string, object> rows = new Dictionary<string, object>();
            rows.Add("rows", sb.ToString());
            model.Data.Add(rows);

            return model;
        }

        private void CreateExcelDataCell(IRow dataRow, int cellIndex, ICellStyle cellStyle, string cellValue)
        {
            ICell cell = dataRow.CreateCell(cellIndex);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(cellValue);
        }

        private int CreateFn10RPT_MWMSWCCExcelSeachCriteria(Fn10RPT_MWMSWCCModel model, IWorkbook wb, ISheet sheet, int datatablePosition)
        {
            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, "Submission Location Report");
            datatablePosition += 2;

            //Search criteria input
            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, "Search criteria input");
            datatablePosition++;

            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("Commencement Date: {0} -> {1} ", model.CommFromDate, model.CommToDate));
            datatablePosition++;

            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("Received Date: {0} -> {1} ", model.ReceivedFromDate, model.ReceivedToDate));
            datatablePosition++;

            string formType = (model.FormTypeMW01 ? "MW01" : "") + ((model.FormTypeMW01 && model.FormTypeMW03) ? "," : "") + (model.FormTypeMW03 ? "MW03" : "");
            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("Form Type: {0} ", formType));
            datatablePosition++;

            string status = "";
            foreach (var statu in model.StatusFormMW0204)
            {
                if (!string.IsNullOrWhiteSpace(status) && statu.IsChecked)
                {
                    status += ",";
                }
                status += (statu.IsChecked ? statu.Code : "");
            }
            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("Status: {0} ", status));
            datatablePosition++;

            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("PBP Reg.: {0} ", model.PBPReg));
            datatablePosition++;

            CreateExcelCriteriaRow(sheet, GetCellStyle(wb), datatablePosition, string.Format("PRC Reg.: {0} ", model.PRCReg));
            datatablePosition += 2;

            return datatablePosition;
        }

        private void CreateExcelCriteriaRow(ISheet sheet, ICellStyle cellStyle, int datatablePosition, string cellValue)
        {
            IRow row = sheet.CreateRow(datatablePosition);
            CreateExcelDataCell(row, 0, cellStyle, cellValue);
            SetCellMergedRegion(sheet, datatablePosition);
        }

        private ICellStyle GetCellStyle(IWorkbook wb)
        {
            ICellStyle cellStyle = wb.CreateCellStyle();
            IFont font = wb.CreateFont();
            font.FontName = "Arial";
            font.FontHeightInPoints = 10;
            cellStyle.SetFont(font);
            return cellStyle;
        }

        private string GetMW0204Status(Dictionary<string, object> item, Fn10RPT_MWMSWCCModel model)
        {

            int mw0204Counter = Convert.ToInt32(item["MW0204_COUNTER"]);
            int mw0204OkCounter = Convert.ToInt32(item["MW0204_OK_COUNTER"]);
            int mw0204NotOkCounter = Convert.ToInt32(item["MW0204_NOT_OK_COUNTER"]);

            string mw0204Status = "";
            if (mw0204Counter > 0)
            {
                if (mw0204OkCounter > 0)
                {
                    if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_ACKNOWLEDGED).IsChecked)
                    {
                        return mw0204Status;
                    }
                    mw0204Status = ProcessingConstant.CHECKBOX_ACKNOWLEDGED;
                }
                else if (mw0204NotOkCounter > 0)
                {
                    if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_REFUSED).IsChecked)
                    {
                        return mw0204Status;
                    }
                    mw0204Status = ProcessingConstant.CHECKBOX_REFUSED;
                }
                else
                {
                    if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_PROCESSING).IsChecked)
                    {
                        return mw0204Status;
                    }
                    mw0204Status = ProcessingConstant.CHECKBOX_PROCESSING;
                }
            }
            else
            {
                if (!model.StatusFormMW0204.Find(m => m.Code == ProcessingConstant.CHECKBOX_NOSUBMISSION).IsChecked)
                {
                    return mw0204Status;
                }
                mw0204Status = ProcessingConstant.CHECKBOX_NOSUBMISSION;
            }
            return mw0204Status;
        }
        private void SetCellMergedRegion(ISheet sheet, int row)
        {
            CellRangeAddress cellRangeAddress = new CellRangeAddress(row, row, 0, 4);
            sheet.AddMergedRegion(cellRangeAddress);
        }
        private string GetHeaderTimeElapsed(int start, int end)
        {
            return "Time elapsed from the Date of Commencement: " + (start == 0 ? "" : start + " months>X") + ">" + (end == 0 ? " current" : end + " months");
        }
        private string GetGroupCount(int groupCount)
        {
            return " (No. of Submissions: " + groupCount + ")";
        }
        private IRow GetDataHeader(IRow row)
        {
            ICell cellFormType = row.CreateCell(0);
            cellFormType.SetCellValue("Form Type");
            ICell cellMWNo = row.CreateCell(1);
            cellMWNo.SetCellValue("MW No.");
            ICell cellReceiveDate = row.CreateCell(2);
            cellReceiveDate.SetCellValue("Date of Receive");
            ICell cellCommDate = row.CreateCell(3);
            cellCommDate.SetCellValue("Date of Commencement");
            ICell cellStatus = row.CreateCell(4);
            cellStatus.SetCellValue("Status of Corresponding Form MW02/MW04");

            return row;
        }

        public string getExcelString(object stringValue)
        {
            return @stringValue.ToString();

        }

        private void SetColWidth(ISheet sheet)
        {
            sheet.SetColumnWidth(0, 18 * 256);
            sheet.SetColumnWidth(1, 18 * 256);
            sheet.SetColumnWidth(2, 18 * 256);
            sheet.SetColumnWidth(3, 18 * 256);
            sheet.SetColumnWidth(4, 18 * 256);
        }
        #endregion

        #region Fn10RPT_SLR
        public string Search_where(Fn10RPT_SLRModel model)
        {
            StringBuilder where_q = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.SortBy))
            {
                model.Sort = model.SortBy;
            }

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                where_q.Append(@" AND RN.REFERENCE_NO like :RefNo ");
                model.QueryParameters.Add("RefNo", "%" + model.RefNo + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                where_q.Append(@" AND R.MW_DSN like :DSN ");
                model.QueryParameters.Add("DSN", "%" + model.DSN + "%");
            }

            return where_q.ToString();
        }

        public Fn10RPT_SLRModel Fn10RPT_SLRSearch(Fn10RPT_SLRModel model)
        {
            model.QueryWhere = Search_where(model);
            DA.Fn10RPT_SLRSearch(model);
            return model;
        }

        public string Fn10RPT_SLRWorkbook(Fn10RPT_SLRModel model)
        {
            model = Fn10RPT_SLRSearch(model);
            //model.Data = new List<Dictionary<string, object>>();
            //Dictionary<string, object> data1 = new Dictionary<string, object>();
            //data1.Add("TASK_DATE", "2019-08-09");
            //data1.Add("TASK_TIME", "2019-08-09");
            //data1.Add("MW_DSN", "2019-08-09");
            //data1.Add("REFERENCE_NO", "asfa");
            //data1.Add("TASK_CODE", "as9");
            //data1.Add("BD_PORTAL_LOGIN", "asf");
            //model.Data.Add(data1);


            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");

            int rowIdx = 1;
            sheet.Cells[rowIdx, 1].LoadFromText("Submission Location Report");
            MergeCells(rowIdx, sheet, model);
            rowIdx += 2;

            sheet.Cells[rowIdx, 1].LoadFromText("Search criteria input");
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            string sortBy = "";
            switch (model.SortBy)
            {
                case ProcessingConstant.SLR_TASK_DATE_TIME_VAL:
                    sortBy = ProcessingConstant.SLR_TASK_DATE_TIME_TEXT;
                    break;
                case ProcessingConstant.SLR_MW_DSN_VAL:
                    sortBy = ProcessingConstant.SLR_MW_DSN_TEXT;
                    break;
                case ProcessingConstant.SLR_REF_NO_VAL:
                    sortBy = ProcessingConstant.SLR_REF_NO_TEXT;
                    break;
                case ProcessingConstant.SLR_ACTIVITY_VAL:
                    sortBy = ProcessingConstant.SLR_ACTIVITY_TEXT;
                    break;
                default:
                    sortBy = "All";
                    break;
            }
            sheet.Cells[rowIdx, 1].LoadFromText("Sort by: " + sortBy);
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Ref. No.: " + model.RefNo);
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("DSN Number: " + model.DSN);
            MergeCells(rowIdx, sheet, model);
            rowIdx += 2;

            return model.ExportWithCriteria(ep, sheet, rowIdx, "Submission_Location_Report");
        }

        private ExcelWorksheet MergeCells(int rowIdx, ExcelWorksheet sheet, DisplayGrid model)
        {
            string mergePosition = "A" + rowIdx + ":" + model.ExcelColumnFromNumber(model.Columns.Length) + rowIdx.ToString();
            sheet.Cells[mergePosition].Merge = true;
            return sheet;
        }
        #endregion

        #region Fn10RPT_SFMW
        public string SearchSFMW_where(Fn10RPT_SFMWModel model)
        {
            StringBuilder where_q = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) || !string.IsNullOrWhiteSpace(model.ReceivedDateaTo))
            {
                if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && string.IsNullOrWhiteSpace(model.ReceivedDateaTo))
                {
                    where_q.Append(@" and to_date(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')>=to_date(:receivedFrom,'dd/MM/yyyy') ");
                    model.QueryParameters.Add(":receivedFrom", model.ReceivedDateFrom);
                }

                if (!string.IsNullOrWhiteSpace(model.ReceivedDateaTo) && string.IsNullOrWhiteSpace(model.ReceivedDateFrom))
                {
                    where_q.Append(@" and to_date(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=to_date(:receivedTo,'dd/MM/yyyy') ");
                    model.QueryParameters.Add(":receivedTo", model.ReceivedDateaTo);
                }

                if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateaTo))
                {
                    where_q.Append(@" and to_date(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') between to_date(:receivedFrom,'dd/MM/yyyy') and to_date(:receivedTo,'dd/MM/yyyy') ");
                    model.QueryParameters.Add(":receivedFrom", model.ReceivedDateFrom);
                    model.QueryParameters.Add(":receivedTo", model.ReceivedDateaTo);
                }
            }
            return where_q.ToString();
        }
        public Fn10RPT_SFMWModel SearchSFMW(Fn10RPT_SFMWModel model)
        {
            model.QueryWhere = SearchSFMW_where(model);
            model.Sort = " MW_No,Form_No ";
            model.SortType = 1;
            return DA.SearchSFMW(model);
        }
        #endregion

        #region ECPR
        public Fn10RPT_ECPRModel SearchEnquiryAndComplaintProgress(Fn10RPT_ECPRModel model)
        {
            //if (StringUtil.isNotBlank(this.getInterval()))
            //{
            //    int intervalOption = 0;
            //    if (this.getInterval().equals(ApplicationConstant.LESS_THAN_5_DAYS))
            //    {
            //        intervalOption = 1;
            //    }
            //    else if (this.getInterval().equals(ApplicationConstant.MORE_THAN_5_DAYS))
            //    {
            //        intervalOption = 2;
            //    }
            //    else if (this.getInterval().equals(ApplicationConstant.MORE_THAN_10_DAYS))
            //    {
            //        intervalOption = 3;
            //    }
            //    mwGeneralRecordList = filterByInterval(mwGeneralRecordList, intervalOption);
            //}
            return DA.SearchEnquiryAndComplaintProgress(model);
        }

        public string ExcelEnquiryAndComplaintProgress(Fn10RPT_ECPRModel model)
        {
            string reportType = "";
            if (string.IsNullOrWhiteSpace(model.ReportType))
            {
                reportType = ProcessingConstant.ENQUIRY_REPORT + " and " + ProcessingConstant.COMPLAINT_REPORT;
            }


            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");

            CreateComplaintCols(model);

            int rowIdx = 1;
            sheet.Cells[rowIdx, 1].LoadFromText("Enquiry and Complaint Progress report");
            MergeCells(rowIdx, sheet, model);
            rowIdx += 2;

            sheet.Cells[rowIdx, 1].LoadFromText("Search criteria input");
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Type of Report: " + (reportType == "" ? model.ReportType : reportType));
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Related Ref. No.:  " + model.RelatedRefNo);
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Received Date: " + model.ReceivedDateFrom + " -> " + model.ReceivedDateTo);
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Status: " + (model.Status ?? "All"));
            MergeCells(rowIdx, sheet, model);
            rowIdx++;
            sheet.Cells[rowIdx, 1].LoadFromText("Channel of Receipt: " + (model.ReceiptChannel ?? "All"));
            MergeCells(rowIdx, sheet, model);
            rowIdx += 2;

            if (reportType != "")
            {
                CreateEnquiryCols(model);
                sheet.Cells[rowIdx, 1].LoadFromText("Enquiry Result");
                MergeCells(rowIdx, sheet, model);
                rowIdx++;
                model.ReportType = ProcessingConstant.ENQUIRY_REPORT;
                model = SearchEnquiryAndComplaintProgress(model);
                rowIdx = model.preloadExcel(sheet, rowIdx);
                rowIdx++;

                CreateComplaintCols(model);
                sheet.Cells[rowIdx, 1].LoadFromText("Complaint Result");
                MergeCells(rowIdx, sheet, model);
                rowIdx++;
                model.ReportType = ProcessingConstant.COMPLAINT_REPORT;
                model.QueryParameters = new Dictionary<string, object>();
                model = SearchEnquiryAndComplaintProgress(model);
            }
            else
            {
                switch (model.ReportType)
                {
                    case ProcessingConstant.ENQUIRY_REPORT:
                        CreateEnquiryCols(model);
                        break;
                    case ProcessingConstant.COMPLAINT_REPORT:
                        CreateComplaintCols(model);
                        break;
                }
            }

            return model.ExportWithCriteria(ep, sheet, rowIdx, "Submission_Report" + DateTime.Now.ToString("yyyy-MM-dd"));
        }
        #endregion

        #region Non Complete Class II with item 2.15 and 2.34 Report
        public string SearchNCCWI_where(Fn10RPT_NCCWIModel model)
        {
            StringBuilder where_q = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom))
            {
                where_q.Append(@"AND To_date(To_char(RECEIVED_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') > To_date(:receivedDateFrom, 'dd/MM/yyyy')");
                model.QueryParameters.Add(":receivedDateFrom", model.ReceivedDateFrom);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                where_q.Append(@"AND To_date(To_char(RECEIVED_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') < To_date(:receivedDateTo, 'dd/MM/yyyy')");
                model.QueryParameters.Add(":receivedDateTo", model.ReceivedDateTo);
            }
            //where_q.Append(@" GROUP  BY   MW_DSN,
            //                              S_FORM_TYPE_CODE,
            //                              REFERENCE_NO,
            //                              RECEIVED_DATE,
            //                              COMMENCEMENT_SUBMISSION_DATE,
            //                              P_get_item_code_by_record_id2(mwrecord.uuid),
            //                              ADDRESS ");
            return where_q.ToString();
        }

        public Fn10RPT_NCCWIModel SearchNCCWI(Fn10RPT_NCCWIModel model)
        {
            model.QueryWhere = SearchNCCWI_where(model);
            return DA.SearchNCCWI(model);
        }

        #endregion

        private void CreateEnquiryCols(Fn10RPT_ECPRModel model)
        {
            Dictionary<string, string> col1 = model.CreateExcelColumn("Reference No.", "REFERENCE_NO");
            Dictionary<string, string> col2 = model.CreateExcelColumn("Received Date", "RECEIVE_DATE");
            Dictionary<string, string> col3 = model.CreateExcelColumn("Reply/Action Date", "FINAL_REPLY_DATE");
            Dictionary<string, string> col4 = model.CreateExcelColumn("Subject Matter", "SUBJECT_MATTER");
            Dictionary<string, string> col5 = model.CreateExcelColumn("Channel of Enquiry", "CHANNEL");
            Dictionary<string, string> col6 = model.CreateExcelColumn("Status", "STATUS");
            Dictionary<string, string> col7 = model.CreateExcelColumn("Handling Officer", "MODIFIED_BY");

            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3
                ,col4,col5,col6
                ,col7
            };
        }
        private void CreateComplaintCols(Fn10RPT_ECPRModel model)
        {
            Dictionary<string, string> col1 = model.CreateExcelColumn("Reference No.", "REFERENCE_NO");
            Dictionary<string, string> col2 = model.CreateExcelColumn("Received Date", "RECEIVE_DATE");
            Dictionary<string, string> col3 = model.CreateExcelColumn("Interim Reply Date", "INTERIM_REPLY_DATE");
            Dictionary<string, string> col4 = model.CreateExcelColumn("Reply Action Date", "FINAL_REPLY_DATE");
            Dictionary<string, string> col5 = model.CreateExcelColumn("Type of Complaint", "SUBJECT_MATTER");
            Dictionary<string, string> col6 = model.CreateExcelColumn("Channel of Complaint", "CHANNEL");
            Dictionary<string, string> col7 = model.CreateExcelColumn("Status", "STATUS");
            Dictionary<string, string> col8 = model.CreateExcelColumn("Handling Officer", "MODIFIED_BY");

            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3
                ,col4,col5,col6
                ,col7,col8
            };
        }

        public void Fn10RPT_QSRGenerateSql(Fn10RPT_QSRModel model)
        {
            model.Query = @"
                    Select p.Reference_No, 
               Case When Sum(Case When p.Answer1 = :Erection Then cast(p.Answer2 as int) ELSE 0 END) is not null 
                Then Sum(Case When p.Answer1 = :Erection Then cast(p.Answer2 as int) ELSE 0 END) Else 0 End
              - 
                Case When Sum(Case When p.Answer1 = :Removal Then cast(p.Answer2 as int) ELSE 0 END) is not null
               Then Sum(Case When p.Answer1 = :Removal Then cast(p.Answer2 as int) ELSE 0 END) ELSE 0 END Increase,
              Case When Sum(Case When p.Answer1 = :Erection Then cast(p.Answer2 as int) ELSE 0 END) is not null 
                Then Sum(Case When p.Answer1 = :Erection Then cast(p.Answer2 as int) ELSE 0 END) Else 0 End Erection,
             Case When Sum(Case When p.Answer1 = :Alteration Then cast(p.Answer2 as int) ELSE 0 END) is not null 
               Then Sum(Case When p.Answer1 = :Alteration Then cast(p.Answer2 as int) ELSE 0 END) ElSE 0 END Alteration,
             Case When Sum(Case When p.Answer1 = :Removal Then cast(p.Answer2 as int) ELSE 0 END) is not null
               Then Sum(Case When p.Answer1 = :Removal Then cast(p.Answer2 as int) ELSE 0 END) ELSE 0 END Removal From
                           (SELECT checklistItem1.Answer Answer1,refNo.Reference_No,checklistItem2.answer Answer2 
                            FROM P_MW_REFERENCE_NO refNo,
                            P_MW_RECORD_ITEM erection , P_MW_RECORD mwRecord ,
                            P_MW_RECORD finalRecord,
                            P_MW_RECORD_ITEM_CHECKLIST checklist1,
                            P_MW_RECORD_ITEM_CHECKLIST_ITEM checklistItem1,
                            P_MW_RECORD_ITEM_CHECKLIST_ITEM checklistItem2,
                            P_MW_SUMMARY_MW_ITEM_CHECKLIST summaryChecklist,
                            P_S_MW_ITEM_CHECKLIST_ITEM sMwCheckItem,
                            P_S_MW_ITEM_CHECKLIST_ITEM sMwCheckItem2";
            model.QueryParameters.Add("Erection", ProcessingConstant.ANSWER_ERECTION);
            model.QueryParameters.Add("Alteration", ProcessingConstant.ANSWER_ALTERATION);
            model.QueryParameters.Add("Removal", ProcessingConstant.ANSWER_REMOVAL);
            StringBuilder queryWhere = new StringBuilder();
            queryWhere.Append(@" Where mwRecord.reference_number=refNo.uuid 
                                 And finalRecord.IS_DATA_ENTRY = :IsDataEntry 
                                 And finalRecord.REFERENCE_NUMBER = mwRecord.REFERENCE_NUMBER 
                                 And mwRecord.STATUS_CODE  =:StatusCode 
                                 AND summaryChecklist.mw_record_id = mwRecord.uuid 
                                 AND summaryChecklist.RECOMMEDATION_APPLICATION = :Valid ");
            model.QueryParameters.Add("IsDataEntry", ProcessingConstant.FLAG_N);
            model.QueryParameters.Add("StatusCode", ProcessingConstant.MW_SECOND_COMPLETE);
            model.QueryParameters.Add("Valid", ProcessingConstant.CHECKING_OK);

            if (!string.IsNullOrEmpty(model.ClassCode))
            {
                queryWhere.Append(" And mwRecord.CLASS_CODE = :ClassCode ");
                model.QueryParameters.Add("ClassCode", model.ClassCode);
            }
            if (!string.IsNullOrEmpty(model.FromDate))
            {
                queryWhere.Append(" AND TO_DATE(summaryChecklist.SPO_ENDORSEMENT_DATE,  'DD-MON-RR') >= :FromDate ");
                model.QueryParameters.Add("FromDate", Convert.ToDateTime(model.FromDate));
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                queryWhere.Append(" AND TO_DATE(summaryChecklist.SPO_ENDORSEMENT_DATE,  'DD-MON-RR') <= :ToDate ");
                model.QueryParameters.Add("ToDate", Convert.ToDateTime(model.ToDate));
            }

            queryWhere.Append(@" AND mwRecord.uuid = erection.mw_record_id 
                                 AND erection.mw_item_code IN 
                                 (SELECT DISTINCT sMwItemNature.item_no
                                 FROM P_S_MW_ITEM_NATURE sMwItemNature 
                                 WHERE sMwItemNature.description='Signboard') 
                                 AND checklist1.mw_record_item_id = erection.uuid 
                                 AND checklist1.SUBMISSION = 'Completion' 
                                 AND checklistItem1.MW_RECORD_ITEM_CHECKLIST_ID = checklist1.uuid 
                                 AND checklistItem2.MW_RECORD_ITEM_CHECKLIST_ID = checklist1.uuid 
                                 --AND checklistItem1.answer LIKE :Answer 
                                 AND checklistItem1.S_MW_ITEM_CHECKLIST_ITEM_ID = sMwCheckItem.uuid 
                                 And sMwCheckItem.CHECKLIST_TYPE='NATURE' 
                                 AND checklistItem2.S_MW_ITEM_CHECKLIST_ITEM_ID = sMwCheckItem2.uuid 
                                 And sMwCheckItem2.CHECKLIST_TYPE='TEXT_1' )
                                 p GROUP BY p.REFERENCE_NO
                                 ");

            //model.QueryParameters.Add("Answer", Answer);
            model.QueryWhere = queryWhere.ToString();

        }

        public Fn10RPT_QSRModel GetMwRecordOfSignboardAnswer(Fn10RPT_QSRModel model)
        {
            Fn10RPT_QSRGenerateSql(model);
            model.Search();
            return model;
        }
        public string Fn10RPT_QSExcel(Fn10RPT_QSRModel model)
        {
            Fn10RPT_QSRGenerateSql(model);
            Dictionary<string, string> col1 = model.CreateExcelColumn("Submission No.", "REFERENCE_NO");
            Dictionary<string, string> col2 = model.CreateExcelColumn("Net Increase in Signboard No.", "INCREASE");
            Dictionary<string, string> col3 = model.CreateExcelColumn("No.of Erection", "ERECTION");
            Dictionary<string, string> col4 = model.CreateExcelColumn("No. of Alteration", "ALTERATION");
            Dictionary<string, string> col5 = model.CreateExcelColumn("No. of Removal", "REMOVAL");


            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3
                ,col4,col5
            };
            return model.Export("Quasi_Signboard_Registration");
        }

        public void Fn10RPT_ODLGenerateSql(Fn10RPT_ODLModel model)
        {
            model.Query = @"Select ds.item_sequence_no,ds.dsn
                        ,to_char(ds.created_date,'dd/MM/yyyy') as BarcodeDate
                        ,to_char(ds.created_date,'HH24:mm') as BarcodeTime
                        ,to_char(ds.rd_delivered_date,'dd/MM/yyyy') As InitialDate
                        ,to_char(ds.rd_delivered_date,'HH24:mm') as InitialTime
                        ,Case When sv.code = 'DSN_RD_RE_SENT' Then 'Y' ElSE 'N' End As Document_Load
                        ,Case When sv.code = 'DSN_RD_MISSING' Then 'Y' ELSE 'N' End As Document_Missing
                        from P_MW_DSN ds inner join p_s_System_Value sv on ds.scanned_status_id = sv.uuid ";
            model.QueryWhere = String.Format(" Where sv.code in ('{0}','{1}','{2}')", ProcessingConstant.DSN_RD_OUTSTANDING, ProcessingConstant.DSN_RD_RE_SENT, ProcessingConstant.DSN_RD_MISSING);
        }

        public Fn10RPT_ODLModel GetOutstandingDocFromList(Fn10RPT_ODLModel model)
        {
            Fn10RPT_ODLGenerateSql(model);
            model.Search();
            return model;
        }

        public string Fn10RPT_ODLExcel(Fn10RPT_ODLModel model)
        {
            Fn10RPT_ODLGenerateSql(model);
            if (model.Columns.Length == 8)
            {
                if (model.Columns[6].Count == 1)
                {
                    model.Columns[6].Add("columnName", "DOCUMENT_LOAD");
                    model.Columns[6].Add("format", "System.String");
                }
                if (model.Columns[7].Count == 1)
                {
                    model.Columns[7].Add("columnName", "DOCUMENT_MISSING");
                    model.Columns[7].Add("format", "System.String");
                }
            }

            return model.Export("Outstanding_Document");
        }


    }
}