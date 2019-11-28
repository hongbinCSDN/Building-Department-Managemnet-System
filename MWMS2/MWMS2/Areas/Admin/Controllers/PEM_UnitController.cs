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
    public class PEM_UnitController : ValidationController
    {
        //PEM_UnitBLService
        private PEM_UnitBLService BLService;
        protected PEM_UnitBLService BL
        {
            get { return BLService ?? (BLService = new PEM_UnitBLService()); }
        }
        // GET: Admin/PEM_Unit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(PEM_UnitModel model)
        {
            return BL.Search(model);
        }

        public ActionResult EditForm(PEM_UnitModel model)
        {
            //get model
            BL.GetRecord(model);
            return View(model);
        }

        public ActionResult Update(PEM_UnitModel model)
        {
            //get model
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.Update(model);
        }

        public ActionResult Export(PEM_UnitModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Export(model) });
        }

        public ActionResult CreateForm()
        {
            return View();
        }

        public ActionResult Create(PEM_UnitModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.Create(model);
        }
    }
}