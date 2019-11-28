using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class SignboardRecordCommonDisplayModel : DisplayGrid
    {
        public B_SV_VALIDATION B_SV_VALIDATION { get; set; }
        public B_SV_RECORD B_SV_RECORD { get; set; }
        public string SelectedLetterType { get; set; }

        public string SelectedLetter { get; set; }
        public List<SelectListItem> getLetterType { get { return SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SYSTEM_TYPE_LETTER_TYPE, 0); } set { } }



        public List<SelectListItem> getLetter { get { return SystemListUtil.GetLetter(); } set { } }
       
        public IEnumerable<SelectListItem> TO_Handling_Officer_List
        {
            get { return SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_TO); }
        }
    }
}