using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_MRASearchModel : DisplayGrid
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Room { get; set; }

        public IEnumerable<SelectListItem> RoomList
        {
            get { return SystemListUtil.GetRoomList(); }
        }

    }
}