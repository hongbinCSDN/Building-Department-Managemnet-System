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
using MWMS2.Utility;
using MWMS2.Services.Signborad.SignboardDAO;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn02TDL_SSController : Controller
    {
        // GET: Signboard/Fn02TSK_SS
        public ActionResult Index(Fn02TDL_SSSearchModel model)
        {
            return View(model);
        }
        public ActionResult Form(string id)
        {
            SignboardTDLService ss = new SignboardTDLService();
            return View(ss.ViewSignboard(id));
        }
        public ActionResult Search(Fn02TDL_SSSearchModel model)
        {
            SignboardTDLService rs = new SignboardTDLService();
            return Content(JsonConvert.SerializeObject(rs.SearchTDL_SS(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn02TDL_SSSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardTDLService rs = new SignboardTDLService();
            return Json(new { key = rs.ExportTDL_SS(model) });
        }

        public ActionResult CheckThumbnailFilePath(string path)
        {
            SignboardTDLService tdl = new SignboardTDLService();
            return Json(new ServiceResult { Result = tdl.checkThumbnailFilePath(path) });
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