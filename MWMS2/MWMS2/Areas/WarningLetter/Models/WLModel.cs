using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.WarningLetter.Models
{
    public class WLModel : DisplayGrid
    {

        public string SUBJECT { get; set; }
        public string REGISTRATION_NO { get; set; }
        public string COMP_CONTRACTOR_NAME_ENG { get; set; }

        public DateTime? SearchString_CreateStartDate { get; set; }
        public DateTime? SearchString_CreateEndDate { get; set; }
        public DateTime? SearchString_IssuedStartDate { get; set; }
        public DateTime? SearchString_IssuedEndDate { get; set; }

        public string Section { get; set; }
        public string CASE_OFFICER { get; set; }
        public string UPDATE_OFFICER { get; set; }
        public string FILE_REF_FOUR { get; set; }
        public string FILE_REF_TWO { get; set; }
        public string REMARK { get; set; }


        public string FOLIO { get; set; }
        public string NOTICE_NO { get; set; }
        public string MW_SUBMISSION_NO { get; set; }

        public string[] v_Offense_Type_CheckBox { get; set; }
        public string[] v_Source_checkbox { get; set; }
        public string[] COM_Creation_Status { get; set; }
        public string[] AS_Creation_Status { get; set; }

        public List<W_S_SYSTEM_VALUE> IrrTechType { get; set; }
        public List<W_S_SYSTEM_VALUE> IrrProType { get; set; }
        public List<W_S_SYSTEM_VALUE> IrrMisType { get; set; }

        public List<W_S_SYSTEM_VALUE> GetWLOffenseType()
        {
            return SystemListUtil.GetWLOffenseType();
        }
        public List<SelectListItem> GetSections()
        {
            return SystemListUtil.GetSections();
        }

    }
}