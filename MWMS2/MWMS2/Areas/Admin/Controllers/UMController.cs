using MWMS2.Entity;
using MWMS2.DaoController;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services.AdminService.BL;
using MWMS2.Areas.Admin.Models;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.Admin.Controllers
{
    public class UMController : ValidationController
    {
        private static volatile Sys_UMBLService _BL;
        private static readonly object locker = new object();
        private static Sys_UMBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new Sys_UMBLService(); return _BL; } }

        public ActionResult Index()
        {
            Sys_UMModel model = new Sys_UMModel();
            return View(model);
        }

        public ActionResult Search(Sys_UMModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchUsers(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult SaveUser(Sys_UMModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveUser(model));
        }
       
        [HttpPost]
        public ActionResult EditUser(string id)
        {
            Sys_UMModel model = BL.GetPostByUUID(id);
            return View(model);
        }

        public ActionResult AjaxPostRole(Sys_UMModel model)
        {
            BL.AjaxPostRole(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult AjaxScuSubordinateMember(Sys_UMModel model)
        {
            BL.AjaxScuSubordinateMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AjaxScuResponsibleArea(Sys_UMModel model)
        {
            BL.AjaxScuResponsibleArea(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult AjaxPemSubordinateMember(Sys_UMModel model)
        {
            BL.AjaxPemSubordinateMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}
