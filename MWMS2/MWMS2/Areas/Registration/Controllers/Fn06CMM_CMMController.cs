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



namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn06CMM_CMMController : ValidationController
    {
        // GET: Registration/Fn06COM_COM
        public ActionResult Index(Fn06CMM_CMMSearchModel model)
        {
            
            return View(model);
        }
        public ActionResult Edit(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            if (model.C_COMMITTEE != null)
            {
                rs.LoadfCMM(model);
            }
          
            return View(model);
        }
        public ActionResult Search(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchCMM(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchPanelMember(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchPanelMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchCommitteeMember(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();

            rs.SearchCommitteeMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        
        public ActionResult getComTypeByParent(Fn06CMM_CMMSearchModel model,string pUUID)
        {
            RegistrationCMMService rs = new RegistrationCMMService();


            return Content(JsonConvert.SerializeObject(model.getComTypeByParent(pUUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn06CMM_CMMSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(new { key = s.ExportCMM(model) });
        }

        public ActionResult AjaxPanelParent()
        {
            
               RegistrationCMMService rs = new RegistrationCMMService();
            return Content(JsonConvert.SerializeObject(rs.AjaxPanelParent(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult AddToMember(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.AddToMember(model));
            // return Content(JsonConvert.SerializeObject(s.AddToMember(checkMember), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Delete(Fn06CMM_CMMSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Delete(model));
        }
        public ActionResult Save(Fn06CMM_CMMSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Save(model));
        }

    }
}