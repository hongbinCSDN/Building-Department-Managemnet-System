using MWMS2.Constant;
using MWMS2.Entity;

using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_DFRDisplayModel
    {
        public C_IND_PROCESS_MONITOR C_IND_PROCESS_MONITOR { get; set; }
        public C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        public C_COMP_PROCESS_MONITOR C_COMP_PROCESS_MONITOR { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public C_S_SYSTEM_VALUE S_INTERVIEW_RESULT { get; set; }
        public C_S_SYSTEM_VALUE S_APPLICATION_FORM { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_S_SYSTEM_VALUE SV_ROLE { get; set; }

        public String showIndApplicatName()
        {
            String appName = "";
            if (C_APPLICANT != null)
            {
                String gender = C_APPLICANT.GENDER;
                String surname = C_APPLICANT.SURNAME;
                String givenName = C_APPLICANT.GIVEN_NAME_ON_ID;

                if (RegistrationConstant.GENDER_M.Equals(gender))
                {
                    appName += "Mr";
                }
                else {
                    appName += "Miss";
                }

                appName += String.IsNullOrEmpty(surname) ? "" : " ";
                appName += String.IsNullOrEmpty(surname) ? "" : surname;
                appName += String.IsNullOrEmpty(givenName) ? "" : " ";
                appName += String.IsNullOrEmpty(givenName) ? "" : givenName;
            }
            return appName;
        }

        public String showCandidateName()
        {
            String candidateName = "";
            if (C_APPLICANT != null)
            {
                String surname = C_APPLICANT.SURNAME;
                String givenName = C_APPLICANT.GIVEN_NAME_ON_ID;

                candidateName += String.IsNullOrEmpty(surname) ? "" : surname;
                candidateName += String.IsNullOrEmpty(givenName) ? "" : " ";
                candidateName += String.IsNullOrEmpty(givenName) ? "" : givenName;
            }
            return candidateName;
        }
    }
}