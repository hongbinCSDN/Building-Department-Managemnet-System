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
    public class AtcpController : Controller
    {



        private static volatile AtcpBLService _BL;
        private static readonly object locker = new object();
        private static AtcpBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new AtcpBLService(); return _BL; } }

        public ActionResult Req(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.dataSource)) return Json(new { });
            return Content(JsonConvert.SerializeObject((BL.GetType()).GetMethod(model.dataSource).Invoke(BL, new object[] { model }), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult BlockId(int? UNIT_ID)
        {
            if (UNIT_ID == null) return Json(new { BLKID = ""});
            string r = BL.BlockId(UNIT_ID.Value);
            return Content(JsonConvert.SerializeObject(new {BLKID = r }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        
    }
    public class AtcpModel : DisplayGrid
    {

        public string id { get; set; }
        public string dataSource { get; set; }
        public int minChar { get; set; }
        public string term { get; set; }
        public Dictionary<string, string> atcpParameters { get; set; } = new Dictionary<string, string>();
    }
}