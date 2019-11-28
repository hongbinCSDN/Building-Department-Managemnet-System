using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_ODLController : Controller
    {
        private ProcessingReportBLService _BL;

        public ProcessingReportBLService BL
        {
            get { return _BL ?? (_BL = new ProcessingReportBLService()); }
        }
        // GET: MWProcessing/Fn10RPT_ODL
        public ActionResult Index()
        {
            return View(new Fn10RPT_ODLModel());
        }

        public ActionResult Search(Fn10RPT_ODLModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetOutstandingDocFromList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(Fn10RPT_ODLModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Fn10RPT_ODLExcel(model) });
        }
    }
}