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
    public class SPMValidationItemController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SPMValidationItem
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SPMValidationItem()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "ValidationItem");
            var ValidationItem = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);


            return View(ValidationItem);
        }

        public ActionResult SPMValidationItemEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id);

            var queryItemNo = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Item No");
            var ItemNo = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == queryItemNo.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);


            List<SelectListItem> MWItems = new List<SelectListItem>();
            foreach (var item in ItemNo)
            {
                MWItems.Add(new SelectListItem
                {
                    Text = item.CODE,
                    Value = item.CODE,

                });

            }
            string selectedDes = query.FirstOrDefault().DESCRIPTION;
            foreach (var item in MWItems)
            {
                if (item.Text == selectedDes)
                {
                    item.Selected = true;
                }
            }

            ViewBag.MWItems = MWItems;

            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(query.FirstOrDefault().MODIFIED_DATE);

            return View(query.FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMValidationItemEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("SPMValidationItem");
        }
        public ActionResult SPMValidationItemCreate()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Item No");
            var ItemNo = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);



            List<SelectListItem> MWItems = new List<SelectListItem>();
            foreach (var item in ItemNo)
            {
                MWItems.Add(new SelectListItem
                {
                    Text = item.CODE,
                    Value = item.CODE,

                });
            }
            ViewBag.MWItems = MWItems;



            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMValidationItemCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {
            if (sv.CODE != null)
            {
                var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "ValidationItem");
                sv.UUID = Guid.NewGuid().ToString();
                sv.SYSTEM_TYPE_ID = query.First().UUID;
                sv.MODIFIED_DATE = System.DateTime.Now;
                sv.MODIFIED_BY = SystemParameterConstant.UserName;
                sv.CREATED_DATE = System.DateTime.Now;
                sv.CREATED_BY = SystemParameterConstant.UserName;
                sv.IS_ACTIVE = "Y";
                db.B_S_SYSTEM_VALUE.Add(sv);
                db.SaveChanges();
            }

            return RedirectToAction("SPMValidationItem");
            // return View();
        }


        public ActionResult SPMValidationItemDelete(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id).SingleOrDefault();
            db.B_S_SYSTEM_VALUE.Remove(query);
            db.SaveChanges();
            return RedirectToAction("SPMValidationItem");
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