using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingModificationDAOService
    {
        private const string SearchModSql = @"SELECT T1.UUID,
                                                       T1.REFERENCE_NO,
                                                       T1.FORM_NO,
                                                       T1.DSN,
                                                       T1.RECEIVED_DATE,
                                                       T1.HANDING_STAFF
                                                FROM   P_MW_MODIFICATION T1
                                                WHERE  1 = 1 ";

        private const string SearchModOfTodaySql = @"SELECT T1.UUID,
                                                               T1.REFERENCE_NO,
                                                               T1.DSN,
                                                               to_char(T1.RECEIVED_DATE,'dd/MM/yyyy') RECEIVED_DATE,
                                                               T1.HANDING_STAFF
                                                        FROM   P_MW_MODIFICATION T1
                                                        WHERE  1 = 1 ";

        private const string SearchOutgoingDocSql = @"SELECT MSD.Document_Type,
                                                               MSD.Folder_Type,
                                                               MSD.Scan_Date,
                                                               MSD.DSN_SUB,
                                                               MD.Form_Code,
                                                               MSD.File_Path
                                                        FROM   P_MW_MODIFICATION MM
                                                               INNER JOIN P_MW_DSN MD
                                                                       ON MM.DSN = MD.DSN
                                                               INNER JOIN P_MW_SCANNED_DOCUMENT MSD
                                                                       ON MD.UUID = MSD.DSN_ID
                                                        WHERE  MM.UUID = '{0}'
                                                               AND Nvl(MD.SUBMIT_TYPE, ' ') = 'Issued Correspondence' ";

        private const string SearchIncomingDocSql = @"SELECT MSD.Document_Type,
                                                               MSD.Folder_Type,
                                                               MSD.Scan_Date,
                                                               MSD.DSN_SUB,
                                                               MD.Form_Code,
                                                               MSD.File_Path
                                                        FROM   P_MW_MODIFICATION MM
                                                               INNER JOIN P_MW_DSN MD
                                                                       ON MM.DSN = MD.DSN
                                                               INNER JOIN P_MW_SCANNED_DOCUMENT MSD
                                                                       ON MD.UUID = MSD.DSN_ID
                                                        WHERE  MM.UUID = '{0}'
                                                               AND Nvl(MD.SUBMIT_TYPE, ' ') != 'Issued Correspondence' ";

        private const string SearchExcelModDataSql = @" SELECT  T1.REFERENCE_NO                      ,
                                                                T1.FORM_NO                           ,
                                                                T1.DSN                               ,
                                                                T1.RECEIVED_DATE                     ,
                                                                T1.HANDING_STAFF                     ,
                                                                T1.RRM_SYN_STATUS                    ,
                                                                T2.ADDRESS_ENG                       ,
                                                                T2.ADDRESS_CHI                       ,
                                                                T2.RECEIVED_DATE_OF_FORM_BA16        ,
                                                                T2.RESULT_OF_THE_APPLICATION         ,
                                                                T2.ISSUE_DATE_OF_BD106               ,
                                                                T2.NO_OF_APPROVED_FLATS_INVOLVED     ,
                                                                T2.NO_OF_CUBICLES_AFTER_SUBDIVISION  ,
                                                                T2.FLOOR_AREA_OF_SUBDIVIDED_CUBICLES ,
                                                                T2.COMPLETION_DATE                   ,
                                                                T2.ANNUAL_INSPECTION_DATE            ,
                                                                T2.CREATED_BY                        ,
                                                                T2.CREATED_DATE                      ,
                                                                T2.MODIFIED_BY                       ,
                                                                T2.MODIFIED_DATE                     ,
                                                                T2.LSS_REFERRAL_DATE                 ,
                                                                T2.EBD_REFERRAL_DATE                 ,
                                                                T2.SITE_INSP_COMPLETED               ,
                                                                T2.SITE_INSP_DATE                    ,
                                                                T2.INSP_RESULT                       ,
                                                                T2.IS_VALID                          ,
                                                                T2.PERMIT_NO                         ,
                                                                T2.OUR_REF_NO                        ,
                                                                T2.STATUS                            ,
                                                                T6.CODE                 
                                                        FROM P_MW_MODIFICATION T1
                                                               LEFT JOIN P_MOD_BD106 T2
                                                                      ON T1.UUID = T2.MW_MODIFICATION_ID
                                                               INNER JOIN (SELECT uuid,
                                                                                  mw_modification_id,
                                                                                  Listagg(CODE, ',') Within GROUP(ORDER BY CODE) AS " + "CODE" + @"
                                                                           FROM(SELECT T3.*,
                                                                                          T5.CODE
                                                                                   FROM   P_MOD_BD106 T3
                                                                                          LEFT JOIN P_MOD_BD106_ITEM T4
                                                                                                 ON T3.UUID = T4.P_MOD_BD106_ID
                                                                                          INNER JOIN P_S_SYSTEM_VALUE T5
                                                                                                  ON T4.S_SYSTEM_VALUE_UUID = T5.UUID)
                                                                           GROUP BY uuid,
                                                                                     mw_modification_id) T6
                                                                       ON T1.UUID = T6.mw_modification_id 
                                                                       where 1=1 ";

        public string GetSearchModCriteria(Fn01LM_SearchModModel model)
        {
            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.REFERENCE_NO))
            {
                whereQ += "\r\n\t" + "AND T1.REFERENCE_NO LIKE :REFERENCE_NO";
                model.QueryParameters.Add("REFERENCE_NO", "%" + model.REFERENCE_NO.Trim().ToUpper() + "%");
            }
            if (model.ReceivedDateFrom != null)
            {
                whereQ += "\r\n\t" + "AND T1.RECEIVED_DATE" + " >= :ReceivedDateFrom";
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom);
            }
            if (model.ReceivedDateTo != null)
            {
                whereQ += "\r\n\t" + "AND T1.RECEIVED_DATE" + " <= :ReceivedDateTo";
                model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.ADDRESS))
            {
                whereQ += "\r\n\t" + "AND T1.ADDRESS LIKE :ADDRESS";
                model.QueryParameters.Add("ADDRESS", "%" + model.ADDRESS.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.LOT_NO))
            {
                whereQ += "\r\n\t" + "AND T1.LOT_NO LIKE :LOT_NO";
                model.QueryParameters.Add("LOT_NO", "%" + model.LOT_NO.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.APPLICANT_NAME))
            {
                whereQ += "\r\n\t" + "AND T1.APPLICANT_NAME LIKE :APPLICANT_NAME";
                model.QueryParameters.Add("APPLICANT_NAME", "%" + model.APPLICANT_NAME.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.APPLICANT_CAPACITY))
            {
                whereQ += "\r\n\t" + "AND T1.APPLICANT_CAPACITY LIKE :APPLICANT_CAPACITY";
                model.QueryParameters.Add("APPLICANT_CAPACITY", "%" + model.APPLICANT_CAPACITY.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HANDING_STAFF))
            {
                whereQ += "\r\n\t" + "AND T1.HANDING_STAFF = :HANDING_STAFF";
                model.QueryParameters.Add("HANDING_STAFF", model.HANDING_STAFF);
            }

            return whereQ;
        }

        public DisplayGrid SearchMod(Fn01LM_SearchModModel model)
        {
            model.Query = SearchModSql;
            model.QueryWhere = GetSearchModCriteria(model);
            model.Search();
            return model;
        }

        public DisplayGrid SearchModOfToday()
        {
            DisplayGrid dg = new DisplayGrid();
            dg.Query = SearchModOfTodaySql;
            dg.QueryWhere = "\r\n\t" + "And TO_CHAR(T1.RECEIVED_DATE,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')";
            dg.Search();
            return dg;
        }

        public DisplayGrid SearchIncomingDoc(string modUuid)
        {
            DisplayGrid dg = new DisplayGrid();
            dg.Query = string.Format(SearchIncomingDocSql, modUuid);
            dg.Search();
            return dg;
        }

        public DisplayGrid SearchOutgoingDoc(string modUuid)
        {
            DisplayGrid dg = new DisplayGrid();
            dg.Query = string.Format(SearchOutgoingDocSql, modUuid);
            dg.Search();
            return dg;
        }

        public P_MW_MODIFICATION Create(Fn01LM_ModModel model)
        {
            P_MW_MODIFICATION record = new P_MW_MODIFICATION();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_MODIFICATION.Add(model.P_MW_MODIFICATION);
                db.SaveChanges();
                record = model.P_MW_MODIFICATION;
            }
            return record;
        }

        public void SaveMwNo(List<P_MW_MODIFICATION_RELATED_MWNO> listMwNo, string modificationID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //Delete 
                db.P_MW_MODIFICATION_RELATED_MWNO.RemoveRange(db.P_MW_MODIFICATION_RELATED_MWNO.Where(d => d.MODIFICATION_ID == modificationID));

                //Add
                foreach (P_MW_MODIFICATION_RELATED_MWNO item in listMwNo)
                {
                    db.P_MW_MODIFICATION_RELATED_MWNO.Add(item);
                }
                db.SaveChanges();
            }
        }

        public int Update(Fn01LM_ModModel model)
        {
            int iResult = 0;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //P_MW_MODIFICATION record = db.P_MW_MODIFICATION.Where(d => d.UUID == model.P_MW_MODIFICATION.UUID).FirstOrDefault();
                db.Entry(model.P_MW_MODIFICATION).State = System.Data.Entity.EntityState.Modified;
                iResult = db.SaveChanges();
            }
            return iResult;
        }

        /// <summary>
        /// Get Modification
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public P_MW_MODIFICATION GetModification(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_MODIFICATION.Where(d => d.UUID == uuid).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Mw No List
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public List<P_MW_MODIFICATION_RELATED_MWNO> GetWmNoList(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_MODIFICATION_RELATED_MWNO.Where(d => d.MODIFICATION_ID == uuid).ToList();
            }
        }

        /// <summary>
        /// Get Mod Bd 106
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public P_MOD_BD106 GetModBd106(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MOD_BD106.Where(d => d.MW_MODIFICATION_ID == uuid).FirstOrDefault();
            }
        }

        public List<T> GetModBd106Items<T>(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.Database.SqlQuery<T>(string.Format(@"SELECT ST.TYPE,
                                                                       SV.CODE,
                                                                       SV.DESCRIPTION,
                                                                       MBI.UUID,
                                                                       MBI.P_MOD_BD106_ID,
                                                                       SV.UUID As S_SYSTEM_VALUE_UUID,
                                                                       ( CASE
                                                                           WHEN sv.uuid = mbi.s_system_value_uuid THEN 'True'
                                                                           ELSE 'False'
                                                                         END ) AS Is_Checked
                                                                FROM   P_S_SYSTEM_TYPE ST
                                                                       INNER JOIN P_S_SYSTEM_VALUE SV
                                                                               ON ST.UUID = SV.SYSTEM_TYPE_ID
                                                                                  AND Type = 'RegulationExempted'
                                                                       LEFT JOIN P_MOD_BD106_ITEM MBI
                                                                              ON SV.UUID = MBI.S_SYSTEM_VALUE_UUID
                                                                                  AND  MBI.P_MOD_BD106_ID = '{0}' 
                                                                Order By SV.Ordering ", uuid)).ToList();
            }
        }

        // Begin Add by Chester

        public List<P_MOD_BD106_ITEM> GetModBD106ItemByBD106(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.Database.SqlQuery<P_MOD_BD106_ITEM>(string.Format(@"select * from P_MOD_BD106_item where P_MOD_BD106_ID='{0}'", uuid)).ToList();
            }
        }

        public ServiceResult UpdateBD106(Fn01LM_ModModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    //P_MOD_BD106 bD106 = db.P_MOD_BD106.Where(m => m.UUID == model.P_MOD_BD106.UUID).FirstOrDefault();
                    //bD106 = model.P_MOD_BD106;
                    if (model.P_MOD_BD106.STATUS == "Accept")
                    {
                        model.P_MOD_BD106.MW_MODIFICATION_ID = model.P_MW_MODIFICATION.UUID;
                        db.Entry(model.P_MOD_BD106).State = EntityState.Modified;

                        if (model.listBD106Item != null && model.listBD106Item.Count() > 0)
                        {
                            db.P_MOD_BD106_ITEM.RemoveRange(db.P_MOD_BD106_ITEM.Where(m => m.P_MOD_BD106_ID == model.P_MOD_BD106.UUID));
                            foreach (P_MOD_BD106_ITEM_View item in model.listBD106Item)
                            {
                                if (item.IsChecked)
                                {
                                    db.P_MOD_BD106_ITEM.Add(new P_MOD_BD106_ITEM()
                                    {
                                        P_MOD_BD106_ID = model.P_MOD_BD106.UUID
                                        ,
                                        S_SYSTEM_VALUE_UUID = item.S_SYSTEM_VALUE_UUID
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        P_MOD_BD106 bD106 = db.P_MOD_BD106.Where(m => m.UUID == model.P_MOD_BD106.UUID).FirstOrDefault();
                        bD106.STATUS = model.P_MOD_BD106.STATUS;
                        bD106.COMPLETION_DATE = model.P_MOD_BD106.COMPLETION_DATE;
                    }



                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message =
                                            {
                                                ex.Message
                                            }
                        };
                    }
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }

        public ServiceResult SaveBD106(Fn01LM_ModModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    model.P_MOD_BD106.MW_MODIFICATION_ID = model.P_MW_MODIFICATION.UUID;
                    db.P_MOD_BD106.Add(model.P_MOD_BD106);
                    db.SaveChanges();
                    foreach (P_MOD_BD106_ITEM_View item in model.listBD106Item)
                    {
                        if (item.IsChecked)
                        {
                            db.P_MOD_BD106_ITEM.Add(new P_MOD_BD106_ITEM()
                            {
                                P_MOD_BD106_ID = model.P_MOD_BD106.UUID
                                ,
                                S_SYSTEM_VALUE_UUID = item.S_SYSTEM_VALUE_UUID
                            });
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message =
                                            {
                                                ex.Message
                                            }
                        };
                    }
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }

            }
        }

        //public ServiceResult SaveBD106Item(Fn01LM_ModModel model)
        //{
        //    using (EntitiesMWProcessing db = new EntitiesMWProcessing())
        //    {
        //        using (DbContextTransaction tran = db.Database.BeginTransaction())
        //        {
        //            //tran.Commit();


        //            try
        //            {
        //                db.SaveChanges();
        //                tran.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                tran.Rollback();
        //                return new ServiceResult()
        //                {
        //                    Result = ServiceResult.RESULT_FAILURE
        //                    ,
        //                    Message =
        //                    {
        //                        ex.Message
        //                    }
        //                };
        //            }
        //            return new ServiceResult()
        //            {
        //                Result = ServiceResult.RESULT_SUCCESS
        //            };
        //        }

        //    }
        //}


        public ServiceResult GetMWNo(string dsn)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_DSN p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                    ,
                    Message =
                    {
                        p_MW_DSN==null?"" :p_MW_DSN.RECORD_ID
                        ,p_MW_DSN==null?"" :p_MW_DSN.FORM_CODE
                    }
                };
            }
        }

        //public List<List<object>> ExcelMod(Fn01LM_SearchModModel model)
        //{
        //    using (EntitiesMWProcessing db = new EntitiesMWProcessing())
        //    {
        //        using (DbConnection conn = db.Database.Connection)
        //        {
        //            List<List<object>> data = new List<List<object>>();
        //            string sql = SearchExcelModDataSql;
        //            sql += GetSearchModCriteria(model);
        //            DbDataReader dr = CommonUtil.GetDataReader(conn, sql, model.QueryParameters);
        //            data = CommonUtil.convertToList(dr);
        //            conn.Close();
        //            return data;
        //        }
        //    }
        //}
        public Fn01LM_SearchModModel ExcelMod(Fn01LM_SearchModModel model)
        {
            model.Query = SearchExcelModDataSql;
            model.QueryWhere = GetSearchModCriteria(model);
            model.Search();
            return model;
        }
        // End Add by Chester
    }
}