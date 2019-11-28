using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
namespace MWMS2.Controllers
{
    public class SMM1103LinkMaintenanceController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM1103LinkMaintenance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM1103LinkMaintenance()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "URL");
            var LinkMaintenance = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);

            return View(LinkMaintenance);
        }
        public ActionResult SMM1103LinkMaintenanceEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id);

        
        


            return View(query.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM1103LinkMaintenanceEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {

            try
            {
                sv.MODIFIED_DATE = System.DateTime.Now;
                sv.MODIFIED_BY = SystemParameterConstant.UserName;
                db.Entry(sv).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("SMM1103LinkMaintenance");
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