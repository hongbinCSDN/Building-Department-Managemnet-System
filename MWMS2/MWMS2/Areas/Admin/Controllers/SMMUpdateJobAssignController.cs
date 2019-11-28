using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SMMUpdateJobAssignController : Controller
    {
        EntitiesRegistration db = new EntitiesRegistration();

        // GET: Admin/SMMUpdateJobAssign
        public ActionResult Index()
        {
            SMMUpdateJobAssignSearchModel model = new SMMUpdateJobAssignSearchModel();
            return View();
        }

        public ActionResult Search(SMMUpdateJobAssignSearchModel model)
        {
            SignboardSearchService rs = new SignboardSearchService();
            rs.SearchUJA(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(SMMUpdateJobAssignSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.Excel(model) });
        }

        public ActionResult Form(string id, string Type, string Task, string recordUser, string refNo, string wftuUuid)
            
        {
            SignboardSearchService rs = new SignboardSearchService();
            SMMUpdateJobAssignSearchModel model = rs.ViewUJA(id, Type, Task, recordUser, refNo, wftuUuid);
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(SMMUpdateJobAssignSearchModel model)
        {
             SignboardSearchService rs = new SignboardSearchService();
             rs.SaveForm(model);
             return View("Index");
        }
    }
}