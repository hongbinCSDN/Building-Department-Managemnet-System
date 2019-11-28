using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingElementaryCheckingDAOService
    {
        public Fn11EC_Model AjaxRegInfo(Fn11EC_Model model)
        {
            model.Query = ""
            + "\r\n\t" + "SELECT DISTINCT UUID, CODE  "
            + "\r\n\t" + ", CERTIFICATION_NO    "
            + "\r\n\t" + ", SURNAME, GIVEN_NAME "
            + "\r\n\t" + ", CHINESE_NAME        "
            + "\r\n\t" + ", AS_SURNAME          "
            + "\r\n\t" + ", AS_GIVEN_NAME       "
            + "\r\n\t" + ", AS_CHINESE_NAME     "
            + "\r\n\t" + ", EXPIRY_DATE         "
            + "\r\n\t" + "FROM V_CRM_INFO       ";
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                string whereQ = "\r\n\t" + "WHERE CERTIFICATION_NO = :RegNo";
                model.QueryParameters.Add("RegNo", model.RegNo);
                model.QueryWhere = whereQ;
            }
            model.Search();
            return model;
        }

    }
}