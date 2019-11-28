using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn07Audit_AcController : Controller
    {
        //ProcessingAcBLService
        private ProcessingAcBLService BLService;
        protected ProcessingAcBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingAcBLService()); }
        }
        // GET: MWProcessing/Fn07Audit_Ac
        public ActionResult Index()
        {
            return View();
        }
    }
}