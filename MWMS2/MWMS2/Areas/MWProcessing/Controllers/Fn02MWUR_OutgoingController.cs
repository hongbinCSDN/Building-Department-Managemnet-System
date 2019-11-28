using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_OutgoingController : Controller
    {
        private ProcessingOutgoingBLService BLService;
        protected ProcessingOutgoingBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingOutgoingBLService()); }
        }
        // GET: MWProcessing/Outgoing
        public ActionResult Index()
        {
            Fn02MWUR_OutgoingModel model = new Fn02MWUR_OutgoingModel();
            BL.GetOutgoingModel(model);
            return View(model);
        }

        public ActionResult GenerateNewDsn()
        {
            return BL.GenerateNewDsn();
        }

        public ActionResult GetDsnInfo(string DSN)
        {
            return BL.GetDsnInfo(DSN);
        }

        public ActionResult SubmitDsnInfo(Fn02MWUR_OutgoingModel model)
        {
            return BL.SubmitDsnInfo(model);
        }
    }
}