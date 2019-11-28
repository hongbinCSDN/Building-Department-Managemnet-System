using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MWMS2.Controllers;
namespace MWMS2.DaoController
{

    public class DaoSMMDataEntry
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();

        public IQueryable<B_S_SYSTEM_VALUE> GetB_System_ValueByType(string Type)
        {
            var query = from st in db.B_S_SYSTEM_TYPE
                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                        where st.TYPE == Type
                        orderby sv.CODE
                        select sv;
            return query;
        }

        public IQueryable<B_S_SYSTEM_VALUE> GetBCISDistrict(string TempRegion)
        {
            if (string.IsNullOrEmpty(TempRegion))
            {
                TempRegion = "HK";
            }
            var BCISDistrictQuery = from st in db.B_S_SYSTEM_TYPE
                                    join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                    where st.TYPE == "BcisDistrict" && sv.PARENT_ID == (from st in db.B_S_SYSTEM_TYPE
                                                                                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                                                                        where st.TYPE == "Region" && sv.CODE == TempRegion
                                                                                        orderby sv.CODE
                                                                                        select sv).FirstOrDefault().UUID
                                    orderby sv.CODE
                                    select sv;
            return BCISDistrictQuery;

        }
        //public IQueryable<B_S_USER_ACCOUNT> GetTOUser()
        //{
        //    var TOHandlingOfficerQuery = from user in db.B_S_USER_ACCOUNT
        //                                 where user.RANK == "TO"
        //                                 orderby user.USERNAME
        //                                 select user;
        //    return TOHandlingOfficerQuery;

       // }


        public IQueryable<B_SV_APPOINTED_PROFESSIONAL> GetAppProfDetailbyIdentifyFlag(string flag, string SVRecordUUID)
        {
            var query = from AppProf in db.B_SV_APPOINTED_PROFESSIONAL
                        where AppProf.SV_RECORD_ID == SVRecordUUID && AppProf.IDENTIFY_FLAG == flag
                        select AppProf;
            return query;
        }
        public IList<String> GetStreetNameByPrefix(string Prefix)
        {
            //plan to RV change to BCIS
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(Prefix))
            {
                if (Prefix.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                {
                    var query = from sn in db.B_RV_STREET_NAME
                                where sn.SM_NAME_CHN.Contains(Prefix)
                                select new { StreetName = sn.SM_NAME_CHN };

                    foreach (var item in query)
                    {
                        result.Add(item.StreetName);
                    }
                }
                else
                {
                    var query = from sn in db.B_RV_STREET_NAME
                                where sn.SM_NAME_ENG.StartsWith(Prefix.ToUpper())
                                select new { StreetName = sn.SM_NAME_ENG };
                    foreach (var item in query)
                    {
                        result.Add(item.StreetName);
                    }
                }

                return result;
            }
            else
            {
                return null;
            }


        }

        public IList<String> GetBuildingNameByPrefix(string Prefix)
        {   //plan to RV change to BCIS
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(Prefix))
            {
                if (Prefix.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                {
                    var query = from sn in db.B_RV_BLOCK
                                where sn.BK_BLDG_NAME_ENG_LINE_1.Contains(Prefix)
                                select new { BLDG_NAME = sn.BK_BLDG_NAME_ENG_LINE_1 };


                    foreach (var item in query)
                    {
                        result.Add(item.BLDG_NAME);
                    }
                }
                else
                {
                    var query = from sn in db.B_RV_BLOCK
                                where sn.BK_BLDG_NAME_CHN_LINE_1.StartsWith(Prefix.ToUpper())
                                select new { BLDG_NAME = sn.BK_BLDG_NAME_CHN_LINE_1 };
                    foreach (var item in query)
                    {
                        result.Add(item.BLDG_NAME);
                    }
                }

                return result;
            }
            else
            {
                return null;
            }

        }
        public IList<String> GetFlatByPrefix(string Prefix)
        {   //plan to RV change to BCIS
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(Prefix))
            {

                var query = (from sn in db.B_RV_UNIT
                             where sn.UT_NO_ENG.StartsWith(Prefix.ToUpper())
                             select new { FLAT = sn.UT_NO_ENG }).Distinct().OrderBy(x => x.FLAT).Take(50);
                foreach (var item in query)
                {
                    result.Add(item.FLAT);

                }
            }
            return result;

        }

        public IList<String> GetFloorByPrefix(string Prefix)
        {   //plan to RV change to BCIS
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(Prefix))
            {


                var query = (from sn in db.B_RV_UNIT
                             where sn.UT_FLR_DESC_ENG.StartsWith(Prefix.ToUpper())
                             select new { Floor = sn.UT_FLR_DESC_ENG }).Distinct().OrderBy(x => x.Floor).Take(50);
                foreach (var item in query)
                {
                    result.Add(item.Floor);

                }
            }
            return result;

        }
        //public IQueryable<B_S_SYSTEM_VALUE> GetRegion()
        //{
        //    var DistrictQuery = from st in db.B_S_SYSTEM_TYPE
        //                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                        where st.TYPE == "Region"
        //                        orderby sv.CODE
        //                        select sv;
        //    return DistrictQuery;
        //}
        //public IQueryable<B_S_SYSTEM_VALUE> GetOrderType()
        //{
        //    var RelatedOrderQuery = from st in db.B_S_SYSTEM_TYPE
        //                            join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                            where st.TYPE == "OrderType"
        //                            orderby sv.CODE
        //                            select sv;
        //    return RelatedOrderQuery;
        //}

        //public IQueryable<B_S_SYSTEM_VALUE> GetValidationItem()
        //{
        //    var ValidationItemQuery = from st in db.B_S_SYSTEM_TYPE
        //                            join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                            where st.TYPE == "ValidationItem"
        //                            orderby sv.CODE
        //                            select sv;
        //    return ValidationItemQuery;

        //}
        //public IQueryable<B_S_SYSTEM_VALUE> GetMWItem()
        //{
        //    var MWItemQuery = from st in db.B_S_SYSTEM_TYPE
        //                      join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                      where st.TYPE == "Item No"
        //                      orderby sv.CODE
        //                      select sv;
        //    return MWItemQuery;
        //}
        //public IQueryable<B_S_SYSTEM_VALUE> GetPawSameAs()
        //{
        //    var PAWSAMEASQuery = from st in db.B_S_SYSTEM_TYPE
        //                         join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                         where st.TYPE == "PawSameAs"
        //                         orderby sv.CODE
        //                         select sv;
        //    return PAWSAMEASQuery;
        //}
        //public IQueryable<B_S_SYSTEM_VALUE> GetLetterType()
        //{
        //    var LetterTypeQuery = from st in db.B_S_SYSTEM_TYPE
        //                join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
        //                where st.TYPE == "LetterType"
        //                orderby sv.CODE
        //                select sv;

        //    return LetterTypeQuery;
        //}
        public IQueryable<B_SV_ADDRESS> GetAddress(string uuid)
        {
            var query = from sv_add in db.B_SV_ADDRESS
                        where sv_add.UUID == uuid
                        select sv_add;
            return query;
        }
        public IQueryable<B_SV_RECORD_VALIDATION_ITEM> GetRecordValidationItem(decimal? ordering, string recordUUID)
        {
            var query = from vi in db.B_SV_RECORD_VALIDATION_ITEM
                        where vi.ORDERING == ordering && vi.SV_RECORD_ID == recordUUID
                        select vi;
            return query;
        }

        public IQueryable<B_SV_RECORD_ITEM> GetRecordMWItem(decimal? ordering, string recordUUID)
        {
            var query = from vi in db.B_SV_RECORD_ITEM
                        where vi.ORDERING == ordering && vi.SV_RECORD_ID == recordUUID
                        select vi;
            return query;
        }

        public IQueryable<B_SV_PERSON_CONTACT> GetPersonContact(string uuid)
        {
            var query = from pc in db.B_SV_PERSON_CONTACT
                        where pc.UUID == uuid
                        select pc;
            return query;
        }
        public void DataEntryCreateSVRecord(ModelSVRecord modelSVRecord)
        {
            try
            {
                var query = db.B_SV_RECORD.Where(x => x.REFERENCE_NO == modelSVRecord.SubmissionNo);
                bool existingRecord = false;
                var TempSVINFOUUID = "";
                var TempSignboardOwnerPCUUID = "";
                var TempOIPCUUID = "";
                var TempPAWPCUUID = "";
                existingRecord = query.Count() != 0 ? true : false;
                ///B_SV_ADDRESS b_SV_ADDRESSForSVInfo = existingRecord ? new B_SV_ADDRESS(): new B_SV_ADDRESS(); ;
                #region SV_Address
                /// B_SV_ADDRESS b_SV_ADDRESSForSVInfo = new B_SV_ADDRESS();
                //b_SV_ADDRESSForSVInfo.UUID = b_SV_SIGNBOARD.LOCATION_ADDRESS_ID;

                var SVRecordJoinnedQuery = from sv_record in db.B_SV_RECORD
                                           join sv_signboard in db.B_SV_SIGNBOARD on sv_record.SV_SIGNBOARD_ID equals sv_signboard.UUID
                                           join sv_address in db.B_SV_ADDRESS on sv_signboard.LOCATION_ADDRESS_ID equals sv_address.UUID
                                           join sv_appprof in db.B_SV_APPOINTED_PROFESSIONAL on sv_record.UUID equals sv_appprof.SV_RECORD_ID

                                           where sv_record.SV_SUBMISSION_ID == modelSVRecord.SubmissionUUID && sv_record.STATUS_CODE == SystemParameterConstant.SVRecordDraftStatus
                                           select new { sv_record, sv_signboard, sv_address, sv_appprof };
                if (existingRecord)
                {
                    TempSVINFOUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.LOCATION_ADDRESS_ID;
                    TempSignboardOwnerPCUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.OWNER_ID;
                    TempOIPCUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_record.OI_ID;
                    TempPAWPCUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_record.PAW_ID;
                }
                //var AddressAndContactForSignboard = from pc in db.B_SV_PERSON_CONTACT
                //                                      join sv_add in db.B_SV_ADDRESS on pc.SV_ADDRESS_ID equals sv_add.UUID
                //                                      select new { pc, sv_add };

                B_SV_ADDRESS b_SV_ADDRESSForSVInfo = new B_SV_ADDRESS();
                //b_SV_ADDRESSForSVInfo = existingRecord ? AddressAndContactForSignboard.Where(x=>x.pc.UUID== TempSVINFOUUID).FirstOrDefault().sv_add : new B_SV_ADDRESS();      
                if (!existingRecord)
                {
                    b_SV_ADDRESSForSVInfo.UUID = Guid.NewGuid().ToString();
                    b_SV_ADDRESSForSVInfo.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_ADDRESSForSVInfo.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_ADDRESSForSVInfo = GetAddress(TempSVINFOUUID).FirstOrDefault();

                }
                //b_SV_ADDRESSForSVInfo = GetAddress(TempSVINFOUUID).FirstOrDefault();
                //  b_SV_ADDRESSForSVInfo =  AddressAndContactForSignboard.Where(x => x.pc.UUID == TempSVINFOUUID).FirstOrDefault().sv_add ;

                b_SV_ADDRESSForSVInfo.STREET = modelSVRecord.SVInfoStreetRoadVillageName;
                b_SV_ADDRESSForSVInfo.STREET_NO = modelSVRecord.SVInfoStreetNumber;
                b_SV_ADDRESSForSVInfo.BUILDINGNAME = modelSVRecord.SVInfoBuildingEstate;
                b_SV_ADDRESSForSVInfo.FLOOR = modelSVRecord.SVInfoFloor;
                b_SV_ADDRESSForSVInfo.FLAT = modelSVRecord.SVInfoFlat;
                b_SV_ADDRESSForSVInfo.BLOCK = modelSVRecord.SVInfoBlock;
                b_SV_ADDRESSForSVInfo.DISTRICT = modelSVRecord.SVInfoDistrict;
                b_SV_ADDRESSForSVInfo.BCIS_BLOCK_ID = modelSVRecord.SVInfoBCISBlockID;
                b_SV_ADDRESSForSVInfo.BCIS_DISTRICT = modelSVRecord.SVInfoBCISDistrict;
                b_SV_ADDRESSForSVInfo.FILE_REFERENCE_NO = modelSVRecord.SVInfoBCIS4plus2;
                b_SV_ADDRESSForSVInfo.REGION = modelSVRecord.SVInfoSelectedRegion;

                b_SV_ADDRESSForSVInfo.FULL_ADDRESS = b_SV_ADDRESSForSVInfo.FLAT + " "
                                                    + b_SV_ADDRESSForSVInfo.FLOOR + " "
                                                    + b_SV_ADDRESSForSVInfo.BLOCK + " "
                                                    + b_SV_ADDRESSForSVInfo.BUILDINGNAME + " "
                                                    + b_SV_ADDRESSForSVInfo.STREET + " "
                                                    + b_SV_ADDRESSForSVInfo.DISTRICT;
                b_SV_ADDRESSForSVInfo.CREATED_BY = SystemParameterConstant.UserName;
                b_SV_ADDRESSForSVInfo.CREATED_DATE = System.DateTime.Now;
                b_SV_ADDRESSForSVInfo.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_ADDRESSForSVInfo.MODIFIED_DATE = System.DateTime.Now;
                ////       db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVInfo);




                B_SV_ADDRESS b_SV_ADDRESSForSVOwner = new B_SV_ADDRESS();

                if (!existingRecord)
                {
                    b_SV_ADDRESSForSVOwner.UUID = Guid.NewGuid().ToString();
                    b_SV_ADDRESSForSVOwner.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_ADDRESSForSVOwner.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_ADDRESSForSVOwner = GetAddress(db.B_SV_PERSON_CONTACT.Where(x => x.UUID == TempSignboardOwnerPCUUID).FirstOrDefault().SV_ADDRESS_ID).FirstOrDefault();

                }
                /// b_SV_ADDRESSForSVOwner.UUID = Guid.NewGuid().ToString();

                b_SV_ADDRESSForSVOwner.STREET = modelSVRecord.SVOwnerAddressStreetRoadVillage;
                b_SV_ADDRESSForSVOwner.STREET_NO = modelSVRecord.SVOwnerAddressStreetNo;
                b_SV_ADDRESSForSVOwner.BUILDINGNAME = modelSVRecord.SVOwnerAddressbuildingnameEst;
                b_SV_ADDRESSForSVOwner.FLOOR = modelSVRecord.SVOwnerAddressfloor;
                b_SV_ADDRESSForSVOwner.FLAT = modelSVRecord.SVOwnerAddressflat;
                b_SV_ADDRESSForSVOwner.BLOCK = modelSVRecord.SVOwnerAddressblock;
                b_SV_ADDRESSForSVOwner.DISTRICT = modelSVRecord.SVOwnerAddressDistrict;
                b_SV_ADDRESSForSVOwner.REGION = modelSVRecord.SVInfoSelectedRegion;
                b_SV_ADDRESSForSVOwner.FULL_ADDRESS = b_SV_ADDRESSForSVOwner.FLAT + " "
                                                    + b_SV_ADDRESSForSVOwner.FLOOR + " "
                                                    + b_SV_ADDRESSForSVOwner.BLOCK + " "
                                                    + b_SV_ADDRESSForSVOwner.BUILDINGNAME + " "
                                                    + b_SV_ADDRESSForSVOwner.STREET + " "
                                                    + b_SV_ADDRESSForSVOwner.DISTRICT;

                /// b_SV_ADDRESSForSVOwner.CREATED_BY = SystemParameterConstant.UserName;
                ///   b_SV_ADDRESSForSVOwner.CREATED_DATE = System.DateTime.Now;
                b_SV_ADDRESSForSVOwner.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_ADDRESSForSVOwner.MODIFIED_DATE = System.DateTime.Now;
                ////////    db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVOwner);

                B_SV_ADDRESS b_SV_ADDRESSForSVOwnerCorp = new B_SV_ADDRESS();
                /// b_SV_ADDRESSForSVOwnerCorp.UUID = Guid.NewGuid().ToString();
                if (!existingRecord)
                {
                    b_SV_ADDRESSForSVOwnerCorp.UUID = Guid.NewGuid().ToString();
                    b_SV_ADDRESSForSVOwnerCorp.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_ADDRESSForSVOwnerCorp.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_ADDRESSForSVOwnerCorp = GetAddress((db.B_SV_PERSON_CONTACT.Where(x => x.UUID == TempOIPCUUID).FirstOrDefault().SV_ADDRESS_ID)).FirstOrDefault();

                }
                b_SV_ADDRESSForSVOwnerCorp.STREET = modelSVRecord.SVOwnerCorpAddressStreetRoadVillage;
                b_SV_ADDRESSForSVOwnerCorp.STREET_NO = modelSVRecord.SVOwnerCorpAddressStreetNo;
                b_SV_ADDRESSForSVOwnerCorp.BUILDINGNAME = modelSVRecord.SVOwnerCorpAddressbuildingnameEst;
                b_SV_ADDRESSForSVOwnerCorp.FLOOR = modelSVRecord.SVOwnerCorpAddressfloor;
                b_SV_ADDRESSForSVOwnerCorp.FLAT = modelSVRecord.SVOwnerCorpAddressflat;
                b_SV_ADDRESSForSVOwnerCorp.BLOCK = modelSVRecord.SVOwnerCorpAddressblock;
                b_SV_ADDRESSForSVOwnerCorp.DISTRICT = modelSVRecord.SVOwnerCorpAddressDistrict;
                b_SV_ADDRESSForSVOwnerCorp.REGION = modelSVRecord.SVOwnerCorpSelectedRegion;
                b_SV_ADDRESSForSVOwnerCorp.FULL_ADDRESS = b_SV_ADDRESSForSVOwnerCorp.FLAT + " "
                                                    + b_SV_ADDRESSForSVOwnerCorp.FLOOR + " "
                                                    + b_SV_ADDRESSForSVOwnerCorp.BLOCK + " "
                                                    + b_SV_ADDRESSForSVOwnerCorp.BUILDINGNAME + " "
                                                    + b_SV_ADDRESSForSVOwnerCorp.STREET + " "
                                                    + b_SV_ADDRESSForSVOwnerCorp.DISTRICT;


                /// b_SV_ADDRESSForSVOwnerCorp.CREATED_BY = SystemParameterConstant.UserName;
                /// b_SV_ADDRESSForSVOwnerCorp.CREATED_DATE = System.DateTime.Now;
                b_SV_ADDRESSForSVOwnerCorp.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_ADDRESSForSVOwnerCorp.MODIFIED_DATE = System.DateTime.Now;
                ///////////   db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVOwnerCorp);




                B_SV_ADDRESS b_SV_ADDRESSForSVPAW = new B_SV_ADDRESS();
                ///  b_SV_ADDRESSForSVPAW.UUID = Guid.NewGuid().ToString();
                if (!existingRecord)
                {
                    b_SV_ADDRESSForSVPAW.UUID = Guid.NewGuid().ToString();
                    b_SV_ADDRESSForSVPAW.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_ADDRESSForSVPAW.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_ADDRESSForSVPAW = GetAddress((db.B_SV_PERSON_CONTACT.Where(x => x.UUID == TempPAWPCUUID).FirstOrDefault().SV_ADDRESS_ID)).FirstOrDefault();

                }

                b_SV_ADDRESSForSVPAW.STREET = modelSVRecord.SVPAWAddressStreetRoadVillage;
                b_SV_ADDRESSForSVPAW.STREET_NO = modelSVRecord.SVPAWAddressStreetNo;
                b_SV_ADDRESSForSVPAW.BUILDINGNAME = modelSVRecord.SVPAWAddressbuildingnameEst;
                b_SV_ADDRESSForSVPAW.FLOOR = modelSVRecord.SVPAWAddressfloor;
                b_SV_ADDRESSForSVPAW.FLAT = modelSVRecord.SVPAWAddressflat;
                b_SV_ADDRESSForSVPAW.BLOCK = modelSVRecord.SVPAWAddressblock;
                b_SV_ADDRESSForSVPAW.DISTRICT = modelSVRecord.SVPAWAddressDistrict;
                b_SV_ADDRESSForSVPAW.REGION = modelSVRecord.SVPAWSelectedRegion;
                b_SV_ADDRESSForSVPAW.FULL_ADDRESS = b_SV_ADDRESSForSVPAW.FLAT + " "
                                                    + b_SV_ADDRESSForSVPAW.FLOOR + " "
                                                    + b_SV_ADDRESSForSVPAW.BLOCK + " "
                                                    + b_SV_ADDRESSForSVPAW.BUILDINGNAME + " "
                                                    + b_SV_ADDRESSForSVPAW.STREET + " "
                                                    + b_SV_ADDRESSForSVPAW.DISTRICT;
                ///   b_SV_ADDRESSForSVPAW.CREATED_BY = SystemParameterConstant.UserName;
                ///    b_SV_ADDRESSForSVPAW.CREATED_DATE = System.DateTime.Now;
                b_SV_ADDRESSForSVPAW.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_ADDRESSForSVPAW.MODIFIED_DATE = System.DateTime.Now;
                ////////////  db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVPAW);
                #endregion


                B_SV_PERSON_CONTACT b_SV_PERSON_CONTACTForSVOwner = new B_SV_PERSON_CONTACT();
                if (!existingRecord)
                {
                    b_SV_PERSON_CONTACTForSVOwner.UUID = Guid.NewGuid().ToString();
                    b_SV_PERSON_CONTACTForSVOwner.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_PERSON_CONTACTForSVOwner.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_PERSON_CONTACTForSVOwner = GetPersonContact(TempSignboardOwnerPCUUID).FirstOrDefault();
                }
                ///  b_SV_PERSON_CONTACTForSVOwner.UUID = Guid.NewGuid().ToString();
                b_SV_PERSON_CONTACTForSVOwner.SV_ADDRESS_ID = b_SV_ADDRESSForSVOwner.UUID;
                b_SV_PERSON_CONTACTForSVOwner.NAME_CHINESE = modelSVRecord.SVOwnerChineseName;
                b_SV_PERSON_CONTACTForSVOwner.NAME_ENGLISH = modelSVRecord.SVOwnerEnglishName;
                b_SV_PERSON_CONTACTForSVOwner.ID_TYPE = modelSVRecord.SVOwnerIdType;
                b_SV_PERSON_CONTACTForSVOwner.ID_NUMBER = modelSVRecord.SVOwnerIdNumber;
                b_SV_PERSON_CONTACTForSVOwner.ID_ISSUE_COUNTRY = modelSVRecord.SVOwnerCountryOfIssue;
                b_SV_PERSON_CONTACTForSVOwner.EMAIL = modelSVRecord.SVOwnerEmailAddress;
                b_SV_PERSON_CONTACTForSVOwner.CONTACT_NO = modelSVRecord.SVOwnerContactNo;
                b_SV_PERSON_CONTACTForSVOwner.FAX_NO = modelSVRecord.SVOwnerFaxNo;
                ///  b_SV_PERSON_CONTACTForSVOwner.CREATED_BY = SystemParameterConstant.UserName;
                /// b_SV_PERSON_CONTACTForSVOwner.CREATED_DATE = System.DateTime.Now;
                b_SV_PERSON_CONTACTForSVOwner.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_PERSON_CONTACTForSVOwner.MODIFIED_DATE = System.DateTime.Now;

                ///////////    db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVOwner);

                B_SV_PERSON_CONTACT b_SV_PERSON_CONTACTForSVOwnerCorp = new B_SV_PERSON_CONTACT();
                ///// b_SV_PERSON_CONTACTForSVOwnerCorp.UUID = Guid.NewGuid().ToString();

                if (!existingRecord)
                {
                    b_SV_PERSON_CONTACTForSVOwnerCorp.UUID = Guid.NewGuid().ToString();
                    b_SV_PERSON_CONTACTForSVOwnerCorp.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_PERSON_CONTACTForSVOwnerCorp.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_PERSON_CONTACTForSVOwnerCorp = GetPersonContact(TempOIPCUUID).FirstOrDefault();
                }
                b_SV_PERSON_CONTACTForSVOwnerCorp.SV_ADDRESS_ID = b_SV_ADDRESSForSVOwnerCorp.UUID;
                b_SV_PERSON_CONTACTForSVOwnerCorp.NAME_CHINESE = modelSVRecord.SVOwnerCorpChineseName;
                b_SV_PERSON_CONTACTForSVOwnerCorp.NAME_ENGLISH = modelSVRecord.SVOwnerCorpEnglishName;
                b_SV_PERSON_CONTACTForSVOwnerCorp.ID_TYPE = modelSVRecord.SVOwnerCorpIdType;
                b_SV_PERSON_CONTACTForSVOwnerCorp.ID_NUMBER = modelSVRecord.SVOwnerCorpIdNumber;
                b_SV_PERSON_CONTACTForSVOwnerCorp.ID_ISSUE_COUNTRY = modelSVRecord.SVOwnerCorpCountryOfIssue;
                b_SV_PERSON_CONTACTForSVOwnerCorp.EMAIL = modelSVRecord.SVOwnerCorpEmailAddress;
                b_SV_PERSON_CONTACTForSVOwnerCorp.CONTACT_NO = modelSVRecord.SVOwnerCorpContactNo;
                b_SV_PERSON_CONTACTForSVOwnerCorp.FAX_NO = modelSVRecord.SVOwnerCorpFaxNo;
                b_SV_PERSON_CONTACTForSVOwnerCorp.PRC_NAME = modelSVRecord.SVOwnerCorpPRCAppointedName;
                b_SV_PERSON_CONTACTForSVOwnerCorp.PRC_CONTACT_NO = modelSVRecord.SVOwnerCorpPRCAppointedContactNo;
                b_SV_PERSON_CONTACTForSVOwnerCorp.PBP_NAME = modelSVRecord.SVOwnerCorpPBPAppointedName;
                b_SV_PERSON_CONTACTForSVOwnerCorp.PBP_CONTACT_NO = modelSVRecord.SVOwnerCorpPBPAppointedContactNo;


                ///  b_SV_PERSON_CONTACTForSVOwnerCorp.CREATED_BY = SystemParameterConstant.UserName;
                /// b_SV_PERSON_CONTACTForSVOwnerCorp.CREATED_DATE = System.DateTime.Now;
                b_SV_PERSON_CONTACTForSVOwnerCorp.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_PERSON_CONTACTForSVOwnerCorp.MODIFIED_DATE = System.DateTime.Now;
                ////////    db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVOwnerCorp);
                //
                //
                //



                B_SV_PERSON_CONTACT b_SV_PERSON_CONTACTForSVPAW = new B_SV_PERSON_CONTACT();
                if (!existingRecord)
                {
                    b_SV_PERSON_CONTACTForSVPAW.UUID = Guid.NewGuid().ToString();
                    b_SV_PERSON_CONTACTForSVPAW.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_PERSON_CONTACTForSVPAW.CREATED_DATE = System.DateTime.Now;
                }
                else
                {
                    b_SV_PERSON_CONTACTForSVPAW = GetPersonContact(TempPAWPCUUID).FirstOrDefault();
                }
                ///b_SV_PERSON_CONTACTForSVPAW.UUID = Guid.NewGuid().ToString();
                b_SV_PERSON_CONTACTForSVPAW.SV_ADDRESS_ID = b_SV_ADDRESSForSVPAW.UUID;
                b_SV_PERSON_CONTACTForSVPAW.NAME_CHINESE = modelSVRecord.SVPAWChineseName;
                b_SV_PERSON_CONTACTForSVPAW.NAME_ENGLISH = modelSVRecord.SVPAWEnglishName;
                b_SV_PERSON_CONTACTForSVPAW.ID_NUMBER = modelSVRecord.SVPAWIdNumber;
                b_SV_PERSON_CONTACTForSVPAW.ID_TYPE = modelSVRecord.SVPAWIdType;
                b_SV_PERSON_CONTACTForSVPAW.ID_ISSUE_COUNTRY = modelSVRecord.SVPAWCountryOfIssue;
                b_SV_PERSON_CONTACTForSVPAW.EMAIL = modelSVRecord.SVPAWEmailAddress;
                b_SV_PERSON_CONTACTForSVPAW.CONTACT_NO = modelSVRecord.SVPAWContactNo;
                b_SV_PERSON_CONTACTForSVPAW.FAX_NO = modelSVRecord.SVPAWFaxNo;
                /// b_SV_PERSON_CONTACTForSVPAW.CREATED_BY = SystemParameterConstant.UserName;
                /// b_SV_PERSON_CONTACTForSVPAW.CREATED_DATE = System.DateTime.Now;
                b_SV_PERSON_CONTACTForSVPAW.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_PERSON_CONTACTForSVPAW.MODIFIED_DATE = System.DateTime.Now;
                b_SV_PERSON_CONTACTForSVPAW.PAW_SAME_AS = modelSVRecord.SVPAWSAMEAS;
                /////////   db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVPAW);


                #region SV_Signboard

                B_SV_SIGNBOARD b_SV_SIGNBOARD = existingRecord ? SVRecordJoinnedQuery.FirstOrDefault().sv_signboard : new B_SV_SIGNBOARD();
                /// b_SV_SIGNBOARD.UUID = Guid.NewGuid().ToString();
                /// 
                if (!existingRecord)
                {
                    b_SV_SIGNBOARD.UUID = Guid.NewGuid().ToString();
                    b_SV_SIGNBOARD.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_SIGNBOARD.CREATED_DATE = System.DateTime.Now;
                    b_SV_SIGNBOARD.LOCATION_ADDRESS_ID = b_SV_ADDRESSForSVInfo.UUID;
                    b_SV_SIGNBOARD.OWNER_ID = b_SV_PERSON_CONTACTForSVOwner.UUID;
                }

                b_SV_SIGNBOARD.LOCATION_OF_SIGNBOARD = modelSVRecord.SVInfoLocationOfSignboard;

                b_SV_SIGNBOARD.S24_ORDER_NO = modelSVRecord.SVInfoS24OrderNo;
                b_SV_SIGNBOARD.S24_ORDER_TYPE = modelSVRecord.SVInfoS24OrderType;
                b_SV_SIGNBOARD.RVD_NO = modelSVRecord.SVInfoRVD_No;
                ///  b_SV_SIGNBOARD.CREATED_BY = SystemParameterConstant.UserName;
                ///  b_SV_SIGNBOARD.CREATED_DATE = System.DateTime.Now;
                b_SV_SIGNBOARD.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_SIGNBOARD.MODIFIED_DATE = System.DateTime.Now;
                /////////     db.B_SV_SIGNBOARD.Add(b_SV_SIGNBOARD);
                #endregion


                B_SV_RECORD b_SV_RECORD = existingRecord ? SVRecordJoinnedQuery.FirstOrDefault().sv_record : new B_SV_RECORD();
                if (!existingRecord)
                {
                    b_SV_RECORD.UUID = Guid.NewGuid().ToString();
                    b_SV_RECORD.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_RECORD.CREATED_DATE = System.DateTime.Now;
                    b_SV_RECORD.PAW_ID = b_SV_PERSON_CONTACTForSVPAW.UUID;
                    b_SV_RECORD.SV_SIGNBOARD_ID = b_SV_SIGNBOARD.UUID;
                    b_SV_RECORD.SV_SUBMISSION_ID = modelSVRecord.SubmissionUUID;
                    b_SV_RECORD.OI_ID = b_SV_PERSON_CONTACTForSVOwnerCorp.UUID;
                    b_SV_RECORD.TO_OFFICER = b_SV_RECORD.TO_USER_ID = b_SV_RECORD.PO_USER_ID = b_SV_RECORD.SPO_USER_ID = modelSVRecord.GALTOHandlingOffice;

                    b_SV_RECORD.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_RECORD.CREATED_DATE = System.DateTime.Now;

                }
                ///b_SV_RECORD.UUID = System.Guid.NewGuid().ToString();
                b_SV_RECORD.REFERENCE_NO = modelSVRecord.SubmissionNo;
                b_SV_RECORD.RECEIVED_DATE = cf.StringToDateTime(modelSVRecord.ReceivedDate);
                b_SV_RECORD.FORM_CODE = modelSVRecord.FormCode;

                b_SV_RECORD.INSPECTION_DATE = cf.StringToDateTime(modelSVRecord.WSInspectionDate);
                b_SV_RECORD.SIGNBOARD_REMOVAL_DISCOV_DATE = cf.StringToDateTime(modelSVRecord.WSDiscoryDateForSVRemoval);
                b_SV_RECORD.VALIDATION_EXPIRY_DATE = b_SV_RECORD.RECEIVED_DATE.Value.AddYears(5);

                //b_SV_RECORD.COMMENCEMENT_DATE //plan to fix
                //b_SV_RECORD.COMPLETION_DATE //plan to fix

                b_SV_RECORD.STATUS_CODE = SystemParameterConstant.SVRecordDraftStatus;//plan to fix
                b_SV_RECORD.LANGUAGE_CODE = modelSVRecord.FormLanguage;
                //b_SV_RECORD.CLASS_CODE //plan to fix
                //b_SV_RECORD.MW_PROGRESS_STATUS_CODE //plan to fix

                b_SV_RECORD.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_RECORD.MODIFIED_DATE = System.DateTime.Now;


                b_SV_RECORD.VALIDITY_AP                   = modelSVRecord.VIValidityAP;
                b_SV_RECORD.SIGNATURE_AP                  = modelSVRecord.VISignatureAP;
                b_SV_RECORD.VALIDITY_PRC                  = modelSVRecord.VIValidityPRC;
                b_SV_RECORD.ITEM_STATED                   = modelSVRecord.VICapacityAS;
                b_SV_RECORD.SIGNATURE_AS                  = modelSVRecord.VISignatureAS;
                b_SV_RECORD.INFO_SIGNBOARD_OWNER_PROVIDED = modelSVRecord.VIInfoSOProvided;
                b_SV_RECORD.OTHER_IRREGULARITIES = modelSVRecord.VIOtherIRRMarked;
                b_SV_RECORD.IO_ADDRESS = modelSVRecord.GALIOAddress;
                b_SV_RECORD.RECOMMENDATION                = modelSVRecord.VIRecommendation;
                //////      db.B_SV_RECORD.Add(b_SV_RECORD);



                B_SV_APPOINTED_PROFESSIONAL b_SV_APPOINTED_PROFESSIONALForRSE =
                     existingRecord ? SVRecordJoinnedQuery.Where(x => x.sv_appprof.IDENTIFY_FLAG == SystemParameterConstant.AppointedProfRSE).FirstOrDefault().sv_appprof :
                    new B_SV_APPOINTED_PROFESSIONAL();
                if (!existingRecord)
                {
                    b_SV_APPOINTED_PROFESSIONALForRSE.UUID = Guid.NewGuid().ToString();
                    b_SV_APPOINTED_PROFESSIONALForRSE.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_APPOINTED_PROFESSIONALForRSE.CREATED_DATE = System.DateTime.Now;
                    b_SV_APPOINTED_PROFESSIONALForRSE.IDENTIFY_FLAG = SystemParameterConstant.AppointedProfRSE;
                    b_SV_APPOINTED_PROFESSIONALForRSE.SV_RECORD_ID = b_SV_RECORD.UUID;
                }
                ///   b_SV_APPOINTED_PROFESSIONALForRSE.UUID = Guid.NewGuid().ToString();

                b_SV_APPOINTED_PROFESSIONALForRSE.CERTIFICATION_NO = modelSVRecord.RSECertRegNo;
                b_SV_APPOINTED_PROFESSIONALForRSE.CHINESE_NAME = modelSVRecord.RSEChineseName;
                b_SV_APPOINTED_PROFESSIONALForRSE.ENGLISH_NAME = modelSVRecord.RSEEnglishName;
                b_SV_APPOINTED_PROFESSIONALForRSE.CONTACT_NO = modelSVRecord.RSEContactNo;
                b_SV_APPOINTED_PROFESSIONALForRSE.FAX_NO = modelSVRecord.RSEFaxNo;
                b_SV_APPOINTED_PROFESSIONALForRSE.EXPIRY_DATE = cf.StringToDateTime(modelSVRecord.RSEExpiryDate);
                b_SV_APPOINTED_PROFESSIONALForRSE.SIGNATURE_DATE = cf.StringToDateTime(modelSVRecord.RSESignDate);
                ///   b_SV_APPOINTED_PROFESSIONALForRSE.CREATED_BY = SystemParameterConstant.UserName;
                ///   b_SV_APPOINTED_PROFESSIONALForRSE.CREATED_DATE = System.DateTime.Now;
                b_SV_APPOINTED_PROFESSIONALForRSE.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_APPOINTED_PROFESSIONALForRSE.MODIFIED_DATE = System.DateTime.Now;
                ///db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForRSE);


                B_SV_APPOINTED_PROFESSIONAL b_SV_APPOINTED_PROFESSIONALForRGE
                       = existingRecord ? SVRecordJoinnedQuery.Where(x => x.sv_appprof.IDENTIFY_FLAG == SystemParameterConstant.AppointedProfRGE).FirstOrDefault().sv_appprof :
                    new B_SV_APPOINTED_PROFESSIONAL();

                if (!existingRecord)
                {
                    b_SV_APPOINTED_PROFESSIONALForRGE.UUID = Guid.NewGuid().ToString();
                    b_SV_APPOINTED_PROFESSIONALForRGE.SV_RECORD_ID = b_SV_RECORD.UUID;
                    b_SV_APPOINTED_PROFESSIONALForRGE.IDENTIFY_FLAG = SystemParameterConstant.AppointedProfRGE;
                    b_SV_APPOINTED_PROFESSIONALForRGE.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_APPOINTED_PROFESSIONALForRGE.CREATED_DATE = System.DateTime.Now;
                }


                b_SV_APPOINTED_PROFESSIONALForRGE.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_APPOINTED_PROFESSIONALForRGE.MODIFIED_DATE = System.DateTime.Now;
                /////       db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForRGE);

                B_SV_APPOINTED_PROFESSIONAL b_SV_APPOINTED_PROFESSIONALForAP = existingRecord ? SVRecordJoinnedQuery.Where(x => x.sv_appprof.IDENTIFY_FLAG == SystemParameterConstant.AppointedProfAP).FirstOrDefault().sv_appprof :
                    new B_SV_APPOINTED_PROFESSIONAL();
                if (!existingRecord)
                {
                    b_SV_APPOINTED_PROFESSIONALForAP.UUID = Guid.NewGuid().ToString();
                    b_SV_APPOINTED_PROFESSIONALForAP.SV_RECORD_ID = b_SV_RECORD.UUID;
                    b_SV_APPOINTED_PROFESSIONALForAP.IDENTIFY_FLAG = SystemParameterConstant.AppointedProfAP;
                    b_SV_APPOINTED_PROFESSIONALForAP.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_APPOINTED_PROFESSIONALForAP.CREATED_DATE = System.DateTime.Now;

                }
                b_SV_APPOINTED_PROFESSIONALForAP.CERTIFICATION_NO = modelSVRecord.APCertRegNo;
                b_SV_APPOINTED_PROFESSIONALForAP.CHINESE_NAME = modelSVRecord.APChineseName;
                b_SV_APPOINTED_PROFESSIONALForAP.ENGLISH_NAME = modelSVRecord.APEnglishName;
                b_SV_APPOINTED_PROFESSIONALForAP.CONTACT_NO = modelSVRecord.APContactNo;
                b_SV_APPOINTED_PROFESSIONALForAP.FAX_NO = modelSVRecord.APFaxNo;
                b_SV_APPOINTED_PROFESSIONALForAP.EXPIRY_DATE = cf.StringToDateTime(modelSVRecord.APExpiryDate);
                b_SV_APPOINTED_PROFESSIONALForAP.SIGNATURE_DATE = cf.StringToDateTime(modelSVRecord.APSignDate);
                b_SV_APPOINTED_PROFESSIONALForAP.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_APPOINTED_PROFESSIONALForAP.MODIFIED_DATE = System.DateTime.Now;
                ///////        db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForAP);

                B_SV_APPOINTED_PROFESSIONAL b_SV_APPOINTED_PROFESSIONALForPRC = existingRecord ? SVRecordJoinnedQuery.Where(x => x.sv_appprof.IDENTIFY_FLAG == SystemParameterConstant.AppointedProfPRC).FirstOrDefault().sv_appprof :
                    new B_SV_APPOINTED_PROFESSIONAL();
                if (!existingRecord)
                {
                    b_SV_APPOINTED_PROFESSIONALForPRC.UUID = Guid.NewGuid().ToString();
                    b_SV_APPOINTED_PROFESSIONALForPRC.SV_RECORD_ID = b_SV_RECORD.UUID;
                    b_SV_APPOINTED_PROFESSIONALForPRC.IDENTIFY_FLAG = SystemParameterConstant.AppointedProfPRC;
                    b_SV_APPOINTED_PROFESSIONALForPRC.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_APPOINTED_PROFESSIONALForPRC.CREATED_DATE = System.DateTime.Now;
                }
                b_SV_APPOINTED_PROFESSIONALForPRC.CERTIFICATION_NO = modelSVRecord.PRCCertRegNo;
                b_SV_APPOINTED_PROFESSIONALForPRC.CHINESE_NAME = modelSVRecord.PRCChineseName;
                b_SV_APPOINTED_PROFESSIONALForPRC.ENGLISH_NAME = modelSVRecord.PRCEnglishName;
                b_SV_APPOINTED_PROFESSIONALForPRC.CONTACT_NO = modelSVRecord.PRCContactNo;
                b_SV_APPOINTED_PROFESSIONALForPRC.FAX_NO = modelSVRecord.PRCFaxNo;
                b_SV_APPOINTED_PROFESSIONALForPRC.AS_CHINESE_NAME = modelSVRecord.PRCASChineseName;
                b_SV_APPOINTED_PROFESSIONALForPRC.AS_ENGLISH_NAME = modelSVRecord.PRCASEnglishName;
                b_SV_APPOINTED_PROFESSIONALForPRC.EXPIRY_DATE = cf.StringToDateTime(modelSVRecord.PRCExpiryDate);
                b_SV_APPOINTED_PROFESSIONALForPRC.SIGNATURE_DATE = cf.StringToDateTime(modelSVRecord.PRCSignDate);

                b_SV_APPOINTED_PROFESSIONALForPRC.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_APPOINTED_PROFESSIONALForPRC.MODIFIED_DATE = System.DateTime.Now;
                ////   db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForPRC);





                /// db.B_SV_RECORD_VALIDATION_ITEM.RemoveRange(db.B_SV_RECORD_VALIDATION_ITEM.Where(x => x.SV_RECORD_ID == b_SV_RECORD.UUID && x.ORDERING >= modelSVRecord.SelectedValidationItems.Count()));
                db.B_SV_RECORD_VALIDATION_ITEM.RemoveRange(db.B_SV_RECORD_VALIDATION_ITEM.Where(x => x.SV_RECORD_ID == b_SV_RECORD.UUID));

                int VIOrdering = 0;
                if (modelSVRecord.SelectedValidationItems != null)
                    for (int i = 0; i < modelSVRecord.SelectedValidationItems.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(modelSVRecord.SelectedValidationItems[i]))
                        {


                            continue;
                        }

                        B_SV_RECORD_VALIDATION_ITEM b_SV_RECORD_VALIDATION_ITEM = new B_SV_RECORD_VALIDATION_ITEM();

                        b_SV_RECORD_VALIDATION_ITEM.UUID = Guid.NewGuid().ToString();
                        b_SV_RECORD_VALIDATION_ITEM.SV_RECORD_ID = b_SV_RECORD.UUID;
                        b_SV_RECORD_VALIDATION_ITEM.ORDERING = VIOrdering;
                        VIOrdering++;
                        b_SV_RECORD_VALIDATION_ITEM.CREATED_BY = SystemParameterConstant.UserName;
                        b_SV_RECORD_VALIDATION_ITEM.CREATED_DATE = System.DateTime.Now;


                        var VIQuery = GetB_System_ValueByType("ValidationItem");

                        string SelectedVIDes = modelSVRecord.SelectedValidationItems[i];
                        var VICodeQuery = VIQuery.Where(x => x.DESCRIPTION == SelectedVIDes);
                        b_SV_RECORD_VALIDATION_ITEM.VALIDATION_ITEM = VICodeQuery.FirstOrDefault().CODE;
                        // b_SV_RECORD_VALIDATION_ITEM.VALIDATION_ITEM = GetB_System_ValueByType("ValidationItem").Where(x=>x.DESCRIPTION== modelSVRecord.SelectedValidationItems[i]).FirstOrDefault().CODE;
                        b_SV_RECORD_VALIDATION_ITEM.CORRESPONDING_ITEM = modelSVRecord.SelectedValidationItems[i];

                        b_SV_RECORD_VALIDATION_ITEM.DESCRIPTION = modelSVRecord.SelectedValidationItemsDescription[i];

                        b_SV_RECORD_VALIDATION_ITEM.MODIFIED_BY = SystemParameterConstant.UserName;
                        b_SV_RECORD_VALIDATION_ITEM.MODIFIED_DATE = System.DateTime.Now;
                        //  if (!existingVI)
                        ///  {
                        db.B_SV_RECORD_VALIDATION_ITEM.Add(b_SV_RECORD_VALIDATION_ITEM);

                        /// }

                    }





                db.B_SV_RECORD_ITEM.RemoveRange(db.B_SV_RECORD_ITEM.Where(x => x.SV_RECORD_ID == b_SV_RECORD.UUID));

                int MWOrdering = 0;
                if (modelSVRecord.SelectedMWItems != null)
                    for (int i = 0; i < modelSVRecord.SelectedMWItems.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(modelSVRecord.SelectedMWItems[i]))
                        {


                            continue;
                        }

                        B_SV_RECORD_ITEM b_SV_RECORD_ITEM = new B_SV_RECORD_ITEM();

                        b_SV_RECORD_ITEM.UUID = Guid.NewGuid().ToString();
                        b_SV_RECORD_ITEM.SV_RECORD_ID = b_SV_RECORD.UUID;
                        b_SV_RECORD_ITEM.ORDERING = MWOrdering;
                        MWOrdering++;
                        b_SV_RECORD_ITEM.CREATED_BY = SystemParameterConstant.UserName;
                        b_SV_RECORD_ITEM.CREATED_DATE = System.DateTime.Now;

                        b_SV_RECORD_ITEM.MODIFIED_BY = SystemParameterConstant.UserName;
                        b_SV_RECORD_ITEM.MODIFIED_DATE = System.DateTime.Now;
                        b_SV_RECORD_ITEM.MW_ITEM_CODE = modelSVRecord.SelectedMWItems[i];
                        b_SV_RECORD_ITEM.LOCATION_DESCRIPTION = modelSVRecord.SelectedMWItemsDescription[i];
                        b_SV_RECORD_ITEM.REVISION = modelSVRecord.SelectedMWItemsRefNo[i];

                        switch (b_SV_RECORD_ITEM.MW_ITEM_CODE.Substring(0,1))
                        {
                            case "1":
                                b_SV_RECORD_ITEM.CLASS_CODE = "Class I";
                                break;
                            case "2":
                                b_SV_RECORD_ITEM.CLASS_CODE = "Class II";
                                break;
                            case "3":
                                b_SV_RECORD_ITEM.CLASS_CODE = "Class III";
                                break;
                        }

                        //  if (!existingVI)
                        ///  {
                        db.B_SV_RECORD_ITEM.Add(b_SV_RECORD_ITEM);

                        /// }

                    }



                

                if (!existingRecord)
                {//new record
                    db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVInfo);
                    db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVOwner);
                    db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVOwnerCorp);
                    db.B_SV_ADDRESS.Add(b_SV_ADDRESSForSVPAW);
                    db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVOwner);
                    db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVOwnerCorp);
                    db.B_SV_PERSON_CONTACT.Add(b_SV_PERSON_CONTACTForSVPAW);
                    db.B_SV_SIGNBOARD.Add(b_SV_SIGNBOARD);
                    db.B_SV_RECORD.Add(b_SV_RECORD);
                    db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForRSE);
                    db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForRGE);
                    db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForAP);
                    db.B_SV_APPOINTED_PROFESSIONAL.Add(b_SV_APPOINTED_PROFESSIONALForPRC);
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }



        public ModelSVRecord DataEntryDocListViewDetail(string SubmissionId, string SumbissionNo)
        {
            ModelSVRecord modelSVRecord = new ModelSVRecord();
            string TempSignboardOwnerUUID = "";
            string TempPAWUUID = "";
            string TEMPOIUUID = "";
            modelSVRecord.SubmissionUUID = SubmissionId;
            modelSVRecord.SubmissionNo = SumbissionNo;
            modelSVRecord.SelectedValidationItems = new List<string>();
            modelSVRecord.SelectedValidationItemsDescription = new List<string>();
            modelSVRecord.SelectedMWItems = new List<string>();
            modelSVRecord.SelectedMWItemsRefNo = new List<string>();
            modelSVRecord.SelectedMWItemsDescription = new List<string>();


            var modelSVSubmissionList = GetDataEntryDocList(SubmissionId, SumbissionNo, "", "");



            modelSVRecord.FormCode = modelSVSubmissionList.FirstOrDefault().Form_Code;
            modelSVRecord.ReceivedDate = cf.DateTimeToString(modelSVSubmissionList.FirstOrDefault().Received_Date.Value);
            modelSVRecord.WSValidationExpiryDate = cf.DateTimeToString(modelSVSubmissionList.FirstOrDefault().Received_Date.Value.AddYears(5));

            //default 
            modelSVRecord.FormLanguage = "ZH";

            modelSVRecord.Regions = new List<Region>();
            foreach (var Ditem in GetB_System_ValueByType("Region"))
            {
                Region region = new Region();
                region.Code = Ditem.CODE;
                region.Description = Ditem.DESCRIPTION;
                modelSVRecord.Regions.Add(region);

            }
            var SVRecordJoinnedQuery = from sv_record in db.B_SV_RECORD
                                       join sv_signboard in db.B_SV_SIGNBOARD on sv_record.SV_SIGNBOARD_ID equals sv_signboard.UUID
                                       join sv_address in db.B_SV_ADDRESS on sv_signboard.LOCATION_ADDRESS_ID equals sv_address.UUID
                                       // join sv_appprof in db.B_SV_APPOINTED_PROFESSIONAL on sv_record.UUID equals sv_appprof.SV_RECORD_ID
                                       //join sv_valItem in db.B_SV_RECORD_VALIDATION_ITEM on sv_record.UUID equals sv_valItem.SV_RECORD_ID
                                       where sv_record.SV_SUBMISSION_ID == SubmissionId && sv_record.STATUS_CODE == SystemParameterConstant.SVRecordDraftStatus
                                       select new { sv_record, sv_signboard, sv_address };





            //var SVRecordQuery = db.B_SV_RECORD.Where(x => x.REFERENCE_NO == id);
            if (SVRecordJoinnedQuery.Count() > 0)
            {



                modelSVRecord.UUID = SVRecordJoinnedQuery.FirstOrDefault().sv_record.UUID;
                modelSVRecord.FormLanguage = SVRecordJoinnedQuery.FirstOrDefault().sv_record.LANGUAGE_CODE;
                modelSVRecord.ReceivedDate = cf.DateTimeToString(SVRecordJoinnedQuery.FirstOrDefault().sv_record.RECEIVED_DATE.Value);
                modelSVRecord.SVInfoLocationOfSignboard = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.LOCATION_OF_SIGNBOARD;
                modelSVRecord.SVInfoSelectedRegion = SVRecordJoinnedQuery.FirstOrDefault().sv_address.REGION;
                modelSVRecord.SVInfoDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_address.DISTRICT;
                modelSVRecord.SVInfoBlock = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BLOCK;
                modelSVRecord.SVInfoFloor = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FLOOR;
                modelSVRecord.SVInfoFlat = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FLAT;
                modelSVRecord.SVInfoBuildingEstate = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BUILDINGNAME;
                modelSVRecord.SVInfoStreetNumber = SVRecordJoinnedQuery.FirstOrDefault().sv_address.STREET_NO;
                modelSVRecord.SVInfoStreetRoadVillageName = SVRecordJoinnedQuery.FirstOrDefault().sv_address.STREET;
                modelSVRecord.SVInfoBCISBlockID = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BCIS_BLOCK_ID;

                modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BCIS_DISTRICT;



                modelSVRecord.SVInfoRVD_No = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.RVD_NO;
                //modelSVRecord.SVInofRVDBlockID= 

                modelSVRecord.SVInfoBCIS4plus2 = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FILE_REFERENCE_NO;
                modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BCIS_DISTRICT;
                modelSVRecord.SVInfoS24OrderType = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.S24_ORDER_TYPE;

                modelSVRecord.SVInfoS24OrderNo = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.S24_ORDER_NO;
                modelSVRecord.ValidationItems = new List<ValidationItem>();

                if (SVRecordJoinnedQuery.FirstOrDefault().sv_record.INSPECTION_DATE != null)
                    modelSVRecord.WSInspectionDate = cf.DateTimeToString(SVRecordJoinnedQuery.FirstOrDefault().sv_record.INSPECTION_DATE);

                //  if (SVRecordJoinnedQuery.FirstOrDefault().sv_record.VALIDATION_EXPIRY_DATE != null)
                //      modelSVRecord.WSValidationExpiryDate = cf.DateTimeToString((DateTime)SVRecordJoinnedQuery.FirstOrDefault().sv_record.VALIDATION_EXPIRY_DATE);
                if (SVRecordJoinnedQuery.FirstOrDefault().sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE != null)
                    modelSVRecord.WSDiscoryDateForSVRemoval = cf.DateTimeToString(SVRecordJoinnedQuery.FirstOrDefault().sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE);
                //foreach (var item in SVRecordJoinnedQuery)
                //{
                //    ValidationItem vi = new ValidationItem();
                //    vi.Code = item.sv_valItem.VALIDATION_ITEM;
                //    vi.Description = item.sv_valItem.DESCRIPTION;
                //
                //
                //}
                TempSignboardOwnerUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.OWNER_ID;
                TEMPOIUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_record.OI_ID;
                TempPAWUUID = SVRecordJoinnedQuery.FirstOrDefault().sv_record.PAW_ID;
                var AddressAndContactForSignboard = from pc in db.B_SV_PERSON_CONTACT
                                                    join sv_add in db.B_SV_ADDRESS on pc.SV_ADDRESS_ID equals sv_add.UUID
                                                    select new { pc, sv_add };

                foreach (var item in AddressAndContactForSignboard.Where(x => x.pc.UUID == TempSignboardOwnerUUID))
                {
                    modelSVRecord.SVOwnerChineseName = item.pc.NAME_CHINESE;
                    modelSVRecord.SVOwnerEnglishName = item.pc.NAME_ENGLISH;
                    modelSVRecord.SVOwnerIdType = item.pc.ID_TYPE;
                    modelSVRecord.SVOwnerIdNumber = item.pc.ID_NUMBER;
                    modelSVRecord.SVOwnerCountryOfIssue = item.pc.ID_ISSUE_COUNTRY;

                    modelSVRecord.SVOwnerAddressStreetRoadVillage = item.sv_add.STREET;
                    modelSVRecord.SVOwnerAddressStreetNo = item.sv_add.STREET_NO;
                    modelSVRecord.SVOwnerAddressbuildingnameEst = item.sv_add.BUILDINGNAME;
                    modelSVRecord.SVOwnerAddressfloor = item.sv_add.FLOOR;
                    modelSVRecord.SVOwnerAddressflat = item.sv_add.FLAT;
                    modelSVRecord.SVOwnerAddressblock = item.sv_add.BLOCK;
                    modelSVRecord.SVOwnerAddressDistrict = item.sv_add.DISTRICT;
                    modelSVRecord.SVOwnerSelectedRegion = item.sv_add.REGION;
                    modelSVRecord.SVOwnerEmailAddress = item.pc.EMAIL;
                    modelSVRecord.SVOwnerContactNo = item.pc.CONTACT_NO;
                    modelSVRecord.SVOwnerFaxNo = item.pc.FAX_NO;

                }

                foreach (var item in AddressAndContactForSignboard.Where(x => x.pc.UUID == TEMPOIUUID))
                {
                    modelSVRecord.SVOwnerCorpChineseName = item.pc.NAME_CHINESE;
                    modelSVRecord.SVOwnerCorpEnglishName = item.pc.NAME_ENGLISH;
                    modelSVRecord.SVOwnerCorpIdType = item.pc.ID_TYPE;
                    modelSVRecord.SVOwnerCorpIdNumber = item.pc.ID_NUMBER;
                    modelSVRecord.SVOwnerCorpCountryOfIssue = item.pc.ID_ISSUE_COUNTRY;

                    modelSVRecord.SVOwnerCorpAddressStreetRoadVillage = item.sv_add.STREET;
                    modelSVRecord.SVOwnerCorpAddressStreetNo = item.sv_add.STREET_NO;
                    modelSVRecord.SVOwnerCorpAddressbuildingnameEst = item.sv_add.BUILDINGNAME;
                    modelSVRecord.SVOwnerCorpAddressfloor = item.sv_add.FLOOR;
                    modelSVRecord.SVOwnerCorpAddressflat = item.sv_add.FLAT;
                    modelSVRecord.SVOwnerCorpAddressblock = item.sv_add.BLOCK;
                    modelSVRecord.SVOwnerCorpAddressDistrict = item.sv_add.DISTRICT;
                    modelSVRecord.SVOwnerCorpSelectedRegion = item.sv_add.REGION;
                    modelSVRecord.SVOwnerCorpPRCAppointedName = item.pc.PRC_NAME;

                    modelSVRecord.SVOwnerCorpEmailAddress = item.pc.EMAIL;
                    modelSVRecord.SVOwnerCorpContactNo = item.pc.CONTACT_NO;
                    modelSVRecord.SVOwnerCorpFaxNo = item.pc.FAX_NO;

                    modelSVRecord.SVOwnerCorpPRCAppointedContactNo = item.pc.PRC_CONTACT_NO;
                    modelSVRecord.SVOwnerCorpPBPAppointedName = item.pc.PBP_NAME;
                    modelSVRecord.SVOwnerCorpPBPAppointedContactNo = item.pc.PBP_CONTACT_NO;
                }
                foreach (var item in AddressAndContactForSignboard.Where(x => x.pc.UUID == TempPAWUUID))
                {

                    modelSVRecord.SVPAWChineseName = item.pc.NAME_CHINESE;
                    modelSVRecord.SVPAWEnglishName = item.pc.NAME_ENGLISH;
                    modelSVRecord.SVPAWIdType = item.pc.ID_TYPE;
                    modelSVRecord.SVPAWIdNumber = item.pc.ID_NUMBER;
                    modelSVRecord.SVPAWCountryOfIssue = item.pc.ID_ISSUE_COUNTRY;

                    modelSVRecord.SVPAWAddressStreetRoadVillage = item.sv_add.STREET;
                    modelSVRecord.SVPAWAddressStreetNo = item.sv_add.STREET_NO;
                    modelSVRecord.SVPAWAddressbuildingnameEst = item.sv_add.BUILDINGNAME;
                    modelSVRecord.SVPAWAddressfloor = item.sv_add.FLOOR;
                    modelSVRecord.SVPAWAddressflat = item.sv_add.FLAT;
                    modelSVRecord.SVPAWAddressblock = item.sv_add.BLOCK;
                    modelSVRecord.SVPAWAddressDistrict = item.sv_add.DISTRICT;
                    modelSVRecord.SVPAWSelectedRegion = item.sv_add.REGION;
                    modelSVRecord.SVPAWEmailAddress = item.pc.EMAIL;
                    modelSVRecord.SVPAWContactNo = item.pc.CONTACT_NO;
                    modelSVRecord.SVPAWFaxNo = item.pc.FAX_NO;
                    modelSVRecord.SVPAWSAMEAS = item.pc.PAW_SAME_AS;




                }
                var APDetail = GetAppProfDetailbyIdentifyFlag(SystemParameterConstant.AppointedProfAP, modelSVRecord.UUID);
                modelSVRecord.APCertRegNo = APDetail.FirstOrDefault().CERTIFICATION_NO;
                modelSVRecord.APChineseName = APDetail.FirstOrDefault().CHINESE_NAME;
                modelSVRecord.APEnglishName = APDetail.FirstOrDefault().ENGLISH_NAME;
                modelSVRecord.APContactNo = APDetail.FirstOrDefault().CONTACT_NO;
                modelSVRecord.APFaxNo = APDetail.FirstOrDefault().FAX_NO;
                if (APDetail.FirstOrDefault().EXPIRY_DATE != null)
                    modelSVRecord.APExpiryDate = cf.DateTimeToString(APDetail.FirstOrDefault().EXPIRY_DATE);
                if (APDetail.FirstOrDefault().SIGNATURE_DATE != null)
                    modelSVRecord.APSignDate = cf.DateTimeToString(APDetail.FirstOrDefault().SIGNATURE_DATE);

                var RSEDetail = GetAppProfDetailbyIdentifyFlag(SystemParameterConstant.AppointedProfRSE, modelSVRecord.UUID);
                modelSVRecord.RSECertRegNo = RSEDetail.FirstOrDefault().CERTIFICATION_NO;
                modelSVRecord.RSEChineseName = RSEDetail.FirstOrDefault().CHINESE_NAME;
                modelSVRecord.RSEEnglishName = RSEDetail.FirstOrDefault().ENGLISH_NAME;
                modelSVRecord.RSEContactNo = RSEDetail.FirstOrDefault().CONTACT_NO;
                modelSVRecord.RSEFaxNo = RSEDetail.FirstOrDefault().FAX_NO;
                if (RSEDetail.FirstOrDefault().EXPIRY_DATE != null)
                    modelSVRecord.RSEExpiryDate = cf.DateTimeToString(RSEDetail.FirstOrDefault().EXPIRY_DATE);
                if (RSEDetail.FirstOrDefault().SIGNATURE_DATE != null)
                    modelSVRecord.RSESignDate = cf.DateTimeToString(RSEDetail.FirstOrDefault().SIGNATURE_DATE);

                var PRCDetail = GetAppProfDetailbyIdentifyFlag(SystemParameterConstant.AppointedProfPRC, modelSVRecord.UUID);
                modelSVRecord.PRCCertRegNo = PRCDetail.FirstOrDefault().CERTIFICATION_NO;
                modelSVRecord.PRCChineseName = PRCDetail.FirstOrDefault().CHINESE_NAME;
                modelSVRecord.PRCEnglishName = PRCDetail.FirstOrDefault().ENGLISH_NAME;
                modelSVRecord.PRCContactNo = PRCDetail.FirstOrDefault().CONTACT_NO;
                modelSVRecord.PRCFaxNo = PRCDetail.FirstOrDefault().FAX_NO;
                modelSVRecord.PRCASChineseName = PRCDetail.FirstOrDefault().AS_CHINESE_NAME;
                modelSVRecord.PRCASEnglishName = PRCDetail.FirstOrDefault().AS_ENGLISH_NAME;

                if (PRCDetail.FirstOrDefault().EXPIRY_DATE != null)
                    modelSVRecord.PRCExpiryDate = cf.DateTimeToString(PRCDetail.FirstOrDefault().EXPIRY_DATE);
                if (PRCDetail.FirstOrDefault().SIGNATURE_DATE != null)
                    modelSVRecord.PRCSignDate = cf.DateTimeToString(PRCDetail.FirstOrDefault().SIGNATURE_DATE);
               // var test = (db.B_SV_RECORD_VALIDATION_ITEM.Where(x => x.SV_RECORD_ID == modelSVRecord.UUID)).Count();
                for (int i = 0; i < (db.B_SV_RECORD_VALIDATION_ITEM.Where(x => x.SV_RECORD_ID == modelSVRecord.UUID)).Count(); i++)
                {
                    var query = GetRecordValidationItem(i, modelSVRecord.UUID);
                    modelSVRecord.SelectedValidationItems.Add(query.FirstOrDefault().CORRESPONDING_ITEM);
                    modelSVRecord.SelectedValidationItemsDescription.Add(query.FirstOrDefault().DESCRIPTION);
                }
                for (int i = 0; i < (db.B_SV_RECORD_ITEM.Where(x => x.SV_RECORD_ID == modelSVRecord.UUID)).Count(); i++)
                {
                    var query = GetRecordMWItem(i, modelSVRecord.UUID);
                    modelSVRecord.SelectedMWItems.Add(query.FirstOrDefault().MW_ITEM_CODE);
                    modelSVRecord.SelectedMWItemsDescription.Add(query.FirstOrDefault().LOCATION_DESCRIPTION);
                    modelSVRecord.SelectedMWItemsRefNo.Add(query.FirstOrDefault().REVISION);

                }

                modelSVRecord.VIValidityAP = SVRecordJoinnedQuery.FirstOrDefault().sv_record.VALIDITY_AP;
                modelSVRecord.VISignatureAP = SVRecordJoinnedQuery.FirstOrDefault().sv_record.SIGNATURE_AP;
                modelSVRecord.VIValidityPRC = SVRecordJoinnedQuery.FirstOrDefault().sv_record.VALIDITY_PRC;
                modelSVRecord.VICapacityAS = SVRecordJoinnedQuery.FirstOrDefault().sv_record.ITEM_STATED;
                modelSVRecord.VISignatureAS = SVRecordJoinnedQuery.FirstOrDefault().sv_record.SIGNATURE_AS;
                modelSVRecord.VIInfoSOProvided = SVRecordJoinnedQuery.FirstOrDefault().sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                modelSVRecord.VIOtherIRRMarked = SVRecordJoinnedQuery.FirstOrDefault().sv_record.OTHER_IRREGULARITIES;
                modelSVRecord.GALIOAddress = SVRecordJoinnedQuery.FirstOrDefault().sv_record.IO_ADDRESS;
                modelSVRecord.VIRecommendation = SVRecordJoinnedQuery.FirstOrDefault().sv_record.RECOMMENDATION;
                modelSVRecord.GALTOHandlingOffice = SVRecordJoinnedQuery.FirstOrDefault().sv_record.TO_OFFICER;
                //modelSVRecord.GALLetterType = SVRecordJoinnedQuery.FirstOrDefault().sv_record
                //modelSVRecord.SelectedMWItems = new List<string>();
                //modelSVRecord.SelectedMWItemsDescription = new List<string>();
                //modelSVRecord.SelectedMWItemsRefNo = new List<string>();
            }



            return modelSVRecord;
        }




        public IList<ModelSVSubmission> GetDataEntryDocList(string SubmissionUUID, string SubmissionNo, string ReceivedStartDate, string ReceivedEndDate)
        {
            var query = from submission in db.B_SV_SUBMISSION
                        join scanned_doc in db.B_SV_SCANNED_DOCUMENT on submission.SV_SCANNED_DOCUMENT_ID equals scanned_doc.UUID
                        where submission.STATUS == SystemParameterConstant.SubmissionDraftStatus
                        orderby submission.REFERENCE_NO
                        select new { submission, scanned_doc };

            if (!String.IsNullOrEmpty(SubmissionNo))
            {
                query = query.Where(x => x.submission.REFERENCE_NO == SubmissionNo);
            }
            if (!String.IsNullOrEmpty(ReceivedStartDate))
            {
                DateTime? tempStart = cf.StringToDateTime(ReceivedStartDate);
                query = query.
                          Where(s => s.submission.SCU_RECEIVED_DATE != null &&
                         System.Data.Entity.DbFunctions.TruncateTime(s.submission.SCU_RECEIVED_DATE.Value) >= tempStart
                       );

            }
            if (!String.IsNullOrEmpty(ReceivedEndDate))
            {
                DateTime? tempEnd = cf.StringToDateTime(ReceivedEndDate);
                query = query.
                          Where(s => s.submission.SCU_RECEIVED_DATE != null &&
                         System.Data.Entity.DbFunctions.TruncateTime(s.submission.SCU_RECEIVED_DATE.Value) >= tempEnd
                       );

            }



            List<ModelSVSubmission> modelSVSubmissionList = new List<ModelSVSubmission>();

            foreach (var item in query)
            {
                ModelSVSubmission modelSVSubmission = new ModelSVSubmission();
                modelSVSubmission.UUID = item.submission.UUID;
                modelSVSubmission.SubmissionNo = item.submission.REFERENCE_NO;
                modelSVSubmission.DSN_NO = item.scanned_doc.DSN_NUMBER;
                modelSVSubmission.Form_Code = item.submission.FORM_CODE;
                modelSVSubmission.Received_Date = item.submission.SCU_RECEIVED_DATE;
                modelSVSubmission.Time = item.submission.SCU_RECEIVED_DATE.Value.ToShortTimeString();
                modelSVSubmission.Status = "Draft"; //query where clause , status must be "SCU_DATA_ENTRY" 
                modelSVSubmission.Batch_Number = item.submission.BATCH_NO;
                modelSVSubmissionList.Add(modelSVSubmission);

            }
            return modelSVSubmissionList;

        }
    }
}