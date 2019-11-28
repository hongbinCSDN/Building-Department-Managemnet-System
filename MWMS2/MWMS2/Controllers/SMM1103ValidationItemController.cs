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
    public class SMM1103ValidationItemController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM1103ValidationItem
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM1103ValidationItem()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "ValidationItem");
            var ValidationItem = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);


            return View(ValidationItem);
        }

        public ActionResult SMM1103ValidationItemEdit(string id)
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
            string selectedDes = query.FirstOrDefault().DESCRIPTION ;
            foreach (var item in MWItems)
            {
                if (item.Text == selectedDes)
                {
                    item.Selected = true;
                }
            }

            ViewBag.MWItems = MWItems;



            return View(query.FirstOrDefault());
         }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM1103ValidationItemEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("SMM1103ValidationItem");
        }
        public ActionResult SMM1103ValidationItemCreate()
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
        public ActionResult SMM1103ValidationItemCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("SMM1103ValidationItem");
           // return View();
        }


        public ActionResult SMM1103ValidationItemDelete(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id).SingleOrDefault();
            db.B_S_SYSTEM_VALUE.Remove(query);
            db.SaveChanges();
            return RedirectToAction("SMM1103ValidationItem");
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