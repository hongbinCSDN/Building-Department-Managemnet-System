using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.WarningLetter.Service.DAO;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.WarningLetter.Service.BL
{
    public class WMLBLService
    {
        private WMLDAOService _DA;
        protected WMLDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new WMLDAOService());
            }
        }


        public WLM_OffenceModel SearchAllOffence(WLM_OffenceModel model)
        {
            return DA.SearchAllOffence(model);
        }

        public bool CheckExistOffense(WLM_OffenceModel model)
        {
            if (DA.CheckExistOffense(model) != null)
                return true;
            else
                return false;
        }

        public ServiceResult UpdateOffenseName(ScoreListModel model)
        {
            return DA.UpdateOffenseName(model);
        }
        public ServiceResult DeleteOffenseName(string id)
        {
            return DA.DeleteOffenseName(id);
        }

        public ServiceResult AddNewOffense(WLM_OffenceModel model)
        {
            return DA.AddNewOffense(model);
        }

        public ScoreListModel SearchDetailScoreList(ScoreListModel model)
        {
            return DA.SearchDetailScoreList(model);
        }

        public bool CheckIsExistEffectDate(ScoreListModel model)
        {
            if (DA.CheckIsExistEffectDate(model) != null)
                return true;
            else
                return false;
        }

        public ServiceResult AddNewScore(ScoreListModel model)
        {
            return DA.AddNewScore(model);
        }

    }
}