using MWMS2.Models;
using MWMS2.Utility;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using MWMS2.Constant;
using System.Web.Configuration;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn05MWIA_RPTController : Controller
    {
        public ActionResult Index(CRMReportModel model)
        {
            model.checkboxAddr = true;
            model.checkboxTel = true;
            model.checkboxEmail = true;
            model.checkboxFax = true;
            model.checkboxEmergency = true;
            return View(model);
        }
        [HttpPost]
        public ActionResult Excel(CRMReportModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService rs = new RegistrationCommonService();
           
            return Json(new { key = rs.ExportIPImg(model, RegistrationConstant.REGISTRATION_TYPE_MWIA) });
        }

        public ActionResult SearchIndCertRecord(CRMReportModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.SearchIPImg(model, RegistrationConstant.REGISTRATION_TYPE_IP);
            return Content(JsonConvert.SerializeObject(rs.SearchIPImg(model, RegistrationConstant.REGISTRATION_TYPE_MWIA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExportMMD0003a_1(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWIA, acting, authId);
        }
        public ActionResult ExportMMD0003a_2(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWIA, acting, authId);
        }
        public ActionResult ExportMMD0003a_3(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWIA, acting, authId);
        }
        public ActionResult ExportMMD0003a_4(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportMwIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWIA, acting, authId);
        }
        public ActionResult JavaExportProLabelTwo(CRMReportModel model)
        {
            string reportName = "LabelTwo";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportProLabelTwoORThree(model, RegistrationConstant.REGISTRATION_TYPE_IP, reportName);
        }
        public ActionResult JavaExportProLabelThree(CRMReportModel model)
        {
            string reportName = "LabelThree";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportProLabelTwoORThree(model, RegistrationConstant.REGISTRATION_TYPE_IP, reportName);
        }
        public ActionResult JavaExportIPLabelTwoInterview(CRMReportModel model)
        {
            string reportName = "IPLabelTwoInterview";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportProLabelTwoORThreeInterview(model, RegistrationConstant.REGISTRATION_TYPE_IP, reportName);
        }
        public ActionResult JavaExportIPLabelThreeInterview(CRMReportModel model)
        {
            string reportName = "IPLabelThreeInterview";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportProLabelTwoORThreeInterview(model, RegistrationConstant.REGISTRATION_TYPE_IP, reportName);
        }
        public FileStreamResult exportIPConvictionCover(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIPConvictionCover(model);
        }
        public ActionResult ExportAPRSERGEExpInfo(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportAPRSERGEExpInfoReport(model, RegistrationConstant.REGISTRATION_TYPE_MWIA);
        }
        public FileResult DownloadFile(string FILEPATH, CRMReportModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            var supportedTypes = new[] { "pdf", "gif", "jpg" };
            var fileExt = "";
            fileExt = System.IO.Path.GetExtension(FILEPATH).Substring(1);
            string tempPath = ApplicationConstant.CRMFilePath;
            tempPath += "MWC_W";
            byte[] fileBytesrs = rs.DownloadFile(tempPath + ApplicationConstant.FileSeparator + FILEPATH);
            return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Jpeg, "temp." + fileExt);
        }
    }
}