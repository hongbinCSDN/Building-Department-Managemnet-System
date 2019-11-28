using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_AFCPRController : ValidationController
    {
        //ProcessingSRBLService
        private ProcessingReportAFCPBLService BLService;
        protected ProcessingReportAFCPBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingReportAFCPBLService()); }
        }

        // GET: MWProcessing/Fn10RPT_SR
        public ActionResult Index()
        {
            Fn10RPT_AFCModel model = new Fn10RPT_AFCModel();
            BL.loadDefault(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult ExportToExcel(Fn10RPT_AFCModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);

            return BL.ExportToExcel(model);
        }

        public ActionResult validation(Fn10RPT_AFCModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
        }
    }
}