using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_SADDisplayModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string DSN { get; set; }
        public string SubDocNo { get; set; }
        public string Form { get; set; }
        public string SubmissionType { get; set; }
        [Display(Name = "SSP Submitted")]
        public string SSPSubmitted { get; set; }
    }
}