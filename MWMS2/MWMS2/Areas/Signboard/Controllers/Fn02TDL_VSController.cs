using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services.Signborad.SignboardServices;
using MWMS2.Constant;
using MWMS2.Services.Signborad.SignboardDAO;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn02TDL_VSController : Controller
    {
        // GET: Signboard/Fn02TSK_VS
        public ActionResult Index(Fn02TDL_VSSearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn02TDL_VSSearchModel model)
        {
            SignboardTDLService rs = new SignboardTDLService();
            return Content(JsonConvert.SerializeObject(rs.SearchTDL_VS(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Form(string refNo)
        {
            SignboardTDLService rs = new SignboardTDLService();
            Fn02TDL_VSDisplayModel model = rs.ViewVS(refNo);
            return View(model);
        }

        public ActionResult SearchSBInfo(Fn02TDL_VSDisplayModel model)
        {
            SignboardTDLService rs = new SignboardTDLService();
            rs.SearchSB_Info(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        [HttpPost]
        public ActionResult Excel(Fn02TDL_VSSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardTDLService rs = new SignboardTDLService();
            return Json(new { key = rs.ExportTDL_VS(model) });
        }

        [HttpPost]
        public ActionResult LoadHandlingOfficers(string svValidationId)
        {
            SignboardTDLService rs = new SignboardTDLService();
            var result = rs.loadValidationHandlingOfficers(svValidationId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult DownloadFile(string uuid)
        {
            SignboardCommonDAOService dao = new SignboardCommonDAOService();
            var file = dao.viewScannedDocumentFile(uuid);
            if (file == null)
            {
                return Content("Image not found.");
            }
            else
            {
                return file;
            }
        }

        public string viewPhotoLibImage(string uuid)
        {
            SignboardCommonDAOService dao = new SignboardCommonDAOService();
            string url = dao.viewPhotoLibImage(uuid);

            return url;
        }
    }
}