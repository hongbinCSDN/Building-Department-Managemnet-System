using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;


namespace MWMS2.Areas.Registration.Controllers
{
    public class ValidationController : Controller
    {
        private ServiceResult _serviceResult;
        public ServiceResult ValidateResult
        {
            get
            {
                if (_serviceResult == null) _serviceResult = ValidationUtil.ValidateForm(ModelState, ViewData);
                return _serviceResult;
            }
        }
    }
}