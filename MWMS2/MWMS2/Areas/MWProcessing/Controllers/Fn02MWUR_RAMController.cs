using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_RAMController : Controller
    {

        private ProcessingMWURegistryBLService _BL;
        protected ProcessingMWURegistryBLService BL
        {
            get
            {
                return _BL ?? (_BL ?? new ProcessingMWURegistryBLService());
            }
        }

        // GET: MWProcessing/Fn02MWUR_RAM
        public ActionResult Index()
        {
            return View(new Fn02MWUR_RAMSearchModel());
        }

        public ActionResult Search(Fn02MWUR_RAMSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchMWRecordAddressMapping(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Detail(string uuid)
        {
            return View(BL.GetMWRecordDetail(uuid));
        }

        public ActionResult TransferToBravo(string mwRefNo)
        {
            return Json(BL.TransferToBravo(mwRefNo));
        }

        public ActionResult SearchItemTable(Fn02MWUR_RAMModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchItemTable(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult SearchFileRef(Fn02MWUR_RAMModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchFileRef(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult DeleteFilRef(string uuid)
        {
            return Json(BL.DeleteFilRef(uuid));
        }

        public ActionResult SaveFilRef(P_MW_FILEREF model)
        {
            return Json(BL.SaveFilRef(model));
        }
    }
}