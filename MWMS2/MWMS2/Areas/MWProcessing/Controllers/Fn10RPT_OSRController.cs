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
    public class Fn10RPT_OSRController : Controller
    {
        //ProcessingOSRBLService
        private ProcessingOSRBLService BLService;
        protected ProcessingOSRBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingOSRBLService()); }
        }
        // GET: MWProcessing/Fn10RPT_OSR
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn10RPT_OSRModel model)
        {
            BL.Search(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Excel(Fn10RPT_OSRModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.Excel(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

















    }
}