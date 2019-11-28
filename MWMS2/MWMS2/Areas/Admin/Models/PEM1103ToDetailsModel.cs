using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103ToDetailsModel
    {
        public List<PEM1103ToDetailListModel> pEM1103ToDetailsListModels { get; set; }
        public IEnumerable<SelectListItem> ToPostList
        {
            get;set;
        }  =  SystemListUtil.GetPEMOfficerUserListBy(Constant.ProcessingConstant.TO);

        public IEnumerable<SelectListItem> PoPostList
        {
            get; set;
        } = SystemListUtil.GetPEMOfficerUserListBy(Constant.ProcessingConstant.PO);

        public IEnumerable<SelectListItem> SpoPostList
        {
            get; set;
        } = SystemListUtil.GetPEMOfficerUserListBy(Constant.ProcessingConstant.SPO);
    }

    public class PEM1103ToDetailListModel
    {
        public string UUID { get; set; }
        public string TO_POST { get; set; }
       
        public string TO_POST_ENG { get; set; }
        public string TO_POST_CHI { get; set; }
        public string TO_NAME_ENG { get; set; }
        public string TO_NAME_CHI { get; set; }
        public string TO_CONTACT { get; set; }
        public string PO_POST { get; set; }
      
        public string PO_POST_ENG { get; set; }
        public string PO_POST_CHI { get; set; }
        public string PO_NAME_ENG { get; set; }
        public string PO_NAME_CHI { get; set; }
        public string PO_CONTACT { get; set; }
        public string SPO_POST { get; set; }
       
        public string SPO_POST_ENG { get; set; }
        public string SPO_POST_CHI { get; set; }
        public string SPO_NAME_ENG { get; set; }
        public string SPO_NAME_CHI { get; set; }
        public string SPO_CONTACT { get; set; }
        public string ISACTIVE { get; set; }
        public bool IsCheckActive { get; set; }
    }
}