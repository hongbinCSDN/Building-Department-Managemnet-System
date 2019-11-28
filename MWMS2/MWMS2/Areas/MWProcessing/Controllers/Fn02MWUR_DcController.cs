using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_DcController : Controller
    {
        private ProcessingDcBLService BLService;
        protected ProcessingDcBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingDcBLService()); }
        }
        // GET: MWProcessing/Fn02MWUR_Dc
        public ActionResult Index()
        {
            return View();
        }
    }
}