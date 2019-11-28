using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
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
    public class Fn02MWUR_GEController : ValidationController
    {
        private ProcessingMWUGEBLService _BL;
        protected ProcessingMWUGEBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingMWUGEBLService());
            }
        }

        // GET: MWProcessing/Fn02MWRU_GE
        public ActionResult Index()
        {
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            BL.Search(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(Fn02MWUR_GEModel model)
        {
            //DisplayGrid dlr = demoSearch();

            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn02MWUR_GEModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.Search(model);
            return Json(new { key = model.ExportCurrentData("Submission_Search_" + DateTime.Now.ToString("dd/MM/yyyy")) });
        }

        [HttpPost]
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Document S/N", "Assignment Date", "Time", "Ref. No.", "Type", "Status" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }

        [HttpPost]
        public ActionResult Enquiry(string dsn)
        {
            return View(BL.Enquiry(dsn));
        }

        [HttpPost]
        public ActionResult GetScannedDoc(Fn02MWUR_GEModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.GetScannedDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Submit1(Fn02MWUR_GEModel model)
        {
            return Json(BL.Submit1(model));
        }

        [HttpPost]
        public ActionResult CheckedBeforeSubmit2(Fn02MWUR_GEModel model)
        {
            return Json(BL.CheckedBeforeSubmit2(model));
        }

        [HttpPost]
        public ActionResult Submit2(string refNo, string dsn)
        {
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            model.DSN = dsn;
            model.Enquiry = new Fn02MWUR_GEEnquiryModel();
            model.Enquiry.REFERENCE_NO = refNo;
            model = BL.Submit2(model);
            if (model.P_MW_GENERAL_RECORD == null)
            {
                return RedirectToAction("Enquiry", new { dsn });
            }
            if (model.P_MW_GENERAL_RECORD.FORM_STATUS == ProcessingConstant.GENERAL_RECORD_COMPLETED)
            {
                return RedirectToAction("EnquiryForm", new { id = model.P_MW_GENERAL_RECORD.UUID });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SubmitEntry(Fn02MWUR_GEModel model)
        {
            return Json(BL.SubmitEntry(model));
        }

        [HttpPost]
        public ActionResult SaveEntry(Fn02MWUR_GEModel model)
        {
            return Json(BL.SaveEntry(model));
        }
        public ActionResult EnquiryForm(string id)
        {
            return View(BL.EnquiryForm(id));
        }
        public ActionResult EnquiryFormReadOnly(string id)
        {
            Fn02MWUR_GEModel model = BL.EnquiryForm(id);
            model.IsReadOnly = true;
            return View(model);
        }
        public ActionResult EnquiryFormChecklist(string id)
        {
            Fn02MWUR_GEModel model = BL.EnquiryForm(id);
            BL.GetCheklist(model);
            return View(model);
        }
        public ActionResult EnquiryFormChecklistReadOnly(string id)
        {
            Fn02MWUR_GEModel model = BL.EnquiryForm(id);
            BL.GetCheklist(model);
            model.IsReadOnly = true;
            return View(model);
        }

        // ToDoList - Save Enquiry Form
        [HttpPost]
        public ActionResult SaveEnquiryForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveEnquiryForm(model));
        }

        // ToDoList - Submit Enquiry Form
        [HttpPost]
        public ActionResult SubmitEnquiryForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveEnquiryForm(model));
            //return RedirectToAction("EnquiryFormChecklist");
            //return Json("");
        }

        [HttpPost]
        public ActionResult SaveCheckListForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveCheckListForm(model));
        }
        [HttpPost]
        public ActionResult SubmitCheckListForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SubmitCheckListForm(model));
        }

        public ActionResult ComplaintForm(string id)
        {
            return View(BL.EnquiryForm(id));
        }

        public ActionResult ComplaintFormChecklist(string id)
        {
            //Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            
            return View(BL.GetCheklist(BL.EnquiryForm(id)));
        }

        public ActionResult SaveComplaintForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveEnquiryForm(model));
        }

        public ActionResult SubmitComplaintForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveEnquiryForm(model));
        }

        public ActionResult SaveComplaintCheckListForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveComplaintCheckListForm(model));
        }

        public ActionResult SubmitComplaintCheckListForm(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SubmitComplaintCheckListForm(model));
        }

        public ActionResult ViewAndAddComment(string id)
        {
            return View(BL.ViewAndAddComment(id));
        }
        [HttpPost]
        public ActionResult EditComment(string id)
        {
            return Json(BL.EditComment(id));
        }
        [HttpPost]
        public ActionResult SaveOrEditComment(Fn02MWUR_GEModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.SaveOrEditComment(model));
        }

        [HttpPost]
        public ActionResult GeneralMWNumber(Fn02MWUR_GEModel model)
        {
            return Json(BL.GeneralMWNumber());
        }
    }
}