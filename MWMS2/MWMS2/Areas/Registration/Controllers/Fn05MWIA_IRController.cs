using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn05MWIA_IRController : Controller
    {
        // GET: Registration/Fn05MWIA_IR
        public ActionResult Index(InterviewResultSearchModel model)
        {
         
            return View("SearchForm_IR", model);
        }
       
        public ActionResult Search(InterviewResultSearchModel model)
        {
            string type = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            RegistrationCompIRService rs = new RegistrationCompIRService();
            rs.SearchIR(model, type);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Form(string id)
        {
            RegistrationCompIRService rs = new RegistrationCompIRService();
            return View("FormIndIR", rs.ViewIndIR(id));
        }

        public ActionResult Import(string importType)
        {
            InterviewResultDisplayModel model = new InterviewResultDisplayModel();
            model.ImportType = importType;
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, InterviewResultSearchModel model)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    Import(file, model);

                    // string path = Path.Combine(Server.MapPath("~/Images"),
                    //                           Path.GetFileName(file.FileName));
                    //file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View(model);
        }

        //[HttpPost]
        //public ActionResult Excel(InterviewResultSearchModel model)
        //{
        //    if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
        //    RegistrationCompIRService rs = new RegistrationCompIRService();

        //    return Json(new { key = rs.ExportIR(model, RegistrationConstant.REGISTRATION_TYPE_MWIA) });
        //}
        [HttpPost]
        public ActionResult Excel(InterviewResultSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            string type = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            RegistrationCompIRService rs = new RegistrationCompIRService();
            return Json(new { key = rs.ExportExcel(model, type) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(InterviewResultDisplayModel model)
        {
            if (ModelState.IsValid)
            {
                RegistrationCompIRService rs = new RegistrationCompIRService();
                rs.SaveComp(model);
                return RedirectToAction("/Index");
            }
            return View(model);
        }
    }
}