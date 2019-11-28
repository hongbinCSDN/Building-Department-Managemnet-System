using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class ProcessingMwItemModel
    {
        public string MWItemTypeUUID { get; set; }
        public string MWItemType { get; set; }
        public string MWItemUUID { get; set; }
        public string MWItemNo { get; set; }
    }
}