using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_ESRController : Controller
    {
        // GET: MWProcessing/Fn09RPT_ESR
        public ActionResult Index()
        {
            Fn10RPT_ESRSearchModel model = new Fn10RPT_ESRSearchModel();
            DateTime now = DateTime.Now;
            model.ScanDateFrom = new DateTime(now.Year, now.Month, 1);
            model.ScanDateTo = new DateTime(now.Year, now.Month, now.Day);

            return View(model);
        }

        public ActionResult Search(Fn10RPT_ESRSearchModel model)
        {
            ProcessingEformReportService rs = new ProcessingEformReportService();
            return Content(JsonConvert.SerializeObject(rs.SearchRPT_ESR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn10RPT_ESRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            ProcessingEformReportService rs = new ProcessingEformReportService();
            return Json(new { key = rs.ExportRPT_ESR(model) });
        }
    }
}