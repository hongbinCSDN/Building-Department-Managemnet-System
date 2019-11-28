using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_DRController : ValidationController
    {
        private ProcessingDirectReturnBLService BLService;
        protected ProcessingDirectReturnBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingDirectReturnBLService()); }
        }

        #region View

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(string id)
        {
            return View("drSubmission", BL.SearchDrDetail(id));
        }

        public ActionResult drSubmission()
        {
            return View("drSubmission", BL.GetSaveModel());
        }

        #endregion



        #region Search

        public ActionResult drMaintenance()
        {
            return View("drMaintenance", BL.GetSearchModel());
        }


        public ActionResult Search(Fn01LM_DRSearchModel model)
        {
            model.IsMaintenance = true;
            return BL.SearchDr(model);
        }

        public ActionResult drSearchToday()
        {
            return BL.SearchDr(new Fn01LM_DRSearchModel() { IsToday = true });
        }

        public ActionResult drStatistics()
        {
            return View("drStatistics", BL.GetStatModel());
        }

        public ActionResult SearchStatistics(Fn01LM_DRStatModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.SearchStatistics(model);
        }
        [HttpPost]
        public ActionResult ExcelStatistics(Fn01LM_DRStatModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(BL.ExcelStatistics(model));
        }


        public ActionResult SearchStatisticsV2(Fn01LM_DRStatModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.SearchStatisticsV2(model);
        }
        [HttpPost]
        public ActionResult ExcelStatisticsV2(Fn01LM_DRStatModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(BL.ExcelStatisticsV2(model));
        }


        #endregion

        #region Export

        public ActionResult Export(Fn01LM_DRSearchModel model)
        {
            model.IsMaintenance = true;
            return BL.ExportDr(model);
        }

        public ActionResult ExportToday(Fn01LM_DRSearchModel model)
        {
            model.IsToday = true;
            return BL.ExportDr(model);
        }


        #endregion

        #region Create

        public ActionResult SaveDr(Fn01LM_DRSaveModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.CreateDr(model);
        }

        #endregion

        #region Update

        public ActionResult UpdateDr(Fn01LM_DRSaveModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.UpdateDr(model);
        }

        #endregion


        public ActionResult AjaxAPRIContractorName(string regNo)
        {
            //plesae move it to the service layer..
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_APPLICANT r = db.C_IND_CERTIFICATE.Where(o => o.CERTIFICATION_NO == regNo).Select(o => o.C_IND_APPLICATION.C_APPLICANT).FirstOrDefault();

                return Json(new { FULL_NAME_DISPLAY = (r == null ? "" : r.FULL_NAME_DISPLAY) });
            }
        }
        [HttpPost]
        public ActionResult AjaxFormByDsn(string dsn)
        {
            //plesae move it to the service layer..
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
               
                P_MW_DSN r = db.P_MW_DSN.Where(o => o.DSN == dsn).FirstOrDefault();
                return Json(new { FULL_NAME_DISPLAY = (r == null ? "" : r.FORM_CODE) });
            }
        }
    }
}