﻿using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_NCCWIModel : DisplayGrid
    {
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
    }
}