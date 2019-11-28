using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_MWIASearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string RegNo { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string SurnName { get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
        public string TelNo { get; set; }
        public string Email { get; set; }
        public bool KeywordSearch { get; set; }
        public string PNAP { get; set; }
        public string ServicesInBuidingSafety { get; set; }
        public string TypeOfDate { get; set; }
        public IEnumerable<SelectListItem> DateTypeList
        {
            get { return SystemListUtil.GetDateTypeList(); }
        }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }



        public string Sex { get; set; }
        public string MWCap { get; set; }

        public List<SelectListItem> retrieveServiceInBSByRegType()
        {
            return SystemListUtil.RetrieveServiceInBSByRegType( RegistrationConstant.REGISTRATION_TYPE_MWIA);
        }
        public List<SelectListItem> retrievePNAPByType()
        {
            return SystemListUtil.RetrievePNAPByType();
        }

    }
}