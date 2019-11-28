using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class EditFormModel : DisplayGrid
    {
        public long EditFormKey { get; set; }
    }
}