using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class SignboardAddressSearchModel : DisplayGrid
    {
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string BuildingName { get; set; }
        public string Floor { get; set; }
        public string Flat { get; set; }
        public string District { get; set; }
     
    }
}