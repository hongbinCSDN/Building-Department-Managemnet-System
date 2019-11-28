using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_MWListModel:DisplayGrid
    {
        public string DSNOrMWNo { get; set; }
    }
}