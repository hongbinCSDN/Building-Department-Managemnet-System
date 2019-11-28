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

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_DeModel : DisplayGrid
    {
        public string Outstanding { get; set; }
        [Display(Name = "Document S/N")]
        public string SearchDsn { get; set; }
        [Display(Name = "Ref. No.")]
        public string SearchRecordId { get; set; }
        [Display(Name = "MWU Received Date From")]
        public DateTime? SearchReceiveDateFrom { get; set; }
        [Display(Name = "To")]
        public DateTime? SearchReceiveDateTo { get; set; }
        [Display(Name = "Status")]
        public string SearchStatus { get; set; }
        public List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text="ALL",Value=""
                }
                //,new SelectListItem()
                //{
                //    Text=ProcessingConstant.DSN_DISPLAY_FIRST_ENTRY_COMPLETED
                //    ,Value=ProcessingConstant.FIRST_ENTRY_COMPLETED
                //}
                ,new SelectListItem()
                {
                    Text=ProcessingConstant.DISPLAY_SECOND_ENTRY
                    ,Value=ProcessingConstant.SECOND_ENTRY
                }
                ,new SelectListItem()
                {
                    Text=ProcessingConstant.DISPLAY_WILL_SCAN
                    ,Value=ProcessingConstant.WILL_SCAN
                }
            };
        }
    }

    public class Fn02MWUR_DeScanModel : DisplayGrid
    {
        public P_MW_DSN P_MW_DSN { get; set; }

        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTs { get; set; }
    }


    //public class Fn02MWUR_DeFormHeaderVM
    //{
    //    public P_MW_RECORD P_MW_RECORD { get; set; }
    //    public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }
    //}
}