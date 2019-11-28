using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SMMValidationOptionController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: Admin/SMMValidationController
        public ActionResult Index()
        {
            var m_uuid = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Validation").First().UUID;

            var query = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == m_uuid).OrderBy(y => y.ORDERING);

            return View(query);
        }
        public ActionResult SMMValidationOptionEdit(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Find(id);

            return View(query);
        }
        public ActionResult SMMValidationOptionCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMValidationOptionCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Validation");
            sv.UUID = Guid.NewGuid().ToString();
            sv.SYSTEM_TYPE_ID = query.First().UUID;
            sv.MODIFIED_DATE = System.DateTime.Now;
            sv.MODIFIED_BY = SystemParameterConstant.UserName;
            sv.CREATED_DATE = System.DateTime.Now;
            sv.CREATED_BY = SystemParameterConstant.UserName;
            db.B_S_SYSTEM_VALUE.Add(sv);
            db.SaveChanges();
            return RedirectToAction("Index"); ;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMValidationOptionEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("Index");
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