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
    public class SMMSignboardTypeController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: Admin/SMMSignboardType
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Admin/SMMSignboardType/Index
        public ActionResult Index()
        {


            var m_uuid = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "SignboardLocationTemplate").First().UUID;

            var query = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == m_uuid).OrderBy(y => y.ORDERING);

            //List<string> code =  query.FirstOrDefault().CODE.Split(',').ToList<string>();
            //ViewBag.typeEng = code[0];
            //ViewBag.typeChi = code[1];


            return View(query);
        }

        // GET: Admin/SMMSignboardType/SMMSignboardTypeCreate
        public ActionResult SMMSignboardTypeCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMSignboardTypeCreate([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv, string typeEng, string typeChi, string descEng, string descChi)
        {
            var query = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "SignboardLocationTemplate");
            sv.UUID = Guid.NewGuid().ToString();
            sv.SYSTEM_TYPE_ID = query.First().UUID;
            // sv.PARENT_ID = db.B_S_SYSTEM_VALUE.Where(x => x.DESCRIPTION == sv.PARENT_ID).FirstOrDefault().UUID;// error
            sv.CODE = "{'typeEng': '" + typeEng + "', " + "'typeChi': '" + typeChi + "'}";
            sv.DESCRIPTION = "{'descEng': '" + descEng + "', " + "'descChi': '" + descChi + "'}";
            sv.MODIFIED_DATE = System.DateTime.Now;
            sv.MODIFIED_BY = SystemParameterConstant.UserName;
            sv.CREATED_DATE = System.DateTime.Now;
            sv.CREATED_BY = SystemParameterConstant.UserName;
            sv.IS_ACTIVE = "Y";
            db.B_S_SYSTEM_VALUE.Add(sv);
            db.SaveChanges();

            return Redirect("Index");
        }

        // GET: Admin/SMMSignboardType/SMMSignboardTypeEdit
        public ActionResult SMMSignboardTypeEdit(string id)
        {
            var query = db.B_S_SYSTEM_VALUE.Find(id);

            List<string> code = query.CODE.Split(',').ToList<string>();
            List<string> typeEng = code[0].Split('\'').ToList<string>();
            List<string> typeChi = code[1].Split('\'').ToList<string>();
            ViewBag.typeEng = typeEng[3];
            ViewBag.typeChi = typeChi[3];

            List<string> desc = query.DESCRIPTION.Split(',').ToList<string>();
            List<string> descEng = desc[0].Split('\'').ToList<string>();
            List<string> descChi = desc[1].Split('\'').ToList<string>();
            ViewBag.descEng = descEng[3];
            ViewBag.descChi = descChi[3];

            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(query.MODIFIED_DATE);

            return View(query);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMMSignboardTypeEdit([Bind(Exclude = "")] B_S_SYSTEM_VALUE sv, string typeEng, string typeChi, string descEng, string descChi)
        {
            try
            {
                var s_v = db.B_S_SYSTEM_VALUE.Find(sv.UUID);

                s_v.CODE = "{'typeEng': '" + typeEng + "', " + "'typeChi': '" + typeChi + "'}";
                s_v.DESCRIPTION = "{'descEng': '" + descEng + "', " + "'descChi': '" + descChi + "'}";
                s_v.ORDERING = sv.ORDERING;
                s_v.MODIFIED_DATE = System.DateTime.Now;
                s_v.MODIFIED_BY = SystemParameterConstant.UserName;
                db.Entry(s_v).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        public ActionResult SMMSignboardTypeDelete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var query = db.B_S_SYSTEM_VALUE.Where(x => x.UUID == id).FirstOrDefault();
                if (query != null)
                {
                    db.B_S_SYSTEM_VALUE.Remove(query);
                    db.SaveChanges();
                }
                
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