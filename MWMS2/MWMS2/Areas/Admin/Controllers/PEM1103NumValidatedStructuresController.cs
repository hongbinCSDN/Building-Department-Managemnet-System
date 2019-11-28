using MWMS2.Areas.Admin.Models;
using MWMS2.Models;
using MWMS2.Services.AdminService.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103NumValidatedStructuresController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }

        // GET: Admin/PEM1103NumValidatedStructures
        public ActionResult Index()
        {
            return View(BL.SearchNumberValidatedStructrues());
        }

        public ActionResult Save(PEM1103NumValidatedStructuresModel model)
        {
            if(BL.UpdateNumberValidatedStructrues(model).Result == ServiceResult.RESULT_SUCCESS)
            {
                ViewBag.Message = "Save Successfully";
            }
            else
            {
                ViewBag.Message = "Save Failed.";
            }
            return View("Index", BL.SearchNumberValidatedStructrues());
        }
    }
}