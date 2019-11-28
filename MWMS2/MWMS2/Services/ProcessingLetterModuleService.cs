using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace MWMS2.Services
{
    public class ProcessingLetterModuleService
    {
        // Add by Chester 2019-06-11
        private const string SearchAck_q = ""
           + "\r\n\t" + "select * from P_MW_ACK_LETTER "
            + "\r\n\t" + " Where 1=1 ";


        // End add by Chester 2019-06-11

        private const string SearchDr_q = ""
                 + "\r\n" + "\t" + "SELECT                                                               "
                 + "\r\n" + "\t" + "T1.UUID                                                              "
                 + "\r\n" + "\t" + ", T1.DSN                                               "
                 + "\r\n" + "\t" + ", T1.FORM_TYPE                                            "
                 + "\r\n" + "\t" + ", T1.CONTRACTOR_REG_NO                                                "
                 + "\r\n" + "\t" + ", T1.RECEIVED_DATE                                                           "
                 + "\r\n" + "\t" + ", T1.HANDING_STAFF_1                                                "
                 + "\r\n" + "\t" + ", T1.HANDING_STAFF_2                                                    "
                 + "\r\n" + "\t" + ", T1.HANDING_STAFF_3                                               "
                 + "\r\n" + "\t" + "FROM P_MW_DIRECT_RETURN T1                                           "
                 + "\r\n" + "\t" + "Where 1 = 1                                           ";
        private const string SearchDr_q2 = @"SELECT    T1.UUID,
                                                       T1.DSN,
                                                       T1.FORM_TYPE,
                                                       T1.CONTRACTOR_REG_NO,
                                                       T1.RECEIVED_DATE,
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



        //private const string SearchDr_q2 = @"Select T1.* 
        //                                    From P_MW_Direct_Return T1
        //                                    Left Join 
        //                                    (Select * From
        //                                        (Select Master_ID , SV_IRREGULARITIES_ID , IS_Checked 
        //                                        From P_MW_DIRECT_RETURN_IRREGULARITIES)DRI
        //                                        Pivot( Max(IS_Checked) for SV_IRREGULARITIES_ID in('A','B','C','D','E','F','G','H','I','J','K','L','M','N')))T2
        //                                    On T1.DSN = T2.Master_ID
        //                                    Where 1 = 1 ";

        //private const string SearchDr_qStatistics1 = @"SELECT DRI.SV_IRREGULARITIES_ID,
        //                                                       ( CASE DRI.is_checked
        //                                                           WHEN 'True' THEN 1
        //                                                           ELSE 0
        //                                                         END ) AS IsTrue,
        //                                                       DRI.Created_Date
        //                                                FROM   P_MW_DIRECT_RETURN_IRREGULARITIES DRI
        //                                                WHERE  1 = 1 ";

        //private const string SearchDr_qStatistics = @"SELECT To_char(T1.Created_Date, 'mm/yyyy') Created_Date,
        //                                                       T1.SV_IRREGULARITIES_ID As Irregularities ,
        //                                                       Sum(T1.IsTrue)                      AS Total
        //                                                FROM   ({0}) T1
        //                                                GROUP  BY T1.SV_IRREGULARITIES_ID,
        //                                                          To_char(Created_Date, 'mm/yyyy') ";

        private const string SearchDr_qStatistics1 = @"SELECT SV.Code,
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

        private const string SearchDr_qStatistics = @"SELECT TT1.Code,
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


        private const string SearchDr_qStatisticsV201 = @"SELECT SV.Code,
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
        private const string SearchDr_qStatisticsV202 = @" SELECT *
                                                            FROM   (SELECT *
                                                                    FROM  (SELECT To_char(T1.Created_Date, 'mm/yyyy') Created_Date,
                                                                                  T1.Code,
                                                                                  Sum(T1.IsTrue)                      AS Total
                                                                           FROM   ({0})T1
                                                                           GROUP  BY T1.Code,
                                                                                     To_char(T1.Created_Date, 'mm/yyyy'))TT1)TTT1
                                                            PIVOT (Sum(Total) For Code in('A','B','C','D','E','F','G','H','I','J','K','L','M','N'))";

        //Add by Chester 2019-06-11
        private string SearchAckByDSN_whereQ(Fn01LM_AckSearchModel model)
        {
            string whereQ = "";
            //if (!string.IsNullOrEmpty(model.SearchReceivedDate))
            //{
            //    whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date('" + model.SearchReceivedDate.Trim() + "','dd/MM/yyyy') ";
            //    //model.QueryParameters.Add("RECEIVED_DATE", model.SearchReceivedDate.Trim());
            //}
            //else
            //{
            //    whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date('" + DateTime.Now + "','dd/MM/yyyy') ";
            //    //model.QueryParameters.Add("RECEIVED_DATE", " To_Date(" + DateTime.Now + ",'dd/MM/yyyy')");
            //}
            if (!string.IsNullOrEmpty(model.SearchDSN))
            {
                whereQ += "\r\n\t" + " AND DSN like :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.SearchDSN.Trim().ToUpper() + "%");
            }


            return whereQ;
        }

        private string SearchAckByReceivedDate_whereQ(Fn01LM_AckSearchModel model)
        {
            string whereQ = "";
            //if (!string.IsNullOrEmpty(model.SearchReceivedDate))
            //{
            //    whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date('" + model.SearchReceivedDate.Trim() + "','dd/MM/yyyy') ";
            //    //model.QueryParameters.Add("RECEIVED_DATE", model.SearchReceivedDate.Trim());
            //}
            //else
            //{
            //    whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date('" + DateTime.Now + "','dd/MM/yyyy') ";
            //    //model.QueryParameters.Add("RECEIVED_DATE", " To_Date(" + DateTime.Now + ",'dd/MM/yyyy')");
            //}
            return whereQ;
        }
        // End add by Chester 2019-06-11

        private string SearchDr_whereQ(Fn01LM_DRSearchModel model)
        {
            string whereQ = "";

            if (model.IsToday)
            {
                whereQ += "\r\n\t" + "And TO_CHAR(T1.CREATED_DATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')";
                return whereQ;
            }

            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereQ += "\r\n\t" + "AND T1.DSN LIKE :DSN";
                model.QueryParameters.Add("DSN", "%" + model.DSN.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.FORM_TYPE))
            {
                whereQ += "\r\n\t" + "AND T1.FORM_TYPE LIKE :FORM_TYPE";
                model.QueryParameters.Add("FORM_TYPE", "%" + model.FORM_TYPE.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CONTRACTOR_REG_NO))
            {
                whereQ += "\r\n\t" + "AND T1.CONTRACTOR_REG_NO LIKE :CONTRACTOR_REG_NO";
                model.QueryParameters.Add("CONTRACTOR_REG_NO", "%" + model.CONTRACTOR_REG_NO.Trim().ToUpper() + "%");
            }
            if (model.RECEIVED_DATE != null)
            {
                //whereQ += "\r\n\t" + "AND T1.RECEIVED_DATE = :RECEIVED_DATE";
                whereQ += "\r\n\t" + "And TO_CHAR(T1.RECEIVED_DATE,'YYYY-MM-DD')=TO_CHAR(:RECEIVED_DATE,'YYYY-MM-DD')";
                model.QueryParameters.Add("RECEIVED_DATE", model.RECEIVED_DATE);
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_1))
            {
                whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_1 LIKE :HANDING_STAFF_1";
                model.QueryParameters.Add("HANDING_STAFF_1", "%" + model.HANDING_STAFF_1.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_2))
            {
                whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_2 LIKE :HANDING_STAFF_2";
                model.QueryParameters.Add("HANDING_STAFF_2", "%" + model.HANDING_STAFF_2.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF_3))
            {
                whereQ += "\r\n\t" + "AND T1.HANDING_STAFF_3 LIKE :HANDING_STAFF_3";
                model.QueryParameters.Add("HANDING_STAFF_3", "%" + model.HANDING_STAFF_3.Trim().ToUpper() + "%");
            }

            string Irregularities = "";

            foreach(Irregularities item in model.IrregularitiesList)
            {
                if (item.IsChecked)
                {
                    Irregularities += item.Code + ",";
                }
            }

            if (!string.IsNullOrWhiteSpace(Irregularities))
            {
                whereQ += "\r\n\t" + "AND T2.IRREGULARTITES LIKE :IRREGULARTITES";
                model.QueryParameters.Add("IRREGULARTITES", "%" + Irregularities.Substring(0, Irregularities.Length-1) + "%");
            }


            return whereQ;
        }

        private string SearchDr_WhereStatisticsQ(Fn01LM_DRStatModel model , bool isV2=false)
        {
            
            string whereQ = SearchDr_qStatistics1;
            if (isV2)
            {
                whereQ = SearchDr_qStatisticsV201;
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

        public Fn01LM_DRSearchModel SearchDr(Fn01LM_DRSearchModel model)
        {
            model.Query = model.IsMaintenance ? SearchDr_q2 : SearchDr_q;
            model.QueryWhere = SearchDr_whereQ(model);
            model.Search();
            return model;
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

                record.P_MW_DIRECT_RETURN.DSN = model.DSN;
                record.P_MW_DIRECT_RETURN.FORM_TYPE = model.FORM_TYPE;
                record.P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO = model.CONTRACTOR_REG_NO;
                record.P_MW_DIRECT_RETURN.RECEIVED_DATE = model.RECEIVED_DATE;
                record.P_MW_DIRECT_RETURN.HANDING_STAFF_1 = model.HANDING_STAFF_1;
                record.P_MW_DIRECT_RETURN.HANDING_STAFF_2 = model.HANDING_STAFF_2;
                record.P_MW_DIRECT_RETURN.HANDING_STAFF_3 = model.HANDING_STAFF_3;

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

        public Fn01LM_DRStatModel SearchStatistics(Fn01LM_DRStatModel model)
        {
            DateTime datePointer = model.DateFrom.Value;
            DateTime dateMax = model.DateTo.Value.AddMonths(1);
            string q = ""
            + "\r\n\t" + " SELECT                                                                                                  "
            + "\r\n\t" + " T1.DESCRIPTION                                                                                          ";

            while(datePointer < model.DateTo)
            {
                string monStr = datePointer.ToString("MMM-yyyy");
                q = q
                + "\r\n\t" + " ,(                                                                                                      "
                + "\r\n\t" + " 	SELECT COUNT(*) FROM P_MW_DIRECT_RETURN T3                                                             "
                + "\r\n\t" + " 	INNER JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T4 ON T3.UUID = T4.MASTER_ID AND T4.IS_CHECKED = 'True'   "
                + "\r\n\t" + " 	WHERE  T4.SV_IRREGULARITIES_ID = T1.UUID                                                               "
                + "\r\n\t" + " 	AND TO_CHAR(T3.RECEIVED_DATE, 'Mon-YYYY') = '" + monStr+ "') AS \"" + monStr + "\"                     ";
                datePointer = datePointer.AddMonths(1);
            }

            /*
            + "\r\n\t" + " ,(                                                                                                      "
            + "\r\n\t" + " 	SELECT COUNT(*) FROM P_MW_DIRECT_RETURN T3                                                             "
            + "\r\n\t" + " 	INNER JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T4 ON T3.UUID = T4.MASTER_ID AND T4.IS_CHECKED = 'True'   "
            + "\r\n\t" + " 	WHERE  T4.SV_IRREGULARITIES_ID = T1.UUID                                                               "
            + "\r\n\t" + " 	AND TO_CHAR(T3.RECEIVED_DATE, 'Mon-YYYY') = 'Jun-2019') AS \"Jun - 2019\"                              "

            + "\r\n\t" + " ,(                                                                                                      "
            + "\r\n\t" + " 	SELECT COUNT(*) FROM P_MW_DIRECT_RETURN T3                                                             "
            + "\r\n\t" + " 	INNER JOIN P_MW_DIRECT_RETURN_IRREGULARITIES T4 ON T3.UUID = T4.MASTER_ID AND T4.IS_CHECKED = 'True'   "
            + "\r\n\t" + " 	WHERE  T4.SV_IRREGULARITIES_ID = T1.UUID                                                               "
            + "\r\n\t" + " 	AND TO_CHAR(T3.RECEIVED_DATE, 'Mon-YYYY') = 'Aug-2019') AS \"Aug - 2019\"                              "
            */
            q=q
            + "\r\n\t" + " FROM P_S_SYSTEM_VALUE T1                                                                                "
            + "\r\n\t" + " INNER JOIN P_S_SYSTEM_TYPE T2 ON T1.SYSTEM_TYPE_ID = T2.UUID AND T2.TYPE = 'Irregularities'             "
            +"\r\n\t" + " ORDER BY T1.DESCRIPTION             ";


            model.Query = q;// string.Format(SearchDr_qStatistics, SearchDr_WhereStatisticsQ(model));
            //model.QueryWhere = "";
            model.Rpp = 9999;
            model.Search();
            return model;
        }

        public Fn01LM_DRStatModel SearchStatisticsV2(Fn01LM_DRStatModel model)
        {
            model.Query = string.Format(SearchDr_qStatisticsV202, SearchDr_WhereStatisticsQ(model,true));
            model.QueryWhere = "";
            model.Search();
            return model;
        }

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


                        List<P_MW_DIRECT_RETURN_IRREGULARITIES> chlidList = db.P_MW_DIRECT_RETURN_IRREGULARITIES.Where(d => d.MASTER_ID == record.UUID).ToList();

                        foreach(var leftItem in chlidList)
                        {
                            foreach(var RightItem in model.IrregularitiesList)
                            {
                                if(leftItem.UUID == RightItem.UUID)
                                {
                                    if(RightItem.Code == "N" )
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

        public ServiceResult SaveDr(Fn01LM_DRSaveModel model)
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
                        };
                        db.P_MW_DIRECT_RETURN.Add(data);
                        db.SaveChanges();

                        P_MW_DIRECT_RETURN record = db.P_MW_DIRECT_RETURN.Where(d => d.DSN == model.P_MW_DIRECT_RETURN.DSN).OrderByDescending(od=>od.CREATED_BY).FirstOrDefault();
                        if(record != null)
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

        #region Ack Letter by chester

        public ServiceResult SaveAckLetter(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    //P_MW_DSN dsn = db.P_MW_DSN.Where(d => d.DSN == model.AcknowlegementLetter.DSN).FirstOrDefault();
                    //if (dsn == null)
                    //{
                    //    return "dsnNotExist";
                    //}
                    //model.P_MW_ACK_LETTER.COUNTER = model.Counter;
                    //model.P_MW_ACK_LETTER.NATURE = model.Nature;
                    //model.P_MW_ACK_LETTER.ORDER_RELATED = model.OrderRelated;
                    //model.P_MW_ACK_LETTER.SSP = model.SSP;
                    model.P_MW_ACK_LETTER.REPEATED = model.Repeated;
                    //model.P_MW_ACK_LETTER.LANGUAGE = model.Language;
                    //model.P_MW_ACK_LETTER.FILE_TYPE = model.FileType;
                    //model.P_MW_ACK_LETTER.BARCODE = model.Barcode;
                    //model.P_MW_ACK_LETTER.RECEIVED_DATE = Convert.ToDateTime(model.SearchReceivedDate);
                    db.P_MW_ACK_LETTER.Add(model.P_MW_ACK_LETTER);
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }

        public ServiceResult UpdateAckLetter(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var origModel = db.P_MW_ACK_LETTER.Where(m => m.UUID == model.P_MW_ACK_LETTER.UUID).FirstOrDefault();
                    origModel.COUNTER = model.P_MW_ACK_LETTER.COUNTER;
                    origModel.NATURE = model.P_MW_ACK_LETTER.NATURE;
                    origModel.RECEIVED_DATE = model.P_MW_ACK_LETTER.RECEIVED_DATE;
                    origModel.LETTER_DATE = model.P_MW_ACK_LETTER.LETTER_DATE;
                    origModel.EFSS_REF_NO = model.P_MW_ACK_LETTER.EFSS_REF_NO;
                    origModel.DSN = model.P_MW_ACK_LETTER.DSN;
                    origModel.MW_NO = model.P_MW_ACK_LETTER.MW_NO;
                    origModel.FORM_NO = model.P_MW_ACK_LETTER.FORM_NO;
                    origModel.COMP_DATE = model.P_MW_ACK_LETTER.COMP_DATE;
                    origModel.MW_ITEM = model.P_MW_ACK_LETTER.MW_ITEM;
                    origModel.AUDIT_RELATED = model.P_MW_ACK_LETTER.AUDIT_RELATED;
                    origModel.ORDER_RELATED = model.P_MW_ACK_LETTER.ORDER_RELATED;
                    origModel.SDF_RELATED = model.P_MW_ACK_LETTER.SDF_RELATED;
                    origModel.SIGNBOARD_RELATED = model.P_MW_ACK_LETTER.SIGNBOARD_RELATED;
                    origModel.SSP = model.P_MW_ACK_LETTER.SSP;
                    origModel.PBP_NO = model.P_MW_ACK_LETTER.PBP_NO;
                    origModel.PRC_NO = model.P_MW_ACK_LETTER.PRC_NO;
                    origModel.ADDRESS = model.P_MW_ACK_LETTER.ADDRESS;
                    origModel.BUILDING = model.P_MW_ACK_LETTER.BUILDING;
                    origModel.STREET = model.P_MW_ACK_LETTER.STREET;
                    origModel.STREET_NO = model.P_MW_ACK_LETTER.STREET_NO;
                    origModel.FLOOR = model.P_MW_ACK_LETTER.FLOOR;
                    origModel.UNIT = model.P_MW_ACK_LETTER.UNIT;
                    origModel.PAW = model.P_MW_ACK_LETTER.PAW;
                    origModel.PAW_CONTACT = model.P_MW_ACK_LETTER.PAW_CONTACT;
                    origModel.IO_MGT = model.P_MW_ACK_LETTER.IO_MGT;
                    origModel.IO_MGT_CONTACT = model.P_MW_ACK_LETTER.IO_MGT_CONTACT;
                    origModel.REMARK = model.P_MW_ACK_LETTER.REMARK;
                    origModel.LANGUAGE = model.P_MW_ACK_LETTER.LANGUAGE;
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }

        public ServiceResult DeleteAckLetter(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                    try
                    {
                        db.P_MW_ACK_LETTER.Remove(ack);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }

        public Fn01LM_AckSearchModel SearchDSN(Fn01LM_AckSearchModel model)
        {
            model.Query = SearchAck_q;
            model.QueryWhere = SearchAckByDSN_whereQ(model);
            model.Search();
            return model;
        }

        public Fn01LM_AckSearchModel SearchReceivedDate(Fn01LM_AckSearchModel model)
        {
            model.Query = SearchAck_q;
            model.QueryWhere = SearchAckByReceivedDate_whereQ(model);
            model.Search();
            return model;
        }

        public Fn01LM_AckSearchModel GetAckLetterById(string id)
        {
            Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_MW_ACK_LETTER> acks = db.P_MW_ACK_LETTER.ToList();
                model.P_MW_ACK_LETTER = db.P_MW_ACK_LETTER.Where(ack => ack.UUID == id).FirstOrDefault();
                return model;
            }

        }
        #endregion
    }
}