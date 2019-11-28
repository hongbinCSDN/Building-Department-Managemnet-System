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
    public class ProcessingDirectReturnDAOService
    {
        #region Search Sql
        private const string SearchDrSqlOfToday = @"SELECT T1.UUID,
                                                   T1.DSN,
                                                   T1.FORM_TYPE,
                                                   T1.CONTRACTOR_REG_NO,
                                                   TO_CHAR(T1.RECEIVED_DATE,'dd/MM/yyyy') AS RECEIVED_DATE,
                                                   T1.HANDING_STAFF_1,
                                                   T1.HANDING_STAFF_2,
                                                   T1.HANDING_STAFF_3
                                            FROM   P_MW_DIRECT_RETURN T1
                                            WHERE  1 = 1 ";

        private const string SearchDrSql = @"SELECT    T1.UUID,
                                                       T1.DSN,
                                                       T1.FORM_TYPE,
                                                       T1.CONTRACTOR_REG_NO,
                                                       TO_CHAR(T1.RECEIVED_DATE,'dd/MM/yyyy') AS RECEIVED_DATE,
                                                       T2.Irregulartites,
                                                       T1.HANDING_STAFF_1,
                                                       T1.HANDING_STAFF_2,
                                                       T1.HANDING_STAFF_3
                                                FROM   P_MW_DIRECT_RETURN T1
                                                       LEFT JOIN (SELECT C1.Master_ID,
                                                                         Listagg(C1.Code, ',')WITHIN GROUP(ORDER BY C1.Code)  AS Irregulartites 
                                                                  FROM   (SELECT DRI.Master_ID,
                                                                                 SV.Code
                                                                          FROM   P_MW_DIRECT_RETURN_IRREGULARITIES DRI
                                                                                 RIGHT JOIN P_S_System_Value SV
                                                                                         ON DRI.SV_IRREGULARITIES_ID = SV.UUID
                                                                                 JOIN P_S_System_Type ST
                                                                                   ON SV.System_type_ID = ST.UUID
                                                                                      AND ST.TYPE = 'Irregularities'
                                                                          WHERE  DRI.Is_Checked = 'True') C1
                                                                  GROUP  BY C1.Master_ID) T2
                                                              ON T1.UUID = T2.Master_ID
                                                WHERE  1 = 1 ";

        private const string StatisticsSqlV1C = @"SELECT SV.Code,
                                                               SV.Description,
                                                               ( CASE DRI.is_checked
                                                                   WHEN 'True' THEN 1
                                                                   ELSE 0
                                                                 END ) AS IsTrue,
                                                               DRI.Created_Date
                                                        FROM   P_MW_DIRECT_RETURN_IRREGULARITIES DRI
                                                               RIGHT JOIN P_S_System_Value SV
                                                                       ON DRI.SV_IRREGULARITIES_ID = SV.UUID
                                                               JOIN P_S_System_Type ST
                                                                 ON SV.System_type_ID = ST.UUID
                                                                    AND ST.TYPE = 'Irregularities'
                                                        WHERE  1 = 1 ";

        private const string StatisticsSqlV1P = @"SELECT TT1.Code,
                                                               TT1.Description As Irregularities,
                                                               TT1.Total AS June
                                                        FROM  (SELECT To_char(T1.Created_Date, 'mm/yyyy') Created_Date,
                                                                      T1.Code,
                                                                      T1.Description,
                                                                      Sum(T1.IsTrue)                      AS Total
                                                               FROM   ({0})T1
                                                               GROUP  BY T1.Code,
                                                                         T1.Description,
                                                                         To_char(T1.Created_Date, 'mm/yyyy')
                                                               HAVING To_char(T1.Created_Date, 'mm/yyyy') = '06/2019')TT1 ";

        private const string StatisticsSqlV2C = @"SELECT SV.Code,
                                                    ( CASE DRI.is_checked
                                                        WHEN 'True' THEN 1
                                                        ELSE 0
                                                      END ) AS IsTrue,
                                                    DRI.Created_Date
                                                    FROM   P_MW_DIRECT_RETURN_IRREGULARITIES DRI
                                                           RIGHT JOIN P_S_System_Value SV
                                                                   ON DRI.SV_IRREGULARITIES_ID = SV.UUID
                                                           JOIN P_S_System_Type ST
                                                             ON SV.System_type_ID = ST.UUID
                                                                AND ST.TYPE = 'Irregularities'
                                                    WHERE  1 = 1 ";

        /* private const string StatisticsSqlV2P = @" SELECT *
                                                             FROM   (SELECT *
                                                                     FROM  (SELECT To_char(T1.Created_Date, 'mm/yyyy') Created_Date,
                                                                                   T1.Code,
                                                                                   Sum(T1.IsTrue)                      AS Total
                                                                            FROM   ({0})T1
                                                                            GROUP  BY T1.Code,
                                                                                      To_char(T1.Created_Date, 'mm/yyyy'))TT1)TTT1
                                                             PIVOT (Sum(Total) For Code in('A','B','C','D','E','F','G','H','I','J','K','L','M','N'))";*/
        #endregion

        #region Search Criteria

        private string SearchDrCriteria(Fn01LM_DRSearchModel model)
        {
            string whereQ = "";

            if (model.IsToday)
            {
                whereQ += "\r\n\t" + "And TO_CHAR(T1.RECEIVED_DATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')";
                return whereQ;
            }

            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                //whereQ += "\r\n\t" + "AND T1.DSN LIKE :DSN";
                //model.QueryParameters.Add("DSN", "%" + model.DSN + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T1.DSN,:DSN,'i') ";
                model.QueryParameters.Add("DSN", model.DSN);
            }
            if (!string.IsNullOrWhiteSpace(model.FORM_TYPE))
            {
                //whereQ += "\r\n\t" + "AND T1.FORM_TYPE LIKE :FORM_TYPE";
                //model.QueryParameters.Add("FORM_TYPE", "%" + model.FORM_TYPE + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T1.FORM_TYPE,:FORM_TYPE,'i') ";
                model.QueryParameters.Add("FORM_TYPE",model.FORM_TYPE);
            }
            if (!string.IsNullOrWhiteSpace(model.CONTRACTOR_REG_NO))
            {
                //whereQ += "\r\n\t" + "AND T1.CONTRACTOR_REG_NO LIKE :CONTRACTOR_REG_NO";
                //model.QueryParameters.Add("CONTRACTOR_REG_NO", "%" + model.CONTRACTOR_REG_NO + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T1.CONTRACTOR_REG_NO,:CONTRACTOR_REG_NO,'i') ";
                model.QueryParameters.Add("CONTRACTOR_REG_NO", model.CONTRACTOR_REG_NO);
            }
            if (model.RECEIVED_DATE != null)
            {
                whereQ += "\r\n\t" + "And TO_CHAR(T1.RECEIVED_DATE,'YYYY-MM-DD')=TO_CHAR(:RECEIVED_DATE,'YYYY-MM-DD')";
                model.QueryParameters.Add("RECEIVED_DATE", model.RECEIVED_DATE);
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_1))
            {
                //whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_1 LIKE :HANDING_STAFF_1";
                //model.QueryParameters.Add("HANDING_STAFF_1", "%" + model.HANDING_STAFF_1 + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T1.HANDING_STAFF_1,:HANDING_STAFF_1,'i') ";
                model.QueryParameters.Add("HANDING_STAFF_1",model.HANDING_STAFF_1);
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_2))
            {
                //whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_2 LIKE :HANDING_STAFF_2";
                //model.QueryParameters.Add("HANDING_STAFF_2", "%" + model.HANDING_STAFF_2 + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T1.HANDING_STAFF_2,:HANDING_STAFF_2,'i') ";
                model.QueryParameters.Add("HANDING_STAFF_2", model.HANDING_STAFF_2 );
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_3))
            {
                //whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_3 LIKE :HANDING_STAFF_3";
                //model.QueryParameters.Add("HANDING_STAFF_3", "%" + model.HANDING_STAFF_3 + "%");

                whereQ += "\r\n\t" + "AND regexp_like(T1.HANDING_STAFF_3,:HANDING_STAFF_3,'i') ";
                model.QueryParameters.Add("HANDING_STAFF_3",model.HANDING_STAFF_3 );
            }

            string Irregularities = "";

            foreach (Irregularities item in model.IrregularitiesList)
            {
                if (item.IsChecked)
                {
                    Irregularities += item.Code + ",";
                }
            }

            if (!string.IsNullOrWhiteSpace(Irregularities))
            {
                //whereQ += "\r\n\t" + "AND T2.IRREGULARTITES LIKE :IRREGULARTITES";
                //model.QueryParameters.Add("IRREGULARTITES", "%" + Irregularities.Substring(0, Irregularities.Length - 1) + "%");
                whereQ += "\r\n\t" + "AND regexp_like(T2.IRREGULARTITES,:IRREGULARTITES,'i') ";
                model.QueryParameters.Add("IRREGULARTITES", Irregularities.Substring(0, Irregularities.Length - 1) );
            }


            return whereQ;
        }

        private string StatisticsCriteria(Fn01LM_DRStatModel model, bool isV2 = false)
        {

            string whereQ = StatisticsSqlV1C;
            if (isV2)
            {
                whereQ = StatisticsSqlV2C;
            }
            if (model.DateFrom != null)
            {
                whereQ += "\r\n\t" + "AND DRI.Created_Date" + " >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.DateFrom);
            }
            if (model.DateTo != null)
            {
                whereQ += "\r\n\t" + "AND DRI.Created_Date" + " <= :DateTo";
                model.QueryParameters.Add("DateTo", model.DateTo);
            }
            return whereQ;
        }
        #endregion

        #region Search

        public DisplayGrid SearchDr(Fn01LM_DRSearchModel model)
        {
            DisplayGrid dg = new DisplayGrid();
            dg.Query = model.IsMaintenance ? SearchDrSql : SearchDrSqlOfToday;
            dg.QueryWhere = SearchDrCriteria(model);
            // Begin Add by chester
            dg.QueryParameters = model.QueryParameters;
            // End Add by chester
            dg.Search();
            return dg;
        }

        public Fn01LM_DRSaveModel SearchDrDetail(string sUUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                Fn01LM_DRSaveModel record = new Fn01LM_DRSaveModel();
                record.P_MW_DIRECT_RETURN = new P_MW_DIRECT_RETURN();

                P_MW_DIRECT_RETURN model = db.P_MW_DIRECT_RETURN.Where(d => d.UUID == sUUID).FirstOrDefault();

                if (model == null)
                {
                    return null;
                }

                //record.P_MW_DIRECT_RETURN.DSN = model.DSN;
                //record.P_MW_DIRECT_RETURN.FORM_TYPE = model.FORM_TYPE;
                //record.P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO = model.CONTRACTOR_REG_NO;
                //record.P_MW_DIRECT_RETURN.RECEIVED_DATE = model.RECEIVED_DATE;
                //record.P_MW_DIRECT_RETURN.HANDING_STAFF_1 = model.HANDING_STAFF_1;
                //record.P_MW_DIRECT_RETURN.HANDING_STAFF_2 = model.HANDING_STAFF_2;
                //record.P_MW_DIRECT_RETURN.HANDING_STAFF_3 = model.HANDING_STAFF_3;
                //record.P_MW_DIRECT_RETURN.ADDRESS = model.ADDRESS;

                record.P_MW_DIRECT_RETURN = model;

                string sqlRegName = string.Format(@"SELECT T3.GIVEN_NAME_ON_ID
                                                           || ' '
                                                           || T3.SURNAME AS FUL_NAME
                                                    FROM   C_IND_CERTIFICATE T1
                                                           INNER JOIN C_IND_APPLICATION T2
                                                                   ON T2.UUID = T1.MASTER_ID
                                                                      AND T1.CERTIFICATION_NO = '{0}'
                                                           INNER JOIN C_APPLICANT T3
                                                                   ON T3.UUID = T2.APPLICANT_ID ", model.CONTRACTOR_REG_NO);

                record.CONTRACTOR_REG_NAME = db.Database.SqlQuery<string>(sqlRegName).FirstOrDefault();

                record.IrregularitiesList = db.Database.SqlQuery<Irregularities>(string.Format(@"SELECT DRI.Master_ID,
                                                                                       DRI.UUID,
                                                                                       SV.Code,
                                                                                       SV.Description,
                                                                                       DRI.Is_Checked,
                                                                                       DRI.Remark
                                                                                FROM   P_MW_DIRECT_RETURN_IRREGULARITIES DRI
                                                                                       LEFT JOIN P_S_System_Value SV
                                                                                               ON DRI.SV_IRREGULARITIES_ID = SV.UUID
                                                                                                  AND DRI.Master_ID = '{0}'
                                                                                       JOIN P_S_System_Type ST
                                                                                         ON SV.System_type_ID = ST.UUID
                                                                                            AND ST.TYPE = 'Irregularities'
                                                                                ORDER  BY SV.Code ", sUUID)).ToList();

                return record;
            }
        }
        private string SearchStatistics_whereq(Fn01LM_DRStatModel model)
        {
            string queryStr = "";
            if (model.DR != null && !string.IsNullOrWhiteSpace(model.DR.CONTRACTOR_REG_NO))
            {
                queryStr += "AND T3.CONTRACTOR_REG_NO like '%"+model.DR.CONTRACTOR_REG_NO.ToString()+"%'";
                //model.QueryParameters.Add("ContractorRegNo", model.DR.CONTRACTOR_REG_NO);
            }
            return queryStr;
        }
        private void Statistics_q(Fn01LM_DRStatModel model)
        {
            List<Dictionary<string, string>> cols = new List<Dictionary<string, string>> { new Dictionary<string, string> { ["displayName"] = "Irregularities", ["columnName"] = "IRREGULARITIES" } };
            string q = ""
            + "\r\n\t" + " SELECT                                                                                                      "
            + "\r\n\t" + " T1.DESCRIPTION   as IRREGULARITIES                                                                          ";

            q = q
             + "\r\n\t" + " ,(                                                                                                      "
             + "\r\n\t" + " 	SELECT COUNT(*) FROM P_MW_DIRECT_RETURN T3                                                             "
             + "\r\n\t" + " 	INNER JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T4 ON T3.UUID = T4.MASTER_ID AND T4.IS_CHECKED = 'True'   "
             + "\r\n\t" + " 	WHERE  T4.SV_IRREGULARITIES_ID = T1.UUID                                                               ";


            if (model.DateFrom != null && model.DateTo != null)
            {
                q = q + "\r\n\t" + " 	AND T3.RECEIVED_DATE BETWEEN :dateFrom AND :dateTo                                          ";
                model.QueryParameters.Add("dateFrom", model.DateFrom.Value);
                model.QueryParameters.Add("dateTo", model.DateTo.Value);
            }
            if (model.RegNo != null)
            {
                q = q    + "\r\n\t" + " 	AND T3.CONTRACTOR_REG_NO = :regNo                                        ";
                model.QueryParameters.Add("regNo", model.RegNo);
            }

            q = q  + "\r\n\t" + " ) AS  TOTAL                                          ";
            cols.Add(new Dictionary<string, string> { ["displayName"] = "Total", ["columnName"] = "TOTAL" });
            //}
            q = q
            + "\r\n\t" + " FROM P_S_SYSTEM_VALUE T1                                                                                    "
            + "\r\n\t" + " INNER JOIN P_S_SYSTEM_TYPE T2 ON T1.SYSTEM_TYPE_ID = T2.UUID AND T2.TYPE = 'Irregularities'                 ";
            model.Query = q;
            model.Rpp = -1;
            model.Columns = cols.ToArray();












            //BREAK DOWN MONTHLY
            /*
            List<Dictionary<string, string>> cols = new List<Dictionary<string, string>> { new Dictionary<string, string> { ["displayName"] = "Irregularities", ["columnName"] = "IRREGULARITIES" } };
            DateTime datePointer = model.DateFrom.Value, dateMax = model.DateTo.Value.AddMonths(1);
            string q = ""
            + "\r\n\t" + " SELECT                                                                                                      "
            + "\r\n\t" + " T1.DESCRIPTION   as IRREGULARITIES                                                                          ";
            while (datePointer < dateMax)
            {
                string monStr = datePointer.ToString("MMM-yyyy");
                q = q
                + "\r\n\t" + " ,(                                                                                                      "
                + "\r\n\t" + " 	SELECT COUNT(*) FROM P_MW_DIRECT_RETURN T3                                                             "
                + "\r\n\t" + " 	INNER JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T4 ON T3.UUID = T4.MASTER_ID AND T4.IS_CHECKED = 'True'   "
                + "\r\n\t" + " 	WHERE  T4.SV_IRREGULARITIES_ID = T1.UUID                                                               "
                + "\r\n\t" + " 	AND TO_CHAR(T3.RECEIVED_DATE, 'Mon-YYYY') = '" + monStr + "'" + SearchStatistics_whereq(model)
                + ") AS \"" + monStr.ToUpper() + "\"          ";
                cols.Add(new Dictionary<string, string> { ["displayName"] = monStr, ["columnName"] = monStr.ToUpper() });
                datePointer = datePointer.AddMonths(1);
            }
            q = q
            + "\r\n\t" + " FROM P_S_SYSTEM_VALUE T1                                                                                    "
            + "\r\n\t" + " INNER JOIN P_S_SYSTEM_TYPE T2 ON T1.SYSTEM_TYPE_ID = T2.UUID AND T2.TYPE = 'Irregularities'                 ";
            model.Query = q;
            model.Rpp = -1;
            model.Columns = cols.ToArray();
            */
        }
        public Fn01LM_DRStatModel SearchStatistics(Fn01LM_DRStatModel model)
        {
            Statistics_q(model);
            model.Search();
            return model;
        }
        public string ExcelStatistics(Fn01LM_DRStatModel model)
        {
            Statistics_q(model);
            return model.Export("Direct Return Statistics");
        }





        private void StatisticsV2_q(Fn01LM_DRStatModel model)
        {
            List<Dictionary<string, string>> cols = new List<Dictionary<string, string>> { new Dictionary<string, string> { ["displayName"] = "Month-Year", ["columnName"] = "MON" } };
            string q = "" + "\r\n\t" + " SELECT T1.SORT, T1.MON                                                                                 ";
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_SYSTEM_TYPE sType = db.P_S_SYSTEM_TYPE.Where(o => o.TYPE == "Irregularities").FirstOrDefault();
                List<P_S_SYSTEM_VALUE> sValue = db.P_S_SYSTEM_VALUE.Where(o => o.SYSTEM_TYPE_ID == sType.UUID).OrderBy(o => o.CODE).ToList();
                for (int i = 0; i < sValue.Count; i++)
                {
                    cols.Add(new Dictionary<string, string> { ["displayName"] = sValue[i].CODE, ["columnName"] = sValue[i].CODE });
                    q = q + "\r\n\t" + " , SUM(CASE WHEN T3.SV_IRREGULARITIES_ID = :SVUUID" + i + " THEN 1 ELSE 0 END) AS  " + sValue[i].CODE;
                    model.QueryParameters.Add("SVUUID" + i, sValue[i].UUID);
                }
            }
            DateTime dateMin = model.DateFrom.Value, dateMax = model.DateTo.Value.AddMonths(1), dateNow;
            dateMin = new DateTime(dateMin.Year, dateMin.Month, 1);
            dateMax = new DateTime(dateMax.Year, dateMax.Month, 1).AddMonths(1).AddTicks(-1);
            dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.QueryParameters.Add("dateMin", dateMin);
            model.QueryParameters.Add("dateMax", dateMax);
            int monDif = ((dateMin.Year - dateNow.Year) * 12) + dateMin.Month - dateNow.Month;
            int monCnt = ((dateMax.Year - dateMin.Year) * 12) + dateMax.Month - dateMin.Month;
            q = q
                + "\r\n\t" + " FROM                                                                                                              "
                + "\r\n\t" + " (                                                                                                                 "
                + "\r\n\t" + "     SELECT ADD_MONTHS(SYSDATE, ROWNUM -1+ (" + monDif + ")) AS SORT, TO_CHAR(ADD_MONTHS(SYSDATE, ROWNUM -1+ (" + monDif + ")), 'Mon-YYYY') AS MON "
                + "\r\n\t" + "     FROM DUAL CONNECT BY ROWNUM <= " + monCnt + "                                                                 "
                + "\r\n\t" + " ) T1                                                                                                              "
                + "\r\n\t" + " LEFT JOIN P_MW_DIRECT_RETURN T2 ON T1.MON = TO_CHAR(T2.RECEIVED_DATE, 'Mon-YYYY')                                 "
                + "\r\n\t" + " AND T2.RECEIVED_DATE BETWEEN :dateMin AND :dateMax                                                                "
                + "\r\n\t" + " LEFT JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T3 ON T2.UUID = T3.MASTER_ID AND T3.IS_CHECKED = 'True'               "
                + "\r\n\t" + "  GROUP BY  T1.SORT, T1.MON                                                                                        ";
            model.Sort = "SORT";
            model.Query = q;
            model.Rpp = -1;
            model.Columns = cols.ToArray();
        }
        public Fn01LM_DRStatModel SearchStatisticsV2(Fn01LM_DRStatModel model)
        {
            StatisticsV2_q(model);
            model.Search();
            return model;
        }
        public string ExcelStatisticsV2(Fn01LM_DRStatModel model)
        {
            StatisticsV2_q(model);
            return model.Export("Direct Return Statistics");
        }




        /*


        public Fn01LM_DRSearchModel SearchStatisticsV2(Fn01LM_DRSearchModel model)
        {
            model.Query = string.Format(StatisticsSqlV2P, StatisticsCriteria(model, true));
            model.QueryWhere = "";
            model.Search();
            return model;
        }*/

        #endregion

        #region Export

        public Fn01LM_DRSearchModel ExportDr(Fn01LM_DRSearchModel model)
        {
            model.Query = model.IsMaintenance ? SearchDrSql : SearchDrSqlOfToday;
            model.QueryWhere = SearchDrCriteria(model);
            model.Search();
            return model;
        }

        #endregion

        #region Create

        /// <summary>
        /// Get DSN Info
        /// </summary>
        /// <param name="DSN"></param>
        /// <returns></returns>
        public P_MW_DSN GetDSN(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(m => m.DSN == DSN).FirstOrDefault();
            }
        }

        public ServiceResult CreateDr(Fn01LM_DRSaveModel model)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_DIRECT_RETURN data = new P_MW_DIRECT_RETURN()
                        {
                            DSN = model.P_MW_DIRECT_RETURN.DSN,
                            FORM_TYPE = model.P_MW_DIRECT_RETURN.FORM_TYPE,
                            CONTRACTOR_REG_NO = model.P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO,
                            RECEIVED_DATE = model.P_MW_DIRECT_RETURN.RECEIVED_DATE,
                            HANDING_STAFF_1 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_1,
                            HANDING_STAFF_2 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_2,
                            HANDING_STAFF_3 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_3,
                            LANGUAGE = model.P_MW_DIRECT_RETURN.LANGUAGE,
                            ADDRESS = model.P_MW_DIRECT_RETURN.ADDRESS
                        };
                        db.P_MW_DIRECT_RETURN.Add(data);
                        db.SaveChanges();

                        P_MW_DIRECT_RETURN record = db.P_MW_DIRECT_RETURN.Where(d => d.DSN == model.P_MW_DIRECT_RETURN.DSN).OrderByDescending(od => od.CREATED_BY).FirstOrDefault();
                        if (record != null)
                        {
                            foreach (var item in model.IrregularitiesList)
                            {
                                if (item.IsChecked && item.Code == "N")
                                {
                                    db.P_MW_DIRECT_RETURN_IRREGULARITIES.Add(new P_MW_DIRECT_RETURN_IRREGULARITIES() { MASTER_ID = record.UUID, SV_IRREGULARITIES_ID = item.UUID, IS_CHECKED = item.IsChecked.ToString(), REMARK = item.Remark });
                                }
                                else
                                {
                                    db.P_MW_DIRECT_RETURN_IRREGULARITIES.Add(new P_MW_DIRECT_RETURN_IRREGULARITIES() { MASTER_ID = record.UUID, SV_IRREGULARITIES_ID = item.UUID, IS_CHECKED = item.IsChecked.ToString() });
                                }

                            }
                            db.SaveChanges();

                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

        #endregion

        #region Update

        public ServiceResult UpdateDr(Fn01LM_DRSaveModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Get DB Record
                        P_MW_DIRECT_RETURN record = db.P_MW_DIRECT_RETURN.Where(d => d.DSN == model.P_MW_DIRECT_RETURN.DSN).FirstOrDefault();

                        if (record == null) { return new ServiceResult() { Result = "Inexistence" }; }

                        //record.DSN = model.DSN;
                        record.FORM_TYPE = model.P_MW_DIRECT_RETURN.FORM_TYPE;
                        record.CONTRACTOR_REG_NO = model.P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO;
                        record.RECEIVED_DATE = model.P_MW_DIRECT_RETURN.RECEIVED_DATE;
                        record.HANDING_STAFF_1 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_1;
                        record.HANDING_STAFF_2 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_2;
                        record.HANDING_STAFF_3 = model.P_MW_DIRECT_RETURN.HANDING_STAFF_3;
                        record.ADDRESS = model.P_MW_DIRECT_RETURN.ADDRESS;
                        record.LANGUAGE = model.P_MW_DIRECT_RETURN.LANGUAGE;


                        List<P_MW_DIRECT_RETURN_IRREGULARITIES> chlidList = db.P_MW_DIRECT_RETURN_IRREGULARITIES.Where(d => d.MASTER_ID == record.UUID).ToList();

                        foreach (var leftItem in chlidList)
                        {
                            foreach (var RightItem in model.IrregularitiesList)
                            {
                                if (leftItem.UUID == RightItem.UUID)
                                {
                                    if (RightItem.Code == "N")
                                    {
                                        leftItem.REMARK = RightItem.IsChecked ? RightItem.Remark : null;
                                    }
                                    leftItem.IS_CHECKED = RightItem.Is_Checked;
                                    break;
                                }
                            }
                        }

                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

        #endregion

    }
}