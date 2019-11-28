using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.Controllers
{
    public class SMM1103AuditCheckPercentageoldController : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SelectedYearChange(short? year)
        {
            var query = db.B_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == year);
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
            //decimal? m_YearPercentage = null ;
            //if (query.Count() == 0)
            //{ m_YearPercentage = null; }
            //else
            //{ m_YearPercentage = query.FirstOrDefault().PERCENTAGE; }
            //var result = new
            //{
            //    //pass Chi, eng, status and as to view

            //    YearPercentage = m_YearPercentage

            //};
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SMM1103AuditCheckPercentage()
        {
            var ThisYear = System.DateTime.Now.Year;
            var query = db.B_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == ThisYear);
            List<SelectListItem> YearList = new List<SelectListItem>();
       
            for (int i = 0; i < 5; i++)
            {
                YearList.Add(new SelectListItem
                {
                    Text = (ThisYear + i).ToString(),
                   Value = (ThisYear + i).ToString()

                });
            }


            ViewBag.Year = YearList;

            ViewBag.LatestModified_Date = db.B_S_AUDIT_CHECK_PERCENTAGE.OrderByDescending(x => x.MODIFIED_DATE).First().MODIFIED_DATE;

            ViewBag.LatestModified_By = db.B_S_AUDIT_CHECK_PERCENTAGE.OrderByDescending(x => x.MODIFIED_DATE).First().MODIFIED_BY;

           


            return View(query.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM1103AuditCheckPercentage([Bind(Exclude = "")] B_S_AUDIT_CHECK_PERCENTAGE ACP)
        {
            try
            {
                if (!(ACP.PERCENTAGE >= 0 && ACP.PERCENTAGE<=100))
                {
                    return Json(false);
                }
                
                var query = db.B_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == ACP.YEAR);
                if (query.Count()==0)
                {

                    ACP.UUID = System.Guid.NewGuid().ToString();

                    ACP.MODIFIED_DATE = System.DateTime.Now;
                    ACP.MODIFIED_BY = SystemParameterConstant.UserName;
                    ACP.CREATED_DATE = System.DateTime.Now;
                    ACP.CREATED_BY = SystemParameterConstant.UserName;
                    db.B_S_AUDIT_CHECK_PERCENTAGE.Add(ACP);
                }
                else
                {

                    query.FirstOrDefault().PERCENTAGE = ACP.PERCENTAGE;
                    query.FirstOrDefault().MODIFIED_DATE = System.DateTime.Now;
                    query.FirstOrDefault().MODIFIED_BY = SystemParameterConstant.UserName;
                    db.Entry(query.FirstOrDefault()).State = EntityState.Modified;
                 
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Redirect("/SMM1103/SMM1103");
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