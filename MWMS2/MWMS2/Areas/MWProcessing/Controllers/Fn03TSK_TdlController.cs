using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
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
    public class Fn03TSK_TdlController : ValidationController
    {
        private ProcessingTdlBLService BLService;
        protected ProcessingTdlBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingTdlBLService()); }
        }

        private ProcessingTDL_VerificationBLService verificationBL;
        protected ProcessingTDL_VerificationBLService VerificationBL
        {
            get { return verificationBL ?? (verificationBL = new ProcessingTDL_VerificationBLService()); }
        }

        private ProcessingTDL_AcknowledgementBLService ackBL;
        protected ProcessingTDL_AcknowledgementBLService AckBL
        {
            get { return ackBL ?? (ackBL = new ProcessingTDL_AcknowledgementBLService()); }
        }

        // GET: MWProcessing/Fn03TASK_Tdl
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchMOD(Fn03TSK_TdlSearchModel model)
        {
            //if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);

            return Content(JsonConvert.SerializeObject(BL.SearchMOD(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ExcelMOD(Fn03TSK_TdlSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelMod(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult SearchDR(Fn03TSK_TdlSearchModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchDR(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult DRDetail(string uuid)
        {
            Fn03TSK_TdlSearchModel model = BL.GetDRDetail(uuid);
            return View(model);
        }

        public ActionResult DownloadDRDetail(string uuid, string fileType, string language)
        {
            string tempPath = Server.MapPath(ProcessingConstant.DIRECT_RETURN_LETTER_TEMPLATE_PATH) + "DirectReturnTemplate_" + (language == "EN" ? "ENG" : "CHI") + ".docx";
            if (fileType == "P")
            {
                string fileName = "Direct Return.pdf";
                string tmpPDFDir = Server.MapPath("~/Template/tmpPDF/");
                return File(BL.PrintDRPDF(uuid, tempPath, tmpPDFDir), "application/pdf", fileName);

            }
            else
            {
                string fileName = "Direct Return.docx";
                XWPFDocument doc = BL.PrintDRWord(uuid, tempPath);
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                return File(fileContent, "application/msword", fileName);
            }

        }

        [HttpPost]
        public ActionResult RemoveFromTDL(Fn03TSK_TdlSearchModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.RemoveFromTDL(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult UpdateDRDetail(Fn03TSK_TdlSearchModel model)
        {
            return Json(BL.UpdateDRDetail(model));
        }




        public ActionResult SearchComplaints(Fn03TSK_TdlSearchModel model)
        {
            BL.SearchComplaints(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExcelComplaints(Fn03TSK_TdlSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelComplaints(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult SearchEnquirys(Fn03TSK_TdlSearchModel model)
        {
            BL.SearchEnquirys(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExcelEnquirys(Fn03TSK_TdlSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelEnquirys(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        /*
        public ActionResult SearchAudits(Fn03TSK_TdlSearchModel model)
        {
            BL.SearchAudits(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExcelAudits(Fn03TSK_TdlSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelAudits(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }*/

        public ActionResult SearchSubmissions(Fn03TSK_TdlSearchModel model)
        {
            model.HandlingUnit = ProcessingConstant.HANDLING_UNIT_PEM;
            BL.SearchSubmissions(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ExcelSubmissions(Fn03TSK_TdlSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelSubmissions(model);
            return Content(JsonConvert.SerializeObject(new { key = model.ExportKey }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        #region Verification

        public ActionResult Verification(VerificaionFormModel model)
        {
            VerificationBL.GetFormData(model);
            return View(model);
        }

        public ActionResult SaveAndNext(VerificaionFormModel model)
        {
            return VerificationBL.SaveAndNext(model);
        }

        public ActionResult AddAddressInfo(VerificaionFormModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return VerificationBL.AddAddressInfo(model);
        }

        public ActionResult DeleteAddressInfo(VerificaionFormModel model)
        {
            return VerificationBL.DeleteAddressInfo(model);
        }

        public ActionResult AddPsacImage(VerificaionFormModel model)
        {
            return VerificationBL.AddPsacImage(model);
        }

        public ActionResult DeletePsacImage(VerificaionFormModel model)
        {
            return VerificationBL.DeletePsacImage(model);
        }

        public ActionResult SaveP_MW_SCANNED_DOCUMENTsIC(VerificaionFormModel model)
        {
            return VerificationBL.SaveP_MW_SCANNED_DOCUMENTsIC(model);
        }

        public ActionResult SaveP_MW_SCANNED_DOCUMENTsNIC(VerificaionFormModel model)
        {
            return VerificationBL.SaveP_MW_SCANNED_DOCUMENTsNIC(model);
        }

        public ActionResult UpdateSPO(VerificaionFormModel model)
        {
            return VerificationBL.UpdateSPO(model);
        }

        public ActionResult Summary(VerificaionFormModel model)
        {
            VerificationBL.GetFormData(model);
            return View("Verification", model);
        }

        public ActionResult SummarySubmit(VerificaionFormModel model)
        {
            return VerificationBL.SummarySubmit(model);
        }

        #endregion

        #region Acknowledgemnet

        public ActionResult Acknowledgement(AcknowledgementModel model)
        {
            AckBL.GetAcknowledgement(model);
            return View(model);
        }

        public ActionResult SaveAcknowledgement(AcknowledgementModel model)
        {
            return AckBL.SaveAcknowledgement(model);
        }

        //public ActionResult SubmitAcknowledgement_PO(VerificaionFormModel model)
        //{
        //    return Json(AckBL.SubmitAcknowledgement_PO(model));
        //    //return Content(JsonConvert.SerializeObject(AckBL.SubmitAcknowledgement_PO(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        public ActionResult SubmitAcknowledgement(AcknowledgementModel model)
        {
            return Json(AckBL.SubmitAcknowledgement(model));
            //return Content(JsonConvert.SerializeObject(AckBL.SubmitAcknowledgement(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        #endregion



    }
}