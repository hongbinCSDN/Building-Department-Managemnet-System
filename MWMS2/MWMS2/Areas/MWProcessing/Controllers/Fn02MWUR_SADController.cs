using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.SysService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_SADController : Controller
    {
        // GET: MWProcessing/Fn02MWRU_SAD
        public ActionResult Index()
        {
            Fn02MWUR_SADModel model = new Fn02MWUR_SADModel();
            return View(model);
        }

        public ActionResult Search(Fn02MWUR_SADModel model)
        {
            ProcessingSADService ss = new ProcessingSADService();
            return Content(JsonConvert.SerializeObject(ss.SearchSAD(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }

        public ActionResult CheckDSN(string dsn)
        {
            ProcessingSADService ss = new ProcessingSADService();
            return Json(ss.CheckDSN(dsn));
        }

        public ActionResult Form(Fn02MWUR_SADDisplayModel model)
        {
            ProcessingSADService ss = new ProcessingSADService();
            if (string.IsNullOrWhiteSpace(model.UUID) && string.IsNullOrWhiteSpace(model.DSN))
            {
                return View(ss.CreateNewDSN(model));
            }
            if (!string.IsNullOrWhiteSpace(model.UUID))
            {
                model = ss.DetailDoc(model.UUID);
            }
            else if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                model = ss.DetailDocByDSN(model.DSN);
            }
            return View(model);
        }

        public ActionResult SearchDSN(Fn02MWUR_SADModel model)
        {
            ProcessingSADService ss = new ProcessingSADService();
            return Content(JsonConvert.SerializeObject(ss.SearchDSN(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }

        public ActionResult SearchScanDoc(Fn02MWUR_SADDisplayModel model)
        {
            ProcessingSADService ss = new ProcessingSADService();

            return Content(JsonConvert.SerializeObject(ss.SearchScanDoc(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn02MWUR_SADModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            ProcessingSADService ss = new ProcessingSADService();
            return Json(new { key = ss.Excel(model) });
        }

        public ActionResult CompleteScan(Fn02MWUR_SADDisplayModel model)
        {
            ProcessingSADService ss = new ProcessingSADService();
            return Json(ss.CompleteScan(model));
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Document S/N", "Assignment Date", "Time", "Ref. No.","Form", "Status" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
    }
}