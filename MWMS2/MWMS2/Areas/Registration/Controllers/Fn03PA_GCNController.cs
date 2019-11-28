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
    public class Fn03PA_GCNController : Controller
    {
        // GET: Registration/Fn03PA_GCN
        public ActionResult Index(Fn03PA_GCNSearchModel model)
        {
         
            return View("SearchGCNInd", model);
        }

        public ActionResult Form(string fileRef, string hkid, string name, string title, string gender)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();

            return View("GCNIndForm", new Fn03PA_GCNSearchModel() { FileRef = fileRef, HKID = hkid , Name = name , Title = title, Gender = gender });
        }

        public ActionResult Search(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            rs.SearchGCNIP(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03PA_GCNSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            return Json(new { key = rs.ExportGCNIP(model) });

        }
        public ActionResult GetGCNResult(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            DisplayGrid r = rs.AjaxApplicantInfos(model.FileRef);
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult Excel(Fn01Search_PASearchModel model)
        //{
        //    if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
        //    RegistrationSearchService rs = new RegistrationSearchService();
        //    return View();

        //}

        public ActionResult GenNo(Fn03PA_GCNSearchModel model)
        {
            RegistrationIndGCNService rs = new RegistrationIndGCNService();
            ServiceResult result = rs.GenNo(model);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

    }
}