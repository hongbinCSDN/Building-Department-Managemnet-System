using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn02MWUR_EAController : Controller
    {
        public ActionResult Index()
        {
            Fn02MWUR_EASearchModel model = new Fn02MWUR_EASearchModel();

            return View(model);
        }

        public ActionResult Search(Fn02MWUR_EASearchModel model)
        {
            ProcessingEformService ss = new ProcessingEformService();

            return Content(JsonConvert.SerializeObject(ss.SearchMWUR_EA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //public ActionResult RedirectFromEfss(Fn01LM_AckSearchModel model)
        //{
        //    return RedirectToAction("RedirectFromEfss", "Fn01LM_Ack", model);
        //}

        public ActionResult ReceiveNewSubmission(string EFSS_ID, string FORM_CODE, string MW_SUBMISSIONNO, string STATUS)
        {
            ProcessingEformService eform = new ProcessingEformService();

            if (STATUS.Equals(ProcessingConstant.EFSS_STATUS_ACK))
            {
                Fn02MWUR_EASearchModel eformModel = new Fn02MWUR_EASearchModel();

                MWPNewSubmissionBLService mWPNewSubmissionBLService = new MWPNewSubmissionBLService();

                Fn02MWUR_MWURC_Model model = new Fn02MWUR_MWURC_Model();
                model.DocType = "MWForm";
                model.FormNo = FORM_CODE;
                model.VsForMW01_MW03 = false;
                // model.TypeOfRefNo = null;
                model.InputRefNo = MW_SUBMISSIONNO;

                // 1. validation
                if (!FORM_CODE.Equals("MW01") && !FORM_CODE.Equals("MW03") && !FORM_CODE.Equals("MW05") && !FORM_CODE.Equals("MW32"))
                {
                    ValidationResult validate = mWPNewSubmissionBLService.Validate_InputRefNo(FORM_CODE, "MW Submission No.", model);
                    if (validate != null) // fail
                    {
                        eformModel.ErrMsg = validate.ToString();
                        return Content(JsonConvert.SerializeObject(eformModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
                    }
                }

                //2.generate #s
                DisplaySubmissionObj obj = new DisplaySubmissionObj();
                obj = mWPNewSubmissionBLService.ReceiveNewMwSubmission(model);

                // 3. add to submission map
                P_EFSS_SUBMISSION_MAP map = new P_EFSS_SUBMISSION_MAP();
                map = eform.createEfssSubmissionMap(EFSS_ID, obj, ProcessingConstant.EFSS_STATUS_ACK);

                // 4. redirect to ACK letter input interface
                //Fn01LM_AckSearchModel ackModel = new Fn01LM_AckSearchModel();
                //ackModel = eform.createAckModel(EFSS_ID, FORM_CODE, map.DSN, map.MW_SUBMISSION);
                eformModel.ACK_DSN = map.DSN;
                eformModel.ACK_EFSS_ID = EFSS_ID;
                eformModel.ACK_FORM_CODE = FORM_CODE;
                eformModel.ACK_MW_SUBMISSION = map.MW_SUBMISSION;
                eformModel.ACKorDR = ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_ACK;

                return Content(JsonConvert.SerializeObject(eformModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
                //TempData["model"] = ackModel;
                //return RedirectToAction("RedirectFromEfss", "Fn01LM_Ack");
            }
            else if(STATUS.Equals(ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN))
            {
                // 1. update submission map
                P_EFSS_SUBMISSION_MAP map = new P_EFSS_SUBMISSION_MAP();
                DisplaySubmissionObj obj = new DisplaySubmissionObj();
                map = eform.createEfssSubmissionMap(EFSS_ID, obj, ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN);

                // 2. add to direct return tables
                eform.directReturn(EFSS_ID, FORM_CODE);
                Fn02MWUR_EASearchModel eformModel = new Fn02MWUR_EASearchModel();
                eformModel.ACKorDR = ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN;

                return Content(JsonConvert.SerializeObject(eformModel, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }
            return Index();
        }
    }
}