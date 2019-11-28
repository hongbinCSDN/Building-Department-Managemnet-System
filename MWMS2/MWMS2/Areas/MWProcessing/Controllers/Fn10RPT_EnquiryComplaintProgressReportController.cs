using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_EnquiryComplaintProgressReportController : Controller
    {

        private ProcessingReportBLService _BL;
        protected ProcessingReportBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingReportBLService());
            }
        }

        // GET: MWProcessing/Fn09RPT_ECPR
        public ActionResult Index()
        {
            Fn10RPT_ECPRModel model = new Fn10RPT_ECPRModel();
            return View(model);
        }
        public ActionResult SearchEnquiry(Fn10RPT_ECPRModel model)
        {
            model.ReportType = ProcessingConstant.ENQUIRY_REPORT; 
            return Content(JsonConvert.SerializeObject(BL.SearchEnquiryAndComplaintProgress(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchComplaint(Fn10RPT_ECPRModel model)
        {
            model.ReportType = ProcessingConstant.COMPLAINT_REPORT; 
            return Content(JsonConvert.SerializeObject(BL.SearchEnquiryAndComplaintProgress(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn10RPT_ECPRModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExcelEnquiryAndComplaintProgress(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plans to add header to table ("result")
            //dlr.Columns = new string[] { "Reference No.	", "Received Date", "Reply/Action Date", "Subject Matter", "Channel of Enquiry","Status","Handling Officer" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}