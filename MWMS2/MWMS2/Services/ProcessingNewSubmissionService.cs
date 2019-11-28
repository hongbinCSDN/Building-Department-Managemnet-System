using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;

namespace MWMS2.Services
{
    public class ProcessingNewSubmissionService
    {
        String SearchMod_q = "select '0' as UUID, '1' as REC_DATE, '2' as TIME, '3' as DSN, '4' as REF_NO, '5' as FORM_NO"
                + "\r\n\t" + "from dual ";

        public Fn02MWUR_MWURC_Model ReceiveNewSubmission(Fn02MWUR_MWURC_Model model)
        {
            //model.Query = SearchMod_q;
            //model.QueryWhere = SearchMOD_whereQ(model);
            //model.Search(); 

            //List<P_MW_DSN> MwDsnList = new List<P_MW_DSN>();
            P_MW_DSN P_MW_DSN = new P_MW_DSN();
            P_MW_DSN.DSN = "D000";
            P_MW_DSN.FORM_CODE = "MW01";

            model.MwDsnList.Add(P_MW_DSN);
            model.MwDsnList.Add(P_MW_DSN);

            return model;
        }

        public P_MW_DSN GetNewDsnByAutoGen()
        {
            P_MW_DSN MwDsn = new P_MW_DSN();



            return MwDsn;
        }

        public String synchronizedSaveNewRecord(P_S_MW_NUMBER P_S_MW_NUMBER, String prefix)
        {
            string newNo = "";

            return newNo;
        }

        public String getMaxNumberByPrefix(String prefix)
        {
            String result = "";

            return result;
        }
    }
}