using MWMS2.Areas.Registration.Controllers;
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
    public class Fn01SCUR_DEController : ValidationController
    {
        private static volatile SignboardCommonDAOService _CommonBL;
        private static volatile SignboardLMService _BL;
        private static readonly object locker = new object();
        private static SignboardCommonDAOService CommonBL { get { if (_CommonBL == null) lock (locker) if (_CommonBL == null) _CommonBL = new SignboardCommonDAOService(); return _CommonBL; } }
        private static SignboardLMService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new SignboardLMService(); return _BL; } }

        public ActionResult SearchLocationSignboard()
        {
            //SignboardCommonDAOService ss = new SignboardCommonDAOService();
            //DataEntrySearchModel model = new DataEntrySearchModel();
            //model = ss.getSignboardLocation(model);
            //return View(model);
            DataEntrySearchModel model = new DataEntrySearchModel();
            model = CommonBL.getSignboardLocation(model);
            return View(model);
        }
        public ActionResult Index(DataEntrySearchModel model)
        {
            model.RegType = "Data Entry";
            return View("DataEntry", model);
        }
        public ActionResult LoadDataEntry(DataEntrySearchModel model)
        {
            //SignboardLMService rs = new SignboardLMService();
            //return Content(JsonConvert.SerializeObject(rs.SearchDataEntryList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            return Content(JsonConvert.SerializeObject(BL.SearchDataEntryList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(DataEntrySearchModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Excel(model) });
        }

        public ActionResult EditDataEntry(string id, string formCode)
        {
            DataEntryDisplayModel model = BL.editDataEntry(id, formCode);
            //SignboardLMService rs = new SignboardLMService();
            //DataEntryDisplayModel model =rs.editDataEntry(id, formCode);
            model.RegType = "Data Entry";
            model.B_SV_VALIDATION = new Entity.B_SV_VALIDATION();
            model.B_SV_VALIDATION.B_SV_RECORD = model.SvRecord;
            return View("DataEntryDetail", model);


        }
        public ActionResult BatchAssignment(DataEntrySearchModel model)
        {
            //SignboardLMService rs = new SignboardLMService();
            //rs.PopulateBatchAssignment(model, SignboardConstant.NuMBER_TYPE_SV_BATCH);
            BL.PopulateBatchAssignment(model, SignboardConstant.NuMBER_TYPE_SV_BATCH);
            return View("DataEntry", model);
        }
        public ActionResult SaveDataEntry(DataEntryDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);

            model.SaveMode = SignboardConstant.SAVE_MODE;
            ;
            // return  RedirectToAction("EditDataEntry",)
            return Content(JsonConvert.SerializeObject(BL.SaveDataEntry(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //   return EditDataEntry(model.SvSubmission.UUID, model.FormCode);

            //SignboardLMService rs = new SignboardLMService();
            //rs.SaveDataEntry(model);
            //return EditDataEntry(model.SvSubmission.UUID, model.FormCode);
            //rs.editDataEntry(model.SvSubmission.UUID, model.FormCode);
            //model.RegType = "Data Entry";
            //model.B_SV_VALIDATION = new Entity.B_SV_VALIDATION();
            //model.B_SV_VALIDATION.B_SV_RECORD = model.SvRecord;
            //return View("DataEntryDetail", model);
        }
        public ActionResult copyFromBaseDataEntry(DataEntryDisplayModel model)
        {
            //SignboardLMService rs = new SignboardLMService();
            //rs.copyFromBaseDataEntry(model);
            BL.copyFromBaseDataEntry(model);
            return EditDataEntry(model.SvSubmission.UUID, model.FormCode);
        }
        public ActionResult SubmitDataEntry(DataEntryDisplayModel model, DataEntrySearchModel SearchModel)
        {
            model.SaveMode = SignboardConstant.SUBMIT_MODE;
            SignboardLMService rs = new SignboardLMService();
            rs.SaveDataEntry(model);
            return View("Index", SearchModel);
        }

        public ActionResult checkFileExist(DataEntryDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            LetterTemplateService lts = new LetterTemplateService();
            B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(null, model.SelectedLetter);

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
                //string tempPath = Server.MapPath("~/Template/LetterModulTemplate/");
                //string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
                //string tempPath = ApplicationConstant.SMMFilePath + SignboardConstant.LETTER_TEMPLATE_FILE_PATH + ApplicationConstant.FileSeparator;
                SignboardLetterModuleService rs2 = new SignboardLetterModuleService();
                //string fileName = rs2.FormName(model) + ".docx";
                SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
                B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(null, model.SelectedLetter);

                XWPFDocument doc = rs2.PrintWord(model, model.RegType, selectedLetter);
                MemoryStream ms = new MemoryStream();
                if (doc != null)
                {
                    doc.Write(ms);
                }
                byte[] fileContent = ms.ToArray();
                BL.SaveToScannedDocument(model,fileContent);
                return File(fileContent, "application/msword", selectedLetter.FILE_NAME);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult getLetter(DataEntryDisplayModel model)
        {
            SignboardLetterModuleService ss = new SignboardLetterModuleService();
            return Content(JsonConvert.SerializeObject(ss.getLetter(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchVDItem(DataEntryDisplayModel model)
        {
            return Content(JsonConvert.SerializeObject((BL.SearchVDItem(model)), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchMWItem(DataEntryDisplayModel model)
        {
            return Content(JsonConvert.SerializeObject((BL.SearchMWItem(model)), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult GetSignImg(string uuid) // certNo
        {
            SignboardCommonDAOService dao = new SignboardCommonDAOService();

            var file = dao.getProfessionalImagebycertificationNo(uuid);

            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }
        }
    }
}