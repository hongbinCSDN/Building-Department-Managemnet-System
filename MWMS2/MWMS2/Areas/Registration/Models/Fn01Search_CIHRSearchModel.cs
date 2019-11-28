using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_CIHRSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public IEnumerable<SelectListItem> VetOfficerList
        {
            get { return SystemListUtil.GetVetCIHROfficer(); }
        }
        public string VettingOfficer { get; set; }
        public DateTime? DateFromReceived { get;set;}
        public DateTime? DateToReceived { get; set; }
        public DateTime? DateFromDue { get; set; }
        public DateTime? DateToDue { get; set; }
        public IEnumerable<SelectListItem> TypeOfRegistersList { set; get; } = SystemListUtil.GetTypeOfRegisters();
        public string TypeOfRegisters { get; set; }
        public IEnumerable<SelectListItem> TypeOfApplicationList_GBC
        {
            get
            {
                return SystemListUtil.GetSVListByRegType(RegistrationConstant.REGISTRATION_TYPE_CGA);
            }
        }
        public IEnumerable<SelectListItem> TypeOfApplicationList_MWC
        {
            get
            {
                return SystemListUtil.GetSVListByRegType(RegistrationConstant.REGISTRATION_TYPE_MWCA);
            }
        }
        public IEnumerable<SelectListItem> TypeOfApplicationList_IP
        {
            get
            {
                return SystemListUtil.GetSVListByRegType(RegistrationConstant.REGISTRATION_TYPE_IP);
            }
        }
        public IEnumerable<SelectListItem> TypeOfApplicationList_MWCW
        {
            get
            {
                return SystemListUtil.GetSVListByRegType(RegistrationConstant.REGISTRATION_TYPE_MWIA);
            }
        }
        public string TypeOfApplication { get; set; }

        public string application_type { get; set; }
    }
}