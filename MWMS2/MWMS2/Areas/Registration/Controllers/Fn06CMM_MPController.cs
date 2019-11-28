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
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn06CMM_MPController : ValidationController
    {
        public ActionResult Index(Fn06CMM_MPSearchModel model)
        {
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            Fn06CMM_MPDisplayModel model = rs.EditMP(id);
            return View(model);
        }

        public ActionResult Save(Fn06CMM_MPDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCMMService rs = new RegistrationCMMService();
            ServiceResult serviceResult = rs.SaveMP(model);
            return Json(serviceResult);
        }

        public ActionResult Search(Fn06CMM_MPSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchMP(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult AjaxPanelMember()
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            return Content(JsonConvert.SerializeObject(rs.AjaxPanelMember(Request["C_COMMITTEE_MEMBER.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult AjaxGroupMember()
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            return Content(JsonConvert.SerializeObject(rs.AjaxGroupMember(Request["C_COMMITTEE_MEMBER.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AjaxInstitute()
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            return Content(JsonConvert.SerializeObject(rs.AjaxInstitute(Request["C_COMMITTEE_MEMBER.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        

        [HttpPost]
        public ActionResult Excel(Fn06CMM_MPSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCMMService rs = new RegistrationCMMService();
            return Json(new { key = rs.ExportMP(model) });
        }
    }
}