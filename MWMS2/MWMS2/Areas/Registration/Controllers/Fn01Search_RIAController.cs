using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using System.IO;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Filter;
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_RIAController : Controller
    {

        public ActionResult Index(Fn01Search_RIASearchModel model)
        {
            return View(model);
        }
        public ActionResult Form(string id)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            Fn01Search_RIADisplayModel model = rs.ViewRIA(id);
            return View(model);
        }
        public ActionResult Search(Fn01Search_RIASearchModel model)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchRIA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn01Search_RIASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportRIA(model) });
        }
        public ActionResult GetSignImg(string uuid)
        {
            RegistrationCommonService rs = new RegistrationCommonService();

            var file = rs.getRIASigByUUID(uuid);

            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }

            //if (rs.DownloadDocBySubpath(filepath).Length > 0)
            //{
            //    return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");

            //}
            //else
            //{

            //    return Content(JsonConvert.SerializeObject("File Not Found", Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //}

        }
    }
}