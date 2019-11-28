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
using OfficeOpenXml;
using MWMS2.Entity;
using System.IO;
using System.Web.Configuration;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_LFController : Controller
    {
        //leaveFormPath..
        private const string leaveFormPath = "~/LeaveForm";

        // GET: Registration/Fn02GCA_PM
        public ActionResult Index(LeaveFormSearchModel model)
        {
            return View("IndexComp_LF",model);
        }

        public ActionResult Search(LeaveFormSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.SearchComp_LF(model, RegistrationConstant.REGISTRATION_TYPE_CGA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string compAppInfoMasterId, string compAppInfoUUID)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            LeaveFormDisplayModel model = rs.ViewCompLF(compAppInfoMasterId, compAppInfoUUID, RegistrationConstant.REGISTRATION_TYPE_CGA, leaveFormPath);
            return View("FormComp_LF", model);
        }
        public ActionResult Save(LeaveFormDisplayModel model, IEnumerable<HttpPostedFileBase> LeaveFormForm)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            if (LeaveFormForm.ElementAt(0) == null && LeaveFormForm.ElementAt(1) ==null)
            {
                LeaveFormDisplayModel model1 = rs.ViewCompLF(model.C_COMP_APPLICANT_INFO.MASTER_ID, model.C_COMP_APPLICANT_INFO.UUID, RegistrationConstant.REGISTRATION_TYPE_CGA, leaveFormPath);
                model1.SaveSuccess = false;
                model1.ErrorMessage = "please input the file";
                return View("FormComp_LF", model1);
            }
            var supportedTypes = new[] { "pdf", "gif", "jpg","png" };
            var fileExt="";
            if (LeaveFormForm.ElementAt(0)!=null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(0).FileName).Substring(1);
            }
            if (LeaveFormForm.ElementAt(1) != null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(1).FileName).Substring(1);
            }
            if (!supportedTypes.Contains(fileExt))
            {
                LeaveFormDisplayModel model1 = rs.ViewCompLF(model.C_COMP_APPLICANT_INFO.MASTER_ID, model.C_COMP_APPLICANT_INFO.UUID, RegistrationConstant.REGISTRATION_TYPE_CGA, leaveFormPath);
                model1.SaveSuccess = false;
                model1.ErrorMessage = "Invalid file type, please confrim the file type.";
                return View("FormComp_LF", model1);
            }
            else
            {
                string tempPath = ApplicationConstant.CRMFilePath;
                tempPath += RegistrationConstant.LEAVEFORM_PATH;
                //string tempPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "Image\\" + RegistrationConstant.LEAVEFORM_PATH;
                if (!Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    catch (Exception ex)
                    {
                        LeaveFormDisplayModel model2 = rs.ViewCompLF(model.C_COMP_APPLICANT_INFO.MASTER_ID, model.C_COMP_APPLICANT_INFO.UUID, RegistrationConstant.REGISTRATION_TYPE_CGA, leaveFormPath);
                        model2.ErrorMessage = ex.Message.ToString().Replace("'", "").Replace("\\", "/");

                        return View("FormComp_LF", model2);
                    }
                }

                rs.SaveComp_LF(model, tempPath, LeaveFormForm);
                //rs.SaveComp_LF(model, Server.MapPath(leaveFormPath), LeaveFormForm);
                LeaveFormDisplayModel model1 = rs.ViewCompLF(model.C_COMP_APPLICANT_INFO.MASTER_ID, model.C_COMP_APPLICANT_INFO.UUID, RegistrationConstant.REGISTRATION_TYPE_CGA, leaveFormPath);
                model1.SaveSuccess = true;
                return View("FormComp_LF", model1);
            }
        }
        public ActionResult Validate(LeaveFormDisplayModel model)
        {
            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(validateResult);
            return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
        }
        public FileResult DownloadFile(string path)
        {
            //Server.MapPath(leaveFormPath)
            //File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LeaveFromFile.jpg");
            RegistrationCommonService rs = new RegistrationCommonService();
            var supportedTypes = new[] { "pdf", "gif", "jpg" };
            var fileExt = "";
            fileExt = System.IO.Path.GetExtension(path).Substring(1);
            string tempPath = ApplicationConstant.CRMFilePath;
            tempPath += RegistrationConstant.LEAVEFORM_PATH;
            byte[] fileBytesrs = rs.DownloadFile(tempPath+ ApplicationConstant.FileSeparator+ path);
            //byte[] fileBytesrs=rs.DownloadFile(Server.MapPath(leaveFormPath), path);

            if (fileExt == "gif")
            {
                return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Gif, "LeaveFromFile." + fileExt);
            }
            else
            {
                return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Jpeg, "LeaveFromFile." + fileExt);
            }
        }
        public ActionResult Excel(LeaveFormSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService s = new RegistrationCommonService();
            var regType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Json(new { key = s.ExportComp_LF(model, regType) }); 
        }

    }
}