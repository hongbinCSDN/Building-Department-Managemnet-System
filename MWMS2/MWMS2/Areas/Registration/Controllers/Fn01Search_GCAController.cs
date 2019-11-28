
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Utility;
using System.Linq;
using MWMS2.Entity;
using System.Data.Entity;
using System.Collections.Generic;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_GCAController : Controller
    {
       
        public ActionResult Index(CompSearchModel model)
        {           
            return View(model);
        }
        public ActionResult Form(string id)
        {
            RegistrationCompAppService rs = new RegistrationCompAppService();
            CompDisplayModel model = rs.ViewComp(id, RegistrationConstant.REGISTRATION_TYPE_CGA);
            return View("FormGCA", model);
        }
        public ActionResult Search(CompSearchModel model)
        {

            RegistrationCompAppService rs = new RegistrationCompAppService();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Content(JsonConvert.SerializeObject(rs.SearchComp(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(CompSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCompAppService rs = new RegistrationCompAppService();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return Json(new { key = rs.ExportComp(model) });
        }
        
    }
}