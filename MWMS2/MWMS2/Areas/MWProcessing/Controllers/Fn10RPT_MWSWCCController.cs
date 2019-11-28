using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using NPOI.SS.UserModel;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_MWSWCCController : Controller
    {
        private ProcessingReportBLService _BL;
        protected ProcessingReportBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingReportBLService());
            }
        }
        // GET: MWProcessing/Fn09RPT_MWSWCC
        public ActionResult Index()
        {
            Fn10RPT_MWMSWCCModel model = new Fn10RPT_MWMSWCCModel();
            model.StatusFormMW0204 = new List<Checkboxlist>()
                {
                    new Checkboxlist(){Code="Acknowledged",Description="Acknowledged",IsChecked=true}
                    ,new Checkboxlist(){Code="No Submission",Description="No Submission",IsChecked=true}
                    ,new Checkboxlist(){Code="Processing",Description="Processing",IsChecked=true}
                    ,new Checkboxlist(){Code="Refused",Description="Refused",IsChecked=true}
                };
            return View(model);
        }
        public ActionResult Search(Fn10RPT_MWMSWCCModel model)
        {

            //return Json(BL.GetIncompletedRecord(model).Data);
            return Content(JsonConvert.SerializeObject(BL.Fn10RPT_MWMSWCC_Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn10RPT_MWMSWCCModel model, FormCollection post)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);

            MemoryStream mstream = new MemoryStream();
            IWorkbook wb = BL.Fn10RPT_MWMSWCC_Workbook(model);


            wb.Write(mstream);

            return File(mstream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Submission_Location_Report.xlsx");
        }


        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Form Type", "MW No.", "Date of Receive", "Date of Commencement", "Status of Corresponding Form MW02/MW04" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }

        //private static Dictionary<DateTime, FileStreamResult> preloadMemory = new Dictionary<DateTime, FileStreamResult>();
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void addMemory(DateTime k, FileStreamResult fsr)
        //{
        //    DateTime nowDt = DateTime.Now;
        //    foreach (KeyValuePair<DateTime, FileStreamResult> entry in preloadMemory.ToList())
        //    {
        //        TimeSpan ts = (nowDt - entry.Key);
        //        if (ts.TotalSeconds > 60) preloadMemory.Remove(entry.Key);
        //    }
        //    preloadMemory.Add(k, fsr);
        //}
    }
}