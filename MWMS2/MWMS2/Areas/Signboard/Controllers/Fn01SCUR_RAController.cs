using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn01SCUR_RAController : Controller
    {
        // GET: Signboard/Fn01SCUR_DE
        public ActionResult Index()
        {
            return  View();
        }
        public ActionResult ReceiveForm(SCUR_Models model)
        {
            SignboardRAService ss = new SignboardRAService();
            
            return Content(JsonConvert.SerializeObject(ss.GenerateSubmissionNumber(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //return View("Index", model);
        }
        public ActionResult ReceiveCompleteForm(SCUR_Models model)
        {
            SignboardRAService ss = new SignboardRAService();
 
            return Content(JsonConvert.SerializeObject(ss.ReceiveCompleteForm(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //return View("Index", model);
        }
        
    }
}