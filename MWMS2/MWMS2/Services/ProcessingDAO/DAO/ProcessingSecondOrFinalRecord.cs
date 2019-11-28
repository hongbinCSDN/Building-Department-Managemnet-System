using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Entity;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingSecondOrFinalRecord
    {

        public P_MW_ADDRESS SaveAddress(P_MW_ADDRESS oiAddress, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(oiAddress.UUID))
            {
                db.P_MW_ADDRESS.Add(oiAddress);
            }
            else
            {
                //Get Record
                P_MW_ADDRESS originOIAddress = db.P_MW_ADDRESS.Where(d => d.UUID == oiAddress.UUID).FirstOrDefault();

                //Set Data
                originOIAddress.DISPLAY_STREET = oiAddress.DISPLAY_STREET;
                originOIAddress.DISPLAY_STREET_NO = oiAddress.DISPLAY_STREET_NO;
                originOIAddress.BUILDING_NAME = oiAddress.BUILDING_NAME;
                originOIAddress.FLOOR = oiAddress.FLOOR;
                originOIAddress.FLAT = oiAddress.FLAT;
                originOIAddress.DISTRICT = oiAddress.DISTRICT;
                originOIAddress.REGION = oiAddress.REGION;
            }
            db.SaveChanges();
            return oiAddress;
        }

        public Dictionary<string, object> SaveOIInfo(P_MW_ADDRESS oiAddress, P_MW_PERSON_CONTACT oiPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            oiAddress = SaveAddress(oiAddress, db);
            oiPersonContact.MW_ADDRESS_ID = oiAddress.UUID;
            SaveOIPersonContact(oiPersonContact, foreignKeys, db);
            return foreignKeys;
        }

        public P_MW_PERSON_CONTACT SaveOIPersonContact(P_MW_PERSON_CONTACT oiPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(oiPersonContact.UUID))
            {
                db.P_MW_PERSON_CONTACT.Add(oiPersonContact);
                db.SaveChanges();

                //Set Person Contact UUID 
                foreignKeys.Add("OI_ID", oiPersonContact.UUID);
            }
            else
            {
                //Get Record
                P_MW_PERSON_CONTACT originOIPersonContact = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == oiPersonContact.UUID).FirstOrDefault();

                //Set Data
                originOIPersonContact.NAME_ENGLISH = oiPersonContact.NAME_ENGLISH;
                originOIPersonContact.EMAIL = oiPersonContact.EMAIL;
                originOIPersonContact.FAX_NO = oiPersonContact.FAX_NO;
                originOIPersonContact.CONTACT_NO = oiPersonContact.CONTACT_NO;

                //Save MW_ADDRESS_ID
                originOIPersonContact.MW_ADDRESS_ID = oiPersonContact.MW_ADDRESS_ID;
            }
            db.SaveChanges();
            return oiPersonContact;
        }

        public Dictionary<string, object> SaveSignBoardInfo(P_MW_ADDRESS signBoardAddress, P_MW_PERSON_CONTACT signBoardPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            signBoardAddress = SaveAddress(signBoardAddress, db);
            signBoardPersonContact.MW_ADDRESS_ID = signBoardAddress.UUID;
            SaveSignBoardPersonContact(signBoardPersonContact, foreignKeys, db);
            return foreignKeys;
        }

        public P_MW_PERSON_CONTACT SaveSignBoardPersonContact(P_MW_PERSON_CONTACT signBoardPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(signBoardPersonContact.UUID))
            {
                db.P_MW_PERSON_CONTACT.Add(signBoardPersonContact);
                db.SaveChanges();

                //Set Person Contact UUID 
                foreignKeys.Add("SIGNBOARD_PERFROMER_ID", signBoardPersonContact.UUID);
            }
            else
            {
                //Get Record
                P_MW_PERSON_CONTACT orginSignBoardPersonContact = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == signBoardPersonContact.UUID).FirstOrDefault();

                //Set Data
                orginSignBoardPersonContact.NAME_CHINESE2 = signBoardPersonContact.NAME_CHINESE2;
                orginSignBoardPersonContact.NAME_ENGLISH2 = signBoardPersonContact.NAME_ENGLISH2;
                orginSignBoardPersonContact.ID_NUMBER = signBoardPersonContact.ID_NUMBER;
                orginSignBoardPersonContact.OTHER_ID_TYPE = signBoardPersonContact.OTHER_ID_TYPE;
                orginSignBoardPersonContact.ID_ISSUE_COUNTRY = signBoardPersonContact.ID_ISSUE_COUNTRY;
                orginSignBoardPersonContact.ADDRESS_SAME_A1 = signBoardPersonContact.ADDRESS_SAME_A1;
                orginSignBoardPersonContact.ADDRESS_SAME_A4 = signBoardPersonContact.ADDRESS_SAME_A4;
                orginSignBoardPersonContact.EMAIL = signBoardPersonContact.EMAIL;
                orginSignBoardPersonContact.FAX_NO = signBoardPersonContact.FAX_NO;
                orginSignBoardPersonContact.CONTACT_NO = signBoardPersonContact.CONTACT_NO;
                orginSignBoardPersonContact.ENDORSEMENT_DATE = signBoardPersonContact.ENDORSEMENT_DATE;
                orginSignBoardPersonContact.ID_TYPE = signBoardPersonContact.ID_TYPE;
                orginSignBoardPersonContact.OTHER_ID_TYPE = signBoardPersonContact.OTHER_ID_TYPE;
                orginSignBoardPersonContact.ID_ISSUE_COUNTRY = signBoardPersonContact.ID_ISSUE_COUNTRY;

                //Save MW_ADDRESS_ID
                orginSignBoardPersonContact.MW_ADDRESS_ID = signBoardPersonContact.MW_ADDRESS_ID;
            }
            db.SaveChanges();
            return signBoardPersonContact;
        }

        public Dictionary<string, object> SaveOwnerInfo(P_MW_ADDRESS ownerAddress, P_MW_PERSON_CONTACT ownerPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            ownerAddress = SaveAddress(ownerAddress, db);
            ownerPersonContact.MW_ADDRESS_ID = ownerAddress.UUID;
            SaveOwnerPersonContact(ownerPersonContact, foreignKeys, db);
            return foreignKeys;
        }

        public P_MW_PERSON_CONTACT SaveOwnerPersonContact(P_MW_PERSON_CONTACT ownerPersonContact, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(ownerPersonContact.UUID))
            {
                db.P_MW_PERSON_CONTACT.Add(ownerPersonContact);
                db.SaveChanges();

                //Set Person Contact UUID 
                //model.P_MW_RECORD.OWNER_ID = ownerPersonContact.UUID;
                foreignKeys.Add("OWNER_ID", ownerPersonContact.UUID);
            }
            else
            {
                //Get Record
                P_MW_PERSON_CONTACT originOwnerPersonContact = db.P_MW_PERSON_CONTACT.Where(d => d.UUID == ownerPersonContact.UUID).FirstOrDefault();

                //Set Data
                originOwnerPersonContact.NAME_CHINESE = ownerPersonContact.NAME_CHINESE;
                originOwnerPersonContact.NAME_ENGLISH = ownerPersonContact.NAME_ENGLISH;
                originOwnerPersonContact.ID_NUMBER = ownerPersonContact.ID_NUMBER;
                originOwnerPersonContact.OTHER_ID_TYPE = ownerPersonContact.OTHER_ID_TYPE;
                originOwnerPersonContact.ID_ISSUE_COUNTRY = ownerPersonContact.ID_ISSUE_COUNTRY;
                originOwnerPersonContact.ADDRESS_SAME_A1 = ownerPersonContact.ADDRESS_SAME_A1;
                originOwnerPersonContact.EMAIL = ownerPersonContact.EMAIL;
                originOwnerPersonContact.FAX_NO = ownerPersonContact.FAX_NO;
                originOwnerPersonContact.CONTACT_NO = ownerPersonContact.CONTACT_NO;
                originOwnerPersonContact.ENDORSEMENT_DATE = ownerPersonContact.ENDORSEMENT_DATE;
                originOwnerPersonContact.ID_TYPE = ownerPersonContact.ID_TYPE;
                originOwnerPersonContact.OTHER_ID_TYPE = ownerPersonContact.OTHER_ID_TYPE;
                originOwnerPersonContact.ID_ISSUE_COUNTRY = ownerPersonContact.ID_ISSUE_COUNTRY;

                //Save MW_ADDRESS_ID
                originOwnerPersonContact.MW_ADDRESS_ID = ownerPersonContact.MW_ADDRESS_ID;
            }
            db.SaveChanges();
            return ownerPersonContact;
        }

        public Dictionary<string, object> SaveMWAddress(P_MW_ADDRESS mwAddress, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            bool isAddMWAddress = mwAddress.UUID == null ? true : false;
            mwAddress = SaveAddress(mwAddress, db);
            if (isAddMWAddress)
            {
                foreignKeys.Add("LOCATION_ADDRESS_ID", mwAddress.UUID);
            }
            return foreignKeys;
        }

        public P_MW_RECORD SaveSecondOrFinalRecord(P_MW_RECORD record, Dictionary<string, object> foreignKeys, bool isFinalRecord, EntitiesMWProcessing db)
        {
            if (string.IsNullOrWhiteSpace(record.UUID))
            {
                return AddSecondOrFinalRecord(record, foreignKeys, isFinalRecord, db);
            }

            return UpdateSecondOrFinalRecord(record, foreignKeys, isFinalRecord, db);
        }


        public P_MW_RECORD AddSecondOrFinalRecord(P_MW_RECORD record, Dictionary<string, object> foreignKeys, bool isFinalRecord, EntitiesMWProcessing db)
        {
            Type type = typeof(P_MW_RECORD);
            foreach (string key in foreignKeys.Keys)
            {
                type.GetProperty(key).SetValue(record, foreignKeys[key]);
            }
            record.STATUS_CODE = (isFinalRecord ? ProcessingConstant.MW_FINAL_VERSION : ProcessingConstant.MW_SECOND_ENTRY);
            record.IS_DATA_ENTRY = (isFinalRecord ? ProcessingConstant.FLAG_N : ProcessingConstant.FLAG_Y);

            db.P_MW_RECORD.Add(record);
            db.SaveChanges();
            return record;
        }

        public P_MW_RECORD AddSecondCompleteRecord(P_MW_RECORD record, Dictionary<string, object> foreignKeys, EntitiesMWProcessing db)
        {
            Type type = typeof(P_MW_RECORD);
            foreach (string key in foreignKeys.Keys)
            {
                type.GetProperty(key).SetValue(record, foreignKeys[key]);
            }
            record.STATUS_CODE = ProcessingConstant.MW_SECOND_COMPLETE;
            record.IS_DATA_ENTRY = ProcessingConstant.FLAG_Y;

            db.P_MW_RECORD.Add(record);
            db.SaveChanges();
            return record;
        }

        public P_MW_RECORD UpdateSecondOrFinalRecord(P_MW_RECORD newRecord, Dictionary<string, object> foreignKeys, bool isFinalRecord, EntitiesMWProcessing db)
        {
            P_MW_RECORD originRecord = db.P_MW_RECORD.Where(d => d.UUID == newRecord.UUID).FirstOrDefault();

            //Set Data
            originRecord.FILEREF_FOUR = newRecord.FILEREF_FOUR;
            originRecord.FILEREF_TWO = newRecord.FILEREF_TWO;
            originRecord.LANGUAGE_CODE = newRecord.LANGUAGE_CODE;
            originRecord.FIRST_RECEIVED_DATE = newRecord.FIRST_RECEIVED_DATE;
            originRecord.LOCATION_OF_MINOR_WORK = newRecord.LOCATION_OF_MINOR_WORK;
            originRecord.STATUS_CODE = (isFinalRecord ?
                ProcessingConstant.MW_FINAL_VERSION : ProcessingConstant.MW_SECOND_ENTRY);
            originRecord.IS_DATA_ENTRY = (isFinalRecord ?
                ProcessingConstant.FLAG_N : ProcessingConstant.FLAG_Y);
            originRecord.PERMIT_NO = newRecord.PERMIT_NO;

            Type type = typeof(P_MW_RECORD);
            foreach (string key in foreignKeys.Keys)
            {
                type.GetProperty(key).SetValue(originRecord, foreignKeys[key]);
            }
            db.SaveChanges();
            return originRecord;
        }

    }
}