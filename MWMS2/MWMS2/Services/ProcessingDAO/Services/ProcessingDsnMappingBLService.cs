using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingDsnMappingBLService
    {
        private ProcessingDsnMappingDAOService DAOService;
        protected ProcessingDsnMappingDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingDsnMappingDAOService()); }
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

        private P_MW_REFERENCE_NO_DAOService _ReferenceService;
        protected P_MW_REFERENCE_NO_DAOService ReferenceService
        {
            get { return _ReferenceService ?? (_ReferenceService = new P_MW_REFERENCE_NO_DAOService()); }
        }


        public int GetDsnMappingTotal()
        {
            return DA.DsnMappingSearch(null, null, null).Count();
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
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "alertError", new List<string>() { "DSN missing!" } } };

                }
                else
                {
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

        public void Search(Fn02MWUR_DsnMappingModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_MW_DSN> dsnList = new List<P_MW_DSN>();

                dsnList = DA.DsnMappingSearch(model.DSN, model.FromDate, model.ToDate, db);

                model.Total = dsnList.Count();
                dsnList = dsnList.Skip((model.Page - 1) * model.Rpp).Take(model.Rpp).ToList();

                model.Data = new List<Dictionary<string, object>>();

                string statusName = "";

                foreach (var item in dsnList)
                {
                    switch (item.P_S_SYSTEM_VALUE.CODE)
                    {
                        case ProcessingConstant.DSN_REGISTRY_RECEIVED:
                            statusName = ProcessingConstant.DSN_DISPLAY_REGISTRY_RECEIVED;
                            break;
                        case ProcessingConstant.DSN_RE_SCANNED:
                            statusName = ProcessingConstant.DSN_DISPLAY_RE_SCANNED;
                            break;
                        case ProcessingConstant.DSN_CHECKLIST_SECTION_NEW:
                            statusName = ProcessingConstant.DSN_DISPLAY_NEW;
                            break;
                        case ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW:
                            statusName = ProcessingConstant.DSN_DISPLAY_NEW;
                            break;
                        case ProcessingConstant.MWU_RD_INCOMING_NEW:
                            statusName = ProcessingConstant.DSN_DISPLAY_INCOMING;
                            break;
                        case ProcessingConstant.DSN_RD_DELIVERED:
                            statusName = ProcessingConstant.DSN_DISPLAY_DELIVERED;
                            break;
                        case ProcessingConstant.DSN_RD_RE_SENT:
                            statusName = ProcessingConstant.DSN_DISPLAY_SENT;
                            break;
                        case ProcessingConstant.DSN_REGISTRY_RECEIPT_COUNTED:
                            statusName = ProcessingConstant.DSN_DISPLAY_COUNTED;
                            break;
                        default:
                            statusName = "";
                            break;
                    }

                    model.Data.Add(new Dictionary<string, object>() {
                        { "DSN", item.DSN },
                        { "Date",item.RD_DELIVERED_DATE==null?"": item.RD_DELIVERED_DATE.Value.ToString("dd/MM/yyyy") },
                        { "Time", item.RD_DELIVERED_DATE==null?"":item.RD_DELIVERED_DATE.Value.ToString("HH:mm") },
                        { "Received", statusName },
                    });
                }
            }
        }

        public JsonResult Assign(Fn02MWUR_DsnMappingModel model)
        {
            JsonResult jsonResult = new JsonResult();
            //F: From
            //E: Enquiry
            //C: Complaint
            if (model.DocType == "F")
            {
                jsonResult.Data = HandleAssignMwSubmission(model);
            }
            else if (model.DocType == "E")
            {
                jsonResult.Data = HandleAssignGeneralSubmission(model);
            }
            else if (model.DocType == "C")
            {
                jsonResult.Data = HandleAssignGeneralSubmission(model);
            }

            return jsonResult;
        }

        public ServiceResult HandleAssignMwSubmission(Fn02MWUR_DsnMappingModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.FormNo == ProcessingConstant.FORM_01 || model.FormNo == ProcessingConstant.FORM_03 || model.FormNo == ProcessingConstant.FORM_06 || model.FormNo == ProcessingConstant.FORM_32)
                        {
                            P_MW_REFERENCE_NO mwReferenceNo = ReferenceService.getMwReferenceNoByMwNo(model.ReferenceNo, db);

                            if (mwReferenceNo != null)
                            {
                                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "alertError", new List<string> { "MW Number already used." } } };
                                tran.Rollback();
                                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                                return serviceResult;
                            }

                            // New P_MW_REFERENCE_NO 
                            P_MW_REFERENCE_NO MwRefNo = new P_MW_REFERENCE_NO();
                            MwRefNo.REFERENCE_NO = model.ReferenceNo;
                            ReferenceService.AddP_MW_REFERENCE_NO(MwRefNo);

                            //Get P_MW_DSN Set REFERENCE_NO
                            P_MW_DSN MwDsn = DsnService.GetP_MW_DSNByDsn(model.DSN, db);
                            MwDsn.RECORD_ID = model.ReferenceNo;

                            //Save
                            P_S_SYSTEM_VALUE sv = SystemValueService.GetSSystemValueByCode(ProcessingConstant.WILL_SCAN);
                            MwDsn.SCANNED_STATUS_ID = sv.UUID;
                            DA.UpdateMwDSN(MwDsn, db);

                        }
                        else
                        {
                            P_MW_REFERENCE_NO mwReferenceNo = ReferenceService.getMwReferenceNoByMwNo(model.ReferenceNo, db);
                            if (mwReferenceNo == null)
                            {
                                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "alertError", new List<string> { "Reference No not found" } } };
                                tran.Rollback();
                                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                                return serviceResult;
                            }

                            //Get P_MW_DSN Set REFERENCE_NO
                            P_MW_DSN MwDsn = DsnService.GetP_MW_DSNByDsn(model.DSN, db);
                            MwDsn.RECORD_ID = model.ReferenceNo;

                            //Save
                            P_S_SYSTEM_VALUE sv = SystemValueService.GetSSystemValueByCode(ProcessingConstant.WILL_SCAN);
                            MwDsn.SCANNED_STATUS_ID = sv.UUID;
                            DA.UpdateMwDSN(MwDsn, db);
                        }

                        db.SaveChanges();
                        tran.Commit();
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        return serviceResult;
                    }
                    catch (Exception ex)
                    {
                        serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "exceptionError", new List<string> { ex.Message } } };
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        return serviceResult;
                    }
                }
            }

        }

        public ServiceResult HandleAssignGeneralSubmission(Fn02MWUR_DsnMappingModel model)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_REFERENCE_NO mwReferenceNo = ReferenceService.getMwReferenceNoByMwNo(model.ReferenceNo, db);
                        if (mwReferenceNo == null)
                        {
                            //Add P_MW_REFERENCE_NO
                            mwReferenceNo = new P_MW_REFERENCE_NO();
                            mwReferenceNo.REFERENCE_NO = model.ReferenceNo;

                            // New Obj and save;
                            ReferenceService.AddP_MW_REFERENCE_NO(mwReferenceNo, db);
                        }

                        //Get P_MW_DSN Set REFERENCE_NO
                        P_MW_DSN MwDsn = DsnService.GetP_MW_DSNByDsn(model.DSN, db);
                        MwDsn.RECORD_ID = model.ReferenceNo;

                        //Save
                        P_S_SYSTEM_VALUE sv = SystemValueService.GetSSystemValueByCode(ProcessingConstant.WILL_SCAN);
                        MwDsn.SCANNED_STATUS_ID = sv.UUID;
                        DA.UpdateMwDSN(MwDsn, db);

                        db.SaveChanges();
                        tran.Commit();
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        return serviceResult;
                    }
                    catch (Exception ex)
                    {
                        serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "exceptionError", new List<string> { ex.Message } } };
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        return serviceResult;
                    }
                }
            }
        }

    }
}