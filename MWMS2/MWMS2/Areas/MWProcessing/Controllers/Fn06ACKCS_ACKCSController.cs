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
    public class Fn06ACKCS_ACKCSController : Controller
    {
        private ProcessingACKCSBLService _BL;
        protected ProcessingACKCSBLService BL
        {
            get
            {
                return _BL ?? (new ProcessingACKCSBLService());
            }
        }

        // GET: MWProcessing/Fn05ACKCS_ACKCS
        public ActionResult Index()
        {
            Fn06ACKCS_ACKCSModel model = new Fn06ACKCS_ACKCSModel();
            model.SubmissionDateFrom = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            model.SubmissionDateTo = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }
        public ActionResult Search(Fn06ACKCS_ACKCSModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn06ACKCS_ACKCSModel model)
        {
            //if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            //DisplayGrid dlr = demoSearch();
            //return Json(new { });
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Excel(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plans to add header to table ("result")
            //dlr.Columns = new string[] { "Ref No.", "Form No.", "Assignment Date", "Status" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}