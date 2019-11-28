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

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn04MWCA_RPTController : Controller
    {
        public ActionResult Index(CRMReportModel model)
        {
            return View(model);
        }
        public ActionResult SearchIndCertRecord(CRMReportModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.SearchIPImg(model, RegistrationConstant.REGISTRATION_TYPE_MWCA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult MMD0003b_1_CGC(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCompGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWCA, acting, authId);
        }
        public ActionResult MMD0003b_4_CGC(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_MWCA, acting, authId);
        }
        public ActionResult JavaExportCGCLabelTwoORThree
            (
            string ReportName, string txtFileRef,
            DateTime? GazDateFro, DateTime? GazDateTo,
            DateTime? ExpDateFro, DateTime? ExpDateTo,
            string ddPNRCUUID, string ddCtrUUID, string textCompName,
            string chkExpiryDate, string dd2Cols,
            string dd2Rows, string dd3Cols, string dd3Rows
            )
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCGCLabelTwoORThree
            (
            ReportName, txtFileRef,
            RegistrationConstant.REGISTRATION_TYPE_MWCA, GazDateFro, GazDateTo,
            ExpDateFro, ExpDateTo, ddPNRCUUID, ddCtrUUID, textCompName,
            chkExpiryDate, dd2Cols, dd2Rows, dd3Cols, dd3Rows
            );
        }
        public ActionResult JavaExportCGCLabelTwoORThreeInterview(
            string ReportName, string exportTypeInterV,
            string MeetingGroup,
            string dd2ColsIntV, string dd2RowsIntV,
            string dd3ColsIntV, string dd3RowsIntV)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCGCLabelTwoORThreeInterview(
                ReportName, RegistrationConstant.REGISTRATION_TYPE_MWCA,
                exportTypeInterV, MeetingGroup,
                dd2ColsIntV, dd2RowsIntV,
                dd3ColsIntV, dd3RowsIntV);
        }
        public ActionResult ExportMWCInfoMWC(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportMWCReport(model, RegistrationConstant.REGISTRATION_TYPE_MWCA);
        }
        public ActionResult ExportAsTdOoExpInfo(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportAsTdOoExpInfoReport(model, RegistrationConstant.REGISTRATION_TYPE_MWCA);
        }
    }
}