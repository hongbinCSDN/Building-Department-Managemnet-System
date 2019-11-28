using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ComSubmissionDAOService : BaseDAOService
    {

        public Fn03TSK_SSModel GetSubmissionInfoByRecordID(Fn03TSK_SSModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.P_MW_REFERENCE_NO = db.P_MW_REFERENCE_NO.Where(m => m.REFERENCE_NO == model.P_MW_REFERENCE_NO.REFERENCE_NO).FirstOrDefault();
                model.P_MW_RECORD = db.P_MW_RECORD.Where(m => m.REFERENCE_NUMBER == model.P_MW_REFERENCE_NO.UUID && m.IS_DATA_ENTRY == ProcessingConstant.FLAG_N).FirstOrDefault();

                if (model.P_MW_RECORD != null)
                {
                    model.PemCompetionDate = db.P_MW_RECORD.Where(m => m.REFERENCE_NUMBER == model.P_MW_RECORD.REFERENCE_NUMBER && m.COMPLETION_DATE != null).Max(m => m.COMPLETION_DATE);
                    if (model.P_MW_RECORD.LANGUAGE_CODE != null && model.P_MW_RECORD.LANGUAGE_CODE == ProcessingConstant.LANG_CHINESE)
                    {
                        model.AddressCarriedOut = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK + " " + model.P_MW_RECORD.P_MW_ADDRESS.CHINESE_DISPLAY;
                    }
                    else
                    {
                        model.AddressCarriedOut = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK + " " + model.P_MW_RECORD.P_MW_ADDRESS.ENGLISH_DISPLAY;
                    }

                    if (model.P_MW_RECORD.MW_PROGRESS_STATUS_CODE == ProcessingConstant.COMMENCEMENT)
                    {
                        model.P_MW_RECORD.MW_PROGRESS_STATUS_CODE = ProcessingConstant.MW_STATUS_IN_PROGRESS;
                    }
                    else if (model.P_MW_RECORD.MW_PROGRESS_STATUS_CODE == ProcessingConstant.COMPLETION)
                    {
                        model.P_MW_RECORD.MW_PROGRESS_STATUS_CODE = ProcessingConstant.MW_STATUS_COMPLETED;
                    }

                    //owner
                    model.OWNER = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_RECORD.OWNER_ID).FirstOrDefault();
                    if (model.OWNER != null)
                    {

                        if (model.P_MW_RECORD.LANGUAGE_CODE != null && model.P_MW_RECORD.LANGUAGE_CODE == ProcessingConstant.LANG_CHINESE)
                        {
                            model.CorrespondenceAddress = model.OWNER.P_MW_ADDRESS.CHINESE_DISPLAY; ;
                        }
                        else
                        {
                            model.CorrespondenceAddress = model.OWNER.P_MW_ADDRESS.ENGLISH_DISPLAY;
                        }
                    }

                    //Signboard
                    model.SIGNBOARD = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_RECORD.SIGNBOARD_PERFROMER_ID).FirstOrDefault();
                    if (model.SIGNBOARD != null)
                    {
                        if (model.P_MW_RECORD.LANGUAGE_CODE != null && model.P_MW_RECORD.LANGUAGE_CODE == ProcessingConstant.LANG_CHINESE)
                        {
                            model.CorrespondenceAddressOfSignboard = model.SIGNBOARD.P_MW_ADDRESS.CHINESE_DISPLAY;
                        }
                        else
                        {
                            model.CorrespondenceAddressOfSignboard = model.SIGNBOARD.P_MW_ADDRESS.ENGLISH_DISPLAY;
                        }
                    }

                    //mw item
                    model.ItemList = db.P_MW_RECORD_ITEM.Where(m => m.MW_RECORD_ID == model.P_MW_RECORD.UUID
                        && m.STATUS_CODE == ProcessingConstant.MW_RECORD_ITEM_STATUS_FINAL).OrderBy(m => m.ORDERING).ToList();

                    //if (model.ItemList != null && model.ItemList.Count > 0)
                    //{

                    //    List<ItemHistoryList> itemHistoryList = new List<ItemHistoryList>();
                    //    for (int i = 0; i < model.ItemList.Count; i++)
                    //    {
                    //        P_MW_RECORD_ITEM item = model.ItemList[i];
                    //        List<P_MW_RECORD_ITEM_HISTORY> historyList = db.P_MW_RECORD_ITEM_HISTORY.Where(m => m.ITEM_KEY == item.UUID).ToList();
                    //        ItemHistoryList itemHistory = new ItemHistoryList();
                    //        itemHistory.Item = item;
                    //        itemHistory.HistoryList = historyList;
                    //        itemHistoryList.Add(itemHistory);
                    //    }
                    //}

                    //appointed professional	
                    List<P_MW_APPOINTED_PROFESSIONAL> apList = db.P_MW_APPOINTED_PROFESSIONAL.Where(m => m.MW_RECORD_ID == model.P_MW_RECORD.UUID).ToList();

                    MwViewFormPerson person1 = new MwViewFormPerson();
                    MwViewFormPerson person2 = new MwViewFormPerson();
                    MwViewFormPerson person3 = new MwViewFormPerson();
                    MwViewFormPerson person4 = new MwViewFormPerson();

                    List<P_MW_APPOINTED_PROF_HISTORY> apHistoryList = null;
                    List<P_MW_APPOINTED_PROF_HISTORY> rseHistoryList = null;
                    List<P_MW_APPOINTED_PROF_HISTORY> rgeHistoryList = null;
                    List<P_MW_APPOINTED_PROF_HISTORY> prcHistoryList = null;

                    for (int i = 0; i < apList.Count; i++)
                    {
                        P_MW_APPOINTED_PROFESSIONAL ap = apList[i];
                        if (ap.IDENTIFY_FLAG == ProcessingConstant.AP)
                        {
                            person1.EffectFromDate = Convert.ToDateTime(ap.EFFECT_FROM_DATE).ToString("dd/MM/yyyy");
                            person1.EffectToDate = Convert.ToDateTime(ap.EFFECT_TO_DATE).ToString("dd/MM/yyyy");
                            person1.Class1CeaseDate = Convert.ToDateTime(ap.CLASS1_CEASE_DATE).ToString("dd/MM/yyyy");

                            CreateProfessional(ap, person1, ProcessingConstant.AUTHORIZED_PERSON + "/" + ProcessingConstant.REGISTERED_INSPECTOR);
                            apHistoryList = db.P_MW_APPOINTED_PROF_HISTORY.Where(m => m.MW_APPOINTED_PROFESSIONAL_ID == ap.UUID).ToList();
                            if (apHistoryList.Count > 0)
                            {
                                model.ApHistoryList = apHistoryList;
                            }
                        }
                        if (ap.IDENTIFY_FLAG == ProcessingConstant.RSE)
                        {
                            person2.EffectFromDate = Convert.ToDateTime(ap.EFFECT_FROM_DATE).ToString("dd/MM/yyyy");
                            person2.EffectToDate = Convert.ToDateTime(ap.EFFECT_TO_DATE).ToString("dd/MM/yyyy");
                            person2.Class1CeaseDate = Convert.ToDateTime(ap.CLASS1_CEASE_DATE).ToString("dd/MM/yyyy");

                            CreateProfessional(ap, person2, ProcessingConstant.REGISTERED_STRUCTURAL_ENGINEER);
                            rseHistoryList = db.P_MW_APPOINTED_PROF_HISTORY.Where(m => m.MW_APPOINTED_PROFESSIONAL_ID == ap.UUID).ToList();
                            if (rseHistoryList.Count > 0)
                            {
                                model.RseHistoryList = rseHistoryList;
                            }
                        }
                        if (ap.IDENTIFY_FLAG == ProcessingConstant.RGE)
                        {
                            person3.EffectFromDate = Convert.ToDateTime(ap.EFFECT_FROM_DATE).ToString("dd/MM/yyyy");
                            person3.EffectToDate = Convert.ToDateTime(ap.EFFECT_TO_DATE).ToString("dd/MM/yyyy");
                            person3.Class1CeaseDate = Convert.ToDateTime(ap.CLASS1_CEASE_DATE).ToString("dd/MM/yyyy");

                            CreateProfessional(ap, person3, ProcessingConstant.REGISTERED_GEOTECHNICAL_ENGINEER);
                            rgeHistoryList = db.P_MW_APPOINTED_PROF_HISTORY.Where(m => m.MW_APPOINTED_PROFESSIONAL_ID == ap.UUID).ToList();
                            if (rgeHistoryList.Count > 0)
                            {
                                model.RgeHistoryList = rgeHistoryList;
                            }
                        }
                        if (ap.IDENTIFY_FLAG == ProcessingConstant.PRC)
                        {
                            person4.EffectFromDate = Convert.ToDateTime(ap.EFFECT_FROM_DATE).ToString("dd/MM/yyyy");
                            person4.EffectToDate = Convert.ToDateTime(ap.EFFECT_TO_DATE).ToString("dd/MM/yyyy");
                            person4.Class1CeaseDate = Convert.ToDateTime(ap.CLASS1_CEASE_DATE).ToString("dd/MM/yyyy");
                            person4.Class2CeaseDate = Convert.ToDateTime(ap.CLASS2_CEASE_DATE).ToString("dd/MM/yyyy");

                            CreateProfessional(ap, person4, ProcessingConstant.PRESCRIBED_REGISTER_CONTRACTOR);
                            prcHistoryList = db.P_MW_APPOINTED_PROF_HISTORY.Where(m => m.MW_APPOINTED_PROFESSIONAL_ID == ap.UUID).ToList();
                            if (prcHistoryList.Count > 0)
                            {
                                model.PrcHistoryList = prcHistoryList;
                            }
                            person4.MwNo = ap.MW_NO;
                            model.Prc = person4;
                        }
                    }
                    model.ProfessionalList = new List<MwViewFormPerson>();
                    if (!string.IsNullOrWhiteSpace(person1.IdentifyFlag))
                    {
                        model.ProfessionalList.Add(person1);
                    }
                    if (!string.IsNullOrWhiteSpace(person2.IdentifyFlag))
                    {
                        model.ProfessionalList.Add(person2);
                    }
                    if (!string.IsNullOrWhiteSpace(person3.IdentifyFlag))
                    {
                        model.ProfessionalList.Add(person3);
                    }
                    if (!string.IsNullOrWhiteSpace(person4.IdentifyFlag))
                    {
                        model.ProfessionalList.Add(person4);
                    }

                    //Start modify by dive 20191017
                    model.P_MW_SCANNED_DOCUMENTsNIC = GetP_MW_SCANNED_DOCUMENTsNICByRefNO(model.P_MW_REFERENCE_NO.REFERENCE_NO);
                    model.P_MW_SCANNED_DOCUMENTsIC = GetP_MW_SCANNED_DOCUMENTsICByRefNO(model.P_MW_REFERENCE_NO.REFERENCE_NO);
                    //End modify by dive 20191017

                    //processing page
                    List<P_MW_RECORD> mwRecordList = db.P_MW_RECORD.Where(m => m.REFERENCE_NUMBER == model.P_MW_REFERENCE_NO.UUID).ToList();
                    for (int i = 0; i < mwRecordList.Count; i++)
                    {
                        P_MW_RECORD p_MW_RECORD = mwRecordList[i];
                        P_MW_DSN dsn = db.P_MW_DSN.Where(m => m.DSN == p_MW_RECORD.MW_DSN).FirstOrDefault();
                        string dsnStr = dsn.DSN;
                        string nature = dsn.SUBMISSION_NATURE;
                        if (ProcessingConstant.SUBMISSION_VALUE_NATURE_SUBMISSION == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_SUBMISSION;
                        }
                        else if (ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_ESUBMISSION;
                        }
                        else if (ProcessingConstant.SUBMISSION_VALUE_NATURE_ICU == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_ICU;
                        }
                        else if (ProcessingConstant.SUBMISSION_VALUE_NATURE_DIRECT_RETURN == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_DIRECT_RETURN;
                        }
                        else if (ProcessingConstant.SUBMISSION_VALUE_NATURE_REVISED_CASE == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_REVISED_CASE;
                        }
                        else if (ProcessingConstant.SUBMISSION_VALUE_NATURE_WITHDRAWAL == nature)
                        {
                            nature = ProcessingConstant.SUBMISSION_DISPLAY_NATURE_WITHDRAWAL;
                        }
                        mwRecordList[i].CLASS_CODE = nature;

                        P_MW_ACK_LETTER mwAckLetter = db.P_MW_ACK_LETTER.Where(m => m.DSN == dsnStr).FirstOrDefault();
                        if (mwAckLetter != null)
                        {
                            //mwRecordList[i].Remarks = mwAckLetter.REMARK;
                        }
                    }
                    model.ProcessingList = mwRecordList;

                    //OI
                    model.OI = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_RECORD.OI_ID).FirstOrDefault();
                    if (model.OI != null)
                    {
                        model.OIAddress = db.P_MW_ADDRESS.Where(m => m.UUID == model.OI.MW_ADDRESS_ID).FirstOrDefault();
                    }
                    else
                    {
                        model.OI = new P_MW_PERSON_CONTACT();
                        model.OIAddress = new P_MW_ADDRESS();
                    }
                }

                return model;
            }
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTsNICByRefNO(string REFERENCE_NO)
        {
            string sSql = @"SELECT SD.*
                            FROM   P_MW_Reference_No RN
                                   JOIN P_MW_DSN D
                                     ON RN.REFERENCE_NO = D.RECORD_ID
                                        AND D.SUBMIT_TYPE != :SUBMIT_TYPE
                                   JOIN P_MW_SCANNED_DOCUMENT SD
                                     ON D.UUID = SD.DSN_ID
                            WHERE  RN.REFERENCE_NO = :REFERENCE_NO ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SUBMIT_TYPE",ProcessingConstant.SUBMIT_TYPE_ISSUED_CORR),
                new OracleParameter(":REFERENCE_NO",REFERENCE_NO)
            };
            return GetObjectData<P_MW_SCANNED_DOCUMENT>(sSql, oracleParameters).ToList();
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTsICByRefNO(string REFERENCE_NO)
        {
            string sSql = @"SELECT SD.*
                            FROM   P_MW_Reference_No RN
                                   JOIN P_MW_DSN D
                                     ON RN.REFERENCE_NO = D.RECORD_ID
                                        AND D.SUBMIT_TYPE = :SUBMIT_TYPE
                                   JOIN P_MW_SCANNED_DOCUMENT SD
                                     ON D.UUID = SD.DSN_ID
                            WHERE  RN.REFERENCE_NO = :REFERENCE_NO ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SUBMIT_TYPE",ProcessingConstant.SUBMIT_TYPE_ISSUED_CORR),
                new OracleParameter(":REFERENCE_NO",REFERENCE_NO)
            };
            return GetObjectData<P_MW_SCANNED_DOCUMENT>(sSql, oracleParameters).ToList();
        }

        public void CreateProfessional(P_MW_APPOINTED_PROFESSIONAL ap, MwViewFormPerson person1, string identifyFlag)
        {
            person1.CertificationNo = ap.CERTIFICATION_NO;
            person1.EnglishName = ap.ENGLISH_NAME;
            person1.ChineseName = ap.CHINESE_NAME;
            person1.IdentifyFlag = identifyFlag;
            person1.ApEnglishName = ap.ENGLISH_COMPANY_NAME;
            person1.ApChineseName = ap.CHINESE_COMPANY_NAME;
            person1.FaxNo = ap.FAX_NO;
            person1.ContactNo = ap.CONTACT_NO;
            person1.MwNo = ap.MW_NO;
        }

        public List<SubDSNRecord> CreateSubDsnList(P_MW_DSN mwDsn, List<MwScannedDocument> docList, String result, String resultDate)
        {
            List<SubDSNRecord> subDSNList = new List<SubDSNRecord>();

            foreach (var doc in docList)
            {
                SubDSNRecord sub = new SubDSNRecord();
                sub.SubDsn = doc.ScannedDocument.DSN_SUB;
                sub.DocType = doc.ScannedDocument.DOCUMENT_TYPE;
                sub.FormCode = mwDsn.FORM_CODE;
                sub.TotalPage = doc.ScannedDocument.PAGE_COUNT.ToString();
                sub.Date = doc.ScannedDocument.SCAN_DATE.ToString("dd/MM/yyyy");
                sub.Status = doc.ScannedDocument.STATUS_CODE;
                sub.DocTitle = doc.ScannedDocument.DOC_TITLE;
                sub.FolderType = doc.ScannedDocument.FOLDER_TYPE;
                sub.EncryptedDsn = doc.ScannedDocument.DSN_SUB;
                sub.FilePath = doc.ScannedDocument.RELATIVE_FILE_PATH;

                if (!string.IsNullOrWhiteSpace(result))
                {
                    sub.Result = result;
                }

                if (!string.IsNullOrWhiteSpace(resultDate))
                {
                    sub.ResultDate = resultDate;
                }

                sub.RefNo = mwDsn.RECORD_ID;
                sub.ReceivedDate = doc.ReceivedDate;

                subDSNList.Add(sub);
            }
            return subDSNList;
        }

        public ServiceResult CheckIsVeri(string r_uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                VerificaionFormModel model = new VerificaionFormModel();
                List<P_MW_VERIFICATION> verifications = db.P_MW_VERIFICATION.Where(m => m.MW_RECORD_ID == r_uuid).ToList();
                if (verifications.Count() > 0)
                {
                    model.R_UUID = r_uuid;
                    model.V_UUID = db.P_MW_VERIFICATION.Where(m => m.MW_RECORD_ID == r_uuid).FirstOrDefault().UUID;
                    model.IsReadonly = true;
                }
                return new ServiceResult()
                {
                    Data = model
                };
            }
        }
    }
}