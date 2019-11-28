using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_AckController : ValidationController
    {

        private ProcessingLetterModuleBLService _BL;
        protected ProcessingLetterModuleBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingLetterModuleBLService());
            }
        }

        // GET: MWProcessing/Fn01LM_Ack
        public ActionResult Index()
        {
            Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
            model.P_MW_ACK_LETTER = new Entity.P_MW_ACK_LETTER();
            model.P_MW_ACK_LETTER.RECEIVED_DATE = DateTime.Now;
            model.P_MW_ACK_LETTER.LETTER_DATE = DateTime.Now;
            return View(model);
        }

        public ActionResult RedirectFromEfss(string EFSS_ID, string FORM_CODE, string DSN, string MW_NO)
        {
            Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
            ProcessingEformService eform = new ProcessingEformService();
            model = eform.createAckModel(EFSS_ID, FORM_CODE, DSN, MW_NO);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Edit(string id)
        {
            Fn01LM_AckSearchModel model = BL.GetAckLetterEditInfo(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update([Bind(Exclude = "")] Fn01LM_AckSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ServiceResult serviceResult = BL.UpdateAckLetter(model);
            return Json(serviceResult);
        }

        [HttpPost]
        public ActionResult CheckAddress([Bind(Exclude = "")] Fn01LM_AckSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ServiceResult serviceResult = BL.CheckAddress(model);
            return Json(serviceResult);
        }

        [HttpPost]
        public ActionResult Save([Bind(Exclude = "")] Fn01LM_AckSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);

            model.P_MW_ACK_LETTER.ITEM_DISPLAY = model.P_MW_ACK_LETTER.MW_ITEM;
            ServiceResult serviceResult = BL.SaveAckLetter(model);

            return Json(serviceResult);

        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            ServiceResult serviceResult = BL.DeleteAckLetter(id);
            return RedirectToAction("/Index");
        }

        public ActionResult SearchDSN(Fn01LM_AckSearchModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchDSN(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchReceivedDate(Fn01LM_AckSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchReceivedDate(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult GetMWNo(string id,string type = "DSN")
        {
            ServiceResult serviceResult = BL.GetMWNo(id,type);
            //ServiceResult serviceResult = new ServiceResult()
            //{
            //    Result = ServiceResult.RESULT_SUCCESS
            //};
            JsonResult json = Json(serviceResult);
            return json;
        }

        public ActionResult GetFileRefNo(string mwno)
        {

            return Json(BL.GetFileRefNo(mwno));
        }

        [HttpPost]
        public ActionResult GetACKLetter(string id)
        {
            IsoDateTimeConverter convert = new IsoDateTimeConverter();
            string ret = JsonConvert.SerializeObject(BL.GetACKLetter(id), Formatting.None, convert);
            return Content(ret, "application/json");
            //return new JsonResult()
            //{
            //    //Data = BL.GetACKLetter(id)
            //    Data = ret
            //};
        }

        [HttpPost]
        public ActionResult Print(string id)
        {
            string tempPath = Server.MapPath(ProcessingConstant.ACKNOWLEDGEMENT_LETTER_TEMPLATE_PATH);
            //string tempPath = Server.MapPath("~/Template/MW01(VS)_chi (draft r.4)_Temp.docx");
            Fn01LM_AckSearchModel model = BL.GetAckLetterEditInfo(id);

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
                return File(BL.PrintPDF(id, tempPath, tmpPDFDir), "application/pdf", fileName);
            }
            else
            {
                string fileName = model.P_MW_ACK_LETTER.FORM_NO + "_" + model.P_MW_ACK_LETTER.MW_NO + "_" + counterName + "_" + ".docx";
                XWPFDocument doc = BL.PrintWord(id, tempPath);
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                return File(fileContent, "application/msword", fileName);
            }

        }

        [HttpPost]
        public ActionResult Excel(Fn01LM_AckSearchModel model)
        {
            //if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(BL.Excel(model));
        }

        public ActionResult GetEmailByCerNo(string CerificationNo, string dsn, string type)
        {
            return BL.GetEmailByCerNo(CerificationNo, dsn, type);
        }

        public ActionResult AddEmailSender(string uuid, string dsn, string email, string type)
        {
            Fn01LM_AckPrintModel model = new Fn01LM_AckPrintModel();
            //string fileName = model.P_MW_ACK_LETTER.FORM_NO + "_" + model.P_MW_ACK_LETTER.MW_NO + "_" + counterName + "_" + ".pdf";
            string tempPath = Server.MapPath(ProcessingConstant.ACKNOWLEDGEMENT_LETTER_TEMPLATE_PATH);
            string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
            return Json(BL.AddEmailSender(uuid, dsn, email, type, tempPath, tmpPDFDir));
        }

    }
}