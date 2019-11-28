using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class CommonController : Controller
    {



        private static volatile CommonBLService _BL;
        private static readonly object locker = new object();
        private static CommonBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new CommonBLService(); return _BL; } }

  
        public FileResult ViewCRMImage(String V_CRM_INFO_UUID)
        {
            return BL.ViewCRMImage(V_CRM_INFO_UUID); ;

        }

    }
}