using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MWMS2.Entity;
using MWMS2.Areas.Registration.Controllers;
using NPOI.XWPF.UserModel;
using System.IO;
using MWMS2.Constant;
using System.Text;
using MWMS2.Services.Signborad.SignboardServices;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    // Modification 
    public class Fn01LM_MODController : ValidationController
    {
        private ProcessingModificationBLService BlService;
        protected ProcessingModificationBLService BL
        {
            get { return BlService ?? (BlService = new ProcessingModificationBLService()); }
        }

        public ActionResult Index()
        {
            return View();
        }

        #region BA16 Maintenance

        public ActionResult ModMaintenance()
        {
            return View();
        }

        public ActionResult SearchMod(Fn01LM_SearchModModel model)
        {
            return BL.SearchMod(model);
        }

        public Fn01LM_AckPrintModel GetLetterAdress(Fn01LM_AckPrintModel model)
        {
            if (string.IsNullOrWhiteSpace(model.address))
            {
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                {
                    StringBuilder addressFirstComponent = new StringBuilder();
                    StringBuilder addressSecondComponent = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(model.unit))
                    {
                        addressFirstComponent.Append(model.unit + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.floor))
                    {
                        addressFirstComponent.Append(model.floor + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.building))
                    {
                        addressFirstComponent.Append(model.building + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(addressFirstComponent.ToString()))
                    {
                        addressFirstComponent.Append(", ");
                    }

                    if (!string.IsNullOrWhiteSpace(model.streetNo))
                    {
                        addressSecondComponent.Append(model.streetNo + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.street))
                    {
                        addressSecondComponent.Append(model.street);
                    }
                    model.address = addressFirstComponent.ToString() + addressSecondComponent.ToString();
                }
                else if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    StringBuilder fullAddress = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(model.street))
                    {
                        fullAddress.Append(model.street + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.streetNo))
                    {
                        fullAddress.Append(model.streetNo + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.building))
                    {
                        fullAddress.Append(model.building + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.floor))
                    {
                        fullAddress.Append(model.floor + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.unit))
                    {
                        fullAddress.Append(model.unit + " ");
                    }
                    model.address = fullAddress.ToString();
                }
            }
            return model;
        }

        public ActionResult GenerateCompletionLetter(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Content("");

            string fileName = "Completion Letter.docx";
            string tempPath = Server.MapPath(ProcessingConstant.MODIFICATION_LETTER_TEMPLATE_PATH) + "ReminderLetterForCompletion.docx";
            XWPFDocument doc = BL.PrintWord(id, tempPath);
            MemoryStream ms = new MemoryStream();
            doc.Write(ms);
            byte[] fileContent = ms.ToArray();
            return File(fileContent, "application/msword", fileName);
        }

        public ActionResult GenerateAnnualLetter(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Content("");
            
            string fileName = "Annual Letter.docx";
            string tempPath = Server.MapPath(ProcessingConstant.MODIFICATION_LETTER_TEMPLATE_PATH) + "ReminderLetterForAnnualReport .docx";
            XWPFDocument doc = BL.PrintWord(id, tempPath);
            MemoryStream ms = new MemoryStream();
            doc.Write(ms);
            byte[] fileContent = ms.ToArray();
            return File(fileContent, "application/msword", fileName);
        }

        [HttpPost]
        public ActionResult ExcelMod(Fn01LM_SearchModModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.ExcelMod(model);
            return Json(new { key = model.ExportCurrentData($"Modification_{DateTime.Now:yyyy-MM-dd}") });
            //return BL.ExcelMod(model);
        }


        #endregion

        #region Receive of BA16

        public ActionResult Form()
        {
            return View(BL.GetFormModel());
        }

        public ActionResult SearchModOfToday()
        {
            return BL.SearchModOfToday();
        }

        public ActionResult SearchIncomingDoc(Fn01LM_ModModel model)
        {
            return BL.SearchIncomingDoc(model);
        }

        public ActionResult SearchOutgoingDoc(Fn01LM_ModModel model)
        {
            return BL.SearchOutgoingDoc(model);
        }

        public ActionResult Create(FormCollection formCollection, Fn01LM_ModModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.Create(formCollection, model);
        }

        public ActionResult EditForm(string uuid)
        {
            return View(BL.GetModification(uuid));
        }

        public ActionResult Update(FormCollection formCollection, Fn01LM_ModModel model)
        {
            return BL.Update(formCollection, model);
        }


        // Begin Add by Chester

        public ActionResult EditFormFromTDL(string uuid)
        {
            //Fn01LM_ModModel model = BL.GetModification(uuid);
            return View(BL.GetModification(uuid));
        }

        public ActionResult SaveBD106(FormCollection formCollection, Fn01LM_ModModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ServiceResult result = BL.SaveBD106(formCollection, model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetMWNo(string id)
        {

            return Json(BL.GetMWNo(id));
        }

        [HttpPost]
        public ActionResult ExcelBA16(DisplayGrid model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            model = BL.ExcelBA16();
            Dictionary<string, string> col1 = model.CreateExcelColumn("Ref No.", "REFERENCE_NO");
            Dictionary<string, string> col2 = model.CreateExcelColumn("DSN No.", "DSN");
            Dictionary<string, string> col3 = model.CreateExcelColumn("Received Date", "RECEIVED_DATE");
            Dictionary<string, string> col4 = model.CreateExcelColumn("Handling staff (PO) ", "HANDING_STAFF");
            model.Columns = new Dictionary<string, string>[]{
                col1,col2,col3,col4
            };
            return Json(new { key = model.ExportCurrentData($"Modification_{DateTime.Now:yyyy-MM-dd}") });
        }
        // End Add by Chester
        #endregion

    }
}