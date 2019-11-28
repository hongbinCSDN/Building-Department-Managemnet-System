using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn03TSK_ASController : Controller
    {
        private ProcessingTSKASBLService _BL;
        protected ProcessingTSKASBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingTSKASBLService());
            }
        }

        // GET: MWProcessing/Fn03TSK_AS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchMwRecordByAddress(Fn03TSK_ASModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchMwRecordByAddress(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchMWGeneralRecordByAddress(Fn03TSK_ASModel model)
        {

            return Content(JsonConvert.SerializeObject(BL.SearchMWGeneralRecordByAddress(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult ExcelRecord(Fn03TSK_ASModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExcelRecord(model) });
        }

        [HttpPost]
        public ActionResult ExcelGR(Fn03TSK_ASModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExcelGR(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Ref No.", "Item No.", "Status", "Received Date", "Commencement  Date", "Completion Date" };
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