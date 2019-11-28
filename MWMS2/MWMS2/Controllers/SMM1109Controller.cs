using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class SMM1109Controller : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM1109
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM1109()
        {
            var m_uuid = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "SignboardLocationTemplate").First().UUID;

            var query = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == m_uuid).OrderBy(y => y.ORDERING);
            

            return View(query);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}