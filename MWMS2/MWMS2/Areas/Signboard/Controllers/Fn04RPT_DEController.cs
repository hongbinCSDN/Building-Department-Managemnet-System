using MWMS2.Areas.Signboard.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_DEController : Controller
    {
        public ActionResult Index(Fn04RPT_DESearchModel model)
        {
            return View(model);
        }
        public ActionResult ExportValidationDataForSelection(Fn04RPT_DESearchModel model)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return rs.ExportValidationDataForSelection(model);
        }
        public ActionResult DataExportByExpiryDate(Fn04RPT_DESearchModel model)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return rs.ExportDataExportByExpiryDate(model);
        }
        public ActionResult SearchDataExport(Fn04RPT_DESearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchDE(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExportResultList(Fn04RPT_DESearchModel model,string UUID)
        {
            SginboardDataExportService rs = new SginboardDataExportService();
            return rs.ExportDataResult(model,UUID);
        }
    }
}