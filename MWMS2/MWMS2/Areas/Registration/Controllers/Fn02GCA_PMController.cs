using MWMS2.Models;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_PMController : Controller
    {
        // GET: Registration/Fn02GCA_PM
        public ActionResult Index(ProcessMonitorSearchModel model)
        {
            return View("IndexComp_PM",model);
        }

        public ActionResult Search(ProcessMonitorSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.SearchGCAandMWCA_PM(model, RegistrationConstant.REGISTRATION_TYPE_CGA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string compUUID, string pmUUID,string compAppUUID)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            ProcessMonitorDisplayModel model = rs.ViewCompPM(compUUID, pmUUID, compAppUUID, RegistrationConstant.REGISTRATION_TYPE_CGA,RegistrationConstant.PROCESS_MONITOR_UPM);
            return View("FormComp_PM", model);
        }
        public ActionResult Delete(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.DeleteCompPMRecord(id);
            return View("IndexComp_PM");
        }

        public ActionResult Validate(ProcessMonitorDisplayModel model)
        {
            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(validateResult);
            return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
        }
        public ActionResult Save([Bind(Exclude = "")]ProcessMonitorDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.SaveComp_PM(model);
            return View("IndexComp_PM");
        }
        [HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationGCAService rs = new RegistrationGCAService();
        //    return Json(new { key = rs.ExportPM(Columns, post) });
        //}
        public ActionResult Excel(ProcessMonitorSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService s = new RegistrationCommonService();
            var regType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Json(new { key = s.ExportGCAandMWCA_PM(model, regType) });
        }
        //public FileStreamResult ExportCertificate()
        //    {
        //        RegistrationCommonService registrationCommonService = new RegistrationCommonService();
        //        String certificateContent = registrationCommonService.RetrieveCertificateContent(process, certNo, compAppUuid, issueDt, lastDocRecDt, signature);
        //    }
        [HttpPost]
        public ActionResult ProcDueDate(string dateStr,string rType)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.GetProcDueDate(dateStr, rType);
            return Content(JsonConvert.SerializeObject(rs.GetProcDueDate(dateStr, rType), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult GetCRC(ProcessMonitorDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.GetProcDueDate(dateStr, rType);
            return Content(JsonConvert.SerializeObject(rs.GetCRCByInterviewDate(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult GetCRCPost(ProcessMonitorDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.GetProcDueDate(dateStr, rType);
            return Content(JsonConvert.SerializeObject(rs.GetCRCPostByInterviewDate(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}