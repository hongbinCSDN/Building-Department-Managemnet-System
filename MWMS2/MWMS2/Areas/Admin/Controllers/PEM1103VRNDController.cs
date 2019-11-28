using MWMS2.Areas.Admin.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103VRNDController : Controller
    {
        //PEM1103VRNDBLService
        private PEM1103VRNDBLService BLService;
        protected PEM1103VRNDBLService BL
        {
            get { return BLService ?? (BLService = new PEM1103VRNDBLService()); }
        }
        // GET: Admin/PEM1103VRND
        public ActionResult Index()
        {
            PEM1103VRNDSearchModel model = new PEM1103VRNDSearchModel();
            return View(model);
        }

        public ActionResult Search(PEM1103VRNDSearchModel model)
        {
            return BL.Search(model);
        }

        public ActionResult Save(PEM1103VRNDSearchModel model)
        {
            //if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            //RegistrationCMMService s = new RegistrationCMMService();
            //return Json(s.Save(model));

            return BL.Update(model);
            //return Json("");
        }
    }
}