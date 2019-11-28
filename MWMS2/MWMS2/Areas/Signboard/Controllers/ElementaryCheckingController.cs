using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class ElementaryCheckingController : Controller
    {
        private ProcessingElementaryCheckingBLService BLService;
        protected ProcessingElementaryCheckingBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingElementaryCheckingBLService()); }
        }
        // GET: Signboard/ElementaryChecing
        public ActionResult Index(Fn11EC_Model model)
        {
            BL.GetElementrayModel(model);
            return View("~/Areas/MWProcessing/Views/Fn11EC_EC/Index.cshtml", model);
        }

    }
}