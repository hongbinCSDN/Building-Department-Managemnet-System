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
    public class Fn10RPT_DTRSController : Controller
    {
        private PrcessingRPT_DTRSBLService _BL;
        protected PrcessingRPT_DTRSBLService BL
        {
            get { return _BL ?? (_BL = new PrcessingRPT_DTRSBLService()); }
        }

        // GET: MWProcessing/Fn09RPT_DTRS
        public ActionResult Index()
        {
            return View(new Fn10RPT_DTRSSearchModel());
        }
        public ActionResult SearchIncoming(Fn10RPT_DTRSSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model,"Incoming"), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchOutgoing(Fn10RPT_DTRSSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model,"Outgoing"), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult IncomingExportExcel(Fn10RPT_DTRSSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.IncomingExportExcel(model, "Incoming") });
        }

        public ActionResult OutgoingExportExcel(Fn10RPT_DTRSSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.OutgoingExportExcel(model, "Outgoing") });
        }

        [HttpPost]
        public ActionResult Excel(FormCollection post)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            DisplayGrid dlr = demoSearch();
            return Json(new {});
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plan to fix header problem
            //dlr.Columns = new string[] { "DSN", "Ref No.", "R&D Outgoing(Date/Time)", "Subject Matter", "Channel of Enquiry", "Status", "Handling Officer" };
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