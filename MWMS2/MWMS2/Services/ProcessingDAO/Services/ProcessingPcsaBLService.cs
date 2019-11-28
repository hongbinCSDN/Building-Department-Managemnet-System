using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingPcsaBLService
    {
        private ProcessingPcsaDAOService DAService;
        protected ProcessingPcsaDAOService DA
        {
            get { return DAService ?? (DAService = new ProcessingPcsaDAOService()); }
        }

        private P_MW_ACK_LETTER_PREAUDIT_DAOService _ackLetterPerauditService;
        protected P_MW_ACK_LETTER_PREAUDIT_DAOService ackLetterPerauditService
        {
            get { return _ackLetterPerauditService ?? (_ackLetterPerauditService = new P_MW_ACK_LETTER_PREAUDIT_DAOService()); }
        }

        private P_MW_ACK_LETTER_DAOService _ackLetterService;
        protected P_MW_ACK_LETTER_DAOService ackLetterService
        {
            get { return _ackLetterService ?? (_ackLetterService = new P_MW_ACK_LETTER_DAOService()); }
        }

        private P_MW_DSN_DAOService _dsnService;
        protected P_MW_DSN_DAOService dsnService
        {
            get { return _dsnService ?? (_dsnService = new P_MW_DSN_DAOService()); }
        }

        private MwCrmInfoDaoImpl _crmInfoService;
        protected MwCrmInfoDaoImpl crmInfoService
        {
            get { return _crmInfoService ?? (_crmInfoService = new MwCrmInfoDaoImpl()); }
        }

        public ActionResult SearchPcsa(Fn01LM_PcsaSearchModel model)
        {
            if (model.IsGeneral)
            {
                GeneralPsac(model);
                if (!string.IsNullOrEmpty(model.RefNo))
                {
                    DA.SearchPcsa(model);
                }
            }
            else
            {
                DA.SearchPcsa(model);
            }
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public void GeneralPsac(Fn01LM_PcsaSearchModel model)
        {
           string uuid = DA.GetRandomPsac(model);

            if (!string.IsNullOrEmpty(uuid))
            {
                //Add to  P_MW_ACK_LETTER_PREAUDIT
                P_MW_ACK_LETTER ackLetter = ackLetterService.GetP_MW_ACK_LETTERByUuid(uuid);

                if(ackLetter != null)
                {
                    model.DSN = ackLetter.DSN;
                    model.RefNo = ackLetter.MW_NO;

                    P_MW_ACK_LETTER_PREAUDIT ackLetterPreaudit = new P_MW_ACK_LETTER_PREAUDIT();

                    ackLetterPreaudit.MW_ACK_LETTER_ID = ackLetter.UUID;
                    ackLetterPreaudit.SELECTION_DATE = DateTime.Now;
                    ackLetterPreaudit.PA_RESULT = ProcessingConstant.PRECOMM_SITE_AUDIT_RESULT_NOT_YET_INSPECTED;

                    ackLetterPerauditService.AddP_MW_ACK_LETTER_PREAUDIT(ackLetterPreaudit);
                }
            }
        }

        public void GetPsacDetailModel(string uuid , PsacDetailModel model)
        {
            model.P_MW_ACK_LETTER_PREAUDIT = ackLetterPerauditService.GetP_MW_ACK_LETTER_PREAUDITByUuid(uuid);

            if(model.P_MW_ACK_LETTER_PREAUDIT != null)
            {
                model.P_MW_ACK_LETTER = ackLetterService.GetP_MW_ACK_LETTERByUuid(model.P_MW_ACK_LETTER_PREAUDIT.MW_ACK_LETTER_ID);

                if(model.P_MW_ACK_LETTER == null)
                {
                    model.P_MW_ACK_LETTER = new P_MW_ACK_LETTER();
                }

                model.P_MW_DSN = dsnService.GetParentMwDsnByRecordId(model.P_MW_ACK_LETTER.MW_NO);

                model.PbpInfo = crmInfoService.GetV_CRM_INFOs(model.P_MW_ACK_LETTER.PBP_NO).FirstOrDefault();
                if(model.PbpInfo == null)
                {
                    model.PbpInfo = new V_CRM_INFO();
                }

                model.PrcInfo = crmInfoService.GetV_CRM_INFOs(model.P_MW_ACK_LETTER.PRC_NO).FirstOrDefault();
                if (model.PrcInfo == null)
                {
                    model.PrcInfo = new V_CRM_INFO();
                }


            }
            
        }

        public JsonResult ExportPcsa(Fn01LM_PcsaSearchModel model)
        {
            return new JsonResult() { Data = new { key = DA.ExportPcsa(model) } };
        }

        public ServiceResult UpdatePsac(PsacDetailModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                       int result = DA.UpdatePsac(db, model.P_MW_ACK_LETTER_PREAUDIT);

                        serviceResult.Result = result > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        AuditLogService.logDebug(e);
                        serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "Exception", new List<string>() { e.Message } } };
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    }
                }
                    
            }

            return serviceResult;
        }

        public ServiceResult RemovePsac(PsacDetailModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        int result = ackLetterPerauditService.RemoveP_MW_ACK_LETTER_PREAUDIT(db, model.P_MW_ACK_LETTER_PREAUDIT.UUID);

                        serviceResult.Result = result>0 ?ServiceResult.RESULT_SUCCESS: ServiceResult.RESULT_FAILURE;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        AuditLogService.logDebug(e);
                        serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "Exception", new List<string>() { e.Message } } };
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    }
                }

            }

            return serviceResult;
        }
    }
}