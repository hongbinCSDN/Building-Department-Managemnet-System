using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class PEM06Controller : Controller
    {
        // GET: PEM06
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PEM0601()
        {
            return View();

        }
        public ActionResult PEM0602()
        {
            return View();
        }
    }
}