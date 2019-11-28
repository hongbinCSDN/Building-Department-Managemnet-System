using MWMS2.Areas.MWProcessing.Models;
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
    public class Fn03TSK_STSController : Controller
    {
        private ProcessingTSKSTSBLService _BL;
        protected ProcessingTSKSTSBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingTSKSTSBLService());
            }
        }

        // GET: MWProcessing/Fn03Tsk_STS
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchComplaintList(Fn03TSK_STSModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchComplaintList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult SearchEnquiryList(Fn03TSK_STSModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchEnquiryList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult SearchAuditList(Fn03TSK_STSModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchAuditList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult SearchSubmissionList(Fn03TSK_STSModel model)
        //{

        //}

        [HttpPost]
        public ActionResult Excel(FormCollection post)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            DisplayGrid dlr = demoSearch();
            return Json(new { });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Task", "Ref. No.", "Form No.", "Received Date", "Assignment Date","Status" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15","v16" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25","v26" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35","v36" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }

    }
}