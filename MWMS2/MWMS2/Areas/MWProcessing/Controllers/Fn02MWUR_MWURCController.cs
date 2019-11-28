using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    // MWU Receive Counter
    public class Fn02MWUR_MWURCController : ValidationController
    {
        private MWPNewSubmissionBLService _MWPNewSubmissionBLService;
        protected MWPNewSubmissionBLService MWPNewSubmissionBLService
        {
            get { return _MWPNewSubmissionBLService ?? (_MWPNewSubmissionBLService = new MWPNewSubmissionBLService()); }
        }
        public ActionResult Index()
        {
            Fn02MWUR_MWURC_Model model = new Fn02MWUR_MWURC_Model();
            return View(model);
        }

        public ActionResult ValidFormNo(Fn02MWUR_MWURC_Model model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                return Json(new { });
            }
        }

        public ActionResult ValidationBeforeReceiveSubmission(Fn02MWUR_MWURC_Model model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                return Json(MWPNewSubmissionBLService.ValidationBeforeReceiveSubmission(model));
            }
        }

        public ActionResult ReceiveNewSubmission(Fn02MWUR_MWURC_Model model)
        {
            
            MWPNewSubmissionBLService.ReceiveNewSubmission(model);

            //ProcessingNewSubmissionService newSubmissionService = new ProcessingNewSubmissionService();
            //newSubmissionService.ReceiveNewSubmission(model);
            //return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            return View("Index", model);
        }


    }
}