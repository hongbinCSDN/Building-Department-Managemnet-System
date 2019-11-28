using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.AdminService.BL;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class PEM11Controller : Controller
    {

        private PEM11BLService _BL;
        protected PEM11BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM11BLService());
            }
        }

        // GET: PEM11
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PEM1101()
        {
            return View();
        }
        public ActionResult PEM1102()
        {
            return View();
        }
        public ActionResult PEM1103()
        {
            return View();
        }
        public ActionResult PEM1104()
        {
            return View();
        }
        public ActionResult PEM1105()
        {
            return View();
        }
        public ActionResult PEM1106()
        {
            return View();
        }
        public ActionResult PEM1107()
        {
            return View();
        }
        public ActionResult PEM1108()
        {
            return View();
        }

        #region Edit DSN Nature
        public ActionResult PEM1109()
        {
            DSNSearchModel model = new DSNSearchModel();
            return View(model);
        }

        public ActionResult SearchDSN(DSNSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDSN(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //SignboardSearchService rs = new SignboardSearchService();
            //return Content(JsonConvert.SerializeObject(rs.SearchDSN(DSN), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Form_DSN(string UUID,string Message)
        {
            if (!string.IsNullOrEmpty(Message))
                ViewBag.Message = Message;
            return  View(BL.DSNDetail(UUID));
        }
            

        //public ActionResult Form_DSN(DSNSearchModel model)
        //{
        //    SignboardSearchService rs = new SignboardSearchService();
        //    DSNSearchModel rss = rs.SearchDSN(model.DSN);


        //    if (rss != null && rss.P_MW_DSN != null) return View(rs.SearchDSN(model.DSN));
        //    else { return View("PEM1109", new DSNSearchModel() {msg="no record found" } ); } 
        //}

        [HttpPost]
        public ActionResult Save_DSN(DSNSearchModel model)
        {
            return Json(BL.SaveDSN(model));

            //if(BL.SaveDSN(model).Result == ServiceResult.RESULT_SUCCESS)
            //{
            //    return RedirectToAction("Form_DSN", new { UUID = model.P_MW_DSN.UUID, Message = "Save Successfully" });
            //}
            //else
            //{
            //    return RedirectToAction("Form_DSN", new { UUID = model.P_MW_DSN.UUID, Message = "Save Failed" });
            //}

            //if (ModelState.IsValid)
            //{
            //    SignboardSearchService rs = new SignboardSearchService();
            //    rs.SaveDSN(model);
            //    return RedirectToAction("/PEM1109");
            //}

            //return null;
        }
        #endregion

        #region PEM1110 Import Records

        public ActionResult PEM1110(string message)
        {
            if (!string.IsNullOrEmpty(message))
                ViewBag.Message = message;
            return View(new PEM1110ImportRecordsModel() { fileType = "Excel" } );
        }

        public ActionResult SearchImportRecords(PEM1110ImportRecordsModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchImportRecords(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ExportPEM1110Records(PEM1110ImportRecordsModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportPEM1110Records(model) });
        }

        [HttpPost]
        public ActionResult ImportRecordsExcel(HttpPostedFileBase file, PEM1110ImportRecordsModel model)
        {
            string message = null;
            model.p_MW_IMPORTED_TASK =  new P_MW_IMPORTED_TASK();
            model.p_MW_IMPORTED_TASK.FILE_NAME = file.FileName;
            using(var package = new ExcelPackage(file.InputStream))
            {
                model.ExcelPackage = package;
                var result = BL.ImportRecordsExcel(model);
                if (result.Result == ServiceResult.RESULT_SUCCESS)
                {
                    message = "File uploaded successfully";
                }
                else
                {
                    message = "Error: " + result.Message;
                }
                return RedirectToAction("PEM1110",new { message = message });
            }
            
        }

        [HttpPost]
        public ActionResult DeleteMWRecords(PEM1110ImportRecordsModel model)
        {
            return Json(BL.DeleteMWRecords(model));
        }

        [HttpPost]
        public ActionResult UpdateFileStatus(PEM1110ImportRecordsModel model)
        {
            return Json(BL.UpdateFileStatus(model));
        }

        public ActionResult ExportItemRecords(PEM1110ImportRecordsModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportItemRecords(model) });
        }


        #endregion

        #region Update PDF History
        public ActionResult PEM1111()
        {
            return View(new PEM1111UploadPDFHistoryModel());
        }

        public ActionResult SearchUploadPDFHistory(PEM1111UploadPDFHistoryModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchUploadPDFHistory(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ExportDetailsPDF(PEM1111UploadPDFHistoryModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportDetailsPDF(model) });
        }

        #endregion

        #region Pending Transfer

        public ActionResult PEM1112()
        {
            return View();
        }

        public ActionResult SearchPendingTransfer(PEM1112PendingTransferModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchPendingTransfer(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SavePendingTransfer(PEM1112PendingTransferModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SavePendingTransfer(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        #endregion

        #region
        public ActionResult PEM1113()
        {
            return View();
        }

        #endregion Update Minor Work Record

        #region MW Number Mapping

        public ActionResult PEM1114()
        {
            return View();
        }

        public ActionResult PEM1114Detail(PEM1114MWNumberMappingModel model)
        {
            return View(model);
        }

        public ActionResult SearchMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchMWNumberMapping(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchMWNumberUserName(PEM1114MWNumberMappingModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchMWNumberUserName(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult SaveMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            return Json(BL.SaveMWNumberMapping(model));
        }

        #endregion

        #region Update Submission Record
        public ActionResult PEM1115()
        {
            return View();
        }

        public ActionResult SearchSubmissionRecord(string refNo)
        {
            return Json(BL.SearchSubmissionRecord(refNo));
        }


        public ActionResult PEM1115Detail(string uuid)
        {
            return View(BL.GetSubmissionRecordDetail(uuid));
        }

        public ActionResult GetIssuedCorrespondence(PEM1103UpdateSubmissionRecordModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetIssuedCorrespondence(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult GetSubmissionOfLetterModule(PEM1103UpdateSubmissionRecordModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetSubmissionOfLetterModule(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult AddMinorWorkItem(PEM1103UpdateSubmissionRecordModel model)
        {
            var submissionModel = BL.GetSubmissionRecordDetail(model.UUID);
            //submissionModel.MwRecordMinorWorkItemList.Add(new P_MW_RECORD_ITEM
            //{
            //    UUID = Guid.NewGuid().ToString("N")
            //    ,
            //    MW_ITEM_CODE = model.ItemNo
            //    ,
            //    LOCATION_DESCRIPTION = model.Description
            //    ,
            //    RELEVANT_REFERENCE = model.ReferenceNo
            //});
            model.MwRecordMinorWorkItem.UUID = Guid.NewGuid().ToString("N");
            model.MwRecordMinorWorkItemList.Add(model.MwRecordMinorWorkItem);
            submissionModel.MwRecordMinorWorkItemList = model.MwRecordMinorWorkItemList;
            return View("PEM1115Detail", submissionModel);
        }

        public ActionResult DeleteMinorWorkItem(PEM1103UpdateSubmissionRecordModel model,string itemUUID)
        {

            var submissionModel = BL.GetSubmissionRecordDetail(model.UUID);
            model.MwRecordMinorWorkItemList.Remove(model.MwRecordMinorWorkItemList.Where(m => m.UUID == itemUUID).FirstOrDefault());
            submissionModel.MwRecordMinorWorkItemList = model.MwRecordMinorWorkItemList;
            return View("PEM1115Detail", submissionModel); ;
        }

        #endregion

        #region Update Job Assignment
        public ActionResult PEM1116()
        {
            return View();
        }
        public ActionResult PEM1116Search(PEMUpdateJobAssignmentSearchModel model)
        {
            model = BL.SearchUJA(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult PEM1116Form(string id, string Type, string Task, string recordUser, string refNo, string wftuUuid)
        {
            PEMUpdateJobAssignmentSearchModel model = BL.ViewUJA(id, Type, Task, recordUser, refNo, wftuUuid);
            return View(model);
        }

        [HttpPost]
        public ActionResult PEM1116Save(PEMUpdateJobAssignmentSearchModel model)
        {
            BL.SaveForm(model);
            return View("PEM1116");
        }
        #endregion
    }
}