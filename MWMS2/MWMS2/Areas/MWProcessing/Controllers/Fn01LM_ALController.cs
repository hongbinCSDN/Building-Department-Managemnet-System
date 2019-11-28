using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_ALController : ValidationController
    {

        private ProcessingLetterModuleBLService _BL;

        protected ProcessingLetterModuleBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingLetterModuleBLService());
            }
        }

        // GET: MWProcessing/Fn01LM_AL
        public ActionResult Index()
        {
            Fn01LM_ALSearchModel model = new Fn01LM_ALSearchModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetPOInfo(string id)
        {
            return Json(BL.GetPOInfo(id));
        }

        [HttpPost]
        public ActionResult GetALList(Fn01LM_ALSearchModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.GetALList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ExportALListExcel(Fn01LM_ALSearchModel model)
        {

            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportALListExcel(model) });
        }

        public ActionResult Detail(string DSN)
        {
            return View("Detail", BL.getAckLetterDetail(DSN));
        }

        public ActionResult AddAdvisoryLetter(Fn01LM_ALSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.AddAdvisoryLetter(model));
        }

        public ActionResult ValidateAlDSN(Fn01LM_ALSearchModel model)
        {
            return Json(BL.ValidateAlDSN(model.SearchDSN));
        }

        public ActionResult updateAckLetterList(Fn01LM_ALDisplayModel model)
        {
            BL.updateAckLetterList(model);

            return View("Detail", BL.getAckLetterDetail(model.DSN));
        }

        public ActionResult removeFromAlList(Fn01LM_ALDisplayModel model)
        {
            BL.removeFromAlList(model);

            return View("Index", new Fn01LM_ALSearchModel());
        }

        public ActionResult ackLetterDownload(Fn01LM_ALDisplayModel model)
        {
            model = BL.getAckLetterDetail(model.DSN);

            string tempPath = Server.MapPath(ProcessingConstant.ADVISORY_LETTER_TEMPLATE_PATH);
            if(ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(model.P_MW_ACK_LETTER.LANGUAGE))
            {
                tempPath += "CHI.docx";
            }
            else
            {
                tempPath += "ENG.docx";
            }
            //string tempPath = Server.MapPath(ProcessingConstant.ACKNOWLEDGEMENT_LETTER_TEMPLATE_PATH) + "MW01_CHI.docx";
            // C:\Users\user\source\repos\MWMS22\MWMS2\Template\ALTemplate\

            string counterName = "";
            switch (model.P_MW_ACK_LETTER.COUNTER)
            {
                case ProcessingConstant.GENERAL_SEARCH_COUNTER_KWUN_TONG_CODE:
                    counterName = ProcessingConstant.GENERAL_SEARCH_COUNTER_KWUN_TONG;
                    break;
                case ProcessingConstant.GENERAL_SEARCH_COUNTER_PIONEER_CENTRE_CODE:
                    counterName = ProcessingConstant.GENERAL_SEARCH_COUNTER_PIONEER_CENTRE;
                    break;
                case ProcessingConstant.GENERAL_SEARCH_COUNTER_E_SUBMISSION_CODE:
                    counterName = ProcessingConstant.GENERAL_SEARCH_COUNTER_E_SUBMISSION;
                    break;
                case ProcessingConstant.GENERAL_SEARCH_COUNTER_WKGO_CODE:
                    counterName = ProcessingConstant.GENERAL_SEARCH_COUNTER_WKGO;
                    break;
            }

            string fileName = model.P_MW_ACK_LETTER.FORM_NO + "_" + model.P_MW_ACK_LETTER.MW_NO + "_" + model.P_MW_ACK_LETTER.DSN + "_" + counterName + "_";

            if(ProcessingConstant.DOC_TYPE_PDF.Equals(model.P_MW_ACK_LETTER.FILE_TYPE)) // pdf
            {
                string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
                return File(BL.PrintPDFAL(model.P_MW_ACK_LETTER.LANGUAGE, tempPath, tmpPDFDir, model), "application/pdf", fileName + ".pdf");
            }
            else // word
            {
                XWPFDocument doc = BL.PrintWordAL(model.P_MW_ACK_LETTER.LANGUAGE, tempPath, model);
                if(doc == null)
                {
                    return null;
                }
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();

                return File(fileContent, "application/msword", fileName + ".docx");
            }

        }
    }
}