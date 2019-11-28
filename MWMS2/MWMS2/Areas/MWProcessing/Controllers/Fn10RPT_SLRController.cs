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
    public class Fn10RPT_SLRController : Controller
    {

        //private ProcessingFn10RPT_SLRBLService _BL;
        //protected ProcessingFn10RPT_SLRBLService BL
        //{
        //    get
        //    {
        //        return _BL ?? (_BL = new ProcessingFn10RPT_SLRBLService());
        //    }
        //}


        private ProcessingReportBLService _BL;
        protected ProcessingReportBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingReportBLService());
            }
        }

        // GET: MWProcessing/Fn09RPT_SLR
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn10RPT_SLRModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.Fn10RPT_SLRSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn10RPT_SLRModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Fn10RPT_SLRWorkbook(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Date", "Time", "Document S/N	", "Ref No.", "Activity", "Handling Officer" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25" , "v26" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35" , "v36" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}