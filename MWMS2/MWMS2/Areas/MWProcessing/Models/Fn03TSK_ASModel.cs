using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn03TSK_ASModel : DisplayGrid
    {
        public string StreetRoadVillage { get; set; }
        public string StreetLotNumber { get; set; }
        public string BuildingEstate { get; set; }
        public string Floor { get; set; }
        public string FlatRoom { get; set; }
        public string District { get; set; }
        public string Region { get; set; }
    }
}