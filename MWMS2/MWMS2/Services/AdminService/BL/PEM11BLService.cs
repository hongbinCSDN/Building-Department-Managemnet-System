using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.AdminService.DAO;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.AdminService.BL
{
    public class PEM11BLService
    {
        private PEM11DAOService _DA;
        protected PEM11DAOService DA
        {
            get
            {
                return _DA ?? (_DA = new PEM11DAOService());
            }
        }

        #region Edit DSN Nature
        public DSNSearchModel SearchDSN(DSNSearchModel model)
        {
            model.Query = @"Select UUID,DSN from P_MW_DSN ";
            if (!string.IsNullOrEmpty(model.DSN))
                model.QueryWhere = @" Where DSN like '%" + model.DSN + "%' ";
            model.Search();
            return model;

        }

        public DSNSearchModel DSNDetail(string UUID)
        {
            return DA.DSNDetail(UUID);
        }

        public ServiceResult SaveDSN(DSNSearchModel model)
        {
            return DA.SaveDSN(model);
        }
        #endregion

        #region PEM1110 Import Records
        public PEM1110ImportRecordsModel SearchImportRecords(PEM1110ImportRecordsModel model)
        {
            return DA.SearchImportRecords(model);
        }

        public string ExportPEM1110Records(PEM1110ImportRecordsModel model)
        {
            model.Query = @"  SELECT
                              item.DSN
                            , item.DATE_RECEIVED
                            , item.DOC_TYPE
                            , item.SUBMISSION_NO
                            , item.PREVIOUS_SEC_ENTRY_NO
                            , CASE
                             WHEN ssv.DESCRIPTION = 'Imported' AND item.SELECTED_FOR_AUDIT = 'Y' THEN 'Audit'
                             ELSE CAST ( ssv.DESCRIPTION AS VARCHAR2 (1000)) END AS STATUS
                            , CASE
                             WHEN ssv.DESCRIPTION = 'Imported' AND item.SELECTED_FOR_AUDIT = 'Y' THEN ''
                             WHEN ssv.DESCRIPTION = 'Imported' THEN ''
                             WHEN ssv2.DESCRIPTION IS NULL THEN CAST (item.STATUS_DESCRIPTION AS VARCHAR2 (1000))
                             ELSE CAST ( ssv2.DESCRIPTION AS VARCHAR2 (1000))
                            END AS DESCRIPTION
                            FROM P_MW_IMPORTED_TASK task 
                            LEFT JOIN P_MW_IMPORTED_TASK_ITEM item ON task.UUID = item.MW_IMPORTED_TASK_ID
                            LEFT JOIN P_S_SYSTEM_VALUE ssv ON item.STATUS = ssv.CODE
                            LEFT JOIN P_S_SYSTEM_TYPE sst ON sst.UUID = ssv.SYSTEM_TYPE_ID AND sst.TYPE = 'IMPORT_STATUS'
                            LEFT JOIN P_S_SYSTEM_VALUE ssv2 ON item.STATUS_DESCRIPTION = ssv2.CODE
                            LEFT JOIN P_S_SYSTEM_TYPE sst2 ON sst2.UUID = ssv2.SYSTEM_TYPE_ID AND sst2.TYPE = :IMPORT_STATUS_DESCRIPTION";
            model.QueryWhere = @"WHERE item.MW_IMPORTED_TASK_ID = :UUID ";
            //ORDER BY item.ORDERING";
            model.Sort = "item.ORDERING";
            model.QueryParameters.Add("IMPORT_STATUS_DESCRIPTION", ProcessingConstant.S_TYPE_IMPORT_STATUS_DESCRIPTION);
            model.QueryParameters.Add("UUID", model.UUID);
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string> { ["columnName"] = "DSN", ["displayName"] = "DSN" })
                .Append(new Dictionary<string, string> { ["columnName"] = "DATE_RECEIVED", ["displayName"] = "Date Received(YYYY-MM-DD)" })
                .Append(new Dictionary<string, string> { ["columnName"] = "DOC_TYPE", ["displayName"] = "Doc Type" })
                .Append(new Dictionary<string, string> { ["columnName"] = "SUBMISSION_NO", ["displayName"] = "Submission No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "PREVIOUS_SEC_ENTRY_NO", ["displayName"] = "Previous Second Entry No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "STATUS", ["displayName"] = "Import Status" })
                .Append(new Dictionary<string, string> { ["columnName"] = "DESCRIPTION", ["displayName"] = "Status Description" })
                .ToArray();
            return model.Export("MW_Records_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        }

        public ServiceResult ImportRecordsExcel(PEM1110ImportRecordsModel model)
        {
            model.p_MW_IMPORTED_TASK.STATUS = ProcessingConstant.S_VAL_UPLOADED;
            model.p_MW_IMPORTED_TASK.CREATED_BY = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
            model.p_MW_IMPORTED_TASK.CREATED_DATE = DateTime.Now;
            model.p_MW_IMPORTED_TASK.MODIFIED_BY = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
            model.p_MW_IMPORTED_TASK.MODIFIED_DATE = DateTime.Now;
            if(DA.ImportTaskRecordsExcel(model).Result == ServiceResult.RESULT_SUCCESS)
            {
                var package = model.ExcelPackage;
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                int stratRow = 2;
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                model.p_MW_IMPORTED_TASK_ITEMList = new List<P_MW_IMPORTED_TASK_ITEM>();
                for (int rowIterator = stratRow; rowIterator <= noOfRow; rowIterator++)
                {
                    model.p_MW_IMPORTED_TASK_ITEMList.Add(new P_MW_IMPORTED_TASK_ITEM()
                    {
                        MW_IMPORTED_TASK_ID = model.p_MW_IMPORTED_TASK.UUID
                        ,
                        SCREENED_NATURE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 1].Value)
                        ,
                        DSN = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value)
                        ,
                        DATE_RECEIVED = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value)
                        ,
                        DOC_TYPE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value)
                        ,
                        SUBMISSION_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value)
                        ,
                        COUNTER = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 6].Value)
                        ,
                        REF_NO_AP = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 7].Value)
                        ,
                        REF_NO_PRC = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 8].Value)
                        ,
                        WORKS_LOCATION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 9].Value)
                        ,
                        STREET = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 10].Value)
                        ,
                        STREET_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 11].Value)
                        ,
                        BUILDING_BLOCK = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 12].Value)
                        ,
                        FLOOR = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 13].Value)
                        ,
                        UNIT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 14].Value)
                        ,
                        MW_ITEM = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 15].Value)
                        ,
                        SELECTED_FOR_AUDIT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 16].Value)
                        ,
                        SSP = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 17].Value)
                         ,
                        DESCRIPTION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 18].Value)
                        ,
                        COMMENCEMENT_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 19].Value)
                        ,
                        COMPLETION_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 20].Value)
                        ,
                        RELATED_SIGNBOARD = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 21].Value)
                        ,
                        ORDER_RELATED = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 22].Value)
                        ,
                        REFERRAL_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 23].Value)
                        ,
                        PAW = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 24].Value)
                         ,
                        PAW_CONTACT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 25].Value)
                         ,
                        IO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 26].Value)
                        ,
                        IO_CONTACT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 27].Value)
                        ,
                        REMARK = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 28].Value)
                        ,
                        LETTER_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 29].Value)
                        ,
                        SDF = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 30].Value)
                        ,
                        PREVIOUS_SEC_ENTRY_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 31].Value)
                        ,
                        RESULT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 32].Value)
                        ,
                        STATUS = ProcessingConstant.S_VAL_UPLOADED
                        ,
                        CREATED_BY = SessionUtil.LoginPost.BD_PORTAL_LOGIN
                        ,
                        CREATED_DATE = DateTime.Now
                        ,
                        MODIFIED_BY = SessionUtil.LoginPost.BD_PORTAL_LOGIN
                        ,
                        MODIFIED_DATE = DateTime.Now
                    });
                }
                return DA.ImportTaskItemRecordsExcel(model);
            }
            else
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { "File uploaded failed." } };
        }

        public ServiceResult DeleteMWRecords(PEM1110ImportRecordsModel model)
        {
            return DA.DeleteMWRecords(model);
        }

        public ServiceResult UpdateFileStatus(PEM1110ImportRecordsModel model)
        {
            return DA.UpdateFileStatus(model);
        }

        public string ExportItemRecords(PEM1110ImportRecordsModel model)
        {
            model.Query = @"Select 
                            item.screened_nature
                            ,item.dsn
                            ,item.date_received
                            ,item.doc_type
                            ,item.submission_no
                            ,item.counter
                            ,item.ref_no_ap
                            ,item.ref_no_prc
                            ,item.works_location
                            ,item.street
                            ,item.street_no
                            ,item.building_block
                            ,item.floor
                            ,item.unit
                            ,item.mw_item
                            ,item.selected_for_audit
                            ,item.ssp
                            ,item.description
                            ,item.commencement_date
                            ,item.completion_date
                            ,item.related_signboard
                            ,item.order_related
                            ,item.referral_date
                            ,item.Paw
                            ,item.paw_contact
                            ,item.io
                            ,item.io_contact
                            ,item.remark
                            ,item.letter_date
                            ,item.sdf
                            ,item.previous_sec_entry_no
                            ,item.result
                             from P_MW_IMPORTED_TASK_ITEM item  ";
            model.QueryWhere = @"Where item.mw_imported_task_id = :UUID ";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Columns = new List<Dictionary<string, string>>()
               .Append(new Dictionary<string, string> { ["displayName"] = "SCREENED NATURE", ["columnName"] = "SCREENED_NATURE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "DSN", ["columnName"] = "DSN" })
               .Append(new Dictionary<string, string> { ["displayName"] = "DATE RECEIVED", ["columnName"] = "DATE_RECEIVED" }) 
               .Append(new Dictionary<string, string> { ["displayName"] = "DOC TYPE", ["columnName"] = "DOC_TYPE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "SUBMISSION NO. (Com for complaint /MW /VS for MW06 /Enq for enquiry) ", ["columnName"] = "SUBMISSION_NO" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Counter", ["columnName"] = "COUNTER" })
               .Append(new Dictionary<string, string> { ["displayName"] = "REG. NO. of AP", ["columnName"] = "REF_NO_AP" })
               .Append(new Dictionary<string, string> { ["displayName"] = "REG. NO. of PRC", ["columnName"] = "REF_NO_PRC" })
               .Append(new Dictionary<string, string> { ["displayName"] = "WORKS LOCATION", ["columnName"] = "WORKS_LOCATION" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Street", ["columnName"] = "STREET" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Street No.", ["columnName"] = "STREET_NO" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Building / Block", ["columnName"] = "BUILDING_BLOCK" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Floor", ["columnName"] = "FLOOR" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Unit", ["columnName"] = "UNIT" })
               .Append(new Dictionary<string, string> { ["displayName"] = "MW. Item(s)", ["columnName"] = "MW_ITEM" })
               .Append(new Dictionary<string, string> { ["displayName"] = "SELECTED FOR AUDIT (Y / N)", ["columnName"] = "SELECTED_FOR_AUDIT" })
               .Append(new Dictionary<string, string> { ["displayName"] = "SSP (For MW01 only)", ["columnName"] = "SSP" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Eng(1) or Chin(2) for description", ["columnName"] = "DESCRIPTION" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Commencement Date", ["columnName"] = "COMMENCEMENT_DATE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Completion Date", ["columnName"] = "COMPLETION_DATE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Items related to Signboard (Y) 1.20-1.24; 2.18 - 2.27;3.16 - 3.22", ["columnName"] = "RELATED_SIGNBOARD" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Order Related", ["columnName"] = "ORDER_RELATED" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Referral Date", ["columnName"] = "REFERRAL_DATE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "PAW", ["columnName"] = "PAW" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Contact of PAW", ["columnName"] = "PAW_CONTACT" })
               .Append(new Dictionary<string, string> { ["displayName"] = "I.O. or Property Management", ["columnName"] = "IO" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Contact of I.O. or Property Management", ["columnName"] = "IO_CONTACT" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Remark", ["columnName"] = "REMARK" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Letter Date", ["columnName"] = "LETTER_DATE" })
               .Append(new Dictionary<string, string> { ["displayName"] = "SDF", ["columnName"] = "SDF" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Previous Related MW No.", ["columnName"] = "PREVIOUS_SEC_ENTRY_NO" })
               .Append(new Dictionary<string, string> { ["displayName"] = "Acknowledgement Result (A / R)", ["columnName"] = "RESULT" })
               .ToArray();
            return model.Export(model.fileName);
        }
        #endregion

        #region PEM1114 MW Number Mapping
        public PEM1114MWNumberMappingModel SearchMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            StringBuilder queryWhere = new StringBuilder(" Where Category_Code in ('MW','VS') ");
            if (!string.IsNullOrEmpty(model.Reference_No))
                 queryWhere.Append(" And Reference_No like '%" + model.Reference_No + "%'");
            model.QueryWhere = queryWhere.ToString();
            model.Sort = "Reference_No";
            return DA.SearchMWNumberMapping(model);
        }

        public PEM1114MWNumberMappingModel SearchMWNumberUserName(PEM1114MWNumberMappingModel model)
        {
            return DA.SearchMWNumberUserName(model);
        }

        public ServiceResult SaveMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            return DA.SaveMWNumberMapping(model);
        }
        #endregion

        #region PEM1112 Pending Transfer
        public PEM1112PendingTransferModel SearchPendingTransfer(PEM1112PendingTransferModel model)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT REF_NO, MW_ITEM_CODE, COMPLETE || ' / ' || TOTAL AS COMPLETE_TOTAL,  READY FROM (
                            SELECT D.RECORD_ID AS REF_NO
                            , SUM(CASE WHEN sd.RRM_SYN_STATUS = 'COMPLETE' THEN 1 ELSE 0 END) AS COMPLETE
                            , SUM(CASE WHEN sd.RRM_SYN_STATUS = 'READY' THEN 1 ELSE 0 END) AS READY
                            , COUNT(1) AS TOTAL
                            FROM P_MW_DSN d
                            INNER JOIN P_MW_SCANNED_DOCUMENT sd ON d.UUID = sd.DSN_ID
                            INNER JOIN P_MW_REFERENCE_NO rn ON d.RECORD_ID = rn.REFERENCE_NO
                            INNER JOIN P_MW_RECORD r ON rn.UUID = r.REFERENCE_NUMBER
                            WHERE r.RRM_SYN_STATUS = 'COMPLETE' AND sd.FOLDER_TYPE = 'Public' ");
            if (!string.IsNullOrEmpty(model.Ref_No))
            {
                query.Append(" And Rn.REFERENCE_NO like '%" + model.Ref_No + "%'");
              
            }
            query.Append(@" GROUP BY D.RECORD_ID
                            ) DT LEFT JOIN (
                            SELECT n.REFERENCE_NO AS REFERENCE_NO, listagg( i.MW_ITEM_CODE) AS MW_ITEM_CODE
                            FROM P_MW_REFERENCE_NO n
                            INNER JOIN P_MW_RECORD r ON r.REFERENCE_NUMBER = n.UUID
                            INNER JOIN P_MW_RECORD_ITEM i ON r.UUID = i.MW_RECORD_ID
                            WHERE r.IS_DATA_ENTRY = 'N'
                            AND i.MW_ITEM_CODE IS NOT NULL
                            AND i.MW_ITEM_CODE != 'NO ITEM NO.'
                            AND i.MW_ITEM_CODE != 'NO ITEM' GROUP BY REFERENCE_NO
                            ) IT ON DT.REF_NO = IT.REFERENCE_NO
                            WHERE COMPLETE != TOTAL
                            ");
            model.Query = query.ToString();
            return DA.SearchPendingTransfer(model);
        }

        public ServiceResult SavePendingTransfer(PEM1112PendingTransferModel model)
        {
            return DA.SavePendingTransfer(model);
        }
        #endregion

        #region PEM1111 Upload PDF History
        public PEM1111UploadPDFHistoryModel SearchUploadPDFHistory(PEM1111UploadPDFHistoryModel model)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.Append(@"SELECT UUID, CREATED_DATE, CREATED_BY, SUCCESS || ' / ' || UNSUCCESS AS RESULT FROM( 
                                 SELECT t0.UUID AS UUID 
                                , TO_CHAR(t0.CREATED_DATE, 'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE 
                                , t0.CREATED_BY AS CREATED_BY 
                                , SUM(CASE WHEN t1.STATUS = 'success' THEN 1 ELSE 0 END) AS SUCCESS 
                                , SUM(CASE WHEN t1.STATUS = 'success' THEN 0 ELSE 1 END) AS UNSUCCESS 
                                FROM P_MW_UPLOAD t0, P_MW_UPLOAD_ITEM t1 WHERE t0.UUID = t1.MW_UPLOAD_ID ");
            if (!string.IsNullOrEmpty(model.UploadBy))
            {
                queryString.Append(" And t0.CREATED_BY like '%"+ model.UploadBy +"%' ");
            }
            if(!string.IsNullOrEmpty(model.FromDate))
            {
                queryString.Append(" And TO_DATE(TO_CHAR(t0.CREATED_DATE, 'YYYYMMDD'), 'YYYYMMDD') >= :FromDate ");
                model.QueryParameters.Add("FromDate",Convert.ToDateTime(model.FromDate));
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                queryString.Append(" And TO_DATE(TO_CHAR(t0.CREATED_DATE, 'YYYYMMDD'), 'YYYYMMDD') <= :ToDate ");
                model.QueryParameters.Add("ToDate", Convert.ToDateTime(model.ToDate));
            }

            queryString.Append(@" GROUP BY t0.UUID, t0.CREATED_DATE, t0.CREATED_BY, t0.STATUS
                                  ORDER BY t0.CREATED_DATE DESC ) Where 1=1  ");
            if (!string.IsNullOrEmpty(model.Status))
            {
                if (model.Status == "SUCCESS")
                    queryString.Append(" AND UNSUCCESS = 0 "); 
                if(model.Status == "UNSUCCESS")        
                    queryString.Append(" AND UNSUCCESS <> 0 ");
            }

            model.Query = queryString.ToString();
            model.Search();
            return model;
        }

        public string ExportDetailsPDF(PEM1111UploadPDFHistoryModel model)
        {
            StringBuilder queryString = new StringBuilder(@"SELECT
                            ui.DSN
                            , ui.DSN_SUB
                            , ui.DOCUMENT_TYPE
                            , ui.FOLDER_TYPE
                            , CASE WHEN ui.STATUS = 'success' OR ui.STATUS IS NULL THEN ui.STATUS ELSE ui.STATUS || ':' || ui.STATUS_DESCRIPTION END AS RESULT
                            FROM P_MW_UPLOAD u, P_MW_UPLOAD_ITEM ui WHERE u.UUID = ui.MW_UPLOAD_ID");
            if (!string.IsNullOrEmpty(model.UUID))
            {
                queryString.Append(@" And u.UUID = :UUID ");
                model.QueryParameters.Add("UUID", model.UUID);
            }
            model.Sort = " ui.DSN_SUB";
            model.Query = queryString.ToString();
            model.Columns = new List<Dictionary<string, string>>()
             .Append(new Dictionary<string, string> { ["columnName"] = "DSN", ["displayName"] = "DSN" })
             .Append(new Dictionary<string, string> { ["columnName"] = "DSN_SUB", ["displayName"] = "Sub-dsn" })
             .Append(new Dictionary<string, string> { ["columnName"] = "DOCUMENT_TYPE", ["displayName"] = "Document Type" })
             .Append(new Dictionary<string, string> { ["columnName"] = "FOLDER_TYPE", ["displayName"] = "Folder Type" })
             .Append(new Dictionary<string, string> { ["columnName"] = "RESULT", ["displayName"] = "Result" })
             .ToArray();
            return model.Export("Scan Document Upload Report on " + model.CreateDate);
        }

        #endregion

        #region Update Submission Record
        public ServiceResult SearchSubmissionRecord(string refNo)
        {
            return DA.SearchSubmissionRecord(refNo);
        }

        public PEM1103UpdateSubmissionRecordModel GetSubmissionRecordDetail(string uuid)
        {
            return DA.GetSubmissionRecordDetail(uuid);
        }

        public PEM1103UpdateSubmissionRecordModel GetIssuedCorrespondence(PEM1103UpdateSubmissionRecordModel model)
        {
            return DA.GetIssuedCorrespondence(model);
        }

        public PEM1103UpdateSubmissionRecordModel GetSubmissionOfLetterModule(PEM1103UpdateSubmissionRecordModel model)
        {
            return DA.GetSubmissionOfLetterModule(model);
        }
        #endregion

        #region PEM1116 Update Job Assignment
        string SearchUJA_q = " \r\n select po.UUID as UUID, dsn.record_id AS REFERENCE_NO, "
            + " \r\n\t info.RECORD_TYPE AS RECORD_TYPE, "
            + " \r\n\t wtf.task_Code as TASK, "
            + " \r\n\t po.code as RECORD_USER, tu.UUID AS WFTU_UUID "
            + " \r\n\t FROM p_wf_info info "
            + " \r\n\t inner join p_wf_task wtf on wtf.p_WF_INFO_ID = info.UUID "
            + " \r\n\t inner join p_wf_task_user tu on tu.p_WF_TASK_ID = wtf.UUID "
            + " \r\n\t inner join sys_post po on po.UUID = tu.user_id "
            + " \r\n\t LEFT JOIN p_mw_dsn dsn ON dsn.dsn = info.RECORD_ID "
            + " \r\n\t where 1=1 ";

        private string SearchUJA_whereQ(PEMUpdateJobAssignmentSearchModel model)
        {
            string whereQ = "";

            whereQ += "\r\n\t AND tu.STATUS = :status";
            model.QueryParameters.Add("status", ProcessingConstant.WF_STATUS_OPEN);

            if (!string.IsNullOrWhiteSpace(model.SubmissionNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(dsn.record_id) LIKE :RefNo";
                model.QueryParameters.Add("RefNo", "%" + model.SubmissionNo.Trim().ToUpper() + "%");
            }

            return whereQ;
        }

        public PEMUpdateJobAssignmentSearchModel SearchUJA(PEMUpdateJobAssignmentSearchModel model)
        {
            model.Query = SearchUJA_q;
            model.QueryWhere = SearchUJA_whereQ(model);
            model.Search();
            return model;
        }

        public PEMUpdateJobAssignmentSearchModel ViewUJA(string id, string type, string task, string user, string refNo, string wftuUuid)
        {
            PEMUpdateJobAssignmentSearchModel model = new PEMUpdateJobAssignmentSearchModel();

            List<SelectListItem> list = new List<SelectListItem>();
            using (EntitiesAuth auth = new EntitiesAuth())
            {
                model.SYS_POST = auth.SYS_POST.Where(o => o.UUID == id).FirstOrDefault();
                model.UUID = id;
                model.RefNo = refNo;
                model.Type = type;
                model.Task = task;
                model.User = user;
                model.NewHandler = model.User;
                model.wftuUuid = wftuUuid;
            }

            return model;
        }

        public void SaveForm(PEMUpdateJobAssignmentSearchModel model)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var query = db.P_WF_TASK_USER.Find(model.wftuUuid);
                query.USER_ID = model.NewHandler;
                db.SaveChanges();
            }
        }

        #endregion
    }
}