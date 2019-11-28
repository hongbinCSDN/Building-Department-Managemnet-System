using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Controllers;
using System.Data.Entity;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SPMWorkflowSettingController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SPMWorkflowSetting()
        {
            var s_param = db.B_S_PARAMETER.FirstOrDefault();
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Yes", Value = "Y"}
                    , new SelectListItem { Text = "No", Value = "N"}
                };

            ViewBag.List = selectListItems;
            ViewBag.LatestModified_Date = db.B_S_PARAMETER.OrderByDescending(x => x.MODIFIED_DATE).First().MODIFIED_DATE;
            ViewBag.LatestModified_By = db.B_S_PARAMETER.OrderByDescending(x => x.MODIFIED_DATE).First().MODIFIED_DATE;
            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(s_param.MODIFIED_DATE);
            return View(s_param);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMWorkflowSetting([Bind(Exclude = "")] B_S_PARAMETER SPARAM)
        {
            try
            {
                // Validation
                if (SPARAM.PROCESS_S24_DAY < 0 )
                {
                    return Json(false);
                }
                if (SPARAM.PROCESS_GC_DAY < 0)
                {
                    return Json(false);
                }
                if (SPARAM.NUMBER_OF_JOB_ASSIGN < 0)
                {
                    return Json(false);
                }
                if (SPARAM.VALIDATION_EXPIRY_YEAR_NUM < 0)
                {
                    return Json(false);
                }
                if (SPARAM.TARGET_DATE < 0)
                {
                    return Json(false);
                }

                var s_param = db.B_S_PARAMETER.FirstOrDefault();
                if (s_param == null) // not exist -> create a new record
                {
                    SPARAM.UUID = System.Guid.NewGuid().ToString();

                    //SPARAM.MODIFIED_DATE = System.DateTime.Now;
                    //SPARAM.MODIFIED_BY = SystemParameterConstant.UserName;
                    //SPARAM.CREATED_DATE = System.DateTime.Now;
                    //SPARAM.CREATED_BY = SystemParameterConstant.UserName;
                    db.B_S_PARAMETER.Add(SPARAM);
                }
                else
                {
                    s_param.SYSTEM_ASSIGN = SPARAM.SYSTEM_ASSIGN;
                    s_param.SPO_VALIDATION = SPARAM.SPO_VALIDATION;
                    s_param.SPO_AUDIT = SPARAM.SPO_AUDIT;
                    s_param.PROCESS_S24_DAY = SPARAM.PROCESS_S24_DAY;
                    s_param.PROCESS_GC_DAY = SPARAM.PROCESS_GC_DAY;
                    s_param.NUMBER_OF_JOB_ASSIGN = SPARAM.NUMBER_OF_JOB_ASSIGN;
                    s_param.VALIDATION_EXPIRY_YEAR_NUM = SPARAM.VALIDATION_EXPIRY_YEAR_NUM;
                    s_param.TARGET_DATE = SPARAM.TARGET_DATE;

                    //s_param.MODIFIED_DATE = System.DateTime.Now;
                    //s_param.MODIFIED_BY = SystemParameterConstant.UserName;

                    db.Entry(s_param).State = EntityState.Modified;

                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("SPMWorkflowSetting");
            //return Redirect("/Admin/SMMSysParaMan/Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}