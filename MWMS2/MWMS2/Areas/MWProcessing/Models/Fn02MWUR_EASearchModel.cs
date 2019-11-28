using MWMS2.Constant;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_EASearchModel : DisplayGrid
    {
        // --- Search Criteria ---
        public string MwNo { get; set; }
        public string DSN { get; set; }
        public string EfssNo { get; set; }
        public string ErrMsg { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> StatusList = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "- All -", Value = "0" },
            new SelectListItem() { Text = "Submitted", Value = "1" },
            new SelectListItem() { Text = "Not yet submitted", Value = "2" }
        };
        public string EfssStatus { get; set; }
        public List<SelectListItem> EfssStatusList = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "- All -", Value = "0" },
            new SelectListItem() { Text = "ACK Letter", Value = "ACK" },
            new SelectListItem() { Text = "Direct Return", Value = "R" }
        };

        public Fn01LM_AckSearchModel ackModel { get; set; }

        public string EFSS_STATUS_ACK = ProcessingConstant.EFSS_STATUS_ACK;
        public string EFSS_STATUS_DIRECT_RETURN = ProcessingConstant.EFSS_STATUS_DIRECT_RETURN;

        // for ACK
        public string ACK_EFSS_ID { get; set; }
        public string ACK_FORM_CODE { get; set; }
        public string ACK_DSN { get; set; }
        public string ACK_MW_SUBMISSION { get; set; }

        public string ACKorDR { get; set; }
    }

    public class Fn02MWUR_EAAckLetterModel
    {
        public DateTime? COMP_DATE { get; set; }
        public string MW_ITEM { get; set; }
        public string PBP_NO { get; set; }
        public string EMAIL_OF_PBP { get; set; }
        public string PRC_NO { get; set; }
        public string EMAIL_OF_PRC { get; set; }
        public DateTime? COMM_DATE { get; set; }
        public string ADDRESS { get; set; }
        public string STREET { get; set; }
        public string STREET_NO { get; set; }
        public string BUILDING { get; set; }
        public string FLOOR { get; set; }
        public string UNIT { get; set; }
        public string PAW { get; set; }
        public string PAW_CONTACT { get; set; }

        public string Separator = "/";
        public string Space = " ";
    }
}