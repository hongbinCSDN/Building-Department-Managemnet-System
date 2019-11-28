using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services.ProcessingDAO.DAO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class DataTransferToBravoService
    {
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

        public void ValidateTransferToBravo(string mwRefNo, ref string formCode, ref bool isEnable, ref string imageTransfer)
        {
            //Get Final Record By UUID
            P_MW_RECORD finalRecord = mwRecordDaoServiec.GetFinalMwRecordByRefNo(mwRefNo);

            //Get MW Records
            List<P_MW_RECORD> mwRecords = mwRecordDaoServiec.GetP_MW_RECORDsByRefNo(mwRefNo);

            //Get FileRefs
            List<P_MW_FILEREF> filRefs = mwFileRefService.GetFileRefsByMwRefNo(mwRefNo);

            if (!ProcessingConstant.RRM_SYN_COMPLETE.Equals(finalRecord.RRM_SYN_STATUS) && filRefs.Count() > 0)
            {
                foreach (P_MW_RECORD item in mwRecords)
                {
                    if (ProcessingConstant.FORM_02.Equals(item.S_FORM_TYPE_CODE)
                        || ProcessingConstant.FORM_04.Equals(item.S_FORM_TYPE_CODE)
                        || ProcessingConstant.FORM_05.Equals(item.S_FORM_TYPE_CODE)
                        || ProcessingConstant.FORM_06.Equals(item.S_FORM_TYPE_CODE))
                    {
                        if (finalRecord.AUDIT_RELATED == "Y")
                        {
                            P_WF_TASK task = ProcessingWorkFlowManagementService.Instance.GetCurrentTaskByRecordID(item.MW_DSN);
                            if (task != null && ProcessingConstant.WF_GO_TASK_END.Equals(task.TASK_CODE))
                            {
                                P_MW_RECORD mwRecord = mwRecordDaoServiec.GetP_MW_RECORDByUuid(task.P_WF_INFO.RECORD_ID);
                                if(mwRecord!=null && mwRecord.P_MW_SUMMARY_MW_ITEM_CHECKLIST.FirstOrDefault() != null)
                                {
                                    imageTransfer = mwRecord.P_MW_SUMMARY_MW_ITEM_CHECKLIST.FirstOrDefault().TRANSFER_RRM;
                                }
                                formCode = item.S_FORM_TYPE_CODE;
                                isEnable = true;
                                break;
                            }
                        }
                        else
                        {
                            formCode = item.S_FORM_TYPE_CODE;
                            isEnable = true;
                            break;
                        }

                    }
                }
            }
        }

        public String UpdateFinalRecordByRefNo(EntitiesMWProcessing db, String formCode, String mwRefNo, string imageTransfer)
        {
            String result = "";
            if (ProcessingConstant.FORM_02.Equals(formCode) ||
                ProcessingConstant.FORM_04.Equals(formCode) ||
                ProcessingConstant.FORM_05.Equals(formCode) ||
                ProcessingConstant.FORM_06.Equals(formCode))
            {
                // get finalRecord and related table > UpdateFinalRecordForRRM();
                bool isImageTransfer = ProcessingConstant.FLAG_Y.Equals(imageTransfer);
                UpdateFinalRecordForRRM(db, mwRefNo, isImageTransfer);
                result = "UPDATED";
            }

            return result;
        }

        public int UpdateFinalRecordForRRM(EntitiesMWProcessing db, string mwRefNo, bool isImageTransfer)
        {
            P_MW_RECORD finalRecord = mwRecordDaoServiec.GetFinalMwRecordByRefNo(mwRefNo, db);
            if (finalRecord == null) { throw new Exception("Final Record Not Found"); }

            List<P_MW_ADDRESS> FinalAddressList = new List<P_MW_ADDRESS>();
            List<P_MW_RECORD_ITEM> FinalRecordItemList = new List<P_MW_RECORD_ITEM>();
            List<P_MW_RECORD_ADDRESS_INFO> FinalRecordAddressInfoList = new List<P_MW_RECORD_ADDRESS_INFO>();
            List<P_MW_SCANNED_DOCUMENT> FinalScannedDocumentList = new List<P_MW_SCANNED_DOCUMENT>();
            List<P_MW_FILEREF> FinalFileRefList = new List<P_MW_FILEREF>();

            FinalAddressList.Add(finalRecord.P_MW_ADDRESS);

            FinalRecordItemList.AddRange(finalRecord.P_MW_RECORD_ITEM);

            FinalRecordAddressInfoList.AddRange(finalRecord.P_MW_RECORD_ADDRESS_INFO);

            P_MW_DSN dsn = db.P_MW_DSN.Where(w => w.DSN == finalRecord.MW_DSN).FirstOrDefault();

            FinalScannedDocumentList.AddRange(dsn.P_MW_SCANNED_DOCUMENT);

            //Get P_MW_FILEREFs
            FinalFileRefList.AddRange(db.P_MW_FILEREF.Where(w => w.MW_RECORD_ID == mwRefNo).ToList());

            UpdateFinalRecord(finalRecord, db);

            foreach (P_MW_ADDRESS mwAddress in FinalAddressList)
            {
                UpdateFinalAddress(mwAddress, db);
            }

            foreach (P_MW_RECORD_ITEM mwRecordItem in FinalRecordItemList)
            {
                UpdateFinalRecordItem(mwRecordItem, db);
            }

            foreach (P_MW_RECORD_ADDRESS_INFO mwRecordAddressInfo in FinalRecordAddressInfoList)
            {
                UpdateRecordAddressInfo(mwRecordAddressInfo, db);
            }


            if (isImageTransfer)
            {
                foreach (P_MW_SCANNED_DOCUMENT mwScannedDocument in FinalScannedDocumentList)
                {
                    UpdateFinalScannedDocument(mwScannedDocument, db);
                }
            }

            foreach (P_MW_FILEREF fileRef in FinalFileRefList)
            {
                UpdateFinalFileRef(fileRef, db);
            }



            return db.SaveChanges();
        }

        public int UpdateFinalRecord(P_MW_RECORD model, EntitiesMWProcessing db)
        {
            P_MW_RECORD record = db.P_MW_RECORD.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
            }

            return db.SaveChanges();
        }

        public int UpdateFinalAddress(P_MW_ADDRESS model, EntitiesMWProcessing db)
        {
            P_MW_ADDRESS record = db.P_MW_ADDRESS.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
            }

            return db.SaveChanges();
        }

        public int UpdateFinalRecordItem(P_MW_RECORD_ITEM model, EntitiesMWProcessing db)
        {
            P_MW_RECORD_ITEM record = db.P_MW_RECORD_ITEM.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
            }

            return db.SaveChanges();
        }

        public int UpdateRecordAddressInfo(P_MW_RECORD_ADDRESS_INFO model, EntitiesMWProcessing db)
        {
            P_MW_RECORD_ADDRESS_INFO record = db.P_MW_RECORD_ADDRESS_INFO.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
            }

            return db.SaveChanges();
        }

        public int UpdateFinalScannedDocument(P_MW_SCANNED_DOCUMENT model, EntitiesMWProcessing db)
        {
            P_MW_SCANNED_DOCUMENT record = db.P_MW_SCANNED_DOCUMENT.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_PENDING_LOAD_DETAILS;
            }

            return db.SaveChanges();
        }

        public int UpdateFinalFileRef(P_MW_FILEREF model, EntitiesMWProcessing db)
        {
            P_MW_FILEREF record = db.P_MW_FILEREF.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null && !ProcessingConstant.RRM_SYN_COMPLETE.Equals(record.RRM_SYN_STATUS))
            {
                record.RRM_SYN_STATUS = ProcessingConstant.RRM_SYN_READY;
            }

            return db.SaveChanges();
        }
    }
}