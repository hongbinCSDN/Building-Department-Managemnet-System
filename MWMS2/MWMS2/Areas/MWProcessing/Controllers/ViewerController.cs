using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.Services;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class ViewerController : Controller
    {
        private ProcessingLetterModuleBLService _letterModuleService;
        protected ProcessingLetterModuleBLService letterModuleService
        {
            get
            {
                return _letterModuleService ?? (_letterModuleService = new ProcessingLetterModuleBLService());
            }
        }

        // GET: MWProcessing/MWViewer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AcknowledgementDocument()
        {
            ViewBag.id = Request["id"];

            if (ViewBag.id != null)
            {
                return GetLetterModuleDocument(ViewBag.id);
            }

            return View();
        }

        public FileResult GetLetterModuleDocument(string id)
        {
            string tempPath = Server.MapPath(ProcessingConstant.ACKNOWLEDGEMENT_LETTER_TEMPLATE_PATH);
            //string tempPath = Server.MapPath("~/Template/MW01(VS)_chi (draft r.4)_Temp.docx");
            Fn01LM_AckSearchModel model = letterModuleService.GetAckLetterEditInfo(id);

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

            if (model.P_MW_ACK_LETTER.FILE_TYPE == ProcessingConstant.DOC_TYPE_PDF)
            {
                string fileName = model.P_MW_ACK_LETTER.FORM_NO + "_" + model.P_MW_ACK_LETTER.MW_NO + "_" + counterName + "_" + ".pdf";
                string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
                return File(letterModuleService.PrintPDF(id, tempPath, tmpPDFDir), "application/pdf");
            }
            else
            {
                string fileName = model.P_MW_ACK_LETTER.FORM_NO + "_" + model.P_MW_ACK_LETTER.MW_NO + "_" + counterName + "_" + ".docx";
                XWPFDocument doc = letterModuleService.PrintWord(id, tempPath);
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                return File(fileContent, "application/msword", fileName);
            }

        }
    }
}