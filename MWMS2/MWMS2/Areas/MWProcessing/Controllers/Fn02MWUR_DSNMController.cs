using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_DSNMController : Controller
    {
        private ProcessingDsnMappingBLService _DsnMappingService;
        protected ProcessingDsnMappingBLService DsnMappingService
        {
            get { return _DsnMappingService ?? (_DsnMappingService = new ProcessingDsnMappingBLService()); }
        }
        // GET: MWProcessing/Fn02MWUR_DSNM
        public ActionResult Index()
        {
            Fn02MWUR_DsnMappingModel model = new Fn02MWUR_DsnMappingModel();
            model.Total = DsnMappingService.GetDsnMappingTotal();
            return View(model);
        }
        public JsonResult Search(Fn02MWUR_DsnMappingModel model)
        {
            DsnMappingService.Search(model);
            return Json(model);
        }
        [HttpPost]
        public ActionResult Excel(Fn02MWUR_DsnMappingModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            DsnMappingService.Search(model);
            return Json(new { key = model.ExportCurrentData("DSN Mapping") });
        }

        public ActionResult GetDsnInfo(string DSN)
        {
            return DsnMappingService.GetDsnInfo(DSN);
        }

        public ActionResult Detail(string DSN)
        {
            if (string.IsNullOrWhiteSpace(DSN)) { return Redirect("Index"); }
            Fn02MWUR_DsnMappingModel model = new Fn02MWUR_DsnMappingModel();
            model.DSN = DSN;
            return View(model);
        }

        //public ActionResult GetDsnInfo(string DSN)
        //{

        //}

        public ActionResult Assign(Fn02MWUR_DsnMappingModel model)
        {
            return DsnMappingService.Assign(model);
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Document S/N", " Date", "Time", "Received", "" };
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