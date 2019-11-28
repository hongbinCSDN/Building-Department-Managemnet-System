using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingDataEntryBLService
    {
        private static volatile ProcessingDataEntryDAOService _DA;
        private static readonly object locker = new object();
        private static ProcessingDataEntryDAOService DA { get { if (_DA == null) lock (locker) if (_DA == null) _DA = new ProcessingDataEntryDAOService(); return _DA; } }

        private static volatile ProcessingEformService _eformService;
        private static readonly object locker_eformService = new object();
        private static ProcessingEformService eformService { get { if (_eformService == null) lock (locker_eformService) if (_eformService == null) _eformService = new ProcessingEformService(); return _eformService; } }

        private static volatile P_MW_ACK_LETTER_DAOService _ackLetterService;
        private static readonly object lockerAckLeter = new object();
        private static P_MW_ACK_LETTER_DAOService ackLetterService { get { if (_ackLetterService == null) lock (lockerAckLeter) if (_ackLetterService == null) _ackLetterService = new P_MW_ACK_LETTER_DAOService(); return _ackLetterService; } }

        private static volatile P_MW_APPOINTED_PROFESSIONAL_DAOService _apService;
        private static readonly object lockerApService = new object();
        private static P_MW_APPOINTED_PROFESSIONAL_DAOService apService { get { if (_apService == null) lock (lockerApService) if (_apService == null) _apService = new P_MW_APPOINTED_PROFESSIONAL_DAOService(); return _apService; } }

        private static volatile ProcessingEformDataEntryService _eFormService;
        private static readonly object lockerEFormService = new object();
        private static ProcessingEformDataEntryService eFormService { get { if (_eFormService == null) lock (lockerEFormService) if (_eFormService == null) _eFormService = new ProcessingEformDataEntryService(); return _eFormService; } }

        private static volatile P_S_MW_ITEM_DAOService _systemMwItemService;
        private static readonly object lockerSystemMwItem = new object();
        private static P_S_MW_ITEM_DAOService systemMwItemService { get { if (_systemMwItemService == null) lock (lockerSystemMwItem) if (_systemMwItemService == null) _systemMwItemService = new P_S_MW_ITEM_DAOService(); return _systemMwItemService; } }

        private static volatile P_MW_DSN_DAOService _mwDsnService;
        private static readonly object lockermwDsnService = new object();
        private static P_MW_DSN_DAOService mwDsnService { get { if (_mwDsnService == null) lock (lockermwDsnService) if (_mwDsnService == null) _mwDsnService = new P_MW_DSN_DAOService(); return _mwDsnService; } }

        public Fn02MWUR_DeModel GetOutstanding(Fn02MWUR_DeModel model)
        {
            return DA.GetOutstanding(model);
        }

        public ContentResult Search(Fn02MWUR_DeModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ContentResult SearchDocument(Fn02MWUR_DeScanModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchDocument(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public void GetScanModel(Fn02MWUR_DeScanModel model)
        {
            model.P_MW_DSN = DA.GetP_MW_DSN(model.P_MW_DSN.UUID);

            model.P_MW_SCANNED_DOCUMENTs = DA.GetP_MW_SCANNED_DOCUMENTs(model.P_MW_DSN.UUID);
            if (model.P_MW_SCANNED_DOCUMENTs == null)
            {
                model.P_MW_SCANNED_DOCUMENTs = new List<P_MW_SCANNED_DOCUMENT>();
            }

            if (ProcessingConstant.FORM_01.Equals(model.P_MW_DSN.FORM_CODE) || ProcessingConstant.FORM_03.Equals(model.P_MW_DSN.FORM_CODE))
            {
                P_MW_ACK_LETTER ackLetter = ackLetterService.GetP_MW_ACK_LETTERByDsn(model.P_MW_DSN.DSN);

                if (ackLetter != null)
                {
                    model.P_MW_DSN.SSP_SUBMITTED = ackLetter.SSP;
                }
            }

        }

        public string ExportDeScan(Fn02MWUR_DeModel model)
        {
            return DA.ExportDeScan(model);
        }

        public JsonResult CompleteScan(Fn02MWUR_DeScanModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Check SSP 
                if (model.P_MW_DSN.FORM_CODE == "MW01" || model.P_MW_DSN.FORM_CODE == "MW11")
                {
                    if (string.IsNullOrEmpty(model.P_MW_DSN.SSP_SUBMITTED))
                    {
                        serviceResult.ErrorMessages.Add("P_MW_DSN.SSP_SUBMITTED", new List<string>() { "Please select SSP Submitted." });
                        return new JsonResult() { Data = serviceResult };
                    }
                }

                model.P_MW_DSN.SCANNED_STATUS_ID = DA.GetStatusUUIDByCode("SECOND_ENTRY");

                P_MW_REFERENCE_NO p_MW_REFERENCE_NO = DA.GetP_MW_REFERENCE_NO(model.P_MW_DSN.RECORD_ID);

                P_MW_RECORD p_MW_RECORD = new P_MW_RECORD();

                p_MW_RECORD.MW_DSN = model.P_MW_DSN.DSN;
                p_MW_RECORD.IS_DATA_ENTRY = "Y";
                p_MW_RECORD.FORM_VERSION = "2";
                p_MW_RECORD.STATUS_CODE = "MW_SECOND_ENTRY";
                p_MW_RECORD.REFERENCE_NUMBER = p_MW_REFERENCE_NO.UUID;
                p_MW_RECORD.S_FORM_TYPE_CODE = model.P_MW_DSN.FORM_CODE;

                if (model.P_MW_DSN.FORM_CODE == "MW01" || model.P_MW_DSN.FORM_CODE == "MW11")
                {
                    p_MW_RECORD.VERIFICATION_SPO = model.P_MW_DSN.SSP_SUBMITTED == "Y" ? "N" : "Y";
                }

                SetDefaultValue(model.P_MW_DSN, p_MW_RECORD);

                // Andy modify on 2019-10-25
                P_MW_ACK_LETTER ackLetter = ackLetterService.GetP_MW_ACK_LETTERByDsn(p_MW_RECORD.MW_DSN);
                List<P_MW_RECORD_ITEM> mwRecordItemList = new List<P_MW_RECORD_ITEM>();
                P_MW_ADDRESS mwAddress = new P_MW_ADDRESS();
                List<P_MW_APPOINTED_PROFESSIONAL> mwAppointedProfessionalList = new List<P_MW_APPOINTED_PROFESSIONAL>();

                bool isAFC = false;
                // Copy data from MwAckLetter to mwRecord 
                if (ackLetter != null)
                {
                    isAFC = ackLetter.AUDIT_RELATED == ProcessingConstant.FLAG_Y;

                    CopyDataFromMwAckLetterToMwRecord(ackLetter, p_MW_RECORD);
                    CopyDataFromMwAckLetterToMwRecordItem(ackLetter, mwRecordItemList, p_MW_RECORD);
                    CopyDataFromMwAckLetterToMwAddress(ackLetter, mwAddress);
                    CopyDataFromMwAckLetterToMwAppointedProfessional(ackLetter, mwAppointedProfessionalList, p_MW_RECORD);
                }


                P_MW_FORM p_MW_Form = new P_MW_FORM();
                p_MW_Form.RECEIVED_DATE = DateTime.Now;
                p_MW_Form.MW_SUBMISSION_NO = p_MW_REFERENCE_NO.REFERENCE_NO;



                serviceResult = DA.CompleteScan(model.P_MW_DSN, p_MW_RECORD, p_MW_Form, isAFC, mwRecordItemList, mwAddress, mwAppointedProfessionalList);

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult DeleteScanDoc(P_MW_SCANNED_DOCUMENT model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                serviceResult.Result = DA.DeleteScanDoc(model) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        //public JsonResult UpdateDsnStatus(Fn02MWUR_DeScanModel model)
        //{
        //    ServiceResult serviceResult = new ServiceResult();
        //    try
        //    {
        //        string sStatusSql = @"SELECT SV.UUID
        //                                FROM   P_S_SYSTEM_VALUE SV
        //                                       JOIN P_S_SYSTEM_TYPE ST
        //                                         ON SV.SYSTEM_TYPE_ID = ST.UUID
        //                                            AND ST.TYPE = 'DSN_STATUS'
        //                                WHERE  SV.CODE = 'SECOND_ENTRY' ";

        //        model.P_MW_DSN.SCANNED_STATUS_ID = DA.GetObjectData<string>(sStatusSql).FirstOrDefault();

        //        serviceResult.Result = DA.UpdateDsnStatus(model.P_MW_DSN) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

        //    }
        //    catch (Exception e)
        //    {
        //        serviceResult.Result = ServiceResult.RESULT_FAILURE;
        //        serviceResult.Message = new List<string>() { e.Message };
        //    }

        //    return new JsonResult() { Data = serviceResult };

        //}

        public JsonResult SaveAsDraft(Fn02MWUR_DeFormModel model)
        {
            return new JsonResult() { Data = DA.SaveAsDraft(model, false) };
        }

        public JsonResult SubmitForm(Fn02MWUR_DeFormModel model)
        {
            return new JsonResult() { Data = DA.SaveAsDraft(model, true) };
        }

        public void GetFormData(Fn02MWUR_DeFormModel model)
        {
            //Get P_MW_DSN
            model.P_MW_DSN = DA.GetP_MW_DSN(model.P_MW_DSN.UUID);

            //Get P_MW_REFERENCE_NO
            model.P_MW_REFERENCE_NO = DA.GetP_MW_REFERENCE_NO(model.P_MW_DSN.RECORD_ID);

            //Get P_MW_RECORD
            model.P_MW_RECORD = DA.GetP_MW_RECORD(model.P_MW_DSN.DSN, model.P_MW_REFERENCE_NO.UUID);
            if (model.P_MW_RECORD == null)
            {
                model.P_MW_RECORD = new P_MW_RECORD();

                model.P_MW_RECORD.REFERENCE_NUMBER = model.P_MW_REFERENCE_NO.UUID;
                model.P_MW_RECORD.MW_DSN = model.P_MW_DSN.DSN;
                model.P_MW_RECORD.S_FORM_TYPE_CODE = model.P_MW_DSN.FORM_CODE;
            }

            //Get P_MW_RECORD_ITEMs
            model.P_MW_RECORD_ITEMs = DA.GetP_MW_RECORD_ITEM(model.P_MW_RECORD.UUID);
            if (model.P_MW_RECORD_ITEMs == null)
            {
                model.P_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();
            }

            model.P_MW_RECORD_ITEMs_CLASS_I = model.P_MW_RECORD_ITEMs.Where(d => d.CLASS_CODE == ProcessingConstant.DB_CLASS_I).OrderBy(o => o.ORDERING).ToList();
            model.P_MW_RECORD_ITEMs_CLASS_II = model.P_MW_RECORD_ITEMs.Where(d => d.CLASS_CODE == ProcessingConstant.DB_CLASS_II).OrderBy(o => o.ORDERING).ToList();
            model.P_MW_RECORD_ITEMs_CLASS_III = model.P_MW_RECORD_ITEMs.Where(d => d.CLASS_CODE == ProcessingConstant.DB_CLASS_III).OrderBy(o => o.ORDERING).ToList();

            //Begin Add by Chester 2019/07/16
            if (model.P_MW_RECORD_ITEMs_CLASS_I.Count() == 0)
            {
                model.P_MW_RECORD_ITEMs_CLASS_I = new List<P_MW_RECORD_ITEM>()
                {
                    new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                };
            }
            if (model.P_MW_RECORD_ITEMs_CLASS_II.Count() == 0)
            {
                model.P_MW_RECORD_ITEMs_CLASS_II = new List<P_MW_RECORD_ITEM>()
                {
                    new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                };
            }
            if (model.P_MW_RECORD_ITEMs_CLASS_III.Count() == 0)
            {
                model.P_MW_RECORD_ITEMs_CLASS_III = new List<P_MW_RECORD_ITEM>
                {
                    new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                    ,new P_MW_RECORD_ITEM()
                };
            }
            //End Add by Chester 2019/07/16

            //Get P_MW_APPOINTED_PROFESSIONAL
            model.P_MW_APPOINTED_PROFESSIONALs = DA.GetP_MW_APPOINTED_PROFESSIONAL(model.P_MW_RECORD.UUID).OrderBy(o => o.ORDERING).ToList();
            if (model.P_MW_APPOINTED_PROFESSIONALs == null)
            {
                model.P_MW_APPOINTED_PROFESSIONALs = new List<P_MW_APPOINTED_PROFESSIONAL>();
            }

            //Get P_MW_PERSON_CONTACT
            model.OwnerPersonContact = DA.GetP_MW_PERSON_CONTACT(model.P_MW_RECORD.OWNER_ID);
            if (model.OwnerPersonContact == null)
            {
                model.OwnerPersonContact = new P_MW_PERSON_CONTACT();
            }
            model.SignBoardPersonContact = DA.GetP_MW_PERSON_CONTACT(model.P_MW_RECORD.SIGNBOARD_PERFROMER_ID);
            if (model.SignBoardPersonContact == null)
            {
                model.SignBoardPersonContact = new P_MW_PERSON_CONTACT();
            }
            model.OIPersonContact = DA.GetP_MW_PERSON_CONTACT(model.P_MW_RECORD.OI_ID);
            if (model.OIPersonContact == null)
            {
                //New P_MW_PERSON_CONTACT_G
                model.OIPersonContact = new P_MW_PERSON_CONTACT();
            }

            //Get P_MW_ADDRESS
            model.MWAddress = DA.GetP_MW_ADDRESS(model.P_MW_RECORD.LOCATION_ADDRESS_ID);
            if (model.MWAddress == null)
            {
                model.MWAddress = new P_MW_ADDRESS();
            }

            model.OwnerAddress = DA.GetP_MW_ADDRESS(model.OwnerPersonContact.MW_ADDRESS_ID);
            if (model.OwnerAddress == null)
            {
                model.OwnerAddress = new P_MW_ADDRESS();
            }

            //Get P_MW_FROM
            model.P_MW_FORM = DA.GetP_MW_FORMByRecordID(model.P_MW_RECORD.UUID);

            model.SignBoardAddress = DA.GetP_MW_ADDRESS(model.SignBoardPersonContact.MW_ADDRESS_ID);
            if (model.SignBoardAddress == null)
            {
                model.SignBoardAddress = new P_MW_ADDRESS();
            }

            model.OIAddress = DA.GetP_MW_ADDRESS(model.OIPersonContact.MW_ADDRESS_ID);
            if (model.OIAddress == null)
            {
                //New P_MW_PERSON_CONTACT_G
                model.OIAddress = new P_MW_ADDRESS();
            }

            model.P_MW_FORM_09s = DA.GetP_MW_FORM_09s(model.P_MW_RECORD.UUID).OrderBy(o => o.ORDERING).ToList();
            if (model.P_MW_FORM_09s == null)
            {
                model.P_MW_FORM_09s = new List<P_MW_FORM_09>();
            }

            for (int i = model.P_MW_FORM_09s.Count(); i < 27; i++)
            {
                model.P_MW_FORM_09s.Add(new P_MW_FORM_09() { ORDERING = i });
            }

            //Start modify by dive 20191016
            //Get P_EFSS_FORM_MASTER
            model.P_EFSS_FORM_MASTER = eformService.getEfssRecordByDsn(model.P_MW_DSN.DSN);
            //End modify by dive 20191016

            P_MW_FILEREF_DAOService fileRefDA = new P_MW_FILEREF_DAOService();
            var fileRefNo = fileRefDA.GetFileRefByMWRecord(model.P_MW_DSN.RECORD_ID);
            if (fileRefNo != null)
            {
                model.P_MW_RECORD.FILEREF_FOUR = fileRefNo.FILEREF_FOUR;
                model.P_MW_RECORD.FILEREF_TWO = fileRefNo.FILEREF_TWO;
            }

        }

        public void SetDefaultValue(P_MW_DSN mwDsn, P_MW_RECORD mwRecord)
        {
            //List<P_MW_APPOINTED_PROFESSIONAL> mwAppointProfessionalList

            string formCode = mwRecord.S_FORM_TYPE_CODE;
            string sspSubmitted = mwDsn.SSP_SUBMITTED;

            P_MW_RECORD mwFinalRecord = DA.GetFinalMwRecord(mwRecord.REFERENCE_NUMBER);

            DateTime dsnSubmissionDate = mwDsn.CREATED_DATE;

            if (mwFinalRecord == null)
            {
                mwFinalRecord = new P_MW_RECORD();
            }

            string classCode = mwFinalRecord.CLASS_CODE;

            mwRecord.CLASS_CODE = classCode;

            mwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_N;

            if (formCode.Equals(ProcessingConstant.FORM_01))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_I;
                if (sspSubmitted.Equals(ProcessingConstant.FLAG_N))
                {
                    mwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_Y;
                }

                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[4].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                mwRecord.COMPLETION_DATE = null;
                mwRecord.COMPLETION_SUBMISSION_DATE = null;
                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_02))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_I;
                //mwRecord.COMPLETION_DATE = mwAppointProfessionalList[0].COMPLETION_DATE;
                mwRecord.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMPLETION;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_03))
            {

                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_II;
                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[1].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_04))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_II;
                //mwRecord.COMPLETION_DATE = mwAppointProfessionalList[0].COMPLETION_DATE;
                mwRecord.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMPLETION;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_05))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_III;

                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[1].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;

                //mwRecord.COMPLETION_DATE = mwAppointProfessionalList[1].COMPLETION_DATE;
                mwRecord.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;

                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMPLETION;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_06))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_VS;
                mwRecord.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_VS;

                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[3].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;

                //mwRecord.COMPLETION_DATE = mwAppointProfessionalList[3].COMPLETION_DATE;
                mwRecord.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;

                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.VALIDATION_SCHEME;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_07))
            {
                //if (sspSubmitted.Equals(ProcessingConstant.FLAG_N) && classCode.Equals((ProcessingConstant.DB_CLASS_I)))
                //{
                //    mwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_Y;
                //}

            }
            else if (formCode.Equals(ProcessingConstant.FORM_08))
            {
                //if (sspSubmitted.Equals(ProcessingConstant.FLAG_N) && classCode.Equals((ProcessingConstant.DB_CLASS_I)))
                //{
                //    mwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_Y;
                //}
            }
            else if (formCode.Equals(ProcessingConstant.FORM_09))
            {
            }
            else if (formCode.Equals(ProcessingConstant.FORM_10))
            {
            }
            else if (formCode.Equals(ProcessingConstant.FORM_31))
            {
            }
            else if (formCode.Equals(ProcessingConstant.FORM_32))
            {
                mwRecord.CLASS_CODE = ProcessingConstant.DB_CLASS_II;

                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[0].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;

                if (formCode.Equals(ProcessingConstant.FORM_32))
                {
                    //mwRecord.COMPLETION_DATE = mwAppointProfessionalList[0].COMPLETION_DATE;
                    mwRecord.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                }
                else
                {
                    mwRecord.COMPLETION_DATE = null;
                    mwRecord.COMPLETION_SUBMISSION_DATE = null;
                }
                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_11))
            {
                if (sspSubmitted.Equals(ProcessingConstant.FLAG_N) && classCode.Equals((ProcessingConstant.DB_CLASS_I)))
                {
                    mwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_Y;
                }
                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[0].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;

                mwRecord.COMPLETION_DATE = null;
                mwRecord.COMPLETION_SUBMISSION_DATE = null;

                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_12))
            {

                //mwRecord.COMMENCEMENT_DATE = mwAppointProfessionalList[0].COMMENCED_DATE;
                mwRecord.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;

                mwRecord.COMPLETION_DATE = null;
                mwRecord.COMPLETION_SUBMISSION_DATE = null;

                mwRecord.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;

            }
            else if (formCode.Equals(ProcessingConstant.FORM_33))
            {
            }

            // Start modify by dive 20191021
            // Get AckLetter 
            P_MW_ACK_LETTER ackRecord = ackLetterService.GetP_MW_ACK_LETTERByDsn(mwDsn.DSN);

            if (ackRecord != null && ackRecord.AUDIT_RELATED == "Y")
            {
                mwRecord.AUDIT_RELATED = ProcessingConstant.FLAG_Y;
                mwRecord.PRE_SITE_AUDIT_RELATED = ackRecord.PRE_SITE_AUDIT_RELATED;
                mwRecord.SITE_AUDIT_RELATED = ackRecord.SITE_AUDIT_RELATED;
                mwRecord.FILEREF_FOUR = ackRecord.FILEREF_FOUR;
                mwRecord.FILEREF_TWO = ackRecord.FILEREF_TWO;
            }
            //End modify by dive 20191021
        }

        public void CopyDataFromMwAckLetterToMwRecord(P_MW_ACK_LETTER ackLetter, P_MW_RECORD mwRecord)
        {
            if (ackLetter != null && mwRecord != null)
            {
                mwRecord.AUDIT_RELATED = ProcessingConstant.FLAG_Y;
                mwRecord.PRE_SITE_AUDIT_RELATED = ackLetter.PRE_SITE_AUDIT_RELATED;
                mwRecord.FILEREF_FOUR = ackLetter.FILEREF_FOUR;
                mwRecord.FILEREF_TWO = ackLetter.FILEREF_TWO;
                mwRecord.FIRST_RECEIVED_DATE = ackLetter.RECEIVED_DATE;
                mwRecord.LANGUAGE_CODE = ackLetter.LANGUAGE == "CHT" ? "ZH" : "EN";
            }
        }

        public void CopyDataFromMwAckLetterToMwRecordItem(P_MW_ACK_LETTER ackLetter, List<P_MW_RECORD_ITEM> mwRecordItemList, P_MW_RECORD p_MW_RECORD)
        {
            string[] parentForms = { ProcessingConstant.FORM_01, ProcessingConstant.FORM_03, ProcessingConstant.FORM_05 };
            string[] inheritorForms = { ProcessingConstant.FORM_33 };
            //Get item info 
            if (ackLetter == null || string.IsNullOrEmpty(ackLetter.ITEM_DISPLAY)) { return; }
            string itemInfo = ackLetter.ITEM_DISPLAY;

            //Convert to array
            string[] arrItems = itemInfo.Split('/');

            string formCode = "";
            string classCode = "";

            formCode = p_MW_RECORD.S_FORM_TYPE_CODE;

            if (inheritorForms.Contains(p_MW_RECORD.S_FORM_TYPE_CODE))
            {
                P_MW_DSN mwDsn = mwDsnService.GetParentMwDsnByRecordId(ackLetter.MW_NO);

                if (mwDsn != null)
                {
                    formCode = mwDsn.FORM_CODE;
                }
            }

            if (ProcessingConstant.FORM_01.Equals(p_MW_RECORD.S_FORM_TYPE_CODE))
            {
                classCode = ProcessingConstant.DB_CLASS_I;
            }
            else if (ProcessingConstant.FORM_03.Equals(p_MW_RECORD.S_FORM_TYPE_CODE))
            {
                classCode = ProcessingConstant.DB_CLASS_II;
            }
            else if (ProcessingConstant.FORM_05.Equals(p_MW_RECORD.S_FORM_TYPE_CODE))
            {
                classCode = ProcessingConstant.DB_CLASS_III;
            }

            //Add to record item list
            foreach (string itemNo in arrItems)
            {
                if (string.IsNullOrWhiteSpace(itemNo)) { continue; }

                if (!parentForms.Contains(formCode) && !inheritorForms.Contains(formCode))
                {
                    if (itemNo.StartsWith("1"))
                    {
                        classCode = ProcessingConstant.DB_CLASS_I;
                    }
                    else if (itemNo.StartsWith("2"))
                    {
                        classCode = ProcessingConstant.DB_CLASS_II;
                    }
                    else if (itemNo.StartsWith("3"))
                    {
                        classCode = ProcessingConstant.DB_CLASS_III;
                    }
                }

                P_S_MW_ITEM systemMwItem = systemMwItemService.GetP_S_MW_ITEMByItemNo(itemNo);
                mwRecordItemList.Add(new P_MW_RECORD_ITEM()
                {
                    MW_ITEM_CODE = itemNo,
                    MW_RECORD_ID = p_MW_RECORD.UUID,
                    CLASS_CODE = classCode,
                    LOCATION_DESCRIPTION = systemMwItem == null ? null : systemMwItem.DESCRIPTION,
                    RELEVANT_REFERENCE = ProcessingConstant.FLAG_N.Equals(ackLetter.ORDER_RELATED) ? ProcessingConstant.FLAG_N : null
                });
            }
        }

        public void CopyDataFromMwAckLetterToMwAddress(P_MW_ACK_LETTER ackLetter, P_MW_ADDRESS mwAddress)
        {
            if (ackLetter != null && mwAddress != null)
            {
                mwAddress.STREE_NAME = ackLetter.STREET;
                mwAddress.STREET_NO = ackLetter.STREET_NO;
                mwAddress.BUILDING_NAME = ackLetter.BUILDING;
                mwAddress.FLOOR = ackLetter.FLOOR;
                mwAddress.UNIT_ID = ackLetter.UNIT;

                mwAddress.DISPLAY_STREET = ackLetter.STREET;
                mwAddress.DISPLAY_STREET_NO = ackLetter.STREET_NO;
                mwAddress.DISPLAY_BUILDINGNAME = ackLetter.BUILDING;
                mwAddress.DISPLAY_FLOOR = ackLetter.FLOOR;
                mwAddress.DISPLAY_FLAT = ackLetter.UNIT;
            }
        }

        public void CopyDataFromMwAckLetterToMwAppointedProfessional(P_MW_ACK_LETTER ackLetter, List<P_MW_APPOINTED_PROFESSIONAL> mwAppointedProfessionalList, P_MW_RECORD p_MW_RECORD)
        {
            if (ackLetter != null)
            {
                mwAppointedProfessionalList = eFormService.createBlankAP(p_MW_RECORD, mwAppointedProfessionalList);
                mwAppointedProfessionalList = eFormService.setAppointedProfessional(p_MW_RECORD, mwAppointedProfessionalList);

                if (mwAppointedProfessionalList != null)
                {
                    apService.FillAppointedProfessionalByAckLetter(mwAppointedProfessionalList, ackLetter);
                }
            }
        }
    }
}