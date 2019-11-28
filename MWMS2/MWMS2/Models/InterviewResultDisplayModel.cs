using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Models
{
    // Common model for Prof, MWC(W), GBC and MWC
    public class InterviewResultDisplayModel : DisplayGrid, IValidatableObject
    {
        public C_INTERVIEW_CANDIDATES C_INTERVIEW_CANDIDATES { get; set; }
        public C_IND_CONVICTION C_IND_CONVICTION { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        public C_INTERVIEW_SCHEDULE C_INTERVIEW_SCHEDULE { get; set; }
        public C_MEETING C_MEETING { get; set; }
        public C_COMMITTEE_GROUP C_COMMITTEE_GROUP { get; set; }
        public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        

        public String ImportType { get; set; }

        public List<SelectListItem> getYesNo()
        {
            return SystemListUtil.RetrieveYesNo();
        }


        public List<SelectListItem> getResultTypeList()
        {
            return SystemListUtil.RetrieveResultType();
        }



       
        public IEnumerable<SelectListItem> showGroupList
        {
            get { return SystemListUtil.GetGroupList(); }
        }

        public IEnumerable<SelectListItem> showTypeList
        {
            get { return SystemListUtil.RetrieveCategoryListByRegType(RegistrationConstant.REGISTRATION_TYPE_CGA); }
        }

        public IEnumerable<SelectListItem> yearList {
            get { return SystemListUtil.RetrieveYearList(); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            //if (RegType == "CGC" || RegType == "CMW")
            //{
            if (string.IsNullOrWhiteSpace(C_INTERVIEW_CANDIDATES.RESULT_ID))
            {

                results.Add(new ValidationResult("Result is required", new string[] { "Result" }));

            }
            //}





            //if (RegType == "IP" || RegType == "IMW")
            //{
            //    if (string.IsNullOrWhiteSpace(Surname))
            //    {
            //        results.Add(new ValidationResult("Surname is required", new string[] { "Surname" }));
            //    }

            //    if (string.IsNullOrWhiteSpace(GivenName))
            //    {
            //        results.Add(new ValidationResult("Given Name is required", new string[] { "GivenName" }));
            //    }

            //    if (string.IsNullOrWhiteSpace(HKID))
            //    {
            //        results.Add(new ValidationResult("HKID is required", new string[] { "HKID" }));
            //    }
            //}

            return results;
            //return new List<ValidationResult>();
        }




    }

}