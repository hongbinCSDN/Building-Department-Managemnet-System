using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn07CNV_CNVSearchModel : DisplayGrid, IValidatableObject
    {

        public C_APPLICANT C_APPLICANT { get; set; }

        public string registrationType { get; set; }
        //Used in Views
        public string ErrorMessage { get; set; }
        public bool result { get; set; }
        public string ErrMsg { get; set; }
        public String RegType { get; set; }

        public String Surname { get; set; }
        public string GivenName { get; set; }
       
        public String ChiName { get; set; }
        public String Sex { get; set; }
        //[RegularExpression(@"^([a-zA-Z]|[a-zA-Z][a-zA-Z0-9]|[*]|[***])[0-9]{6}[(][0-9ABC][)]$", ErrorMessage = "Wrong HKID Format")]
       
        public String HKID { get; set; }
        public String PassportNo { get; set; }

        
        public String ComName { get; set; }
        
        public String ChiComName { get; set; }
        public String ProprietorName { get; set; }
        public String BRNo { get; set; }
        public String Ref { get; set; }
        public String CompanyType { get; set; }
        public String SiteDesc { get; set; }
        public String ConvictionSource { get; set; }
        [StringLength(500)]
        public String Remarks { get; set; }
        public String RecordType { get; set; }

        public String OrdReg { get; set; }
        public Nullable<System.DateTime> DateOfOffence { get; set; }
        public Nullable<System.DateTime> DateOfJudgement { get; set; }
        public Nullable<decimal> Fine { get; set; }
        public String Accident { get; set; }
        public String Fatal { get; set; }
        public bool CrReport { get; set; }

        public String SrrAction { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> DateOfApproval { get; set; }
        public Nullable<System.DateTime> SuspFromDate { get; set; }
        public Nullable<System.DateTime> SuspToDate { get; set; }
        public String Category { get; set; }
        
        public String SrrDetails { get; set; }
        public bool SrrReport { get; set; }

        public Nullable<System.DateTime> DateOfDecision { get; set; }
        
        public String DaDetails { get; set; }
        public bool DaReport { get; set; }

        public Nullable<System.DateTime> ReceivingDate { get; set; }
        
        public String MiscDetails { get; set; }
        public bool MiscReport { get; set; }

        public String AsTdOoEngName { get; set; }
        public String AsTdOoChiName { get; set; }
        public String IDNo { get; set; }
        public String BRNumber { get; set; }
        public bool Category1 { get; set; }
        public bool Category2 { get; set; }
        public bool Category3 { get; set; }
        public bool Category4 { get; set; }
        public bool Category5 { get; set; }
        public bool Category6 { get; set; }
        public bool Category7 { get; set; }
        public string categortstring1 {
            get
            {
                string r =
                    (Category1 ? ", 1" : "") +
                    (Category2 ? ", 2" : "") +
                    (Category3 ? ", 3" : "") +
                    (Category4 ? ", 4" : "") +
                    (Category5 ? ", 5" : "") +
                    (Category6 ? ", 6" : "") +
                    (Category7 ? ", 7" : "");
                return r.Length > 0 ? r.Substring(2) : r;
            }

        }
        public String RecordClear { get; set; }
        public String CRCInterview { get; set; }
        public DateTime? CRCInterviewDt { get; set; }


        //Used in CNV_Service
        public String ComType { get; set; }
        public String SuspReason { get; set; }
        public String CnvSource { get; set; }
        public DateTime? FromDateOfOffence { get; set; }
        public DateTime? ToDateOfOffence { get; set; }
        public DateTime? FromDateOfJudgement { get; set; }
        public DateTime? ToDateOfJudgement { get; set; }
        public String CnvType { get; set; }
        public String DateFrom { get; set; }
        public String DateTo { get; set; }
        public String Date { get; set; }
        public String SelectRegType { get; set; }
        public String Source { get; set; }
        public String ImportType { get; set; }


        public IEnumerable<SelectListItem> RegTypeList
        {
            get { return SystemListUtil.RetrieveCompInd(); }
        }

        public IEnumerable<SelectListItem> SexList
        {
            get { return SystemListUtil.RetrieveGender(); }
        }

        public IEnumerable<SelectListItem> YesNoList
        {
            get { return SystemListUtil.RetrieveYesNo(); }
        }
        public IEnumerable<SelectListItem> YesNoListWithNoIndication
        {
            get { return SystemListUtil.RetrieveYesNoExtra(); }
        }

        public IEnumerable<SelectListItem> CompanyTypeList
        {
            get { return SystemListUtil.GetCompanyTypeList(); }
        }

        public IEnumerable<SelectListItem> ConvictionSourceList
        {
            get { return SystemListUtil.GetConvictionSourceList(); }
        }

       

        public List<SelectListItem> RetrieveCNVSORUCE()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE)
                                     select new SelectListItem()
                                     {
                                         Text = sv.ENGLISH_DESCRIPTION,
                                         Value = sv.UUID,
                                     }

             );

            return selectListItems;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
    
            if (RegType == "CGC" || RegType == "CMW")
            {
                if (string.IsNullOrWhiteSpace(ComName))
                {
                    results.Add(new ValidationResult("Company Name (Eng) is required", new string[] { "ComName" }));

                }
            }

            if (RegType == "IP" || RegType == "IMW")
            {
                if (string.IsNullOrWhiteSpace(Surname))
                {
                    results.Add(new ValidationResult("Surname is required", new string[] { "Surname" }));
                }

                if (string.IsNullOrWhiteSpace(GivenName))
                {
                    results.Add(new ValidationResult("Given Name is required", new string[] { "GivenName" }));
                }

                if (string.IsNullOrWhiteSpace(HKID))
                {
                    results.Add(new ValidationResult("HKID is required", new string[] { "HKID" }));
                }
            }
           
            return results;
            //return new List<ValidationResult>();
        }

        public string getDecryptHKID()
        {
            if (C_APPLICANT != null)
                return EncryptDecryptUtil.getDecryptHKID(C_APPLICANT.HKID);
            else
                return null;
        }


    }
}