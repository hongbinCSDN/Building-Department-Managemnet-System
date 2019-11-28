using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.SysService
{
    public class ProcessingSADService
    {
        private P_S_MW_NUMBER_DAOService _MwNumberService;
        protected P_S_MW_NUMBER_DAOService MwNumberService
        {
            get { return _MwNumberService ?? (_MwNumberService = new P_S_MW_NUMBER_DAOService()); }
        }

        private const string SearchSAD_q = @"   SELECT D.UUID,
                                                       D.MWU_RECEIVED_DATE                     AS DT,
                                                       To_char(D.MWU_RECEIVED_DATE, 'hh24:mi') AS rec_time,
                                                       D.DSN,
                                                       D.RECORD_ID,
                                                       D.SUBMIT_TYPE,
                                                       D.FORM_CODE,
                                                       CASE
                                                         WHEN V.CODE = 'DSN_CHECKLIST_SECTION_NEW'
                                                               OR V.CODE = 'DSN_CHECKLIST_PHOTO_NEW'
                                                               OR V.CODE = 'MWU_RD_INCOMING_NEW' THEN 'New'
                                                         WHEN V.CODE = 'RESCANNED' THEN 'Rescan'
                                                         WHEN V.CODE = 'REGISTRY_RECEIVED' THEN 'Incoming'
                                                       END                                     AS SCANNED_CODE
                                                FROM   P_MW_DSN D
                                                       INNER JOIN P_S_SYSTEM_VALUE V
                                                               ON D.SCANNED_STATUS_ID = V.UUID
                                                WHERE  1 = 1 
                                                ";

        private const string SearchDSN_q = @" select * from P_MW_Scanned_document doc inner join p_mw_dsn dsn on dsn.uuid = doc.dsn_id
                                                where 1=1 ";

        private const string SearchScanDoc_q = @"Select * from P_MW_Scanned_document doc
                                                Inner join p_mw_dsn dsn on dsn.uuid = doc.dsn_id
                                                Where 1=1
                                                And dsn.dsn = :DSN ";

        public string SearchSAD_whereq(Fn02MWUR_SADModel model)
        {
            string whereq = "";
            List<string> systemCodeWorkFlowArrayList = new List<string>();
            List<string> systemCodeNonWorkFlowArrayList = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereq = @" and DSN like :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.DSN.Trim() + "%");
            }

            if (model.ReceiveDateFrom != null)
            {
                whereq += "\r\n\t" + " AND D.MWU_RECEIVED_DATE  >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.ReceiveDateFrom);
            }

            if (model.ReceiveDateTo != null)
            {
                whereq += "\r\n\t" + " AND D.MWU_RECEIVED_DATE  <= :DateTo";
                model.QueryParameters.Add("DateTo", model.ReceiveDateTo);


            }
            whereq += "\r\n\t" + " AND    V.UUID IN (SELECT UUID FROM P_S_SYSTEM_VALUE WHERE CODE IN (:nonWorkflowCodeList))";
            if (!string.IsNullOrEmpty(model.Status))
            {
                if (ProcessingConstant.DSN_REGISTRY_RECEIVED.Equals(model.Status))
                {
                    systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_REGISTRY_RECEIVED);
                }
                else if (ProcessingConstant.DSN_RE_SCANNED.Equals(model.Status))
                {
                    systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_RE_SCANNED);
                }
                else if (ProcessingConstant.MWU_RD_INCOMING_NEW.Equals(model.Status))
                {
                    systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_CHECKLIST_SECTION_NEW);
                    systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW);
                    systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.MWU_RD_INCOMING_NEW);
                }
            }
            else
            {
                systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_REGISTRY_RECEIVED);
                systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_RE_SCANNED);
                systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_CHECKLIST_SECTION_NEW);
                systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW);
                systemCodeNonWorkFlowArrayList.Add(ProcessingConstant.MWU_RD_INCOMING_NEW);

            }
            model.QueryParameters.Add("nonWorkflowCodeList", systemCodeNonWorkFlowArrayList);


            return whereq;
        }
        public Fn02MWUR_SADModel SearchSAD(Fn02MWUR_SADModel model)
        {
            model.Query = SearchSAD_q;

            model.QueryWhere = SearchSAD_whereq(model);
            model.Search();
            return model;
        }

        public string Excel(Fn02MWUR_SADModel model)
        {
            model.Query = SearchSAD_q;
            model.QueryWhere = SearchSAD_whereq(model);
            return model.Export("Scan and Assign Document " + DateTime.Now.ToLongDateString());
        }
        public ServiceResult CheckDSN(string dsn)
        {
            using (EntitiesMWProcessing db=new EntitiesMWProcessing())
            {
                P_MW_DSN mw_dsn = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();
                if (mw_dsn != null)
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                else 
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE
                        ,Message=new List<string>()
                        {
                            "Invalid Document S/N"
                        }
                    };
            }
        }

        public Fn02MWUR_SADDisplayModel DetailDoc(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from dsn in db.P_MW_DSN
                        where dsn.UUID == uuid
                        select new Fn02MWUR_SADDisplayModel()
                        {
                            UUID = dsn.UUID
,
                            DSN = dsn.DSN
,
                            SubDocNo = dsn.RECORD_ID
,
                            Form = dsn.FORM_CODE
,
                            SubmissionType = dsn.SUBMIT_TYPE
,
                            SSPSubmitted = dsn.SSP_SUBMITTED
                        }).FirstOrDefault();
            }
        }
        public Fn02MWUR_SADDisplayModel DetailDocByDSN(string dsn_no)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from dsn in db.P_MW_DSN
                        where dsn.DSN == dsn_no
                        select new Fn02MWUR_SADDisplayModel()
                        {
                            UUID = dsn.UUID
,
                            DSN = dsn.DSN
,
                            SubDocNo = dsn.RECORD_ID
,
                            Form = dsn.FORM_CODE
,
                            SubmissionType = dsn.SUBMIT_TYPE
,
                            SSPSubmitted = dsn.SSP_SUBMITTED
                        }).FirstOrDefault();
            }
        }
        public Fn02MWUR_SADModel SearchDSN(Fn02MWUR_SADModel model)
        {
            model.Query = SearchDSN_q;

            model.QueryWhere = SearchDSN_whereq(model);
            model.Search();
            return model;
        }

        public string SearchDSN_whereq(Fn02MWUR_SADModel model)
        {
            string whereq = "";
            whereq += "\r\n\t" + " AND DSN.DSN  = :DSN";
            model.QueryParameters.Add("DSN", model.DSN);

            return whereq;
        }

        public Fn02MWUR_SADDisplayModel CreateNewDSN(Fn02MWUR_SADDisplayModel model)
        {
            try
            {
                string maxNumber = MwNumberService.GetNewNumberOfDsn();

                //Add DSN 
                P_MW_DSN dsnModel = new P_MW_DSN();
                dsnModel.DSN = maxNumber;
                dsnModel.MWU_RECEIVED_DATE = DateTime.Now;

                model.DSN = dsnModel.DSN;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return model;
        }

        public Fn02MWUR_SADDisplayModel SearchScanDoc(Fn02MWUR_SADDisplayModel model)
        {
            model.Query = SearchScanDoc_q;
            model.QueryParameters.Add("DSN", model.DSN);
            model.Search();
            return model;
        }

        public ServiceResult CompleteScan(Fn02MWUR_SADDisplayModel model)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        ServiceResult serviceResult = new ServiceResult();
                        P_MW_DSN_DAOService mwDSNService = new P_MW_DSN_DAOService();
                        P_MW_DSN dsn = db.P_MW_DSN.Where(m => m.DSN == model.DSN).FirstOrDefault() ;
                        if (dsn == null)
                        {
                            serviceResult.Result = ServiceResult.RESULT_FAILURE;
                            serviceResult.Message.Add("Not DSN");
                            return serviceResult;
                        }

                        P_S_SYSTEM_VALUE p_S_SYSTEM_VALUE = null;
                        P_S_SYSTEM_VALUE doc_value = ProcessingSystemValueService.GetSystemValueByUUID(dsn.SCANNED_STATUS_ID);
                        if (ProcessingConstant.MWU_RD_INCOMING_NEW.Equals(doc_value.CODE))
                        {
                            p_S_SYSTEM_VALUE = ProcessingSystemValueService.GetSystemListByCode(ProcessingConstant.MWU_RD_INCOMING_SCANNED).FirstOrDefault();
                        }
                        else
                        {
                            p_S_SYSTEM_VALUE = ProcessingSystemValueService.GetSystemListByCode(ProcessingConstant.DSN_SCANNED).FirstOrDefault();
                        }
                        dsn.SCANNED_STATUS_ID = p_S_SYSTEM_VALUE.UUID;
                        dsn.SSP_SUBMITTED = model.SSPSubmitted;

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        AuditLogService.logDebug(ex);
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
    }
}