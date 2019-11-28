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
    public class Fn05MWIA_GCNController : Controller
    {
        // GET: Registration/Fn05MWIA_GCN
        public ActionResult Index(Fn03PA_GCNSearchModel model)
        {
         
            return View("SearchGCNInd", model);
        }

        public ActionResult Search(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            rs.SearchGCNIMW(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(Fn03PA_GCNSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            return Json(new { key = rs.Excel(model) });
        }

        public ActionResult Form(string fileRef, string hkid, string name, string title, string gender)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();

            return View("GCNIndForm", new Fn03PA_GCNSearchModel() { FileRef = fileRef, HKID = hkid, Name = name, Title = title, Gender = gender });
        }

        public ActionResult GetGCNResult(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            DisplayGrid r = rs.AjaxApplicantInfos(model.FileRef);
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationMWIAService registrationMWIAService = new RegistrationMWIAService();
        //    return Json(new { key = registrationMWIAService.ExportMWIA(Columns, post) });
        //}

        public ActionResult GenNo(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            ServiceResult result = rs.GenNo(model);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

    }
}