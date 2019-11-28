using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_SearchResultModel : DisplayGrid
    {
        public string MWNo { get; set; }
        public P_MW_ACK_LETTER FinalRecord { get; set; }
        public List<P_MW_ACK_LETTER> ACKRecords { get; set; }
    }
    public class Fn01LM_SearchMWLModel : DisplayGrid
    {
        public string MWNo { get; set; }
        public List<Fn01LM_SearchResultModel> ResultModels { get; set; }
    }
}