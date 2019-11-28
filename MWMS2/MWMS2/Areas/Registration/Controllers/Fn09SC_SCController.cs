using MWMS2.Areas.Registration.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn09SC_SCController : Controller
    {

        private ScoringBLService _BL;
        protected ScoringBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ScoringBLService());
            }
        }

        // GET: Registration/Fn09SC_SC
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchWarningLetterRecord(Fn09SC_SCModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchWarningLetterRecord(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel(Fn09SC_SCModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Excel(model) });
        }

        public ActionResult Detail(Fn09SC_SCModel model)
        {
            return View(model);
        }

        public ActionResult Form(string uuid, string type, string fileRef)
        {
            return View("Form", BL.getScoringDetail(uuid, type, fileRef));
        }

        public ActionResult SearchCompanyInfo(Fn09SC_SCModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchCompanyInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult SearchIndInfo(Fn09SC_SCModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchIndInfo(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchOffenceList(Fn09SC_SCModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchOffenceList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchCourseList(Fn09SC_SCModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchCourseList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult UpdateCourseSource(UpdateScoreModel model)
        {
            return Json(BL.UpdateCourseSource(model));
        }

        public ActionResult DeleteCourse(string UUID)
        {
            return Json(BL.DeleteCourse(UUID));
        }

        public ActionResult CalculateTotalScore(string uuid)
        {
            return Json(BL.CalculateTotalScore(uuid));
        }

        public ActionResult AddNewCourse(Fn09SC_SCModel model)
        {
            // checking
            bool flag = BL.containsNegativeScore(model);
            if(flag)
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "Course score must be non-negative integer." } });
            }
            bool isNullOrEmpty = false;
            string message = null;
            for (int i = 0; i < model.CourseNameList.Count; i++)
            {
                if (string.IsNullOrEmpty(model.CourseNameList[i]))
                    message = "Course Name";
                if (string.IsNullOrEmpty(model.CourseIssueDateList[i]))
                    message = message == null ? "Course Issue Date" : message + ", Course Issue Date";
                if (string.IsNullOrEmpty(model.CourseScoreList[i]))
                    message = message == null ? "Course Score" : message + ", Course Score";
                if (message != null)
                    isNullOrEmpty = true;
            }

            if (isNullOrEmpty)
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { message + " should not be empty." } });
            else
                return Json(BL.AddNewCourse(model));
        }

    }
}