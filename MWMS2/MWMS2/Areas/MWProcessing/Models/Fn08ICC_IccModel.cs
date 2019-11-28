using MWMS2.Constant;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn08ICC_IccModel: PEM_BaseModel
    {

        public Fn08ICC_IccModel()
        {
            typeList = new List<SelectListItem>()
               {   new SelectListItem{Value = ProcessingConstant.SUBMIT_TYPE_ENQ , Text=ProcessingConstant.SUBMIT_TYPE_ENQ },
                  new SelectListItem{Value = ProcessingConstant.SUBMIT_TYPE_COM , Text=ProcessingConstant.SUBMIT_TYPE_COM}
            };
        }

        public IEnumerable<SelectListItem> UserList
        {
            get; set;
        } = SystemListUtil.GetPEMOfficerUserListBy(Constant.ProcessingConstant.PO);


        public bool isSaved { get; set; }

        public string iccNo { get; set; }
        public string assignNo { get; set; }
        public string assignType { get; set; }
        public string assignOfficerType { get; set; }
        public string assignedOfficer { get; set; }

        public List<SelectListItem> typeList { set; get; }
        public List<SelectListItem> officeList { set; get; }
    }
}