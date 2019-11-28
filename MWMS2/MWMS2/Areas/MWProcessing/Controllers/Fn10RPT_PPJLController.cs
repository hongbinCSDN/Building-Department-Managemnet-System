using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_PPJLController : Controller
    {
        private ProcessingReportBLService _BL;

        public ProcessingReportBLService BL
        {
            get { return _BL ?? (_BL = new ProcessingReportBLService()); }
        }

        // GET: MWProcessing/Fn09RPT_PPJL
        public ActionResult Index()
        {
            return View(new Fn10RPT_PPJLModel());
        }

        public ActionResult Search(Fn10RPT_PPJLModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchRPT_PPJL(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn10RPT_PPJLModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key))return DisplayGrid.getMemory(model.Key);
            return Json( new { key = BL.ExportRPT_PPJL(model) });
        }
    }
}