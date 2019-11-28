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
    public class Fn04MWCA_GCNController : Controller
    {
        // GET: Registration/Fn04MWCA_GCN
        public ActionResult Index(Fn02GCA_GCNSearchModel model)
        {
       
            return View("SearchGCNComp", model);
        }

        public ActionResult Search(Fn02GCA_GCNSearchModel model)
        {
            RegistrationCompGCNService rs = new RegistrationCompGCNService();
            rs.SearchGCNCMW(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn02GCA_GCNSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCompGCNService rs = new RegistrationCompGCNService();
            return Json(new { key = rs.Excel(model) });
        }

        public ActionResult Form(string fileRef, string comName)
        {
            RegistrationCompGCNService rs = new RegistrationCompGCNService();
            return View("GCNCompForm", new Fn02GCA_GCNSearchModel() { FileRef = fileRef, ComName = comName });
            
        }

        //need check
        public ActionResult GetGCNResult(Fn02GCA_GCNSearchModel model)
        {
            RegistrationCompGCNService rs = new RegistrationCompGCNService();
            DisplayGrid r = rs.AjaxApplicantInfos(model.FileRef);
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationMWCAService registrationMCAService = new RegistrationMWCAService();
        //    return Json(new { key = registrationMCAService.ExportTemp(Columns, post) });
        //}

        public ActionResult GenNo(Fn02GCA_GCNSearchModel model)
        {
            RegistrationCompGCNService rs = new RegistrationCompGCNService();
            ServiceResult result = rs.GenNo(model);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

    }
}