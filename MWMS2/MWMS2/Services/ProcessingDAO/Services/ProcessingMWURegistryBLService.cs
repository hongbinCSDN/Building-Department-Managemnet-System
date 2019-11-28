using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingMWURegistryBLService
    {
        private ProcessingMWURegistryDAOService _DA;
        protected ProcessingMWURegistryDAOService DA
        {
            get
            {
                return _DA ?? (_DA ?? new ProcessingMWURegistryDAOService());
            }
        }

        private MWPNewSubmissionDAOService _MWPNewSubmission_DA;
        protected MWPNewSubmissionDAOService MWPNewSubmission_DA
        {
            get
            {
                return _MWPNewSubmission_DA ?? (_MWPNewSubmission_DA ?? new MWPNewSubmissionDAOService());
            }
        }

        private ProcessingSystemValueDAOService processingSystemValueDAOService;
        protected ProcessingSystemValueDAOService ProcessingSystemValueDAOService
        {
            get { return processingSystemValueDAOService ?? (processingSystemValueDAOService = new ProcessingSystemValueDAOService()); }
        }

        private P_MW_DSN_DAOService _mwDsnService;
        protected P_MW_DSN_DAOService mwDsnService
        {
            get { return _mwDsnService ?? (_mwDsnService = new P_MW_DSN_DAOService()); }
        }

        private MWPNewSubmissionDAOService mWPNewSubmissionDAOService;
        protected MWPNewSubmissionDAOService MWPNewSubmissionDAOService
        {
            get { return mWPNewSubmissionDAOService ?? (mWPNewSubmissionDAOService = new MWPNewSubmissionDAOService()); }
        }

        private DataTransferToBravoService _bravoService;
        protected DataTransferToBravoService bravoService
        {
            get { return _bravoService ?? (_bravoService = new DataTransferToBravoService()); }
        }

        private P_MW_RECORD_DAOService _mwRecordDaoServiec;

        protected P_MW_RECORD_DAOService mwRecordDaoServiec
        {
            get { return _mwRecordDaoServiec ?? (_mwRecordDaoServiec = new P_MW_RECORD_DAOService()); }
        }

        private P_MW_FILEREF_DAOService _mwFilRefService;
        protected P_MW_FILEREF_DAOService mwFileRefService
        {
            get { return _mwFilRefService ?? (_mwFilRefService = new P_MW_FILEREF_DAOService()); }
        }

        #region Receipt
        public Fn02MWUR_ReceiptModel SearchReceipt(Fn02MWUR_ReceiptModel model)
        {
            return DA.SearchReceipt(model);
        }
        #endregion

        #region Scan and Dispatch
        public string SearchSD_whereq(Fn02MWUR_SDModel model)
        {
            string whereq = "";
            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereq = @" and DSN like :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.DSN.Trim() + "%");
            }

            if (model.CompilingDateFrom != null && model.CompilingDateTo != null)
            {
                //whereq=@" and "
                whereq += "\r\n\t" + " AND DT  >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.CompilingDateFrom);

                whereq += "\r\n\t" + " AND DT  <= :DateTo";
                model.QueryParameters.Add("DateTo", model.CompilingDateTo);

            }
            if (model.CompilingDateFrom != null && model.CompilingDateTo == null)
            {
                whereq += "\r\n\t" + " AND DT  >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.CompilingDateFrom);
            }
            if (model.CompilingDateFrom == null && model.CompilingDateTo != null)
            {
                whereq += "\r\n\t" + " AND DT  <= :DateTo";
                model.QueryParameters.Add("DateTo", model.CompilingDateTo);


            }
            return whereq;
        }
        public Fn02MWUR_SDModel SearchSD(Fn02MWUR_SDModel model)
        {
            model.QueryWhere = SearchSD_whereq(model);
            return DA.SearchSD(model);
        }

        public string ExportSD(Fn02MWUR_SDModel model)
        {
            //model.QueryWhere = SearchSD_whereq(model);
            model = SearchSD(model);
            return model.Export("ExportData");

        }

        public P_MW_DSN DetailSD(string uuid)
        {
            return DA.DetailSD(uuid);
        }


        public Fn02MWUR_SDDisplayModel SearchDoc(Fn02MWUR_SDDisplayModel model)
        {
            return DA.SearchDoc(model);
        }

        public ServiceResult CompletScan(string uuid)
        {
            return DA.CompletScan(uuid);
        }

        public Fn02MWUR_SDDisplayModel ViewSDDetail(string uuid)
        {
            Fn02MWUR_SDDisplayModel model = new Fn02MWUR_SDDisplayModel();

            return model;
        }

        #endregion

        #region Dispatch
        public Fn02MWUR_DSPModel SearchDispatch(Fn02MWUR_DSPModel model)
        {
            return DA.SearchDispatch(model);
        }
        #endregion

        #region Receipt
        public Fn02MWUR_ReceiptModel ReceiveCountedDsnAction(Fn02MWUR_ReceiptModel model)
        {
            // Exception handling
            if (string.IsNullOrEmpty(model.DSN)) return model;
            String dsn = model.DSN;

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // get mwDsn by dsn
                        var origMwDsn = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();

                        if (origMwDsn != null && !String.IsNullOrEmpty(origMwDsn.SCANNED_STATUS_ID))
                        {
                            // get system value
                            P_S_SYSTEM_VALUE svStatus = ProcessingSystemValueDAOService.GetSSystemValueByUuid(origMwDsn.SCANNED_STATUS_ID);

                            if (ProcessingConstant.RD_DELIVERED.Equals(svStatus.CODE) ||
                                ProcessingConstant.DSN_RD_RE_SENT.Equals(svStatus.CODE))
                            {
                                P_S_SYSTEM_VALUE svDsnRegReceiptCnt = ProcessingSystemValueDAOService.GetSSystemValueByCode(ProcessingConstant.DSN_REGISTRY_RECEIPT_COUNTED);

                                // update MwDsn
                                origMwDsn.SCANNED_STATUS_ID = svDsnRegReceiptCnt.UUID;
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return null;
                    }
                }
            }

            return model;
        }

        public Fn02MWUR_ReceiptModel confirmOutstanding(Fn02MWUR_ReceiptModel model)
        {
            // Exception handling
            if (model.OutstandingList == null) return model;

            Dictionary<string, string> OutstandingList = model.OutstandingList;

            // get system value
            P_S_SYSTEM_VALUE svDsnRdOutstanding = ProcessingSystemValueDAOService.GetSSystemValueByCode(ProcessingConstant.DSN_RD_OUTSTANDING);

            // loop Outstanding DSN List
            foreach (var item in OutstandingList)
            {
                String dsn = item.Key;

                //P_MW_DSN mwDsn = mwDsnService.GetP_MW_DSNByDsn(dsn);
                if (!string.IsNullOrEmpty(dsn))
                {
                    using (EntitiesMWProcessing db = new EntitiesMWProcessing())
                    {
                        using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                // get max OI no
                                String currMaxOINo = MWPNewSubmission_DA.getMaxMwNumber(ProcessingConstant.PREFIX_OI, null);
                                int newOINo = int.Parse(currMaxOINo.Substring(2)) + 1;
                                String formattedNewOINo = ProcessingConstant.PREFIX_OI + formatFixedWidth(newOINo.ToString(), 8);

                                // syn seq no
                                P_S_MW_NUMBER sMwNoOI = new P_S_MW_NUMBER();
                                sMwNoOI.MW_NUMBER = formattedNewOINo;

                                // update mwDsn
                                var origMwDsn = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();
                                origMwDsn.ITEM_SEQUENCE_NO = formattedNewOINo;
                                origMwDsn.SCANNED_STATUS_ID = svDsnRdOutstanding.UUID;
                                db.SaveChanges();

                                // Syn seqNo to DB
                                String msgSaveSMwNoOI = MWPNewSubmissionDAOService.SaveSMwNo(sMwNoOI, db);

                                transaction.Commit();
                            }

                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Console.WriteLine("Error :" + ex.Message);
                                return null;
                            }
                        }
                    }
                }
            }

            return model;
        }

        public Fn02MWUR_ReceiptModel ConfirmReceivedAction(Fn02MWUR_ReceiptModel model)
        {
            // Exception handling
            if (model.ConfirmReceiptList == null) return model;

            Dictionary<string, string> ConfirmReceiptList = model.ConfirmReceiptList;

            // get system value
            P_S_SYSTEM_VALUE svDsnRegistryReceived = ProcessingSystemValueDAOService.GetSSystemValueByCode(ProcessingConstant.DSN_REGISTRY_RECEIVED);

            // loop Outstanding DSN List
            foreach (var item in ConfirmReceiptList)
            {
                String dsn = item.Key;

                //P_MW_DSN mwDsn = mwDsnService.GetP_MW_DSNByDsn(dsn);
                if (!string.IsNullOrEmpty(dsn))
                {
                    using (EntitiesMWProcessing db = new EntitiesMWProcessing())
                    {
                        using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                // update mwDsn
                                var origMwDsn = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();

                                if (origMwDsn != null && !String.IsNullOrEmpty(origMwDsn.SCANNED_STATUS_ID))
                                {
                                    // get system value
                                    P_S_SYSTEM_VALUE svStatus = ProcessingSystemValueDAOService.GetSSystemValueByUuid(origMwDsn.SCANNED_STATUS_ID);

                                    if (ProcessingConstant.REGISTRY_RECEIPT_COUNTED.Equals(svStatus.CODE))
                                    {
                                        // update MwDsn
                                        origMwDsn.SCANNED_STATUS_ID = svDsnRegistryReceived.UUID;
                                    }
                                }

                                db.SaveChanges();

                                transaction.Commit();
                            }

                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Console.WriteLine("Error :" + ex.Message);
                                return null;
                            }
                        }
                    }
                }
            }

            return model;
        }

        #endregion

        public String formatFixedWidth(String s, int width)
        {
            String result = s;
            //int totalWidth = s.Length;
            for (int i = result.Length; result.Length < width; i++)
            {
                result = "0" + result;
            }
            return result;
        }

        #region MW Record Address Mapping
        public Fn02MWUR_RAMSearchModel SearchMWRecordAddressMapping(Fn02MWUR_RAMSearchModel model)
        {
            StringBuilder QueryWhere = new StringBuilder(" Where 1 = 1 ");
            if (!string.IsNullOrEmpty(model.RefNo))
            {
                QueryWhere.Append(" And RN.REFERENCE_NO like :MwRefNo ");
                model.QueryParameters.Add("MwRefNo", "%" + model.RefNo + "%");
            }
            if (!string.IsNullOrEmpty(model.DSN))
            {
                QueryWhere.Append(" And R.MW_DSN like :DSN ");
                model.QueryParameters.Add("DSN", "%" + model.DSN + "%");
            }
            if (!string.IsNullOrEmpty(model.File_Reference_Four))
            {
                //          //inner join P_MW_FILE_REF mwFileRef on mwFileRef.MW_RECORD_ID = res.reference_no 
                QueryWhere.Append(" And (R.FILEREF_FOUR like :FileRefNoFour Or FR.FILEREF_FOUR_2 like :FileRefNoFour) ");
                model.QueryParameters.Add("FileRefNoFour", "%" + model.File_Reference_Four + "%");
            }
            if (!string.IsNullOrEmpty(model.File_Reference_Two))
            {
                QueryWhere.Append(" And (R.FILEREF_TWO like :FileRefNoTwo Or FR.FILEREF_TWO_2 like :FileRefNoTwo) ");
                model.QueryParameters.Add("FileRefNoTwo", "%" + model.File_Reference_Two + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.IsCaseTransfertoBravo))
            {
                if (ProcessingConstant.FLAG_Y == model.IsCaseTransfertoBravo)
                {
                    QueryWhere.Append(" AND R.RRM_SYN_STATUS = :RrmStatus ");
                    model.QueryParameters.Add("RrmStatus", ProcessingConstant.RRM_SYN_COMPLETE);
                }
                else
                {
                    QueryWhere.Append(" AND R.RRM_SYN_STATUS != :RrmStatus ");
                    model.QueryParameters.Add("RrmStatus", ProcessingConstant.RRM_SYN_COMPLETE);
                }
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom))
            {
                QueryWhere.Append("  AND TO_DATE(TO_CHAR(r.created_date,'dd/MM/yyyy'),'dd/MM/yyyy') >= TO_DATE(:fromDate,'dd/MM/yyyy') ");
                model.QueryParameters.Add("fromDate", model.ReceivedDateFrom);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                QueryWhere.Append(" AND TO_DATE(TO_CHAR(r.created_date,'dd/MM/yyyy'),'dd/MM/yyyy') <= TO_DATE(:toDate,'dd/MM/yyyy') ");
                model.QueryParameters.Add("toDate", model.ReceivedDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.BlockID))
            {
                QueryWhere.Append(" AND FR.BLK_ID Like :BLK_ID ");
                model.QueryParameters.Add("BLK_ID", "%" + model.BlockID + "%");
            }

            model.QueryWhere = QueryWhere.ToString();
            return DA.SearchMWRecordAddressMapping(model);
        }

        public Fn02MWUR_RAMModel GetMWRecordDetail(string uuid)
        {
            Fn02MWUR_RAMModel model = DA.GetMWRecordDetail(uuid);

            return model;
        }

        public ServiceResult TransferToBravo(string mwRefNo)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        string formCode = "";
                        string imageTransfer = "";
                        bool isEnable = false;

                        bravoService.ValidateTransferToBravo(mwRefNo, ref formCode, ref isEnable, ref imageTransfer);

                        serviceResult.Result = (bravoService.UpdateFinalRecordByRefNo(db, formCode, mwRefNo, ProcessingConstant.FLAG_Y) == "UPDATED") ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string>() { e.Message };
                        AuditLogService.logDebug(e);
                    }

                }
            }

            return serviceResult;
        }

        public Fn02MWUR_RAMModel SearchItemTable(Fn02MWUR_RAMModel model)
        {
            model.Query = @"Select i.MW_ITEM_CODE,i.LOCATION_DESCRIPTION,i.RELEVANT_REFERENCE from P_MW_RECORD r inner join P_MW_RECORD_ITEM  i on r.UUID = i.MW_RECORD_ID And 
                            i.STATUS_CODE = :Status Where r.UUID = :UUID ";
            model.Sort = "i.ORDERING";
            model.QueryParameters.Add("Status", ProcessingConstant.MW_RECORD_ITEM_STATUS_FINAL);
            model.QueryParameters.Add("UUID", model.MWRecordID);
            model.Search();
            return model;
        }

        public Fn02MWUR_RAMModel SearchFileRef(Fn02MWUR_RAMModel model)
        {
            model.Query = @"SELECT F.*
                            FROM   P_MW_FILEREF F
                            WHERE  F.MW_RECORD_ID = :RefNo ";

            model.QueryParameters.Add("RefNo", model.RefNo);
            model.Search();

            //Start modify by dive 20191108
            string formCode = "";
            string imageTransfer = "";
            bool isEnable = false;

            bravoService.ValidateTransferToBravo(model.RefNo, ref formCode, ref isEnable, ref imageTransfer);
            model.IsEnableTransfer = isEnable;
            model.TRANSFER_RRM = imageTransfer;
            //End modify by dive 20191108

            return model;
        }

        public ServiceResult DeleteFilRef(string uuid)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        DA.DeleteFileRef(db, uuid);
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string>() { e.Message };
                        tran.Rollback();
                        AuditLogService.logDebug(e);
                    }
                }

            }

            return serviceResult;
        }

        public ServiceResult SaveFilRef(P_MW_FILEREF model)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(model.UUID))
                        {
                            DA.AddFileRef(db, model);
                        }
                        else
                        {
                            DA.UpdateFileRef(db, model);
                        }

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string>() { e.Message };
                        tran.Rollback();
                        AuditLogService.logDebug(e);
                    }
                }

            }

            return serviceResult;

        }
        #endregion
    }
}