using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_PebController : Controller
    {
        private ProcessingPebDAOService BLservice;
        protected ProcessingPebDAOService BL
        {
            get { return BLservice ?? (BLservice = new ProcessingPebDAOService()); }
        }

        // GET: MWProcessing/Fn01LM_Peb
        public ActionResult Index()
        {
            return View();
        }
    }
}