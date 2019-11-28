using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using MWMS2.Areas.Admin.Models;
using MWMS2.Services.AdminService.BL;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.CMN.Controllers
{
    public class CMN03Controller : ValidationController
    {
        private static volatile Sys_UMBLService _BL;
        private static readonly object locker = new object();
        private static Sys_UMBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new Sys_UMBLService(); return _BL; } }

        public ActionResult Index()
        {   Sys_UMModel model = BL.GetPostByUUID(MWMS2.Utility.SessionUtil.LoginPost.UUID);
            return View(model);
        }


        [HttpPost]
        public ActionResult SaveUser(Sys_UMModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateUserPost(model));
        }
    }
}