using MWMS2.Entity;
using MWMS2.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1110ImportRecordsModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string fileType { get; set; }
        public string fileName { get; set; }
        public ExcelPackage ExcelPackage { get; set; }
        public P_MW_IMPORTED_TASK p_MW_IMPORTED_TASK { get; set; }
        public List<P_MW_IMPORTED_TASK_ITEM> p_MW_IMPORTED_TASK_ITEMList { get; set; }
    }
}