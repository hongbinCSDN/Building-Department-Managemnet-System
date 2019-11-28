using MWMS2.Areas.Admin.Models;
using MWMS2.Services.AdminService.BL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103ImportMWItemController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }
        // GET: Admin/PEM1103ImportMWItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(PEM1103ImportMWItemModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchImportMWItem(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Edit(string uuid)
        {
            return View(BL.SearchDetailImportMWItem(uuid));
        }

        public ActionResult UpdateImportMwItemDescription(PEM1103ImportMWItemModel model)
        {
            return Json(BL.UpdateImportMwItemDescription(model)); 
        }
    }
}