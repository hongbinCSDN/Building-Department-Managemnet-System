using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingTDL_AcknowledgementBLService
    {
        private ProcessingTdlDAOService TdlDaoService;
        protected ProcessingTdlDAOService DA
        {
            get { return TdlDaoService ?? (TdlDaoService = new ProcessingTdlDAOService()); }
        }

        private DataTransferToBravoService _bravoService;
        protected DataTransferToBravoService bravoService
        {
            get { return _bravoService ?? (_bravoService = new DataTransferToBravoService()); }
        }

        private P_MW_RECORD_REFERRED_TO_LSS_EBD_DAOService _referredToLssEbdDaoService;
        protected P_MW_RECORD_REFERRED_TO_LSS_EBD_DAOService referredToLssEbdDaoService
        {
            get { return _referredToLssEbdDaoService ?? (_referredToLssEbdDaoService = new P_MW_RECORD_REFERRED_TO_LSS_EBD_DAOService()); }
        }

        private P_MW_RECORD_WL_FOLLOW_UP_DAOService _wlFollowUpDaoService;
        protected P_MW_RECORD_WL_FOLLOW_UP_DAOService wlFollowUpDaoService
        {
            get { return _wlFollowUpDaoService ?? (_wlFollowUpDaoService = new P_MW_RECORD_WL_FOLLOW_UP_DAOService()); }
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

        public void GetAcknowledgement(AcknowledgementModel model)
        {
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

            }
            else if (model.TaskUserID != Utility.SessionUtil.LoginPost.UUID)
            {
                model.IsReadonly = true;
                model.IsSPO = false;
            }

            //Get P_MW_RECORD
            model.P_MW_RECORD = DA.GetP_MW_RECORD(model.R_UUID);

            //Get P_MW_VERIFICATION
            model.P_MW_VERIFICATION = mwVerificationService.GetP_MW_VERIFICATIONByUuid(model.V_UUID);

            if (model.P_MW_VERIFICATION != null)
            {
                model.HandlingUnit = model.P_MW_VERIFICATION.HANDLING_UNIT;
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
                model.P_MW_RECORD_PSAC.RECORD_ID = model.P_MW_RECORD.UUID;
                model.P_MW_RECORD_PSAC.HANDLING_UNIT = model.HandlingUnit;
                DA.AddP_MW_RECORD_PSAC(model.P_MW_RECORD_PSAC);
            }

            //GetP_MW_RECORD_SAC --SAC
            model.P_MW_RECORD_SAC = DA.GetP_MW_RECORD_SAC(model.P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_SAC, model.HandlingUnit);
            if (model.P_MW_RECORD_SAC == null)
            {
                model.P_MW_RECORD_SAC = new P_MW_RECORD_SAC();
                model.P_MW_RECORD_SAC.SYS_PAGE_CODE = ProcessingConstant.PAGE_CODE_SAC;
                model.P_MW_RECORD_SAC.HANDLING_UNIT = model.HandlingUnit;
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

            //GetP_MW_RECORD_SAC --WL
            model.P_MW_RECORD_SAC_WL = DA.GetP_MW_RECORD_SAC(model.P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_WL, model.HandlingUnit);
            if (model.P_MW_RECORD_SAC_WL == null)
            {
                model.P_MW_RECORD_SAC_WL = new P_MW_RECORD_SAC();
                model.P_MW_RECORD_SAC_WL.SYS_PAGE_CODE = ProcessingConstant.PAGE_CODE_WL;
                model.P_MW_RECORD_SAC_WL.HANDLING_UNIT = model.HandlingUnit;
            }
            else
            {
                model.IsSacWL = true;
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

            //Get P_MW_RECORD_FORM_CHECKLIST
            model.P_MW_RECORD_FORM_CHECKLIST = DA.GetP_MW_RECORD_FORM_CHECKLIST(model.P_MW_RECORD.UUID, model.V_UUID, model.HandlingUnit);
            if (model.P_MW_RECORD_FORM_CHECKLIST == null)
            {
                model.P_MW_RECORD_FORM_CHECKLIST = new P_MW_RECORD_FORM_CHECKLIST();
                model.P_MW_RECORD_FORM_CHECKLIST.HANDLING_UNIT = model.HandlingUnit;
            }

            model.P_MW_RECORD_FORM_CHECKLIST_PO = DA.GetP_MW_RECORD_FORM_CHECKLIST_PO(model.P_MW_RECORD_FORM_CHECKLIST.UUID, model.HandlingUnit);
            if (model.P_MW_RECORD_FORM_CHECKLIST_PO == null)
            {
                model.P_MW_RECORD_FORM_CHECKLIST_PO = new P_MW_RECORD_FORM_CHECKLIST_PO();
                model.P_MW_RECORD_FORM_CHECKLIST_PO.HANDLING_UNIT = model.HandlingUnit;
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

            //Get OffenseList
            model.OffenseList = DA.GetSelectListItem();

            //Get AL Follow Up
            model.P_MW_RECORD_AL_FOLLOW_UP = DA.GetP_MW_RECORD_AL_FOLLOW_UPByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);

            if (model.P_MW_RECORD_AL_FOLLOW_UP == null)
            {
                model.P_MW_RECORD_AL_FOLLOW_UP = new P_MW_RECORD_AL_FOLLOW_UP();
                model.P_MW_RECORD_AL_FOLLOW_UP.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                model.P_MW_RECORD_AL_FOLLOW_UP.HANDLING_UNIT = model.HandlingUnit;
            }

            model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs = DA.GetP_MW_RECORD_AL_FOLLOW_UP_OFFENCESByMasterID(model.P_MW_RECORD_AL_FOLLOW_UP.UUID);

            if (model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs.Count() < 3)
            {
                for (int i = model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs.Count(); i < 3; i++)
                {
                    model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs.Add(new P_MW_RECORD_AL_FOLLOW_UP_OFFENCES());
                }
            }

            model.P_MW_RECORD_WL_FOLLOW_UP = wlFollowUpDaoService.GetP_MW_RECORD_WL_FOLLOW_UPByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);

            if (model.P_MW_RECORD_WL_FOLLOW_UP == null)
            {
                model.P_MW_RECORD_WL_FOLLOW_UP = new P_MW_RECORD_WL_FOLLOW_UP();
                model.P_MW_RECORD_WL_FOLLOW_UP.HANDLING_UNIT = model.HandlingUnit;
            }

            model.P_MW_RECORD_REFERRED_TO_LSS_EBD = referredToLssEbdDaoService.GetP_MW_RECORD_REFERRED_TO_LSS_EBDByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);

            if (model.P_MW_RECORD_REFERRED_TO_LSS_EBD == null)
            {
                model.P_MW_RECORD_REFERRED_TO_LSS_EBD = new P_MW_RECORD_REFERRED_TO_LSS_EBD();
                model.P_MW_RECORD_REFERRED_TO_LSS_EBD.HANDLING_UNIT = model.HandlingUnit;
            }

            //Get P_MW_RECORD_WL_FOLLOW_UP

            //Get P_MW_RECORD_REFERRED_TO_LSS_EBD

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

            model.AppointedAP = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedRSE = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedRGE = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedPRC = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedOther0 = new P_MW_APPOINTED_PROFESSIONAL();
            model.AppointedOther1 = new P_MW_APPOINTED_PROFESSIONAL();

            GetPbpPrcListBySecondEntry(model);

            SpecifiedChecklistSummary(model);
        }

        public void GetPbpPrcListBySecondEntry(AcknowledgementModel model)
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

        public JsonResult SaveAcknowledgement(AcknowledgementModel model)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Save P_MW_SUMMARY_MW_ITEM_CHECKLIST
                        DA.SaveP_MW_SUMMARY_MW_ITEM_CHECKLIST_ACK(model.P_MW_SUMMARY_MW_ITEM_CHECKLIST, db);

                        //Save P_MW_RECORD_PSAC
                        if (model.P_MW_RECORD_PSAC != null)
                        {
                            model.P_MW_RECORD_PSAC.RECORD_ID = model.P_MW_RECORD.UUID;
                            model.P_MW_RECORD_PSAC.HANDLING_UNIT = model.HandlingUnit;
                            P_MW_RECORD_PSAC psacRecord = DA.GetP_MW_RECORD_PSAC(model.P_MW_RECORD.UUID, model.HandlingUnit);
                            if (psacRecord != null)
                            {
                                model.P_MW_RECORD_PSAC.UUID = psacRecord.UUID;
                            }
                            DA.SaveP_MW_RECORD_PSAC(model.P_MW_RECORD_PSAC, db);
                        }


                        //Save P_MW_RECORD_SAC
                        if (model.P_MW_RECORD_SAC != null)
                        {
                            model.P_MW_RECORD_SAC.RECORD_ID = model.P_MW_RECORD.UUID;
                            model.P_MW_RECORD_SAC.HANDLING_UNIT = model.HandlingUnit;
                            P_MW_RECORD_SAC sacRecord = DA.GetP_MW_RECORD_SAC(model.P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_SAC, model.HandlingUnit);
                            if (sacRecord != null)
                            {
                                model.P_MW_RECORD_SAC.UUID = sacRecord.UUID;
                            }
                            DA.SaveP_MW_RECORD_SAC(model.P_MW_RECORD_SAC, db);
                        }


                        //Save P_MW_RECORD_SAC_WL
                        if (model.IsSacWL && model.P_MW_RECORD_SAC_WL != null)
                        {
                            model.P_MW_RECORD_SAC_WL.RECORD_ID = model.P_MW_RECORD.UUID;
                            model.P_MW_RECORD_SAC_WL.HANDLING_UNIT = model.HandlingUnit;
                            P_MW_RECORD_SAC sacRecordWl = DA.GetP_MW_RECORD_SAC(model.P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_WL, model.HandlingUnit);
                            if (sacRecordWl != null)
                            {
                                model.P_MW_RECORD_SAC_WL.UUID = sacRecordWl.UUID;
                            }
                            DA.SaveP_MW_RECORD_SAC(model.P_MW_RECORD_SAC_WL, db);
                        }


                        //Save P_MW_RECORD_AL_FOLLOW_UP
                        model.P_MW_RECORD_AL_FOLLOW_UP.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                        model.P_MW_RECORD_AL_FOLLOW_UP.HANDLING_UNIT = model.HandlingUnit;
                        P_MW_RECORD_AL_FOLLOW_UP alRecord = DA.GetP_MW_RECORD_AL_FOLLOW_UPByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);
                        if (alRecord != null)
                        {
                            model.P_MW_RECORD_AL_FOLLOW_UP.UUID = alRecord.UUID;
                        }
                        DA.SaveP_MW_RECORD_AL_FOLLOW_UP(model.P_MW_RECORD_AL_FOLLOW_UP, db);


                        //Save P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs
                        if (model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs != null)
                        {
                            model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs.ForEach(i => i.MASTER_ID = model.P_MW_RECORD_AL_FOLLOW_UP.UUID);

                            DA.SaveP_MW_RECORD_AL_FOLLOW_UP_OFFENCESs(model.P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs, model.P_MW_RECORD_AL_FOLLOW_UP.UUID, db);
                        }

                        //Save P_MW_RECORD_WL_FOLLOW_UP
                        model.P_MW_RECORD_WL_FOLLOW_UP.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                        model.P_MW_RECORD_WL_FOLLOW_UP.HANDLING_UNIT = model.HandlingUnit;
                        P_MW_RECORD_WL_FOLLOW_UP wlRecord = wlFollowUpDaoService.GetP_MW_RECORD_WL_FOLLOW_UPByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);
                        if (wlRecord != null)
                        {
                            model.P_MW_RECORD_WL_FOLLOW_UP.UUID = wlRecord.UUID;
                        }
                        DA.SaveP_MW_RECORD_WL_FOLLOW_UP(model.P_MW_RECORD_WL_FOLLOW_UP, db);

                        //Save P_MW_RECORD_REFERRED_TO_LSS_EBD
                        model.P_MW_RECORD_REFERRED_TO_LSS_EBD.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                        model.P_MW_RECORD_REFERRED_TO_LSS_EBD.HANDLING_UNIT = model.HandlingUnit;
                        P_MW_RECORD_REFERRED_TO_LSS_EBD lssEbdRecord = referredToLssEbdDaoService.GetP_MW_RECORD_REFERRED_TO_LSS_EBDByRecordID(model.P_MW_RECORD.UUID, model.HandlingUnit);
                        if (lssEbdRecord != null)
                        {
                            model.P_MW_RECORD_REFERRED_TO_LSS_EBD.UUID = lssEbdRecord.UUID;
                        }
                        DA.SaveP_MW_RECORD_REFERRED_TO_LSS_EBD(model.P_MW_RECORD_REFERRED_TO_LSS_EBD, db);


                        //Save P_MW_RECORD_FORM_CHECKLIST
                        DA.SaveP_MW_RECORD_FORM_CHECKLIST_ACK(model.P_MW_RECORD_FORM_CHECKLIST, model.P_MW_RECORD.UUID, model.V_UUID, db);

                        //Save P_MW_RECORD_FORM_CHECKLIST_PO
                        model.P_MW_RECORD_FORM_CHECKLIST_PO.MW_RECORD_FORM_CHECKLIST_ID = model.P_MW_RECORD_FORM_CHECKLIST.UUID;
                        model.P_MW_RECORD_FORM_CHECKLIST_PO.HANDLING_UNIT = model.HandlingUnit;
                        DA.SaveP_MW_RECORD_FORM_CHECKLIST_PO(model.P_MW_RECORD_FORM_CHECKLIST_PO, db);

                        //Save record item checklist item
                        if (model.RecordItemCheckListItems != null)
                        {
                            List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> p_MW_RECORD_ITEM_CHECKLIST_ITEMs = new List<P_MW_RECORD_ITEM_CHECKLIST_ITEM>();
                            foreach (var item in model.RecordItemCheckListItems)
                            {
                                p_MW_RECORD_ITEM_CHECKLIST_ITEMs.Add(new P_MW_RECORD_ITEM_CHECKLIST_ITEM()
                                {
                                    UUID = item.UUID,
                                    PO_AGREEMENT = item.PO_AGREEMENT,
                                    PO_REMARK = item.PO_REMARK
                                });
                            }
                            DA.SaveP_MW_RECORD_ITEM_CHECKLIST_ITEM_PO(p_MW_RECORD_ITEM_CHECKLIST_ITEMs, db);
                        }

                        //model.P_MW_FORM_09s
                        if (model.P_MW_FORM_09s != null)
                        {
                            DA.SaveP_MW_FORM_09s(model.P_MW_FORM_09s, db);
                        }

                        if (model.IsSubmit)
                        {
                            DA.SubmitAcknowledgement_PO(model.P_MW_REFERENCE_NO.UUID, model.P_MW_REFERENCE_NO.REFERENCE_NO, model.P_MW_DSN.DSN, db, model.V_UUID);
                        }

                        db.SaveChanges();
                        tran.Commit();
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;

                        AuditLogService.logDebug(ex);
                    }
                }
            }

            return new JsonResult { Data = serviceResult };
        }

        public ServiceResult SubmitAcknowledgement(AcknowledgementModel model)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Start modify by dive 20191021
                        //P_MW_VERIFICATION v = db.P_MW_VERIFICATION.Where(O => O.MW_RECORD_ID == model.P_MW_RECORD.UUID && O.STATUS_CODE == "MW_ACKN_STATUS_OPEN").FirstOrDefault();

                        P_MW_VERIFICATION v = db.P_MW_VERIFICATION.Where(O => O.UUID == model.V_UUID && O.STATUS_CODE == "MW_ACKN_STATUS_OPEN").Include(o => o.P_MW_RECORD).FirstOrDefault();
                        //P_MW_VERIFICATION v = db.P_MW_VERIFICATION.Where(O => O.MW_RECORD_ID == model.P_MW_RECORD.UUID && O.STATUS_CODE == "MW_ACKN_STATUS_OPEN").FirstOrDefault();
                        if (v == null) throw new Exception("Verification record not valid.");
                        v.STATUS_CODE = ProcessingConstant.MW_ACKN_STATUS_COMPLETE;
                        string MW_DSN = v.P_MW_RECORD.MW_DSN;
                        //P_WF_TASK wfTask = db.P_WF_TASK.Where(o => o.STATUS == "WF_STATUS_OPEN").Where(o => o.P_WF_INFO.RECORD_ID == MW_DSN).FirstOrDefault();
                        //if (wfTask == null) throw new Exception("Flow not found.");

                        string taskCode = (ProcessingConstant.HANDLING_UNIT_SMM.Equals(v.HANDLING_UNIT) ? ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_SPO_SMM : ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_SPO);

                        ProcessingWorkFlowManagementService.Instance.ToNext(db
                           , ProcessingWorkFlowManagementService.WF_TYPE_SUBMISSION
                           // , finalMwRecord.UUID
                           , MW_DSN
                           //, wfTask.TASK_CODE
                           , taskCode
                           , SessionUtil.LoginPost.UUID);

                        //Get HANDLING_UNIT
                        string handlingUnit = v.HANDLING_UNIT;

                        if (ProcessingConstant.HANDLING_UNIT_PEM.Equals(handlingUnit))
                        {
                            //Get SMM
                            P_MW_VERIFICATION vSMM = db.P_MW_VERIFICATION.Where(O => O.MW_RECORD_ID == model.P_MW_RECORD.UUID && O.HANDLING_UNIT == ProcessingConstant.HANDLING_UNIT_SMM).FirstOrDefault();

                            if (vSMM != null && !ProcessingConstant.MW_ACKN_STATUS_COMPLETE.Equals(vSMM.STATUS_CODE))
                            {
                                //Rollback
                                tran.Rollback();
                                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                                serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "ALERT", new List<string>() { "Fail to submit. The SPO of SCU have not submit the submission." } } };
                                return serviceResult;
                            }
                        }

                        //End modify by dive 20191021

                        //Get P_MW_SUMMARY_MW_ITEM_CHECKLIST

                        P_MW_SUMMARY_MW_ITEM_CHECKLIST smic = DA.GetP_MW_SUMMARY_MW_ITEM_CHECKLIST(model.P_MW_RECORD.UUID, model.HandlingUnit);

                        //Start modify by dive 20191106 Comment UpdateFinalRecordByRefNo function
                        //bravoService.UpdateFinalRecordByRefNo(db, model.P_MW_RECORD.S_FORM_TYPE_CODE, model.P_MW_REFERENCE_NO.REFERENCE_NO, smic.TRANSFER_RRM);
                        //End modify by dive 20191106 

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "Exception", new List<string>() { ex.Message } } };
                        AuditLogService.logDebug(ex);
                    }
                }
            }

            return serviceResult;
        }

        public void SpecifiedChecklistSummary(AcknowledgementModel model, int mwItemVersion = 1)
        {
            List<SpecifiedMWRecordFormCheckItem> newMwRecordFormChecklist = new List<SpecifiedMWRecordFormCheckItem>();

            List<P_MW_FORM_09> mwForm09List = new List<P_MW_FORM_09>();

            //title:Submission Information Checking
            SpecifiedMWRecordFormCheckItem submissionTitle = new SpecifiedMWRecordFormCheckItem();
            submissionTitle.ItemName = ProcessingConstant.TITLE;
            submissionTitle.ItemDescription = "<tr><td class=\"sTh\" >&nbsp;</td><td class=\"sTh\" colspan=7 >Submission Information Checking</td></tr>";

            //title:PBP Checking
            SpecifiedMWRecordFormCheckItem pbpTitle = new SpecifiedMWRecordFormCheckItem();
            pbpTitle.ItemName = ProcessingConstant.TITLE;
            pbpTitle.ItemDescription = "<tr><td class=\"sTh\" >&nbsp;</td><td class=\"sTh\" colspan=7 >PBP Checking</td></tr>";

            //title:PRC Checking
            SpecifiedMWRecordFormCheckItem prcTitle = new SpecifiedMWRecordFormCheckItem();
            prcTitle.ItemName = ProcessingConstant.TITLE;
            prcTitle.ItemDescription = "<tr><td class=\"sTh\" >&nbsp;</td><td class=\"sTh\" colspan=7 >PRC Checking</td></tr>";

            //title:Checking of AP
            SpecifiedMWRecordFormCheckItem apChecking = new SpecifiedMWRecordFormCheckItem();
            apChecking.ItemName = ProcessingConstant.TITLE;
            apChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Checking of AP" + (mwItemVersion == 2 ? "/RI" : "") + "</td></tr>";

            SpecifiedMWRecordFormCheckItem form09PbpChecking = new SpecifiedMWRecordFormCheckItem();
            form09PbpChecking.ItemName = ProcessingConstant.TITLE;
            form09PbpChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Checking of AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE</td></tr>";

            //title:Checking of RSE
            SpecifiedMWRecordFormCheckItem rseChecking = new SpecifiedMWRecordFormCheckItem();
            rseChecking.ItemName = ProcessingConstant.TITLE;
            rseChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Checking of RSE</td></tr>";

            //title:Checking of RGE
            SpecifiedMWRecordFormCheckItem rgeChecking = new SpecifiedMWRecordFormCheckItem();
            rgeChecking.ItemName = ProcessingConstant.TITLE;
            rgeChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Checking of RGE</td></tr>";

            //title:Checking of PRC
            SpecifiedMWRecordFormCheckItem prcChecking = new SpecifiedMWRecordFormCheckItem();
            prcChecking.ItemName = ProcessingConstant.TITLE;
            prcChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Checking of PRC</td></tr>";

            //title:Other Checking
            SpecifiedMWRecordFormCheckItem otherChecking = new SpecifiedMWRecordFormCheckItem();
            otherChecking.ItemName = ProcessingConstant.TITLE;
            otherChecking.ItemDescription = "<tr class=\"sRow1\" ><td class=\"sCell\" >&nbsp;</td><td class=\"sCell\" colspan=7 >Other Checking</td></tr>";

            newMwRecordFormChecklist.Add(submissionTitle);

            //Notification
            if (!string.IsNullOrEmpty(model.P_MW_RECORD_FORM_CHECKLIST.INFO_NOT))
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "InfoNot";
                checkListItem.ColumnName = "INFO_NOT";
                checkListItem.ItemDescription = "Notification";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.INFO_NOT;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.INFO_NOT_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_NOT;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_NOT_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //Commencement date and completion date
            if (model.P_MW_RECORD_FORM_CHECKLIST.INFO_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "InfoDate";
                checkListItem.ColumnName = "INFO_DATE";
                checkListItem.ItemDescription = "Completion date is not earlier then commencement date.";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.INFO_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.INFO_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form05 works
            if (model.P_MW_RECORD_FORM_CHECKLIST.FORM05_WORK_RELATED == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "Form05WorkRelated";
                checkListItem.ColumnName = "FORM05_WORK_RELATED";
                checkListItem.ItemDescription = "Works are relating to signboard with MW No. obtained in Form MW32";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.FORM05_WORK_RELATED;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.FORM05_WORK_RELATED_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM05_WORK_RELATED;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM05_WORK_RELATED_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form05 location
            if (model.P_MW_RECORD_FORM_CHECKLIST.FORM05_LOCATION_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "Form05LocationValid";
                checkListItem.ColumnName = "FORM05_LOCATION_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.FORM05_LOCATION_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.FORM05_LOCATION_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.FORM05_LOCATION_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM05_LOCATION_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM05_LOCATION_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //other info
            if (model.P_MW_RECORD_FORM_CHECKLIST.INFO_OTHER == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "InfoOther";
                checkListItem.ColumnName = "INFO_OTHER";
                checkListItem.ItemDescription = "Other Information";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.INFO_OTHER;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.INFO_OTHER_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_OTHER;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_OTHER_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form32 work item
            if (model.P_MW_RECORD_FORM_CHECKLIST.FORM32_WORK_ITEMS == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "Form32WorkItems";
                checkListItem.ColumnName = "FORM32_WORK_ITEMS";
                checkListItem.ItemDescription = "Work items restrict to 3.16 and/or 3.17";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.FORM32_WORK_ITEMS;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.FORM32_WORK_ITEMS_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM32_WORK_ITEMS;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM32_WORK_ITEMS_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //applicant detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "applicantDetailValid";
                checkListItem.ColumnName = "APPLICANT_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_DETAIL_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.APPLICANT_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.APPLICANT_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //applicant signature
            if (model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_SIGN_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "applicantSignValid";
                checkListItem.ColumnName = "APPLICANT_SIGN_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_DETAIL_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.APPLICANT_SIGN_VALID;
                checkListItem.ToRemarks = "Applicant signature in Part A = Applicant signature in Form 1.";

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.APPLICANT_SIGN_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.APPLICANT_SIGN_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //ap detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.AP_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "apDetailValid";
                checkListItem.ColumnName = "AP_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.AP_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.AP_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.AP_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.AP_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.AP_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //rse detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.RSE_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "rseDetailValid";
                checkListItem.ColumnName = "RSE_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.RSE_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.RSE_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.RSE_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.RSE_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.RSE_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //rge detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.RGE_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "rgeDetailValid";
                checkListItem.ColumnName = "RGE_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.RGE_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.RGE_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.RGE_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.RGE_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.RGE_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //prc detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "prcDetailValid";
                checkListItem.ColumnName = "PRC_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.PRC_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //date detail valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.DATE_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "dateValid";
                checkListItem.ColumnName = "DATE_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.DATE_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.DATE_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.DATE_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.DATE_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.DATE_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }


            if (model.P_MW_RECORD_FORM_CHECKLIST.SIGNBOARD_DETAIL_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "dateValid";
                checkListItem.ColumnName = "SIGNBOARD_DETAIL_VALID";
                checkListItem.ItemDescription = model.P_MW_RECORD_FORM_CHECKLIST.SIGNBOARD_DETAIL_VALID_MSG;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.SIGNBOARD_DETAIL_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.SIGNBOARD_DETAIL_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.SIGNBOARD_DETAIL_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.SIGNBOARD_DETAIL_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }


            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
            {
                newMwRecordFormChecklist.Add(pbpTitle);
                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    newMwRecordFormChecklist.Add(form09PbpChecking);
                }
                else
                {
                    newMwRecordFormChecklist.Add(apChecking);
                }

            }

            //AP valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpApValid";
                checkListItem.ColumnName = "PBP_AP_VALID";

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    checkListItem.ItemDescription = "Validity of appointed AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE (nominator)";
                }
                else
                {
                    checkListItem.ItemDescription = "Validity of appointed AP" + (mwItemVersion == 2 ? "/RI" : "");
                }

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD_FORM_CHECKLIST.FORM09_REASON == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "Form09Reason";
                checkListItem.ColumnName = "FORM09_REASON";

                //TBC -- 2406 Java
                P_MW_FORM mwForm = DA.GetP_MW_FORM(model.P_MW_RECORD.UUID);
                string desc = "Nominator Ceasing Reason:";
                if (mwForm.FORM09_REASON_ILL != null && mwForm.FORM09_REASON_ILL.Equals(ProcessingConstant.FLAG_Y))
                {
                    desc += " Illness";
                }
                if (mwForm.FORM09_REASON_ILL != null && mwForm.FORM09_REASON_ABSENCE.Equals(ProcessingConstant.FLAG_Y))
                {
                    desc += " Temporary absence from HK";
                }
                checkListItem.ItemDescription = desc;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.FORM09_REASON;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.FORM09_REASON_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM09_REASON;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM09_REASON_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD_FORM_CHECKLIST.FORM09_ACTING_PERIOD == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "Form09ActingPeriod";
                checkListItem.ColumnName = "FORM09_ACTING_PERIOD";

                //TBC -- 2428 Java
                P_MW_FORM mwForm = DA.GetP_MW_FORM(model.P_MW_RECORD.UUID);
                string desc = "Acting Period<br/>CommencementDate:" + model.AppointedOther1.EFFECT_FROM_DATE.ToString();

                if (model.AppointedOther1.EFFECT_TO_DATE != null)
                {
                    desc += "<br/>Completion Date:" + model.AppointedOther1.EFFECT_TO_DATE.ToString();
                }
                if (mwForm.FORM09_FURTHER_NOTICE != null && mwForm.FORM09_FURTHER_NOTICE.Equals(ProcessingConstant.FLAG_Y))
                {
                    desc += "<br/>Until further notice";
                }
                checkListItem.ItemDescription = desc;

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.FORM09_ACTING_PERIOD;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.FORM09_ACTING_PERIOD_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM09_ACTING_PERIOD;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.FORM09_ACTING_PERIOD_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form 09 match up
            mwForm09List = DA.GetP_MW_FORM_09s(model.P_MW_RECORD.UUID);

            foreach (P_MW_FORM_09 mwForm09 in mwForm09List)
            {
                if (mwForm09.TO_CHECKING == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.Form09UUID = mwForm09.UUID;
                    checkListItem.ItemName = "mwForm09";
                    checkListItem.ColumnName = "TO_CHECKING";

                    checkListItem.ItemDescription = "Appointed AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE match up with nominator (" + mwForm09.MW_NUMBER.ToString() + ")";

                    checkListItem.ToChecking = mwForm09.TO_CHECKING;
                    checkListItem.ToRemarks = mwForm09.TO_RMK;

                    checkListItem.PoAgreement = mwForm09.PO_AGREEMENT;
                    checkListItem.PoRemarks = mwForm09.PO_RMK;

                    checkListItem.MwNumber = mwForm09.MW_NUMBER;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //form09 nominee valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpNewApValid";
                checkListItem.ColumnName = "PBP_NEW_AP_VALID";
                checkListItem.ItemDescription = "Validity of appointed AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE (nominee)";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_AP_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_AP_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //AP signature
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGN == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpApSign";
                checkListItem.ColumnName = "PBP_AP_SIGN";

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    checkListItem.ItemDescription = "Signature of AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE (nominator)";
                }
                else
                {
                    checkListItem.ItemDescription = "Signature of AP" + (mwItemVersion == 2 ? "/RI" : "");
                }

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGN;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGN_RMK;

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    checkListItem.Signature = true;

                    if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                    {
                        checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                    }
                    else
                    {
                        checkListItem.SignatureUuid = model.AppointedAP.UUID;
                    }

                }

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SIGN;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SIGN_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form09 nominee signature
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_SIGN == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpNewApSign";
                checkListItem.ColumnName = "PBP_NEW_AP_SIGN";
                checkListItem.ItemDescription = "Signature of AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE (nominee)";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_SIGN;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_AP_SIGN_RMK;

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    checkListItem.Signature = true;
                    checkListItem.SignatureUuid = model.AppointedOther1.UUID;
                }

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_AP_SIGN;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_AP_SIGN_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //declaration for form01
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
            {
                //dec6
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec6";
                    checkListItem.ColumnName = "PBP_AP_DEC6";
                    checkListItem.ItemDescription = "Declaration on capability of building";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec7
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec7";
                    checkListItem.ColumnName = "PBP_AP_DEC7";
                    checkListItem.ItemDescription = "Declaration on erection of signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //SSP required
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SSP == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApSsp";
                    checkListItem.ColumnName = "PBP_AP_SSP";
                    checkListItem.ItemDescription = "Supervision plan required";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SSP;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SSP_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SSP;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SSP_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec8
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec8";
                    checkListItem.ColumnName = "PBP_AP_DEC8";
                    checkListItem.ItemDescription = "Supervision Plan submitted";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC8;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC8_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //declaration for form02
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
            {
                //dec1
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC1 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec1";
                    checkListItem.ColumnName = "PBP_AP_DEC1";
                    checkListItem.ItemDescription = "Declaration on revised work items";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC1;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC1_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC1;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC1_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec5
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec5";
                    checkListItem.ColumnName = "PBP_AP_DEC5";
                    checkListItem.ItemDescription = "Declaration on completed class I works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec6
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec6";
                    checkListItem.ColumnName = "PBP_AP_DEC6";
                    checkListItem.ItemDescription = "Declaration on remaining structure";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //declaration for form08
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_08))
            {
                //PBP_AP_DEC_S483
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC_S483 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDecS483";
                    checkListItem.ColumnName = "PBP_AP_DEC_S483";
                    checkListItem.ItemDescription = "Declaration: Supervision Plan Submitted";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC_S483;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC_S483_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC_S483;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC_S483_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //declaration for form11
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
            {
                //dec6
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec6";
                    checkListItem.ColumnName = "PBP_AP_DEC6";
                    checkListItem.ItemDescription = "The building is capable of bearing loads & stresses";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec7
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec7";
                    checkListItem.ColumnName = "PBP_AP_DEC7";
                    checkListItem.ItemDescription = "Involve erection of a signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                //dec8
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpApDec8";
                    checkListItem.ColumnName = "PBP_AP_DEC8";
                    checkListItem.ItemDescription = "Supervision Plan submitted";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_DEC8_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC8;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_DEC8_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //AP signature date
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGNATURE_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpApSignatureDate";
                checkListItem.ColumnName = "PBP_AP_SIGNATURE_DATE";

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                {
                    checkListItem.ItemDescription = "AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE(Nominator) Signature Date" + "<br/>" + "Signature Date:" + model.AppointedOther0.ENDORSEMENT_DATE.ToString();
                }
                else
                {
                    checkListItem.ItemDescription = "AP" + (mwItemVersion == 2 ? "/RI" : "") + " Signature Date";
                }

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_AP_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_AP_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //form09 nominee signature date
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_SIGNATURE_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpNewSignatureDate";
                checkListItem.ColumnName = "PBP_NEW_SIGNATURE_DATE";
                checkListItem.ItemDescription = "AP" + (mwItemVersion == 2 ? "/RI" : "") + "/RSE/RGE(Nominee) Signature Date" + "<br/>" + "Signature Date:" + model.AppointedOther1.ToString();

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_NEW_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_NEW_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
            {
                newMwRecordFormChecklist.Add(rseChecking);
            }

            //rse valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRseValid";
                checkListItem.ColumnName = "PBP_RSE_VALID";
                checkListItem.ItemDescription = "Validity of appointed RSE";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }


            //rse signature
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGN == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRseSign";
                checkListItem.ColumnName = "PBP_RSE_SIGN";
                checkListItem.ItemDescription = "Signature of RSE";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGN;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGN_RMK;

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    checkListItem.Signature = true;

                    if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                    {
                        checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                    }
                    else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                    {
                        checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                    }
                    else
                    {
                        checkListItem.SignatureUuid = model.AppointedRSE.UUID;
                    }
                }

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_SIGN;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_SIGN_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRseDec3";
                    checkListItem.ColumnName = "PBP_RSE_DEC3";
                    checkListItem.ItemDescription = "Declaration on capability of building";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC1 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRseDec1";
                    checkListItem.ColumnName = "PBP_RSE_DEC1";
                    checkListItem.ItemDescription = "Declaration on revised work items";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC1;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC1_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC1;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC1_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRseDec3";
                    checkListItem.ColumnName = "PBP_RSE_DEC3";
                    checkListItem.ItemDescription = "Declaration on completed class I works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC4 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRseDec4";
                    checkListItem.ColumnName = "PBP_RSE_DEC4";
                    checkListItem.ItemDescription = "Declaration on remaining structure";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC4;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC4_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC4;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC4_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRseDec3";
                    checkListItem.ColumnName = "PBP_RSE_DEC3";
                    checkListItem.ItemDescription = "The building is capable of bearing loads & stresses";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //rse signature date
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGNATURE_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRseSignatureDate";
                checkListItem.ColumnName = "PBP_RSE_SIGNATURE_DATE";
                checkListItem.ItemDescription = "RSE Signature Date";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
            {
                newMwRecordFormChecklist.Add(rgeChecking);
            }

            //rge valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRgeValid";
                checkListItem.ColumnName = "PBP_RGE_VALID";
                checkListItem.ItemDescription = "Validity of appointed RGE";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //rge sign
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGN == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRgeSign";
                checkListItem.ColumnName = "PBP_RGE_SIGN";
                checkListItem.ItemDescription = "Signature of RGE";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGN;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGN_RMK;

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    checkListItem.Signature = true;
                    if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09))
                    {
                        checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                    }
                    else if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31) || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                    {
                        checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                    }
                    else
                    {
                        checkListItem.SignatureUuid = model.AppointedRGE.UUID;
                    }
                }

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_SIGN;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_SIGN_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC1 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRgeDec1";
                    checkListItem.ColumnName = "PBP_RGE_DEC1";
                    checkListItem.ItemDescription = "Declaration on revised work items";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC1;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC1_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_DEC1;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC1_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRgeDec3";
                    checkListItem.ColumnName = "PBP_RGE_DEC3";
                    checkListItem.ItemDescription = "Declaration on completed class I works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC4 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PbpRgeDec4";
                    checkListItem.ColumnName = "PBP_RGE_DEC4";
                    checkListItem.ItemDescription = "Declaration on remaining structure";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_DEC4;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RSE_DEC4_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_DEC4;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RSE_DEC4_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            //rge signature date
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGNATURE_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpRgeSignatureDate";
                checkListItem.ColumnName = "PBP_RGE_SIGNATURE_DATE";
                checkListItem.ItemDescription = "RGE Signature Date";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_RGE_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_RGE_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_09)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
            {
                newMwRecordFormChecklist.Add(otherChecking);
            }

            //inspection date
            if (model.P_MW_RECORD_FORM_CHECKLIST.INFO_INS_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "InfoInsDate";
                checkListItem.ColumnName = "INFO_INS_DATE";
                checkListItem.ItemDescription = "Inspection date";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.INFO_INS_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.INFO_INS_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_INS_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.INFO_INS_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //pbp other
            if (model.P_MW_RECORD_FORM_CHECKLIST.PBP_OTHER == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PbpOther";
                checkListItem.ColumnName = "PBP_OTHER";
                checkListItem.ItemDescription = "Other Information";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PBP_OTHER;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PBP_OTHER_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_OTHER;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PBP_OTHER_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_32)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
            {
                newMwRecordFormChecklist.Add(prcTitle);
                newMwRecordFormChecklist.Add(prcChecking);
            }

            //prc valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcValid";
                checkListItem.ColumnName = "PRC_VALID";
                checkListItem.ItemDescription = "Validity of appointed PRC";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //prc valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcCap";
                checkListItem.ColumnName = "PRC_CAP";
                checkListItem.ItemDescription = "Capability of appointed PRC";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_CAP_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_CAP;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_CAP_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //AS valid
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcAsValid";
                checkListItem.ColumnName = "PRC_AS_VALID";
                checkListItem.ItemDescription = "Validity of Authorized Signatory";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_VALID_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_VALID;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_VALID_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //AS signature
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_SIGN == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcAsSign";
                checkListItem.ColumnName = "PRC_AS_SIGN";
                checkListItem.ItemDescription = "Signature Of AS";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_SIGN;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_SIGN_RMK;

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_32)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                    || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    checkListItem.Signature = true;
                }

                if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
                {
                    checkListItem.SignatureUuid = model.AppointedOther0.UUID;
                }
                else
                {
                    checkListItem.SignatureUuid = model.AppointedPRC.UUID;
                }

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_SIGN;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_SIGN_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            //any other AS
            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcAsOther";
                    checkListItem.ColumnName = "PRC_AS_OTHER";
                    checkListItem.ItemDescription = "The person who arranged for the minor works to be carried out is same as the person in Form 1";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_OTHER;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_OTHER_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER))
                    {
                        SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                        checkListItem.ItemName = "PrcAsOther";
                        checkListItem.ColumnName = "PRC_AS_OTHER";
                        checkListItem.ItemDescription = "Availability of other suitable AS<br/>" + model.P_MW_RECORD_FORM_CHECKLIST.PRC_OTHER_AS_LIST;

                        checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER;
                        checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_AS_OTHER_RMK;

                        checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_OTHER;
                        checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_AS_OTHER_RMK;

                        newMwRecordFormChecklist.Add(checkListItem);
                    }
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
            {
                //prc dec
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS33";
                    checkListItem.ColumnName = "PRC_DEC_S33";
                    checkListItem.ItemDescription = "Declaration: S33 & 37 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S33;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S33_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec2";
                    checkListItem.ColumnName = "PRC_DEC2";
                    checkListItem.ItemDescription = "Commencement date for Class II works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec3";
                    checkListItem.ColumnName = "PRC_DEC3";
                    checkListItem.ItemDescription = "Declaration on submitting photographs";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec4";
                    checkListItem.ColumnName = "PRC_DEC4";
                    checkListItem.ItemDescription = "Declaration on submitting plans & details";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Declaration on signing & preparing plans & details";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "Declaration on capability of building";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec7";
                    checkListItem.ColumnName = "PRC_DEC7";
                    checkListItem.ItemDescription = "Declaration on erection of signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02))
            {
                //prc dec
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S34 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS34";
                    checkListItem.ColumnName = "PRC_DEC_S34";
                    checkListItem.ItemDescription = "Declaration: S34 & 35 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S34;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S34_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S34;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S34_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS36";
                    checkListItem.ColumnName = "PRC_DEC_S36";
                    checkListItem.ItemDescription = "Declaration: S36 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S36;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S36_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS37";
                    checkListItem.ColumnName = "PRC_DEC_S37";
                    checkListItem.ItemDescription = "Declaration: S37 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S37;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S37_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS2 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcInvolveClass2";
                    checkListItem.ColumnName = "PRC_INVOLVE_CLASS2";
                    checkListItem.ItemDescription = "Declaration: When class II MW are involved";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS2;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS2_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS2;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS2_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec2";
                    checkListItem.ColumnName = "PRC_DEC2";
                    checkListItem.ItemDescription = "Declaration on revised works items";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "Declaration on completed Class II Works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec7";
                    checkListItem.ColumnName = "PRC_DEC7";
                    checkListItem.ItemDescription = "Declaration on remaining structure";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcInvolveClass3";
                    checkListItem.ColumnName = "PRC_INVOLVE_CLASS3";
                    checkListItem.ItemDescription = "Declaration: When class III MW are involved";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC13 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec13";
                    checkListItem.ColumnName = "PRC_DEC13";
                    checkListItem.ItemDescription = "Declaration on erection of class III signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC13;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC13_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC13;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC13_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "Declaration on capability of building";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec7";
                    checkListItem.ColumnName = "PRC_DEC7";
                    checkListItem.ItemDescription = "Declaration on erection on signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS36";
                    checkListItem.ColumnName = "PRC_DEC_S36";
                    checkListItem.ItemDescription = "Declaration: S36 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S36_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S36;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S36_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS37";
                    checkListItem.ColumnName = "PRC_DEC_S37";
                    checkListItem.ItemDescription = "Declaration: S37 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S37_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S37;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S37_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec1";
                    checkListItem.ColumnName = "PRC_DEC1";
                    checkListItem.ItemDescription = "Assume all responsibilities";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC1;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC1_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Works are structurally safe";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "Remaining structure is structurally safe";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcInvolveClass3";
                    checkListItem.ColumnName = "PRC_INVOLVE_CLASS3";
                    checkListItem.ItemDescription = "Declaration: When class III MW are involved";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_INVOLVE_CLASS3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_INVOLVE_CLASS3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC12 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec12";
                    checkListItem.ColumnName = "PRC_DEC12";
                    checkListItem.ItemDescription = "Involved erection of signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC12;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC12_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC12;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC12_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Declaration on completed class III minor works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "Declaration on erection of signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Personally carried out the works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec1";
                    checkListItem.ColumnName = "PRC_DEC1";
                    checkListItem.ItemDescription = "Declaration on submitting plans";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC1_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC1;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC1_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec2";
                    checkListItem.ColumnName = "PRC_DEC2";
                    checkListItem.ItemDescription = "Declaration on submitting photographs";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec3";
                    checkListItem.ColumnName = "PRC_DEC3";
                    checkListItem.ItemDescription = "Declaration on completed works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec4";
                    checkListItem.ColumnName = "PRC_DEC4";
                    checkListItem.ItemDescription = "Declaration on remaining structure";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Personally carried out the works";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
            {
                //prc dec
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDecS33";
                    checkListItem.ColumnName = "PRC_DEC_S33";
                    checkListItem.ItemDescription = "Declaration: S33 & 37 of the B(MW)R";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC_S33_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S33;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC_S33_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec2";
                    checkListItem.ColumnName = "PRC_DEC2";

                    P_MW_FORM mwForm = DA.GetP_MW_FORM(model.P_MW_RECORD.UUID);
                    string desc = "Class II MW to be commenced on";
                    if (ProcessingConstant.FLAG_Y.Equals(mwForm.FORM11_E_SAME_DATE))
                    {
                        desc += "Same date of Part B";
                    }
                    else if (ProcessingConstant.FLAG_Y.Equals(mwForm.FORM11_E_NEW_DATE))
                    {
                        desc += model.AppointedPRC.COMMENCED_DATE.ToString();
                    }

                    checkListItem.ItemDescription = desc;

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC2_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC2_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec3";
                    checkListItem.ColumnName = "PRC_DEC3";
                    checkListItem.ItemDescription = "Photos submitted";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC3_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC3_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec4";
                    checkListItem.ColumnName = "PRC_DEC4";
                    checkListItem.ItemDescription = "Prescribed plans & details submitted";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC4_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC4_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec5";
                    checkListItem.ColumnName = "PRC_DEC5";
                    checkListItem.ItemDescription = "Assume all responsibilities";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC5_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC5_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "The building is capable of bearing loads & stresses";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec7";
                    checkListItem.ColumnName = "PRC_DEC7";
                    checkListItem.ItemDescription = "Involved erection of a signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12))
            {
                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec6";
                    checkListItem.ColumnName = "PRC_DEC6";
                    checkListItem.ItemDescription = "The building is capable of bearing loads & stresses";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC6_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC6_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

                if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7 == "N")
                {
                    SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                    checkListItem.ItemName = "PrcDec7";
                    checkListItem.ColumnName = "PRC_DEC7";
                    checkListItem.ItemDescription = "Involved erection of a signboard";

                    checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7;
                    checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_DEC7_RMK;

                    checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7;
                    checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_DEC7_RMK;

                    newMwRecordFormChecklist.Add(checkListItem);
                }

            }

            //prc signature date
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcSignatureDate";
                checkListItem.ColumnName = "PRC_SIGNATURE_DATE";
                checkListItem.ItemDescription = "PRC Signature Date";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            if (model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_07)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_32)
                || model.P_MW_RECORD.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_33))
            {
                newMwRecordFormChecklist.Add(otherChecking);
            }

            //prc other checking
            if (model.P_MW_RECORD_FORM_CHECKLIST.PRC_OTHER == "N")
            {
                SpecifiedMWRecordFormCheckItem checkListItem = new SpecifiedMWRecordFormCheckItem();
                checkListItem.ItemName = "PrcOther";
                checkListItem.ColumnName = "PRC_SIGNATURE_DATE";
                checkListItem.ItemDescription = "Other Information";

                checkListItem.ToChecking = model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE;
                checkListItem.ToRemarks = model.P_MW_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE_RMK;

                checkListItem.PoAgreement = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_SIGNATURE_DATE;
                checkListItem.PoRemarks = model.P_MW_RECORD_FORM_CHECKLIST_PO.PRC_SIGNATURE_DATE_RMK;

                newMwRecordFormChecklist.Add(checkListItem);
            }

            model.SpecifiedMWRecordFormCheckItemList = newMwRecordFormChecklist;

        }

    }
}