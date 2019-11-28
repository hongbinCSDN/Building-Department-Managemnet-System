using MWMS2.Areas.Signboard.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_TBESController : Controller
    {
        // GET: Signboard/Fn04RPT_TBES
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SearchTBES(Fn04PRT_TBESSearchModel model)
        {
            SignboardRPTServices ss = new SignboardRPTServices();
            return ss.ExportTBESReport(model);
          
        }

    }
}