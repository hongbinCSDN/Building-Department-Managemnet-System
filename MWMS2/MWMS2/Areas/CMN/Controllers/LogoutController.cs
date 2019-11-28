using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;

namespace MWMS2.Areas.CMN.Controllers
{
    public class LogoutController : Controller
    {

        public ActionResult Index()
        {

           
            Utility.SessionUtil.Logout();

            return View();
        }

      
    }
}