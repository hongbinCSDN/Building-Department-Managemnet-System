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
    public class SMM1103DistrictMaintenanceController : Controller
    {
       
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM1103DistrictMaintenance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM1103DistrictMaintenance()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "BcisDistrict");
            var DistrictMain = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);
            List<ModelSystemValue> modelSystemValueList = new List<ModelSystemValue>();
            foreach (var item in DistrictMain)
            {
                ModelSystemValue m_sv = new ModelSystemValue();
                m_sv.UUID = item.UUID;
                m_sv.CODE = item.CODE;
                m_sv.DESCRIPTION = item.DESCRIPTION;
                m_sv.Region = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == item.PARENT_ID).FirstOrDefault().DESCRIPTION;
                m_sv.ORDERING = item.ORDERING;
                modelSystemValueList.Add(m_sv);
            } 
            
            return View(modelSystemValueList);
        }
        public ActionResult SMM1103DistrictMaintenanceCreate()
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Region");
            var ItemNo = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == query.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);



            List<SelectListItem> RegionList = new List<SelectListItem>();
            foreach (var item in ItemNo)
            {
                RegionList.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.DESCRIPTION,

                });
            }
            ViewBag.RegionList = RegionList;



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM1103DistrictMaintenanceCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "BcisDistrict");
            sv.UUID = Guid.NewGuid().ToString();
            sv.SYSTEM_TYPE_ID = query.First().UUID;
            sv.PARENT_ID = db.B_S_SYSTEM_VALUE.Where(x => x.DESCRIPTION == sv.PARENT_ID).FirstOrDefault().UUID;
            sv.MODIFIED_DATE = System.DateTime.Now;
            sv.MODIFIED_BY = SystemParameterConstant.UserName;
            sv.CREATED_DATE = System.DateTime.Now;
            sv.CREATED_BY = SystemParameterConstant.UserName;
            sv.IS_ACTIVE = "Y";
            db.B_S_SYSTEM_VALUE.Add(sv);
            db.SaveChanges();

            return RedirectToAction("SMM1103DistrictMaintenance");
            // return View();
        }

        public ActionResult SMM1103DistrictMaintenanceEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id);

            var queryItemNo = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "Region");
            var ItemNo = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == queryItemNo.FirstOrDefault().UUID).OrderBy(x => x.ORDERING);




            

            List<SelectListItem> RegionList = new List<SelectListItem>();
            foreach (var item in ItemNo)
            {
                RegionList.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.UUID,

                });

            }
     
            string selectedDes = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == query.FirstOrDefault().PARENT_ID).FirstOrDefault().UUID;
            foreach (var item in RegionList)
            {
                if (item.Value == selectedDes)
                {
                    item.Selected = true;
                }
            }

            ViewBag.RegionList = RegionList;



            return View(query.FirstOrDefault());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM1103DistrictMaintenanceEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv)
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

            return RedirectToAction("SMM1103DistrictMaintenance");
        }

        public ActionResult SMM1103ValidationItemDelete(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id).SingleOrDefault();
            db.B_S_SYSTEM_VALUE.Remove(query);
            db.SaveChanges();
            return RedirectToAction("SMM1103DistrictMaintenance");
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