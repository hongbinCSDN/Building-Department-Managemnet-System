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
using System.Web.Configuration;
using System.IO;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn05MWIA_LFController : Controller
    {
        //leaveFormPath..
        private const string leaveFormPath = "~/LeaveForm";
        public ActionResult Index()
        {
            return View("IndexInd_LF");
        }

        public ActionResult Search(LeaveFormSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.SearchInd_LF(model, RegistrationConstant.REGISTRATION_TYPE_MWIA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string indAppUUID, string indCertUUID)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            LeaveFormDisplayModel model = rs.ViewIndLF(indAppUUID, indCertUUID, RegistrationConstant.REGISTRATION_TYPE_MWIA);
            return View("FormInd_LF", model);
        }
        public ActionResult Save([Bind(Exclude = "")]LeaveFormDisplayModel model, IEnumerable<HttpPostedFileBase> LeaveFormForm)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            if (LeaveFormForm.ElementAt(0) == null && LeaveFormForm.ElementAt(1) == null&&LeaveFormForm.ElementAt(2) == null && LeaveFormForm.ElementAt(3) == null)
            {
                LeaveFormDisplayModel model1 = rs.ViewIndLF(model.C_IND_APPLICATION.UUID, model.C_IND_CERTIFICATE.UUID, RegistrationConstant.REGISTRATION_TYPE_IP);
                model1.SaveSuccess = false;
                model1.ErrorMessage = "please input the file";
                return View("FormInd_LF", model1);
            }
            var supportedTypes = new[] { "pdf", "gif", "jpg" };
            var fileExt = "";
            if (LeaveFormForm.ElementAt(0) != null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(0).FileName).Substring(1);
            }
            if (LeaveFormForm.ElementAt(1) != null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(1).FileName).Substring(1);
            }
            if (LeaveFormForm.ElementAt(2) != null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(2).FileName).Substring(1);
            }
            if (LeaveFormForm.ElementAt(3) != null)
            {
                fileExt = System.IO.Path.GetExtension(LeaveFormForm.ElementAt(3).FileName).Substring(1);
            }
            if (!supportedTypes.Contains(fileExt))
            {
                LeaveFormDisplayModel model1 = rs.ViewIndLF(model.C_IND_APPLICATION.UUID, model.C_IND_CERTIFICATE.UUID, RegistrationConstant.REGISTRATION_TYPE_IP);
                model1.SaveSuccess = false;
                model1.ErrorMessage = "Invalid file type, please confrim the file type.";
                return View("FormInd_LF", model1);
            }
            else
            {
                string tempPath = ApplicationConstant.CRMFilePath;
                tempPath += RegistrationConstant.LEAVEFORM_PATH;
                if (!Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.CreateDirectory(tempPath);
                    } 
                    catch(Exception ex)
                    {
                        LeaveFormDisplayModel model2 = rs.ViewIndLF(model.C_IND_APPLICATION.UUID, model.C_IND_CERTIFICATE.UUID, RegistrationConstant.REGISTRATION_TYPE_MWIA);
                        model2.ErrorMessage = ex.Message.ToString().Replace("'","").Replace("\\","/");
                        
                        return View("FormInd_LF", model2);
                    }
                    
                }
                rs.SaveInd_LF(model, tempPath, LeaveFormForm);
                //rs.SaveInd_LF(model, Server.MapPath(leaveFormPath), LeaveFormForm);
                LeaveFormDisplayModel model1 = rs.ViewIndLF(model.C_IND_APPLICATION.UUID, model.C_IND_CERTIFICATE.UUID, RegistrationConstant.REGISTRATION_TYPE_MWIA);
                model1.SaveSuccess = true;
                return View("FormInd_LF", model1);
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
            RegistrationCommonService rs = new RegistrationCommonService();
            var supportedTypes = new[] { "pdf", "gif", "jpg" };
            var fileExt = "";
            fileExt = System.IO.Path.GetExtension(path).Substring(1);
            string tempPath = ApplicationConstant.CRMFilePath; ;
            tempPath += RegistrationConstant.LEAVEFORM_PATH;
            byte[] fileBytesrs = rs.DownloadFile(tempPath + ApplicationConstant.FileSeparator + path);
            //byte[] fileBytesrs = rs.DownloadFile(Server.MapPath(leaveFormPath), path);
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
            RegistrationCommonService rs = new RegistrationCommonService();
            //model.RegType = RegistrationConstant.REGISTRATION_TYPE_IP;
            var regType = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            return Json(new { key = rs.ExportInd_LF(model, regType) });
        }
    }
}