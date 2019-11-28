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
    public class PEM1103NoOfDirectReturnController : Controller
    {

        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }

        // GET: Admin/PEM1103NoOfDirectReturn
        public ActionResult Index(PEM1103NoOfDirectReturnModel model)
        {
            return View(BL.SearchNoOfDailyDirectReturnData(model));
        }

        public ActionResult Save(PEM1103NoOfDirectReturnModel model)
        {
            if (BL.SaveNoOfDirectReturn(model).Result == ServiceResult.RESULT_SUCCESS)
            {
                ViewBag.Message = "Save Successfully";
            }
            else
            {
                ViewBag.Message = "Save Failed.";
            }
            return View("Index", BL.SearchNoOfDailyDirectReturnData(model));
        }
    }
}