using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MWMS2.Services.AdminService.DAO
{
    public class PEM11DAOService
    {
        #region
        public DSNSearchModel DSNDetail(string UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return new DSNSearchModel() { P_MW_DSN = (from dsn in db.P_MW_DSN where dsn.UUID == UUID select dsn).FirstOrDefault() };
            }
        }

        public ServiceResult SaveDSN(DSNSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = db.P_MW_DSN.Where(m => m.UUID == model.P_MW_DSN.UUID).FirstOrDefault();
                    if (record != null)
                    {
                        record.SUBMISSION_NATURE = model.P_MW_DSN.SUBMISSION_NATURE;
                    }
                    else
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE };
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
                        AuditLogService.Log("SaveDSN", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("SaveDSN", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        #endregion

        #region PEM1110 Import Records
        public PEM1110ImportRecordsModel SearchImportRecords(PEM1110ImportRecordsModel model)
        {
            model.Query = @"    SELECT
                                T.UUID AS UUID
                                , t.FILE_NAME AS FILE_NAME
                                , TO_CHAR(t.CREATED_DATE, 'DD/MM/YYYY HH24:MI:SS') AS CREATED_DATE
                                , t.CREATED_BY AS CREATED_BY
                                , ssv.DESCRIPTION AS STATUS

                                , CASE WHEN t.STATUS = 'UPLOADED' THEN (
                                    SELECT COUNT(*)
                                    FROM P_MW_IMPORTED_TASK_ITEM item
                                    WHERE item.mw_imported_task_id = t.UUID AND item.STATUS <> 'UPLOAD_VALID'
                                ) ELSE (
                                    SELECT COUNT(*)
                                    FROM P_MW_IMPORTED_TASK_ITEM item
                                    WHERE item.mw_imported_task_id = t.UUID AND item.STATUS <> 'IMPORT_VALID'
                                ) END AS INVALID
                                , CASE WHEN t.STATUS = 'UPLOADED' THEN (
                                    SELECT COUNT(*)
                                    FROM P_MW_IMPORTED_TASK_ITEM item
                                    WHERE item.mw_imported_task_id = t.UUID AND item.STATUS = 'UPLOAD_VALID'
                                ) ELSE (
                                    SELECT COUNT(*)
                                    FROM P_MW_IMPORTED_TASK_ITEM item
                                    WHERE item.mw_imported_task_id = t.UUID AND item.STATUS = 'IMPORT_VALID'
                                ) END AS VALID
                                , (
                                    SELECT COUNT(*)
                                    FROM P_MW_IMPORTED_TASK_ITEM item
                                    WHERE item.mw_imported_task_id = t.UUID
                                ) AS TOTAL
                                ,'View' As RESULT
                                FROM P_MW_IMPORTED_TASK t, P_S_SYSTEM_VALUE ssv, P_S_SYSTEM_TYPE sst";
            model.QueryWhere = @"WHERE ssv.CODE = t.STATUS AND ssv.SYSTEM_TYPE_ID = sst.UUID
                                AND sst.TYPE = 'IMPORT_STATUS'";
            model.Sort = " t.CREATED_DATE ";
            model.SortType = 1;
            model.Search();
            return model;
        }

        public ServiceResult ImportTaskRecordsExcel(PEM1110ImportRecordsModel model)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    db.P_MW_IMPORTED_TASK.Add(model.p_MW_IMPORTED_TASK);
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
                        AuditLogService.Log("ImportTaskRecordsExcel", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("ImportTaskRecordsExcel", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }

            }
        }

        public ServiceResult ImportTaskItemRecordsExcel(PEM1110ImportRecordsModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    foreach(var item in model.p_MW_IMPORTED_TASK_ITEMList)
                    {
                        db.P_MW_IMPORTED_TASK_ITEM.Add(item);
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
                        AuditLogService.Log("ImportTaskItemRecordsExcel", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("ImportTaskItemRecordsExcel", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        public ServiceResult DeleteMWRecords(PEM1110ImportRecordsModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from task in db.P_MW_IMPORTED_TASK
                                  where task.UUID == model.UUID
                                  select task).FirstOrDefault();
                    if(record != null)
                    {
                        var itemRecord = (from taskItem in db.P_MW_IMPORTED_TASK_ITEM
                                          where taskItem.MW_IMPORTED_TASK_ID == record.UUID
                                          select taskItem).ToList();
                        foreach(var item in itemRecord)
                        {
                            db.P_MW_IMPORTED_TASK_ITEM.Remove(item);
                        }

                        db.P_MW_IMPORTED_TASK.Remove(record);
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
                        AuditLogService.Log("DeleteMWRecords", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("DeleteMWRecords", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,Message = { "Delete Successfully." }
                    };
                }
            }
        }

        public ServiceResult UpdateFileStatus(PEM1110ImportRecordsModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from task in db.P_MW_IMPORTED_TASK
                                  where task.UUID == model.UUID
                                  select task).FirstOrDefault();
                    if(record != null)
                    {
                        record.STATUS = ProcessingConstant.S_VAL_IMPORTED;
                    }
                    else
                    {
                        return new ServiceResult
                        {
                            Result = ServiceResult.RESULT_FAILURE,
                            Message = { "Import Failed." }
                        };
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
                        AuditLogService.Log("UpdateFileStatus", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("UpdateFileStatus", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,Message = { "Import Successfully." }
                    };

                }
            }
        }
        #endregion

        #region PEM1114 MW Number Mapping
        public PEM1114MWNumberMappingModel SearchMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            model.Query = @"Select * from P_Mw_Reference_No";
            model.Search();
            return model;
        }

        public PEM1114MWNumberMappingModel SearchMWNumberUserName(PEM1114MWNumberMappingModel model)
        {
            
            model.Query = @"Select sp.uuid, sp.bd_portal_login,
                            case when (select Count(sl.reference_no) from P_S_MW_NO_SECURITY_LEVEL sl Where sl.reference_no = :Reference_No And sp.uuid = to_char(sl.user_id)) > 0
                            Then 1 Else 0 End AS IsCheck 
                            from SYS_POST sp";
            model.QueryParameters.Add("Reference_No", model.Reference_No);
            model.Search();
            return model;
        }

        public ServiceResult SaveMWNumberMapping(PEM1114MWNumberMappingModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from sl in db.P_S_MW_NO_SECURITY_LEVEL where sl.REFERENCE_NO == model.Reference_No select sl).ToList();
                    if (record.Count > 0)
                    {
                        foreach (var item in record)
                        {
                            db.P_S_MW_NO_SECURITY_LEVEL.Remove(item);
                        }
                    }
                    foreach (var item in model.UserIDList)
                    {
                        if(Regex.IsMatch(item, @"^[+-]?\d*$"))
                        {
                            P_S_MW_NO_SECURITY_LEVEL SecurityLevel = new P_S_MW_NO_SECURITY_LEVEL();
                            SecurityLevel.UUID = Guid.NewGuid().ToString("N");
                            SecurityLevel.REFERENCE_NO = model.Reference_No;
                            SecurityLevel.USER_ID = Convert.ToDecimal(item);
                            SecurityLevel.CREATED_BY = "Admin";
                            SecurityLevel.CREATED_DATE = DateTime.Now;
                            SecurityLevel.MODIFIED_BY = "Admin";
                            SecurityLevel.MODIFIED_DATE = DateTime.Now;
                            db.P_S_MW_NO_SECURITY_LEVEL.Add(SecurityLevel);
                        }
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
                        AuditLogService.Log("SaveMWNumberMapping", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("SaveMWNumberMapping", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,Message = { "Save Successfully." }
                    };
                }
              
            }

        }

        #endregion

        #region PEM1112 Pending Transfer

        public PEM1112PendingTransferModel SearchPendingTransfer(PEM1112PendingTransferModel model)
        {
            model.Search();
            return model;
        }


        public ServiceResult SavePendingTransfer(PEM1112PendingTransferModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var uuidlist = (from d in db.P_MW_DSN where model.RefNoList.Contains(d.RECORD_ID) select d.UUID).ToList();
                    var records = (from sd in db.P_MW_SCANNED_DOCUMENT
                                   where (sd.RRM_SYN_STATUS == null || sd.RRM_SYN_STATUS != ProcessingConstant.RRM_SYN_COMPLETE)
                                   && uuidlist.Contains(sd.DSN_ID) && sd.FOLDER_TYPE == ProcessingConstant.DSN_FOLDER_TYPE_PUBLIC
                                   select sd).ToList();
                    
                    foreach(var item in records)
                    {
                        item.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
                        item.MODIFIED_DATE = DateTime.Now;
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
                        AuditLogService.Log("SavePendingTransfer", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("SavePendingTransfer", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                        Message = { "Transfer Successfully." }
                    };

                }
            }
        }
        #endregion

        #region Update Submission Record
        public ServiceResult SearchSubmissionRecord(string refNo)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var record = (from re in db.P_MW_RECORD
                              join refno in db.P_MW_REFERENCE_NO on re.REFERENCE_NUMBER equals refno.UUID
                              where re.IS_DATA_ENTRY == "N" && refno.REFERENCE_NO == refNo
                        select re).FirstOrDefault();
                if (record != null)
                {
                    return new ServiceResult { Result = ServiceResult.RESULT_SUCCESS, Data = record.UUID };
                }
                else
                    return new ServiceResult { Result = ServiceResult.RESULT_FAILURE, Message = { " Record not found. " } };
            }
        }

        public PEM1103UpdateSubmissionRecordModel GetSubmissionRecordDetail(string uuid)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                PEM1103UpdateSubmissionRecordModel model = new PEM1103UpdateSubmissionRecordModel();
                model.UUID = uuid;
                model.MwRecordMinorWorkItemList = (from re in db.P_MW_RECORD
                                          join item in db.P_MW_RECORD_ITEM on re.UUID equals item.MW_RECORD_ID
                                          where re.UUID == uuid
                                          select item).ToList();
                model.AppointedProfessionalList = (from re in db.P_MW_RECORD
                                                   join professional in db.P_MW_APPOINTED_PROFESSIONAL on re.UUID equals professional.MW_RECORD_ID
                                                   where re.UUID == uuid
                                                   select professional).ToList();
                return model;
            }
        }

        public PEM1103UpdateSubmissionRecordModel GetIssuedCorrespondence(PEM1103UpdateSubmissionRecordModel model)
        {
            model.Query = @"Select document.UUID,document.DOCUMENT_TYPE,document.CREATED_DATE,document.DSN_SUB,document.DOC_TITLE,
                            document.FILE_PATH from P_MW_Record record inner join P_MW_DSN dsn on record.MW_DSN = dsn.RECORD_ID
                         inner join P_MW_SCANNED_DOCUMENT document on dsn.uuid = document.DSN_ID";
            model.QueryWhere = @" Where record.UUID = :UUID";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Search();
            return model;
        }

        public PEM1103UpdateSubmissionRecordModel GetSubmissionOfLetterModule(PEM1103UpdateSubmissionRecordModel model)
        {
            model.Query = @"Select ack.DSN,ack.FORM_NO,ack.COMM_DATE,ack.COMP_DATE,ack.MW_ITEM,ack.PBP_NO,ack.PRC_NO,ack.REMARK 
                           from  P_MW_Record record inner join P_MW_ACK_LETTER ack on record.MW_DSN = ack.DSN" ;
            model.QueryWhere = @" Where record.UUID = :UUID";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Search();
            return model;
        }
        #endregion
    }
}