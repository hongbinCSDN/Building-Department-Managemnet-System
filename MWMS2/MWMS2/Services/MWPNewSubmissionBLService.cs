using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Services;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace MWMS2.Services
{
    public class MWPNewSubmissionBLService
    {
        String success = "Success";

        private MWPNewSubmissionDAOService mWPNewSubmissionDAOService;
        protected MWPNewSubmissionDAOService MWPNewSubmissionDAOService
        {
            get { return mWPNewSubmissionDAOService ?? (mWPNewSubmissionDAOService = new MWPNewSubmissionDAOService()); }
        }

        private ProcessingSystemValueDAOService processingSystemValueDAOService;
        protected ProcessingSystemValueDAOService ProcessingSystemValueDAOService
        {
            get { return processingSystemValueDAOService ?? (processingSystemValueDAOService = new ProcessingSystemValueDAOService()); }
        }

        private P_MW_DSN_DAOService _dsnService;
        protected P_MW_DSN_DAOService dsnService
        {
            get { return _dsnService ?? (_dsnService = new P_MW_DSN_DAOService()); }
        }

        public ServiceResult ValidationBeforeReceiveSubmission(Fn02MWUR_MWURC_Model model)
        {
            ServiceResult serviceResult = new ServiceResult();

            if (ProcessingConstant.FORM_MW05.Equals(model.FormNo) && model.TypeOfRefNo == "inputRefNo")
            {
                //Cheking reference number
                List<P_MW_DSN> dsnList = dsnService.GetMwDsnListByRecordId(model.InputRefNo);
                bool isMW32 = dsnList.Where(w => w.FORM_CODE == ProcessingConstant.FORM_MW32).ToList().Count() > 0;

                if (!isMW32)
                {
                    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "InputRefNo", new List<string>() { "Ref. Number dose not contain MW32." } } };
                    return serviceResult;
                }

            }

            serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            return serviceResult;
        }

        public Fn02MWUR_MWURC_Model ReceiveNewSubmission(Fn02MWUR_MWURC_Model model)
        {

            String MWForm = "MWForm";
            String Enquiry = "Enquiry";
            String Complaint = "Complaint";
            String Modification = "Modification";

            DisplaySubmissionObj displaySubmissionObj = new DisplaySubmissionObj();

            if (MWForm.Equals(model.DocType))
            {
                displaySubmissionObj = ReceiveNewMwSubmission(model);
            }
            else if (Enquiry.Equals(model.DocType) || Complaint.Equals(model.DocType))
            {
                displaySubmissionObj = ReceiveEnqCompSubmission(model);
            }
            else if (Modification.Equals(model.DocType))
            {
                displaySubmissionObj = ReceiveModiSubmission(model);
            }

            if (displaySubmissionObj != null)
            {
                model.ToBePrintFormNo = displaySubmissionObj.FormNo;
                model.ToBePrintDSN = displaySubmissionObj.Dsn;
                model.ToBePrintSubmissionNo = displaySubmissionObj.RefNo;
                model.submissionList.Add(displaySubmissionObj);
            }

            return model;
        }

        public DisplaySubmissionObj ReceiveNewMwSubmission(Fn02MWUR_MWURC_Model model)
        {

            // init current date
            DateTime currDate = DateTime.Now;
            String yearPrefix = currDate.Year.ToString();
            String yearYYYYStr = currDate.Year.ToString();
            yearPrefix = yearPrefix.Substring(yearPrefix.Length - 2);
            String monthPrefix = currDate.Month.ToString();
            monthPrefix = monthPrefix.Length == 1 ? "0" + monthPrefix : monthPrefix;
            String datePrefix = yearPrefix + monthPrefix;

            // get current max MW/VS/Enq/Com reference no
            String currMaxRefNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_MW, datePrefix);

            String newRefNo = "";

            if (String.IsNullOrEmpty(currMaxRefNo))
            {
                // handle seq no not existing in db
                newRefNo = getMwSubmissionNewSequenceNo(datePrefix, "");
            }
            else
            {
                // current max reference no +1
                newRefNo = getMwSubmissionNewSequenceNo(datePrefix, currMaxRefNo);
            }

            // add prefix for refNo
            String formattedNewRefNo = ProcessingConstant.PREFIX_MW + newRefNo;

            if (ProcessingConstant.FORM_MW06.Equals(model.FormNo) ||
                (((ProcessingConstant.FORM_MW01.Equals(model.FormNo) || (ProcessingConstant.FORM_MW03.Equals(model.FormNo)))
                && true == model.VsForMW01_MW03)))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_VS + newRefNo;
            }


            // new dsn = current max dsn no +1
            // get current max dsn no from db
            String currMaxDsnNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_D, "");

            int newDsnNo = int.Parse(currMaxDsnNo.Substring(1)) + 1;

            String formattedNewDsn = ProcessingConstant.PREFIX_D + formatFixedWidth(newDsnNo.ToString(), 10);

            P_MW_REFERENCE_NO mwRefNo = new P_MW_REFERENCE_NO();
            mwRefNo.REFERENCE_NO = formattedNewRefNo;
            mwRefNo.CATEGORY_CODE = ProcessingConstant.PREFIX_MW;


            Boolean isCommForm = isCommencementForm(model.FormNo);

            if (ProcessingConstant.FORM_MW05.Equals(model.FormNo) && model.TypeOfRefNo == "inputRefNo")
            {
                mwRefNo.REFERENCE_NO = model.InputRefNo;
            }

            if (!isCommForm)
            {
                mwRefNo.REFERENCE_NO = model.InputRefNo;
            }

            // get system value "WILL_SCAN"
            P_S_SYSTEM_VALUE svWillScan = ProcessingSystemValueDAOService.GetSSystemValueByTypeAndCode(ProcessingConstant.DSN_STATUS, ProcessingConstant.WILL_SCAN);

            // create new mwDsn obj
            P_MW_DSN mwDsn = new P_MW_DSN();
            mwDsn.DSN = formattedNewDsn;
            mwDsn.SUBMIT_TYPE = ProcessingConstant.MW_SUBMISSION;
            mwDsn.SCANNED_STATUS_ID = (null == svWillScan ? null : svWillScan.UUID);
            mwDsn.RECORD_ID = mwRefNo.REFERENCE_NO;
            mwDsn.FORM_CODE = model.FormNo;
            mwDsn.RD_DELIVERED_DATE = currDate;
            mwDsn.SUBMIT_FLOW = ProcessingConstant.MWU_TO_SECOND;
            mwDsn.SUBMIT_TYPE = ProcessingConstant.MW_SUBMISSION;

            // syn seq no
            P_S_MW_NUMBER sMwNoDsn = new P_S_MW_NUMBER();
            sMwNoDsn.MW_NUMBER = formattedNewDsn;//ProcessingConstant.PREFIX_D + newDsnNo;

            P_S_MW_NUMBER sMwNo = new P_S_MW_NUMBER();
            sMwNo.MW_NUMBER = newRefNo;

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // save to db
                        String msgSaveMwRefNo = isCommForm ? MWPNewSubmissionDAOService.SaveMwReferenceNo(mwRefNo, db) : success;
                        String msgSaveMwDsn = MWPNewSubmissionDAOService.SaveMwDSN(mwDsn, db);
                        String msgSaveSMwNoDsn = MWPNewSubmissionDAOService.SaveSMwNo(sMwNoDsn, db);
                        String msgSaveSMwNo = isCommForm ? MWPNewSubmissionDAOService.SaveSMwNo(sMwNo, db) : success;


                        DisplaySubmissionObj displaySubmissionObj = null;
                        // add obj to display list
                        if (success.Equals(msgSaveMwRefNo) && success.Equals(msgSaveMwDsn) && success.Equals(msgSaveSMwNoDsn) &&
                            success.Equals(msgSaveSMwNo))
                        {
                            displaySubmissionObj = new DisplaySubmissionObj();
                            displaySubmissionObj.RecDate = ((DateTime)mwDsn.RD_DELIVERED_DATE).ToString(String.Format("dd/MM/yyyy"));
                            displaySubmissionObj.Time = ((DateTime)mwDsn.RD_DELIVERED_DATE).ToString(String.Format("HH:mm"));
                            displaySubmissionObj.Dsn = mwDsn.DSN;
                            displaySubmissionObj.RefNo = mwDsn.RECORD_ID;
                            displaySubmissionObj.FormNo = mwDsn.FORM_CODE;
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return displaySubmissionObj;
                    }
                    catch (DbEntityValidationException e)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + e.Message);
                        return null;
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

        public DisplaySubmissionObj ReceiveEnqCompSubmission(Fn02MWUR_MWURC_Model model)
        {
            String Enquiry = "Enquiry";
            String Complaint = "Complaint";

            // init current date
            DateTime currDate = DateTime.Now;
            String yearPrefix = currDate.Year.ToString();
            String yearYYYYStr = currDate.Year.ToString();
            yearPrefix = yearPrefix.Substring(yearPrefix.Length - 2);
            String monthPrefix = currDate.Month.ToString();
            monthPrefix = monthPrefix.Length == 1 ? "0" + monthPrefix : monthPrefix;
            String datePrefix = yearPrefix + monthPrefix;


            // get current max MW/VS/Enq/Com reference no
            String currMaxRefNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_MW, datePrefix);

            String newRefNo = "";

            if (String.IsNullOrEmpty(currMaxRefNo))
            {
                // handle seq no not existing in db
                newRefNo = getMwSubmissionNewSequenceNo(datePrefix, "");
            }
            else
            {
                // current max reference no +1
                newRefNo = getMwSubmissionNewSequenceNo(datePrefix, currMaxRefNo);
            }


            // add prefix for refNo
            String formattedNewRefNo = ProcessingConstant.PREFIX_ENQ + newRefNo;

            if (Enquiry.Equals(model.DocType))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_ENQ + newRefNo;
            }
            else if (Complaint.Equals(model.DocType))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_COMP + newRefNo;
            }

            // new dsn = current max dsn no +1
            // get current max dsn no from db
            String currMaxDsnNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_D, "");
            int newDsnNo = int.Parse(currMaxDsnNo.Substring(1)) + 1;
            String formattedNewDsn = ProcessingConstant.PREFIX_D + formatFixedWidth(newDsnNo.ToString(), 10);

            P_MW_REFERENCE_NO mwRefNo = new P_MW_REFERENCE_NO();
            mwRefNo.REFERENCE_NO = formattedNewRefNo;


            if (Enquiry.Equals(model.DocType))
            {
                mwRefNo.CATEGORY_CODE = ProcessingConstant.PREFIX_ENQ;
            }
            else if (Complaint.Equals(model.DocType))
            {
                mwRefNo.CATEGORY_CODE = ProcessingConstant.PREFIX_COMP;
            }


            // get system value "WILL_SCAN"
            // P_S_SYSTEM_VALUE svWillScan = ProcessingSystemValueDAOService.GetSSystemValueByTypeAndCode(ProcessingConstant.DSN_STATUS, ProcessingConstant.WILL_SCAN);

            // get system value "GENERAL_ENTRY_WILL_SCAN"
            P_S_SYSTEM_VALUE svGeneralEntryWillScan = ProcessingSystemValueDAOService.GetSSystemValueByTypeAndCode(ProcessingConstant.DSN_STATUS, ProcessingConstant.GENERAL_ENTRY_WILL_SCAN);

            // create new mwDsn obj
            P_MW_DSN mwDsn = new P_MW_DSN();
            mwDsn.DSN = formattedNewDsn;
            mwDsn.SUBMIT_TYPE = Enquiry.Equals(model.DocType) ? ProcessingConstant.MW_ENQUIRY : ProcessingConstant.MW_COMPLAINT;
            mwDsn.SCANNED_STATUS_ID = (null == svGeneralEntryWillScan ? null : svGeneralEntryWillScan.UUID);
            mwDsn.RECORD_ID = mwRefNo.REFERENCE_NO;
            mwDsn.FORM_CODE = model.FormNo;
            mwDsn.SUBMIT_FLOW = ProcessingConstant.MWU_TO_SECOND;

            // syn seq no
            P_S_MW_NUMBER sMwNoDsn = new P_S_MW_NUMBER();
            sMwNoDsn.MW_NUMBER = formattedNewDsn;// ProcessingConstant.PREFIX_D + newDsnNo;

            P_S_MW_NUMBER sMwNo = new P_S_MW_NUMBER();
            sMwNo.MW_NUMBER = newRefNo;

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {

                    try
                    {
                        // save to db
                        String msgSaveMwRefNo = MWPNewSubmissionDAOService.SaveMwReferenceNo(mwRefNo, db);
                        String msgSaveMwDsn = MWPNewSubmissionDAOService.SaveMwDSN(mwDsn, db);
                        String msgSaveSMwNoDsn = MWPNewSubmissionDAOService.SaveSMwNo(sMwNoDsn, db);
                        String msgSaveSMwNo = MWPNewSubmissionDAOService.SaveSMwNo(sMwNo, db);


                        DisplaySubmissionObj displaySubmissionObj = null;
                        // add obj to display list
                        if (success.Equals(msgSaveMwRefNo) && success.Equals(msgSaveMwDsn) && success.Equals(msgSaveSMwNoDsn) &&
                            success.Equals(msgSaveSMwNo))
                        {
                            displaySubmissionObj = new DisplaySubmissionObj();
                            displaySubmissionObj.RecDate = ((DateTime)currDate).ToString(String.Format("dd/MM/yyyy"));
                            displaySubmissionObj.Time = ((DateTime)currDate).ToString(String.Format("HH:mm"));
                            displaySubmissionObj.Dsn = mwDsn.DSN;
                            displaySubmissionObj.RefNo = mwDsn.RECORD_ID;
                            displaySubmissionObj.FormNo = mwDsn.FORM_CODE;
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return displaySubmissionObj;
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

        public DisplaySubmissionObj ReceiveModiSubmission(Fn02MWUR_MWURC_Model model)
        {


            // init current date
            DateTime currDate = DateTime.Now;
            String yearPrefix = currDate.Year.ToString();
            String yearYYYYStr = currDate.Year.ToString();
            yearPrefix = yearPrefix.Substring(yearPrefix.Length - 2);
            String monthPrefix = currDate.Month.ToString();
            monthPrefix = monthPrefix.Length == 1 ? "0" + monthPrefix : monthPrefix;
            String datePrefix = yearPrefix + monthPrefix;


            // get current max Modification reference no
            String currMaxModiNo = MWPNewSubmissionDAOService.getMaxModNumber(yearYYYYStr);

            String newRefNo = "";

            if (String.IsNullOrEmpty(currMaxModiNo))
            {
                newRefNo = "1";
            }
            else
            {
                // current reference no +1
                newRefNo = (int.Parse(currMaxModiNo) + 1) + "";
            }


            // add prefix for refNo
            String formattedNewRefNo = ProcessingConstant.PREFIX_MOD + newRefNo;

            if ("newGen".Equals((model.TypeOfRefNo)))
            {
                // e.g. MW 001/2019 (MOD)
                formattedNewRefNo = "MW " + formatFixedWidth(newRefNo, 3) + "/" + yearYYYYStr + " (MOD)";
            }
            else
            {
                // get input of existing Mod No
                formattedNewRefNo = model.InputRefNo;
            }



            // new dsn = current max dsn no +1
            // get current max dsn no from db
            String currMaxDsnNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_D, "");
            int newDsnNo = int.Parse(currMaxDsnNo.Substring(1)) + 1;
            String formattedNewDsn = ProcessingConstant.PREFIX_D + formatFixedWidth(newDsnNo.ToString(), 10);


            // get system value "WILL_SCAN"
            P_S_SYSTEM_VALUE svWillScan = ProcessingSystemValueDAOService.GetSSystemValueByTypeAndCode(ProcessingConstant.DSN_STATUS, ProcessingConstant.WILL_SCAN);

            // create new mwDsn obj
            P_MW_DSN mwDsn = new P_MW_DSN();
            mwDsn.DSN = formattedNewDsn;
            mwDsn.SUBMIT_TYPE = ProcessingConstant.MW_SUBMISSION;
            mwDsn.SCANNED_STATUS_ID = (null == svWillScan ? null : svWillScan.UUID);
            mwDsn.RECORD_ID = formattedNewRefNo;
            mwDsn.FORM_CODE = model.FormNo;
            mwDsn.MWU_RECEIVED_DATE = currDate;
            mwDsn.SUBMIT_FLOW = ProcessingConstant.MWU_TO_SECOND;
            mwDsn.SUBMIT_TYPE = ProcessingConstant.MW_MODIFICATION;

            // syn seq no
            P_S_MW_NUMBER sMwNoDsn = new P_S_MW_NUMBER();
            sMwNoDsn.MW_NUMBER = formattedNewDsn;// ProcessingConstant.PREFIX_D + newDsnNo;

            // syn modification
            P_MW_MODIFICATION_NO mwModNo = new P_MW_MODIFICATION_NO();
            mwModNo.REFERENCE_NO = formattedNewRefNo;
            mwModNo.PREFIX = yearYYYYStr;
            mwModNo.CURRENT_NUMBER = decimal.Parse(newRefNo);

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // save to db
                        String msgSaveMwDsn = MWPNewSubmissionDAOService.SaveMwDSN(mwDsn, db);
                        String msgSaveSMwNoDsn = MWPNewSubmissionDAOService.SaveSMwNo(sMwNoDsn, db);
                        String msgSaveMwModNo = "";

                        if ("newGen".Equals((model.TypeOfRefNo)))
                        {
                            msgSaveMwModNo = MWPNewSubmissionDAOService.SaveMwModNo(mwModNo, db);
                        }
                        else
                        {
                            msgSaveMwModNo = success;
                        }

                        DisplaySubmissionObj displaySubmissionObj = null;


                        if (success.Equals(msgSaveMwDsn) && success.Equals(msgSaveSMwNoDsn) && success.Equals(msgSaveMwModNo))
                        {
                            displaySubmissionObj = new DisplaySubmissionObj();

                            displaySubmissionObj.RecDate = ((DateTime)mwDsn.MWU_RECEIVED_DATE).ToString(String.Format("dd/MM/yyyy"));
                            displaySubmissionObj.Time = ((DateTime)mwDsn.MWU_RECEIVED_DATE).ToString(String.Format("HH:mm"));
                            displaySubmissionObj.Dsn = mwDsn.DSN;
                            displaySubmissionObj.RefNo = mwDsn.RECORD_ID;
                            displaySubmissionObj.FormNo = mwDsn.FORM_CODE;
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return displaySubmissionObj;
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

        public String getMwSubmissionNewSequenceNo(String prefix, String latestRecord)
        {
            String newNo = "";
            if (String.IsNullOrEmpty(latestRecord))
            {
                newNo = prefix + "00001";
            }
            else
            {
                int newSeqNo = (int.Parse(latestRecord) + 1);
                //newNo = prefix + formatFixedWidth(newSeqNo.ToString(), 5);
                newNo = newSeqNo.ToString();
            }

            return newNo;
        }

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

        public Boolean FindParentDsnByRefNumberAndFormNo(String refNo, String formNo)
        {
            List<P_MW_DSN> dsnList = MWPNewSubmissionDAOService.FindParentDsnByRefNumberAndFormNo(refNo, formNo);

            List<String> parentFormList = new List<string> { "" };
            if (ProcessingConstant.FORM_MW02.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW01 };
            }
            else if (ProcessingConstant.FORM_MW04.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW03 };
            }
            else if (ProcessingConstant.FORM_MW06_01.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW01 };
            }
            else if (ProcessingConstant.FORM_MW06_02.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW03 };
            }
            else if (ProcessingConstant.FORM_MW07.Equals(formNo) ||
                ProcessingConstant.FORM_MW08.Equals(formNo) ||
                ProcessingConstant.FORM_MW09.Equals(formNo) ||
                ProcessingConstant.FORM_MW10.Equals(formNo) ||
                ProcessingConstant.FORM_MW31.Equals(formNo) ||
                ProcessingConstant.FORM_MW33.Equals(formNo))
            {
                parentFormList = new List<string> {
                    ProcessingConstant.FORM_MW01, ProcessingConstant.FORM_MW03,
                    ProcessingConstant.FORM_MW05, ProcessingConstant.FORM_MW06};
            }
            else if (ProcessingConstant.FORM_MW11.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW01 };
            }
            else if (ProcessingConstant.FORM_MW12.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_MW03 };
            }
            else if (ProcessingConstant.FORM_BA16.Equals(formNo))
            {
                parentFormList = new List<string> { ProcessingConstant.FORM_BA16 };
            }

            Boolean isParentFormExists = false;
            for (int i = 0; i < dsnList.Count; i++)
            {
                P_MW_DSN P_MW_DSN = dsnList.ElementAt(i);
                if (parentFormList.Contains(P_MW_DSN.FORM_CODE))
                {
                    isParentFormExists = true;
                    break;
                }
            }

            return isParentFormExists;
        }

        public Boolean isCommencementForm(String formNo)
        {
            Boolean isCommencementForm = false;

            String[] forms = {
                ProcessingConstant.FORM_MW01, ProcessingConstant.FORM_MW03,
                ProcessingConstant.FORM_MW05, ProcessingConstant.FORM_MW06,
                ProcessingConstant.FORM_MW06_03, ProcessingConstant.FORM_MW32
            };
            List<String> formList = new List<String>(forms);

            if (formList.Contains(formNo))
            {
                isCommencementForm = true;
            }

            return isCommencementForm;
        }

        public ValidationResult Validate_InputRefNo(string formNo, string propName, Fn02MWUR_MWURC_Model model)
        {
            string refNo = model.InputRefNo;

            // For MW01, MW03, MW05, MW06, MW06_03, MW32 Ref No not required to input
            if (ProcessingConstant.FORM_MW01.Equals(formNo) || ProcessingConstant.FORM_MW03.Equals(formNo) ||
                ProcessingConstant.FORM_MW05.Equals(formNo) || ProcessingConstant.FORM_MW06.Equals(formNo) ||
                ProcessingConstant.FORM_MW06_03.Equals(formNo) || ProcessingConstant.FORM_MW32.Equals(formNo))
            {
                return null;
            }

            // Sub Sequence forms
            String[] subSeqFormNos = {
                ProcessingConstant.FORM_MW02, ProcessingConstant.FORM_MW04,
                ProcessingConstant.FORM_MW06_01, ProcessingConstant.FORM_MW06_02,
                ProcessingConstant.FORM_MW07, ProcessingConstant.FORM_MW08, ProcessingConstant.FORM_MW09,
                ProcessingConstant.FORM_MW10, ProcessingConstant.FORM_MW11, ProcessingConstant.FORM_MW12,
                ProcessingConstant.FORM_MW31, ProcessingConstant.FORM_MW33
            };
            List<String> subFormNosList = new List<String>(subSeqFormNos);

            if (subFormNosList.Contains(formNo) && String.IsNullOrEmpty(refNo))
            {
                return new ValidationResult("Please input Ref No.", new List<string> { propName });
            }
            else if (subFormNosList.Contains(formNo) && !String.IsNullOrEmpty(refNo))
            {
                Boolean isParentFormExist = false;
                isParentFormExist = FindParentDsnByRefNumberAndFormNo(refNo, formNo);
                if (isParentFormExist)
                {
                    // Valid
                    return null;
                }
                else
                {
                    return new ValidationResult(" Parent form not found.", new List<string> { propName });
                }
            }

            // validation for MOD
            if (ProcessingConstant.FORM_BA16.Equals(formNo))
            {
                if ("newGen".Equals(model.TypeOfRefNo))
                {
                    // valid
                    return null;
                }
                else if ("inputRefNo".Equals(model.TypeOfRefNo))
                {
                    // check if input Mod No exist
                    Boolean isParentFormExist = false;
                    isParentFormExist = FindParentDsnByRefNumberAndFormNo(refNo, formNo);
                    if (isParentFormExist)
                    {
                        // Valid
                        return null;
                    }
                    else
                    {
                        return new ValidationResult(" Modification No. not found.", new List<string> { propName });
                    }
                }
            }
            return null;
        }
    }
}