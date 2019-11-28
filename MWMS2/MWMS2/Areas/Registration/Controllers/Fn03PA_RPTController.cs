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
    public class Fn03PA_RPTController : Controller
    {
        private const string ImgPath = "~/LeaveForm";
        // GET: Registration/Fn02GCA_USA
        public ActionResult Index(CRMReportModel model)
        {
            return View(model);
        }
        public ActionResult SearchIndCertRecord(CRMReportModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.SearchIPImg(model, RegistrationConstant.REGISTRATION_TYPE_IP);
            return Content(JsonConvert.SerializeObject(rs.SearchIPImg(model, RegistrationConstant.REGISTRATION_TYPE_IP), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult DownloadFile(string UUID)
        {
            //RegistrationCommonService rs = new RegistrationCommonService();
            //var supportedTypes = new[] { "pdf", "gif", "jpg" };
            //var fileExt = "";
            //if (FILEPATHNO != null) {
            //fileExt = System.IO.Path.GetExtension(FILEPATHNO).Substring(1);

            //string tempPath = ApplicationConstant.CRMFilePath;

            //tempPath += "MWC_W";//RegistrationConstant.LEAVEFORM_PATH;
            //byte[] fileBytesrs = rs.DownloadFile(tempPath+ ApplicationConstant.FileSeparator + FILEPATHNO);

            //    return File(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Jpeg, "temp." + fileExt);

            //}
            //else
            //{
            //    return null;
            //}
            RegistrationDataExportService rs = new RegistrationDataExportService();
            var file = rs.getIndCertImgByUUID(UUID);
            if(file != null)
            {
                return file;
            }
            else
            {
                return Content("Image not found.");
            }
        }
        public ActionResult ExportMMD0003a_1_IP(DateTime? dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_CGA, acting, authId);
        }
        public ActionResult ExportMMD0003a_2_IP(DateTime? dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_CGA, acting, authId);
        }
        public ActionResult ExportMMD0003a_3_IP(DateTime? dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_CGA, acting, authId);
        }
        public ActionResult ExportMMD0003a_4_IP(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4RIA(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "RI(A)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4RIE(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "RI(E)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4RIS(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "RI(S)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4APA(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "AP(A)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4APE(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "AP(E)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4APS(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "AP(S)";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4RSE(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "RSE";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult exportMMD0003a_4RGE(DateTime? dataOfGazette, string acting, string authId)
        {
            string cat_code = "RGE";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportIndivWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_IP, acting, authId, cat_code);
        }
        public ActionResult JavaExportProLabelTwo(CRMReportModel model)
        {
            string reportName = "LabelTwo";
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportProLabelTwoORThree(model,RegistrationConstant.REGISTRATION_TYPE_IP, reportName);
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
            return rs.ExportAPRSERGEExpInfoReport(model, RegistrationConstant.REGISTRATION_TYPE_IP);
        }
    }
}