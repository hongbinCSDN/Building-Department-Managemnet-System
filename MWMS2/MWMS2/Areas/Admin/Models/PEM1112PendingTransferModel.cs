using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1112PendingTransferModel : DisplayGrid
    {
        public string Ref_No { get; set; }
        public List<string> RefNoList { get; set; }

    } 

   
}