using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using MWMS2.Areas.CMN.Models;
using MWMS2.Services.AdminService.DAO;
using Newtonsoft.Json;

namespace MWMS2.Areas.CMN.Controllers
{
    public class CMN02Controller : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(CMN02Model model)
        {
            SYS_Quick_Search_Service ss = new SYS_Quick_Search_Service();
   
            ss.RegistrationQuickSearch(model);
            return View("RegistrationResult", model);
        }
        public ActionResult Detail(string REGISTRATION_NO)
        {
            SYS_Quick_Search_Service ss = new SYS_Quick_Search_Service();
          
            return View("RegistrationDetail", ss.ViewRegistrationDetail(REGISTRATION_NO));

        }
        public ActionResult PEMSearch(CMN02Model model)      
        {
            return View("ProcessingResult", model);
        }
        public ActionResult ProcessingSearch(CMN02Model model)
        {
            SYS_Quick_Search_Service ss = new SYS_Quick_Search_Service();
            
            return Content(JsonConvert.SerializeObject(ss.ProcessingQuickSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult ProcessingExcel(CMN02Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SYS_Quick_Search_Service ss = new SYS_Quick_Search_Service();            
            return Json(new { key = ss.ExportProcessing(model) });
        }


        [HttpPost]
        public ActionResult RegistrationAdvanceSearch()
        {

            return View("RegistrationAdvanceSearch");
        }
    }
}