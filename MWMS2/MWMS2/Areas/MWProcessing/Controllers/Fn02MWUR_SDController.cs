using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
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
    public class Fn02MWUR_SDController : Controller
    {

        private ProcessingMWURegistryBLService _BL;
        protected ProcessingMWURegistryBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingMWURegistryBLService());
            }
        }
        // GET: MWProcessing/Fn02MWRU_SD
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn02MWUR_SDModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchSD(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }
        [HttpPost]
        public ActionResult Excel(Fn02MWUR_SDModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
        
            
            return Json(new { key = BL.ExportSD(model) });
        }
        public ActionResult Detail(string uuid)
        {
            //_BL
            return View(BL.DetailSD(uuid));
        }

        public ActionResult SearchDoc(Fn02MWUR_SDDisplayModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult CompletScan(string uuid)
        {
            return Json(BL.CompletScan(uuid));
        }
    }
}