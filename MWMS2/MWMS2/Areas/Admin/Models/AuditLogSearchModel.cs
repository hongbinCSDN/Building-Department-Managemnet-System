using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class AuditLogSearchModel : DisplayGrid 
    {

        public string Action { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Post { get; set; }
        public string Detail { get; set; }
  
    }
}