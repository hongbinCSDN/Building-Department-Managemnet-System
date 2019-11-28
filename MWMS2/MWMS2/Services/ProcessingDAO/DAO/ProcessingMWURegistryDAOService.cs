using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingMWURegistryDAOService
    {
        private const string SearchReceipt_q = @"SELECT DT,
                                                   T,
                                                   DSN,
                                                   STATUS
                                            FROM   (SELECT To_char(T1.RD_DELIVERED_DATE, 'dd/MM/YYYY') AS DT,
                                                           To_char(T1.RD_DELIVERED_DATE, 'HH:MI')      AS T,
                                                           T1.DSN,
                                                           CASE
                                                             WHEN T2.CODE = 'RD_DELIVERED' THEN 'Delivered'
                                                             WHEN T2.CODE = 'DSN_RD_RE_SENT' THEN 'Resent'
                                                             WHEN T2.CODE = 'REGISTRY_RECEIPT_COUNTED' THEN 'Counted'
                                                             ELSE ''
                                                           END                                         AS STATUS,
                                                           CASE
                                                             WHEN T2.CODE = 'RD_DELIVERED' THEN 1
                                                             WHEN T2.CODE = 'DSN_RD_RE_SENT' THEN 2
                                                             WHEN T2.CODE = 'REGISTRY_RECEIPT_COUNTED' THEN 3
                                                             ELSE 4
                                                           END                                         AS ORDERING,
                                                           T1.RD_DELIVERED_DATE                        AS ORDERING2
                                                    FROM   P_MW_DSN T1
                                                           INNER JOIN P_S_SYSTEM_VALUE T2
                                                                   ON T2.UUID = T1.SCANNED_STATUS_ID
                                                    WHERE  T1.SCANNED_STATUS_ID = T2.UUID
                                                           AND T2.CODE IN ( 'RD_DELIVERED', 'DSN_RD_RE_SENT', 'REGISTRY_RECEIPT_COUNTED' ))
                                             ";

        private const string SearchSD_q = @"SELECT DT,
                                                   T,
                                                   DSN,
                                                   RECORD_ID,
                                                   STATUS,
                                                   UUID
                                            FROM   (SELECT T1.MODIFIED_DATE AS DT, T1.UUID as UUID,
                                                           To_char(T1.MODIFIED_DATE, 'HH:MI')      AS T,
                                                           T1.DSN,
                                                           T1.RECORD_ID                            AS RECORD_ID,
                                                           CASE
                                                             WHEN T2.CODE = 'DSN_LETTER_CONFIRMED' THEN 'New'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH' THEN 'New'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_HOOK' THEN 'New'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH' THEN 'New'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_HOOK' THEN 'New'
                                                             ELSE ''
                                                           END                                     AS STATUS,
                                                           CASE
                                                             WHEN T2.CODE = 'DSN_LETTER_CONFIRMED' THEN 1
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH' THEN 2
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_HOOK' THEN 3
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH' THEN 4
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_HOOK' THEN 5
                                                             ELSE 4
                                                           END                                     AS ORDERING,
                                                           T1.MODIFIED_DATE                        AS ORDERING2
                                                    FROM   P_MW_DSN T1
                                                           INNER JOIN P_S_SYSTEM_VALUE T2
                                                                   ON T2.UUID = T1.SCANNED_STATUS_ID
                                                    WHERE  T1.SCANNED_STATUS_ID = T2.UUID
                                                           --AND T1.DSN LIKE 'XXX%'
                                                           --AND T1.MODIFIED_BY >= :START_DATE
                                                           --AND T1.MODIFIED_BY <= :END_DATE
                                                           AND T2.CODE IN ( 'DSN_LETTER_CONFIRMED', 'MWU_OUTGOING_NEW_DISPATCH', 'MWU_OUTGOING_NEW_HOOK', 'MWU_OUTGOING_RESENT_DISPATCH', 'MWU_OUTGOING_RESENT_HOOK' ))
                                                where 1=1 ";

        private const string SearchDSP_q = @"SELECT DT,
                                                   T,
                                                   DSN,
                                                   RECORD_ID,
                                                   STATUS
                                            FROM   (SELECT To_char(T1.CREATED_DATE, 'dd/MM/YYYY') AS DT,
                                                           To_char(T1.CREATED_DATE, 'HH:MI')      AS T,
                                                           T1.DSN,
                                                           T1.RECORD_ID                           AS RECORD_ID,
                                                           CASE
                                                             WHEN T2.CODE = 'REGISTRY_NEW' THEN 'Scanned'
                                                             WHEN T2.CODE = 'DSN_REGISTRY_DELIVERED_COUNTED' THEN 'Counted'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH_SCANNED' THEN 'Scanned'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH_SCANNED' THEN 'Scanned'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH_COUNTED' THEN 'Counted'
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH_COUNTED' THEN 'Counted'
                                                             ELSE ''
                                                           END                                    AS STATUS,
                                                           CASE
                                                             WHEN T2.CODE = 'REGISTRY_NEW' THEN 1
                                                             WHEN T2.CODE = 'DSN_REGISTRY_DELIVERED_COUNTED' THEN 2
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH_SCANNED' THEN 3
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH_SCANNED' THEN 4
                                                             WHEN T2.CODE = 'MWU_OUTGOING_NEW_DISPATCH_COUNTED' THEN 5
                                                             WHEN T2.CODE = 'MWU_OUTGOING_RESENT_DISPATCH_COUNTED' THEN 6
                                                             ELSE 4
                                                           END                                    AS ORDERING,
                                                           T1.CREATED_DATE                        AS ORDERING2
                                                    FROM   P_MW_DSN T1
                                                           INNER JOIN P_S_SYSTEM_VALUE T2
                                                                   ON T2.UUID = T1.SCANNED_STATUS_ID
                                                    WHERE  T1.SCANNED_STATUS_ID = T2.UUID
                                                           AND T2.CODE IN ( 'REGISTRY_NEW', 'DSN_REGISTRY_DELIVERED_COUNTED', 'MWU_OUTGOING_NEW_DISPATCH_SCANNED', 'MWU_OUTGOING_RESENT_DISPATCH_SCANNED',
                                                                            'MWU_OUTGOING_NEW_DISPATCH_COUNTED', 'MWU_OUTGOING_RESENT_DISPATCH_COUNTED' ))";

        #region Receipt
        public Fn02MWUR_ReceiptModel SearchReceipt(Fn02MWUR_ReceiptModel model)
        {
            model.Query = SearchReceipt_q;
            model.Sort = "To_Date(DT, 'dd/MM/YYYY')";
            model.Search();
            return model;
        }
        #endregion

        #region Scan and dispatch

        public Fn02MWUR_SDModel SearchSD(Fn02MWUR_SDModel model)
        {
            model.Query = SearchSD_q;
            model.Search();
            return model;
        }
        public Fn02MWUR_SDModel ExportSD(Fn02MWUR_SDModel model)
        {
            model.Query = SearchSD_q;

            return model;
        }

        public P_MW_DSN DetailSD(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from dsn in db.P_MW_DSN where dsn.UUID == uuid select dsn).FirstOrDefault();
            }
        }

        public Fn02MWUR_SDDisplayModel SearchDoc(Fn02MWUR_SDDisplayModel model)
        {
            model.Query = "Select UUID,DSN_SUB,DOCUMENT_TYPE,PAGE_COUNT,CREATED_DATE,FILE_PATH from P_MW_SCANNED_DOCUMENT ";
            model.QueryWhere = " Where DSN_ID = :UUID";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Search();
            return model;
        }

        public ServiceResult CompletScan(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from dsn in db.P_MW_DSN where dsn.UUID == uuid select dsn).FirstOrDefault();
                    if (record != null)
                    {
                        var systemValue = (from sys in db.P_S_SYSTEM_VALUE where sys.UUID == record.SCANNED_STATUS_ID select sys.CODE).FirstOrDefault();
                        if (systemValue == ProcessingConstant.MWU_OUTGOING_NEW_DISPATCH)
                        {
                            record.SCANNED_STATUS_ID = (from sys in db.P_S_SYSTEM_VALUE where sys.CODE == ProcessingConstant.MWU_OUTGOING_NEW_DISPATCH_SCANNED select sys.UUID).FirstOrDefault();
                        }
                        else if (systemValue == ProcessingConstant.MWU_OUTGOING_NEW_HOOK)
                        {
                            record.SCANNED_STATUS_ID = (from sys in db.P_S_SYSTEM_VALUE where sys.CODE == ProcessingConstant.MWU_OUTGOING_NEW_HOOK_SCANNED select sys.UUID).FirstOrDefault();
                        }
                        else if (systemValue == ProcessingConstant.MWU_OUTGOING_RESENT_DISPATCH)
                        {
                            record.SCANNED_STATUS_ID = (from sys in db.P_S_SYSTEM_VALUE where sys.CODE == ProcessingConstant.MWU_OUTGOING_RESENT_DISPATCH_SCANNED select sys.UUID).FirstOrDefault();
                        }
                        else if (systemValue == ProcessingConstant.MWU_OUTGOING_RESENT_HOOK)
                        {
                            record.SCANNED_STATUS_ID = (from sys in db.P_S_SYSTEM_VALUE where sys.CODE == ProcessingConstant.MWU_OUTGOING_RESENT_HOOK_SCANNED select sys.UUID).FirstOrDefault();
                        }
                        else
                        {
                            record.SCANNED_STATUS_ID = (from sys in db.P_S_SYSTEM_VALUE where sys.CODE == ProcessingConstant.DSN_REGISTRY_NEW select sys.UUID).FirstOrDefault();
                        }
                        record.MODIFIED_DATE = DateTime.Now;
                        if (record.SUBMIT_TYPE == ProcessingConstant.SUBMIT_TYPE_UNKNOWN)
                        {
                            record.RE_ASSIGN = ProcessingConstant.FLAG_Y;
                        }
                        else
                        {
                            record.RE_ASSIGN = ProcessingConstant.FLAG_N;
                        }
                    }
                    else
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { "Record does not exist." } };
                    }
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
                        AuditLogService.Log("CompletScan", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("CompletScan", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                        Message = { "Update Successfully." }
                    };
                }


            }
        }

        #endregion

        #region Dispatch
        public Fn02MWUR_DSPModel SearchDispatch(Fn02MWUR_DSPModel model)
        {
            model.Query = SearchDSP_q;
            if (!string.IsNullOrEmpty(model.DSN))
                model.QueryWhere = @" Where  DSN like '%" + model.DSN + "%'";
            model.Search();
            return model;
        }
        #endregion

        public Fn02MWUR_ReceiptModel confirmOutstanding(Fn02MWUR_ReceiptModel model)
        {
            // model.QueryWhere = SearchSD_whereq(model);
            return model;
        }

        #region MW Record Address Mapping
        public Fn02MWUR_RAMSearchModel SearchMWRecordAddressMapping(Fn02MWUR_RAMSearchModel model)
        {
            model.Query = @"SELECT R.UUID,
                                   RN.REFERENCE_NO,
                                   P_get_item_code_by_record_id2(R.UUID) ITEMCODE,
                                   R.COMMENCEMENT_DATE,
                                   R.COMPLETION_DATE,
                                   R.LOCATION_OF_MINOR_WORK,
                                   R.MW_DSN
                            FROM   P_MW_RECORD R
                                   JOIN P_MW_REFERENCE_NO RN
                                     ON R.REFERENCE_NUMBER = RN.UUID
                                        AND R.IS_DATA_ENTRY = 'N'
                                   LEFT JOIN (SELECT MW_RECORD_ID,
                                                     Listagg(FILEREF_FOUR, '/')
                                                       WITHIN GROUP(ORDER BY MW_RECORD_ID) AS FILEREF_FOUR_2,
                                                     Listagg(FILEREF_TWO, '/')
                                                       WITHIN GROUP(ORDER BY MW_RECORD_ID) AS FILEREF_TWO_2,
                                                     Listagg(BLK_ID, '/')
                                                       WITHIN GROUP(ORDER BY MW_RECORD_ID) AS BLK_ID
                                              FROM   P_MW_FILEREF
                                              GROUP  BY MW_RECORD_ID) FR
                                          ON RN.REFERENCE_NO = FR.MW_RECORD_ID ";
            model.Search();
            return model;
        }

        public Fn02MWUR_RAMModel GetMWRecordDetail(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                ComSubmissionDAOService com = new ComSubmissionDAOService();
                var model = (from r in db.P_MW_RECORD
                             join rn in db.P_MW_REFERENCE_NO on r.REFERENCE_NUMBER equals rn.UUID
                             where r.IS_DATA_ENTRY == "N" && r.UUID == uuid
                             select new Fn02MWUR_RAMModel()
                             {
                                 MWRecordID = uuid
                                 , RefNo = rn.REFERENCE_NO
                                 , CommencementDate = r.COMMENCEMENT_DATE.Value
                                 , CompletionCondintionalDate = r.COMPLETION_CONDINTIONAL_DATE.Value
                                 , FileReference = r.FILE_REFERENCE
                                 , CompletionAcknowledgeDate = r.COMPLETION_ACKNOWLEDGE_DATE.Value
                                 , LocationOfMinorWork = r.LOCATION_OF_MINOR_WORK
                                 , IsCaseTransferToBravo = ProcessingConstant.RRM_SYN_COMPLETE.Equals(r.RRM_SYN_STATUS) ? ProcessingConstant.FLAG_Y : ProcessingConstant.FLAG_N
                             }).FirstOrDefault();
                model.P_MW_SCANNED_DOCUMENTsNIC = com.GetP_MW_SCANNED_DOCUMENTsNICByRefNO(model.RefNo);
                model.P_MW_SCANNED_DOCUMENTsIC = com.GetP_MW_SCANNED_DOCUMENTsICByRefNO(model.RefNo);
                return model;

            }
        }
        #endregion

        public int DeleteFileRef(EntitiesMWProcessing db, string uuid)
        {
            P_MW_FILEREF record = db.P_MW_FILEREF.Where(w => w.UUID == uuid).FirstOrDefault();

            if (record != null)
            {
                db.P_MW_FILEREF.Remove(record);
            }

            return db.SaveChanges();
        }

        public int AddFileRef(EntitiesMWProcessing db, P_MW_FILEREF model)
        {
            db.P_MW_FILEREF.Add(model);
            return db.SaveChanges();
        }

        public int UpdateFileRef(EntitiesMWProcessing db, P_MW_FILEREF model)
        {
            P_MW_FILEREF record = db.P_MW_FILEREF.Where(w => w.UUID == model.UUID).FirstOrDefault();
            if (record != null)
            {
                record.FILEREF_FOUR = model.FILEREF_FOUR;
                record.FILEREF_TWO = model.FILEREF_TWO;
                record.BLK_ID = model.BLK_ID;
                record.UNIT_ID = model.UNIT_ID;
            }

            return db.SaveChanges();
        }

    }
}