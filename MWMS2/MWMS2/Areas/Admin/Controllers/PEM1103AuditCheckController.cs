using MWMS2.Areas.Admin.Models;
using MWMS2.Controllers;
using MWMS2.DaoController;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103AuditCheckController : Controller
    {
        // GET: Admin/PEM1103AuditCheck
        public ActionResult Index()
        {
            return View();
        }

        private EntitiesRegistration db = new EntitiesRegistration();
        CommonFunction cf = new CommonFunction();

        DaoPEMAuditPercentage daoPEMAuditPercentage = new DaoPEMAuditPercentage();
     
        public ActionResult SelectedYearChange(int year)
        {

            var query = daoPEMAuditPercentage.GetAuditCheckPercentageByYear(year);

            if (query.Count() > 0)
            {
                var result = new
                {
                    //pass Chi, eng, status and as to view
                    query.FirstOrDefault().MODIFIED_BY,
                    YearPercentage = query.FirstOrDefault().PERCENTAGE,
                    MODIFIED_DATE = cf.DateTimeToString(query.FirstOrDefault().MODIFIED_DATE)



                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            return Json("", JsonRequestBehavior.AllowGet);




        }

        public ActionResult PEM1103AuditCheckPercentage(string year, string message)
        {

            var ThisYear = System.DateTime.Now.Year;
            if (!string.IsNullOrEmpty(year))
                ThisYear = Convert.ToInt32(year);
            var query = daoPEMAuditPercentage.GetAuditCheckPercentageByYear(ThisYear);
            //List<SelectListItem> YearList = new List<SelectListItem>();



            //for (int i = 0; i < 5; i++)
            //{
            //    YearList.Add(new SelectListItem
            //    {
            //        Text = (ThisYear + i).ToString(),
            //        Value = (ThisYear + i).ToString()

            //    });
            //}


            //ViewBag.Year = YearList;

            ViewBag.LatestModified_Date = query.FirstOrDefault().MODIFIED_DATE;

            ViewBag.LatestModified_By = query.FirstOrDefault().MODIFIED_BY;

            if (!string.IsNullOrEmpty(message))
                ViewBag.Message = message;


            return View(new PEM1103AuditCheckPercentageModel() { AuditCheckPercentage = query.FirstOrDefault() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPEM1103AuditCheckPercentage([Bind(Exclude = "")] PEM1103AuditCheckPercentageModel ACP)
        {
            if (daoPEMAuditPercentage.SetAuditCheckPercentage(ACP.AuditCheckPercentage))
            {
                //return Redirect("/PEM1103/PEM1103");
                return RedirectToAction("PEM1103AuditCheckPercentage", new { year = ACP.AuditCheckPercentage.YEAR.ToString(), message = "Save Successfully." });
            }
            else
            {

                return Json(false);
            }


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