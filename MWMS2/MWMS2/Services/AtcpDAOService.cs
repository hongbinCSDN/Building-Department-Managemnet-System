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
    public class AtcpDAOService
    {
        public List<Dictionary<string, object>> SMMPersonContact(string NAME_CHINESE, string NAME_ENGLISH, string ID_NUMBER)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT NAME_CHINESE, NAME_ENGLISH, ID_NUMBER FROM B_SV_PERSON_CONTACT",
                Rpp = 50
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(NAME_CHINESE))
            {
                whereQ += " AND NAME_CHINESE LIKE :NAME_CHINESE";
                model.QueryParameters.Add("NAME_CHINESE", NAME_CHINESE.Trim().ToUpper() + "%");
                model.Sort = "NAME_CHINESE";
            }
            else if (!string.IsNullOrWhiteSpace(NAME_ENGLISH))
            {
                whereQ += " AND NAME_CHINESE LIKE :NAME_ENGLISH";
                model.QueryParameters.Add("NAME_ENGLISH", NAME_ENGLISH.Trim().ToUpper() + "%");
                model.Sort = "NAME_ENGLISH";
            }
            else if (!string.IsNullOrWhiteSpace(ID_NUMBER))
            {
                whereQ += " AND ID_NUMBER LIKE :NAME_CHINESE";
                model.QueryParameters.Add("ID_NUMBER", ID_NUMBER.Trim().ToUpper() + "%");
                model.Sort = "ID_NUMBER";
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }


        public List<Dictionary<string, object>> AddressFullAddress(string streetName, string streetNo, string building, string blockId)
        {
            DisplayGrid model = new DisplayGrid
            {
                //Query =q,
                Rpp = 10,
                Sort = "1"
            };
            string loading = "";
            if (streetName != null) loading = "STREET";
            if (streetNo != null) loading = "STREETNO";
            if (building != null) loading = "BUILDING";
            string q = "";

            string whereQ = "";
            if (loading == "STREET")
            {
                if (string.IsNullOrWhiteSpace(streetName)) return null;
                q = q
                + "   SELECT  DISTINCT                                                "
                + "  T4.ST_NAME AS STREETNAME                                    "
                + " FROM MWMS_ST_NAME T4      WHERE 1=1                           ";
                model.Sort="1";
                whereQ += " AND T4.ST_NAME LIKE :STREETNAME";
                model.QueryParameters.Add("STREETNAME", streetName.Trim().ToUpper() + "%");

                if (streetName.Length > 2) model.Rpp = -1;

            }
            else if (loading == "STREETNO")
            {
                if (string.IsNullOrWhiteSpace(streetName)) return null;
                q = q
             + "  SELECT DISTINCT                                                  "
             + "                                                               "
             + " T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT AS STREETNO                            "
             + " , LPAD(T2.ST_NO_NUM,10,'0') || LPAD(T2.ST_NO_ALPHA,10,'0') || LPAD(T2.ST_NO_EXT,10,'0') AS ORD      "

             + " FROM  MWMS_BLK_SL T2                                                                       "
             + "  INNER JOIN MWMS_ST_LOC T3   ON     T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                                                                  "
             + "  INNER JOIN MWMS_ST_NAME T4  ON T3.SYS_ST_NAME_ID = T4.SYS_ST_NAME_ID                                 "

             + " WHERE 1=1                                                   ";
                model.Rpp = -1;
                model.Sort = "ORD";
                whereQ += " AND T4.ST_NAME = :STREETNAME";
                model.QueryParameters.Add("STREETNAME", streetName.Trim().ToUpper());
                whereQ += " AND T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT LIKE :STREETNO";
                model.QueryParameters.Add("STREETNO", streetNo.Trim().ToUpper() + "%");
            }
            else if (loading == "BUILDING")
            {
                if (string.IsNullOrWhiteSpace(streetName) && string.IsNullOrWhiteSpace(building)) return null;
                q = q
             + "   SELECT DISTINCT T1.BLDG_NAME_E1||' '||T1.BLDG_NAME_E2||' '||T1.BLDG_NAME_E3 AS BUILDINGNAME,                                                    "
             + " T4.ST_NAME AS STREETNAME                                                                "
             + " , T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT AS STREETNO  ,T1.ADR_BLK_ID AS BLK_ID                            "

             + " FROM  MWMS_BLK T1  INNER JOIN MWMS_BLK_SL T2   ON T1.ADR_BLK_ID = T2.ADR_BLK_ID                                                                        "
             + "  INNER JOIN MWMS_ST_LOC T3   ON     T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                                                                  "
             + "  INNER JOIN MWMS_ST_NAME T4  ON T3.SYS_ST_NAME_ID = T4.SYS_ST_NAME_ID  LEFT JOIN MWMS_BLK_FILEREF t5  ON t5.ADR_BLK_ID = T1.ADR_BLK_ID                                "

             + " WHERE 1=1                                                   ";
               
                model.Sort = "1";
                if (!string.IsNullOrWhiteSpace(streetName))
                {
                    model.Rpp = -1;
                    whereQ += " AND T4.ST_NAME = :STREETNAME";
                    model.QueryParameters.Add("STREETNAME", streetName.Trim().ToUpper());
                }

                if (!string.IsNullOrWhiteSpace(streetNo))
                {
                    whereQ += " AND T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT = :STREETNO";
                    model.QueryParameters.Add("STREETNO", streetNo.Trim().ToUpper());
                }
                if (!string.IsNullOrWhiteSpace(building) && building.Length > 2)
                {
                    model.Rpp = -1;
                }
                    whereQ += " AND T1.BLDG_NAME_E1||' '||T1.BLDG_NAME_E2||' '||T1.BLDG_NAME_E3 LIKE :BUILDINGNAME";
                model.QueryParameters.Add("BUILDINGNAME", building.Trim().ToUpper() + "%");
                //whereQ += " AND BLK_ID LIKE :BLK_ID";
                //model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper() + "%");
            }
            /*
            string selectField = "";
            string q = "";
           // + " SELECT BLK_ID, KEYWORD, STREETNAME, STREETNO, BUILDINGNAME, FREF_SEQ ,FREF_YR                         ";
            
            q=q
            + "   FROM (SELECT  T1.ADR_BLK_ID AS BLK_ID,                                                         "
            + "  	 SUBSTR(                                                                              "
            + "  	  CASE WHEN T1.BLDG_NAME_E1 IS NULL THEN '' ELSE ' '||T1.BLDG_NAME_E1 END             "
            + "  	||CASE WHEN T1.BLDG_NAME_E2 IS NULL THEN '' ELSE ' '||T1.BLDG_NAME_E2 END             "
            + "  	||CASE WHEN T1.BLDG_NAME_E3 IS NULL THEN '' ELSE ' '||T1.BLDG_NAME_E3 END             "
            + "                                                                                           "
            + "                                                                                           "
            + "  	||CASE WHEN T2.OSADR_ST_E1 IS NULL THEN '' ELSE ' '||T2.OSADR_ST_E1 END               "
            + "  	||CASE WHEN T2.OSADR_ST_E2 IS NULL THEN '' ELSE ' '||T2.OSADR_ST_E2 END               "
            + "                                                                                           "
            + "  	||CASE WHEN T3.SYS_LOC_NAME_ID1 IS NULL THEN '' ELSE ' '||T3.SYS_LOC_NAME_ID1 END     "
            + "  	||CASE WHEN T3.SYS_LOC_NAME_ID2 IS NULL THEN '' ELSE ' '||T3.SYS_LOC_NAME_ID2 END     "
            + "  	||CASE WHEN T3.SYS_LOC_NAME_ID3 IS NULL THEN '' ELSE ' '||T3.SYS_LOC_NAME_ID3 END     "
            + " ,2) AS KEYWORD                                                                            "
            + " , T4.ST_NAME AS STREETNAME                                                                "
            + " , T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT AS STREETNO                              "
            + " , T1.BLDG_NAME_E2 AS BUILDINGNAME                                                         "
            + "   , t5.FREF_SEQ , t5.FREF_YR                                                                                        "
            + " FROM MWMS_BLK T1                                                                          "
            + " , MWMS_BLK_SL T2                                                                          "
            + " , MWMS_ST_LOC T3                                                                          "
            + " , MWMS_ST_NAME T4                                                                         "

             + " , MWMS_BLK_FILEREF t5         "
           
            + " WHERE T1.ADR_BLK_ID = T2.ADR_BLK_ID   and t5.ADR_BLK_ID = T1.ADR_BLK_ID                                                    "
            + " AND T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                                                   "
            + " AND T3.SYS_ST_NAME_ID = T4.SYS_ST_NAME_ID)                                                ";
            
            string whereQ = "WHERE 1=1";
            /*if (!string.IsNullOrWhiteSpace(FULL))
            {
                if (FULL.Length > 2) model.Rpp = -1;
                whereQ += " AND KEYWORD LIKE :KEYWORD";
                model.QueryParameters.Add("KEYWORD","%" + FULL.Trim().ToUpper() + "%");
            }*/
            /*
            if (!string.IsNullOrWhiteSpace(streetName))
            {
                
               
                if (streetName.Length > 2) model.Rpp = -1;
                whereQ += " AND T4.ST_NAME LIKE :STREETNAME";
                model.QueryParameters.Add("STREETNAME",  streetName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(streetNo))
            {
                
                model.Rpp = -1;
                whereQ += " AND T2.ST_NO_NUM || T2.ST_NO_ALPHA || T2.ST_NO_EXT LIKE :STREETNO";
                model.QueryParameters.Add("STREETNO", streetNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(building))
            {
                
                if (building.Length > 2) model.Rpp = -1;
                whereQ += " AND T1.BLDG_NAME_E2 LIKE :BUILDINGNAME";
                model.QueryParameters.Add("BUILDINGNAME",  building.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(blockId))
            {
                if (blockId.Length > 2) model.Rpp = -1;
                whereQ += " AND BLK_ID LIKE :BLK_ID";
                model.QueryParameters.Add("BLK_ID",  blockId.Trim().ToUpper() + "%");
            }*/
           // if(selectField == "")
           // {
           //     selectField = "STREETNAME";
           // }
           //
           // if(streetName != null)
           // selectField = "STREETNAME";
           // if (streetNo != null)
           // {
           //     selectField = " STREETNO, BUILDINGNAME";
           //     model.Sort = "STREETNO";
           // }
           // if (building != null)
           // {
           //     selectField = "  BUILDINGNAME";
           //     model.Sort = "BUILDINGNAME";
           // }

            model.Query =  q;
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }



        
        public List<Dictionary<string, object>> Address42(string FILEREF_FOUR, string FILEREF_TWO, string blockId)
        {
            if (string.IsNullOrWhiteSpace(blockId)) return null;
            DisplayGrid model = new DisplayGrid();
            model.Rpp = -1;
            string q = "";
            string whereQ = "";
            string fun = "";
            if (FILEREF_FOUR != null) fun = "FILEREF_FOUR";
            if (FILEREF_TWO != null) fun = "FILEREF_TWO";
            q = q
               + "\r\n\t" + " SELECT DISTINCT  FREF_SEQ AS FILEREF_FOUR, FREF_YR AS FILEREF_TWO  FROM MWMS_BLK_FILEREF WHERE ADR_BLK_ID  =:BLK_ID                    ";
            // + "\r\n\t" + " WHERE ADR_BLK_ID = :BLK_ID                                                                            "
            // + "\r\n\t" + "                                                                                                       "
            //+ "\r\n\t" + " ORDER BY LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0)   ";

            model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper());
            /*  
            if (fun == "FILEREF_FOUR")
            {


                q = q
                + "\r\n\t" + " SELECT  FREF_SEQ, FREF_YR  FROM MWMS_BLK_FILEREF WHERE ADR_BLK_ID  =:BLK_ID                    ";
                 // + "\r\n\t" + " WHERE ADR_BLK_ID = :BLK_ID                                                                            "
                // + "\r\n\t" + "                                                                                                       "
                //+ "\r\n\t" + " ORDER BY LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0)   ";

                model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper());
               // whereQ += " WHERE ADR_BLK_ID = :BLK_ID";
               // model.Sort = "LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0)";

               // model.QueryParameters.Add("FLOOR", floor.Trim().ToUpper() + "%");

            }
            else
            if (fun == "FILEREF_TWO")
            {
                q = q
                + "\r\n\t" + " SELECT DISTINCT     ADR_UNIT_ID,                                                                                                          "
                + "\r\n\t" + " LPAD(UNT_NO_NUM, 10, 0) || LPAD(UNT_NO_ALPHA, 10, 0) || LPAD(UNT_NO_A_PREC, 10, 0) || LPAD(UNT_NO_SUF, 10, 0) AS UNITORD,     "
                + "\r\n\t" + " UNT_NO_NUM|| UNT_NO_ALPHA|| UNT_NO_SUF AS UNIT                                                                                             "
                + "\r\n\t" + " FROM MWMS_UNIT                                                                                                                ";


                whereQ += " WHERE ADR_BLK_ID = :BLK_ID                                                                                                     ";
                model.Sort = " LPAD(UNT_NO_NUM, 10, 0) || LPAD(UNT_NO_ALPHA, 10, 0) || LPAD(UNT_NO_A_PREC, 10, 0) || LPAD(UNT_NO_SUF, 10, 0)        ";
                model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper());

                if (!string.IsNullOrWhiteSpace(floor))
                {
                    whereQ += " AND SYS_FLR_ALPHA_E_ID || FLR_NUM || FLR_ALPHA_E_SUFFIX = :FLOOR";
                    model.QueryParameters.Add("FLOOR", floor.Trim().ToUpper());
                }

                whereQ += " AND UNT_NO_NUM || UNT_NO_ALPHA || UNT_NO_SUF LIKE :UNIT";
                model.QueryParameters.Add("UNIT", unit.Trim().ToUpper() + "%");




            }*/
            if (string.IsNullOrWhiteSpace(q)) return null;
            model.Query = q;
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }


        public List<Dictionary<string, object>> AddressFullUnit(  string streetName, string streetNo, string building, string blockId, string floor, string unit)
        {
            if (string.IsNullOrWhiteSpace(blockId)) return null;
            DisplayGrid model = new DisplayGrid();
            model.Rpp = -1;
            string q = "";
            string whereQ = "";
            string fun = "";
            if (floor != null) fun = "FLOOR";
            if (unit != null) fun = "UNIT";
                if (fun =="FLOOR")
            {
                q = q
                + "\r\n\t" + " SELECT DISTINCT                                                                                       "
                + "\r\n\t" + " LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0) AS FLOORORD,           "
                + "\r\n\t" + " SYS_FLR_ALPHA_E_ID || FLR_NUM || FLR_ALPHA_E_SUFFIX        as FLOOR                                           "
                + "\r\n\t" + "                                                                                                       "
                + "\r\n\t" + " FROM MWMS_UNIT                                                                                        ";
               // + "\r\n\t" + " WHERE ADR_BLK_ID = :BLK_ID                                                                            "
               // + "\r\n\t" + "                                                                                                       "
                //+ "\r\n\t" + " ORDER BY LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0)   ";

                model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper());
                whereQ += " WHERE ADR_BLK_ID = :BLK_ID AND SYS_FLR_ALPHA_E_ID || FLR_NUM || FLR_ALPHA_E_SUFFIX LIKE :FLOOR";
                model.Sort = "LPAD(SYS_FLR_ALPHA_E_ID, 10, 0) || LPAD(FLR_NUM, 10, 0) || LPAD(FLR_ALPHA_E_SUFFIX, 10, 0)";

                model.QueryParameters.Add("FLOOR", floor.Trim().ToUpper() + "%");

             }
            else
                if (fun == "UNIT")
            {
                q=q
                +"\r\n\t" + " SELECT DISTINCT     ADR_UNIT_ID,                                                                                                          "
                + "\r\n\t" + " LPAD(UNT_NO_NUM, 10, 0) || LPAD(UNT_NO_ALPHA, 10, 0) || LPAD(UNT_NO_A_PREC, 10, 0) || LPAD(UNT_NO_SUF, 10, 0) AS UNITORD,     "
                + "\r\n\t" + " UNT_NO_NUM|| UNT_NO_ALPHA|| UNT_NO_SUF AS UNIT                                                                                             "
                + "\r\n\t" + " FROM MWMS_UNIT                                                                                                                ";


                whereQ += " WHERE ADR_BLK_ID = :BLK_ID                                                                                                     ";
                model.Sort = " LPAD(UNT_NO_NUM, 10, 0) || LPAD(UNT_NO_ALPHA, 10, 0) || LPAD(UNT_NO_A_PREC, 10, 0) || LPAD(UNT_NO_SUF, 10, 0)        ";
                model.QueryParameters.Add("BLK_ID", blockId.Trim().ToUpper());

                if (!string.IsNullOrWhiteSpace(floor))
                {
                    whereQ += " AND SYS_FLR_ALPHA_E_ID || FLR_NUM || FLR_ALPHA_E_SUFFIX = :FLOOR";
                    model.QueryParameters.Add("FLOOR", floor.Trim().ToUpper());
                }

                whereQ += " AND UNT_NO_NUM || UNT_NO_ALPHA || UNT_NO_SUF LIKE :UNIT";
                model.QueryParameters.Add("UNIT", unit.Trim().ToUpper() + "%");



               
            }
            if (string.IsNullOrWhiteSpace(q)) return null;
            model.Query = q;
            
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }








        public List<Dictionary<string, object>> AddressStreetName(string ST_NAME)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT ST_NAME FROM MWMS_ST_NAME",
                Rpp = 50,
                Sort = "ST_NAME"
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += " AND ST_NAME LIKE :ST_NAME";
                model.QueryParameters.Add("ST_NAME", ST_NAME.Trim().ToUpper() + "%");
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }

        public List<Dictionary<string,object>> CERTIFICATION_NO(string certNo)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "Select distinct CERTIFICATION_NO ,CHINESE_NAME , SURNAME ||' ' ||GIVEN_NAME AS FULLNAME  ,CONTACT_NO,FAX_NO, EXPIRY_DATE, AS_CHINESE_NAME,AS_SURNAME ||' '|| AS_GIVEN_NAME AS ASFULLNAME  from B_CRM_PBP_PRC "
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(certNo))
            {
                whereQ += " AND UPPER(CERTIFICATION_NO) LIKE :certNo";
                model.QueryParameters.Add("certNo", "%" + certNo.Trim().ToUpper() + "%");
            }
            model.QueryWhere = whereQ;
            model.Search();
        
            return model.Data;
            
        }


        public List<Dictionary<string, object>> AddressStreetNumber(string ST_NAME, string ST_NO)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT T4.BLK_NO, T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT AS STREETNO, T4.BLDG_NAME_E1 AS BUILDING, T5.CODE AS REGION_CODE, T6.ENGLISH_DESCRIPTION AS DISTRICT, T6.CODE AS DISTRICT_CODE, T4.ADR_BLK_ID, T4.SYS_REGION_ID, T4.SYS_DISTRICT_ID, AREA_ID                         "

                + "\r\n" + " FROM MWMS_ST_NAME T1                                                                    "
                + "\r\n" + " INNER JOIN MWMS_ST_LOC T2 ON T1.SYS_ST_NAME_ID = T2.SYS_ST_NAME_ID                      "
                + "\r\n" + " INNER JOIN MWMS_BLK_SL T3 ON T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                        "
                + "\r\n" + " INNER JOIN MWMS_BLK T4 ON T3.ADR_BLK_ID = T4.ADR_BLK_ID                                 "


                + "\r\n" + " INNER JOIN MWMS_ADDRESS_META_DATA T5 ON T4.SYS_REGION_ID = T5.SYS_META_DATA_ID             "
                + "\r\n" + " INNER JOIN MWMS_ADDRESS_META_DATA T6 ON T4.SYS_DISTRICT_ID = T6.SYS_META_DATA_ID             ",



            //SYS_META_DATA_ID = SYS_REGION_ID
                Rpp = 50,
                Sort = "1,2"
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += "\r\n" + " AND T1.ST_NAME = :ST_NAME";
                model.QueryParameters.Add("ST_NAME", ST_NAME);
            }
            if (!string.IsNullOrWhiteSpace(ST_NO))
            {
                whereQ += "\r\n" + " AND T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT LIKE  :ST_NO";
                model.QueryParameters.Add("ST_NO", (ST_NO.Trim().ToUpper() + "%"));
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }
        
        public List<Dictionary<string, object>> AddressBuilding(string ST_NAME, string ST_NO, string BUILDING)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT T4.BLDG_NAME_E1 AS BUILDING, T5.CODE AS REGION_CODE, T6.ENGLISH_DESCRIPTION AS DISTRICT, T6.CODE AS DISTRICT_CODE, T4.BLK_NO, T4.ADR_BLK_ID, T4.SYS_REGION_ID, T4.SYS_DISTRICT_ID, T4.AREA_ID                         "
                + "\r\n" + " , T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT AS STREETNO "
                + "\r\n" + " FROM MWMS_ST_NAME T1                                                                    "
                + "\r\n" + " INNER JOIN MWMS_ST_LOC T2 ON T1.SYS_ST_NAME_ID = T2.SYS_ST_NAME_ID                      "
                + "\r\n" + " INNER JOIN MWMS_BLK_SL T3 ON T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                        "
                + "\r\n" + " INNER JOIN MWMS_BLK T4 ON T3.ADR_BLK_ID = T4.ADR_BLK_ID                                 "
                + "\r\n" + " INNER JOIN MWMS_ADDRESS_META_DATA T5 ON T4.SYS_REGION_ID = T5.SYS_META_DATA_ID             "
                + "\r\n" + " INNER JOIN MWMS_ADDRESS_META_DATA T6 ON T4.SYS_DISTRICT_ID = T6.SYS_META_DATA_ID             ",
                Rpp = 50,
                Sort = "1"
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += "\r\n" + " AND T1.ST_NAME = :ST_NAME";
                model.QueryParameters.Add("ST_NAME", ST_NAME);
            }
            if (!string.IsNullOrWhiteSpace(ST_NO))
            {
                whereQ += "\r\n" + " AND T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT LIKE  :ST_NO";
                model.QueryParameters.Add("ST_NO", (ST_NO.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(BUILDING))
            {
                whereQ += "\r\n" + " AND T4.BLDG_NAME_E1 LIKE  :BUILDING";
                model.QueryParameters.Add("BUILDING", (BUILDING.Trim().ToUpper() + "%"));
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }

        public List<Dictionary<string, object>> AddressFloor(string ST_NAME, string ST_NO, string BUILDING, string FLOOR)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT T5.SYS_FLR_ALPHA_E_ID||T5.FLR_NUM||T5.FLR_ALPHA_E_SUFFIX AS FLOOR, T4.ADR_BLK_ID, T4.SYS_REGION_ID, T4.SYS_DISTRICT_ID, T4.AREA_ID                         "
                + "\r\n" + " , T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT AS STREETNO, T4.BLDG_NAME_E1 AS BUILDING "
                + "\r\n" + " FROM MWMS_ST_NAME T1                                                                    "
                + "\r\n" + " INNER JOIN MWMS_ST_LOC T2 ON T1.SYS_ST_NAME_ID = T2.SYS_ST_NAME_ID                      "
                + "\r\n" + " INNER JOIN MWMS_BLK_SL T3 ON T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                        "
                + "\r\n" + " INNER JOIN MWMS_BLK T4 ON T3.ADR_BLK_ID = T4.ADR_BLK_ID                                 "
                + "\r\n" + " INNER JOIN MWMS_UNIT T5 ON T4.ADR_BLK_ID = T5.ADR_BLK_ID                                ",
                Rpp = 50,
                Sort = "1,2"
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += "\r\n" + " AND T1.ST_NAME = :ST_NAME";
                model.QueryParameters.Add("ST_NAME", ST_NAME);
            }
            if (!string.IsNullOrWhiteSpace(ST_NO))
            {
                whereQ += "\r\n" + " AND T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT LIKE  :ST_NO";
                model.QueryParameters.Add("ST_NO", (ST_NO.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(BUILDING))
            {
                whereQ += "\r\n" + " AND T4.BLDG_NAME_E1 LIKE  :BUILDING";
                model.QueryParameters.Add("BUILDING", (BUILDING.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(FLOOR))
            {
                whereQ += "\r\n" + " AND T5.SYS_FLR_ALPHA_E_ID||T5.FLR_NUM||T5.FLR_ALPHA_E_SUFFIX LIKE  :FLOOR";
                model.QueryParameters.Add("FLOOR", (FLOOR.Trim().ToUpper() + "%"));
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }

        public List<Dictionary<string, object>> AddressUnit(string ST_NAME, string ST_NO, string BUILDING, string FLOOR, string UNIT)
        {
            DisplayGrid model = new DisplayGrid
            {
                Query = "SELECT DISTINCT T5.SYS_FLR_ALPHA_E_ID||T5.FLR_NUM||T5.FLR_ALPHA_E_SUFFIX AS FLOOR"
                + "\r\n" + " ,T5.UNT_NO_NUM||T5.UNT_NO_ALPHA||T5.UNT_NO_SUF AS UNIT" 
//             + "\r\n" + " ,T5.UNT_DESC_E_ID||T5.UNT_NO_NUM||T5.UNT_NO_ALPHA||T5.UNT_NO_A_PREC||T5.UNT_NO_SUF AS UNIT"

                + "\r\n" + " ,T5.ADR_UNIT_ID, T4.ADR_BLK_ID, T4.SYS_REGION_ID, T4.SYS_DISTRICT_ID, T4.AREA_ID                         "
                + "\r\n" + " , T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT AS STREETNO, T4.BLDG_NAME_E1 AS BUILDING "
                + "\r\n" + " FROM MWMS_ST_NAME T1                                                                    "
                + "\r\n" + " INNER JOIN MWMS_ST_LOC T2 ON T1.SYS_ST_NAME_ID = T2.SYS_ST_NAME_ID                      "
                + "\r\n" + " INNER JOIN MWMS_BLK_SL T3 ON T2.SYS_ST_LOC_ID = T3.SYS_ST_LOC_ID                        "
                + "\r\n" + " INNER JOIN MWMS_BLK T4 ON T3.ADR_BLK_ID = T4.ADR_BLK_ID                                 "
                + "\r\n" + " INNER JOIN MWMS_UNIT T5 ON T4.ADR_BLK_ID = T5.ADR_BLK_ID                                ",
                Rpp = 50,
                Sort = "1,2"
            };
            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += "\r\n" + " AND T1.ST_NAME = :ST_NAME";
                model.QueryParameters.Add("ST_NAME", ST_NAME);
            }
            if (!string.IsNullOrWhiteSpace(ST_NO))
            {
                whereQ += "\r\n" + " AND T3.ST_NO_NUM || T3.ST_NO_ALPHA || T3.ST_NO_EXT LIKE  :ST_NO";
                model.QueryParameters.Add("ST_NO", (ST_NO.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(BUILDING))
            {
                whereQ += "\r\n" + " AND T4.BLDG_NAME_E1 LIKE  :BUILDING";
                model.QueryParameters.Add("BUILDING", (BUILDING.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(FLOOR))
            {
                whereQ += "\r\n" + " AND T5.SYS_FLR_ALPHA_E_ID||T5.FLR_NUM||T5.FLR_ALPHA_E_SUFFIX LIKE  :FLOOR";
                model.QueryParameters.Add("FLOOR", (FLOOR.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(UNIT))
            {
                whereQ += "\r\n" + " AND T5.UNT_DESC_E_ID||T5.UNT_NO_NUM||T5.UNT_NO_ALPHA||T5.UNT_NO_A_PREC||T5.UNT_NO_SUF LIKE  :UNIT";
                model.QueryParameters.Add("UNIT", (UNIT.Trim().ToUpper() + "%"));
            }
            
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }

        //UNT_DESC_E_ID, UNT_NO_NUM, UNT_NO_ALPHA, UNT_NO_A_PREC, UNT_NO_SUF


        /*
               + "\r\n" + " INNER JOIN MWMS_UNIT T5 ON T4.ADR_BLK_ID = T5.ADR_BLK_ID                                ",
                T5.SYS_FLR_ALPHA_E_ID, T5.FLR_NUM, T5.FLR_ALPHA_E_SUFFIX
                */




        public List<Dictionary<string, object>> StructuralAddress(string[] selectColumns, string[] orderByColumns
           , bool eqUNIT, string UNIT
           , bool eqFLOOR, string FLOOR
           , bool eqBUILDING, string BUILDING
           , bool eqSTREETNO, string STREETNO
           , bool eqST_NAME, string ST_NAME
           , bool eqSYS_DISTRICT, string SYS_DISTRICT
           , string ADR_UNIT_ID
           , string ADR_BLK_ID)
        {

            DisplayGrid model = new DisplayGrid
            {
                Query = " SELECT DISTINCT " + string.Join(",", selectColumns) + " FROM BCIS_ADDRESS",
                Rpp = 50,
                Sort = string.Join(",", orderByColumns)
            };

            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(UNIT))
            {

                whereQ += " AND UNIT " + (eqUNIT ? "=" : "LIKE") + " :UNIT"; model.QueryParameters.Add("UNIT", eqUNIT ? UNIT.Trim().ToUpper() : (UNIT.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(FLOOR))
            {
                whereQ += " AND FLOOR " + (eqFLOOR ? "=" : "LIKE") + " :FLOOR"; model.QueryParameters.Add("FLOOR", eqFLOOR ? FLOOR.Trim().ToUpper() : (FLOOR.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(BUILDING))
            {
                whereQ += " AND BUILDING " + (eqBUILDING ? "=" : "LIKE") + " :BUILDING"; model.QueryParameters.Add("BUILDING", eqBUILDING ? BUILDING.Trim().ToUpper() : (BUILDING.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(STREETNO))
            {
                whereQ += " AND STREETNO " + (eqSTREETNO ? "=" : "LIKE") + " :STREETNO"; model.QueryParameters.Add("STREETNO", eqSTREETNO ? STREETNO.Trim().ToUpper() : (STREETNO.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(ST_NAME))
            {
                whereQ += " AND ST_NAME " + (eqST_NAME ? "=" : "LIKE") + " :ST_NAME"; model.QueryParameters.Add("ST_NAME", eqST_NAME ? ST_NAME.Trim().ToUpper() : (ST_NAME.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(SYS_DISTRICT))
            {
                whereQ += " AND SYS_DISTRICT " + (eqSYS_DISTRICT ? "=" : "LIKE") + " :SYS_DISTRICT"; model.QueryParameters.Add("SYS_DISTRICT", eqSYS_DISTRICT ? SYS_DISTRICT.Trim().ToUpper() : (SYS_DISTRICT.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(ADR_UNIT_ID))
            {
                whereQ += " AND ADR_UNIT_ID = :ADR_UNIT_ID"; model.QueryParameters.Add("ADR_UNIT_ID", ADR_UNIT_ID.Trim().ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(ADR_BLK_ID))
            {
                whereQ += " AND ADR_BLK_ID = :ADR_BLK_ID"; model.QueryParameters.Add("ADR_BLK_ID", ADR_BLK_ID.Trim().ToUpper());
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }


        public List<Dictionary<string, object>> SysRank(AtcpModel model)
        {
            model.Query = ""
            + "\r\n\t" + " SELECT CASE                                      "
            + "\r\n\t" + " WHEN RANK_GROUP = 'CPO' THEN 0                   "
            + "\r\n\t" + " WHEN RANK_GROUP = 'SPO' THEN 1                   "
            + "\r\n\t" + " WHEN RANK_GROUP = 'PO' THEN 2                    "
            + "\r\n\t" + " WHEN RANK_GROUP = 'TO' THEN 3                    "
            + "\r\n\t" + " WHEN RANK_GROUP = 'TC' THEN 4                    "
            + "\r\n\t" + " ELSE 999 END                                     "
            + "\r\n\t" + " AS ORD, UUID, CODE, DESCRIPTION, RANK_GROUP      "
            + "\r\n\t" + " FROM SYS_RANK                                    ";



            if (!string.IsNullOrWhiteSpace(model.id))
            {
                model.QueryWhere = "WHERE UUID = :id";
                model.QueryParameters.Add("id", model.id);
            }
            else if (!string.IsNullOrWhiteSpace(model.term))
            {
                model.QueryWhere = "WHERE CODE LIKE :k1 OR DESCRIPTION LIKE :k2 OR RANK_GROUP LIKE :k3";
                model.QueryParameters.Add("k1", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k2", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k3", model.term.ToUpper() + "%");
            }
            model.Rpp = -1;
            model.Sort = "1";
            model.Search();
            return model.Data;
        }

        public List<Dictionary<string, object>> QSearchKeyword(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.term)) return null;
            if (!model.atcpParameters.ContainsKey("selectedType")) return null;
            string q = "";
            q = q + " SELECT DISTINCT KEYWORD";
            if (model.atcpParameters["selectedType"] == "1") q = q + " FROM SYS_QUICK_SEARCH";
            else q = q + " FROM SYS_QUICK_SEARCH_MW";
            q = q + " WHERE KEYWORD_TYPE !='HKID' and KEYWORD_TYPE != 'PASSPORT' and  UPPER(KEYWORD) LIKE :k ";
            model.Query = q;
            model.QueryParameters.Add("k",  model.term.ToUpper() + "%");

            model.Rpp = 10;
            model.Sort = "KEYWORD";
            model.Search();
            return model.Data;
        }
        public List<Dictionary<string, object>> SysUnit(AtcpModel model)
        {


            model.Query = "SELECT T1.UUID, T2.CODE AS SECTION, T1.CODE, T1.DESCRIPTION FROM SYS_UNIT T1 LEFT JOIN SYS_SECTION T2 ON T2.UUID = T1.SYS_SECTION_ID";

            if (!string.IsNullOrWhiteSpace(model.id))
            {
                model.QueryWhere = "WHERE T1.UUID = :id";
                model.QueryParameters.Add("id", model.id);
            }
            else if (!string.IsNullOrWhiteSpace(model.term))
            {
                model.QueryWhere = "WHERE T2.CODE LIKE :k1 OR T1.CODE LIKE :k2 OR UPPER(T1.DESCRIPTION) LIKE :k3";
                model.QueryParameters.Add("k1", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k2", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k3", model.term.ToUpper() + "%");
            }
            model.Rpp = 50;
            model.Sort = "1, 3, 4, 2";
            model.Search();
            return model.Data;
        }
        public List<Dictionary<string, object>> ValidationTemplate(AtcpModel model)
        {

            List<SelectListItem> ggg =
                SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SV_Validation, 0);
            return ggg.Select(o => new Dictionary<string, object>() { ["code"] = o.Text }).ToList();

        }//LetterRefuse
        public List<Dictionary<string, object>> LetterRefuse(AtcpModel model)
        {

            List<SelectListItem> ggg =
                SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SYSTEM_TYPE_REASON_FOR_REFUSE, 0);
            return ggg.Select(o => new Dictionary<string, object>() { ["description"] = o.Text }).ToList();

        }


        
        public List<SYS_POST> AtcpScuSubordinateMembers(AtcpModel model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                string RECVERSION = model.atcpParameters["RECVERSION"];
                int intRECVERSION = -10;
                Int32.TryParse(RECVERSION, out intRECVERSION);
                return db.SYS_POST
                    .Where(o => o.SYS_RANK.RECVERSION == intRECVERSION + 1 || intRECVERSION == 0)
                    .Where(o => o.SYS_UNIT.CODE == "SU")
                    .Where(
                        o => (
                            o.UUID == model.id
                            ||
                            ((model.id == null || model.id == "") && o.IS_ACTIVE == "Y" && o.CODE.Contains(model.term))
                        )
                    )
                    .OrderBy(o => o.CODE)
                    .ToList();
            }
        }




        public List<SYS_POST> AtcpPemSubordinateMembers(AtcpModel model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.SYS_POST
                    .Where(
                        o => (
                            o.UUID == model.id 
                            || 
                            ((model.id == null || model.id == "")&& o.IS_ACTIVE == "Y" && o.CODE.Contains(model.term))
                        )
                    )
                    .OrderBy(o => o.CODE).Take(50)//take all will lag
                    .ToList();


            }
        }


        public List<Dictionary<string, object>> AtcpScuResponsibleAreas(AtcpModel model)
        {
            model.Query = "SELECT CODE AS AREA_CODE, ENGLISH_DESCRIPTION AS AREA_DESC FROM MWMS_ADDRESS_META_DATA  ";
            string whereQ = "WHERE REC_TYPE =  'AREA'";

            if (!string.IsNullOrWhiteSpace(model.term))
            {
                whereQ += "\r\n" + " AND CODE LIKE :CODE";
                model.QueryParameters.Add("CODE", model.term.ToUpper() + "%");
            }
            else if (!string.IsNullOrWhiteSpace(model.id))
            {
                whereQ += "\r\n" + " AND CODE = :CODE";
                model.QueryParameters.Add("CODE", model.id);
            }
            model.QueryWhere = whereQ;
            model.Sort = "CODE";
            model.Search();
            model.Rpp = -1;
            return model.Data;
        }

        public List<B_S_SYSTEM_VALUE> B_S_SYSTEM_TYPE_List(string type)
        {
            using (EntitiesSignboard db = new EntitiesSignboard()) return db.B_S_SYSTEM_VALUE.Where(o => o.B_S_SYSTEM_TYPE.TYPE == type && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).ToList();
        }
    }
}