using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_SearchController : Controller
    {
        private ProcessingLetterModuleBLService _BL;
        protected ProcessingLetterModuleBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingLetterModuleBLService());
            }
        }
        // GET: MWProcessing/Fn01LM_Search
        public ActionResult Index()
        {
            Fn01LM_SearchModel model = new Fn01LM_SearchModel();
            return View(model);
        }

        public ActionResult Search(Fn01LM_SearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchLetter(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult PrintPDF(string id)
        {
            string tempPath = Server.MapPath("~/Template/AckLetterTemplate/");

            //string fileName = ProcessingConstant.ACKNOWLEDGEMENT_PRINT_FILENAME + "_" + Guid.NewGuid() + ".pdf";
            string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
            return File(BL.PrintPDF(id, tempPath, tmpPDFDir), "application/pdf");
            //return File(BL.PrintPDF(id, tempPath, tmpPDFDir),"application/pdf", "fff.pdf");
        }

        public ActionResult Export(Fn01LM_SearchModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            //model.Rpp = -1;
            //BL.SearchLetter(model);
            return Json(new { key = BL.SearchExcel(model) });
        }
    }
}