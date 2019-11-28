using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn05IIF_IIFController : Controller
    {
        private ProcessingIIF_IIFBLService _BL;
        protected ProcessingIIF_IIFBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingIIF_IIFBLService());
            }
        }

        // GET: MWProcessing/Fn05IIF_IIF
        public ActionResult Index()
        {
            Fn05IIF_IIFModel model = new Fn05IIF_IIFModel();
            model.ImportType = "Excel";
            return View(model);
        }
        public JsonResult Search()
        {
            DisplayGrid dlr = demoSearch();
            return Json(dlr);
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


            //dlr.Columns = new string[] { "File", "Update Date", "Uploaded By", "Status(Success/Total)", "Action", "Result" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }

        [HttpPost]
        public ActionResult Search(Fn05IIF_IIFModel model)
        {
            BL.Search(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file, Fn05IIF_IIFModel model)
        {
            model.Import36 = new Entity.P_IMPORT_36();
            model.Import36.FILENAME = file.FileName;
            model.Import36.CREATED_DT = DateTime.Now;

            //byte[] fileBytes = new byte[file.ContentLength];
            //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

            using (var package = new ExcelPackage(file.InputStream))
            {
                model.ExcelPackage = package;
                ServiceResult result = BL.ImportExcel(model);
                //return Json(result);
                if (result.Result == ServiceResult.RESULT_SUCCESS)
                {
                    ViewBag.Message = "File uploaded successfully";
                }
                else
                {
                    ViewBag.Message = "ERROR:" + result.Message[0];
                }
                //return RedirectToAction("/Index", model);
                return View("Index", model);
            }
        }

        public ActionResult ExportExcel(Fn05IIF_IIFModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportExcel(model) });
        }

    }
}