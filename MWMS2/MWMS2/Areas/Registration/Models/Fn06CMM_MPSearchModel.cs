using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn06CMM_MPSearchModel: DisplayGrid
    {
        public string Surname{ get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
    }
}