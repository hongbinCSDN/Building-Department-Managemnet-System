using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn09DS_DsController : Controller
    {
        //ProcessingDocSortingBLService
        private ProcessingDocSortingBLService BLService;
        protected ProcessingDocSortingBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingDocSortingBLService()); }
        }

        [HttpPost]
        public ActionResult Index(Fn09DS_DsModel model)
        {
            return View();
        }

        [HttpPost]
        public ActionResult FindDocSorting(Fn09DS_DsModel model)
        {   ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
            return Content(JsonConvert.SerializeObject(rs.FindDocSorting(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        [HttpPost]
        public ActionResult Excel(Fn09DS_DsModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
            return Json(new { key = rs.ExportFindDocSorting(model) });
        }

        [HttpPost]
        public ActionResult ViewDetail(string dsn, string taskID)
        {
            ModelState.Clear();
            ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
            Fn09DS_DsModel model = rs.ViewDetail(dsn, taskID);
            return View("Edit",model);

        }
        [HttpPost]
        public ActionResult changeType(Fn09DS_DsModel model)
        {
            ModelState.Clear();
            ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
             rs.changeType(model);
            return View("Edit", model);

        }
        [HttpPost]
        public ActionResult genNumber(Fn09DS_DsModel model)
        {
            ModelState.Clear();

            ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
            rs.genNumber(model);
            return View("Edit", model);

        }
        [HttpPost]
        public ActionResult submit(Fn09DS_DsModel model)
        {
            ModelState.Clear();
            ProcessingDocSortingBLService rs = new ProcessingDocSortingBLService();
            if (rs.submit(model))
            {
                return View("Index", model);
            }
            else
            {
                return View("Edit", model);
            };

        }


    }
}