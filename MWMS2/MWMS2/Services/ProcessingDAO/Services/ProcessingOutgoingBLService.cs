using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingOutgoingBLService
    {
        private ProcessingOutgoingDAOService DAOService;
        protected ProcessingOutgoingDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingOutgoingDAOService()); }
        }

        private P_S_MW_NUMBER_DAOService _MwNumberService;
        protected P_S_MW_NUMBER_DAOService MwNumberService
        {
            get { return _MwNumberService ?? (_MwNumberService = new P_S_MW_NUMBER_DAOService()); }
        }

        private P_MW_DSN_DAOService _DsnService;
        protected P_MW_DSN_DAOService DsnService
        {
            get { return _DsnService ?? (_DsnService = new P_MW_DSN_DAOService()); }
        }

        private P_MW_REFERENCE_NO_DAOService _MwReferenceNoService;
        protected P_MW_REFERENCE_NO_DAOService MwReferenceNoService
        {
            get { return _MwReferenceNoService ?? (_MwReferenceNoService = new P_MW_REFERENCE_NO_DAOService()); }
        }

        private ProcessingSystemValueDAOService _SystemValueService;
        protected ProcessingSystemValueDAOService SystemValueService
        {
            get { return _SystemValueService ?? (_SystemValueService = new ProcessingSystemValueDAOService()); }
        }

        private P_MW_RECORD_LETTER_INFO_DAOService _LetterInfoService;
        protected P_MW_RECORD_LETTER_INFO_DAOService LetterInfoService
        {
            get { return _LetterInfoService ?? (_LetterInfoService = new P_MW_RECORD_LETTER_INFO_DAOService()); }
        }

        private P_LETTER_INFO_DAOService _LetterTemplateService;
        protected P_LETTER_INFO_DAOService LetterTemplateService
        {
            get { return _LetterTemplateService ?? (_LetterTemplateService = new P_LETTER_INFO_DAOService()); }
        }

        public void GetOutgoingModel(Fn02MWUR_OutgoingModel model)
        {
            //Get Letter Template List
            model.LetterTemplateList = new List<P_LETTER_INFO>();
            model.LetterTemplateList.Add(new P_LETTER_INFO() { NAME = "- Select -" });
            model.LetterTemplateList.AddRange(LetterTemplateService.GetP_LETTER_INFOLs().OrderBy(o => o.NAME));

        }

        public JsonResult GenerateNewDsn()
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                string maxNumber = MwNumberService.GetNewNumberOfDsn();

                //Add DSN 
                P_MW_DSN model = new P_MW_DSN();
                model.DSN = maxNumber;
                model.ISSUED_DATE = DateTime.Now;

                //DsnService.AddP_MW_DSN(model.P_MW_DSN);

                serviceResult.Data = model;
                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "ErrorMessage", new List<string>() { e.Message } } };
            }

            return new JsonResult() { Data = serviceResult };

        }

        public JsonResult SubmitDsnInfo(Fn02MWUR_OutgoingModel model)
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                if (string.IsNullOrWhiteSpace(model.P_MW_DSN.RECORD_ID))
                {
                    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "P_MW_DSN.RECORD_ID", new List<string>() { "Ref. No. missing!" } } };
                    return new JsonResult() { Data = serviceResult };
                }

                String issuedDate = null;
                String letterType = null;
                String templateName = null;
                String dispatchOrHookSelected = model.ScanType;

                if (!model.P_MW_DSN.RECORD_ID.StartsWith(ProcessingConstant.COM) && !model.P_MW_DSN.RECORD_ID.StartsWith(ProcessingConstant.ENQ))
                {
                    issuedDate = model.P_MW_DSN.ISSUED_DATE.ToString();
                    letterType = model.LetterType;
                    templateName = model.LetterTemplateNo;
                }


                //Check MW No
                //Get MW No
                P_MW_REFERENCE_NO refRecord = MwReferenceNoService.getMwReferenceNoByMwNo(model.P_MW_DSN.RECORD_ID);

                if (refRecord == null)
                {
                    //Invalid 
                    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "P_MW_DSN.RECORD_ID", new List<string>() { "Ref. No. not found!" } } };

                }
                else
                {
                    //Get System Value
                    P_S_SYSTEM_VALUE svRecord = new P_S_SYSTEM_VALUE();

                    //Check dispatchOrHookSelected
                    if (dispatchOrHookSelected.Equals(ProcessingConstant.SCAN_DISPATCH))
                    {
                        svRecord = SystemValueService.GetSSystemValueByCode(ProcessingConstant.MWU_OUTGOING_NEW_DISPATCH);
                    }
                    else if (dispatchOrHookSelected.Equals(ProcessingConstant.SCAN_HOOK))
                    {
                        svRecord = SystemValueService.GetSSystemValueByCode(ProcessingConstant.MWU_OUTGOING_NEW_HOOK);
                    }

                    P_MW_DSN dsnRecord = DsnService.GetP_MW_DSNByDsn(model.P_MW_DSN.DSN);

                    if (dsnRecord == null)
                    {
                        //Save new record

                        if (svRecord != null && !string.IsNullOrWhiteSpace(model.P_MW_DSN.DSN))
                        {
                            //New Dsn
                            model.P_MW_DSN.SCANNED_STATUS_ID = svRecord.UUID;
                            model.P_MW_DSN.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_ISSUED_CORR;

                            DsnService.AddP_MW_DSN(model.P_MW_DSN);

                            //New Mw Record Letter Info
                            P_MW_RECORD_LETTER_INFO mwRecordLetterInfo = new P_MW_RECORD_LETTER_INFO();

                            mwRecordLetterInfo.LETTER_TYPE = letterType;
                            mwRecordLetterInfo.TEMPLATE_NAME = templateName;
                            mwRecordLetterInfo.MW_DSN_ID = model.P_MW_DSN.UUID;
                            mwRecordLetterInfo.LETTER_STATUS = ProcessingConstant.LETTER_CONFIRMED;

                            LetterInfoService.AddP_MW_RECORD_LETTER_INFO(mwRecordLetterInfo);

                        }


                    }
                    else
                    {
                        //Update DSN 

                        dsnRecord.SCANNED_STATUS_ID = svRecord.UUID;
                        dsnRecord.ISSUED_DATE = model.P_MW_DSN.ISSUED_DATE;
                        dsnRecord.RECORD_ID = model.P_MW_DSN.RECORD_ID;

                        DsnService.UpdateP_MW_DSN(dsnRecord);


                        //Save mwRecordLetterInfos
                        bool isUpdate = true;
                        P_MW_RECORD_LETTER_INFO mwRecordLetterInfo = LetterInfoService.GetP_MW_RECORD_LETTER_INFOByDsn(dsnRecord.UUID);

                        if (mwRecordLetterInfo == null)
                        {
                            mwRecordLetterInfo = new P_MW_RECORD_LETTER_INFO();
                            isUpdate = false;
                        }

                        mwRecordLetterInfo.LETTER_TYPE = letterType;
                        mwRecordLetterInfo.TEMPLATE_NAME = templateName;
                        mwRecordLetterInfo.MW_DSN_ID = dsnRecord.UUID;
                        mwRecordLetterInfo.LETTER_STATUS = ProcessingConstant.LETTER_CONFIRMED;

                        if (isUpdate)
                        {
                            LetterInfoService.UpdateP_MW_RECORD_LETTER_INFO(mwRecordLetterInfo);
                        }
                        else
                        {
                            LetterInfoService.AddP_MW_RECORD_LETTER_INFO(mwRecordLetterInfo);
                        }
                    }


                    serviceResult.Result = ServiceResult.RESULT_SUCCESS;

                }
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "ErrorMessage", new List<string>() { e.Message } } };
            }

            return new JsonResult() { Data = serviceResult };
        }


        public JsonResult GetDsnInfo(string DSN)
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                //Get DSN 
                P_MW_DSN dsnRecord = DsnService.GetP_MW_DSNByDsn(DSN);

                if (dsnRecord == null)
                {
                    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "DSN", new List<string>() { "DSN missing!" } } };

                }
                else
                {
                    //Get Letter Info
                    P_MW_RECORD_LETTER_INFO letterInfoRecord = LetterInfoService.GetP_MW_RECORD_LETTER_INFOByDsn(dsnRecord.UUID);

                    serviceResult.Data = new Dictionary<string, object>() { { "P_MW_DSN", dsnRecord }, { "P_MW_RECORD_LETTER_INFO", letterInfoRecord } };
                    serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                }

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "ErrorMessage", new List<string>() { e.Message } } };
            }

            return new JsonResult() { Data = serviceResult };

        }
    }
}