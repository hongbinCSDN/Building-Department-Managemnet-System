using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn12RD_PdController : Controller
    {
        private ProcessingRDBLService _BL;
        protected ProcessingRDBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingRDBLService());
            }
        }

        // GET: MWProcessing/Fn12RD_Pd
        public ActionResult Index()
        {
            Fn12RD_DtmModel model = new Fn12RD_DtmModel();
            BL.loadProceedDelivery(model);
            return View(model);
        }

        public ActionResult rollBack(Fn12RD_DtmModel model)
        {
            BL.rollBack(model);
            return View("Index",model);
        }

        public ActionResult confirmDelivery(Fn12RD_DtmModel model)
        {
            BL.confirmDelivery(model);
            return View("Index", model);
        }

        public ActionResult deliveryCounted(Fn12RD_DtmModel model)
        {
           // ModelState.Clear();
            BL.deliveryCounted(model);
            return View("Index", model);
        }

    }
}