using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.MWProcessing.Models;

namespace MWMS2.Services
{
    public class ProcessingModService
    {
        String SearchMod_q = "select mod.uuid, mod.REFERENCE_NO, mod.FORM_NO, mod.DSN, mod.RECEIVED_DATE, mod.HANDING_STAFF "
                + "\r\n\t" + "from p_mw_modification mod ";

        public Fn01LM_Search_MOD_Model SearchMOD(Fn01LM_Search_MOD_Model model)
        {
            model.Query = SearchMod_q;
            model.Sort = "REFERENCE_NO";
            //model.QueryWhere = SearchMOD_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchMOD_whereQ(Fn01LM_Search_MOD_Model model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.ModRefNo))
            {
                whereQ += "\r\n\t" + "AND ind.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.ModRefNo.Trim().ToUpper() + "%");
            }
            /*
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            */
            return whereQ;
        }
    }
}