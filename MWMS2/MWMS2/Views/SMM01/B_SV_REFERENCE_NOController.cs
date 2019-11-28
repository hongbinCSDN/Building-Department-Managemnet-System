using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;

namespace MWMS2.Views.SMM01
{
    public class B_SV_REFERENCE_NOController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();

        // GET: B_SV_REFERENCE_NO
        public ActionResult Index()
        {
            return View(db.B_SV_REFERENCE_NO.ToList());
        }

        // GET: B_SV_REFERENCE_NO/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_SV_REFERENCE_NO b_SV_REFERENCE_NO = db.B_SV_REFERENCE_NO.Find(id);
            if (b_SV_REFERENCE_NO == null)
            {
                return HttpNotFound();
            }
            return View(b_SV_REFERENCE_NO);
        }

        // GET: B_SV_REFERENCE_NO/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: B_SV_REFERENCE_NO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UUID,REFERENCE_NO,MODIFIED_BY,MODIFIED_DATE,CATEGORY_CODE,CREATED_BY,CREATED_DATE,PREFIX,TYPE,CURRENT_NUMBER")] B_SV_REFERENCE_NO b_SV_REFERENCE_NO)
        {
            if (ModelState.IsValid)
            {
                db.B_SV_REFERENCE_NO.Add(b_SV_REFERENCE_NO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(b_SV_REFERENCE_NO);
        }

        // GET: B_SV_REFERENCE_NO/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_SV_REFERENCE_NO b_SV_REFERENCE_NO = db.B_SV_REFERENCE_NO.Find(id);
            if (b_SV_REFERENCE_NO == null)
            {
                return HttpNotFound();
            }
            return View(b_SV_REFERENCE_NO);
        }

        // POST: B_SV_REFERENCE_NO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UUID,REFERENCE_NO,MODIFIED_BY,MODIFIED_DATE,CATEGORY_CODE,CREATED_BY,CREATED_DATE,PREFIX,TYPE,CURRENT_NUMBER")] B_SV_REFERENCE_NO b_SV_REFERENCE_NO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(b_SV_REFERENCE_NO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(b_SV_REFERENCE_NO);
        }

        // GET: B_SV_REFERENCE_NO/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_SV_REFERENCE_NO b_SV_REFERENCE_NO = db.B_SV_REFERENCE_NO.Find(id);
            if (b_SV_REFERENCE_NO == null)
            {
                return HttpNotFound();
            }
            return View(b_SV_REFERENCE_NO);
        }

        // POST: B_SV_REFERENCE_NO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            B_SV_REFERENCE_NO b_SV_REFERENCE_NO = db.B_SV_REFERENCE_NO.Find(id);
            db.B_SV_REFERENCE_NO.Remove(b_SV_REFERENCE_NO);
            db.SaveChanges();
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
