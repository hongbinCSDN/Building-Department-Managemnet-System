using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.CMN.Models
{
    public class RegistrationModel
    {
       public string Type { get; set; }
       public  string CERTIFICATION_NO { get; set; }
       public DateTime? EXPIRY_DATE { get; set; }
       public string NAME { get; set; }
       public string CHINESE_NAME { get; set; }
        public List<string> AS_NAME { get; set; } = new List<string>();
        public List<string> AS_CHI_NAME { get; set; } = new List<string>();
        public string BS_ITEM { get; set; }
        public string TEL_NO { get; set; }


   



    }
}