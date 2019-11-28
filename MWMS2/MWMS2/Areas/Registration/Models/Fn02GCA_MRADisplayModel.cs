using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_MRADisplayModel : DisplayGrid
    {
        public string InterviewDate { get; set; }
        public string Session { get; set; }
        public string Room { get; set; }
        public string ComGrp { get; set; }
        public string CancelMeeting { get; set; }

        public IEnumerable<SelectListItem> SessionList { set; get; } = SystemListUtil.RetrievePNAPByType();
        public IEnumerable<SelectListItem> RoomList { set; get; } = SystemListUtil.RetrievePNAPByType();
        public IEnumerable<SelectListItem> ComGrpList { set; get; } = SystemListUtil.RetrievePNAPByType();
        public IEnumerable<SelectListItem> CancelList { set; get; } = SystemListUtil.RetrievePNAPByType();

    }
}