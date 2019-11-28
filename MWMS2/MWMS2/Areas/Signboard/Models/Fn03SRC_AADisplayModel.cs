using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Constant;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn03SRC_AADisplayModel : DisplayGrid
    {
        public string Uuid { get; set; }
        public string FileRefNo { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string SignboardAddress { get; set; }
        public string Status { get; set; }
        public string AuditStatus { get; set; }
        public string AuditResultOption { get; set; }
        public string OtherOfficer { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string AuditResult { get; set; }
        public DateTime? ReferralDate { get; set; }
        public string Remarks { get; set; }
        public string WfStatus { get; set; }
        public string FormCode { get; set; }
        public List<B_SV_RECORD_ITEM> SvRecordItem = new List<B_SV_RECORD_ITEM>();
        public B_SV_SIGNBOARD Signboard = new B_SV_SIGNBOARD();
        public B_SV_APPOINTED_PROFESSIONAL Ap = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Rse = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Rge = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Prc = new B_SV_APPOINTED_PROFESSIONAL();
        public List<B_SV_SCANNED_DOCUMENT> DocList = new List<B_SV_SCANNED_DOCUMENT>();
        public List<B_SV_PHOTO_LIBRARY> PhotoLib = new List<B_SV_PHOTO_LIBRARY>();

        public IEnumerable<SelectListItem> AuditStatusList
        {
            get { return SystemListUtil.GetAuditStatus(); }
        }

        public string InfoSignboardOwnerProvided { get; set; }
        public string ValidityAp { get; set; }
        public string SignatureAp { get; set; }
        public string ValidityPrc { get; set; }
        public string SignatureAs { get; set; }
        public string ItemStated { get; set; }
        public string OtherIrregularities { get; set; }
        public string Recommendation { get; set; }
        public string ToOfficer { get; set; }
        public string IoAddress { get; set; }
        public DateTime? AckletterIssDate { get; set; }

        // Form Screening Check
        public string SChkVsNo { get; set; }
        public string SChkInspDate { get; set; }
        public string SChkWorkDate { get; set; }
        public string SChkSignboard { get; set; }
        public string SChkSig { get; set; }
        public string SChkSigDate { get; set; }
        public string SChkMwItemNo { get; set; }
        public string SChkSupportDoc { get; set; }
        public string SChkSboPwaAp { get; set; }
        public string SChkOthers { get; set; }

        // Preliminary Check
        public string PChkAppApMwItem { get; set; }
        public string PChkValAp { get; set; }
        public string PChkSigAp { get; set; }
        public string PChkValRse { get; set; }
        public string PChkSigRse { get; set; }
        public string PChkValRi { get; set; }
        public string PChkSigRi { get; set; }
        public string PChkValPrc { get; set; }
        public string PChkSigAs { get; set; }
        public string PChkCapAsMwItem { get; set; }

        public B_SV_RECORD svRecord { get; set; }
        public string SvReocrdUuid { get; set; }

        //public DateTime? ModifiedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public string EditMode { get; set; } // comment
        public string ViewEditMode { get; set; } // view, show buttons

        public string ErrMsg { get; set; }
        public string RecordType
        {
            get { return "AUDIT"; }
        }

        public string AUDIT_RESULT_COMPELETE = SignboardConstant.AUDIT_RESULT_COMPELETE;
        public string AUDIT_RESULT_NOT_COMPELETE = SignboardConstant.AUDIT_RESULT_NOT_COMPELETE;
    
        public List<SelectListItem> auditResultCompleteList = new List<SelectListItem>{

                    new SelectListItem { Text = "(a) In order", Value = "a"}
                    , new SelectListItem { Text = "(b) Not in order", Value = "b"}
                    , new SelectListItem { Text = "i. Pending", Value = "bi"}
                    , new SelectListItem { Text = "ii. Intermediate action taken (remark)", Value = "bii"}
                    , new SelectListItem { Text = "iii. Final action taken (remark)", Value = "biii"}
                };

        public List<SelectListItem> auditResultNotCompleteList = new List<SelectListItem>{

                new SelectListItem { Text = "(a) Overdue", Value = "a"}
                , new SelectListItem { Text = "(b) Not yet overdue", Value = "b"}
                , new SelectListItem { Text = "i. Pending", Value = "bi"}
                , new SelectListItem { Text = "ii. Intermediate action taken (remark)", Value = "bii"}
            };

        public string WF_MAP_AUDIT_TO = SignboardConstant.WF_MAP_AUDIT_TO;
        public string WF_MAP_AUDIT_PO = SignboardConstant.WF_MAP_AUDIT_PO;
        public string WF_MAP_AUDIT_SPO = SignboardConstant.WF_MAP_AUDIT_SPO;

        public string CREATE_MODE = SignboardConstant.CREATE_MODE;
        public string EDIT_MODE = SignboardConstant.EDIT_MODE;
        public string VIEW_MODE = SignboardConstant.VIEW_MODE;

        public string SAVE_MODE = SignboardConstant.SAVE_MODE;
        public string PASS_MODE = SignboardConstant.PASS_MODE;
        public string SUBMIT_MODE = SignboardConstant.SUBMIT_MODE;
        public string ROLLBACK_MODE = SignboardConstant.ROLLBACK_MODE;
        public string END_MODE = SignboardConstant.END_MODE;
}
}