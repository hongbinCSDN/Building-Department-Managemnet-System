using MWMS2.Areas.Registration.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_RCRController : Controller
    {
        public ActionResult Index(Fn01Search_RCRSearchModel model)
        {
            return View("Index", model);
        }
        public ActionResult ExportExcelReport(Fn01Search_RCRSearchModel model)
        {
            string ExportType = "Exc";
            RegistrationRegisteredCompReportService rs = new RegistrationRegisteredCompReportService();
            return rs.exportRegisteredCompanyReport(model, ExportType);
        }
        public ActionResult exportCSVReport(Fn01Search_RCRSearchModel model)
        {
            string ExportType = "CSV";
            RegistrationRegisteredCompReportService rs = new RegistrationRegisteredCompReportService();
            return rs.exportRegisteredCompanyReport(model, ExportType);
        }
        
    }
}