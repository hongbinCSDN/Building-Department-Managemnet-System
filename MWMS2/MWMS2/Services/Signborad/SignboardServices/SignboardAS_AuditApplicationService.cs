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
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using System.Data.Entity;
using MWMS2.Areas.Signboard.Models;
using System.Globalization;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.WorkFlow;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardAS_AuditApplicationService : BaseCommonService
    {
        string SearchSRC_AA_q = ""
                    + "\r\n" + "\t" + " select distinct svv.uuid AS UUID, svr.reference_no as REF_NO, svr.received_date as RCV_DATE,"
                    + "\r\n" + "\t" + " CASE WHEN wfu.status = '" + SignboardConstant.WF_STATUS_OPEN + "' THEN 'Open' ELSE 'Close' END as STATUS,"
                    + "\r\n" + "\t" + " CASE WHEN wft.task_code = '" + SignboardConstant.WF_MAP_AUDIT_TO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_TO + "'"
                    + "\r\n" + "\t" + " WHEN wft.task_code = '" + SignboardConstant.WF_MAP_AUDIT_PO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_PO + "'"
                    + "\r\n" + "\t" + " WHEN wft.task_code = '" + SignboardConstant.WF_MAP_AUDIT_SPO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_SPO + "'"
                    + "\r\n" + "\t" + " END as TASK_CODE,"
                    + "\r\n" + "\t" + " svr.FORM_CODE as FORM_CODE"
                    + "\r\n" + "\t" + " from b_sv_audit_record svv, b_sv_record svr, b_wf_info wfi, b_wf_task wft, b_wf_task_user wfu"
                    + "\r\n" + "\t" + " where svr.uuid = svv.sv_record_id"
                    + "\r\n" + "\t" + " AND svr.uuid = wfi.record_id"
                    + "\r\n" + "\t" + " AND wfi.uuid = wft.wf_info_id"
                    + "\r\n" + "\t" + "  AND wft.uuid = wfu.wf_task_id"
                    + "\r\n" + "\t" + " and wft.task_code in ('" + SignboardConstant.WF_MAP_AUDIT_TO + "','" + SignboardConstant.WF_MAP_AUDIT_PO + "','" + SignboardConstant.WF_MAP_AUDIT_SPO + "')";

        string ExportAuditData_q = ""
            + "\r\n select DISTINCT svr.reference_no AS SUBMISSION_NO, svr.form_code AS FORM_CODE, svr.received_date AS RECEIVED_DATE,"
            + "\r\n\t svv.AUDIT_RESULT as AUDIT_RESULT, svv.audit_remark AS REAMRK, svv.referral_date AS REFERRAL_DATE, svv.reply_date AS REPLY_DATE, sba.full_address AS ADDRESS,"
            + "\r\n\t owner.name_chinese as OWNER_CHINESE_NAME, owner.name_english as OWNER_ENGLISH_NAME, ownera.full_address as OWNER_ADDRESS, owner.email as OWNER_EMAIL, owner.contact_no as OWNER_CONTACT_NO, owner.fax_no as OWNER_FAX_NO,"
            + "\r\n\t paw.name_chinese as PAW_CHINESE_NAME, paw.name_english as PAW_ENGLISH_NAME, pawa.full_address as PAW_ADDRESS, paw.email as PAW_EMAIL,paw.contact_no as PAW_CONTACT_NO, paw.fax_no as PAW_FAX_NO,"
            + "\r\n\t io.name_chinese as IO_CHINESE_NAME, io.name_english as IO_ENGLISH_NAME, ioa.full_address as IO_ADDRESS, io.email as IO_EMAIL, io.contact_no as IO_CONTACT_NO, io.fax_no as IO_FAX_NO, "
            + "\r\n\t io.pbp_name AS IO_PBP_NAME, io.pbp_contact_no AS IO_PBP_CONTACT_NO, io.prc_name AS IO_PRC_NAME, io.prc_contact_no AS IO_PRC_CONTACT_NO,"
            + "\r\n\t ap.certification_no as AP_CERT_NO, ap.chinese_name as AP_CHINESE_NAME, ap.english_name as AP_ENGLISH_NAME, "
            + "\r\n\t add_ap.EN_ADDRESS_LINE1 AS AP_EN_ADD_1, add_ap.EN_ADDRESS_LINE2 AS AP_EN_ADD_2, add_ap.EN_ADDRESS_LINE3 AS AP_EN_ADD_3, add_ap.EN_ADDRESS_LINE4 as AP_EN_ADD_4, add_ap.EN_ADDRESS_LINE5 AS AP_EN_ADD_5,"
            + "\r\n\t add_ap.CN_ADDRESS_LINE1 AS AP_CN_ADD_1, add_ap.CN_ADDRESS_LINE2 AS AP_CN_ADD_2, add_ap.CN_ADDRESS_LINE3 AS AP_CN_ADD_3, add_ap.CN_ADDRESS_LINE4 AS AP_CN_ADD_4, add_ap.CN_ADDRESS_LINE5 AS AP_CN_ADD_5,"
            + "\r\n\t ap.contact_no as AP_CONTACT_NO, ap.fax_no as AP_FAX_NO,"
            + "\r\n\t rse.certification_no as RSE_CERT_NO , rse.chinese_name as RSE_CHINESE_NAME, rse.english_name as RSE_ENGLISH_NAME,"
            + "\r\n\t add_rse.EN_ADDRESS_LINE1 AS RSE_EN_ADD_1, add_rse.EN_ADDRESS_LINE2 AS RSE_EN_ADD_2, add_rse.EN_ADDRESS_LINE3 AS RSE_EN_ADD_3, add_rse.EN_ADDRESS_LINE4 as RSE_EN_ADD_4, add_rse.EN_ADDRESS_LINE5 AS RSE_EN_ADD_5,"
            + "\r\n\t add_rse.CN_ADDRESS_LINE1 AS RSE_CN_ADD_1, add_rse.CN_ADDRESS_LINE2 AS RSE_CN_ADD_2, add_rse.CN_ADDRESS_LINE3 AS RSE_CN_ADD_3, add_rse.CN_ADDRESS_LINE4 AS RSE_CN_ADD_4, add_rse.CN_ADDRESS_LINE5 AS RSE_CN_ADD_5,"
            + "\r\n\t rse.contact_no as RSE_CONTACT_NO, rse.fax_no as RSE_FAX_NO,"
            + "\r\n\t rge.certification_no as RGE_CERT_NO, rge.chinese_name as RGE_CHINESE_NAME, rge.english_name as RGE_ENGLISH_NAME, "
            + "\r\n\t add_rge.EN_ADDRESS_LINE1 AS RGE_EN_ADD_1, add_rge.EN_ADDRESS_LINE2 AS RGE_EN_ADD_2, add_rge.EN_ADDRESS_LINE3 AS RGE_EN_ADD_3, add_rge.EN_ADDRESS_LINE4 as RGE_EN_ADD_4, add_rge.EN_ADDRESS_LINE5 AS RGE_EN_ADD_5,"
            + "\r\n\t add_rge.CN_ADDRESS_LINE1 AS RGE_CN_ADD_1, add_rge.CN_ADDRESS_LINE2 AS RGE_CN_ADD_2, add_rge.CN_ADDRESS_LINE3 AS RGE_CN_ADD_3, add_rge.CN_ADDRESS_LINE4 AS RGE_CN_ADD_4, add_rge.CN_ADDRESS_LINE5 AS RGE_CN_ADD_5,"
            + "\r\n\t rge.contact_no as RGE_CONTACT_NO, rge.fax_no as RGE_FAX_NO,"
            + "\r\n\t prc.certification_no as PRC_CERT_NO, prc.chinese_name as PRC_CHINESE_NAME, prc.english_name as PRC_ENGLISH_NAME, "
            + "\r\n\t add_prc.EN_ADDRESS_LINE1 AS PRC_EN_ADD_1, add_prc.EN_ADDRESS_LINE2 AS PRC_EN_ADD_2, add_prc.EN_ADDRESS_LINE3 AS PRC_EN_ADD_3, add_prc.EN_ADDRESS_LINE4 as PRC_EN_ADD_4, add_prc.EN_ADDRESS_LINE5 AS PRC_EN_ADD_5,"
            + "\r\n\t add_prc.CN_ADDRESS_LINE1 AS PRC_CN_ADD_1, add_prc.CN_ADDRESS_LINE2 AS PRC_CN_ADD_2, add_prc.CN_ADDRESS_LINE3 AS PRC_CN_ADD_3, add_prc.CN_ADDRESS_LINE4 AS PRC_CN_ADD_4, add_prc.CN_ADDRESS_LINE5 AS PRC_CN_ADD_5,"
            + "\r\n\t prc.contact_no as PRC_CONTACT_NO, prc.fax_no as PRC_FAX_NO, prc.as_chinese_name AS PRC_AS_CHINESE_NAME, prc.as_english_name AS PRC_AS_ENGLISH_NAME"
            + "\r\n from b_sv_audit_record svv"
            + "\r\n\t LEFT JOIN b_sv_record svr ON svr.uuid = svv.sv_record_id"
            + "\r\n\t LEFT JOIN b_sv_signboard svs ON svs.uuid = svr.sv_signboard_id"
            + "\r\n\t LEFT JOIN b_sv_address sba ON sba.uuid = svs.location_address_id"
            + "\r\n\t LEFT JOIN b_sv_person_contact owner ON owner.uuid = svs.owner_id"
            + "\r\n\t LEFT JOIN b_sv_address ownera ON ownera.uuid = owner.sv_address_id"
            + "\r\n\t LEFT JOIN b_sv_person_contact paw ON paw.uuid = svr.paw_id"
            + "\r\n\t LEFT JOIN b_sv_address pawa ON pawa.uuid = paw.sv_address_id"
            + "\r\n\t LEFT JOIN b_sv_person_contact io ON io.uuid = svr.oi_id"
            + "\r\n\t LEFT JOIN b_sv_address ioa ON ioa.uuid = io.sv_address_id"
            + "\r\n\t LEFT JOIN b_sv_appointed_professional ap ON ap.sv_record_id = svr.uuid"
            + "\r\n\t LEFT JOIN b_sv_appointed_professional rse ON rse.sv_record_id = svr.uuid"
            + "\r\n\t LEFT JOIN b_sv_appointed_professional rge ON rge.sv_record_id = svr.uuid"
            + "\r\n\t LEFT JOIN b_sv_appointed_professional prc ON prc.sv_record_id = svr.uuid"
            + "\r\n\t LEFT JOIN B_CRM_PBP_PRC add_ap ON add_ap.CERTIFICATION_NO = ap.CERTIFICATION_NO"
            + "\r\n\t LEFT JOIN B_CRM_PBP_PRC add_rse ON add_rse.CERTIFICATION_NO = rse.CERTIFICATION_NO"
            + "\r\n\t LEFT JOIN B_CRM_PBP_PRC add_rge ON add_rge.CERTIFICATION_NO = rge.CERTIFICATION_NO"
            + "\r\n\t LEFT JOIN B_CRM_PBP_PRC add_prc ON add_prc.CERTIFICATION_NO = prc.CERTIFICATION_NO";

        // Fn03SRC_AASearchModel: Seach Function
        public Fn03SRC_AASearchModel SearchSRC_AA(Fn03SRC_AASearchModel model)
        {
            model.Query = SearchSRC_AA_q;
            model.QueryWhere = SearchSRC_AA_whereQ(model);

            model.Search();
            return model;
        }
        private string SearchSRC_AA_whereQ(Fn03SRC_AASearchModel model)
        {
            string whereQ = "";
            string CurrUserId = "";
            string RefNo = "";
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            List<String> StatusList = new List<string>();
            CurrUserId = SessionUtil.LoginPost.UUID;
           // CurrUserId = SystemParameterConstant.WFUserUUID;    // Current logged in user's uuid

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                RefNo += model.RefNo;
                whereQ += "\r\n\t" + " and lower(svr.reference_no) like :fileRefNo ";
                model.QueryParameters.Add("fileRefNo", "%" + RefNo.ToLower() + "%");
            }
            if (model.PeriodDateFrom.HasValue)
            {
                FromDate = model.PeriodDateFrom.Value;
                whereQ += "\r\n\t" + " and svr.received_date >= :receivedDateFrom ";
                model.QueryParameters.Add("receivedDateFrom", FromDate);
            }
            if (model.PeriodDateTo.HasValue)
            {
                ToDate = model.PeriodDateTo.Value;
                whereQ += "\r\n\t" + " and svr.received_date <= :receivedDateTo ";
                model.QueryParameters.Add("receivedDateTo", ToDate);
            }
            if (model.Status != null)
            {
                if (model.Status.Equals("Open"))
                {
                    whereQ += "\r\n\t" + " and wfu.status = '" + SignboardConstant.WF_STATUS_OPEN + "'";
                }
                else
                {
                    whereQ += "\r\n\t" + " and wfu.status in ('" + SignboardConstant.WF_STATUS_CLOSE + "', '" + SignboardConstant.WF_STATUS_DONE + "')";
                }
            }

            whereQ += "\r\n\t" + " and wfu.sys_post_id = :userId ";
            model.QueryParameters.Add("userId", CurrUserId);

            return whereQ;
        }
        // Fn03SRC_AASearchModel: Export Seach Function
        public string ExportSRC_AA(Fn03SRC_AASearchModel model)
        {
            model.Query = SearchSRC_AA_q;
            model.QueryWhere = SearchSRC_AA_whereQ(model);
            return model.Export("Audit Application Report");
        }

        public Fn03SRC_AADisplayModel ViewAudit(string uuid)
        {
           using (EntitiesSignboard db = new EntitiesSignboard())
            {
                //SignboardAuditRecordDAOService dao = new SignboardAuditRecordDAOService();
                //var resultList = dao.findByProperty("uuid", id).ToList();

                // check sercurity right

                SignboardAuditRecordDAOService SignboardAuditRecordDAOService = new SignboardAuditRecordDAOService();
                SvRecordItemDAOService SvRecordItemDAOService = new SvRecordItemDAOService();

                List<B_SV_AUDIT_RECORD> recordList = SignboardAuditRecordDAOService.FindByProperty("uuid", uuid);

                B_SV_AUDIT_RECORD auditRecord = recordList.FirstOrDefault();

                B_SV_RECORD svRecord = db.B_SV_RECORD.Where(x => x.UUID == auditRecord.SV_RECORD_ID).FirstOrDefault();

                List<B_SV_RECORD_ITEM> itemList = SvRecordItemDAOService.FindByProperty("SV_RECORD_ID", auditRecord.SV_RECORD_ID);
                List<B_SV_APPOINTED_PROFESSIONAL> profList = db.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.SV_RECORD_ID == auditRecord.SV_RECORD_ID).ToList();

                B_SV_APPOINTED_PROFESSIONAL svAppointedAP = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL svAppointedRSE = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL svAppointedRGE = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL svAppointedPRC = new B_SV_APPOINTED_PROFESSIONAL();

                if(profList != null && profList.Count() > 0)
                {
                    foreach(var prof in profList)
                    {
                        if(prof.IDENTIFY_FLAG.Equals(SignboardConstant.PBP_CODE_AP, StringComparison.InvariantCultureIgnoreCase))
                        {
                            svAppointedAP = prof;
                        }
                        else if(prof.IDENTIFY_FLAG.Equals(SignboardConstant.PBP_CODE_RSE, StringComparison.InvariantCultureIgnoreCase))
                        {
                            svAppointedRSE = prof;
                        }
                        else if(prof.IDENTIFY_FLAG.Equals(SignboardConstant.PBP_CODE_RGE, StringComparison.InvariantCultureIgnoreCase))
                        {
                            svAppointedRGE = prof;
                        }
                        else if(prof.IDENTIFY_FLAG.Equals(SignboardConstant.PRC_CODE, StringComparison.InvariantCultureIgnoreCase))
                        {
                            svAppointedPRC = prof;
                        }
                    }
                }

                B_SV_SIGNBOARD svSignboard = db.B_SV_SIGNBOARD.Where(x => x.UUID == svRecord.SV_SIGNBOARD_ID).FirstOrDefault();

                List<B_SV_SCANNED_DOCUMENT> docList = SignboardAuditRecordDAOService.GetScannedDocumentList(svRecord.REFERENCE_NO);
                List<B_SV_PHOTO_LIBRARY> photoLib = SignboardAuditRecordDAOService.GetPhotoLibraryList(svSignboard.UUID);

                B_SV_ADDRESS svAddress = db.B_SV_ADDRESS.Where(x => x.UUID == svSignboard.LOCATION_ADDRESS_ID).FirstOrDefault();

                string edit_mode = SignboardConstant.VIEW_MODE; // default as view mode

                //Fn03SRC_AASearchModel aa_model = new Fn03SRC_AASearchModel();
                //aa_model.RefNo = svRecord.REFERENCE_NO;
                //var aa = SearchSRC_AA(aa_model);

                var wfInfo = db.B_WF_INFO.Where(x => x.RECORD_ID == svRecord.UUID).FirstOrDefault();
                if(SignboardConstant.WF_STATUS_OPEN.Equals(wfInfo.CURRENT_STATUS))
                {
                    WorkFlowManagementService wf = new WorkFlowManagementService();
                    var wfTaskUserList = wf.getCurrentWfTaskUser(db, SignboardConstant.WF_TYPE_VALIDATION, svRecord.UUID);
                    if (wfTaskUserList != null && wfTaskUserList.Count() > 0)
                    {
                        List<string> wfTaskUserList_Uuid = wfTaskUserList.Select(x => x.SYS_POST_ID).ToList();
                        if (wfTaskUserList_Uuid.Contains(SessionUtil.LoginPost.UUID))
                        {
                            edit_mode = SignboardConstant.EDIT_MODE;
                        }
                    }
                }

                // set EditMode as "edit"
                //if (aa.Data[0]["STATUS"].Equals("Open"))
                //{
                //    WorkFlowDAOService wf = new WorkFlowDAOService();
                //    var wf_task_users = wf.getWFTaskUserStatusOpen(SignboardConstant.WF_TYPE_VALIDATION, svRecord.UUID, auditRecord.WF_STATUS);
                //    if (wf_task_users != null && wf_task_users.Count() > 0)
                //    {
                //        for (int i = 0; i < wf_task_users.Count(); i++)
                //        {
                //            if (SessionUtil.LoginPost.UUID.Equals(wf_task_users[i][0]))
                //            {
                //                edit_mode = SignboardConstant.EDIT_MODE;
                //            }
                //        }
                //    }
                //}

                return new Fn03SRC_AADisplayModel()
                {
                    Uuid = auditRecord != null ? auditRecord.UUID : null,
                    FileRefNo = svRecord != null ? svRecord.REFERENCE_NO : null,
                    ReceivedDate = svRecord != null ? svRecord.RECEIVED_DATE : null,
                    SignboardAddress = svAddress != null ? svAddress.FULL_ADDRESS : null,
                    Status = auditRecord != null ? auditRecord.AUDIT_STATUS : null,
                    AuditStatus = auditRecord != null ? auditRecord.AUDIT_STATUS : null,
                    AuditResultOption = auditRecord != null ? auditRecord.AUDIT_RESULT : null,
                    OtherOfficer = auditRecord != null ? auditRecord.OTHER_HANDLING_OFFICER : null,
                    ReplyDate = auditRecord != null ? auditRecord.REPLY_DATE : null,
                    AuditResult = auditRecord != null ? auditRecord.AUDIT_DESCRIPTION : null,
                    ReferralDate = auditRecord != null ? auditRecord.REFERRAL_DATE : null,
                    Remarks = auditRecord != null ? auditRecord.AUDIT_REMARK : null,
                    WfStatus = auditRecord != null ? auditRecord.WF_STATUS : null,
                    FormCode = svRecord != null ? svRecord.FORM_CODE : null,
                    SvRecordItem = (itemList != null && itemList.Count() > 0) ? itemList : null,
                    Signboard = svSignboard != null ? svSignboard : null,
                    Ap = svAppointedAP != null ? svAppointedAP : null,
                    Rse = svAppointedRSE != null ? svAppointedRSE : null,
                    Rge = svAppointedRGE != null ? svAppointedRGE : null,
                    Prc = svAppointedPRC != null ? svAppointedPRC : null,
                    DocList = (docList != null && docList.Count() > 0) ? docList : null,
                    PhotoLib = (photoLib != null && photoLib.Count() > 0) ? photoLib : null,

                    // auditStatusList

                    // remove default values
                    InfoSignboardOwnerProvided = svRecord != null ? svRecord.INFO_SIGNBOARD_OWNER_PROVIDED : null,
                    ValidityAp = svRecord != null ? svRecord.VALIDITY_AP : null,
                    SignatureAp = svRecord != null ? svRecord.SIGNATURE_AP : null,
                    ValidityPrc = svRecord != null ? svRecord.VALIDITY_PRC : null,
                    SignatureAs = svRecord != null ? svRecord.SIGNATURE_AS : null,
                    ItemStated = svRecord != null ? svRecord.ITEM_STATED : null,
                    OtherIrregularities = svRecord != null ? svRecord.OTHER_IRREGULARITIES : null,
                    Recommendation = svRecord != null ? svRecord.RECOMMENDATION : null,
                    ToOfficer = svRecord != null ? svRecord.TO_OFFICER : null,
                    IoAddress = svRecord != null ? svRecord.IO_ADDRESS : null,
                    AckletterIssDate = svRecord != null ? svRecord.ACK_LETTERISS_DATE : null,

                    // Form Screening Check
                    SChkVsNo = svRecord != null ? svRecord.S_CHK_VS_NO : null,
                    SChkInspDate = svRecord != null ? svRecord.S_CHK_INSP_DATE : null,
                    SChkWorkDate = svRecord != null ? svRecord.S_CHK_WORK_DATE : null,
                    SChkSignboard = svRecord != null ? svRecord.S_CHK_SIGNBOARD : null,
                    SChkSig = svRecord != null ? svRecord.S_CHK_SIG : null,
                    SChkSigDate = svRecord != null ? svRecord.S_CHK_SIG_DATE : null,
                    SChkMwItemNo = svRecord != null ? svRecord.S_CHK_MW_ITEM_NO : null,
                    SChkSupportDoc = svRecord != null ? svRecord.S_CHK_SUPPORT_DOC : null,
                    SChkSboPwaAp = svRecord != null ? svRecord.S_CHK_SBO_PWA_AP : null,
                    SChkOthers = svRecord != null ? svRecord.S_CHK_OTHERS : null,

                    // Preliminary Check
                    PChkAppApMwItem = svRecord != null ? svRecord.P_CHK_APP_AP_MW_ITEM : null,
                    PChkValAp = svRecord != null ? svRecord.P_CHK_VAL_AP : null,
                    PChkSigAp = svRecord != null ? svRecord.P_CHK_SIG_AP : null,
                    PChkValRse = svRecord != null ? svRecord.P_CHK_VAL_RSE : null,
                    PChkSigRse = svRecord != null ? svRecord.P_CHK_SIG_RSE : null,
                    PChkValRi = svRecord != null ? svRecord.P_CHK_VAL_RI : null,
                    PChkSigRi = svRecord != null ? svRecord.P_CHK_SIG_RI : null,
                    PChkValPrc = svRecord != null ? svRecord.P_CHK_VAL_PRC : null,
                    PChkSigAs = svRecord != null ? svRecord.P_CHK_SIG_AS : null,
                    PChkCapAsMwItem = svRecord != null ? svRecord.P_CHK_CAP_AS_MW_ITEM : null,

                    svRecord = svRecord != null ? svRecord : null,
                    SvReocrdUuid = svRecord != null ? svRecord.UUID : null,

                    //ModifiedDate = auditRecord != null ? auditRecord.MODIFIED_DATE : null,
                    ModifiedDate = auditRecord != null ? DateUtil.ConvertToDateTimeDisplay(auditRecord.MODIFIED_DATE) : null,
                    ModifiedBy = auditRecord != null ? auditRecord.MODIFIED_BY : null,

                    EditMode = "edit",
                    ViewEditMode = edit_mode,
                    ErrMsg = "",
                };
            }
        }

        private string ExportAuditData_whereQ(Fn03SRC_AADisplayModel model)
        {
            string whereQ = "";
            whereQ += "\r\n where svv.uuid= ':uuid'"
                      + "\r\n\t and ap.identify_flag= '" + ApplicationConstant.PBP_CODE_AP + "'"
                      + "\r\n\t and rse.identify_flag= '" + ApplicationConstant.PBP_CODE_RSE + "'"
                      + "\r\n\t  and rge.identify_flag= '" + ApplicationConstant.PBP_CODE_RGE + "'"
                      + "\r\n\t and prc.identify_flag= '" + ApplicationConstant.PRC_CODE + "'";
            model.QueryParameters.Add("uuid", model.Uuid);
            return whereQ;
        }

        public FileStreamResult ExportAuditData(Fn03SRC_AADisplayModel model)
        {
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code", "Received Date", "Audit Result", "Remarks", "Referral Date", "Reply Date", "Signboard Location", "Owner Chinese Name", "Owner English Name"
                , "Owner Address", "Owner Email", "Owner Contact No", "Owner Fax NO", "PAW Chinese Name", "PAW English Name", "PAW Address", "PAW Email", "PAW Contact No", "PAW Fax NO"
                , "IO Chinese Name", "IO English Name", "IO Address", "IO Email", "IO Contact No", "IO Fax NO", "IO PBP Name", "IO PBP Contact No", "IO PRC Name", "IO PRC Contact No"
                , "AP Certification No", "AP Chinese Name", "AP English Name"
                , "AP EN_ADDRESS_LINE1", "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3", "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5", "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2", "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4", "AP CN_ADDRESS_LINE5"
                , "AP Contact No", "AP Fax No", "RSE Certification No", "RSE Chinese Name", "RSE English Name"
                , "RSE EN_ADDRESS_LINE1", "RSE EN_ADDRESS_LINE2", "RSE EN_ADDRESS_LINE3", "RSE EN_ADDRESS_LINE4", "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1", "RSE CN_ADDRESS_LINE2", "RSE CN_ADDRESS_LINE3", "RSE CN_ADDRESS_LINE4", "RSE CN_ADDRESS_LINE5"
                , "RSE Contact No", "RSE Fax No", "RGE Certification No", "RGE Chinese Name", "RGE English Name"
                , "RGE EN_ADDRESS_LINE1", "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3", "RGE EN_ADDRESS_LINE4", "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1", "RGE CN_ADDRESS_LINE2", "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4", "RGE CN_ADDRESS_LINE5"
                , "RGE Contact No", "RGE Fax No", "PRC Certification No", "PRC Chinese Name", "PRC English Name"
                , "PRC EN_ADDRESS_LINE1", "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3", "PRC EN_ADDRESS_LINE4", "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1", "PRC CN_ADDRESS_LINE2", "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4", "PRC CN_ADDRESS_LINE5"
                , "PRC Contact No", "PRC Fax No", "PRC AS Chinese Name", "PRC AS English Name"
            };
            List<List<object>> data = new List<List<object>>();
            SignboardAuditRecordDAOService ss = new SignboardAuditRecordDAOService();
            //List<object> resultList = ss.GetAuditRecord(model.Uuid);
            //data.Add(resultList);
            data = ss.GetAuditRecord(model.Uuid);
            return this.exportCSVFile("AuditDataExport", headerList, data);
        }

        public FileStreamResult ExportAuditDataToExcel(Fn03SRC_AADisplayModel model)
        {
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code",
                "Received Date", "Audit Result", "Remarks",
                "Referral Date", "Reply Date",
                "Signboard Location", "Owner Chinese Name",
                "Owner English Name",
                "Owner Address", "Owner Email",
                "Owner Contact No", "Owner Fax NO",
                "PAW Chinese Name", "PAW English Name",
                "PAW Address", "PAW Email",
                "PAW Contact No", "PAW Fax NO",
                "IO Chinese Name", "IO English Name",
                "IO Address", "IO Email",
                "IO Contact No", "IO Fax NO", "IO PBP Name",
                "IO PBP Contact No", "IO PRC Name",
                "IO PRC Contact No",
                "AP Certification No",
                "AP Chinese Name", "AP English Name",
                "AP EN_ADDRESS_LINE1",
                "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5",
                "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2",
                "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No",
                "RSE Certification No", "RSE Chinese Name",
                "RSE English Name",
                "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2", "RSE EN_ADDRESS_LINE3",
                "RSE EN_ADDRESS_LINE4",
                "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",
                "RSE CN_ADDRESS_LINE3", "RSE CN_ADDRESS_LINE4",
                "RSE CN_ADDRESS_LINE5",
                "RSE Contact No", "RSE Fax No",
                "RGE Certification No", "RGE Chinese Name",
                "RGE English Name",
                "RGE EN_ADDRESS_LINE1",
                "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3",
                "RGE EN_ADDRESS_LINE4",
                "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2",
                "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",
                "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name",
                "PRC English Name",
                "PRC EN_ADDRESS_LINE1",
                "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4",
                "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1",
                "PRC CN_ADDRESS_LINE2",
                "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5",
                "PRC Contact No", "PRC Fax No",
                "PRC AS Chinese Name", "PRC AS English Name"
            };
            List<List<object>> data = new List<List<object>>();
            SignboardAuditRecordDAOService ss = new SignboardAuditRecordDAOService();
            data = ss.GetAuditRecord(model.Uuid);
            return this.exportExcelFile("Audit", headerList, data);
        }

        public Fn03SRC_AADisplayModel SaveAuditRecord(Fn03SRC_AADisplayModel model)
        {
            
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var svAuditRecord = db.B_SV_AUDIT_RECORD.Where(x => x.UUID == model.Uuid).FirstOrDefault();
                        svAuditRecord.OTHER_HANDLING_OFFICER = model.OtherOfficer;
                        svAuditRecord.AUDIT_STATUS = model.AuditStatus;
                        svAuditRecord.AUDIT_REMARK = model.Remarks;
                        svAuditRecord.AUDIT_RESULT = model.AuditResultOption;
                        svAuditRecord.AUDIT_DESCRIPTION = model.AuditResult;
                        svAuditRecord.REFERRAL_DATE = model.ReferralDate;
                        svAuditRecord.REPLY_DATE = model.ReplyDate;

                        string mode = model.EditMode;
                        string wfDirect = "";

                        if (mode.Equals(SignboardConstant.PASS_MODE))
                        {
                            wfDirect = SignboardConstant.WF_GO_SPO;
                        }
                        if (mode.Equals(SignboardConstant.SUBMIT_MODE))
                        {
                            wfDirect = SignboardConstant.WF_GO_NEXT;
                        }
                        if (mode.Equals(SignboardConstant.ROLLBACK_MODE))
                        {
                            wfDirect = SignboardConstant.WF_GO_BACK;
                        }
                        if (mode.Equals(SignboardConstant.SAVE_MODE))
                        {
                            wfDirect = "";
                        }

                        // txn = session.beginTransaction();
                        // auditRecordDAOManager.save(svAuditRecord);
                        db.SaveChanges();

                        if (!wfDirect.Equals(""))
                        {
                            WorkFlowManagementService wfs = new WorkFlowManagementService();

                            String returnDirect = wfs.goNextWorkFlow(db,SignboardConstant.WF_TYPE_VALIDATION, model.SvReocrdUuid, wfDirect);
                            svAuditRecord.WF_STATUS = returnDirect;
                            // auditRecordDAOManager.save(svAuditRecord);
                            db.SaveChanges();

                            B_SV_RECORD record = db.B_SV_RECORD.Where(x => x.UUID == model.SvReocrdUuid).FirstOrDefault();
                            record.WF_STATUS = returnDirect;
                            // svRecordDAO.save(record);
                            db.SaveChanges();
                        }
                        else
                        {
                            // auditRecordDAOManager.save(svAuditRecord);
                            db.SaveChanges();
                        }
                        transaction.Commit();
                        return model;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        model.ErrMsg = "Error saveAuditRecord: " + ex.Message;
                        return model;
                        // throw ex;
                    }
                }


                
            }
            
        }

    }
}