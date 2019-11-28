using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System.Web.Mvc;
using MWMS2.Areas.Admin.Models;
using MWMS2.Filter;
using System.Collections.Generic;
using System.Linq;
using System;
using MWMS2.Entity;

namespace MWMS2.Areas.Admin.Controllers
{
    public class CRM_ARController : Controller
    {
        EntitiesRegistration db = new EntitiesRegistration();

        // GET: Admin/CRM
        public ActionResult Index()
        {
            CRM_ARModel model = new CRM_ARModel();
            return View(model);
        }

        //For existing record
        public ActionResult Form(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_ARDisplayModel model = rs.ViewAR(id);
            return View(model);
        }

        //For creating new record
        public ActionResult CreateNew()
        {
            CRM_ARModel model = new CRM_ARModel();
            return View(model);
        }

        public ActionResult Search(CRM_ARModel model)
        {
            AdminCRMService rs = new AdminCRMService();
            rs.SearchAR(model);
            return Content(JsonConvert.SerializeObject(rs.SearchAR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CRM_ARDisplayModel model)
        {

            EntitiesRegistration db = new EntitiesRegistration();
            var query = db.C_S_SYSTEM_VALUE.Find(model.C_S_SYSTEM_VALUE.UUID);
            query.ENGLISH_DESCRIPTION = model.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION;
            query.CHINESE_DESCRIPTION = model.C_S_SYSTEM_VALUE.CHINESE_DESCRIPTION;
            query.ORDERING = model.C_S_SYSTEM_VALUE.ORDERING;
            query.IS_ACTIVE = model.C_S_SYSTEM_VALUE.IS_ACTIVE;  //not changed yet 
            query.MODIFIED_DATE = System.DateTime.Now;
            query.MODIFIED_BY = SystemParameterConstant.UserName; //"Admin";
            db.SaveChanges();
           
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew([Bind(Exclude = "")] CRM_ARModel model)
        {
            EntitiesRegistration db = new EntitiesRegistration();
            C_S_SYSTEM_VALUE sv = new C_S_SYSTEM_VALUE();
      
                var query = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == "APPLICANT_ROLE");
                sv.UUID = Guid.NewGuid().ToString();   //gen new UUID
                sv.REGISTRATION_TYPE = model.RegType;
                sv.SYSTEM_TYPE_ID = query.First().UUID; 
                sv.CODE = model.CODE;
                sv.PARENT_ID = "";
                sv.ENGLISH_DESCRIPTION = model.EngDesc;
                sv.CHINESE_DESCRIPTION = model.ChiDesc;
                sv.IS_ACTIVE = model.IsActive ==true ? "Y":"N";
                sv.ORDERING = model.ORDERING;
                sv.MODIFIED_DATE = System.DateTime.Now;
                sv.MODIFIED_BY = SystemParameterConstant.UserName;
                sv.CREATED_DATE = System.DateTime.Now;
                sv.CREATED_BY = SystemParameterConstant.UserName;

                db.C_S_SYSTEM_VALUE.Add(sv);
                db.SaveChanges();

            //return RedirectToAction("/Index");
            return View("Index");
        }

        public ActionResult Excel(CRM_ARModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportAR(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();
            dlr.Rpp = 25;
            return dlr;
        }
       
    }
}