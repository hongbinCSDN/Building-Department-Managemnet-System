using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn07Audit_AscController : Controller
    {
        private ProcessingAscBLService BLService;
        protected ProcessingAscBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingAscBLService()); }
        }
        // GET: MWProcessing/Fn07Audit_Asc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchPart1()
        {
            return Json("");
        }

        public ActionResult SearchPart2()
        {
            return Json("");
        }
    }
}