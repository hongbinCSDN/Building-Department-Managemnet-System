using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_CASearchModel: DisplayGrid
    {
        public string FileRef { get; set; }
        public string BRNo { get; set; }
        public string ComName { get; set; }
       

    }
}