﻿using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class HKIDPASSPORTDetailModel : DisplayGrid
    {

        public string HKID  { get; set; }
        public string PASSPORT { get; set; }
        public string ErrorMessage { get; set; }
    }
}