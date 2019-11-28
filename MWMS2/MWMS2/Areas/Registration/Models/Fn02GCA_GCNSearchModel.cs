using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_GCNSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        
        public string ComName { get; set; }

        public string RegType { get; set; }

        public List<string> C_COMP_APPLICANT_INFO_UUIDs { get; set; }

    }
}