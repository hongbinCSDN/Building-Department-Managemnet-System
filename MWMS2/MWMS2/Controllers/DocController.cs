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
    public class DocModel
    {
        public string  Keyword { get; set; }
        public List<HttpPostedFileBase> uloadFile { get; set; }
        public string Session { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Dsn { get; set; }
        public string Stage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Data { get; set; }
        public string sheet { get; set; }

        public DRMS_DOCUMENT_META_DATA Doc { get; set; }
        public string SUB { get; set; }



    }

    public class DocController : Controller
    {


        private static volatile DocBLService _BL;
        private static readonly object locker = new object();
        private static DocBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new DocBLService(); return _BL; } }

        /*
        public ActionResult GetSession(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetSession(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        */
        public ActionResult ChangeInfo(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.ChangeInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult LoadDir(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadDir(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult DeleteDoc(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.DeleteDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult Upload(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Upload(model, Request.Files), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult GenerateNewDsn(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GenerateNewDsn(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }


        public ActionResult BatchUpdate(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.BatchUpdate(model, Request.Files), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }


        public ActionResult LoadSubDocs(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadSubDocs(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult LoadDocInfo(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadDocInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult LoadDocRelated(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.LoadDocRelated(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        
        public ActionResult SearchDoc(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        

        public FileResult Download(string fileType, string fileUuid)
        {
            var rs = BL.Download(fileType, fileUuid);
            return File(rs.data, rs.type, rs.name);
        }



        public ActionResult Login(DocModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Login(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        /*

    public ActionResult Req(AtcpModel model)
    {
        if (string.IsNullOrWhiteSpace(model.dataSource)) return Json(new { });
        return Content(JsonConvert.SerializeObject((BL.GetType()).GetMethod(model.dataSource).Invoke(BL, new object[] { model }), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
    }*/
    }
}