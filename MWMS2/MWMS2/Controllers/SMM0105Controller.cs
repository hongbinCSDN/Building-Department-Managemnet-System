using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class SMM0105Controller : Controller
    {
        // GET: SMM0105
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM0105()
        {
            return View();
        }

        public ActionResult PrintBarcode(string DSN)
        {
            CommonFunction cf = new CommonFunction();
            var result =  cf.PrintBarcodeLabel(DSN);

            if (result == false)
            {
                return Json("Print Services Stopped", JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        
    }
}