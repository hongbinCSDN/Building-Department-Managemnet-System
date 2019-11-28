using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_OtherController : ValidationController
    {
        private ProcessingAckOtherBLService _BL;
        protected ProcessingAckOtherBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingAckOtherBLService());
            }
        }

        // GET: MWProcessing/Fn01LM_Other
        public ActionResult Index()
        {
            return View();
        }

        #region Direct Returned Over Counter
        [HttpPost]
        public ActionResult DROverCounter()
        {
            return PartialView(new Fn01LM_OtherDROverCounterModel());
        }

        [HttpPost]
        public ActionResult UpdateDROverCounter(Fn01LM_OtherDROverCounterModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateDROverCounter(model));
        }
        #endregion

        #region Change File Reference No
        [HttpPost]
        public ActionResult AckOrderRelatedStatus()
        {
            return PartialView(new Fn01LM_OtherChangeORS());
        }
        [HttpPost]
        public ActionResult UpdateAckOrderRelatedStatus(Fn01LM_OtherChangeORS model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateAckOrderRelatedStatus(model));
        }
        [HttpPost]
        public ActionResult GetOrderRelated(Fn01LM_OtherChangeORS model)
        {
            return Json(BL.GetOrderRelated(model.DSN));
        }
        #endregion

        #region Update File Reference No
        [HttpPost]
        public ActionResult AckFileReferrNo()
        {
            return PartialView(new Fn01LM_OtherUpdateFRN());
        }
        [HttpPost]
        public ActionResult UpdateAckFileReferrNo(Fn01LM_OtherUpdateFRN model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateAckFileReferrNo(model));
        }
        #endregion

        #region Batch update referral date
        [HttpPost]
        public ActionResult AckReferrDate()
        {
            return PartialView(new Fn01LM_OtherBatchUpdateRD());
        }
        [HttpPost]
        public ActionResult UpdateAckReferrDate(Fn01LM_OtherBatchUpdateRD model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateAckReferrDate(model));
        }
        #endregion

    }
}