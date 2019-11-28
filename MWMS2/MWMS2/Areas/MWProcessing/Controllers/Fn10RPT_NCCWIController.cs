using MWMS2.Areas.MWProcessing.Models;
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
    public class Fn10RPT_NCCWIController : Controller
    {


        private ProcessingReportBLService _BL;
        protected ProcessingReportBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingReportBLService());
            }
        }
        // GET: MWProcessing/Fn10RPT_NCCWI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(Fn10RPT_NCCWIModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchNCCWI(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(Fn10RPT_NCCWIModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.SearchNCCWI(model);
            return Json(new { key = model.Export("Submission_Search_" + DateTime.Now.ToString("")) });
        }
    }
}