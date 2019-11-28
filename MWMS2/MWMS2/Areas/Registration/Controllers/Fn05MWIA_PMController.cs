using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using Newtonsoft.Json.Linq;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn05MWIA_PMController : ValidationController
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Registration/Fn03PA_PM
        public ActionResult Index(ProcessMonitorSearchModel model)
        {
            return View("IndexInd_PM", model);
            //return View("Index");
        }
        //public JsonResult Search()
        //{
        //    DisplayGrid dlr = demoSearch();
        //    return Json(dlr);
        //}

        public ActionResult Search(ProcessMonitorSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(rs.SearchPAandMWIA_PM(model, RegistrationConstant.REGISTRATION_TYPE_MWIA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string certUUID, string pmUUID)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            ProcessMonitorDisplayModel model = rs.ViewIndPM(certUUID, pmUUID, RegistrationConstant.REGISTRATION_TYPE_MWIA);
            return View("FormMWIA_PM", model);
        }
        public ActionResult Validate(ProcessMonitorDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(validateResult);
            return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
        }
        public ActionResult Save([Bind(Exclude = "")]ProcessMonitorDisplayModel model)
        {

            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.SaveInd_PM(model, RegistrationConstant.REGISTRATION_TYPE_MWIA);
            //return View("IndexInd_PM");
            Console.WriteLine(model.C_IND_PROCESS_MONITOR.OS_DATE);
            return RedirectToAction("Index",new ProcessMonitorDisplayModel());
        }
        public ActionResult Delete(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.DeleteIndPMRecord(id);
            return View("IndexInd_PM");
        }
        //[HttpPost]
        //public ActionResult Excel(Fn01Search_PASearchModel model)
        //{
        //    if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
        //    RegistrationSearchService rs = new RegistrationSearchService();
        //    return View();

        //}
        public ActionResult Excel(ProcessMonitorSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService s = new RegistrationCommonService();
            var regType = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            return Json(new { key = s.ExportPAandMWIA_PM(model, regType) });
        }
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "","File Reference",  "Name", "Category Code","Received Date","Under Monitor" };
            //dlr.Datas = new List<object[]>();
            ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14","v15","v16" });
            ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24","v25","v26" });
            ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34","v35","v36" });

            dlr.Rpp = 25;
            ////dlr.TotalRecord = 3;
            ////dlr.CurrentPage = 1;
            return dlr;
        }
    }
}