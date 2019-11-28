using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingIncomingBLService
    {
        private ProcessingIncomingDAOService DAOService;
        protected ProcessingIncomingDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingIncomingDAOService()); }
        }

        private P_S_MW_NUMBER_DAOService _MwNumberService;
        protected P_S_MW_NUMBER_DAOService MwNumberService
        {
            get { return _MwNumberService ?? (_MwNumberService = new P_S_MW_NUMBER_DAOService()); }
        }

        private P_MW_REFERENCE_NO_DAOService _MwReferenceNoService;
        protected P_MW_REFERENCE_NO_DAOService MwReferenceNoService
        {
            get { return _MwReferenceNoService ?? (_MwReferenceNoService = new P_MW_REFERENCE_NO_DAOService()); }
        }

        private P_MW_DSN_DAOService _DsnService;
        protected P_MW_DSN_DAOService DsnService
        {
            get { return _DsnService ?? (_DsnService = new P_MW_DSN_DAOService()); }
        }

        private ProcessingSystemValueDAOService _SystemValueService;
        protected ProcessingSystemValueDAOService SystemValueService
        {
            get { return _SystemValueService ?? (_SystemValueService = new ProcessingSystemValueDAOService()); }
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
                model.MWU_RECEIVED_DATE = DateTime.Now;

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

        public JsonResult SubmitDsnInfo(Fn02MWUR_IncomingModel model)
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
                    P_S_SYSTEM_VALUE svRecord = SystemValueService.GetSSystemValueByCode(ProcessingConstant.MWU_RD_INCOMING_NEW);


                    //Check DSN 
                    //Get DSN 
                    P_MW_DSN dsnRecord = DsnService.GetP_MW_DSNByDsn(model.P_MW_DSN.DSN);

                    if (dsnRecord == null)
                    {
                        //Create
                        model.P_MW_DSN.SCANNED_STATUS_ID = svRecord.UUID;
                        DsnService.AddP_MW_DSN(model.P_MW_DSN);
                    }
                    else
                    {
                        //Update DSN 

                        dsnRecord.SCANNED_STATUS_ID = svRecord.UUID;
                        dsnRecord.MWU_RECEIVED_DATE = model.P_MW_DSN.MWU_RECEIVED_DATE;
                        dsnRecord.RECORD_ID = model.P_MW_DSN.RECORD_ID;

                        DsnService.UpdateP_MW_DSN(dsnRecord);
                        //DA.UpdateDsn(model.P_MW_DSN);
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

                    serviceResult.Data = new Dictionary<string, object>() { { "P_MW_DSN", dsnRecord } };
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