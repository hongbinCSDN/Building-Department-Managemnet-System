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
    public class Fn10RPT_SEWSAController : Controller
    {
        private ProcessingSEWSAService BLService;
        protected ProcessingSEWSAService BL
        {
            get { return BLService ?? (BLService = new ProcessingSEWSAService()); }
        }
        // GET: MWProcessing/Fn10RPT_SEWSA
        public ActionResult Index()
        {
            return View();
        }
        /*public ActionResult Search(Fn10RPT_SEWSAModel model)
        {
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }*/
        public ActionResult Search(Fn10RPT_SEWSAModel model)
        {
            BL.Search(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Excel(Fn10RPT_SEWSAModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.Excel(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}