using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_SearchModel : DisplayGrid
    {
        public string DSNOrMWNo { get; set; }
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
        public string LetterDateFrom { get; set; }
        public string LetterDateTo { get; set; }
        public string BeginWorkFrom { get; set; }
        public string BeginWorkTo { get; set; }
        public string FileReferenceNo { get; set; }
        public string MwAckLetterFixed { get; set; }
        public string CommDateFrom { get; set; }
        public string CommDateTo { get; set; }
        public string CompDate1 { get; set; }
        public string CompDate2 { get; set; }
        public bool IsLetterDateNull { get; set; }
        public bool IsCompDateNull { get; set; }
        public bool IsReferralDateNull { get; set; }
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
    }
}