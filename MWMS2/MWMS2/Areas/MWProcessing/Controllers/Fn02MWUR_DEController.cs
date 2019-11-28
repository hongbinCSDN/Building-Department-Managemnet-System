using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_DEController : ValidationController
    {
        private static volatile ProcessingDataEntryBLService _BL;
        private static readonly object locker = new object();
        private static ProcessingDataEntryBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new ProcessingDataEntryBLService(); return _BL; } }

        public ActionResult Index()
        {
            return View(BL.GetOutstanding(new Fn02MWUR_DeModel()));
        }
        public ActionResult Search(Fn02MWUR_DeModel model)
        {
            return BL.Search(model);
        }

        public ActionResult SearchDocument(Fn02MWUR_DeScanModel model)
        {
            return BL.SearchDocument(model);
        }

        public ActionResult Scanning(Fn02MWUR_DeScanModel model)
        {
            BL.GetScanModel(model);
            return View(model);
        }

        public ActionResult Excel(Fn02MWUR_DeModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportDeScan(model) });
        }

        public ActionResult CompleteScan(Fn02MWUR_DeScanModel model)
        {
            //return BL.UpdateDsnStatus(model);
            return BL.CompleteScan(model);
        }

        public ActionResult DeleteScanDoc(P_MW_SCANNED_DOCUMENT model)
        {
            return BL.DeleteScanDoc(model);
        }

        public ActionResult Form(Fn02MWUR_DeFormModel model)
        {
            BL.GetFormData(model);
            return View(model);
        }

        public ActionResult SaveAsDraft(Fn02MWUR_DeFormModel model)
        {
            return BL.SaveAsDraft(model);
        }

        // Data entry submit form
        public ActionResult SubmitForm(Fn02MWUR_DeFormModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.SubmitForm(model);
        }
    }
}