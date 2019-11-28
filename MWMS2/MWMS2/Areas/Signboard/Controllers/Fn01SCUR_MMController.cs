using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn01SCUR_MMController : Controller
    {
        private SignboardMMBLService _BL;
        protected SignboardMMBLService BL
        {
            get
            {
                return _BL ?? (_BL = new SignboardMMBLService());
            }
        }
        // GET: Signboard/Fn01SCUR_MM
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(Fn01SCUR_MMSearchModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.mailMergeSearch(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Submission No.,", "DSN No", "Form Code", "Received Date", "Time", "Status", "Batch Number" };
            //dlr.Datas = new List<object[]>();

            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37" });


            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }
        public ActionResult exportForExcel(string a)
        {
            string uuid = "";
            if (a.Length > 0)
            {
                uuid = a.Substring(0, a.Length - 1);
            }
            SignboardMMBLService s = new SignboardMMBLService();
            return s.exportToXLSX(uuid);
        }

        public ActionResult exportForMailMerge(string a)
        {
            string uuid = "";
            if (a.Length > 0)
            {
                uuid = a.Substring(0, a.Length - 1);
            }
            SignboardMMBLService s = new SignboardMMBLService();
            return s.exportToCSV(uuid);
        }

        public ActionResult completeMailMerge(string a)
        {
            Fn01SCUR_MMSearchModel model = new Fn01SCUR_MMSearchModel();
            string uuid = "";
            if (a.Length > 0)
            {
                uuid = a.Substring(0, a.Length - 1);
            }
            SignboardMMBLService s = new SignboardMMBLService();
            String[] uuids = uuid.Split(',');
            if (uuids.Length > 1) {
                for (int i = 0; i < uuids.Length; i++) {
                    s.UpdateLetterStatus(uuids[i], ApplicationConstant.SV_VALIDATION_LETTER_STATUS_DONE, "abc");
                    //return Content(JsonConvert.SerializeObject(s.UpdateLetterStatus(uuids[i], ApplicationConstant.SV_VALIDATION_LETTER_STATUS_DONE, "abc"), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
                }
            }
            else {
                s.UpdateLetterStatus(uuid, ApplicationConstant.SV_VALIDATION_LETTER_STATUS_DONE, "abc");
                //return Content(JsonConvert.SerializeObject(s.UpdateLetterStatus(uuid, ApplicationConstant.SV_VALIDATION_LETTER_STATUS_DONE, "abc"), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }
            return View("Index");
        }
    }
}