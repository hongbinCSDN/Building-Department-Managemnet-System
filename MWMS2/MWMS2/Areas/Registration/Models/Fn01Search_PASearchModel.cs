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
    public class Fn01Search_PASearchModel : DisplayGrid
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
        public string Address { get; set; }
        public string CatCode { get; set; }
        public string ServicesInBuidingSafety { get; set; }
        public string PNAP { get; set; }
        public string TypeOfDate { get; set; }
        public IEnumerable<SelectListItem> DateTypeList
        {
            get { return SystemListUtil.GetDateTypeList(); }
        }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Sex { get; set; }
        public List<string> SelectedPRBList { get; set; }

        public IList<string> SelectedQC { get; set; }
        public IList<string> SelectedQualificationDetails { get; set; }
        public List<string> DisciplinesDiv { get; set; }
        public bool KeywordSearch { get; set; }
        public List<SelectListItem> RetrieveCategoryCode()
        {
            return SystemListUtil.RetrieveCategoryCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_IP);
        }
        public List<SelectListItem> RetrieveServiceInBSByRegType()
        {
            return SystemListUtil.RetrieveServiceInBSByRegType(RegistrationConstant.REGISTRATION_TYPE_IP);
        }
        public List<SelectListItem> RetrievePNAPByType()
        {
            return SystemListUtil.RetrievePNAPByType();
        }
        public List<SelectListItem> RetrievePRBByType()
        {
            return (from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD)
                                   select new SelectListItem()
                                   {
                                       Text = sv.ENGLISH_DESCRIPTION,
                                       Value = sv.UUID,
                                   }
                    ).ToList();
        }
        public IList<SelectListItem> RetrieveQCByType()
        {
            return SystemListUtil.RetrieveQCByType(RegistrationConstant.REGISTRATION_TYPE_IP);
        }
        public List<SelectListItem> RetrieveDisDivByType()
        {
            return SystemListUtil.RetrieveDisDivByType();
        }
    }
}