using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.WarningLetter.Service.BL;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services;
using MWMS2.Utility;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.Admin.Controllers
{
    public class WLM_OffenceController : ValidationController
    {

        private WMLBLService _BL;
        protected WMLBLService BL
        {
            get
            {
                return _BL ?? (_BL = new WMLBLService());
            }
        }
        // GET: Admin/WLM_Offence
        public ActionResult Index()
        {
            
            return View(new WLM_OffenceModel());
        }
        public ActionResult DeleteWOT(string uuid)
        {
            
            return Content(JsonConvert.SerializeObject(BL.DeleteOffenseName(uuid), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult SearchAllOffence(WLM_OffenceModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchAllOffence(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult UpdateOffenseName(ScoreListModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return Json(BL.UpdateOffenseName(model));
        }

        public ActionResult AddNewOffence(WLM_OffenceModel model)
        {
            bool isEmptyOrNull = false;
            for (int i = 0; i < model.DESCRIPTION_ENG.Count; i++)
            {
                if (string.IsNullOrEmpty(model.DESCRIPTION_ENG[i]) || string.IsNullOrEmpty(model.TYPE[i]))
                {
                    isEmptyOrNull = true;
                    break;
                }

            }

            if (isEmptyOrNull)
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Data = "Offense and Type should not be empty." });
            else if (BL.CheckExistOffense(model))
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Data = "The offence already exists." });
            else
                return Json(BL.AddNewOffense(model));
        }

        public ActionResult Detail(string uuid,string offense_name,string type)
        {
            ScoreListModel model = new ScoreListModel() { Offense_Id = uuid,Offense_Name = offense_name,Type = type };
            return View(model);
        }
       

        public ActionResult SearchScoreList(ScoreListModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.SearchDetailScoreList(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult AddNewScore(ScoreListModel model)
        {
            bool isEmptyOrNull = false;
            for(int i=0;i<model.Effect_Date.Count;i++)
            {
                if (string.IsNullOrEmpty(model.Effect_Date[i]) || string.IsNullOrEmpty(model.Score[i]))
                {
                    isEmptyOrNull = true;
                    break;
                }
                 
            }
            if(isEmptyOrNull)
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "Effect Date or Score should not be empty." } });
            else if(BL.CheckIsExistEffectDate(model))
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The effect date already exists." } });
            else
                return Json(BL.AddNewScore(model));
        }


    }
}