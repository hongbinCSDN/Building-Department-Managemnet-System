using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_RCPController : Controller
    {
        private ProcessingMWURegistryBLService _BL;
        protected ProcessingMWURegistryBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingMWURegistryBLService());
            }
        }

        private MWPNewSubmissionDAOService mWPNewSubmissionDAOService;
        protected MWPNewSubmissionDAOService MWPNewSubmissionDAOService
        {
            get { return mWPNewSubmissionDAOService ?? (mWPNewSubmissionDAOService = new MWPNewSubmissionDAOService()); }
        }

        // GET: MWProcessing/Fn02MWUR_RCP
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn02MWUR_ReceiptModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchReceipt(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }

        // Click Outstanding Button
        public ActionResult OustandingAction(Fn02MWUR_ReceiptModel model)
        {
            BL.confirmOutstanding(model);
            return View("Index"); ;
        }

        // Receive DSN Action
        public ActionResult ReceiveCountedDsnAction(Fn02MWUR_ReceiptModel model)
        {
            BL.ReceiveCountedDsnAction(model);
            return View("Index"); ;
        }

        // Receive DSN Action
        public ActionResult ConfirmReceivedAction(Fn02MWUR_ReceiptModel model)
        {
            BL.ConfirmReceivedAction(model);
            return View("Index"); ;
        }

        [HttpPost]
        public ActionResult Excel(Fn02MWUR_ReceiptModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.SearchReceipt(model);
            return Json(new { key = model.ExportCurrentData("Submission_Search_"+DateTime.Now.ToString("yyyy-MM-dd")) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Date",  "Time", "Document S/N", "Received", "Outstanding" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}