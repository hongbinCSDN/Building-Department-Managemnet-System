using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_ALMController : ValidationController
    {
        private static volatile ProcessingLetterModuleBLService _BL;
        private static readonly object locker = new object();
        private static ProcessingLetterModuleBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new ProcessingLetterModuleBLService(); return _BL; } }

        // GET: MWProcessing/Fn01LM_ALM
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PickAudit()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(Fn01LM_ALMSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchALM(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult SearchClass1And2(Fn01LM_ALMSearchModel model)
        //{
        //    return Content(JsonConvert.SerializeObject(BL.SearchClass1And2(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        [HttpPost]
        public ActionResult PickAuditAFC(string id)
        {
            return Json(BL.PickAuditAFC(id));
        }

        [HttpPost]
        public ActionResult PickAuditSAC(string id)
        {
            return Json(BL.PickAuditSAC(id));
        }

        [HttpPost]
        public ActionResult PickAuditPSAC(string id)
        {
            return Json(BL.PickAuditPSAC(id));
        }

        [HttpPost]
        public JsonResult PickAudit(Fn01LM_ALMSearchModel model)
        {
            return Json(BL.PickAudit(model));
        }

        [HttpGet]
        public ActionResult Completion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Completion(Fn01LM_ALMSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Completion(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ValidationDSN(Fn01LM_ALMSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {

                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
        }

        [HttpPost]
        public ActionResult UpdateAuditRelated(Fn01LM_ALMSearchModel model)
        {
            return Json(BL.UpdateAuditRelated(model));
        }

    }
}