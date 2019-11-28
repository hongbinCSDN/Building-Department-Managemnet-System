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
    public class Fn04RPT_STController : Controller
    {
        // GET: Signboard/Fn04RPT_ST
        public ActionResult Index(Fn04RPT_STSearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn04RPT_STSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchST(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        [HttpPost]
        public ActionResult Excel(Fn04RPT_STSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportST(model) });
        }
    }
}