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
    public class Fn05MWIA_ExportLetterController : Fn02GCA_ExportLetterController
    {
        public override string registrationType(){
            return RegistrationConstant.REGISTRATION_TYPE_MWIA;
        }

        public override ActionResult Index()
        {
            Fn02GCA_ExportLetterSearchModel model = new Fn02GCA_ExportLetterSearchModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            return View("SearchExportLetter", model);
        }


    }
}