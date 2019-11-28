using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingDocSortingBLService
    {
        //ProcessingDocSortingDAOService
        private ProcessingDocSortingDAOService DAOService;
       protected ProcessingDocSortingDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingDocSortingDAOService()); }
        }

        public Fn09DS_DsModel FindDocSorting(Fn09DS_DsModel model)
        {
            String sql =
               " SELECT " +
               " D.UUID,  " +
               " D.DSN, D.RECORD_ID, D.MWU_RECEIVED_DATE, D.MODIFIED_DATE, " +
               " D.SUBMIT_TYPE " +
               " FROM P_MW_DSN D ";
            string whereQ =  " WHERE D.RE_ASSIGN = :reassign ";
            model.QueryParameters.Add("reassign", "Y");
            model.Query = sql;
            model.QueryWhere = whereQ;
            model.Search();
            return model;
        }

        public string ExportFindDocSorting(Fn09DS_DsModel model)
        {   String sql =
               " SELECT " +
               " D.UUID,  " +
               " D.DSN, D.RECORD_ID, D.MWU_RECEIVED_DATE, D.MODIFIED_DATE, " +
               " D.SUBMIT_TYPE " +
               " FROM P_MW_DSN D ";
            string whereQ = " WHERE D.RE_ASSIGN = :reassign ";
            model.QueryParameters.Add("reassign", "Y");
            model.Query = sql;
            model.QueryWhere = whereQ;
            return model.Export("ExportData");
        }

        public Fn09DS_DsModel ViewDetail(String dsn, string taskId)
        {
            Fn09DS_DsModel result = new Fn09DS_DsModel();

            result.DSN = dsn;
            result.TaskID = taskId;

            result.p_mw_dsn = DA.GetP_MW_DSN(dsn);
            result.P_MW_SCANNED_DOCUMENT_LIST = DA.GetP_MW_SCANNED_DOCUMENT(result.p_mw_dsn.UUID);

            result.submissionType = result.p_mw_dsn.SUBMIT_TYPE;

            result.refNumber = result.p_mw_dsn.RECORD_ID;
            if (String.IsNullOrEmpty(result.submissionType))
            {
                result.submissionType = ProcessingConstant.SUBMIT_TYPE_ENQ;
            }
                                 
            return result;

        }

        public void changeType(Fn09DS_DsModel model)
        {
            model.p_mw_dsn = DA.GetP_MW_DSN(model.DSN);
            model.P_MW_SCANNED_DOCUMENT_LIST = DA.GetP_MW_SCANNED_DOCUMENT(model.p_mw_dsn.UUID);


            string prefix =
                 ProcessingCommonService.getCategoryOutOfSubmitType(model.submissionType);

            if (!String.IsNullOrEmpty(model.refNumber))
            {
                model.refNumber = prefix + model.refNumber.Substring(3);

            }
        }
        public void genNumber(Fn09DS_DsModel model)
        {
            model.p_mw_dsn = DA.GetP_MW_DSN(model.DSN);
            model.P_MW_SCANNED_DOCUMENT_LIST = DA.GetP_MW_SCANNED_DOCUMENT(model.p_mw_dsn.UUID);
            string prefix =
                 ProcessingCommonService.getCategoryOutOfSubmitType(model.submissionType);

            if (!String.IsNullOrEmpty(prefix))
            {
                P_S_MW_NUMBER sMwNumber = new P_S_MW_NUMBER();
                String refNumber = ProcessingCommonService.SynchronizedSaveNewRecord(sMwNumber, prefix);
                //String refNumber = prefix + newNo;
                model.refNumber = refNumber;
                model.GeneratedNumber = "Y";
                model.GEN_NUM = "Y";
            }


        }

        public bool submit(Fn09DS_DsModel model)
        {
            try
            {

                MWPNewSubmissionDAOService MWPNewSubmissionDAOService = new MWPNewSubmissionDAOService();

                string submissionType = model.submissionType;
                string newRefNumber = model.refNumber; 

                P_MW_REFERENCE_NO newReferNo = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo(newRefNumber);

                if (newReferNo == null){
                    newReferNo = ProcessingCommonService.getMwReferenceNo(newRefNumber,
                        ProcessingCommonService.getCategoryOutOfSubmitType(submissionType));
                }

                P_MW_DSN dsn = DA.GetP_MW_DSN(model.DSN);

                P_MW_REFERENCE_NO oldReferNo = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo(dsn.RECORD_ID);
                P_MW_GENERAL_RECORD preGeneralRecord = DA.GetMWGeneralRecord(oldReferNo.UUID);
                if(preGeneralRecord != null) 
                {   preGeneralRecord.FLOW_STATUS = ProcessingConstant.GENERAL_RECORD_UNUSED;
                }

                P_MW_GENERAL_RECORD mwNewGeneralRecord = new P_MW_GENERAL_RECORD();

                //mwNewGeneralRecord.P_MW_REFERENCE_NO = newReferNo;
                mwNewGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_NEW;
                mwNewGeneralRecord.SUBMIT_TYPE = submissionType;
                mwNewGeneralRecord.STATUS = ProcessingConstant.ENQUIRY_STATUS_OUTSTANDING;


                P_S_SYSTEM_VALUE dsnStatus =  DA.GetPSystemValue(ProcessingConstant.DSN_DOCUMENT_SORTING);

                dsn.RE_ASSIGN = "";
                dsn.SUBMIT_TYPE = submissionType;
                dsn.SCANNED_STATUS_ID = dsnStatus.UUID;
                dsn.RECORD_ID = newRefNumber;

                DA.saveDocSorting(newReferNo, dsn, preGeneralRecord, mwNewGeneralRecord);
               //---------------------------------------------------------------------------------------
            }
            catch (Exception e)
            { 
                model.errorMsg = e.Message;
                model.p_mw_dsn = DA.GetP_MW_DSN(model.DSN);
                model.P_MW_SCANNED_DOCUMENT_LIST = DA.GetP_MW_SCANNED_DOCUMENT(model.p_mw_dsn.UUID);

                return false;
            }
            return true;


        }



    }
}