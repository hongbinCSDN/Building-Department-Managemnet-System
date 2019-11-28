using MWMS2.Areas.MWProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn12RD_DtmController : Controller
    {
        private ProcessingRDBLService _BL;
        protected ProcessingRDBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingRDBLService());
            }
        }
        // GET: MWProcessing/Fn12RD_Dtm
        public ActionResult Index()
        {
            Fn12RD_DtmModel model = new Fn12RD_DtmModel();
            
            return View(model);
        }

        public ActionResult assignNewDSN(Fn12RD_DtmModel model)
        {
            ModelState.Clear();
            BL.assignNewDSN(model);
            return View("Index", model);
        }
        

        public ActionResult Search(Fn12RD_DtmModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDtm(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}