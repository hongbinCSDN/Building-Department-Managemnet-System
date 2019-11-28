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
    public class Fn04RPT_UPSRController : Controller
    {
        public ActionResult Index(Fn04RPT_UPSRSearchModel model)
        {
            return View("Index", model);
        }
        public ActionResult Search(Fn04RPT_UPSRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchUPSR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn04RPT_UPSRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportUPSR(model) });
        }
    }
}