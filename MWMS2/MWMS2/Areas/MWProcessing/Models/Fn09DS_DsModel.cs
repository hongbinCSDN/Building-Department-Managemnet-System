using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn09DS_DsModel : DisplayGrid
    {

        public string DSN { get; set; }
        public string TaskID { get; set; }

        public string GEN_NUM { get; set; }


        public string GeneratedNumber { get; set; }

        public P_MW_DSN p_mw_dsn { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENT_LIST { get; set; }

        public string RE_ASSIGNMENT { get; set; }



        public string errorMsg { get; set; }

        public List<SelectListItem> submissionTypeList
        {
            get
            {
                return
                new List<SelectListItem>()
               {   new SelectListItem{Value = ProcessingConstant.SUBMIT_TYPE_ENQ , Text=ProcessingConstant.SUBMIT_TYPE_ENQ },
                  new SelectListItem{Value = ProcessingConstant.SUBMIT_TYPE_COM , Text=ProcessingConstant.SUBMIT_TYPE_COM}
            };
            }
        }

        public string submissionType { get; set; }
        public string refNumber { get; set; }
    }
}