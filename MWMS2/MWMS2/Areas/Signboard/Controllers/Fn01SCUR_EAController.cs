using System;
using System.Web.Mvc;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn01SCUR_EAController : Controller
    {
        public ActionResult Index(Fn01SCUR_EASearchModel model)
        {
            return View(model);
        }

        public ActionResult Search(Fn01SCUR_EASearchModel model)
        {
            SignboardEformService ss = new SignboardEformService();

            return Content(JsonConvert.SerializeObject(ss.SearchSCUR_EA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult ReceiveForm(string EFSS_ID, string FORM_CODE)
        {
            SCUR_Models scurModel = new SCUR_Models();
            scurModel.FormCode = FORM_CODE;

            SignboardRAService ras = new SignboardRAService();

            // 1. generate SMM submission #, DSN #
            scurModel = ras.GenerateSubmissionNumber(scurModel);

            SignboardEformService eform = new SignboardEformService();

            // 2. add SMM submission #, DSN # to B_EFSS_SUBMISSION_MAP table
            B_EFSS_SUBMISSION_MAP map = eform.createEfssSubmissionMap(EFSS_ID, scurModel);

            // 3. data entry
            if (FORM_CODE.Equals(SignboardConstant.FORM_CODE_SC01))
            {
                eform.dataEntry_SC01(EFSS_ID, scurModel);
            }
            else if (FORM_CODE.Equals(SignboardConstant.FORM_CODE_SC02))
            {
                eform.dataEntry_SC02(EFSS_ID, scurModel);
            }
            else if (FORM_CODE.Equals(SignboardConstant.FORM_CODE_SC03))
            {
                eform.dataEntry_SC03(EFSS_ID, scurModel);
            }

            Fn01SCUR_EASearchModel efssModel = new Fn01SCUR_EASearchModel();
            //return View("Index", efssModel);
            //return RedirectToAction("Index", efssModel);

            //return RedirectToAction("Index");
            return Content(JsonConvert.SerializeObject(efssModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult ReceiveCompleteForm(string EFSS_ID, string FORM_CODE, string INPUT_SUBMISSION_NO)
        {
            SCUR_Models scurModel = new SCUR_Models();
            scurModel.FormCode = FORM_CODE;
            scurModel.SubmissionNo = INPUT_SUBMISSION_NO;

            SignboardRAService ras = new SignboardRAService();
            Fn01SCUR_EASearchModel efssModel = new Fn01SCUR_EASearchModel();

            // 1. generate SMM submission #, DSN #
            scurModel = ras.ReceiveCompleteForm(scurModel);
            if (scurModel.ErrMsg != "Received")
            {
                efssModel.ErrMsg = scurModel.ErrMsg;
                //return View("Index", efssModel);
                return Content(JsonConvert.SerializeObject(scurModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }
            scurModel.FormCode = FORM_CODE;
            scurModel.SubmissionNo = INPUT_SUBMISSION_NO;

            SignboardEformService eform = new SignboardEformService();

            // 2. add SMM submission #, DSN # to B_EFSS_SUBMISSION_MAP table
            B_EFSS_SUBMISSION_MAP map = eform.createEfssSubmissionMap(EFSS_ID, scurModel);

            // 3. data entry
            if (FORM_CODE.Equals(SignboardConstant.FORM_CODE_SC01C))
            {
                eform.dataEntry_SC01C(EFSS_ID, scurModel);
            }
            else if (FORM_CODE.Equals(SignboardConstant.FORM_CODE_SC02C))
            {
                eform.dataEntry_SC02C(EFSS_ID, scurModel);
            }

            return Content(JsonConvert.SerializeObject(efssModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn01SCUR_EASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardEformService eform = new SignboardEformService();
            return Json(new { key = eform.ExportSCUR_EA(model) });
        }
    }
}