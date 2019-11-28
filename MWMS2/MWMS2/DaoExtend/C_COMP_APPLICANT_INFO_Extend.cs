﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2.Entity
{
    using MWMS2.Areas.Registration.Models;
    using MWMS2.Constant;
    using MWMS2.Utility;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Web;

    [MetadataType(typeof(C_COMP_APPLICANT_INFO_Extend))] public partial class C_COMP_APPLICANT_INFO
    {
        public bool EFORM { get; set; }
        public bool FILEEXSIST { get; set; }
        public C_INTERVIEW_CANDIDATES C_INTERVIEW_CANDIDATES { get; set; }
        public HttpPostedFileBase Applicant_File { get; set; }
        public string refNo;
        private int _TIMEDURATION = -1;
        public int TIMEDURATION {
            get
            {
                return C_INTERVIEW_CANDIDATES==null? 0:  _TIMEDURATION < 0 ? (C_INTERVIEW_CANDIDATES.END_DATE - C_INTERVIEW_CANDIDATES.START_DATE).Value.Minutes : _TIMEDURATION ;
            }
            set { _TIMEDURATION = value; }
        }
        public C_COMP_APPLICANT_INFO MergeByList(List<C_COMP_APPLICANT_INFO> list)
        {
            if (list == null) return this;
            C_COMP_APPLICANT_INFO v = list.Where(o => o.UUID == this.UUID).FirstOrDefault();
            if (v == null) return this;
            if(v.C_APPLICANT != null)
            {
                if (C_APPLICANT == null) C_APPLICANT = new C_APPLICANT();
                C_APPLICANT.HKID = v.C_APPLICANT.HKID;
                C_APPLICANT.PASSPORT_NO = v.C_APPLICANT.PASSPORT_NO;
                C_APPLICANT.SURNAME = v.C_APPLICANT.SURNAME;
                C_APPLICANT.GIVEN_NAME_ON_ID = v.C_APPLICANT.GIVEN_NAME_ON_ID;
                C_APPLICANT.GENDER = v.C_APPLICANT.GENDER;
                C_APPLICANT.CHINESE_NAME = v.C_APPLICANT.CHINESE_NAME;
                C_APPLICANT.TITLE_ID = v.C_APPLICANT.C_S_SYSTEM_VALUE?.UUID;
                C_APPLICANT.DOB = v.C_APPLICANT.DOB;
            }
            REMOVAL_DATE = v?.REMOVAL_DATE;
            CARD_ISSUE_DATE = v?.CARD_ISSUE_DATE;
            CARD_EXPIRY_DATE = v?.CARD_EXPIRY_DATE;
            CARD_SERIAL_NO = v?.CARD_SERIAL_NO;
            CARD_RETURN_DATE = v?.CARD_RETURN_DATE;
            REMARK = v?.REMARK;
            ACCEPT_DATE = v?.ACCEPT_DATE;
            INTERVIEW_WITHDRAWN_DATE = v?.INTERVIEW_WITHDRAWN_DATE;
            APPLICANT_STATUS_ID = v?.C_S_SYSTEM_VALUE1?.UUID;
            APPLICANT_ROLE_ID = v?.C_S_SYSTEM_VALUE?.UUID;
            CORRESPONDENCE_LANG_ID = v?.CORRESPONDENCE_LANG_ID;
            SUPPORTED_BY_ID = v?.SUPPORTED_BY_ID;
            RES_ADDRESS_E1 = v?.RES_ADDRESS_E1;
            RES_ADDRESS_E2 = v?.RES_ADDRESS_E2;
            RES_ADDRESS_E3 = v?.RES_ADDRESS_E3;
            RES_ADDRESS_E4 = v?.RES_ADDRESS_E4;
            RES_ADDRESS_E5 = v?.RES_ADDRESS_E5;
            RES_ADDRESS_C1 = v?.RES_ADDRESS_C1;
            RES_ADDRESS_C2 = v?.RES_ADDRESS_C2;
            RES_ADDRESS_C3 = v?.RES_ADDRESS_C3;
            RES_ADDRESS_C4 = v?.RES_ADDRESS_C4;
            RES_ADDRESS_C5 = v?.RES_ADDRESS_C5;
            OFFICE_TEL = v?.OFFICE_TEL;
            MOBILE_TEL = v?.MOBILE_TEL;
            RES_TEL = v?.RES_TEL;
            EMAIL1 = v?.EMAIL1;
            EMAIL2 = v?.EMAIL2;
            INTERVIEW_REFUSAL_DATE = v?.INTERVIEW_REFUSAL_DATE;
            if (v.Applicant_File != null)
            {
                FILE_PATH_NONRESTRICTED = RegistrationConstant.SIGNATURE_PATH + v.refNo.Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("/", "_") + "\\" +
                                    v.C_APPLICANT.SURNAME + ", " + v.C_APPLICANT.GIVEN_NAME_ON_ID + System.IO.Path.GetExtension(v.Applicant_File.FileName);
                string filePath = ApplicationConstant.CRMFilePath + RegistrationConstant.SIGNATURE_PATH + v.refNo.Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("/", "_") + "\\";
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                file.Directory.Create();
           
                v.Applicant_File.SaveAs(Path.Combine(ApplicationConstant.CRMFilePath, FILE_PATH_NONRESTRICTED));

            }
            EFORM = v.EFORM;
            FILEEXSIST = v.Applicant_File == null ? false : true;
            return this;
        }
    }

    public partial class C_COMP_APPLICANT_INFO_Extend
    {

        [Display(Name = "Communication Language")]
        public string CORRESPONDENCE_LANG_ID { get; set; }


        [Display(Name = "Role")]
        public string APPLICANT_ROLE_ID { get; set; }

        [Display(Name = "Supporting To")]
        public string SUPPORTED_BY_ID { get; set; }

        [Display(Name = "Office Telephone No.")]
        public string OFFICE_TEL { get; set; }
        [Display(Name = "Mobile Telephone No.")]
        public string MOBILE_TEL { get; set; }
        [Display(Name = "Residential Telephone No.")]
        public string RES_TEL { get; set; }
        [Display(Name = "Email")]
        public string EMAIL1 { get; set; }
        [Display(Name = "Email")]
        public string EMAIL2 { get; set; }

        [Display(Name = "Date of Acceptance")]
        public Nullable<System.DateTime> ACCEPT_DATE { get; set; }

        [Display(Name = "Status")]
        public string APPLICANT_STATUS_ID { get; set; }

        [Display(Name = "Remarks")]
        public string REMARK { get; set; }

        [Display(Name = "Date of Removal")]
        public Nullable<System.DateTime> REMOVAL_DATE { get; set; }
        [Display(Name = "Date of withdraw")]
        public Nullable<System.DateTime> INTERVIEW_WITHDRAWN_DATE { get; set; }
        [Display(Name = "Date of Refusal")]
        public Nullable<System.DateTime> INTERVIEW_REFUSAL_DATE { get; set; }


        [Display(Name = "Specimen Signature")]
        public string FILE_PATH_NONRESTRICTED { get; set; }


        /*
         * 
 public string UUID { get; set; }
        public string MASTER_ID { get; set; }
        public string APPLICANT_ID { get; set; }
        public string APPLICANT_ROLE_ID { get; set; }
        public string REMARK { get; set; }
        public Nullable<System.DateTime> ACCEPT_DATE { get; set; }
        public Nullable<System.DateTime> REMOVAL_DATE { get; set; }
        public Nullable<int> CANDIDATE_NUMBER { get; set; }
        public string LAST_INTERVIEW_NUMBER { get; set; }
        public string LAST_ASSESSMENT_NUMBER { get; set; }
        public string VETTING_STATUS { get; set; }
        public string FILE_PATH_RESTRICTED { get; set; }
        public string APPLICANT_STATUS_ID { get; set; }
        public string FILE_PATH_NONRESTRICTED { get; set; }
        public Nullable<System.DateTime> INTERVIEW_REFUSAL_DATE { get; set; }
        public Nullable<System.DateTime> INTERVIEW_WITHDRAWN_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string SUPPORTED_BY_ID { get; set; }
        public string CORRESPONDENCE_LANG_ID { get; set; }
        public Nullable<System.DateTime> CARD_EXPIRY_DATE { get; set; }
        public Nullable<System.DateTime> CARD_ISSUE_DATE { get; set; }
        public Nullable<System.DateTime> CARD_RETURN_DATE { get; set; }
        public string CARD_SERIAL_NO { get; set; }
        public string OFFICE_TEL { get; set; }
        public string MOBILE_TEL { get; set; }
        public string RES_TEL { get; set; }
        public string EMAIL1 { get; set; }
        public string EMAIL2 { get; set; }
        public string RES_ADDRESS_E1 { get; set; }
        public string RES_ADDRESS_E2 { get; set; }
        public string RES_ADDRESS_E3 { get; set; }
        public string RES_ADDRESS_E4 { get; set; }
        public string RES_ADDRESS_E5 { get; set; }
        public string RES_ADDRESS_C1 { get; set; }
        public string RES_ADDRESS_C2 { get; set; }
        public string RES_ADDRESS_C3 { get; set; }
        public string RES_ADDRESS_C4 { get; set; }
        public string RES_ADDRESS_C5 { get; set; }
         */
    }
}