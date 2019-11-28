using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingSRDAOService: BaseDAOService
    {
        public class dsnCountRecord
        {
            public int DSNCount  { get; set; }
            public string RECEIVED_DATE { get; set; }
            public string COL { get; set; }
            public string CAT { get; set; }


        }


        public Dictionary<String, Dictionary<String, int>> getReceivedData(DateTime fromDate, DateTime toDate)
        {

            StringBuilder sql = new StringBuilder();

            sql.Append("\r\n" + "SELECT COUNT(DSN) as DSNCount, RECEIVED_DATE, COL || '' AS COL FROM( ");
            sql.Append("\r\n" + "SELECT DSN, TO_CHAR(RECEIVED_DATE, 'YYYYMM') AS RECEIVED_DATE, COL FROM(");
            sql.Append("\r\n" + " SELECT");
            sql.Append("\r\n" + " DISTINCT r.MW_DSN AS DSN");
            sql.Append("\r\n" + "	, P_GET_RECEIVED_DATE( r.MW_DSN ) AS RECEIVED_DATE");
            // sql.Append("\r\n" + " ,(");
            // sql.Append("\r\n" + " SELECT TO_DATE(TO_CHAR(MAX(f0.RECEIVED_DATE),
            // 'YYYYMM'),'YYYYMM') FROM MW_RECORD r0");
            // sql.Append("\r\n" + " INNER JOIN MW_FORM f0 ON f0.MW_RECORD_ID =
            // r0.UUID");
            // sql.Append("\r\n" + " WHERE r0.MW_DSN = r.MW_DSN AND r0.IS_DATA_ENTRY
            // = 'Y' AND f0.RECEIVED_DATE IS NOT NULL");
            // sql.Append("\r\n" + " AND r0.CREATED_DATE = (SELECT MIN(CREATED_DATE)
            // FROM MW_RECORD r1 WHERE r1.MW_DSN = r0.MW_DSN)");
            // sql.Append("\r\n" + " ) AS RECEIVED_DATE");

            sql.Append("\r\n" + "		,CASE");

            // WHEN n.CATEGORY_CODE = 'Enq' THEN 'ENQ'
            // WHEN n.CATEGORY_CODE = 'Com' THEN 'COM'
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_I' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"+ "					WHEN r.S_FORM_TYPE_CODE IN('MW01', 'MW11') THEN '1'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW02' THEN '2'");
            sql.Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN ('MW07', 'MW08', 'MW09', 'MW10', 'MW31', 'MW33') THEN '3'");
            sql.Append("\r\n" + "				ELSE '-1' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_II' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"
                    + "					WHEN r.S_FORM_TYPE_CODE IN('MW03', 'MW12') THEN '4'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW04' THEN '5'");
            sql.Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '6'");
            sql.Append("\r\n" + "				ELSE '-2' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_III' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05' THEN '7'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05(ITEM3.6)' THEN '3.6'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '8'");
            sql.Append("\r\n" + "				ELSE '-3' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_VS' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW06' THEN '9'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '10'");
            sql.Append("\r\n" + "				ELSE '-4' END");
            sql.Append("\r\n" + "		ELSE '-5' END AS COL");
            sql.Append("\r\n" + "		FROM P_MW_FORM f");
            sql.Append("\r\n" + "		INNER JOIN P_MW_RECORD r ON f.MW_RECORD_ID = r.UUID");
            sql.Append("\r\n"  + "		INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "		WHERE r.IS_DATA_ENTRY = 'Y'");
            sql.Append("\r\n"
                    + "		AND (n.CATEGORY_CODE = 'MW' OR n.CATEGORY_CODE = 'VS')");

            sql.Append("\r\n" + "		) WHERE 1=1");

            OracleParameter[] sqlParams =null;

            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "		AND RECEIVED_DATE > :fromDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate)};
            }

            if (fromDate == null && toDate != null) { 
                sql.Append("\r\n" + "		AND RECEIVED_DATE < :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("toDate",toDate)};

            }
            if (fromDate != null && toDate != null)
            {
                sql.Append("\r\n" + "		AND RECEIVED_DATE BETWEEN :fromDate AND :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }
                
            sql.Append("\r\n" + "	) GROUP BY RECEIVED_DATE, COL");
            sql.Append("\r\n" + "		ORDER BY RECEIVED_DATE DESC, COL");

            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();

            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    // COUNT(DSN), RECEIVED_DATE, COL
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String column = CommonUtil.getDisplay(result[i].COL);

                    

                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {   dateTable = data[receivedDate];
                    }
                    dateTable.Add(column, count);

                }
            return data;
        }


        public Dictionary<String, Dictionary<String, int>> getCountData(
        DateTime fromDate, DateTime toDate)
        {
            
            StringBuilder sql = new StringBuilder();

            sql
                    .Append("\r\n"
                            + "SELECT COUNT(DSN) AS DSNCount , RECEIVED_DATE AS RECEIVED_DATE , COL, CATEGORY_CODE  AS CAT FROM (");
            sql
                    .Append("\r\n"
                            + "SELECT DSN, TO_CHAR(RECEIVED_DATE, 'YYYYMM') AS RECEIVED_DATE, COL, CATEGORY_CODE FROM (");
            sql.Append("\r\n" + "		SELECT");
            sql
                    .Append("\r\n"
                            + "		DISTINCT n.CATEGORY_CODE AS CATEGORY_CODE, r.MW_DSN AS DSN");
            sql
                    .Append("\r\n"
                            + "		, P_GET_RECEIVED_DATE( r.MW_DSN ) AS RECEIVED_DATE");
            // sql.Append("\r\n" + " ,(");
            // sql.Append("\r\n" + " SELECT TO_DATE(TO_CHAR(MAX(f0.RECEIVED_DATE),
            // 'YYYYMM'),'YYYYMM') FROM MW_RECORD r0");
            // sql.Append("\r\n" + " INNER JOIN MW_FORM f0 ON f0.MW_RECORD_ID =
            // r0.UUID");
            // sql.Append("\r\n" + " WHERE r0.MW_DSN = r.MW_DSN AND r0.IS_DATA_ENTRY
            // = 'Y' AND f0.RECEIVED_DATE IS NOT NULL");
            // sql.Append("\r\n" + " AND r0.CREATED_DATE = (SELECT MIN(CREATED_DATE)
            // FROM MW_RECORD r1 WHERE r1.MW_DSN = r0.MW_DSN)");
            // sql.Append("\r\n" + " ) AS RECEIVED_DATE");

            sql.Append("\r\n" + "		,CASE");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_I' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"
                    + "					WHEN r.S_FORM_TYPE_CODE IN('MW01', 'MW11') THEN '1'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW02' THEN '1'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN ('MW07', 'MW08', 'MW09', 'MW10', 'MW31', 'MW33') THEN '1'");
            sql.Append("\r\n" + "				ELSE '-1' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_II' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"
                    + "					WHEN r.S_FORM_TYPE_CODE IN('MW03', 'MW12') THEN '1'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW04' THEN '1'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '1'");
            sql.Append("\r\n" + "				ELSE '-2' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_III' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05' THEN '1'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05(ITEM3.6)' THEN '1'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '1'");
            sql.Append("\r\n" + "				ELSE '-3' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_VS' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW06' THEN '1'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '1'");
            sql.Append("\r\n" + "				ELSE '-4' END");
            sql.Append("\r\n" + "		ELSE '-5' END AS COL");
            sql.Append("\r\n" + "		FROM P_MW_FORM f");
            sql.Append("\r\n"
                    + "		INNER JOIN P_MW_RECORD r ON f.MW_RECORD_ID = r.UUID");
            sql
                    .Append("\r\n"
                            + "		INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "		WHERE r.IS_DATA_ENTRY = 'Y'");
            sql.Append("\r\n" + "		) WHERE 1=1");


            OracleParameter[] sqlParams = null;

            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "		AND RECEIVED_DATE > :fromDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate)};
            }

            if (fromDate == null && toDate != null)
            {
                sql.Append("\r\n" + "		AND RECEIVED_DATE < :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("toDate",toDate)};

            }
            if (fromDate != null && toDate != null)
            {
                sql.Append("\r\n"
                        + "		AND RECEIVED_DATE BETWEEN :fromDate AND :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }

            sql.Append("\r\n" + "		) GROUP BY RECEIVED_DATE, COL, CATEGORY_CODE");
            sql
                    .Append("\r\n"
                            + "		ORDER BY RECEIVED_DATE DESC, COL, CATEGORY_CODE");


            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();

            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    // COUNT(DSN), RECEIVED_DATE, COL
                    int count = result[i].DSNCount;
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String column = CommonUtil.getDisplay(result[i].COL);
                    String categoryCode = CommonUtil.getDisplay(result[i].CAT);




                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(("1".Equals(column) ? categoryCode : column),
                            count);

                }
            return data;
        }

        public Dictionary<String, Dictionary<String, int>> getProcessingCountData(
            DateTime fromDate, DateTime toDate)
        {
            
            StringBuilder sql = new StringBuilder();
            sql.Append("\r\n" + "SELECT CNT AS DSNCount , TO_CHAR(D, 'YYYYMM') AS RECEIVED_DATE , COL || '' AS COL FROM(");
            sql.Append("\r\n" + "SELECT SUM(DATA.CNT) AS CNT, TO_DATE(DATELIST.D,'YYYYMM') AS D, DATA.COL AS COL FROM");
            sql.Append("\r\n" + "(");
            sql.Append("\r\n" + "SELECT DISTINCT TO_CHAR(:fromDate+ROWNUM-1, 'YYYYMM') AS D");
            sql.Append("\r\n" + "FROM P_MW_FORM WHERE :fromDate+ROWNUM-1 <= :toDate");
            sql.Append("\r\n" + ") DATELIST");
            sql.Append("\r\n" + "LEFT JOIN");
            sql.Append("\r\n" + "(");
            sql.Append("\r\n" + "	SELECT RECEIVE.DSN - COMPLETE.DSN AS CNT, RECEIVE.D AS D, RECEIVE.COL AS COL FROM (");
            sql.Append("\r\n" + "SELECT joiner.a AS D , joiner.b AS COL, NVL( joinee.DSN,0) AS DSN FROM");
            sql.Append("\r\n" + "(SELECT a, b.N AS b FROM (	SELECT DISTINCT TO_CHAR(RECEIVED_DATE, 'YYYYMM') AS a FROM P_MW_FORM),( (SELECT ROWNUM AS N FROM P_MW_RECORD WHERE ROWNUM <= 10)  b) WHERE a IS NOT NULL) joiner");
            sql.Append("\r\n" + "LEFT JOIN (");
            sql.Append("\r\n" + "	SELECT COUNT(DSN) AS DSN, TO_CHAR(RECEIVED_DATE, 'YYYYMM') AS D, COL FROM(");
            sql.Append("\r\n" + "			SELECT");
            sql.Append("\r\n" + "			DISTINCT r.MW_DSN AS DSN");
            sql.Append("\r\n" + "			, TO_DATE(TO_CHAR(P_GET_RECEIVED_DATE(r.MW_DSN), 'YYYYMM'),'YYYYMM') AS RECEIVED_DATE");
            sql.Append("\r\n" + "			,CASE");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_I' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW01', 'MW11') THEN '1'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW02' THEN '2'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN ('MW07', 'MW08', 'MW09', 'MW10', 'MW31', 'MW33') THEN '3'");
            sql.Append("\r\n" + "					ELSE '-1' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_II' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW03', 'MW12') THEN '4'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW04' THEN '5'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '6'");
            sql.Append("\r\n" + "					ELSE '-2' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_III' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW05' THEN '7'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05(ITEM3.6)' THEN '3.6'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '8'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '8'");
            sql.Append("\r\n" + "					ELSE '-3' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_VS' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW06' THEN '9'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '10'");
            sql.Append("\r\n" + "					ELSE '-4' END");
            sql.Append("\r\n" + "			ELSE '-5' END AS COL");
            sql.Append("\r\n" + "			FROM P_MW_FORM f");
            sql.Append("\r\n" + "			INNER JOIN P_MW_RECORD r ON f.MW_RECORD_ID = r.UUID");
            sql.Append("\r\n" + "			INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "			WHERE r.IS_DATA_ENTRY = 'Y'");
            sql.Append("\r\n" + "			AND (n.CATEGORY_CODE = 'MW' OR n.CATEGORY_CODE = 'VS')");
            sql.Append("\r\n" + "			) WHERE 1=1");
            sql.Append("\r\n" + "			GROUP BY RECEIVED_DATE, COL");
            sql.Append("\r\n" + "			ORDER BY RECEIVED_DATE DESC, COL");
            sql.Append("\r\n" + ") joinee ON joiner.a = joinee.D AND joiner.b = joinee.COL");
            sql.Append("\r\n" + "	) RECEIVE LEFT JOIN (");
            sql.Append("\r\n" + "SELECT joiner.a AS D , joiner.b AS COL, NVL( joinee.DSN,0) AS DSN FROM");
            sql.Append("\r\n" + "(SELECT a, b.N AS b FROM (	SELECT DISTINCT TO_CHAR(RECEIVED_DATE, 'YYYYMM') AS a FROM P_MW_FORM),( (SELECT ROWNUM AS N FROM P_MW_RECORD WHERE ROWNUM <= 10)  b) WHERE a IS NOT NULL) joiner");
            sql.Append("\r\n" + "LEFT JOIN (");
            sql.Append("\r\n" + "		SELECT COUNT(DSN) AS DSN, TO_CHAR(D, 'YYYYMM') AS D, COL FROM(");
            sql.Append("\r\n" + "			SELECT r.MW_DSN AS DSN");
            sql.Append("\r\n" + "			,TO_DATE(TO_CHAR(s.SPO_ENDORSEMENT_DATE, 'YYYYMM'), 'YYYYMM') AS D");
            sql.Append("\r\n" + "			,CASE");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_I' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW01', 'MW11') THEN '1'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW02' THEN '2'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN ('MW07', 'MW08', 'MW09', 'MW10', 'MW31', 'MW33') THEN '3'");
            sql.Append("\r\n" + "					ELSE '-1' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_II' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW03', 'MW12') THEN '4'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW04' THEN '5'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '6'");
            sql.Append("\r\n" + "					ELSE '-2' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_III' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW05' THEN '7'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05(ITEM3.6)' THEN '3.6'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '8'");
            sql.Append("\r\n" + "					ELSE '-3' END");
            sql.Append("\r\n" + "				WHEN r.CLASS_CODE = 'CLASS_VS' THEN");
            sql.Append("\r\n" + "					CASE");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE = 'MW06' THEN '9'");
            sql.Append("\r\n" + "						WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '10'");
            sql.Append("\r\n" + "					ELSE '-4' END");
            sql.Append("\r\n" + "			ELSE '-5' END AS COL");
            sql.Append("\r\n" + "			FROM P_MW_SUMMARY_MW_ITEM_CHECKLIST s");
            sql.Append("\r\n" + "			INNER JOIN P_MW_RECORD r ON r.UUID = s.MW_RECORD_ID");
            sql.Append("\r\n" + "			INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "			INNER JOIN P_MW_FORM f ON f.MW_RECORD_ID = r.UUID");
            sql.Append("\r\n" + "			WHERE r.IS_DATA_ENTRY = 'Y'");
            sql.Append("\r\n" + "			AND (n.CATEGORY_CODE = 'MW' OR n.CATEGORY_CODE = 'VS')");
            sql.Append("\r\n" + "			) WHERE 1=1");
            sql.Append("\r\n" + "			GROUP BY D, COL HAVING D IS NOT NULL");
            sql.Append("\r\n" + "			ORDER BY D DESC, COL");
            sql.Append("\r\n" + ") joinee ON joiner.a = joinee.D AND joiner.b = joinee.COL");
            sql.Append("\r\n" + "	) COMPLETE ON RECEIVE.D = COMPLETE.D AND RECEIVE.COL = COMPLETE.COL");
            sql.Append("\r\n" + "	WHERE RECEIVE.D IS NOT NULL AND RECEIVE.COL > 0");
            sql.Append("\r\n" + ") DATA ON DATELIST.D >= DATA.D");
            sql.Append("\r\n" + "GROUP BY DATELIST.D, DATA.COL");
            sql.Append("\r\n" + "ORDER BY DATELIST.D DESC, DATA.COL");
            sql.Append("\r\n" + ") WHERE 1=1");


            OracleParameter[] sqlParams = null;

            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "	AND D > :fromDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate)};
            }
                
            if (fromDate == null && toDate != null)
            {
                sql.Append("\r\n" + "	AND D < :toDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("toDate",toDate)};
            }
                
            if (fromDate != null && toDate != null)
            {
                sql.Append("\r\n" + "	AND D BETWEEN :fromDate AND :toDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }

            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();

            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String column = CommonUtil.getDisplay(result[i].COL);


                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(column, count);
                }
            return data;
        }

        public Dictionary<String, Dictionary<String, int>> getAckData(
            DateTime fromDate, DateTime toDate)
        {
            
            StringBuilder sql = new StringBuilder();

            sql.Append("\r\n" + "SELECT COUNT(DSN) AS DSNCount , D AS RECEIVED_DATE , (COL || T|| '' ) AS COL FROM(");
            sql.Append("\r\n" + "		SELECT r.MW_DSN AS DSN");
            sql.Append("\r\n"
                    + "		, TO_CHAR(s.SPO_ENDORSEMENT_DATE, 'YYYYMM') AS D");
            sql.Append("\r\n" + "		, s.RECOMMEDATION_APPLICATION AS T");
            sql.Append("\r\n" + "		,CASE");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_I' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"
                    + "					WHEN r.S_FORM_TYPE_CODE IN('MW01', 'MW11') THEN '1'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW02' THEN '2'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN ('MW07', 'MW08', 'MW09', 'MW10', 'MW31', 'MW33') THEN '3'");
            sql.Append("\r\n" + "				ELSE '-1' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_II' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n"
                    + "					WHEN r.S_FORM_TYPE_CODE IN('MW03', 'MW12') THEN '4'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW04' THEN '5'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '6'");
            sql.Append("\r\n" + "				ELSE '-2' END");
            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_III' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05' THEN '7'");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW05(ITEM3.6)' THEN '3.6'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '8'");
            sql.Append("\r\n" + "				ELSE '-3' END");

            sql.Append("\r\n" + "			WHEN r.CLASS_CODE = 'CLASS_VS' THEN");
            sql.Append("\r\n" + "				CASE");
            sql.Append("\r\n" + "					WHEN r.S_FORM_TYPE_CODE = 'MW06' THEN '9'");
            sql
                    .Append("\r\n"
                            + "					WHEN r.S_FORM_TYPE_CODE IN('MW07', 'MW10', 'MW33') THEN '10'");
            sql.Append("\r\n" + "				ELSE '-4' END");
            sql.Append("\r\n" + "		ELSE '-5' END AS COL");
            sql.Append("\r\n" + "		FROM P_MW_SUMMARY_MW_ITEM_CHECKLIST s");
            sql.Append("\r\n"
                    + "		INNER JOIN P_MW_RECORD r ON r.UUID = s.MW_RECORD_ID");
            sql
                    .Append("\r\n"
                            + "		INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql
                    .Append("\r\n"
                            + "		INNER JOIN P_MW_FORM f ON f.MW_RECORD_ID = r.UUID");
            sql.Append("\r\n"
                    + "		WHERE (n.CATEGORY_CODE = 'MW' OR n.CATEGORY_CODE = 'VS' ) ");


            OracleParameter[] sqlParams = null;

            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "		AND s.SPO_ENDORSEMENT_DATE > :fromDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate)};
            }

            if (fromDate == null && toDate != null)
            {
                sql.Append("\r\n" + "		AND s.SPO_ENDORSEMENT_DATE < :toDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("toDate",toDate)};
            }

            if (fromDate != null && toDate != null)
            {
                sql
                       .Append("\r\n"
                               + "		AND s.SPO_ENDORSEMENT_DATE BETWEEN :fromDate AND :toDate");

                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }



            sql.Append("\r\n" + "		) WHERE 1=1");
            sql.Append("\r\n" + "		GROUP BY D, COL, T HAVING D IS NOT NULL");
            sql.Append("\r\n" + "		ORDER BY D DESC, T, COL");


            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();
            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {

                    // COUNT(DSN), RECEIVED_DATE, COL
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String column = CommonUtil.getDisplay(result[i].COL);

                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(column, count);
                }
                return data;
        }


        public Dictionary<String, Dictionary<String, int>> getGeneralReceivedCountData(
        DateTime fromDate, DateTime toDate)

        {
          
            StringBuilder sql = new StringBuilder();

            sql.Append("\r\n" + "SELECT COUNT(UUID) AS DSNCount, RECEIVE_DATE AS RECEIVED_DATE, CATEGORY_CODE AS  COL FROM (");
            sql.Append("\r\n" + "	SELECT r.UUID AS UUID");
            sql.Append("\r\n" + "	, n.CATEGORY_CODE AS CATEGORY_CODE");
            sql.Append("\r\n" + "	, TO_CHAR(r.RECEIVE_DATE, 'YYYYMM') AS RECEIVE_DATE");
            sql.Append("\r\n" + "	FROM P_MW_GENERAL_RECORD r");
            sql.Append("\r\n" + "	INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "	WHERE n.CATEGORY_CODE IN ('Enq', 'Com') ");
            OracleParameter[] sqlParams = null;
            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "		AND r.RECEIVE_DATE > :fromDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate)};
            }

            if (fromDate == null && toDate != null)
            {
                sql.Append("\r\n" + "		AND r.RECEIVE_DATE < :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("toDate",toDate)};
            }

            if (fromDate != null && toDate != null)
            {
                sql.Append("\r\n" + "		AND r.RECEIVE_DATE BETWEEN :fromDate AND :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }
            sql.Append("\r\n" + "	)");
            sql.Append("\r\n" + "	GROUP BY  CATEGORY_CODE, RECEIVE_DATE");

            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();
            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String categoryCode = CommonUtil.getDisplay(result[i].COL);

                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(categoryCode, count);
                }
            return data;
        }

        public Dictionary<String, Dictionary<String, int>> getGeneralProcessingCountData(
            DateTime fromDate, DateTime toDate)
        {
          
            StringBuilder sql = new StringBuilder();

            sql.Append("\r\n" + "SELECT SUM(DATA.CNT) AS  DSNCount , DATELIST.D AS RECEIVED_DATE, DATA.CATEGORY_CODE AS COL FROM");
            sql.Append("\r\n" + "(");
            sql.Append("\r\n" + "	SELECT DISTINCT (TO_CHAR(RECEIVE_DATE, 'YYYYMM')) AS D");
            sql.Append("\r\n" + "	FROM P_MW_GENERAL_RECORD");
            sql.Append("\r\n" + "	WHERE RECEIVE_DATE IS NOT NULL");

            OracleParameter[] sqlParams = null;
            if (fromDate != null && toDate == null) {
                sql.Append("\r\n" + "		AND RECEIVE_DATE > :fromDate");
                sqlParams = new OracleParameter[] {new OracleParameter("fromDate",fromDate)};
            }

            if (fromDate == null && toDate != null) {
                sql.Append("\r\n" + "		AND RECEIVE_DATE < :toDate");
                sqlParams = new OracleParameter[] { new OracleParameter("toDate",toDate)};
            }

            if (fromDate != null && toDate != null){

                sql.Append("\r\n" + "		AND RECEIVE_DATE BETWEEN :fromDate AND :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }

            sql.Append("\r\n" + ") DATELIST");
            sql.Append("\r\n" + "LEFT JOIN");
            sql.Append("\r\n" + "(");
            sql
                    .Append("\r\n"
                            + "	SELECT COUNT(UUID) AS CNT, RECEIVE_DATE, CATEGORY_CODE FROM (");
            sql.Append("\r\n" + "		SELECT r.UUID AS UUID");
            sql.Append("\r\n" + "		, n.CATEGORY_CODE AS CATEGORY_CODE");
            sql.Append("\r\n" + "		, TO_CHAR(r.RECEIVE_DATE, 'YYYYMM') AS RECEIVE_DATE");
            sql.Append("\r\n" + "		FROM P_MW_GENERAL_RECORD r");
            sql.Append("\r\n" + "		INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "		LEFT JOIN P_MW_COMPLAINT_CHECKLIST c ON c.RECORD_ID = r.UUID");
            sql.Append("\r\n" + "		WHERE n.CATEGORY_CODE IN ('Enq', 'Com')");
            sql.Append("\r\n" + "		AND (c.FLOW_STATUS IS NULL OR c.FLOW_STATUS != 'DONE')");
            sql.Append("\r\n" + "	) GROUP BY  CATEGORY_CODE, RECEIVE_DATE");
            sql.Append("\r\n" + ") DATA ON DATELIST.D >= DATA.RECEIVE_DATE");
            sql.Append("\r\n" + "GROUP BY DATELIST.D, DATA.CATEGORY_CODE");
            sql.Append("\r\n" + "ORDER BY DATELIST.D DESC, DATA.CATEGORY_CODE");


            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();
            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String categoryCode = CommonUtil.getDisplay(result[i].COL);

                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(categoryCode, count);
                }
            return data;
        }


        public Dictionary<String, Dictionary<String, int>> getGeneralCompletedCountData(DateTime fromDate, DateTime toDate)
        {
           
            StringBuilder sql = new StringBuilder();

            sql.Append("\r\n" + "SELECT COUNT(UUID) AS DSNCount, RECEIVE_DATE AS RECEIVED_DATE, CATEGORY_CODE AS COL FROM (");
            sql.Append("\r\n" + "	SELECT r.UUID AS UUID");
            sql.Append("\r\n" + "	, n.CATEGORY_CODE AS CATEGORY_CODE");
            sql.Append("\r\n" + "	, TO_CHAR(r.RECEIVE_DATE, 'YYYYMM') AS RECEIVE_DATE");
            sql.Append("\r\n" + "	FROM P_MW_GENERAL_RECORD r");
            sql.Append("\r\n" + "	INNER JOIN P_MW_REFERENCE_NO n ON r.REFERENCE_NUMBER = n.UUID");
            sql.Append("\r\n" + "	LEFT JOIN P_MW_COMPLAINT_CHECKLIST c ON c.RECORD_ID = r.UUID");
            sql.Append("\r\n" + "	WHERE n.CATEGORY_CODE IN ('Enq', 'Com') ");
            sql.Append("\r\n" + "	AND c.FLOW_STATUS = 'DONE'");

            OracleParameter[] sqlParams = null;
            if (fromDate != null && toDate == null)
            {
                sql.Append("\r\n" + "		AND r.RECEIVE_DATE > :fromDate");
                sqlParams = new OracleParameter[] { new OracleParameter("fromDate", fromDate) };
            }

            if (fromDate == null && toDate != null)
            {   sql.Append("\r\n" + "		AND r.RECEIVE_DATE < :toDate");
                sqlParams = new OracleParameter[] { new OracleParameter("toDate", toDate) };
            }
            if (fromDate != null && toDate != null)
            {
                sql.Append("\r\n" + "		AND r.RECEIVE_DATE BETWEEN :fromDate AND :toDate");
                sqlParams = new OracleParameter[] {
                    new OracleParameter("fromDate",fromDate),
                    new OracleParameter("toDate",toDate)};
            }
            sql.Append("\r\n" + "	)");
            sql.Append("\r\n" + "	GROUP BY  CATEGORY_CODE, RECEIVE_DATE");

            Dictionary<String, Dictionary<String, int>> data = new Dictionary<String, Dictionary<String, int>>();
            List<dsnCountRecord> result = GetObjectData<dsnCountRecord>(sql.ToString(), sqlParams).ToList();
            if (result != null)
                for (int i = 0; i < result.Count; i++)
                {
                    int count = result[i].DSNCount;// Convert.ToInt32(result[i][0]);
                    String receivedDate = CommonUtil.getDisplay(result[i].RECEIVED_DATE);
                    String categoryCode = CommonUtil.getDisplay(result[i].COL);

                    Dictionary<String, int> dateTable;
                    if (!data.ContainsKey(receivedDate))
                    {
                        dateTable = new Dictionary<String, int>();
                        data.Add(receivedDate, dateTable);
                    }
                    else
                    {
                        dateTable = data[receivedDate];
                    }
                    dateTable.Add(categoryCode, count);
                }
            return data;
            
        }


    }
}