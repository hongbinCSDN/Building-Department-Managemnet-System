using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using System.IO;
using MWMS2.Utility;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Filter;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_PAController : Controller
    {

        public ActionResult Index(Fn01Search_PASearchModel model)
        {           
            return View(model);
        }

        public FileResult GetSignImg(string filepath)
        {
            RegistrationCommonService rs = new RegistrationCommonService();

            return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");
            //ImageUtil image = new ImageUtil();
            //string fullPath = image.retreiveAbsoluteSignatureFilePath(filepath);
            //byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            //var response = new FileContentResult(fileBytes, "application/octet-stream");
            //response.FileDownloadName = "Sign.jpg";
            //return response;

        }

        public ActionResult Form(string id)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            return View("FormPA", rs.ViewPA(id));

            //return View();
        }

        public ActionResult Search(Fn01Search_PASearchModel model)
       {                       
            RegistrationSearchService rs = new RegistrationSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchPA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        ///public ActionResult Search(FormCollection post)
        ///{
        ///    RegistrationSearchService rs = new RegistrationSearchService();
        ///    return Content(JsonConvert.SerializeObject(rs.SearchPA(post), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        ///}
        [HttpPost]
        public ActionResult Excel(Fn01Search_PASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportPA(model) });
        }
 

    }
}