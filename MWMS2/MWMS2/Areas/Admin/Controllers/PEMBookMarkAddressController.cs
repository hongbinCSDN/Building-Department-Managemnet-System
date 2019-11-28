using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEMBookMarkAddressController : ValidationController
    {
        // GET: Admin/PEMBookMarkAddress
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(PEMBookMarkSearchModel model)
        {
            ProcessingBookMarkSerivce ss = new ProcessingBookMarkSerivce();
           // RegistrationCompAppService rs = new RegistrationCompAppService();
           //   model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Content(JsonConvert.SerializeObject(ss.SearchBM(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Detail(string uuid)
        {
            ProcessingBookMarkSerivce ss = new ProcessingBookMarkSerivce();
            PEMBookMarkSearchModel model = ss.EditBMAddress(uuid);
            return View(model);
        }
        public ActionResult Save(PEMBookMarkSearchModel model)
        {
    
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ProcessingBookMarkSerivce s = new ProcessingBookMarkSerivce();
            ServiceResult serviceResult = s.SaveBMAddress(model);
            return Json(serviceResult);
        }

        public ActionResult Excel(PEMBookMarkSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            ProcessingBookMarkSerivce s = new ProcessingBookMarkSerivce();
            return Json(new { key = s.Excel(model) });
        }
    }
}