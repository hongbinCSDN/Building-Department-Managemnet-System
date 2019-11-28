using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class SMMDocModel
    {
        public string  Keyword { get; set; }
        public List<HttpPostedFileBase> uloadFile { get; set; }
        public string Session { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Dsn { get; set; }
        public string Data { get; set; }

        public DRMS_DOCUMENT_META_DATA Doc { get; set; }




    }

    public class SMMDocController : Controller
    {


        private static volatile S_DocBLService _BL;
        private static readonly object locker = new object();
        private static S_DocBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new S_DocBLService(); return _BL; } }


        public ActionResult GetSession(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetSession(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ChangeInfo(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.ChangeInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult LoadDir(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadDir(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult DeleteDoc(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.DeleteDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult Upload(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Upload(model, Request.Files), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult LoadDocInfo(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadDocInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult SearchDoc(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        

        public FileResult Download(string fileType, string fileUuid)
        {
            var rs = BL.Download(fileType, fileUuid);
            return File(rs.data, rs.type, rs.name);
        }



        public ActionResult Login(SMMDocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Login(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
  
    }
}