using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_ERController : Controller
    {
        // GET: MWProcessing/Fn10RPT_ER
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search( )
        {
            ProcessingErrorService ss = new ProcessingErrorService();
            Fn10RPT_ERModel model = new Fn10RPT_ERModel();

            return Content(JsonConvert.SerializeObject(ss.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn10RPT_ERModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            ProcessingErrorService ss = new ProcessingErrorService();

            return Json(new { key = ss.Export(model) });
        }
    }
}