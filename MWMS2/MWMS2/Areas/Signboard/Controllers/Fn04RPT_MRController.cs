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
    public class Fn04RPT_MRController : Controller
    {
        public ActionResult Index()
        {
            SignboardSearchService rs = new SignboardSearchService();
            Fn04RPT_MRSearchModel model = new Fn04RPT_MRSearchModel();
            return View(rs.IndexMR(model)); 
           // return View(rs.IndexMR(model));
        }
        public ActionResult SearchMR(Fn04RPT_MRSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            //rs.ViewSvRecord(model);
            //return Content(JsonConvert.SerializeObject(rs.SearchMR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            return View("Form", rs.ViewMRRecord(model));
        }
        [HttpPost]
        public ActionResult Excel(Fn04RPT_MRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return rs.ExportMR(model);
        }
    }
}