using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class CommentModel
    {
        public bool IsRollback { get; set; }
        public string RECORD_ID { get; set; }
        public string COMMENT_AREA { get; set; }
        public string V_UUID { get; set; }
        public string SubmissionType { get; set; }
        public string HandlingUnit { get; set; }
        public bool IsSPO { get; set; }
        public List<P_MW_COMMENT> P_MW_COMMENTs { get; set; }
        //public P_MW_COMMENT P_MW_COMMENT { get; set; }
    }
}