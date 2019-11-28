using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.SignboardServices;
using MWMS2.Utility;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn02TDL_TDLController : ValidationController
    {
        // GET: Signboard/Fn02TDL_TDL
        public ActionResult Index()
        {
           
            
            return View();
        }
        public ActionResult GetValidation(ValidationDisplayModel model)
        {



            return Content(JsonConvert.SerializeObject(model.GetValidation, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


        }
        public ActionResult IssueLetter(string uuid)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
          

            return View(ss.ViewIssueLetter(uuid));
        }
        public ActionResult goToValidationToDoTO(string uuid, string type)
        {
            SessionUtil.DraftObject = null;
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ValidationDisplayModel model = new ValidationDisplayModel();
            model.TaskType = type;
            model.currUser = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
            model.EditMode = SignboardConstant.VIEW_MODE;
            if (model.SaveMode == "submit")
            {
                return View("Index");
            }
            else
            {
                return View("Validation", ss.ViewValidation(uuid, model));
            }
      
        }
        public ActionResult getLetter(IssueLetterDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();

            return Content(JsonConvert.SerializeObject(ss.getLetter(model) , Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult popupSignboardRelation()
        {
            return View();
        }
        public ActionResult popupMWItem(ValidationDisplayModel model)
        {

            return View("popupMWItem",model);
        }
        public ActionResult popupPhotoLib(ValidationDisplayModel model)
        {
            return View(model);
        }
        public ActionResult SearchSignboard(SignboardAddressSearchModel model)
        {
            SignboardCommonDAOService ss = new SignboardCommonDAOService();
            model = ss.getSignboardByAddress(model);
           return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
    
        }
        public ActionResult SaveIssueLetter(IssueLetterDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.SaveIssueLetter(model);

            return RedirectToAction("IssueLetter",  new { uuid = model.B_SV_VALIDATION.UUID });

        }
        
        public ActionResult SubmitIssueLetter(IssueLetterDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.SubmitIssueLetter(model);

            return View("Index");

        }
        public ActionResult IssueLetterSearch(Fn02TDL_TDLDisplayModel model)
        {
            SignboardTDLDAOService ss = new SignboardTDLDAOService();
            return Content(JsonConvert.SerializeObject(ss.IssueLetterSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ValidationSearch(Fn02TDL_TDLDisplayModel model)
        {
            SignboardTDLDAOService ss = new SignboardTDLDAOService();
            return Content(JsonConvert.SerializeObject(ss.ValidationSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


        }
        public ActionResult AuditSearch(Fn02TDL_TDLDisplayModel model)
        {
            SignboardTDLDAOService ss = new SignboardTDLDAOService();
            return Content(JsonConvert.SerializeObject(ss.AuditSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


        }


        public ActionResult SubmissionsSearch(Fn03TSK_TdlSearchModel model)
        {
            ProcessingTdlBLService BL = new ProcessingTdlBLService();
            model.HandlingUnit = ProcessingConstant.HANDLING_UNIT_SMM;
            BL.SearchSubmissions(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }



        public ActionResult DeleteDSN(ValidationDisplayModel model)
        {
            SignboardSDDaoService ss = new SignboardSDDaoService();
            ss.DeleteScannedDocument(model.TargetDSNUUID);
            return  RedirectToAction("goToValidationToDoTO",new { uuid =model.TargetValidationUUID });
        }
        public ActionResult postSubmittedDocs(ValidationDisplayModel model)
        {
            SignboardSDDaoService ss = new SignboardSDDaoService();
            ServiceResult result = ss.postSubmittedDocs(model.SubmittedDocListFolderType);
            result.Data = new { uuid = model.TargetValidationUUID,taskType = model.TaskType };
            return Json(result);
            //return RedirectToAction("goToValidationToDoTO", new { uuid = model.TargetValidationUUID });

        }
        public ActionResult SaveTOValidation(ValidationDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);

            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            //ss.post(model);
            if (model.SaveMode == ApplicationConstant.SUBMIT_MODE || model.SaveMode == ApplicationConstant.PASS_MODE)
            {
                //if (model.TaskType == "PO")
                //{
                //    model.TaskType = SignboardConstant.WF_MAP_VALIDATION_PO;
                //}
                //else if(model.TaskType =="SPO")
                //{

                //    model.TaskType = SignboardConstant.WF_MAP_VALIDATION_SPO;
                //}
                ss.post(model);
                return View("Index");
            }
            else
            {
                return Content(JsonConvert.SerializeObject(ss.post(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }

            //return RedirectToAction("goToValidationToDoTO", new { uuid = model.TargetValidationUUID });

            //  return goToValidationToDoTO(model.TargetValidationUUID,"");
        }
        public ActionResult Rollback(ValidationDisplayModel model)
        {
            model.SaveMode = ApplicationConstant.ROLLBACK_MODE;
            //model.TaskType = SignboardConstant.WF_MAP_VALIDATION_PO;

            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.post(model);


            return View("Index");
        }
        public ActionResult RollbackSPO(ValidationDisplayModel model)
        {
            model.SaveMode = ApplicationConstant.ROLLBACK_MODE;
            model.TaskType = SignboardConstant.WF_MAP_VALIDATION_SPO;

            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.post(model);


            return View("Index");
        }


        public void AjaxAddMWItem(string mwitem, string desc , string rev)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.AjaxAddMWItem(mwitem, desc,rev);
           // return View("Validation", model);
        }
        public void AjaxSaveMWItem(string mwitem, string desc, string rev,string uuid)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.AjaxSaveMWItem(mwitem, desc, rev,uuid);
            // return View("Validation", model);
        }


        public void AjaxAddPhotoLib( string desc, string url)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.AjaxAddPhotoLib(desc, url);
            // return View("Validation", model);
        }
        public void AjaxSavePhotoLib(string desc, string url, string uuid)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.AjaxSavePhotoLib( desc, url, uuid);
            // return View("Validation", model);
        }
        public ActionResult MWItemSearch(ValidationDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            return Content(JsonConvert.SerializeObject(ss.MWItemSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult PhotoLibSearch(string uuid)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            return Content(JsonConvert.SerializeObject(ss.PhotoLibSearch(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult popupComment(string uuid ,string EditMode,string type)
        {
            // if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right

            return View("Comment", cs.list(SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION, uuid, EditMode));
        }


        public ActionResult Load(CommentDisplayModel model)
        {
            // if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right

            return View("Comment", cs.load(model));
        }
        [HttpPost]
        public ActionResult Post(CommentDisplayModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardCommentService cs = new SignboardCommentService();
            // check security right
            cs.post(model);
            if (model.EditMode.Equals("add"))
            {
                model.JavascriptToRun = "closeWindow()";
                return View("Comment", model); // randomly return sth
                //return RedirectToAction("Index", "Fn02TDL_TDL");
            }
            else
            {
                return View("Comment", cs.list(model.RecordType, model.RecordId, "add"));
            }


        }


        public void deleteSignboardRelation(string uuid)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            ss.AjaxdeleteSignboardRelation(uuid);

        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult goToAuditDetail(string uuid)
        {
            return RedirectToAction("Detail", "Fn03Search_AA", new { uuid = uuid });
        }

        public ActionResult popupAddress(string uuid) // B_SV_ADDRESS: UUID
        {
            // if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();

            return View("popupAddress", ss.loadAddress(uuid));
        }

        [HttpPost]
        public ActionResult ValidationExportMailMerge(string uuid) // B_SV_VALIDATION: UUID
        {
            //if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            //SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();
            //return ss.ExportAuditData(uuid);
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            return ss.exportValidationData(uuid);            
        }

        [HttpPost]
        public ActionResult ValidationExportExcel(string uuid)
        {
            //if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            //SignboardAS_AuditApplicationService ss = new SignboardAS_AuditApplicationService();

            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            return ss.exportValidationDataExcel(uuid);
        }

        public ActionResult checkFileExist(IssueLetterDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            LetterTemplateService lts = new LetterTemplateService();
            B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(model.B_SV_VALIDATION.UUID, model.SelectedLetter);
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

        [HttpPost]
        public ActionResult generateIssueLetter(IssueLetterDisplayModel model)
        {
            try
            {
                // save
                SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
                ss.SaveIssueLetter(model);

                // generate letter
                B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(model.B_SV_VALIDATION.UUID, model.SelectedLetter);
                XWPFDocument doc = ss.PrintWord(model.B_SV_VALIDATION.B_SV_RECORD.UUID, model.B_SV_VALIDATION.B_SV_RECORD.LANGUAGE_CODE, selectedLetter);
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                string fileName = selectedLetter.FILE_NAME;
                var letter = File(fileContent, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                if (letter != null)
                {
                    return letter;
                }
                else
                {
                    return Content("File not found.");
                }
            }
            catch(Exception ex)
            {
                return Content("File not found.");
            }
        }

        public ActionResult checkFileExistValidation(ValidationDisplayModel model)
        {
            SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
            LetterTemplateService lts = new LetterTemplateService();
            B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(model.B_SV_VALIDATION.UUID, model.letterUuid);
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

        [HttpPost]
        public ActionResult mergeLetter(ValidationDisplayModel model)
        {
            try
            {
                SignboardTDLDetailDAOService ss = new SignboardTDLDetailDAOService();
                B_S_LETTER_TEMPLATE selectedLetter = ss.getLetterTemplate(model.B_SV_VALIDATION.UUID, model.letterUuid);

                XWPFDocument doc = ss.PrintWord(model.B_SV_VALIDATION.SV_RECORD_ID, model.B_SV_VALIDATION.B_SV_RECORD.LANGUAGE_CODE, selectedLetter);
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                byte[] fileContent = ms.ToArray();
                string fileName = selectedLetter.FILE_NAME;
                var letter = File(fileContent, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                if (letter != null)
                {
                    return letter;
                }
                else
                {
                    return Content("File not found."); //Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Data = "File not found." });
                }
            }
            catch (Exception ex)
            {
                return Content("File not found.");
            }
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