using MWMS2.Areas.Admin.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MWMS2.Services.AdminService.DAO
{
    public class PEM1103DAOService
    {
        #region Number Prefix
        public PEM1103MWNumberPrefixModel GetMWNumberPrefix(PEM1103MWNumberPrefixModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var record = (from sv in db.P_S_SYSTEM_VALUE
                              join st in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                              where st.TYPE == ProcessingConstant.MW_NO_PREFIX
                              select sv).FirstOrDefault();
                if(record != null)
                {
                    model.Code = Convert.ToInt32(record.CODE);
                }
                return model;
            }
        }

        public ServiceResult SaveMWNumberPrefix(PEM1103MWNumberPrefixModel model)
        {
            using(EntitiesMWProcessing db =new EntitiesMWProcessing())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from sv in db.P_S_SYSTEM_VALUE
                                  join st in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                  where st.TYPE == ProcessingConstant.MW_NO_PREFIX
                                  select sv).FirstOrDefault();

                    if (model.Code <= Convert.ToInt32(record.CODE))
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "New prefix must be greater then current prefix." } };
                    }

                    record.CODE = model.Code.ToString();
                    record.MODIFIED_DATE = DateTime.Now;
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
                    return new ServiceResult { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Save Successfully." } };
                }


            }
        }

        #endregion

        #region Import MW Item
        public PEM1103ImportMWItemModel SearchImportMWItem(PEM1103ImportMWItemModel model)
        {
            model.Query = @"select UUID,ITEM_NO,CHI_DESCRIPTION,ENG_DESCRIPTION,TRANSFER_RRM from P_S_import_MW_ITEM ";
            model.QueryWhere = @" where 1=1  ";
            model.Search();
            return model;
        }

        public PEM1103ImportMWItemModel SearchDetailImportMWItem(string uuid)
        {
            using(EntitiesMWProcessing db =new EntitiesMWProcessing())
            {
                return (from mw in db.P_S_IMPORT_MW_ITEM where mw.UUID == uuid
                        select new PEM1103ImportMWItemModel
                        {
                              UUID = mw.UUID 
                            , Item_No = mw.ITEM_NO
                            , English_Description = mw.ENG_DESCRIPTION
                            , Chinese_Description = mw.CHI_DESCRIPTION
                            ,
                            Image_Not_Transfer_To_RRM = mw.TRANSFER_RRM == "N" ? false : true
                        }).FirstOrDefault();
            }
        }

        public ServiceResult UpdateImportMwItemDescription(PEM1103ImportMWItemModel model)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = db.P_S_IMPORT_MW_ITEM.Where(o => o.UUID == model.UUID).FirstOrDefault();
                    if(record != null)
                    {
                        record.ITEM_NO = model.Item_No;
                        record.ENG_DESCRIPTION = model.English_Description;
                        record.CHI_DESCRIPTION = model.Chinese_Description;
                        record.TRANSFER_RRM = model.Image_Not_Transfer_To_RRM == true ? "Y" : "N";
                    }
                    else
                    {
                        var existRecord = db.P_S_IMPORT_MW_ITEM.Where(o => o.ITEM_NO == model.Item_No).FirstOrDefault();
                        if (existRecord == null)
                        {
                            P_S_IMPORT_MW_ITEM newRecord = new P_S_IMPORT_MW_ITEM();
                            newRecord.UUID = Guid.NewGuid().ToString("N");
                            newRecord.ITEM_NO = model.Item_No;
                            newRecord.ENG_DESCRIPTION = model.English_Description;
                            newRecord.CHI_DESCRIPTION = model.Chinese_Description;
                            newRecord.TRANSFER_RRM = model.Image_Not_Transfer_To_RRM == true ? "Y" : "N";
                            newRecord.CREATED_DATE = DateTime.Now;
                            newRecord.CREATED_BY = "SYSTEM";
                            newRecord.MODIFIED_BY = "SYSTEM";
                            newRecord.MODIFIED_DATE = DateTime.Now;
                            db.P_S_IMPORT_MW_ITEM.Add(newRecord);
                        }
                        else
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "Item no already exists." } };
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Save Successfully." } };
                }
            }
        }

        #endregion

        #region  Audit Percentage
        public PEM1103AuditPercentageModel SearchAuditPercentage()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_MW01", "Class 1");
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_MW03", "Class 2");
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_MW05", "Class 3");
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_MW06", "VS");
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_PSAC", "PSAC");
                dict.Add("ACK_LETTER_AUDIT_PERCENTAGE_SAC", "SAC");
                var SVList = (from sv in db.P_S_SYSTEM_VALUE
                              join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                              where stype.TYPE == ProcessingConstant.ACK_LETTER_AUDIT_PERCENTAGE
                              orderby sv.CODE
                              select sv).ToList();
                foreach(var item in SVList)
                {
                    if (dict.Keys.Contains(item.CODE))
                        item.CODE = dict[item.CODE];
                }
                return new PEM1103AuditPercentageModel { SVList = SVList };
            }
        }

        public ServiceResult SaveAuditPercentage(PEM1103AuditPercentageModel model)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    foreach(var item in model.SVList)
                    {
                        var record = db.P_S_SYSTEM_VALUE.Where(o => o.UUID == item.UUID).FirstOrDefault();
                        if(record != null)
                        {
                            record.ORDERING = item.ORDERING;
                            record.DESCRIPTION = item.DESCRIPTION;
                            record.MODIFIED_DATE = DateTime.Now;
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                     {
                        Result = ServiceResult.RESULT_SUCCESS,
                        Message = new List<string>() { "Save Successfully." }
                     };
                    }
            }
        }

        #endregion

        #region Number Validated Structrues
        public PEM1103NumValidatedStructuresModel SearchNumberValidatedStructrues()
        {
            using(EntitiesMWProcessing db =new EntitiesMWProcessing())
            {

                string year = DateTime.Now.Year.ToString();
                var record = (from sv in db.P_S_SYSTEM_VALUE
                        join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                        where stype.TYPE ==  ProcessingConstant.S_TYPE_SDM_NO_OF_VALIDATE && sv.CODE == year
                        select sv).FirstOrDefault();


                if (record != null)
                { 
                    List<string> strList = record.DESCRIPTION.Split(new string[] { ProcessingConstant.CUSTOM_DELIMITER }, StringSplitOptions.None).ToList<string>();
                    return new PEM1103NumValidatedStructuresModel()
                    {
                        UUID= record.UUID
                        ,
                        Number = strList.Count == 3 ? strList[2] : null
                    };
                }
                else
                    return new PEM1103NumValidatedStructuresModel();


            }
        }

        public ServiceResult UpdateNumberValidatedStructrues(PEM1103NumValidatedStructuresModel model)
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    string year = DateTime.Now.Year.ToString();
                    var record = (from sv in db.P_S_SYSTEM_VALUE
                                  join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                                  where stype.TYPE == ProcessingConstant.S_TYPE_SDM_NO_OF_VALIDATE && sv.CODE == year
                                  select sv).FirstOrDefault();

                    if (record != null)
                    {
                        //var record = (from sv in db.P_S_SYSTEM_VALUE
                        //              join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                        //              where stype.TYPE == ProcessingConstant.S_TYPE_SDM_NO_OF_VALIDATE && sv.UUID == model.UUID
                        //              select sv).FirstOrDefault();
                        record.DESCRIPTION = "1/1" + DateTime.Now.Year.ToString() + ProcessingConstant.CUSTOM_DELIMITER + DateTime.Now.ToString("dd/MM/yyyy") + ProcessingConstant.CUSTOM_DELIMITER + model.Number;
                    }
                    else
                    {
                        P_S_SYSTEM_VALUE sv = new P_S_SYSTEM_VALUE()
                        {
                            UUID = Guid.NewGuid().ToString("N")
                            ,
                            SYSTEM_TYPE_ID = (from stype in db.P_S_SYSTEM_TYPE where stype.TYPE == ProcessingConstant.S_TYPE_SDM_NO_OF_VALIDATE select stype.UUID).FirstOrDefault()
                            ,
                            CODE = DateTime.Now.Year.ToString()
                            ,
                            PARENT_ID = (from stype in db.P_S_SYSTEM_TYPE where stype.TYPE == ProcessingConstant.S_TYPE_SDM_NO_OF_VALIDATE select stype.UUID).FirstOrDefault()
                            ,
                            DESCRIPTION = "1/1/" + DateTime.Now.Year.ToString() + ProcessingConstant.CUSTOM_DELIMITER + DateTime.Now.ToString("dd/MM/yyyy") + ProcessingConstant.CUSTOM_DELIMITER + model.Number
                            ,
                            ORDERING = Convert.ToDecimal(DateTime.Now.Year.ToString())
                            ,
                            CREATED_BY = "ADMIN"
                            ,
                            CREATED_DATE = DateTime.Now
                            ,
                            MODIFIED_BY = "ADMIN"
                            ,
                            MODIFIED_DATE = DateTime.Now
                        };

                        db.P_S_SYSTEM_VALUE.Add(sv);
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
                   
            }
        }

        #endregion

        #region  Day Back
        public PEM1103DayBackModel SearchDayBack()
        {
            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from sv in db.P_S_SYSTEM_VALUE
                        join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                        where stype.TYPE == ProcessingConstant.S_TYPE_PRECOMM_SITE_AUDIT
                        select new PEM1103DayBackModel() { DayBack = sv.DESCRIPTION  }).FirstOrDefault();
            }
        }

        public ServiceResult UpdateDayBack(PEM1103DayBackModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from sv in db.P_S_SYSTEM_VALUE
                                  join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                                  where stype.TYPE == ProcessingConstant.S_TYPE_PRECOMM_SITE_AUDIT
                                  select sv).FirstOrDefault();
                    if(record != null)
                    {
                        record.DESCRIPTION = model.DayBack;
                        record.MODIFIED_DATE = DateTime.Now;
                    }
                    else
                        return new ServiceResult
                        {
                            Result = ServiceResult.RESULT_FAILURE,
                        };
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
                        AuditLogService.Log("UpdateDayBack", validationError.Message);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("UpdateDayBack", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        #endregion

        #region Ack Letter Template Signature / Remind Letter Template Signature 
        public PEM1103AckLetterTemplateSignatureModel SearchAckLetterTemplateSignature(string letterType)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var code = new[] { ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION,
                                    ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION };
                var record = (from sv in db.P_S_SYSTEM_VALUE
                                     join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                                     where stype.TYPE == letterType 
                                 select sv).ToList();
                if (record.Count > 0)
                {
                    return new PEM1103AckLetterTemplateSignatureModel()
                    {
                        SPO_Eng_Name = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME).FirstOrDefault().DESCRIPTION
                    ,
                        SPO_Eng_Position = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION).FirstOrDefault().DESCRIPTION
                    ,
                        SPO_Chi_Name = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME).FirstOrDefault().DESCRIPTION
                    ,
                        SPO_Chi_Position = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION).FirstOrDefault().DESCRIPTION
                    };
                }
                else
                    return new PEM1103AckLetterTemplateSignatureModel();


            }
        }

        public ServiceResult UpdateSearchAckLetterTemplateSignature(PEM1103AckLetterTemplateSignatureModel model,string letterType)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var code = new[] { ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION,
                                    ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION };
                    var recordList = (from sv in db.P_S_SYSTEM_VALUE
                                  join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
                                  where stype.TYPE == letterType 
                                  select sv).ToList();
                    var record1 = recordList.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME).FirstOrDefault();
                    var record2 = recordList.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION).FirstOrDefault();
                    var record3 = recordList.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME).FirstOrDefault();
                    var record4 = recordList.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION).FirstOrDefault();

                    if (!string.IsNullOrEmpty(model.SPO_Eng_Name))
                    {
                        record1.DESCRIPTION = model.SPO_Eng_Name;
                        record1.MODIFIED_DATE = DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(model.SPO_Eng_Position))
                    {
                        record2.DESCRIPTION = model.SPO_Eng_Position;
                        record2.MODIFIED_DATE = DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(model.SPO_Chi_Name))
                    {
                        record3.DESCRIPTION = model.SPO_Chi_Name;
                        record3.MODIFIED_DATE = DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(model.SPO_Chi_Position))
                    {
                        record4.DESCRIPTION = model.SPO_Chi_Position;
                        record4.MODIFIED_DATE = DateTime.Now;
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
                
            }
        }

        #endregion

        #region Public Holiday
        public PEM1103PublicHolidayModel GetPublicHolidayData(PEM1103PublicHolidayModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                int year = Convert.ToInt32(model.Year);
                var holidays = (from ph in db.P_S_PUBLIC_HOLIDAY
                            where ph.HOLIDAY.Year == year
                            select ph).ToList();
                model.holidays = new List<Holiday>();
                foreach(var item  in holidays)
                {
                    model.holidays.Add(new Holiday() { UUID = item.UUID, Date = item.HOLIDAY.ToString(), HolidayName_Desc = item.DESCRIPTION });
                }
                //model.holidays = ( from ph in db.P_S_PUBLIC_HOLIDAY where ph.HOLIDAY.Year == year
                //                   select
                //                   new Holiday()
                //                   {
                //                       Date = ph.HOLIDAY.ToString(),
                //                       HolidayName_Desc = ph.DESCRIPTION
                //                   }).ToList();
                return model;
            }
        }
        public ServiceResult SavePublicHolidays(PEM1103PublicHolidayModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    foreach(var item in model.holidays)
                    {
                        if (!string.IsNullOrEmpty(item.Date))
                        {
                            DateTime holidayDate = Convert.ToDateTime(item.Date);
                            var record = (from ph in db.P_S_PUBLIC_HOLIDAY
                                          where ph.HOLIDAY == holidayDate
                                          select ph).FirstOrDefault();
                            if (record != null)
                            {
                                record.DESCRIPTION = item.HolidayName_Desc;
                                record.MODIFIED_DATE = DateTime.Now;
                                record.MODIFIED_BY = "System";
                            }
                            else
                            {
                                db.P_S_PUBLIC_HOLIDAY.Add(new P_S_PUBLIC_HOLIDAY()
                                {
                                    UUID = Guid.NewGuid().ToString("N")
                                    ,
                                    HOLIDAY = Convert.ToDateTime(item.Date)
                                    ,
                                    DESCRIPTION = item.HolidayName_Desc
                                    ,
                                    CREATED_DATE = DateTime.Now
                                    ,
                                    CREATED_BY = "System"
                                    ,
                                    MODIFIED_DATE = DateTime.Now
                                    ,
                                    MODIFIED_BY = "System"
                                });
                            }
                           
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
                        AuditLogService.Log("SavePublicHolidays", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("SavePublicHolidays", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }

                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        public ServiceResult DeleteHoliday(string UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from ph in db.P_S_PUBLIC_HOLIDAY
                                  where ph.UUID == UUID
                                  select ph).FirstOrDefault();
                    if(record != null)
                    {
                        db.P_S_PUBLIC_HOLIDAY.Remove(record);
                    }
                    else
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE, Message = { "Record does not exist." } };
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
                        AuditLogService.Log("DeleteHoliday", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("DeleteHoliday", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }

                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        #endregion

        #region Number of daily Direct Return Over Counter
        public PEM1103NoOfDirectReturnModel SearchNoOfDailyDirectReturnData(PEM1103NoOfDirectReturnModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                foreach(var item in model.calendarModels)
                {
                    DateTime date = Convert.ToDateTime(item.Date);
                    var record = (from ddroc in db.P_S_DAILY_DIRECT_RT_OVER_CNT
                                  where ddroc.RECEIVED_DATE == date
                                  select ddroc).FirstOrDefault();
                    if(record != null)
                    {
                        item.Counter = record.COUNT.Value;
                    }

                }

                return model;
            }
        }

        public ServiceResult SaveNoOfDirectReturn(PEM1103NoOfDirectReturnModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    foreach(var item in model.calendarModels)
                    {
                        DateTime date = Convert.ToDateTime(item.Date);
                        var record = (from ddroc in db.P_S_DAILY_DIRECT_RT_OVER_CNT
                                      where ddroc.RECEIVED_DATE == date
                                      select ddroc).FirstOrDefault();
                        if(record != null)
                        {
                            record.COUNT = item.Counter;
                            record.MODIFIED_DATE = DateTime.Now;
                            record.MODIFIED_BY = "System";
                        }
                        else
                        {
                            db.P_S_DAILY_DIRECT_RT_OVER_CNT.Add(new P_S_DAILY_DIRECT_RT_OVER_CNT()
                            {
                                UUID = Guid.NewGuid().ToString("N")
                                ,
                                RECEIVED_DATE = date
                                , COUNT = item.Counter
                                , CREATED_BY = "System"
                                , CREATED_DATE = DateTime.Now
                                , MODIFIED_BY = "System"
                                , MODIFIED_DATE = DateTime.Now
                            });
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
                        AuditLogService.Log("SaveNoOfDirectReturn", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("SaveNoOfDirectReturn", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };

                }
            }
        }

        #endregion

        #region To Details
        public List<PEM1103ToDetailListModel> GetToDetails()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from td in db.P_S_TO_DETAILS 
                        select new PEM1103ToDetailListModel()
                        {
                            UUID = td.UUID
                            ,TO_POST = td.TO_POST
                            ,TO_POST_ENG = td.TO_POST_ENG
                            ,TO_POST_CHI = td.TO_POST_CHI
                            ,TO_NAME_ENG = td.TO_NAME_ENG
                            ,TO_NAME_CHI = td.TO_NAME_CHI
                            ,TO_CONTACT = td.TO_CONTACT
                            ,
                            PO_POST = td.PO_POST
                            ,
                            PO_POST_ENG = td.PO_POST_ENG
                            ,
                            PO_POST_CHI = td.PO_POST_CHI
                            ,
                            PO_NAME_ENG = td.PO_NAME_ENG
                            ,
                            PO_NAME_CHI = td.PO_NAME_CHI
                            ,
                            PO_CONTACT = td.PO_CONTACT
                            ,SPO_POST = td.SPO_POST
                            ,
                            SPO_POST_ENG = td.SPO_POST_ENG
                            ,
                            SPO_POST_CHI = td.SPO_POST_CHI
                            ,
                            SPO_NAME_ENG = td.SPO_NAME_ENG
                            ,
                            SPO_NAME_CHI = td.SPO_NAME_CHI
                            ,
                            SPO_CONTACT = td.SPO_CONTACT
                            ,ISACTIVE  = td.IS_ACTIVE
                            ,IsCheckActive = td.IS_ACTIVE == "Y" ? true : false

                        }).ToList();
            }
        }

        public ServiceResult DeleteToDetails(string UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var record = (from td in db.P_S_TO_DETAILS where td.UUID == UUID select td).FirstOrDefault();
                    if(record != null)
                    {
                        db.P_S_TO_DETAILS.Remove(record);
                    }
                    else
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE, Message = { "Record does not exist." } };
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
                        AuditLogService.Log("DeleteToDetails", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("DeleteToDetails", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }

                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        public ServiceResult SaveToDetails(PEM1103ToDetailsModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var count = (from td in db.P_S_TO_DETAILS select td).ToList().Count();
                    foreach (var item in model.pEM1103ToDetailsListModels)
                    {
                        var record = (from td in db.P_S_TO_DETAILS where td.UUID == item.UUID select td).FirstOrDefault();
                        if(record != null)
                        {
                            

                            record.TO_POST = item.TO_POST;
                            record.TO_POST_ENG = item.TO_POST_ENG;
                            record.TO_POST_CHI = item.TO_POST_CHI;
                            record.TO_NAME_ENG = item.TO_NAME_ENG;
                            record.TO_NAME_CHI = item.TO_NAME_CHI;
                            record.TO_CONTACT = item.TO_CONTACT;
                            record.PO_POST = item.PO_POST;
                            record.PO_POST_ENG = item.PO_POST_ENG;
                            record.PO_POST_CHI = item.PO_POST_CHI;
                            record.PO_NAME_ENG = item.PO_NAME_ENG;
                            record.PO_NAME_CHI = item.PO_NAME_CHI;
                            record.PO_CONTACT = item.PO_CONTACT;
                            record.SPO_POST = item.SPO_POST;
                            record.SPO_POST_ENG = item.SPO_POST_ENG;
                            record.SPO_POST_CHI = item.SPO_POST_CHI;
                            record.SPO_NAME_ENG = item.SPO_NAME_ENG;
                            record.SPO_NAME_CHI = item.SPO_NAME_CHI;
                            record.SPO_CONTACT = item.SPO_CONTACT;
                            record.IS_ACTIVE = item.ISACTIVE == "D" ? "D" : (item.IsCheckActive == true ? "Y" : "N");
                            record.MODIFIED_BY = "ADMIN";
                            record.MODIFIED_DATE = DateTime.Now;
                        }
                        else
                        {
                            
                            string UUID = "SYS" + count.ToString("0000");

                            db.P_S_TO_DETAILS.Add(new P_S_TO_DETAILS()
                            {
                                UUID = UUID,
                                TO_POST = item.TO_POST,
                                TO_POST_ENG = item.TO_POST_ENG,
                                TO_POST_CHI = item.TO_POST_CHI,
                                TO_NAME_ENG = item.TO_NAME_ENG,
                                TO_NAME_CHI = item.TO_NAME_CHI,
                                TO_CONTACT = item.TO_CONTACT,
                                PO_POST = item.PO_POST,
                                PO_POST_ENG = item.PO_POST_ENG,
                                PO_POST_CHI = item.PO_POST_CHI,
                                PO_NAME_ENG = item.PO_NAME_ENG,
                                PO_NAME_CHI = item.PO_NAME_CHI,
                                PO_CONTACT = item.PO_CONTACT,
                                SPO_POST = item.SPO_POST,
                                SPO_POST_ENG = item.SPO_POST_ENG,
                                SPO_POST_CHI = item.SPO_POST_CHI,
                                SPO_NAME_ENG = item.SPO_NAME_ENG,
                                SPO_NAME_CHI = item.SPO_NAME_CHI,
                                SPO_CONTACT = item.SPO_CONTACT,
                                IS_ACTIVE = item.ISACTIVE == "D" ? "D" : (item.IsCheckActive == true ? "Y" : "N"),
                                CREATED_BY = "ADMIN",
                                CREATED_DATE = DateTime.Now,
                                MODIFIED_BY = "ADMIN",
                                MODIFIED_DATE = DateTime.Now
                            });
                            count++;
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
                        AuditLogService.Log("DeleteToDetails", validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        AuditLogService.Log("DeleteToDetails", ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }

                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                    };
                }
            }
        }

        #endregion

        #region Remind Letter Template Signature

        //public PEM1103RemindLetterTemplateSignatureModel SearchRemindLetterTemplateSignature()
        //{
        //    using (EntitiesMWProcessing db = new EntitiesMWProcessing())
        //    {
        //        var code = new[] { ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION,
        //                            ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME, ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION };
        //        var record = (from sv in db.P_S_SYSTEM_VALUE
        //                      join stype in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals stype.UUID
        //                      where stype.TYPE == ProcessingConstant.S_TYPE_MOD_LETTER_TEMPLETE
        //                      select sv).ToList();
        //        return new PEM1103RemindLetterTemplateSignatureModel()
        //        {
        //            SPO_Eng_Name = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME).FirstOrDefault().DESCRIPTION
        //           ,
        //            SPO_Eng_Position = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION).FirstOrDefault().DESCRIPTION
        //           ,
        //            SPO_Chi_Name = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME).FirstOrDefault().DESCRIPTION
        //           ,
        //            SPO_Chi_Position = record.Where(o => o.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION).FirstOrDefault().DESCRIPTION
        //        };
        //    }
        //}

        #endregion
    }
}