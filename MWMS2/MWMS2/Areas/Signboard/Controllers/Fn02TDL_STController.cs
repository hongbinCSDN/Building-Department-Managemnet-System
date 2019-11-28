using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.SignboardServices;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn02TDL_STController : Controller
    {
        // GET: Signboard/Fn01SCUR_ST
        public ActionResult Index(Fn02TDL_STSearchModel model)
        {
            model.ChildList = SystemListUtil.GetScuTeamChildList(SessionUtil.LoginPost.UUID);
            return View(model);
        }
        public ActionResult Search(Fn02TDL_STSearchModel model)
        {
            SignboardTDL_SubordinateTaskService sts = new SignboardTDL_SubordinateTaskService();
            return Content(JsonConvert.SerializeObject(sts.SearchTDL_ST(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Detail(string uuid)
        {
            SignboardTDL_SubordinateTaskService sts = new SignboardTDL_SubordinateTaskService();
            ViewBag.WF_MAP_VALIDATION_TO = SignboardConstant.WF_MAP_VALIDATION_TO;
            ViewBag.WF_MAP_VALIDATION_PO = SignboardConstant.WF_MAP_VALIDATION_PO;
            ViewBag.WF_MAP_VALIDATION_SPO = SignboardConstant.WF_MAP_VALIDATION_SPO;
            ViewBag.DISPLAY_WF_MAP_VALIDATION_TO = SignboardConstant.DISPLAY_WF_MAP_VALIDATION_TO;
            ViewBag.DISPLAY_WF_MAP_VALIDATION_PO = SignboardConstant.DISPLAY_WF_MAP_VALIDATION_PO;
            ViewBag.DISPLAY_WF_MAP_VALIDATION_SPO = SignboardConstant.DISPLAY_WF_MAP_VALIDATION_SPO;

            ViewBag.WF_MAP_AUDIT_TO = SignboardConstant.WF_MAP_AUDIT_TO;
            ViewBag.WF_MAP_AUDIT_PO = SignboardConstant.WF_MAP_AUDIT_PO;
            ViewBag.WF_MAP_AUDIT_SPO = SignboardConstant.WF_MAP_AUDIT_SPO;
            ViewBag.DISPLAY_WF_MAP_AUDIT_TO = SignboardConstant.DISPLAY_WF_MAP_AUDIT_TO;
            ViewBag.DISPLAY_WF_MAP_AUDIT_PO = SignboardConstant.DISPLAY_WF_MAP_AUDIT_PO;
            ViewBag.DISPLAY_WF_MAP_AUDIT_SPO = SignboardConstant.DISPLAY_WF_MAP_AUDIT_SPO;

            ViewBag.WF_MAP_VALIDATION_ISSUE_LETTER_TO = SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO;
            ViewBag.DISPLAY_WF_MAP_VALIDATION_ISSUE_LETTER_TO = SignboardConstant.DISPLAY_WF_MAP_VALIDATION_ISSUE_LETTER_TO;

            return View("Detail", sts.populateSubordinateTaskSearchForm(uuid));
        }
    }
}