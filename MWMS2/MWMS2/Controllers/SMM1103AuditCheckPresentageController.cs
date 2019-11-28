using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Dao;
namespace MWMS2.Controllers
{
    public class SMM1103AuditCheckPresentageController : Controller
    {
        Entities db = new Entities();
        // GET: SMM1103AuditCheckPresentage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM1103AuditCheckPresentage()
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
        public ActionResult SMM1103AuditCheckPresentage([Bind(Exclude = "")] B_S_AUDIT_CHECK_PERCENTAGE ACP)
        {
            try
            {
                var query = db.B_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == ACP.YEAR);
                if (query.Count()==0)
                {

                    ACP.UUID = System.Guid.NewGuid().ToString();

                    ACP.MODIFIED_DATE = System.DateTime.Now;
                    ACP.MODIFIED_BY = "Admin";
                    ACP.CREATED_DATE = System.DateTime.Now;
                    ACP.CREATED_BY = "Admin";
                    db.B_S_AUDIT_CHECK_PERCENTAGE.Add(ACP);
                }
                else
                {

                    query.FirstOrDefault().PERCENTAGE = ACP.PERCENTAGE;
                    query.FirstOrDefault().MODIFIED_DATE = System.DateTime.Now;
                    query.FirstOrDefault().MODIFIED_BY = "Admin";
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