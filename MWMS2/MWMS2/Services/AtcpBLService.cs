using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using System.Data.Entity;
using MWMS2.Controllers;
using MWMS2.Constant;

namespace MWMS2.Services
{


    public class AtcpBLService
    {
        //store in app memory list :
        private static volatile IEnumerable<object> _SMMItemNo;
        private static volatile IEnumerable<object> _SMMValidationItem;


        private static volatile AtcpDAOService _DAO;
        private static readonly object locker = new object();
        private static AtcpDAOService DAO { get { if (_DAO == null) lock (locker) if (_DAO == null) _DAO = new AtcpDAOService(); return _DAO; } }

        public IEnumerable<object> SMMValidationItem(AtcpModel model)
        {
            if (_SMMValidationItem == null) lock (locker) if (_SMMValidationItem == null) _SMMValidationItem = DAO.B_S_SYSTEM_TYPE_List(SignboardConstant.SYSTEM_TYPE_VALIDATION_ITEM).Select(o => new { o.CODE, o.DESCRIPTION });
            return _SMMValidationItem;
        }
        public IEnumerable<object> SMMinorWorkItem(AtcpModel model)
        {
            if (_SMMItemNo == null) lock (locker) if (_SMMItemNo == null) _SMMItemNo = DAO.B_S_SYSTEM_TYPE_List(SignboardConstant.SYSTEM_TYPE_ITEM_NO).Select(o => new { o.CODE });
            return _SMMItemNo;
        }



        //B_SV_PERSON_CONTACT

        public IEnumerable<object> SMMPersonContact(AtcpModel model)
        {
            return DAO.SMMPersonContact(
                model.atcpParameters["NAME_CHINESE"]
                , model.atcpParameters["NAME_ENGLISH"]
                , model.atcpParameters["ID_NUMBER"])
                .Select(o => new { NAME_CHINESE = o["NAME_CHINESE"], NAME_ENGLISH = o["NAME_ENGLISH"], ID_NUMBER = o["ID_NUMBER"] });
            //if (_SMMValidationItem == null) lock (locker) if (_SMMValidationItem == null) _SMMValidationItem = DAO.B_S_SYSTEM_TYPE_List(SignboardConstant.SYSTEM_TYPE_VALIDATION_ITEM).Select(o => new { o.CODE, o.DESCRIPTION });
            //return _SMMValidationItem;
        }



        public IEnumerable<object> Address42(AtcpModel model)
        {
            return DAO.Address42(
              //model.atcpParameters["FULL"]
              !model.atcpParameters.ContainsKey("FILEREF_FOUR") ? null : model.atcpParameters["FILEREF_FOUR"]
                , !model.atcpParameters.ContainsKey("FILEREF_TWO") ? null : model.atcpParameters["FILEREF_TWO"]
                , !model.atcpParameters.ContainsKey("BLK_ID") ? null : model.atcpParameters["BLK_ID"]);
        }


        public IEnumerable<object> AddressFullAddress(AtcpModel model)
        {
            return DAO.AddressFullAddress(
              //model.atcpParameters["FULL"]
              !model.atcpParameters.ContainsKey("STREETNAME") ? null : model.atcpParameters["STREETNAME"]
                , !model.atcpParameters.ContainsKey("STREETNO") ? null : model.atcpParameters["STREETNO"]
                , !model.atcpParameters.ContainsKey("BUILDING") ? null : model.atcpParameters["BUILDING"]
                , !model.atcpParameters.ContainsKey("BLK_ID") ? null : model.atcpParameters["BLK_ID"]);
        }
        public IEnumerable<object> AddressFullUnit(AtcpModel model)
        {
            return DAO.AddressFullUnit(//model.atcpParameters["FULL"]
                !model.atcpParameters.ContainsKey("STREETNAME") ? null : model.atcpParameters["STREETNAME"]
                , !model.atcpParameters.ContainsKey("STREETNO") ? null : model.atcpParameters["STREETNO"]
                , !model.atcpParameters.ContainsKey("BUILDING") ? null : model.atcpParameters["BUILDING"]
                , !model.atcpParameters.ContainsKey("BLK_ID") ? null : model.atcpParameters["BLK_ID"]
                , !model.atcpParameters.ContainsKey("FLOOR") ? null : model.atcpParameters["FLOOR"]
                , !model.atcpParameters.ContainsKey("UNIT") ? null : model.atcpParameters["UNIT"]
                );
        }
        

        public IEnumerable<object> AddressStreetName(AtcpModel model)
        {
            return DAO.AddressStreetName(model.term).Select(o => new { code = o["ST_NAME"] });
        }

        public IEnumerable<object> CERTIFICATION_NO(AtcpModel model)
        {
            return DAO.CERTIFICATION_NO(model.term);
        }

        public IEnumerable<object> AddressStreetNumber(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["ST_NAME"])) return null;

            return DAO.AddressStreetNumber(model.atcpParameters["ST_NAME"], model.term)
                .Select(o => new
                {
                    code = o["STREETNO"],
                    building = o["BUILDING"],
                    BLK_NO = o["BLK_NO"],
                    ADR_BLK_ID = o["ADR_BLK_ID"],
                    REGION_CODE = o["REGION_CODE"],
                    DISTRICT = o["DISTRICT"],
                    DISTRICT_CODE = o["DISTRICT_CODE"],
                    SYS_REGION_ID = o["SYS_REGION_ID"],
                    SYS_DISTRICT_ID = o["SYS_DISTRICT_ID"],
                    AREA_ID = o["AREA_ID"]
                });
        }

        public IEnumerable<object> AddressBuilding(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["ST_NAME"])) return null;
            return DAO.AddressBuilding(model.atcpParameters["ST_NAME"], model.atcpParameters["STREETNO"], model.term)
                .Select(o => new
                {
                    code = o["BUILDING"],
                    BLK_NO = o["BLK_NO"],
                    STREETNO = o["STREETNO"],
                    REGION_CODE = o["REGION_CODE"],
                    DISTRICT = o["DISTRICT"],
                    DISTRICT_CODE = o["DISTRICT_CODE"],
                    ADR_BLK_ID = o["ADR_BLK_ID"],
                    SYS_REGION_ID = o["SYS_REGION_ID"],
                    SYS_DISTRICT_ID = o["SYS_DISTRICT_ID"],
                    AREA_ID = o["AREA_ID"]
                });
        }

        public IEnumerable<object> AddressFloor(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["BUILDING"])) return null;
            return DAO.AddressFloor(model.atcpParameters["ST_NAME"], model.atcpParameters["STREETNO"], model.atcpParameters["BUILDING"], model.term)
                 .Select(o => new
                 {
                     code = o["FLOOR"],
                     //BUILDING = o["BUILDING"],
                     //STREETNO = o["STREETNO"],
                     //ST_NAME = o["ST_NAME"],
                     ADR_BLK_ID = o["ADR_BLK_ID"],
                     SYS_REGION_ID = o["SYS_REGION_ID"],
                     SYS_DISTRICT_ID = o["SYS_DISTRICT_ID"],
                     AREA_ID = o["AREA_ID"]
                 });
        }


        public IEnumerable<object> AddressUnit(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["BUILDING"])) return null;
            return DAO.AddressUnit(model.atcpParameters["ST_NAME"]
                , model.atcpParameters["STREETNO"]
                , model.atcpParameters["BUILDING"]
                , model.atcpParameters["FLOOR"]
                , model.term)
                 .Select(o => new
                 {
                     code = o["UNIT"],
                     //BUILDING = o["BUILDING"],
                     //STREETNO = o["STREETNO"],
                     //ST_NAME = o["ST_NAME"],
                     ADR_UNIT_ID = o["ADR_UNIT_ID"],
                     ADR_BLK_ID = o["ADR_BLK_ID"],
                     SYS_REGION_ID = o["SYS_REGION_ID"],
                     SYS_DISTRICT_ID = o["SYS_DISTRICT_ID"],
                     AREA_ID = o["AREA_ID"]
                 });
        }

        /*
        public IEnumerable<object> AddressUnit(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["BUILDING"]) || string.IsNullOrWhiteSpace(model.atcpParameters["FLOOR"])) return null;
            return DAO.StructuralAddress(
                new string[] { "UNIT" }, new string[] { "UNIT" }
                , false, model.term
                , true, model.atcpParameters["FLOOR"] 
                , true, model.atcpParameters["BUILDING"] 
                , true, model.atcpParameters["STREETNO"] 
                , true, model.atcpParameters["ST_NAME"] 
                , true, null
                , null
                , null).Select(o => new { code = o["UNIT"] });
        }
        */


        public List<Dictionary<string, object>> QSearchKeyword(AtcpModel model)
        {
            return DAO.QSearchKeyword(model);
            // .Select(o => new { UUID = o["UUID"], CODE = o["CODE"], DESCRIPTION = o["DESCRIPTION"], RANK_GROUP = o["RANK_GROUP"] });
        }
        


        public List<Dictionary<string, object>> SysRank(AtcpModel model)
        {
            return DAO.SysRank(model);
            // .Select(o => new { UUID = o["UUID"], CODE = o["CODE"], DESCRIPTION = o["DESCRIPTION"], RANK_GROUP = o["RANK_GROUP"] });
        }
        public List<Dictionary<string, object>> SysUnit(AtcpModel model)
        {
            return DAO.SysUnit(model);
            // .Select(o => new { uuid = o["UUID"], section = o["SECTION"], unit = o["UNIT"], description = o["DESCRIPTION"] });
        }
        public List<Dictionary<string, object>> ValidationTemplate(AtcpModel model)
        {
            return DAO.ValidationTemplate(model);
            // .Select(o => new { uuid = o["UUID"], section = o["SECTION"], unit = o["UNIT"], description = o["DESCRIPTION"] });
        }
        public List<Dictionary<string, object>> LetterRefuse(AtcpModel model)
        {
            return DAO.LetterRefuse(model);
            // .Select(o => new { uuid = o["UUID"], section = o["SECTION"], unit = o["UNIT"], description = o["DESCRIPTION"] });
        }
        //ValidationTemplate




        public List<Dictionary<string, object>> AtcpScuResponsibleAreas(AtcpModel model)
        {
            return DAO.AtcpScuResponsibleAreas(model);
        }

        public List<Dictionary<string, object>> AtcpScuSubordinateMembers(AtcpModel model)
        {
            return DAO.AtcpScuSubordinateMembers(model).Select(o => new Dictionary<string, object>() { ["POST"] = o.CODE, ["UUID"] = o.UUID }).ToList();
        }
        public List<Dictionary<string, object>> AtcpPemSubordinateMembers(AtcpModel model)
        {
            return DAO.AtcpPemSubordinateMembers(model).Select(o => new Dictionary<string, object>() { ["POST"] = o.CODE, ["UUID"] = o.UUID }).ToList();
        }

        public string BlockId(int unitID) {
            using(EntitiesAddress db = new EntitiesAddress())
            {
                MWMS_UNIT rs = db.MWMS_UNIT.Where(o => o.ADR_UNIT_ID == unitID).FirstOrDefault();
                return rs == null ? "" : rs.ADR_BLK_ID.ToString();
            }
        }


    }
}