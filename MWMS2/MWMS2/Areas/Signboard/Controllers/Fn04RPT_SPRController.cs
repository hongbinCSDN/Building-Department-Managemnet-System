using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_SPRController : Controller
    {
        public ActionResult Index(Fn04RPT_SPRSearchModel model)
        {
            return View(model);
        }
        public ActionResult SearchSPR(Fn04RPT_SPRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return View("Form",model);
        }
        public ActionResult SubmissionProgressReport(Fn04RPT_SPRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SPReport(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult SubmissionCountReport(Fn04RPT_SPRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SCReport(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SubmissionInvolvedReport(Fn04RPT_SPRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SIReport(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult SPExcel(Fn04RPT_SPRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportSPReport(model) });
        }
        public ActionResult SCExcel(Fn04RPT_SPRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportSCReport(model) });
        }
        public ActionResult SIExcel(Fn04RPT_SPRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportSIReport(model) });
        }
    }
}