using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103ImportMWItemModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string Item_No { get; set; }
        public string English_Description { get; set; }
        public string Chinese_Description { get; set; }
        public bool Image_Not_Transfer_To_RRM { get;set; }
    }
}