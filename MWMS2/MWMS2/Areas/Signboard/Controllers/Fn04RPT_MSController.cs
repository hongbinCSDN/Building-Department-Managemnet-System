using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_MSController : Controller
    {
        public ActionResult Index(Fn04RPT_MSSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            return View("Index", rs.IndexMS(model));
        }
        public ActionResult SearchMS(Fn04RPT_MSSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            //rs.ViewMSRecord(model);
            //return Content(JsonConvert.SerializeObject(rs.SearchMR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            return View("Form", rs.ViewMSRecord(model));
        }
        [HttpPost]
        public ActionResult Excel(Fn04RPT_MSSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return rs.ExportMS(model);
        }
    }
}