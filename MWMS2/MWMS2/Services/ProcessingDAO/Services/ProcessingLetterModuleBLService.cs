using Microsoft.Office.Interop.Word;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingLetterModuleBLService
    {
        private static volatile ProcessingLetterModuleDAOService _DA;
        private static readonly object locker = new object();
        private static ProcessingLetterModuleDAOService DA { get { if (_DA == null) lock (locker) if (_DA == null) _DA = new ProcessingLetterModuleDAOService(); return _DA; } }
        ProcessingEformDataEntryService eform = new ProcessingEformDataEntryService();

        private static volatile P_MW_ACK_LETTER_DAOService _ackLetterService;
        private static readonly object lockerAckLeter = new object();
        private static P_MW_ACK_LETTER_DAOService ackLetterService { get { if (_ackLetterService == null) lock (lockerAckLeter) if (_ackLetterService == null) _ackLetterService = new P_MW_ACK_LETTER_DAOService(); return _ackLetterService; } }


        private ProcessingSystemValueDAOService _SystemValueDA;
        protected ProcessingSystemValueDAOService SystemValueDA
        {
            get
            {
                return _SystemValueDA ?? (_SystemValueDA = new ProcessingSystemValueDAOService());
            }
        }

        private MwCrmInfoDaoImpl _CrmInfoDA;
        protected MwCrmInfoDaoImpl CrmInfoDA
        {
            get
            {
                return _CrmInfoDA ?? (_CrmInfoDA = new MwCrmInfoDaoImpl());
            }
        }

        private P_MW_DSN_DAOService _MWDsnService;
        protected P_MW_DSN_DAOService MWDsnService
        {
            get { return _MWDsnService ?? (_MWDsnService = new P_MW_DSN_DAOService()); }
        }

        private ProcessingDataEntryDAOService _DEDAService;
        protected ProcessingDataEntryDAOService DEDAService
        {
            get { return _DEDAService ?? (_DEDAService = new ProcessingDataEntryDAOService()); }
        }

        private ProcessingSecondOrFinalRecord _SecondRecordDA;
        protected ProcessingSecondOrFinalRecord SecondRecordDA
        {
            get
            {
                return _SecondRecordDA ?? (_SecondRecordDA = new ProcessingSecondOrFinalRecord());
            }
        }

        private DataTransferToBravoService _bravoService;
        protected DataTransferToBravoService bravoService
        {
            get { return _bravoService ?? (_bravoService = new DataTransferToBravoService()); }
        }

        private P_MW_RECORD_DAOService _mwRecordService;
        protected P_MW_RECORD_DAOService mwRecordService
        {
            get { return _mwRecordService ?? (_mwRecordService = new P_MW_RECORD_DAOService()); }
        }

        private P_MW_APPOINTED_PROFESSIONAL_DAOService _apService;
        protected P_MW_APPOINTED_PROFESSIONAL_DAOService apService
        {
            get { return _apService ?? (_apService = new P_MW_APPOINTED_PROFESSIONAL_DAOService()); }
        }

        #region Acknowledgement


        private string SearchAckByDSN_whereQ(Fn01LM_AckSearchModel model)
        {
            string whereQ = "";

            if (model.P_MW_ACK_LETTER.RECEIVED_DATE != null)
            {
                whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date( :RECEIVED_DATE ,'dd/MM/yyyy') ";
                model.QueryParameters.Add("RECEIVED_DATE", model.P_MW_ACK_LETTER.RECEIVED_DATE.ToString().Trim());
            }
            else
            {
                whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date( :RECEIVED_DATE ,'dd/MM/yyyy') ";
                model.QueryParameters.Add("RECEIVED_DATE", DateTime.Now.ToString());
            }

            if (!string.IsNullOrEmpty(model.SearchDSN))
            {
                whereQ += "\r\n\t" + " AND DSN like :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.SearchDSN.Trim().ToUpper() + "%");
            }


            return whereQ;
        }

        public ServiceResult GetFileRefNo(string mwno)
        {
            P_MW_FILEREF_DAOService fileRefDA = new P_MW_FILEREF_DAOService();
            var fileRefNo = fileRefDA.GetFileRefByMWRecord(mwno);
            if (fileRefNo == null)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE
                };
            }
            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
                ,
                Message = new List<string>()
                {
                    fileRefNo.FILEREF_FOUR
                    ,fileRefNo.FILEREF_TWO
                }
            };
        }

        private string SearchAckByReceivedDate_whereQ(Fn01LM_AckSearchModel model)
        {
            string whereQ = "";

            if (model.P_MW_ACK_LETTER.RECEIVED_DATE != null)
            {
                whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date( :RECEIVED_DATE ,'dd/MM/yyyy') ";
                model.QueryParameters.Add("RECEIVED_DATE", model.P_MW_ACK_LETTER.RECEIVED_DATE.ToString().Trim());
            }
            else
            {
                whereQ += "\r\n\t" + " AND To_Date(To_Char(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=To_Date( :RECEIVED_DATE ,'dd/MM/yyyy') ";
                model.QueryParameters.Add("RECEIVED_DATE", DateTime.Now);
            }
            return whereQ;
        }

        public Fn01LM_AckSearchModel SearchDSN(Fn01LM_AckSearchModel model)
        {
            model.QueryWhere = SearchAckByDSN_whereQ(model);
            model.Sort = " MODIFIED_DATE ";
            model.SortType = 1;
            return DA.SearchDSN(model);
        }

        public Fn01LM_AckSearchModel SearchReceivedDate(Fn01LM_AckSearchModel model)
        {
            model.QueryWhere = SearchAckByReceivedDate_whereQ(model);
            model.Sort = " MODIFIED_DATE ";
            model.SortType = 1;
            return DA.SearchReceivedDate(model);
        }

        public Fn01LM_AckSearchModel GetAckLetterEditInfo(string id)
        {
            Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
            model = DA.GetAckLetterById(id);
            model.PBP = CrmInfoDA.findByCertNo(model.P_MW_ACK_LETTER.PBP_NO) ?? new V_CRM_INFO();
            model.PRC = CrmInfoDA.findByCertNo(model.P_MW_ACK_LETTER.PRC_NO) ?? new V_CRM_INFO();
            P_MW_FILEREF_DAOService fileRefDA = new P_MW_FILEREF_DAOService();
            var fileRefNo = fileRefDA.GetFileRefByMWRecord(model.P_MW_ACK_LETTER.MW_NO);
            if (fileRefNo != null)
            {
                model.P_MW_ACK_LETTER.FILEREF_FOUR = fileRefNo.FILEREF_FOUR;
                model.P_MW_ACK_LETTER.FILEREF_TWO = fileRefNo.FILEREF_TWO;
            }
            return model;
        }

        public ServiceResult CheckAddress(Fn01LM_AckSearchModel model)
        {
            return DA.CheckAddress(model);
        }

        public ServiceResult SaveAckLetter(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    ServiceResult serviceResult = new ServiceResult();

                    //if ((model.P_MW_ACK_LETTER.COMP_DATE != null) && (Convert.ToDateTime(model.P_MW_ACK_LETTER.COMP_DATE) > DateTime.Now.AddDays(0)))
                    //{
                    //    serviceResult.Result = ServiceResult.RESULT_FAILURE;
                    //    serviceResult.Message = new List<string>()
                    //    {
                    //        ProcessingConstant.LM_ACK_COMP_DATE_ERROR_MSG
                    //    };
                    //    return serviceResult;
                    //}

                    P_MW_ACK_LETTER previousACK = ackLetterService.GetP_MW_ACK_LETTERByMWNo(model.P_MW_ACK_LETTER.PREVIOUS_RELATED_MW_NO, model.P_MW_ACK_LETTER.FORM_NO, db);
                    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PREVIOUS_RELATED_MW_NO) && previousACK == null)
                    {
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string>()
                        {
                            ProcessingConstant.LM_ACK_PREVIOUS_MWNO_ERROR_MSG
                        };
                        return serviceResult;
                    }

                    try
                    {
                        P_MW_ACK_LETTER mwAckLetterDbObj = null;

                        // Validate Dsn
                        if (MWDsnService.GetP_MW_DSNByDsn(model.P_MW_ACK_LETTER.DSN) == null)
                        {
                            serviceResult.Result = ServiceResult.RESULT_FAILURE;
                            serviceResult.ErrorMessages = new Dictionary<string, List<string>> { { "P_MW_ACK_LETTER.DSN", new List<string> { "Invalid DSN" } } };
                            return serviceResult;
                        }

                        // get exisiting record from db
                        mwAckLetterDbObj = DA.GetP_MW_ACK_LETTER(model.P_MW_ACK_LETTER.DSN);

                        if (mwAckLetterDbObj != null)
                        {
                            // Update record
                            model.P_MW_ACK_LETTER.UUID = mwAckLetterDbObj.UUID;
                            SetSignboardAndSDFRealted(model.P_MW_ACK_LETTER);
                            DA.UpdateAckLetter(model, db);
                        }
                        else
                        {
                            // ******* RandomPickAuditAndLoadParent *********
                            RandomPickAuditAndLoadParent(model);

                            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PREVIOUS_RELATED_MW_NO) && previousACK != null)
                            {
                                model.P_MW_ACK_LETTER.AUDIT_RELATED = previousACK.AUDIT_RELATED;
                                model.P_MW_ACK_LETTER.PRE_SITE_AUDIT_RELATED = previousACK.PRE_SITE_AUDIT_RELATED;
                            }

                            // Save record
                            serviceResult = DA.SaveAckLetter(model, db);

                            if (model.P_MW_ACK_LETTER.AUDIT_RELATED == ProcessingConstant.FLAG_N)
                            {
                                SaveSecondCompleteAndFinalRecord(model, db);
                            }

                            if (eform.isEfss(model))
                            {
                                eform.uploadScannedDocument(model.P_MW_ACK_LETTER.FORM_NO, model.P_MW_ACK_LETTER.DSN, db);
                            }

                            if (ProcessingConstant.FORM_02.Equals(model.P_MW_ACK_LETTER.FORM_NO) || ProcessingConstant.FORM_04.Equals(model.P_MW_ACK_LETTER.FORM_NO) || ProcessingConstant.FORM_11.Equals(model.P_MW_ACK_LETTER.FORM_NO) || ProcessingConstant.FORM_12.Equals(model.P_MW_ACK_LETTER.FORM_NO))
                            {
                                var parentAckLetter=db.P_MW_ACK_LETTER.Where(m => m.MW_NO == model.P_MW_ACK_LETTER.MW_NO &&( m.FORM_NO == ProcessingConstant.FORM_01|| m.FORM_NO == ProcessingConstant.FORM_03)).First();
                                //parentAckLetter.SSP = model.P_MW_ACK_LETTER.SSP;
                                model.P_MW_ACK_LETTER.ORDER_RELATED = parentAckLetter.ORDER_RELATED;

                                if (string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.ITEM_DISPLAY))
                                {
                                    model.P_MW_ACK_LETTER.MW_ITEM = parentAckLetter.MW_ITEM;
                                    model.P_MW_ACK_LETTER.ITEM_DISPLAY = parentAckLetter.ITEM_DISPLAY;
                                }
                                
                            }
                        }

                        //// Update final record if AFC='N'
                        //if (ProcessingConstant.FLAG_N.Equals(model.P_MW_ACK_LETTER.AUDIT_RELATED))
                        //{
                        //    bravoService.UpdateFinalRecordByRefNo(db, model.P_MW_ACK_LETTER.FORM_NO, model.P_MW_ACK_LETTER.MW_NO, ProcessingConstant.FLAG_Y);
                        //}


                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();

                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        if (e.Source == "LM300")
                        {
                            serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { {"ALERT",new List<string>(){e.Message }
                                } };
                        }
                        else
                        {
                            serviceResult.Message = new List<string> { e.Message };
                        }

                    }

                    return serviceResult;
                }
            }


        }

        public void SaveSecondCompleteAndFinalRecord(Fn01LM_AckSearchModel model, EntitiesMWProcessing db)
        {

            bool isEfss = eform.isEfss(model);

            Dictionary<string, object> secondComForeignKeys = new Dictionary<string, object>();
            Dictionary<string, object> finalRecordForeignKeys = new Dictionary<string, object>();

            P_MW_RECORD record = new P_MW_RECORD();
            record.REFERENCE_NUMBER = db.P_MW_REFERENCE_NO.Where(m => m.REFERENCE_NO == model.P_MW_ACK_LETTER.MW_NO).FirstOrDefault().UUID;
            record.MW_DSN = model.P_MW_ACK_LETTER.DSN;
            record.S_FORM_TYPE_CODE = model.P_MW_ACK_LETTER.FORM_NO;
            record.COMPLETION_DATE = model.P_MW_ACK_LETTER.COMP_DATE;
            record.COMMENCEMENT_DATE = model.P_MW_ACK_LETTER.COMM_DATE;
            record.LANGUAGE_CODE = ProcessingConstant.LANGUAGE_RADIO_ENGLISH.Equals(model.P_MW_ACK_LETTER.LANGUAGE) ? ProcessingConstant.LANGUAGE_RADIO_ENGLISH : ProcessingConstant.LANGUAGE_RADIO_CHINESE;
            record.FILEREF_FOUR = model.P_MW_ACK_LETTER.FILEREF_FOUR;
            record.FILEREF_TWO = model.P_MW_ACK_LETTER.FILEREF_TWO;
            record.AUDIT_RELATED = model.P_MW_ACK_LETTER.AUDIT_RELATED;
            record = isEfss ? eform.setMwRecord(record) : record;
            record.AUDIT_RELATED = model.P_MW_ACK_LETTER.AUDIT_RELATED;

            record.PRE_SITE_AUDIT_RELATED = model.P_MW_ACK_LETTER.PRE_SITE_AUDIT_RELATED;

            P_MW_RECORD finalRecord = JsonConvert.DeserializeObject<P_MW_RECORD>(JsonConvert.SerializeObject(record));

            //finalRecord.REFERENCE_NUMBER = db.P_MW_REFERENCE_NO.Where(m => m.REFERENCE_NO == model.P_MW_ACK_LETTER.MW_NO).FirstOrDefault().UUID;
            //finalRecord.MW_DSN = model.P_MW_ACK_LETTER.DSN;
            //finalRecord.S_FORM_TYPE_CODE = model.P_MW_ACK_LETTER.FORM_NO;

            // OI_ID : owners' corporation
            P_MW_ADDRESS oiAddress = new P_MW_ADDRESS();
            oiAddress = isEfss ? eform.setOIAddress(record, oiAddress) : oiAddress;
            P_MW_PERSON_CONTACT oiPersonContact = new P_MW_PERSON_CONTACT();
            oiPersonContact = isEfss ? eform.setOIInfo(record, oiPersonContact) : oiPersonContact;
            P_MW_ADDRESS finalOIAddress = JsonConvert.DeserializeObject<P_MW_ADDRESS>(JsonConvert.SerializeObject(oiAddress));
            P_MW_PERSON_CONTACT finalOIPersonContact = JsonConvert.DeserializeObject<P_MW_PERSON_CONTACT>(JsonConvert.SerializeObject(oiPersonContact));

            // SIGNBOARD_PERFROMER_ID : signboard owner
            P_MW_ADDRESS signBoardAddress = new P_MW_ADDRESS();
            signBoardAddress = isEfss ? eform.setSignboardOwnerAddress(record, signBoardAddress) : signBoardAddress;
            P_MW_PERSON_CONTACT signBoardPersonContact = new P_MW_PERSON_CONTACT();
            signBoardPersonContact = isEfss ? eform.setSignboardOnwerInfo(record, signBoardPersonContact) : signBoardPersonContact;
            P_MW_ADDRESS finalSignBoardAddress = JsonConvert.DeserializeObject<P_MW_ADDRESS>(JsonConvert.SerializeObject(signBoardAddress));
            P_MW_PERSON_CONTACT finalSignBoardPersonContact = JsonConvert.DeserializeObject<P_MW_PERSON_CONTACT>(JsonConvert.SerializeObject(signBoardPersonContact));

            // OWNER_ID : paw
            P_MW_ADDRESS ownerAddress = new P_MW_ADDRESS();
            ownerAddress = isEfss ? eform.setOwnerAddress(record, ownerAddress) : ownerAddress;
            P_MW_PERSON_CONTACT ownerPersonContact = new P_MW_PERSON_CONTACT();
            ownerPersonContact = isEfss ? eform.setOwnerInfo(record, ownerPersonContact) : ownerPersonContact;
            P_MW_ADDRESS finalOwnerAddress = JsonConvert.DeserializeObject<P_MW_ADDRESS>(JsonConvert.SerializeObject(ownerAddress));
            P_MW_PERSON_CONTACT finalOwnerPersonContact = JsonConvert.DeserializeObject<P_MW_PERSON_CONTACT>(JsonConvert.SerializeObject(ownerPersonContact));

            P_MW_ADDRESS mwAddress = new P_MW_ADDRESS();
            mwAddress.ENGLISH_RRM_ADDRESS = model.P_MW_ACK_LETTER.ADDRESS;
            mwAddress.DISPLAY_STREET = model.P_MW_ACK_LETTER.STREET;
            mwAddress.DISPLAY_STREET_NO = model.P_MW_ACK_LETTER.STREET_NO;
            mwAddress.DISPLAY_BUILDINGNAME = model.P_MW_ACK_LETTER.BUILDING;
            mwAddress.DISPLAY_FLOOR = model.P_MW_ACK_LETTER.FLOOR;
            mwAddress.UNIT_ID = model.P_MW_ACK_LETTER.UNIT;
            mwAddress = isEfss ? eform.setMwAddress(record, mwAddress) : mwAddress;
            P_MW_ADDRESS finalMWAddress = JsonConvert.DeserializeObject<P_MW_ADDRESS>(JsonConvert.SerializeObject(mwAddress));

            SecondRecordDA.SaveOIInfo(oiAddress, oiPersonContact, secondComForeignKeys, db);
            SecondRecordDA.SaveSignBoardInfo(signBoardAddress, signBoardPersonContact, secondComForeignKeys, db);
            SecondRecordDA.SaveOwnerInfo(ownerAddress, ownerPersonContact, secondComForeignKeys, db);
            SecondRecordDA.SaveMWAddress(mwAddress, secondComForeignKeys, db);
            SecondRecordDA.AddSecondCompleteRecord(record, secondComForeignKeys, db);

            //Start modify by dive 20191021
            List<String> autoGenMwFormCodeList = ProcessingConstant.autoGenMwFormCodeList();

            //SecondRecordDA.SaveOIInfo(finalOIAddress, finalOIPersonContact, finalRecordForeignKeys, db);
            //SecondRecordDA.SaveSignBoardInfo(finalSignBoardAddress, finalSignBoardPersonContact, finalRecordForeignKeys, db);
            //SecondRecordDA.SaveOwnerInfo(finalOwnerAddress, finalOwnerPersonContact, finalRecordForeignKeys, db);
            //SecondRecordDA.SaveMWAddress(finalMWAddress, finalRecordForeignKeys, db);
            //SecondRecordDA.AddSecondOrFinalRecord(finalRecord, finalRecordForeignKeys, true, db);
            if (autoGenMwFormCodeList.Contains(record.S_FORM_TYPE_CODE))
            {
                SecondRecordDA.SaveOIInfo(finalOIAddress, finalOIPersonContact, finalRecordForeignKeys, db);
                SecondRecordDA.SaveSignBoardInfo(finalSignBoardAddress, finalSignBoardPersonContact, finalRecordForeignKeys, db);
                SecondRecordDA.SaveOwnerInfo(finalOwnerAddress, finalOwnerPersonContact, finalRecordForeignKeys, db);
                SecondRecordDA.SaveMWAddress(finalMWAddress, finalRecordForeignKeys, db);
                SecondRecordDA.AddSecondOrFinalRecord(finalRecord, finalRecordForeignKeys, true, db);
            }
            else
            {
                //Update Final Record
                //Get Final Record
                finalRecord = mwRecordService.GetFinalMwRecordByRefNo(model.P_MW_ACK_LETTER.MW_NO, db);

                if (finalRecord == null)
                {
                    throw new Exception("Parent submission not found")
                    {
                        Source = "LM300"
                    };
                }
                else
                {
                    finalRecord.COMPLETION_DATE = record.COMPLETION_DATE;
                    //Update Final Record 
                    DA.UpdateFinalRecordCompletionDate(finalRecord, db);

                }

            }

            //End modify by dive 20191021

            // Add Record Items
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.ITEM_DISPLAY))
            {
                string[] items = model.P_MW_ACK_LETTER.ITEM_DISPLAY.Split('/');
                SaveRecordItems(items, record, false, db, isEfss);
                SaveRecordItems(items, finalRecord, true, db, isEfss);
            }

            //Start modify by dive 20191011
            //Save Appointed Professional
            List<P_MW_APPOINTED_PROFESSIONAL> aps = new List<P_MW_APPOINTED_PROFESSIONAL>();
            aps = eform.createBlankAP(record, aps);
            aps = eform.setAppointedProfessional(record, aps);

            if (aps != null)
            {
                apService.FillAppointedProfessionalByAckLetter(aps, model.P_MW_ACK_LETTER);

                foreach (var ap in aps)
                {
                    SaveRecordAppointedProfessional(ap, record.UUID, db);
                }

                //Save Final Appointed Professional

                if (autoGenMwFormCodeList.Contains(record.S_FORM_TYPE_CODE))
                {
                    List<P_MW_APPOINTED_PROFESSIONAL> finaAps = new List<P_MW_APPOINTED_PROFESSIONAL>();
                    finaAps = GetFinalAppointedProfessional(aps, record.S_FORM_TYPE_CODE);

                    foreach (var finaAp in finaAps)
                    {
                        SaveRecordAppointedProfessional(finaAp, finalRecord.UUID, db);
                    }
                }
            }


            //// Add P_MW_APPOINTED_PROFESSIONAL
            //if(!isEfss)
            //{
            //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PRC_NO))
            //    {
            //        P_MW_APPOINTED_PROFESSIONAL prc = new P_MW_APPOINTED_PROFESSIONAL()
            //        {
            //            CERTIFICATION_NO = model.P_MW_ACK_LETTER.PRC_NO,
            //            IDENTIFY_FLAG = ProcessingConstant.PRC
            //        };
            //        P_MW_APPOINTED_PROFESSIONAL finalPrc = JsonConvert.DeserializeObject<P_MW_APPOINTED_PROFESSIONAL>(JsonConvert.SerializeObject(prc));
            //        SaveRecordAppointedProfessional(prc, record.UUID, db);
            //        SaveRecordAppointedProfessional(finalPrc, finalRecord.UUID, db);
            //    }
            //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PBP_NO))
            //    {
            //        P_MW_APPOINTED_PROFESSIONAL pbp = new P_MW_APPOINTED_PROFESSIONAL()
            //        {
            //            CERTIFICATION_NO = model.P_MW_ACK_LETTER.PBP_NO,
            //            IDENTIFY_FLAG = ProcessingConstant.PBP
            //        };
            //        P_MW_APPOINTED_PROFESSIONAL finalPBP = JsonConvert.DeserializeObject<P_MW_APPOINTED_PROFESSIONAL>(JsonConvert.SerializeObject(pbp));
            //        SaveRecordAppointedProfessional(pbp, record.UUID, db);
            //        SaveRecordAppointedProfessional(finalPBP, finalRecord.UUID, db);
            //    }
            //}
            //else
            //{
            //    List<P_MW_APPOINTED_PROFESSIONAL> aps = new List<P_MW_APPOINTED_PROFESSIONAL>();
            //    aps = eform.createBlankAP(record, aps);
            //    aps = eform.setAppointedProfessional(record, aps);
            //    foreach(var ap in aps)
            //    {
            //        P_MW_APPOINTED_PROFESSIONAL finalAp = JsonConvert.DeserializeObject<P_MW_APPOINTED_PROFESSIONAL>(JsonConvert.SerializeObject(ap));
            //        SaveRecordAppointedProfessional(ap, record.UUID, db);
            //        SaveRecordAppointedProfessional(finalAp, finalRecord.UUID, db);
            //    }
            //}

            //End modify by dive 20191011

            // Update MwDsn
            //P_MW_DSN.SCANNED_STATUS_ID = ProcessingConstant.SECOND_ENTRY_COMPLETED;
            P_MW_DSN p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == model.P_MW_ACK_LETTER.DSN).FirstOrDefault();
            p_MW_DSN.SCANNED_STATUS_ID = SystemValueDA.GetSSystemValueByTypeAndCode("DSN_STATUS", "SECOND_ENTRY_COMPLETED").UUID;
            db.SaveChanges();

            if (isEfss)
            {
                P_MW_FORM form = new P_MW_FORM();
                form = eform.setMwForm(record, form);
                P_MW_FORM finalForm = JsonConvert.DeserializeObject<P_MW_FORM>(JsonConvert.SerializeObject(form));
                SaveMwForm(form, record.UUID, db);
                SaveMwForm(finalForm, finalRecord.UUID, db);
                if (ProcessingConstant.FORM_09.Equals(record.S_FORM_TYPE_CODE))
                {
                    List<P_MW_FORM_09> form09 = new List<P_MW_FORM_09>();
                    form09 = eform.createBlankMw09(record, form09);
                    form09 = eform.setMW09Form(record, form09);
                    foreach (var form09i in form09)
                    {
                        P_MW_FORM_09 final = JsonConvert.DeserializeObject<P_MW_FORM_09>(JsonConvert.SerializeObject(form09i));
                        SaveMw09Form(form09i, record.UUID, db);
                        SaveMw09Form(final, finalRecord.UUID, db);
                    }


                }
            }

            db.SaveChanges();

        }

        public List<P_MW_APPOINTED_PROFESSIONAL> GetFinalAppointedProfessional(List<P_MW_APPOINTED_PROFESSIONAL> p_MW_APPOINTED_PROFESSIONALs, string formCode)
        {
            List<P_MW_APPOINTED_PROFESSIONAL> finalAppointedProfessions = new List<P_MW_APPOINTED_PROFESSIONAL>();

            //Default
            finalAppointedProfessions.Add(new P_MW_APPOINTED_PROFESSIONAL() { IDENTIFY_FLAG = ProcessingConstant.AP, ORDERING = 0 });
            finalAppointedProfessions.Add(new P_MW_APPOINTED_PROFESSIONAL() { IDENTIFY_FLAG = ProcessingConstant.RSE, ORDERING = 1 });
            finalAppointedProfessions.Add(new P_MW_APPOINTED_PROFESSIONAL() { IDENTIFY_FLAG = ProcessingConstant.RGE, ORDERING = 2 });
            finalAppointedProfessions.Add(new P_MW_APPOINTED_PROFESSIONAL() { IDENTIFY_FLAG = ProcessingConstant.PRC, ORDERING = 3 });

            if (formCode.Equals(ProcessingConstant.FORM_01))
            {
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[4]);
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[5]);
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[6]);
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[7]);
            }
            else if (formCode.Equals(ProcessingConstant.FORM_03))
            {
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[1]);
            }
            else if (formCode.Equals(ProcessingConstant.FORM_05) || formCode.Equals(ProcessingConstant.FORM_05_36))
            {
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[1]);
            }
            else if (formCode.Equals(ProcessingConstant.FORM_06))
            {
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[2]);
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[3]);
            }
            else if (formCode.Equals(ProcessingConstant.FORM_32))
            {
                replaceFinalAppointedProfessional(finalAppointedProfessions, p_MW_APPOINTED_PROFESSIONALs[0]);
            }

            return finalAppointedProfessions;

        }

        private void replaceFinalAppointedProfessional(List<P_MW_APPOINTED_PROFESSIONAL> list, P_MW_APPOINTED_PROFESSIONAL item)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].IDENTIFY_FLAG == item.IDENTIFY_FLAG)
                {
                    list[i].CERTIFICATION_NO = item.CERTIFICATION_NO;
                    list[i].CHINESE_NAME = item.CHINESE_NAME;
                    list[i].ENGLISH_NAME = item.ENGLISH_NAME;
                    list[i].FORM_PART = item.FORM_PART;
                    list[i].IS_UPDATE_ADDRESS = item.IS_UPDATE_ADDRESS;
                    list[i].ENDORSEMENT_DATE = item.ENDORSEMENT_DATE;
                    list[i].EFFECT_FROM_DATE = item.EFFECT_FROM_DATE;
                    list[i].EFFECT_TO_DATE = item.EFFECT_TO_DATE;
                    list[i].CHINESE_COMPANY_NAME = item.CHINESE_COMPANY_NAME;
                    list[i].ENGLISH_COMPANY_NAME = item.ENGLISH_COMPANY_NAME;
                    list[i].COMMENCED_DATE = item.COMMENCED_DATE;
                    list[i].ISCHECKED = item.ISCHECKED;
                    list[i].COMPLETION_DATE = item.COMPLETION_DATE;
                    list[i].CLASS_COMPLETION_DATE = item.CLASS_COMPLETION_DATE;
                    list[i].CLASS_ENDORSEMENT_DATE = item.CLASS_ENDORSEMENT_DATE;
                    list[i].EXPIRY_DATE = item.EXPIRY_DATE;
                    list[i].CLASS1_CEASE_DATE = item.CLASS1_CEASE_DATE;
                    list[i].CLASS2_CEASE_DATE = item.CLASS2_CEASE_DATE;
                    list[i].MW_NO = item.MW_NO;
                    list[i].CONTACT_NO = item.CONTACT_NO;
                    list[i].FAX_NO = item.FAX_NO;
                    list[i].STATUS_ID = item.STATUS_ID;
                    list[i].RECEIVE_SMS = item.RECEIVE_SMS;
                }
            }
        }

        public void SaveRecordItems(string[] items, P_MW_RECORD record, bool isFinalRecord, EntitiesMWProcessing db, bool isEfss)
        {
            ProcessingEformDataEntryService eform = new ProcessingEformDataEntryService();
            List<P_MW_RECORD_ITEM> recordItems = new List<P_MW_RECORD_ITEM>();
            if (!isEfss)
            {
                foreach (var item in items)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        string classCode = "";
                        if (item.StartsWith("1"))
                        {
                            classCode = ProcessingConstant.DB_CLASS_I;
                        }
                        else if (item.StartsWith("2"))
                        {
                            classCode = ProcessingConstant.DB_CLASS_II;
                        }
                        else if (item.StartsWith("3"))
                        {
                            classCode = ProcessingConstant.DB_CLASS_III;
                        }
                        recordItems.Add(new P_MW_RECORD_ITEM()
                        {
                            MW_RECORD_ID = record.UUID,
                            MW_ITEM_CODE = item,
                            STATUS_CODE = record.S_FORM_TYPE_CODE,
                            CLASS_CODE = classCode
                        });
                    }
                }
            }
            else
            {
                recordItems = eform.saveMwItems(record, recordItems);
            }
            if (isFinalRecord)
            {
                List<P_MW_RECORD_ITEM> finalRecordItems = new List<P_MW_RECORD_ITEM>();
                foreach (var item in recordItems)
                {
                    P_MW_RECORD_ITEM finalItem = JsonConvert.DeserializeObject<P_MW_RECORD_ITEM>(JsonConvert.SerializeObject(item));
                    finalItem.LAST_MODIFIED_FORM_CODE = item.STATUS_CODE;
                    finalItem.STATUS_CODE = ProcessingConstant.MW_RECORD_ITEM_STATUS_FINAL_VERSION;
                    finalRecordItems.Add(finalItem);
                }
                db.P_MW_RECORD_ITEM.AddRange(finalRecordItems);
            }
            db.P_MW_RECORD_ITEM.AddRange(recordItems);
            db.SaveChanges();
        }

        public void SaveRecordAppointedProfessional(P_MW_APPOINTED_PROFESSIONAL appointedProfessional, string recordID, EntitiesMWProcessing db)
        {
            appointedProfessional.MW_RECORD_ID = recordID;
            db.P_MW_APPOINTED_PROFESSIONAL.Add(appointedProfessional);
            db.SaveChanges();
        }

        public void SaveMwForm(P_MW_FORM form, string recordID, EntitiesMWProcessing db)
        {
            form.MW_RECORD_ID = recordID;
            db.P_MW_FORM.Add(form);
            db.SaveChanges();
        }

        public void SaveMw09Form(P_MW_FORM_09 form, string recordID, EntitiesMWProcessing db)
        {
            form.MW_RECORD_ID = recordID;
            db.P_MW_FORM_09.Add(form);
            db.SaveChanges();
        }

        // Andy: 09-Jul-2019
        public void RandomPickAuditAndLoadParent(Fn01LM_AckSearchModel model)
        {
            List<string> AFCFormList = (new string[]{
                ProcessingConstant.FORM_MW01,
                ProcessingConstant.FORM_MW03,
                ProcessingConstant.FORM_MW05,
                ProcessingConstant.FORM_MW06,
            }).ToList();

            List<string> VSFormList = (new string[]{
                ProcessingConstant.FORM_MW06_01,
                ProcessingConstant.FORM_MW06_02,
            }).ToList();

            P_MW_ACK_LETTER mwAckLetter = model.P_MW_ACK_LETTER;
            P_MW_ACK_LETTER mwAckLetterDbObj = null;

            Boolean isOrderRelated = ProcessingConstant.FLAG_Y.Equals(mwAckLetter.ORDER_RELATED) ? true : false;
            Boolean needRandomPickAudit =
                (mwAckLetterDbObj == null && !isOrderRelated && AFCFormList.Contains(mwAckLetter.FORM_NO)) ? true : false;
            if (!needRandomPickAudit)
            {
                // Set all audit result to N
                model.P_MW_ACK_LETTER.AUDIT_RELATED = ProcessingConstant.FLAG_N;
                model.P_MW_ACK_LETTER.SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
                model.P_MW_ACK_LETTER.PRE_SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
            }

            // MW32 is signboard form
            if (ProcessingConstant.FORM_MW32.Equals(mwAckLetter.FORM_NO))
            {
                mwAckLetter.SIGNBOARD_RELATED = ProcessingConstant.FLAG_Y;
            }

            // check if MW Item related to signboard or SDF (SIGNBOARD_RELATED && SDF_RELATED)
            SetSignboardAndSDFRealted(mwAckLetter);

            // for commencement form
            if (AFCFormList.Contains(mwAckLetter.FORM_NO))
            {
                if (needRandomPickAudit)
                {
                    // System random pick AFC
                    randomPickAudit(mwAckLetter, "AFC");

                    // for AFC = 'N', trigger AFC again for specific MW Item
                    if (ProcessingConstant.FLAG_N.Equals(mwAckLetter.AUDIT_RELATED))
                    {
                        // System random pick IB_SDF
                        randomPickAudit(mwAckLetter, "IB_SDF");
                    }
                }

            }
            else if (VSFormList.Contains(mwAckLetter.FORM_NO))
            {
                /*** Complete this part ***/
                /*
                List ackLetterList = service > 利用mwAckLetter.MW_NO, 找到ACK.List
                if (ackLetterList == null || ackLetterList.Count == 0){
                    // System random pick AFC
                    randomPickAudit(mwAckLetter, "AFC");

                    // for AFC = 'N', trigger AFC again for specific MW Item
                    if (ProcessingConstant.FLAG_N.Equals(mwAckLetter.AUDIT_RELATED))
                    {
                        // System random pick IB_SDF
                        randomPickAudit(mwAckLetter, "IB_SDF");
                    }
                }
                else {
                    for (int i = 0; i < ackLetterList.Count; i++)
                    {
                        P_MW_ACK_LETTER parentAckLetter = ackLetterList.ElementAt(i);
                        SetRelatedData(pMwAckLetter, parentAckLetter);
                    }
                }
                */

                List<P_MW_ACK_LETTER> ackLetterList = DA.GetListACKLetterByMWNo(mwAckLetter);
                if (ackLetterList == null || ackLetterList.Count() == 0)
                {
                    randomPickAudit(mwAckLetter, "AFC");
                    if (ProcessingConstant.FLAG_N.Equals(mwAckLetter.AUDIT_RELATED))
                    {
                        randomPickAudit(mwAckLetter, "IB_SDF");
                    }
                }
                else
                {
                    for (int i = 0; i < ackLetterList.Count(); i++)
                    {
                        P_MW_ACK_LETTER parentAckLetter = ackLetterList.ElementAt(i);
                        SetRelatedData(mwAckLetter, parentAckLetter);
                    }
                }
            }
            else
            {
                // load data from parent form
                LoadParent(mwAckLetter);
            }
        }

        public void randomPickAudit(P_MW_ACK_LETTER mwAckLetter, String AuditType)
        {
            String FormNo = mwAckLetter.FORM_NO;

            List<string> PSACFormList = (new string[]{
                ProcessingConstant.FORM_MW01,
                ProcessingConstant.FORM_MW03
            }).ToList();

            if ("AFC".Equals(AuditType))
            {
                // default result
                String AFCResult = ProcessingConstant.FLAG_N;

                // generate random No 0-100
                int randomNo = GenerateRandomNo();
                int percentage = Convert.ToInt32(SystemValueDA.GetSSystemValueByTypeAndCode("ACK_LETTER_AUDIT_PERCENTAGE", "ACK_LETTER_AUDIT_PERCENTAGE_" + FormNo).ORDERING);
                if (percentage == 100 || percentage > randomNo)
                {
                    AFCResult = ProcessingConstant.FLAG_Y;
                }
                mwAckLetter.AUDIT_RELATED = AFCResult;

                if (ProcessingConstant.FLAG_Y.Equals(mwAckLetter.AUDIT_RELATED))
                {
                    if (PSACFormList.Contains(mwAckLetter.FORM_NO))
                    {
                        // System random pick PSAC
                        randomPickAudit(mwAckLetter, "PSAC");
                    }
                    else
                    {
                        mwAckLetter.PRE_SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
                    }

                }
                else
                {
                    mwAckLetter.PRE_SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
                    mwAckLetter.SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
                }
            }
            else if ("SAC".Equals(AuditType))
            {
                // default result
                String SACResult = ProcessingConstant.FLAG_N;

                // generate random no
                int randomNo = GenerateRandomNo();
                int percentage = Convert.ToInt32(SystemValueDA.GetSSystemValueByTypeAndCode("ACK_LETTER_AUDIT_PERCENTAGE", "ACK_LETTER_AUDIT_PERCENTAGE_SAC").ORDERING);
                if (percentage == 100 || percentage > randomNo)
                {
                    SACResult = ProcessingConstant.FLAG_Y;
                }
                else
                {
                    SACResult = ProcessingConstant.FLAG_N;
                }
                mwAckLetter.SITE_AUDIT_RELATED = SACResult;
            }
            else if ("PSAC".Equals(AuditType))
            {
                // default result
                String PSACResult = ProcessingConstant.FLAG_N;

                // generate random no
                int randomNo = GenerateRandomNo();
                int percentage = Convert.ToInt32(SystemValueDA.GetSSystemValueByTypeAndCode("ACK_LETTER_AUDIT_PERCENTAGE", "ACK_LETTER_AUDIT_PERCENTAGE_PSAC").ORDERING);
                if (percentage == 100 || percentage > randomNo)
                {
                    PSACResult = ProcessingConstant.FLAG_Y;
                }
                else
                {
                    PSACResult = ProcessingConstant.FLAG_N;
                }

                mwAckLetter.PRE_SITE_AUDIT_RELATED = PSACResult;
            }
            else if ("IB_SDF".Equals(AuditType))
            {
                String[] IB_SDF_List = ProcessingConstant.IB_SDF_List;
                /******* 以下function 要使用BCIS的data,  **********/
                /*
                List<String> itemList = mwAckLetter.spit(",")
                if IB_SDF_List contains(itemList) {
                    // System random pick AFC
                    randomPickAudit(mwAckLetter, "AFC");
                }
                */
            }

        }

        public int GenerateRandomNo()
        {
            Random random = new Random();
            return random.Next(0, 100);
        }

        public ServiceResult UpdateAckLetter(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    ServiceResult serviceResult = new ServiceResult();
                    try
                    {
                        if ((model.P_MW_ACK_LETTER.COMP_DATE != null) && (Convert.ToDateTime(model.P_MW_ACK_LETTER.COMP_DATE) < DateTime.Now.AddDays(-1)))
                        {
                            serviceResult.Result = ServiceResult.RESULT_FAILURE;
                            serviceResult.Message = new List<string>()
                            {
                                ProcessingConstant.LM_ACK_COMP_DATE_ERROR_MSG
                            };
                            return serviceResult;
                        }
                        DA.UpdateAckLetter(model, db);
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        serviceResult.Message = new List<string> { ex.Message };
                        AuditLogService.logDebug(ex);
                    }


                    return serviceResult;
                }
            }

        }

        public ServiceResult DeleteAckLetter(string id)
        {
            return DA.DeleteAckLetter(id);
        }

        public ServiceResult GetMWNo(string dsn, string type)
        {
            return DA.GetMWNo(dsn, type);
        }

        public P_MW_ACK_LETTER GetACKLetter(string dsn)
        {
            P_MW_ACK_LETTER model = new P_MW_ACK_LETTER()
            {
                DSN = dsn
            };
            return DA.GetACKLetter(model);
            //return model;
        }

        public XWPFDocument PrintWord(string id, string tempPath)
        {
            Fn01LM_AckPrintModel model = GetPrintModel(id);
            tempPath += GetTemplate(model);
            //CT_SectPr sectPr = null;
            CT_SectPr sectPr = new CT_SectPr();
            //sectPr.pgMar = new CT_PageMar();
            XWPFDocument doc;
            using (FileStream fs = File.OpenRead(tempPath))
            {
                doc = new XWPFDocument(fs);
                ProcessingAckWord(doc, model, model.Language);
                sectPr = doc.Document.body.sectPr;
                sectPr.pgMar = doc.Document.body.sectPr.pgMar;
                //sectPr.pgMar.bottom = "0";
                //sectPr.pgMar.top = "0";
                //sectPr.pgMar.left = 1;
                //sectPr.pgMar.right = 1;
            }
            if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
            {
                return BaseCommonService.GetWordDocument(tempPath, doc, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY);
            }
            else
            {
                return BaseCommonService.GetWordDocument(tempPath, doc, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
            }
        }

        public byte[] PrintPDF(string id, string tempPath, string tmpPDFDir)
        {
            Fn01LM_AckPrintModel model = GetPrintModel(id);
            tempPath += GetTemplate(model);

            string tmpPDFPath = tmpPDFDir + "tmpPDF" + Guid.NewGuid() + ".pdf";
            string tmpWordPath = tmpPDFDir + Guid.NewGuid() + "tmpWord.docx";
            CT_SectPr sectPr = null;
            //CT_SectPr sectPr = new CT_SectPr();
            //sectPr.pgMar = new CT_PageMar();
            //sectPr.pgMar.bottom = "0";
            //sectPr.pgMar.top = "0";
            //sectPr.pgMar.left = 1;
            //sectPr.pgMar.right = 1;
            XWPFDocument doc;
            using (FileStream fs = File.OpenRead(tempPath))
            {
                doc = new XWPFDocument(fs);
                ProcessingAckWord(doc, model, model.Language);
                sectPr = doc.Document.body.sectPr;
            }
            if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
            {
                return BaseCommonService.WordToPDF(tempPath, doc, tmpPDFPath, tmpWordPath, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY);
            }
            else
            {
                return BaseCommonService.WordToPDF(tempPath, doc, tmpPDFPath, tmpWordPath, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
            }
        }

        public string GetTemplate(Fn01LM_AckPrintModel model)
        {
            StringBuilder path = new StringBuilder();
            path.Append(model.FormOn);
            if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
            {
                path.Append("_CHI.docx");
            }
            else
            {
                path.Append("_ENG.docx");
            }
            return path.ToString();
        }

        public XWPFDocument PrintWordAL(string languageCode, string filePath, Fn01LM_ALDisplayModel viewModel)
        {
            try
            {
                Fn01LM_ALPrintModel model = getALPrintModel(viewModel, languageCode);
                CT_SectPr sectPr = new CT_SectPr();
                XWPFDocument doc;
                using (FileStream fs = File.OpenRead(filePath))
                {
                    doc = new XWPFDocument(fs);
                    ProcessingWordAL(doc, model);
                    sectPr = doc.Document.body.sectPr;
                }
                if (languageCode == null)
                {
                    languageCode = viewModel.P_MW_ACK_LETTER.LANGUAGE;
                }
                if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(languageCode)) // chinese
                {
                    return BaseCommonService.GetWordDocument(filePath, doc, model, sectPr, ProcessingConstant.LANG_CHINESE);
                }
                else // english
                {
                    return BaseCommonService.GetWordDocument(filePath, doc, model, sectPr, ProcessingConstant.LANG_ENGLISH);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void ProcessingWordAL(XWPFDocument doc, Fn01LM_ALPrintModel model)
        {
            foreach (var table in doc.Tables)
            {
                var rowIdx = 0;
                while (rowIdx < table.Rows.Count)
                {
                    var row = table.Rows[rowIdx];
                    bool isRomoveRow = false;
                    foreach (var cell in row.GetTableCells())
                    {
                        for (int i = 0; i < cell.Tables.Count; i++)
                        {
                            foreach (var subRow in cell.Tables[i].Rows)
                            {
                                foreach (var subCell in subRow.GetTableCells())
                                {
                                    foreach (var subParag in subCell.Paragraphs)
                                    {
                                        Type entityType = typeof(Fn01LM_ALPrintModel);
                                        PropertyInfo[] propertyInfos = entityType.GetProperties();
                                        //string entityName = entityType.Name;
                                        string paragraphText = subParag.ParagraphText;
                                        string styleId = subParag.Style;
                                        string text = subParag.ParagraphText;
                                        foreach (var p in propertyInfos)
                                        {
                                            if (p.Name.Contains("Desc"))
                                            {
                                                string replaceName = "${" + p.Name + "}";
                                                object value = p.GetValue(model);
                                                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                                {
                                                    if (text.Contains(replaceName))
                                                    {
                                                        isRomoveRow = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (isRomoveRow) break;
                                    }
                                    if (isRomoveRow) break;
                                }
                                if (isRomoveRow) break;
                            }
                            if (isRomoveRow) break;
                        }
                        if (isRomoveRow) break;
                    }
                    if (isRomoveRow)
                    {
                        table.RemoveRow(rowIdx);
                    }
                    else
                    {
                        rowIdx++;
                    }
                }
            }
        }
        private void ProcessingAckWord<T>(XWPFDocument doc, T model, string language)
        {
            foreach (var table in doc.Tables)
            {
                var rowIdx = 0;
                while (rowIdx < table.Rows.Count)
                {
                    var row = table.Rows[rowIdx];
                    bool isRomoveRow = false;
                    foreach (var cell in row.GetTableCells())
                    {
                        foreach (var parag in cell.Paragraphs)
                        {
                            Type entityType = typeof(T);
                            PropertyInfo[] propertyInfos = entityType.GetProperties();
                            //string entityName = entityType.Name;
                            string paragraphText = parag.ParagraphText;
                            string styleId = parag.Style;
                            string text = parag.ParagraphText;
                            foreach (var p in propertyInfos)
                            {
                                if ((p.Name.Contains("pbpName") && language == ProcessingConstant.LANGUAGE_RADIO_CHINESE) || p.Name.Contains("prcName"))
                                {
                                    string replaceName = "${" + p.Name + "}";
                                    object value = p.GetValue(model);
                                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                    {
                                        if (text.Contains(replaceName))
                                        {
                                            isRomoveRow = true;
                                            break;
                                        }
                                    }
                                }
                                if (p.Name.Contains("pbpContact") || p.Name.Contains("prcContact"))
                                {
                                    string replaceName = "${" + p.Name + "}";
                                    object value = p.GetValue(model);
                                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                    {
                                        if (text.Contains(replaceName))
                                        {
                                            isRomoveRow = true;
                                            table.RemoveRow(rowIdx + 1);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (isRomoveRow) break;
                        }
                        //for (int i = 0; i < cell.Tables.Count; i++)
                        //{
                        //    foreach (var subRow in cell.Tables[i].Rows)
                        //    {
                        //        foreach (var subCell in subRow.GetTableCells())
                        //        {

                        //            if (isRomoveRow) break;
                        //        }
                        //        if (isRomoveRow) break;
                        //    }
                        //    if (isRomoveRow) break;
                        //}
                        if (isRomoveRow) break;
                    }
                    if (isRomoveRow)
                    {
                        table.RemoveRow(rowIdx);
                    }
                    else
                    {
                        rowIdx++;
                    }
                }
            }
        }

        public byte[] PrintPDFAL(string languageCode, string tempPath, string tmpPDFDir, Fn01LM_ALDisplayModel viewModel)
        {
            Fn01LM_ALPrintModel model = getALPrintModel(viewModel, languageCode);
            string tmpPDFPath = tmpPDFDir + "tmpPDF" + Guid.NewGuid() + ".pdf";
            string tmpWordPath = tmpPDFDir + "tmpWord.docx";
            XWPFDocument doc;
            CT_SectPr sectPr = new CT_SectPr();
            using (FileStream fs = File.OpenRead(tempPath))
            {
                doc = new XWPFDocument(fs);
                ProcessingWordAL(doc, model);
                sectPr.pgMar = new CT_PageMar();
                sectPr.pgMar.bottom = "0";
                sectPr.pgMar.top = "0";
                sectPr.pgMar.left = 1;
                sectPr.pgMar.right = 1;
            }
            if (languageCode == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
            {
                return BaseCommonService.WordToPDF(tempPath, doc, tmpPDFPath, tmpWordPath, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY);
            }
            else
            {
                return BaseCommonService.WordToPDF(tempPath, doc, tmpPDFPath, tmpWordPath, model, sectPr, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
            }
        }


        public Fn01LM_ALPrintModel getALPrintModel(Fn01LM_ALDisplayModel viewModel, string languageCode)
        {
            Fn01LM_ALPrintModel model = new Fn01LM_ALPrintModel();
            model.mwno = viewModel.P_MW_ACK_LETTER.MW_NO;
            model.pbpName = viewModel.PbPName;
            model.pbpFax = viewModel.PbpFax;
            model.prcName = viewModel.PrcName;
            model.prcFax = viewModel.PrcFax;
            model.title = viewModel.Title;

            model.address = viewModel.Address;

            model.firstPara = viewModel.FirstPara;
            model.aDesc = "(a)    ";
            model.bDesc = "(b)    ";
            model.cDesc = "(c)    ";
            model.dDesc = "(d)    ";
            model.eDesc = "(e)    ";
            model.fDesc = "(f)    ";
            model.gDesc = "(g)    ";
            model.hDesc = "(h)    ";
            model.iDesc = "(i)    ";
            model.jDesc = "(j)    ";

            model.secondPara = viewModel.SecondPara;
            model.thirdPara = viewModel.ThirdPara;
            model.fourthPara = viewModel.FourthPara;
            model.fifthPara = viewModel.FifthPara;
            model.spoName = viewModel.SpoName;
            model.letterDate = viewModel.LetterDateDisplay;

            if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(viewModel.Language)) // chinese
            {
                if (viewModel.PbpAddr != null && viewModel.PbpAddr.Count() > 0)
                {
                    model.pbpAddr1 = viewModel.PbpAddr[0];
                }
                else
                {
                    model.pbpAddr1 = "";
                }
                if (viewModel.PrcAddr != null && viewModel.PrcAddr.Count() > 0)
                {
                    model.prcAddr1 = viewModel.PrcAddr[0];
                }
                else
                {
                    model.prcAddr1 = "";
                }

                if (!string.IsNullOrWhiteSpace(viewModel.PawName))
                {
                    model.pawName = "副本送: " + viewModel.PawName;
                    model.pawContact = "(傳真: " + viewModel.PawContact + ")";
                }
                else
                {
                    model.pawName = "";
                    model.pawContact = "";
                }

                model.spoPost = "( " + viewModel.SpoPost;

                model.aDesc += viewModel.Items.Where(x => x.ITEM_NO == "a").FirstOrDefault().ITEM_TEXT;
                model.bDesc += viewModel.Items.Where(x => x.ITEM_NO == "b").FirstOrDefault().ITEM_TEXT;
                model.cDesc += viewModel.Items.Where(x => x.ITEM_NO == "c").FirstOrDefault().ITEM_TEXT;
                model.dDesc += viewModel.Items.Where(x => x.ITEM_NO == "d").FirstOrDefault().ITEM_TEXT;
                model.eDesc += viewModel.Items.Where(x => x.ITEM_NO == "e").FirstOrDefault().ITEM_TEXT;
                model.fDesc += viewModel.Items.Where(x => x.ITEM_NO == "f").FirstOrDefault().ITEM_TEXT;
                model.gDesc += viewModel.Items.Where(x => x.ITEM_NO == "g").FirstOrDefault().ITEM_TEXT;
                model.hDesc += viewModel.Items.Where(x => x.ITEM_NO == "h").FirstOrDefault().ITEM_TEXT;
                model.iDesc += viewModel.Items.Where(x => x.ITEM_NO == "i").FirstOrDefault().ITEM_TEXT;
                model.jDesc += viewModel.Items.Where(x => x.ITEM_NO == "j").FirstOrDefault().ITEM_TEXT;

            }
            else // english
            {
                if (viewModel.PbpAddr != null && viewModel.PbpAddr.Count() > 0)
                {
                    model.pbpAddr1 = viewModel.PbpAddr[0];
                    model.pbpAddr2 = viewModel.PbpAddr[1];
                    model.pbpAddr3 = viewModel.PbpAddr[2];
                    model.pbpAddr4 = viewModel.PbpAddr[3];
                    model.pbpAddr5 = viewModel.PbpAddr[4];
                }
                else
                {
                    model.pbpAddr1 = "";
                    model.pbpAddr2 = "";
                    model.pbpAddr3 = "";
                    model.pbpAddr4 = "";
                    model.pbpAddr5 = "";
                }
                if (viewModel.PrcAddr != null && viewModel.PrcAddr.Count() > 0)
                {
                    model.prcAddr1 = viewModel.PrcAddr[0];
                    model.prcAddr2 = viewModel.PrcAddr[1];
                    model.prcAddr3 = viewModel.PrcAddr[2];
                    model.prcAddr4 = viewModel.PrcAddr[3];
                    model.prcAddr5 = viewModel.PrcAddr[4];
                }
                else
                {
                    model.prcAddr1 = "";
                    model.prcAddr2 = "";
                    model.prcAddr3 = "";
                    model.prcAddr4 = "";
                    model.prcAddr5 = "";
                }

                if (!string.IsNullOrWhiteSpace(viewModel.PawName))
                {
                    model.pawName = "cc: " + viewModel.PawName;
                    model.pawContact = "(Fax No. " + viewModel.PawContact + ")";
                }
                else
                {
                    model.pawName = "";
                    model.pawContact = "";
                }

                model.spoPost = viewModel.SpoPost;

                model.aDesc += viewModel.Items.Where(x => x.ITEM_NO == "a").FirstOrDefault().ITEM_TEXT_E;
                model.bDesc += viewModel.Items.Where(x => x.ITEM_NO == "b").FirstOrDefault().ITEM_TEXT_E;
                model.cDesc += viewModel.Items.Where(x => x.ITEM_NO == "c").FirstOrDefault().ITEM_TEXT_E;
                model.dDesc += viewModel.Items.Where(x => x.ITEM_NO == "d").FirstOrDefault().ITEM_TEXT_E;
                model.eDesc += viewModel.Items.Where(x => x.ITEM_NO == "e").FirstOrDefault().ITEM_TEXT_E;
                model.fDesc += viewModel.Items.Where(x => x.ITEM_NO == "f").FirstOrDefault().ITEM_TEXT_E;
                model.gDesc += viewModel.Items.Where(x => x.ITEM_NO == "g").FirstOrDefault().ITEM_TEXT_E;
                model.hDesc += viewModel.Items.Where(x => x.ITEM_NO == "h").FirstOrDefault().ITEM_TEXT_E;
                model.iDesc += viewModel.Items.Where(x => x.ITEM_NO == "i").FirstOrDefault().ITEM_TEXT_E;
                model.jDesc += viewModel.Items.Where(x => x.ITEM_NO == "j").FirstOrDefault().ITEM_TEXT_E;
            }
            model.aDesc = viewModel.Items.Where(x => x.ITEM_NO == "a").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.aDesc : "";
            model.bDesc = viewModel.Items.Where(x => x.ITEM_NO == "b").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.bDesc : "";
            model.cDesc = viewModel.Items.Where(x => x.ITEM_NO == "c").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.cDesc : "";
            model.dDesc = viewModel.Items.Where(x => x.ITEM_NO == "d").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.dDesc : "";
            model.eDesc = viewModel.Items.Where(x => x.ITEM_NO == "e").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.eDesc : "";
            model.fDesc = viewModel.Items.Where(x => x.ITEM_NO == "f").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.fDesc : "";
            model.gDesc = viewModel.Items.Where(x => x.ITEM_NO == "g").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.gDesc : "";
            model.hDesc = viewModel.Items.Where(x => x.ITEM_NO == "h").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.hDesc : "";
            model.iDesc = viewModel.Items.Where(x => x.ITEM_NO == "i").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.iDesc : "";
            model.jDesc = viewModel.Items.Where(x => x.ITEM_NO == "j").FirstOrDefault().CHECKED.Equals(ProcessingConstant.FLAG_Y) ? model.jDesc : "";

            return model;
        }

        public Fn01LM_AckPrintModel GetPrintModel(string id)
        {
            Fn01LM_AckPrintModel model = DA.GetPrintModel(id);
            GetPBPInfo(model);
            GetPRCInfo(model);
            GetLetterAdress(model);
            GetLetterPaw(model);
            GetSPO(model);
            return model;
        }

        public Fn01LM_AckPrintModel GetPBPInfo(Fn01LM_AckPrintModel model)
        {
            V_CRM_INFO pbp = CrmInfoDA.findByCertNo(model.PBPNo);
            string pbpFax = "";
            if (pbp != null)
            {
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    model.pbpName = pbp.CHINESE_NAME ?? (pbp.SURNAME + " " + pbp.GIVEN_NAME);
                }
                else
                {
                    model.pbpName = (pbp.SURNAME + " " + pbp.GIVEN_NAME) ?? pbp.CHINESE_NAME;
                }

                pbpFax = pbp.FAX_NO;

                // Find Chinese address
                StringBuilder pbpChiAddress = new StringBuilder();
                pbpChiAddress.Append(pbp.CN_ADDRESS_LINE1 ?? "");
                if (!string.IsNullOrWhiteSpace(pbp.CN_ADDRESS_LINE2))
                {
                    pbpChiAddress.Append(pbp.CN_ADDRESS_LINE2);
                }
                if (!string.IsNullOrWhiteSpace(pbp.CN_ADDRESS_LINE3))
                {
                    pbpChiAddress.Append(pbp.CN_ADDRESS_LINE3);
                }
                if (!string.IsNullOrWhiteSpace(pbp.CN_ADDRESS_LINE4))
                {
                    pbpChiAddress.Append(pbp.CN_ADDRESS_LINE4);
                }
                if (!string.IsNullOrWhiteSpace(pbp.CN_ADDRESS_LINE5))
                {
                    pbpChiAddress.Append(pbp.CN_ADDRESS_LINE5);
                }
                pbpChiAddress = pbpChiAddress.Replace("&", "&amp;");

                // Find English address
                StringBuilder pbpEngAddress = new StringBuilder();
                pbpEngAddress.Append(pbp.EN_ADDRESS_LINE1 ?? "");
                if (!string.IsNullOrWhiteSpace(pbp.EN_ADDRESS_LINE2))
                {
                    pbpEngAddress.Append(pbp.EN_ADDRESS_LINE2);
                }
                if (!string.IsNullOrWhiteSpace(pbp.EN_ADDRESS_LINE3))
                {
                    pbpEngAddress.Append(pbp.EN_ADDRESS_LINE3);
                }
                if (!string.IsNullOrWhiteSpace(pbp.EN_ADDRESS_LINE4))
                {
                    pbpEngAddress.Append(pbp.EN_ADDRESS_LINE4);
                }
                if (!string.IsNullOrWhiteSpace(pbp.EN_ADDRESS_LINE5))
                {
                    pbpEngAddress.Append(pbp.EN_ADDRESS_LINE5);
                }
                pbpEngAddress = pbpEngAddress.Replace("&", "&amp;");

                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    model.pbpAddress = !string.IsNullOrWhiteSpace(pbpChiAddress.ToString()) ? pbpChiAddress.ToString() : pbpEngAddress.ToString();
                }
                else
                {
                    model.pbpAddress = !string.IsNullOrWhiteSpace(pbpEngAddress.ToString()) ? pbpEngAddress.ToString() : pbpChiAddress.ToString();
                }
            }
            if (string.IsNullOrWhiteSpace(pbpFax))
            {
                model.pbpContact += model.pbpAddress;
            }
            else
            {
                model.pbpContact = FormatContact(pbpFax, model);
            }
            return model;
        }

        public Fn01LM_AckPrintModel GetPRCInfo(Fn01LM_AckPrintModel model)
        {
            V_CRM_INFO prc = CrmInfoDA.findByCertNo(model.PRCNo);
            string prcFax = "";
            if (prc != null)
            {
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    model.prcName = prc.CHINESE_NAME ?? (prc.SURNAME + " " + prc.GIVEN_NAME);
                }
                else
                {
                    model.prcName = (prc.SURNAME + " " + prc.GIVEN_NAME) ?? prc.CHINESE_NAME;
                }

                prcFax = prc.FAX_NO;

                // Find Chinese address
                StringBuilder prcChiAddress = new StringBuilder();
                prcChiAddress.Append(prc.CN_ADDRESS_LINE1 ?? "");
                if (!string.IsNullOrWhiteSpace(prc.CN_ADDRESS_LINE2))
                {
                    prcChiAddress.Append(prc.CN_ADDRESS_LINE2);
                }
                if (!string.IsNullOrWhiteSpace(prc.CN_ADDRESS_LINE3))
                {
                    prcChiAddress.Append(prc.CN_ADDRESS_LINE3);
                }
                if (!string.IsNullOrWhiteSpace(prc.CN_ADDRESS_LINE4))
                {
                    prcChiAddress.Append(prc.CN_ADDRESS_LINE4);
                }
                if (!string.IsNullOrWhiteSpace(prc.CN_ADDRESS_LINE5))
                {
                    prcChiAddress.Append(prc.CN_ADDRESS_LINE5);
                }
                prcChiAddress = prcChiAddress.Replace("&", "&amp;");

                // Find English address
                StringBuilder prcEngAddress = new StringBuilder();
                prcEngAddress.Append(prc.EN_ADDRESS_LINE1 ?? "");
                if (!string.IsNullOrWhiteSpace(prc.EN_ADDRESS_LINE2))
                {
                    prcEngAddress.Append(prc.EN_ADDRESS_LINE2);
                }
                if (!string.IsNullOrWhiteSpace(prc.EN_ADDRESS_LINE3))
                {
                    prcEngAddress.Append(prc.EN_ADDRESS_LINE3);
                }
                if (!string.IsNullOrWhiteSpace(prc.EN_ADDRESS_LINE4))
                {
                    prcEngAddress.Append(prc.EN_ADDRESS_LINE4);
                }
                if (!string.IsNullOrWhiteSpace(prc.EN_ADDRESS_LINE5))
                {
                    prcEngAddress.Append(prc.EN_ADDRESS_LINE5);
                }
                prcEngAddress = prcEngAddress.Replace("&", "&amp;");

                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    model.prcAddress = !string.IsNullOrWhiteSpace(prcChiAddress.ToString()) ? prcChiAddress.ToString() : prcEngAddress.ToString();
                }
                else
                {
                    model.prcAddress = !string.IsNullOrWhiteSpace(prcEngAddress.ToString()) ? prcEngAddress.ToString() : prcChiAddress.ToString();
                }

            }
            if (string.IsNullOrWhiteSpace(prcFax))
            {
                model.prcContact += model.prcAddress;
            }
            else
            {
                model.prcContact = FormatContact(prcFax, model);
            }
            return model;
        }

        public Fn01LM_AckPrintModel GetLetterAdress(Fn01LM_AckPrintModel model)
        {
            if (string.IsNullOrWhiteSpace(model.address))
            {
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                {
                    StringBuilder addressFirstComponent = new StringBuilder();
                    StringBuilder addressSecondComponent = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(model.unit))
                    {
                        addressFirstComponent.Append(model.unit + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.floor))
                    {
                        addressFirstComponent.Append(model.floor + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.building))
                    {
                        addressFirstComponent.Append(model.building + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(addressFirstComponent.ToString()))
                    {
                        addressFirstComponent.Append(", ");
                    }

                    if (!string.IsNullOrWhiteSpace(model.streetNo))
                    {
                        addressSecondComponent.Append(model.streetNo + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.street))
                    {
                        addressSecondComponent.Append(model.street);
                    }
                    model.address = addressFirstComponent.ToString() + addressSecondComponent.ToString();
                }
                else if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    StringBuilder fullAddress = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(model.street))
                    {
                        fullAddress.Append(model.street + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.streetNo))
                    {
                        fullAddress.Append(model.streetNo + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.building))
                    {
                        fullAddress.Append(model.building + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.floor))
                    {
                        fullAddress.Append(model.floor + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.unit))
                    {
                        fullAddress.Append(model.unit + " ");
                    }
                    model.address = fullAddress.ToString();
                }
            }
            return model;
        }

        private string FormatContact(string contact, Fn01LM_AckPrintModel model)
        {
            long contactResult = 0;
            if (long.TryParse(contact.Replace(" ", ""), out contactResult))
            {
                if (contact.Length > 4)
                {
                    contact = contact.Substring(0, contact.Length - 4) + " " + contact.Substring(contact.Length - 4);
                }
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                {
                    contact = "(" + ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_CONTACT + contact + ")";
                }
                else
                {
                    contact = "(" + ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_CONTACT + contact + ")";
                }
            }
            else
            {
                contact = "(" + contact + ")";
            }
            return contact;
        }

        public Fn01LM_AckPrintModel GetLetterPaw(Fn01LM_AckPrintModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.paw))
            {
                if (model.Language == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                {
                    model.paw = "c.c. : " + model.paw.ToUpper();
                }
                else
                {
                    model.paw = "副本送 : " + model.paw;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.pawContact))
            {

                model.pawContact = FormatContact(model.pawContact, model);
                //long pawContactResult = 0;
                //if (long.TryParse(model.pawContact, out pawContactResult))
                //{
                //    if (model.pawContact.Length > 4)
                //    {
                //        model.pawContact = model.pawContact.Substring(0, model.pawContact.Length - 4) + " " + model.pawContact.Substring(model.pawContact.Length - 4);
                //    }
                //    if (model.Language == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                //    {
                //        model.pawContact = "(By Fax Only :  " + model.pawContact + ")";
                //    }
                //    else
                //    {
                //        model.pawContact = "(  只发传真 :  " + model.pawContact + ")";
                //    }
                //}
                //else
                //{
                //    model.pawContact = "(" + model.pawContact + ")";
                //}
            }
            return model;
        }

        public Fn01LM_AckPrintModel GetSPO(Fn01LM_AckPrintModel model)
        {
            List<P_S_SYSTEM_VALUE> spo = DA.GeSPO(model);
            if (model.Language == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
            {
                model.spoName = spo.Where(m => m.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_NAME).FirstOrDefault().DESCRIPTION;
                model.spoPosition = spo.Where(m => m.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION).FirstOrDefault().DESCRIPTION;
            }
            else
            {
                model.spoName = spo.Where(m => m.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_NAME).FirstOrDefault().DESCRIPTION;
                model.spoPosition = spo.Where(m => m.CODE == ProcessingConstant.S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION).FirstOrDefault().DESCRIPTION;
            }
            return model;
        }

        public dynamic Excel(Fn01LM_AckSearchModel model)
        {
            model.QueryWhere = SearchAckByDSN_whereQ(model);
            return new { key = DA.Excel(model) };
        }

        #endregion

        #region Search

        private string SearchLetter_whereQ(Fn01LM_SearchModel model)
        {
            string whereQ = "";
            if (model.P_MW_ACK_LETTER.COUNTER.ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND COUNTER=:Counter ";
                model.QueryParameters.Add("Counter", model.P_MW_ACK_LETTER.COUNTER.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.NATURE.ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND NATURE=:Nature ";
                model.QueryParameters.Add("Nature", model.P_MW_ACK_LETTER.NATURE.Trim().ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom);
                model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom);
                model.QueryParameters.Add("ReceivedDateTo", DateTime.Now.ToString());
            }
            if (string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo);
            }

            if (!string.IsNullOrWhiteSpace(model.LetterDateFrom) && !string.IsNullOrWhiteSpace(model.LetterDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:LetterDateFrom,'dd/MM/yyyy') AND TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("LetterDateFrom", model.LetterDateFrom);
                model.QueryParameters.Add("LetterDateTo", model.LetterDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.LetterDateFrom) && string.IsNullOrWhiteSpace(model.LetterDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:LetterDateFrom,'dd/MM/yyyy') AND TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("LetterDateFrom", model.LetterDateFrom);
                model.QueryParameters.Add("LetterDateTo", DateTime.Now.ToString());
            }
            if (string.IsNullOrWhiteSpace(model.LetterDateFrom) && !string.IsNullOrWhiteSpace(model.LetterDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("LetterDateTo", model.LetterDateTo);
            }
            if (model.IsLetterDateNull)
            {
                whereQ += "\r\n\t" + " AND LETTER_DATE IS NULL ";
            }
            else
            {
                whereQ += "\r\n\t" + " AND LETTER_DATE IS NOT NULL ";
            }

            if (!string.IsNullOrWhiteSpace(model.CommDateFrom) && !string.IsNullOrWhiteSpace(model.CommDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:CommDateFrom,'dd/MM/yyyy') AND TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("CommDateFrom", model.CommDateFrom);
                model.QueryParameters.Add("CommDateTo", model.CommDateTo);
            }
            else if (!string.IsNullOrWhiteSpace(model.CommDateFrom) && string.IsNullOrWhiteSpace(model.CommDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:CommDateFrom,'dd/MM/yyyy') AND TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("CommDateFrom", model.CommDateFrom);
                model.QueryParameters.Add("CommDateTo", DateTime.Now.ToString());
            }
            else if (string.IsNullOrWhiteSpace(model.CommDateFrom) && !string.IsNullOrWhiteSpace(model.CommDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("CommDateTo", model.CommDateTo);
            }

            if (model.IsCompDateNull)
            {
                whereQ += "\r\n\t" + " AND COMP_DATE IS NULL ";
            }
            else
            {
                whereQ += "\r\n\t" + " AND COMP_DATE IS NOT NULL ";
            }

            if (model.IsReferralDateNull)
            {
                whereQ += "\r\n\t" + " AND REFERRAL_DATE IS NULL ";
            }
            else
            {
                whereQ += "\r\n\t" + " AND REFERRAL_DATE IS NOT NULL ";
            }

            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.DSN))
            {
                whereQ += "\r\n\t" + " AND DSN like :DSN ";
                model.QueryParameters.Add("DSN", "%" + model.P_MW_ACK_LETTER.DSN.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.MW_NO))
            {
                whereQ += "\r\n\t" + " AND MW_NO LIKE :MWNo ";
                model.QueryParameters.Add("MWNo", "%" + model.P_MW_ACK_LETTER.MW_NO.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.FORM_NO))
            {
                whereQ += "\r\n\t" + " AND FORM_NO LIKE :FormNo ";
                model.QueryParameters.Add("FormNo", "%" + model.P_MW_ACK_LETTER.FORM_NO.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.MW_ITEM))
            {
                whereQ += "\r\n\t" + " AND MW_ITEM= :MWItem ";
                model.QueryParameters.Add("MWItem", model.P_MW_ACK_LETTER.MW_ITEM.Trim());
            }
            if (model.P_MW_ACK_LETTER.AUDIT_RELATED.Trim().ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND AUDIT_RELATED= :AuditRelated ";
                model.QueryParameters.Add("AuditRelated", model.P_MW_ACK_LETTER.AUDIT_RELATED.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.ORDER_RELATED.Trim().ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND ORDER_RELATED= :OrderRelated ";
                model.QueryParameters.Add("OrderRelated", model.P_MW_ACK_LETTER.ORDER_RELATED.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.SDF_RELATED.Trim().ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND SDF_RELATED=:SDFRelated ";
                model.QueryParameters.Add("SDFRelated", model.P_MW_ACK_LETTER.SDF_RELATED.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.SIGNBOARD_RELATED.Trim().ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND SIGNBOARD_RELATED= :SignboardRelated ";
                model.QueryParameters.Add("SignboardRelated", model.P_MW_ACK_LETTER.SIGNBOARD_RELATED.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.SSP.Trim().ToUpper() != "ALL")
            {
                whereQ += "\r\n\t" + " AND SSP= :SSP ";
                model.QueryParameters.Add("SSP", model.P_MW_ACK_LETTER.SSP.Trim().ToUpper());
            }
            if (model.P_MW_ACK_LETTER.REFERRAL_DATE != null)
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(REFERRAL_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=TO_DATE(:ReferralDate,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReferralDate", model.P_MW_ACK_LETTER.REFERRAL_DATE.ToString());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PBP_NO))
            {
                switch (model.P_MW_ACK_LETTER.PBP_NO.Trim().ToUpper())
                {
                    case "1":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(A)%");
                        break;
                    case "2":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(E)%");
                        break;
                    case "3":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(S)%");
                        break;
                    default:
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", model.P_MW_ACK_LETTER.PBP_NO.Trim().ToUpper() + "%");
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PRC_NO))
            {
                switch (model.P_MW_ACK_LETTER.PRC_NO.Trim().ToUpper())
                {
                    case "1":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "GBC%");
                        break;
                    case "2":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "MWC%");
                        break;
                    case "3":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "MWC(W)%");
                        break;
                    default:
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", model.P_MW_ACK_LETTER.PRC_NO.Trim().ToUpper() + "%");
                        break;
                }

            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.ADDRESS))
            {
                whereQ += "\r\n\t" + "  AND ADDRESS= :Address ";
                model.QueryParameters.Add("Address", model.P_MW_ACK_LETTER.ADDRESS.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.BUILDING))
            {
                whereQ += "\r\n\t" + " AND BUILDING= :Building ";
                model.QueryParameters.Add("Building", model.P_MW_ACK_LETTER.BUILDING.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.STREET))
            {
                whereQ += "\r\n\t" + " AND STREET= :Street ";
                model.QueryParameters.Add("Street", model.P_MW_ACK_LETTER.STREET.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.STREET_NO))
            {
                whereQ += "\r\n\t" + " AND STREET_NO= :StreetNo ";
                model.QueryParameters.Add("StreetNo", model.P_MW_ACK_LETTER.STREET_NO.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.FLOOR))
            {
                whereQ += "\r\n\t" + " AND FLOOR= :Floor ";
                model.QueryParameters.Add("Floor", model.P_MW_ACK_LETTER.FLOOR.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.UNIT))
            {
                whereQ += "\r\n\t" + " AND UNIT= :Unit ";
                model.QueryParameters.Add("Unit", model.P_MW_ACK_LETTER.UNIT.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PAW))
            {
                whereQ += "\r\n\t" + " AND PAW= :PAW ";
                model.QueryParameters.Add("PAW", model.P_MW_ACK_LETTER.PAW.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PAW_CONTACT))
            {
                whereQ += "\r\n\t" + " AND PAW_CONTACT= :PAWContact ";
                model.QueryParameters.Add("PAWContact", model.P_MW_ACK_LETTER.PAW_CONTACT.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.IO_MGT))
            {
                whereQ += "\r\n\t" + " AND IO_MGT= :IOMGT ";
                model.QueryParameters.Add("IOMGT", model.P_MW_ACK_LETTER.IO_MGT.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.IO_MGT_CONTACT))
            {
                whereQ += "\r\n\t" + " AND IO_MGT_CONTACT= :IOMGTContact ";
                model.QueryParameters.Add("IOMGTContact", model.P_MW_ACK_LETTER.IO_MGT_CONTACT.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.REMARK))
            {
                whereQ += "\r\n\t" + " AND REMARK= :Remark ";
                model.QueryParameters.Add("Remark", model.P_MW_ACK_LETTER.REMARK.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.LANGUAGE))
            {
                whereQ += "\r\n\t" + " AND LANGUAGE= :Language ";
                model.QueryParameters.Add("Language", model.P_MW_ACK_LETTER.LANGUAGE.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.CompDate1))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMP_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')>TO_DATE(:comp1,'dd/MM/yyyy') ";
                model.QueryParameters.Add("comp1", model.CompDate1);
            }
            if (!string.IsNullOrWhiteSpace(model.CompDate2))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMP_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<TO_DATE(:comp2,'dd/MM/yyyy') ";
                model.QueryParameters.Add("comp2", model.CompDate2);
            }

            return whereQ;
        }

        public Fn01LM_SearchModel SearchLetter(Fn01LM_SearchModel model)
        {
            string lettersWhereq = SearchLetter_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.GetACKLettersDividedByMWNo(model, lettersWhereq);
        }

        public string SearchExcel(Fn01LM_SearchModel model)
        {
            model.QueryWhere = SearchLetter_whereQ(model);
            model = DA.GetSearchExcelData(model);
            Dictionary<string, string> col1 = model.CreateExcelColumn("DSN", "DSN");
            Dictionary<string, string> col2 = model.CreateExcelColumn("MW NO.", "MW_NO");
            Dictionary<string, string> col3 = model.CreateExcelColumn("FORM", "FORM_NO");
            Dictionary<string, string> col4 = model.CreateExcelColumn("RECEIVED DATE", "RECEIVED_DATE");
            Dictionary<string, string> col5 = model.CreateExcelColumn("LETTER DATE", "LETTER_DATE");
            Dictionary<string, string> col6 = model.CreateExcelColumn("PBP", "PBP_NO");
            Dictionary<string, string> col7 = model.CreateExcelColumn("ADDRESS", "ADDRESS");
            Dictionary<string, string> col8 = model.CreateExcelColumn("PRC", "PRC_NO");
            Dictionary<string, string> col9 = model.CreateExcelColumn("AUDIT(AFC)", "AUDIT_RELATED");
            Dictionary<string, string> col10 = model.CreateExcelColumn("AUDIT(SAC)", "SITE_AUDIT_RELATED");
            Dictionary<string, string> col11 = model.CreateExcelColumn("AUDIT(PSAC)", "PRE_SITE_AUDIT_RELATED");
            Dictionary<string, string> col12 = model.CreateExcelColumn("O", "ORDER_RELATED");
            Dictionary<string, string> col13 = model.CreateExcelColumn("S", "SIGNBOARD_RELATED");
            Dictionary<string, string> col14 = model.CreateExcelColumn("P", "SSP");
            model.Columns = new Dictionary<string, string>[]{
                col1
                ,col2
                ,col3
                ,col4
                ,col5
                ,col6
                ,col7
                ,col8
                ,col9
                ,col10
                ,col11
                ,col12
                ,col13
                ,col14
            };
            return model.ExportCurrentData("Search_" + DateTime.Now.ToString("yyyy-MM-dd"));
        }

        //private string SearchLetter_whereQ(Fn01LM_SearchModel model)
        //{
        //    string whereQ = "";
        //    if (model.P_MW_ACK_LETTER.COUNTER.ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND COUNTER=:Counter ";
        //        model.QueryParameters.Add("Counter", model.P_MW_ACK_LETTER.COUNTER.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.NATURE.ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND NATURE=:Nature ";
        //        model.QueryParameters.Add("Nature", model.P_MW_ACK_LETTER.NATURE.Trim().ToUpper());
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom);
        //        model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && string.IsNullOrWhiteSpace(model.ReceivedDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom);
        //        model.QueryParameters.Add("ReceivedDateTo", DateTime.Now.ToString());
        //    }
        //    if (string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo);
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.LetterDateFrom) && !string.IsNullOrWhiteSpace(model.LetterDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:LetterDateFrom,'dd/MM/yyyy') AND TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("LetterDateFrom", model.LetterDateFrom);
        //        model.QueryParameters.Add("LetterDateTo", model.LetterDateTo);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.LetterDateFrom) && string.IsNullOrWhiteSpace(model.LetterDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:LetterDateFrom,'dd/MM/yyyy') AND TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("LetterDateFrom", model.LetterDateFrom);
        //        model.QueryParameters.Add("LetterDateTo", DateTime.Now.ToString());
        //    }
        //    if (string.IsNullOrWhiteSpace(model.LetterDateFrom) && !string.IsNullOrWhiteSpace(model.LetterDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(LETTER_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:LetterDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("LetterDateTo", model.LetterDateTo);
        //    }
        //    if (model.IsLetterDateNull)
        //    {
        //        whereQ += "\r\n\t" + " AND LETTER_DATE IS NULL ";
        //    }
        //    else
        //    {
        //        whereQ += "\r\n\t" + " AND LETTER_DATE IS NOT NULL ";
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.CommDateFrom) && !string.IsNullOrWhiteSpace(model.CommDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:CommDateFrom,'dd/MM/yyyy') AND TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("CommDateFrom", model.CommDateFrom);
        //        model.QueryParameters.Add("CommDateTo", model.CommDateTo);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(model.CommDateFrom) && string.IsNullOrWhiteSpace(model.CommDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:CommDateFrom,'dd/MM/yyyy') AND TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("CommDateFrom", model.CommDateFrom);
        //        model.QueryParameters.Add("CommDateTo", DateTime.Now.ToString());
        //    }
        //    else if (string.IsNullOrWhiteSpace(model.CommDateFrom) && !string.IsNullOrWhiteSpace(model.CommDateTo))
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(COMM_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:CommDateTo,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("CommDateTo", model.CommDateTo);
        //    }

        //    if (model.IsCompDateNull)
        //    {
        //        whereQ += "\r\n\t" + " AND COMP_DATE IS NULL ";
        //    }
        //    else
        //    {
        //        whereQ += "\r\n\t" + " AND COMP_DATE IS NOT NULL ";
        //    }

        //    if (model.IsReferralDateNull)
        //    {
        //        whereQ += "\r\n\t" + " AND REFERRAL_DATE IS NULL ";
        //    }
        //    else
        //    {
        //        whereQ += "\r\n\t" + " AND REFERRAL_DATE IS NOT NULL ";
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.DSN))
        //    {
        //        whereQ += "\r\n\t" + " AND DSN like :DSN ";
        //        model.QueryParameters.Add("DSN", "%" + model.P_MW_ACK_LETTER.DSN.Trim().ToUpper() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.MW_NO))
        //    {
        //        whereQ += "\r\n\t" + " AND MW_NO LIKE :MWNo ";
        //        model.QueryParameters.Add("MWNo", "%" + model.P_MW_ACK_LETTER.MW_NO.Trim().ToUpper());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.FORM_NO))
        //    {
        //        whereQ += "\r\n\t" + " AND FORM_NO LIKE :FormNo ";
        //        model.QueryParameters.Add("FormNo", "%" + model.P_MW_ACK_LETTER.FORM_NO.Trim().ToUpper());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.MW_ITEM))
        //    {
        //        whereQ += "\r\n\t" + " AND MW_ITEM= :MWItem ";
        //        model.QueryParameters.Add("MWItem", model.P_MW_ACK_LETTER.MW_ITEM.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.AUDIT_RELATED.Trim().ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND AUDIT_RELATED= :AuditRelated ";
        //        model.QueryParameters.Add("AuditRelated", model.P_MW_ACK_LETTER.AUDIT_RELATED.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.ORDER_RELATED.Trim().ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND ORDER_RELATED= :OrderRelated ";
        //        model.QueryParameters.Add("OrderRelated", model.P_MW_ACK_LETTER.ORDER_RELATED.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.SDF_RELATED.Trim().ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND SDF_RELATED=:SDFRelated ";
        //        model.QueryParameters.Add("SDFRelated", model.P_MW_ACK_LETTER.SDF_RELATED.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.SIGNBOARD_RELATED.Trim().ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND SIGNBOARD_RELATED= :SignboardRelated ";
        //        model.QueryParameters.Add("SignboardRelated", model.P_MW_ACK_LETTER.SIGNBOARD_RELATED.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.SSP.Trim().ToUpper() != "ALL")
        //    {
        //        whereQ += "\r\n\t" + " AND SSP= :SSP ";
        //        model.QueryParameters.Add("SSP", model.P_MW_ACK_LETTER.SSP.Trim().ToUpper());
        //    }
        //    if (model.P_MW_ACK_LETTER.REFERRAL_DATE != null)
        //    {
        //        whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(REFERRAL_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')=TO_DATE(:ReferralDate,'dd/MM/yyyy') ";
        //        model.QueryParameters.Add("ReferralDate", model.P_MW_ACK_LETTER.REFERRAL_DATE.ToString());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PBP_NO))
        //    {
        //        switch (model.P_MW_ACK_LETTER.PBP_NO.Trim().ToUpper())
        //        {
        //            case "1":
        //                whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
        //                model.QueryParameters.Add("PBPNo", "AP(A)%");
        //                break;
        //            case "2":
        //                whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
        //                model.QueryParameters.Add("PBPNo", "AP(E)%");
        //                break;
        //            case "3":
        //                whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
        //                model.QueryParameters.Add("PBPNo", "AP(S)%");
        //                break;
        //            default:
        //                whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
        //                model.QueryParameters.Add("PBPNo", model.P_MW_ACK_LETTER.PBP_NO.Trim().ToUpper() + "%");
        //                break;
        //        }
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PRC_NO))
        //    {
        //        switch (model.P_MW_ACK_LETTER.PRC_NO.Trim().ToUpper())
        //        {
        //            case "1":
        //                whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
        //                model.QueryParameters.Add("PRCNo", "GBC%");
        //                break;
        //            case "2":
        //                whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
        //                model.QueryParameters.Add("PRCNo", "MWC%");
        //                break;
        //            case "3":
        //                whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
        //                model.QueryParameters.Add("PRCNo", "MWC(W)%");
        //                break;
        //            default:
        //                whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
        //                model.QueryParameters.Add("PRCNo", model.P_MW_ACK_LETTER.PRC_NO.Trim().ToUpper() + "%");
        //                break;
        //        }

        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.ADDRESS))
        //    {
        //        whereQ += "\r\n\t" + "  AND ADDRESS= :Address ";
        //        model.QueryParameters.Add("Address", model.P_MW_ACK_LETTER.ADDRESS.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.BUILDING))
        //    {
        //        whereQ += "\r\n\t" + " AND BUILDING= :Building ";
        //        model.QueryParameters.Add("Building", model.P_MW_ACK_LETTER.BUILDING.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.STREET))
        //    {
        //        whereQ += "\r\n\t" + " AND STREET= :Street ";
        //        model.QueryParameters.Add("Street", model.P_MW_ACK_LETTER.STREET.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.STREET_NO))
        //    {
        //        whereQ += "\r\n\t" + " AND STREET_NO= :StreetNo ";
        //        model.QueryParameters.Add("StreetNo", model.P_MW_ACK_LETTER.STREET_NO.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.FLOOR))
        //    {
        //        whereQ += "\r\n\t" + " AND FLOOR= :Floor ";
        //        model.QueryParameters.Add("Floor", model.P_MW_ACK_LETTER.FLOOR.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.UNIT))
        //    {
        //        whereQ += "\r\n\t" + " AND UNIT= :Unit ";
        //        model.QueryParameters.Add("Unit", model.P_MW_ACK_LETTER.UNIT.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PAW))
        //    {
        //        whereQ += "\r\n\t" + " AND PAW= :PAW ";
        //        model.QueryParameters.Add("PAW", model.P_MW_ACK_LETTER.PAW.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.PAW_CONTACT))
        //    {
        //        whereQ += "\r\n\t" + " AND PAW_CONTACT= :PAWContact ";
        //        model.QueryParameters.Add("PAWContact", model.P_MW_ACK_LETTER.PAW_CONTACT.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.IO_MGT))
        //    {
        //        whereQ += "\r\n\t" + " AND IO_MGT= :IOMGT ";
        //        model.QueryParameters.Add("IOMGT", model.P_MW_ACK_LETTER.IO_MGT.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.IO_MGT_CONTACT))
        //    {
        //        whereQ += "\r\n\t" + " AND IO_MGT_CONTACT= :IOMGTContact ";
        //        model.QueryParameters.Add("IOMGTContact", model.P_MW_ACK_LETTER.IO_MGT_CONTACT.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.REMARK))
        //    {
        //        whereQ += "\r\n\t" + " AND REMARK= :Remark ";
        //        model.QueryParameters.Add("Remark", model.P_MW_ACK_LETTER.REMARK.Trim());
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.LANGUAGE))
        //    {
        //        whereQ += "\r\n\t" + " AND LANGUAGE= :Language ";
        //        model.QueryParameters.Add("Language", model.P_MW_ACK_LETTER.LANGUAGE.Trim());
        //    }

        //    return whereQ;
        //}

        //public Fn01LM_SearchModel SearchLetter(Fn01LM_SearchModel model)
        //{
        //    model.QueryWhere = SearchLetter_whereQ(model);
        //    return DA.SearchLetter(model);
        //    //return DA.SearchLetter(model, SearchLetter_whereQ(model));
        //}
        #endregion

        #region Order

        private string SearchOrder_whereQ(Fn01LM_SearchModel model)
        {
            string whereQ = "";

            whereQ += @" and ORDER_RELATED = 'Y' ";

            if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
            {
                whereQ += @" and (MW_NO like :MWNo or DSN like :DSN) ";
                model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
                model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
            }

            return whereQ;
        }
        public Fn01LM_SearchModel SearchOrder(Fn01LM_SearchModel model)
        {
            string lettersWhereq = SearchOrder_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.GetACKLettersDividedByMWNo(model, lettersWhereq);
        }

        //private string SearchOrder_whereQ(Fn01LM_OrderModel model)
        //{
        //    string whereQ = "";

        //    whereQ += @" and ORDER_RELATED = 'Y' ";

        //    if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
        //    {
        //        whereQ += @" and MW_NO like :MWNo or DSN like :DSN ";
        //        model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
        //        model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
        //    }

        //    return whereQ;
        //}
        //public Fn01LM_OrderModel SearchOrder(Fn01LM_OrderModel model)
        //{
        //    model.QueryWhere = SearchOrder_whereQ(model);
        //    return DA.SearchOrder(model);
        //    //return DA.searchOrder(model, SearchOrder_whereQ(model));
        //}

        #endregion

        #region Signboard

        //private string SearchSignboard_whereQ(Fn01LM_SignboardModel model)
        //{
        //    string whereQ = "";

        //    whereQ += @"  and signboard_related = 'Y'  ";

        //    if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
        //    {
        //        whereQ += @" and MW_NO like :MWNo or DSN like :DSN ";
        //        model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
        //        model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
        //    }

        //    return whereQ;
        //}
        //public Fn01LM_SignboardModel SearchSignboard(Fn01LM_SignboardModel model)
        //{
        //    model.QueryWhere = SearchSignboard_whereQ(model);
        //    return DA.SearchSignboard(model);
        //    //return DA.SearchSignboard(model, SearchSignboard_whereQ(model));
        //}

        private string SearchSignboard_whereQ(Fn01LM_SearchModel model)
        {
            string whereQ = "";

            whereQ += @"  and signboard_related = 'Y'  ";

            if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
            {
                whereQ += @" and (MW_NO like :MWNo or DSN like :DSN) ";
                model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
                model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
            }

            return whereQ;
        }
        public Fn01LM_SearchModel SearchSignboard(Fn01LM_SearchModel model)
        {
            string lettersWhereq = SearchSignboard_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.GetACKLettersDividedByMWNo(model, lettersWhereq);
        }

        #endregion

        #region Minor Works List

        private string SearchMWList_whereQ(Fn01LM_SearchModel model)
        {
            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
            {
                whereQ += @" and (MW_NO like :MWNo or DSN like :DSN) ";
                model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
                model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
            }

            return whereQ;
        }
        //private string SearchMWList_whereQ(Fn01LM_MWListModel model)
        //{
        //    string whereQ = "";

        //    if (!string.IsNullOrWhiteSpace(model.DSNOrMWNo))
        //    {
        //        whereQ += @" and MW_NO like :MWNo or DSN like :DSN ";
        //        model.QueryParameters.Add("MWNo", "%" + model.DSNOrMWNo + "%");
        //        model.QueryParameters.Add("DSN", "%" + model.DSNOrMWNo + "%");
        //    }

        //    return whereQ;
        //}

        //public List<Fn01LM_SearchResultModel> SearchMWList(Fn01LM_MWListModel model)
        //{
        //    return DA.SearchMWList(model, SearchMWList_whereQ(model));
        //}

        //public Fn01LM_MWListModel SearchMWList(Fn01LM_MWListModel model)
        //{
        //    model.QueryWhere = SearchMWList_whereQ(model);
        //    return DA.SearchMWList(model);
        //}
        public Fn01LM_SearchModel SearchMWList(Fn01LM_SearchModel model)
        {
            string lettersWhereq = SearchMWList_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.GetACKLettersDividedByMWNo(model, lettersWhereq);
        }
        #endregion

        #region Audit List Management

        private string SearchALM_whereQ(Fn01LM_ALMSearchModel model)
        {
            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.MWItem))
            {
                whereQ += @" and mw_item like :MWItem ";
                model.QueryParameters.Add("MWItem", "%" + model.MWItem.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.PBP))
            {
                switch (model.PBP.Trim().ToUpper())
                {
                    case "1":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(A)%");
                        break;
                    case "2":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(E)%");
                        break;
                    case "3":
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", "AP(S)%");
                        break;
                    default:
                        whereQ += "\r\n\t" + " AND PBP_NO like :PBPNo ";
                        model.QueryParameters.Add("PBPNo", model.PBP.Trim().ToUpper() + "%");
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(model.PRC))
            {
                switch (model.PRC.Trim().ToUpper())
                {
                    case "1":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "GBC%");
                        break;
                    case "2":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "MWC%");
                        break;
                    case "3":
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", "MWC(W)%");
                        break;
                    default:
                        whereQ += "\r\n\t" + " AND PRC_NO like :PRCNo ";
                        model.QueryParameters.Add("PRCNo", model.PRC.Trim().ToUpper() + "%");
                        break;
                }

            }

            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom.Trim());
                model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') BETWEEN TO_DATE(:ReceivedDateFrom,'dd/MM/yyyy') AND TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceivedDateFrom.Trim());
                model.QueryParameters.Add("ReceivedDateTo", DateTime.Now.ToString());
            }
            if (string.IsNullOrWhiteSpace(model.ReceivedDateFrom) && !string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                whereQ += "\r\n\t" + " AND TO_DATE(TO_CHAR(RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') < TO_DATE(:ReceivedDateTo,'dd/MM/yyyy') ";
                model.QueryParameters.Add("ReceivedDateTo", model.ReceivedDateTo.Trim());
            }

            return whereQ;
        }

        public Fn01LM_ALMSearchModel SearchALM(Fn01LM_ALMSearchModel model)
        {
            //model.QueryWhere = SearchALM_whereQ(model);
            //return DA.SearchALM(model);

            string lettersWhereq = SearchALM_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.SearchALM(model, lettersWhereq);
        }

        //private string SearchClass1And2_whereQ(Fn01LM_ALMSearchModel model)
        //{
        //    StringBuilder whereQ = new StringBuilder();

        //    return whereQ.ToString();
        //}

        //public Fn01LM_ALMSearchModel SearchClass1And2(Fn01LM_ALMSearchModel model)
        //{
        //    model.QueryWhere = SearchClass1And2_whereQ(model);
        //    return DA.SearchALM(model);
        //}

        public ServiceResult PickAuditAFC(string id)
        {
            return DA.PickAuditAFC(id);
        }

        public ServiceResult PickAuditSAC(string id)
        {
            return DA.PickAuditSAC(id);
        }

        public ServiceResult PickAuditPSAC(string id)
        {
            return DA.PickAuditPSAC(id);
        }

        public ServiceResult PickAudit(Fn01LM_ALMSearchModel model)
        {
            string lettersWhereq = SearchALM_whereQ(model);
            model.QueryWhere = lettersWhereq;
            return DA.PickAudit(model, lettersWhereq);
        }

        private string SearchCompletion_whereQ()
        {
            string whereQ = "";

            return whereQ;
        }

        public Fn01LM_ALMSearchModel Completion(Fn01LM_ALMSearchModel model)
        {
            model.QueryWhere = SearchCompletion_whereQ();
            model.Sort = " ack.MODIFIED_DATE ";
            model.SortType = 1;
            return DA.Completion(model);
        }

        public ValidationResult Validaton_DSN(string dsn, string propName)
        {
            int result = DA.Validaton_DSN(dsn);
            if (result == 0)
                return new ValidationResult("Document S/N not found.", new List<string> { propName });
            else if (result == -1)
                return new ValidationResult(string.Format("The submissions ralated to this DSN {0} has been updated", dsn), new List<string> { propName });
            else if (result == -2)
                return new ValidationResult("Manual pick audit fail for order related submisison", new List<string> { propName });
            else
                // valid
                return null;
        }

        public ServiceResult UpdateAuditRelated(Fn01LM_ALMSearchModel model)
        {
            return DA.UpdateAuditRelated(model);
        }

        #endregion

        #region Advisory Letters
        public ServiceResult GetPOInfo(string popost)
        {
            return DA.GetPOInfo(popost);
        }

        public string SearchALListWhereq(Fn01LM_ALSearchModel model)
        {
            StringBuilder whereq = new StringBuilder();
            //if (!string.IsNullOrWhiteSpace(model.P_S_TO_DETAILS.PO_POST))
            //{
            //    whereq.Append(" and PO= :PO ");
            //    model.QueryParameters.Add("PO", model.P_S_TO_DETAILS.PO_NAME_CHI);
            //}
            if (model.IsSearchDSN)
            {
                if (!string.IsNullOrWhiteSpace(model.SearchDSN))
                {
                    whereq.Append(" and AL.DSN= :DSN ");
                    model.QueryParameters.Add("DSN", model.SearchDSN.Trim());
                }
                return whereq.ToString();
            }

            if (model.P_S_TO_DETAILS != null && !string.IsNullOrWhiteSpace(model.P_S_TO_DETAILS.PO_NAME_CHI))
            {
                whereq.Append(" and TD.PO_NAME_CHI Like :PO_NAME_CHI ");
                model.QueryParameters.Add("PO_NAME_CHI", "%" + model.P_S_TO_DETAILS.PO_NAME_CHI.Trim() + "%");
            }

            if (model.P_MW_ACK_LETTER != null && !string.IsNullOrWhiteSpace(model.P_MW_ACK_LETTER.DSN))
            {
                whereq.Append(" and AL.DSN= :DSN ");
                model.QueryParameters.Add("DSN", model.P_MW_ACK_LETTER.DSN.Trim());
            }

            if (model.P_S_TO_DETAILS != null && !string.IsNullOrWhiteSpace(model.P_S_TO_DETAILS.PO_CONTACT))
            {
                whereq.Append(" and TD.PO_CONTACT= :PO_CONTACT ");
                model.QueryParameters.Add("PO_CONTACT", model.P_S_TO_DETAILS.PO_CONTACT.Trim());
            }

            return whereq.ToString();
        }
        public Fn01LM_ALSearchModel GetALList(Fn01LM_ALSearchModel model)
        {
            model.QueryWhere = SearchALListWhereq(model);
            return DA.GetALList(model);
        }

        public string ExportALListExcel(Fn01LM_ALSearchModel model)
        {
            model.QueryWhere = SearchALListWhereq(model);
            return DA.ExportALListExcel(model);
        }

        public void removeFromAlList(Fn01LM_ALDisplayModel model)
        {
            DA.setInAlList(model.P_MW_ACK_LETTER.UUID, ProcessingConstant.FLAG_N);
        }

        public ServiceResult ValidateAlDSN(string dsn)
        {
            P_MW_ACK_LETTER record = string.IsNullOrEmpty(dsn) ? null : ackLetterService.GetP_MW_ACK_LETTERByDsn(dsn);

            return new ServiceResult()
            {
                Result = record == null ? ServiceResult.RESULT_FAILURE : record.IN_AL_LIST == "Y" ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE
            };
        }

        public ServiceResult AddAdvisoryLetter(Fn01LM_ALSearchModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Check DSN
                        P_MW_ACK_LETTER record = ackLetterService.GetP_MW_ACK_LETTERByDsn(model.P_MW_ACK_LETTER.DSN);

                        if (record == null)
                        {
                            serviceResult.Result = ServiceResult.RESULT_FAILURE;
                            serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "P_MW_ACK_LETTER.DSN", new List<string>() { "DSN NOT FOUND" } } };
                            return serviceResult;
                        }

                        if (record.IN_AL_LIST == "Y")
                        {
                            serviceResult.Result = ServiceResult.RESULT_FAILURE;
                            serviceResult.ErrorMessages = new Dictionary<string, List<string>>() { { "P_MW_ACK_LETTER.DSN", new List<string>() { "The DSN has been added to list" } } };
                            return serviceResult;
                        }

                        //Update P_MW_ACK_LETTER
                        record.IN_AL_LIST = "Y";
                        record.PO_POST = model.P_S_TO_DETAILS.PO_POST;

                        DA.UpdateALInfo(record, db);

                        db.SaveChanges();
                        tran.Commit();
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;

                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        AuditLogService.logDebug(e);
                    }
                }
            }

            return serviceResult;
        }
        #endregion

        // Load parent Ack Submission
        public void LoadParent(P_MW_ACK_LETTER pMwAckLetter)
        {
            if (pMwAckLetter == null) { return; }

            // get parent Ack Letter by MwNo
            List<P_MW_ACK_LETTER> ackList = LoadAckLetterListByMwRefNo(pMwAckLetter.MW_NO);


            // Select commencement form
            P_MW_ACK_LETTER ackLetterCommencementForm = null;
            if (ackList != null && ackList.Count > 0)
            {
                ackLetterCommencementForm = ackList.ElementAt(0);
            }

            // copy data from commencement form
            if (ackLetterCommencementForm != null)
            {
                SetRelatedData(pMwAckLetter, ackLetterCommencementForm);
            }

            // Update lastest PBP & PRC
            for (int i = 0; i < ackList.Count; i++)
            {
                P_MW_ACK_LETTER ackLetter = ackList.ElementAt(i);
                String formNo = ackLetter.FORM_NO;
                if (ProcessingConstant.FORM_07.Equals(formNo) || ProcessingConstant.FORM_08.Equals(formNo) || ProcessingConstant.FORM_09.Equals(formNo))
                {
                    SetPbpPrc(pMwAckLetter, ackLetter);
                }
            }

            /*
            if (ProcessingConstant.FORM_MW02.Equals(pMwAckLetter.FORM_NO))
            {
                SetRelatedData(pMwAckLetter, LoadAckLetterListByMwRefNo(pMwAckLetter.MW_NO).Where(m => m.FORM_NO == ProcessingConstant.FORM_MW01).FirstOrDefault());
            }
            else
            {
                for (int i = 0; i < ackList.Count; i++)
                {
                    P_MW_ACK_LETTER parentAckLetter = ackList.ElementAt(i);
                    SetRelatedData(pMwAckLetter, parentAckLetter);
                }
            }
            */

        }

        // load list of Ack Submission by MW Ref No.
        public List<P_MW_ACK_LETTER> LoadAckLetterListByMwRefNo(String mwNo)
        {
            List<P_MW_ACK_LETTER> ackList = null;

            // Get List of Ack Letter by MwRefNo
            // ******* 請完成這 *********
            // Sample Sql: "select * from p_mw_ack_letter where mw_no = <baseMWNo> order by FORM_NO asc, Modified_date asc"

            ackList = DA.GetAckLetter(mwNo).ToList();

            return ackList;
        }

        // set parent Ack Letter to base Ack Letter
        public void SetRelatedData(P_MW_ACK_LETTER baseAckLetter, P_MW_ACK_LETTER parentAckLetter)
        {
            if (baseAckLetter == null)
            {
                return;
            }

            // get base submission Form No
            String baseFormNo = baseAckLetter.FORM_NO;

            // copy audit result from parent
            if (ProcessingConstant.FLAG_Y.Equals(parentAckLetter.AUDIT_RELATED))
            {
                baseAckLetter.AUDIT_RELATED = parentAckLetter.AUDIT_RELATED;
                baseAckLetter.SITE_AUDIT_RELATED = parentAckLetter.SITE_AUDIT_RELATED;
            }

            // set commencement date for complation form
            if (ProcessingConstant.FORM_MW02.Equals(baseFormNo) && ProcessingConstant.FORM_MW01.Equals(parentAckLetter.FORM_NO) ||
                ProcessingConstant.FORM_MW04.Equals(baseFormNo) && ProcessingConstant.FORM_MW03.Equals(parentAckLetter.FORM_NO))
            {
                SetCommDate(baseAckLetter, parentAckLetter);
            }

            if (ProcessingConstant.FORM_MW02.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW04.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW07.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW08.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW09.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW10.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW11.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW12.Equals(baseFormNo) ||
                ProcessingConstant.FORM_MW33.Equals(baseFormNo))
            {
                // call common method set data from parent form
                SetPbpPrcAndAddressAndOtherInfo(baseAckLetter, parentAckLetter);
            }

            if (ProcessingConstant.FORM_MW11.Equals(baseFormNo) ||
                     ProcessingConstant.FORM_MW12.Equals(baseFormNo))
            {
                // check if MW Item related to signboard or SDF
                SetSignboardAndSDFRealted(baseAckLetter);
            }

            // Special handle for New VS Form
            if ((ProcessingConstant.FORM_MW06_01.Equals(baseFormNo) || ProcessingConstant.FORM_MW06_02.Equals(baseFormNo))
                && parentAckLetter == null)
            {
                // check if MW Item related to signboard or SDF
                SetSignboardAndSDFRealted(baseAckLetter);
            }
            else if ((ProcessingConstant.FORM_MW06_01.Equals(baseFormNo) || ProcessingConstant.FORM_MW06_02.Equals(baseFormNo))
              && parentAckLetter != null)
            {
                // call common method set data from parent form
                SetPbpPrcAndAddressAndOtherInfo(baseAckLetter, parentAckLetter);
            }


        }

        public void SetCommDate(P_MW_ACK_LETTER baseAckLetter, P_MW_ACK_LETTER parentAckLetter)
        {
            if (baseAckLetter == null || parentAckLetter == null) { return; }

            if (null == baseAckLetter.COMM_DATE) { baseAckLetter.COMM_DATE = parentAckLetter.COMM_DATE; }
        }

        public void SetPbpPrc(P_MW_ACK_LETTER baseAckLetter, P_MW_ACK_LETTER parentAckLetter)
        {
            if (baseAckLetter == null) { return; }
            if (parentAckLetter == null) { return; }

            String baseMWNo = baseAckLetter.MW_NO;
            String updatedPbpNo = parentAckLetter.PBP_NO;
            String updatedPrcNo = parentAckLetter.PRC_NO;

            // Set PBP and PRC from parent form
            if (!String.IsNullOrEmpty(updatedPbpNo))
            {
                baseAckLetter.PBP_NO = updatedPbpNo;
            }
            if (!String.IsNullOrEmpty(updatedPbpNo))
            {
                baseAckLetter.PRC_NO = updatedPrcNo;
            }
        }

        public void SetPbpPrcAndAddressAndOtherInfo(P_MW_ACK_LETTER baseAckLetter, P_MW_ACK_LETTER parentAckLetter)
        {
            if (baseAckLetter == null) { return; }
            if (parentAckLetter == null) { return; }

            String baseMWNo = baseAckLetter.MW_NO;
            String updatedPbpNo = parentAckLetter.PBP_NO;
            String updatedPrcNo = parentAckLetter.PRC_NO;

            // Set PBP and PRC from parent form
            if (String.IsNullOrEmpty(baseAckLetter.PBP_NO) && !String.IsNullOrEmpty(updatedPbpNo))
            {
                baseAckLetter.PBP_NO = updatedPbpNo;
            }
            if (String.IsNullOrEmpty(baseAckLetter.PRC_NO) && !String.IsNullOrEmpty(updatedPrcNo))
            {
                baseAckLetter.PRC_NO = updatedPrcNo;
            }

            // set full address/Structural address
            baseAckLetter.ADDRESS = (String.IsNullOrEmpty(baseAckLetter.ADDRESS) ? parentAckLetter.ADDRESS : baseAckLetter.ADDRESS);
            baseAckLetter.STREET = (String.IsNullOrEmpty(baseAckLetter.STREET) ? parentAckLetter.STREET : baseAckLetter.STREET);
            baseAckLetter.STREET_NO = (String.IsNullOrEmpty(baseAckLetter.STREET_NO) ? parentAckLetter.STREET_NO : baseAckLetter.STREET_NO);
            baseAckLetter.BUILDING = (String.IsNullOrEmpty(baseAckLetter.BUILDING) ? parentAckLetter.BUILDING : baseAckLetter.BUILDING);
            baseAckLetter.FLOOR = (String.IsNullOrEmpty(baseAckLetter.FLOOR) ? parentAckLetter.FLOOR : baseAckLetter.FLOOR);
            baseAckLetter.UNIT = (String.IsNullOrEmpty(baseAckLetter.UNIT) ? parentAckLetter.UNIT : baseAckLetter.UNIT);

            // set other info
            baseAckLetter.PAW = (String.IsNullOrEmpty(baseAckLetter.PAW) ? parentAckLetter.PAW : baseAckLetter.PAW);
            baseAckLetter.PAW_CONTACT = (String.IsNullOrEmpty(baseAckLetter.PAW_CONTACT) ? parentAckLetter.PAW_CONTACT : baseAckLetter.PAW_CONTACT);
            baseAckLetter.LANGUAGE = parentAckLetter.LANGUAGE;
            baseAckLetter.IO_MGT = (String.IsNullOrEmpty(baseAckLetter.IO_MGT) ? parentAckLetter.IO_MGT : baseAckLetter.IO_MGT);
            baseAckLetter.IO_MGT_CONTACT = (String.IsNullOrEmpty(baseAckLetter.IO_MGT_CONTACT) ? parentAckLetter.IO_MGT_CONTACT : baseAckLetter.IO_MGT_CONTACT);
            baseAckLetter.REMARK = (String.IsNullOrEmpty(baseAckLetter.REMARK) ? parentAckLetter.REMARK : baseAckLetter.REMARK);

            // set order related
            baseAckLetter.ORDER_RELATED = ProcessingConstant.FLAG_Y.Equals(parentAckLetter.ORDER_RELATED) ?
                ProcessingConstant.FLAG_Y : ProcessingConstant.FLAG_N;
            baseAckLetter.SSP = ProcessingConstant.FLAG_Y.Equals(parentAckLetter.SSP) ?
                ProcessingConstant.FLAG_Y : ProcessingConstant.FLAG_N;
        }

        public void SetSignboardAndSDFRealted(P_MW_ACK_LETTER baseAckLetter)
        {
            if (baseAckLetter == null) { return; }

            ProcessingSystemValueDAOService pSystemValueDaoService = new ProcessingSystemValueDAOService();
            P_S_SYSTEM_VALUE svSignBoardItem = pSystemValueDaoService.GetSSystemValueByTypeAndCode(ProcessingConstant.TYPE_S_MW_ITEM, ProcessingConstant.CODE_SIGNBOARD_ITEMS);
            P_S_SYSTEM_VALUE svSDFItem = pSystemValueDaoService.GetSSystemValueByTypeAndCode(ProcessingConstant.TYPE_S_MW_ITEM, ProcessingConstant.CODE_SDF_ITEMS);

            string[] SignboardItemList = svSignBoardItem.DESCRIPTION.Split(',');
            string[] SDFItemList = svSignBoardItem.DESCRIPTION.Split(',');

            string[] inputMwItemList = new string[0];
            if (!stringUtil.isBlank(baseAckLetter.MW_ITEM))
            {
                inputMwItemList = baseAckLetter.MW_ITEM.Split('/');
            }

            // set signboard related
            baseAckLetter.SIGNBOARD_RELATED = ProcessingConstant.FLAG_N;
            foreach (String item in inputMwItemList)
            {
                if (SignboardItemList.Contains(item))
                {
                    baseAckLetter.SIGNBOARD_RELATED = ProcessingConstant.FLAG_Y;
                    break;
                }
            }

            // set SDF related
            baseAckLetter.SDF_RELATED = ProcessingConstant.FLAG_N;
            foreach (String item in inputMwItemList)
            {
                if (SDFItemList.Contains(item))
                {
                    baseAckLetter.SDF_RELATED = ProcessingConstant.FLAG_Y;
                    break;
                }
            }
        }

        #region Statistics
        public List<P_S_SYSTEM_VALUE> SearchMWSubmission()
        {
            return SystemValueDA.GetSSystemValueByType(ProcessingConstant.S_TYPE_SDM_SUBMISSION);
        }



        public Fn01LM_StatisticsModel SearchStatistics(Fn01LM_StatisticsModel model)
        {
            /*Default Value*/
            if (model.Month == null)
                model.Month = DateTime.Now.Month.ToString();
            if (model.Year == null)
                model.Year = DateTime.Now.ToString("yyyy");
            if (model.SSRModel == null)
            {
                model.SSRModel = new SearchSubmissionReceivedModel()
                {
                    ReceivedDateFrom = new DateTime(DateTime.Today.Year, 1, 1).ToString("dd/MM/yyyy")
                    ,
                    ReceivedDateTo = DateTime.Now.ToString("dd/MM/yyyy")
                };
            }
            model.SSRModel.LetterDateFrom = model.SSRModel.ReceivedDateFrom;
            model.SSRModel.LetterDateTo = model.SSRModel.ReceivedDateTo;
            model.SSRModel.ReferralDateFrom = model.SSRModel.ReceivedDateFrom;
            model.SSRModel.ReferralDateTo = model.SSRModel.ReceivedDateTo;

            string AllNatureList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL + "'";


            GetIncomingOutgoingParameterModel pModel = new GetIncomingOutgoingParameterModel()
            {
                YearMonth = model.Year + (model.Month.Length == 1 ? "0" : "") + model.Month
               ,
                PCCounter = ProcessingConstant.SUBMISSION_VALUE_COUNTER_VALUE_PC
               ,
                KTCounter = ProcessingConstant.SUBMISSION_VALUE_COUNTER_VALUE_KT
               ,
                ECounter = ProcessingConstant.SUBMISSION_VALUE_COUNTER_VALUE_ES
               ,
                WKGOCounter = ProcessingConstant.SUBMISSION_VALUE_COUNTER_VALUE_WKGO
               ,
                RecNatureList = AllNatureList
               ,
                DirectandReviseList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN + "','"
                                          + ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE + "'"
               ,
                CRNatureList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
                                   + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "'"
               ,
                ICUNatureList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_ICU + "'"
               ,
                WKGNatureList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "'"
               /*Outgoing Parameter*/
               ,
                LetterDateFrom = model.Year + "/" + model.Month + "/01"
               ,
                LetterDateTo = Convert.ToDateTime(model.Year + "/" + model.Month + "/01").AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd")
               ,
                YearStartDate = "2014/01/01"
               ,
                MonthYear = (model.Month.Length == 1 ? "0" : "") + model.Month + " " + model.Year
            };

            /*Default Value*/

            if (model.View == "Incoming")
            {
                return StatisticsIncoming(model, pModel);
            }
            else if (model.View == "Outgoing")
            {
                return StatisticsOutgoing(model, pModel);
            }
            else
            {
                return StatisticsSDM(model);
            }

        }

        public void IncomingOutgoingCommonTable(Fn01LM_StatisticsModel model, GetIncomingOutgoingParameterModel pModel)
        {
            model.TypeReceivedTableModel = DA.GetIncomingOutgoingTypeReceivedTable(pModel);
            foreach (var item in model.TypeReceivedTableModel)
            {
                model.TotalCountOfAuditSelected += item.AUDIT_COUNT;
                model.TotalCountOfSubmission += item.TOTAL_COUNT;
            }
            model.PercentageOfSubmissionSelected = model.TotalCountOfSubmission == 0 ? "0" : (Math.Round(((double)model.TotalCountOfAuditSelected / (double)model.TotalCountOfSubmission), 4) * 100).ToString();
            model.AccumulatedTotalSubmission = DA.GetAccumulatedTotalSubminssion(pModel);
        }

        public Fn01LM_StatisticsModel StatisticsIncoming(Fn01LM_StatisticsModel model, GetIncomingOutgoingParameterModel pModel)
        {
            IncomingOutgoingCommonTable(model, pModel);
            var incomingList = GetStatisticsIncomingCalendarData(pModel);
            model.IncomingModel = new List<Fn01LM_StatisticsIncomingListModel>();
            model.IncomingWeeklySummaryModel = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList1 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList1.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList2 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList2.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList3 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList3.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList4 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList4.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList5 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList5.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            var incomingModelList6 = new Fn01LM_StatisticsIncomingListModel(); incomingModelList6.IncomingModelList = new List<Fn01LM_StatisticsIncomingModel>();
            foreach (var item in incomingList)
            {
                int incominngWhichWeek = WeekOfMonth(Convert.ToDateTime(item.RECEIVED_DATE), 1);
                switch (incominngWhichWeek)
                {
                    case 1:
                        incomingModelList1.IncomingModelList.Add(item);
                        break;
                    case 2:
                        incomingModelList2.IncomingModelList.Add(item);
                        break;
                    case 3:
                        incomingModelList3.IncomingModelList.Add(item);
                        break;
                    case 4:
                        incomingModelList4.IncomingModelList.Add(item);
                        break;
                    case 5:
                        incomingModelList5.IncomingModelList.Add(item);
                        break;
                    case 6:
                        incomingModelList6.IncomingModelList.Add(item);
                        break;

                }
            }
            if (incomingModelList1.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList1);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList1));

            }
            if (incomingModelList2.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList2);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList2));
            }
            if (incomingModelList3.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList3);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList3));
            }
            if (incomingModelList4.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList4);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList4));
            }
            if (incomingModelList5.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList5);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList5));
            }

            if (incomingModelList6.IncomingModelList.Count > 0)
            {
                model.IncomingModel.Add(incomingModelList6);
                model.IncomingWeeklySummaryModel.Add(CalculateIncomingWeeklySummary(incomingModelList6));
            }
            return model;
        }

        public Fn01LM_StatisticsModel StatisticsOutgoing(Fn01LM_StatisticsModel model, GetIncomingOutgoingParameterModel pModel)
        {
            IncomingOutgoingCommonTable(model, pModel);
            //var outgoingList = DA.GetStatisticsOutgoingCalendarData(pModel).Where(x => x.WEEK_DAY != 6 && x.WEEK_DAY != 0).ToList();
            var outgoingList = DA.GetStatisticsOutgoingCalendarData(pModel).ToList();
            model.OutgoingModel = new List<Fn01LM_StatisticsOutgoingListModel>();
            model.OutgoingWeeklySummaryModel = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList1 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList1.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList2 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList2.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList3 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList3.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList4 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList4.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList5 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList5.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            var outgoingModelList6 = new Fn01LM_StatisticsOutgoingListModel(); outgoingModelList6.OutgoingModelList = new List<Fn01LM_StatisticsOutgoingModel>();
            foreach (var item in outgoingList)
            {
                int outgoingWhichWeek = WeekOfMonth(Convert.ToDateTime(item.DATERANGE), 1);
                switch (outgoingWhichWeek)
                {
                    case 1:
                        outgoingModelList1.OutgoingModelList.Add(item);
                        break;
                    case 2:
                        outgoingModelList2.OutgoingModelList.Add(item);
                        break;
                    case 3:
                        outgoingModelList3.OutgoingModelList.Add(item);
                        break;
                    case 4:
                        outgoingModelList4.OutgoingModelList.Add(item);
                        break;
                    case 5:
                        outgoingModelList5.OutgoingModelList.Add(item);
                        break;
                    case 6:
                        outgoingModelList6.OutgoingModelList.Add(item);
                        break;

                }
            }
            if (outgoingModelList1.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList1);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList1));

            }
            if (outgoingModelList2.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList2);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList2));
            }
            if (outgoingModelList3.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList3);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList3));
            }
            if (outgoingModelList4.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList4);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList4));
            }
            if (outgoingModelList5.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList5);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList5));
            }
            if (outgoingModelList6.OutgoingModelList.Count > 0)
            {
                model.OutgoingModel.Add(outgoingModelList6);
                model.OutgoingWeeklySummaryModel.Add(CalculateOutgoingWeeklySummary(outgoingModelList6));
            }
            return model;
        }

        public Fn01LM_StatisticsModel StatisticsSDM(Fn01LM_StatisticsModel model)
        {
            model.SubmissionList = SearchMWSubmission().OrderBy(x => x.CODE).ToList();
            var SDMList = GetMWSubmissionData(model.SSRModel);
            model.ReceivedCount = SDMList["ReceivedCount"];
            model.AcknowledgedOverCounter = SDMList["AcknowledgedOverCounter"];
            model.AcknowledgedByFax = SDMList["AcknowledgedByFax"];
            model.ReturnedByFaxNote = SDMList["ReturnedByFaxNote"];
            model.ReturnedOverCounter = SDMList["ReturnedOverCounter"];
            model.WeeklyAverage = Convert.ToDateTime(model.SSRModel.ReceivedDateTo).Subtract(Convert.ToDateTime(model.SSRModel.ReceivedDateFrom)).Days;
            model.ReceivedWeekly = StatisticsCalculateWeekly(model.ReceivedCount, model.WeeklyAverage);
            model.AcknowledgedOverCounterWeekly = StatisticsCalculateWeekly(model.AcknowledgedOverCounter, model.WeeklyAverage);
            model.AcknowledgedByFaxWeekly = StatisticsCalculateWeekly(model.AcknowledgedByFax, model.WeeklyAverage);
            model.ReturnedByFaxNoteWeekly = StatisticsCalculateWeekly(model.ReturnedByFaxNote, model.WeeklyAverage);
            model.ReturnedOverCounterWeekly = StatisticsCalculateWeekly(model.ReturnedOverCounter, model.WeeklyAverage);
            model.C112Value = SDMList["C112Value"];
            model.C112Percent = Math.Round(Convert.ToDouble(model.C112Value) / Convert.ToDouble(model.ReceivedCount) * 100, 2);
            double totalC113Class = SDMList["C113Class1Value"] + SDMList["C113Class2Value"] + SDMList["C113Class3Value"];
            model.C113Class1 = Math.Round((Convert.ToDouble(SDMList["C113Class1Value"]) / totalC113Class) * 100);
            model.C113Class2 = Math.Round((Convert.ToDouble(SDMList["C113Class2Value"]) / totalC113Class) * 100);
            model.C113Class3 = 100 - model.C113Class1 - model.C113Class2;
            var C114Result = DA.GetC114ValidationScheme();
            model.SDMValidationSchemeModel = new List<Fn01LM_StatisticsSDMValidationSchemeModel>();
            foreach (var item in C114Result)
            {
                string[] sArray = Regex.Split(item.DESCRIPTION, "CustomDSplit", RegexOptions.IgnoreCase);
                model.SDMValidationSchemeModel.Add(new Fn01LM_StatisticsSDMValidationSchemeModel()
                {
                    Period = sArray[0] + " - " + sArray[1]
                        ,
                    NoOfStructiresValidated = Convert.ToInt32(sArray[2])
                }
                );
            }
            model.SDMParticularItemModel = DA.GetSDMParticularItem(model.SSRModel);
            model.TotalSDMParticularItem = model.SDMParticularItemModel.WindowsSubmission
                                         + model.SDMParticularItemModel.RenderingSubmission
                                         + model.SDMParticularItemModel.RepairSubmission
                                         + model.SDMParticularItemModel.AbovegroudDrainageSubmission
                                         + model.SDMParticularItemModel.AcSupportingFrameSubmission
                                         + model.SDMParticularItemModel.DryingRackSubmission
                                         + model.SDMParticularItemModel.CanopySubmission
                                         + model.SDMParticularItemModel.SdfSubmission
                                         + model.SDMParticularItemModel.SignboardRelatedSubmission;
            model.RectificationNote3Model = DA.GetRectificationNote3Data();
            string AllNatureList = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE + "','"
                                     + ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL + "'";
            model.KtBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_KWUN_TONG_CODE, "Y", AllNatureList);
            model.PcBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_PIONEER_CENTRE_CODE, "Y", AllNatureList);
            model.EcBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_E_SUBMISSION_CODE, "Y", AllNatureList);
            model.WKGOBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_WKGO_CODE, "Y", AllNatureList);
            model.KtNonBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_KWUN_TONG_CODE, "N", AllNatureList);
            model.PcNonBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_PIONEER_CENTRE_CODE, "N", AllNatureList);
            model.EcNonBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_E_SUBMISSION_CODE, "N", AllNatureList);
            model.WKGONonBarCode = DA.GetTotalCounterByDate(model.SSRModel, ProcessingConstant.GENERAL_SEARCH_COUNTER_KWUN_TONG_CODE, "N", AllNatureList);
            return model;
        }


        public string GetNatureList(List<string> natureList)
        {
            StringBuilder natureListString = new StringBuilder();
            if (natureList.Count == 0)
                return null;
            else if (natureList.Count == 1)
            {
                natureListString.Append("'" + natureList[0] + "'");
            }
            else
            {
                for (int i = 0; i < natureList.Count; i++)
                {
                    if (i == 0)
                    {
                        natureListString.Append("'" + natureList[i] + "','");
                    }
                    else if (i == natureList.Count - 1)
                    {
                        natureListString.Append(natureList[i] + "'");
                    }
                    else
                    {
                        natureListString.Append(natureList[i] + "','");
                    }
                }
            }
            return natureListString.ToString();
        }


        public string GetMWAckLetterCountQuery(SearchSubmissionReceivedModel model)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.Append("Select count(ACK.uuid) as count from P_MW_ACK_LETTER ACK where 1=1 ");
            if (model.ReceivedDateFrom != null)
                queryString.Append(" AND to_date(ACK.RECEIVED_DATE) >= :ReceivedDateFrom ");

            if (model.ReceivedDateTo != null)
                queryString.Append("  AND to_date(ACK.RECEIVED_DATE) <= :ReceivedDateTo ");

            if (model.LetterDateFrom != null)
                queryString.Append(" AND to_date(ACK.LETTER_DATE) >= :LetterDateFrom ");

            if (model.LetterDateTo != null)
                queryString.Append(" AND to_date(ACK.LETTER_DATE) <= :LetterDateTo ");

            if (model.ReferralDateFrom != null)
                queryString.Append(" AND to_date(ACK.referral_Date) >= :ReferralDateFrom ");

            if (model.ReferralDateTo != null)
                queryString.Append(" AND to_date(ACK.referral_Date) <= :ReferralDateTo ");

            if (model.NatureList != null)
                queryString.Append(string.Format(" AND ACK.NATURE IN ( {0} ) ", GetNatureList(model.NatureList)));

            if (model.FormTypeList != null)
                queryString.Append(string.Format(" AND ACK.FORM_NO IN ( {0} ) ", GetNatureList(model.FormTypeList)));

            if (model.OrderRelatedOnly)
                queryString.Append(" AND ACK.ORDER_RELATED = 'Y' ");

            if (model.LetterDateAndReceivedDateLogic == 1)
                queryString.Append(" AND to_date(ACK.LETTER_DATE) = to_date(ACK.RECEIVED_DATE) ");
            else if (model.LetterDateAndReceivedDateLogic == 2)
                queryString.Append(" AND to_date(ACK.LETTER_DATE) != to_date(ACK.RECEIVED_DATE) ");

            if (model.VsOnly)
                queryString.Append(" AND MW_NO LIKE 'VS%' ");

            return queryString.ToString();
        }

        public Dictionary<string, int> GetMWSubmissionData(SearchSubmissionReceivedModel model)
        {
            Dictionary<string, int> resultDic = new Dictionary<string, int>();
            StringBuilder queryStringUnionAll = new StringBuilder("Select * from (");
            List<string> queryStringList = new List<string>();

            //Received
            string queryStringA1_0 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
              ,
                ReceivedDateTo = model.ReceivedDateTo
              ,
                NatureList = new List<string>
              {
                  ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                 ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                 ,ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN
                 ,ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE
                 ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
              }
            });
            queryStringList.Add(queryStringA1_0);

            //Acknowledged (over counter)
            string queryStringA2_1 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 1
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringA2_1);

            //Acknowledged (by fax)
            string queryStringA3_2 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                LetterDateFrom = model.LetterDateFrom
                ,
                LetterDateTo = model.LetterDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 2
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringA3_2);

            //Returned  by faxNote 1
            string queryStringA4_3 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                LetterDateFrom = model.LetterDateFrom
                ,
                LetterDateTo = model.LetterDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 0
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringA4_3);

            //Returned over counter
            string queryStringA5_4 = @"SELECT sum(t0.count) FROM P_S_DAILY_DIRECT_RT_OVER_CNT t0 WHERE 1 = 1 
                                    AND TO_CHAR(t0.received_Date, 'YYYYMMDD') >= TO_CHAR(:ReceivedDateFrom,'YYYYMMDD') 
                                    AND TO_CHAR(t0.received_Date, 'YYYYMMDD') <= TO_CHAR(:ReceivedDateTo,'YYYYMMDD') ";
            queryStringList.Add(queryStringA5_4);

            //C1.1.2
            string queryStringC112_5 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                ReferralDateFrom = model.ReferralDateFrom
                ,
                ReferralDateTo = model.ReferralDateTo
                ,
                OrderRelatedOnly = true
                ,
                LetterDateAndReceivedDateLogic = 0
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringC112_5);

            //C1.1.3 class1
            string queryStringC113_class1_6 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
                }
                ,
                FormTypeList = new List<string>
                {
                    ProcessingConstant.FORM_01
                    ,ProcessingConstant.FORM_11
                    ,ProcessingConstant.FORM_02
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 0
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringC113_class1_6);

            //C1.1.3 class2
            string queryStringC113_class2_7 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
                }
                ,
                FormTypeList = new List<string>
                {
                     ProcessingConstant.FORM_03
                     , ProcessingConstant.FORM_12
                     , ProcessingConstant.FORM_04
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 0
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringC113_class2_7);

            //C1.1.3.class3
            string queryStringC113_class3_8 = GetMWAckLetterCountQuery(new SearchSubmissionReceivedModel
            {
                ReceivedDateFrom = model.ReceivedDateFrom
                ,
                ReceivedDateTo = model.ReceivedDateTo
                ,
                NatureList = new List<string>
                {
                     ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE
                    ,ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL
                }
                ,
                FormTypeList = new List<string>
                {
                     ProcessingConstant.FORM_05
                }
                ,
                OrderRelatedOnly = false
                ,
                LetterDateAndReceivedDateLogic = 0
                ,
                VsOnly = false
            });
            queryStringList.Add(queryStringC113_class3_8);



            for (int i = 0; i < queryStringList.Count; i++)
            {
                if (i != queryStringList.Count - 1)
                    queryStringUnionAll.Append("\r\n\t" + queryStringList[i] + " union all ");
                else
                    queryStringUnionAll.Append("\r\n\t" + queryStringList[i] + " ) T");
            }

            var result = DA.GetMWSubmissionData(model, queryStringUnionAll.ToString());
            resultDic.Add("ReceivedCount", result[0]);
            resultDic.Add("AcknowledgedOverCounter", result[1]);
            resultDic.Add("AcknowledgedByFax", result[2]);
            resultDic.Add("ReturnedByFaxNote", result[3]);
            resultDic.Add("ReturnedOverCounter", result[4]);
            resultDic.Add("C112Value", result[5]);
            resultDic.Add("C113Class1Value", result[6]);
            resultDic.Add("C113Class2Value", result[7]);
            resultDic.Add("C113Class3Value", result[8]);
            return resultDic;


            //Dictionary<string, int> resultDic = new Dictionary<string, int>();
            //string queryString = "Select count(ACK.uuid) as count from P_MW_ACK_LETTER ACK where 1=1 ";
            //StringBuilder whereQ = new StringBuilder("Select * from (");
            //StringBuilder whereQ1 = new StringBuilder(queryString);
            //StringBuilder whereQ2 = new StringBuilder(queryString);
            //StringBuilder whereQ3 = new StringBuilder(queryString);
            //StringBuilder whereQ4 = new StringBuilder(queryString);
            //StringBuilder whereQ5 = new StringBuilder(@"SELECT sum(t0.count) FROM P_S_DAILY_DIRECT_RT_OVER_CNT t0 WHERE 1 = 1 ");
            //StringBuilder whereQC112 = new StringBuilder("Select Count(ACK.uuid) from P_MW_ACK_LETTER ACK where 1=1 ");

            //if (model.ReceivedDateFrom != null)
            //{
            //    whereQ1.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) >= :ReceivedDateFrom");
            //    whereQ2.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) >= :ReceivedDateFrom");
            //    whereQ3.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) >= :ReceivedDateFrom");
            //    whereQ4.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) >= :ReceivedDateFrom");
            //    whereQ5.Append("\r\n\t" + " AND TO_CHAR(t0.received_Date, 'YYYYMMDD') >= TO_CHAR(:ReceivedDateFrom,'YYYYMMDD')");
            //}
            //if (model.ReceivedDateTo != null)
            //{
            //    whereQ1.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) <= :ReceivedDateTo");
            //    whereQ2.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) <= :ReceivedDateTo");
            //    whereQ3.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) <= :ReceivedDateTo");
            //    whereQ4.Append("\r\n\t" + " AND to_date(ACK.RECEIVED_DATE) <= :ReceivedDateTo");
            //    whereQ5.Append("\r\n\t" + " AND TO_CHAR(t0.received_Date, 'YYYYMMDD') <= TO_CHAR(:ReceivedDateTo,'YYYYMMDD')");
            //}


            //whereQ2.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) = to_date(ACK.RECEIVED_DATE) ");
            //whereQ3.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) != to_date(ACK.RECEIVED_DATE) ");

            //if (model.LetterDateFrom != null)
            //{
            //    whereQ3.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) >= :LetterDateFrom ");
            //    whereQ4.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) >= :LetterDateFrom ");
            //}
            //if (model.LetterDateTo != null)
            //{
            //    whereQ3.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) <= :LetterDateTo ");
            //    whereQ4.Append("\r\n\t" + " AND to_date(ACK.LETTER_DATE) <= :LetterDateTo ");
            //}

            //string natureList1 = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL + "'";

            //string natureList2 = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL + "'";

            //string natureList3 = "'" + ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN + "','"
            //                         + ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE + "'";

            //whereQ1.Append("\r\n\t" + string.Format("  AND ACK.NATURE IN ( {0} ) ", natureList1));
            //whereQ2.Append("\r\n\t" + string.Format("  AND ACK.NATURE IN ( {0} ) ", natureList2));
            //whereQ3.Append("\r\n\t" + string.Format("  AND ACK.NATURE IN ( {0} ) ", natureList2));
            //whereQ4.Append("\r\n\t" + string.Format("  AND ACK.NATURE IN ( {0} ) ", natureList3));

            //if (model.ReferralDateFrom != null)
            //{
            //    whereQC112.Append("\r\n\t" + " AND to_date(ACK.referral_Date) >= :ReferralDateFrom ");
            //}
            //if (model.ReferralDateTo != null)
            //{
            //    whereQC112.Append("\r\n\t" + " AND to_date(ACK.referral_Date) <= :ReferralDateTo ");
            //}
            //whereQ.Append(whereQ1  + " union all "  + whereQ2  + " union all " + whereQ3 + " union all "  + whereQ4 + " union all " + whereQ5 + " union all " + whereQC112 + "\r\n\t" + "  ) T");
            //var result = DA.GetMWSubmissionData(model, whereQ.ToString());
            //resultDic.Add("ReceivedCount", result[0]);
            //resultDic.Add("AcknowledgedOverCounter", result[1]);
            //resultDic.Add("AcknowledgedByFax", result[2]);
            //resultDic.Add("ReturnedByFaxNote", result[3]);
            //resultDic.Add("ReturnedOverCounter", result[4]);
            //resultDic.Add("C112Value", result[5]);
            //return resultDic;
        }

        public int StatisticsCalculateWeekly(int count, int weeklydays)
        {
            return Convert.ToInt32(Math.Round(Convert.ToDouble(count) / (Convert.ToDouble(weeklydays) / 7)));
        }

        public List<Fn01LM_StatisticsIncomingModel> GetStatisticsIncomingCalendarData(GetIncomingOutgoingParameterModel model)
        {
            return DA.GetStatisticsIncomingCalendarData(model);
        }

        public int WeekOfMonth(DateTime day, int WeekStart)
        {
            DateTime FirstofMonth;
            FirstofMonth = Convert.ToDateTime(day.Date.Year + "-" + day.Date.Month + "-" + 1);
            int i = (int)FirstofMonth.Date.DayOfWeek;
            if (i == 0)
            {
                i = 7;
            }

            if (WeekStart == 1)
            {
                return (day.Date.Day + i - 2) / 7 + 1;
            }
            if (WeekStart == 2)
            {
                return (day.Date.Day + i - 1) / 7;

            }
            return 0;
        }


        public Fn01LM_StatisticsIncomingModel CalculateIncomingWeeklySummary(Fn01LM_StatisticsIncomingListModel model)
        {
            Fn01LM_StatisticsIncomingModel resultModel = new Fn01LM_StatisticsIncomingModel()
            {
                PC_REC = 0,
                PC_OD = 0,
                PC_OS = 0,
                KT_REC = 0,
                KT_OD = 0,
                KT_OS = 0,
                WKGO_PEC = 0,
                WKGO_OD = 0,
                WKGO_OS = 0,
                ES_REC = 0,
                ES_OD = 0,
                ES_OS = 0,
                OR_REC = 0,
                OR_OD = 0,
                OR_OS = 0,
                DL_REC = 0,
                DL_OD = 0,
                DL_OS = 0,
                AUDIT = 0,
                ICU = 0,
                CR = 0
            };
            foreach (var item in model.IncomingModelList)
            {
                resultModel.PC_REC += item.PC_REC; resultModel.PC_OD += item.PC_OD; resultModel.PC_OS += item.PC_OS;
                resultModel.KT_REC += item.KT_REC; resultModel.KT_OD += item.KT_OD; resultModel.KT_OS += item.KT_OS;
                resultModel.WKGO_PEC += item.WKGO_PEC; resultModel.WKGO_OD += item.WKGO_OD; resultModel.WKGO_OS += item.WKGO_OS;
                resultModel.ES_REC += item.ES_REC; resultModel.ES_OD += item.ES_OD; resultModel.ES_OS += item.ES_OS;
                resultModel.OR_REC += item.OR_REC; resultModel.OR_OD += item.OR_OD; resultModel.OR_OS += item.OR_OS;
                resultModel.DL_REC += item.DL_REC; resultModel.DL_OD += item.DL_OD; resultModel.DL_OS += item.DL_OS;
                resultModel.AUDIT += item.AUDIT; resultModel.ICU += item.ICU; resultModel.CR += item.CR;
                resultModel.IncomingWeeklySummaryDateRange += item.RECEIVED_DATE.ToString() + ",";
            }
            return resultModel;
        }

        public Fn01LM_StatisticsOutgoingModel CalculateOutgoingWeeklySummary(Fn01LM_StatisticsOutgoingListModel model)
        {
            Fn01LM_StatisticsOutgoingModel resultModel = new Fn01LM_StatisticsOutgoingModel()
            {
                PC_COUNTER = 0,
                KT_COUNTER = 0,
                ES_COUNTER = 0,
                D_LET = 0,
                O_REL = 0,
                ICU = 0,
                CR = 0
            };
            foreach (var item in model.OutgoingModelList)
            {
                resultModel.PC_COUNTER += item.PC_COUNTER;
                resultModel.KT_COUNTER += item.KT_COUNTER;
                resultModel.WKGO_COUNTER += item.WKGO_COUNTER;
                resultModel.ES_COUNTER += item.ES_COUNTER;
                resultModel.O_REL += item.O_REL;
                resultModel.ICU += item.ICU;
                resultModel.CR += item.CR;
                resultModel.WeeklySummaryDateRange += item.DATERANGE.ToString() + ",";
            }
            return resultModel;
        }

        //public Fn01LM_StatisticsOutgoingModel CalculateIncomingWeeklySummary1(Fn01LM_StatisticsOutgoingListModel model)
        //{
        //    var res = (from l in outgoingModelList1.OutgoingModelList
        //    group l by new
        //                       {
        //                           l.DATERANGE,
        //                           l.WEEK_DAY,
        //                           l.DAY,
        //                           l.PC_COUNTER,
        //                           l.KT_COUNTER,
        //                           l.ES_COUNTER,
        //                           l.D_LET,
        //                           l.O_REL,
        //                           l.ICU,
        //                           l.CR
        //                      }
        //                          into g
        //                       select new
        //                       {
        //                           PC_COUNTER = g.Sum(c => c.PC_COUNTER),
        //                           KT_COUNTER = g.Sum(c => c.KT_COUNTER),
        //                           ES_COUNTER = g.Sum(c => c.ES_COUNTER),
        //                           D_LET = g.Sum(c => c.D_LET),
        //                           O_REL = g.Sum(c => c.O_REL),
        //                           ICU = g.Sum(c => c.ICU),
        //                           CR = g.Sum(c => c.CR)
        //                       }).FirstOrDefault();

        //    return res;
        //}

        public string Export_WhereQ(Fn01LM_StatisticsModel model)
        {
            StringBuilder whereQ = new StringBuilder();
            if (model.ExportReportType == ProcessingConstant.INCOMING)
            {
                if (model.Received_Date.Contains(","))
                {
                    var dateList = model.Received_Date.Split(',').ToList<string>();
                    whereQ.Append("\r\n\t" + " AND RECEIVED_DATE BETWEEN TO_DATE( :startDate ,'dd/mm/yyyy') AND TO_DATE( :endDate ,'dd/mm/yyyy') ");
                    model.QueryParameters.Add("startDate", dateList[0]);
                    model.QueryParameters.Add("endDate", dateList[dateList.Count - 2]);
                }
                else
                {
                    whereQ.Append("\r\n\t" + " AND TO_CHAR(RECEIVED_DATE, 'dd/mm/yyyy') = TO_CHAR( :exportDate , 'dd/mm/yyyy') ");
                    model.QueryParameters.Add("exportDate", Convert.ToDateTime(model.Received_Date));
                }

            }
            if (model.ExportReportType == ProcessingConstant.OUTGOING)
            {
                if (model.Received_Date.Contains(","))
                {
                    var dateList = model.Received_Date.Split(',').ToList<string>();
                    if (model.ExportType == "O")
                        whereQ.Append("\r\n\t" + " And REFERRAL_DATE BETWEEN TO_DATE( :startDate ,'dd/mm/yyyy') AND TO_DATE( :endDate ,'dd/mm/yyyy')  ");
                    else
                        whereQ.Append("\r\n\t" + " And LETTER_DATE BETWEEN TO_DATE( :startDate ,'dd/mm/yyyy') AND TO_DATE( :endDate ,'dd/mm/yyyy')  ");
                    whereQ.Append("\r\n\t" + " AND RECEIVED_DATE BETWEEN TO_DATE( :yearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE ");
                    model.QueryParameters.Add("startDate", dateList[0]);
                    model.QueryParameters.Add("endDate", dateList[dateList.Count - 2]);
                    model.QueryParameters.Add("yearStartDate", "2014/01/01");
                }
                else
                {
                    if (model.ExportType == "O")
                        whereQ.Append("\r\n\t" + " AND TO_CHAR(REFERRAL_DATE, 'dd/mm/yyyy') = TO_CHAR( :exportDate , 'dd/mm/yyyy') ");
                    else
                        whereQ.Append("\r\n\t" + " AND TO_CHAR(LETTER_DATE, 'dd/mm/yyyy') = TO_CHAR( :exportDate , 'dd/mm/yyyy') ");
                    whereQ.Append("\r\n\t" + " AND RECEIVED_DATE BETWEEN TO_DATE( :yearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE ");
                    model.QueryParameters.Add("exportDate", Convert.ToDateTime(model.Received_Date));
                    model.QueryParameters.Add("yearStartDate", "2014/01/01");
                }


            }
            if (model.ExportType == "D")
            {
                whereQ.Append("\r\n\t" + " AND nature IN ('DIRECT RETURN','REVISED CASE') ");
            }
            else if (model.ExportType == "O")
            {
                whereQ.Append("\r\n\t" + " AND nature IN ('SUBMISSION','E-SUBMISSION') ");
            }
            else
            {
                whereQ.Append("\r\n\t" + " AND nature IN ('SUBMISSION','E-SUBMISSION','DIRECT RETURN','REVISED CASE','WITHDRAWAL') ");
            }

            if (model.ExportType == "O")
            {
                whereQ.Append("\r\n\t" + " AND ORDER_RELATED= 'Y' ");
            }
            else if (model.ExportType == "A")
            {
                whereQ.Append("\r\n\t" + " AND AUDIT_RELATED= 'Y' ");
            }
            model.Sort = "DSN";
            //whereQ.Append(" ORDER BY DSN ASC ");
            return whereQ.ToString();

        }

        public string Export(Fn01LM_StatisticsModel model)
        {
            model.Query = @"SELECT NATURE, DSN, TO_CHAR(RECEIVED_DATE, 'dd/mm/yyyy') RECEIVED_DATE, FORM_NO, MW_NO, COUNTER,
                            PBP_NO, PRC_NO, UPPER(ADDRESS) ADDRESS, UPPER(STREET) STREET, UPPER(STREET_NO) STREET_NO, UPPER(BUILDING) BUILDING, UPPER(FLOOR) FLOOR, UPPER(UNIT) UNIT,
                            AUDIT_RELATED, ITEM_DISPLAY, SSP, TO_CHAR(COMM_DATE, 'dd/mm/yyyy') COMM_DATE, TO_CHAR(COMP_DATE, 'dd/mm/yyyy') COMP_DATE, SIGNBOARD_RELATED,
                            ORDER_RELATED, TO_CHAR(REFERRAL_DATE, 'dd/mm/yyyy') REFERRAL_DATE, PAW,	PAW_CONTACT, IO_MGT, IO_MGT_CONTACT, 
                            CASE LANGUAGE WHEN 'ENG' THEN 'E' 
                            WHEN 'E' THEN 'E' 
                            WHEN 'CHT' THEN 'C' 
                            WHEN 'C' THEN 'C' 
                            ELSE 'E' END LANGUAGE,
                            REMARK, TO_CHAR(LETTER_DATE, 'dd/mm/yyyy') LETTER_DATE, 
                            CASE SDF_RELATED WHEN 'Y' THEN 'Y' ELSE 'N' END SDF_RELATED,
                            PREVIOUS_RELATED_MW_NO, CASE BARCODE WHEN 'Y' THEN 'Y' ELSE 'N' END BARCODE,
                            FILEREF_FOUR || '/' || FILEREF_TWO FILE_REF_NO , MODIFIED_DATE
                            FROM P_MW_ACK_LETTER
                            Where 1=1";  // AND DSN ='D0000567957'
            model.QueryWhere = Export_WhereQ(model);
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string> { ["columnName"] = "NATURE", ["displayName"] = "Nature" })
                .Append(new Dictionary<string, string> { ["columnName"] = "DSN", ["displayName"] = "DSN" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RECEIVED_DATE", ["displayName"] = "Received Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "FORM_NO", ["displayName"] = "Form Type" })
                .Append(new Dictionary<string, string> { ["columnName"] = "MW_NO", ["displayName"] = "MW NO" })
                .Append(new Dictionary<string, string> { ["columnName"] = "COUNTER", ["displayName"] = "Counter" })
                .Append(new Dictionary<string, string> { ["columnName"] = "PBP_NO", ["displayName"] = "PBP Reg" })
                .Append(new Dictionary<string, string> { ["columnName"] = "PRC_NO", ["displayName"] = "PRC Reg" })
                .Append(new Dictionary<string, string> { ["columnName"] = "ADDRESS", ["displayName"] = "Works Location" })
                .Append(new Dictionary<string, string> { ["columnName"] = "STREET", ["displayName"] = "Street" })
                .Append(new Dictionary<string, string> { ["columnName"] = "STREET_NO", ["displayName"] = "Street No" })
                .Append(new Dictionary<string, string> { ["columnName"] = "BUILDING", ["displayName"] = "Building" })
                .Append(new Dictionary<string, string> { ["columnName"] = "FLOOR", ["displayName"] = "Floor" })
                .Append(new Dictionary<string, string> { ["columnName"] = "UNIT", ["displayName"] = "Unit" })
                .Append(new Dictionary<string, string> { ["columnName"] = "AUDIT_RELATED", ["displayName"] = "Selected" })
                .Append(new Dictionary<string, string> { ["columnName"] = "ITEM_DISPLAY", ["displayName"] = "MW Items" })
                .Append(new Dictionary<string, string> { ["columnName"] = "SSP", ["displayName"] = "SSP" })
                .Append(new Dictionary<string, string> { ["columnName"] = "COMM_DATE", ["displayName"] = "Commencement Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "COMP_DATE", ["displayName"] = "Completion Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "SIGNBOARD_RELATED", ["displayName"] = "Signboard" })
                .Append(new Dictionary<string, string> { ["columnName"] = "ORDER_RELATED", ["displayName"] = "Order Related" })
                .Append(new Dictionary<string, string> { ["columnName"] = "REFERRAL_DATE", ["displayName"] = "Referral Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "PAW", ["displayName"] = "PAW" })
                .Append(new Dictionary<string, string> { ["columnName"] = "PAW_CONTACT", ["displayName"] = "PAW Contact" })
                .Append(new Dictionary<string, string> { ["columnName"] = "IO_MGT", ["displayName"] = "IO/Mgt" })
                .Append(new Dictionary<string, string> { ["columnName"] = "IO_MGT_CONTACT", ["displayName"] = "IO/Mgt Contact" })
                .Append(new Dictionary<string, string> { ["columnName"] = "LANGUAGE", ["displayName"] = "Language" })
                .Append(new Dictionary<string, string> { ["columnName"] = "REMARK", ["displayName"] = "Remarks" })
                .Append(new Dictionary<string, string> { ["columnName"] = "LETTER_DATE", ["displayName"] = "Letter Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "SDF_RELATED", ["displayName"] = "SDF Related" })
                .Append(new Dictionary<string, string> { ["columnName"] = "PREVIOUS_RELATED_MW_NO", ["displayName"] = "Previous MW NO" })
                .Append(new Dictionary<string, string> { ["columnName"] = "BARCODE", ["displayName"] = "Barcode" })
                .Append(new Dictionary<string, string> { ["columnName"] = "FILE_REF_NO", ["displayName"] = "File Ref No" })
                .Append(new Dictionary<string, string> { ["columnName"] = "MODIFIED_DATE", ["displayName"] = "Time Log" })
                .ToArray();



            return model.Export("Submission_Data_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));


        }



        #endregion

        public JsonResult GetEmailByCerNo(string CerificationNo, string dsn, string type)
        {
            ServiceResult serviceResult = new ServiceResult();

            V_CRM_INFO record = CrmInfoDA.findByCertNo(CerificationNo);

            if (record != null)
            {
                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                serviceResult.Data = record.EMAIL_ADDRESS;
            }

            SYS_EMAIL_SENDER emailSender = DA.GetEmailSender(dsn, type);
            if (emailSender == null)
            {
                serviceResult.Message = new List<string>()
                {
                    "NoSender"
                };
            }
            else
            {
                serviceResult.Message = new List<string>()
                {
                    emailSender.STATUS
                };
            }

            return new JsonResult { Data = serviceResult };
        }

        public ServiceResult AddEmailSender(string uuid, string dsn, string email, string type, string tempPath, string tmpPDFDir)
        {
            return DA.AddEmailSender(dsn, email, type, PrintPDF(uuid, tempPath, tmpPDFDir));
        }

        public Fn01LM_ALDisplayModel getAckLetterDetail(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                Fn01LM_ALDisplayModel model = new Fn01LM_ALDisplayModel();

                P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(x => x.DSN == DSN).FirstOrDefault();

                model.P_MW_ACK_LETTER = ack;
                //model.P_MW_ACK_LETTER.P_MW_DW_LETTER = (ICollection<P_MW_DW_LETTER>) db.P_MW_DW_LETTER.Where(x => x.MW_ACK_LETTER_ID == ack.UUID);
                model.DSN = ack.DSN;

                if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                {
                    string fullAddress = "";
                    if (!string.IsNullOrWhiteSpace(ack.STREET)) fullAddress += ack.STREET;
                    if (!string.IsNullOrWhiteSpace(ack.STREET_NO)) fullAddress += ack.STREET_NO;
                    if (!string.IsNullOrWhiteSpace(ack.BUILDING)) fullAddress += ack.BUILDING;
                    if (!string.IsNullOrWhiteSpace(ack.FLOOR)) fullAddress += ack.FLOOR;
                    if (!string.IsNullOrWhiteSpace(ack.UNIT)) fullAddress += ack.UNIT;
                    model.Address = fullAddress;
                }
                else // english
                {
                    string addressFirstComponent = "";
                    string addressSecondComponent = "";
                    if (!string.IsNullOrWhiteSpace(ack.UNIT)) addressFirstComponent += ack.UNIT + " ";
                    if (!string.IsNullOrWhiteSpace(ack.FLOOR)) addressFirstComponent += ack.FLOOR + " ";
                    if (!string.IsNullOrWhiteSpace(ack.BUILDING)) addressFirstComponent += ack.BUILDING + " ";
                    if (string.IsNullOrWhiteSpace(addressFirstComponent)) addressFirstComponent += ", ";
                    if (!string.IsNullOrWhiteSpace(ack.STREET_NO)) addressSecondComponent += ack.STREET_NO + " ";
                    if (!string.IsNullOrWhiteSpace(ack.STREET)) addressSecondComponent += ack.STREET + " ";
                    model.Address = addressFirstComponent + addressSecondComponent;
                }

                //model.Items = new List<List<object>>();
                model.FileType = ack.FILE_TYPE;

                List<P_MW_DW_LETTER_ITEM> P_MW_DW_LETTER_ITEMs = db.P_MW_DW_LETTER_ITEM.Where(x => x.MW_ACK_LETTER_ID == ack.UUID && x.LETTER_TYPE == ProcessingConstant.AL_LETTER_ITEM_LETTER_TYPE).OrderBy(x => x.ITEM_NO).ToList();
                if (P_MW_DW_LETTER_ITEMs == null || P_MW_DW_LETTER_ITEMs.Count() == 0)
                {
                    ProcessingLetterModuleDAOService dao = new ProcessingLetterModuleDAOService();
                    P_MW_DW_LETTER_ITEMs = dao.createMwDwItems(ack);
                }
                model.Items = P_MW_DW_LETTER_ITEMs;
                model.ItemCheckList = new Dictionary<string, bool>();
                model.ItemNoList = new Dictionary<string, string>();
                model.ItemTextList = new Dictionary<string, string>();
                foreach (var item in P_MW_DW_LETTER_ITEMs)
                {
                    model.ItemCheckList.Add(item.UUID, item.CHECKED == ProcessingConstant.FLAG_Y ? true : false);
                    model.ItemNoList.Add(item.UUID, item.ITEM_NO);
                    if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                    {
                        model.ItemTextList.Add(item.UUID, item.ITEM_TEXT);
                    }
                    else
                    {
                        model.ItemTextList.Add(item.UUID, item.ITEM_TEXT_E);
                    }
                }

                V_CRM_INFO pbp = db.V_CRM_INFO.Where(x => x.CERTIFICATION_NO == ack.PBP_NO).FirstOrDefault();
                if (pbp != null)
                {
                    model.PbpAddr = new List<string>();
                    if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                    {
                        model.PbpFax = "(傳真: " + pbp.FAX_NO + ")";
                        model.PbPName = (!string.IsNullOrWhiteSpace(pbp.CHINESE_NAME)) ? pbp.CHINESE_NAME : pbp.SURNAME + " " + pbp.GIVEN_NAME;
                        model.PbpAddrFull = pbp.CN_ADDRESS_LINE1 + pbp.CN_ADDRESS_LINE2 + pbp.CN_ADDRESS_LINE3 + pbp.CN_ADDRESS_LINE4 + pbp.CN_ADDRESS_LINE5;
                        model.PbpAddr.Add(pbp.CN_ADDRESS_LINE1);
                        model.PbpAddr.Add(pbp.CN_ADDRESS_LINE2);
                        model.PbpAddr.Add(pbp.CN_ADDRESS_LINE3);
                        model.PbpAddr.Add(pbp.CN_ADDRESS_LINE4);
                        model.PbpAddr.Add(pbp.CN_ADDRESS_LINE5);
                    }
                    else // english
                    {
                        model.PbpFax = "(Fax: " + pbp.FAX_NO + ")";
                        model.PbPName = (!string.IsNullOrWhiteSpace(pbp.SURNAME + " " + pbp.GIVEN_NAME)) ? pbp.SURNAME + " " + pbp.GIVEN_NAME : pbp.CHINESE_NAME;
                        model.PbpAddr.Add(pbp.EN_ADDRESS_LINE1);
                        model.PbpAddr.Add(pbp.EN_ADDRESS_LINE2);
                        model.PbpAddr.Add(pbp.EN_ADDRESS_LINE3);
                        model.PbpAddr.Add(pbp.EN_ADDRESS_LINE4);
                        model.PbpAddr.Add(pbp.EN_ADDRESS_LINE5);

                    }
                }
                V_CRM_INFO prc = db.V_CRM_INFO.Where(x => x.CERTIFICATION_NO == ack.PRC_NO).FirstOrDefault();
                if (prc != null)
                {
                    model.PrcAddr = new List<string>();
                    if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                    {
                        model.PrcFax = "(傳真: " + prc.FAX_NO + ")";
                        model.PrcName = (!string.IsNullOrWhiteSpace(prc.CHINESE_NAME)) ? prc.CHINESE_NAME : prc.SURNAME + " " + prc.GIVEN_NAME;
                        model.PrcAddrFull = prc.CN_ADDRESS_LINE1 + prc.CN_ADDRESS_LINE2 + prc.CN_ADDRESS_LINE3 + prc.CN_ADDRESS_LINE4 + prc.CN_ADDRESS_LINE5;
                        model.PrcAddr.Add(prc.CN_ADDRESS_LINE1);
                        model.PrcAddr.Add(prc.CN_ADDRESS_LINE2);
                        model.PrcAddr.Add(prc.CN_ADDRESS_LINE3);
                        model.PrcAddr.Add(prc.CN_ADDRESS_LINE4);
                        model.PrcAddr.Add(prc.CN_ADDRESS_LINE5);
                    }
                    else // english
                    {
                        model.PrcFax = "(Fax: " + prc.FAX_NO + ")";
                        model.PrcName = (!string.IsNullOrWhiteSpace(prc.SURNAME + " " + prc.GIVEN_NAME)) ? prc.SURNAME + " " + prc.GIVEN_NAME : prc.CHINESE_NAME;
                        model.PrcAddr.Add(prc.EN_ADDRESS_LINE1);
                        model.PrcAddr.Add(prc.EN_ADDRESS_LINE2);
                        model.PrcAddr.Add(prc.EN_ADDRESS_LINE3);
                        model.PrcAddr.Add(prc.EN_ADDRESS_LINE4);
                        model.PrcAddr.Add(prc.EN_ADDRESS_LINE5);

                    }
                }
                if (ack.LETTER_DATE == null)
                {
                    ack.LETTER_DATE = DateTime.Now;
                }
                model.LetterDate = ack.LETTER_DATE;
                if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                {
                    //model.ReceivedDateDisplay = ((DateTime) ack.RECEIVED_DATE).ToString("yyyymmdd", CultureInfo.CreateSpecificCulture("zh-cn"));

                    //model.ReceivedDateDisplay = ack.RECEIVED_DATE.Value.ToString("yyyy年MM月dd日", new CultureInfo("zh-cn"));
                    model.ReceivedDateDisplay =
                         Utility.DateUtil.getChineseFormatDate(ack.RECEIVED_DATE);
                    model.LetterDateDisplay = Utility.DateUtil.getChineseFormatDate(ack.LETTER_DATE);


                    // .ToString("yyyy年MM月dd日", new CultureInfo("zh-cn"));
                }
                else // english
                {
                    //  model.ReceivedDateDisplay = ((DateTime)ack.RECEIVED_DATE).ToString("dd MMMM, yyyy");
                    model.ReceivedDateDisplay =
                         Utility.DateUtil.getEnglishDateDisplayFormat(ack.RECEIVED_DATE, "dd MMMM, yyyy");

                    model.LetterDateDisplay =
                         Utility.DateUtil.getEnglishDateDisplayFormat(ack.LETTER_DATE, "dd MMMM, yyyy");

                    //  model.LetterDateDisplay = ((DateTime)ack.LETTER_DATE).ToString("dd MMMM, yyyy");
                }

                P_S_TO_DETAILS po = db.P_S_TO_DETAILS.Where(m => m.PO_POST == ack.PO_POST).FirstOrDefault();

                string poNameEng = po != null ? po.PO_NAME_ENG : "";

                string poNameChi = po != null ? po.PO_NAME_CHI : "";
                string poPostEng = po != null ? po.PO_POST_ENG : "";
                string poPostChi = po != null ? po.PO_POST_CHI : "";
                string poContact = po != null ? po.PO_CONTACT : "";
                model.SpoContact = po != null ? po.SPO_CONTACT : "";
                model.PoContact = poContact;

                string status = "";

                if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(ack.LANGUAGE)) // chinese
                {
                    model.PoName = poNameChi;
                    model.PoPost = poPostChi;
                    if (ProcessingConstant.FORM_01.Equals(ack.FORM_NO) || ProcessingConstant.FORM_03.Equals(ack.FORM_NO))
                    {
                        status = "展開";
                    }
                    else if (ProcessingConstant.FORM_02.Equals(ack.FORM_NO) || ProcessingConstant.FORM_04.Equals(ack.FORM_NO) || ProcessingConstant.FORM_05.Equals(ack.FORM_NO))
                    {
                        status = "完成";
                    }
                    else
                    {
                        status = "進行";
                    }
                    model.Heading = "傳真 及 郵寄";
                    model.Title = "先生／女士：";
                    model.Caption = "小型工程呈交編號 " + ack.MW_NO;
                    model.FirstPara =
                        "\t\t本署於" + model.ReceivedDateDisplay + "收到你就根據簡化規定在上址" + status + "的小型工程呈交的(" + ack.FORM_NO + "指明表格)。" +
                        "上述工程被選中進行審核，以確保有關工程符合《建築物條例》及其附屬法例的規定，並按照呈交的圖則及工程描述進行。" +
                        "本署初步審核你呈交的文件後，發現有以下欠妥之處：";
                    model.SecondPara =
                        "請於本信發出日起計14天內呈交補充資料，並一併提交MW33表格，以及修正上述欠妥之處，以便本署繼續處理有關個案。";
                    model.ThirdPara =
                        "如就此小型工程重新呈交新的表格及相關文件，請於新的表格內 “早前相關的小型工程呈交編號”一欄填上小型工程呈交編號" + ack.MW_NO + "，以便跟進。";
                    model.FourthPara = "如本署實地審核上述工程時發現任何欠妥之處，將另函通知。";
                    model.FifthPara = "如對本信有任何疑問，可致電" + poContact + "與本署職員" + poNameChi + "聯絡。";
                    model.SpoName = po != null ? po.SPO_NAME_CHI : "";
                    model.SpoPost = po != null ? po.SPO_POST_CHI : "";
                    model.PawName = ack.PAW;
                    model.PawContact = ack.PAW_CONTACT;
                }
                else // english
                {
                    model.PoName = poNameEng;
                    model.PoPost = poPostEng;
                    if (ProcessingConstant.FORM_01.Equals(ack.FORM_NO) || ProcessingConstant.FORM_03.Equals(ack.FORM_NO))
                    {
                        status = "to be commenced";
                    }
                    else if (ProcessingConstant.FORM_02.Equals(ack.FORM_NO) || ProcessingConstant.FORM_04.Equals(ack.FORM_NO) || ProcessingConstant.FORM_05.Equals(ack.FORM_NO))
                    {
                        status = "completed";
                    }
                    else
                    {
                        status = "carried out";
                    }
                    model.Heading = "(By Fax and Post)";
                    model.Title = "Dear Sir/Madam,";
                    model.Caption = "Minor Works Submission No. " + ack.MW_NO;
                    model.FirstPara =
                        "The specified form (" + ack.FORM_NO + ") submitted by you under the simplified requirements for the minor works " + status + " at " +
                        "the captioned premises was received on " + model.ReceivedDateDisplay + ". The said minor works have been selected for audit check to ensure that " +
                        "the works comply with the Buildings Ordinance and its subsidiary legislation, and have been carried out in accordance with " +
                        "the submitted plans and description of works. During our initial audit check of the documents you submitted, the following irregularities were found:";
                    model.SecondPara =
                        "Please submit supplementary information together with Form MW33 and rectify the above irregularities within 14 days from the date of this letter, for us to proceed with your case.";
                    model.ThirdPara =
                        "When resubmitting forms and documents for the above minor works, please specify " + ack.MW_NO + " in the \"Previously Related Minor Works Submission Number\" box in your new form to facilitate follow-up.";
                    model.FourthPara =
                        "If any irregularity is found during the site audit of the above minor works, " +
                        "we will write to you again.";
                    model.FifthPara =
                        "Should you have any enquiries, please contact " + poNameEng + " at " + poContact + ". ";
                    model.SpoName = po != null ? po.SPO_NAME_ENG : "";
                    model.SpoPost = po != null ? po.SPO_POST_ENG : "";
                    model.PawName = ack.PAW;
                    model.PawContact = ack.PAW_CONTACT;
                }


                model.Language = ack.LANGUAGE;

                return model;
            }

        }

        public void updateAckLetterList(Fn01LM_ALDisplayModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                ProcessingLetterModuleDAOService dao = new ProcessingLetterModuleDAOService();
                Fn01LM_AckSearchModel saveModel = new Fn01LM_AckSearchModel();
                saveModel.P_MW_ACK_LETTER = db.P_MW_ACK_LETTER.Where(x => x.DSN == model.DSN).FirstOrDefault();
                saveModel.P_MW_ACK_LETTER.LETTER_DATE = model.LetterDate;
                saveModel.P_MW_ACK_LETTER.LANGUAGE = model.Language;
                saveModel.P_MW_ACK_LETTER.FILE_TYPE = model.FileType;
                dao.UpdateAckLetter(saveModel, db);

                List<P_MW_DW_LETTER_ITEM> items = db.P_MW_DW_LETTER_ITEM.Where(x => x.MW_ACK_LETTER_ID == model.P_MW_ACK_LETTER.UUID).ToList();
                List<P_MW_DW_LETTER_ITEM> newItems = new List<P_MW_DW_LETTER_ITEM>();
                foreach (var item in items)
                {
                    P_MW_DW_LETTER_ITEM newItem = new P_MW_DW_LETTER_ITEM();
                    newItem.LETTER_TYPE = item.LETTER_TYPE;
                    newItem.ITEM_NO = item.ITEM_NO;
                    newItem.ITEM_TEXT = item.ITEM_TEXT;
                    newItem.ITEM_TEXT_E = item.ITEM_TEXT_E;
                    newItem.MW_ACK_LETTER_ID = item.MW_ACK_LETTER_ID;

                    newItem.CHECKED = model.ItemCheckList[item.UUID] == true ? ProcessingConstant.FLAG_Y : ProcessingConstant.FLAG_N;
                    if (ProcessingConstant.LANGUAGE_RADIO_CHINESE.Equals(model.Language)) // chinese
                    {
                        newItem.ITEM_TEXT = model.ItemTextList[item.UUID];
                    }
                    else // english
                    {
                        newItem.ITEM_TEXT_E = model.ItemTextList[item.UUID];
                    }
                    newItems.Add(newItem);
                }
                dao.saveMwDwLetterItemForAckLetter(newItems, saveModel.P_MW_ACK_LETTER);
            }
        }

    }
}













