using MWMS2.Areas.Admin.Models;
using MWMS2.Services.AdminService.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103MWNumberPrefixController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }

        // GET: Admin/PEM1103MWNumberPrefix
        public ActionResult Index()
        {
            return View(BL.GetMWNumberPrefix());
        }

        public ActionResult Save(PEM1103MWNumberPrefixModel model)
        {
            return Json(BL.SaveMWNumberPrefix(model));
        }
    }
}