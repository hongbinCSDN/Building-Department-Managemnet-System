using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn08ADM_RPTController : Controller
    {
        private RegistrationDataExportService _rs;
        protected RegistrationDataExportService rs
        {
            get
            {
                return _rs ?? (_rs = new RegistrationDataExportService());
            }
        }


        public ActionResult Index(Fn08ADM_ReportModel model)
        {
            //RegistrationDataExportService rs = new RegistrationDataExportService();
            rs.loadApplication(model);
            return View(model);
        }
        public ActionResult ExportAdmZReport(Fn08ADM_ReportModel model)
        {
            
            Response.AddHeader("Content-Disposition", "attachment; filename=Admin.zip");
            return File(rs.ExportApplicationStagesReport(model), "application/zip");
        }
        public ActionResult GetCheckBoxValue(Fn08ADM_ReportModel model)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            //return rs.ExportApplicationStagesReport(model);
            return null;
        }

    }
}