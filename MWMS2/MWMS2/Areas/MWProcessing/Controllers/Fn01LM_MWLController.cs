using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_MWLController : Controller
    {
        private ProcessingLetterModuleBLService _BL;
        protected ProcessingLetterModuleBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingLetterModuleBLService());
            }
        }

        // GET: MWProcessing/Fn01LM_MWL
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Search(Fn01LM_MWListModel model)
        //{
        //    return Content(JsonConvert.SerializeObject(BL.SearchMWList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        [HttpGet]
        public ActionResult PrintPDF(string id)
        {
            string tempPath = Server.MapPath("~/Template/AckLetterTemplate/");

            //string fileName = ProcessingConstant.ACKNOWLEDGEMENT_PRINT_FILENAME + "_" + Guid.NewGuid() + ".pdf";
            string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
            return File(BL.PrintPDF(id, tempPath, tmpPDFDir), "application/pdf");
        }

        //public ActionResult TestTemplate()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult Search(Fn01LM_SearchModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchMWList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }
}