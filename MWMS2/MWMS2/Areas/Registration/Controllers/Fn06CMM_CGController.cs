using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using System.Web.Configuration;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn06CMM_CGController : ValidationController
    {
        // GET: Registration/Fn06CMM_CG
        public ActionResult Index(Fn06CMM_CGSearchModel model)
        {
            //string path = WebConfigurationManager.AppSettings["ImagePath"];
            return View(model);
        }

        public ActionResult Search(Fn06CMM_CGSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchCG(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Excel(Fn06CMM_CGSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(new { key = s.ExportCG(model) });
        }
        public ActionResult AjaxPanelParent()
        {

            RegistrationCMMService rs = new RegistrationCMMService();
            return Content(JsonConvert.SerializeObject(rs.AjaxPanelParent(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCMMService rs = new RegistrationCMMService();

            return View(rs.LoadfCG(id));
        }




        public ActionResult SearchCommitteeMember(Fn06CMM_CGSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchCommitteeMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchPanelMember(Fn06CMM_CGSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchPanelMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AddToMember(Fn06CMM_CGSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.AddToMember(model));
            // return Content(JsonConvert.SerializeObject(s.AddToMember(checkMember), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Delete(Fn06CMM_CGSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Delete(model));
        }
        public ActionResult Save(Fn06CMM_CGSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Save(model));
        }




    }
}