using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace MWMS2.Services
{
    public class ProcessingDataEntryDAOService : BaseDAOService
    {
        public ProcessingCommonService pc = new ProcessingCommonService();
        private ProcessingSystemValueDAOService _SystemValueDA;
        protected ProcessingSystemValueDAOService SystemValueDA
        {
            get
            {
                return _SystemValueDA ?? (_SystemValueDA = new ProcessingSystemValueDAOService());
            }
        }

        public Fn02MWUR_DeModel GetOutstanding(Fn02MWUR_DeModel model)
        {
            string sSql = ""
                        + "\r\n\t" + "SELECT                                                                "
                        + "\r\n\t" + "COUNT(*)                                                               "
                        + "\r\n\t" + "FROM P_MW_DSN T1                                                      "
                        + "\r\n\t" + "LEFT JOIN P_S_SYSTEM_VALUE T2                                         "
                        + "\r\n\t" + "ON  T1.SCANNED_STATUS_ID = T2.UUID                                    "
                        + "\r\n\t" + "WHERE 1 = 1                                                           "
                        + "\r\n\t" + "AND T2.CODE IN ('FIRST_ENTRY_COMPLETED', 'SECOND_ENTRY', 'WILL_SCAN') ";
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    model.Outstanding = CommonUtil.LoadDbCount(CommonUtil.GetDataReader(conn
                            , sSql
                            , model.QueryParameters)).ToString();
                }
            }
            return model;
        }
        public Fn02MWUR_DeModel Search(Fn02MWUR_DeModel model)
        {
            model.Query = ""
                        + "\r\n\t" + "SELECT                                                                "
                        + "\r\n\t" + "T1.DSN                                                                "
                        + "\r\n\t" + ", T1.UUID                                                             "
                        + "\r\n\t" + ", TO_CHAR(T1.MODIFIED_DATE, 'dd/MM/YYYY') AS DT                       "
                        + "\r\n\t" + ", TO_CHAR(T1.MODIFIED_DATE, 'HH:mm') AS TI                            "
                        + "\r\n\t" + ", T1.RECORD_ID                                                        "
                        + "\r\n\t" + ", T1.FORM_CODE                                                        "
                        + "\r\n\t" + ", T2.DESCRIPTION As STATUS                                            "
                        + "\r\n\t" + "FROM P_MW_DSN T1                                                      "
                        + "\r\n\t" + "LEFT JOIN P_S_SYSTEM_VALUE T2                                         "
                        + "\r\n\t" + "ON  T1.SCANNED_STATUS_ID = T2.UUID                                    "
                        + "\r\n\t" + "WHERE 1 = 1                                                           "
                        //+ "\r\n\t" + "AND T2.CODE IN ('FIRST_ENTRY_COMPLETED', 'SECOND_ENTRY', 'WILL_SCAN') "
                        + "\r\n\t" + "AND T2.CODE IN ('SECOND_ENTRY', 'WILL_SCAN') "
                        + "\r\n\t" + "AND T1.DSN Not IN (Select DSN From P_MW_ACK_LETTER Where AUDIT_RELATED != 'Y' )";

            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.SearchDsn))
            {
                whereQ += "\r\n\t" + "AND T1.DSN = :SearchDsn";
                model.QueryParameters.Add("SearchDsn", model.SearchDsn);
            }

            if (!string.IsNullOrWhiteSpace(model.SearchRecordId))
            {
                whereQ += "\r\n\t" + "AND T1.RECORD_ID = :SearchRecordId";
                model.QueryParameters.Add("SearchRecordId", model.SearchRecordId);
            }

            if (!string.IsNullOrWhiteSpace(model.SearchStatus))
            {
                whereQ += "\r\n\t" + "AND T2.CODE = :SearchStatus";
                model.QueryParameters.Add("SearchStatus", model.SearchStatus);
            }

            if (model.SearchReceiveDateFrom != null)
            {
                whereQ += "\r\n\t" + "AND T1.MWU_RECEIVED_DATE >= :SearchReceiveDateFrom";
                model.QueryParameters.Add("SearchReceiveDateFrom", model.SearchReceiveDateFrom.Value);
            }

            if (model.SearchReceiveDateTo != null)
            {
                whereQ += "\r\n\t" + "AND T1.MWU_RECEIVED_DATE <= :SearchReceiveDateTo";
                model.QueryParameters.Add("SearchReceiveDateTo", model.SearchReceiveDateTo.Value.AddDays(1));
            }

            model.QueryWhere = whereQ;
            model.Search();

            return model;
        }

        public string ExportDeScan(Fn02MWUR_DeModel model)
        {
            model.Query = ""
                       + "\r\n\t" + "SELECT                                                                "
                       + "\r\n\t" + "T1.DSN                                                                "
                       + "\r\n\t" + ", T1.UUID                                                             "
                       + "\r\n\t" + ", TO_CHAR(T1.MODIFIED_DATE, 'dd/MM/YYYY') AS DT                       "
                       + "\r\n\t" + ", TO_CHAR(T1.MODIFIED_DATE, 'HH:mm') AS TI                            "
                       + "\r\n\t" + ", T1.RECORD_ID                                                        "
                       + "\r\n\t" + ", T1.FORM_CODE                                                        "
                       + "\r\n\t" + ", T2.DESCRIPTION As STATUS                                            "
                       + "\r\n\t" + "FROM P_MW_DSN T1                                                      "
                       + "\r\n\t" + "LEFT JOIN P_S_SYSTEM_VALUE T2                                         "
                       + "\r\n\t" + "ON  T1.SCANNED_STATUS_ID = T2.UUID                                    "
                       + "\r\n\t" + "WHERE 1 = 1                                                           "
                       + "\r\n\t" + "AND T2.CODE IN ('FIRST_ENTRY_COMPLETED', 'SECOND_ENTRY', 'WILL_SCAN') "
                       + "\r\n\t" + "AND T1.DSN Not IN (Select DSN From P_MW_ACK_LETTER Where AUDIT_RELATED != 'Y' )";

            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.SearchDsn))
            {
                whereQ += "\r\n\t" + "AND T1.DSN = :SearchDsn";
                model.QueryParameters.Add("SearchDsn", model.SearchDsn);
            }

            if (!string.IsNullOrWhiteSpace(model.SearchRecordId))
            {
                whereQ += "\r\n\t" + "AND T1.RECORD_ID = :SearchRecordId";
                model.QueryParameters.Add("SearchRecordId", model.SearchRecordId);
            }

            if (!string.IsNullOrWhiteSpace(model.SearchStatus))
            {
                whereQ += "\r\n\t" + "AND T2.CODE = :SearchStatus";
                model.QueryParameters.Add("SearchStatus", model.SearchStatus);
            }

            if (model.SearchReceiveDateFrom != null)
            {
                whereQ += "\r\n\t" + "AND T1.MWU_RECEIVED_DATE >= :SearchReceiveDateFrom";
                model.QueryParameters.Add("SearchReceiveDateFrom", model.SearchReceiveDateFrom.Value);
            }

            if (model.SearchReceiveDateTo != null)
            {
                whereQ += "\r\n\t" + "AND T1.MWU_RECEIVED_DATE <= :SearchReceiveDateTo";
                model.QueryParameters.Add("SearchReceiveDateTo", model.SearchReceiveDateTo.Value.AddDays(1));
            }

            model.QueryWhere = whereQ;
            return model.Export("Data Entry");
        }

        public Fn02MWUR_DeScanModel SearchDocument(Fn02MWUR_DeScanModel model)
        {
            model.Query = @"SELECT T1.*
                                    FROM   P_MW_SCANNED_DOCUMENT T1
                                    WHERE  T1.DSN_ID = :DSN_ID ";

            model.QueryParameters.Add("DSN_ID", model.P_MW_DSN.UUID);
            model.Search();
            return model;

        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTs(string DSN_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_SCANNED_DOCUMENT.Where(w => w.DSN_ID == DSN_ID).ToList();
            }
        }

        public string GetStatusUUIDByCode(string CODE)
        {
            string sStatusSql = @"SELECT SV.UUID
                                        FROM   P_S_SYSTEM_VALUE SV
                                               JOIN P_S_SYSTEM_TYPE ST
                                                 ON SV.SYSTEM_TYPE_ID = ST.UUID
                                                    AND ST.TYPE = 'DSN_STATUS'
                                        WHERE  SV.CODE = :CODE ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":CODE",CODE)
            };

            return GetObjectData<string>(sStatusSql, oracleParameters).FirstOrDefault();
        }

        public ServiceResult CompleteScan(P_MW_DSN p_MW_DSN, P_MW_RECORD p_MW_RECORD, P_MW_FORM p_MW_FORM, bool isAFC, List<P_MW_RECORD_ITEM> mwRecordItemList, P_MW_ADDRESS p_MW_ADDRESS, List<P_MW_APPOINTED_PROFESSIONAL> mwAppointedProfessionalList)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_EFSS_SUBMISSION_MAP efss = db.P_EFSS_SUBMISSION_MAP.Where(x => x.DSN == p_MW_RECORD.MW_DSN).FirstOrDefault();
                        bool isEfss = (efss == null ? false : true);
                        ProcessingEformDataEntryService eform = new ProcessingEformDataEntryService();

                        //Update DSN Status
                        UpdateDsnStatus(p_MW_DSN, db);

                        //Add P_MW_RECORD
                        p_MW_RECORD = isEfss ? eform.setMwRecord(p_MW_RECORD) : p_MW_RECORD;
                        AddP_MW_RECORD(p_MW_RECORD, db);

                        p_MW_FORM.MW_RECORD_ID = p_MW_RECORD.UUID;
                        //Add P_MW_Form
                        p_MW_FORM = isEfss ? eform.setMwForm(p_MW_RECORD, p_MW_FORM) : p_MW_FORM;
                        AddP_MW_Form(p_MW_FORM, db);

                        // EFSS
                        if (isEfss)
                        {
                            // MW09 form
                            if (ProcessingConstant.FORM_09.Equals(p_MW_RECORD.S_FORM_TYPE_CODE))
                            {
                                List<P_MW_FORM_09> form09 = new List<P_MW_FORM_09>();
                                form09 = eform.createBlankMw09(p_MW_RECORD, form09);
                                form09 = eform.setMW09Form(p_MW_RECORD, form09);
                                foreach (var form in form09)
                                {
                                    form.MW_RECORD_ID = p_MW_RECORD.UUID;
                                    db.P_MW_FORM_09.Add(form);
                                    db.SaveChanges();
                                }
                            }

                            // OI_ID : owners' corporation
                            P_MW_ADDRESS oiAddress = new P_MW_ADDRESS();
                            oiAddress = isEfss ? eform.setOIAddress(p_MW_RECORD, oiAddress) : oiAddress;
                            db.P_MW_ADDRESS.Add(oiAddress);
                            db.SaveChanges();

                            P_MW_PERSON_CONTACT oiPersonContact = new P_MW_PERSON_CONTACT();
                            oiPersonContact = isEfss ? eform.setOIInfo(p_MW_RECORD, oiPersonContact) : oiPersonContact;
                            oiPersonContact.MW_ADDRESS_ID = oiAddress.UUID;
                            db.P_MW_PERSON_CONTACT.Add(oiPersonContact);
                            db.SaveChanges();

                            p_MW_RECORD.OI_ID = oiPersonContact.UUID;
                            db.SaveChanges();

                            // SIGNBOARD_PERFROMER_ID : signboard owner
                            P_MW_ADDRESS signBoardAddress = new P_MW_ADDRESS();
                            signBoardAddress = isEfss ? eform.setSignboardOwnerAddress(p_MW_RECORD, signBoardAddress) : signBoardAddress;
                            db.P_MW_ADDRESS.Add(signBoardAddress);
                            db.SaveChanges();

                            P_MW_PERSON_CONTACT signBoardPersonContact = new P_MW_PERSON_CONTACT();
                            signBoardPersonContact = isEfss ? eform.setSignboardOnwerInfo(p_MW_RECORD, signBoardPersonContact) : signBoardPersonContact;
                            signBoardPersonContact.MW_ADDRESS_ID = signBoardAddress.UUID;
                            db.P_MW_PERSON_CONTACT.Add(signBoardPersonContact);
                            db.SaveChanges();

                            p_MW_RECORD.SIGNBOARD_PERFROMER_ID = signBoardPersonContact.UUID;
                            db.SaveChanges();

                            // OWNER_ID : paw
                            P_MW_ADDRESS ownerAddress = new P_MW_ADDRESS();
                            ownerAddress = isEfss ? eform.setOwnerAddress(p_MW_RECORD, ownerAddress) : ownerAddress;
                            db.P_MW_ADDRESS.Add(ownerAddress);
                            db.SaveChanges();

                            P_MW_PERSON_CONTACT ownerPersonContact = new P_MW_PERSON_CONTACT();
                            ownerPersonContact = isEfss ? eform.setOwnerInfo(p_MW_RECORD, ownerPersonContact) : ownerPersonContact;
                            ownerPersonContact.MW_ADDRESS_ID = ownerAddress.UUID;
                            db.P_MW_PERSON_CONTACT.Add(ownerPersonContact);
                            db.SaveChanges();

                            p_MW_RECORD.OWNER_ID = ownerPersonContact.UUID;
                            db.SaveChanges();

                            // mw address
                            P_MW_ADDRESS mwAddress = new P_MW_ADDRESS();
                            mwAddress = isEfss ? eform.setMwAddress(p_MW_RECORD, mwAddress) : mwAddress;
                            db.P_MW_ADDRESS.Add(mwAddress);
                            db.SaveChanges();

                            p_MW_RECORD.LOCATION_ADDRESS_ID = mwAddress.UUID;
                            db.SaveChanges();

                            // mw items
                            List<P_MW_RECORD_ITEM> items = new List<P_MW_RECORD_ITEM>();
                            items = eform.saveMwItems(p_MW_RECORD, items);
                            db.P_MW_RECORD_ITEM.AddRange(items);

                            // ap
                            List<P_MW_APPOINTED_PROFESSIONAL> aps = new List<P_MW_APPOINTED_PROFESSIONAL>();
                            aps = eform.createBlankAP(p_MW_RECORD, aps);
                            aps = eform.setAppointedProfessional(p_MW_RECORD, aps);
                            if (aps != null)
                            {
                                foreach (var ap in aps)
                                {
                                    ap.MW_RECORD_ID = p_MW_RECORD.UUID;
                                    db.P_MW_APPOINTED_PROFESSIONAL.Add(ap);
                                    db.SaveChanges();

                                }
                            }

                        }

                        if (isAFC)
                        {

                            if (mwRecordItemList != null)
                            {
                                //Fill record id to item list
                                mwRecordItemList.ForEach(i => i.MW_RECORD_ID = p_MW_RECORD.UUID);
                                //Add P_MW_RECORD_ITEM
                                AddP_MW_RECORD_ITEMs(db, mwRecordItemList);
                            }

                            //Add P_MW_ADDRESS
                            if (p_MW_ADDRESS != null)
                            {
                                AddP_MW_ADDRESS(db, p_MW_ADDRESS);
                                //Save address uuid
                                p_MW_RECORD.LOCATION_ADDRESS_ID = p_MW_ADDRESS.UUID;
                            }

                            //Add P_MW_APPOINTED_PROFESSIONAL
                            if (mwAppointedProfessionalList != null)
                            {
                                mwAppointedProfessionalList.ForEach(i => i.MW_RECORD_ID = p_MW_RECORD.UUID);
                                AddP_MW_APPOINTED_PROFESSIONAL(db, mwAppointedProfessionalList);
                            }


                        }

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        tran.Rollback();
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }


        public int AddP_MW_RECORD_ITEMs(EntitiesMWProcessing db, List<P_MW_RECORD_ITEM> models)
        {
            foreach (var model in models)
            {
                db.P_MW_RECORD_ITEM.Add(model);
            }

            return db.SaveChanges();
        }

        public int AddP_MW_ADDRESS(EntitiesMWProcessing db, P_MW_ADDRESS model)
        {
            db.P_MW_ADDRESS.Add(model);
            return db.SaveChanges();
        }

        public int AddP_MW_APPOINTED_PROFESSIONAL(EntitiesMWProcessing db, List<P_MW_APPOINTED_PROFESSIONAL> models)
        {
            foreach (var model in models)
            {
                db.P_MW_APPOINTED_PROFESSIONAL.Add(model);
            }
            return db.SaveChanges();
        }

        public int DeleteScanDoc(P_MW_SCANNED_DOCUMENT model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_SCANNED_DOCUMENT.Remove(db.P_MW_SCANNED_DOCUMENT.Where(w => w.UUID == model.UUID).FirstOrDefault());
                return db.SaveChanges();
            }
        }

        public void UpdateDsnStatus(P_MW_DSN model, EntitiesMWProcessing db)
        {

            //Get record 
            P_MW_DSN record = db.P_MW_DSN.Where(d => d.UUID == model.UUID).FirstOrDefault();

            record.SSP_SUBMITTED = model.SSP_SUBMITTED;
            record.SCANNED_STATUS_ID = model.SCANNED_STATUS_ID;

            db.SaveChanges();

        }

        public void AddP_MW_RECORD(P_MW_RECORD model, EntitiesMWProcessing db)
        {
            db.P_MW_RECORD.Add(model);
            db.SaveChanges();
        }

        public void AddP_MW_Form(P_MW_FORM model, EntitiesMWProcessing db)
        {
            db.P_MW_FORM.Add(model);
            db.SaveChanges();
        }

        public void AddP_MW_PERSON_CONTACT(P_MW_PERSON_CONTACT model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_PERSON_CONTACT.Add(model);

                db.SaveChanges();
            }
        }

        public void AddP_MW_ADDRESS(P_MW_ADDRESS model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_ADDRESS.Add(model);

                //db.SaveChanges();
            }
        }
        public P_MW_DSN GetP_MW_DSN(string sUUID)
        {
            return GetObjectRecordByUuid<P_MW_DSN>(sUUID);
        }

        public List<P_MW_FORM_09> GetP_MW_FORM_09s(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM_09.Where(d => d.MW_RECORD_ID == MW_RECORD_ID).ToList();
            }
        }

        public P_MW_RECORD GetP_MW_RECORD(string MW_DSN, string REFERENCE_NUMBER)
        {
            string RecordSql = @"SELECT R.*
                                FROM   P_MW_RECORD R
                                WHERE  R.MW_DSN = :MW_DSN
                                       AND R.REFERENCE_NUMBER = :REFERENCE_NUMBER
                                       AND R.STATUS_CODE = 'MW_SECOND_ENTRY' ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_DSN",MW_DSN)
                ,new OracleParameter(":REFERENCE_NUMBER",REFERENCE_NUMBER)
            };

            return GetObjectData<P_MW_RECORD>(RecordSql, oracleParameters).FirstOrDefault();
        }
        public P_MW_REFERENCE_NO GetP_MW_REFERENCE_NO(string REFERENCE_NO)
        {
            string ReferenceSql = @"SELECT RN.*
                                    FROM   P_MW_REFERENCE_NO RN
                                    WHERE  RN.REFERENCE_NO = :REFERENCE_NO ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":REFERENCE_NO",REFERENCE_NO)
            };

            return GetObjectData<P_MW_REFERENCE_NO>(ReferenceSql, oracleParameters).FirstOrDefault();
        }

        public List<P_MW_RECORD_ITEM> GetP_MW_RECORD_ITEM(string RecordID)
        {
            string RecordItemSql = @"SELECT RI.*
                                    FROM   P_MW_RECORD_ITEM RI
                                           JOIN P_MW_RECORD R
                                             ON RI.MW_RECORD_ID = R.UUID
                                    WHERE  R.UUID = :RecordID ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":RecordID",RecordID)
            };

            return GetObjectData<P_MW_RECORD_ITEM>(RecordItemSql, oracleParameters).ToList();
        }

        public P_MW_FORM GetP_MW_FORMByRecordID(string recordID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM.Where(w => w.MW_RECORD_ID == recordID).FirstOrDefault();
            }
        }

        public P_MW_PERSON_CONTACT GetP_MW_PERSON_CONTACT(string sUUID)
        {
            return GetObjectRecordByUuid<P_MW_PERSON_CONTACT>(sUUID);
        }

        public P_MW_ADDRESS GetP_MW_ADDRESS(string sUUID)
        {
            return GetObjectRecordByUuid<P_MW_ADDRESS>(sUUID);
        }

        public List<P_MW_APPOINTED_PROFESSIONAL> GetP_MW_APPOINTED_PROFESSIONAL(string sRecordID)
        {
            string APSql = @"SELECT *
                            FROM   P_MW_APPOINTED_PROFESSIONAL
                            WHERE  MW_RECORD_ID = :RecordID ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":RecordID",sRecordID)
            };
            return GetObjectData<P_MW_APPOINTED_PROFESSIONAL>(APSql, oracleParameters).ToList();
        }

        public void SaveOIInfo(Fn02MWUR_DeFormModel model, Boolean isFinalRecord, EntitiesMWProcessing db)
        {

            if (model.OIAddress != null && model.OIPersonContact != null)
            {
                //Save Address
                if (string.IsNullOrEmpty(model.OIAddress.UUID))
                {
                    db.P_MW_ADDRESS.Add(model.OIAddress);
                }
                else
                {
                    //Get Record
                    P_MW_ADDRESS record = db.P_MW_ADDRESS.Where(d => d.UUID == model.OIAddress.UUID).FirstOrDefault();

                    //Set Data
                    record.STREE_NAME = model.OIAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET = model.OIAddress.DISPLAY_STREET;
                    record.STREET_NO = model.OIAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET_NO = model.OIAddress.DISPLAY_STREET_NO;
                    record.BUILDING_NAME = model.OIAddress.BUILDING_NAME;
                    record.DISPLAY_BUILDINGNAME = model.OIAddress.BUILDING_NAME;
                    record.FLOOR = model.OIAddress.FLOOR;
                    record.DISPLAY_FLOOR = model.OIAddress.FLOOR;
                    record.FLAT = model.OIAddress.FLAT;
                    record.DISPLAY_FLAT = model.OIAddress.FLAT;
                    record.DISTRICT = model.OIAddress.DISTRICT;
                    record.DISPLAY_DISTRICT = model.OIAddress.DISTRICT;
                    record.REGION = model.OIAddress.REGION;
                    record.DISPLAY_REGION = model.OIAddress.REGION;
                    if (ProcessingConstant.LANG_ENGLISH.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                    else if (ProcessingConstant.LANG_CHINESE.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.CHINESE_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                }
                db.SaveChanges();

                //Save Person Contact
                //Set Address UUID
                model.OIPersonContact.MW_ADDRESS_ID = model.OIAddress.UUID;
                if (string.IsNullOrEmpty(model.OIPersonContact.UUID))
                {
                    db.P_MW_PERSON_CONTACT.Add(model.OIPersonContact);
                    db.SaveChanges();

                    //Set Person Contact UUID 
                    //model.P_MW_RECORD.OI_ID = model.OIPersonContact.UUID;
                    model.DictID.Add("OI_ID", model.OIPersonContact.UUID);
                }
                else
                {
                    //Get Record
                    P_MW_PERSON_CONTACT record = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == model.OIPersonContact.UUID).FirstOrDefault();

                    //Set Data
                    record.NAME_ENGLISH = model.OIPersonContact.NAME_ENGLISH;
                    record.EMAIL = model.OIPersonContact.EMAIL;
                    record.FAX_NO = model.OIPersonContact.FAX_NO;
                    record.CONTACT_NO = model.OIPersonContact.CONTACT_NO;

                    //Save MW_ADDRESS_ID
                    record.MW_ADDRESS_ID = model.OIPersonContact.MW_ADDRESS_ID;
                }
                db.SaveChanges();

            }

        }

        private void SaveSignBoardInfo(Fn02MWUR_DeFormModel model, Boolean isFinalRecord, EntitiesMWProcessing db)
        {
            if (model.SignBoardAddress != null && model.SignBoardPersonContact != null)
            {
                //Save Address
                if (string.IsNullOrEmpty(model.SignBoardAddress.UUID))
                {
                    db.P_MW_ADDRESS.Add(model.SignBoardAddress);
                }
                else
                {
                    //Get Record
                    P_MW_ADDRESS record = db.P_MW_ADDRESS.Where(d => d.UUID == model.SignBoardAddress.UUID).FirstOrDefault();

                    //Set Data
                    record.STREE_NAME = model.SignBoardAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET = model.SignBoardAddress.DISPLAY_STREET;
                    record.STREET_NO = model.SignBoardAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET_NO = model.SignBoardAddress.DISPLAY_STREET_NO;
                    record.BUILDING_NAME = model.SignBoardAddress.BUILDING_NAME;
                    record.DISPLAY_BUILDINGNAME = model.SignBoardAddress.BUILDING_NAME;
                    record.FLOOR = model.SignBoardAddress.FLOOR;
                    record.DISPLAY_FLOOR = model.SignBoardAddress.FLOOR;
                    record.FLAT = model.SignBoardAddress.FLAT;
                    record.DISPLAY_FLAT = model.SignBoardAddress.FLAT;
                    record.DISTRICT = model.SignBoardAddress.DISTRICT;
                    record.DISPLAY_DISTRICT = model.SignBoardAddress.DISTRICT;
                    record.REGION = model.SignBoardAddress.REGION;
                    record.DISPLAY_REGION = model.SignBoardAddress.REGION;
                    if (ProcessingConstant.LANG_ENGLISH.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                    else if (ProcessingConstant.LANG_CHINESE.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.CHINESE_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                }

                db.SaveChanges();

                //Save Person Contact
                //Set Address UUID
                model.SignBoardPersonContact.MW_ADDRESS_ID = model.SignBoardAddress.UUID;
                if (string.IsNullOrEmpty(model.SignBoardPersonContact.UUID))
                {
                    db.P_MW_PERSON_CONTACT.Add(model.SignBoardPersonContact);
                    db.SaveChanges();

                    //Set Person Contact UUID 
                    //model.P_MW_RECORD.SIGNBOARD_PERFROMER_ID = model.SignBoardPersonContact.UUID;
                    model.DictID.Add("SIGNBOARD_PERFROMER_ID", model.SignBoardPersonContact.UUID);
                }
                else
                {
                    //Get Record
                    P_MW_PERSON_CONTACT record = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == model.SignBoardPersonContact.UUID).FirstOrDefault();

                    //Set Data
                    record.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;
                    record.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                    record.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                    record.OTHER_ID_TYPE = model.SignBoardPersonContact.OTHER_ID_TYPE;
                    record.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                    record.ADDRESS_SAME_A1 = model.SignBoardPersonContact.ADDRESS_SAME_A1;
                    record.ADDRESS_SAME_A4 = model.SignBoardPersonContact.ADDRESS_SAME_A4;
                    record.EMAIL = model.SignBoardPersonContact.EMAIL;
                    record.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                    record.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                    record.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;
                    record.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                    record.OTHER_ID_TYPE = model.SignBoardPersonContact.OTHER_ID_TYPE;
                    record.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;

                    //Save MW_ADDRESS_ID
                    record.MW_ADDRESS_ID = model.SignBoardPersonContact.MW_ADDRESS_ID;
                }

                db.SaveChanges();

            }

        }

        private void SaveOwnerInfo(Fn02MWUR_DeFormModel model, Boolean isFinalRecord, EntitiesMWProcessing db)
        {
            if (model.OwnerAddress != null && model.OwnerPersonContact != null)
            {
                //Save Address
                if (string.IsNullOrEmpty(model.OwnerAddress.UUID))
                {
                    db.P_MW_ADDRESS.Add(model.OwnerAddress);
                }
                else
                {
                    //Get Record
                    P_MW_ADDRESS record = db.P_MW_ADDRESS.Where(d => d.UUID == model.OwnerAddress.UUID).FirstOrDefault();

                    //Set Data
                    record.STREE_NAME = model.OwnerAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET = model.OwnerAddress.DISPLAY_STREET;
                    record.STREET_NO = model.OwnerAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET_NO = model.OwnerAddress.DISPLAY_STREET_NO;
                    record.BUILDING_NAME = model.OwnerAddress.BUILDING_NAME;
                    record.DISPLAY_BUILDINGNAME = model.OwnerAddress.BUILDING_NAME;
                    record.FLOOR = model.OwnerAddress.FLOOR;
                    record.DISPLAY_FLOOR = model.OwnerAddress.FLOOR;
                    record.FLAT = model.OwnerAddress.FLAT;
                    record.DISPLAY_FLAT = model.OwnerAddress.FLAT;
                    record.DISTRICT = model.OwnerAddress.DISTRICT;
                    record.DISPLAY_DISTRICT = model.OwnerAddress.DISTRICT;
                    record.REGION = model.OwnerAddress.REGION;
                    record.DISPLAY_REGION = model.OwnerAddress.REGION;
                    if (ProcessingConstant.LANG_ENGLISH.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                    else if (ProcessingConstant.LANG_CHINESE.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.CHINESE_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                }

                db.SaveChanges();

                //Save Person Contact
                //Set Address UUID
                model.OwnerPersonContact.MW_ADDRESS_ID = model.OwnerAddress.UUID;
                if (string.IsNullOrEmpty(model.OwnerPersonContact.UUID))
                {
                    db.P_MW_PERSON_CONTACT.Add(model.OwnerPersonContact);
                    db.SaveChanges();

                    //Set Person Contact UUID 
                    //model.P_MW_RECORD.OWNER_ID = model.OwnerPersonContact.UUID;
                    model.DictID.Add("OWNER_ID", model.OwnerPersonContact.UUID);
                }
                else
                {
                    //Get Record
                    P_MW_PERSON_CONTACT record = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == model.OwnerPersonContact.UUID).FirstOrDefault();

                    //Set Data
                    record.NAME_CHINESE = model.OwnerPersonContact.NAME_CHINESE;
                    record.NAME_ENGLISH = model.OwnerPersonContact.NAME_ENGLISH;
                    record.ID_NUMBER = model.OwnerPersonContact.ID_NUMBER;
                    record.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                    record.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;
                    record.ADDRESS_SAME_A1 = model.OwnerPersonContact.ADDRESS_SAME_A1;
                    record.EMAIL = model.OwnerPersonContact.EMAIL;
                    record.FAX_NO = model.OwnerPersonContact.FAX_NO;
                    record.CONTACT_NO = model.OwnerPersonContact.CONTACT_NO;
                    record.ENDORSEMENT_DATE = model.OwnerPersonContact.ENDORSEMENT_DATE;
                    record.ID_TYPE = model.OwnerPersonContact.ID_TYPE;
                    record.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                    record.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;

                    //Save MW_ADDRESS_ID
                    record.MW_ADDRESS_ID = model.OwnerPersonContact.MW_ADDRESS_ID;
                }
                db.SaveChanges();

            }
        }

        private void SaveMWAddress(Fn02MWUR_DeFormModel model, Boolean isFinalRecord, EntitiesMWProcessing db)
        {
            if (model.MWAddress != null)
            {
                if (string.IsNullOrEmpty(model.MWAddress.UUID))
                {
                    db.P_MW_ADDRESS.Add(model.MWAddress);
                    db.SaveChanges();

                    //Set Address UUID
                    //model.P_MW_RECORD.LOCATION_ADDRESS_ID = model.MWAddress.UUID;
                    model.DictID.Add("LOCATION_ADDRESS_ID", model.MWAddress.UUID);
                }
                else
                {
                    //Get Record
                    P_MW_ADDRESS record = db.P_MW_ADDRESS.Where(d => d.UUID == model.MWAddress.UUID).FirstOrDefault();

                    //Set Data
                    record.STREE_NAME = model.MWAddress.DISPLAY_STREET;
                    record.DISPLAY_STREET = model.MWAddress.DISPLAY_STREET;
                    record.STREET_NO = model.MWAddress.DISPLAY_STREET_NO;
                    record.DISPLAY_STREET_NO = model.MWAddress.DISPLAY_STREET_NO;
                    record.BUILDING_NAME = model.MWAddress.BUILDING_NAME;
                    record.DISPLAY_BUILDINGNAME = model.MWAddress.BUILDING_NAME;
                    record.FLOOR = model.MWAddress.FLOOR;
                    record.DISPLAY_FLOOR = model.MWAddress.FLOOR;
                    record.FLAT = model.MWAddress.FLAT;
                    record.DISPLAY_FLAT = model.MWAddress.FLAT;
                    record.DISTRICT = model.MWAddress.DISTRICT;
                    record.DISPLAY_DISTRICT = model.MWAddress.DISTRICT;
                    record.REGION =
                    record.DISPLAY_REGION = model.MWAddress.REGION;
                    if (ProcessingConstant.LANG_ENGLISH.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                    else if (ProcessingConstant.LANG_CHINESE.Equals(model.P_MW_RECORD.LANGUAGE_CODE))
                    {
                        record.CHINESE_DISPLAY = pc.getAddressDisplayFormat(record, model.P_MW_RECORD.LANGUAGE_CODE);
                    }
                }
                db.SaveChanges();
            }
        }

        private void SaveP_MW_RECORD(Fn02MWUR_DeFormModel model, Boolean isFinalRecord, EntitiesMWProcessing db)
        {
            Type type = typeof(P_MW_RECORD);
            if (string.IsNullOrEmpty(model.P_MW_RECORD.UUID))
            {
                foreach (string key in model.DictID.Keys)
                {
                    type.GetProperty(key).SetValue(model.P_MW_RECORD, model.DictID[key]);
                }
                model.P_MW_RECORD.STATUS_CODE = (isFinalRecord ?
                    ProcessingConstant.MW_FINAL_VERSION : ProcessingConstant.MW_SECOND_ENTRY);
                model.P_MW_RECORD.IS_DATA_ENTRY = (isFinalRecord ?
                    ProcessingConstant.FLAG_N : ProcessingConstant.FLAG_Y);

                db.P_MW_RECORD.Add(model.P_MW_RECORD);
            }
            else
            {
                //Get Record
                P_MW_RECORD record = db.P_MW_RECORD.Where(d => d.UUID == model.P_MW_RECORD.UUID).FirstOrDefault();

                //Set Data
                record.FILEREF_FOUR = model.P_MW_RECORD.FILEREF_FOUR;
                record.FILEREF_TWO = model.P_MW_RECORD.FILEREF_TWO;
                record.LANGUAGE_CODE = model.P_MW_RECORD.LANGUAGE_CODE;
                record.FIRST_RECEIVED_DATE = model.P_MW_RECORD.FIRST_RECEIVED_DATE;
                record.LOCATION_OF_MINOR_WORK = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK;
                record.STATUS_CODE = (isFinalRecord ?
                    ProcessingConstant.MW_FINAL_VERSION : ProcessingConstant.MW_SECOND_ENTRY);
                record.IS_DATA_ENTRY = (isFinalRecord ?
                    ProcessingConstant.FLAG_N : ProcessingConstant.FLAG_Y);
                record.PERMIT_NO = model.P_MW_RECORD.PERMIT_NO;
                record.SITE_AUDIT_RELATED = model.P_MW_RECORD.SITE_AUDIT_RELATED;
                record.SITE_AUDIT_RELATED_MW = model.P_MW_RECORD.SITE_AUDIT_RELATED_MW;
                record.SITE_AUDIT_RELATED_SB = model.P_MW_RECORD.SITE_AUDIT_RELATED_SB;

                // Begin add by Chester 2019-07-31

                //record.EFSS_REF_NO = isFinalRecord ? model.P_MW_RECORD.EFSS_REF_NO : record.EFSS_REF_NO;
                //record.SUBMIT_TYPE = isFinalRecord ? model.P_MW_RECORD.SUBMIT_TYPE : record.SUBMIT_TYPE;
                //record.P_MW_REFERENCE_NO = isFinalRecord ? model.P_MW_RECORD.P_MW_REFERENCE_NO : record.P_MW_REFERENCE_NO;
                //record.S_FORM_TYPE_CODE = isFinalRecord ? model.P_MW_RECORD.S_FORM_TYPE_CODE : record.S_FORM_TYPE_CODE;
                //record.MW_DSN = isFinalRecord ? model.P_MW_RECORD.MW_DSN : record.MW_DSN;
                //record.FORM_VERSION = isFinalRecord ? model.P_MW_RECORD.FORM_VERSION : record.FORM_VERSION;
                //record.MW05_ITEM36 = isFinalRecord ? model.P_MW_RECORD.MW05_ITEM36 : record.MW05_ITEM36;
                //record.NO_OF_PREMISES = isFinalRecord ? model.P_MW_RECORD.NO_OF_PREMISES : record.NO_OF_PREMISES;

                // End add by Chester 2019-07-31

                foreach (string key in model.DictID.Keys)
                {
                    type.GetProperty(key).SetValue(record, model.DictID[key]);
                }
            }

            db.SaveChanges();
        }

        private void SaveP_MW_RECORD_ITEMs(Fn02MWUR_DeFormModel model, EntitiesMWProcessing db)
        {
            for (int i = 0; i < model.P_MW_RECORD_ITEMs.Count(); i++)
            {
                var item = model.P_MW_RECORD_ITEMs[i];
                //Add Or Update
                if (string.IsNullOrEmpty(item.UUID))
                {
                    //Add
                    db.P_MW_RECORD_ITEM.Add(new P_MW_RECORD_ITEM
                    {
                        MW_ITEM_CODE = item.MW_ITEM_CODE,
                        MW_RECORD_ID = item.MW_RECORD_ID,
                        LOCATION_DESCRIPTION = item.LOCATION_DESCRIPTION,
                        RELEVANT_REFERENCE = item.RELEVANT_REFERENCE,
                        CLASS_CODE = item.CLASS_CODE,
                        LAST_MODIFIED_FORM_CODE = item.LAST_MODIFIED_FORM_CODE,
                        STATUS_CODE = item.STATUS_CODE,
                        ORDERING = i
                    });
                }
                else
                {
                    //Get Record
                    //P_MW_RECORD_ITEM record = db.P_MW_RECORD_ITEM.Where(d => d.UUID == models[i].UUID).FirstOrDefault();
                    P_MW_RECORD_ITEM record = db.P_MW_RECORD_ITEM.Where(d => d.UUID == item.UUID).FirstOrDefault();
                    //Update
                    record.MW_ITEM_CODE = item.MW_ITEM_CODE;
                    record.MW_RECORD_ID = item.MW_RECORD_ID;
                    record.LOCATION_DESCRIPTION = item.LOCATION_DESCRIPTION;
                    record.RELEVANT_REFERENCE = item.RELEVANT_REFERENCE;
                    record.LAST_MODIFIED_FORM_CODE = item.LAST_MODIFIED_FORM_CODE;
                    record.STATUS_CODE = item.STATUS_CODE;
                    record.ORDERING = i;
                }
            }

            db.SaveChanges();
        }

        private void SaveP_MW_APPOINTED_PROFESSIONALs(Fn02MWUR_DeFormModel model, EntitiesMWProcessing db)
        {
            foreach (var item in model.P_MW_APPOINTED_PROFESSIONALs)
            {
                if (string.IsNullOrEmpty(item.UUID))
                {
                    db.P_MW_APPOINTED_PROFESSIONAL.Add(item);
                }
                else
                {
                    //Get Record
                    P_MW_APPOINTED_PROFESSIONAL record = db.P_MW_APPOINTED_PROFESSIONAL.Where(d => d.UUID == item.UUID).FirstOrDefault();
                    //Update
                    record.CHINESE_NAME = item.CHINESE_NAME;
                    record.ENGLISH_NAME = item.ENGLISH_NAME;
                    record.CHINESE_COMPANY_NAME = item.CHINESE_COMPANY_NAME;
                    record.ENGLISH_COMPANY_NAME = item.ENGLISH_COMPANY_NAME;
                    record.CERTIFICATION_NO = item.CERTIFICATION_NO;
                    record.IDENTIFY_FLAG = item.IDENTIFY_FLAG;
                    record.ISCHECKED = item.ISCHECKED;
                    record.RECEIVE_SMS = item.RECEIVE_SMS;
                    record.MW_NO = item.MW_NO;
                    record.FAX_NO = item.FAX_NO;
                    record.CONTACT_NO = item.CONTACT_NO;
                    record.EXPIRY_DATE = item.EXPIRY_DATE;
                    record.ENDORSEMENT_DATE = item.ENDORSEMENT_DATE;
                    record.COMMENCED_DATE = item.COMMENCED_DATE;
                    record.COMPLETION_DATE = item.COMPLETION_DATE;
                    record.EFFECT_FROM_DATE = item.EFFECT_FROM_DATE;
                    record.EFFECT_TO_DATE = item.EFFECT_TO_DATE;
                    record.APPOINTMENT_DATE = item.APPOINTMENT_DATE;
                    record.CLASS1_CEASE_DATE = item.CLASS1_CEASE_DATE;
                }

            }

            db.SaveChanges();
        }

        private void SaveP_MW_FORM_09s(Fn02MWUR_DeFormModel model, EntitiesMWProcessing db)
        {
            if (model.P_MW_FORM_09s != null)
            {
                db.P_MW_FORM_09.RemoveRange(db.P_MW_FORM_09.Where(d => d.MW_RECORD_ID == model.P_MW_RECORD.UUID));

                foreach (var item in model.P_MW_FORM_09s)
                {
                    item.MW_RECORD_ID = model.P_MW_RECORD.UUID;
                    db.P_MW_FORM_09.Add(item);
                }
            }
        }

        private void SaveP_MW_FORM(Fn02MWUR_DeFormModel model, EntitiesMWProcessing db)
        {
            if (!string.IsNullOrEmpty(model.P_MW_FORM.UUID))
            {
                P_MW_FORM record = db.P_MW_FORM.Where(w => w.UUID == model.P_MW_FORM.UUID).FirstOrDefault();

                if (record != null)
                {
                    model.P_MW_FORM.RECEIVED_DATE = record.RECEIVED_DATE;

                    db.P_MW_FORM.Remove(record);
                }
            }
            model.P_MW_FORM.UUID = null;
            model.P_MW_FORM.MW_RECORD_ID = model.P_MW_RECORD.UUID;

            db.P_MW_FORM.Add(model.P_MW_FORM);

            db.SaveChanges();
        }

        // handle Data entry submit form 
        public ServiceResult SubmitForm(Fn02MWUR_DeFormModel model)
        {
            // Copy model as final model
            Fn02MWUR_DeFormModel finalModel = model;

            ServiceResult serviceResult = SaveAsDraft(model, true);

            if (ServiceResult.RESULT_SUCCESS.Equals(serviceResult))
            {
                using (EntitiesMWProcessing db = new EntitiesMWProcessing())
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            List<String> autoGenMwFormCodeList = ProcessingConstant.autoGenMwFormCodeList();
                            Boolean isUpdateFinalRecord = false;
                            P_MW_RECORD mwRecordFinal = null;

                            if (autoGenMwFormCodeList.Contains(model.P_MW_RECORD.S_FORM_TYPE_CODE))
                            {
                                isUpdateFinalRecord = true;
                            }

                            // get final record by Mw Reference No.
                            mwRecordFinal = GetFinalMwRecord(model.P_MW_REFERENCE_NO.REFERENCE_NO);

                            if (isUpdateFinalRecord && mwRecordFinal == null)
                            {
                                // Create a new final record
                            }
                            else
                            {
                                // Update final record
                            }

                            // Update MwDsn
                            // P_MW_DSN.SCANNED_STATUS_ID = ProcessingConstant.SECOND_ENTRY_COMPLETED;

                            // Update MwRecord
                            // 找到P_MW_RECORD, update status from MW_SECOND_ENTRY to SECOND_ENTRY_COMPLETED

                            // Create MwVerification
                            // P_MW_VERIFICATION mwVerification = new P_MW_VERIFICATION();
                            // mwVerification.MW_RECORD_ID = XXXXXXXXX
                            // mwVerification.STATUS_CODE = 'MW_VERT_STATUS_OPEN';
                        }
                        catch (DbEntityValidationException validationError)
                        {
                            var errorMessages = validationError.EntityValidationErrors
                            .SelectMany(validationResult => validationResult.ValidationErrors)
                            .Select(m => m.ErrorMessage);
                            Console.WriteLine("Error :" + validationError.Message);
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            Console.WriteLine("Error :" + ex.Message);
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                        }
                    }
                }
            }

            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }



        //public void UpdateForm01(Fn02MWUR_DeFormModel model, P_MW_RECORD record, P_MW_FORM mwform, P_MW_PERSON_CONTACT owner, P_MW_PERSON_CONTACT oi, P_MW_PERSON_CONTACT signboard, DateTime dsnSubmissionDate, string sspSubmitted, EntitiesMWProcessing db)
        public void UpdateForm01(Fn02MWUR_DeFormModel oldModel, Fn02MWUR_DeFormModel newModel, string sspSubmitted, EntitiesMWProcessing db)
        {
            P_MW_RECORD record = newModel.P_MW_RECORD;
            P_MW_FORM mwform = newModel.P_MW_FORM;
            P_MW_PERSON_CONTACT owner = newModel.OwnerPersonContact;
            P_MW_PERSON_CONTACT oi = newModel.OIPersonContact;
            P_MW_PERSON_CONTACT signboard = newModel.SignBoardPersonContact;
            List<P_MW_APPOINTED_PROFESSIONAL> professionals = newModel.P_MW_APPOINTED_PROFESSIONALs;
            DateTime dsnSubmissionDate = newModel.P_MW_DSN.CREATED_DATE;
            //MW_Record
            if (sspSubmitted == ProcessingConstant.FLAG_N)
            {
                oldModel.P_MW_RECORD.VERIFICATION_SPO = ProcessingConstant.FLAG_Y;
            }
            //P_MW_RECORD record = db.P_MW_RECORD.Where(d => d.UUID == model.P_MW_RECORD.UUID).FirstOrDefault();
            if (record != null && oldModel.P_MW_RECORD != null)
            {
                record.CLASS_CODE = ProcessingConstant.DB_CLASS_I;
                record.LOCATION_OF_MINOR_WORK = oldModel.P_MW_RECORD.LOCATION_OF_MINOR_WORK;
                record.COMMENCEMENT_DATE = oldModel.P_MW_APPOINTED_PROFESSIONALs[4].COMMENCED_DATE;
                record.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                record.COMPLETION_DATE = null;
                record.COMPLETION_SUBMISSION_DATE = null;
                record.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;
            }

            //MW_FORM
            //P_MW_FORM mwform = db.P_MW_FORM.Where(m => m.UUID == model.P_MW_FORM.UUID).FirstOrDefault();
            if (mwform != null && oldModel.P_MW_FORM != null)
            {
                mwform.FORM01_B_6 = oldModel.P_MW_FORM.FORM01_B_6;
                mwform.FORM01_B_7 = oldModel.P_MW_FORM.FORM01_B_7;
                mwform.FORM01_B_8 = oldModel.P_MW_FORM.FORM01_B_8;
                mwform.FORM01_C_3 = oldModel.P_MW_FORM.FORM01_C_3;
                mwform.FORM01_E_S23 = oldModel.P_MW_FORM.FORM01_E_S23;
                mwform.FORM01_E_2 = oldModel.P_MW_FORM.FORM01_E_2;
                mwform.FORM01_E_3 = oldModel.P_MW_FORM.FORM01_E_3;
                mwform.FORM01_E_4 = oldModel.P_MW_FORM.FORM01_E_4;
                mwform.FORM01_E_5 = oldModel.P_MW_FORM.FORM01_E_5;
                mwform.FORM01_E_6 = oldModel.P_MW_FORM.FORM01_E_6;
                mwform.FORM01_E_7 = oldModel.P_MW_FORM.FORM01_E_6;
            }

            //MW_Address
            if (oldModel.MWAddress != null)
            {
                if (oldModel.MWAddress.UUID == null)
                {
                    oldModel.MWAddress.UUID = record.LOCATION_ADDRESS_ID;
                }
                //oldModel.MWAddress.CREATED_BY = record.CREATED_BY;
                //oldModel.MWAddress.CREATED_DATE = record.CREATED_DATE;
                db.Entry<P_MW_ADDRESS>(oldModel.MWAddress).State = EntityState.Modified;
            }
            db.SaveChanges();
            if (oldModel.OwnerAddress != null)
            {
                if (oldModel.OwnerAddress.UUID == null)
                {
                    oldModel.OwnerAddress.UUID = owner.MW_ADDRESS_ID;
                }
                oldModel.OwnerAddress.CREATED_BY = owner.CREATED_BY;
                oldModel.OwnerAddress.CREATED_DATE = owner.CREATED_DATE;
                db.Entry<P_MW_ADDRESS>(oldModel.OwnerAddress).State = EntityState.Modified;
            }
            db.SaveChanges();
            if (oldModel.OIAddress != null)
            {
                if (oldModel.OIAddress.UUID == null)
                {
                    oldModel.OIAddress.UUID = oi.MW_ADDRESS_ID;
                }
                oldModel.OIAddress.CREATED_BY = oi.CREATED_BY;
                oldModel.OIAddress.CREATED_DATE = oi.CREATED_DATE;
                db.Entry<P_MW_ADDRESS>(oldModel.OIAddress).State = EntityState.Modified;
            }
            db.SaveChanges();
            if (oldModel.SignBoardAddress != null)
            {
                if (oldModel.SignBoardAddress.UUID == null)
                {
                    oldModel.SignBoardAddress.UUID = signboard.MW_ADDRESS_ID;
                }
                oldModel.SignBoardAddress.CREATED_BY = signboard.CREATED_BY;
                oldModel.SignBoardAddress.CREATED_DATE = signboard.CREATED_DATE;
                db.Entry<P_MW_ADDRESS>(oldModel.SignBoardAddress).State = EntityState.Modified;
            }
            db.SaveChanges();

            //MW PersonContact
            //P_MW_PERSON_CONTACT owner = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.OwnerPersonContact.UUID).FirstOrDefault();
            if (owner != null && oldModel.OwnerPersonContact != null)
            {
                owner.NAME_CHINESE = oldModel.OwnerPersonContact.NAME_CHINESE;
                owner.NAME_ENGLISH = oldModel.OwnerPersonContact.NAME_ENGLISH;
                owner.NAME_CHINESE2 = oldModel.OwnerPersonContact.NAME_CHINESE2;
                owner.NAME_ENGLISH2 = oldModel.OwnerPersonContact.NAME_CHINESE2;
                owner.EMAIL = oldModel.OwnerPersonContact.EMAIL;
                owner.CONTACT_NO = oldModel.OwnerPersonContact.CONTACT_NO;
                owner.FAX_NO = oldModel.OwnerPersonContact.FAX_NO;
                owner.ID_TYPE = oldModel.OwnerPersonContact.ID_TYPE;
                owner.OTHER_ID_TYPE = oldModel.OwnerPersonContact.OTHER_ID_TYPE;
                owner.ID_NUMBER = oldModel.OwnerPersonContact.ID_NUMBER;
                owner.CONTACT_PERSON_TITLE = oldModel.OwnerPersonContact.CONTACT_PERSON_TITLE;
                owner.FIRST_NAME = oldModel.OwnerPersonContact.FIRST_NAME;
                owner.LAST_NAME = oldModel.OwnerPersonContact.LAST_NAME;
                owner.MOBILE = oldModel.OwnerPersonContact.MOBILE;
                owner.ENDORSEMENT_DATE = oldModel.OwnerPersonContact.ENDORSEMENT_DATE;
            }
            db.SaveChanges();

            //P_MW_PERSON_CONTACT oi = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.OIPersonContact.UUID).FirstOrDefault();
            if (oi != null && oldModel.OIPersonContact != null)
            {
                oi.NAME_CHINESE = oldModel.OIPersonContact.NAME_CHINESE;
                oi.NAME_ENGLISH = oldModel.OIPersonContact.NAME_ENGLISH;
                oi.NAME_CHINESE2 = oldModel.OIPersonContact.NAME_CHINESE2;
                oi.NAME_ENGLISH2 = oldModel.OIPersonContact.NAME_ENGLISH2;
                oi.EMAIL = oldModel.OIPersonContact.EMAIL;
                oi.CONTACT_NO = oldModel.OIPersonContact.CONTACT_NO;
                oi.FAX_NO = oldModel.OIPersonContact.FAX_NO;
                oi.ID_TYPE = oldModel.OIPersonContact.ID_TYPE;
                oi.OTHER_ID_TYPE = oldModel.OIPersonContact.OTHER_ID_TYPE;
                oi.ID_NUMBER = oldModel.OIPersonContact.ID_NUMBER;
                oi.ID_ISSUE_COUNTRY = oldModel.OIPersonContact.ID_ISSUE_COUNTRY;
                oi.CONTACT_PERSON_TITLE = oldModel.OIPersonContact.CONTACT_PERSON_TITLE;
                oi.FIRST_NAME = oldModel.OIPersonContact.FIRST_NAME;
                oi.LAST_NAME = oldModel.OIPersonContact.LAST_NAME;
                oi.MOBILE = oldModel.OIPersonContact.MOBILE;
                oi.ENDORSEMENT_DATE = oldModel.OIPersonContact.ENDORSEMENT_DATE;
                oi.PRC = oldModel.OIPersonContact.PRC;
                oi.PRC_CONTACT_NO = oldModel.OIPersonContact.PRC_CONTACT_NO;
                oi.PBP = oldModel.OIPersonContact.PBP;
                oi.PBP_CONTACT_NO = oldModel.OIPersonContact.PBP_CONTACT_NO;
            }
            db.SaveChanges();

            //P_MW_PERSON_CONTACT signboard = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.SignBoardPersonContact.UUID).FirstOrDefault();
            if (signboard != null && oldModel.SignBoardPersonContact != null)
            {
                signboard.NAME_CHINESE = oldModel.SignBoardPersonContact.NAME_CHINESE;
                signboard.NAME_CHINESE2 = oldModel.SignBoardPersonContact.NAME_CHINESE2;
                signboard.NAME_ENGLISH = oldModel.SignBoardPersonContact.NAME_ENGLISH;
                signboard.NAME_ENGLISH2 = oldModel.SignBoardPersonContact.NAME_ENGLISH2;
                signboard.EMAIL = oldModel.SignBoardPersonContact.EMAIL;
                signboard.CONTACT_NO = oldModel.SignBoardPersonContact.CONTACT_NO;
                signboard.FAX_NO = oldModel.SignBoardPersonContact.FAX_NO;
                signboard.ID_TYPE = oldModel.SignBoardPersonContact.ID_TYPE;
                signboard.OTHER_ID_TYPE = oldModel.SignBoardPersonContact.OTHER_ID_TYPE;
                signboard.ID_NUMBER = oldModel.SignBoardPersonContact.ID_NUMBER;
                signboard.ID_ISSUE_COUNTRY = oldModel.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                signboard.CONTACT_PERSON_TITLE = oldModel.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                signboard.FIRST_NAME = oldModel.SignBoardPersonContact.FIRST_NAME;
                signboard.LAST_NAME = oldModel.SignBoardPersonContact.LAST_NAME;
                signboard.MOBILE = oldModel.SignBoardPersonContact.MOBILE;
                signboard.ENDORSEMENT_DATE = oldModel.SignBoardPersonContact.ENDORSEMENT_DATE;
            }
            db.SaveChanges();

            //Professional
            P_MW_APPOINTED_PROFESSIONAL secProf_AP = oldModel.P_MW_APPOINTED_PROFESSIONALs[4];
            secProf_AP = professionals[4];
            P_MW_APPOINTED_PROFESSIONAL secProf_RSE = oldModel.P_MW_APPOINTED_PROFESSIONALs[5];
            secProf_RSE = professionals[5];
            P_MW_APPOINTED_PROFESSIONAL secProf_RGE = oldModel.P_MW_APPOINTED_PROFESSIONALs[6];
            secProf_RGE = professionals[6];
            P_MW_APPOINTED_PROFESSIONAL secProf_PRC = oldModel.P_MW_APPOINTED_PROFESSIONALs[7];
            secProf_PRC = professionals[7];

            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_AP).State = EntityState.Modified;
            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_RSE).State = EntityState.Modified;
            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_RGE).State = EntityState.Modified;
            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_PRC).State = EntityState.Modified;

            //MW ITEM
            db.SaveChanges();

        }

        private void UpdateForm03(Fn02MWUR_DeFormModel model, P_MW_RECORD record, P_MW_FORM mwform, P_MW_PERSON_CONTACT owner, P_MW_PERSON_CONTACT oi, P_MW_PERSON_CONTACT signboard, DateTime dsnSubmissionDate, EntitiesMWProcessing db)
        {
            //MW_RECORD
            if (record != null && model.P_MW_RECORD != null)
            {
                record.CLASS_CODE = ProcessingConstant.DB_CLASS_II;
                record.LOCATION_OF_MINOR_WORK = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK;
                record.COMMENCEMENT_DATE = model.P_MW_APPOINTED_PROFESSIONALs[1].COMMENCED_DATE;
                record.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                record.MW_PROGRESS_STATUS_CODE = model.P_MW_RECORD.MW_PROGRESS_STATUS_CODE;
            }

            //MW FORM
            if (mwform != null && model.P_MW_FORM != null)
            {
                mwform.FORM03_B_6 = model.P_MW_FORM.FORM03_B_6;
                mwform.FORM03_B_7 = model.P_MW_FORM.FORM03_B_7;
            }

            //MW Address
            db.Entry<P_MW_ADDRESS>(model.MWAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OwnerAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OIAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.SignBoardAddress).State = EntityState.Modified;

            //MW PersonContact
            if (owner != null && model.OwnerPersonContact != null)
            {
                owner.NAME_CHINESE = model.OwnerPersonContact.NAME_CHINESE;
                owner.NAME_ENGLISH = model.OwnerPersonContact.NAME_ENGLISH;
                owner.NAME_CHINESE2 = model.OwnerPersonContact.NAME_CHINESE2;
                owner.NAME_ENGLISH2 = model.OwnerPersonContact.NAME_ENGLISH2;
                owner.EMAIL = model.OwnerPersonContact.EMAIL;
                owner.CONTACT_NO = model.OwnerPersonContact.CONTACT_NO;
                owner.FAX_NO = model.OwnerPersonContact.FAX_NO;
                owner.ID_TYPE = model.OwnerPersonContact.ID_TYPE;
                owner.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                owner.ID_NUMBER = model.OwnerPersonContact.ID_NUMBER;
                owner.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;
                owner.CONTACT_PERSON_TITLE = model.OwnerPersonContact.CONTACT_PERSON_TITLE;
                owner.FIRST_NAME = model.OwnerPersonContact.FIRST_NAME;
                owner.LAST_NAME = model.OwnerPersonContact.LAST_NAME;
                owner.MOBILE = model.OwnerPersonContact.MOBILE;
                owner.ENDORSEMENT_DATE = model.OwnerPersonContact.ENDORSEMENT_DATE;
            }

            if (oi != null && model.OIPersonContact != null)
            {
                oi.NAME_CHINESE = model.OIPersonContact.NAME_CHINESE;
                oi.NAME_ENGLISH = model.OIPersonContact.NAME_ENGLISH;
                oi.NAME_CHINESE2 = model.OIPersonContact.NAME_CHINESE2;
                oi.NAME_ENGLISH2 = model.OIPersonContact.NAME_ENGLISH2;
                oi.EMAIL = model.OIPersonContact.EMAIL;
                oi.CONTACT_NO = model.OIPersonContact.CONTACT_NO;
                oi.FAX_NO = model.OIPersonContact.FAX_NO;
                oi.ID_TYPE = model.OIPersonContact.ID_TYPE;
                oi.ID_NUMBER = model.OIPersonContact.ID_NUMBER;
                oi.ID_ISSUE_COUNTRY = model.OIPersonContact.ID_ISSUE_COUNTRY;
                oi.CONTACT_PERSON_TITLE = model.OIPersonContact.CONTACT_PERSON_TITLE;
                oi.FIRST_NAME = model.OIPersonContact.FIRST_NAME;
                oi.LAST_NAME = model.OIPersonContact.LAST_NAME;
                oi.MOBILE = model.OIPersonContact.MOBILE;
                oi.ENDORSEMENT_DATE = model.OIPersonContact.ENDORSEMENT_DATE;
            }

            if (signboard != null && model.SignBoardPersonContact != null)
            {
                signboard.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;
                signboard.NAME_CHINESE2 = model.SignBoardPersonContact.NAME_CHINESE2;
                signboard.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                signboard.NAME_ENGLISH2 = model.SignBoardPersonContact.NAME_ENGLISH2;
                signboard.EMAIL = model.SignBoardPersonContact.EMAIL;
                signboard.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                signboard.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                signboard.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                signboard.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                signboard.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                signboard.CONTACT_PERSON_TITLE = model.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                signboard.FIRST_NAME = model.SignBoardPersonContact.FIRST_NAME;
                signboard.LAST_NAME = model.SignBoardPersonContact.LAST_NAME;
                signboard.MOBILE = model.SignBoardPersonContact.MOBILE;
                signboard.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;
            }

            //Professional
            P_MW_APPOINTED_PROFESSIONAL secProf_PRC = model.P_MW_APPOINTED_PROFESSIONALs[1];
            secProf_PRC.UUID = record.P_MW_APPOINTED_PROFESSIONAL.ToList()[1].UUID;
            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_PRC).State = EntityState.Modified;

            //MW ITEM



        }

        private void UpdateForm05(Fn02MWUR_DeFormModel model, P_MW_RECORD record, P_MW_FORM mwform, P_MW_PERSON_CONTACT owner, P_MW_PERSON_CONTACT oi, P_MW_PERSON_CONTACT signboard, DateTime dsnSubmissionDate, EntitiesMWProcessing db)
        {
            P_MW_RECORD finalRecord = db.P_MW_RECORD.Where(m => m.P_MW_REFERENCE_NO.REFERENCE_NO == model.P_MW_RECORD.P_MW_REFERENCE_NO.REFERENCE_NO).FirstOrDefault();
            if (finalRecord != null)
            {
                //MW_RECORD
                if (record != null && model.P_MW_RECORD != null)
                {
                    record.P_MW_REFERENCE_NO.REFERENCE_NO = model.P_MW_REFERENCE_NO.REFERENCE_NO;
                    record.S_FORM_TYPE_CODE = model.P_MW_RECORD.S_FORM_TYPE_CODE;
                    record.LANGUAGE_CODE = model.P_MW_RECORD.LANGUAGE_CODE;
                    record.COMMENCEMENT_DATE = model.P_MW_APPOINTED_PROFESSIONALs[1].COMMENCED_DATE;
                    record.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                    record.COMPLETION_DATE = model.P_MW_APPOINTED_PROFESSIONALs[1].COMPLETION_DATE;
                    record.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                    record.CLASS_CODE = ProcessingConstant.DB_CLASS_III;
                    record.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMPLETION;
                    record.LOCATION_OF_MINOR_WORK = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK;
                }

                //MW_FORM
                if (mwform != null && model.P_MW_FORM != null)
                {
                    mwform.FORM05_B_5 = model.P_MW_FORM.FORM05_B_5;
                    mwform.FORM05_B_6 = model.P_MW_FORM.FORM05_B_6;
                }

                //MW_Address
                db.Entry<P_MW_ADDRESS>(model.MWAddress).State = EntityState.Modified;
                db.Entry<P_MW_ADDRESS>(model.OwnerAddress).State = EntityState.Modified;
                db.Entry<P_MW_ADDRESS>(model.OIAddress).State = EntityState.Modified;

                //Mw_PersonContact
                if (owner != null && model.OwnerPersonContact != null)
                {
                    owner.NAME_CHINESE = model.OwnerPersonContact.NAME_CHINESE;
                    owner.NAME_ENGLISH = model.OwnerPersonContact.NAME_ENGLISH;
                    owner.NAME_CHINESE2 = model.OwnerPersonContact.NAME_CHINESE2;
                    owner.NAME_ENGLISH2 = model.OwnerPersonContact.NAME_ENGLISH2;
                    owner.EMAIL = model.OwnerPersonContact.EMAIL;
                    owner.CONTACT_NO = model.OwnerPersonContact.CONTACT_NO;
                    owner.FAX_NO = model.OwnerPersonContact.FAX_NO;
                    owner.ID_TYPE = model.OwnerPersonContact.ID_TYPE;
                    owner.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                    owner.ID_NUMBER = model.OwnerPersonContact.ID_NUMBER;
                    owner.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;
                    owner.CONTACT_PERSON_TITLE = model.OwnerPersonContact.CONTACT_PERSON_TITLE;
                    owner.FIRST_NAME = model.OwnerPersonContact.FIRST_NAME;
                    owner.LAST_NAME = model.OwnerPersonContact.LAST_NAME;
                    owner.MOBILE = model.OwnerPersonContact.MOBILE;
                    owner.ENDORSEMENT_DATE = model.OwnerPersonContact.ENDORSEMENT_DATE;
                }

                if (oi != null && model.OIPersonContact != null)
                {
                    oi.NAME_CHINESE = model.OIPersonContact.NAME_CHINESE;
                    oi.NAME_ENGLISH = model.OIPersonContact.NAME_ENGLISH;
                    oi.NAME_CHINESE2 = model.OIPersonContact.NAME_CHINESE2;
                    oi.NAME_ENGLISH2 = model.OIPersonContact.NAME_ENGLISH2;
                    oi.EMAIL = model.OIPersonContact.EMAIL;
                    oi.CONTACT_NO = model.OIPersonContact.CONTACT_NO;
                    oi.FAX_NO = model.OIPersonContact.FAX_NO;
                    oi.ID_TYPE = model.OIPersonContact.ID_TYPE;
                    oi.OTHER_ID_TYPE = model.OIPersonContact.OTHER_ID_TYPE;
                    oi.ID_NUMBER = model.OIPersonContact.ID_NUMBER;
                    oi.ID_ISSUE_COUNTRY = model.OIPersonContact.ID_ISSUE_COUNTRY;
                    oi.CONTACT_PERSON_TITLE = model.OIPersonContact.CONTACT_PERSON_TITLE;
                    oi.FIRST_NAME = model.OIPersonContact.FIRST_NAME;
                    oi.LAST_NAME = model.OIPersonContact.LAST_NAME;
                    oi.MOBILE = model.OIPersonContact.MOBILE;
                    oi.ENDORSEMENT_DATE = model.OIPersonContact.ENDORSEMENT_DATE;
                }

                //Professional
                P_MW_APPOINTED_PROFESSIONAL secProf_PRC = model.P_MW_APPOINTED_PROFESSIONALs[1];
                secProf_PRC.UUID = record.P_MW_APPOINTED_PROFESSIONAL.ToList()[1].UUID;
                db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_PRC).State = EntityState.Modified;

                //MW_ITEM


            }
        }

        private void UpdateForm06(Fn02MWUR_DeFormModel model, P_MW_RECORD record, P_MW_FORM mwform, P_MW_PERSON_CONTACT owner, P_MW_PERSON_CONTACT oi, P_MW_PERSON_CONTACT signboard, DateTime dsnSubmissionDate, EntitiesMWProcessing db)
        {
            //MW_RECORD
            if (record != null && model.P_MW_RECORD != null)
            {
                record.CLASS_CODE = ProcessingConstant.DB_CLASS_VS;
                record.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_VS;
                record.MW_PROGRESS_STATUS_CODE = ProcessingConstant.VALIDATION_SCHEME;
                record.P_MW_REFERENCE_NO = model.P_MW_REFERENCE_NO;
                record.COMMENCEMENT_DATE = model.P_MW_APPOINTED_PROFESSIONALs[3].COMMENCED_DATE;
                record.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                record.COMPLETION_DATE = model.P_MW_APPOINTED_PROFESSIONALs[3].COMPLETION_DATE;
                record.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                record.INSPECTION_DATE = model.P_MW_APPOINTED_PROFESSIONALs[2].COMPLETION_DATE;
                record.MW_PROGRESS_STATUS_CODE = ProcessingConstant.VALIDATION_SCHEME;
                record.LOCATION_OF_MINOR_WORK = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK;

            }

            //MW_FORM
            if (mwform != null && model.P_MW_FORM != null)
            {
                mwform.FORM06_A_1_INVOLVE = model.P_MW_FORM.FORM06_A_1_INVOLVE;
                mwform.FORM06_A_4_AP = model.P_MW_FORM.FORM06_A_4_AP;
                mwform.FORM06_A_4_RSE = model.P_MW_FORM.FORM06_A_4_RSE;
                mwform.FORM06_A_4_RGBC = model.P_MW_FORM.FORM06_A_4_RGBC;
                mwform.FORM06_A_4_RMWC = model.P_MW_FORM.FORM06_A_4_RMWC;
                mwform.FORM06_A_5_COMPLETED_MENTION = model.P_MW_FORM.FORM06_A_5_COMPLETED_MENTION;
                mwform.FORM06_A_5_IDENTICAL = model.P_MW_FORM.FORM06_A_5_IDENTICAL;
                mwform.FORM06_B_1_AP = model.P_MW_FORM.FORM06_B_1_AP;
                mwform.FORM06_B_1_RSE = model.P_MW_FORM.FORM06_B_1_RSE;
                mwform.FORM06_B_1_RGBC = model.P_MW_FORM.FORM06_B_1_RGBC;
                mwform.FORM06_B_1_RMWC = model.P_MW_FORM.FORM06_B_1_RMWC;
                mwform.FORM06_C_5 = model.P_MW_FORM.FORM06_C_5;
            }

            //MW_Address
            db.Entry<P_MW_ADDRESS>(model.MWAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OwnerAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OIAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.SignBoardAddress).State = EntityState.Modified;

            //MW_PersonContact
            if (owner != null && model.OwnerPersonContact != null)
            {
                owner.NAME_CHINESE = model.OwnerPersonContact.NAME_CHINESE;
                owner.NAME_ENGLISH = model.OwnerPersonContact.NAME_ENGLISH;
                owner.NAME_CHINESE2 = model.OwnerPersonContact.NAME_CHINESE2;
                owner.NAME_ENGLISH2 = model.OwnerPersonContact.NAME_CHINESE2;
                owner.EMAIL = model.OwnerPersonContact.EMAIL;
                owner.CONTACT_NO = model.OwnerPersonContact.CONTACT_NO;
                owner.FAX_NO = model.OwnerPersonContact.FAX_NO;
                owner.ID_TYPE = model.OwnerPersonContact.ID_TYPE;
                owner.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                owner.ID_NUMBER = model.OwnerPersonContact.ID_NUMBER;
                owner.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;
                owner.CONTACT_PERSON_TITLE = model.OwnerPersonContact.CONTACT_PERSON_TITLE;
                owner.FIRST_NAME = model.OwnerPersonContact.FIRST_NAME;
                owner.LAST_NAME = model.OwnerPersonContact.LAST_NAME;
                owner.MOBILE = model.OwnerPersonContact.MOBILE;
                owner.ENDORSEMENT_DATE = model.OwnerPersonContact.ENDORSEMENT_DATE;
            }

            if (oi != null && model.OIPersonContact != null)
            {
                oi.NAME_CHINESE = model.OIPersonContact.NAME_CHINESE;
                oi.NAME_ENGLISH = model.OIPersonContact.NAME_ENGLISH;
                oi.NAME_CHINESE2 = model.OIPersonContact.NAME_CHINESE2;
                oi.NAME_ENGLISH2 = model.OIPersonContact.NAME_ENGLISH2;
                oi.EMAIL = model.OIPersonContact.EMAIL;
                oi.CONTACT_NO = model.OIPersonContact.CONTACT_NO;
                oi.FAX_NO = model.OIPersonContact.FAX_NO;
                oi.ID_TYPE = model.OIPersonContact.ID_TYPE;
                oi.OTHER_ID_TYPE = model.OIPersonContact.OTHER_ID_TYPE;
                oi.ID_NUMBER = model.OIPersonContact.ID_NUMBER;
                oi.ID_ISSUE_COUNTRY = model.OIPersonContact.ID_ISSUE_COUNTRY;
                oi.CONTACT_PERSON_TITLE = model.OIPersonContact.CONTACT_PERSON_TITLE;
                oi.FIRST_NAME = model.OIPersonContact.FIRST_NAME;
                oi.LAST_NAME = model.OIPersonContact.LAST_NAME;
                oi.MOBILE = model.OIPersonContact.MOBILE;
                oi.ENDORSEMENT_DATE = model.OIPersonContact.ENDORSEMENT_DATE;
            }

            if (signboard != null && model.SignBoardPersonContact != null)
            {
                signboard.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;
                signboard.NAME_CHINESE2 = model.SignBoardPersonContact.NAME_CHINESE2;
                signboard.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                signboard.NAME_ENGLISH2 = model.SignBoardPersonContact.NAME_ENGLISH2;
                signboard.EMAIL = model.SignBoardPersonContact.EMAIL;
                signboard.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                signboard.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                signboard.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                signboard.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                signboard.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                signboard.CONTACT_PERSON_TITLE = model.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                signboard.FIRST_NAME = model.SignBoardPersonContact.FIRST_NAME;
                signboard.LAST_NAME = model.SignBoardPersonContact.LAST_NAME;
                signboard.MOBILE = model.SignBoardPersonContact.MOBILE;
                signboard.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;
            }

            //Professional
            P_MW_APPOINTED_PROFESSIONAL secProf_AP = model.P_MW_APPOINTED_PROFESSIONALs[2];
            secProf_AP.UUID = record.P_MW_APPOINTED_PROFESSIONAL.ToList()[2].UUID;
            P_MW_APPOINTED_PROFESSIONAL secProf_PRC = model.P_MW_APPOINTED_PROFESSIONALs[3];
            secProf_PRC.UUID = record.P_MW_APPOINTED_PROFESSIONAL.ToList()[3].UUID;

            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_AP).State = EntityState.Modified;
            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_PRC).State = EntityState.Modified;

            //MW ITEM

        }

        private void UpdateForm32(Fn02MWUR_DeFormModel model, P_MW_RECORD record, P_MW_FORM mwform, P_MW_PERSON_CONTACT owner, P_MW_PERSON_CONTACT oi, P_MW_PERSON_CONTACT signboard, DateTime dsnSubmissionDate, EntitiesMWProcessing db)
        {
            //MW_RECORD
            if (record != null && model.P_MW_RECORD != null)
            {
                record.CLASS_CODE = ProcessingConstant.DB_CLASS_II;
                record.LOCATION_OF_MINOR_WORK = model.P_MW_RECORD.LOCATION_OF_MINOR_WORK;

                record.COMMENCEMENT_DATE = model.P_MW_APPOINTED_PROFESSIONALs[0].COMMENCED_DATE;
                record.COMMENCEMENT_SUBMISSION_DATE = dsnSubmissionDate;
                record.COMPLETION_DATE = null;
                record.COMPLETION_SUBMISSION_DATE = dsnSubmissionDate;
                record.MW_PROGRESS_STATUS_CODE = ProcessingConstant.COMMENCEMENT;
            }

            //MW_FORM
            if (mwform != null && model.P_MW_FORM != null)
            {
                mwform.FORM32_A_NO_SIGNBOARD = model.P_MW_FORM.FORM32_A_NO_SIGNBOARD;
            }

            //MW_Address
            db.Entry<P_MW_ADDRESS>(model.MWAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OwnerAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.OIAddress).State = EntityState.Modified;
            db.Entry<P_MW_ADDRESS>(model.SignBoardAddress).State = EntityState.Modified;

            //MW_PersonContact
            if (owner != null && model.OwnerPersonContact != null)
            {
                owner.NAME_CHINESE = model.OwnerPersonContact.NAME_CHINESE;
                owner.NAME_ENGLISH = model.OwnerPersonContact.NAME_ENGLISH;
                owner.NAME_CHINESE2 = model.OwnerPersonContact.NAME_CHINESE2;
                owner.NAME_ENGLISH2 = model.OwnerPersonContact.NAME_CHINESE2;
                owner.EMAIL = model.OwnerPersonContact.EMAIL;
                owner.CONTACT_NO = model.OwnerPersonContact.CONTACT_NO;
                owner.FAX_NO = model.OwnerPersonContact.FAX_NO;
                owner.ID_TYPE = model.OwnerPersonContact.ID_TYPE;
                owner.OTHER_ID_TYPE = model.OwnerPersonContact.OTHER_ID_TYPE;
                owner.ID_NUMBER = model.OwnerPersonContact.ID_NUMBER;
                owner.ID_ISSUE_COUNTRY = model.OwnerPersonContact.ID_ISSUE_COUNTRY;
                owner.CONTACT_PERSON_TITLE = model.OwnerPersonContact.CONTACT_PERSON_TITLE;
                owner.FIRST_NAME = model.OwnerPersonContact.FIRST_NAME;
                owner.LAST_NAME = model.OwnerPersonContact.LAST_NAME;
                owner.MOBILE = model.OwnerPersonContact.MOBILE;
                owner.ENDORSEMENT_DATE = model.OwnerPersonContact.ENDORSEMENT_DATE;
            }

            if (oi != null && model.OIPersonContact != null)
            {
                oi.NAME_CHINESE = model.OIPersonContact.NAME_CHINESE;
                oi.NAME_ENGLISH = model.OIPersonContact.NAME_ENGLISH;
                oi.NAME_CHINESE2 = model.OIPersonContact.NAME_CHINESE2;
                oi.NAME_ENGLISH2 = model.OIPersonContact.NAME_ENGLISH2;
                oi.EMAIL = model.OIPersonContact.EMAIL;
                oi.CONTACT_NO = model.OIPersonContact.CONTACT_NO;
                oi.FAX_NO = model.OIPersonContact.FAX_NO;
                oi.ID_TYPE = model.OIPersonContact.ID_TYPE;
                oi.OTHER_ID_TYPE = model.OIPersonContact.OTHER_ID_TYPE;
                oi.ID_NUMBER = model.OIPersonContact.ID_NUMBER;
                oi.ID_ISSUE_COUNTRY = model.OIPersonContact.ID_ISSUE_COUNTRY;
                oi.CONTACT_PERSON_TITLE = model.OIPersonContact.CONTACT_PERSON_TITLE;
                oi.FIRST_NAME = model.OIPersonContact.FIRST_NAME;
                oi.LAST_NAME = model.OIPersonContact.LAST_NAME;
                oi.MOBILE = model.OIPersonContact.MOBILE;
                oi.ENDORSEMENT_DATE = model.OIPersonContact.ENDORSEMENT_DATE;
            }

            if (signboard != null && model.SignBoardPersonContact != null)
            {
                signboard.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;
                signboard.NAME_CHINESE2 = model.SignBoardPersonContact.NAME_CHINESE2;
                signboard.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                signboard.NAME_ENGLISH2 = model.SignBoardPersonContact.NAME_ENGLISH2;
                signboard.EMAIL = model.SignBoardPersonContact.EMAIL;
                signboard.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                signboard.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                signboard.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                signboard.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                signboard.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                signboard.CONTACT_PERSON_TITLE = model.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                signboard.FIRST_NAME = model.SignBoardPersonContact.FIRST_NAME;
                signboard.LAST_NAME = model.SignBoardPersonContact.LAST_NAME;
                signboard.MOBILE = model.SignBoardPersonContact.MOBILE;
                signboard.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;
            }

            //Professional
            P_MW_APPOINTED_PROFESSIONAL secProf_PRC = model.P_MW_APPOINTED_PROFESSIONALs[3];

            db.Entry<P_MW_APPOINTED_PROFESSIONAL>(secProf_PRC).State = EntityState.Modified;

            //MW ITEM

        }

        private void UpdateSignalPersonContact(string formCode, Fn02MWUR_DeFormModel model, P_MW_PERSON_CONTACT signboardPersonContact, EntitiesMWProcessing db)
        {
            if (formCode == ProcessingConstant.FORM_02
                                    || formCode == ProcessingConstant.FORM_04
                                    || formCode == ProcessingConstant.FORM_05
                                    || formCode == ProcessingConstant.FORM_11
                                    || formCode == ProcessingConstant.FORM_12)
            {
                if (string.IsNullOrWhiteSpace(model.SignBoardPersonContact.NAME_ENGLISH)
                    && string.IsNullOrWhiteSpace(model.SignBoardPersonContact.NAME_CHINESE))
                {
                    signboardPersonContact.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                    signboardPersonContact.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;

                    signboardPersonContact.EMAIL = model.SignBoardPersonContact.EMAIL;
                    signboardPersonContact.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                    signboardPersonContact.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                    signboardPersonContact.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                    signboardPersonContact.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                    signboardPersonContact.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                    signboardPersonContact.CONTACT_PERSON_TITLE = model.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                    signboardPersonContact.FIRST_NAME = model.SignBoardPersonContact.FIRST_NAME;
                    signboardPersonContact.LAST_NAME = model.SignBoardPersonContact.LAST_NAME;
                    signboardPersonContact.MOBILE = model.SignBoardPersonContact.MOBILE;
                    signboardPersonContact.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;

                    db.Entry<P_MW_ADDRESS>(model.SignBoardAddress).State = EntityState.Modified;
                }
                else
                {
                    signboardPersonContact.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH2;
                    signboardPersonContact.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE2;
                    signboardPersonContact.NAME_CHINESE2 = model.SignBoardPersonContact.NAME_CHINESE2;
                    signboardPersonContact.NAME_ENGLISH2 = model.SignBoardPersonContact.NAME_ENGLISH2;
                }
            }
            else if (formCode == ProcessingConstant.FORM_05)
            {
                signboardPersonContact.NAME_ENGLISH = model.SignBoardPersonContact.NAME_ENGLISH;
                signboardPersonContact.NAME_CHINESE = model.SignBoardPersonContact.NAME_CHINESE;
                signboardPersonContact.NAME_CHINESE2 = model.SignBoardPersonContact.NAME_CHINESE2;
                signboardPersonContact.NAME_ENGLISH2 = model.SignBoardPersonContact.NAME_ENGLISH2;

                signboardPersonContact.EMAIL = model.SignBoardPersonContact.EMAIL;
                signboardPersonContact.CONTACT_NO = model.SignBoardPersonContact.CONTACT_NO;
                signboardPersonContact.FAX_NO = model.SignBoardPersonContact.FAX_NO;
                signboardPersonContact.ID_TYPE = model.SignBoardPersonContact.ID_TYPE;
                signboardPersonContact.ID_NUMBER = model.SignBoardPersonContact.ID_NUMBER;
                signboardPersonContact.ID_ISSUE_COUNTRY = model.SignBoardPersonContact.ID_ISSUE_COUNTRY;
                signboardPersonContact.CONTACT_PERSON_TITLE = model.SignBoardPersonContact.CONTACT_PERSON_TITLE;
                signboardPersonContact.FIRST_NAME = model.SignBoardPersonContact.FIRST_NAME;
                signboardPersonContact.LAST_NAME = model.SignBoardPersonContact.LAST_NAME;
                signboardPersonContact.MOBILE = model.SignBoardPersonContact.MOBILE;
                signboardPersonContact.ENDORSEMENT_DATE = model.SignBoardPersonContact.ENDORSEMENT_DATE;

                db.Entry<P_MW_ADDRESS>(model.SignBoardAddress).State = EntityState.Modified;
            }
        }
        public void SetFinalRecordBeforeCreate(P_MW_RECORD finalRecord, P_MW_RECORD secondRecord)
        {
            finalRecord.COMMENCEMENT_DATE = secondRecord.COMMENCEMENT_DATE;
            finalRecord.COMPLETION_DATE = secondRecord.COMPLETION_DATE;
            finalRecord.COMMENCEMENT_SUBMISSION_DATE = secondRecord.COMMENCEMENT_SUBMISSION_DATE;
            finalRecord.S_FORM_TYPE_CODE = secondRecord.S_FORM_TYPE_CODE;
            finalRecord.CLASS_CODE = secondRecord.CLASS_CODE;
            finalRecord.MW_PROGRESS_STATUS_CODE = secondRecord.MW_PROGRESS_STATUS_CODE;
            finalRecord.LOCATION_OF_MINOR_WORK = secondRecord.LOCATION_OF_MINOR_WORK;
            finalRecord.FILE_REFERENCE = secondRecord.FILE_REFERENCE;
            finalRecord.SUBMIT_TYPE = ProcessingConstant.MW_SUBMISSION;
            finalRecord.VERIFICATION_SPO = secondRecord.VERIFICATION_SPO;
            finalRecord.PERMIT_NO = secondRecord.PERMIT_NO;
            finalRecord.FORM_VERSION = secondRecord.FORM_VERSION;
            finalRecord.NO_OF_PREMISES = secondRecord.NO_OF_PREMISES;
            finalRecord.FILEREF_FOUR = secondRecord.FILEREF_FOUR;
            finalRecord.FILEREF_TWO = secondRecord.FILEREF_TWO;
            finalRecord.AUDIT_RELATED = secondRecord.AUDIT_RELATED;
            finalRecord.IMPORT_FORM_TYPE = secondRecord.IMPORT_FORM_TYPE;
            finalRecord.EFSS_REF_NO = secondRecord.EFSS_REF_NO;
            finalRecord.SITE_AUDIT_RELATED = secondRecord.SITE_AUDIT_RELATED;
            finalRecord.PRE_SITE_AUDIT_RELATED = secondRecord.PRE_SITE_AUDIT_RELATED;
        }

        public ServiceResult SaveAsDraft(Fn02MWUR_DeFormModel model, Boolean isSubmit)
        {

            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Get Record Item
                        model.P_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();
                        if (model.P_MW_RECORD_ITEMs_CLASS_I != null)
                        {
                            model.P_MW_RECORD_ITEMs_CLASS_I.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_I);
                            model.P_MW_RECORD_ITEMs.AddRange(model.P_MW_RECORD_ITEMs_CLASS_I.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                        }
                        if (model.P_MW_RECORD_ITEMs_CLASS_II != null)
                        {
                            model.P_MW_RECORD_ITEMs_CLASS_II.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_II);
                            model.P_MW_RECORD_ITEMs.AddRange(model.P_MW_RECORD_ITEMs_CLASS_II.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                        }
                        if (model.P_MW_RECORD_ITEMs_CLASS_III != null)
                        {
                            model.P_MW_RECORD_ITEMs_CLASS_III.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_III);
                            model.P_MW_RECORD_ITEMs.AddRange(model.P_MW_RECORD_ITEMs_CLASS_III.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                        }

                        //Set SAC
                        if (isSubmit)
                        {
                            SetMwRecordSacValue(model.P_MW_RECORD_ITEMs, model.P_MW_RECORD);
                        }
                        

                        // clone finalModel
                        //Fn02MWUR_DeFormModel finalModel = (Fn02MWUR_DeFormModel)model.Clone();
                        //Fn02MWUR_DeFormModel finalModel = BaseCommonService.DeepCopy<Fn02MWUR_DeFormModel>(model);
                        Fn02MWUR_DeFormModel finalModel = JsonConvert.DeserializeObject<Fn02MWUR_DeFormModel>(JsonConvert.SerializeObject(model));

                        SaveDataEntryRecord(model, false, db);
                        // if DataEntry Submit, create/update final record 
                        if (isSubmit)
                        {
                            P_MW_RECORD mwRecordFinal = null;

                            // get final record by Mw Reference No.
                            mwRecordFinal = GetFinalMwRecord(model.P_MW_RECORD.REFERENCE_NUMBER);

                            if (!IsUpdateRecordFinal(model) && mwRecordFinal == null)
                            {
                                if (finalModel.OIAddress != null)
                                {
                                    finalModel.OIAddress.UUID = "";
                                    finalModel.OIPersonContact.UUID = "";
                                }
                                if (finalModel.SignBoardAddress != null)
                                {
                                    finalModel.SignBoardAddress.UUID = "";
                                    finalModel.SignBoardPersonContact.UUID = "";
                                }
                                if (finalModel.OwnerAddress != null)
                                {
                                    finalModel.OwnerAddress.UUID = "";
                                    finalModel.OwnerPersonContact.UUID = "";
                                }
                                if (finalModel.MWAddress != null)
                                {
                                    finalModel.MWAddress.UUID = "";
                                }
                                finalModel.P_MW_RECORD.UUID = "";

                                P_MW_RECORD secondRecord = db.P_MW_RECORD.Where(d => d.UUID == model.P_MW_RECORD.UUID).FirstOrDefault();
                                SetFinalRecordBeforeCreate(finalModel.P_MW_RECORD, secondRecord);
                                SaveDataEntryRecord(finalModel, true, db);

                                //// Create a new final record
                                //finalModel.DictID = new Dictionary<string, object>();

                                ////Save The Address First , Then Person Contact
                                ////Save OI Info
                                ////finalModel.OIPersonContact.UUID = null;
                                //SaveOIInfo(finalModel, true, db);

                                ////Save SignBoard Info
                                //SaveSignBoardInfo(finalModel, true, db);

                                ////Save Owner Info
                                //SaveOwnerInfo(finalModel, true, db);

                                ////Save MWAddress
                                //SaveMWAddress(finalModel, true, db);

                                ////Save P_MW_RECORD
                                //finalModel.P_MW_RECORD.UUID = "";
                                //SaveP_MW_RECORD(finalModel, true, db);

                                ////Save  P_MW_RECORD_ITEMs
                                //finalModel.P_MW_RECORD_ITEMs = new List<P_MW_RECORD_ITEM>();
                                //if (finalModel.P_MW_RECORD_ITEMs_CLASS_I != null)
                                //{
                                //    finalModel.P_MW_RECORD_ITEMs_CLASS_I.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_I);
                                //    finalModel.P_MW_RECORD_ITEMs.AddRange(finalModel.P_MW_RECORD_ITEMs_CLASS_I.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                                //}
                                //if (finalModel.P_MW_RECORD_ITEMs_CLASS_II != null)
                                //{
                                //    finalModel.P_MW_RECORD_ITEMs_CLASS_II.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_II);
                                //    finalModel.P_MW_RECORD_ITEMs.AddRange(finalModel.P_MW_RECORD_ITEMs_CLASS_II.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                                //}
                                //if (finalModel.P_MW_RECORD_ITEMs_CLASS_III != null)
                                //{
                                //    finalModel.P_MW_RECORD_ITEMs_CLASS_III.ForEach(i => i.CLASS_CODE = ProcessingConstant.DB_CLASS_III);
                                //    finalModel.P_MW_RECORD_ITEMs.AddRange(finalModel.P_MW_RECORD_ITEMs_CLASS_III.Where(w => !string.IsNullOrEmpty(w.MW_ITEM_CODE)));
                                //}
                                //finalModel.P_MW_RECORD_ITEMs.ForEach(d => d.MW_RECORD_ID = finalModel.P_MW_RECORD.UUID);
                                //SaveP_MW_RECORD_ITEMs(finalModel, db);

                                ////Save P_MW_APPOINTED_PROFESSIONAL
                                //finalModel.P_MW_APPOINTED_PROFESSIONALs.ForEach(d => d.MW_RECORD_ID = finalModel.P_MW_RECORD.UUID);
                                //SaveP_MW_APPOINTED_PROFESSIONALs(finalModel, db);

                                ////SaveP_MW_FORM_09s
                                //SaveP_MW_FORM_09s(finalModel, db);

                                //string formCode = model.P_MW_RECORD.S_FORM_TYPE_CODE;
                                //P_MW_RECORD finalMwRecord = model.P_MW_RECORD;
                                //P_MW_FORM finalMwForm = model.P_MW_FORM;
                                //P_MW_PERSON_CONTACT finalOwner = model.OwnerPersonContact;
                                //P_MW_PERSON_CONTACT finalOi = model.OIPersonContact;
                                //P_MW_PERSON_CONTACT finalsignboard = model.SignBoardPersonContact;
                                //List<P_MW_APPOINTED_PROFESSIONAL> finalProfessionals = model.P_MW_APPOINTED_PROFESSIONALs;
                                //DateTime dsnSubmissionDate = model.P_MW_DSN.CREATED_DATE;
                                //string sspSubmitted = model.P_MW_DSN.SSP_SUBMITTED;

                                //if (formCode == ProcessingConstant.FORM_MW01)
                                //{
                                //    //UpdateForm01(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, sspSubmitted, db);
                                //    UpdateForm01(finalModel, model, sspSubmitted, db);
                                //}
                                //else if (formCode == ProcessingConstant.FORM_03)
                                //{
                                //    UpdateForm03(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
                                //}
                                //else if (formCode == ProcessingConstant.FORM_05 || formCode == ProcessingConstant.FORM_05_36)
                                //{
                                //    UpdateForm05(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
                                //}
                                //else if (formCode == ProcessingConstant.FORM_06)
                                //{
                                //    UpdateForm06(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
                                //}
                                //else if (formCode == ProcessingConstant.FORM_32)
                                //{
                                //    UpdateForm32(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
                                //}
                            }
                            else if (IsUpdateRecordFinal(model) && mwRecordFinal == null)
                            {
                                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
                                errors.Add("ALERT", new List<string>()
                                {
                                    "Parent submission not found"
                                });
                                return new ServiceResult()
                                {
                                    Result = ServiceResult.RESULT_FAILURE
                                    ,
                                    ErrorMessages = errors
                                };
                            }
                            else
                            {
                                // Update final record
                                // *** Logic refer to other document ***
                                //UpdateFinalRecord(finalModel, mwRecordFinal, db);

                            }

                            // Update MwDsn
                            //P_MW_DSN.SCANNED_STATUS_ID = ProcessingConstant.SECOND_ENTRY_COMPLETED;
                            P_MW_DSN p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == model.P_MW_RECORD.MW_DSN).FirstOrDefault();
                            p_MW_DSN.SCANNED_STATUS_ID = GetStatusUUIDByCode("SECOND_ENTRY_COMPLETED");

                            // Update MwRecord
                            // 找到P_MW_RECORD, update status from MW_SECOND_ENTRY to SECOND_ENTRY_COMPLETED
                            P_MW_RECORD p_MW_RECORD = db.P_MW_RECORD.Where(m => m.UUID == model.P_MW_RECORD.UUID && m.STATUS_CODE == ProcessingConstant.MW_SECOND_ENTRY).FirstOrDefault();

                            if (p_MW_RECORD != null)
                            {
                                p_MW_RECORD.STATUS_CODE = ProcessingConstant.MW_SECOND_COMPLETE;
                            }
                            //Check Signbarod Item 

                            ProcessingSystemValueDAOService pSystemValueDaoService = new ProcessingSystemValueDAOService();
                            P_S_SYSTEM_VALUE svSignBoardItem = pSystemValueDaoService.GetSSystemValueByTypeAndCode(ProcessingConstant.TYPE_S_MW_ITEM,
                                ProcessingConstant.CODE_SIGNBOARD_ITEMS);

                            string[] SignboardItemList = svSignBoardItem.DESCRIPTION.Split(',');

                            bool isMWItem = false;
                            bool isSignBoradItem = false;
                            for (int i = 0; i < model.P_MW_RECORD_ITEMs.Count(); i++)
                            {
                                if (SignboardItemList.Contains(model.P_MW_RECORD_ITEMs[i].MW_ITEM_CODE))
                                {
                                    isSignBoradItem = true;
                                }
                                else
                                {
                                    isMWItem = true;
                                }
                            }

                            if (!isMWItem && !isSignBoradItem)
                            {
                                isMWItem = true;
                            }

                            // Create MwVerification
                            // P_MW_VERIFICATION mwVerification = new P_MW_VERIFICATION();
                            // mwVerification.MW_RECORD_ID = XXXXXXXXX
                            // mwVerification.STATUS_CODE = 'MW_VERT_STATUS_OPEN';

                            if (isMWItem)
                            {
                                P_MW_VERIFICATION mwVerification = new P_MW_VERIFICATION();
                                mwVerification.MW_RECORD_ID = p_MW_RECORD.UUID;
                                mwVerification.STATUS_CODE = ProcessingConstant.MW_VERT_STATUS_OPEN;
                                mwVerification.HANDLING_UNIT = ProcessingConstant.HANDLING_UNIT_PEM;
                                db.P_MW_VERIFICATION.Add(mwVerification);
                            }
                            if (isSignBoradItem)
                            {
                                P_MW_VERIFICATION mwVerification = new P_MW_VERIFICATION();
                                mwVerification.MW_RECORD_ID = p_MW_RECORD.UUID;
                                mwVerification.STATUS_CODE = ProcessingConstant.MW_VERT_STATUS_OPEN;
                                mwVerification.HANDLING_UNIT = ProcessingConstant.HANDLING_UNIT_SMM;
                                db.P_MW_VERIFICATION.Add(mwVerification);
                            }
                            ProcessingWorkFlowManagementService.Instance.StartWorkFlowSubmission(
                                 db, p_MW_RECORD.MW_DSN,
                                 ("Y" == p_MW_RECORD.VERIFICATION_SPO ? true : false), isMWItem, isSignBoradItem);
                        }
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (ProcessingWorkFlowUserNotFoundException ex)
                    {
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE,
                            ErrorMessages = new Dictionary<string, List<string>>()
                            {
                                ["ALERT"] = new List<string>() { "Handling officer not found, plesae contact you admin." }
                            }
                        };
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        tran.Rollback();
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = new Dictionary<string, List<string>>() { ["ALERT"] = new List<string>() { validationError.Message } } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = new Dictionary<string, List<string>>() { ["ALERT"] = new List<string>() { ex.Message } } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }

        private static bool IsUpdateRecordFinal(Fn02MWUR_DeFormModel model)
        {
            List<String> autoGenMwFormCodeList = ProcessingConstant.autoGenMwFormCodeList();
            if (autoGenMwFormCodeList.Contains(model.P_MW_RECORD.S_FORM_TYPE_CODE))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SaveDataEntryRecord(Fn02MWUR_DeFormModel model, bool isSubmit, EntitiesMWProcessing db)
        {
            // Save dataEntry record
            model.DictID = new Dictionary<string, object>();

            //Save The Address First , Then Person Contact
            //Save OI Info
            SaveOIInfo(model, isSubmit, db);

            //Save SignBoard Info
            SaveSignBoardInfo(model, isSubmit, db);

            //Save Owner Info
            SaveOwnerInfo(model, isSubmit, db);

            //Save MWAddress
            SaveMWAddress(model, isSubmit, db);

            //Save P_MW_RECORD
            SaveP_MW_RECORD(model, isSubmit, db);

            //Save  P_MW_RECORD_ITEMs
            model.P_MW_RECORD_ITEMs.ForEach(d => { d.MW_RECORD_ID = model.P_MW_RECORD.UUID; d.STATUS_CODE = model.P_MW_RECORD.S_FORM_TYPE_CODE; });


            if (isSubmit)
            {
                //Save final record item status code is form code
                List<P_MW_RECORD_ITEM> finalFormCodeRecordItems = JsonConvert.DeserializeObject<List<P_MW_RECORD_ITEM>>(JsonConvert.SerializeObject(model.P_MW_RECORD_ITEMs));
                finalFormCodeRecordItems.ForEach(d =>
                {
                    d.UUID = "";
                    d.LAST_MODIFIED_FORM_CODE = d.STATUS_CODE;
                });
                Fn02MWUR_DeFormModel finalFormCodeModel = new Fn02MWUR_DeFormModel()
                {
                    P_MW_RECORD_ITEMs = finalFormCodeRecordItems
                };
                SaveP_MW_RECORD_ITEMs(finalFormCodeModel, db);

                //Save final record item statis code is FINAL
                List<P_MW_RECORD_ITEM> finalRecordItems = JsonConvert.DeserializeObject<List<P_MW_RECORD_ITEM>>(JsonConvert.SerializeObject(model.P_MW_RECORD_ITEMs));
                finalRecordItems.ForEach(d =>
                {
                    d.UUID = "";
                    d.LAST_MODIFIED_FORM_CODE = d.STATUS_CODE;
                    d.STATUS_CODE = "FINAL";
                });
                Fn02MWUR_DeFormModel finalModel = new Fn02MWUR_DeFormModel()
                {
                    P_MW_RECORD_ITEMs = finalRecordItems
                };
                SaveP_MW_RECORD_ITEMs(finalModel, db);
            }
            else
            {
                SaveP_MW_RECORD_ITEMs(model, db);
            }

            // Save P_MW_APPOINTED_PROFESSIONAL

            if (isSubmit)
            {
                model.P_MW_APPOINTED_PROFESSIONALs = GetFinalAppointedProfessional(model.P_MW_APPOINTED_PROFESSIONALs, model.P_MW_RECORD.S_FORM_TYPE_CODE);
            }

            model.P_MW_APPOINTED_PROFESSIONALs.ForEach(d => d.MW_RECORD_ID = model.P_MW_RECORD.UUID);
            SaveP_MW_APPOINTED_PROFESSIONALs(model, db);

            // SaveP_MW_FORM_09s
            SaveP_MW_FORM_09s(model, db);

            //SaveP_MW_FORM
            SaveP_MW_FORM(model, db);

            db.SaveChanges();
        }

        private void SetMwRecordSacValue(List<P_MW_RECORD_ITEM> p_MW_RECORD_ITEMs , P_MW_RECORD p_MW_RECORD)
        {
            //get random item list 
            List<P_S_SYSTEM_VALUE> sacRandomList = SystemValueDA.GetSSystemValueByType(ProcessingConstant.SAC_ITEM_LIST);

            bool isMW = false;
            bool isSB = false;
            bool isRandomSAC = false;
            string randomSacResult = ProcessingConstant.FLAG_N;

            foreach (var item in p_MW_RECORD_ITEMs)
            {
                P_S_SYSTEM_VALUE itemValue = sacRandomList.Where(w => w.CODE.Trim() == item.MW_ITEM_CODE.Trim()).FirstOrDefault();
                if (itemValue != null)
                {
                    if (ProcessingConstant.SAC_ITEM_MW.Equals(itemValue.PARENT_ID))
                    {
                        isMW = true;
                    }
                    else if (ProcessingConstant.SAC_ITEM_SB.Equals(itemValue.PARENT_ID))
                    {
                        isSB = true;
                    }
                }
            }

            isRandomSAC = (isMW || isSB);
            if (isRandomSAC)
            {
                randomSacResult = randomSAC();
            }

            if (ProcessingConstant.FLAG_Y.Equals(randomSacResult))
            {
                if (isMW && isSB)
                {
                    p_MW_RECORD.SITE_AUDIT_RELATED = ProcessingConstant.SAC_MW_AND_SB;
                    p_MW_RECORD.SITE_AUDIT_RELATED_MW = ProcessingConstant.FLAG_Y;
                    p_MW_RECORD.SITE_AUDIT_RELATED_SB = ProcessingConstant.FLAG_Y;
                }
                else if (isMW)
                {
                    p_MW_RECORD.SITE_AUDIT_RELATED = ProcessingConstant.SAC_MW;
                    p_MW_RECORD.SITE_AUDIT_RELATED_MW = ProcessingConstant.FLAG_Y;
                    p_MW_RECORD.SITE_AUDIT_RELATED_SB = ProcessingConstant.FLAG_N;
                }
                else if (isSB)
                {
                    p_MW_RECORD.SITE_AUDIT_RELATED = ProcessingConstant.SAC_SB;
                    p_MW_RECORD.SITE_AUDIT_RELATED_MW = ProcessingConstant.FLAG_N;
                    p_MW_RECORD.SITE_AUDIT_RELATED_SB = ProcessingConstant.FLAG_Y;
                }
            }
            else
            {
                p_MW_RECORD.SITE_AUDIT_RELATED = ProcessingConstant.FLAG_N;
                p_MW_RECORD.SITE_AUDIT_RELATED_MW = ProcessingConstant.FLAG_N;
                p_MW_RECORD.SITE_AUDIT_RELATED_SB = ProcessingConstant.FLAG_N;
            }
        }

        private string randomSAC()
        {
            // default result
            string SACResult = ProcessingConstant.FLAG_N;

            // generate random no
            int randomNo = generateRandomNo();
            int percentage = Convert.ToInt32(SystemValueDA.GetSSystemValueByTypeAndCode("ACK_LETTER_AUDIT_PERCENTAGE", "ACK_LETTER_AUDIT_PERCENTAGE_SAC").ORDERING);
            if (percentage == 100 || percentage > randomNo)
            {
                SACResult = ProcessingConstant.FLAG_Y;
            }
            else
            {
                SACResult = ProcessingConstant.FLAG_N;
            }

            return SACResult;
        }

        private int generateRandomNo()
        {
            Random random = new Random();
            return random.Next(0, 100);
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

        public void UpdateFinalRecord(Fn02MWUR_DeFormModel finalModel, P_MW_RECORD mwRecordFinal, EntitiesMWProcessing db)
        {
            // Update final record
            // *** Logic refer to other document ***
            DateTime dsnSubmissionDate = db.P_MW_DSN.Where(m => m.DSN == finalModel.P_MW_RECORD.MW_DSN).FirstOrDefault().CREATED_DATE;

            // Get final record
            P_MW_RECORD finalMwRecord = db.P_MW_RECORD.Where(d => d.UUID == mwRecordFinal.UUID).FirstOrDefault();
            string formCode = finalMwRecord.S_FORM_TYPE_CODE;
            P_MW_FORM finalMwForm = db.P_MW_FORM.Where(m => m.MW_RECORD_ID == mwRecordFinal.UUID).FirstOrDefault();

            // Get final person contact
            P_MW_PERSON_CONTACT finalOwner = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == mwRecordFinal.OWNER_ID).FirstOrDefault();
            P_MW_PERSON_CONTACT finalOi = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == mwRecordFinal.OI_ID).FirstOrDefault();
            P_MW_PERSON_CONTACT finalsignboard = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == mwRecordFinal.SIGNBOARD_PERFROMER_ID).FirstOrDefault();

            // Get final person address
            //P_MW_ADDRESS finalOwnerAddress = db.P_MW_ADDRESS.Where(m => m.UUID == finalOwner.MW_ADDRESS_ID).FirstOrDefault();
            //finalModel.OwnerAddress = new P_MW_ADDRESS();
            //finalModel.OwnerAddress.UUID = finalOwnerAddress.UUID;
            ////finalModel.OwnerAddress = finalOwnerAddress;
            //P_MW_ADDRESS finalOiAddress = db.P_MW_ADDRESS.Where(m => m.UUID == finalOi.MW_ADDRESS_ID).FirstOrDefault();
            ////finalModel.OIAddress = new P_MW_ADDRESS();
            ////finalModel.OIAddress.UUID = finalOiAddress.UUID;
            //finalModel.OIAddress = finalOiAddress;
            //P_MW_ADDRESS finalSignboardAddress = db.P_MW_ADDRESS.Where(m => m.UUID == finalsignboard.MW_ADDRESS_ID).FirstOrDefault();
            ////finalModel.SignBoardAddress = new P_MW_ADDRESS();
            ////finalModel.SignBoardAddress.UUID = finalSignboardAddress.UUID;
            //finalModel.SignBoardAddress = finalSignboardAddress;
            //P_MW_ADDRESS finalMwAddress = db.P_MW_ADDRESS.Where(m => m.UUID == finalMwRecord.LOCATION_ADDRESS_ID).FirstOrDefault();
            ////finalModel.MWAddress = new P_MW_ADDRESS();
            ////finalModel.MWAddress.UUID = finalMwAddress.UUID;
            //finalModel.MWAddress = finalMwAddress;

            // Get final professional
            P_MW_APPOINTED_PROFESSIONAL finalprofessional_AP = finalMwRecord.P_MW_APPOINTED_PROFESSIONAL.FirstOrDefault();
            if (finalprofessional_AP != null) finalModel.P_MW_APPOINTED_PROFESSIONALs[0].UUID = finalprofessional_AP.UUID;
            P_MW_APPOINTED_PROFESSIONAL finalprofessional_RSE = finalMwRecord.P_MW_APPOINTED_PROFESSIONAL.Skip(1).Take(1).FirstOrDefault();
            if (finalprofessional_RSE != null) finalModel.P_MW_APPOINTED_PROFESSIONALs[1].UUID = finalprofessional_RSE.UUID;
            P_MW_APPOINTED_PROFESSIONAL finalprofessional_RGE = finalMwRecord.P_MW_APPOINTED_PROFESSIONAL.Skip(2).Take(1).FirstOrDefault();
            if (finalprofessional_RGE != null) finalModel.P_MW_APPOINTED_PROFESSIONALs[2].UUID = finalprofessional_RGE.UUID;
            P_MW_APPOINTED_PROFESSIONAL finalprofessional_PRC = finalMwRecord.P_MW_APPOINTED_PROFESSIONAL.Skip(3).Take(1).FirstOrDefault();
            if (finalprofessional_PRC != null) finalModel.P_MW_APPOINTED_PROFESSIONALs[3].UUID = finalprofessional_PRC.UUID;

            // For fileRefNo 4 + 2
            finalMwRecord.FILEREF_FOUR = finalModel.P_MW_RECORD.FILEREF_FOUR;
            finalMwRecord.FILEREF_TWO = finalModel.P_MW_RECORD.FILEREF_TWO;
            finalMwRecord.EFSS_REF_NO = finalModel.P_MW_RECORD.EFSS_REF_NO;
            finalMwRecord.SUBMIT_TYPE = finalModel.P_MW_RECORD.SUBMIT_TYPE;
            finalMwRecord.P_MW_REFERENCE_NO = finalModel.P_MW_RECORD.P_MW_REFERENCE_NO;
            finalMwRecord.S_FORM_TYPE_CODE = finalModel.P_MW_RECORD.S_FORM_TYPE_CODE;
            finalMwRecord.LANGUAGE_CODE = finalModel.P_MW_RECORD.LANGUAGE_CODE;
            finalMwRecord.IS_DATA_ENTRY = ProcessingConstant.FLAG_N;
            finalMwRecord.PERMIT_NO = finalModel.P_MW_RECORD.PERMIT_NO;

            finalMwRecord.MW_DSN = finalModel.P_MW_RECORD.MW_DSN;
            finalMwRecord.FORM_VERSION = finalModel.P_MW_RECORD.FORM_VERSION;

            finalMwRecord.MW05_ITEM36 = finalModel.P_MW_RECORD.MW05_ITEM36;
            finalMwRecord.NO_OF_PREMISES = finalModel.P_MW_RECORD.NO_OF_PREMISES;

            if (finalprofessional_AP != null) finalprofessional_AP.STATUS_ID = ProcessingConstant.MW_APPOINTED_PROFESSIONAL_STATUS_OLD;
            if (finalprofessional_RSE != null) finalprofessional_RSE.STATUS_ID = ProcessingConstant.MW_APPOINTED_PROFESSIONAL_STATUS_OLD;
            if (finalprofessional_RGE != null) finalprofessional_RGE.STATUS_ID = ProcessingConstant.MW_APPOINTED_PROFESSIONAL_STATUS_OLD;
            if (finalprofessional_PRC != null) finalprofessional_PRC.STATUS_ID = ProcessingConstant.MW_APPOINTED_PROFESSIONAL_STATUS_OLD;

            //item list
            List<P_MW_RECORD_ITEM> finalRecordItemList = new List<P_MW_RECORD_ITEM>();
            if (finalModel.P_MW_RECORD_ITEMs != null)
            {
                for (int i = 0; i < finalModel.P_MW_RECORD_ITEMs.Count(); i++)
                {
                    P_MW_RECORD_ITEM input = finalModel.P_MW_RECORD_ITEMs[i];
                    if (finalModel.P_MW_RECORD_ITEMs[i].MW_ITEM_CODE != "")
                    {
                        P_MW_RECORD_ITEM item = JsonConvert.DeserializeObject<P_MW_RECORD_ITEM>(JsonConvert.SerializeObject(input));
                        item.STATUS_CODE = finalModel.P_MW_RECORD.S_FORM_TYPE_CODE;
                        finalRecordItemList.Add(item);

                        P_MW_RECORD_ITEM itemFinal = JsonConvert.DeserializeObject<P_MW_RECORD_ITEM>(JsonConvert.SerializeObject(input));
                        itemFinal.STATUS_CODE = ProcessingConstant.MW_RECORD_ITEM_STATUS_FINAL;
                        itemFinal.LAST_MODIFIED_FORM_CODE = finalModel.P_MW_RECORD.S_FORM_TYPE_CODE;
                        finalRecordItemList.Add(itemFinal);
                        //finalModel.P_MW_RECORD_ITEMs[i].STATUS_CODE = model.P_MW_RECORD.S_FORM_TYPE_CODE;
                    }
                }
            }

            db.P_MW_RECORD_ITEM.AddRange(finalRecordItemList);
            db.SaveChanges();

            //MWForm09
            List<P_MW_FORM_09> finalMwForm09List = new List<P_MW_FORM_09>();
            if (finalModel.P_MW_FORM_09s != null)
            {
                for (int i = 0; i < finalModel.P_MW_FORM_09s.Count(); i++)
                {
                    P_MW_FORM_09 item = new P_MW_FORM_09();
                    item = finalModel.P_MW_FORM_09s[i];
                    finalMwForm09List.Add(item);
                }
            }

            db.P_MW_FORM_09.AddRange(finalMwForm09List);
            db.SaveChanges();

            string sspSubmitted = db.P_MW_DSN.Where(m => m.DSN == finalModel.P_MW_RECORD.MW_DSN).FirstOrDefault().SSP_SUBMITTED;
            finalMwRecord.VERIFICATION_SPO = ProcessingConstant.FLAG_N;

            if (finalModel.P_MW_FORM != null)
            {
                finalMwForm.RECEIVED_DATE = finalModel.P_MW_FORM.RECEIVED_DATE;
                finalMwForm.AS_ABOVE = finalModel.P_MW_FORM.AS_ABOVE;
                finalMwForm.MW_SUBMISSION_NO = finalModel.P_MW_FORM.MW_SUBMISSION_NO;
                finalMwForm.INVOLVE_SIGNBOARD = finalModel.P_MW_FORM.INVOLVE_SIGNBOARD;
                finalMwForm.NOT_INVOLVE_SIGNBOARD = finalModel.P_MW_FORM.NOT_INVOLVE_SIGNBOARD;
            }

            Fn02MWUR_DeFormModel newModel = new Fn02MWUR_DeFormModel();
            newModel.P_MW_RECORD = finalMwRecord;
            newModel.P_MW_FORM = finalMwForm;
            newModel.OwnerPersonContact = finalOwner;
            newModel.OIPersonContact = finalOi;
            newModel.SignBoardPersonContact = finalsignboard;
            newModel.P_MW_APPOINTED_PROFESSIONALs = finalMwRecord.P_MW_APPOINTED_PROFESSIONAL.ToList();
            newModel.P_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == finalMwRecord.MW_DSN).FirstOrDefault();

            if (formCode == ProcessingConstant.FORM_MW01)
            {
                //UpdateForm01(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, sspSubmitted, db);
                UpdateForm01(finalModel, newModel, sspSubmitted, db);
            }
            else if (formCode == ProcessingConstant.FORM_03)
            {
                UpdateForm03(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
            }
            else if (formCode == ProcessingConstant.FORM_05 || formCode == ProcessingConstant.FORM_05_36)
            {
                UpdateForm05(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
            }
            else if (formCode == ProcessingConstant.FORM_06)
            {
                UpdateForm06(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
            }
            else if (formCode == ProcessingConstant.FORM_32)
            {
                UpdateForm32(finalModel, finalMwRecord, finalMwForm, finalOwner, finalOi, finalsignboard, dsnSubmissionDate, db);
            }

            UpdateSignalPersonContact(formCode, finalModel, finalsignboard, db);

            P_MW_APPOINTED_PROF_HISTORY professionalHist_AP = SetPMWAppointedProfHistory(finalprofessional_AP);
            P_MW_APPOINTED_PROF_HISTORY professionalHist_RSE = SetPMWAppointedProfHistory(finalprofessional_RSE);
            P_MW_APPOINTED_PROF_HISTORY professionalHist_RGE = SetPMWAppointedProfHistory(finalprofessional_RGE);
            P_MW_APPOINTED_PROF_HISTORY professionalHist_PRC = SetPMWAppointedProfHistory(finalprofessional_PRC);

            db.P_MW_APPOINTED_PROF_HISTORY.Add(professionalHist_AP);
            db.P_MW_APPOINTED_PROF_HISTORY.Add(professionalHist_RSE);
            db.P_MW_APPOINTED_PROF_HISTORY.Add(professionalHist_RGE);
            db.P_MW_APPOINTED_PROF_HISTORY.Add(professionalHist_PRC);

        }


        // Andy: Get final MwRecord
        public P_MW_RECORD GetFinalMwRecord(String REFERENCE_NUMBER)
        {
            string RecordSql = @"SELECT R.*
                                FROM P_MW_RECORD R
                                inner join P_MW_REFERENCE_NO RefNo on RefNo.uuid = R.REFERENCE_NUMBER
                                WHERE 1=1
                                AND RefNo.UUID = :REFERENCE_NUMBER
                                AND R.IS_DATA_ENTRY = :FLAG_N ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":REFERENCE_NUMBER", REFERENCE_NUMBER),
                new OracleParameter(":FLAG_N", ProcessingConstant.FLAG_N)
            };

            return GetObjectData<P_MW_RECORD>(RecordSql, oracleParameters).FirstOrDefault();
        }

        public P_MW_APPOINTED_PROF_HISTORY SetPMWAppointedProfHistory(P_MW_APPOINTED_PROFESSIONAL p_MW_APPOINTED_PROFESSIONAL)
        {
            P_MW_APPOINTED_PROF_HISTORY professionHistory = new P_MW_APPOINTED_PROF_HISTORY();
            if (p_MW_APPOINTED_PROFESSIONAL == null)
            {
                return professionHistory;
            }
            professionHistory.MW_APPOINTED_PROFESSIONAL_ID = p_MW_APPOINTED_PROFESSIONAL.UUID;
            professionHistory.CERTIFICATION_NO = p_MW_APPOINTED_PROFESSIONAL.CERTIFICATION_NO;
            professionHistory.PROFESSIONAL_TYPE_ID = p_MW_APPOINTED_PROFESSIONAL.PROFESSIONAL_TYPE_ID;
            professionHistory.CHINESE_NAME = p_MW_APPOINTED_PROFESSIONAL.CHINESE_NAME;
            professionHistory.ENGLISH_COMPANY_NAME = p_MW_APPOINTED_PROFESSIONAL.ENGLISH_NAME;
            professionHistory.FORM_PART = p_MW_APPOINTED_PROFESSIONAL.FORM_PART;
            professionHistory.IS_UPDATE_ADDRESS = p_MW_APPOINTED_PROFESSIONAL.IS_UPDATE_ADDRESS;
            professionHistory.STATUS_ID = p_MW_APPOINTED_PROFESSIONAL.STATUS_ID;
            professionHistory.ENDORSEMENT_DATE = p_MW_APPOINTED_PROFESSIONAL.ENDORSEMENT_DATE;
            professionHistory.EFFECT_FROM_DATE = p_MW_APPOINTED_PROFESSIONAL.EFFECT_FROM_DATE;
            professionHistory.EFFECT_TO_DATE = p_MW_APPOINTED_PROFESSIONAL.EFFECT_TO_DATE;
            professionHistory.CHINESE_COMPANY_NAME = p_MW_APPOINTED_PROFESSIONAL.CHINESE_COMPANY_NAME;
            professionHistory.ENGLISH_COMPANY_NAME = p_MW_APPOINTED_PROFESSIONAL.ENGLISH_COMPANY_NAME;
            professionHistory.IDENTIFY_FLAG = p_MW_APPOINTED_PROFESSIONAL.IDENTIFY_FLAG;
            professionHistory.ORDERING = p_MW_APPOINTED_PROFESSIONAL.ORDERING;
            professionHistory.COMMENCED_DATE = p_MW_APPOINTED_PROFESSIONAL.COMMENCED_DATE;
            professionHistory.ISCHECKED = p_MW_APPOINTED_PROFESSIONAL.ISCHECKED;
            professionHistory.COMPLETION_DATE = p_MW_APPOINTED_PROFESSIONAL.COMPLETION_DATE;
            professionHistory.CLASS_COMPLETION_DATE = p_MW_APPOINTED_PROFESSIONAL.CLASS_COMPLETION_DATE;
            professionHistory.CLASS_ENDORSEMENT_DATE = p_MW_APPOINTED_PROFESSIONAL.CLASS_ENDORSEMENT_DATE;
            professionHistory.EXPIRY_DATE = p_MW_APPOINTED_PROFESSIONAL.EXPIRY_DATE;
            professionHistory.CLASS1_CEASE_DATE = p_MW_APPOINTED_PROFESSIONAL.CLASS1_CEASE_DATE;
            professionHistory.CLASS2_CEASE_DATE = p_MW_APPOINTED_PROFESSIONAL.CLASS2_CEASE_DATE;
            professionHistory.APPOINTMENT_DATE = p_MW_APPOINTED_PROFESSIONAL.APPOINTMENT_DATE;
            return professionHistory;
        }
    }
}