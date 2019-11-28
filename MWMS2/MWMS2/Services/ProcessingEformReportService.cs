using MWMS2.Areas.MWProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingEformReportService : BaseCommonService
    {
        public string SearchRPT_ESR_q =
            " SELECT map.UUID AS MAP_UUID, master.SUBMISSIONNO AS EFSS_SUBMISSION, map.DSN AS DSN, map.MW_SUBMISSION AS MW_SUBMISSION, "
            + " \r\n\t CASE WHEN MW01.ID IS NOT NULL THEN 'MW01' "
            + " \r\n\t WHEN MW02.ID IS NOT NULL THEN 'MW02' "
            + " \r\n\t WHEN MW03.ID IS NOT NULL THEN 'MW03' "
            + " \r\n\t WHEN MW04.ID IS NOT NULL THEN 'MW04' "
            + " \r\n\t WHEN MW05.ID IS NOT NULL THEN 'MW05' "
            + " \r\n\t WHEN MW06.ID IS NOT NULL THEN 'MW06' "
            + " \r\n\t WHEN MW07.ID IS NOT NULL THEN 'MW07' "
            + " \r\n\t WHEN MW08.ID IS NOT NULL THEN 'MW08' "
            + " \r\n\t WHEN MW09.ID IS NOT NULL THEN 'MW09' "
            + " \r\n\t WHEN MW10.ID IS NOT NULL THEN 'MW10' "
            + " \r\n\t WHEN MW11.ID IS NOT NULL THEN 'MW11' "
            + " \r\n\t WHEN MW12.ID IS NOT NULL THEN 'MW12' "
            + " \r\n\t WHEN MW31.ID IS NOT NULL THEN 'MW31' "
            + " \r\n\t WHEN MW32.ID IS NOT NULL THEN 'MW32' "
            + " \r\n\t WHEN MW33.ID IS NOT NULL THEN 'MW33' "
            + " \r\n\t END AS FORM_CODE, "
            + " \r\n\t to_char(map.CREATED_DATE, 'yyyy/mm/dd hh:mm') AS RECEIVED_DATE,"
            + " \r\n\t master.FOURPLUSTWO AS FOUR_TWO, "
            + " \r\n\t CASE WHEN map.DSN IS NOT NULL THEN 'ACK'  "
            + " \r\n\t WHEN map.DSN IS NULL THEN 'DR' "
            + " \r\n\t END AS STATUS "
            + " \r\n FROM P_EFSS_SUBMISSION_MAP map "
            + " \r\n\t LEFT JOIN P_EFSS_FORM_MASTER master ON master.ID = map.EFSS_ID "
            + " \r\n\t LEFT JOIN P_EFSS_SUBMISSION_MAP map ON master.ID = map.EFSS_ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW01 MW01 ON master.FORMCONTENTID = MW01.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW02 MW02 ON master.FORMCONTENTID = MW02.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW03 MW03 ON master.FORMCONTENTID = MW03.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW04 MW04 ON master.FORMCONTENTID = MW04.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW05 MW05 ON master.FORMCONTENTID = MW05.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW06 MW06 ON master.FORMCONTENTID = MW06.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW07 MW07 ON master.FORMCONTENTID = MW07.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW08 MW08 ON master.FORMCONTENTID = MW08.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW09 MW09 ON master.FORMCONTENTID = MW09.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW10 MW10 ON master.FORMCONTENTID = MW10.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW11 MW11 ON master.FORMCONTENTID = MW11.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW12 MW12 ON master.FORMCONTENTID = MW12.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW31 MW31 ON master.FORMCONTENTID = MW31.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW32 MW32 ON master.FORMCONTENTID = MW32.ID "
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW33 MW33 ON master.FORMCONTENTID = MW33.ID "
            + " \r\n WHERE 1=1 "
            ;

        public string SearchRPT_ESSR_q =
            " SELECT TO_CHAR(a.created_date, 'YYYY-MM') AS GROUP_DATE, "
            + " \r\n\t (SELECT count(uuid) FROM P_EFSS_Submission_Map b WHERE to_char(b.created_date, 'YYYY-MM') = to_char(a.created_date, 'YYYY-MM') AND b.status = 'ACK') AS COUNT_MW, "
            + " \r\n\t (SELECT count(uuid) FROM P_EFSS_Submission_Map b WHERE to_char(b.created_date, 'YYYY-MM') = to_char(a.created_date, 'YYYY-MM') AND b.status = 'DR') AS COUNT_DR, "
            + " \r\n\t count(a.created_date) AS COUNT_TOTAL, "
            + " \r\n\t 'Received' AS RECEIVED "
            + " \r\n FROM P_EFSS_Submission_Map a "
            + " \r\n WHERE 1=1 "
            ;

        public Fn10RPT_ESRSearchModel SearchRPT_ESR(Fn10RPT_ESRSearchModel model)
        {
            model.Query = SearchRPT_ESR_q;
            model.QueryWhere = SearchRPT_ESR_whereQ(model);

            model.Search();
            return model;
        }

        private string SearchRPT_ESR_whereQ(Fn10RPT_ESRSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Dsn))
            {
                whereQ += "\r\n\t AND map.DSN LIKE :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.Dsn.Trim() + "%");
            }
            if(model.ScanDateFrom.HasValue)
            {
                whereQ += "\r\n\t AND trunc(map.CREATED_DATE) >= :scanDateFrom";
                model.QueryParameters.Add("scanDateFrom", model.ScanDateFrom.Value);
            }
            if(model.ScanDateTo.HasValue)
            {
                whereQ += "\r\n\t AND trunc(map.CREATED_DATE) <= :scanDateTo";
                model.QueryParameters.Add("scanDateTo", model.ScanDateTo.Value);
            }
            return whereQ;
        }

        public string ExportRPT_ESR(Fn10RPT_ESRSearchModel model)
        {
            model.Query = SearchRPT_ESR_q;
            model.QueryWhere = SearchRPT_ESR_whereQ(model);

            return model.Export("eSyubmission_Report");
        }

        // ----------------------------------------------------------------------

        public Fn10RPT_ESSRSearchModel SearchRPT_ESSR(Fn10RPT_ESSRSearchModel model)
        {
            model.Query = SearchRPT_ESSR_q;
            model.QueryWhere = SearchRPT_ESSR_whereQ(model);

            model.Search();
            return model;
        }

        private string SearchRPT_ESSR_whereQ(Fn10RPT_ESSRSearchModel model)
        {
            string whereQ = "";
            if(model.ScanDateFrom.HasValue)
            {
                whereQ += "\r\n\t AND trunc(a.CREATED_DATE) >= :scanDateFrom";
                model.QueryParameters.Add("scanDateFrom", model.ScanDateFrom.Value);
            }
            if(model.ScanDateTo.HasValue)
            {
                whereQ += "\r\n\t AND trunc(a.CREATED_DATE) <= :scanDateTo";
                model.QueryParameters.Add("scanDateTo", model.ScanDateTo.Value);
            }
            whereQ += " \r\n\t GROUP BY to_char(a.created_date, 'YYYY-MM') ";
            //whereQ += " \r\n\t ORDER BY to_char(a.created_date, 'YYYY-MM') DESC ";
            return whereQ;
        }

        public string ExportRPT_ESSR(Fn10RPT_ESSRSearchModel model)
        {
            model.Query = SearchRPT_ESSR_q;
            model.QueryWhere = SearchRPT_ESSR_whereQ(model);

            return model.Export("eSyubmission_Summary_Report");
        }

    }
}