using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn11EC_ECController : Controller
    {

        private ProcessingElementaryCheckingBLService BLService;
        protected ProcessingElementaryCheckingBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingElementaryCheckingBLService()); }
        }
        public ActionResult Index(Fn11EC_Model model)
        {
            BL.GetElementrayModel(model);
            return View(model);
        }
        public ActionResult AjaxRegInfo(Fn11EC_Model model)
        {
            return BL.AjaxRegInfo(model);
        }


    }
}