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

    public class CompSearchModel : DisplayGrid
    {
      
        public string FileRef { get; set; }
        public string RegNo { get; set; }
        public string CompNameEng { get; set; }
        public string CompNameChn { get; set; }
        public string AddressEng { get; set; }
        public string AddressChn { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string TelNo { get; set; }
        public string Email { get; set; }
        public bool KeywordSearch { get; set; }
        public string ChineseName { get; set; }
        public string BrNo { get; set; }

        public string Pnrc { get; set; }
        public IEnumerable<SelectListItem> PnrcList {  get; } = SystemListUtil.RetrievePNAPByType();

        public string ServiceInBS { get; set; }
        public IEnumerable<SelectListItem> ServiceInBSList { get; } = SystemListUtil.RetrieveServiceInBSByRegType(RegistrationConstant.REGISTRATION_TYPE_CGA);


        public string DateType { get; set; }
        public IEnumerable<SelectListItem> DateTypeList { get; } = SystemListUtil.GetDateTypeList();

        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public string RegType { get; set; }

        public string MWCap { get; set; }
        public IEnumerable<SelectListItem> MWCapList { set; get; } = SystemListUtil.RetrieveMWCap();
        public string MWType { get; set; }
        public IEnumerable<SelectListItem> MWTypeList { set; get; } = SystemListUtil.RetrieveMWTypeByClass("Class 1");

    }
}