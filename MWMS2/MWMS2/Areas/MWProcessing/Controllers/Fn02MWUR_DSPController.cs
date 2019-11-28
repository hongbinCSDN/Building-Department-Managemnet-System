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
    public class Fn02MWUR_DSPController : Controller
    {
        private ProcessingMWURegistryBLService _BL;
        protected ProcessingMWURegistryBLService BL
        {
            get
            {
                return _BL ?? (_BL ?? new ProcessingMWURegistryBLService());
            }
        }

        // GET: MWProcessing/Fn02MWUR_DSP
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn02MWUR_DSPModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDispatch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }
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


            //dlr.Columns = new string[] { "Date", "Time", "Document S/N", "Received", "Outstanding" };
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