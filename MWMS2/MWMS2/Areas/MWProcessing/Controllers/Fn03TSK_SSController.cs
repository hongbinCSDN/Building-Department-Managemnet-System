using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
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
    public class Fn03TSK_SSController : Controller
    {
        private ProcessingTSKSSBLService _BL;
        protected ProcessingTSKSSBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingTSKSSBLService());
            }
        }

        // GET: MWProcessing/Fn03TSK_SS
        public ActionResult Index()
        {
            return View(new Fn03TSK_SSSearchModel()
            {
                AP = new P_MW_APPOINTED_PROFESSIONAL()
                ,
                PRC = new P_MW_APPOINTED_PROFESSIONAL()
                ,
                MWAddress = new P_MW_ADDRESS()
            });
        }
        public ActionResult Search(Fn03TSK_SSSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03TSK_SSSearchModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Excel(model) });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plans to add header to table ("result")
            //dlr.Columns = new string[] { "Ref No.", "Item No.", "Commencement Date", "Completion Date ", "Location of Minor Work" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15"});
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25"});
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35"});

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }


    }
}