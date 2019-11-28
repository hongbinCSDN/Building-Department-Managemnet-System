using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Models
{
    public class ProcessMonitorSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string ComName { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
    }
}