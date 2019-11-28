
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Filter;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_CAController : ValidationController
    {
        public ActionResult Index(CompSearchModel model)
        {
          
            return View("IndexComp", model);
        }
        public ActionResult Edit(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCompAppService s = new RegistrationCompAppService();
            CompDisplayModel model = s.ViewComp(id, RegistrationConstant.REGISTRATION_TYPE_CGA);
            return View("EditComp", model);
        }

        public ActionResult Save(CompDisplayModel model)
        {
            int i = this.Request.Files.Count;
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            RegistrationCompAppService s = new RegistrationCompAppService();
            ServiceResult serviceResult = s.SaveComp(model);
            return Json(serviceResult);
        }

        public ActionResult Search(CompSearchModel model)
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Content(JsonConvert.SerializeObject(s.SearchComp(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Delete(CompDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            RegistrationCompAppService s = new RegistrationCompAppService();
            ServiceResult serviceResult = s.DeleteComp(model);
            return Json(serviceResult);
        }

        [HttpPost]
        public ActionResult Excel(CompSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCompAppService s = new RegistrationCompAppService();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Json(new { key = s.ExportComp(model) });
        }



        public ActionResult TabPageCompApplicantInfo()
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            ApplicantDisplayListModel model = s.GetCompApplicantInfo(Request["C_COMP_APPLICATION.UUID"], Request["C_COMP_APPLICANT_INFO.UUID"], RegistrationConstant.REGISTRATION_TYPE_CGA);
            return View(model);
        }



        public ActionResult DraftApplicant(CompApplicantEditModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            int i = this.Request.Files.Count;
            var supportedTypes = new[] { "gif", "jpg" };
            var fileExt = "";
            if (i != 0)
            {
                fileExt = System.IO.Path.GetExtension(this.Request.Files["APPLICANT_FILE"].FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    return Content(JsonConvert.SerializeObject(new ServiceResult() { Result = "Failed", Message = new List<string> { "Invalid file type, please confrim the file type." } }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
                }
            }
             
            
      
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(s.DraftApplicant(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult DeleteApplicant()
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(s.DeleteApplicant(Request["C_COMP_APPLICANT_INFO.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult DraftAppHist(CompAppHistEditModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(s.DraftAppHist(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult DeleteAppHist()
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(s.DeleteAppHist(Request["C_COMP_APPLICANT_INFO_MASTER.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }



        public ActionResult AjaxApplicantList()
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(rs.AjaxMergedApplicantList(Request["C_COMP_APPLICATION.UUID"])
                , Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult PopupBrDetails(BrDetailsModel model)
        {
            return View(model);
        }
        public ActionResult BrSearch(BrDetailsModel model)
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Content(JsonConvert.SerializeObject(s.SearchBr(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult PopupPoolingDetails(PoolingDetailsModel model)
        {
            return View(model);
        }
        public ActionResult PoolingSearch(PoolingDetailsModel model)
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
          
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Content(JsonConvert.SerializeObject(s.SearchPooling(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult PopupCompanyName(CompanyNamesModel model)
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();
            model = rs.GetCompanyNames(model);
            return View(model);
        }
        public ActionResult CompanyNameSearch(CompanyNamesModel model)
        {
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(s.SearchCompName(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult AjaxProcDueDate(DateTime? applicationDate)
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(rs.AjaxProcDueDate(applicationDate), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult WindowEForm(string FILE_REF)
        {
            RegistrationEformService rs = new RegistrationEformService();
            EformModel model = rs.initialiseEformModel(FILE_REF, "C_EFSS_COMPANY");
            return View("EformSelection", rs.populateEformSelection(model));
        }

        [HttpPost]
        public ActionResult EformEFSSCompany(string Uuid, string RefNo, string FileType)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.getDatafromEFSSCOMPANY(Uuid, RefNo, FileType);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult AddEformApplicant(string formId, string formType, string refNo)
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();
            return Content(JsonConvert.SerializeObject(rs.AddEformApplicantGC(formId, formType, refNo), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        [HttpPost]
        public ActionResult UpdateEformStatus(string UUID, string STATUS)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.updateEformStatus_COMPANY(UUID, STATUS);
            //return;
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult GetSignImg(string uuid)
        {

            CommonBLService ss = new CommonBLService();

            var file = ss.ViewCompCRMImageByUUID(uuid);

            if(file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }
 

        }

        //public FileResult GetSignImg(string filepath)
        //{
        //    RegistrationCommonService rs = new RegistrationCommonService();
        //    if (filepath.Contains("jpg"))
        //    {
        //        return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");

        //    }
        //    else
        //    {
        //        return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Gif, "Sign.gif");

        //    }

        //}
        public ActionResult GenerateSerialNo(string uuid)
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();

          
            string serial_no = "";
            string expiryDt = "";
            rs.GenerateSerialNo(uuid, ref serial_no, ref expiryDt);
            var result = new
            {
                SERIAL_NO = serial_no,
                EXPIRY_DATE = expiryDt,

            };
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

       



        }




    }
}