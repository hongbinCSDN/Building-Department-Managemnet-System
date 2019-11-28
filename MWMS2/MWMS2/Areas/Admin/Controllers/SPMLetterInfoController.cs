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

namespace MWMS2.Areas.Admin.Controllers
{
    public class SPMLetterInfoController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SMM1103LetterInfo()
        {
            return View();
        }

        // Load edit page
        public ActionResult SPMLetterInfoEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // Edit record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMLetterInfoEdit([Bind(Exclude = "")] B_S_PARAMETER sv)
        {
            try
            {
                sv.MODIFIED_DATE = System.DateTime.Now;
                db.Entry(sv).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Redirect("/Admin/SMMSysParaMan/Index");
        }

        // Load create page
        public ActionResult SPMLetterInfoCreate(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // Create record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMLetterInfoCreate([Bind(Exclude = "")] B_S_PARAMETER sv)
        {
            try
            {
                sv.MODIFIED_DATE = System.DateTime.Now;
                db.Entry(sv).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Redirect("/Admin/SMMSysParaMan/Index");
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