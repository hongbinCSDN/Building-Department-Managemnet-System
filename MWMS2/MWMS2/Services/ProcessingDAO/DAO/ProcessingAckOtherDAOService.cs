using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Entity;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Entity;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingAckOtherDAOService
    {
        #region Direct Returned Over Counter
        public ServiceResult UpdateDROverCounter(Fn01LM_OtherDROverCounterModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    DateTime? receivedDate = Convert.ToDateTime(model.ReceivedDate);
                    List<P_S_DAILY_DIRECT_RT_OVER_CNT> p_S_DAILY_DIRECT_RT_OVER_CNT = db.P_S_DAILY_DIRECT_RT_OVER_CNT.Where(m =>
                   m.RECEIVED_DATE.Value.Year == receivedDate.Value.Year
                   && m.RECEIVED_DATE.Value.Month == receivedDate.Value.Month
                   && m.RECEIVED_DATE.Value.Day == receivedDate.Value.Day).ToList();

                    if (p_S_DAILY_DIRECT_RT_OVER_CNT.Count() == 0)
                    {
                        //Dictionary<string, List<string>> errorMsgs = new Dictionary<string, List<string>>();
                        //errorMsgs.Add("ReceivedDate", new List<string>()
                        //{
                        //    "This record is not exist "
                        //});
                        //return new ServiceResult()
                        //{
                        //    Result = ServiceResult.RESULT_FAILURE
                        //    ,
                        //    ErrorMessages = errorMsgs
                        //};
                        db.P_S_DAILY_DIRECT_RT_OVER_CNT.Add(new P_S_DAILY_DIRECT_RT_OVER_CNT()
                        {
                            RECEIVED_DATE = Convert.ToDateTime(model.ReceivedDate)
                            ,
                            COUNT = Convert.ToInt32(model.ReturnOverCounter)
                        });
                    }
                    else
                    {
                        foreach (P_S_DAILY_DIRECT_RT_OVER_CNT item in p_S_DAILY_DIRECT_RT_OVER_CNT)
                        {
                            item.COUNT = Convert.ToInt32(model.ReturnOverCounter);
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
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        #endregion

        #region Change Order Related Status
        public ServiceResult UpdateAckOrderRelatedStatus(Fn01LM_OtherChangeORS model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    P_MW_ACK_LETTER p_MW_ACK_LETTER = db.P_MW_ACK_LETTER.Where(m => m.DSN == model.DSN).FirstOrDefault();
                    if (p_MW_ACK_LETTER == null)
                    {
                        Dictionary<string, List<string>> errorMsgs = new Dictionary<string, List<string>>();
                        errorMsgs.Add("DSN", new List<string>()
                        {
                            "This record is not exist "
                        });
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            ErrorMessages = errorMsgs
                        };
                    }

                    p_MW_ACK_LETTER.ORDER_RELATED = model.OrderRelated;
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
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        public ServiceResult GetOrderRelated(string dsn)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER model = db.P_MW_ACK_LETTER.Where(m => m.DSN == dsn).FirstOrDefault();
                if (model != null)
                {
                    serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                    serviceResult.Message = new List<string>()
                    {
                        model.ORDER_RELATED
                    };
                    return serviceResult;
                }
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                return serviceResult;
            }
        }
        #endregion

        #region Update File Reference No
        public ServiceResult UpdateAckFileReferrNo(Fn01LM_OtherUpdateFRN model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    List<P_MW_ACK_LETTER> p_MW_ACK_LETTERs = db.P_MW_ACK_LETTER.Where(m => m.MW_NO == model.MWNo).ToList();
                    if (p_MW_ACK_LETTERs.Count() == 0)
                    {
                        Dictionary<string, List<string>> errorMsgs = new Dictionary<string, List<string>>();
                        errorMsgs.Add("MWNo", new List<string>()
                        {
                            "This record is not exist "
                        });
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            ErrorMessages = errorMsgs
                        };
                    }
                    foreach (P_MW_ACK_LETTER item in p_MW_ACK_LETTERs)
                    {
                        item.FILEREF_FOUR = model.FileReferenceFour;
                        item.FILEREF_TWO = model.FileReferenceTwo;
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
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        #endregion

        #region Batch update referral date
        public ServiceResult UpdateAckReferrDate(Fn01LM_OtherBatchUpdateRD model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    DateTime? receivedDateFrom = Convert.ToDateTime(model.ReceiveDateFrom);
                    DateTime? receivedDateTo = Convert.ToDateTime(model.ReceiveDateTo);
                    List<P_MW_ACK_LETTER> p_MW_ACK_LETTERs;

                    p_MW_ACK_LETTERs = db.P_MW_ACK_LETTER.Where(m => m.RECEIVED_DATE >= receivedDateFrom && m.RECEIVED_DATE <= receivedDateTo).ToList();

                    if (p_MW_ACK_LETTERs.Count() == 0)
                    {
                        Dictionary<string, List<string>> errorMsg = new Dictionary<string, List<string>>();
                        errorMsg.Add("ReceiveDateTo", new List<string>()
                        {
                            "This record is not exist "
                        });
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            ErrorMessages = errorMsg
                        };
                    }

                    foreach (P_MW_ACK_LETTER item in p_MW_ACK_LETTERs)
                    {
                        item.REFERRAL_DATE = Convert.ToDateTime(model.ReferralDate);
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
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        #endregion

    }
}