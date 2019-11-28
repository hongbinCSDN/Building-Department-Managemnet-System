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

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn03PA_ExportController : Fn02GCA_ExportController
    {
        public override string registrationType(){
            return RegistrationConstant.REGISTRATION_TYPE_IP;
        }

        public override ActionResult IndexExportApplicationData()
        {
            CRMDataExportModel dataExport = new CRMDataExportModel();
            dataExport.RegType = registrationType();
            return View("IndexExport_Ind", dataExport);
        }
        public override FileStreamResult ExportApplicationData(CRMDataExportModel dataExport)
        {
            RegistrationDataExportService registrationCommonService = new RegistrationDataExportService();
            dataExport.RegType = registrationType();
            return registrationCommonService.exportIndApplicationData(dataExport);
        }


    }
}