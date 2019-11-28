using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWMS2.Dao;

namespace MWMS2.Controllers
{
    public class WLs1Controller : Controller
    {
        private Entities db = new Entities();

        // GET: WLs1
        public ActionResult Index()
        {
            return View(db.WL.ToList());
        }

        // GET: WLs1/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WL wL = db.WL.Find(id);
            if (wL == null)
            {
                return HttpNotFound();
            }
            return View(wL);
        }

        // GET: WLs1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WLs1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UUID,SUBJECT,CATEGORY,REGISTRATION_NO,MW_SUBMISSION_NO,MW_ITEMS,COMP_CONTRACTOR_NAME_ENG,COMP_CONTRACTOR_NAME_CHI,CREATION_DATE,SECTION_UNIT,FILE_REF_FOUR,FILE_REF_TWO,WL_ISSUED_BY,POST,CASE_OFFICER,RELATED_TO,SOURCE,LETTER_ISSUE_DATE,LETTER_FILE_PATH,AUTHORIZED_SIGNATURE,STATUS,REMARK,CREATED_DATE,CREATED_BY,MODIFIED_DATE,MODIFIED_BY")] WL wL)
        {
            if (ModelState.IsValid)
            {
                db.WL.Add(wL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wL);
        }

        // GET: WLs1/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WL wL = db.WL.Find(id);
            if (wL == null)
            {
                return HttpNotFound();
            }
            return View(wL);
        }

        // POST: WLs1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UUID,SUBJECT,CATEGORY,REGISTRATION_NO,MW_SUBMISSION_NO,MW_ITEMS,COMP_CONTRACTOR_NAME_ENG,COMP_CONTRACTOR_NAME_CHI,CREATION_DATE,SECTION_UNIT,FILE_REF_FOUR,FILE_REF_TWO,WL_ISSUED_BY,POST,CASE_OFFICER,RELATED_TO,SOURCE,LETTER_ISSUE_DATE,LETTER_FILE_PATH,AUTHORIZED_SIGNATURE,STATUS,REMARK,CREATED_DATE,CREATED_BY,MODIFIED_DATE,MODIFIED_BY")] WL wL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wL);
        }

        // GET: WLs1/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WL wL = db.WL.Find(id);
            if (wL == null)
            {
                return HttpNotFound();
            }
            return View(wL);
        }

        // POST: WLs1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            WL wL = db.WL.Find(id);
            db.WL.Remove(wL);
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
