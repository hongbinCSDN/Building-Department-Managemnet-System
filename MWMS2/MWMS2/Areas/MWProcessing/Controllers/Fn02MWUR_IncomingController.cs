using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_IncomingController : Controller
    {
        private ProcessingIncomingBLService BLService;
        protected ProcessingIncomingBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingIncomingBLService()); }
        }
        // GET: MWProcessing/Fn02MWUR_Incoming
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateNewDsn()
        {
            return BL.GenerateNewDsn();
        }

        public ActionResult GetDsnInfo(string DSN)
        {
            return BL.GetDsnInfo(DSN);
        }

        public ActionResult SubmitDsnInfo(Fn02MWUR_IncomingModel model)
        {
            return BL.SubmitDsnInfo(model);
        }
    }
}