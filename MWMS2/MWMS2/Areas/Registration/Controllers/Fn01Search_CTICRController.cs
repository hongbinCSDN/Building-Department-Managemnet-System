using MWMS2.Areas.Registration.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_CTICRController : Controller
    {
        public ActionResult Index(Fn01Search_CTICRSearchModel model)
        {
            return View("Index", model);
        }
        public ActionResult exportButtion(Fn01Search_CTICRSearchModel model)
        {
            return null;
        }
    }
}