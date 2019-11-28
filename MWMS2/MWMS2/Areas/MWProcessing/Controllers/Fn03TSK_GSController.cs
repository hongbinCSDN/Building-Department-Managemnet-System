using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn03TSK_GSController : Controller
    {
        private ProcessingTSKGSBLService _BL;
        protected ProcessingTSKGSBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingTSKGSBLService());
            }
        }

        // GET: MWProcessing/Fn03TSK_GS
        public ActionResult Index()
        {
            return View(new Fn03TSK_GSModel());
        }
        public ActionResult Search(Fn03TSK_GSModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03TSK_GSModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.Search(model);
            return Json(new { key = model.ExportCurrentData("Submission Search_" + DateTime.Now.ToString("yyyy-MM-dd")) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plans to add header to table ("result")
            //dlr.Columns = new string[] { "DSN/ ICC No.", "Ref. No.", "Date of Assignment", "Final Reply Due Date", "Final Reply Remaining Days", "Interim Reply Date", "Interim Reply Remaining Days", "Title", "Channel", "Status"};
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17", "v18", "v19", "v20" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28", "v29", "v30" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37", "v38", "v39", "v40" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}