using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingIccBLService
    {
        //ProcessingIccDAOService
        private ProcessingIccDAOService DAOService;
        protected ProcessingIccDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingIccDAOService()); }
        }

        public void loadDefault(Fn08ICC_IccModel model)
        {
            model.assignOfficerType = "A";
            model.officeList = new List<SelectListItem>();

        }

      

        public void saveNewRecord(Fn08ICC_IccModel model)
        {
            MWPNewSubmissionDAOService MWPNewSubmissionDAOService = new MWPNewSubmissionDAOService();
         //   ProcessingSystemValueDAOService ProcessingSystemValueDAOService = new ProcessingSystemValueDAOService();

            ProcessingCommonService ProcessingCommonService = new ProcessingCommonService();
            String prefix = "";
            if (ProcessingConstant.SUBMIT_TYPE_ENQ.Equals(model.assignType))
            {
                prefix = ProcessingConstant.ENQ;
            }
            else if (ProcessingConstant.SUBMIT_TYPE_COM.Equals(model.assignType))
            {
                prefix = ProcessingConstant.COM;
            }

            P_S_MW_NUMBER sMwNumber = new P_S_MW_NUMBER();

            //Start modify by dive 20191014
            //String newNo = ProcessingCommonService.SynchronizedSaveNewRecord(sMwNumber, prefix);
            //String refNumber = prefix + newNo;
            String refNumber = ProcessingCommonService.SynchronizedSaveNewRecord(sMwNumber, prefix);
            //End modify by dive 20191014

            P_MW_REFERENCE_NO referNo = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo(refNumber);
            if (referNo == null)
            {   referNo = ProcessingCommonService.getMwReferenceNo(refNumber,  prefix);
              //  mwReferenceNoService.save(referNo);
            }

            P_MW_GENERAL_RECORD mwGeneralRecord = new P_MW_GENERAL_RECORD();

          //  mwGeneralRecord.P_MW_REFERENCE_NO = referNo;
            mwGeneralRecord.STATUS = ProcessingConstant.ENQUIRY_STATUS_OUTSTANDING;
            mwGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_NEW;
            mwGeneralRecord.SUBMIT_TYPE = ProcessingConstant.SOURCE_ICC;
            mwGeneralRecord.ICC_NUMBER = model.iccNo;
            mwGeneralRecord.ICC_TYPE = model.assignType;
            mwGeneralRecord.ICC_OFFICER_TYPE = model.assignOfficerType;
            mwGeneralRecord.ICC_OFFICER_ID = model.assignedOfficer;
            try {
                String handlingOfficer = "";
                if ("A".Equals(model.assignOfficerType)) handlingOfficer = "";
                if ("M".Equals(model.assignOfficerType)) handlingOfficer = model.assignedOfficer;

                MWPNewSubmissionDAOService.saveNewICCRecord(referNo, mwGeneralRecord, handlingOfficer);
            }catch(Exception e)
            {
                model.errorMsg = e.Message;
                model.isSaved = false;

                loadDefault(model);
                return;
            }

            model.assignNo = refNumber;
            model.isSaved = true;
            loadDefault(model);
        }
    }
 }