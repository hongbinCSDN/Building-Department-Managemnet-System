using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Interface.Controllers
{
    public class InfoPageController : Controller
    {
        // GET: Interface/InfoPage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InfoPage(string BLOCK_ID, string MW_REF_NO, string BD_FILE_REF_NO)
        {

            return  RedirectToAction("CMN01","CMN", new { area ="", BLOCK_ID, MW_REF_NO, BD_FILE_REF_NO });
        }
    }
}