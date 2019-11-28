using MWMS2.Areas.Signboard.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_SVSBHController : Controller
    {
        public ActionResult Index(Fn04RPT_SVSBHSearchModel model)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return View("Index", rs.SVSBHIndex(model));
        }
        public ActionResult ExportSVSYear(Fn04RPT_SVSBHSearchModel model)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return rs.ExportSVSYearReport(model);
        }
        public ActionResult ExportSVSUnit(Fn04RPT_SVSBHSearchModel model)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return rs.ExportSVSUnitReport(model);
        }
    }
}