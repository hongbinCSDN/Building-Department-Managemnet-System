using MWMS2.DaoController;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SMMSCUTeamManController : Controller
    {
        private static volatile SMMSCUTeamManBLService _BL;
        private static readonly object locker = new object();
        private static SMMSCUTeamManBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new SMMSCUTeamManBLService(); return _BL; } }

        // GET: Admin/RM
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AjaxTeamData()
        {
            return Content(JsonConvert.SerializeObject(BL.AjaxTeamData(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}











