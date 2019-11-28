using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_JADisplayModel : DisplayGrid
    {
        public List<List<object>> CurrentUserCountList { get; set; } // table at top
        public List<JobAssignmentModel> ResultList { get; set; } // table at bottom
        public List<SelectListItem> PoLookUpList { get; set; }
        public List<SelectListItem> ToLookUpList { get; set; }
        public List<SelectListItem> SpoLookUpList { get; set; }
        public string SaveMode { get; set; }

        public int ResultListSize { get; set; }

        //public string ToLevel { get; set; }
        //public string PoLevel { get; set; }
        //public string SpoLevel { get; set; }

        public Dictionary<string, string> UuidList { get; set; } // B_SV_RECORD:UUID
        public Dictionary<string, string> ToUserId { get; set; } // B_SV_RECORD:UUID maps with SYS_POST:UUID
        public Dictionary<string, string> PoUserId { get; set; }
        public Dictionary<string, string> SpoUserId { get; set; }

    }
}