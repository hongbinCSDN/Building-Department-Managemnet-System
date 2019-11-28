using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class DisplayGridColumn
    {
        public bool display { get; set; } = true;
        public string field { get; set; }
        public string columnName { get; set; }
    }
}