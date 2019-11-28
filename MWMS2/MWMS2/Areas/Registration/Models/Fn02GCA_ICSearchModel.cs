using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_ICSearchModel : DisplayGrid
    {
        public string Year { get; set; }
        public string Date { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
       

    }
}