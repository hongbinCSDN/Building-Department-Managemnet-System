using MWMS2.Areas.MWProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingErrorService
    {
        public string Search_q =
            " SELECT t1.mw_no, t1.dsn, t1.FORM_NO FROM P_MW_ACK_LETTER t1"
            + " \r\n\t INNER JOIN  "
            + " \r\n\t (SELECT   MW_NO  FROM P_MW_ACK_LETTER"
            + " \r\n\t WHERE form_no IN ('MW02','MW04')"
            + " \r\n\t GROUP BY MW_NO  "
            + " \r\n HAVING count(mw_no) > 1 ) t2 on t2.mw_no = t1.mw_no "
            + " \r\n WHERE 1=1   "
            ;
        public Fn10RPT_ERModel Search(Fn10RPT_ERModel model)
        {
            model.Query = Search_q;
         //   model.QueryWhere = Search_whereQ(model);

            model.Search();
            return model;
        }
        public string Export(Fn10RPT_ERModel model)
        {
            model.Query = Search_q;
            return model.Export("ExportData");
        }
    }
}