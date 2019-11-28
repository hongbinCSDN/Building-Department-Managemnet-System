using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn01SCUR_LMController : Controller
    {

        private static volatile SignboardCommonDAOService _CommonBL;
        private static volatile SignboardLMService _BL;
        private static readonly object locker = new object();
        private static SignboardCommonDAOService CommonBL { get { if (_CommonBL == null) lock (locker) if (_CommonBL == null) _CommonBL = new SignboardCommonDAOService(); return _CommonBL; } }
        private static SignboardLMService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new SignboardLMService(); return _BL; } }


        public ActionResult SearchLocationSignboard()
        {
            //SignboardCommonDAOService ss = new SignboardCommonDAOService();
            DataEntrySearchModel model = new DataEntrySearchModel();
            model = CommonBL.getSignboardLocation(model);
            return View(model);
        }

        public ActionResult Index(DataEntrySearchModel model)
        {
            model.RegType = "Letter Module";
            return View("DataEntry", model);
        }

        public ActionResult RedirectFromRA(string submissionNo)
        {
            DataEntrySearchModel model = new DataEntrySearchModel();
            model.RegType = "Letter Module";
            model.searchFileRefNo = submissionNo;
            return View("DataEntry", model);
        }

        public ActionResult LoadDataEntry(DataEntrySearchModel model)
        {
            //SignboardLMService rs = new SignboardLMService();
            return Content(JsonConvert.SerializeObject(BL.SearchDataEntryList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult EditDataEntry(string id, string formCode)
        {
            DataEntryDisplayModel model = BL.editDataEntry(id, formCode);
            model.RegType = "Letter Module";
            model.B_SV_VALIDATION = new Entity.B_SV_VALIDATION();
            model.B_SV_VALIDATION.B_SV_RECORD = model.SvRecord;
            return View("DataEntryDetail", model);
        }
        public ActionResult BatchAssignment(DataEntrySearchModel model)
        {
            BL.PopulateBatchAssignment(model, SignboardConstant.NuMBER_TYPE_SV_BATCH);
            return View("DataEntry", model);
        }
        public ActionResult removeBatch(string UUID)
        {
            DataEntrySearchModel model = new DataEntrySearchModel();
            model.svSubmission = new Entity.B_SV_SUBMISSION() { UUID = UUID };
            BL.removeBatch(model);
            return View("DataEntry", model);
        }
        public ActionResult Excel(DataEntrySearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            //SignboardLMService rs = new SignboardLMService();
            return Json(new { key = BL.ExportLM(model) });
        }
        public ActionResult SaveDataEntry(DataEntryDisplayModel model)
        {
            BL.SaveDataEntry(model);
            return EditDataEntry(model.SvSubmission.UUID, model.FormCode);
        }

        public ActionResult checkFileExist(DataEntryDisplayModel model)
        {
            SignboardLetterModuleService rs2 = new SignboardLetterModuleService();
            LetterTemplateService lts = new LetterTemplateService();
            B_S_LETTER_TEMPLATE selectedLetter = rs2.getLetterModuleLetterTemplate(model);

            string path = lts.getFilePathByLetterType(selectedLetter.LETTER_TYPE);

            var isExist = lts.checkFileExist(path + selectedLetter.FILE_NAME);
            if (isExist)
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "File not found." } });
            }
        }

        public ActionResult GenerateLetter(DataEntryDisplayModel model)
        {
            try
            {
                BL.SaveDataEntry(model);
                model.ExportLetterFlag = "y";
                SignboardLetterModuleService rs2 = new SignboardLetterModuleService();
                B_S_LETTER_TEMPLATE selectedLetter = rs2.getLetterModuleLetterTemplate(model);

                XWPFDocument doc = rs2.PrintWord(model, model.RegType, selectedLetter);
                MemoryStream ms = new MemoryStream();
                
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                BL.SaveToScannedDocument(model, fileContent);
               
                return File(fileContent, "application/msword", selectedLetter.FILE_NAME);
            }
            catch(Exception ex)
            {
                return Content("File not found.");
            }
        }
        public ActionResult copyFromBaseDataEntry(DataEntryDisplayModel model)
        {
            BL.copyFromBaseDataEntry(model);
            return EditDataEntry(model.SvSubmission.UUID, model.FormCode);
        }

        public ActionResult SearchVDItem(DataEntryDisplayModel model)
        {
            return Content(JsonConvert.SerializeObject((BL.SearchVDItem(model)), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchMWItem(DataEntryDisplayModel model)
        {
            return Content(JsonConvert.SerializeObject((BL.SearchMWItem(model)), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }




    }
}