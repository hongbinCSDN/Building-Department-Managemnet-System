using System;
using System.Linq;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn04VRF_VRFController : ValidationController
    {
        //ProcessingVerficationBLService
        private ProcessingVerficationBLService BLService;
        protected ProcessingVerficationBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingVerficationBLService()); }
        }
        // GET: MWProcessing/Fn04Vrf_Vrf
        public ActionResult Index()
        {
            Fn04VRF_VRFModel model = new Fn04VRF_VRFModel();
            model.SubmissionDateFrom = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            model.SubmissionDateTo = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        //public ActionResult Form(VerificaionFormModel model)
        //{
        //    //Get Form Data
        //    BL.GetFormData(model);
        //    return View(model);
        //}

        public ActionResult Search(Fn04VRF_VRFModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn04VRF_VRFModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            model.Rpp = -1;
            Search(model);
            return Json(new { key = model.ExportCurrentData("Excel_" + DateTime.Now.ToString("yyyy-MM-dd")) });

            //if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            //DisplayGrid dlr = demoSearch();
            //return Json(new { });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();

            //plans to add header to table ("result")
            //dlr.Columns = new string[] { "Ref No.", "Form No.", "Minor Work Item", "Assignment Date", "Status", "SSP" };
            //dlr.Datas = new List<object[]>();
            //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16" });
            //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26" });
            //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36" });

            dlr.Rpp = 25;
            //dlr.TotalRecord = 3;
            //dlr.CurrentPage = 1;
            return dlr;
        }

        //public ActionResult SaveAndNext(VerificaionFormModel model)
        //{
        //    return BL.SaveAndNext(model);
        //}

        //public ActionResult AddAddressInfo(VerificaionFormModel model)
        //{
        //    if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
        //    return BL.AddAddressInfo(model);
        //}

        //public ActionResult DeleteAddressInfo(VerificaionFormModel model)
        //{
        //    return BL.DeleteAddressInfo(model);
        //}

        //public ActionResult AddPsacImage(VerificaionFormModel model)
        //{
        //    return BL.AddPsacImage(model);
        //}

        //public ActionResult DeletePsacImage(VerificaionFormModel model)
        //{
        //    return BL.DeletePsacImage(model);
        //}

        //public ActionResult SaveP_MW_SCANNED_DOCUMENTsIC(VerificaionFormModel model)
        //{
        //    return BL.SaveP_MW_SCANNED_DOCUMENTsIC(model);
        //}

        //public ActionResult SaveP_MW_SCANNED_DOCUMENTsNIC(VerificaionFormModel model)
        //{
        //    return BL.SaveP_MW_SCANNED_DOCUMENTsNIC(model);
        //}

        //public ActionResult UpdateSPO(VerificaionFormModel model)
        //{
        //    return BL.UpdateSPO(model);
        //}

        //public ActionResult Summary(VerificaionFormModel model)
        //{
        //    BL.GetFormData(model);
        //    return View("Form",model);
        //}

        //public ActionResult SummarySubmit(VerificaionFormModel model)
        //{
        //    return BL.SummarySubmit(model);
        //}
    }
}