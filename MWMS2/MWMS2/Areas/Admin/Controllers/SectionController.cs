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
    public class SectionController : ValidationController
    {
        //SysSectionBLService
        private static volatile SysSectionBLService _BL;
        private static readonly object locker = new object();
        private static SysSectionBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new SysSectionBLService(); return _BL; } }


        // GET: Admin/Section
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(Sys_SectionModel model)
        {
            return BL.Search(model);
        }

        public ActionResult EditForm(Sys_SectionModel model)
        {
            //get model
            BL.GetRecord(model);
            return View(model);
        }

        public ActionResult Excel(Sys_SectionModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Excel(model) } );
        }

        public ActionResult Update(Sys_SectionModel model)
        {
            //get model
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.Update(model);
        }

        public ActionResult CreateForm()
        {
            return View();
        }

        public ActionResult Create(Sys_SectionModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.Create(model);
        }
    }
}