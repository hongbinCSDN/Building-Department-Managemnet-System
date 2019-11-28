using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using OfficeOpenXml;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn05IIF_IIFModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string FileName { get; set; }
        public string ImportType { get; set; }

        public P_IMPORT_36 Import36 { get; set; }
        public P_IMPORT_36_ITEM Import36Item { get; set; }
        public List<P_IMPORT_36_ITEM> Import36ItemList { get; set; }
        public ExcelPackage ExcelPackage { get; set; }

    }
}