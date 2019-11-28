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
using MWMS2.Constant;
using System.Web.Configuration;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_MWIAController : Controller
    {
        public ActionResult Index(Fn01Search_MWIASearchModel model)
        {
        
          

            return View(model);
        }
        public ActionResult Form(string id)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            return View("FormMWIA", rs.ViewMWIA(id));

            //return View();
        }
        public ActionResult Search(Fn01Search_MWIASearchModel model)
        {
           

            RegistrationSearchService rs = new RegistrationSearchService();
            rs.SearchMWIA(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        

        [HttpPost]
        public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportMWIA(Columns, post) });
        }

        public ActionResult GetSignImg(string filepath)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            
          //  return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");
            if (rs.DownloadDocBySubpath(filepath).Length > 0)
            {
                return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");

            }
            else
            {

                return Content(JsonConvert.SerializeObject("File Not Found", Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            }
        }
    }
}