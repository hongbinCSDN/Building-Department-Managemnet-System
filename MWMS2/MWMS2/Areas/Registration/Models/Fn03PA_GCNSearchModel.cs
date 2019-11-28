using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;


namespace MWMS2.Areas.Registration.Models
{
    public class Fn03PA_GCNSearchModel: DisplayGrid
    {
         public string FileRef { get; set; }
        public string HKID { get; set; }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }

        public List<string> C_IND_CERTIFICATE_UUIDs { get; set; }

    }
}