using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingTDL_VerificationBLService
    {
        const string VER_FORM_INFO = "formInfo";
        const string VER_FORM_PBP = "formPBP";
        const string VER_FORM_PRC = "formPRC";

        private ProcessingTdlDAOService TdlDaoService;
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SysDAOService SysDAOService;

        protected ProcessingTdlDAOService DA
        {
            get { return TdlDaoService ?? (TdlDaoService = new ProcessingTdlDAOService()); }
        }

        protected SysDAOService sysDAOService
        {
            get { return SysDAOService ?? (SysDAOService = new SysDAOService()); }
        }

        private ProcessingDataEntryDAOService DataEntryService;
        protected ProcessingDataEntryDAOService DeDA
        {
            get { return DataEntryService ?? (DataEntryService = new ProcessingDataEntryDAOService()); }
        }

        private P_MW_APPOINTED_PROFESSIONAL_DAOService _mwAppointedProfessionalService;
        protected P_MW_APPOINTED_PROFESSIONAL_DAOService mwAppointedProfessionalService
        {
            get { return _mwAppointedProfessionalService ?? (_mwAppointedProfessionalService = new P_MW_APPOINTED_PROFESSIONAL_DAOService()); }
        }

        private MwCrmInfoDaoImpl mwCrmService;
        protected MwCrmInfoDaoImpl mwCrmInfoService
        {
            get { return mwCrmService ?? (mwCrmService = new MwCrmInfoDaoImpl()); }
        }

        private P_MW_RECORD_ITEM_DAOService itemService;
        protected P_MW_RECORD_ITEM_DAOService mwRecordItemService
        {
            get { return itemService ?? (itemService = new P_MW_RECORD_ITEM_DAOService()); }
        }

        private P_MW_FORM_09_DAOService form09DaoService;
        protected P_MW_FORM_09_DAOService mwForm09Service
        {
            get { return form09DaoService ?? (form09DaoService = new P_MW_FORM_09_DAOService()); }
        }

        private P_S_MW_ITEM_DAOService _sMwItemService;
        protected P_S_MW_ITEM_DAOService sMwItemService
        {
            get { return _sMwItemService ?? (_sMwItemService = new P_S_MW_ITEM_DAOService()); }
        }

        private P_MW_FORM_DAOService _mwFormService;
        protected P_MW_FORM_DAOService mwFormService
        {
            get { return _mwFormService ?? (_mwFormService = new P_MW_FORM_DAOService()); }
        }

        private P_MW_RECORD_DAOService _mwRecordService;
        protected P_MW_RECORD_DAOService mwRecordService
        {
            get { return _mwRecordService ?? (_mwRecordService = new P_MW_RECORD_DAOService()); }
        }

        private P_MW_REFERENCE_NO_DAOService _mwReferenceNoService;
        protected P_MW_REFERENCE_NO_DAOService mwReferenceNoService
        {
            get { return _mwReferenceNoService ?? (_mwReferenceNoService = new P_MW_REFERENCE_NO_DAOService()); }
        }

        private P_S_CAPACITY_OF_PRC_DAOService _sCapacityOfPRCService;
        protected P_S_CAPACITY_OF_PRC_DAOService sCapacityOfPRCService
        {
            get { return _sCapacityOfPRCService ?? (_sCapacityOfPRCService = new P_S_CAPACITY_OF_PRC_DAOService()); }
        }

        private P_MW_RECORD_ITEM_CHECKLIST_DAOService _ItemChecklistService;
        protected P_MW_RECORD_ITEM_CHECKLIST_DAOService ItemChecklistService
        {
            get { return _ItemChecklistService ?? (_ItemChecklistService = new P_MW_RECORD_ITEM_CHECKLIST_DAOService()); }
        }

        private P_MW_RECORD_ITEM_INFO_DAOService _ItemInfoService;
        protected P_MW_RECORD_ITEM_INFO_DAOService ItemInfoService
        {
            get { return _ItemInfoService ?? (_ItemInfoService = new P_MW_RECORD_ITEM_INFO_DAOService()); }
        }

        private P_MW_VERIFICATION_DAPService _mwVerificationService;
        protected P_MW_VERIFICATION_DAPService mwVerificationService
        {
            get { return _mwVerificationService ?? (_mwVerificationService = new P_MW_VERIFICATION_DAPService()); }
        }

        private ProcessingSystemValueDAOService _systemValueService;
        protected ProcessingSystemValueDAOService systemValueService
        {
            get { return _systemValueService ?? (_systemValueService = new ProcessingSystemValueDAOService()); }
        }

        public void GetFormData(VerificaionFormModel model)
        {
            // Set current LoginAc info
            SYS_POST sysPost = Utility.SessionUtil.LoginPost;
            if (sysPost != null)
            {
                String rankId = sysPost.SYS_RANK_ID;
                if (rankId != null)
                {
                    SYS_RANK SYS_RANK = sysDAOService.getSysRankByRankUuid(rankId);
                    if (SYS_RANK != null)
                    {
                        model.CurrentRankOfLoginAc = SYS_RANK.DESCRIPTION;
                    }
                }
            }

            if (model.IsSummary)
            {
                model.POList = DA.GetPOListByTOID(sysPost.UUID);
            }

            var admin = DA.GetObjectData<SYS_ROLE>(@"SELECT R.*
                                                    FROM   SYS_POST_ROLE PR
                                                           JOIN SYS_POST P
                                                             ON PR.SYS_POST_ID = P.UUID
                                                                AND P.CODE = :UserCode
                                                           JOIN SYS_ROLE R
                                                             ON PR.SYS_ROLE_ID = R.UUID
                                                                AND R.CODE = :RoleCode ",
                                                                new OracleParameter[] {
                                                                    new OracleParameter(":UserCode",SessionUtil.LoginPost.CODE),
                                                                    new OracleParameter(":RoleCode",ProcessingConstant.ADMIN)
                                                                }).FirstOrDefault();

            //Check task user id
            if (admin != null)
            {
                //model.IsReadonly = false;
            }
            else if (model.TaskUserID != sysPost.UUID)
            {
                model.IsReadonly = true;
            }

            //Get P_MW_RECORD
            model.P_MW_RECORD = DA.GetP_MW_RECORD(model.R_UUID);

            //Get P_MW_VERIFICATION
            model.P_MW_VERIFICATION = mwVerificationService.GetP_MW_VERIFICATIONByUuid(model.V_UUID);
            if (model.P_MW_VERIFICATION != null)
            {
                if (model.P_MW_VERIFICATION.HANDLING_UNIT == ProcessingConstant.HANDLING_UNIT_SMM)
                {
                    model.HandlingUnit = ProcessingConstant.HANDLING_UNIT_SMM;
                }
                else
                {
                    model.HandlingUnit = ProcessingConstant.HANDLING_UNIT_PEM;
                }

            }

            //Get P_MW_REFERENCE_NO
            model.P_MW_REFERENCE_NO = DA.GetP_MW_REFERENCE_NO(model.P_MW_RECORD.REFERENCE_NUMBER);

            //Get P_MW_DSN
            model.P_MW_DSN = DA.GetP_MW_DSN(model.P_MW_RECORD.MW_DSN);

            //Get P_MW_SCANNED_DOCUMENTs
            model.P_MW_SCANNED_DOCUMENTsIC = DA.GetP_MW_SCANNED_DOCUMENTsIC(model.P_MW_REFERENCE_NO.REFERENCE_NO, model.P_MW_RECORD.UUID);
            if (model.P_MW_SCANNED_DOCUMENTsIC == null)
            {
                model.P_MW_SCANNED_DOCUMENTsIC = new List<P_MW_SCANNED_DOCUMENT>();
            }

            model.P_MW_SCANNED_DOCUMENTsNIC = DA.GetP_MW_SCANNED_DOCUMENTsNIC(model.P_MW_REFERENCE_NO.REFERENCE_NO);
            if (model.P_MW_SCANNED_DOCUMENTsNIC == null)
            {
                model.P_MW_SCANNED_DOCUMENTsNIC = new List<P_MW_SCANNED_DOCUMENT>();
            }

            //Get P_MW_RECORD_PSAC
            model.P_MW_RECORD_PSAC = DA.GetP_MW_RECORD_PSAC(model.P_MW_RECORD.UUID, model.HandlingUnit);
            if (model.P_MW_RECORD_PSAC == null)
            {
                model.P_MW_RECORD_PSAC = new P_MW_RECORD_PSAC();
                //model.P_MW_RECORD_PSAC.RECORD_ID = model.P_MW_RECORD.UUID;
                //DA.AddP_MW_RECORD_PSAC(model.P_MW_RECORD_PSAC);
            }

            //GetP_MW_RECORD_SAC
            model.P_MW_RECORD_SAC = DA.GetP_MW_RECORD_SAC(model.P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_SAC, model.HandlingUnit);
            if (model.P_MW_RECORD_SAC == null)
            {
                model.P_MW_RECORD_SAC = new P_MW_RECORD_SAC();
                model.P_MW_RECORD_SAC.SYS_PAGE_CODE = ProcessingConstant.PAGE_CODE_SAC;
            }

            //Set IsSAC
            if (ProcessingConstant.HANDLING_UNIT_PEM.Equals(model.P_MW_VERIFICATION.HANDLING_UNIT))
            {
                model.IsSAC = (ProcessingConstant.FLAG_Y.Equals(model.P_MW_RECORD.SITE_AUDIT_RELATED_MW));
            }
            else if (ProcessingConstant.HANDLING_UNIT_SMM.Equals(model.P_MW_VERIFICATION.HANDLING_UNIT))
            {
                model.IsSAC = (ProcessingConstant.FLAG_Y.Equals(model.P_MW_RECORD.SITE_AUDIT_RELATED_SB));
            }

            //Get P_MW_RECORD_PSAC_IMAGEs
            model.P_MW_RECORD_PSAC_IMAGEs = DA.GetP_MW_RECORD_PSAC_IMAGEs(model.P_MW_RECORD.UUID);
            if (model.P_MW_RECORD_PSAC_IMAGEs == null)
            {
                model.P_MW_RECORD_PSAC_IMAGEs = new List<P_MW_RECORD_PSAC_IMAGE>();
            }

            model.P_MW_RECORD_PSAC_IMAGEsPlan = model.P_MW_RECORD_PSAC_IMAGEs.Where(w => w.DOCUMENT_TYPE == "Plan").ToList();
            if (model.P_MW_RECORD_PSAC_IMAGEsPlan == null)
            {
                model.P_MW_RECORD_PSAC_IMAGEsPlan = new List<P_MW_RECORD_PSAC_IMAGE>();
            }

            model.P_MW_RECORD_PSAC_IMAGEsPhoto = model.P_MW_RECORD_PSAC_IMAGEs.Where(w => w.DOCUMENT_TYPE == "Photo").ToList();
            if (model.P_MW_RECORD_PSAC_IMAGEsPhoto == null)
            {
                model.P_MW_RECORD_PSAC_IMAGEsPhoto = new List<P_MW_RECORD_PSAC_IMAGE>();
            }

            //Get P_MW_RECORD_ITEMs
            model.P_MW_RECORD_ITEMs = DA.GetP_MW_RECORD_ITEMs(model.P_MW_RECORD.UUID).OrderBy(o => o.ORDERING).ToList();
            if (model.P_MW_RECORD_ITEMs == null)
            {
                model.P_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();
            }

            //Start modify by dive 20191105
            //Distinguish PEM SMM
            //Get signboard item
            P_S_SYSTEM_VALUE svSignBoardItem = systemValueService.GetSSystemValueByTypeAndCode(ProcessingConstant.TYPE_S_MW_ITEM,
                                ProcessingConstant.CODE_SIGNBOARD_ITEMS);
            string[] SignboardItemList = svSignBoardItem.DESCRIPTION.Split(',');

            bool isSignboard = ProcessingConstant.HANDLING_UNIT_SMM.Equals(model.P_MW_VERIFICATION.HANDLING_UNIT);

            model.FilterP_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();

            for (int i = 0; i < model.P_MW_RECORD_ITEMs.Count(); i++)
            {
                if ((isSignboard && SignboardItemList.Contains(model.P_MW_RECORD_ITEMs[i].MW_ITEM_CODE)) || (!isSignboard && !SignboardItemList.Contains(model.P_MW_RECORD_ITEMs[i].MW_ITEM_CODE)))
                {
                    model.FilterP_MW_RECORD_ITEMs.Add(model.P_MW_RECORD_ITEMs[i]);
                }
            }

            //End modify by dive 20191105

            //Get Final P_MW_RECORD_ITEMs
            model.FinalP_MW_RECORD_ITEMs = mwRecordItemService.GetFinalP_MW_RECORD_ITEMsByRefNo(model.P_MW_REFERENCE_NO.REFERENCE_NO);
            if (model.FinalP_MW_RECORD_ITEMs == null)
            {
                model.FinalP_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();
            }

            //Get P_MW_RECORD_FORM_CHECKLIST
            model.P_MW_RECORD_FORM_CHECKLIST = DA.GetP_MW_RECORD_FORM_CHECKLIST(model.P_MW_RECORD.UUID, model.V_UUID, model.HandlingUnit);
            if (model.P_MW_RECORD_FORM_CHECKLIST == null)
            {
                model.P_MW_RECORD_FORM_CHECKLIST = new P_MW_RECORD_FORM_CHECKLIST();
                model.P_MW_RECORD_FORM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
            }

            //Get ITEM_CODEs
            string ITEM_NOs = "";
            foreach (var item in model.P_MW_RECORD_ITEMs)
            {
                ITEM_NOs += "," + string.Format("'{0}'", item.MW_ITEM_CODE);
            }

            if (!string.IsNullOrEmpty(ITEM_NOs))
            {
                ITEM_NOs = ITEM_NOs.Substring(1);
            }
            else
            {
                ITEM_NOs = "''";

            }

            int MW_ITEM_VERSION = 1;

            //Get P_S_MW_ITEM_CHECKBOXs
            model.P_S_MW_ITEM_CHECKBOXs = DA.GetP_S_MW_ITEM_CHECKBOXs(ITEM_NOs, MW_ITEM_VERSION);

            //Get P_S_MW_ITEM_NATUREs
            model.P_S_MW_ITEM_NATUREs = DA.GetP_S_MW_ITEM_NATUREs(ITEM_NOs, MW_ITEM_VERSION);

            //Get RecordItemCheckLists
            // model.RecordItemCheckLists = DA.GetRecordItemCheckLists(model.P_MW_RECORD.UUID);

            //Get RecordItemCheckListItems
            model.RecordItemCheckListItems = DA.GetRecordItemCheckListItems(model.P_MW_RECORD.UUID, MW_ITEM_VERSION, model.V_UUID);

            //Start modify by dive 20191105
            //Filter by item no
            List<RecordItemCheckListItem> FilterRecordItemCheckListItems = new List<RecordItemCheckListItem>();
            foreach (var item in model.FilterP_MW_RECORD_ITEMs)
            {
                FilterRecordItemCheckListItems.AddRange(model.RecordItemCheckListItems.Where(w => w.MW_ITEM_NO == item.MW_ITEM_CODE).ToList());
            }

            model.RecordItemCheckListItems = FilterRecordItemCheckListItems;
            //End modify by dive 20191105

            //Get P_MW_ADDRESSes
            model.P_MW_ADDRESSes = DA.GetP_MW_ADDRESSes(model.P_MW_RECORD.UUID);

            //Get P_MW_APPOINTED_PROFESSIONALs
            // Ordering 1 --AP   --PART_A
            // Ordering 2 --RSE  --PART_A
            // Ordering 3 --RGE  --PART_A
            // Ordering 4 --PRC  --PART_A
            // Ordering 5 --AP   --PART_B
            // Ordering 6 --RSE  --PART_C
            // Ordering 7 --RGE  --PART_D
            // Ordering 8 --PRC  --PART_E
            model.P_MW_APPOINTED_PROFESSIONALs = DA.GetP_MW_APPOINTED_PROFESSIONALs(model.P_MW_RECORD.UUID).OrderBy(o => o.ORDERING).ToList();

            if (model.P_MW_APPOINTED_PROFESSIONALs == null)
            {
                model.P_MW_APPOINTED_PROFESSIONALs = new List<P_MW_APPOINTED_PROFESSIONAL>();
            }

            for (int i = model.P_MW_APPOINTED_PROFESSIONALs.Count(); i < 8; i++)
            {
                model.P_MW_APPOINTED_PROFESSIONALs.Add(new P_MW_APPOINTED_PROFESSIONAL());
            }

            model.P_MW_SUMMARY_MW_ITEM_CHECKLIST = DA.GetP_MW_SUMMARY_MW_ITEM_CHECKLIST(model.P_MW_RECORD.UUID, model.HandlingUnit);
            if (model.P_MW_SUMMARY_MW_ITEM_CHECKLIST == null)
            {
                model.P_MW_SUMMARY_MW_ITEM_CHECKLIST = new P_MW_SUMMARY_MW_ITEM_CHECKLIST();
                model.P_MW_SUMMARY_MW_ITEM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
            }

            model.P_MW_FORM_09s = DA.GetP_MW_FORM_09s(model.P_MW_RECORD.UUID).OrderBy(o => o.ORDERING).ToList();
            if (model.P_MW_FORM_09s == null)
            {
                model.P_MW_FORM_09s = new List<P_MW_FORM_09>();
            }

            model.P_MW_FORM = DA.GetP_MW_FORM(model.P_MW_RECORD.UUID);
            if (model.P_MW_FORM == null)
            {
                model.P_MW_FORM = new P_MW_FORM();
            }

            model.AppointedAP = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedRSE = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedRGE = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedPRC = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedOther0 = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedOther1 = new P_MW_APPOINTED_PROFESSIONAL();

            GetPbpPrcListBySecondEntry(model);

            //Notification
            LoadDefaultNotificationValue(model.P_MW_RECORD, model.P_MW_FORM, model.P_MW_RECORD_FORM_CHECKLIST, model.P_MW_APPOINTED_PROFESSIONALs);

            //Appointed Professional
            ValidateProfessionList(model.P_MW_RECORD, model.P_MW_FORM, model.P_MW_REFERENCE_NO, model.P_MW_RECORD_FORM_CHECKLIST);

            //PBP Checking
            prefillPBPValid(model.P_MW_RECORD, model.AppointedAP, model.AppointedRSE, model.AppointedRGE, model.AppointedPRC, model.P_MW_RECORD_FORM_CHECKLIST, model.P_MW_FORM, model.AppointedOther1, model.AppointedOther0, model.P_MW_FORM_09s, model.AppointedAPForm8, model);

            //PRC Checking

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                    && string.IsNullOrEmpty(model.AppointedPRC.ENGLISH_NAME)
                    && string.IsNullOrEmpty(model.AppointedPRC.CHINESE_NAME)
                    && string.IsNullOrEmpty(model.AppointedPRC.ENGLISH_COMPANY_NAME)
                    && string.IsNullOrEmpty(model.AppointedPRC.CHINESE_COMPANY_NAME))
            {
                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_NAME = (ProcessingConstant.CHECKING_NA);
                model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP_RMK = (ProcessingConstant.NA);
                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID_RMK = (ProcessingConstant.NA);
                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_SIGN_RMK = (ProcessingConstant.NA);
                model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE_RMK = (ProcessingConstant.NA);
                model.P_MW_RECORD_FORM_CHECKLIST.FORM07_DEC_S48_2_RMK = (ProcessingConstant.NA);
                model.P_MW_RECORD_FORM_CHECKLIST.FORM07_DEC_S48_4_RMK = (ProcessingConstant.NA);
            }
            else
            {
                prefillCapacityOfPRC(model);
                prefillValidityOfAs(model);
            }

            //Pre
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE == ProcessingConstant.FORM_02 || model.P_MW_RECORD.S_FORM_TYPE_CODE == ProcessingConstant.FORM_04 || model.P_MW_RECORD.S_FORM_TYPE_CODE == ProcessingConstant.FORM_10)
            {
                //Get Final Record 
                model.FinalRecord = mwRecordService.GetFinalMwRecordByRefNoUuid(model.P_MW_REFERENCE_NO.UUID);


                if (model.FinalRecord != null)
                {
                    //Get Pre Item
                    model.PreMwItems = mwRecordItemService.GetPreItemsByRecordID(model.FinalRecord.UUID);
                }

                //model.FinalRecordItem = ProcessingConstant.FLAG_Y;
                if (model.PreMwItems != null && model.PreMwItems.Count() > 0)
                {
                    //Get VariationDeclared
                    model.PreRecordItemChecklists = ItemChecklistService.GetPreRecordItemChecklists(model.R_UUID, model.V_UUID).OrderBy(o => o.ORDERING).ToList();


                    ////FinalRecordItem
                    //List<string> ItemCodes = new List<string>();

                    //ItemCodes.AddRange((from x in model.PreMwItems select x.FianlItemUUID));

                    //List<P_MW_RECORD_ITEM_INFO> ItemInfoList = ItemInfoService.GetP_MW_RECORD_ITEM_INFOsByItemCodes(ItemCodes.ToArray());


                    //if (ItemInfoList != null && ItemInfoList.Count() != model.PreMwItems.Count())
                    //{
                    //    model.FinalRecordItem = ProcessingConstant.FLAG_N;
                    //}
                }

                //GetFianlRecordItemCheckListItems
                model.FinalRecordItemCheckListItems = DA.GetFinalRecordItemCheckListItems(model.FinalRecord.UUID, MW_ITEM_VERSION);


            }

            //model.P_MW_RECORD_FORM_CHECKLIST
            //DA.SaveP_MW_RECORD_FORM_CHECKLIST()
            try
            {
                string statysCode = ProcessingConstant.FORM_SAVE_MODE_DRAFT;
                DA.SaveP_MW_RECORD_FORM_CHECKLIST(model.P_MW_RECORD_FORM_CHECKLIST, model.P_MW_RECORD.UUID, model.V_UUID, model.P_MW_RECORD.S_FORM_TYPE_CODE, statysCode, new EntitiesMWProcessing());
            }
            catch (Exception e)
            {
                AuditLogService.logDebug(e);
            }



        }

        public JsonResult UpdateSPO(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                model.P_MW_RECORD_FORM_CHECKLIST.MW_VERIFICATION_ID = model.V_UUID;
                model.P_MW_RECORD_FORM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
                serviceResult.Result = DA.UpdateSPO(model.P_MW_RECORD, model.P_MW_RECORD_FORM_CHECKLIST) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult SaveAndNext(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Summary Save And Next

                if (model.IsSummary)
                {
                    model.P_MW_SUMMARY_MW_ITEM_CHECKLIST.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                    model.P_MW_SUMMARY_MW_ITEM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
                    return new JsonResult()
                    {
                        Data = DA.SaveSummary(model.P_MW_SUMMARY_MW_ITEM_CHECKLIST)
                    };
                }

                model.P_MW_RECORD_ITEM_CHECKLIST_ITEMs = new List<P_MW_RECORD_ITEM_CHECKLIST_ITEM>();

                if (model.RecordItemCheckListItems == null)
                {
                    model.RecordItemCheckListItems = new List<RecordItemCheckListItem>();
                }

                if (model.FinalRecordItemCheckListItems == null)
                {
                    model.FinalRecordItemCheckListItems = new List<RecordItemCheckListItem>();
                }

                if (model.PreMwItems == null)
                {
                    model.PreMwItems = new List<PreMwItem>();

                }


                serviceResult = DA.SaveAndNext(model.P_MW_RECORD, model.P_MW_RECORD_FORM_CHECKLIST, model.P_MW_RECORD_PSAC, model.P_MW_RECORD_ITEM_CHECKLIST_ITEMs, model.P_MW_RECORD_SAC, model.RecordItemCheckListItems, model.V_UUID, model.PreRecordItemChecklists, model.PreMwItems, model.FinalRecordItemCheckListItems, model.IsSubmit, model.HandlingUnit);
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult AddAddressInfo(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                DA.AddP_MW_ADDRESS(model.P_MW_ADDRESS);

                P_MW_RECORD_ADDRESS_INFO record = new P_MW_RECORD_ADDRESS_INFO();

                record.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                record.MW_ADDRESS_ID = model.P_MW_ADDRESS.UUID;

                DA.AddP_MW_RECORD_ADDRESS_INFO(record);

                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                serviceResult.Data = model.P_MW_ADDRESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult DeleteAddressInfo(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Delete Address Info 
                P_MW_RECORD_ADDRESS_INFO record = new P_MW_RECORD_ADDRESS_INFO();

                record.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                record.MW_ADDRESS_ID = model.P_MW_ADDRESS.UUID;

                DA.DeleteP_MW_RECORD_ADDRESS_INFO(record);

                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult AddPsacImage(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Checking DSN
                P_MW_DSN DsnRecord = DA.GetP_MW_DSN(model.P_MW_RECORD_PSAC_IMAGE.DSN);

                string textName = model.P_MW_RECORD_PSAC_IMAGE.DOCUMENT_TYPE == "Plan" ? "PlanDSN" : "PhotoDSN";

                if (DsnRecord == null)
                {
                    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    serviceResult.ErrorMessages = new Dictionary<string, List<string>>()
                    {
                        { textName,new List<string>(){ "DSN not exists , please check" } }
                    };
                }


                model.P_MW_RECORD_PSAC_IMAGE.P_DSN_ID = DsnRecord.UUID;

                //Add P_MW_RECORD_PSAC_IMAGE
                DA.AddP_MW_RECORD_PSAC_IMAGE(model.P_MW_RECORD_PSAC_IMAGE);

                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                serviceResult.Data = model.P_MW_RECORD_PSAC_IMAGE;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult DeletePsacImage(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Delete P_MW_RECORD_PSAC_IMAGE
                DA.DeleteP_MW_RECORD_PSAC_IMAGE(model.P_MW_RECORD_PSAC_IMAGE);


                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult SaveP_MW_SCANNED_DOCUMENTsIC(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                DA.SaveP_MW_SCANNED_DOCUMENTs(model.P_MW_SCANNED_DOCUMENTsIC);

                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult SaveP_MW_SCANNED_DOCUMENTsNIC(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                DA.SaveP_MW_SCANNED_DOCUMENTs(model.P_MW_SCANNED_DOCUMENTsNIC);

                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public JsonResult SummarySubmit(VerificaionFormModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                model.P_MW_SUMMARY_MW_ITEM_CHECKLIST.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                model.P_MW_SUMMARY_MW_ITEM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
                serviceResult = DA.SummarySubmit(model.P_MW_REFERENCE_NO.UUID, model.P_MW_REFERENCE_NO.REFERENCE_NO, model.P_MW_DSN.DSN, model.P_MW_SUMMARY_MW_ITEM_CHECKLIST, model.V_UUID);
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };
        }

        public void GetPbpPrcListBySecondEntry(VerificaionFormModel model)
        {
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[4];
                model.AppointedRSE = model.P_MW_APPOINTED_PROFESSIONALs[5];
                model.AppointedRGE = model.P_MW_APPOINTED_PROFESSIONALs[6];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[7];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[0];
                model.AppointedRSE = model.P_MW_APPOINTED_PROFESSIONALs[1];
                model.AppointedRGE = model.P_MW_APPOINTED_PROFESSIONALs[2];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[3];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
            {
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[1];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04))
            {
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[0];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05))
            {
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[1];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[2];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[3];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[4];
                model.AppointedRSE = model.P_MW_APPOINTED_PROFESSIONALs[5];
                model.AppointedRGE = model.P_MW_APPOINTED_PROFESSIONALs[6];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[7];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[1];
                model.AppointedAPForm8 = model.P_MW_APPOINTED_PROFESSIONALs[2];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
            {
                model.AppointedOther0 = model.P_MW_APPOINTED_PROFESSIONALs[0];
                model.AppointedOther1 = model.P_MW_APPOINTED_PROFESSIONALs[1];

                if (!string.IsNullOrEmpty(model.AppointedOther0.CERTIFICATION_NO))
                {
                    if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.AP;
                        model.AppointedOther1.IDENTIFY_FLAG = ProcessingConstant.AP;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RI;
                        model.AppointedOther1.IDENTIFY_FLAG = ProcessingConstant.RI;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        model.AppointedOther1.IDENTIFY_FLAG = ProcessingConstant.RSE;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        model.AppointedOther1.IDENTIFY_FLAG = ProcessingConstant.RGE;
                    }
                }

            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[0];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[1];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
            {
                model.AppointedAP = model.P_MW_APPOINTED_PROFESSIONALs[0];
                model.AppointedRSE = model.P_MW_APPOINTED_PROFESSIONALs[1];
                model.AppointedRGE = model.P_MW_APPOINTED_PROFESSIONALs[2];
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[3];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12))
            {
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[0];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
            {
                model.AppointedOther0 = model.P_MW_APPOINTED_PROFESSIONALs[0];

                if (!string.IsNullOrEmpty(model.AppointedOther0.CERTIFICATION_NO))
                {
                    if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.AP;
                        model.AppointedAP = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RI;
                        model.AppointedAP = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        model.AppointedRSE = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        model.AppointedRGE = model.AppointedOther0;
                    }
                }
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_32))
            {
                model.AppointedPRC = model.P_MW_APPOINTED_PROFESSIONALs[0];
            }
            else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
            {
                model.AppointedOther0 = model.P_MW_APPOINTED_PROFESSIONALs[0];

                if (!string.IsNullOrEmpty(model.AppointedOther0.CERTIFICATION_NO))
                {
                    if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.AP;
                        model.AppointedAP = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RI;
                        model.AppointedAP = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        model.AppointedRSE = model.AppointedOther0;
                    }
                    else if (model.AppointedOther0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        model.AppointedRGE = model.AppointedOther0;
                    }
                    else
                    {
                        model.AppointedOther0.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        model.AppointedPRC = model.AppointedOther0;
                    }
                }
            }

        }

        public void LoadDefaultNotificationValue(P_MW_RECORD mwRecord, P_MW_FORM mwForm, P_MW_RECORD_FORM_CHECKLIST formChecklist, List<P_MW_APPOINTED_PROFESSIONAL> mwAppointedProfessionalList)
        {
            String formTypeCode = mwRecord.S_FORM_TYPE_CODE;

            if (string.IsNullOrEmpty(formChecklist.INFO_NOT))
            {
                int? numOfNotificationDay = FindNumOfNotificationDay(mwRecord, mwForm, ProcessingConstant.AP, null, mwAppointedProfessionalList);
                if (numOfNotificationDay != null)
                {
                    formChecklist.INFO_NOT = DetermineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                    // sRuleOfConLetterAndRefService.determineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                }
            }
            if (ProcessingConstant.FORM_10.Equals(formTypeCode))
            {
                if (string.IsNullOrEmpty(formChecklist.INFO_NOT_PRC_CLASS1))
                {
                    int? numOfNotificationDay = FindNumOfNotificationDay(mwRecord, mwForm, ProcessingConstant.PRC, ProcessingConstant.DB_CLASS_I, mwAppointedProfessionalList);
                    if (numOfNotificationDay != null)
                    {
                        formChecklist.INFO_NOT_PRC_CLASS1 = DetermineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                        // sRuleOfConLetterAndRefService.determineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                    }
                }
                if (string.IsNullOrEmpty(formChecklist.INFO_NOT_PRC_CLASS2))
                {
                    int? numOfNotificationDay = FindNumOfNotificationDay(mwRecord, mwForm, ProcessingConstant.PRC, ProcessingConstant.DB_CLASS_II, mwAppointedProfessionalList);
                    if (numOfNotificationDay != null)
                    {
                        formChecklist.INFO_NOT_PRC_CLASS2 = DetermineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                        // sRuleOfConLetterAndRefService.determineVerifyNotificationResult(formTypeCode, numOfNotificationDay);
                    }
                }
            }
        }

        public int? FindNumOfNotificationDay(P_MW_RECORD mwRecord, P_MW_FORM mwForm, String form10Proffessional, String form10PrcDbClass, List<P_MW_APPOINTED_PROFESSIONAL> mwAppointedProfessionalList)
        {
            int? numOfNotiDay = null;

            if (mwRecord != null)
            {
                DateTime? receivedDate = null;
                if (mwForm != null)
                    if (mwForm.RECEIVED_DATE != null)
                        receivedDate = mwForm.RECEIVED_DATE;

                if (receivedDate != null)
                {
                    String formCode = mwRecord.S_FORM_TYPE_CODE;
                    if (ProcessingConstant.FORM_01.Equals(formCode))
                    {

                        //  No. of notification days = commencement date in Part B or Part E (which ever is the earliest) - received date

                        DateTime? partBCommencementDate = null;
                        //List<P_MW_APPOINTED_PROFESSIONAL> partBAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, AP);
                        List<P_MW_APPOINTED_PROFESSIONAL> partBAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.AP);

                        if (partBAppProfList != null && partBAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL appProf = partBAppProfList[0];

                            partBCommencementDate = appProf.COMMENCED_DATE;
                        }
                        else
                        {
                            partBAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.AP);

                            if (partBAppProfList != null && partBAppProfList.Count() > 0)
                            {
                                P_MW_APPOINTED_PROFESSIONAL appProf = partBAppProfList[0];

                                partBCommencementDate = appProf.COMMENCED_DATE;

                            }
                        }

                        DateTime? partECommencementDate = null;

                        //List<P_MW_APPOINTED_PROFESSIONAL> partEAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_E, PRC);

                        List<P_MW_APPOINTED_PROFESSIONAL> partEAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_E, ProcessingConstant.PRC);

                        if (partEAppProfList != null && partEAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL appProf = partEAppProfList[0];

                            partECommencementDate = appProf.COMMENCED_DATE;

                        }

                        DateTime? commencementDate = getEarliestDate(partBCommencementDate, partECommencementDate);

                        if (commencementDate != null)
                        {
                            numOfNotiDay = DateUtil.daysOfTwo(commencementDate, receivedDate);
                        }


                    }
                    else if (ProcessingConstant.FORM_02.Equals(formCode))
                    {

                        //  No. of notification days = received date - completion date in part A or Part D (which ever is the earliest) 

                        DateTime? partACompletionDate = null;
                        ///List<P_MW_APPOINTED_PROFESSIONAL> partAAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, AP);
                        ///

                        List<P_MW_APPOINTED_PROFESSIONAL> partAAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.AP);

                        if (partAAppProfList != null && partAAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL appProf = partAAppProfList[0];

                            partACompletionDate = appProf.COMPLETION_DATE;
                        }
                        else
                        {
                            // partAAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, RI);

                            partAAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.RI);

                            if (partAAppProfList != null && partAAppProfList.Count() > 0)
                            {
                                P_MW_APPOINTED_PROFESSIONAL appProf = partAAppProfList[0];
                                if (appProf != null)
                                {
                                    partACompletionDate = appProf.COMPLETION_DATE;
                                }
                            }
                        }

                        DateTime? partDCompletionDate = null;

                        // List<P_MW_APPOINTED_PROFESSIONAL> partDAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, ProcessingConstant.PART_D, PRC);

                        List<P_MW_APPOINTED_PROFESSIONAL> partDAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_D, ProcessingConstant.PRC);

                        if (partDAppProfList != null && partDAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL appProf = partDAppProfList[0];

                            partDCompletionDate = appProf.COMPLETION_DATE;

                        }

                        DateTime? completionDate = getEarliestDate(partACompletionDate, partDCompletionDate);

                        if (completionDate != null)
                        {
                            numOfNotiDay = DateUtil.daysOfTwo(receivedDate, completionDate);
                        }


                    }
                    else if (ProcessingConstant.FORM_03.Equals(formCode))
                    {

                        //   no. of notification days = commencement date in Part B �V received date
                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, PRC);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.PRC);

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                            if (partBAppProf != null)
                            {
                                DateTime? partBCommencementDate = partBAppProf.COMMENCED_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(partBCommencementDate, receivedDate);
                            }
                        }
                    }
                    else if (ProcessingConstant.FORM_04.Equals(formCode))
                    {

                        //   no. of notification days = received date - completion date  
                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, PRC);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.PRC);


                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL appProf = resultList[0];
                            if (appProf != null)
                            {
                                DateTime? completionDate = appProf.COMPLETION_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, completionDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_05.Equals(formCode))
                    {

                        //   no. of notification days = received date-completion date in part B  

                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, PRC);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.PRC);

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                            if (partBAppProf != null)
                            {
                                DateTime? partBCompletionDate = partBAppProf.COMPLETION_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partBCompletionDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_06.Equals(formCode))
                    {

                        //   no. of notification days = received date-inspection date in Part B 

                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, AP);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.AP);

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                            if (partBAppProf != null)
                            {
                                DateTime? partBCompletionDate = partBAppProf.COMPLETION_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partBCompletionDate);
                            }
                        }
                        else
                        {
                            //resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, ProcessingConstant.RI);

                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.RI);

                            if (resultList != null && resultList.Count() > 0)
                            {
                                P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                                if (partBAppProf != null)
                                {
                                    DateTime? partBCompletionDate = partBAppProf.COMPLETION_DATE;

                                    numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partBCompletionDate);
                                }
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_07.Equals(formCode))
                    {

                        //   no. of notification days = received date- appointment date in Parts B or C 

                        DateTime? partBAppointmentDate = null;
                        //List<P_MW_APPOINTED_PROFESSIONAL> partBAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, RSE);

                        List<P_MW_APPOINTED_PROFESSIONAL> partBAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.RSE);

                        if (partBAppProfList != null && partBAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = partBAppProfList[0];
                            if (partBAppProf != null)
                            {
                                partBAppointmentDate = partBAppProf.ENDORSEMENT_DATE;
                            }
                        }

                        DateTime? partCAppointmentDate = null;
                        //List<P_MW_APPOINTED_PROFESSIONAL> partCAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_C, RGE);

                        List<P_MW_APPOINTED_PROFESSIONAL> partCAppProfList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_C, ProcessingConstant.RGE);

                        if (partCAppProfList != null && partCAppProfList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partCAppProf = partCAppProfList[0];
                            if (partCAppProf != null)
                            {
                                partCAppointmentDate = partCAppProf.ENDORSEMENT_DATE;
                            }
                        }

                        DateTime? appointmentDate = getEarliestDate(partBAppointmentDate, partCAppointmentDate);
                        if (appointmentDate != null)
                        {
                            numOfNotiDay = DateUtil.daysOfTwo(receivedDate, appointmentDate);
                        }


                    }
                    else if (ProcessingConstant.FORM_08.Equals(formCode))
                    {

                        //   no. of notification days = received date - appointment date in Part B 

                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, AP);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.AP);

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                            if (partBAppProf != null)
                            {
                                DateTime? partBAppointmentDate = partBAppProf.APPOINTMENT_DATE;
                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partBAppointmentDate);
                            }
                        }
                        else
                        {
                            //resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, RI);

                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.RI);

                            if (resultList != null && resultList.Count() > 0)
                            {
                                P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                                if (partBAppProf != null)
                                {
                                    DateTime? partBAppointmentDate = partBAppProf.APPOINTMENT_DATE;
                                    numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partBAppointmentDate);
                                }
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_09.Equals(formCode))
                    {

                        //   no. of notification days = received date- first day of nomination period in Part A

                        //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_B, OTHER);

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.OTHER);

                        if (resultList != null && resultList.Count() == 0)
                        {
                            //resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, AP);

                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.AP);
                        }

                        if (resultList != null && resultList.Count() == 0)
                        {
                            //resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, RI);

                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.RI);
                        }

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partAAppProf = resultList[0];
                            if (partAAppProf != null)
                            {
                                DateTime? partAEffectFromDate = partAAppProf.EFFECT_FROM_DATE;
                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, partAEffectFromDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_10.Equals(formCode))
                    {
                        //if the case is from Form03, then no delivery date and class I cessation date
                        //no. of notification days (PRC class II) = received date-cessation date in Part A (class II)
                        //MwRecord recordForm03 = mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_03);


                        P_MW_RECORD recordForm03 = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_03);

                        if (recordForm03 != null)
                        {
                            if (form10Proffessional.Equals(ProcessingConstant.PRC))
                            {
                                //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, PRC);

                                List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.PRC);

                                if (resultList != null && resultList.Count() > 0)
                                {
                                    P_MW_APPOINTED_PROFESSIONAL partAAppProf = resultList[0];
                                    if (form10PrcDbClass.Equals(ProcessingConstant.DB_CLASS_II))
                                    {
                                        if (partAAppProf.CLASS2_CEASE_DATE != null)
                                        {
                                            DateTime? class2FromDate = partAAppProf.CLASS2_CEASE_DATE;

                                            numOfNotiDay = DateUtil.daysOfTwo(receivedDate, class2FromDate);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //else the case is from Form01.
                            //   no. of notification days (PRC class I)  = delivery date-cessation date in Part A (class I)
                            //   no. of notification days (PRC class II) = delivery date-cessation date in Part A (class II)
                            //   no. of notification days (AP)			 = received date-delivery date  	

                            if (form10Proffessional.Equals(ProcessingConstant.PRC))
                            {
                                //List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwRecord, PART_A, PRC);

                                List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.PRC);

                                if (resultList != null && resultList.Count() > 0)
                                {
                                    P_MW_APPOINTED_PROFESSIONAL partAAppProf = resultList[0];
                                    DateTime? deliveryDate = null;
                                    if (!string.IsNullOrEmpty(mwForm.FORM10_B_DELIVERY_DATE))
                                    {
                                        deliveryDate = DateUtil.getDisplayDateToDBDate(mwForm.FORM10_B_DELIVERY_DATE);
                                    }
                                    if (form10PrcDbClass.Equals(ProcessingConstant.DB_CLASS_I))
                                    { // notification of PRC Class I
                                        if (partAAppProf.CLASS1_CEASE_DATE != null)
                                        {
                                            DateTime? class1FromDate = partAAppProf.CLASS1_CEASE_DATE;

                                            numOfNotiDay = DateUtil.daysOfTwo(deliveryDate, class1FromDate);
                                        }
                                    }
                                    else if (form10PrcDbClass.Equals(ProcessingConstant.DB_CLASS_II))
                                    { // notification of PRC Class II
                                        if (partAAppProf.CLASS2_CEASE_DATE != null)
                                        {
                                            DateTime? class2FromDate = partAAppProf.CLASS2_CEASE_DATE;

                                            numOfNotiDay = DateUtil.daysOfTwo(deliveryDate, class2FromDate);
                                        }

                                    }
                                    else
                                    {
                                        //throw new RuntimeException("Invalid Function Param: form10PrcDbClass");
                                    }
                                }
                            }
                            else if (form10Proffessional.Equals(ProcessingConstant.AP))
                            {// notification of AP
                                if (!string.IsNullOrEmpty(mwForm.FORM10_B_DELIVERY_DATE))
                                {
                                    DateTime? deliveryDate = DateUtil.getDisplayDateToDBDate(mwForm.FORM10_B_DELIVERY_DATE);

                                    numOfNotiDay = DateUtil.daysOfTwo(receivedDate, deliveryDate);
                                }
                            }
                            else
                            {
                                //throw new RuntimeException("Invalid Function Param: form10Proffessional");
                            }
                        }
                    }
                    else if (ProcessingConstant.FORM_31.Equals(formCode))
                    {

                        //   no. of notification days = received date-cessation date
                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.OTHER);

                        if (resultList != null && resultList.Count() == 0)
                        {
                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.AP);
                        }

                        if (resultList != null && resultList.Count() == 0)
                        {
                            resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_A, ProcessingConstant.RI);
                        }

                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partAAppProf = resultList[0];
                            if (partAAppProf != null)
                            {
                                DateTime? ceaseDate = partAAppProf.CLASS1_CEASE_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(receivedDate, ceaseDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_32.Equals(formCode))
                    {
                        //   notification days not applicable for Form 12
                    }
                    else if (ProcessingConstant.FORM_11.Equals(formCode))
                    {

                        //   no. of notification days = commencement date in Part B �V received date
                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.PRC);
                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];
                            if (partBAppProf != null)
                            {
                                DateTime? commencementDate = partBAppProf.COMMENCED_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(commencementDate, receivedDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_12.Equals(formCode))
                    {

                        //   no. of notification days = commencement date in Part B �V received date

                        List<P_MW_APPOINTED_PROFESSIONAL> resultList = GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(mwAppointedProfessionalList, ProcessingConstant.PART_B, ProcessingConstant.PRC);
                        if (resultList != null && resultList.Count() > 0)
                        {
                            P_MW_APPOINTED_PROFESSIONAL partBAppProf = resultList[0];

                            if (partBAppProf != null)
                            {
                                DateTime? partBCommencementDate = partBAppProf.COMMENCED_DATE;

                                numOfNotiDay = DateUtil.daysOfTwo(partBCommencementDate, receivedDate);
                            }
                        }

                    }
                    else if (ProcessingConstant.FORM_33.Equals(formCode))
                    {
                        //   notification days not applicable for Form 15
                    }
                }
            }

            return numOfNotiDay;
        }

        private List<P_MW_APPOINTED_PROFESSIONAL> GetP_MW_APPOINTED_PROFESSIONALsByPartAndFlag(List<P_MW_APPOINTED_PROFESSIONAL> list, string Part, string Flag)
        {
            return list.Where(w => w.FORM_PART == Part && w.IDENTIFY_FLAG == Flag).OrderBy(o => o.ORDERING).ToList();
        }

        public String DetermineVerifyNotificationResult(String sFormTypeCode, int? numOfNotificationDay)
        {
            String notificationResult = null;

            //SRuleOfConLetterAndRef ruleOfConLetter = findBySFormTypeCode(sFormTypeCode);
            P_S_RULE_OF_CON_LETTER_AND_REF ruleOfConLetter = DA.GetP_S_RULE_OF_CON_LETTER_AND_REFByFormCode(sFormTypeCode);

            if (ruleOfConLetter == null)
            {
                //throw new RuntimeException("The record of SRuleOfConLetterAndRef for Form '" + sFormTypeCode + "' is not found!");
            }

            if (IsNotificationValid(ruleOfConLetter, numOfNotificationDay))
            {
                notificationResult = ProcessingConstant.NOTIFICATION_VALID;
            }
            else if (IsNotificationConditional(ruleOfConLetter, numOfNotificationDay).Value)
            {
                notificationResult = ProcessingConstant.NOTIFICATION_CONDITIONAL;
            }
            else if (IsNotificationRefusal(ruleOfConLetter, numOfNotificationDay))
            {
                notificationResult = ProcessingConstant.NOTIFICATION_REFUSAL;
            }

            return notificationResult;
        }

        public bool IsNotificationRefusal(P_S_RULE_OF_CON_LETTER_AND_REF ruleOfConLetter, int? numOfNotificationDay)
        {
            String compareStr1 = ruleOfConLetter.REFUSAL_COMPARE;
            decimal? compareValue1 = ruleOfConLetter.REFUSAL_VALUE;

            if (compareStr1 != null && compareValue1 != null)
            {
                if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN_OR_EQUAL))
                {
                    return numOfNotificationDay <= compareValue1 || numOfNotificationDay < 0;
                }
                else if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN))
                {
                    return numOfNotificationDay < compareValue1 || numOfNotificationDay < 0;
                }
                else if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_LARGER_THAN))
                {
                    return numOfNotificationDay > compareValue1 || numOfNotificationDay < 0;
                }
            }
            return false;
        }

        public bool IsNotificationValid(P_S_RULE_OF_CON_LETTER_AND_REF ruleOfConLetter, int? numOfNotificationDay)
        {
            String compareStr1 = ruleOfConLetter.REFUSAL_COMPARE;
            decimal? compareValue1 = ruleOfConLetter.DAYS_OF_NOTIFICATION;

            if (compareStr1 != null && compareValue1 != null)
            {
                if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN_OR_EQUAL))
                {
                    return numOfNotificationDay <= compareValue1 && numOfNotificationDay >= 0;
                }
                else if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_LARGER_THAN_OR_EQUAL))
                {
                    return numOfNotificationDay >= compareValue1; //&& numOfNotificationDay>0;
                }
            }
            return false;
        }

        public bool? IsNotificationConditional(P_S_RULE_OF_CON_LETTER_AND_REF ruleOfConLetter, int? numOfNotificationDay)
        {
            String compareStr1 = ruleOfConLetter.CONDITIONAL_LETTER_COMPARE1;
            decimal? compareValue1 = ruleOfConLetter.CONDITIONAL_LETTER_VALUE1;
            String compareStr2 = ruleOfConLetter.CONDITIONAL_LETTER_COMPARE2;
            decimal? compareValue2 = ruleOfConLetter.CONDITIONAL_LETTER_VALUE2;

            Boolean? compare1 = null;
            Boolean? compare2 = null;

            if (compareStr1 != null && compareValue1 != null)
            {
                if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN_OR_EQUAL))
                {
                    compare1 = compareValue1 <= numOfNotificationDay;
                }
                else if (compareStr1.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN))
                {
                    compare1 = compareValue1 < numOfNotificationDay;
                }
            }


            if (compareStr2 != null && compareValue2 != null)
            {
                if (compareStr2.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN_OR_EQUAL))
                {
                    compare2 = numOfNotificationDay <= compareValue2;
                }
                else if (compareStr2.Equals(ProcessingConstant.RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN))
                {
                    compare2 = numOfNotificationDay < compareValue2;
                }
            }

            if (compare1 != null && compare2 != null)
            {
                return compare1.Value && compare2.Value;//&& numOfNotificationDay>0;
            }
            else if (compare1 == null && compare2 == null)
            {
                return false;
            }
            else if (compare1 == null)
            {
                return compare2.Value && numOfNotificationDay >= 0;
            }
            else
            {
                return compare1;//&& numOfNotificationDay>0;
            }
        }

        private DateTime? getEarliestDate(DateTime? date1, DateTime? date2)
        {
            if (date1 == null)
            {
                return date2;
            }
            else if (date2 == null)
            {
                return date1;
            }
            else if (date1 < date2)
            {
                return date1;
            }
            else
            {
                return date2;
            }
        }

        private void ValidateProfessionList(P_MW_RECORD mwRecord, P_MW_FORM mwForm, P_MW_REFERENCE_NO p_MW_REFERENCE_NO, P_MW_RECORD_FORM_CHECKLIST formChecklist)
        {
            String mwRefNo = "";

            try
            {
                mwRefNo = p_MW_REFERENCE_NO.REFERENCE_NO;

                P_MW_RECORD latestSecondEntry = mwRecord;

                //mwRecord.getMwPersonContactByOwnerId();
                P_MW_PERSON_CONTACT owner = DA.GetObjectRecordByUuid<P_MW_PERSON_CONTACT>(mwRecord.OWNER_ID);

                //mwRecord.getMwPersonContactBySignboardPerfromerId();
                P_MW_PERSON_CONTACT signboard = DA.GetObjectRecordByUuid<P_MW_PERSON_CONTACT>(mwRecord.SIGNBOARD_PERFROMER_ID);

                //List<P_MW_APPOINTED_PROFESSIONAL> professionalList = mwAppointedProfessionalService.getMwAppointedProfessionalByMwRecord(latestSecondEntry);

                List<P_MW_APPOINTED_PROFESSIONAL> professionalList = DA.GetP_MW_APPOINTED_PROFESSIONALs(latestSecondEntry.UUID);

                P_MW_APPOINTED_PROFESSIONAL ap0 = null; // AP - PART_A
                P_MW_APPOINTED_PROFESSIONAL ap1 = null; // RSE - PART_A
                P_MW_APPOINTED_PROFESSIONAL ap2 = null; // RGE - PART_A
                P_MW_APPOINTED_PROFESSIONAL ap3 = null; // PRC - PART_A
                P_MW_APPOINTED_PROFESSIONAL ap4 = null; // AP - PART_B
                P_MW_APPOINTED_PROFESSIONAL ap5 = null; // RSE - PART_C
                P_MW_APPOINTED_PROFESSIONAL ap6 = null; // RGE - PART_D
                P_MW_APPOINTED_PROFESSIONAL ap7 = null; // PRC - PART_E

                if (professionalList == null) { professionalList = new List<P_MW_APPOINTED_PROFESSIONAL>(); }

                if (professionalList.Count < 8)
                {
                    for (int i = professionalList.Count(); i < 8; i++)
                    {
                        professionalList.Add(new P_MW_APPOINTED_PROFESSIONAL());
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    P_MW_APPOINTED_PROFESSIONAL ap = professionalList[i];
                    switch (ap.ORDERING)
                    {
                        case 0:
                            ap0 = ap;
                            if (ap0 == null) { ap0 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 1:
                            ap1 = ap;
                            if (ap1 == null) { ap1 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 2:
                            ap2 = ap;
                            if (ap2 == null) { ap2 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 3:
                            ap3 = ap;
                            if (ap3 == null) { ap3 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 4:
                            ap4 = ap;
                            if (ap4 == null) { ap4 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 5:
                            ap5 = ap;
                            if (ap5 == null) { ap5 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 6:
                            ap6 = ap;
                            if (ap6 == null) { ap6 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                        case 7:
                            ap7 = ap;
                            if (ap7 == null) { ap7 = new P_MW_APPOINTED_PROFESSIONAL(); }
                            break;
                    }
                }

                if (ap0 != null && ap0.IDENTIFY_FLAG == ProcessingConstant.AP)
                {
                    formChecklist.CertType = (ProcessingConstant.AP);
                }
                else
                {
                    formChecklist.CertType = (ProcessingConstant.RI);
                }

                if (formChecklist.UUID == null)
                {
                    if (ap0.CERTIFICATION_NO != null)
                    {
                        formChecklist.CertNo = (ap0.CERTIFICATION_NO);

                        if (formChecklist.CertNo.IndexOf(formChecklist.CertType) == 0)
                        {
                            formChecklist.INFO_AP_RI = "O";//.setInfoApRi("O");
                        }
                        else
                        {
                            formChecklist.INFO_AP_RI = "N"; //.setInfoApRi("N");
                        }
                    }
                    else
                    {
                        formChecklist.INFO_AP_RI = "N";
                    }
                }


                //P_MW_RECORD finalRecord = mwRecordService.getFinalMwRecordByRefNo(this.getMwRecord().getMwReferenceNo());

                P_MW_RECORD finalRecord = DeDA.GetFinalMwRecord(mwRecord.REFERENCE_NUMBER);


                P_MW_PERSON_CONTACT finalSignboard = null;
                P_MW_PERSON_CONTACT finalOwner = null;

                P_MW_APPOINTED_PROFESSIONAL finalAp = null;
                P_MW_APPOINTED_PROFESSIONAL finalRse = null;
                P_MW_APPOINTED_PROFESSIONAL finalRge = null;
                P_MW_APPOINTED_PROFESSIONAL finalPrc = null;

                if (finalRecord != null)
                {
                    finalSignboard = DeDA.GetP_MW_PERSON_CONTACT(finalRecord.SIGNBOARD_PERFROMER_ID);// finalRecord.getMwPersonContactBySignboardPerfromerId();
                    finalOwner = DeDA.GetP_MW_PERSON_CONTACT(finalRecord.OWNER_ID);// finalRecord.getMwPersonContactByOwnerId();

                    //finalAp = mwAppointedProfessionalService.getPBPByFinalMwRecord(finalRecord, ProcessingConstant.AP, ProcessingConstant.RI);
                    //finalRse = mwAppointedProfessionalService.getPBPByFinalMwRecord(finalRecord, ProcessingConstant.RSE);
                    //finalRge = mwAppointedProfessionalService.getPBPByFinalMwRecord(finalRecord, ProcessingConstant.RGE);
                    //finalPrc = mwAppointedProfessionalService.getPBPByFinalMwRecord(finalRecord, ProcessingConstant.PRC);

                    finalAp = mwAppointedProfessionalService.FindPBPByFinalMWRecord(finalRecord, ProcessingConstant.AP, ProcessingConstant.RI);
                    finalRse = mwAppointedProfessionalService.FindPBPByFinalMWRecord(finalRecord, ProcessingConstant.RSE);
                    finalRge = mwAppointedProfessionalService.FindPBPByFinalMWRecord(finalRecord, ProcessingConstant.RGE);
                    finalPrc = mwAppointedProfessionalService.FindPBPByFinalMWRecord(finalRecord, ProcessingConstant.PRC);
                }
                long itemVersion = 1;
                if (!string.IsNullOrEmpty(mwRecord.FORM_VERSION))
                {
                    itemVersion = long.Parse(mwRecord.FORM_VERSION);
                }
                else
                {
                    itemVersion = 1;
                }
                bool isOk = true;

                // ----------------------------------------------------------------
                //
                // Form 01
                //
                // ----------------------------------------------------------------
                if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
                {
                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";




                        if (itemVersion == 1)
                        {
                            apMsg += "Particulars of appointed AP = Particulars of signed AP<font color='red'>";
                        }
                        else if (itemVersion == 2)
                        {
                            apMsg += "Particulars of appointed AP/RI = Particulars of signed AP/RI<font color='red'>";
                        }
                        isOk = true;
                        if (ap0 != null && ap4 != null)
                        {
                            if (!(ap0.CERTIFICATION_NO == ap4.CERTIFICATION_NO))
                            {
                                apMsg += "<br />Certification Nos are not the same.";
                                apMsg += "<br />Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                apMsg += "<br />Part B: " + ap4.CERTIFICATION_NO;// (ap4.CERTIFICATION_NO);
                                isOk = false;
                            }

                            if (string.IsNullOrEmpty(ap0.ENGLISH_NAME) && string.IsNullOrEmpty(ap0.CHINESE_NAME)
                                && string.IsNullOrEmpty(ap4.ENGLISH_NAME) && string.IsNullOrEmpty(ap4.CHINESE_NAME))
                            {
                                if (itemVersion == 1)
                                {
                                    apMsg += "<br />The names of appointed AP and signed AP are blank";
                                }
                                else if (itemVersion == 2)
                                {
                                    apMsg += "<br />The names of appointed AP/RI and signed AP/RI are blank";
                                }

                                isOk = false;
                            }
                            else
                            {
                                if (!string.Equals(ap0.ENGLISH_NAME, ap4.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    apMsg += "<br />English Names are not the same.";
                                    apMsg += "<br />Part A: " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                    apMsg += "<br />Part B: " + ap4.ENGLISH_NAME;// (ap4.ENGLISH_NAME);
                                    isOk = false;
                                }

                                if (!string.Equals(ap0.CHINESE_NAME, ap4.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    apMsg += "<br />Chinese Names are not the same.";
                                    apMsg += "<br />Part A: " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                    apMsg += "<br />Part B: " + ap4.CHINESE_NAME;// (ap4.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                apMsg += "<br />The appointment of Authorized Person / Registered Inspector in Part A is blank.";
                                isOk = false;
                                if (ap4 != null)
                                {
                                    apMsg += "<br />Part B: " + ap4.CERTIFICATION_NO;// (ap4.CERTIFICATION_NO);
                                    apMsg += ", " + ap4.ENGLISH_NAME;// (ap4.ENGLISH_NAME);
                                    apMsg += ", " + ap4.CHINESE_NAME;// (ap4.CHINESE_NAME);
                                }
                            }
                            if (ap4 == null)
                            {
                                apMsg += "<br />Confirmation of appointment by authorized person in Part B is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                    apMsg += ", " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        //formChecklist.AP_VALID_MSG = (apMsg);

                        formChecklist.AP_VALID_MSG = apMsg;



                        if (isOk)
                        {
                            // formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);

                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }


                    //if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    {
                        isOk = true;
                        String rseMsg = "Particulars of appointed RSE = Particulars of signed RSE<font color='red'>";
                        if (ap1 != null && ap5 != null)
                        {
                            //if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            {
                                //if (!(ap1.CERTIFICATION_NO).Equals((ap5.CERTIFICATION_NO)))
                                if (!string.Equals(ap1.CERTIFICATION_NO, ap5.CERTIFICATION_NO))
                                {
                                    rseMsg += "<br />Certification Nos are not the same.";
                                    rseMsg += "<br />Part A:" + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                    rseMsg += "<br />Part C: " + ap5.CERTIFICATION_NO;// (ap5.CERTIFICATION_NO);
                                    isOk = false;
                                }

                                //if (!(ap1.ENGLISH_NAME).EqualsIgnoreCase((ap5.ENGLISH_NAME)))
                                if (!string.Equals(ap1.ENGLISH_NAME, ap5.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />English Names are not the same.";
                                    rseMsg += "<br />Part A: " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    rseMsg += "<br />Part C: " + ap5.ENGLISH_NAME;// (ap5.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap1.CHINESE_NAME).EqualsIgnoreCase((ap5.CHINESE_NAME)))
                                if (!string.Equals(ap1.CHINESE_NAME, ap5.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />Chinese Names are not the same.";
                                    rseMsg += "<br />Part A: " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                    rseMsg += "<br />Part C: " + ap5.CHINESE_NAME;// (ap5.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap5 == null)
                            {
                                rseMsg += "<br />Confirmation of appointment by registered structural engineer in Part C is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    rseMsg += "<br />Part A: " + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                    rseMsg += ", " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    rseMsg += ", " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                }
                            }
                        }
                        rseMsg += "</font>";
                        //formChecklist.RSE_VALID_MSG = (rseMsg);

                        formChecklist.RSE_VALID_MSG = (rseMsg);

                        //if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                        if (ap1 != null && (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME)))
                        {
                            if (isOk)
                            {
                                //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            //formChecklist.RSE_DETAIL_VALID = Rmk(ProcessingConstant.NA);
                            formChecklist.RSE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    //if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    {
                        isOk = true;
                        String rgeMsg = "Particulars of appointed RGE = Particulars of signed RGE<font color='red'>";
                        if (ap2 != null && ap6 != null)
                        {
                            //if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            {
                                //if (!(ap2.CERTIFICATION_NO).Equals((ap6.CERTIFICATION_NO)))
                                if (!string.Equals(ap2.CERTIFICATION_NO, ap6.CERTIFICATION_NO))
                                {
                                    rgeMsg += "<br />Certification Nos are not the same.";
                                    rgeMsg += "<br />Part A: " + ap2.CERTIFICATION_NO;// (ap2.CERTIFICATION_NO);
                                    rgeMsg += "<br />Part D: " + ap2.CERTIFICATION_NO;// (ap6.CERTIFICATION_NO);
                                    isOk = false;
                                }

                                //if (!(ap2.ENGLISH_NAME).EqualsIgnoreCase((ap6.ENGLISH_NAME)))
                                if (!string.Equals(ap2.ENGLISH_NAME, ap6.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />English Names are not the same.";
                                    rgeMsg += "<br />Part A: " + ap2.ENGLISH_NAME;// (ap2.ENGLISH_NAME);
                                    rgeMsg += "<br />Part D: " + ap6.ENGLISH_NAME;// (ap6.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap2.CHINESE_NAME).EqualsIgnoreCase((ap6.CHINESE_NAME)))
                                if (!string.Equals(ap2.CHINESE_NAME, ap6.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />Chinese Names are not the same.";
                                    rgeMsg += "<br />Part A: " + ap2.CHINESE_NAME;// (ap2.CHINESE_NAME);
                                    rgeMsg += "<br />Part D: " + ap6.CHINESE_NAME;// (ap6.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap6 == null)
                            {
                                rgeMsg += "<br />Confirmation of appointment by registered geotechnical engineer in Part D is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    rgeMsg += "<br />Part A: " + ap2.CERTIFICATION_NO;// (ap2.CERTIFICATION_NO);
                                    rgeMsg += ", " + ap2.ENGLISH_NAME;// (ap2.ENGLISH_NAME);
                                    rgeMsg += ", " + ap2.ENGLISH_NAME;// (ap2.CHINESE_NAME);
                                }
                            }
                        }
                        rgeMsg += "</font>";
                        //formChecklist.RGE_VALID_MSG = (rgeMsg);
                        formChecklist.RGE_VALID_MSG = (rgeMsg);

                        //if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                        if (ap2 != null && (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME)))
                        {
                            if (isOk)
                            {
                                //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            //formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                            formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    //if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of appointed PRC = Particulars of signed PRC<font color='red'>";
                        if (ap3 != null && ap7 != null)
                        {
                            //if (!(ap3.CERTIFICATION_NO).Equals((ap7.CERTIFICATION_NO)))
                            if (!string.Equals(ap3.CERTIFICATION_NO, ap7.CERTIFICATION_NO))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part A: " + ap3.CERTIFICATION_NO;// (ap3.CERTIFICATION_NO);
                                prcMsg += "<br />Part E: " + ap7.CERTIFICATION_NO;// (ap7.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (StringUtil.string.IsNullOrEmpty(ap3.ENGLISH_NAME) && StringUtil.string.IsNullOrEmpty(ap3.CHINESE_NAME) && StringUtil.string.IsNullOrEmpty(ap7.ENGLISH_NAME) && StringUtil.string.IsNullOrEmpty(ap7.CHINESE_NAME))

                            if (string.IsNullOrEmpty(ap3.ENGLISH_NAME) && string.IsNullOrEmpty(ap3.CHINESE_NAME)
                                && string.IsNullOrEmpty(ap7.ENGLISH_NAME) && string.IsNullOrEmpty(ap7.CHINESE_NAME))
                            {
                                prcMsg += "<br />The names of appointed PRC and signed PRC are blank";
                                isOk = false;
                            }
                            else
                            {
                                //if (!(ap3.ENGLISH_NAME).EqualsIgnoreCase((ap7.ENGLISH_NAME)))
                                if (!string.Equals(ap3.ENGLISH_NAME, ap7.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />English Names are not the same.";
                                    prcMsg += "<br />Part A: " + ap3.ENGLISH_NAME;// (ap3.ENGLISH_NAME);
                                    prcMsg += "<br />Part E: " + ap7.ENGLISH_NAME;// (ap7.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap3.CHINESE_NAME).EqualsIgnoreCase((ap7.CHINESE_NAME)))
                                if (!string.Equals(ap3.CHINESE_NAME, ap7.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />Chinese Names are not the same.";
                                    prcMsg += "<br />Part A: " + ap3.CHINESE_NAME;// (ap3.CHINESE_NAME);
                                    prcMsg += "<br />Part E: " + ap7.CHINESE_NAME;// (ap7.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap3 == null)
                            {
                                prcMsg += "<br />The appointment of prescribed registered contractor in Part A is blank.";
                                isOk = false;
                                if (ap7 != null)
                                {
                                    prcMsg += "<br />Part E: " + ap7.CERTIFICATION_NO;// (ap7.CERTIFICATION_NO);
                                    prcMsg += ", " + ap7.ENGLISH_NAME;// (ap7.ENGLISH_NAME);
                                    prcMsg += ", " + ap7.CHINESE_NAME;// (ap7.CHINESE_NAME);
                                }
                            }
                            if (ap7 == null)
                            {
                                prcMsg += "<br />Confirmation of appointment by prescribed registered contractor in Part E is blank.";
                                isOk = false;
                                if (ap3 != null)
                                {
                                    prcMsg += "<br />Part A: " + ap3.CERTIFICATION_NO;// (ap3.CERTIFICATION_NO);
                                    prcMsg += ", " + ap3.ENGLISH_NAME;// (ap3.ENGLISH_NAME);
                                    prcMsg += ", " + ap3.CHINESE_NAME;// (ap3.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);


                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);

                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                        }
                    }


                    //if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "F";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        //if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                        //        || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                        //        || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER) || !string.IsNullOrEmpty(signboard.NAME_ENGLISH) || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))

                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER) || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase) || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (The signboard person in submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";

                        //formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }
                }


                // ----------------------------------------------------------------
                //
                // Form 02
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
                {
                    //if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";
                        isOk = true;


                        if (itemVersion == 1)
                        {
                            apMsg += "Particulars of appointed AP = Particulars of signed AP in Information Page<font color='red'>";
                        }
                        else if (itemVersion == 2)
                        {
                            apMsg += "Particulars of appointed AP/RI = Particulars of signed AP/RI in Information Page<font color='red'>";
                        }
                        if (ap0 != null && ap4 != null && finalAp != null)
                        {
                            //if (!(ap0.CERTIFICATION_NO).Equals((finalAp.CERTIFICATION_NO)))
                            if (!string.Equals(ap0.CERTIFICATION_NO, finalAp.CERTIFICATION_NO))
                            {
                                apMsg += "<br />Certification Nos are not the same.";
                                apMsg += "<br />Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                apMsg += "<br />Information Page: " + finalAp.CERTIFICATION_NO;// (finalAp.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalAp.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, finalAp.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />English Names are not the same.";
                                apMsg += "<br />Part A: " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                apMsg += "<br />Information Page: " + finalAp.ENGLISH_NAME;// (finalAp.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((finalAp.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, finalAp.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />Chinese Names are not the same.";
                                apMsg += "<br />Part A: " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                apMsg += "<br />Information Page: " + finalAp.CHINESE_NAME;// (finalAp.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                apMsg += "<br />Certificate of completion by the appointed authorized person in Part A is blank.";
                                isOk = false;
                                if (ap4 != null)
                                {
                                    apMsg += "<br />Part E: " + ap4.CERTIFICATION_NO;// (ap4.CERTIFICATION_NO);
                                    apMsg += ", " + ap4.ENGLISH_NAME;// (ap4.ENGLISH_NAME);
                                    apMsg += ", " + ap4.CHINESE_NAME;// (ap4.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        //formChecklist.AP_VALID_MSG = (apMsg);
                        formChecklist.AP_VALID_MSG = (apMsg);


                        if (isOk)
                        {
                            //formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    completionDatecommencementDateChecking(mwForm, mwRecord, formChecklist);

                    //if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    {
                        isOk = true;
                        String rseMsg = "Particulars of appointed RSE = Particulars of signed RSE in Information Page<font color='red'>";
                        if (ap1 != null && finalRse != null)
                        {
                            //if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            {
                                //if (!(ap1.CERTIFICATION_NO).Equals((finalRse.CERTIFICATION_NO)))

                                if (!string.Equals(ap1.CERTIFICATION_NO, finalRse.CERTIFICATION_NO))
                                {
                                    rseMsg += "<br />Certification Nos are not the same.";
                                    rseMsg += "<br />Part B: " + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                    rseMsg += "<br />Information: " + finalRse.CERTIFICATION_NO;// (finalRse.CERTIFICATION_NO);
                                    isOk = false;
                                }

                                //if (!(ap1.ENGLISH_NAME).EqualsIgnoreCase((finalRse.ENGLISH_NAME)))
                                if (!string.Equals(ap1.ENGLISH_NAME, finalRse.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />English Names are not the same.";
                                    rseMsg += "<br />Part B: " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    rseMsg += "<br />Information Page: " + finalRse.ENGLISH_NAME;// (finalRse.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap1.CHINESE_NAME).EqualsIgnoreCase((finalRse.CHINESE_NAME)))
                                if (!string.Equals(ap1.CHINESE_NAME, finalRse.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />Chinese Names are not the same.";
                                    rseMsg += "<br />Part B: " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                    rseMsg += "<br />Information Page: " + finalRse.CHINESE_NAME;// (finalRse.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap1 == null)
                            {
                                rseMsg += "<br />The appointment of registered structural engineer in Part B is blank.";
                                isOk = false;
                                if (finalRse != null)
                                {
                                    rseMsg += "<br />Information Page: " + finalRse.CERTIFICATION_NO;// (finalRse.CERTIFICATION_NO);
                                    rseMsg += ", " + finalRse.ENGLISH_NAME;// (finalRse.ENGLISH_NAME);
                                    rseMsg += ", " + finalRse.CHINESE_NAME;// (finalRse.CHINESE_NAME);
                                }
                            }
                            if (finalRse == null)
                            {
                                rseMsg += "<br />Confirmation of appointment by registered structural engineer in Information Page is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    rseMsg += "<br />Part A: " + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                    rseMsg += ", " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    rseMsg += ", " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                }
                            }
                        }
                        rseMsg += "</font>";
                        //formChecklist.RSE_VALID_MSG = (rseMsg);
                        formChecklist.RSE_VALID_MSG = (rseMsg);

                        //if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                        if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            //formChecklist.RSE_DETAIL_VALID = Rmk(ProcessingConstant.NA);
                            formChecklist.RSE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    //if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    {
                        isOk = true;
                        String rgeMsg = "Particulars of appointed RGE = Particulars of signed RGE in Information Page<font color='red'>";
                        if (ap2 != null && finalRge != null)
                        {
                            //if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            {
                                //if (!(ap2.CERTIFICATION_NO).Equals((finalRge.CERTIFICATION_NO)))
                                if (!string.Equals(ap2.CERTIFICATION_NO, finalRge.CERTIFICATION_NO))
                                {
                                    rgeMsg += "<br />Certification Nos are not the same.";
                                    rgeMsg += "<br />Part B: " + ap2.CERTIFICATION_NO;// (ap2.CERTIFICATION_NO);
                                    rgeMsg += "<br />Information Page: " + finalRge.CERTIFICATION_NO;// (finalRge.CERTIFICATION_NO);
                                    isOk = false;
                                }

                                //if (!(ap2.ENGLISH_NAME).EqualsIgnoreCase((finalRge.ENGLISH_NAME)))
                                if (!string.Equals(ap2.ENGLISH_NAME, finalRge.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />English Names are not the same.";
                                    rgeMsg += "<br />Part B: " + ap2.ENGLISH_NAME;
                                    rgeMsg += "<br />Information Page: " + finalRge.ENGLISH_NAME;
                                    isOk = false;
                                }
                                //if (!(ap2.CHINESE_NAME).EqualsIgnoreCase((finalRge.CHINESE_NAME)))
                                if (!string.Equals(ap2.CHINESE_NAME, finalRge.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />Chinese Names are not the same.";
                                    rgeMsg += "<br />Part B: " + ap2.CHINESE_NAME;
                                    rgeMsg += "<br />Information Page:" + finalRge.CHINESE_NAME;
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap2 == null)
                            {
                                rgeMsg += "<br />The appointment of registered geotechnical engineer in Part B is blank.";
                                isOk = false;
                                if (finalRge != null)
                                {
                                    rgeMsg += "<br />Information Page: " + finalRge.CERTIFICATION_NO;// (finalRge.CERTIFICATION_NO);
                                    rgeMsg += ", " + finalRge.ENGLISH_NAME;// (finalRge.ENGLISH_NAME);
                                    rgeMsg += ", " + finalRge.CHINESE_NAME;// (finalRge.CHINESE_NAME);
                                }
                            }
                            if (finalRge == null)
                            {
                                rgeMsg += "<br />Confirmation of appointment by registered geotechnical engineer in Information Page is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    rgeMsg += "<br />Part A: " + ap2.CERTIFICATION_NO;// ; (ap2.CERTIFICATION_NO);
                                    rgeMsg += ", " + ap2.ENGLISH_NAME;// (ap2.ENGLISH_NAME);
                                    rgeMsg += ", " + ap2.CHINESE_NAME;// (ap2.CHINESE_NAME);
                                }
                            }
                        }
                        rgeMsg += "</font>";
                        //formChecklist.RGE_VALID_MSG = (rgeMsg);
                        formChecklist.RGE_VALID_MSG = (rgeMsg);

                        //if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                        if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            //formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                            formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    //if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of appointed PRC = Particulars of signed PRC in Information Page<font color='red'>";
                        if (ap3 != null//&&ap5!=null
                                && finalPrc != null)
                        {
                            //if (!(ap3.CERTIFICATION_NO).Equals((finalPrc.CERTIFICATION_NO)))
                            if (!string.Equals(ap3.CERTIFICATION_NO, finalPrc.CERTIFICATION_NO))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part D: " + ap3.CERTIFICATION_NO;// (ap3.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + finalPrc.CERTIFICATION_NO;// (finalPrc.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (!(ap3.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))
                            if (!string.Equals(ap3.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part D: " + ap3.ENGLISH_NAME;// (ap3.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + finalPrc.ENGLISH_NAME;// (finalPrc.ENGLISH_NAME);
                                isOk = false;
                            }

                            //if (!(ap3.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))
                            if (!string.Equals(ap3.CHINESE_NAME, finalPrc.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part D: " + ap3.CHINESE_NAME;// (ap3.CHINESE_NAME);
                                prcMsg += "<br />Information Page: " + finalPrc.CHINESE_NAME;// (finalPrc.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap3 == null)
                            {
                                prcMsg += "<br />Appointed prescribed registered contractor in Part D is blank.";
                                isOk = false;
                                if (ap5 != null)
                                {
                                    prcMsg += "<br />Part F: " + ap5.CERTIFICATION_NO;// (ap5.CERTIFICATION_NO);
                                    prcMsg += ", " + ap5.ENGLISH_NAME;// (ap5.ENGLISH_NAME);
                                    prcMsg += ", " + ap5.CHINESE_NAME;// (ap5.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    //if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "H";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        //if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                        //        || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                        //        || !string.IsNullOrEmpty(signboard.NAME_CHINESE))

                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER) || !string.IsNullOrEmpty(signboard.NAME_ENGLISH) || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))

                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                   || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                   || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (Signboard person in pervious submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }

                        signboardMsg += "</font>";

                        //formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 03
                //
                // ----------------------------------------------------------------			
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
                {
                    //if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of appointed PRC = Particulars of signed PRC<font color='red'>";
                        if (ap0 != null && ap1 != null)
                        {
                            //if (!(ap0.CERTIFICATION_NO).Equals((ap1.CERTIFICATION_NO)))
                            if (!string.Equals(ap0.CERTIFICATION_NO, ap1.CERTIFICATION_NO))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br/ >Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                prcMsg += "<br/ >Part B: " + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (StringUtil.string.IsNullOrEmpty(ap0.ENGLISH_NAME)
                            //        && StringUtil.string.IsNullOrEmpty(ap0.CHINESE_NAME)
                            //        && StringUtil.string.IsNullOrEmpty(ap1.ENGLISH_NAME)
                            //        && StringUtil.string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            if (string.IsNullOrEmpty(ap0.ENGLISH_NAME) && string.IsNullOrEmpty(ap0.CHINESE_NAME)
                                && string.IsNullOrEmpty(ap1.ENGLISH_NAME) && string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            {
                                prcMsg += "<br />The names of appointed PRC and signed PRC are blank";
                                isOk = false;
                            }
                            else
                            {
                                //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((ap1.ENGLISH_NAME)))
                                if (!string.Equals(ap0.ENGLISH_NAME, ap1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />English Names are not the same.";
                                    prcMsg += "<br />Part A: " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                    prcMsg += "<br />Part B: " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((ap1.CHINESE_NAME)))
                                if (!string.Equals(ap0.CHINESE_NAME, ap1.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />Chinese Names are not the same.";
                                    prcMsg += "<br />Part A: " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                    prcMsg += "<br />Part B: " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                prcMsg += "<br />The appointment of prescribed registered contractor in Part A is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    prcMsg += "<br />Part B: " + ap1.CERTIFICATION_NO;// (ap1.CERTIFICATION_NO);
                                    prcMsg += ", " + ap1.ENGLISH_NAME;// (ap1.ENGLISH_NAME);
                                    prcMsg += ", " + ap1.CHINESE_NAME;// (ap1.CHINESE_NAME);
                                }
                            }
                            if (ap1 == null)
                            {
                                prcMsg += "<br />Confirmation of appointment by prescribed registered contractor in Part B is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    prcMsg += "<br />Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                    prcMsg += ", " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                    prcMsg += ", " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }


                    //if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "C";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        //if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                        //        || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                        //        || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER) || !string.IsNullOrEmpty(signboard.NAME_ENGLISH) || string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))
                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (The signboard person in submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";
                        //formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }

                }

                // ----------------------------------------------------------------
                //
                // Form 04
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04))
                {
                    //if (string.IsNullOrEmpty(formChecklist.APPLICANT_DETAIL_MSG))
                    if (string.IsNullOrEmpty(formChecklist.APPLICANT_DETAIL_MSG))
                    {
                        isOk = true;
                        String applicantMsg = "Particulars of applicant in Part C(if any) = Particulars of applicant in Information Page<font color='red'>";
                        //if (ap2 != null
                        //        && !string.IsNullOrEmpty(ap2.ENGLISH_NAME)
                        //        && !string.IsNullOrEmpty(ap2.CHINESE_NAME)
                        //        && finalOwner != null)
                        if (ap2 != null && !string.IsNullOrEmpty(ap2.ENGLISH_NAME) && !string.IsNullOrEmpty(ap2.CHINESE_NAME) && finalOwner != null)
                        {
                            //if (!(ap2.ENGLISH_NAME).EqualsIgnoreCase((finalOwner.NAME_ENGLISH)))
                            if (!string.Equals(ap2.ENGLISH_NAME, finalOwner.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />English Names are not the same.";
                                applicantMsg += "<br />Part A: " + ap2.ENGLISH_NAME;// (ap2.ENGLISH_NAME);
                                applicantMsg += "<br />Information Page: " + finalOwner.NAME_ENGLISH;// (finalOwner.NAME_ENGLISH);
                                isOk = false;
                            }
                            //if (!(ap2.CHINESE_NAME).EqualsIgnoreCase((finalOwner.NAME_CHINESE)))
                            if (!string.Equals(ap2.CHINESE_NAME, finalOwner.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />Chinese Names are not the same.";
                                applicantMsg += "<br />Part A: " + ap2.CHINESE_NAME;// (ap2.CHINESE_NAME);
                                applicantMsg += "<br />Information Page: " + finalOwner.NAME_CHINESE;// (finalOwner.NAME_CHINESE);
                                isOk = false;
                            }
                        }
                        applicantMsg += "</font>";
                        //formChecklist.APPLICANT_DETAIL_MSG = (applicantMsg);
                        formChecklist.APPLICANT_DETAIL_MSG = (applicantMsg);

                        if (isOk)
                        {
                            //formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    completionDatecommencementDateChecking(mwForm, mwRecord, formChecklist);

                    //if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Details of PRC in Part A = Details of PRC in Information Page<font color='red'>";
                        if (ap0 != null
                                && finalPrc != null)
                        {
                            //if (!(ap0.CERTIFICATION_NO).Equals((finalPrc.CERTIFICATION_NO)))
                            if (!string.Equals(ap0.CERTIFICATION_NO, finalPrc.CERTIFICATION_NO))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part A: " + ap0.CERTIFICATION_NO;// (ap0.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + finalPrc.CERTIFICATION_NO;// (finalPrc.CERTIFICATION_NO);
                                isOk = false;
                            }
                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part A: " + ap0.ENGLISH_NAME;// (ap0.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + finalPrc.ENGLISH_NAME;// (finalPrc.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, finalPrc.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part A: " + ap0.CHINESE_NAME;// (ap0.CHINESE_NAME);
                                prcMsg += "<br />Information Page: " + finalPrc.CHINESE_NAME;// (finalPrc.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }



                    //if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "D";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        //if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                        //        || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                        //        || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER) || !string.IsNullOrEmpty(signboard.NAME_ENGLISH) || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))
                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (Signboard person in pervious submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";
                        //formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }

                }

                // ----------------------------------------------------------------
                //
                // Form05
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05))
                {
                    completionDatecommencementDateChecking(mwForm, mwRecord, formChecklist);

                    //if (string.IsNullOrEmpty(formChecklist.getForm05LocationMsg()))
                    if (string.IsNullOrEmpty(formChecklist.FORM05_LOCATION_MSG))
                    {
                        isOk = true;
                        String locationMsg = "Location or address of minor works = Location or address of minor works in information page<font color='red'>";

                        //TBC
                        //P_MW_ADDRESS mwAddress = mwRecord.getMwAddress();
                        //P_MW_ADDRESS finalAddress = finalRecord.getMwAddress();

                        P_MW_ADDRESS mwAddress = DA.GetObjectRecordByUuid<P_MW_ADDRESS>(mwRecord.LOCATION_ADDRESS_ID);
                        P_MW_ADDRESS finalAddress = DA.GetObjectRecordByUuid<P_MW_ADDRESS>(finalRecord.LOCATION_ADDRESS_ID);

                        if (mwAddress != null && finalAddress != null)
                        {
                            //if (!(mwRecord.LOCATION_OF_MINOR_WORK).Equals((finalRecord.LOCATION_OF_MINOR_WORK))
                            //        || !(mwAddress.ENGLISH_DISPLAY).EqualsIgnoreCase((finalAddress.ENGLISH_DISPLAY))
                            //        || !(mwAddress.CHINESE_DISPLAY).EqualsIgnoreCase((finalAddress.CHINESE_DISPLAY)))

                            if (!string.Equals(mwRecord.LOCATION_OF_MINOR_WORK, finalRecord.LOCATION_OF_MINOR_WORK)
                            || !string.Equals(mwAddress.ENGLISH_DISPLAY, finalAddress.ENGLISH_DISPLAY, StringComparison.CurrentCultureIgnoreCase)
                            || !string.Equals(mwAddress.CHINESE_DISPLAY, finalAddress.CHINESE_DISPLAY, StringComparison.CurrentCultureIgnoreCase))
                            {
                                locationMsg += "<br />Location or address of minor works not equal to location or address of minor works in information page";
                                locationMsg += "<br />" + mwRecord.LOCATION_OF_MINOR_WORK + " " + (mwAddress.ENGLISH_DISPLAY);
                                locationMsg += "<br />Information Page: " + (finalRecord.LOCATION_OF_MINOR_WORK) + " " + (finalAddress.ENGLISH_DISPLAY);

                                isOk = false;
                            }
                        }
                        else
                        {
                            if (finalAddress == null)
                            {
                                //formChecklist.setForm05LocationValidRmk(ProcessingConstant.NA);
                                formChecklist.FORM05_LOCATION_VALID_RMK = (ProcessingConstant.NA);
                            }
                        }
                        locationMsg += "</font>";
                        //formChecklist.setForm05LocationMsg(locationMsg);
                        formChecklist.FORM05_LOCATION_MSG = (locationMsg);

                        if (isOk)
                        {
                            //formChecklist.setForm05LocationValid(ProcessingConstant.CHECKING_OK);
                            formChecklist.FORM05_LOCATION_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.setForm05LocationValid(ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.FORM05_LOCATION_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of Appointed PRC = Particulars of signed PRC = Particulars of original PRC<font color='red'>";
                        if (ap0 != null && ap1 != null && finalPrc != null)
                        {
                            //if (!(ap0.CERTIFICATION_NO == ap1.CERTIFICATION_NO)
                            //        || !(ap0.CERTIFICATION_NO == finalPrc.CERTIFICATION_NO))
                            if (!string.Equals(ap0.CERTIFICATION_NO, ap1.CERTIFICATION_NO)
                            || !string.Equals(ap0.CERTIFICATION_NO, finalPrc.CERTIFICATION_NO))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                prcMsg += "<br />Part B: " + (ap1.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);

                                isOk = false;
                            }
                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((ap1.ENGLISH_NAME))
                            //        || !(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))

                            if (!string.Equals(ap0.ENGLISH_NAME, ap1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase)
                            || !string.Equals(ap0.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                prcMsg += "<br />Part B: " + (ap1.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.ENGLISH_NAME);

                                isOk = false;
                            }

                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((ap1.CHINESE_NAME))
                            //        || !(ap0.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))

                            if (!string.Equals(ap0.CHINESE_NAME, ap1.CHINESE_NAME) || !string.Equals(ap0.CHINESE_NAME, finalPrc.CHINESE_NAME))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                prcMsg += "<br />Part B: " + (ap1.CHINESE_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.CHINESE_NAME);

                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                prcMsg += "<br />Particulars of the appointed prescribed registered contractor in Part A is blank.";

                                if (ap1 != null)
                                {
                                    prcMsg += "<br />Part B: " + (ap1.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap1.ENGLISH_NAME);
                                    prcMsg += ", " + (ap1.CHINESE_NAME);
                                }

                                isOk = false;
                            }
                            if (ap1 == null)
                            {
                                prcMsg += "<br />Confirmation and certification by prescribed registered contractor in Part B is blank.";

                                if (ap0 != null)
                                {
                                    prcMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap0.ENGLISH_NAME);
                                    prcMsg += ", " + (ap0.CHINESE_NAME);
                                }

                                isOk = false;
                            }
                            if (ap0 != null && ap1 != null)
                            {
                                //if (!(ap0.CERTIFICATION_NO).Equals((ap1.CERTIFICATION_NO)))
                                if (!string.Equals(ap0.CERTIFICATION_NO, ap1.CERTIFICATION_NO))
                                {
                                    prcMsg += "<br />Certification Nos are not the same.";
                                    prcMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                    prcMsg += "<br />Part B: " + (ap1.CERTIFICATION_NO);

                                    isOk = false;
                                }

                                //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((ap1.ENGLISH_NAME)))
                                if (!string.Equals(ap0.ENGLISH_NAME, ap1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />English Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                    prcMsg += "<br />Part B: " + (ap1.ENGLISH_NAME);

                                    isOk = false;
                                }

                                //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((ap1.CHINESE_NAME)))
                                if (!string.Equals(ap0.CHINESE_NAME, ap1.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />Chinese Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                    prcMsg += "<br />Part B: " + (ap1.CHINESE_NAME);

                                    isOk = false;
                                }
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }


                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "C";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                            || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                            || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))
                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (The signboard person in submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";
                        //formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }

                }

                // ----------------------------------------------------------------
                //
                // Form06
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
                {
                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        isOk = true;
                        String apMsg = "Particulars of Appointed Person = Particulars of signed Appointed Person<font color='red'>";
                        if (ap0 != null && ap2 != null)
                        {
                            if (!(ap0.CERTIFICATION_NO).Equals((ap2.CERTIFICATION_NO)))
                            {
                                apMsg += "<br />Certification Nos are not the same.";
                                apMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                apMsg += "<br />Part B: " + (ap2.CERTIFICATION_NO);
                                isOk = false;
                            }
                            if (string.IsNullOrEmpty(ap0.ENGLISH_NAME)
                                    && string.IsNullOrEmpty(ap0.CHINESE_NAME)
                                    && string.IsNullOrEmpty(ap2.ENGLISH_NAME)
                                    && string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            {
                                apMsg += "<br />The Appointed Person and signed Appointed Person are blank.";
                                isOk = false;
                            }
                            else
                            {
                                //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((ap2.ENGLISH_NAME)))
                                if (!string.Equals(ap0.ENGLISH_NAME, ap2.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    apMsg += "<br />English Names are not the same.";
                                    apMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                    apMsg += "<br />Part B: " + (ap2.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((ap2.CHINESE_NAME)))
                                if (!string.Equals(ap0.CHINESE_NAME, ap2.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    apMsg += "<br />Chinese Names are not the same.";
                                    apMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                    apMsg += "<br />Part B: " + (ap2.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                apMsg += "<br />Particulars of the appointed person in Part A is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    apMsg += "<br />Part B: " + (ap2.CERTIFICATION_NO);
                                    apMsg += ", " + (ap2.ENGLISH_NAME);
                                    apMsg += ", " + (ap2.CHINESE_NAME);
                                }
                            }
                            if (ap2 == null)
                            {
                                apMsg += "<br />Confirmation of appointment and certification by appointed person in Part B is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        //formChecklist.AP_VALID_MSG = (apMsg);
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            //formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of appointed PRC = Particulars of signed PRC<font color='red'>";

                        //TBC
                        mwForm = DA.GetP_MW_FORM(mwRecord.UUID);// new P_MW_FORM();// mwFormService.getMwFormByMwRecordandFormCode(mwRecord);



                        if (mwForm.FORM06_A_5_IDENTICAL == ProcessingConstant.FLAG_Y)
                        {
                            ap1 = ap0;
                        }

                        if (ap1 != null && ap3 != null)
                        {
                            if (!(ap1.CERTIFICATION_NO).Equals((ap3.CERTIFICATION_NO)))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part A: " + (ap1.CERTIFICATION_NO);
                                prcMsg += "<br />Part C: " + (ap3.CERTIFICATION_NO);
                                isOk = false;
                            }
                            if (string.IsNullOrEmpty(ap1.ENGLISH_NAME)
                                    && string.IsNullOrEmpty(ap1.CHINESE_NAME)
                                    && string.IsNullOrEmpty(ap3.ENGLISH_NAME)
                                    && string.IsNullOrEmpty(ap3.CHINESE_NAME))
                            {
                                prcMsg += "<br />The appointed PRC and signed PRC are blank.";
                                isOk = false;
                            }
                            else
                            {
                                //if (!(ap1.ENGLISH_NAME).EqualsIgnoreCase((ap3.ENGLISH_NAME)))
                                if (!string.Equals(ap1.ENGLISH_NAME, ap3.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />English Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap1.ENGLISH_NAME);
                                    prcMsg += "<br />Part C: " + (ap3.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap1.CHINESE_NAME).EqualsIgnoreCase((ap3.CHINESE_NAME)))
                                if (!string.Equals(ap1.CHINESE_NAME, ap3.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />Chinese Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap1.CHINESE_NAME);
                                    prcMsg += "<br />Part C: " + (ap3.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap1 == null)
                            {
                                prcMsg += "<br />Completed Class III minor works - alteration, rectification or reinforcement of prescribed building or building works in Part A is blank.";
                                isOk = false;
                                if (ap3 != null)
                                {
                                    prcMsg += "<br />Part C: " + (ap3.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap3.ENGLISH_NAME);
                                    prcMsg += ", " + (ap3.CHINESE_NAME);
                                }
                            }
                            if (ap3 == null)
                            {
                                prcMsg += "<br />Confirmation and certification by prescribed registered contractor in Part C is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    prcMsg += "Part A: " + (ap1.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap1.ENGLISH_NAME);
                                    prcMsg += ", " + (ap1.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        //formChecklist.PRC_VALID_MSG = (prcMsg);
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    //check for inspection date
                    if (string.IsNullOrEmpty(formChecklist.DATE_VALID_MSG))
                    {
                        isOk = true;
                        String dateMsg = "Inspection date in item 1 of Part B is not later than " +//the signed date item 6 in Part A; and 
                            "the signed date in Part B<font color='red'>";
                        if (ap2 != null && ap1 != null)
                        {
                            if (ap2.COMPLETION_DATE == null)
                            {
                                dateMsg += "<br />Inspection date in item 1 of Part B is blank.";
                                isOk = false;
                            }
                            else if (ap2.ENDORSEMENT_DATE == null)
                            {
                                dateMsg += "<br />The signed date in Part B is blank.";
                                isOk = false;
                            }
                            else
                            {
                                if (DateUtil.compareDate(ap2.COMPLETION_DATE.Value, ap2.ENDORSEMENT_DATE.Value) == 2)
                                {
                                    dateMsg += "<br />Inspection date in item 1 of Part B is later than the signed date in Part B.";
                                    dateMsg += "<br />Inspection date: " + (ap2.COMPLETION_DATE);
                                    dateMsg += "<br />signed date:" + (ap2.ENDORSEMENT_DATE);
                                    isOk = false;
                                }
                            }
                        }
                        dateMsg += "</font>";
                        //formChecklist.DATE_VALID_MSG = (dateMsg);
                        formChecklist.DATE_VALID_MSG = (dateMsg);

                        if (isOk)
                        {
                            //formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    //check for commencement date
                    if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    {
                        isOk = true;
                        String rseMsg = "Commencement date in item 2 of Part C is not later than the Completion date in item 2 of Part C;";
                        rseMsg += "<br />and not later than the signed date in Part C;";
                        rseMsg += "<br />and earlier than the inspection date in item 1 of Part B";
                        rseMsg += "<font color='red'>";

                        if (ap3 != null && ap2 != null)
                        {
                            if (ap3.COMMENCED_DATE == null)
                            {
                                rseMsg += "<br />Commencement date in item 2 of Part C is blank.";
                                isOk = false;
                            }
                            else if (ap3.COMPLETION_DATE == null)
                            {
                                rseMsg += "<br />Completion date in item 2 of Part C is blank.";
                                isOk = false;
                            }
                            else if (ap3.ENDORSEMENT_DATE == null)
                            {
                                rseMsg += "<br />The signed date in Part C is blank.";
                                isOk = false;
                                //	   				}else if(ap1.ENDORSEMENT_DATE==null){
                                //	   					rseMsg+="The signed date item 6 in Part A is blank.<br/>";
                                //	   					isOk=false;
                            }
                            else if (ap2.COMPLETION_DATE == null)
                            {
                                rseMsg += "<br />Inspection date in item 1 of Part B is blank.";
                                isOk = false;
                            }
                            else
                            {
                                if (DateUtil.compareDate(ap3.COMMENCED_DATE.Value, ap3.COMPLETION_DATE.Value) == 2)
                                {
                                    rseMsg += "<br />Commencement date in item 2 of Part C is later than Completion date in item 2 of Part C.";
                                    rseMsg += "<br />Commencement date in item 2 of Part C: " + (ap3.COMMENCED_DATE);
                                    rseMsg += "<br />Completion date in item 2 of Part C: " + (ap3.COMPLETION_DATE);
                                    isOk = false;
                                }
                                if (DateUtil.compareDate(ap3.COMMENCED_DATE.Value, ap3.ENDORSEMENT_DATE.Value) == 2)
                                {
                                    rseMsg += "<br />Commencement date in item 2 of Part C is later than the signed date in Part C.";
                                    rseMsg += "<br />Commencement date in item 2 of Part C: " + (ap3.COMMENCED_DATE);
                                    rseMsg += "<br />The signed date in Part C: " + (ap3.ENDORSEMENT_DATE);
                                    isOk = false;
                                }
                                if (DateUtil.compareDate(ap3.COMMENCED_DATE.Value, ap2.COMPLETION_DATE.Value) == 2)
                                {
                                    rseMsg += "<br />Commencement date in item 2 of Part C is not earlier than Inspection date in item 1 of Part B.";
                                    rseMsg += "<br />Commencement date in item 2 of Part C: " + (ap3.COMMENCED_DATE);
                                    rseMsg += "<br />Inspection date in item 1 of Part B: " + (ap2.COMPLETION_DATE);
                                    isOk = false;
                                }
                            }
                        }
                        rseMsg += "</font>";
                        //formChecklist.RSE_VALID_MSG = (rseMsg);
                        formChecklist.RSE_VALID_MSG = (rseMsg);

                        if (isOk)
                        {
                            //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    //check for completion date
                    if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    {
                        isOk = true;
                        String rgeMsg = "Completion date in item 2 of Part C " + //is not earlier than the date in item 6 of Part A; " +
                            "is not later than the signed date in Part C<font color='red'>";
                        if (ap3 != null)
                        {
                            if (ap3.COMPLETION_DATE == null)
                            {
                                rgeMsg += "<br />Completion date in item 2 of Part C is blank.";
                                isOk = false;
                            }
                            else if (ap3.ENDORSEMENT_DATE == null)
                            {
                                rgeMsg += "<br />The signed date in Part C is blank.";
                                isOk = false;
                            }
                            else
                            {
                                if (DateUtil.compareDate(ap3.COMPLETION_DATE.Value, ap3.ENDORSEMENT_DATE.Value) == 2)
                                {
                                    rgeMsg += "<br />Completion date in item 2 of Part C is later than the signed date in Part C.";
                                    rgeMsg += "<br />Completion date in item 2 of Part C: " + (ap3.COMPLETION_DATE);
                                    rgeMsg += "<br />The signed date in Part C: " + (ap3.ENDORSEMENT_DATE);
                                    isOk = false;
                                }
                            }
                        }
                        rgeMsg += "</font>";
                        //formChecklist.RGE_VALID_MSG = (rgeMsg);
                        formChecklist.RGE_VALID_MSG = (rgeMsg);

                        if (isOk)
                        {
                            //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            //formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 07
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07))
                {
                    if (string.IsNullOrEmpty(formChecklist.DATE_VALID_MSG))
                    {
                        isOk = true;
                        String dateMsg = "Effective Date in Part A<font color='red'>";
                        if (ap7 != null && ap4 != null && ap5 != null && ap6 != null)
                        {
                            if (ap7.APPOINTMENT_DATE == null)
                            {
                                dateMsg += "<br />Effective date in Part A is blank.";
                                isOk = false;
                            }

                            if (ap4.APPOINTMENT_DATE != null)
                            {
                                //if (!(ap7.APPOINTMENT_DATE).Equals((ap4.APPOINTMENT_DATE)))
                                if (!string.Equals(ap7.APPOINTMENT_DATE, ap4.APPOINTMENT_DATE))
                                {
                                    dateMsg += "<br />Effective dates are not the same.";
                                    dateMsg += "<br />Part A: " + (ap7.APPOINTMENT_DATE);
                                    dateMsg += "<br />Part B: " + (ap4.APPOINTMENT_DATE);
                                    isOk = false;
                                }
                            }

                            if (ap5.APPOINTMENT_DATE != null)
                            {
                                //if (!(ap7.APPOINTMENT_DATE).Equals((ap5.APPOINTMENT_DATE)))
                                if (!string.Equals(ap7.APPOINTMENT_DATE, ap5.APPOINTMENT_DATE))
                                {
                                    dateMsg += "<br />Effective dates are not the same.";
                                    dateMsg += "<br />Part A: " + (ap7.APPOINTMENT_DATE);
                                    dateMsg += "<br />Part C: " + (ap5.APPOINTMENT_DATE);
                                    isOk = false;
                                }
                            }

                            if (ap6.APPOINTMENT_DATE != null)
                            {
                                //if (!(ap7.APPOINTMENT_DATE).Equals((ap6.APPOINTMENT_DATE)))
                                if (!string.Equals(ap7.APPOINTMENT_DATE, ap6.APPOINTMENT_DATE))
                                {
                                    dateMsg += "<br />Effective dates are not the same.";
                                    dateMsg += "<br />Part A: " + (ap7.APPOINTMENT_DATE);
                                    dateMsg += "<br />Part D: " + (ap6.APPOINTMENT_DATE);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap7 == null)
                            {
                                dateMsg += "<br />The appointment of the new appointed person in Part A is blank.";
                                isOk = false;
                                if (ap4 != null)
                                {
                                    dateMsg += "<br />Part B: " + (ap4.APPOINTMENT_DATE);
                                }
                                if (ap5 != null)
                                {
                                    dateMsg += "<br />Part C: " + (ap5.APPOINTMENT_DATE);
                                }
                                if (ap6 != null)
                                {
                                    dateMsg += "<br />Part D: " + (ap6.APPOINTMENT_DATE);
                                }
                            }

                            if (ap4 == null)
                            {
                                dateMsg += "<br />Confirmation of appointment by registered structural engineer in Part B is blank.";
                                isOk = false;
                                if (ap7 != null)
                                {
                                    dateMsg += "<br />Part A: " + (ap3.APPOINTMENT_DATE);
                                }
                                if (ap5 != null)
                                {
                                    dateMsg += "<br />Part C: " + (ap5.APPOINTMENT_DATE);
                                }
                                if (ap6 != null)
                                {
                                    dateMsg += "<br />Part D: " + (ap6.APPOINTMENT_DATE);
                                }
                            }

                            if (ap5 == null)
                            {
                                dateMsg += "<br />Confirmation of appointment by registered geotechnical engineer in Part C is blank.";
                                isOk = false;
                                if (ap7 != null)
                                {
                                    dateMsg += "<br />Part A: " + (ap3.APPOINTMENT_DATE);
                                }
                                if (ap4 != null)
                                {
                                    dateMsg += "<br />Part B: " + (ap4.APPOINTMENT_DATE);
                                }
                                if (ap6 != null)
                                {
                                    dateMsg += "<br />Part D: " + (ap6.APPOINTMENT_DATE);
                                }
                            }

                            if (ap6 == null)
                            {
                                dateMsg += "<br />Confirmation of appointment by prescribed registered contractor in Part D is blank.";
                                isOk = false;
                                if (ap7 != null)
                                {
                                    dateMsg += "<br />Part A: " + (ap3.APPOINTMENT_DATE);
                                }
                                if (ap4 != null)
                                {
                                    dateMsg += "<br />Part B: " + (ap4.APPOINTMENT_DATE);
                                }
                                if (ap5 != null)
                                {
                                    dateMsg += "<br />Part C: " + (ap5.APPOINTMENT_DATE);
                                }
                            }
                        }
                        dateMsg += "</font>";
                        formChecklist.DATE_VALID_MSG = (dateMsg);

                        if (isOk)
                        {
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    {
                        isOk = true;
                        String rseMsg = "Details of RSE in Part A (if any) = details of RSE in Part B<font color='red'>";
                        if (ap1 != null && ap4 != null)
                        {
                            if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            {
                                //if (!(ap1.CERTIFICATION_NO).Equals((ap4.CERTIFICATION_NO)))
                                if (!string.Equals(ap1.CERTIFICATION_NO, ap4.CERTIFICATION_NO))
                                {
                                    rseMsg += "<br />Certification Nos are not the same.";
                                    rseMsg += "<br />Part A: " + (ap1.CERTIFICATION_NO);
                                    rseMsg += "<br />Part B: " + (ap4.CERTIFICATION_NO);
                                    isOk = false;
                                }
                                //if (!(ap1.ENGLISH_NAME).EqualsIgnoreCase((ap4.ENGLISH_NAME)))
                                if (!string.Equals(ap1.ENGLISH_NAME, ap4.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />English Names are not the same.";
                                    rseMsg += "<br />Part A: " + (ap1.ENGLISH_NAME);
                                    rseMsg += "<br />Part B: " + (ap4.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap1.CHINESE_NAME).EqualsIgnoreCase((ap4.CHINESE_NAME)))
                                if (!string.Equals(ap1.CHINESE_NAME, ap4.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />Chinese Names are not the same.";
                                    rseMsg += "<br />Part A: " + (ap1.CHINESE_NAME);
                                    rseMsg += "<br />Part B: " + (ap4.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap4 == null)
                            {
                                rseMsg += "<br />Confirmation of appointment by registered structural engineer in Part B is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    rseMsg += "<br />Part A: " + (ap1.CERTIFICATION_NO);
                                    rseMsg += ", " + (ap1.ENGLISH_NAME);
                                    rseMsg += ", " + (ap1.CHINESE_NAME);
                                }
                            }
                        }
                        rseMsg += "</font>";
                        formChecklist.RSE_VALID_MSG = (rseMsg);

                        if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            formChecklist.RSE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    {
                        isOk = true;
                        String rgeMsg = "Details of RGE in Part A (if any) = details of RGE in Part C<font color='red'>";
                        if (ap2 != null && ap5 != null)
                        {
                            if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            {
                                //if (!(ap2.CERTIFICATION_NO).Equals((ap5.CERTIFICATION_NO)))
                                if (!string.Equals(ap2.CERTIFICATION_NO, ap5.CERTIFICATION_NO))
                                {
                                    rgeMsg += "<br />Certification Nos are not the same.";
                                    rgeMsg += "<br />Part A: " + (ap2.CERTIFICATION_NO);
                                    rgeMsg += "<br />Part C: " + (ap5.CERTIFICATION_NO);
                                    isOk = false;
                                }
                                //if (!(ap2.ENGLISH_NAME).EqualsIgnoreCase((ap5.ENGLISH_NAME)))
                                if (!string.Equals(ap2.ENGLISH_NAME, ap5.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />English Names are not the same.";
                                    rgeMsg += "<br />Part A: " + (ap2.ENGLISH_NAME);
                                    rgeMsg += "<br />Part C: " + (ap5.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap2.CHINESE_NAME).EqualsIgnoreCase((ap5.CHINESE_NAME)))
                                if (!string.Equals(ap2.CHINESE_NAME, ap5.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />Chinese Names are not the same.";
                                    rgeMsg += "<br />Part A: " + (ap2.CHINESE_NAME);
                                    rgeMsg += "<br />Part C: " + (ap5.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap2 == null)
                            {
                                rgeMsg += "<br />The appointment of registered geotechnical engineer in Part A is blank.";
                                isOk = false;
                                if (ap5 != null)
                                {
                                    rgeMsg += "<br />Part C: " + (ap5.CERTIFICATION_NO);
                                    rgeMsg += ", " + (ap5.ENGLISH_NAME);
                                    rgeMsg += ", " + (ap5.CHINESE_NAME);
                                }
                            }
                            else if (ap5 == null)
                            {
                                rgeMsg += "<br />Confirmation of appointment by registered geotechnical engineer in Part C is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    rgeMsg += "<br />Part A: " + (ap2.CERTIFICATION_NO);
                                    rgeMsg += ", " + (ap2.ENGLISH_NAME);
                                    rgeMsg += ", " + (ap2.CHINESE_NAME);
                                }
                            }
                        }
                        rgeMsg += "</font>";
                        formChecklist.RGE_VALID_MSG = (rgeMsg);

                        if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Details of PRC in Part A (if any) = details of PRC in Part D<font color='red'>";
                        if (ap3 != null && ap6 != null)
                        {
                            if (!string.IsNullOrEmpty(ap3.ENGLISH_NAME) || !string.IsNullOrEmpty(ap3.CHINESE_NAME))
                            {
                                //if (!(ap3.CERTIFICATION_NO).Equals((ap6.CERTIFICATION_NO)))
                                if (!string.Equals(ap3.CERTIFICATION_NO, ap6.CERTIFICATION_NO))
                                {
                                    prcMsg += "<br />Certification Nos are not the same.";
                                    prcMsg += "<br />Part A: " + (ap3.CERTIFICATION_NO);
                                    prcMsg += "<br />Part D: " + (ap6.CERTIFICATION_NO);
                                    isOk = false;
                                }
                                //if (!(ap3.ENGLISH_NAME).EqualsIgnoreCase((ap6.ENGLISH_NAME)))
                                if (!string.Equals(ap3.ENGLISH_NAME, ap6.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />English Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap3.ENGLISH_NAME);
                                    prcMsg += "<br />Part D: " + (ap6.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap3.CHINESE_NAME).EqualsIgnoreCase((ap6.CHINESE_NAME)))
                                if (!string.Equals(ap3.CHINESE_NAME, ap6.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    prcMsg += "<br />Chinese Names are not the same.";
                                    prcMsg += "<br />Part A: " + (ap3.CHINESE_NAME);
                                    prcMsg += "<br />Part D: " + (ap6.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap3 == null)
                            {
                                prcMsg += "<br />The appointment of prescribed registered contractor in Part A is blank.";
                                isOk = false;
                                if (ap6 != null)
                                {
                                    prcMsg += "<br />Part D: " + (ap6.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap6.ENGLISH_NAME);
                                    prcMsg += ", " + (ap6.CHINESE_NAME);
                                }
                            }
                            else if (ap6 == null)
                            {
                                prcMsg += "<br />Confirmation of appointment by prescribed registered contractor in Part D is blank.";
                                isOk = false;
                                if (ap3 != null)
                                {
                                    prcMsg += "<br />Part A: " + (ap3.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap3.ENGLISH_NAME);
                                    prcMsg += ", " + (ap3.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (!string.IsNullOrEmpty(ap3.ENGLISH_NAME) || !string.IsNullOrEmpty(ap3.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            formChecklist.PRC_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    String englishName = "";
                    String chineseName = "";
                    if (ap0 != null)
                    {
                        englishName = (ap0.ENGLISH_NAME);
                        chineseName = (ap0.CHINESE_NAME);
                    }

                    //TBC
                    P_MW_RECORD formOneSec = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_01);
                    //  mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_01);

                    if (formOneSec != null)
                    {
                        //TBC
                        owner = new P_MW_PERSON_CONTACT();// formOneSec.getMwPersonContactByOwnerId();
                        //if (owner != null
                        //        && (englishName.EqualsIgnoreCase((owner.NAME_ENGLISH))
                        //        && chineseName.EqualsIgnoreCase((owner.NAME_CHINESE))))

                        if (owner != null
                        && string.Equals(englishName, owner.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                        && string.Equals(chineseName, owner.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                        {
                            formChecklist.PRC_AS_OTHER = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.PRC_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                    else
                    {
                        //TBC
                        P_MW_RECORD formThreeSec = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_03);
                        // mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_03);

                        if (formThreeSec != null)
                        {
                            //TBC
                            owner = DA.GetObjectRecordByUuid<P_MW_PERSON_CONTACT>(formThreeSec.OWNER_ID);
                            // formThreeSec.getMwPersonContactByOwnerId();

                            //if (owner != null
                            //        && (englishName.EqualsIgnoreCase((owner.NAME_ENGLISH))
                            //        && chineseName.EqualsIgnoreCase((owner.NAME_CHINESE))))
                            if (owner != null
                            && string.Equals(englishName, owner.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(chineseName, owner.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                            {
                                formChecklist.PRC_AS_OTHER = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.PRC_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";
                        if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                        {
                            apMsg += "Particulars of AP in Part E (if any) = Particulars of AP in Information Page<font color='red'>";
                        }
                        else
                        {
                            apMsg += "Particulars of RI in Part E (if any) = Particulars of RI in Information Page<font color='red'>";
                        }
                        isOk = true;
                        if (ap7 != null
                                && !string.IsNullOrEmpty(ap7.ENGLISH_NAME)
                                && !string.IsNullOrEmpty(ap7.ENGLISH_NAME)
                                && finalAp != null)
                        {
                            if (!(ap7.CERTIFICATION_NO).Equals((finalAp.CERTIFICATION_NO)))
                            {
                                apMsg += "<br />Certification Nos are not the same.";
                                apMsg += "<br />Part A: " + (ap7.CERTIFICATION_NO);
                                apMsg += "<br />Information Page: " + (finalAp.CERTIFICATION_NO);
                                isOk = false;
                            }
                            //if (!(ap7.ENGLISH_NAME).EqualsIgnoreCase((finalAp.ENGLISH_NAME)))
                            if (!string.Equals(ap7.ENGLISH_NAME, finalAp.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />English Names are not the same.";
                                apMsg += "<br />Part A: " + (ap7.ENGLISH_NAME);
                                apMsg += "<br />Information Page: " + (finalAp.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(ap7.CHINESE_NAME).EqualsIgnoreCase((finalAp.CHINESE_NAME)))
                            if (!string.Equals(ap7.CHINESE_NAME, finalAp.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />Chinese Names are not the same.";
                                apMsg += "<br />Part A: " + (ap7.CHINESE_NAME);
                                apMsg += "<br />Information Page:" + (finalAp.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap7 == null)
                            {
                                apMsg += "<br />The appointment of Authorized Person in Part E is blank.";
                                isOk = false;
                                if (finalAp != null)
                                {
                                    apMsg += "<br />Information Page: " + (finalAp.CERTIFICATION_NO);
                                    apMsg += ", " + (finalAp.ENGLISH_NAME);
                                    apMsg += ", " + (finalAp.CHINESE_NAME);
                                }
                            }
                            if (finalAp == null)
                            {
                                apMsg += "<br />authorized person in Information Page is blank.";
                                isOk = false;
                                if (ap7 != null)
                                {
                                    apMsg += "<br />Part E: " + (ap7.CERTIFICATION_NO);
                                    apMsg += ", " + (ap7.ENGLISH_NAME);
                                    apMsg += ", " + (ap7.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 08
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08))
                {
                    if (string.IsNullOrEmpty(formChecklist.DATE_VALID_MSG))
                    {
                        isOk = true;
                        String dateMsg = "Effective Date in Part A<font color='red'>";
                        if (ap1 != null//&&ap2!=null
                        )
                        {
                            if (ap1.APPOINTMENT_DATE == null //&&ap2.APPOINTMENT_DATE==null 
                            )
                            {
                                dateMsg += "<br />Effective dates are blank.";
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap1 == null)
                            {
                                dateMsg += "<br />The appointment of the new appointed authorized person in Part A is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    dateMsg += "<br />Part B: " + (ap2.APPOINTMENT_DATE);
                                }
                            }
                            else if (ap2 == null)
                            {
                                dateMsg += "<br />Confirmation of appointment by authorized person in Part B is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    dateMsg += "<br />Part A: " + (ap1.APPOINTMENT_DATE);
                                }
                            }
                        }
                        dateMsg += "</font>";
                        formChecklist.DATE_VALID_MSG = (dateMsg);

                        if (isOk)
                        {
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.DATE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        isOk = true;
                        String apMsg = "Details of applicant in Part A = details of applicant in Information page<font color='red'>";
                        if (finalOwner != null && ap0 != null)
                        {
                            //if (!(finalOwner.NAME_ENGLISH).EqualsIgnoreCase((ap0.ENGLISH_NAME)))
                            if (!string.Equals(finalOwner.NAME_ENGLISH, ap0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />English Names are not the same.";
                                apMsg += "<br />Information page: " + (finalOwner.NAME_ENGLISH);
                                apMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(finalOwner.NAME_CHINESE).EqualsIgnoreCase((ap0.CHINESE_NAME)))
                            if (!string.Equals(finalOwner.NAME_CHINESE, ap0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />Chinese Names are not the same.";
                                apMsg += "<br />Information page: " + (finalOwner.NAME_CHINESE);
                                apMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (finalOwner == null)
                            {
                                apMsg += "<br />Particulars of applicant in Information page is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part B: ";//+(ap0.CERTIFICATION_NO)+","
                                    apMsg += (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                            if (ap0 == null)
                            {
                                apMsg += "<br />applicant in Part A is blank.";
                                isOk = false;
                                if (finalOwner != null)
                                {
                                    apMsg += "<br />Part A: ";//+(finalOwner.CERTIFICATION_NO)+","
                                    apMsg += (finalOwner.NAME_ENGLISH);
                                    apMsg += ", " + (finalOwner.NAME_CHINESE);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 09
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Details of the nominator in Part A is not equal to details of the nominee in Part B<font color='red'>";
                        if (ap0 != null && ap1 != null)
                        {
                            if ((ap0.CERTIFICATION_NO).Equals((ap1.CERTIFICATION_NO)))
                            {
                                prcMsg += "<br />Certification Nos are the same.";
                                prcMsg += "<br />Part A:" + (ap0.CERTIFICATION_NO);
                                prcMsg += "<br />Part B: " + (ap1.CERTIFICATION_NO);
                                isOk = false;
                            }
                            //if ((ap0.ENGLISH_NAME).EqualsIgnoreCase((ap1.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, ap1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are the same.";
                                prcMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                prcMsg += "<br />Part B: " + (ap1.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if ((ap0.CHINESE_NAME).EqualsIgnoreCase((ap1.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, ap1.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are the same.";
                                prcMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                prcMsg += "<br />Part B: " + (ap1.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                prcMsg += "<br />Nominee in Part A is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    prcMsg += "<br />Part B: " + (ap1.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap1.ENGLISH_NAME);
                                    prcMsg += ", " + (ap1.CHINESE_NAME);
                                }
                            }
                            if (ap1 == null)
                            {
                                prcMsg += "<br />Nominee in Part B is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    prcMsg += "<br />Part A:" + (ap0.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap0.ENGLISH_NAME);
                                    prcMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";
                        isOk = true;
                        if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                        {
                            apMsg += "Details of the nominator in Part A = details of AP/RSE/RGE in Information Page<font color='red'>";
                        }
                        else
                        {
                            apMsg += "Details of the nominator in Part A = details of RI/RSE/RGE in Information Page<font color='red'>";
                        }
                        P_MW_APPOINTED_PROFESSIONAL finalPerson = null;
                        if (ap0 != null)
                        {
                            if (!string.IsNullOrEmpty(ap0.CERTIFICATION_NO))
                            {
                                if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP) || ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                                {
                                    finalPerson = finalAp;
                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                                {
                                    finalPerson = finalRse;
                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                                {
                                    finalPerson = finalRge;
                                }
                                if (finalPerson != null)
                                {
                                    if (!(finalPerson.CERTIFICATION_NO).Equals((ap0.CERTIFICATION_NO)))
                                    {
                                        apMsg += "<br />Certification Nos are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                        apMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.ENGLISH_NAME).EqualsIgnoreCase((ap0.ENGLISH_NAME)))
                                    if (!string.Equals(finalPerson.ENGLISH_NAME, ap0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />English Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.ENGLISH_NAME);
                                        apMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.CHINESE_NAME).EqualsIgnoreCase((ap0.CHINESE_NAME)))
                                    if (!string.Equals(finalPerson.CHINESE_NAME, ap0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />Chinese Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CHINESE_NAME);
                                        apMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                        isOk = false;
                                    }
                                }
                            }
                            else
                            {
                                apMsg += "<br />Certificate No. of the nominator is blank.";
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (finalPerson == null)
                            {
                                if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                                {
                                    apMsg += "<br />details of AP/RSE/RGE is blank.";
                                }
                                else
                                {
                                    apMsg += "<br />details of RI/RSE/RGE is blank.";
                                }
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                            if (ap0 == null)
                            {
                                apMsg += "<br />Nominator in Part A is blank.";
                                isOk = false;
                                if (finalPerson != null)
                                {
                                    apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                    apMsg += ", " + (finalPerson.ENGLISH_NAME);
                                    apMsg += ", " + (finalPerson.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);
                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 10
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
                {
                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Particulars of appointed PRC = Particulars of signed PRC<font color='red'>";
                        if (ap0 != null && finalPrc != null)
                        {
                            if (!(ap0.CERTIFICATION_NO).Equals((finalPrc.CERTIFICATION_NO)))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                isOk = false;
                            }
                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, finalPrc.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part A: " + (ap0.CHINESE_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                prcMsg += "<br />PRC in Part A is blank.";
                                isOk = false;
                                if (finalPrc != null)
                                {
                                    prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                    prcMsg += ", " + (finalPrc.ENGLISH_NAME);
                                    prcMsg += ", " + (finalPrc.CHINESE_NAME);
                                }
                            }
                            if (finalPrc == null)
                            {
                                prcMsg += "<br />PRC in Information Page is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    prcMsg += "<br />Part A: " + (ap0.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap0.ENGLISH_NAME);
                                    prcMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //	
                // Form 11
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
                {
                    if (string.IsNullOrEmpty(formChecklist.APPLICANT_DETAIL_MSG))
                    {
                        isOk = true;
                        String applicantMsg = "Particulars of applicant in Part A = Particulars of applicant in Information Page<font color='red'>";
                        if (owner != null && finalOwner != null)
                        {
                            //if (!(owner.NAME_ENGLISH).EqualsIgnoreCase((finalOwner.NAME_ENGLISH)))
                            if (!string.Equals(owner.NAME_ENGLISH, finalOwner.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />English Names are not the same.";
                                applicantMsg += "<br />Part A: " + (owner.NAME_ENGLISH);
                                applicantMsg += "<br />Information Page: " + (finalOwner.NAME_ENGLISH);
                                isOk = false;
                            }
                            //if (!(owner.NAME_CHINESE).EqualsIgnoreCase((finalOwner.NAME_CHINESE)))
                            if (!string.Equals(owner.NAME_CHINESE, finalOwner.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />Chinese Names are not the same.";
                                applicantMsg += "<br />Part A: " + (owner.NAME_CHINESE);
                                applicantMsg += "<br />Information Page: " + (finalOwner.NAME_CHINESE);
                                isOk = false;
                            }
                        }
                        applicantMsg += "</font>";
                        formChecklist.APPLICANT_DETAIL_MSG = (applicantMsg);

                        if (isOk)
                        {
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";
                        isOk = true;
                        if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                        {
                            apMsg += "Details of AP in Part B = details of AP in Information Page<font color='red'>";
                        }
                        else
                        {
                            apMsg += "Details of RI in Part B = details of RI in Information Page<font color='red'>";
                        }
                        if (ap0 != null && finalAp != null)
                        {
                            if (!(ap0.CERTIFICATION_NO).Equals((finalAp.CERTIFICATION_NO)))
                            {
                                apMsg += "<br />Certification Nos are not the same.";
                                apMsg += "<br />Part B: " + (ap0.CERTIFICATION_NO);
                                apMsg += "<br />Information Page:" + (finalAp.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalAp.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, finalAp.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />English Names are not the same.";
                                apMsg += "<br />Part B:" + (ap0.ENGLISH_NAME);
                                apMsg += "<br />Information Page: " + (finalAp.ENGLISH_NAME);
                                isOk = false;
                            }

                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((finalAp.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, finalAp.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                apMsg += "<br />Chinese Names are not the same.";
                                apMsg += "<br />Part B: " + (ap0.CHINESE_NAME);
                                apMsg += "<br />Information Page: " + (finalAp.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                apMsg += "<br />Certificate of completion by the appointed authorized person in Part B is blank.";
                                isOk = false;
                                if (finalAp != null)
                                {
                                    apMsg += "<br />Information Page: " + (finalAp.CERTIFICATION_NO);
                                    apMsg += ", " + (finalAp.ENGLISH_NAME);
                                    apMsg += ", " + (finalAp.CHINESE_NAME);
                                }
                            }
                            if (finalAp == null)
                            {
                                apMsg += "Appointed authorized person in Information Page is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part B: " + (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.RSE_VALID_MSG))
                    {
                        isOk = true;
                        String rseMsg = "Details of RSE in Part C (if any) = details of RSE in Information Page<font color='red'>";
                        if (ap1 != null && finalRse != null)
                        {
                            if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                            {
                                if (!(ap1.CERTIFICATION_NO).Equals((finalRse.CERTIFICATION_NO)))
                                {
                                    rseMsg += "<br />Certification Nos are not the same.";
                                    rseMsg += "<br />Part C: " + (ap1.CERTIFICATION_NO);
                                    rseMsg += "<br />Information: " + (finalRse.CERTIFICATION_NO);
                                    isOk = false;
                                }

                                //if (!(ap1.ENGLISH_NAME).EqualsIgnoreCase((finalRse.ENGLISH_NAME)))
                                if (!string.Equals(ap1.ENGLISH_NAME, finalRse.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />English Names are not the same.";
                                    rseMsg += "<br />Part C: " + (ap1.ENGLISH_NAME);
                                    rseMsg += "<br />Information Page: " + (finalRse.ENGLISH_NAME);
                                    isOk = false;
                                }

                                //if (!(ap1.CHINESE_NAME).EqualsIgnoreCase((finalRse.CHINESE_NAME)))
                                if (!string.Equals(ap1.CHINESE_NAME, finalRse.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rseMsg += "<br />Chinese Names are not the same.";
                                    rseMsg += "<br />Part C: " + (ap1.CHINESE_NAME);
                                    rseMsg += "<br />Information Page:" + (finalRse.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap1 == null)
                            {
                                rseMsg += "<br />The appointment of registered structural engineer in Part C is blank.";
                                isOk = false;
                                if (finalRse != null)
                                {
                                    rseMsg += "<br />Information Page: " + (finalRse.CERTIFICATION_NO);
                                    rseMsg += ", " + (finalRse.ENGLISH_NAME);
                                    rseMsg += ", " + (finalRse.CHINESE_NAME);
                                }
                            }
                            if (finalRse == null)
                            {
                                rseMsg += "<br />Confirmation of appointment by registered structural engineer in Information Page is blank.";
                                isOk = false;
                                if (ap1 != null)
                                {
                                    rseMsg += "<br />Part C: " + (ap1.CERTIFICATION_NO);
                                    rseMsg += ", " + (ap1.ENGLISH_NAME);
                                    rseMsg += ", " + (ap1.CHINESE_NAME);
                                }
                            }
                        }
                        rseMsg += "</font>";
                        formChecklist.RSE_VALID_MSG = (rseMsg);

                        if (!string.IsNullOrEmpty(ap1.ENGLISH_NAME) || !string.IsNullOrEmpty(ap1.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.RSE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            formChecklist.RSE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.RGE_VALID_MSG))
                    {
                        isOk = true;
                        String rgeMsg = "Details of RGE in Part D (if any) = details of RGE in Information Page<font color='red'>";
                        if (ap2 != null && finalRge != null)
                        {
                            if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                            {
                                if (!(ap2.CERTIFICATION_NO).Equals((finalRge.CERTIFICATION_NO)))
                                {
                                    rgeMsg += "<br />Certification Nos are not the same.";
                                    rgeMsg += "<br />Part D: " + (ap2.CERTIFICATION_NO);
                                    rgeMsg += "<br />Information Page: " + (finalRge.CERTIFICATION_NO);
                                    isOk = false;
                                }
                                //if (!(ap2.ENGLISH_NAME).EqualsIgnoreCase((finalRge.ENGLISH_NAME)))
                                if (!string.Equals(ap2.ENGLISH_NAME, finalRge.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />English Names are not the same.";
                                    rgeMsg += "<br />Part D: " + (ap2.ENGLISH_NAME);
                                    rgeMsg += "<br />Information Page: " + (finalRge.ENGLISH_NAME);
                                    isOk = false;
                                }
                                //if (!(ap2.CHINESE_NAME).EqualsIgnoreCase((finalRge.CHINESE_NAME)))
                                if (!string.Equals(ap2.CHINESE_NAME, finalRge.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    rgeMsg += "<br />Chinese Names are not the same.";
                                    rgeMsg += "<br />Part D: " + (ap2.CHINESE_NAME);
                                    rgeMsg += "<br />Information Page: " + (finalRge.CHINESE_NAME);
                                    isOk = false;
                                }
                            }
                        }
                        else
                        {
                            if (ap2 == null)
                            {
                                rgeMsg += "<br />The appointment of registered geotechnical engineer in Part D is blank.";
                                isOk = false;
                                if (finalRge != null)
                                {
                                    rgeMsg += "<br />Information Page: " + (finalRge.CERTIFICATION_NO);
                                    rgeMsg += ", " + (finalRge.ENGLISH_NAME);
                                    rgeMsg += ", " + (finalRge.CHINESE_NAME);
                                }
                            }
                            if (finalRge == null)
                            {
                                rgeMsg += "<br />Confirmation of appointment by registered geotechnical engineer in Information Page is blank.";
                                isOk = false;
                                if (ap2 != null)
                                {
                                    rgeMsg += "<br />Part D: " + (ap2.CERTIFICATION_NO);
                                    rgeMsg += ", " + (ap2.ENGLISH_NAME);
                                    rgeMsg += ", " + (ap2.CHINESE_NAME);
                                }
                            }
                        }
                        rgeMsg += "</font>";
                        formChecklist.RGE_VALID_MSG = (rgeMsg);

                        if (!string.IsNullOrEmpty(ap2.ENGLISH_NAME) || !string.IsNullOrEmpty(ap2.CHINESE_NAME))
                        {
                            if (isOk)
                            {
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else
                            {
                                formChecklist.RGE_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                        }
                        else
                        {
                            formChecklist.RGE_DETAIL_VALID_RMK = (ProcessingConstant.NA);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Details of PRC in Part E = details of PRC in Information Page<font color='red'>";
                        if (ap0 != null && finalPrc != null)
                        {
                            if (!(ap3.CERTIFICATION_NO).Equals((finalPrc.CERTIFICATION_NO)))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part E: " + (ap3.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                isOk = false;
                            }

                            //if (!(ap3.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))
                            if (!string.Equals(ap3.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part E: " + (ap3.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.ENGLISH_NAME);
                                isOk = false;
                            }

                            //if (!(ap3.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))
                            if (!string.Equals(ap3.CHINESE_NAME, finalPrc.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part E:" + (ap3.CHINESE_NAME);
                                prcMsg += "<br />Information Page:" + (finalPrc.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap3 == null)
                            {
                                prcMsg += "<br />PRC in Part E is blank.";
                                isOk = false;
                                if (finalPrc != null)
                                {
                                    prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                    prcMsg += ", " + (finalPrc.ENGLISH_NAME);
                                    prcMsg += ", " + (finalPrc.CHINESE_NAME);
                                }
                            }
                            if (finalPrc == null)
                            {
                                prcMsg += "<br />PRC in Information Page is blank.";
                                isOk = false;
                                if (ap3 != null)
                                {
                                    prcMsg += "<br />Part E: " + (ap3.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap3.ENGLISH_NAME);
                                    prcMsg += ", " + (ap3.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "F";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                                || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                                || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))
                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (Signboard person in pervious submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                        /*if(!string.IsNullOrEmpty(signboard.ID_NUMBER)
                                || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                                || !string.IsNullOrEmpty(signboard.NAME_CHINESE)) {
                            if(isOk) {
                                formChecklist.setSignboardDetailValid(ProcessingConstant.CHECKING_OK);
                            } else {
                                formChecklist.setSignboardDetailValid(ProcessingConstant.CHECKING_NOT_OK);
                            }
                        } else {
                            //formChecklist.setSignboardDetailValidRmk(ProcessingConstant.NA);	
                        }*/
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 12
                //
                // ----------------------------------------------------------------					
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12))
                {
                    if (string.IsNullOrEmpty(formChecklist.APPLICANT_DETAIL_MSG))
                    {
                        isOk = true;
                        String applicantMsg = "Particulars of applicant in Part A = Particulars of applicant in Information Page<font color='red'>";
                        if (owner != null && finalOwner != null)
                        {
                            //if (!(owner.NAME_ENGLISH).EqualsIgnoreCase((finalOwner.NAME_ENGLISH)))
                            if (!string.Equals(owner.NAME_ENGLISH, finalOwner.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />English Names are not the same.";
                                applicantMsg += "<br />Part A: " + (owner.NAME_ENGLISH);
                                applicantMsg += "<br />Information Page: " + (finalOwner.NAME_ENGLISH);
                                isOk = false;
                            }
                            //if (!(owner.NAME_CHINESE).EqualsIgnoreCase((finalOwner.NAME_CHINESE)))
                            if (!string.Equals(owner.NAME_CHINESE, finalOwner.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                            {
                                applicantMsg += "<br />Chinese Names are not the same.";
                                applicantMsg += "<br />Part A: " + (owner.NAME_CHINESE);
                                applicantMsg += "<br />Information Page: " + (finalOwner.NAME_CHINESE);
                                isOk = false;
                            }
                        }
                        applicantMsg += "</font>";
                        formChecklist.APPLICANT_DETAIL_MSG = (applicantMsg);

                        if (isOk)
                        {
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.APPLICANT_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.PRC_VALID_MSG))
                    {
                        isOk = true;
                        String prcMsg = "Details of PRC in Part B = details of PRC in Information Page<font color='red'>";
                        if (ap0 != null && finalPrc != null)
                        {
                            if (!(ap0.CERTIFICATION_NO).Equals((finalPrc.CERTIFICATION_NO)))
                            {
                                prcMsg += "<br />Certification Nos are not the same.";
                                prcMsg += "<br />Part B: " + (ap0.CERTIFICATION_NO);
                                prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                isOk = false;
                            }
                            //if (!(ap0.ENGLISH_NAME).EqualsIgnoreCase((finalPrc.ENGLISH_NAME)))
                            if (!string.Equals(ap0.ENGLISH_NAME, finalPrc.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />English Names are not the same.";
                                prcMsg += "<br />Part B: " + (ap0.ENGLISH_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.ENGLISH_NAME);
                                isOk = false;
                            }
                            //if (!(ap0.CHINESE_NAME).EqualsIgnoreCase((finalPrc.CHINESE_NAME)))
                            if (!string.Equals(ap0.CHINESE_NAME, finalPrc.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                            {
                                prcMsg += "<br />Chinese Names are not the same.";
                                prcMsg += "<br />Part B: " + (ap0.CHINESE_NAME);
                                prcMsg += "<br />Information Page: " + (finalPrc.CHINESE_NAME);
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (ap0 == null)
                            {
                                prcMsg += "<br />PRC in Part B is blank.";
                                isOk = false;
                                if (finalPrc != null)
                                {
                                    prcMsg += "<br />Information Page: " + (finalPrc.CERTIFICATION_NO);
                                    prcMsg += ", " + (finalPrc.ENGLISH_NAME);
                                    prcMsg += ", " + (finalPrc.CHINESE_NAME);
                                }
                            }
                            if (finalPrc == null)
                            {
                                prcMsg += "<br />PRC in Information Page is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    prcMsg += "<br />Part B: " + (ap0.CERTIFICATION_NO);
                                    prcMsg += ", " + (ap0.ENGLISH_NAME);
                                    prcMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                        }
                        prcMsg += "</font>";
                        formChecklist.PRC_VALID_MSG = (prcMsg);

                        if (isOk)
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.PRC_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    if (string.IsNullOrEmpty(formChecklist.SIGNBOARD_DETAIL_VALID_MSG))
                    {
                        String part = "C";
                        isOk = true;
                        String signboardMsg = "Details of signboard person in Part " + part + " = Details of signboard person in Information Page<font color='red'>";
                        if (!string.IsNullOrEmpty(signboard.ID_NUMBER)
                                || !string.IsNullOrEmpty(signboard.NAME_ENGLISH)
                                || !string.IsNullOrEmpty(signboard.NAME_CHINESE))
                        {
                            if (finalSignboard != null)
                            {
                                //if ((!(signboard.ID_NUMBER).Equals((finalSignboard.ID_NUMBER)))
                                //        || (!(signboard.NAME_ENGLISH).EqualsIgnoreCase((finalSignboard.NAME_ENGLISH)))
                                //        || (!(signboard.NAME_CHINESE).EqualsIgnoreCase((finalSignboard.NAME_CHINESE))))

                                if (!string.Equals(signboard.ID_NUMBER, finalSignboard.ID_NUMBER)
                                || !string.Equals(signboard.NAME_ENGLISH, finalSignboard.NAME_ENGLISH, StringComparison.CurrentCultureIgnoreCase)
                                || !string.Equals(signboard.NAME_CHINESE, finalSignboard.NAME_CHINESE, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    //signboardMsg += "<br />Details of signboard person in Part " + part + " not equal to Details of signboard person in Information Page";

                                    signboardMsg += "<br />" + "Information page: " +
                                    "ID No.:" + finalSignboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + finalSignboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin Name:" + finalSignboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (Signboard person in pervious submitted Form):" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH2 + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE2 + "&nbsp;&nbsp;&nbsp;";

                                    signboardMsg += "<br />" + "Part " + part + " (New signboard person):" +
                                    "ID No.:" + signboard.ID_NUMBER + "&nbsp;&nbsp;&nbsp;" +
                                    "Eng. Name:" + signboard.NAME_ENGLISH + "&nbsp;&nbsp;&nbsp;" +
                                    "Chin. Name:" + signboard.NAME_CHINESE + "&nbsp;&nbsp;&nbsp;";
                                    isOk = false;
                                }
                            }
                        }
                        signboardMsg += "</font>";
                        formChecklist.SIGNBOARD_DETAIL_VALID_MSG = (signboardMsg);

                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 31
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
                {
                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        isOk = true;
                        String apMsg = "Particulars of appointed PBP = Particulars of signed PBP<font color='red'>";
                        P_MW_APPOINTED_PROFESSIONAL finalPerson = null;
                        if (ap0 != null)
                        {
                            if (!string.IsNullOrEmpty(ap0.CERTIFICATION_NO))
                            {
                                if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP) || ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                                {
                                    finalPerson = finalAp;


                                    if (itemVersion == 1)
                                    {
                                        apMsg = "Particulars of appointed AP = Particulars of signed AP<font color='red'>";
                                    }
                                    else if (itemVersion == 2)
                                    {
                                        apMsg = "Particulars of appointed AP/RI = Particulars of signed AP/RI<font color='red'>";
                                    }




                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                                {
                                    finalPerson = finalRse;
                                    apMsg = "Particulars of appointed RSE = Particulars of signed RSE<font color='red'>";
                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                                {
                                    finalPerson = finalRge;
                                    apMsg = "Particulars of appointed RGE = Particulars of signed RGE<font color='red'>";
                                }
                                if (finalPerson != null)
                                {
                                    if (!(finalPerson.CERTIFICATION_NO).Equals((ap0.CERTIFICATION_NO)))
                                    {
                                        apMsg += "<br />Certification Nos are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                        apMsg += "<br />PBP: " + (ap0.CERTIFICATION_NO);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.ENGLISH_NAME).EqualsIgnoreCase((ap0.ENGLISH_NAME)))
                                    if (!string.Equals(finalPerson.ENGLISH_NAME, ap0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />English Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.ENGLISH_NAME);
                                        apMsg += "<br />PBP: " + (ap0.ENGLISH_NAME);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.CHINESE_NAME).EqualsIgnoreCase((ap0.CHINESE_NAME)))
                                    if (!string.Equals(finalPerson.CHINESE_NAME, ap0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />Chinese Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CHINESE_NAME);
                                        apMsg += "<br />PBP: " + (ap0.CHINESE_NAME);
                                        isOk = false;
                                    }
                                }
                            }
                            else
                            {
                                apMsg += "<br />Certificate No. of the PBP is blank.";
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (finalPerson == null)
                            {
                                apMsg += "<br />details of PBP in Information Page is blank.";
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />Part B: " + (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                            if (ap0 == null)
                            {
                                apMsg += "<br />PBP is blank.";
                                isOk = false;
                                if (finalPerson != null)
                                {
                                    apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                    apMsg += ", " + (finalPerson.ENGLISH_NAME);
                                    apMsg += ", " + (finalPerson.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }

                // ----------------------------------------------------------------
                //
                // Form 33
                //
                // ----------------------------------------------------------------
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    if (string.IsNullOrEmpty(formChecklist.AP_VALID_MSG))
                    {
                        String apMsg = "";
                        isOk = true;
                        if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                        {
                            apMsg = "Details of appointed person = details of AP/PRC in Information Page<font color='red'>";
                        }
                        else
                        {
                            apMsg = "Details of appointed person = details of Ri/PRC in Information Page<font color='red'>";
                        }
                        P_MW_APPOINTED_PROFESSIONAL finalPerson = null;
                        if (ap0 != null)
                        {
                            if (!string.IsNullOrEmpty(ap0.CERTIFICATION_NO))
                            {
                                if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.AP) || ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RI))
                                {
                                    finalPerson = finalAp;
                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RSE))
                                {
                                    finalPerson = finalRse;
                                }
                                else if (ap0.CERTIFICATION_NO.StartsWith(ProcessingConstant.RGE))
                                {
                                    finalPerson = finalRge;
                                }
                                else
                                {
                                    finalPerson = finalPrc;
                                }
                                if (finalPerson != null)
                                {
                                    if (!(finalPerson.CERTIFICATION_NO).Equals((ap0.CERTIFICATION_NO)))
                                    {
                                        apMsg += "<br />Certification Nos are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                        apMsg += "<br />new Appointed Person:" + (ap0.CERTIFICATION_NO);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.ENGLISH_NAME).EqualsIgnoreCase((ap0.ENGLISH_NAME)))
                                    if (!string.Equals(finalPerson.ENGLISH_NAME, ap0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />English Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.ENGLISH_NAME);
                                        apMsg += "<br />new Appointed Person: " + (ap0.ENGLISH_NAME);
                                        isOk = false;
                                    }
                                    //if (!(finalPerson.CHINESE_NAME).EqualsIgnoreCase((ap0.CHINESE_NAME)))
                                    if (!string.Equals(finalPerson.CHINESE_NAME, ap0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        apMsg += "<br />Chinese Names are not the same.";
                                        apMsg += "<br />Information Page: " + (finalPerson.CHINESE_NAME);
                                        apMsg += "<br />new Appointed Person:" + (ap0.CHINESE_NAME);
                                        isOk = false;
                                    }
                                }
                            }
                            else
                            {
                                apMsg += "<br />Certificate No. of the new Appointed Person is blank.";
                                isOk = false;
                            }
                        }
                        else
                        {
                            if (finalPerson == null)
                            {
                                if ((ap0.CERTIFICATION_NO).StartsWith(ProcessingConstant.AP))
                                {
                                    apMsg += "<br />details of AP/RSE/RGE/PRC is blank.";
                                }
                                else
                                {
                                    apMsg += "<br />details of RI/RSE/RGE/PRC is blank.";
                                }
                                isOk = false;
                                if (ap0 != null)
                                {
                                    apMsg += "<br />new Appointed Person: " + (ap0.CERTIFICATION_NO);
                                    apMsg += ", " + (ap0.ENGLISH_NAME);
                                    apMsg += ", " + (ap0.CHINESE_NAME);
                                }
                            }
                            if (ap0 == null)
                            {
                                apMsg += "<br />new Appointed Person is blank.";
                                isOk = false;
                                if (finalPerson != null)
                                {
                                    apMsg += "<br />Information Page: " + (finalPerson.CERTIFICATION_NO);
                                    apMsg += ", " + (finalPerson.ENGLISH_NAME);
                                    apMsg += ", " + (finalPerson.CHINESE_NAME);
                                }
                            }
                        }
                        apMsg += "</font>";
                        formChecklist.AP_VALID_MSG = (apMsg);

                        if (isOk)
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_OK);
                        }
                        else
                        {
                            formChecklist.AP_DETAIL_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("----------Error---------- ");
                Log.Error("VerificationAction.prefillPBPValid(): " + e.Message);
            }
        }


        private void completionDatecommencementDateChecking(P_MW_FORM mwForm, P_MW_RECORD mwRecord, P_MW_RECORD_FORM_CHECKLIST formChecklist)
        {

            bool checking = true;

            //		checking = (mwRecord.getCommecementDate()==null) ? false : true;
            try
            {
                //mwForm = mwFormService.getMwFormByMwRecordandFormCode(mwRecord);

                checking = (mwRecord.COMPLETION_DATE == null) ? false : true;

                if (checking == true)
                {
                    if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
                    {
                        //TBC
                        P_MW_RECORD form01 = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_01);
                        // mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_01);

                        P_MW_RECORD form11 = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_11);
                        // mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_11);

                        if (form01 == null && form11 == null)
                        {
                            checking = false;
                        }
                        else
                        {
                            bool cForm01 = true;
                            bool cForm11 = true;

                            if (form01 != null)
                            {
                                //cForm01 = (!mwRecord.COMPLETION_DATE.before(form01.getCommencementDate())) ? true : false;
                                cForm01 = (!(mwRecord.COMPLETION_DATE < form01.COMMENCEMENT_DATE)) ? true : false;
                            }
                            if (form11 != null)
                            {
                                //cForm11 = (!mwRecord.COMPLETION_DATE.before(form11.getCommencementDate())) ? true : false;
                                cForm11 = (!(mwRecord.COMPLETION_DATE < form11.COMMENCEMENT_DATE)) ? true : false;
                            }

                            checking = (cForm01 == false || cForm11 == false) ? false : true;
                        }
                    }
                    else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04))
                    {
                        //TBC
                        P_MW_RECORD form03 = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_03);
                        // mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_03);


                        P_MW_RECORD form12 = mwAppointedProfessionalService.GetLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.REFERENCE_NUMBER, ProcessingConstant.FORM_12);
                        // mwRecordService.getLatestSecondEntryMwRecordByRefNoAndFormCode(mwRecord.getMwReferenceNo(), ProcessingConstant.FORM_12);

                        if (form03 == null && form12 == null)
                        {
                            checking = false;
                        }
                        else
                        {
                            bool cForm03 = true;
                            bool cForm12 = true;

                            if (form03 != null)
                            {
                                //cForm03 = (!mwRecord.COMPLETION_DATE.before(form03.getCommencementDate())) ? true : false;
                                cForm03 = (!(mwRecord.COMPLETION_DATE < form03.COMMENCEMENT_DATE)) ? true : false;
                            }

                            if (form12 != null)
                            {
                                //cForm12 = (!mwRecord.COMPLETION_DATE.before(form12.getCommencementDate())) ? true : false;
                                cForm12 = (!(mwRecord.COMPLETION_DATE < form12.COMMENCEMENT_DATE)) ? true : false;

                            }

                            checking = (cForm03 == false || cForm12 == false) ? false : true;
                        }
                    }
                    else
                    {
                        //checking = (!mwRecord.COMPLETION_DATE.before(mwRecord.getCommencementDate())) ? true : false;
                        checking = (!(mwRecord.COMPLETION_DATE < mwRecord.COMMENCEMENT_DATE)) ? true : false;
                    }
                }
            }
            catch (Exception e)
            {
            }


            //formChecklist.setInfoDate((checking) ? ProcessingConstant.CHECKING_OK : ProcessingConstant.CHECKING_NOT_OK);
            formChecklist.INFO_DATE = ((checking) ? ProcessingConstant.CHECKING_OK : ProcessingConstant.CHECKING_NOT_OK);

        }


        private void prefillPBPValid(P_MW_RECORD mwRecord, P_MW_APPOINTED_PROFESSIONAL appointedAP, P_MW_APPOINTED_PROFESSIONAL appointedRSE, P_MW_APPOINTED_PROFESSIONAL appointedRGE, P_MW_APPOINTED_PROFESSIONAL appointedPRC, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_FORM mwForm, P_MW_APPOINTED_PROFESSIONAL appointedOther1, P_MW_APPOINTED_PROFESSIONAL appointedOther0, List<P_MW_FORM_09> mwForm09List, P_MW_APPOINTED_PROFESSIONAL appointedAPForm8, VerificaionFormModel model)
        {
            try
            {
                //
                // Form 01
                //
                if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
                {
                    prefillApValid(appointedAP.COMMENCED_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                    if (appointedRSE != null)
                    {
                        prefillRSEValid(appointedAP.COMMENCED_DATE, model, appointedRSE, formChecklist, mwForm);
                    }
                    if (appointedRGE != null)
                    {
                        prefillRGEValid(appointedAP.COMMENCED_DATE, model, appointedRGE, formChecklist, mwForm);
                    }
                    prefillPRCValid(appointedAP.COMMENCED_DATE, appointedPRC.COMMENCED_DATE, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 02
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
                {
                    prefillApValid(appointedAP.COMPLETION_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                    if (appointedRSE != null)
                    {
                        prefillRSEValid(appointedAP.COMPLETION_DATE, model, appointedRSE, formChecklist, mwForm);
                    }
                    if (appointedRGE != null)
                    {
                        prefillRGEValid(appointedAP.COMPLETION_DATE, model, appointedRGE, formChecklist, mwForm);
                    }

                    P_MW_APPOINTED_PROFESSIONAL appointedPRC7 = mwAppointedProfessionalService.findFormPBPByMWRecordOrdering(mwRecord, 7);

                    prefillPRCValid(appointedAP.COMPLETION_DATE, appointedPRC7.COMPLETION_DATE, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 03
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
                {
                    prefillPRCValid(appointedPRC.COMMENCED_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 04
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04))
                {
                    prefillPRCValid(appointedPRC.COMPLETION_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 05
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05))
                {
                    prefillPRCValid(appointedPRC.COMPLETION_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 06
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
                {
                    String certNo = (appointedAP.CERTIFICATION_NO).ToUpper();
                    //			formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);			
                    //			formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                    //			formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    //			formChecklist.FORM6_AP_AS_SIGN = (ProcessingConstant.CHECKING_NOT_OK);
                    //			formChecklist.FORM6_AP_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);
                    //			formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);

                    if ((model.CheckPage.Equals(VER_FORM_PBP) || true)
                            && appointedAP != null
                            && certNo.StartsWith(ProcessingConstant.AP) || certNo.StartsWith(ProcessingConstant.RI) || certNo.StartsWith(ProcessingConstant.RSE))
                    {
                        prefillApValid(appointedAP.COMPLETION_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                    }
                    else if ((model.CheckPage.Equals(VER_FORM_PBP) || true)
                          && appointedAP != null
                          && (appointedAP.IDENTIFY_FLAG.Equals(ProcessingConstant.RGBC)
                                  || appointedAP.IDENTIFY_FLAG.Equals(ProcessingConstant.MWCP)
                                  || appointedAP.IDENTIFY_FLAG.Equals(ProcessingConstant.MWC)))
                    {
                        //TBC
                        List<P_MW_RECORD_ITEM> mwRecordItemList = mwRecordItemService.getMwRecordItemByMwRecordOrdering(mwRecord, false);
                        removeUncheckedItems(mwRecordItemList, mwRecord);

                        List<V_CRM_INFO> profInfoList = mwCrmInfoService.findListByCertNo((appointedAP.CERTIFICATION_NO));

                        if (profInfoList != null && profInfoList.Count() > 0)
                        {
                            V_CRM_INFO mwCrmInfoPBP = null;

                            bool checkEngNamePassed = false;
                            bool checkChiNamePassed = false;
                            bool checkEngAsNamePassed = false;
                            bool checkChiAsNamePassed = false;

                            for (int i = 0; i < profInfoList.Count(); i++)
                            {
                                V_CRM_INFO mwCrmInfo = profInfoList[i];

                                // RGBC, MWCP, MWC & AS name checking						
                                // RGBC, MWCP, MWC English name
                                if (!checkEngNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.SURNAME + " " + mwCrmInfo.GIVEN_NAME)
                                        && !string.IsNullOrEmpty(appointedAP.ENGLISH_NAME))
                                {
                                    String surName = (mwCrmInfo.SURNAME);
                                    String givenName = (mwCrmInfo.GIVEN_NAME);

                                    String prcEnglishName = (appointedAP.ENGLISH_NAME);

                                    //if (prcEnglishName.EqualsIgnoreCase((surName + " " + givenName)))
                                    if (string.Equals(prcEnglishName, surName + " " + givenName, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngNamePassed = true; // RGBC, MWCP, MWC English name check passed, no need to check again.
                                    }
                                }

                                // RGBC, MWCP, MWC Chinese name
                                if (!checkChiNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                        && !string.IsNullOrEmpty(appointedAP.CHINESE_NAME))
                                {
                                    String chineseName = (mwCrmInfo.CHINESE_NAME);

                                    String prcChineseName = (appointedAP.CHINESE_NAME);

                                    //if (prcChineseName.EqualsIgnoreCase(chineseName))
                                    if (string.Equals(prcChineseName, chineseName, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiNamePassed = true; // RGBC, MWCP, MWC Chinese name check passed, no need to check again.
                                    }
                                }

                                // AS English name
                                if (!checkEngAsNamePassed)
                                {
                                    String asSurName = (mwCrmInfo.AS_SURNAME);
                                    String asGivenName = (mwCrmInfo.AS_GIVEN_NAME);

                                    String prcEnglishCompanyName = (appointedAP.ENGLISH_COMPANY_NAME);

                                    if (string.IsNullOrEmpty(prcEnglishCompanyName) && string.IsNullOrEmpty(asSurName) && string.IsNullOrEmpty(asGivenName))
                                    {
                                        checkEngAsNamePassed = true;
                                        formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NA);
                                    }
                                    else if (string.Equals(prcEnglishCompanyName, asSurName + " " + asGivenName, StringComparison.CurrentCultureIgnoreCase))
                                    //else if (prcEnglishCompanyName.EqualsIgnoreCase((asSurName + " " + asGivenName)))
                                    {
                                        checkEngAsNamePassed = true; // AS English name check passed, no need to check again.
                                    }
                                }

                                // AS Chinese name
                                if (!checkChiAsNamePassed)
                                {
                                    String chineseAsName = (mwCrmInfo.AS_CHINESE_NAME);

                                    String prcChineseCompanyName = (appointedAP.CHINESE_COMPANY_NAME);

                                    if (string.IsNullOrEmpty(prcChineseCompanyName) && string.IsNullOrEmpty(chineseAsName))
                                    {
                                        checkChiAsNamePassed = true;
                                        formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NA);
                                    }
                                    else if (string.Equals(prcChineseCompanyName, chineseAsName, StringComparison.CurrentCultureIgnoreCase))
                                    //else if (prcChineseCompanyName.EqualsIgnoreCase(chineseAsName))
                                    {
                                        checkChiAsNamePassed = true; // AS Chinese name check passed, no need to check again.
                                    }
                                }

                                if (checkEngNamePassed && checkChiNamePassed)
                                {
                                    mwCrmInfoPBP = mwCrmInfo;
                                }
                            }

                            // set error msg
                            V_CRM_INFO prcInfo = profInfoList[0];
                            if (checkEngNamePassed && checkChiNamePassed)
                            {
                                if (string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_NAME))
                                {
                                    formChecklist.RGBC_MWC_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                                }
                                formChecklist.RGBC_MWC_INFO_NAME_MSG = (null);
                                formChecklist.RGBC_MWC_INFO_ENG_NAME_MSG = (null);
                                formChecklist.RGBC_MWC_INFO_CHI_NAME_MSG = (null);

                                formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else if (string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_NAME))
                            {
                                formChecklist.RGBC_MWC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RGBC_MWC_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);

                                formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                                if (!checkEngNamePassed)
                                {
                                    // Display CRM English name
                                    String surName = prcInfo.SURNAME;
                                    String givenName = prcInfo.GIVEN_NAME;

                                    if (surName == null && givenName == null)
                                    {
                                        surName = "No input vaule";
                                        givenName = "";
                                    }
                                    else if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    else if (givenName == null)
                                    {
                                        givenName = "";
                                    }
                                    String prcEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                    formChecklist.RGBC_MWC_INFO_ENG_NAME_MSG = (prcEngNameMsg);
                                }
                                else
                                {
                                    formChecklist.RGBC_MWC_INFO_ENG_NAME_MSG = (null);
                                }

                                if (!checkChiNamePassed)
                                {
                                    // Display CRM Chinese name
                                    String chineseName = prcInfo.CHINESE_NAME;
                                    if (chineseName == null)
                                    {
                                        chineseName = "No input value";
                                    }
                                    String prcChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                    formChecklist.RGBC_MWC_INFO_CHI_NAME_MSG = (prcChiNameMsg);
                                }
                                else
                                {
                                    formChecklist.RGBC_MWC_INFO_CHI_NAME_MSG = (null);
                                }
                            }

                            // AS in AP filed
                            if (checkEngAsNamePassed && checkChiAsNamePassed)
                            {
                                if (string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_AS_NAME))
                                {
                                    if (string.IsNullOrEmpty(appointedAP.ENGLISH_COMPANY_NAME)
                                            && string.IsNullOrEmpty(appointedAP.CHINESE_COMPANY_NAME))
                                    {
                                        formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NA);
                                    }
                                    else
                                    {
                                        formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_OK);

                                        formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_OK);
                                    }
                                }
                                formChecklist.RGBC_MWC_INFO_AS_NAME_MSG = (null);
                                formChecklist.RGBC_MWC_INFO_AS_ENG_NAME_MSG = (null);
                                formChecklist.RGBC_MWC_INFO_AS_CHI_NAME_MSG = (null);
                            }
                            else if (string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_AS_NAME))
                            {
                                formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RGBC_MWC_INFO_AS_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);

                                formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                                if (!checkEngAsNamePassed)
                                {
                                    // Display AS English name for PRC field
                                    String prcAsEngNameMsg = "";
                                    //TBC
                                    List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedAP.CERTIFICATION_NO));
                                    for (int i = 0; i < profNameList.Count(); i++)
                                    {
                                        V_CRM_INFO mwCrmInfo = profNameList[i];

                                        String surAsName = mwCrmInfo.AS_SURNAME;
                                        String givenAsName = mwCrmInfo.AS_GIVEN_NAME;

                                        if (surAsName == null && givenAsName == null)
                                        {
                                            surAsName = "No input vaule";
                                            givenAsName = "";
                                        }
                                        else if (surAsName == null)
                                        {
                                            surAsName = "";
                                        }
                                        else if (givenAsName == null)
                                        {
                                            givenAsName = "";
                                        }
                                        prcAsEngNameMsg += "<br/><font color='red'>CRM: " + surAsName + " " + givenAsName + "</font>";
                                    }

                                    formChecklist.RGBC_MWC_INFO_AS_ENG_NAME_MSG = ("<font color='#FF0000'>" + prcAsEngNameMsg + "</font>");
                                }

                                if (!checkChiAsNamePassed)
                                {
                                    // Display AS Chinese name for PRC field
                                    String prcAsChiNameMsg = "";
                                    //TBC
                                    List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedAP.CERTIFICATION_NO));
                                    for (int i = 0; i < profNameList.Count(); i++)
                                    {
                                        V_CRM_INFO mwCrmInfo = profNameList[i];

                                        String chineseAsName = mwCrmInfo.AS_CHINESE_NAME;
                                        if (chineseAsName == null)
                                        {
                                            chineseAsName = "No input value";
                                        }

                                        prcAsChiNameMsg += "<br/><font color='red'>CRM: " + chineseAsName + "</font>";
                                    }

                                    formChecklist.RGBC_MWC_INFO_AS_CHI_NAME_MSG = ("<font color='#FF0000'>" + prcAsChiNameMsg + "</font>");
                                }
                            }

                            if (profInfoList.Count() > 0)
                            {
                                V_CRM_INFO prc = profInfoList[0];
                                // check ap validity
                                if ((prc.COMPANY_EXPIRY_DATE != null
                                        && appointedAP.COMPLETION_DATE != null
                                        && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, appointedAP.COMPLETION_DATE.Value) == 2)))
                                {
                                    formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                                }
                                // end check ap validity
                                // check capability
                                if ((appointedAP.CERTIFICATION_NO).StartsWith(ProcessingConstant.GBC))
                                {
                                    if (formChecklist.RGBC_MWC_INFO_NAME.Equals(ProcessingConstant.CHECKING_OK))
                                    {
                                        formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_OK);
                                    }
                                    else
                                    {
                                        formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                                    }
                                }
                                else
                                {
                                    bool capable = false;
                                    for (int i = 0; i < profInfoList.Count(); i++)
                                    {
                                        V_CRM_INFO info = (V_CRM_INFO)profInfoList[i];
                                        if (info.TYPE_CODE != null && (info.TYPE_CODE.Equals("Type A") || info.TYPE_CODE.Equals("Type E")))
                                        {
                                            capable = true;
                                        }
                                    }
                                    if (capable && formChecklist.RGBC_MWC_INFO_NAME.Equals(ProcessingConstant.CHECKING_OK))
                                    {
                                        formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_OK);
                                    }
                                    else
                                    {
                                        List<String> itemList = new List<String>();

                                        for (int i = 0; i < profInfoList.Count(); i++)
                                        {
                                            V_CRM_INFO info = profInfoList[i];
                                            //TBC
                                            itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                                        }
                                        if (itemList.Count() > 0)
                                        {
                                            int notContainItem = 0;
                                            for (int i = 0; i < mwRecordItemList.Count(); i++)
                                            {
                                                P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                                                bool contain = false;
                                                for (int j = 0; j < itemList.Count(); j++)
                                                {
                                                    String item = itemList[j];
                                                    if (item.Trim().Equals((mwRecordItem.MW_ITEM_CODE)))
                                                    {
                                                        contain = true;
                                                        break;
                                                    }
                                                }
                                                if (!contain)
                                                {
                                                    notContainItem++;
                                                }
                                            }
                                            if (mwRecordItemList.Count() > 0)
                                            {
                                                if (notContainItem > 0)
                                                {
                                                    formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                                                }
                                                else
                                                {
                                                    formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_OK);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // end capability
                            // check as name/as validity/ other as
                            if (mwCrmInfoPBP != null)
                            {
                                if (appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.GBC))
                                {
                                    if (formChecklist.RGBC_MWC_INFO_NAME.Equals(ProcessingConstant.CHECKING_OK))
                                    {
                                        formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_OK);
                                    }
                                    else
                                    {
                                        formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                                    }
                                }
                                else
                                {
                                    //TBC
                                    List<V_CRM_INFO> profInfos = mwCrmInfoService.findListByCertNoEnglishNameChineseName(mwCrmInfoPBP.CERTIFICATION_NO, mwCrmInfoPBP.AS_SURNAME, mwCrmInfoPBP.AS_GIVEN_NAME, mwCrmInfoPBP.AS_CHINESE_NAME);
                                    bool capable = false;
                                    for (int i = 0; i < profInfos.Count(); i++)
                                    {
                                        V_CRM_INFO info = (V_CRM_INFO)profInfos[i];
                                        if (info.TYPE_CODE != null
                                                && (info.TYPE_CODE.Equals("Type A")
                                                        || info.TYPE_CODE.Equals("Type E")))
                                        {
                                            capable = true;
                                        }
                                    }
                                    if (capable && formChecklist.RGBC_MWC_INFO_AS_NAME.Equals(ProcessingConstant.CHECKING_OK))
                                    {
                                        formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_OK);
                                    }
                                    else
                                    {
                                        List<String> itemList = new List<String>();
                                        if (appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                                        {
                                            //TBC
                                            itemList = mwCrmInfoService.getItemNosByCertNo(appointedAP.CERTIFICATION_NO);
                                        }
                                        else if (appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWC)
                                              || appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCP))
                                        {
                                            for (int i = 0; i < profInfos.Count(); i++)
                                            {
                                                V_CRM_INFO info = (V_CRM_INFO)profInfos[i];
                                                //TBC
                                                itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                                            }
                                        }

                                        if (itemList.Count() > 0)
                                        {
                                            int notContainItem = 0;
                                            for (int i = 0; i < mwRecordItemList.Count(); i++)
                                            {
                                                P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                                                bool contain = false;
                                                for (int j = 0; j < itemList.Count(); j++)
                                                {
                                                    String item = (String)itemList[j];
                                                    if (item.Trim().Equals((mwRecordItem.MW_ITEM_CODE)))
                                                    {
                                                        contain = true;
                                                        break;
                                                    }
                                                }
                                                if (!contain)
                                                {
                                                    notContainItem++;
                                                }
                                            }
                                            if (mwRecordItemList.Count() > 0)
                                            {
                                                if (notContainItem > 0)
                                                {
                                                    formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //prefill other as
                            if (!appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                            {
                                if (formChecklist.FORM6_AP_AS_VALID.Equals(ProcessingConstant.CHECKING_NOT_OK))
                                {
                                    String otherAs = "";
                                    for (int i = 0; i < profInfoList.Count(); i++)
                                    {
                                        V_CRM_INFO otherInfo = (V_CRM_INFO)profInfoList[i];
                                        List<String> itemList = new List<String>();
                                        int notContainItem = 0;
                                        if (!appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.GBC))
                                        {
                                            if (appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWC)
                                                    || appointedAP.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCP))
                                            {
                                                //TBC
                                                List<V_CRM_INFO> profInfos = mwCrmInfoService.findListByCertNoEnglishNameChineseName(otherInfo.CERTIFICATION_NO, otherInfo.AS_SURNAME, otherInfo.AS_GIVEN_NAME, otherInfo.AS_CHINESE_NAME);
                                                for (int j = 0; j < profInfos.Count(); j++)
                                                {
                                                    V_CRM_INFO info = profInfos[j];
                                                    itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                                                }
                                            }

                                            if (itemList.Count() > 0)
                                            {
                                                for (int k = 0; k < mwRecordItemList.Count(); k++)
                                                {
                                                    P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[k];
                                                    bool contain = false;
                                                    for (int j = 0; j < itemList.Count(); j++)
                                                    {
                                                        String item = (String)itemList[j];
                                                        if (item.Trim().Equals(mwRecordItem.MW_ITEM_CODE))
                                                        {
                                                            contain = true;
                                                            break;
                                                        }
                                                    }
                                                    if (!contain)
                                                    {
                                                        notContainItem++;
                                                    }
                                                }
                                            }
                                        }
                                        if (mwRecordItemList.Count() > 0)
                                        {
                                            if (notContainItem == 0)
                                            {
                                                otherAs += otherInfo.AS_SURNAME + " " + otherInfo.AS_GIVEN_NAME + " <img style='cursor:pointer;' src='images/ico_view.gif' onclick=openAsFile('" + otherInfo.UUID + "') alt='ico' />" + "<br />";
                                                for (int k = 0; k < profInfoList.Count(); k++)
                                                {
                                                    V_CRM_INFO mci = profInfoList[k];
                                                    if ((mci.AS_SURNAME).Equals((otherInfo.AS_SURNAME))
                                                            && (mci.AS_GIVEN_NAME).Equals((otherInfo.AS_GIVEN_NAME))
                                                            && (mci.AS_CHINESE_NAME).Equals((otherInfo.AS_CHINESE_NAME)))
                                                    {
                                                        profInfoList.Remove(mci);
                                                        k--;
                                                    }
                                                }
                                                i--;
                                            }
                                        }
                                    }
                                    formChecklist.FORM6_AP_OTHER_AS_LIST = (otherAs);
                                }
                            }
                        }

                        // checking AP is not expiry
                        if (profInfoList.Count() > 0)
                        {
                            V_CRM_INFO prc = (V_CRM_INFO)profInfoList[0];
                            if ((prc.COMPANY_EXPIRY_DATE != null
                                    && appointedAP.COMPLETION_DATE != null
                                    && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, appointedAP.COMPLETION_DATE.Value) == 2)))
                            {
                                formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        if (appointedAP.ENDORSEMENT_DATE != null
                                && !(appointedAP.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_AP_SIGNATURE_DATE))
                            {
                                formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                            }
                        }
                    }
                    else
                    {
                        if (model.CheckPage.Equals(VER_FORM_PBP) || true)
                        {
                            if (string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_NAME)
                                    || string.IsNullOrEmpty(formChecklist.RGBC_MWC_INFO_AS_NAME))
                            {
                                formChecklist.RGBC_MWC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.RGBC_MWC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            }

                            formChecklist.RGBC_MWC_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                            formChecklist.RGBC_MWC_INFO_AS_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                            formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.FORM6_AP_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.FORM6_AP_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.FORM6_AP_AS_SIGN = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.FORM6_AP_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }

                    prefillPRCValid(appointedPRC.COMPLETION_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 07
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07))
                {
                    prefillApValid(appointedAP.ENDORSEMENT_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);

                    if (appointedRSE != null)
                    {
                        prefillRSEValid(appointedRSE.ENDORSEMENT_DATE, model, appointedRSE, formChecklist, mwForm);
                    }

                    if (appointedRGE != null)
                    {
                        prefillRGEValid(appointedRGE.ENDORSEMENT_DATE, model, appointedRGE, formChecklist, mwForm);
                    }

                    if (string.IsNullOrEmpty(appointedPRC.ENGLISH_NAME)
                            && string.IsNullOrEmpty(appointedPRC.CHINESE_NAME)
                            && string.IsNullOrEmpty(appointedPRC.ENGLISH_COMPANY_NAME)
                            && string.IsNullOrEmpty(appointedPRC.CHINESE_COMPANY_NAME))
                    {
                        formChecklist.PRC_NAME = (ProcessingConstant.CHECKING_NA);
                        formChecklist.PRC_VALID_RMK = (ProcessingConstant.NA);
                        formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_NA);
                        formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NA);
                    }
                    else
                    {
                        prefillPRCValid(appointedPRC.ENDORSEMENT_DATE, null, model, appointedPRC, formChecklist, mwForm);
                    }
                }
                //
                // Form 08
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08))
                {
                    prefillApValid(appointedAP.APPOINTMENT_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                }
                //
                // Form 10
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
                {
                    //TBC
                    //P_MW_FORM mwForm = mwFormService.getMwFormByMwRecordandFormCode(mwRecord);
                    prefillPRCValid(appointedPRC.CLASS1_CEASE_DATE, appointedPRC.CLASS2_CEASE_DATE, model, appointedPRC, formChecklist, mwForm);
                    prefillApValid(mwForm.RECEIVED_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                }
                //
                // Form 11
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
                {
                    prefillApValid(appointedAP.COMMENCED_DATE, mwRecord, appointedAP, appointedAPForm8, formChecklist, mwForm, model);
                    if (appointedRSE != null)
                    {
                        prefillRSEValid(appointedAP.COMMENCED_DATE, model, appointedRSE, formChecklist, mwForm);
                    }
                    if (appointedRGE != null)
                    {
                        prefillRGEValid(appointedAP.COMMENCED_DATE, model, appointedRGE, formChecklist, mwForm);
                    }
                    prefillPRCValid(appointedAP.COMMENCED_DATE, appointedPRC.COMMENCED_DATE, model, appointedPRC, formChecklist, mwForm);
                }
                // 
                // Form 12
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12))
                {
                    prefillPRCValid(appointedPRC.COMMENCED_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 31
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
                {
                    prefillOtherValid(appointedOther0, appointedOther0.CLASS1_CEASE_DATE, null, formChecklist, appointedPRC);
                }
                //	
                // Form 32
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_32))
                {
                    prefillPRCValid(appointedPRC.COMMENCED_DATE, null, model, appointedPRC, formChecklist, mwForm);
                }
                //
                // Form 33
                //
                else if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    //TBC
                    //P_MW_FORM mwForm = mwFormService.getMwFormByMwRecordandFormCode(mwRecord);
                    String certNo = (appointedOther0.CERTIFICATION_NO).ToUpper();

                    if (certNo.StartsWith(ProcessingConstant.AP))
                    {
                        model.ProfessionalType = (ProcessingConstant.AP);
                    }
                    else if (certNo.StartsWith(ProcessingConstant.RI))
                    {
                        model.ProfessionalType = (ProcessingConstant.RI);
                    }
                    else if (certNo.StartsWith(ProcessingConstant.RSE))
                    {
                        model.ProfessionalType = (ProcessingConstant.RSE);
                    }
                    else if (certNo.StartsWith(ProcessingConstant.RGE))
                    {
                        model.ProfessionalType = (ProcessingConstant.RGE);
                    }
                    else
                    {
                        model.ProfessionalType = (ProcessingConstant.PRC);
                    }
                    prefillOtherValid(appointedOther0, mwForm.RECEIVED_DATE, null, formChecklist, appointedPRC);
                }
                //
                // Form 09
                //
                else if (model.CheckPage.Equals(VER_FORM_PBP) || true)
                {
                    if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                    {
                        formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_NA);
                        bool pbpNewInfoNameOk = false;
                        bool pbpApInfoNameOK = false;

                        if (appointedOther1 != null && appointedOther1.CERTIFICATION_NO != null)
                        {
                            V_CRM_INFO profInfo;
                            //TBC
                            //P_MW_FORM mwForm = mwFormService.getMwFormByMwRecordandFormCode(mwRecord);

                            formChecklist.PBP_NEW_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_NEW_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                            pbpNewInfoNameOk = false;
                            pbpApInfoNameOK = false;

                            if (appointedOther0 != null && appointedOther0.CERTIFICATION_NO != null)
                            {
                                //TBC
                                profInfo = mwCrmInfoService.findByCertNo((appointedOther0.CERTIFICATION_NO));
                                if (profInfo != null)
                                {
                                    //if ((string.IsNullOrEmpty(appointedOther0.CHINESE_NAME)
                                    //            && (profInfo.SURNAME + " " + profInfo.GIVEN_NAME).EqualsIgnoreCase((appointedOther0.ENGLISH_NAME)))
                                    //        || (string.IsNullOrEmpty(appointedOther0.ENGLISH_NAME)
                                    //            && (profInfo.CHINESE_NAME).EqualsIgnoreCase((appointedOther0.CHINESE_NAME)))
                                    //        || (!string.IsNullOrEmpty(appointedOther0.ENGLISH_NAME)
                                    //            && (!string.IsNullOrEmpty(appointedOther0.CHINESE_NAME)
                                    //            && (profInfo.SURNAME + " " + profInfo.GIVEN_NAME).EqualsIgnoreCase((appointedOther0.ENGLISH_NAME))
                                    //            && (profInfo.CHINESE_NAME).EqualsIgnoreCase((appointedOther0.CHINESE_NAME)))))
                                    if ((string.IsNullOrEmpty(appointedOther0.CHINESE_NAME) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, appointedOther0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (string.IsNullOrEmpty(appointedOther0.ENGLISH_NAME) && string.Equals(profInfo.CHINESE_NAME, appointedOther0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (!string.IsNullOrEmpty(appointedOther0.ENGLISH_NAME) && (!string.IsNullOrEmpty(appointedOther0.CHINESE_NAME) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, appointedOther0.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.CHINESE_NAME, appointedOther0.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase)))
                                          )
                                    {
                                        formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_OK);
                                        pbpApInfoNameOK = true;

                                        if (profInfo.EXPIRY_DATE != null
                                                && appointedOther1.EFFECT_FROM_DATE != null
                                                && (DateUtil.compareDate(profInfo.EXPIRY_DATE.Value, appointedOther1.EFFECT_FROM_DATE.Value) == 2))
                                        {
                                            formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                                        }
                                    }
                                }
                                // PBP AP (Nominator) name checking
                                if (formChecklist.PBP_AP_NAME != null && formChecklist.PBP_AP_NAME.Equals(ProcessingConstant.CHECKING_NA))
                                {
                                    formChecklist.PBP_AP_INFO_NAME = (null);
                                    formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                                    formChecklist.PBP_AP_INFO_ENGLISH_NAME = (null);
                                    formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                                    formChecklist.PBP_AP_INFO_CHINESE_NAME = (null);
                                    formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                                }
                                else if (!pbpApInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                                {
                                    formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                    formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                                    // Display CRM English name
                                    String surName = null;
                                    String givenName = null;
                                    String chineseName = null;

                                    if (profInfo != null)
                                    {
                                        surName = profInfo.SURNAME;
                                        givenName = profInfo.GIVEN_NAME;
                                        chineseName = profInfo.CHINESE_NAME;
                                    }

                                    String apApEngName = appointedOther0.ENGLISH_NAME;
                                    String apApChiName = appointedOther0.CHINESE_NAME;

                                    if (surName == null && givenName == null)
                                    {
                                        surName = "No input vaule";
                                        givenName = "";
                                    }
                                    else if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    else if (givenName == null)
                                    {
                                        givenName = "";
                                    }

                                    if (apApEngName == null)
                                    {
                                        apApEngName = "";
                                    }

                                    //if (!apApEngName.Trim().EqualsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                    if (!string.Equals(apApEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        String pbpApEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                        formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                        formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (pbpApEngNameMsg);
                                    }

                                    // Display CRM Chinese name
                                    if (chineseName == null)
                                    {
                                        chineseName = "No input value";
                                    }

                                    if (apApChiName == null)
                                    {
                                        apApChiName = "";
                                    }

                                    //if (!apApChiName.Trim().EqualsIgnoreCase(chineseName.Trim()))
                                    if (!string.Equals(apApChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        String pbpApChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                        formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                        formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (pbpApChiNameMsg);
                                    }
                                }
                                else if (pbpApInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                                {
                                    formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                                    formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                                    formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                                    formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                                    formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                                    formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                                }
                            }

                            profInfo = mwCrmInfoService.findByCertNo((appointedOther1.CERTIFICATION_NO));
                            if (profInfo != null)
                            {
                                //if ((string.IsNullOrEmpty(appointedOther1.CHINESE_NAME)
                                //            && (profInfo.SURNAME + " " + profInfo.GIVEN_NAME).EqualsIgnoreCase((appointedOther1.ENGLISH_NAME)))
                                //        || (string.IsNullOrEmpty(appointedOther1.ENGLISH_NAME)
                                //            && (profInfo.CHINESE_NAME).EqualsIgnoreCase((appointedOther1.CHINESE_NAME)))
                                //        || (!string.IsNullOrEmpty(appointedOther1.ENGLISH_NAME)
                                //            && (!string.IsNullOrEmpty(appointedOther1.CHINESE_NAME)
                                //            && (profInfo.SURNAME + " " + profInfo.GIVEN_NAME).EqualsIgnoreCase((appointedOther1.ENGLISH_NAME))
                                //            && (profInfo.CHINESE_NAME).EqualsIgnoreCase((appointedOther1.CHINESE_NAME)))))
                                if ((string.IsNullOrEmpty(appointedOther1.CHINESE_NAME) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, appointedOther1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    || (string.IsNullOrEmpty(appointedOther1.ENGLISH_NAME) && string.Equals(profInfo.CHINESE_NAME, appointedOther1.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    || (!string.IsNullOrEmpty(appointedOther1.ENGLISH_NAME) && (!string.IsNullOrEmpty(appointedOther1.CHINESE_NAME) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, appointedOther1.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.CHINESE_NAME, appointedOther1.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase)))
                                    )
                                {
                                    formChecklist.PBP_NEW_NAME = (ProcessingConstant.CHECKING_OK);
                                    pbpNewInfoNameOk = true;

                                    if (profInfo.EXPIRY_DATE != null
                                                && ((appointedOther1.EFFECT_TO_DATE != null
                                                && (DateUtil.compareDate(profInfo.EXPIRY_DATE.Value, appointedOther1.EFFECT_TO_DATE.Value) == 2))
                                            || (mwForm != null
                                                && (mwForm.FORM09_FURTHER_NOTICE).Equals(ProcessingConstant.FLAG_Y)
                                                && appointedOther1.EFFECT_FROM_DATE != null
                                                && (DateUtil.compareDate(profInfo.EXPIRY_DATE.Value, appointedOther1.EFFECT_FROM_DATE.Value) == 2))))
                                    {
                                        formChecklist.PBP_NEW_AP_VALID = (ProcessingConstant.CHECKING_OK);
                                    }
                                }
                            }
                            // PBP Nominee name checking
                            if (formChecklist.PBP_NEW_NAME != null && formChecklist.PBP_NEW_NAME.Equals(ProcessingConstant.CHECKING_NA))
                            {
                                formChecklist.PBP_NEW_INFO_NAME = (null);
                                formChecklist.PBP_NEW_INFO_NAME_MSG = (null);
                                formChecklist.PBP_NEW_INFO_ENGLISH_NAME = (null);
                                formChecklist.PBP_NEW_INFO_ENGLISH_NAME_MSG = (null);
                                formChecklist.PBP_NEW_INFO_CHINESE_NAME = (null);
                                formChecklist.PBP_NEW_INFO_CHINESE_NAME_MSG = (null);
                            }
                            else if (!pbpNewInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_NEW_INFO_NAME))
                            {
                                formChecklist.PBP_NEW_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_NEW_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                                // Display CRM English name

                                String surName = null;
                                String givenName = null;
                                String chineseName = null;

                                if (profInfo != null)
                                {
                                    surName = profInfo.SURNAME;
                                    givenName = profInfo.GIVEN_NAME;
                                    chineseName = profInfo.CHINESE_NAME;
                                }

                                String apNewEngName = appointedOther1.ENGLISH_NAME;
                                String apNewChiName = appointedOther1.CHINESE_NAME;

                                if (surName == null && givenName == null)
                                {
                                    surName = "No input vaule";
                                    givenName = "";
                                }
                                else if (surName == null)
                                {
                                    surName = "";
                                }
                                else if (givenName == null)
                                {
                                    givenName = "";
                                }

                                if (apNewEngName == null)
                                {
                                    apNewEngName = "";
                                }

                                //if (!apNewEngName.Trim().EqualsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                if (!string.Equals(apNewEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    String pbpNewEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                    formChecklist.PBP_NEW_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                    formChecklist.PBP_NEW_INFO_ENGLISH_NAME_MSG = (pbpNewEngNameMsg);
                                }

                                // Display CRM Chinese name
                                if (chineseName == null)
                                {
                                    chineseName = "No input value";
                                }

                                if (apNewChiName == null)
                                {
                                    apNewChiName = "";
                                }

                                //if (!apNewChiName.Trim().EqualsIgnoreCase(chineseName.Trim()))
                                if (!string.Equals(apNewChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    String pbpNewChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                    formChecklist.PBP_NEW_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                    formChecklist.PBP_NEW_INFO_CHINESE_NAME_MSG = (pbpNewChiNameMsg);
                                }
                            }
                            else if (pbpNewInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_NEW_INFO_NAME))
                            {
                                formChecklist.PBP_NEW_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                                formChecklist.PBP_NEW_INFO_NAME_MSG = (null);
                                formChecklist.PBP_NEW_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                                formChecklist.PBP_NEW_INFO_ENGLISH_NAME_MSG = (null);
                                formChecklist.PBP_NEW_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                                formChecklist.PBP_NEW_INFO_CHINESE_NAME_MSG = (null);
                            }

                            if (!string.IsNullOrEmpty(appointedOther0.CERTIFICATION_NO))
                            {
                                //TBC
                                mwForm09List = mwForm09Service.getMwForm09ByMwRecord(mwRecord);

                                for (int i = 0; i < mwForm09List.Count(); i++)
                                {
                                    P_MW_FORM_09 mwForm09 = mwForm09List[i];
                                    mwForm09.TO_CHECKING = (ProcessingConstant.CHECKING_NOT_OK);
                                    //TBC
                                    P_MW_REFERENCE_NO mwReferenceNo = mwReferenceNoService.getMwReferenceNoByMwNo(mwForm09.MW_NUMBER);
                                    P_MW_RECORD correspondingRecord = null;
                                    P_MW_APPOINTED_PROFESSIONAL ap = null;
                                    if (mwReferenceNo != null)
                                    {
                                        //TBC
                                        correspondingRecord = mwRecordService.getFinalMwRecordByRefNo(mwReferenceNo);
                                    }
                                    if (correspondingRecord != null)
                                    {
                                        //TBC
                                        ap = mwAppointedProfessionalService.findByMWRecordCertNo(correspondingRecord, appointedOther0.CERTIFICATION_NO);
                                    }
                                    if (ap != null)
                                    {
                                        mwForm09.TO_CHECKING = (ProcessingConstant.CHECKING_OK);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME) || string.IsNullOrEmpty(formChecklist.PBP_NEW_INFO_NAME))
                            {
                                formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_NEW_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            }
                            formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                            formChecklist.PBP_NEW_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //log.fatalError("----------Error---------- ");
                //log.fatalError("VerificationAction.prefillPBPValid()");
                //log.fatalError(e.getMessage());
                //log.fatalError("----------Error---------- ");
            }
        }



        private void prefillApValid(DateTime? compareDate, P_MW_RECORD mwRecord, P_MW_APPOINTED_PROFESSIONAL appointedAP, P_MW_APPOINTED_PROFESSIONAL appointedAPForm8, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_FORM mwForm, VerificaionFormModel model)
        {
            if ((VER_FORM_PBP.Equals(model.CheckPage) || true) || ProcessingConstant.FORM_10.Equals(mwRecord.S_FORM_TYPE_CODE))
            {
                if (appointedAP != null)
                {
                    V_CRM_INFO mwCrmInfoAP = mwCrmInfoService.findByCertNo(appointedAP.CERTIFICATION_NO);

                    formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    //				formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);				
                    //				formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);

                    bool pbpApInfoNameOK = false;

                    if (mwCrmInfoAP != null)
                    {
                        //if ((string.IsNullOrEmpty(appointedAP.CHINESE_NAME)
                        //        && (mwCrmInfoAP.SURNAME + " " + mwCrmInfoAP.GIVEN_NAME).EqualsIgnoreCase((appointedAP.ENGLISH_NAME)))
                        //    || (string.IsNullOrEmpty(appointedAP.ENGLISH_NAME)
                        //        && (mwCrmInfoAP.CHINESE_NAME).EqualsIgnoreCase((appointedAP.CHINESE_NAME)))
                        //    || (!string.IsNullOrEmpty(appointedAP.ENGLISH_NAME)
                        //        && !string.IsNullOrEmpty(appointedAP.CHINESE_NAME)
                        //        && (mwCrmInfoAP.SURNAME + " " + mwCrmInfoAP.GIVEN_NAME).EqualsIgnoreCase((appointedAP.ENGLISH_NAME))
                        //        && (mwCrmInfoAP.CHINESE_NAME).EqualsIgnoreCase((appointedAP.CHINESE_NAME))))

                        if ((string.IsNullOrEmpty(appointedAP.CHINESE_NAME) && string.Equals(mwCrmInfoAP.SURNAME + " " + mwCrmInfoAP.GIVEN_NAME, appointedAP.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                               || (string.IsNullOrEmpty(appointedAP.ENGLISH_NAME) && string.Equals(mwCrmInfoAP.CHINESE_NAME, appointedAP.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                               || (!string.IsNullOrEmpty(appointedAP.ENGLISH_NAME) && !string.IsNullOrEmpty(appointedAP.CHINESE_NAME) && string.Equals(mwCrmInfoAP.SURNAME + " " + mwCrmInfoAP.GIVEN_NAME, appointedAP.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) &&
                               string.Equals(mwCrmInfoAP.CHINESE_NAME, appointedAP.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpApInfoNameOK = true;
                        }

                        if (formChecklist.PBP_AP_NAME != null && formChecklist.PBP_AP_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (null);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpApInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoAP.SURNAME;
                            String givenName = mwCrmInfoAP.GIVEN_NAME;
                            String apApEngName = appointedAP.ENGLISH_NAME;
                            String apApChiName = appointedAP.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "No input vaule";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apApEngName == null)
                            {
                                apApEngName = "";
                            }

                            //if (!apApEngName.Trim().EqualsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apApEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpApEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (pbpApEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoAP.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apApChiName == null)
                            {
                                apApChiName = "";
                            }

                            //if (!apApChiName.Trim().EqualsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apApChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpApChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (pbpApChiNameMsg);
                            }
                        }
                        else if (pbpApInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                        }

                        if (mwCrmInfoAP.EXPIRY_DATE != null
                                && compareDate != null
                                && (DateUtil.compareDate(mwCrmInfoAP.EXPIRY_DATE.Value, compareDate.Value) == 2))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_AP_VALID))
                            {
                                formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08))
                        {
                            if (appointedAPForm8.ENDORSEMENT_DATE != null
                                    && !(appointedAPForm8.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                            {
                                if (string.IsNullOrEmpty(formChecklist.PBP_AP_SIGNATURE_DATE))
                                {
                                    formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                                }
                            }

                            appointedAP.ENDORSEMENT_DATE = (appointedAPForm8.ENDORSEMENT_DATE);
                        }
                        else
                        {
                            if (appointedAP.ENDORSEMENT_DATE != null
                                    && !(appointedAP.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                            {
                                if (string.IsNullOrEmpty(formChecklist.PBP_AP_SIGNATURE_DATE))
                                {
                                    formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                        formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                        formChecklist.PBP_AP_SIGN = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                }
            }
        }
        private void prefillRSEValid(DateTime? compareDate, VerificaionFormModel model, P_MW_APPOINTED_PROFESSIONAL appointedRSE, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_FORM mwForm)
        {
            if (VER_FORM_PBP.Equals(model.CheckPage) || true)
            {
                if (appointedRSE != null)
                {
                    V_CRM_INFO mwCrmInfoRSE = mwCrmInfoService.findByCertNo(appointedRSE.CERTIFICATION_NO);

                    formChecklist.PBP_RSE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    //				formChecklist.PBP_RSE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    //				formChecklist.PBP_RSE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);

                    bool pbpRseInfoNameOK = false;

                    if (mwCrmInfoRSE != null)
                    {
                        //if ((string.IsNullOrEmpty(appointedRSE.CHINESE_NAME)
                        //        && (mwCrmInfoRSE.SURNAME + " " + mwCrmInfoRSE.GIVEN_NAME).equalsIgnoreCase((appointedRSE.ENGLISH_NAME)))
                        //    || (string.IsNullOrEmpty(appointedRSE.ENGLISH_NAME)
                        //        && (mwCrmInfoRSE.CHINESE_NAME).equalsIgnoreCase((appointedRSE.CHINESE_NAME)))
                        //    || (!string.IsNullOrEmpty(appointedRSE.ENGLISH_NAME)
                        //        && !string.IsNullOrEmpty(appointedRSE.CHINESE_NAME)
                        //        && (mwCrmInfoRSE.SURNAME + " " + mwCrmInfoRSE.GIVEN_NAME).equalsIgnoreCase((appointedRSE.ENGLISH_NAME))
                        //        && (mwCrmInfoRSE.CHINESE_NAME).equalsIgnoreCase((appointedRSE.CHINESE_NAME))))

                        if ((string.IsNullOrEmpty(appointedRSE.CHINESE_NAME) && string.Equals(mwCrmInfoRSE.SURNAME + " " + mwCrmInfoRSE.GIVEN_NAME, appointedRSE.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                        || (string.IsNullOrEmpty(appointedRSE.ENGLISH_NAME) && string.Equals(mwCrmInfoRSE.CHINESE_NAME, appointedRSE.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                        || (!string.IsNullOrEmpty(appointedRSE.ENGLISH_NAME) && !string.IsNullOrEmpty(appointedRSE.CHINESE_NAME) && string.Equals(mwCrmInfoRSE.SURNAME + " " + mwCrmInfoRSE.GIVEN_NAME, appointedRSE.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(mwCrmInfoRSE.CHINESE_NAME, appointedRSE.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            formChecklist.PBP_RSE_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpRseInfoNameOK = true;
                        }

                        if (formChecklist.PBP_RSE_NAME != null && formChecklist.PBP_RSE_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_RSE_INFO_NAME = (null);
                            formChecklist.PBP_RSE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpRseInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_RSE_INFO_NAME))
                        {
                            formChecklist.PBP_RSE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_RSE_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoRSE.SURNAME;
                            String givenName = mwCrmInfoRSE.GIVEN_NAME;
                            String apRseEngName = appointedRSE.ENGLISH_NAME;
                            String apRseChiName = appointedRSE.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "No input vaule";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apRseEngName == null)
                            {
                                apRseEngName = "";
                            }

                            // if (!apRseEngName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apRseEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRseEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (pbpRseEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoRSE.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apRseChiName == null)
                            {
                                apRseChiName = "";
                            }

                            //if (!apRseChiName.Trim().equalsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apRseChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRseChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_RSE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (pbpRseChiNameMsg);
                            }
                        }
                        else if (pbpRseInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_RSE_INFO_NAME))
                        {
                            formChecklist.PBP_RSE_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (null);
                        }

                        if (mwCrmInfoRSE.EXPIRY_DATE != null
                                && compareDate != null
                                && (DateUtil.compareDate(mwCrmInfoRSE.EXPIRY_DATE.Value, compareDate.Value) == 2))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_RSE_VALID))
                            {
                                formChecklist.PBP_RSE_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        if (appointedRSE.ENDORSEMENT_DATE != null
                                && !(appointedRSE.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_RSE_SIGNATURE_DATE))
                            {
                                formChecklist.PBP_RSE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(formChecklist.PBP_RSE_INFO_NAME))
                        {
                            formChecklist.PBP_RSE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                        formChecklist.PBP_RSE_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                        formChecklist.PBP_RSE_SIGN = (ProcessingConstant.CHECKING_NOT_OK);
                    }

                    if (string.IsNullOrEmpty(appointedRSE.ENGLISH_NAME) && string.IsNullOrEmpty(appointedRSE.CHINESE_NAME))
                    {
                        formChecklist.PBP_RSE_DEC1_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_DEC3_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_DEC4_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_SIGNATURE_DATE = (null);
                        formChecklist.PBP_RSE_SIGNATURE_DATE_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_SIGN = (null);
                        formChecklist.PBP_RSE_SIGN_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_VALID_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RSE_NAME = (ProcessingConstant.CHECKING_NA);
                        formChecklist.PBP_RSE_VALID = (ProcessingConstant.CHECKING_NA);
                    }
                }
            }
        }

        private void prefillRGEValid(DateTime? compareDate, VerificaionFormModel model, P_MW_APPOINTED_PROFESSIONAL appointedRGE, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_FORM mwForm)
        {
            if (VER_FORM_PBP.Equals(model.CheckPage) || true)
            {
                if (appointedRGE != null)
                {
                    V_CRM_INFO mwCrmInfoRGE = mwCrmInfoService.findByCertNo(appointedRGE.CERTIFICATION_NO);

                    formChecklist.PBP_RGE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    //				formChecklist.PBP_RGE_VALID = (ProcessingConstant.CHECKING_NOT_OK);				
                    //				formChecklist.PBP_RGE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);

                    bool pbpRgeInfoNameOK = false;

                    if (mwCrmInfoRGE != null)
                    {
                        //if ((string.IsNullOrEmpty(appointedRGE.CHINESE_NAME)
                        //        && (mwCrmInfoRGE.SURNAME + " " + mwCrmInfoRGE.GIVEN_NAME).equalsIgnoreCase((appointedRGE.ENGLISH_NAME)))
                        //    || (string.IsNullOrEmpty(appointedRGE.ENGLISH_NAME)
                        //        && (mwCrmInfoRGE.CHINESE_NAME).equalsIgnoreCase((appointedRGE.CHINESE_NAME)))
                        //    || (!string.IsNullOrEmpty(appointedRGE.ENGLISH_NAME)
                        //        && !string.IsNullOrEmpty(appointedRGE.CHINESE_NAME)
                        //        && (mwCrmInfoRGE.SURNAME + " " + mwCrmInfoRGE.GIVEN_NAME).equalsIgnoreCase((appointedRGE.ENGLISH_NAME))
                        //        && (mwCrmInfoRGE.CHINESE_NAME).equalsIgnoreCase((appointedRGE.CHINESE_NAME))))

                        if ((string.IsNullOrEmpty(appointedRGE.CHINESE_NAME) && string.Equals(mwCrmInfoRGE.SURNAME + " " + mwCrmInfoRGE.GIVEN_NAME, appointedRGE.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                        || (string.IsNullOrEmpty(appointedRGE.ENGLISH_NAME) && string.Equals(mwCrmInfoRGE.CHINESE_NAME, appointedRGE.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                        || (!string.IsNullOrEmpty(appointedRGE.ENGLISH_NAME) && string.Equals(mwCrmInfoRGE.SURNAME + " " + mwCrmInfoRGE.GIVEN_NAME, appointedRGE.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(mwCrmInfoRGE.CHINESE_NAME, appointedRGE.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                        )
                        {
                            formChecklist.PBP_RGE_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpRgeInfoNameOK = true;
                        }

                        if (formChecklist.PBP_RGE_NAME != null && formChecklist.PBP_RGE_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (null);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpRgeInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoRGE.SURNAME;
                            String givenName = mwCrmInfoRGE.GIVEN_NAME;
                            String apRgeEngName = appointedRGE.ENGLISH_NAME;
                            String apRgeChiName = appointedRGE.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apRgeEngName == null)
                            {
                                apRgeEngName = "";
                            }

                            //if (!apRgeEngName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apRgeEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRgeEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (pbpRgeEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoRGE.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apRgeChiName == null)
                            {
                                apRgeChiName = "";
                            }

                            //if (!apRgeChiName.Trim().equalsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apRgeChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRgeChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_RGE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (pbpRgeChiNameMsg);
                            }
                        }
                        else if (pbpRgeInfoNameOK && string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (null);
                        }

                        if (mwCrmInfoRGE.EXPIRY_DATE != null
                                && compareDate != null
                                && (DateUtil.compareDate(mwCrmInfoRGE.EXPIRY_DATE.Value, compareDate.Value) == 2))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_RGE_VALID))
                            {
                                formChecklist.PBP_RGE_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        if (appointedRGE.ENDORSEMENT_DATE != null
                                && !(appointedRGE.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PBP_RGE_SIGNATURE_DATE))
                            {
                                formChecklist.PBP_RGE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                        formChecklist.PBP_RGE_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                        formChecklist.PBP_RGE_SIGN = (ProcessingConstant.CHECKING_NOT_OK);
                    }

                    if (string.IsNullOrEmpty(appointedRGE.ENGLISH_NAME) && string.IsNullOrEmpty(appointedRGE.CHINESE_NAME))
                    {
                        formChecklist.PBP_RGE_DEC1_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_DEC3_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_DEC4_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_SIGNATURE_DATE = (null);
                        formChecklist.PBP_RGE_SIGNATURE_DATE_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_SIGN = (null);
                        formChecklist.PBP_RGE_SIGN_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_VALID_RMK = (ProcessingConstant.NA);
                        formChecklist.PBP_RGE_NAME = (ProcessingConstant.CHECKING_NA);
                        formChecklist.PBP_RGE_VALID = (ProcessingConstant.CHECKING_NA);
                    }
                }
            }
        }

        private void prefillPRCValid(DateTime? compareDate1, DateTime? compareDate2, VerificaionFormModel model, P_MW_APPOINTED_PROFESSIONAL appointedPRC, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_FORM mwForm)
        {
            if (VER_FORM_PRC.Equals(model.CheckPage) || true)
            {
                if (appointedPRC != null)
                {
                    //List<V_CRM_INFO> profInfoList = mwCrmInfoService.findListByCertNo((appointedPRC.CERTIFICATION_NO));
                    //find Active MW_CRM_INFO
                    //TBC
                    List<V_CRM_INFO> profInfoList = mwCrmInfoService.findActiveListByCertNo((appointedPRC.CERTIFICATION_NO));

                    if (profInfoList != null && profInfoList.Count() > 0)
                    {
                        V_CRM_INFO prc = profInfoList[0];
                        if ((prc.COMPANY_EXPIRY_DATE != null && compareDate1 != null && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, compareDate1.Value) == 2))
                             || (prc.COMPANY_EXPIRY_DATE != null && compareDate2 != null && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, compareDate2.Value) == 2)))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_VALID))
                            {
                                formChecklist.PRC_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        if (appointedPRC.ENDORSEMENT_DATE != null && !(appointedPRC.ENDORSEMENT_DATE > mwForm.RECEIVED_DATE))
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_SIGNATURE_DATE))
                            {
                                formChecklist.PRC_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        bool checkEngNamePassed = false;
                        bool checkChiNamePassed = false;
                        bool checkEngAsNamePassed = false;
                        bool checkChiAsNamePassed = false;

                        for (int i = 0; i < profInfoList.Count(); i++)
                        {
                            V_CRM_INFO mwCrmInfo = profInfoList[i];

                            // authorized signatory only
                            if (appointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                            {
                                // AS English name in PRC field only
                                if (!checkEngNamePassed
                                        && !string.IsNullOrEmpty((mwCrmInfo.SURNAME + " " + mwCrmInfo.GIVEN_NAME))
                                        && !string.IsNullOrEmpty(appointedPRC.ENGLISH_NAME))
                                {

                                    String surName = mwCrmInfo.SURNAME;
                                    String givenName = mwCrmInfo.GIVEN_NAME;
                                    if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    if (givenName == null)
                                    {
                                        givenName = "";
                                    }

                                    String prcEnglishName = appointedPRC.ENGLISH_NAME;
                                    if (prcEnglishName == null)
                                    {
                                        prcEnglishName = "";
                                    }

                                    //if (prcEnglishName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngNamePassed = true; // AS English name check passed, no need to check again.
                                    }
                                }

                                // AS Chinese name in PRC field only
                                if (!checkChiNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                        && !string.IsNullOrEmpty(appointedPRC.CHINESE_NAME))
                                {

                                    String chineseName = mwCrmInfo.CHINESE_NAME;
                                    if (chineseName == null)
                                    {
                                        chineseName = "";
                                    }

                                    String prcChineseName = appointedPRC.CHINESE_NAME;
                                    if (prcChineseName == null)
                                    {
                                        prcChineseName = "";
                                    }

                                    //if (prcChineseName.Trim().equalsIgnoreCase(chineseName.Trim()))
                                    if (string.Equals(prcChineseName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }

                                // cant have both AS name and AS in PRC values (English name)
                                if (!string.IsNullOrEmpty(mwCrmInfo.AS_SURNAME) && string.IsNullOrEmpty(mwCrmInfo.AS_GIVEN_NAME))
                                {
                                    checkEngAsNamePassed = false;
                                }
                                else
                                {
                                    checkEngAsNamePassed = true;
                                }

                                // cant have both AS name and AS in PRC values (Chinese name)
                                if (!string.IsNullOrEmpty(mwCrmInfo.AS_CHINESE_NAME))
                                {
                                    checkChiAsNamePassed = false;
                                }
                                else
                                {
                                    checkChiAsNamePassed = true;
                                }
                            }
                            else
                            {   // PRC & AS name checking						
                                // PRC English name
                                if (!checkEngNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.SURNAME + " " + mwCrmInfo.GIVEN_NAME)
                                        && !string.IsNullOrEmpty(appointedPRC.ENGLISH_NAME))
                                {

                                    String surName = mwCrmInfo.SURNAME;
                                    String givenName = mwCrmInfo.GIVEN_NAME;
                                    if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    if (givenName == null)
                                    {
                                        givenName = "";
                                    }

                                    String prcEnglishName = appointedPRC.ENGLISH_NAME;
                                    if (prcEnglishName == null)
                                    {
                                        prcEnglishName = "";
                                    }

                                    //if (prcEnglishName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngNamePassed = true; // English company name check passed, no need to check again.
                                    }
                                }

                                // PRC Chinese name
                                if (!checkChiNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                        && !string.IsNullOrEmpty(appointedPRC.CHINESE_NAME))
                                {

                                    String chineseName = mwCrmInfo.CHINESE_NAME;
                                    if (chineseName == null)
                                    {
                                        chineseName = "";
                                    }

                                    String prcChineseName = appointedPRC.CHINESE_NAME;
                                    if (prcChineseName == null)
                                    {
                                        prcChineseName = "";
                                    }

                                    //if (prcChineseName.Trim().equalsIgnoreCase(chineseName.Trim()))
                                    if (string.Equals(prcChineseName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }

                                // AS English name
                                if (!checkEngAsNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.AS_SURNAME + " " + mwCrmInfo.AS_GIVEN_NAME)
                                        && !string.IsNullOrEmpty(appointedPRC.ENGLISH_COMPANY_NAME))
                                {

                                    String asSurName = mwCrmInfo.SURNAME;
                                    String asGivenName = mwCrmInfo.GIVEN_NAME;
                                    if (asSurName == null)
                                    {
                                        asSurName = "";
                                    }
                                    if (asGivenName == null)
                                    {
                                        asGivenName = "";
                                    }

                                    String prcEnglishCompanyName = appointedPRC.ENGLISH_COMPANY_NAME;
                                    if (prcEnglishCompanyName == null)
                                    {
                                        prcEnglishCompanyName = "";
                                    }

                                    //if (prcEnglishCompanyName.Trim().equalsIgnoreCase((asSurName.Trim() + " " + asGivenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishCompanyName.Trim(), (asSurName.Trim() + " " + asGivenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngAsNamePassed = true; // English company name check passed, no need to check again.
                                    }
                                }

                                // AS Chinese name
                                if (!checkChiAsNamePassed
                                        && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                        && !string.IsNullOrEmpty(appointedPRC.CHINESE_COMPANY_NAME))
                                {

                                    String chineseAsName = mwCrmInfo.AS_CHINESE_NAME;
                                    if (chineseAsName == null)
                                    {
                                        chineseAsName = "";
                                    }

                                    String prcChineseCompanyName = appointedPRC.CHINESE_COMPANY_NAME;
                                    if (prcChineseCompanyName == null)
                                    {
                                        prcChineseCompanyName = "";
                                    }

                                    //if (prcChineseCompanyName.Trim().equalsIgnoreCase(chineseAsName.Trim()))
                                    if (string.Equals(prcChineseCompanyName.Trim(), chineseAsName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiAsNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }
                            }
                        }

                        // set error msg
                        if (checkEngNamePassed && checkChiNamePassed)
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME))
                            {
                                formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            }
                            formChecklist.PRC_INFO_NAME_MSG = (null);
                            formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PRC_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME))
                        {
                            formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            if (!checkEngNamePassed)
                            {
                                // Display CRM English name
                                String surName = prc.SURNAME;
                                String givenName = prc.GIVEN_NAME;

                                if (surName == null && givenName == null)
                                {
                                    surName = "No input vaule";
                                    givenName = "";
                                }
                                else if (surName == null)
                                {
                                    surName = "";
                                }
                                else if (givenName == null)
                                {
                                    givenName = "";
                                }
                                String prcEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (prcEngNameMsg);
                            }
                            else
                            {
                                formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (null);
                            }

                            if (!checkChiNamePassed)
                            {
                                // Display CRM Chinese name
                                String chineseName = prc.CHINESE_NAME;
                                if (chineseName == null)
                                {
                                    chineseName = "No input value";
                                }
                                String prcChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PRC_INFO_CHINESE_NAME_MSG = (prcChiNameMsg);
                            }
                            else
                            {
                                formChecklist.PRC_INFO_CHINESE_NAME_MSG = (null);
                            }
                        }

                        // AS in PRC filed
                        if (checkEngAsNamePassed && checkChiAsNamePassed)
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                            {
                                formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_OK);
                            }
                            formChecklist.PRC_INFO_AS_NAME_MSG = (null);
                            formChecklist.PRC_INFO_AS_ENGLISH_NAME_MSG = (null);
                            formChecklist.PRC_INFO_AS_CHINESE_NAME_MSG = (null);
                        }
                        else if (string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                        {
                            formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_INFO_AS_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            if (!checkEngAsNamePassed)
                            {
                                // Display AS English name for PRC field
                                String prcAsEngNameMsg = "";
                                //TBC
                                List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedPRC.CERTIFICATION_NO));

                                for (int i = 0; i < profNameList.Count(); i++)
                                {
                                    V_CRM_INFO mwCrmInfo = profNameList[i];

                                    String surAsName = mwCrmInfo.AS_SURNAME;
                                    String givenAsName = mwCrmInfo.AS_GIVEN_NAME;

                                    if (surAsName == null && givenAsName == null)
                                    {
                                        surAsName = "No input vaule";
                                        givenAsName = "";
                                    }
                                    else if (surAsName == null)
                                    {
                                        surAsName = "";
                                    }
                                    else if (givenAsName == null)
                                    {
                                        givenAsName = "";
                                    }

                                    prcAsEngNameMsg += "<br/>CRM: " + surAsName + " " + givenAsName;
                                }

                                formChecklist.PRC_INFO_AS_ENGLISH_NAME_MSG = ("<font color='red'>" + prcAsEngNameMsg + "</font>");
                            }

                            if (!checkChiAsNamePassed)
                            {
                                // Display AS Chinese name for PRC field
                                String prcAsChiNameMsg = "";
                                //TBC
                                List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedPRC.CERTIFICATION_NO));
                                for (int i = 0; i < profNameList.Count(); i++)
                                {
                                    V_CRM_INFO mwCrmInfo = profNameList[i];
                                    String chineseAsName = mwCrmInfo.AS_CHINESE_NAME;
                                    if (chineseAsName == null)
                                    {
                                        chineseAsName = "No input value";
                                    }

                                    prcAsChiNameMsg += "<br/><font color='red'>CRM: " + chineseAsName + "</font>";
                                }

                                formChecklist.PRC_INFO_AS_CHINESE_NAME_MSG = ("<font color='#FF0000'>" + prcAsChiNameMsg + "</font>");
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME) || string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                        {
                            formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                        formChecklist.PRC_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                        formChecklist.PRC_INFO_AS_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                        formChecklist.PRC_VALID = (ProcessingConstant.CHECKING_NOT_OK);

                        formChecklist.PRC_AS_SIGN = (ProcessingConstant.CHECKING_NOT_OK);

                        formChecklist.PRC_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                }
            }
        }


        private void prefillOtherValid(P_MW_APPOINTED_PROFESSIONAL ap, DateTime? compareDate1, DateTime? compareDate2, P_MW_RECORD_FORM_CHECKLIST formChecklist, P_MW_APPOINTED_PROFESSIONAL appointedPRC)
        {
            if (ap != null && ap.CERTIFICATION_NO != null)
            {
                //TBC
                V_CRM_INFO mwCrmInfoOther = mwCrmInfoService.findByCertNo(ap.CERTIFICATION_NO);

                bool pbpApInfoNameOk = false;
                bool pbpRseInfoNameOk = false;
                bool pbpRgeInfoNameOk = false;

                if (ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.AP) || ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.RI))
                {
                    formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                }
                else if (ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.RSE))
                {
                    formChecklist.PBP_RSE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_RSE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_RSE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                }
                else if (ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.RGE))
                {
                    formChecklist.PBP_RGE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_RGE_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PBP_RGE_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                }
                else
                {
                    formChecklist.PRC_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PRC_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PRC_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    formChecklist.PRC_SIGNATURE_DATE = (ProcessingConstant.CHECKING_NOT_OK);
                }

                if (mwCrmInfoOther != null)
                {
                    if (ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.AP) || ap.CERTIFICATION_NO.ToUpper().StartsWith(ProcessingConstant.RI))
                    {
                        //if ((string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //            && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME)))
                        //        || (string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //                && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME)))
                        //        || (!string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //                && !string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //                && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME))
                        //                && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME))))

                        if ((string.IsNullOrEmpty(ap.CHINESE_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                              || (string.IsNullOrEmpty(ap.ENGLISH_NAME) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                              || (!string.IsNullOrEmpty(ap.ENGLISH_NAME) && !string.IsNullOrEmpty(ap.CHINESE_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            formChecklist.PBP_AP_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpApInfoNameOk = true;
                        }

                        if (ap.ENDORSEMENT_DATE != null && !(ap.ENDORSEMENT_DATE > compareDate1))
                        {
                            formChecklist.PBP_AP_SIGNATURE_DATE = (ProcessingConstant.CHECKING_OK);
                        }

                        // PBP AP name checking
                        if (formChecklist.PBP_AP_NAME != null && formChecklist.PBP_AP_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (null);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpApInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoOther.SURNAME;
                            String givenName = mwCrmInfoOther.GIVEN_NAME;
                            String apApEngName = ap.ENGLISH_NAME;
                            String apApChiName = ap.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "No input vaule";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apApEngName == null)
                            {
                                apApEngName = "";
                            }

                            //if (!apApEngName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apApEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpApEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (pbpApEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoOther.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apApChiName == null)
                            {
                                apApChiName = "";
                            }

                            //if (!apApChiName.Trim().equalsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apApChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpApChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (pbpApChiNameMsg);
                            }
                        }
                        else if (pbpApInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_AP_INFO_CHINESE_NAME_MSG = (null);
                        }
                    }
                    else if ((ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.RSE))
                    {
                        //if ((string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //            && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME)))
                        //        || (string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //            && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME)))
                        //        || (!string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //            && !string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //            && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME))
                        //            && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME))))

                        if ((string.IsNullOrEmpty(ap.CHINESE_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                              || (string.IsNullOrEmpty(ap.ENGLISH_NAME) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                              || (!string.IsNullOrEmpty(ap.ENGLISH_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                              )
                        {
                            formChecklist.PBP_RSE_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpRseInfoNameOk = true;
                        }

                        // PBP RSE name checking
                        if (formChecklist.PBP_RSE_NAME != null && formChecklist.PBP_RSE_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_RSE_INFO_NAME = (null);
                            formChecklist.PBP_RSE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpRseInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_RSE_INFO_NAME.Equals(ProcessingConstant.CHECKING_NOT_OK);

                            formChecklist.PBP_RSE_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoOther.SURNAME;
                            String givenName = mwCrmInfoOther.GIVEN_NAME;
                            String apRseEngName = ap.ENGLISH_NAME;
                            String apRseChiName = ap.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "No input vaule";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apRseEngName == null)
                            {
                                apRseEngName = "";
                            }

                            //if (!apRseEngName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apRseEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRseEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (pbpRseEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoOther.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apRseChiName == null)
                            {
                                apRseChiName = "";
                            }

                            //if (!apRseChiName.Trim().equalsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apRseChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRseChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_RSE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (pbpRseChiNameMsg);
                            }
                        }
                        else if (pbpRseInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                        {
                            formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RSE_INFO_CHINESE_NAME_MSG = (null);
                        }
                    }
                    else if ((ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.RGE))
                    {
                        //if ((string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //            && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME)))
                        //        || (string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //                && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME)))
                        //                || (!string.IsNullOrEmpty(ap.ENGLISH_NAME)
                        //                        && !string.IsNullOrEmpty(ap.CHINESE_NAME)
                        //                        && (mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME).equalsIgnoreCase((ap.ENGLISH_NAME))
                        //                        && (mwCrmInfoOther.CHINESE_NAME).equalsIgnoreCase((ap.CHINESE_NAME))))


                        if ((string.IsNullOrEmpty(ap.CHINESE_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase))
                               || (string.IsNullOrEmpty(ap.ENGLISH_NAME) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                               || (!string.IsNullOrEmpty(ap.ENGLISH_NAME) && !string.IsNullOrEmpty(ap.CHINESE_NAME) && string.Equals(mwCrmInfoOther.SURNAME + " " + mwCrmInfoOther.GIVEN_NAME, ap.ENGLISH_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(mwCrmInfoOther.CHINESE_NAME, ap.CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))

                               )
                        {
                            formChecklist.PBP_RGE_NAME = (ProcessingConstant.CHECKING_OK);
                            pbpRgeInfoNameOk = true;
                        }

                        // PBP RGE
                        if (formChecklist.PBP_RGE_NAME != null && formChecklist.PBP_RGE_NAME.Equals(ProcessingConstant.CHECKING_NA))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (null);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (!pbpRgeInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                        {
                            formChecklist.PBP_RGE_INFO_NAME.Equals(ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            // Display CRM English name
                            String surName = mwCrmInfoOther.SURNAME;
                            String givenName = mwCrmInfoOther.GIVEN_NAME;
                            String apRgeEngName = ap.ENGLISH_NAME;
                            String apRgeChiName = ap.CHINESE_NAME;

                            if (surName == null && givenName == null)
                            {
                                surName = "";
                                givenName = "";
                            }
                            else if (surName == null)
                            {
                                surName = "";
                            }
                            else if (givenName == null)
                            {
                                givenName = "";
                            }

                            if (apRgeEngName == null)
                            {
                                apRgeEngName = "";
                            }

                            //if (!apRgeEngName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                            if (!string.Equals(apRgeEngName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRgeEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (pbpRgeEngNameMsg);
                            }

                            // Display CRM Chinese name
                            String chineseName = mwCrmInfoOther.CHINESE_NAME;
                            if (chineseName == null)
                            {
                                chineseName = "No input value";
                            }

                            if (apRgeChiName == null)
                            {
                                apRgeChiName = "";
                            }

                            //if (!apRgeChiName.Trim().equalsIgnoreCase(chineseName.Trim()))
                            if (!string.Equals(apRgeChiName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                String pbpRgeChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PBP_RGE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                                formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (pbpRgeChiNameMsg);
                            }
                        }
                        else if (pbpRgeInfoNameOk && string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                        {
                            formChecklist.PBP_RGE_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME = (ProcessingConstant.CHECKING_OK);
                            formChecklist.PBP_RGE_INFO_CHINESE_NAME_MSG = (null);
                        }
                    }
                    else
                    { // PRC name checking
                      //List<V_CRM_INFO> profInfoList = mwCrmInfoService.findListByCertNo((ap.CERTIFICATION_NO));
                      //find Active MW_CRM_INFO
                      //TBC
                        List<V_CRM_INFO> profInfoList = mwCrmInfoService.findActiveListByCertNo((ap.CERTIFICATION_NO));
                        V_CRM_INFO prc;
                        if (profInfoList.Count() > 0)
                        {
                            prc = profInfoList[0];
                            if ((prc.COMPANY_EXPIRY_DATE != null
                                        && compareDate1 != null
                                        && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, compareDate1.Value) == 2))
                                    || (prc.COMPANY_EXPIRY_DATE != null
                                        && compareDate2 != null
                                        && (DateUtil.compareDate(prc.COMPANY_EXPIRY_DATE.Value, compareDate2.Value) == 2)))
                            {
                                formChecklist.PRC_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }

                        bool checkEngNamePassed = false;
                        bool checkChiNamePassed = false;
                        bool checkEngAsNamePassed = false;
                        bool checkChiAsNamePassed = false;

                        for (int i = 0; i < profInfoList.Count(); i++)
                        {
                            V_CRM_INFO mwCrmInfo = (V_CRM_INFO)profInfoList[i];

                            // authorized signatory only
                            if (ap.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                            {
                                // AS English name in PRC field only
                                if (!checkEngNamePassed
                                   && !string.IsNullOrEmpty((mwCrmInfo.SURNAME + " " + mwCrmInfo.GIVEN_NAME))
                                   && !string.IsNullOrEmpty(ap.ENGLISH_NAME))
                                {

                                    String surName = mwCrmInfo.SURNAME;
                                    String givenName = mwCrmInfo.GIVEN_NAME;
                                    if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    if (givenName == null)
                                    {
                                        givenName = "";
                                    }

                                    String prcEnglishName = ap.ENGLISH_NAME;
                                    if (prcEnglishName == null)
                                    {
                                        prcEnglishName = "";
                                    }

                                    //if (prcEnglishName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngNamePassed = true; // AS English name check passed, no need to check again.
                                    }
                                }

                                // AS Chinese name in PRC field only
                                if (!checkChiNamePassed
                                   && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                   && !string.IsNullOrEmpty(ap.CHINESE_NAME))
                                {

                                    String chineseName = mwCrmInfo.CHINESE_NAME;
                                    if (chineseName == null)
                                    {
                                        chineseName = "";
                                    }

                                    String prcChineseName = ap.CHINESE_NAME;
                                    if (prcChineseName == null)
                                    {
                                        prcChineseName = "";
                                    }

                                    //if (prcChineseName.Trim().equalsIgnoreCase(chineseName.Trim()))
                                    if (string.Equals(prcChineseName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }

                                // cant have both AS name and AS in PRC values (English name)
                                if (!string.IsNullOrEmpty(mwCrmInfo.AS_SURNAME) && string.IsNullOrEmpty(mwCrmInfo.AS_GIVEN_NAME))
                                {
                                    checkEngAsNamePassed = false;
                                }
                                else
                                {
                                    checkEngAsNamePassed = true;
                                }

                                // cant have both AS name and AS in PRC values (Chinese name)
                                if (!string.IsNullOrEmpty(mwCrmInfo.AS_CHINESE_NAME))
                                {
                                    checkChiAsNamePassed = false;
                                }
                                else
                                {
                                    checkChiAsNamePassed = true;
                                }
                            }
                            else
                            {   // PRC & AS name checking						
                                // PRC English name
                                if (!checkEngNamePassed
                                   && !string.IsNullOrEmpty(mwCrmInfo.SURNAME + " " + mwCrmInfo.GIVEN_NAME)
                                   && !string.IsNullOrEmpty(ap.ENGLISH_NAME))
                                {

                                    String surName = mwCrmInfo.SURNAME;
                                    String givenName = mwCrmInfo.GIVEN_NAME;
                                    if (surName == null)
                                    {
                                        surName = "";
                                    }
                                    if (givenName == null)
                                    {
                                        givenName = "";
                                    }

                                    String prcEnglishName = ap.ENGLISH_NAME;
                                    if (prcEnglishName == null)
                                    {
                                        prcEnglishName = "";
                                    }

                                    //if (prcEnglishName.Trim().equalsIgnoreCase((surName.Trim() + " " + givenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishName.Trim(), (surName.Trim() + " " + givenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngNamePassed = true; // English company name check passed, no need to check again.
                                    }
                                }

                                // PRC Chinese name
                                if (!checkChiNamePassed
                                   && !string.IsNullOrEmpty(mwCrmInfo.CHINESE_NAME)
                                   && !string.IsNullOrEmpty(ap.CHINESE_NAME))
                                {

                                    String chineseName = mwCrmInfo.CHINESE_NAME;
                                    if (chineseName == null)
                                    {
                                        chineseName = "";
                                    }

                                    String prcChineseName = ap.CHINESE_NAME;
                                    if (prcChineseName == null)
                                    {
                                        prcChineseName = "";
                                    }

                                    //if (prcChineseName.Trim().equalsIgnoreCase(chineseName.Trim()))
                                    if (string.Equals(prcChineseName.Trim(), chineseName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }

                                // AS English name
                                if (!checkEngAsNamePassed
                                   && !string.IsNullOrEmpty(mwCrmInfo.AS_SURNAME + " " + mwCrmInfo.AS_GIVEN_NAME)
                                   && !string.IsNullOrEmpty(ap.ENGLISH_COMPANY_NAME))
                                {

                                    String asSurName = mwCrmInfo.AS_SURNAME;
                                    String asGivenName = mwCrmInfo.AS_GIVEN_NAME;
                                    if (asSurName == null)
                                    {
                                        asSurName = "";
                                    }
                                    if (asGivenName == null)
                                    {
                                        asGivenName = "";
                                    }

                                    String prcEnglishCompanyName = ap.ENGLISH_COMPANY_NAME;
                                    if (prcEnglishCompanyName == null)
                                    {
                                        prcEnglishCompanyName = "";
                                    }

                                    //if (prcEnglishCompanyName.Trim().equalsIgnoreCase((asSurName.Trim() + " " + asGivenName.Trim()).Trim()))
                                    if (string.Equals(prcEnglishCompanyName.Trim(), (asSurName.Trim() + " " + asGivenName.Trim()).Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkEngAsNamePassed = true; // English company name check passed, no need to check again.
                                    }
                                }

                                // AS Chinese name
                                if (!checkChiAsNamePassed
                                   && !string.IsNullOrEmpty(mwCrmInfo.AS_CHINESE_NAME)
                                   && !string.IsNullOrEmpty(ap.CHINESE_COMPANY_NAME))
                                {

                                    String chineseAsName = mwCrmInfo.AS_CHINESE_NAME;
                                    if (chineseAsName == null)
                                    {
                                        chineseAsName = "";
                                    }

                                    String prcChineseCompanyName = ap.CHINESE_COMPANY_NAME;
                                    if (prcChineseCompanyName == null)
                                    {
                                        prcChineseCompanyName = "";
                                    }

                                    //if (prcChineseCompanyName.Trim().equalsIgnoreCase(chineseAsName.Trim()))
                                    if (string.Equals(prcChineseCompanyName.Trim(), chineseAsName.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        checkChiAsNamePassed = true; // Chinese name check passed, no need to check again.
                                    }
                                }
                            }
                        }

                        // set error msg
                        prc = profInfoList[0];
                        if (checkEngNamePassed && checkChiNamePassed)
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME))
                            {
                                formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_OK);
                            }
                            formChecklist.PRC_INFO_NAME_MSG = (null);
                            formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (null);
                            formChecklist.PRC_INFO_CHINESE_NAME_MSG = (null);
                        }
                        else if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME))
                        {
                            formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_INFO_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            if (!checkEngNamePassed)
                            {
                                // Display CRM English name
                                String surName = prc.SURNAME;
                                String givenName = prc.GIVEN_NAME;

                                if (surName == null && givenName == null)
                                {
                                    surName = "No input vaule";
                                    givenName = "";
                                }
                                else if (surName == null)
                                {
                                    surName = "";
                                }
                                else if (givenName == null)
                                {
                                    givenName = "";
                                }
                                String prcEngNameMsg = "<br/><font color='red'>CRM: " + surName + " " + givenName + "</font>";
                                formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (prcEngNameMsg);
                            }
                            else
                            {
                                formChecklist.PRC_INFO_ENGLISH_NAME_MSG = (null);
                            }

                            if (!checkChiNamePassed)
                            {
                                // Display CRM Chinese name
                                String chineseName = prc.CHINESE_NAME;
                                if (chineseName == null)
                                {
                                    chineseName = "No input value";
                                }
                                String prcChiNameMsg = "<br/><font color='red'>CRM: " + chineseName + "</font>";
                                formChecklist.PRC_INFO_CHINESE_NAME_MSG = (prcChiNameMsg);
                            }
                            else
                            {
                                formChecklist.PRC_INFO_CHINESE_NAME_MSG = (null);
                            }
                        }

                        // AS in PRC filed
                        if (checkEngAsNamePassed && checkChiAsNamePassed)
                        {
                            if (string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                            {
                                formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_OK);
                            }
                            formChecklist.PRC_INFO_AS_NAME_MSG = (null);
                            formChecklist.PRC_INFO_AS_ENGLISH_NAME_MSG = (null);
                            formChecklist.PRC_INFO_AS_CHINESE_NAME_MSG = (null);
                        }
                        else if (string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                        {
                            formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                            formChecklist.PRC_INFO_AS_NAME_MSG = (ProcessingConstant.NAME_ARE_NOT_IDENTICAL);
                            if (!checkEngAsNamePassed)
                            {
                                // Display AS English name for PRC field
                                String prcAsEngNameMsg = "";
                                //TBC
                                List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedPRC.CERTIFICATION_NO));
                                for (int i = 0; i < profNameList.Count(); i++)
                                {
                                    V_CRM_INFO mwCrmInfo = (V_CRM_INFO)profNameList[i];

                                    String surAsName = mwCrmInfo.AS_SURNAME;
                                    String givenAsName = mwCrmInfo.AS_GIVEN_NAME;

                                    if (surAsName == null && givenAsName == null)
                                    {
                                        surAsName = "No input vaule";
                                        givenAsName = "";
                                    }
                                    else if (surAsName == null)
                                    {
                                        surAsName = "";
                                    }
                                    else if (givenAsName == null)
                                    {
                                        givenAsName = "";
                                    }

                                    prcAsEngNameMsg += "<br/><font color='red'>CRM: " + surAsName + " " + givenAsName + "</font>";
                                }

                                formChecklist.PRC_INFO_AS_ENGLISH_NAME_MSG = ("<font color='#FF0000'>" + prcAsEngNameMsg + "</font>");
                            }

                            if (!checkChiAsNamePassed)
                            {
                                // Display AS Chinese name for PRC field
                                String prcAsChiNameMsg = "";
                                List<V_CRM_INFO> profNameList = mwCrmInfoService.findListDistinctByCertNo((appointedPRC.CERTIFICATION_NO));
                                for (int i = 0; i < profNameList.Count(); i++)
                                {
                                    V_CRM_INFO mwCrmInfo = (V_CRM_INFO)profNameList[i];
                                    String chineseAsName = mwCrmInfo.AS_CHINESE_NAME;
                                    if (chineseAsName == null)
                                    {
                                        chineseAsName = "No input value";
                                    }

                                    prcAsChiNameMsg += "<br/><font color='red'>CRM: " + chineseAsName + "</font>";
                                }

                                formChecklist.PRC_INFO_AS_CHINESE_NAME_MSG = ("<font color='#FF0000'>" + prcAsChiNameMsg + "</font>");
                            }
                        }

                        if ((mwCrmInfoOther.EXPIRY_DATE != null
                                && compareDate1 != null
                                && (DateUtil.compareDate(mwCrmInfoOther.EXPIRY_DATE.Value, compareDate1.Value) == 2))
                                || (mwCrmInfoOther.EXPIRY_DATE != null
                                && compareDate2 != null
                                && (DateUtil.compareDate(mwCrmInfoOther.EXPIRY_DATE.Value, compareDate2.Value) == 2)))
                        {
                            if ((ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.AP) || (ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.RI))
                            {
                                formChecklist.PBP_AP_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else if ((ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.RSE))
                            {
                                formChecklist.PBP_RSE_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                            else if ((ap.CERTIFICATION_NO).ToUpper().StartsWith(ProcessingConstant.RGE))
                            {
                                formChecklist.PBP_RGE_VALID = (ProcessingConstant.CHECKING_OK);
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(formChecklist.PRC_INFO_NAME)
                            || string.IsNullOrEmpty(formChecklist.PRC_INFO_AS_NAME))
                    {
                        formChecklist.PRC_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                        formChecklist.PRC_INFO_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                    formChecklist.PRC_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                    formChecklist.PRC_INFO_AS_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                    if (string.IsNullOrEmpty(formChecklist.PBP_AP_INFO_NAME))
                    {
                        formChecklist.PBP_AP_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                    formChecklist.PBP_AP_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                    if (string.IsNullOrEmpty(formChecklist.PBP_RSE_INFO_NAME))
                    {
                        formChecklist.PBP_RSE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                    formChecklist.PBP_RSE_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);

                    if (string.IsNullOrEmpty(formChecklist.PBP_RGE_INFO_NAME))
                    {
                        formChecklist.PBP_RGE_INFO_NAME = (ProcessingConstant.CHECKING_NOT_OK);
                    }
                    formChecklist.PBP_RGE_INFO_NAME_MSG = (ProcessingConstant.CERT_NOT_FOUND);
                }
            }
        }

        private void removeUncheckedItems(List<P_MW_RECORD_ITEM> mwRecordItemList, P_MW_RECORD mwRecord)
        {
            if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
            {
                for (int i = 0; i < mwRecordItemList.Count(); i++)
                {
                    P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                    if (mwRecordItem.MW_ITEM_CODE.Equals("1")
                            || mwRecordItem.MW_ITEM_CODE.Equals("2")
                            || mwRecordItem.MW_ITEM_CODE.Equals("3")
                            || mwRecordItem.MW_ITEM_CODE.Equals("4"))
                    {
                        mwRecordItemList.Remove(mwRecordItem);
                        i--;
                    }
                }
            }
        }

        private void prefillCapacityOfPRC(VerificaionFormModel model)
        {
            try
            {
                if (VER_FORM_PRC.Equals(model.CheckPage) || true)
                {
                    //			formChecklist.PRC_CAP = (ProcessingConstant.CHECKING_NOT_OK);			

                    if (model.AppointedPRC != null && model.AppointedPRC.CERTIFICATION_NO != null)
                    {
                        //List<MwCrmInfo> profInfoList = mwCrmInfoService.findListByCertNo(stringUtil.getDisplay(appointedPRC.CERTIFICATION_NO));
                        //find Active MW_CRM_INFO
                        List<V_CRM_INFO> profInfoList = mwCrmInfoService.findActiveListByCertNo(stringUtil.getDisplay(model.AppointedPRC.CERTIFICATION_NO));
                        if (profInfoList.Count() > 0)
                        {
                            if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.GBC))
                            {
                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP))
                                {
                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP = (ProcessingConstant.CHECKING_OK);
                                }
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCD))
                            {
                                prefillCapacityByRole(ProcessingConstant.SCD, model);
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCSF))
                            {
                                prefillCapacityByRole(ProcessingConstant.SCSF, model);
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCF))
                            {
                                prefillCapacityByRole(ProcessingConstant.SCF, model);
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCGI))
                            {
                                prefillCapacityByRole(ProcessingConstant.SCGI, model);
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                            {
                                prefillCapacityOfPRCByAs(model.AppointedPRC.ENGLISH_COMPANY_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, model);
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWC)
                                  || model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCP))
                            {
                                prefillCapacityOfPRCByAs(model.AppointedPRC.ENGLISH_COMPANY_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, model);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //log.fatalError("----------Error---------- ");
                //log.fatalError("VerificationAction.prefillCapacityOfPRC()");
                //log.fatalError(e.getMessage());
                //log.fatalError("----------Error---------- ");
            }
        }

        private void prefillCapacityOfPRCByAs(String englishName, String chineseName, VerificaionFormModel model)
        {
            if (model.AppointedPRC != null)
            {
                //			formChecklist.PRC_CAP = (ProcessingConstant.CHECKING_NOT_OK);

                V_CRM_INFO profInfo = null;
                //List<V_CRM_INFO> profInfoList = mwCrmInfoService.findListByCertNo(stringUtil.getDisplay(appointedPRC.CERTIFICATION_NO));
                //find Active MW_CRM_INFO
                List<V_CRM_INFO> profInfoList = mwCrmInfoService.findActiveListByCertNo(stringUtil.getDisplay(model.AppointedPRC.CERTIFICATION_NO));

                if (profInfoList.Count() > 0)
                {
                    for (int i = 0; i < profInfoList.Count(); i++)
                    {
                        profInfo = profInfoList[i];
                        if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                        {
                            //if ((string.IsNullOrWhiteSpace(englishName)
                            //        && stringUtil.getDisplay(profInfo.CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(chineseName)))
                            //|| (string.IsNullOrWhiteSpace(chineseName)
                            //                && (stringUtil.getDisplay(profInfo.SURNAME) + " " + stringUtil.getDisplay(profInfo.GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(englishName)))
                            //|| (!string.IsNullOrWhiteSpace(chineseName) && !string.IsNullOrWhiteSpace(englishName)
                            //        && stringUtil.getDisplay(profInfo.CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(chineseName))
                            //        && (stringUtil.getDisplay(profInfo.SURNAME) + " " + stringUtil.getDisplay(profInfo.GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(englishName))))
                            if ((string.IsNullOrWhiteSpace(englishName) && string.Equals(profInfo.CHINESE_NAME, chineseName, StringComparison.CurrentCultureIgnoreCase))
                                    || (string.IsNullOrWhiteSpace(chineseName) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, englishName, StringComparison.CurrentCultureIgnoreCase))
                                    || (!string.IsNullOrWhiteSpace(chineseName) && !string.IsNullOrWhiteSpace(englishName) && string.Equals(profInfo.CHINESE_NAME, chineseName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, englishName, StringComparison.CurrentCultureIgnoreCase))
                                    )
                                break;
                            else profInfo = null;
                        }
                        else
                        {
                            //if ((string.IsNullOrWhiteSpace(englishName)
                            //        && stringUtil.getDisplay(profInfo.AS_CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(chineseName)))
                            // || (string.IsNullOrWhiteSpace(chineseName)
                            //         && (stringUtil.getDisplay(profInfo.AS_SURNAME) + " " + stringUtil.getDisplay(profInfo.AS_GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(englishName)))
                            // || (!string.IsNullOrWhiteSpace(englishName) && !string.IsNullOrWhiteSpace(chineseName)
                            //         && stringUtil.getDisplay(profInfo.AS_CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(chineseName))
                            //         && (stringUtil.getDisplay(profInfo.AS_SURNAME) + " " + stringUtil.getDisplay(profInfo.AS_GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(englishName))))

                            if ((string.IsNullOrWhiteSpace(englishName) && string.Equals(profInfo.AS_CHINESE_NAME, chineseName, StringComparison.CurrentCultureIgnoreCase))
                                || (string.IsNullOrWhiteSpace(chineseName) && string.Equals(profInfo.AS_SURNAME + " " + profInfo.AS_GIVEN_NAME, englishName, StringComparison.CurrentCultureIgnoreCase))
                                || (!string.IsNullOrWhiteSpace(chineseName) && !string.IsNullOrWhiteSpace(englishName) && string.Equals(profInfo.AS_CHINESE_NAME, chineseName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.AS_SURNAME + " " + profInfo.AS_GIVEN_NAME, englishName, StringComparison.CurrentCultureIgnoreCase))
                                )
                                break;
                            else profInfo = null;
                        }
                    }

                    List<String> itemList = new List<String>();
                    if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                    {
                        itemList = mwCrmInfoService.getItemNosByCertNo(model.AppointedPRC.CERTIFICATION_NO);
                    }
                    else
                    {
                        for (int i = 0; i < profInfoList.Count(); i++)
                        {
                            V_CRM_INFO info = (V_CRM_INFO)profInfoList[i];
                            itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                        }
                    }
                    if (itemList.Count() > 0)
                    {
                        List<P_MW_RECORD_ITEM> mwRecordItemList = mwRecordItemService.getMwRecordItemByMwRecordOrdering(model.P_MW_RECORD, false);
                        removeUncheckedItems(mwRecordItemList, model.P_MW_RECORD);
                        int notContainItem = 0;
                        for (int i = 0; i < mwRecordItemList.Count(); i++)
                        {
                            P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                            bool contain = false;
                            for (int j = 0; j < itemList.Count(); j++)
                            {
                                String item = (String)itemList[j];
                                if (item.Trim().Equals(mwRecordItem.MW_ITEM_CODE))
                                {
                                    contain = true;
                                    break;
                                }
                            }
                            if (!contain)
                                notContainItem++;

                        }
                        if (mwRecordItemList.Count() > 0)
                        {
                            if (notContainItem > 0)
                            {
                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP))
                                {
                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP))
                                {
                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP = (ProcessingConstant.CHECKING_OK);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void prefillValidityOfAs(VerificaionFormModel model)
        {
            try
            {
                if (VER_FORM_PRC.Equals(model.CheckPage) || true)
                {
                    List<P_MW_RECORD_ITEM> mwRecordItemList = mwRecordItemService.getMwRecordItemByMwRecordOrdering(model.P_MW_RECORD, false);
                    removeUncheckedItems(mwRecordItemList, model.P_MW_RECORD);
                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_NAME = (ProcessingConstant.CHECKING_NOT_OK);

                    //			formChecklist.PRC_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                    //			formChecklist.PRC_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);

                    if (model.AppointedPRC != null)
                    {
                        V_CRM_INFO profInfo = null;
                        //List<MwCrmInfo> profInfoList = mwCrmInfoService.findListByCertNo(stringUtil.getDisplay(appointedPRC.CERTIFICATION_NO));
                        //find Active MW_CRM_INFO
                        List<V_CRM_INFO> profInfoList = mwCrmInfoService.findActiveListByCertNo(stringUtil.getDisplay(model.AppointedPRC.CERTIFICATION_NO));
                        if (profInfoList.Count() > 0)
                        {
                            for (int i = 0; i < profInfoList.Count(); i++)
                            {
                                profInfo = (V_CRM_INFO)profInfoList[i];
                                if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                                {
                                    //if ((string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME)
                                    //                && stringUtil.getDisplay(profInfo.CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.CHINESE_COMPANY_NAME)))
                                    //        || (string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME)
                                    //                && (stringUtil.getDisplay(profInfo.SURNAME) + " " + stringUtil.getDisplay(profInfo.GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.ENGLISH_COMPANY_NAME)))
                                    //        || (!string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && !string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME)
                                    //                && stringUtil.getDisplay(profInfo.CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.CHINESE_COMPANY_NAME))
                                    //                && (stringUtil.getDisplay(profInfo.SURNAME) + " " + stringUtil.getDisplay(profInfo.GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.ENGLISH_COMPANY_NAME))))
                                    if ((string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && string.Equals(profInfo.CHINESE_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, model.AppointedPRC.ENGLISH_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (!string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && !string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME) && string.Equals(profInfo.CHINESE_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.SURNAME + " " + profInfo.GIVEN_NAME, model.AppointedPRC.ENGLISH_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         )
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        profInfo = null;
                                    }
                                }
                                else
                                {
                                    //if ((string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME)
                                    //                && stringUtil.getDisplay(profInfo.AS_CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.CHINESE_COMPANY_NAME)))
                                    //        || (string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME)
                                    //                && (stringUtil.getDisplay(profInfo.AS_SURNAME) + " " + stringUtil.getDisplay(profInfo.AS_GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.ENGLISH_COMPANY_NAME)))
                                    //        || (!string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && !string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME)
                                    //                && stringUtil.getDisplay(profInfo.AS_CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.CHINESE_COMPANY_NAME))
                                    //                && (stringUtil.getDisplay(profInfo.AS_SURNAME) + " " + stringUtil.getDisplay(profInfo.AS_GIVEN_NAME)).equalsIgnoreCase(stringUtil.getDisplay(model.AppointedPRC.ENGLISH_COMPANY_NAME))))
                                    if ((string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && string.Equals(profInfo.AS_CHINESE_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME) && string.Equals(profInfo.AS_SURNAME + " " + profInfo.AS_GIVEN_NAME, model.AppointedPRC.ENGLISH_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         || (!string.IsNullOrWhiteSpace(model.AppointedPRC.ENGLISH_COMPANY_NAME) && !string.IsNullOrWhiteSpace(model.AppointedPRC.CHINESE_COMPANY_NAME) && string.Equals(profInfo.AS_CHINESE_NAME, model.AppointedPRC.CHINESE_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase) && string.Equals(profInfo.AS_SURNAME + " " + profInfo.AS_GIVEN_NAME, model.AppointedPRC.ENGLISH_COMPANY_NAME, StringComparison.CurrentCultureIgnoreCase))
                                         )
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        profInfo = null;
                                    }
                                }
                            } // for

                            if (profInfo != null)
                            {
                                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_NAME = (ProcessingConstant.CHECKING_OK);
                                if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.GBC))
                                {
                                    if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID))
                                    {
                                        model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID = (ProcessingConstant.CHECKING_OK);
                                    }
                                }
                                else
                                {
                                    List<String> itemList = new List<String>();
                                    if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                                    {
                                        itemList = mwCrmInfoService.getItemNosByCertNo(model.AppointedPRC.CERTIFICATION_NO);
                                    }
                                    else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWC)
                                          || model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCP))
                                    {
                                        List<V_CRM_INFO> profInfos = mwCrmInfoService.findListByCertNoEnglishNameChineseName(profInfo.CERTIFICATION_NO, profInfo.AS_SURNAME, profInfo.AS_GIVEN_NAME, profInfo.AS_CHINESE_NAME);
                                        for (int i = 0; i < profInfos.Count(); i++)
                                        {
                                            V_CRM_INFO info = (V_CRM_INFO)profInfos[i];
                                            itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                                        }
                                    }
                                    else
                                    {
                                        String contractorRole = "";
                                        if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCD))
                                        {
                                            contractorRole = ProcessingConstant.SCD;
                                        }
                                        else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCSF))
                                        {
                                            contractorRole = ProcessingConstant.SCSF;
                                        }
                                        else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCF))
                                        {
                                            contractorRole = ProcessingConstant.SCF;
                                        }
                                        else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCGI))
                                        {
                                            contractorRole = ProcessingConstant.SCGI;
                                        }
                                        itemList = sCapacityOfPRCService.getItemListByRole(contractorRole);
                                    }

                                    if (itemList.Count() > 0)
                                    {
                                        int notContainItem = 0;
                                        for (int i = 0; i < mwRecordItemList.Count(); i++)
                                        {
                                            P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                                            bool contain = false;
                                            for (int j = 0; j < itemList.Count(); j++)
                                            {
                                                String item = (String)itemList[j];
                                                if (item.Trim().Equals(mwRecordItem.MW_ITEM_CODE))
                                                {
                                                    contain = true;
                                                    break;
                                                }
                                            }
                                            if (!contain)
                                            {
                                                notContainItem++;
                                            }
                                        }
                                        if (mwRecordItemList.Count() > 0)
                                        {
                                            if (notContainItem > 0)
                                            {
                                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID))
                                                {
                                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                                }
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID))
                                                {
                                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID = (ProcessingConstant.CHECKING_OK);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW))
                            {
                                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_INFO_AS_NAME.Equals(ProcessingConstant.CHECKING_NOT_OK))
                                {
                                    String otherAs = "";
                                    for (int i = 0; i < profInfoList.Count(); i++)
                                    {
                                        V_CRM_INFO otherInfo = (V_CRM_INFO)profInfoList[i];
                                        List<String> itemList = new List<String>();
                                        int notContainItem = 0;
                                        if (!model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.GBC))
                                        {
                                            if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWC)
                                                    || model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCP))
                                            {
                                                List<V_CRM_INFO> profInfos = mwCrmInfoService.findListByCertNoEnglishNameChineseName(otherInfo.CERTIFICATION_NO, otherInfo.AS_SURNAME, otherInfo.AS_GIVEN_NAME, otherInfo.AS_CHINESE_NAME);
                                                for (int j = 0; j < profInfos.Count(); j++)
                                                {
                                                    V_CRM_INFO info = (V_CRM_INFO)profInfos[j];
                                                    itemList.AddRange(sMwItemService.getItemNosByClassAndType(info.CLASS_CODE, info.TYPE_CODE));
                                                }
                                            }
                                            else
                                            {
                                                String contractorRole = "";
                                                if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCD))
                                                {
                                                    contractorRole = ProcessingConstant.SCD;
                                                }
                                                else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCSF))
                                                {
                                                    contractorRole = ProcessingConstant.SCSF;
                                                }
                                                else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCF))
                                                {
                                                    contractorRole = ProcessingConstant.SCF;
                                                }
                                                else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.SCGI))
                                                {
                                                    contractorRole = ProcessingConstant.SCGI;
                                                }
                                                itemList = sCapacityOfPRCService.getItemListByRole(contractorRole);
                                            }

                                            if (itemList.Count() > 0)
                                            {
                                                for (int k = 0; k < mwRecordItemList.Count(); k++)
                                                {
                                                    P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[k];
                                                    bool contain = false;
                                                    for (int j = 0; j < itemList.Count(); j++)
                                                    {
                                                        String item = (String)itemList[j];
                                                        if (item.Trim().Equals(mwRecordItem.MW_ITEM_CODE))
                                                        {
                                                            contain = true;
                                                            break;
                                                        }
                                                    }
                                                    if (!contain)
                                                    {
                                                        notContainItem++;
                                                    }
                                                }
                                            }
                                        }
                                        if (mwRecordItemList.Count() > 0)
                                        {
                                            if (notContainItem == 0)
                                            {
                                                if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER))
                                                {
                                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER = (ProcessingConstant.CHECKING_OK);
                                                }
                                                otherAs += otherInfo.AS_SURNAME + " " + otherInfo.AS_GIVEN_NAME + " <img style='cursor:pointer;' src='images/ico_view.gif' onclick=openAsFile('" + otherInfo.COMP_UUID + "') alt='ico' />" + "<br/>";
                                                for (int k = 0; k < profInfoList.Count(); k++)
                                                {
                                                    V_CRM_INFO mci = (V_CRM_INFO)profInfoList[k];
                                                    //if (stringUtil.getDisplay(mci.AS_SURNAME).equalsIgnoreCase(stringUtil.getDisplay(otherInfo.AS_SURNAME))
                                                    //        && stringUtil.getDisplay(mci.AS_GIVEN_NAME).equalsIgnoreCase(stringUtil.getDisplay(otherInfo.AS_GIVEN_NAME))
                                                    //        && stringUtil.getDisplay(mci.AS_CHINESE_NAME).equalsIgnoreCase(stringUtil.getDisplay(otherInfo.AS_CHINESE_NAME)))
                                                    if (string.Equals(mci.AS_SURNAME, otherInfo.AS_SURNAME, StringComparison.CurrentCultureIgnoreCase)
                                                          && string.Equals(mci.AS_GIVEN_NAME, otherInfo.AS_GIVEN_NAME, StringComparison.CurrentCultureIgnoreCase)
                                                          && string.Equals(mci.AS_CHINESE_NAME, otherInfo.AS_CHINESE_NAME, StringComparison.CurrentCultureIgnoreCase))
                                                    {
                                                        profInfoList.Remove(mci);
                                                        k--;
                                                    }
                                                }
                                                i--;
                                            }
                                        }
                                    }
                                    model.P_MW_RECORD_FORM_CHECKLIST.PRC_OTHER_AS_LIST = (otherAs);
                                }
                            }
                            else if (model.AppointedPRC.CERTIFICATION_NO.StartsWith(ProcessingConstant.MWCW) && model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
                            {
                                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID = (ProcessingConstant.CHECKING_NOT_OK);
                                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID_RMK = (ProcessingConstant.NA);
                                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER = (ProcessingConstant.CHECKING_NOT_OK);
                                model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER_RMK = (ProcessingConstant.NA);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //log.fatalError("----------Error---------- ");
                //log.fatalError("VerificationAction.prefillValidityOfAs()");
                //log.fatalError(e.getMessage());
                //log.fatalError("----------Error---------- ");
            }
        }

        private void prefillCapacityByRole(String contractorRole, VerificaionFormModel model)
        {
            int notContainItem = 0;
            List<String> itemList = sCapacityOfPRCService.getItemListByRole(contractorRole);

            //		formChecklist.PRC_CAP = (ProcessingConstant.CHECKING_NOT_OK);

            if (itemList != null)
            {
                List<P_MW_RECORD_ITEM> mwRecordItemList = mwRecordItemService.getMwRecordItemByMwRecordOrdering(model.P_MW_RECORD, false);
                removeUncheckedItems(mwRecordItemList, model.P_MW_RECORD);
                for (int i = 0; i < mwRecordItemList.Count(); i++)
                {
                    P_MW_RECORD_ITEM mwRecordItem = mwRecordItemList[i];
                    bool contain = false;
                    for (int j = 0; j < itemList.Count(); j++)
                    {
                        String item = (String)itemList[j];
                        if (item.Equals(mwRecordItem.MW_ITEM_CODE))
                        {
                            contain = true;
                            break;
                        }
                    }
                    if (!contain)
                    {
                        notContainItem++;
                    }
                }
                if (mwRecordItemList.Count() > 0)
                {
                    if (notContainItem > 0)
                    {
                        if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP))
                        {
                            model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP = (ProcessingConstant.CHECKING_NOT_OK);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP))
                        {
                            model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP = (ProcessingConstant.CHECKING_OK);
                        }
                    }
                }
            }
        }




    }


}