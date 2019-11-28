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
using MWMS2.Constant;
using System.Data.Entity.Validation;

namespace MWMS2.Services
{
    public class AdminCRMService
    {


        String SearchAR_q = ""
               + "\r\n" + "\t" + "select "
               + "\r\n" + "\t" + " T2.UUID, CODE "
               + "\r\n" + "\t" + ", REGISTRATION_TYPE "
               + "\r\n" + "\t" + ", ENGLISH_DESCRIPTION "
               + "\r\n" + "\t" + ", CHINESE_DESCRIPTION "
               + "\r\n" + "\t" + ", ORDERING "
               + "\r\n" + "\t" + ", IS_ACTIVE "
               + "\r\n" + "\t" + "from C_S_SYSTEM_TYPE T1 "
               + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE T2 on T2.SYSTEM_TYPE_ID = T1.UUID "
               + "\r\n" + "\t" + "where T1.TYPE ='APPLICANT_ROLE' ";

        String SearchAS_q = ""
              + "\r\n" + "\t" + "select "
              + "\r\n" + "\t" + "T2.UUID "
              + "\r\n" + "\t" + ", CODE "
              + "\r\n" + "\t" + ", ENGLISH_DESCRIPTION "
              + "\r\n" + "\t" + ", CHINESE_DESCRIPTION "
              + "\r\n" + "\t" + ", ORDERING "
              + "\r\n" + "\t" + ", IS_ACTIVE "
              + "\r\n" + "\t" + "from C_S_SYSTEM_TYPE T1 "
              + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE T2 on T2.SYSTEM_TYPE_ID = T1.UUID "
              + "\r\n" + "\t" + "where T1.TYPE ='APPLICANT_STATUS' ";

        String SearchAF_q = ""
                + "\r\n" + "\t" + "select "
                + "\r\n" + "\t" + " T2.UUID "
                + "\r\n" + "\t" + ", CODE "
                + "\r\n" + "\t" + ", REGISTRATION_TYPE "
                + "\r\n" + "\t" + ", ENGLISH_DESCRIPTION "
                + "\r\n" + "\t" + ", CHINESE_DESCRIPTION "
                + "\r\n" + "\t" + ", ORDERING "
                + "\r\n" + "\t" + ", IS_ACTIVE "
                + "\r\n" + "\t" + "from C_S_SYSTEM_TYPE T1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE T2 on T2.SYSTEM_TYPE_ID = T1.UUID "
                + "\r\n" + "\t" + "where T1.TYPE ='APPLICATION_FORM' ";

        //BETTER SQL!!!
        // select sv.* ,
        //(select ENGLISH_DESCRIPTION from  C_S_SYSTEM_VALUE inner join c_S_SYSTEM_TYPE
        //on C_S_SYSTEM_VALUE.SYSTEM_TYPE_ID = c_S_SYSTEM_TYPE.uuid  where TYPE = 'REGISTRATION_TYPE' and code = sv.REGISTRATION_TYPE
        //) as test
        //from C_S_SYSTEM_VALUE sv
        //inner
        //join c_S_SYSTEM_TYPE st
        //on sv.SYSTEM_TYPE_ID = st.uuid
        //where type = 'APPLICANT_ROLE'

        public string ExportAR(CRM_ARModel model)
        {
            model.Query = SearchAR_q;
            return model.Export("ExportData");
        }

        public string ExportAF(CRM_AFModel model)
        {
            model.Query = SearchAR_q;
            return model.Export("ExportData");
        }

        public string ExportAS(CRM_ASModel model)
        {
            model.Query = SearchAR_q;
            return model.Export("ExportData");
        }

        //public CRM_ARModel SaveAR(CRM_ARDisplayModel model)
        //{
        //    model.Query = SearchAR_q;
        //    model.Search();
        //    return model;
        //}

        public CRM_ARModel SearchAR(CRM_ARModel model)
        {
            model.Query = SearchAR_q;
            model.Search();
            return model;
        }

        public CRM_ASModel SearchAS(CRM_ASModel model)
        {
            model.Query = SearchAS_q;
            model.Search();
            return model;
        }
        public string ExcelAS(CRM_ASModel model)
        {
            model.Query = SearchAS_q;
            return model.Export("ExportData");
        }

        public CRM_AFModel SearchAF(CRM_AFModel model)
        {
            model.Query = SearchAF_q;
            model.Search();
            return model;
        }

        public string ExcelAF(CRM_AFModel model)
        {
            model.Query = SearchAF_q;
            return model.Export("ExportData");
        }

        public CRM_Model SearchSystemValue(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " T2.UUID "
                + "\r\n" + "\t" + ", CODE "
                + "\r\n" + "\t" + ", REGISTRATION_TYPE "
                + "\r\n" + "\t" + ", ENGLISH_DESCRIPTION "
                + "\r\n" + "\t" + ", CHINESE_DESCRIPTION "
                + "\r\n" + "\t" + ", ORDERING "
                + "\r\n" + "\t" + ", IS_ACTIVE "
                + "\r\n" + "\t" + "from C_S_SYSTEM_TYPE T1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE T2 on T2.SYSTEM_TYPE_ID = T1.UUID ";
            model.QueryWhere = " where T1.TYPE = :TYPE ";
            model.QueryParameters.Add("TYPE", model.SystemType);
            model.Search();
            return model;
        }

        public CRM_Model SearchAN(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " UUID "
                + "\r\n" + "\t" + ", ENGLISH_NAME "
                + "\r\n" + "\t" + ", CHINESE_NAME "
                + "\r\n" + "\t" + ", ENGLISH_RANK "
                + "\r\n" + "\t" + ", CHINESE_RANK "
                + "\r\n" + "\t" + ", ENGLISH_ACTION_NAME "
                + "\r\n" + "\t" + ", CHINESE_ACTION_NAME "
                + "\r\n" + "\t" + ", ENGLISH_ACTION_RANK "
                + "\r\n" + "\t" + ", CHINESE_ACTION_RANK "
                + "\r\n" + "\t" + "from C_S_AUTHORITY ";

            model.Search();
            return model;
        }

        public CRM_Model SearchBSCI(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " sbsi.UUID, sv.REGISTRATION_TYPE as REGISTRATION_TYPE, sc.CODE as CATEGORY_CODE "
                + "\r\n" + "\t" + ", sv.CODE as BUILDING_SAFETY_CODE, sbsi.ENGLISH_HTML_HEADER as ENGLISH_HTML_HEADER , sbsi.CHINESE_HTML_HEADER as CHINESE_HTML_HEADER "
                + "\r\n" + "\t" + "from C_S_BUILDING_SAFETY_ITEM sbsi "
                + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.UUID = sbsi.BUILDING_SAFETY_ID "
                + "\r\n" + "\t" + "left join C_S_CATEGORY_CODE sc on sc.UUID = sbsi.CATEGORY_ID ";

            model.Search();
            return model;
        }
        public string ExcelBSCI(CRM_Model model)
        {
            model.Query = "select "
               + "\r\n" + "\t" + " sbsi.UUID, sv.REGISTRATION_TYPE as REGISTRATION_TYPE, sc.CODE as CATEGORY_CODE "
               + "\r\n" + "\t" + ", sv.CODE as BUILDING_SAFETY_CODE, sbsi.ENGLISH_HTML_HEADER as ENGLISH_HTML_HEADER , sbsi.CHINESE_HTML_HEADER as CHINESE_HTML_HEADER "
               + "\r\n" + "\t" + "from C_S_BUILDING_SAFETY_ITEM sbsi "
               + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.UUID = sbsi.BUILDING_SAFETY_ID "
               + "\r\n" + "\t" + "left join C_S_CATEGORY_CODE sc on sc.UUID = sbsi.CATEGORY_ID ";

            return model.Export("ExportData");
        }

        public void GetSearchSystemValueWithParentTypeSql(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " sv1.UUID as UUID, sv1.CODE as CODE, sv2.CODE as PANEL "
                + "\r\n" + "\t" + ", case when sv1.REGISTRATION_TYPE = 'CGC' then 'General Contractor' "
                + "\r\n" + "\t" + "when sv1.REGISTRATION_TYPE = 'IP' then 'Professional' "
                + "\r\n" + "\t" + "else 'MW' end as REGISTRATION_TYPE "
                + "\r\n" + "\t" + ", sv1.ENGLISH_DESCRIPTION as ENGLISH_DESCRIPTION "
                + "\r\n" + "\t" + ", sv1.CHINESE_DESCRIPTION as CHINESE_DESCRIPTION "
                + "\r\n" + "\t" + ", sv1.ORDERING as ORDERING, sv1.IS_ACTIVE as IS_ACTIVE "
                + "\r\n" + "\t" + "from C_S_SYSTEM_VALUE sv1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = sv1.PARENT_ID "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_TYPE st on st.UUID = sv1.SYSTEM_TYPE_ID ";
            model.QueryWhere = " where st.TYPE = :TYPE ";
            model.QueryParameters.Add("TYPE", model.SystemType);
        }

        public CRM_Model SearchSystemValueWithParentType(CRM_Model model)
        {
            GetSearchSystemValueWithParentTypeSql(model);
            model.Search();
            return model;
        }
        public string ExcelSystemValueWithParentType(CRM_Model model)
        {
            GetSearchSystemValueWithParentTypeSql(model);
            return model.Export("ExportData");
        }

        public string ExportMWT(CRM_Model model)
        {
            GetSearchSystemValueWithParentTypeSql(model);
            return model.Export("ExportData");
        }

        public CRM_Model SearchCC(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " c1.UUID, c1.CODE as CODE, c2.CODE as CATEGORY_GROUP "
                + "\r\n" + "\t" + ", c1.ENGLISH_DESCRIPTION, c1.CHINESE_DESCRIPTION  "
                + "\r\n" + "\t" + ", c1.REGISTRATION_TYPE, c1.ENGLISH_SUB_TITLE_DESCRIPTION as ENG_TITLE_DESC "
                + "\r\n" + "\t" + ", c1.CHINESE_SUB_TITLE_DESCRIPTION as CHI_TITLE_DESC "
                + "\r\n" + "\t" + "from C_S_CATEGORY_CODE c1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE c2 on c2.UUID = c1.CATEGORY_GROUP_ID ";

            model.Search();
            return model;
        }

        public string ExcelCC(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " c1.UUID, c1.CODE as CODE, c2.CODE as CATEGORY_GROUP "
                + "\r\n" + "\t" + ", c1.ENGLISH_DESCRIPTION, c1.CHINESE_DESCRIPTION  "
                + "\r\n" + "\t" + ", c1.REGISTRATION_TYPE, c1.ENGLISH_SUB_TITLE_DESCRIPTION as ENG_TITLE_DESC "
                + "\r\n" + "\t" + ", c1.CHINESE_SUB_TITLE_DESCRIPTION as CHI_TITLE_DESC "
                + "\r\n" + "\t" + "from C_S_CATEGORY_CODE c1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE c2 on c2.UUID = c1.CATEGORY_GROUP_ID ";
            return model.Export("ExportData");
        }

        public CRM_Model SearchHN(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " hn.UUID, hn.CODE "
                + "\r\n" + "\t" + ", hn.ENGLISH_DESCRIPTION, hn.CHINESE_DESCRIPTION  "
                + "\r\n" + "\t" + ", hn.ORDER_SEQUENCE, sv.REGISTRATION_TYPE "
                + "\r\n" + "\t" + "from C_S_HTML_NOTES hn "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE sv on sv.UUID = hn.CATEGORY_GROUP_ID ";

            model.Search();
            return model;
        }

        public string ExcelHN(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " hn.UUID, hn.CODE "
                + "\r\n" + "\t" + ", hn.ENGLISH_DESCRIPTION, hn.CHINESE_DESCRIPTION  "
                + "\r\n" + "\t" + ", hn.ORDER_SEQUENCE, sv.REGISTRATION_TYPE "
                + "\r\n" + "\t" + "from C_S_HTML_NOTES hn "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE sv on sv.UUID = hn.CATEGORY_GROUP_ID ";
            return model.Export("ExportData");
        }

        public void GetSearchRDSql(CRM_Model model)
        {
            model.Query = @"SELECT r.UUID,
                                   r.ROOM,
                                   r.TELEPHONE_NO,
                                   r.FAX_NO,
                                   addr.floor
                                   || addr.building_name
                                   || addr.stree_name
                                   || addr.street_no AS ADDRESS
                            FROM   C_S_ROOM r
                                   LEFT JOIN P_MW_ADDRESS addr
                                          ON addr.uuid = r.chinese_address_id 
";
        }

        public CRM_Model SearchRD(CRM_Model model)
        {
            GetSearchRDSql(model);
            model.Search();
            return model;
        }

        public string ExportRD(CRM_Model model)
        {
            GetSearchRDSql(model);
            return model.Export("ExportData");
        }

        public CRM_Model SearchPHM(CRM_Model model)
        {
            //model.Query = "select ROWNUM-1 as ROW_INDEX, ROWNUM  AS ROW_NO , "
            //    + "\r\n" + "\t" + " UUID, HOLIDAY as HDATE, Extract(Year from Holiday) as Year, ENGLISH_DESCRIPTION, CHINESE_DESCRIPTION "
            //    + "\r\n" + "\t" + "from C_S_PUBLIC_HOLIDAY "
            //    + "\r\n" + "\t" + "where 1=1 ";
            model.Query = "select (row_number() over (order by HOLIDAY))-1 as ROW_INDEX, row_number() over (order by HOLIDAY)  AS ROW_NO , "
               + "\r\n" + "\t" + " UUID, HOLIDAY as HDATE, Extract(Year from Holiday) as Year, ENGLISH_DESCRIPTION, CHINESE_DESCRIPTION "
               + "\r\n" + "\t" + "from C_S_PUBLIC_HOLIDAY "
               + "\r\n" + "\t" + "where 1=1 ";

            model.QueryWhere = SearchPHM_whereQ(model);

            model.Search();

            while (model.Data.Count < 20)
            {
                Dictionary<string, object> each = new Dictionary<string, object>();

                each.Add("ROW_INDEX", model.Data.Count);
                each.Add("ROW_NO", model.Data.Count + 1);
                each.Add("UUID", "");
                each.Add("HDATE", "");
                each.Add("Year", "");
                each.Add("ENGLISH_DESCRIPTION", "");
                each.Add("CHINESE_DESCRIPTION", "");
                model.Data.Add(each);
            }


            return model;
        }
        private string SearchPHM_whereQ(CRM_Model model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Year))
            {
                whereQ += "\r\n\t" + "AND UPPER(Extract(Year from Holiday)) LIKE :Year";
                model.QueryParameters.Add("Year", "%" + model.Year.Trim().ToUpper() + "%");
            }

            return whereQ;
        }

        public ServiceResult SavePHM(CRM_Model model)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    int year = Convert.ToInt32(model.Year);
                    var holidayList = (from ph in db.C_S_PUBLIC_HOLIDAY
                                       where ph.HOLIDAY.Year == year
                                       select ph).ToList();
                    foreach(var item in holidayList)
                    {
                        db.C_S_PUBLIC_HOLIDAY.Remove(item);
                    }

                    foreach(var item in model.Publicholidays)
                    {
                        if(item.HOLIDAY != null)
                        {
                            C_S_PUBLIC_HOLIDAY ph = new C_S_PUBLIC_HOLIDAY();
                            ph.HOLIDAY = item.HOLIDAY;
                            ph.ENGLISH_DESCRIPTION = item.ENGLISH_DESCRIPTION;
                            ph.CHINESE_DESCRIPTION = item.CHINESE_DESCRIPTION;
                            ph.CREATED_DATE = DateTime.Now;
                            ph.MODIFIED_DATE = DateTime.Now;
                            db.C_S_PUBLIC_HOLIDAY.Add(ph);
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
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
                    return new ServiceResult
                    {
                        Result = ServiceResult.RESULT_SUCCESS,
                        Message = new List<string>() { "Save Successfully." }
                    };

                }
            }
        }



        public void GetSearchCCDSql(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " ccd.UUID, ccd.CODE, cc.CODE as CATEGORY_CODE, ccd.ENGLISH_DESCRIPTION, ccd.CHINESE_DESCRIPTION "
                + "\r\n" + "\t" + "from C_S_CATEGORY_CODE_DETAIL ccd"
                + "\r\n" + "\t" + "Inner join C_S_CATEGORY_CODE cc on cc.UUID = ccd.S_CATEGORY_CODE_ID "
                + "\r\n" + "\t" + "where 1=1 ";
        }

        public CRM_Model SearchCCD(CRM_Model model)
        {
            GetSearchCCDSql(model);
            model.Search();
            return model;
        }

        public string ExportCCD(CRM_Model model)
        {
            GetSearchCCDSql(model);
            return model.Export("ExportData");
        }

        public void GetSearchLTSql(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " el.UUID, el.CODE, el.LETTER_NO, el.LETTER_NAME, el.LETTER_DESCRIPTION as REMARK "
                + "\r\n" + "\t" + " , CASE WHEN el.REGISTRATION_TYPE = 'CGC' THEN 'General Contractor' WHEN el.REGISTRATION_TYPE = 'IP' THEN 'Professional' "
                + "\r\n" + "\t" + " WHEN el.REGISTRATION_TYPE = 'CMW' THEN 'MW Company' ELSE 'MW Individual' END as REGISTRATION_TYPE "
                + "\r\n" + "\t" + " ,el.ORDERING, el.FILE_NAME, sv.ENGLISH_DESCRIPTION, el.SELECTION_TYPE "
                + "\r\n" + "\t" + " FROM C_S_EXPORT_LETTER el "
                + "\r\n" + "\t" + " INNER JOIN C_S_SYSTEM_VALUE sv on sv.CODE = el.REGISTRATION_TYPE "
                + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_TYPE st on st.UUID = sv.SYSTEM_TYPE_ID "
                + "\r\n" + "\t" + " WHERE st.TYPE = 'REGISTRATION_TYPE' ";
        }

        public CRM_Model SearchLT(CRM_Model model)
        {
            GetSearchLTSql(model);
            model.Search();
            return model;
        }

        public string ExportLT(CRM_Model model)
        {
            GetSearchLTSql(model);
            return model.Export("ExportData");
        }

        public void GetSearchMWISql(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " sv.UUID, sv.CODE as ITEM, sv1.CODE as TYPE, sv2.CODE as CLASS, sv.ENGLISH_DESCRIPTION "
                + "\r\n" + "\t" + " FROM C_S_SYSTEM_VALUE sv "
                + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE sv1 on sv1.UUID = sv.PARENT_ID "
                + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE sv2 on sv2.UUID = sv1.PARENT_ID "
                + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_TYPE st on st.UUID = sv.SYSTEM_TYPE_ID ";

            model.QueryWhere = " where st.TYPE = :TYPE ";
            model.QueryParameters.Add("TYPE", RegistrationConstant.MINOR_WORKS_ITEM);
        }

        public CRM_Model SearchMWI(CRM_Model model)
        {
            GetSearchMWISql(model);
            model.Search();
            return model;
        }

        public string ExportMWI(CRM_Model model)
        {
            GetSearchMWISql(model);
            return model.Export("ExportData");
        }



        public ServiceResult SaveSystemValue(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                var C_S_SYSTEM_TYPE = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == model.SystemType).First();
                //var sysType = db.C_S_SYSTEM_VALUE.Where(o => o.UUID == model.C_S_SYSTEM_VALUE.PARENT_ID).FirstOrDefault();
                //var C_S_SYSTEM_TYPE = db.C_S_SYSTEM_TYPE.Where(x => x.UUID == sysType.SYSTEM_TYPE_ID).FirstOrDefault();
                var query = db.C_S_SYSTEM_VALUE.Find(model.C_S_SYSTEM_VALUE.UUID);
                if (query == null)
                {
                    query = new C_S_SYSTEM_VALUE();
                    query.REGISTRATION_TYPE = model.RegType;
                    query.CODE = model.C_S_SYSTEM_VALUE.CODE;
                    query.PARENT_ID = model.C_S_SYSTEM_VALUE.PARENT_ID;
                }
                query.ENGLISH_DESCRIPTION = model.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION;
                query.CHINESE_DESCRIPTION = model.C_S_SYSTEM_VALUE.CHINESE_DESCRIPTION;
                query.ORDERING = model.C_S_SYSTEM_VALUE.ORDERING;
                query.IS_ACTIVE = model.C_S_SYSTEM_VALUE.IS_ACTIVE;   //not changed yet 
                query.SYSTEM_TYPE_ID = C_S_SYSTEM_TYPE.UUID;

                if (query.UUID == null)
                {
                    db.C_S_SYSTEM_VALUE.Add(query);
                }


                db.SaveChanges();


                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }

        public ServiceResult Save_AN(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_AUTHORITY = db.C_S_AUTHORITY.Where(x => x. == model.SystemType).First();
                var query = db.C_S_AUTHORITY.Find(model.C_S_AUTHORITY.UUID);

                if (query == null)
                {
                    query = new C_S_AUTHORITY();

                }

                query.ENGLISH_NAME = model.C_S_AUTHORITY.ENGLISH_NAME;
                query.CHINESE_NAME = model.C_S_AUTHORITY.CHINESE_NAME;
                query.ENGLISH_RANK = model.C_S_AUTHORITY.ENGLISH_RANK;
                query.CHINESE_RANK = model.C_S_AUTHORITY.CHINESE_RANK;
                query.TELEPHONE_NO = model.C_S_AUTHORITY.TELEPHONE_NO;
                query.FAX_NO = model.C_S_AUTHORITY.FAX_NO;
                query.ENGLISH_ACTION_NAME = model.C_S_AUTHORITY.ENGLISH_ACTION_NAME;
                query.CHINESE_ACTION_NAME = model.C_S_AUTHORITY.CHINESE_ACTION_NAME;
                query.ENGLISH_ACTION_RANK = model.C_S_AUTHORITY.ENGLISH_ACTION_RANK;
                query.CHINESE_ACTION_RANK = model.C_S_AUTHORITY.CHINESE_ACTION_RANK;

                if (query.UUID == null)
                {
                    db.C_S_AUTHORITY.Add(query);
                }

                db.SaveChanges();

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }

        public ServiceResult Save_BSCI(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_AUTHORITY = db.C_S_AUTHORITY.Where(x => x. == model.SystemType).First();
                var query = db.C_S_BUILDING_SAFETY_ITEM.Find(model.C_S_BUILDING_SAFETY_ITEM.UUID);

                if (query == null)
                {
                    query = new C_S_BUILDING_SAFETY_ITEM();

                }

                //query.BUILDING_SAFETY_ID = model.C_S_BUILDING_SAFETY_ITEM.BUILDING_SAFETY_ID;
                if (model.BuildingSafetyCode != null)
                {
                    //query.BUILDING_SAFETY_ID = model.BuildingSafetyCode;
                    var category_code = db.C_S_CATEGORY_CODE.Find(model.C_S_BUILDING_SAFETY_ITEM.CATEGORY_ID).REGISTRATION_TYPE;
                    query.BUILDING_SAFETY_ID = db.C_S_SYSTEM_VALUE
                        .Include(x => x.C_S_SYSTEM_TYPE)
                        .Where(x => x.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_BUILDING_SAFETY_CODE
                        && x.REGISTRATION_TYPE == category_code
                        && x.CODE == model.BuildingSafetyCode).FirstOrDefault().UUID;
                }
                query.CATEGORY_ID = model.C_S_BUILDING_SAFETY_ITEM.CATEGORY_ID;
                query.ENGLISH_HTML_HEADER = model.C_S_BUILDING_SAFETY_ITEM.ENGLISH_HTML_HEADER;
                query.CHINESE_HTML_HEADER = model.C_S_BUILDING_SAFETY_ITEM.CHINESE_HTML_HEADER;


                if (query.UUID == null)
                {
                    db.C_S_BUILDING_SAFETY_ITEM.Add(query);
                }

                db.SaveChanges();

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }

        public ServiceResult Save_CC(CRM_DisplayModel model)
        {
            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_AUTHORITY = db.C_S_AUTHORITY.Where(x => x. == model.SystemType).First();
                var query = db.C_S_CATEGORY_CODE.Find(model.C_S_CATEGORY_CODE.UUID);

                if (query == null)
                {
                    query = new C_S_CATEGORY_CODE();
                    query.CODE = model.C_S_CATEGORY_CODE.CODE;
                    query.CATEGORY_GROUP_ID = model.C_S_CATEGORY_CODE.CATEGORY_GROUP_ID;
                }

                query.REGISTRATION_TYPE = model.C_S_CATEGORY_CODE.REGISTRATION_TYPE;
                query.ENGLISH_DESCRIPTION = model.C_S_CATEGORY_CODE.ENGLISH_DESCRIPTION;
                query.CHINESE_DESCRIPTION = model.C_S_CATEGORY_CODE.CHINESE_DESCRIPTION;
                query.ENGLISH_SUB_TITLE_DESCRIPTION = model.C_S_CATEGORY_CODE.ENGLISH_SUB_TITLE_DESCRIPTION;
                query.CHINESE_SUB_TITLE_DESCRIPTION = model.C_S_CATEGORY_CODE.CHINESE_SUB_TITLE_DESCRIPTION;
                query.ACTIVE = model.Active == true ? "Y" : "N";


                if (query.UUID == null)
                {
                    db.C_S_CATEGORY_CODE.Add(query);
                }

                db.SaveChanges();

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };
            }

        }

        public ServiceResult Save_HN(CRM_DisplayModel model)
        {
            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_AUTHORITY = db.C_S_AUTHORITY.Where(x => x. == model.SystemType).First();
                var query = db.C_S_HTML_NOTES.Find(model.C_S_HTML_NOTES.UUID);

                if (query == null)
                {
                    query = new C_S_HTML_NOTES();

                }

                query.CODE = model.C_S_HTML_NOTES.CODE;
                query.ENGLISH_DESCRIPTION = model.C_S_HTML_NOTES.ENGLISH_DESCRIPTION;
                query.CHINESE_DESCRIPTION = model.C_S_HTML_NOTES.CHINESE_DESCRIPTION;
                query.ORDER_SEQUENCE = model.C_S_HTML_NOTES.ORDER_SEQUENCE;
                query.CATEGORY_GROUP_ID = model.C_S_HTML_NOTES.CATEGORY_GROUP_ID;

                if (query.UUID == null)
                {
                    db.C_S_HTML_NOTES.Add(query);
                }

                db.SaveChanges();

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };
            }

        }

        public ServiceResult Save_RD(CRM_DisplayModel model)
        {
            try
            {
                EntitiesRegistration db = new EntitiesRegistration();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sRoom = db.C_S_ROOM.Find(model.C_S_ROOM.UUID);
                        var addressChinese = db.C_ADDRESS.Find(model.C_S_ROOM.C_ADDRESS.UUID);
                        var addressEnglish = db.C_ADDRESS.Find(model.C_S_ROOM.C_ADDRESS1.UUID);


                        if (addressChinese == null)
                        {
                            addressChinese = new C_ADDRESS();
                            // addressChinese.UUID = CommonUtil.NewUuid();
                            db.C_ADDRESS.Add(addressChinese);
                        }
                        if (addressEnglish == null)
                        {
                            addressEnglish = new C_ADDRESS();
                            // addressEnglish.UUID = CommonUtil.NewUuid();
                            db.C_ADDRESS.Add(addressEnglish);
                        }

                        addressChinese.ADDRESS_LINE1 = model.C_S_ROOM.C_ADDRESS.ADDRESS_LINE1;
                        addressChinese.ADDRESS_LINE2 = model.C_S_ROOM.C_ADDRESS.ADDRESS_LINE2;
                        addressChinese.ADDRESS_LINE3 = model.C_S_ROOM.C_ADDRESS.ADDRESS_LINE3;
                        addressChinese.ADDRESS_LINE4 = model.C_S_ROOM.C_ADDRESS.ADDRESS_LINE4;
                        addressChinese.ADDRESS_LINE5 = model.C_S_ROOM.C_ADDRESS.ADDRESS_LINE5;

                        addressEnglish.ADDRESS_LINE1 = model.C_S_ROOM.C_ADDRESS1.ADDRESS_LINE1;
                        addressEnglish.ADDRESS_LINE2 = model.C_S_ROOM.C_ADDRESS1.ADDRESS_LINE2;
                        addressEnglish.ADDRESS_LINE3 = model.C_S_ROOM.C_ADDRESS1.ADDRESS_LINE3;
                        addressEnglish.ADDRESS_LINE4 = model.C_S_ROOM.C_ADDRESS1.ADDRESS_LINE4;
                        addressEnglish.ADDRESS_LINE5 = model.C_S_ROOM.C_ADDRESS1.ADDRESS_LINE5;

                        db.SaveChanges();

                        if (sRoom == null)
                        {
                            sRoom = new C_S_ROOM();
                            sRoom.ROOM = model.C_S_ROOM.ROOM;

                            sRoom.ENGLISH_ADDRESS_ID = addressEnglish.UUID;
                            sRoom.CHINESE_ADDRESS_ID = addressChinese.UUID;
                            sRoom.TELEPHONE_NO = model.C_S_ROOM.TELEPHONE_NO;
                            sRoom.FAX_NO = model.C_S_ROOM.FAX_NO;
                            db.C_S_ROOM.Add(sRoom);
                        }
                        else
                        {
                            sRoom.ENGLISH_ADDRESS_ID = addressEnglish.UUID;
                            sRoom.CHINESE_ADDRESS_ID = addressChinese.UUID;
                            sRoom.TELEPHONE_NO = model.C_S_ROOM.TELEPHONE_NO;
                            sRoom.FAX_NO = model.C_S_ROOM.FAX_NO;

                        }



                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };
            }

        }

        public ServiceResult Save_CCD(CRM_DisplayModel model)
        {
            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_AUTHORITY = db.C_S_AUTHORITY.Where(x => x. == model.SystemType).First();
                var query = db.C_S_CATEGORY_CODE_DETAIL.Find(model.C_S_CATEGORY_CODE_DETAIL.UUID);

                if (query == null)
                {
                    query = new C_S_CATEGORY_CODE_DETAIL();
                    query.CODE = model.C_S_CATEGORY_CODE_DETAIL.CODE;
                }

                query.ENGLISH_DESCRIPTION = model.C_S_CATEGORY_CODE_DETAIL.ENGLISH_DESCRIPTION;
                query.CHINESE_DESCRIPTION = model.C_S_CATEGORY_CODE_DETAIL.CHINESE_DESCRIPTION;
                query.S_CATEGORY_CODE_ID = model.C_S_CATEGORY_CODE_DETAIL.S_CATEGORY_CODE_ID;


                if (query.UUID == null)
                {
                    db.C_S_CATEGORY_CODE_DETAIL.Add(query);
                }

                db.SaveChanges();

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };
            }

        }

        public ServiceResult Save_MWT(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_SYSTEM_TYPE = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == model.SystemType).First();
                var query = db.C_S_SYSTEM_VALUE.Find(model.C_S_SYSTEM_VALUE.UUID);
                if (query == null)
                {
                    query = new C_S_SYSTEM_VALUE();
                    query.PARENT_ID = model.C_S_SYSTEM_VALUE.C_S_SYSTEM_VALUE2.CODE;
                    query.REGISTRATION_TYPE = "ALL";
                    query.SYSTEM_TYPE_ID = "8a85932a25bf91ee0125bf91eea4000d";
                }

                query.CODE = model.C_S_SYSTEM_VALUE.CODE;
                query.ENGLISH_DESCRIPTION = model.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION;

                if (query.UUID == null)
                {
                    db.C_S_SYSTEM_VALUE.Add(query);
                }


                db.SaveChanges();


                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }

        public ServiceResult Save_MWI(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_SYSTEM_TYPE = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == model.SystemType).First();
                var query = db.C_S_SYSTEM_VALUE.Find(model.C_S_SYSTEM_VALUE.UUID);
                if (query == null)
                {
                    query = new C_S_SYSTEM_VALUE();
                    query.REGISTRATION_TYPE = "ALL";
                    query.SYSTEM_TYPE_ID = "8a85932a25bf91ee0125bf91eea4000d";
                }

                query.PARENT_ID = model.C_S_SYSTEM_VALUE.C_S_SYSTEM_VALUE2.C_S_SYSTEM_VALUE2.CODE;
                query.CODE = model.C_S_SYSTEM_VALUE.CODE;
                query.ENGLISH_DESCRIPTION = model.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION;

                if (query.UUID == null)
                {
                    db.C_S_SYSTEM_VALUE.Add(query);
                }


                db.SaveChanges();


                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }

        public ServiceResult Save_LT(CRM_DisplayModel model)
        {

            try
            {
                EntitiesRegistration db = new EntitiesRegistration();
                //var C_S_SYSTEM_TYPE = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == model.SystemType).First();
                var query = db.C_S_EXPORT_LETTER.Find(model.C_S_EXPORT_LETTER.UUID);
                if (query == null)
                {
                    query = new C_S_EXPORT_LETTER();

                }

                query.CODE = model.C_S_EXPORT_LETTER.CODE;
                query.LETTER_NO = model.C_S_EXPORT_LETTER.LETTER_NO;
                query.REGISTRATION_TYPE = model.C_S_EXPORT_LETTER.REGISTRATION_TYPE;  //need hidden field
                query.LETTER_NAME = model.C_S_EXPORT_LETTER.LETTER_NAME;
                query.LETTER_DESCRIPTION = model.C_S_EXPORT_LETTER.LETTER_DESCRIPTION;
                query.ORDERING = model.C_S_EXPORT_LETTER.ORDERING;
                //query.SELECTION_TYPE = model.C_S_EXPORT_LETTER.SELECTION_TYPE;  //new add
                query.AUTHORITY_NAME_SELECT = "Y";  //Y for all regType

                if (query.REGISTRATION_TYPE == "CGC")
                {
                    query.SELECTION_TYPE = model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE; //new add
                    model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE = "";
                    model.C_S_EXPORT_LETTER_save2.SELECTION_TYPE = "";
                    model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE = "";
                }
                else if (query.REGISTRATION_TYPE == "IP")
                {
                    query.SELECTION_TYPE = model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE; //new add
                }
                else if (query.REGISTRATION_TYPE == "CMW")
                {
                    query.SELECTION_TYPE = model.C_S_EXPORT_LETTER_save2.SELECTION_TYPE; //new add
                    model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE = "";
                    model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE = "";
                    model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE = "";
                }
                else if (query.REGISTRATION_TYPE == "IMW")
                {
                    query.SELECTION_TYPE = model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE; //new add
                }
                else
                {
                    query.SELECTION_TYPE = "";
                }

                //for CGC, CMW
                if (model.C_S_EXPORT_LETTER.REGISTRATION_TYPE == "CGC" || model.C_S_EXPORT_LETTER.REGISTRATION_TYPE == "CMW")
                {
                    query.COMPANY_SELECT = "Y";
                    if (model.C_S_EXPORT_LETTER_save2.SELECTION_TYPE == "6") { query.LETTER_SPECIAL_REMARK = "APPROVEDLETTER"; }
                    if (model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE == "2") { query.AS_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE == "3") { query.TD_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE == "4") { query.AS_SELECT = "Y"; query.TD_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE == "5") { query.AS_SELECT = "Y"; query.TD_SELECT = "Y"; query.INTERVIEW_CANDIDATES_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save0.SELECTION_TYPE == "1") { }
                }

                //for IMW
                if (model.C_S_EXPORT_LETTER.REGISTRATION_TYPE == "IP")
                {
                    query.APPLICANT_SELECT = "Y";
                    query.CERT_SELECT = "Y";
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "8") { query.PRB_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "9") { query.PRB_SELECT = "Y"; query.COMMITTEE_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "10") { query.COMMITTEE_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "11") { query.COMMITTEE_SELECT = "Y"; query.PROCESS_MONITOR_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "12") { query.PROCESS_MONITOR_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "13") { query.PRB_SELECT = "Y"; query.COMMITTEE_SELECT = "Y"; query.PROCESS_MONITOR_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save1.SELECTION_TYPE == "7") { }
                }

                //for IMW
                if (model.C_S_EXPORT_LETTER.REGISTRATION_TYPE == "IMW")
                {
                    query.APPLICANT_SELECT = "Y";
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "14") { }
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "15") { query.LETTER_SPECIAL_REMARK = "2"; }
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "16") { query.LETTER_SPECIAL_REMARK = "3"; }
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "17") { query.INTERVIEW_CANDIDATES_SELECT = "Y"; }
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "18") { query.LETTER_SPECIAL_REMARK = "2"; query.INTERVIEW_CANDIDATES_SELECT = "Y"; ; }
                    if (model.C_S_EXPORT_LETTER_save3.SELECTION_TYPE == "19") { query.LETTER_SPECIAL_REMARK = "3"; query.INTERVIEW_CANDIDATES_SELECT = "Y"; }

                }

                if (query.UUID == null)
                {
                    db.C_S_EXPORT_LETTER.Add(query);
                }


                db.SaveChanges();


                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception e)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { e.Message } };

            }

        }





        public string ExportSystemValue(CRM_Model model)
        {
            model.Query = "select "
                + "\r\n" + "\t" + " T2.UUID "
                + "\r\n" + "\t" + ", CODE "
                + "\r\n" + "\t" + ", REGISTRATION_TYPE "
                + "\r\n" + "\t" + ", ENGLISH_DESCRIPTION "
                + "\r\n" + "\t" + ", CHINESE_DESCRIPTION "
                + "\r\n" + "\t" + ", ORDERING "
                + "\r\n" + "\t" + ", IS_ACTIVE "
                + "\r\n" + "\t" + "from C_S_SYSTEM_TYPE T1 "
                + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE T2 on T2.SYSTEM_TYPE_ID = T1.UUID ";
            model.QueryWhere = " where T1.TYPE = :TYPE ";
            model.QueryParameters.Add("TYPE", model.SystemType);
            return model.Export("ExportData");
        }

        public string ExportAN(CRM_Model model)
        {
            model.Query = "select "
                //+ "\r\n" + "\t" + " UUID, "
                + "\r\n" + "\t" + "  ENGLISH_NAME "
                + "\r\n" + "\t" + ", CHINESE_NAME "
                + "\r\n" + "\t" + ", ENGLISH_RANK "
                + "\r\n" + "\t" + ", CHINESE_RANK "
                + "\r\n" + "\t" + ", ENGLISH_ACTION_NAME as ENGLISH_ACTION_NAME "
                + "\r\n" + "\t" + ", CHINESE_ACTION_NAME as CHINESE_ACTION_NAME "
                + "\r\n" + "\t" + ", ENGLISH_ACTION_RANK as ENGLISH_ACTION_RANK "
                + "\r\n" + "\t" + ", CHINESE_ACTION_RANK as CHINESE_ACTION_RANK"
                + "\r\n" + "\t" + "from C_S_AUTHORITY ";

            //model.Search();
            return model.Export("ExportData");
        }



        public CRM_ARDisplayModel ViewAR(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE aplts = (from t1 in db.C_S_SYSTEM_VALUE
                                          join t2 in db.C_S_SYSTEM_TYPE on t1.SYSTEM_TYPE_ID equals t2.UUID
                                          where t2.TYPE == "APPLICANT_ROLE" && t1.UUID == id
                                          select t1).FirstOrDefault();

                return new CRM_ARDisplayModel()
                {
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }


        public CRM_DisplayModel ViewSystemValue(string id, string systemType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE aplts = (from t1 in db.C_S_SYSTEM_VALUE
                                          join t2 in db.C_S_SYSTEM_TYPE on t1.SYSTEM_TYPE_ID equals t2.UUID
                                          where t1.UUID == id
                                          select t1).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_SYSTEM_VALUE();
                }

                return new CRM_DisplayModel()
                {
                    SystemType = systemType,
                    RegType = aplts.REGISTRATION_TYPE,
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }

        public CRM_DisplayModel ViewAN(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_AUTHORITY aplts = (from t1 in db.C_S_AUTHORITY
                                       where t1.UUID == id
                                       select t1).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_AUTHORITY();
                }

                return new CRM_DisplayModel()
                {

                    C_S_AUTHORITY = aplts
                };

            }
        }

        public CRM_DisplayModel ViewBSCI(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                /* C_S_BUILDING_SAFETY_ITEM aplts = (from sbsi in db.C_S_BUILDING_SAFETY_ITEM
                                                   join sv in db.C_S_SYSTEM_VALUE on sbsi.BUILDING_SAFETY_ID equals sv.UUID
                                                   join sc in db.C_S_CATEGORY_CODE on sbsi.CATEGORY_ID equals sc.UUID
                                                   where sbsi.UUID == id
                                                   select sbsi).FirstOrDefault();*/
                C_S_BUILDING_SAFETY_ITEM aplts =
                db.C_S_BUILDING_SAFETY_ITEM.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_CATEGORY_CODE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_BUILDING_SAFETY_ITEM();
                }

                return new CRM_DisplayModel()
                {
                    C_S_BUILDING_SAFETY_ITEM = aplts,
                };

            }
        }

        public CRM_DisplayModel ViewSystemValueWithParentType(string id, string systemType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //C_S_SYSTEM_VALUE aplts = (from sv1 in db.C_S_SYSTEM_VALUE
                //                          join sv2 in db.C_S_SYSTEM_VALUE on sv1.PARENT_ID equals sv2.UUID
                //                          join st in db.C_S_SYSTEM_TYPE on sv1.SYSTEM_TYPE_ID equals st.UUID
                //                          where sv1.UUID == id
                //                          select sv1).FirstOrDefault();

                C_S_SYSTEM_VALUE aplts = db.C_S_SYSTEM_VALUE.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE2)
                    .Include(o => o.C_S_SYSTEM_TYPE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_SYSTEM_VALUE();
                }

                return new CRM_DisplayModel()
                {
                    SystemType = systemType,
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }

        public CRM_DisplayModel ViewCC(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                /* C_S_BUILDING_SAFETY_ITEM aplts = (from sbsi in db.C_S_BUILDING_SAFETY_ITEM
                                                   join sv in db.C_S_SYSTEM_VALUE on sbsi.BUILDING_SAFETY_ID equals sv.UUID
                                                   join sc in db.C_S_CATEGORY_CODE on sbsi.CATEGORY_ID equals sc.UUID
                                                   where sbsi.UUID == id
                                                   select sbsi).FirstOrDefault();*/
                C_S_CATEGORY_CODE aplts =
                db.C_S_CATEGORY_CODE.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_CATEGORY_CODE();
                }

                return new CRM_DisplayModel()
                {
                    C_S_CATEGORY_CODE = aplts,
                };

            }
        }

        public CRM_DisplayModel ViewHN(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_HTML_NOTES aplts =
                db.C_S_HTML_NOTES.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_HTML_NOTES();
                    aplts.ORDER_SEQUENCE = 1;
                }

                return new CRM_DisplayModel()
                {
                    C_S_HTML_NOTES = aplts
                };

            }
        }

        public CRM_DisplayModel ViewRD(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_ROOM aplts =
                db.C_S_ROOM.Where(o => o.UUID == id)
                    .Include(o => o.C_ADDRESS)
                    .Include(o => o.C_ADDRESS1).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_ROOM();
                }

                return new CRM_DisplayModel()
                {
                    C_S_ROOM = aplts
                };

            }
        }

        public CRM_DisplayModel ViewCCD(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_CATEGORY_CODE_DETAIL aplts =
                db.C_S_CATEGORY_CODE_DETAIL.Where(o => o.UUID == id)
                    .Include(o => o.C_S_CATEGORY_CODE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_CATEGORY_CODE_DETAIL();
                }

                return new CRM_DisplayModel()
                {
                    C_S_CATEGORY_CODE_DETAIL = aplts
                };

            }
        }

        public CRM_DisplayModel ViewLT(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_EXPORT_LETTER aplts =
                db.C_S_EXPORT_LETTER.Where(o => o.UUID == id).FirstOrDefault();
                /*List<C_S_EXPORT_LETTER> C_S_EXPORT_LETTERs = new List<C_S_EXPORT_LETTER>();
                C_S_EXPORT_LETTERs.Add(aplts);

                C_S_EXPORT_LETTERs.Add(aplts);

                C_S_EXPORT_LETTERs.Add(aplts);

                C_S_EXPORT_LETTERs.Add(aplts);*/
                if (aplts == null)
                {
                    aplts = new C_S_EXPORT_LETTER();
                }

                return new CRM_DisplayModel()
                {
                    C_S_EXPORT_LETTER = aplts
                    ,
                    C_S_EXPORT_LETTER_save0 = aplts
                    ,
                    C_S_EXPORT_LETTER_save1 = aplts
                    ,
                    C_S_EXPORT_LETTER_save2 = aplts
                    ,
                    C_S_EXPORT_LETTER_save3 = aplts
                };

            }
        }

        public CRM_DisplayModel ViewMWI(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_SYSTEM_VALUE aplts = db.C_S_SYSTEM_VALUE.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE2.C_S_SYSTEM_VALUE2)
                    .Include(o => o.C_S_SYSTEM_VALUE2)

                    .Include(o => o.C_S_SYSTEM_TYPE).FirstOrDefault();

                if (aplts == null)
                {
                    aplts = new C_S_SYSTEM_VALUE();
                }

                return new CRM_DisplayModel()
                {
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }

        public CRM_ASDisplayModel ViewAS(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_SYSTEM_VALUE aplts = (from t1 in db.C_S_SYSTEM_VALUE
                                          join t2 in db.C_S_SYSTEM_TYPE on t1.SYSTEM_TYPE_ID equals t2.UUID
                                          where t2.TYPE == "APPLICANT_STATUS" && t1.UUID == id
                                          select t1).FirstOrDefault();

                return new CRM_ASDisplayModel()
                {
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }

        public CRM_AFDisplayModel ViewAF(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_S_SYSTEM_VALUE aplts = (from t1 in db.C_S_SYSTEM_VALUE
                                          join t2 in db.C_S_SYSTEM_TYPE on t1.SYSTEM_TYPE_ID equals t2.UUID
                                          where t2.TYPE == "APPLICATION_FORM" && t1.UUID == id
                                          select t1).FirstOrDefault();

                return new CRM_AFDisplayModel()
                {
                    C_S_SYSTEM_VALUE = aplts
                };

            }
        }




    }



}