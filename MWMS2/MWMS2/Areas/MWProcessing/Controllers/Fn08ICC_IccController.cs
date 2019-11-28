using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn08ICC_IccController : Controller
    {
        //ProcessingIccBLService
        private ProcessingIccBLService BLService;
        protected ProcessingIccBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingIccBLService()); }
        }
        // GET: MWProcessing/Fn08ICC_Icc
        public ActionResult Index()
        { 
            Fn08ICC_IccModel model = new Fn08ICC_IccModel();
            BL.loadDefault(model);
            return View(model);
        }
        public ActionResult save(Fn08ICC_IccModel model)
        {
            BL.saveNewRecord(model);
            return View("Index", model);
        }


    }
}