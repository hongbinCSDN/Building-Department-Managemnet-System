using System;
using MWMS2.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using MWMS2.Filter;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Areas.CMN.Models;

namespace MWMS2.Areas.CMN.Controllers
{
    public class LoginController : ValidationController
    {
        public ActionResult login()
        {
            return View(new LoginModel());
        }

        public ActionResult checkLogin(LoginModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            Request.Headers.Add(AuthManager.HEADER_KEY_POST, model.post.CODE);
            Request.Headers.Add(AuthManager.HEADER_KEY_DEPT, AuthManager.HEADER_BD_DEPT);
            return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
        }

        public ActionResult doLogin()
        {
            Request.Headers.Add(AuthManager.HEADER_KEY_POST, "XXX");
            Request.Headers.Add(AuthManager.HEADER_KEY_DEPT, AuthManager.HEADER_BD_DEPT);
            AuthManager.Auth();
            string pageCode = "";
            MenuManager MenuManager = new MenuManager(SessionUtil.LoginPost, null);
            MenuManager.LoadMenu(null);

            return View();
        }
        public ActionResult timeOut()
        {
            return RedirectToAction("login");
            //return View();
        }

    }


}