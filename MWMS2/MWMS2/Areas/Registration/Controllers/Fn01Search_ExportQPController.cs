using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using System.IO;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Filter;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_ExportQPController : Controller
    {
        public virtual string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }

        public ActionResult Index()
        {
            Fn01Search_ExportQPSearchModel model = new Fn01Search_ExportQPSearchModel();
            return View(model);
        }

        public FileStreamResult ExportQPExcelData(Fn01Search_ExportQPSearchModel model)
        {
            RegistrationSearchExportQPService rs = new RegistrationSearchExportQPService();
           model.RegType = registrationType();
            return rs.exportQPExcelData(model);

        }
    }
}