using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Models
{
    public class InterviewCandidatesSearchModel : DisplayGrid
    {
        public string Year { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string FileRef { get; set; }
        public string ComName { get; set; }


        public IEnumerable<SelectListItem> showGroupList
        {
            get { return SystemListUtil.GetGroupList(); }
        }

        public IEnumerable<SelectListItem> showTypeList
        {
            get { return SystemListUtil.RetrieveTypeByCommitteeTypeID(RegistrationConstant.REGISTRATION_TYPE_MWCA); }
        }

        public IEnumerable<SelectListItem> yearList {
            get { return SystemListUtil.RetrieveYearList(); }
        }
    }

}