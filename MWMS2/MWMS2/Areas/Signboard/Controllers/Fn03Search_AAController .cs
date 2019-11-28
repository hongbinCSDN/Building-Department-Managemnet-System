using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn03Search_AAController : Controller
    {
        // GET: Signboard/Fn03AS_VA
        public ActionResult Index(Fn03SRC_AASearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn03SRC_AASearchModel model)
        {
            SignboardAS_AuditApplicationService rs = new SignboardAS_AuditApplicationService();
            return Content(JsonConvert.SerializeObject(rs.SearchSRC_AA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03SRC_AASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardAS_AuditApplicationService rs = new SignboardAS_AuditApplicationService();
            return Json(new { key = rs.ExportSRC_AA(model) });
        }
        // --------------------------------------------------------------------------------------------
        //[HttpPost]
        public ActionResult Detail(string uuid)
        {
            SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();
            return View("Detail", ss.ViewAudit(uuid));
        }
        [HttpPost]
        public ActionResult AuditExport(Fn03SRC_AADisplayModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();
            // Fn03SRC_AADisplayModel model = ss.ViewAudit(id);

            //return Json(new { key = ss.ExportAuditData(model) });
            return ss.ExportAuditData(model);
        }
        [HttpPost]
        public ActionResult AuditExportExcel(Fn03SRC_AADisplayModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();
            // Fn03SRC_AADisplayModel model = ss.ViewAudit(id);

            //return Json(new { key = ss.ExportAuditData(model) });
            return ss.ExportAuditDataToExcel(model);
        }
        //[HttpPost]
        public ActionResult List(string auditUuid, string RecordType, string EditMode)
        {
            // if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right

            return View("Comment", cs.list(RecordType, auditUuid, EditMode));
        }
        //[HttpPost]
        public ActionResult Load(CommentDisplayModel model)
        {
            // if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right

            return View("Comment", cs.load(model));
        }
        [HttpPost]
        public ActionResult Post(CommentDisplayModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right
            cs.post(model);
            if(model.EditMode.Equals("add"))
            {
                model.JavascriptToRun = "closeWindow()";
                return View("Comment", model); // randomly return sth
                //return RedirectToAction("Index", "Fn02TDL_TDL");
            }
            //if(model.EditMode.Equals("edit"))
            else
            {
                return View("Comment", cs.list(model.RecordType, model.RecordId, "add"));
            }
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Fn03SRC_AADisplayModel model)
        {
            SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();
            // check security right
            try
            {
                Fn03SRC_AADisplayModel newModel = new Fn03SRC_AADisplayModel();
                newModel = ss.SaveAuditRecord(model);
                if (/*!newModel.ErrMsg.Equals("")*/ newModel.ErrMsg != null || newModel.EditMode.Equals(SignboardConstant.SAVE_MODE))
                {
                    // newModel = ss.ViewAudit(model.Uuid);
                    // return Detail(newModel.Uuid);
                    return Detail(newModel.Uuid);
                }
            }
            catch
            {
            }

            //return RedirectToAction("Index", "Fn02TDL_TDL");
            return Json(new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
            });
        }

    }
}