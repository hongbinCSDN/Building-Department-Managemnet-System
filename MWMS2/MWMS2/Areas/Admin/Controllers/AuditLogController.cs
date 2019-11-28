using MWMS2.Areas.Admin.Models;
using MWMS2.Models;
using MWMS2.Services.AdminService.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class AuditLogController : Controller
    {
        // GET: Admin/AuditLog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(AuditLogSearchModel model)
        {
            AuditLogDAOService rs = new AuditLogDAOService();
            return Content(JsonConvert.SerializeObject(rs.SearchAL(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(AuditLogSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AuditLogDAOService rs = new AuditLogDAOService();
            return Json(new { key = rs.ExportAL(model) });
        }
    }
}