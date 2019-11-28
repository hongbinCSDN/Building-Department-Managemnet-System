using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn06AS_DBController : Controller
    {
        public ActionResult Index(Fn06AS_DBSearchModel model)
        {
           
            return View(model);
        }
        public ActionResult Search(Fn06AS_DBSearchModel model)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchAS(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string APPUUID, string STATUS)
        {
            RegistrationASService rs = new RegistrationASService();
            Fn06AS_DBDisplayModel model = rs.ViewASdb(APPUUID, STATUS);
            return View("FormAS_DB", model);
        }
        public ActionResult ASdata(Fn06AS_DBDisplayModel model)
        {
            RegistrationASService rs = new RegistrationASService();
            rs.GetASdata(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public FileResult DownloadFile(string path)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            var supportedTypes = new[] { "jpeg", "png", "pdf", "gif", "jpg" };
            var fileExt = "";
            fileExt = System.IO.Path.GetExtension(path).Substring(1);
            string tempPath = ApplicationConstant.CRMFilePath;
            tempPath += RegistrationConstant.ASFORM_PATH + ApplicationConstant.FileSeparator.ToString();
            byte[] fileBytesrs = rs.DownloadFile(tempPath + path);
            if (fileExt != null && !"".Equals(fileExt))
            {
                fileExt = fileExt.ToLower();
            }
            if (fileExt == "gif")
            {
                return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Gif, "ASFormFile." + fileExt);
            }
            else
            {
                return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Jpeg, "ASFormFile." + fileExt);
            }
        }

        public ActionResult GetSignImg(string uuid)
        {
            //RegistrationASService rs = new RegistrationASService();
            CommonBLService ss = new CommonBLService();

            var file = ss.ViewCompCRMImageByUUID(uuid);
                //rs.ViewCRMImageByUUID(uuid);

            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }
        }

        public ActionResult Save (Fn06AS_DBSearchModel model2,Fn06AS_DBDisplayModel model, IEnumerable<HttpPostedFileBase> ASdbForm)
        {
            RegistrationASService rs = new RegistrationASService();
            var supportedTypes = new[] { "jpeg","png","pdf", "gif", "jpg" };
            var fileExt = "";
            if (ASdbForm.ElementAt(0) != null)
            {
                fileExt = System.IO.Path.GetExtension(ASdbForm.ElementAt(0).FileName).Substring(1).ToLower();
            }
            if (!supportedTypes.Contains(fileExt) && fileExt!="")
            {
                Fn06AS_DBDisplayModel model1 = rs.ViewASdb(model.C_APPLICANT.UUID,model.STATUS);
                model1.SaveSuccess = false;
                model1.ErrorMessage = "Invalid file type, please confrim the file type.";
                return View("FormAS_DB", model1);
            }
            else
            {
                string tempPath = ApplicationConstant.CRMFilePath;
                tempPath += RegistrationConstant.ASFORM_PATH;
                rs.SaveAS_DB(model, tempPath, ASdbForm);
                RegistrationSearchService rs2 = new RegistrationSearchService();
                rs2.SearchAS(model2);
                //model1.SaveSuccess = true;
                return View("Index", model2);
            }
        }
        public ActionResult RemoveConsentImg(string uuid)
        {
            RegistrationASService rs = new RegistrationASService();
            rs.DeleteASImg(uuid);

            return Json(new { });
            //return View("FormAS_DB");
        }
        [HttpPost]
        public ActionResult ExcelListOfAS(Fn06AS_DBSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);


            RegistrationSearchService rs = new RegistrationSearchService();
         
            return Json(new { key = rs.ExportAS(model) });
        }

    }
}