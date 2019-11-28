using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;


namespace MWMS2.Controllers
{
    public class CRM07Controller : Controller
    {

        //private EntitiesRegistration db = new EntitiesRegistration();
   
    
        // GET: CRM07
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult CRM0701()
        {

            // var s_SYSTEM_VALUE = db.S_SYSTEM_VALUE.Include(s => s.S_SYSTEM_TYPE).Include(s => s.S_SYSTEM_VALUE2);
            return View();
           // return View(s_SYSTEM_VALUE.ToList());
        }
        public ActionResult CRM0702()
        {
            return View();

        }
      
    }
}