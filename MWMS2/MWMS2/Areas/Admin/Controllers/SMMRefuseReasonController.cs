using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SMMRefuseReasonController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();

        // GET: Admin/SMMRefuseReason/Index
        public ActionResult Index()
        {
            var m_uuid = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "ReasonForRefuse").First().UUID;

            var query = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == m_uuid).OrderBy(y => y.ORDERING);

            return View(query);
        }

        public ActionResult SMMRefuseReasonEdit(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Find(id);

            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(query.MODIFIED_DATE);

            return View(query);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMRefuseReasonEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("Index");
        }
        public ActionResult SMMRefuseReasonDelete(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id).SingleOrDefault();
            db.B_S_SYSTEM_VALUE.Remove(query);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SMMRefuseReasonCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMRefuseReasonCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "ReasonForRefuse");
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