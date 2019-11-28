﻿using MWMS2.Models;
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
    public class Fn02GCA_RPTController : Controller
    {
        // GET: Registration/Fn02GCA_USA
        public ActionResult Index(CRMReportModel model)
        {
            return View(model);
        }
        public virtual string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }
        public FileStreamResult submitPnlInfoExportConvictionCover(string CompName)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportPnlInfoExportConvictionCover(CompName);
        }
        public ActionResult MMD0003b_1_CGC(DateTime? dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCompGazettememoData(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_CGA, acting, authId);
        }
        public ActionResult MMD0003b_4_CGC(DateTime dataOfGazette, string acting, string authId)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportWeeklyNotice(dataOfGazette, RegistrationConstant.REGISTRATION_TYPE_CGA, acting, authId);
        }
        public ActionResult JavaExportCGCLabelTwoORThree
            (
            string ReportName,string txtFileRef,
            DateTime? GazDateFro, DateTime? GazDateTo,
            DateTime? ExpDateFro, DateTime? ExpDateTo, 
            string ddPNRCUUID, string ddCtrUUID,string textCompName,
            string chkExpiryDate, string dd2Cols, 
            string dd2Rows, string dd3Cols,string dd3Rows
            )
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCGCLabelTwoORThree
            (
            ReportName, txtFileRef,
            RegistrationConstant.REGISTRATION_TYPE_CGA, GazDateFro, GazDateTo,
            ExpDateFro,ExpDateTo, ddPNRCUUID, ddCtrUUID,textCompName,
            chkExpiryDate, dd2Cols, dd2Rows, dd3Cols, dd3Rows
            );
        }
        public ActionResult JavaExportCGCLabelTwoORThreeInterview(
            string ReportName,string exportTypeInterV,
            string MeetingGroup,
            string dd2ColsIntV, string dd2RowsIntV,
            string dd3ColsIntV,string dd3RowsIntV)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportCGCLabelTwoORThreeInterview(
                ReportName, RegistrationConstant.REGISTRATION_TYPE_CGA,
                exportTypeInterV,MeetingGroup,
                dd2ColsIntV, dd2RowsIntV,
                dd3ColsIntV, dd3RowsIntV);
        }
        public ActionResult ExportGBCSC(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportGBCSCReport(model, RegistrationConstant.REGISTRATION_TYPE_CGA);
        }
        public ActionResult ExportAsTdOoExpInfo(CRMReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            return rs.ExportAsTdOoExpInfoReport(model, RegistrationConstant.REGISTRATION_TYPE_CGA);
        }
    }    
}