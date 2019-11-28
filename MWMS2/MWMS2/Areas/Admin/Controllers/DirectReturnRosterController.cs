using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class DirectReturnRosterController : ValidationController
    {
        private DirectReturnRosterBLService _blService;
        protected DirectReturnRosterBLService blService
        {
            get { return _blService ?? (_blService = new DirectReturnRosterBLService()); }
        }

        // GET: Admin/DirectReturnRoster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(DirectReturnRosterModel model)
        {
            return Content(blService.Search(model), "application/json");
        }

        public ActionResult Excel(DirectReturnRosterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = blService.ExportExcel(model) });
        }

        public ActionResult Add(DirectReturnRosterModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(blService.Add(model));
        }

        public ActionResult Update(DirectReturnRosterModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(blService.Update(model));
        }

        public ActionResult Delete(DirectReturnRosterModel model)
        {
            return Json(blService.Delete(model));
        }

        public ActionResult GetRosterInfoByDate(DateTime? onDutyDate)
        {
            return Json(blService.GetRosterInfoByDate(onDutyDate));
        }
    }
}