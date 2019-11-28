using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class ComSubmissionController : Controller
    {
        private ComSubmissionBLService _BL;
        protected ComSubmissionBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ComSubmissionBLService());
            }
        }

        // GET: MWProcessing/ComSubmissionInfo
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SubmissionInfoPage(string refNo)
        {
            Fn03TSK_SSModel model = new Fn03TSK_SSModel()
            {
                P_MW_REFERENCE_NO = new P_MW_REFERENCE_NO()
                {
                    REFERENCE_NO = refNo
                }
            };
            return View(BL.GetSubmissionInfoByRecordID(model));
        }

        public ActionResult SearchFileRefer(Fn03TSK_SSModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchFileRefer(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public JsonResult CheckIsVeri(string id)
        {
            return Json(BL.CheckIsVeri(id));
        }

        public ActionResult SearchWI1Form(Fn03TSK_SSModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchWI1Form(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchModData(Fn03TSK_SSModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchModData(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


    }
}