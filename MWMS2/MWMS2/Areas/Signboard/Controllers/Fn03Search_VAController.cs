using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.SignboardServices;
using MWMS2.Utility;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn03Search_VAController : Controller
    {
        // GET: Signboard/Fn03AS_VA
        public ActionResult Index(Fn03SRC_VASearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn03SRC_VASearchModel model)
        {
     
            SignboardAS_ValidationApplicationService vas = new SignboardAS_ValidationApplicationService();
            return Content(JsonConvert.SerializeObject(vas.SearchSRC_VA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03SRC_VASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardAS_ValidationApplicationService vas = new SignboardAS_ValidationApplicationService();
            return Json(new { key = vas.ExportSRC_VA(model) });
        }
        public ActionResult goToValidationToDoTO(string uuid, string type)
        {
            SessionUtil.DraftObject = null;
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ValidationDisplayModel model = new ValidationDisplayModel();
            model.TaskType = type;
            //model.EditMode = SignboardConstant.VIEW_MODE;
            model.EditMode = SignboardConstant.EDIT_MODE;
            if (model.SaveMode == "submit")
            {
                return View("Index");
            }
            else
            {
                return View("Validation", ss.ViewValidation(uuid, model));
            }

        }
    }
}