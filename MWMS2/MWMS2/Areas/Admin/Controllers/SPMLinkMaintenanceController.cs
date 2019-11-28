using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Controllers;
using System.Data.Entity;
using System.Net;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SPMLinkMaintenanceController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SPMLinkMaintenance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SPMLinkMaintenance()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "URL");
            var LinkMaintenance = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);

            return View(LinkMaintenance);
        }
        public ActionResult SPMLinkMaintenanceEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id);

            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(query.FirstOrDefault().MODIFIED_DATE);



            return View(query.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMLinkMaintenanceEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {

            try
            {
                //sv.MODIFIED_DATE = System.DateTime.Now;
                //sv.MODIFIED_BY = SystemParameterConstant.UserName;
                db.Entry(sv).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("SPMLinkMaintenance");
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