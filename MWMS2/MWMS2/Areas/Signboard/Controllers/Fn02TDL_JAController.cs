using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Services.Signborad.SignboardServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn02TDL_JAController : Controller
    {
        public ActionResult Index()
        {
            SignboardJobAssignmentService SignboardJobAssignmentService = new SignboardJobAssignmentService();
            ViewBag.SUBMIT_MODE = SignboardConstant.SUBMIT_MODE;
            ViewBag.SAVE_MODE = SignboardConstant.SAVE_MODE;

            return View(SignboardJobAssignmentService.LoadSpoAssignment());
        }

        [HttpPost]
        public ActionResult Save(Fn02TDL_JADisplayModel model)
        {
            // check security right

            SignboardJobAssignmentService SignboardJobAssignmentService = new SignboardJobAssignmentService();
            SignboardJobAssignmentService.PopulateSaveJobList(model);
            ViewBag.SUBMIT_MODE = SignboardConstant.SUBMIT_MODE;
            ViewBag.SAVE_MODE = SignboardConstant.SAVE_MODE;
            return View("Index", SignboardJobAssignmentService.LoadSpoAssignment());
            //return View(SignboardJobAssignmentService.LoadSpoAssignment());
        }
    }
}