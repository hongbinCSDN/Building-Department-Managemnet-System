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
    public class Fn06CMM_CPController : ValidationController
    {
        // GET: Registration/Fn06COM_CP
        public ActionResult Index(Fn06CMM_CPSearchModel model)
        {
            return View(model );
        }
        public ActionResult Form(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCMMService rs = new RegistrationCMMService();
     
            return View(rs.LoadfCP(id));
        }
        public ActionResult Search(Fn06CMM_CPSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchCP(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchCommitteeMember(Fn06CMM_CPSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchCommitteeMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchPanelMember(Fn06CMM_CPSearchModel model)
        {
            RegistrationCMMService rs = new RegistrationCMMService();
            rs.SearchPanelMember(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AddToMember(Fn06CMM_CPSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.AddToMember(model));
            // return Content(JsonConvert.SerializeObject(s.AddToMember(checkMember), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Delete(Fn06CMM_CPSearchModel model)
        {
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Delete(model));
        }
        public ActionResult Save(Fn06CMM_CPSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCMMService s = new RegistrationCMMService();
            return Json(s.Save(model));
        }

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationCMMService registrationCMMService = new RegistrationCMMService();
        //    return Json(new { key = registrationCMMService.ExportCP(Columns, post) });
        //}
        public ActionResult Excel(Fn06CMM_CPSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCMMService rs = new RegistrationCMMService();
            return Json(new { key = rs.ExportCP(model)});
        }
        public ActionResult CPToPA(string hkid)
        {
            RegistrationCMMService ss = new RegistrationCMMService();
            var result=  ss.GetPAbyHKID(hkid);
            if (string.IsNullOrWhiteSpace(result))
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "No Professional Application Record" } });

            }
            else {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string> { result } });

            }
        }
        public ActionResult GoToPA(string uuid)
        {
          return   RedirectToAction("Form", "Fn03PA_PA", new { uuid = uuid });
        }

    }
}