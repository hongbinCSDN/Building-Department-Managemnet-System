using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using NPOI.SS.Formula.Functions;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.WorkFlow;
using System.Web.Mvc;
using System.IO;
using NPOI.XWPF.UserModel;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardLMService : SignboardCommentService
    {


        private static readonly object locker = new object();
        //private static volatile List<SelectListItem> _MWItems;
        //private static volatile List<SelectListItem> _VDItems;
        //private static IEnumerable<SelectListItem> MWItems { get { if (_MWItems == null) lock (locker) if (_MWItems == null) { using (EntitiesSignboard db = new EntitiesSignboard()) _MWItems = db.B_S_SYSTEM_VALUE.Where(o => o.B_S_SYSTEM_TYPE.TYPE == SignboardConstant.SYSTEM_TYPE_ITEM_NO && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }).ToList(); _MWItems.Insert(0, new SelectListItem { Text = "", Value = "" }); } return _MWItems; } }
        //private static IEnumerable<SelectListItem> VDItems { get { if (_VDItems == null) lock (locker) if (_VDItems == null) { using (EntitiesSignboard db = new EntitiesSignboard()) _VDItems = db.B_S_SYSTEM_VALUE.Where(o => o.B_S_SYSTEM_TYPE.TYPE == SignboardConstant.SYSTEM_TYPE_VALIDATION_ITEM && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }).ToList(); _VDItems.Insert(0, new SelectListItem { Text = "", Value = "" }); } return _VDItems; } }



        /* public statuic



         private static volatile AtcpBLService _BL;
         private static readonly object locker = new object();
         private static AtcpBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new AtcpBLService(); return _BL; } }
         */

        private EntitiesSignboard db = new EntitiesSignboard();
        string SearchDE_q = ""
             + "\r\n" + "\t" + " select                                                                          "
             + "\r\n" + "\t" + " svs.UUID,                                                                       "
             + "\r\n" + "\t" + " svs.REFERENCE_NO as REFNO, svsd.DSN_NUMBER as DSNNUM,                           "
             + "\r\n" + "\t" + " svs.FORM_CODE as FCODE, svs.SCU_RECEIVED_DATE as RECEDATE,                      "
             + "\r\n" + "\t" + " TO_CHAR(svs.SCU_RECEIVED_DATE, 'hh24:mi')as RDTIME,                             "
             + "\r\n" + "\t" + " svs.BATCH_NO as BNO,                                                            "
             + "\r\n" + "\t" + " decode(svs.status,                                                              "
             + "\r\n" + "\t" + " '" + SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY + "', 'Draft',       "
             + "\r\n" + "\t" + " '" + SignboardConstant.SV_SUBMISSION_STATUS_SCU_RECEIVED + "','Delivered',      "
             + "\r\n" + "\t" + " svs.status) as STATUS                                                           "
             + "\r\n" + "\t" + " from b_sv_submission svs                                                        "
             + "\r\n" + "\t" + " inner join B_SV_SCANNED_DOCUMENT svsd on svs.SV_SCANNED_DOCUMENT_ID = svsd.uuid "
             + "\r\n" + "\t" + " where 1=1                                                                       "
            ;
        public DataEntrySearchModel SearchDataEntryList(DataEntrySearchModel model)
        {
            model.Query = SearchDE_q;
            model.QueryWhere = SearchDE_q_whereQ(model);
            model.Rpp = -1;
            model.Search();
            return model;
        }

        public string Excel(DataEntrySearchModel model)
        {
            model.Query = SearchDE_q;
            model.QueryWhere = SearchDE_q_whereQ(model);

            return model.Export("Excel_" + DateTime.Now.ToString("yyyy_MM_dd"));
        }

        private string SearchDE_q_whereQ(DataEntrySearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.searchFileRefNo))
            {
                whereQ += "\r\n\t" + "AND svs.REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.searchFileRefNo.Trim().ToUpper() + "%");
            }
            if (model.searchReceivedDateFrom != null)
            {
                whereQ += "\r\n\t" + "AND svs.SCU_RECEIVED_DATE >= :scuReceivedFromDate";
                model.QueryParameters.Add("scuReceivedFromDate", model.searchReceivedDateFrom);
            }
            if (model.searchReceivedDateTo != null)
            {
                whereQ += "\r\n\t" + "AND svs.SCU_RECEIVED_DATE <= :scuReceivedToDate";
                model.QueryParameters.Add("scuReceivedToDate", model.searchReceivedDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                whereQ += "\r\n\t" + "AND svs.status = :status";
                model.QueryParameters.Add("status", model.Status);
            }
            else
            {
                whereQ += "\r\n\t" + "AND svs.status in ('" + SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY + "','" + SignboardConstant.SV_SUBMISSION_STATUS_SCU_RECEIVED + "')";
            }
            return whereQ;
        }
        public void removeBatch(DataEntrySearchModel model)
        {
            SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_SV_SUBMISSION svSubmission = new B_SV_SUBMISSION();
                svSubmission = svSubmissionDAOService.getByUuid(model.svSubmission.UUID);
                svSubmission.BATCH_NO = "";
                var fromSVSData = db.B_SV_SUBMISSION.Find(svSubmission.UUID);
                db.Entry(fromSVSData).CurrentValues.SetValues(svSubmission);
                db.SaveChanges();
            }
        }
        public DataEntrySearchModel PopulateBatchAssignment(DataEntrySearchModel model, string NumType)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SignboardSequenceNumberGenerator signboardSequenceNumberGenerator = new SignboardSequenceNumberGenerator();
                List<string> selectSVList = new List<string>();

                selectSVList = model.selectedDSNList;
                selectSVList.RemoveAll(item => item == "");
                if (NumType == SignboardConstant.NuMBER_TYPE_SV_BATCH)
                {

                    for (int i = 0; i < selectSVList.Count(); i++)
                    {
                        string svsUUID = selectSVList[i].ToString();
                        DateTime today = DateTime.Now;
                        string Year = today.Year.ToString().Substring(2);
                        string Month = today.Month.ToString();
                        Month = DateUtil.padZero(Month, 2);

                        string prefix = Year + "/" + Month;
                        long runningNumber = signboardSequenceNumberGenerator.getSequenceNumber(NumType, prefix);
                        model.BactchNum = SignboardConstant.NuMBER_TYPE_SV_BATCH + Month + "-" + DateUtil.padZero(runningNumber.ToString(), 4) + "/" + Year;

                        B_SV_SUBMISSION BatchNo = (from svsubm in db.B_SV_SUBMISSION
                                                   where svsubm.UUID == svsUUID
                                                   select svsubm).FirstOrDefault();

                        BatchNo.BATCH_NO = model.BactchNum;
                        var fromSVSData = db.B_SV_SUBMISSION.Find(BatchNo.UUID);
                        db.Entry(fromSVSData).CurrentValues.SetValues(BatchNo);
                        db.SaveChanges();
                    }

                }

            }

            return new DataEntrySearchModel()
            {
                BactchNum = model.BactchNum
            };
        }


        public string ExportLM(DataEntrySearchModel model)
        {
            model.Query = SearchDE_q;
            model.QueryWhere = SearchDE_q_whereQ(model);
            return model.Export("Letter Module Report");
        }

        public DataEntryDisplayModel editDataEntry(string submssionUUID, string formCodeType)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                SvRecordItemDAOService svRecordItemDAOService = new SvRecordItemDAOService();
                SvAppointedProfessionalDAOService svAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();
                SvRecordValidationItemDAOService svRecordValidationItemDAOService = new SvRecordValidationItemDAOService();
                LetterTemplateDAOService letterTemplateDAOService = new LetterTemplateDAOService();
                List<B_SV_RECORD_VALIDATION_ITEM> svRECORDValidationItem = new List<B_SV_RECORD_VALIDATION_ITEM>();

                B_SV_SUBMISSION SvSubmission = (from svs in db.B_SV_SUBMISSION
                                                where svs.UUID == submssionUUID
                                                select svs).FirstOrDefault();

                B_SV_RECORD svRecord = svRecordDAOService.getSVRecordBySvSubmissionUUID(submssionUUID);

                B_SV_SIGNBOARD svSignboard = new B_SV_SIGNBOARD();
                B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                List<B_SV_RECORD_ITEM> itemList = new List<B_SV_RECORD_ITEM>();
                B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_ADDRESS ownerAddress = new B_SV_ADDRESS();
                string formCode = formCodeType;
                string svRecordUUID = "";
                if (svRecord != null)
                {
                    svRecordUUID = svRecord.UUID;
                }


                string RFIDList = "";
                string FormMode = "1";
                DateTime? RfidTime = null;
                string SaveMode = "save";
                string ExportLetterFlag = "N";
                string involved = "1";
                string validated = "1";
                DateTime? ValidationExpiryDate = new DateTime();
                if (svRecord.VALIDATION_EXPIRY_DATE != null)
                {
                    ValidationExpiryDate = svRecord.VALIDATION_EXPIRY_DATE;
                }
                else
                {
                    ValidationExpiryDate = new DateTime(DateTime.Now.Year + 5, DateTime.Now.Month, DateTime.Now.Day);
                }

                //List<B_SV_RECORD_VALIDATION_ITEM> selectedVaItemsR = (from svItem in db.B_SV_RECORD_VALIDATION_ITEM
                //                                                      select svItem.VALIDATION_ITEM).Distinct().ToList();
                DateTime? ReceivedDate = new DateTime();
                if (!string.IsNullOrWhiteSpace(svRecord.UUID))
                {
                    ReceivedDate = svRecord.RECEIVED_DATE;
                }
                else
                {
                    ReceivedDate = SvSubmission.SCU_RECEIVED_DATE;
                }

                if (!string.IsNullOrWhiteSpace(svRecordUUID))
                {
                    //List<B_SV_RECORD_VALIDATION_ITEM> svRECORDValidationItem = (from item in db.B_SV_RECORD_VALIDATION_ITEM
                    //                                                            where item.SV_RECORD_ID == svRecordUUID
                    //                                                            select item).ToList();
                    svRECORDValidationItem = (from item in db.B_SV_RECORD_VALIDATION_ITEM
                                              where item.SV_RECORD_ID == svRecordUUID
                                              select item).ToList();

                    svSignboard = db.B_SV_SIGNBOARD.Where
                        (o => o.UUID == svRecord.SV_SIGNBOARD_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    owner = db.B_SV_PERSON_CONTACT.Where
                        (o => o.UUID == svSignboard.OWNER_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    paw = db.B_SV_PERSON_CONTACT.Where
                        (o => o.UUID == svRecord.PAW_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    oi = db.B_SV_PERSON_CONTACT.Where
                        (o => o.UUID == svRecord.OI_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    ownerAddress = (from svaddre in db.B_SV_ADDRESS
                                    join svpc in db.B_SV_PERSON_CONTACT on svaddre.UUID equals svpc.SV_ADDRESS_ID
                                    join svs in db.B_SV_SIGNBOARD on svpc.UUID equals svSignboard.OWNER_ID
                                    select svaddre).FirstOrDefault();

                    itemList = (from svri in db.B_SV_RECORD_ITEM
                                where svri.UUID == svRecordUUID
                                select svri).ToList();

                    ap = (from svri in db.B_SV_APPOINTED_PROFESSIONAL
                          where svri.SV_RECORD_ID == svRecordUUID
                          && svri.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_AP
                          select svri).FirstOrDefault();

                    rge = (from svri in db.B_SV_APPOINTED_PROFESSIONAL
                           where svri.SV_RECORD_ID == svRecordUUID
                           && svri.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RGE
                           select svri).FirstOrDefault();

                    rse = (from svri in db.B_SV_APPOINTED_PROFESSIONAL
                           where svri.SV_RECORD_ID == svRecordUUID
                           && svri.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RSE
                           select svri).FirstOrDefault();

                    prc = (from svri in db.B_SV_APPOINTED_PROFESSIONAL
                           where svri.SV_RECORD_ID == svRecordUUID
                           && svri.IDENTIFY_FLAG == SignboardConstant.PRC_CODE
                           select svri).FirstOrDefault();

                    List<B_SV_RFID> rfidList = (from svrfid in db.B_SV_RFID
                                                where svrfid.UUID == svRecordUUID
                                                select svrfid).ToList();

                    if (rfidList.Count() > 0)
                    {
                        RFIDList = rfidList[0].RFID;
                        RfidTime = rfidList[0].RFID_DATE;
                    }

                    FormMode = "1";
                    ExportLetterFlag = "N";
                }
                else
                {

                    svRecord = new B_SV_RECORD();

                }

                if (string.IsNullOrEmpty(svRecord.LANGUAGE_CODE))
                    svRecord.LANGUAGE_CODE = "ZH";
                if (svSignboard.B_SV_ADDRESS == null)
                    svSignboard.B_SV_ADDRESS = new B_SV_ADDRESS() { REGION = "HK" };
                if (string.IsNullOrEmpty(owner.ID_TYPE))
                    owner.ID_TYPE = "1";
                if (string.IsNullOrEmpty(ownerAddress.REGION))
                    ownerAddress.REGION = "HK";
                if (string.IsNullOrEmpty(oi.ID_TYPE))
                    oi.ID_TYPE = "1";
                if (oi.B_SV_ADDRESS == null)
                    oi.B_SV_ADDRESS = new B_SV_ADDRESS() { REGION = "HK" };
                if (string.IsNullOrEmpty(paw.ID_TYPE))
                    paw.ID_TYPE = "1";
                if (paw.B_SV_ADDRESS == null)
                    paw.B_SV_ADDRESS = new B_SV_ADDRESS() { REGION = "HK" };
                if (svRecord.NO_OF_SIGNBOARD_VALIDATED != null)
                {
                    validated = svRecord.NO_OF_SIGNBOARD_VALIDATED;
                }
                if (svRecord.NO_OF_SIGNBOARD_INVOLVED != null)
                {
                    involved = svRecord.NO_OF_SIGNBOARD_INVOLVED;
                }
                // set current user as TO handling officer
                string toOfficer = "";
                if (string.IsNullOrWhiteSpace(svRecord.TO_USER_ID))
                {
                    toOfficer = SessionUtil.LoginPost.UUID;
                }
                return new DataEntryDisplayModel()
                {
                    ReceivedDate = ReceivedDate,
                    SvSubmission = SvSubmission,
                    SvRecord = svRecord,
                    SvSignboard = svSignboard,
                    Owner = owner,
                    Paw = paw,
                    Oi = oi,
                    ItemList = itemList,
                    Ap = ap,
                    Rse = rse,
                    Rge = rge,
                    Prc = prc,
                    Rfid = RFIDList,
                    RfidTime = RfidTime,
                    FormCode = formCode,
                    OwnerAddress = ownerAddress,
                    FormMode = FormMode,
                    SaveMode = SaveMode,
                    ExportLetterFlag = ExportLetterFlag,
                    ValidationExpiryDate = ValidationExpiryDate,
                    validated = validated,
                    involved = involved,
                    SvRecordValidationItems = svRECORDValidationItem,
                    TO_Handling_Officer = toOfficer,
                };
            }
        }
        public ServiceResult SaveToScannedDocument(DataEntryDisplayModel model, byte[] doc)
        {
         
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                B_SV_SCANNED_DOCUMENT b = new B_SV_SCANNED_DOCUMENT();
                b.RECORD_ID = svRecordDAOService.getSVRecordBySvSubmissionUUID(model.SvSubmission.UUID).REFERENCE_NO;
                SignboardCommonDAOService ss = new SignboardCommonDAOService();
                b.DSN_NUMBER = ss.GetNumber();

                string PrePath = ApplicationConstant.SMMSCANFilePath;
                string tempRelPath =  DateTime.Now.Year.ToString() + ApplicationConstant.FileSeparator
                                 + DateTime.Now.Month.ToString() + ApplicationConstant.FileSeparator + b.DSN_NUMBER;
                string fullPath = PrePath + tempRelPath;
                string FileName = "LM-" + b.DSN_NUMBER + ".docx";
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                FileStream outFile = new FileStream(fullPath + ApplicationConstant.FileSeparator +FileName, FileMode.CreateNew);
                outFile.Write(doc, 0, doc.Length);
                outFile.Close();


                b.FILE_PATH = fullPath + ApplicationConstant.FileSeparator +
                                   FileName;
                b.DOCUMENT_TYPE = "Letter";
                b.FILE_SIZE_CODE = "A4";
                b.RELATIVE_FILE_PATH = tempRelPath + ApplicationConstant.FileSeparator +
                                   FileName;

                b.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                db.B_SV_SCANNED_DOCUMENT.Add(b);


                db.SaveChanges();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }
        public ServiceResult SaveDataEntry(DataEntryDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SScuTeamManagerService scuTeamManagerService = new SScuTeamManagerService();
                SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                SvRecordItemDAOService svRecordItemDAOService = new SvRecordItemDAOService();
                SvRecordValidationItemDAOService svRecordValidationItemDAOService = new SvRecordValidationItemDAOService();
                SvAppointedProfessionalDAOService svAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();
                SignboardCommonService commonService = new SignboardCommonService();
                LetterTemplateDAOService letterTemplateDAOService = new LetterTemplateDAOService();

                bool isSystemAssign = false;
                bool isSaveMode = false;
                bool updateApplication = false;
                string saveModel = model.SaveMode;
                string submssionUUID = model.SvSubmission.UUID;
                string letterType;
                isSaveMode = SignboardConstant.SAVE_MODE.Equals(model.SaveMode);

                updateApplication = SignboardConstant.FORM_MODE_UPDATE_SIGNBOARD_APPLICATION.Equals(model.FormMode);

                B_SV_SUBMISSION SvSubmission = svSubmissionDAOService.getByUuid(submssionUUID);

                B_SV_RECORD svRecord = svRecordDAOService.getSVRecordBySvSubmissionUUID(submssionUUID);

                //db.B_SV_RECORD_ITEM.RemoveRange(svRecord.B_SV_RECORD_ITEM);
                //List<B_SV_RECORD_ITEM> B_SV_RECORD_ITEMs = model.B_SV_RECORD_ITEMs;
                //db.B_SV_RECORD_ITEM.AddRange(B_SV_RECORD_ITEMs);

                //db.B_SV_RECORD_VALIDATION_ITEM.RemoveRange(svRecord.B_SV_RECORD_VALIDATION_ITEM);
                //List<B_SV_RECORD_VALIDATION_ITEM> B_SV_RECORD_VALIDATION_ITEMs = model.B_SV_RECORD_VALIDATION_ITEMs;
                //db.B_SV_RECORD_VALIDATION_ITEM.AddRange(B_SV_RECORD_VALIDATION_ITEMs);

                if (svRecord.UUID == null)
                {
                    svRecord = new B_SV_RECORD();
                }

                if (model.SelectedLetter == null)
                {
                    letterType = SignboardConstant.LETTER_TYPE_ACKNOWLEDGEMENT_LETTER_CODE;
                }
                else
                {
                    letterType = model.SelectedLetter;
                }
                //List<List<object>> selectedLetterList = new List<List<object>>();
                //selectedLetterList = letterTemplateDAOService.getLetterTemplateListWithPara(model.FormCode, letterType, svRecord.RECOMMENDATION);
                //model.SelectedLetterTypeList.Add(selectedLetterList[0].ToString());

                if (!updateApplication)
                {

                    if (SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_COMPLETED.Equals(svRecord.STATUS_CODE))
                    {
                        throw new Exception("can no save! This record had been changed by other!");
                    }
                    if (SignboardConstant.SAVE_MODE.Equals(saveModel))
                    {
                        SvSubmission.STATUS = SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY;
                    }
                    else
                    {
                        SvSubmission.STATUS = SignboardConstant.SV_SUBMISSION_STATUS_VALIDATION;
                        svRecord.SUBMITTED_BY = "LoginName";
                    }
                    if (model.FormCode != null)
                    {
                        SvSubmission.FORM_CODE = model.FormCode;
                        svRecord.FORM_CODE = model.FormCode;
                    }
                    SvSubmission.BATCH_NO = model.SvSubmission.BATCH_NO;
                }

                B_SV_SIGNBOARD svSignboard = new B_SV_SIGNBOARD();
                B_SV_ADDRESS svSignboardAddress = new B_SV_ADDRESS();
                B_SV_PERSON_CONTACT svSignboardPersonContact = new B_SV_PERSON_CONTACT();
                B_SV_ADDRESS svSignboardPersonContactAddress = new B_SV_ADDRESS();

                B_SV_PERSON_CONTACT pawPersonContact = new B_SV_PERSON_CONTACT();
                B_SV_PERSON_CONTACT oiPersonContact = new B_SV_PERSON_CONTACT();
                B_SV_ADDRESS pawAddress = new B_SV_ADDRESS();
                B_SV_ADDRESS oiAddress = new B_SV_ADDRESS();

                List<B_SV_RECORD_ITEM> itemList = new List<B_SV_RECORD_ITEM>();
                List<B_SV_RECORD_VALIDATION_ITEM> vaItemList = new List<B_SV_RECORD_VALIDATION_ITEM>();



                B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();

                //List<B_SV_RFID> rfidList = new List<B_SV_RFID>();

                B_SV_RV_ADDRESS svRVSignboardAddress = new B_SV_RV_ADDRESS();
                B_SV_RV_ADDRESS svRVSignboardPersonContactAddress = new B_SV_RV_ADDRESS();
                B_SV_RV_ADDRESS pawRVAddress = new B_SV_RV_ADDRESS();
                B_SV_RV_ADDRESS oiRVAddress = new B_SV_RV_ADDRESS();

                string svRecordUUID = "";

                if (!string.IsNullOrWhiteSpace(svRecord.UUID))
                {
                    svRecordUUID = svRecord.UUID;
                }
                if (!string.IsNullOrWhiteSpace(svRecordUUID))
                {
                    svSignboard = db.B_SV_SIGNBOARD.Where
                        (o => o.UUID == svRecord.SV_SIGNBOARD_ID).FirstOrDefault();

                    svSignboardAddress = db.B_SV_ADDRESS.Where
                        (o => o.UUID == svSignboard.LOCATION_ADDRESS_ID).FirstOrDefault();

                    svSignboardPersonContact = db.B_SV_PERSON_CONTACT.Where
                        (o => o.UUID == svSignboard.OWNER_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    svSignboardPersonContactAddress = db.B_SV_ADDRESS.Where
                        (o => o.UUID == svSignboardPersonContact.SV_ADDRESS_ID).FirstOrDefault();



                    pawPersonContact = db.B_SV_PERSON_CONTACT.Where
                            (o => o.UUID == svRecord.PAW_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    oiPersonContact = db.B_SV_PERSON_CONTACT.Where
                            (o => o.UUID == svRecord.OI_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                    pawAddress = db.B_SV_ADDRESS.Where
                                (o => o.UUID == pawPersonContact.SV_ADDRESS_ID).FirstOrDefault();

                    oiAddress = db.B_SV_ADDRESS.Where
                                (o => o.UUID == oiPersonContact.SV_ADDRESS_ID).FirstOrDefault();

                    itemList = db.B_SV_RECORD_ITEM
                        .Where(o => o.SV_RECORD_ID == svRecordUUID).ToList();

                    ap = db.B_SV_APPOINTED_PROFESSIONAL.Where
                            (o => o.SV_RECORD_ID == svRecord.UUID && o.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_AP)
                            .FirstOrDefault();

                    rse = db.B_SV_APPOINTED_PROFESSIONAL.Where
                            (o => o.SV_RECORD_ID == svRecord.UUID && o.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RSE)
                            .FirstOrDefault();

                    rge = db.B_SV_APPOINTED_PROFESSIONAL.Where
                            (o => o.SV_RECORD_ID == svRecord.UUID && o.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RGE)
                            .FirstOrDefault();

                    prc = db.B_SV_APPOINTED_PROFESSIONAL.Where
                            (o => o.SV_RECORD_ID == svRecord.UUID && o.IDENTIFY_FLAG == SignboardConstant.PRC_CODE)
                            .FirstOrDefault();

                    //rfidList
                }
                //for (int i = 0; i < rfidList.size(); i++) {
                //SvRfid rfid = rfidList.get(i);
                //rfid.setRfid(dataEntryObject.getRfid());
                //rfid.setRfidDate(DateUtil
                //		.getDisplayDateToDBDate(dataEntryObject.getRfidDate()));
                //setTracking(request, rfid);
                //   }

                //   if (rfidList.size() == 0) {
                //    SvRfid rfid = new SvRfid();
                //    rfid.setRfid(dataEntryObject.getRfid());
                //    rfid.setRfidDate(DateUtil
                //		    .getDisplayDateToDBDate(dataEntryObject.getRfidDate()));
                //    rfid.setSvRecord(svRecord);
                //    setTracking(request, rfid);
                //    rfidList.add(rfid);
                //   }

                //TO PO SPO
                svRecord.TO_USER_ID = model.TO_Handling_Officer;
                svRecord.PO_USER_ID = model.TO_Handling_Officer;
                svRecord.SPO_USER_ID = model.TO_Handling_Officer;
                //string tuuid = svRecord.TO_USER_ID;
                //while (scuTeamManagerService.getParents(tuuid) != null && !scuTeamManagerService.getParents(tuuid).Equals(SignboardConstant.S_USER_ACCOUNT_RANK_PO))
                //{
                //    tuuid = scuTeamManagerService.getParents(tuuid).UUID;
                //}
                //svRecord.PO_USER_ID = scuTeamManagerService.getParents(tuuid).SYS_POST_ID;
                //while (scuTeamManagerService.getParents(tuuid) != null && !scuTeamManagerService.getParents(tuuid).Equals(SignboardConstant.S_USER_ACCOUNT_RANK_PO))
                //{
                //    tuuid = scuTeamManagerService.getParents(tuuid).UUID;
                //}
                //svRecord.SPO_USER_ID = scuTeamManagerService.getParents(tuuid).SYS_POST_ID;

                //svRecord.TO_USER_ID = scuTeamManagerService.getWFUser(SignboardConstant.S_USER_ACCOUNT_RANK_TO).SYS_POST_ID;
                //svRecord.PO_USER_ID = scuTeamManagerService.getWFUser(SignboardConstant.S_USER_ACCOUNT_RANK_PO).SYS_POST_ID;
                //svRecord.SPO_USER_ID = scuTeamManagerService.getWFUser(SignboardConstant.S_USER_ACCOUNT_RANK_SPO).SYS_POST_ID;

                //UR
                svRecord.AREA_CODE = model.SvRecord.AREA_CODE;
                svRecord.RECOMMENDATION = model.SvRecord.RECOMMENDATION;
                svRecord.NO_OF_SIGNBOARD_VALIDATED = model.validated;
                svRecord.NO_OF_SIGNBOARD_INVOLVED = model.involved;
                svRecord.LSO_VS_OPERATION_YEAR = model.SvRecord.LSO_VS_OPERATION_YEAR;
                svRecord.PREVIOUS_SUBMISSION_NUMBER = model.SvRecord.PREVIOUS_SUBMISSION_NUMBER;
                svRecord.NO_OF_SUBMISSION_ERECTION = model.SvRecord.NO_OF_SUBMISSION_ERECTION;
                svRecord.NO_OF_SUBMISSION_REMOVAL = model.SvRecord.NO_OF_SUBMISSION_REMOVAL;

                //Validation Information
                svRecord.VALIDITY_AP = model.SvRecord.VALIDITY_AP;
                svRecord.SIGNATURE_AP = model.SvRecord.SIGNATURE_AP;
                svRecord.ITEM_STATED = model.SvRecord.ITEM_STATED;
                svRecord.VALIDITY_PRC = model.SvRecord.VALIDITY_PRC;
                svRecord.SIGNATURE_AS = model.SvRecord.SIGNATURE_AS;
                svRecord.INFO_SIGNBOARD_OWNER_PROVIDED = model.SvRecord.INFO_SIGNBOARD_OWNER_PROVIDED;
                svRecord.OTHER_IRREGULARITIES = model.SvRecord.OTHER_IRREGULARITIES;
                svRecord.RECOMMENDATION = model.SvRecord.RECOMMENDATION;

                //Data Entry
                svRecord.RECEIVED_DATE = model.ReceivedDate;
                svRecord.REFERENCE_NO = SvSubmission.REFERENCE_NO;
                svRecord.LANGUAGE_CODE = model.SvRecord.LANGUAGE_CODE;
                svRecord.WITH_ALTERATION = model.WithAlteration.ToString();
                svRecord.INSPECTION_DATE = model.SvRecord.INSPECTION_DATE;
                svRecord.PROPOSED_ALTERATION_COMM_DATE = model.SvRecord.PROPOSED_ALTERATION_COMM_DATE;
                svRecord.ACTUAL_ALTERATION_COMM_DATE = model.SvRecord.ACTUAL_ALTERATION_COMM_DATE;
                svRecord.ACTUAL_ALTERATION_COMP_DATE = model.SvRecord.ACTUAL_ALTERATION_COMP_DATE;
                svRecord.VALIDATION_EXPIRY_DATE = model.ValidationExpiryDate;
                svRecord.SIGNBOARD_REMOVAL = model.SignboardRemoval.ToString();
                svRecord.SIGNBOARD_REMOVAL_DISCOV_DATE = model.SvRecord.SIGNBOARD_REMOVAL_DISCOV_DATE;

                // form Screening Check
                svRecord.S_CHK_VS_NO = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_VS_NO;
                svRecord.S_CHK_INSP_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_INSP_DATE;
                svRecord.S_CHK_WORK_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_WORK_DATE;
                svRecord.S_CHK_SIGNBOARD = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIGNBOARD;
                svRecord.S_CHK_SIG = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIG;
                svRecord.S_CHK_SIG_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIG_DATE;
                svRecord.S_CHK_MW_ITEM_NO = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_MW_ITEM_NO;
                svRecord.S_CHK_SUPPORT_DOC = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SUPPORT_DOC;
                svRecord.S_CHK_SBO_PWA_AP = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SBO_PWA_AP;
                svRecord.S_CHK_OTHERS = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_OTHERS;

                // Preliminary Check
                svRecord.P_CHK_APP_AP_MW_ITEM = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_APP_AP_MW_ITEM;
                svRecord.P_CHK_VAL_AP = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_AP;
                svRecord.P_CHK_SIG_AP = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_AP;
                svRecord.P_CHK_VAL_RSE = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_RSE;
                svRecord.P_CHK_SIG_RSE = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_RSE;
                svRecord.P_CHK_VAL_RI = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_RI;
                svRecord.P_CHK_SIG_RI = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_RI;
                svRecord.P_CHK_VAL_PRC = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_PRC;
                svRecord.P_CHK_SIG_AS = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_AS;
                svRecord.P_CHK_CAP_AS_MW_ITEM = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_CAP_AS_MW_ITEM;

                if (model.ExportLetterFlag.Equals("Y"))
                {
                    DateTime issueDate = new DateTime();
                    svRecord.ACK_LETTERISS_DATE = issueDate;
                }
                else
                {
                    svRecord.ACK_LETTERISS_DATE = model.SvRecord.ACK_LETTERISS_DATE;
                }
                //svRecord.SUserAccountByToUserId
                if (!updateApplication)
                {
                    if (isSaveMode)
                    {
                        svRecord.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;
                    }
                    else
                    {
                        svRecord.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_COMPLETED;
                        svRecord.WF_STATUS = SignboardConstant.WF_MAP_ASSIGING;
                        if (isSystemAssign)
                        {
                            svRecord.IS_FOR_SPO_ASSIGNMENT = SignboardConstant.DB_UNCHECKED;
                        }
                        else
                        {
                            svRecord.IS_FOR_SPO_ASSIGNMENT = SignboardConstant.DB_CHECKED;
                        }
                    }
                }

                //svSignboard
                svSignboard.LOCATION_OF_SIGNBOARD = model.SvSignboard.LOCATION_OF_SIGNBOARD;
                svSignboard.RVD_NO = model.SvSignboard.RVD_NO;
                svSignboard.FACADE = model.SvSignboard.FACADE;
                svSignboard.TYPE = model.SvSignboard.TYPE;
                svSignboard.BTM_FLOOR = model.SvSignboard.BTM_FLOOR;
                svSignboard.TOP_FLOOR = model.SvSignboard.TOP_FLOOR;
                svSignboard.A_M2 = model.SvSignboard.A_M2;
                svSignboard.P_M = model.SvSignboard.P_M;
                svSignboard.H_M = model.SvSignboard.H_M;
                svSignboard.H2_M = model.SvSignboard.H2_M;
                svSignboard.LED = model.SvSignboard.LED;
                svSignboard.BUILDING_PORTION = model.SvSignboard.BUILDING_PORTION;
                svSignboard.STATUS = model.SvSignboard.STATUS;
                svSignboard.DESCRIPTION = model.SvSignboard.DESCRIPTION;
                svSignboard.S24_ORDER_NO = model.SvSignboard.S24_ORDER_NO;
                svSignboard.S24_ORDER_TYPE = model.SvSignboard.S24_ORDER_TYPE;

                //svAddress
                svSignboardAddress.STREET = model.SvSignboard.B_SV_ADDRESS.STREET;
                svSignboardAddress.STREET_NO = model.SvSignboard.B_SV_ADDRESS.STREET_NO;
                svSignboardAddress.BUILDINGNAME = model.SvSignboard.B_SV_ADDRESS.BUILDINGNAME;
                svSignboardAddress.FLOOR = model.SvSignboard.B_SV_ADDRESS.FLOOR;
                svSignboardAddress.FLAT = model.SvSignboard.B_SV_ADDRESS.FLAT;
                svSignboardAddress.DISTRICT = model.SvSignboard.B_SV_ADDRESS.DISTRICT;
                svSignboardAddress.REGION = model.SvSignboard.B_SV_ADDRESS.REGION;
                svSignboardAddress.BCIS_BLOCK_ID = model.SvSignboard.B_SV_ADDRESS.BCIS_BLOCK_ID;
                svSignboardAddress.BLOCK = model.SvSignboard.B_SV_ADDRESS.BLOCK;
                svSignboardAddress.FILE_REFERENCE_NO = model.SvSignboard.B_SV_ADDRESS.FILE_REFERENCE_NO;
                svSignboardAddress.RV_BLOCK_ID = model.SvSignboard.B_SV_ADDRESS.RV_BLOCK_ID;
                svSignboardAddress.BCIS_DISTRICT = model.SvSignboard.B_SV_ADDRESS.BCIS_DISTRICT;

                //OWNER
                svSignboardPersonContact.NAME_CHINESE = model.Owner.NAME_CHINESE;
                svSignboardPersonContact.NAME_ENGLISH = model.Owner.NAME_ENGLISH;
                svSignboardPersonContact.EMAIL = model.Owner.EMAIL;
                svSignboardPersonContact.CONTACT_NO = model.Owner.CONTACT_NO;
                svSignboardPersonContact.FAX_NO = model.Owner.ID_TYPE;
                svSignboardPersonContact.ID_TYPE = model.Owner.ID_TYPE;
                svSignboardPersonContact.ID_NUMBER = model.Owner.ID_NUMBER;
                svSignboardPersonContact.ID_ISSUE_COUNTRY = model.Owner.ID_ISSUE_COUNTRY;
                svSignboardPersonContact.CONTACT_PERSON_TITLE = model.Owner.CONTACT_PERSON_TITLE;
                svSignboardPersonContact.FIRST_NAME = model.Owner.FIRST_NAME;
                svSignboardPersonContact.LAST_NAME = model.Owner.LAST_NAME;
                svSignboardPersonContact.MOBILE = model.Owner.MOBILE;
                svSignboardPersonContact.SIGNATURE_DATE = model.Owner.SIGNATURE_DATE;
                svSignboardPersonContact.PRC_NAME = model.Owner.PRC_NAME;
                svSignboardPersonContact.PRC_CONTACT_NO = model.Owner.PRC_CONTACT_NO;
                svSignboardPersonContact.PBP_NAME = model.Owner.PBP_NAME;
                svSignboardPersonContact.PBP_CONTACT_NO = model.Owner.PBP_CONTACT_NO;
                svSignboardPersonContact.PAW_SAME_AS = model.Owner.PAW_SAME_AS;
                svSignboardPersonContactAddress.BLOCK = model.OwnerAddress.BLOCK;
                svSignboardPersonContactAddress.STREET = model.OwnerAddress.STREET;
                svSignboardPersonContactAddress.STREET_NO = model.OwnerAddress.STREET;
                svSignboardPersonContactAddress.BUILDINGNAME = model.OwnerAddress.BUILDINGNAME;
                svSignboardPersonContactAddress.FLOOR = model.OwnerAddress.FLOOR;
                svSignboardPersonContactAddress.FLAT = model.OwnerAddress.FLAT;
                svSignboardPersonContactAddress.DISTRICT = model.OwnerAddress.DISTRICT;
                svSignboardPersonContactAddress.REGION = model.OwnerAddress.REGION;

                //PAW
                pawPersonContact.NAME_CHINESE = model.Paw.NAME_CHINESE;
                pawPersonContact.NAME_ENGLISH = model.Paw.NAME_ENGLISH;
                pawPersonContact.EMAIL = model.Paw.EMAIL;
                pawPersonContact.CONTACT_NO = model.Paw.CONTACT_NO;
                pawPersonContact.FAX_NO = model.Paw.ID_TYPE;
                pawPersonContact.ID_TYPE = model.Paw.ID_TYPE;
                pawPersonContact.ID_NUMBER = model.Paw.ID_NUMBER;
                pawPersonContact.ID_ISSUE_COUNTRY = model.Paw.ID_ISSUE_COUNTRY;
                pawPersonContact.CONTACT_PERSON_TITLE = model.Paw.CONTACT_PERSON_TITLE;
                pawPersonContact.FIRST_NAME = model.Paw.FIRST_NAME;
                pawPersonContact.LAST_NAME = model.Paw.LAST_NAME;
                pawPersonContact.MOBILE = model.Paw.MOBILE;
                pawPersonContact.SIGNATURE_DATE = model.Paw.SIGNATURE_DATE;
                pawPersonContact.PRC_NAME = model.Paw.PRC_NAME;
                pawPersonContact.PRC_CONTACT_NO = model.Paw.PRC_CONTACT_NO;
                pawPersonContact.PBP_NAME = model.Paw.PBP_NAME;
                pawPersonContact.PBP_CONTACT_NO = model.Paw.PBP_CONTACT_NO;
                pawPersonContact.PAW_SAME_AS = model.Paw.PAW_SAME_AS;
                pawAddress.BLOCK = model.Paw.B_SV_ADDRESS.BLOCK;
                pawAddress.STREET = model.Paw.B_SV_ADDRESS.STREET;
                pawAddress.STREET_NO = model.Paw.B_SV_ADDRESS.STREET;
                pawAddress.BUILDINGNAME = model.Paw.B_SV_ADDRESS.BUILDINGNAME;
                pawAddress.FLOOR = model.Paw.B_SV_ADDRESS.FLOOR;
                pawAddress.FLAT = model.Paw.B_SV_ADDRESS.FLAT;
                pawAddress.DISTRICT = model.Paw.B_SV_ADDRESS.DISTRICT;
                pawAddress.REGION = model.Paw.B_SV_ADDRESS.REGION;
                //OI
                oiPersonContact.NAME_CHINESE = model.Oi.NAME_CHINESE;
                oiPersonContact.NAME_ENGLISH = model.Oi.NAME_ENGLISH;
                oiPersonContact.EMAIL = model.Oi.EMAIL;
                oiPersonContact.CONTACT_NO = model.Oi.CONTACT_NO;
                oiPersonContact.FAX_NO = model.Oi.ID_TYPE;
                oiPersonContact.ID_TYPE = model.Oi.ID_TYPE;
                oiPersonContact.ID_NUMBER = model.Oi.ID_NUMBER;
                oiPersonContact.ID_ISSUE_COUNTRY = model.Oi.ID_ISSUE_COUNTRY;
                oiPersonContact.CONTACT_PERSON_TITLE = model.Oi.CONTACT_PERSON_TITLE;
                oiPersonContact.FIRST_NAME = model.Oi.FIRST_NAME;
                oiPersonContact.LAST_NAME = model.Oi.LAST_NAME;
                oiPersonContact.MOBILE = model.Oi.MOBILE;
                oiPersonContact.SIGNATURE_DATE = model.Oi.SIGNATURE_DATE;
                oiPersonContact.PRC_NAME = model.Oi.PRC_NAME;
                oiPersonContact.PRC_CONTACT_NO = model.Oi.PRC_CONTACT_NO;
                oiPersonContact.PBP_NAME = model.Oi.PBP_NAME;
                oiPersonContact.PBP_CONTACT_NO = model.Oi.PBP_CONTACT_NO;
                oiPersonContact.PAW_SAME_AS = model.Oi.PAW_SAME_AS;
                oiAddress.BLOCK = model.Oi.B_SV_ADDRESS.BLOCK;
                oiAddress.STREET = model.Oi.B_SV_ADDRESS.STREET;
                oiAddress.STREET_NO = model.Oi.B_SV_ADDRESS.STREET;
                oiAddress.BUILDINGNAME = model.Oi.B_SV_ADDRESS.BUILDINGNAME;
                oiAddress.FLOOR = model.Oi.B_SV_ADDRESS.FLOOR;
                oiAddress.FLAT = model.Oi.B_SV_ADDRESS.FLAT;
                oiAddress.DISTRICT = model.Oi.B_SV_ADDRESS.DISTRICT;
                oiAddress.REGION = model.Oi.B_SV_ADDRESS.REGION;

                List<B_SV_APPOINTED_PROFESSIONAL> svAppointedProfessionalList = new List<B_SV_APPOINTED_PROFESSIONAL>();
                //ap
                ap.CERTIFICATION_NO = model.Ap.CERTIFICATION_NO;
                ap.CHINESE_NAME = model.Ap.CHINESE_NAME;
                ap.ENGLISH_NAME = model.Ap.ENGLISH_NAME;
                ap.AS_CHINESE_NAME = model.Ap.AS_CHINESE_NAME;
                ap.AS_ENGLISH_NAME = model.Ap.AS_ENGLISH_NAME;
                ap.IDENTIFY_FLAG = model.Ap.IDENTIFY_FLAG;
                ap.STATUS_CODE = model.Ap.STATUS_CODE;
                ap.EXPIRY_DATE = model.Ap.EXPIRY_DATE;
                ap.SIGNATURE_DATE = model.Ap.SIGNATURE_DATE;
                ap.CONTACT_NO = model.Ap.CONTACT_NO;
                ap.FAX_NO = model.Ap.FAX_NO;
                ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;
                svAppointedProfessionalList.Add(ap);
                //prc
                prc.CERTIFICATION_NO = model.Prc.CERTIFICATION_NO;
                prc.CHINESE_NAME = model.Prc.CHINESE_NAME;
                prc.ENGLISH_NAME = model.Prc.ENGLISH_NAME;
                prc.AS_CHINESE_NAME = model.Prc.AS_CHINESE_NAME;
                prc.AS_ENGLISH_NAME = model.Prc.AS_ENGLISH_NAME;
                prc.IDENTIFY_FLAG = model.Prc.IDENTIFY_FLAG;
                prc.STATUS_CODE = model.Prc.STATUS_CODE;
                prc.EXPIRY_DATE = model.Prc.EXPIRY_DATE;
                prc.SIGNATURE_DATE = model.Prc.SIGNATURE_DATE;
                prc.CONTACT_NO = model.Prc.CONTACT_NO;
                prc.FAX_NO = model.Prc.FAX_NO;
                prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;
                svAppointedProfessionalList.Add(prc);

                //RSE
                rse.CERTIFICATION_NO = model.Rse.CERTIFICATION_NO;
                rse.CHINESE_NAME = model.Rse.CHINESE_NAME;
                rse.ENGLISH_NAME = model.Rse.ENGLISH_NAME;
                rse.AS_CHINESE_NAME = model.Rse.AS_CHINESE_NAME;
                rse.AS_ENGLISH_NAME = model.Rse.AS_ENGLISH_NAME;
                rse.IDENTIFY_FLAG = model.Rse.IDENTIFY_FLAG;
                rse.STATUS_CODE = model.Rse.STATUS_CODE;
                rse.EXPIRY_DATE = model.Rse.EXPIRY_DATE;
                rse.SIGNATURE_DATE = model.Rse.SIGNATURE_DATE;
                rse.CONTACT_NO = model.Rse.CONTACT_NO;
                rse.FAX_NO = model.Rse.FAX_NO;
                rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                svAppointedProfessionalList.Add(rse);
                //RGE
                //if (model.FormCode == "SC03" || model.FormCode == "SC02C" || model.FormCode == "SC02")
                //{
                if (rge == null) { rge = new B_SV_APPOINTED_PROFESSIONAL(); }
                rge.CERTIFICATION_NO = model.Rge.CERTIFICATION_NO;/* != null ? model.Rge.CERTIFICATION_NO : " ";*/
                rge.CHINESE_NAME = model.Rge.CHINESE_NAME;
                rge.ENGLISH_NAME = model.Rge.ENGLISH_NAME;
                rge.AS_CHINESE_NAME = model.Rge.AS_CHINESE_NAME;
                rge.AS_ENGLISH_NAME = model.Rge.AS_ENGLISH_NAME;
                rge.IDENTIFY_FLAG = model.Rge.IDENTIFY_FLAG;
                rge.STATUS_CODE = model.Rge.STATUS_CODE;
                rge.EXPIRY_DATE = model.Rge.EXPIRY_DATE;
                rge.SIGNATURE_DATE = model.Rge.SIGNATURE_DATE;
                rge.CONTACT_NO = model.Rge.CONTACT_NO;
                rge.FAX_NO = model.Rge.FAX_NO;
                rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;
                svAppointedProfessionalList.Add(rge);
                //}
                //List<B_SV_RECORD_ITEM> recordItem = model.B_SV_RECORD_ITEMs;
                itemList = model.B_SV_RECORD_ITEMs; ;
                //B_SV_RECORD_ITEM svRecordItem = new B_SV_RECORD_ITEM();
                for (int i = 0; i < itemList.Count(); i++)
                {
                    B_SV_RECORD_ITEM svRecordItem = itemList[i];

                    if (string.IsNullOrEmpty(svRecordItem.MW_ITEM_CODE))
                    {
                        //    Remove the item from the list if it is a blank item
                        var fromRecordItemTable = db.B_SV_RECORD_ITEM.Find(itemList[i].UUID);
                        db.B_SV_RECORD_ITEM.Remove(itemList[i]);
                        continue;
                    }
                    B_SV_RECORD_ITEM svRecordItemDB = svRecordItemDAOService.getSvRecordItemByUuid(svRecordItem.UUID);

                    if (svRecordItemDB != null)
                    {
                        svRecordItem.CREATED_BY = svRecordItemDB.CREATED_BY;
                        svRecordItem.CREATED_DATE = svRecordItemDB.CREATED_DATE;
                    }
                    svRecordItem.ORDERING = new decimal(i);
                    svRecordItem.CLASS_CODE = getClassCode(svRecordItem.MW_ITEM_CODE);
                    //itemList.Add(svRecordItem);
                }


                for (int i = 0; i < model.B_SV_RECORD_VALIDATION_ITEMs.Count(); i++)
                {
                    B_SV_RECORD_VALIDATION_ITEM each = new B_SV_RECORD_VALIDATION_ITEM();
                    each.VALIDATION_ITEM = model.B_SV_RECORD_VALIDATION_ITEMs[i].VALIDATION_ITEM;
                    each.DESCRIPTION = model.B_SV_RECORD_VALIDATION_ITEMs[i].DESCRIPTION;
                    //System.out.println("M ->"+dataEntryObject.getSelectedVaDescription().get(i));	
                    if (string.IsNullOrWhiteSpace(each.VALIDATION_ITEM)) { continue; }
                    each.SV_RECORD_ID = svRecord.UUID;
                    each.CORRESPONDING_ITEM = model.B_SV_RECORD_VALIDATION_ITEMs[i].CORRESPONDING_ITEM;
                    each.ORDERING = new decimal(i);
                    vaItemList.Add(each);
                }


                B_SV_VALIDATION svValidation = null;
                B_SV_PHOTO_LIBRARY svPhotoLibrary = null;

                if (!updateApplication)
                {
                    if (!isSaveMode)
                    {
                        svValidation = new B_SV_VALIDATION();
                        svValidation.SV_RECORD_ID = svRecord.UUID;
                        svValidation.VALIDATION_STATUS = SignboardConstant.SV_VALIDATION_STATUS_CODE_SPO_ASSIGNMENT;

                        //svPhotoLibrary = new B_SV_PHOTO_LIBRARY();

                        //if (svSignboardAddress != null)
                        //{
                        //    svPhotoLibrary.FILE_PATH = "dataEntryForm.getPhotoURL()"/* .setUrl(StringUtil.getDisplay(dataEntryForm.getPhotoURL())*/
                        //            + svSignboardAddress.BCIS_BLOCK_ID;
                        //    svPhotoLibrary.DESCRIPTION= svSignboardAddress.FULL_ADDRESS +"_"+ svSignboardAddress.BCIS_BLOCK_ID;
                        //    //setDescription(StringUtil.getDisplay(svSignboardAddress.getFullAddress()) + "_" + StringUtil.getDisplay(svSignboardAddress.getBcisBlockId()));
                        //}
                        //svPhotoLibrary.B_SV_SIGNBOARD= svSignboard; /*.setSvSignboard(svSignboard);*/
                    }
                }

                commonService.setRVDDataToSvAddress(svSignboardAddress);
                commonService.setRVDDataToSvAddress(svSignboardPersonContactAddress);
                commonService.setRVDDataToSvAddress(pawAddress);
                commonService.setRVDDataToSvAddress(oiAddress);
                svRVSignboardAddress = commonService.getSvRVSignboardAddress(svSignboardAddress);
                svRVSignboardPersonContactAddress = commonService.getSvRVSignboardAddress(svSignboardPersonContactAddress);
                pawRVAddress = commonService.getSvRVSignboardAddress(pawAddress);
                oiRVAddress = commonService.getSvRVSignboardAddress(oiAddress);

                string errMsg = saveDataEntryToDB(updateApplication,
                    isSaveMode, SvSubmission, svRecord, svSignboard,
                    svPhotoLibrary, svSignboardAddress, svRVSignboardAddress,
                    svSignboardPersonContact, svSignboardPersonContactAddress,
                    svRVSignboardPersonContactAddress, pawPersonContact,
                    oiPersonContact, pawAddress, oiAddress, pawRVAddress,
                    oiRVAddress, svValidation, svAppointedProfessionalList,
                    itemList, vaItemList,
                    //rfidList,
                    model.FormMode);
                if (string.IsNullOrEmpty(errMsg))
                {
                    model.Saved = true;
                }
                else
                {
                    model.Saved = false;
                }
                model.SvRecord = svRecord;

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                // return model;

            }

        }

        public string saveDataEntryToDB(
            bool isUpdate, bool isSaveMode, B_SV_SUBMISSION svSubmission,
            B_SV_RECORD svRecord, B_SV_SIGNBOARD svSignboard,
            B_SV_PHOTO_LIBRARY svPhotoLibrary, B_SV_ADDRESS svSignboardAddress,
            B_SV_RV_ADDRESS svRVSignboardAddress,
            B_SV_PERSON_CONTACT svSignboardPersonContact,
            B_SV_ADDRESS svSignboardPersonContactAddress,
            B_SV_RV_ADDRESS svRVSignboardPersonContactAddress,
            B_SV_PERSON_CONTACT pawPersonContact, B_SV_PERSON_CONTACT oiPersonContact,
            B_SV_ADDRESS pawAddress, B_SV_ADDRESS oiAddress,
            B_SV_RV_ADDRESS pawRVAddress, B_SV_RV_ADDRESS oiRVAddress,
            B_SV_VALIDATION svValidation,
            List<B_SV_APPOINTED_PROFESSIONAL> svAppointedProfessionalList,
            List<B_SV_RECORD_ITEM> svRecordItemList, List<B_SV_RECORD_VALIDATION_ITEM> vaItemList,
            //List<B_SV_RFID> rfidList,
            string formMode)
        {
            WorkFlowManagementService workFlowManagementService = new WorkFlowManagementService();
            SvRecordValidationItemDAOService svRecordValidationItemDAOService = new SvRecordValidationItemDAOService();
            SvRecordItemDAOService svRecordItemDAOService = new SvRecordItemDAOService();
            string result = "";
            //try
            //{
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    if (svSubmission.UUID != null)
                    {
                        var fromSVSData = db.B_SV_SUBMISSION.Find(svSubmission.UUID);
                        db.Entry(fromSVSData).CurrentValues.SetValues(svSubmission);
                        db.SaveChanges();
                    }
                    bool isNew = IsNew(svRecord.UUID);

                    if (isNew)
                    {
                        //create svSignboardPersonContactAddress
                        if (string.IsNullOrEmpty(svSignboardPersonContactAddress.UUID))
                        {
                            db.B_SV_ADDRESS.Add(svSignboardPersonContactAddress);
                            db.SaveChanges();
                        }
                        //create svSignboardAddress
                        if (string.IsNullOrEmpty(svSignboardAddress.UUID))
                        {
                            db.B_SV_ADDRESS.Add(svSignboardAddress);
                            db.SaveChanges();
                        }
                        //create pawAddress
                        if (string.IsNullOrEmpty(pawAddress.UUID))
                        {
                            db.B_SV_ADDRESS.Add(pawAddress);
                            db.SaveChanges();
                        }
                        //create oiAddress
                        if (string.IsNullOrEmpty(oiAddress.UUID))
                        {
                            db.B_SV_ADDRESS.Add(oiAddress);
                            db.SaveChanges();
                        }
                        //create oiPersonContact
                        if (string.IsNullOrEmpty(oiPersonContact.UUID))
                        {
                            oiPersonContact.SV_ADDRESS_ID = oiAddress.UUID;
                            db.B_SV_PERSON_CONTACT.Add(oiPersonContact);
                            db.SaveChanges();
                        }
                        //create svSignboardPersonContact
                        if (string.IsNullOrEmpty(svSignboardPersonContact.UUID))
                        {
                            svSignboardPersonContact.SV_ADDRESS_ID = svSignboardPersonContactAddress.UUID;
                            db.B_SV_PERSON_CONTACT.Add(svSignboardPersonContact);
                            db.SaveChanges();
                        }
                        //create svSignboard
                        if (string.IsNullOrEmpty(svSignboard.UUID))
                        {
                            svSignboard.LOCATION_ADDRESS_ID = svSignboardAddress.UUID;
                            svSignboard.OWNER_ID = svSignboardPersonContact.UUID;
                            db.B_SV_SIGNBOARD.Add(svSignboard);
                            db.SaveChanges();
                        }
                        //create pawPersonContact
                        if (string.IsNullOrEmpty(pawPersonContact.UUID))
                        {
                            pawPersonContact.SV_ADDRESS_ID = pawAddress.UUID;
                            db.B_SV_PERSON_CONTACT.Add(pawPersonContact);
                            db.SaveChanges();
                        }
                        //create B_SV_RECORD
                        if (string.IsNullOrEmpty(svRecord.UUID))
                        {

                            svRecord.SV_SIGNBOARD_ID = svSignboard.UUID;
                            svRecord.PAW_ID = pawPersonContact.UUID;
                            svRecord.OI_ID = oiPersonContact.UUID;
                            svRecord.SV_SUBMISSION_ID = svSubmission.UUID;
                            db.B_SV_RECORD.Add(svRecord);
                            db.SaveChanges();
                        }
                        //create svPhotoLibrary
                        //if (!string.IsNullOrEmpty(svPhotoLibrary.UUID))
                        //{
                        //    svPhotoLibrary.SV_SIGNBOARD_ID = svSignboard.UUID;
                        //    db.B_SV_PHOTO_LIBRARY.Add(svPhotoLibrary);
                        //    db.SaveChanges();
                        //}

                        //create svAppointedProfessionalList
                        for (int i = 0; i < svAppointedProfessionalList.Count(); i++)
                        {
                            svAppointedProfessionalList[i].SV_RECORD_ID = svRecord.UUID;
                            db.B_SV_APPOINTED_PROFESSIONAL.Add(svAppointedProfessionalList[i]);
                            db.SaveChanges();
                        }

                    }
                    else
                    {

                        //svSignboardPersonContactAddress
                        var fromSCAData = db.B_SV_ADDRESS.Find(svSignboardPersonContactAddress.UUID);
                        db.Entry(fromSCAData).CurrentValues.SetValues(svSignboardPersonContactAddress);
                        db.SaveChanges();

                        //svSignboardAddress
                        var fromSVSAData = db.B_SV_ADDRESS.Find(svSignboardAddress.UUID);
                        db.Entry(fromSVSAData).CurrentValues.SetValues(svSignboardAddress);
                        db.SaveChanges();

                        //pawAddress
                        var fromPAWData = db.B_SV_ADDRESS.Find(pawAddress.UUID);
                        db.Entry(fromPAWData).CurrentValues.SetValues(pawAddress);
                        db.SaveChanges();
                        //pawAddress
                        //pawAddress.MODIFIED_DATE = System.DateTime.Now;
                        //pawAddress.MODIFIED_BY = SystemParameterConstant.UserName;
                        //    var fromPAWData = db.B_SV_ADDRESS.Find(pawAddress.UUID);
                        //    db.Entry(fromPAWData).CurrentValues.SetValues(pawAddress);

                        //oiAddress
                        var fromOiData = db.B_SV_ADDRESS.Find(oiAddress.UUID);
                        db.Entry(fromOiData).CurrentValues.SetValues(oiAddress);
                        db.SaveChanges();

                        //svSignboardPersonContact
                        var fromSPCData = db.B_SV_PERSON_CONTACT.Find(svSignboardPersonContact.UUID);
                        db.Entry(fromSPCData).CurrentValues.SetValues(svSignboardPersonContact);
                        db.SaveChanges();

                        //pawPersonContact
                        var fromPPCData = db.B_SV_PERSON_CONTACT.Find(pawPersonContact.UUID);
                        db.Entry(fromPPCData).CurrentValues.SetValues(pawPersonContact);
                        db.SaveChanges();

                        //oiPersonContact
                        var fromOiCData = db.B_SV_PERSON_CONTACT.Find(oiPersonContact.UUID);
                        db.Entry(fromOiCData).CurrentValues.SetValues(oiPersonContact);
                        db.SaveChanges();

                        //svSignboard
                        var fromSVSData = db.B_SV_SIGNBOARD.Find(svSignboard.UUID);
                        db.Entry(fromSVSData).CurrentValues.SetValues(svSignboard);
                        db.SaveChanges();

                        //svPhotoLibrary
                        if (svPhotoLibrary != null)
                        {
                            var fromPLData = db.B_SV_PHOTO_LIBRARY.Find(svPhotoLibrary.UUID);
                            db.Entry(fromPLData).CurrentValues.SetValues(svPhotoLibrary);
                            db.SaveChanges();
                        }

                        //svRecord
                        var fromSVRTable = db.B_SV_RECORD.Find(svRecord.UUID);
                        db.Entry(fromSVRTable).CurrentValues.SetValues(svRecord);
                        db.SaveChanges();

                        //svAppointedProfessionalList
                        for (int i = 0; i < svAppointedProfessionalList.Count(); i++)
                        {
                            svAppointedProfessionalList[i].SV_RECORD_ID = svRecord.UUID;
                            var fromSvAppProTabe = db.B_SV_APPOINTED_PROFESSIONAL.Find(svAppointedProfessionalList[i].UUID);
                            if (fromSvAppProTabe == null)
                                db.B_SV_APPOINTED_PROFESSIONAL.Add(svAppointedProfessionalList[i]);
                            else
                                db.Entry(fromSvAppProTabe).CurrentValues.SetValues(svAppointedProfessionalList[i]);
                            db.SaveChanges();
                        }
                    }
                    //if (rfidList.size() > 0)
                    //{
                    //    for (int i = 0; i < rfidList.size(); i++)
                    //    {
                    //        if (rfidList.get(i).getUuid() == null)
                    //        {
                    //            svRfidDAO.save(rfidList.get(i));
                    //        }
                    //        else
                    //        {
                    //            svRfidDAO.merge(rfidList.get(i));
                    //        }
                    //    }
                    //}
                    if (svValidation != null && IsNew(svValidation.UUID)) //&& isNew(svValidation.UUID)
                    {
                        //create svValidation
                        svValidation.SV_RECORD_ID = svRecord.UUID;
                        db.B_SV_VALIDATION.Add(svValidation);
                        db.SaveChanges();
                    }

                    bool passValidation = false;
                    bool editMWitem = false;

                    if (svRecord.STATUS_CODE.Equals(SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_COMPLETED))
                    {
                        if (svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_VALIDATION_TO) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_VALIDATION_PO) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_VALIDATION_SPO) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_VALIDATION_END) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_AUDIT_TO) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_AUDIT_PO) ||
                            svRecord.WF_STATUS.Equals(SignboardConstant.WF_MAP_AUDIT_SPO)
                        )
                        {
                            passValidation = true;
                        }
                    }
                    int a = 0;
                    if (!isUpdate || SignboardConstant.FORM_MODE_UPDATE_SIGNBOARD_APPLICATION.Equals(formMode))
                    {
                        List<B_SV_RECORD_ITEM> itemList = svRecordItemDAOService.findByProperty(svRecord);

                        if (!passValidation)
                        {
                            for (int i = 0; i < itemList.Count(); i++)
                            {
                                var RecordItem = db.B_SV_RECORD_ITEM.Find(itemList[i].UUID);
                                db.B_SV_RECORD_ITEM.Remove(RecordItem);
                                db.SaveChanges();//svRecordItemDAO.delete(itemList.get(i));
                            }
                            for (int i = 0; i < svRecordItemList.Count(); i++, a++)
                            {
                                B_SV_RECORD_ITEM svRecordITem = new B_SV_RECORD_ITEM();
                                svRecordITem.SV_RECORD_ID = svRecord.UUID;
                                svRecordITem.MW_ITEM_CODE = svRecordItemList[i].MW_ITEM_CODE;
                                svRecordITem.CLASS_CODE = svRecordItemList[i].CLASS_CODE;
                                svRecordITem.LOCATION_DESCRIPTION = svRecordItemList[i].LOCATION_DESCRIPTION;
                                svRecordITem.REVISION = svRecordItemList[i].REVISION;
                                db.B_SV_RECORD_ITEM.Add(svRecordITem);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (itemList.Count() != svRecordItemList.Count() && passValidation)
                            {
                                editMWitem = true;
                            }
                            else
                            {
                                for (int i = 0; i < svRecordItemList.Count(); i++)
                                {
                                    svRecordItemList[i].UUID = itemList[i].UUID;
                                    svRecordItemList[i].SV_RECORD_ID = svRecord.UUID;
                                    var fromDB = db.B_SV_RV_ADDRESS.Find(svRecordItemList[i].UUID);
                                    db.Entry(fromDB).CurrentValues.SetValues(svRecordItemList[i]);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < svRecordItemList.Count(); i++)
                        {
                            svRecordItemList[i].SV_RECORD_ID = svRecord.UUID;

                            if (string.IsNullOrEmpty(svRecordItemList[i].UUID))
                            {
                                db.B_SV_RECORD_ITEM.Add(svRecordItemList[i]);
                                db.SaveChanges();
                            }
                            else
                            {
                                var fromDB = db.B_SV_RV_ADDRESS.Find(svRecordItemList[i].UUID);
                                db.Entry(fromDB).CurrentValues.SetValues(svRecordItemList[i]);
                                db.SaveChanges();
                            }
                        }
                    }

                    svRecordValidationItemDAOService.deleteSvRecordValidationItems(svRecord.UUID);

                    for (int i = 0; i < vaItemList.Count(); i++)
                    {
                        B_SV_RECORD_VALIDATION_ITEM valItem = new B_SV_RECORD_VALIDATION_ITEM();
                        valItem.SV_RECORD_ID = svRecord.UUID;
                        valItem.VALIDATION_ITEM = vaItemList[i].VALIDATION_ITEM;
                        valItem.CORRESPONDING_ITEM = vaItemList[i].CORRESPONDING_ITEM;
                        valItem.DESCRIPTION = vaItemList[i].DESCRIPTION;
                        valItem.ORDERING = vaItemList[i].ORDERING;
                        //vaItemList[i].SV_RECORD_ID = svRecord.UUID;
                        db.B_SV_RECORD_VALIDATION_ITEM.Add(valItem);
                        db.SaveChanges();
                    }


                    if (string.IsNullOrEmpty(svRVSignboardAddress.UUID))
                    {
                        db.B_SV_RV_ADDRESS.Add(svRVSignboardAddress);
                        db.SaveChanges();
                    }
                    else
                    {
                        var fromDB = db.B_SV_RV_ADDRESS.Find(svRVSignboardAddress.UUID);
                        db.Entry(fromDB).CurrentValues.SetValues(svRVSignboardAddress);
                        db.SaveChanges();
                    }
                    if (string.IsNullOrEmpty(svRVSignboardPersonContactAddress.UUID))
                    {
                        db.B_SV_RV_ADDRESS.Add(svRVSignboardPersonContactAddress);
                        db.SaveChanges();
                    }
                    else
                    {
                        var fromDB = db.B_SV_RV_ADDRESS.Find(svRVSignboardPersonContactAddress.UUID);
                        db.Entry(fromDB).CurrentValues.SetValues(svRVSignboardPersonContactAddress);
                        db.SaveChanges();
                    }
                    if (string.IsNullOrEmpty(pawRVAddress.UUID))
                    {
                        db.B_SV_RV_ADDRESS.Add(pawRVAddress);
                        db.SaveChanges();
                    }
                    else
                    {
                        var fromDB = db.B_SV_RV_ADDRESS.Find(pawRVAddress.UUID);
                        db.Entry(fromDB).CurrentValues.SetValues(pawRVAddress);
                    }
                    if (string.IsNullOrEmpty(oiRVAddress.UUID))
                    {
                        db.B_SV_RV_ADDRESS.Add(oiRVAddress);
                        db.SaveChanges();
                    }
                    else
                    {
                        var fromDB = db.B_SV_RV_ADDRESS.Find(oiRVAddress.UUID);
                        db.Entry(fromDB).CurrentValues.SetValues(oiRVAddress);
                        db.SaveChanges();
                    }

                    //WF
                    if (!isUpdate)
                    {
                        if (!isSaveMode)
                        {
                            workFlowManagementService.assignJobForValidation(SignboardConstant.WF_TYPE_VALIDATION, svRecord);
                        }
                    }

                    if (editMWitem)
                    {
                        result = "Validation already existed. Can not edit or delete the Minor work Item.";
                    }
                    else
                    {
                        result = "";
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
            }
            //}
            //catch(Exception ex)
            //{
            //  transaction.Rollback();
            //  Console.WriteLine("Error :" + ex.Message);
            //  return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            //}
            return result;
        }

        public DataEntryDisplayModel copyFromBaseDataEntry(DataEntryDisplayModel model)
        {
            SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
            SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();
            SvRecordItemDAOService svRecordItemDAOService = new SvRecordItemDAOService();
            SvRecordValidationItemDAOService svRecordValidationItemDAOService = new SvRecordValidationItemDAOService();
            SvAppointedProfessionalDAOService svAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();

            string submssionUUID = model.SvSubmission.UUID;

            B_SV_SUBMISSION SvSubmission = svSubmissionDAOService.getSVRecordBySvSubmissionUUID(submssionUUID);
            B_SV_RECORD svRecord = svRecordDAOService.getSVRecordBySvSubmissionUUID(submssionUUID);

            B_SV_SIGNBOARD svSignboard = new B_SV_SIGNBOARD();
            B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
            B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
            List<B_SV_RECORD_ITEM> itemList = new List<B_SV_RECORD_ITEM>();

            B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
            B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
            B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
            B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();

            List<B_SV_RECORD_VALIDATION_ITEM> svRecordValidationItems = new List<B_SV_RECORD_VALIDATION_ITEM>();

            string svRecordUUID = svRecord.UUID;
            model.FormCode = SvSubmission.FORM_CODE;

            if (CommonUtil.isCompletedForm(model.FormCode))
            {
                string copyfromFromCode = "";
                if (SignboardConstant.FORM_CODE_SC01C.Equals(model.FormCode))
                {
                    copyfromFromCode = SignboardConstant.FORM_CODE_SC01;
                }
                if (SignboardConstant.FORM_CODE_SC02C.Equals(model.FormCode))
                {
                    copyfromFromCode = SignboardConstant.FORM_CODE_SC02;
                }
                svRecord = svRecordDAOService.getLatestSVRecordByRefNo(SvSubmission.REFERENCE_NO, copyfromFromCode);

                svSignboard.UUID = svRecord.SV_SIGNBOARD_ID;
                svRecord.RECEIVED_DATE = SvSubmission.SCU_RECEIVED_DATE;
                B_S_PARAMETER sParameter = new B_S_PARAMETER();
                sParameter.VALIDATION_EXPIRY_YEAR_NUM = (from spt in db.B_S_PARAMETER
                                                         select spt.VALIDATION_EXPIRY_YEAR_NUM).FirstOrDefault();

                int validationExpiryYearNum = Decimal.ToInt32(sParameter.VALIDATION_EXPIRY_YEAR_NUM ?? 0);
                DateTime? recDate = SvSubmission.SCU_RECEIVED_DATE;
                svRecord.VALIDATION_EXPIRY_DATE = recDate.Value.AddYears(validationExpiryYearNum);

                if (svSignboard == null)
                {
                    svSignboard = new B_SV_SIGNBOARD();
                }
                svSignboard.UUID = null;
                paw.UUID = svRecord.PAW_ID;
                if (paw == null)
                {
                    paw = new B_SV_PERSON_CONTACT();
                }
                paw.UUID = null;
                oi.UUID = svRecord.OI_ID;
                if (oi == null)
                {
                    oi = new B_SV_PERSON_CONTACT();
                }
                oi.UUID = null;

                itemList = svRecordItemDAOService.getSvRecordItemBySVRecord(svRecord.UUID);

                for (int i = 0; i < itemList.Count(); i++)
                {
                    itemList[i].UUID = null;
                }

                svRecordValidationItems =
                    svRecordValidationItemDAOService.getSvRecordValidationItems(svRecordUUID);

                ap = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID,
                        SignboardConstant.PBP_CODE_AP);
                ap.UUID = null;
                rse = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID,
                                            SignboardConstant.PBP_CODE_RSE);
                rse.UUID = null;

                rge = svAppointedProfessionalDAOService
                        .getSvAppointedProfessional(svRecord.UUID,
                                SignboardConstant.PBP_CODE_RGE);
                rge.UUID = null;

                prc = svAppointedProfessionalDAOService
                        .getSvAppointedProfessional(svRecord.UUID,
                                SignboardConstant.PRC_CODE);
                prc.UUID = null;
            }
            return new DataEntryDisplayModel()
            {
                SvSubmission = SvSubmission,
                SvRecord = svRecord,
                SvSignboard = svSignboard,
                Paw = paw,
                Oi = oi,
                ItemList = itemList,
                Ap = ap,
                Rse = rse,
                Rge = rge,
                Prc = prc,
                SvRecordValidationItems = svRecordValidationItems,
                ValidationExpiryDate = svRecord.VALIDATION_EXPIRY_DATE
            };
        }
        public DisplayGrid SearchVDItem(DataEntryDisplayModel model)
        {

            string recordUUID = "0";
            if (model != null && model.SvRecord != null && !string.IsNullOrWhiteSpace(model.SvRecord.UUID))
            {
                recordUUID = model.SvRecord.UUID;
            }
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return new DisplayGrid()
                {
                    Data = db.B_SV_RECORD_VALIDATION_ITEM
                    .Where(o => o.SV_RECORD_ID == recordUUID)
                    .OrderBy(o => o.ORDERING)
                    .ToList()
                    .Select(o =>
                        new Dictionary<string, object>() {
                                { "UUID", o.UUID },
                                { "VALIDATION_ITEM", o.VALIDATION_ITEM },
                                { "CORRESPONDING_ITEM", o.CORRESPONDING_ITEM },
                                { "DESCRIPTION", o.DESCRIPTION }
                        }
                    ).ToList(),
                    Rpp = -1
                };
            }
        }
        public DisplayGrid SearchMWItem(DataEntryDisplayModel model)
        {
            string recordUUID = "0";
            if (model != null && model.SvRecord != null && !string.IsNullOrWhiteSpace(model.SvRecord.UUID))
            {
                recordUUID = model.SvRecord.UUID;
            }
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return new DisplayGrid()
                {
                    Data = db.B_SV_RECORD_ITEM
                    .Where(o => o.SV_RECORD_ID == model.SvRecord.UUID)
                    .OrderBy(o => o.ORDERING)
                    .ToList()
                    .Select(o =>
                        new Dictionary<string, object>() {
                            { "UUID", o.UUID },
                            { "MW_ITEM_CODE", o.MW_ITEM_CODE },
                            { "LOCATION_DESCRIPTION", o.LOCATION_DESCRIPTION },
                            { "REVISION", o.REVISION }
                        }
                    ).ToList(),
                    Rpp = -1
                };
            }
        }
    }
}