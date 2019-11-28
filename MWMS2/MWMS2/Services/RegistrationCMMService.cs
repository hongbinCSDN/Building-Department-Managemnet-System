using MWMS2.Areas.Registration.Models;
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

namespace MWMS2.Services
{
    public class RegistrationCMMService
    {
        String SearchGCA_q_copy = ""
             + "\r\n" + "\t" + "SELECT                                                               "
             + "\r\n" + "\t" + "T1.UUID                                                              "
             + "\r\n" + "\t" + ", T1.FILE_REFERENCE_NO                                               "
             + "\r\n" + "\t" + ", T1.ENGLISH_COMPANY_NAME                                            "
             + "\r\n" + "\t" + ", T1.CERTIFICATION_NO                                                "
             + "\r\n" + "\t" + ", T2.ENGLISH_DESCRIPTION                                             "
             + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
             + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
             + "\r\n" + "\t" + "WHERE T1.REGISTRATION_TYPE   ='CGC' ";

        String Search_CP_q = ""
             + "\r\n" + "\t" + "SELECT                                                               "
             + "\r\n" + "\t" + "SV.CODE                                                            "
             + "\r\n" + "\t" + ", CP.UUID as UUID, CP.YEAR  from C_committee_panel CP                                             "
             
            + "\r\n" + "\t" + "inner join C_s_system_value SV on CP.panel_type_id= SV.uuid                                           "
             + "\r\n" + "\t" + "WHERE 1=1 ";

        String Search_CG_q = ""
                             + "\r\n" + "\t" + "SELECT                                                               "
                             + "\r\n" + "\t" + "cg.uuid, cg.year ||'-'||sv.code ||'-'||cg.name as comRef,                                             "
                             + "\r\n" + "\t" + "cg.year,sv.code,cg.name, cg.month                          "
                             + "\r\n" + "\t" + " from c_committee_group cg                              "
                             + "\r\n" + "\t" + "   inner join c_committee c on c.uuid = cg.COMMITTEE_ID                          "
                             + "\r\n" + "\t" + "    inner join c_s_system_value sv on  sv.uuid = cg.COMMITTEE_TYPE_ID                      "
                             + "\r\n" + "\t" + "    inner join c_committee_panel cp on c.COMMITTEE_PANEL_ID = cp.uuid                  "
                             + "\r\n" + "\t" + "WHERE 1=1 ";




        String Search_MP_q = "" +
                " SELECT " +
                " M.UUID, " +
                " A.SURNAME || ' '|| A.GIVEN_NAME_ON_ID AS NAME, M.RANK " +
                " FROM C_COMMITTEE_MEMBER M, C_APPLICANT A " +
                " WHERE M.APPLICANT_ID = A.UUID ";

        String Search_CMM_q = ""
            + "\r\n\t" + "SELECT                                                            "
            + "\r\n\t" + "T1.YEAR                                                           "
            + "\r\n\t" + ", T1.UUID                                                         "
            + "\r\n\t" + ", T3.CODE AS PANEL_TYPE_ID                                        "
            + "\r\n\t" + ", T4.CODE AS COMMITTEE_TYPE_ID                                    "
            + "\r\n\t" + "FROM C_COMMITTEE T1                                                 "
            + "\r\n\t" + "INNER JOIN C_COMMITTEE_PANEL T2 ON T2.UUID = T1.COMMITTEE_PANEL_ID  "
            + "\r\n\t" + "INNER JOIN C_S_SYSTEM_VALUE T3 ON T3.UUID = T2.PANEL_TYPE_ID        "
            + "\r\n\t" + "INNER JOIN C_S_SYSTEM_VALUE T4 ON T4.UUID = T1.COMMITTEE_TYPE_ID    ";



        public void SearchMP(Fn06CMM_MPSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = Search_MP_q;  //need to change query after all
            model.QueryWhere = SearchMP_whereQ(model);
            model.Search();
        }
        public Fn06CMM_MPDisplayModel EditMP(string id)
        {
            Fn06CMM_MPDisplayModel model = new Fn06CMM_MPDisplayModel();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                model.C_COMMITTEE_MEMBER = db.C_COMMITTEE_MEMBER
                    .Where(o => o.UUID == id)
                    .Include(o => o.C_APPLICANT)
                    .Include(o => o.C_ADDRESS)
                    .Include(o => o.C_ADDRESS1)
                    .FirstOrDefault();
                model.C_COMMITTEE_MEMBER?.C_APPLICANT?.Decrypt();

                if (model.C_COMMITTEE_MEMBER == null) model.C_COMMITTEE_MEMBER = new C_COMMITTEE_MEMBER() { UUID = SessionUtil.DRAFT_NEXT_ID };
                if (string.IsNullOrWhiteSpace(model.C_COMMITTEE_MEMBER?.UUID))
                {
                    model.C_COMMITTEE_MEMBER.UUID = SessionUtil.DRAFT_NEXT_ID;
                }
                model.EditFormKey = model.C_COMMITTEE_MEMBER == null ? -1 : model.C_COMMITTEE_MEMBER.MODIFIED_DATE.Ticks;
            }
            return model;
        }
        public ServiceResult SaveMP(Fn06CMM_MPDisplayModel model)
        {
            int dummy;
            C_COMMITTEE_MEMBER memTemp = model.C_COMMITTEE_MEMBER;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        C_COMMITTEE_MEMBER mem = null;

                        //if (int.TryParse(memTemp.UUID, out dummy))
                        if(!model.EditMode)
                        {
                            memTemp.UUID = null;
                            mem = new C_COMMITTEE_MEMBER();
                        }
                        else
                        {
                            mem = db.C_COMMITTEE_MEMBER
                                .Where(o => o.UUID == memTemp.UUID && o.MODIFIED_DATE == new DateTime(model.EditFormKey))
                                .Include(o => o.C_ADDRESS).Include(o => o.C_ADDRESS1).Include(o => o.C_APPLICANT)
                                .FirstOrDefault();
                        }
                        mem.ENGLISH_CARE_OF = memTemp.ENGLISH_CARE_OF;
                        mem.CHINESE_CARE_OF = memTemp.CHINESE_CARE_OF;
                        mem.TELEPHONE_NO1 = memTemp.TELEPHONE_NO1;
                        mem.TELEPHONE_NO2 = memTemp.TELEPHONE_NO2;
                        mem.TELEPHONE_NO3 = memTemp.TELEPHONE_NO3;
                        mem.FAX_NO1 = memTemp.FAX_NO1;
                        mem.FAX_NO2 = memTemp.FAX_NO2;
                        mem.EMAIL = memTemp.EMAIL;
                        mem.RANK = memTemp.RANK;
                        mem.POST = memTemp.POST;
                        mem.CAREER = memTemp.CAREER;
                        mem.REGISTRATION_TYPE = RegistrationConstant.ALL_TYPE;
                        if (mem.C_ADDRESS == null) mem.C_ADDRESS = new C_ADDRESS();
                        mem.C_ADDRESS.ADDRESS_LINE1 = memTemp.C_ADDRESS.ADDRESS_LINE1;
                        mem.C_ADDRESS.ADDRESS_LINE2 = memTemp.C_ADDRESS.ADDRESS_LINE2;
                        mem.C_ADDRESS.ADDRESS_LINE3 = memTemp.C_ADDRESS.ADDRESS_LINE3;
                        mem.C_ADDRESS.ADDRESS_LINE4 = memTemp.C_ADDRESS.ADDRESS_LINE4;
                        mem.C_ADDRESS.ADDRESS_LINE5 = memTemp.C_ADDRESS.ADDRESS_LINE5;

                        if (mem.C_ADDRESS1 == null) mem.C_ADDRESS1 = new C_ADDRESS();
                        mem.C_ADDRESS1.ADDRESS_LINE1 = memTemp.C_ADDRESS1.ADDRESS_LINE1;
                        mem.C_ADDRESS1.ADDRESS_LINE2 = memTemp.C_ADDRESS1.ADDRESS_LINE2;
                        mem.C_ADDRESS1.ADDRESS_LINE3 = memTemp.C_ADDRESS1.ADDRESS_LINE3;
                        mem.C_ADDRESS1.ADDRESS_LINE4 = memTemp.C_ADDRESS1.ADDRESS_LINE4;
                        mem.C_ADDRESS1.ADDRESS_LINE5 = memTemp.C_ADDRESS1.ADDRESS_LINE5;

                        if (mem.C_APPLICANT == null) mem.C_APPLICANT = new C_APPLICANT();
                        memTemp.C_APPLICANT.Encrypt();
                        mem.C_APPLICANT.HKID = memTemp.C_APPLICANT.HKID;
                        mem.C_APPLICANT.PASSPORT_NO = memTemp.C_APPLICANT.PASSPORT_NO;
                        mem.C_APPLICANT.TITLE_ID = memTemp.C_APPLICANT.TITLE_ID;
                        mem.C_APPLICANT.GENDER = memTemp.C_APPLICANT.GENDER;
                        mem.C_APPLICANT.SURNAME = memTemp.C_APPLICANT.SURNAME;
                        mem.C_APPLICANT.GIVEN_NAME_ON_ID = memTemp.C_APPLICANT.GIVEN_NAME_ON_ID;
                        mem.C_APPLICANT.CHINESE_NAME = memTemp.C_APPLICANT.CHINESE_NAME;

                        if(memTemp.UUID != null)
                        {
                            if(model.PanelStatus !=null)
                            foreach (string key in model.PanelStatus.Keys) {
                                C_COMMITTEE_PANEL_MEMBER r = db.C_COMMITTEE_PANEL_MEMBER
                                    .Where(o => o.UUID == key)
                                    .FirstOrDefault();
                                r.MEMBER_STATUS_ID = model.PanelStatus[key];
                                r.TERMINATED_DATE = (DateTime?)model.PanelEndDate[key];
                            }
                        }
                        if(!model.EditMode)
                        {
                            db.C_COMMITTEE_MEMBER.Add(mem);
                        }
                        db.SaveChanges();
                        db.C_COMMITTEE_MEMBER_INSTITUTE.RemoveRange(db.C_COMMITTEE_MEMBER_INSTITUTE.Where(o => o.MEMBER_ID == mem.UUID));
                        if(model.Institutes != null)
                            for (int i =0;i < model.Institutes.Count; i++)
                                db.C_COMMITTEE_MEMBER_INSTITUTE.Add(new C_COMMITTEE_MEMBER_INSTITUTE() { MEMBER_ID = mem.UUID, SOCIETY_ID = model.Institutes[i] });
                            
                        /*
                        C_COMMITTEE_PANEL_MEMBER C_COMMITTEE_PANEL_MEMBER = new C_COMMITTEE_PANEL_MEMBER()
                        { COMMITTEE_PANEL_ID };



                        using (EntitiesRegistration db = new EntitiesRegistration())
                        {
                            DisplayGrid displayGrid = new DisplayGrid();

                            List<C_COMMITTEE_PANEL_MEMBER> r = db.C_COMMITTEE_PANEL_MEMBER
                                .Where(o => o.MEMBER_ID == id)
                                .Include(o => o.C_COMMITTEE_PANEL)
                                .Include(o => o.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                                .Include(o => o.C_S_SYSTEM_VALUE)
                                .ToList();
                            List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                            for (int i = 0; i < r.Count; i++)
                            {
                                data.Add(new Dictionary<string, object>() {
                      { "V1" ,r[i].C_COMMITTEE_PANEL.YEAR }
                    , { "V2" ,r[i].C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION }
                    , { "V3" ,r[i].C_S_SYSTEM_VALUE1?.ENGLISH_DESCRIPTION }
                    , { "V4" ,r[i].EXPIRY_DATE?.ToString(ApplicationConstant.DISPLAY_DATE_FORMAT)}
                    , { "V5" ,r[i].C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION }
                    , { "V5UUID" ,r[i].UUID }
                    , { "V6" ,r[i].TERMINATED_DATE?.ToString(ApplicationConstant.DISPLAY_DATE_FORMAT) }});
                            }
                            displayGrid.Data = data;
                            displayGrid.Total = displayGrid.Data.Count;
                            return displayGrid;
                        }
                    }
                    */

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }
        public DisplayGrid AjaxInstitute(string id)
        {
            DisplayGrid displayGrid = new DisplayGrid();
            displayGrid.Query = ""
                + "\r\n\t" + " SELECT                                                            "
                + "\r\n\t" + " T1.UUID                                                           "
                + "\r\n\t" + " , T1.ENGLISH_DESCRIPTION                                          "
                + "\r\n\t" + " , COUNT(T2.UUID) AS SELECTED                                      "
                + "\r\n\t" + " FROM C_S_SYSTEM_VALUE T1                                          "
                + "\r\n\t" + " LEFT JOIN C_S_SYSTEM_TYPE T3                                     "
                + "\r\n\t" + " ON T1.SYSTEM_TYPE_ID = T3.UUID                                    "
                + "\r\n\t" + " LEFT JOIN C_COMMITTEE_MEMBER_INSTITUTE T2                         "
                + "\r\n\t" + " ON T1.UUID = T2.SOCIETY_ID                                        "
                + "\r\n\t" + " AND T2.MEMBER_ID = :MEMBERID             "
                + "\r\n\t" + " WHERE T1.SYSTEM_TYPE_ID = T3.UUID AND T3.TYPE = 'SOCIETY_NAME'    "
                + "\r\n\t" + " GROUP BY T1.UUID   , T1.ENGLISH_DESCRIPTION                       ";


            displayGrid.QueryParameters.Add("MEMBERID", id);
            displayGrid.Rpp = 99999;
            displayGrid.Sort = "ENGLISH_DESCRIPTION";
            displayGrid.Search();
            return displayGrid;
        }
        public DisplayGrid AjaxGroupMember(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                DisplayGrid displayGrid = new DisplayGrid();
                List<C_COMMITTEE_GROUP_MEMBER> r = db.C_COMMITTEE_GROUP_MEMBER
                    .Where(o => o.MEMBER_ID == id)
                    .Include(o => o.C_COMMITTEE_GROUP)
                    .Include(o => o.C_COMMITTEE_GROUP.C_COMMITTEE)
                    .Include(o => o.C_COMMITTEE_GROUP.C_COMMITTEE.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .ToList();
                List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                if(r != null)
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        data.Add(new Dictionary<string, object>() {
                      { "V1" ,r[i].C_COMMITTEE_GROUP.YEAR }
                    , { "V2" ,r[i].C_COMMITTEE_GROUP.C_COMMITTEE.C_S_SYSTEM_VALUE.CODE }
                    , { "V3" ,r[i].C_COMMITTEE_GROUP.NAME }
                    , { "V4" ,r[i].C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION }});
                    }
                }
                displayGrid.Data = data;
                displayGrid.Total = displayGrid.Data.Count;
                return displayGrid;
            }
        }
        public DisplayGrid AjaxPanelMember(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                DisplayGrid displayGrid = new DisplayGrid();

                List<C_COMMITTEE_PANEL_MEMBER> r = db.C_COMMITTEE_PANEL_MEMBER
                    .Where(o => o.MEMBER_ID == id)
                    .Include(o => o.C_COMMITTEE_PANEL)
                    .Include(o => o.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .ToList();
                List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                for (int i = 0; i < r.Count; i++)
                {
                    data.Add(new Dictionary<string, object>() {
                      { "V1" ,r[i].C_COMMITTEE_PANEL.YEAR }
                    , { "V1UUID" ,r[i].C_COMMITTEE_PANEL.UUID }
                    , { "V2" ,r[i].C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION }
                    , { "V3" ,r[i].C_S_SYSTEM_VALUE1?.ENGLISH_DESCRIPTION }
                    , { "V4" ,r[i].EXPIRY_DATE?.ToString(ApplicationConstant.DISPLAY_DATE_FORMAT)}
                    , { "V5" ,r[i].C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION }
                    , { "V52UUID" ,r[i].C_S_SYSTEM_VALUE?.UUID }
                    , { "V5UUID" ,r[i].UUID }
                    , { "V6" ,r[i].TERMINATED_DATE?.ToString(ApplicationConstant.DISPLAY_DATE_FORMAT) }});
                }
                displayGrid.Data = data;
                displayGrid.Total = displayGrid.Data.Count;
                return displayGrid;
            }
        }
        public string ExportMP(Fn06CMM_MPSearchModel model)
        {
            model.Query = Search_MP_q;  //need to change query after all
            model.QueryWhere = SearchMP_whereQ(model);
            return model.Export("ExportData");
        }
        private string SearchMP_whereQ(Fn06CMM_MPSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND  A.CHINESE_NAME LIKE :CHINESE_NAME";
                model.QueryParameters.Add("CHINESE_NAME", "%" + model.ChiName.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                whereQ += "\r\n\t" + "AND UPPER(A.SURNAME) LIKE :SURNAME";
                model.QueryParameters.Add("SURNAME", model.Surname.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(A.GIVEN_NAME_ON_ID) LIKE :GIVEN_NAME_ON_ID";
                model.QueryParameters.Add("GIVEN_NAME_ON_ID", "%" + model.GivenName.Trim().ToUpper() + "%");
            }

            return whereQ;
        }
        private string SearchCMM_whereQ(Fn06CMM_CMMSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Year))
            {
                whereQ += "\r\n\t" + "AND T1.YEAR = :YEAR";
                model.QueryParameters.Add("YEAR", model.Year.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.Committee))
            {
                whereQ += "\r\n\t" + "AND T4.UUID = :COMMITTEE_TYPE_ID";
                model.QueryParameters.Add("COMMITTEE_TYPE_ID", model.Committee.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.Panel))
            {
                whereQ += "\r\n\t" + "AND T3.UUID = :PANEL_TYPE_ID";
                model.QueryParameters.Add("PANEL_TYPE_ID", model.Panel.Trim());
            }
            return whereQ;

        }
        public void LoadfCMM(Fn06CMM_CMMSearchModel model)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                model.C_COMMITTEE = db.C_COMMITTEE
                    .Where(o => o.UUID == model.C_COMMITTEE.UUID)
                    .Include(o => o.C_COMMITTEE_PANEL.C_COMMITTEE_PANEL_MEMBER.Select(o2 => o2.C_COMMITTEE_MEMBER))
                    .Include(o => o.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .FirstOrDefault();
                model.SearchType = 1;
            }
        }
        public void SearchCMM(Fn06CMM_CMMSearchModel model)
        {
            model.Query = Search_CMM_q;
            model.QueryWhere = SearchCMM_whereQ(model);
            model.Search();
        }


        private void getCommitteeMembers(DisplayGrid displayGrid, string type, List<string> memberUuid, string year, string uuid, string surname, string givenname,string committeeType,string groupName,string month)
        {
            string q = ""
             + "\r\n\t" + " SELECT DISTINCT                                                                "
             + "\r\n\t" + " T3.UUID ,T4.HKID, T4.PASSPORT_NO                                               "
             + "\r\n\t" + " , T4.SURNAME||' '||T4.GIVEN_NAME_ON_ID AS NAME                                 "
             + "\r\n\t" + " , T3.POST                                                                      "
             + "\r\n\t" + " , T3.RANK                                                                      "
             + "\r\n\t" + " , T3.CAREER                                                                    ";
            if ("PANEL".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                q = q
               // + "\r\n\t" + " , T2.PANEL_ROLE_ID AS ROLE_ID                                                 "
                + "\r\n\t" + " FROM C_COMMITTEE_PANEL T1                                                     "
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_PANEL_MEMBER T2 ON T2.COMMITTEE_PANEL_ID = T1.UUID     "
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_MEMBER T3 ON T3.UUID = T2.MEMBER_ID                    ";
            }
            else if ("COMMITTEE".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
               
                q = q
                + "\r\n\t" + " , T2.COMMITTEE_ROLE_ID AS ROLE_ID                                             "
                + "\r\n\t" + " FROM C_COMMITTEE T1                                                           "
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_COMMITTEE_MEMBER T2 ON T2.COMMITTEE_ID = T1.UUID       "
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_MEMBER T3 ON T3.UUID = T2.MEMBER_ID                    ";
            }
            else if ("MEMBER".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                q = q
                + "\r\n\t" + " FROM C_COMMITTEE_MEMBER T3                                   ";
            }
            else if ("GROUP".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                q = q
                      + "\r\n\t" + " , T2.COMMITTEE_ROLE_ID AS ROLE_ID  "   
                      + "\r\n\t" + " FROM C_COMMITTEE_PANEL  T1                                                           "
                      + "\r\n\t" + " INNER JOIN C_COMMITTEE T7 ON T1.UUID = T7.COMMITTEE_PANEL_ID       "
                      + "\r\n\t" + " INNER JOIN C_COMMITTEE_GROUP T8 ON T7.UUID = T8.COMMITTEE_ID       "
                      + "\r\n\t" + " INNER JOIN C_COMMITTEE_GROUP_MEMBER T2 ON T8.UUID = T2.COMMITTEE_GROUP_ID       "
                      + "\r\n\t" + " INNER JOIN C_COMMITTEE_MEMBER T3 ON T3.UUID = T2.MEMBER_ID       ";
                  
                   

            }
            q = q
                + "\r\n\t" + " INNER JOIN C_APPLICANT T4 ON T4.UUID = T3.APPLICANT_ID                        "
                + "\r\n\t" + " WHERE 1=1                                                                   ";
            if (memberUuid != null)
            {
                q = q + "\r\n\t" + " AND T3.UUID IN(:memberUuid)                                    ";
                displayGrid.QueryParameters.Add("memberUuid", memberUuid);
            }
            
            if ("COMMITTEE".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                if (uuid == null)
                {
                    q = q
                      + "\r\n\t" + " AND 1=2";
                }
            }

            if (!string.IsNullOrWhiteSpace(uuid))
            {
                q = q + "\r\n\t" + " AND T1.UUID = :UUID                                                   ";
                displayGrid.QueryParameters.Add("UUID", uuid);
            }
    
            if (!string.IsNullOrWhiteSpace(year))
            {
                q = q + "\r\n\t" + " AND T1.YEAR = :YEAR                                                   ";
                displayGrid.QueryParameters.Add("YEAR", year);
            }
            if (!string.IsNullOrWhiteSpace(surname))
            {
                q = q + "\r\n\t" + " AND upper(T4.SURNAME) like :SURNAME                                             ";
                displayGrid.QueryParameters.Add("SURNAME","%" + surname.ToUpper() +"%");
            }
            if (!string.IsNullOrWhiteSpace(givenname))
            {
                q = q + "\r\n\t" + " AND upper(T4.GIVEN_NAME_ON_ID) like :GIVEN_NAME_ON_ID                           ";
                displayGrid.QueryParameters.Add("GIVEN_NAME_ON_ID","%" + givenname.ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(committeeType))
            {
                q = q + "\r\n\t" + " AND upper(T8.COMMITTEE_TYPE_ID) = :committeeType                           ";
                displayGrid.QueryParameters.Add("committeeType",  committeeType.ToUpper() );

            }
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                q = q + "\r\n\t" + " AND upper(T8.NAME) = :groupName                           ";
                displayGrid.QueryParameters.Add("groupName",   groupName.ToUpper()  );

            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                q = q + "\r\n\t" + " AND upper(T8.MONTH) = :month                           ";
                displayGrid.QueryParameters.Add("month", month.ToUpper() );

            }
            displayGrid.Rpp = 99999;
            displayGrid.Query = q;
            displayGrid.Sort = "NAME";
    
            displayGrid.Search();
       
            //displayGrid.Rpp = displayGrid.Total;
            for (int i = 0; i < displayGrid.Data.Count; i++)
            {
                string hkid = displayGrid.Data[i]["HKID"] as string;
                string passport = displayGrid.Data[i]["PASSPORT_NO"] as string;
                hkid = string.IsNullOrWhiteSpace(hkid) ? "" : EncryptDecryptUtil.getDecryptHKID(hkid);
                passport = string.IsNullOrWhiteSpace(passport) ? "" : EncryptDecryptUtil.getDecryptHKID(passport);
                displayGrid.Data[i].Add("HKID_PASSPORT", (hkid + " " + passport).Trim());

            }
        }
        /*
        private void GetDBMember(Fn06CMM_CMMSearchModel model)
        {
            string q = ""
                + "\r\n\t" + " SELECT DISTINCT  T1.UUID,                                "
                + "\r\n\t" + " T2.SURNAME||' '||T2.GIVEN_NAME_ON_ID AS NAME             "
                + "\r\n\t" + " , T1.RANK                                                "
                + "\r\n\t" + " , T1.POST                                                "
                + "\r\n\t" + " , T1.CAREER, T3.PANEL_ROLE_ID ,T2.HKID, T2.PASSPORT_NO   "
                + "\r\n\t" + " FROM C_COMMITTEE_MEMBER T1                               "
                + "\r\n\t" + " INNER JOIN C_APPLICANT T2 ON T1.APPLICANT_ID = T2.UUID   ";
            if (model.CheckMembers == null || model.CheckMembers.Count == 0 || model.SearchType == null || model.SearchType == 1)
            {
                q = q
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_PANEL_MEMBER T3 ON T3.MEMBER_ID = T1.UUID"
                + "\r\n\t" + " INNER JOIN C_COMMITTEE_PANEL T4 ON T4.UUID = T3.COMMITTEE_PANEL_ID"
                + "\r\n\t" + " INNER JOIN C_COMMITTEE T5 ON T5.COMMITTEE_PANEL_ID = T4.UUID";
            }
            string whereQ = "";
            if (model.CheckMembers != null && model.CheckMembers.Count > 0)
            {
                whereQ += "\r\n\t" + "AND T1.UUID IN(:UUIDs)";
                model.QueryParameters.Add("UUIDs", model.CheckMembers);
            }
            else   if (model.C_COMMITTEE != null)
            {
                whereQ += "\r\n\t" + "AND T5.UUID = :UUID";
                model.QueryParameters.Add("UUID", model.C_COMMITTEE.UUID.Trim());
                //whereQ += "\r\n\t" + "AND T4.UUID = :C_COMMITTEE_PANEL_UUID";
                // model.QueryParameters.Add("C_COMMITTEE_PANEL_UUID", model.C_COMMITTEE_PANEL_UUID.Trim());
            }
            else if (model.SearchType == 1)
            {
                if (!string.IsNullOrWhiteSpace(model.Year))
                {
                    whereQ += "\r\n\t" + "AND T4.YEAR = :YEAR";
                    model.QueryParameters.Add("YEAR", model.Year.Trim());
                }
                if (!string.IsNullOrWhiteSpace(model.Panel))
                {
                    whereQ += "\r\n\t" + "AND T3.COMMITTEE_PANEL_ID = :COMMITTEE_PANEL_ID";
                    model.QueryParameters.Add("COMMITTEE_PANEL_ID", model.Panel.Trim());
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(model.Surname))
                {
                    whereQ += "\r\n\t" + "AND T2.SURNAME LIKE :SURNAME";
                    model.QueryParameters.Add("SURNAME", model.Surname.Trim().ToUpper() + '%');
                }
                if (!string.IsNullOrWhiteSpace(model.GivenName))
                {
                    whereQ += "\r\n\t" + "AND T2.GIVEN_NAME_ON_ID LIKE :GIVEN_NAME_ON_ID";
                    model.QueryParameters.Add("GIVEN_NAME_ON_ID", '%' + model.GivenName.Trim().ToUpper() + '%');
                }
            }
            model.Query = q;
            model.QueryWhere = whereQ;
            model.Search();
            for(int i = 0; i < model.Data.Count; i++)
            {
                string hkid = model.Data[i]["HKID"] as string;
                string passport = model.Data[i]["PASSPORT_NO"] as string;
                hkid = string.IsNullOrWhiteSpace(hkid) ? "" : EncryptDecryptUtil.getDecryptHKID(hkid);
                passport = string.IsNullOrWhiteSpace(passport) ? "" : EncryptDecryptUtil.getDecryptHKID(passport);
                model.Data[i].Add("HKID_PASSPORT",( hkid + " " + passport).Trim());

            }
        }
        */


        public void SearchPanelMember(Fn06CMM_CMMSearchModel model)
        {
            getCommitteeMembers(model, "PANEL", null, model.Year, model.Panel, model.Surname, model.GivenName,null,null,null);
            
        }
        public void SearchCommitteeMember(Fn06CMM_CMMSearchModel model)
        {
            getCommitteeMembers(model, "COMMITTEE", null, null, model.C_COMMITTEE.UUID, null, null, null, null, null);

            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            for (int i = 0; i < draftMember.Count; i++)
            {
                Dictionary<string, object> draftEach = draftMember[i];
                if (model.Data.Where(o => o["UUID"] as string == draftEach["UUID"] as string).ToList().Count == 0)
                    model.Data.Add(draftEach);
            }
            model.Data.RemoveAll(o => deleteMember.Contains(o["UUID"]));
            model.Total = model.Data.Count;
        }



        public string ExportCMM(Fn06CMM_CMMSearchModel model)
        {
            model.Query = Search_CMM_q;  //need to change query after all
            //model.QueryWhere = SearchMP_whereQ(model);
            return model.Export("ExportData");
        }
        public DisplayGrid AjaxPanelParent()
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                DisplayGrid displayGrid = new DisplayGrid() { Data = new List<Dictionary<string, object>>() };
                Dictionary<string, string> r = new Dictionary<string, string>();
                List<C_S_SYSTEM_VALUE> l = SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE).ToList();
                for (int i = 0; i < l.Count; i++)
                {
                    displayGrid.Data.Add(new Dictionary<string, object>() { { "UUID", l[i].UUID } , { "PARENT_ID", l[i].PARENT_ID } });
                }
                return displayGrid;
            }
        }

        public ServiceResult AddToMember(Fn06CMM_CMMSearchModel model)
        {
            getCommitteeMembers(model, "MEMBER", model.CheckMembers, null, null, null, null, null, null, null);
            //GetDBMember(model);
            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);


            if(model.Data != null)
            {
                for(int i=0; i < model.Data.Count; i++)
                {
                    var currentData = model.Data[i];
                    if ((draftMember.Select(o => o["UUID"] as string)).Equals(currentData["UUID"] as string)) continue;
                    draftMember.Add(currentData);
                }
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        

        public ServiceResult Delete(Fn06CMM_CMMSearchModel model)
        {
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            if(model.PanelRole.Count == 0)
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            foreach(string key in model.PanelRole.Keys)
            {
                if (!deleteMember.Contains(key)) deleteMember.Add(key);
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult Save(Fn06CMM_CMMSearchModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //string delUuid = model.C_COMMITTEE.UUID;
                //C_COMMITTEE C_COMMITTEE= db.C_COMMITTEE
                //  .Include(o => o.C_COMMITTEE_PANEL.C_COMMITTEE_PANEL_MEMBER)
                //  .Where(o => o.UUID == delUuid).FirstOrDefault();
                //db.C_COMMITTEE_PANEL_MEMBER
                //    .RemoveRange(C_COMMITTEE.C_COMMITTEE_PANEL.C_COMMITTEE_PANEL_MEMBER);


                //model.PanelRole.Select(o =>
                //new C_COMMITTEE_PANEL_MEMBER()
                //{
                //    COMMITTEE_PANEL_ID = C_COMMITTEE.C_COMMITTEE_PANEL.UUID,
                //    MEMBER_ID = o.Key,
                //    PANEL_ROLE_ID = o.Value
                //});

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    string committeeUuid = "";
                    if (model.C_COMMITTEE.UUID != null)
                    {
                        committeeUuid = model.C_COMMITTEE.UUID;
                    }
                    else
                    {
                        C_COMMITTEE_PANEL cp = new C_COMMITTEE_PANEL();
                        cp.YEAR = model.C_COMMITTEE.YEAR;
                        cp.PANEL_TYPE_ID = model.C_COMMITTEE.C_COMMITTEE_PANEL.PANEL_TYPE_ID;
                        db.C_COMMITTEE_PANEL.Add(cp);
                        db.SaveChanges();
                       

                        C_COMMITTEE c_COMMITTEE = new C_COMMITTEE();
                        c_COMMITTEE.COMMITTEE_PANEL_ID = cp.UUID ;
                        c_COMMITTEE.COMMITTEE_TYPE_ID = model.C_COMMITTEE.COMMITTEE_TYPE_ID;
                        c_COMMITTEE.YEAR = model.C_COMMITTEE.YEAR;
                        db.C_COMMITTEE.Add(c_COMMITTEE);
                        db.SaveChanges();
                        committeeUuid =  c_COMMITTEE.UUID;
                    }

                    var ccmList = db.C_COMMITTEE_COMMITTEE_MEMBER.Where(x => x.COMMITTEE_ID == committeeUuid).ToList();
                    if (ccmList != null)
                    {
                        db.C_COMMITTEE_COMMITTEE_MEMBER.RemoveRange(ccmList);
                    }
                    else
                    {

                    }

                    if (model.PanelRole != null && model.PanelRole.Count() > 0)
                    {
                        foreach (var s in model.PanelRole)
                        {
                            C_COMMITTEE_COMMITTEE_MEMBER dummyCommitteeMember = new C_COMMITTEE_COMMITTEE_MEMBER();
                            dummyCommitteeMember.COMMITTEE_ID = committeeUuid;
                            dummyCommitteeMember.MEMBER_ID = s.Key;
                            dummyCommitteeMember.COMMITTEE_ROLE_ID = s.Value;

                            db.C_COMMITTEE_COMMITTEE_MEMBER.Add(dummyCommitteeMember);
                        }
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    transaction.Commit();

                }
            }
              

            //model.PanelRole
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        //public string ExportCP(Dictionary<string, string>[] Columns, FormCollection post)
        public string ExportCP(Fn06CMM_CPSearchModel model)
        {
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q_copy, Columns = Columns, Parameters = post };
            model.Query = Search_CP_q;  //need to change query after all
            model.QueryWhere = SearchCP_whereQ(model);
            //dlr.Sort = "indCert.certification_No";
            //dlr.SortType = 2;
            return model.Export("ExportData");
            //return dlr.Export("ExportData");
        }
        public void SearchCP(Fn06CMM_CPSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = Search_CP_q;  //need to change query after all
            model.QueryWhere = SearchCP_whereQ(model);
            model.Search();
        }
        private string SearchCP_whereQ(Fn06CMM_CPSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Year))
            {
                whereQ += "\r\n\t" + "AND CP.YEAR = :YEAR";
                model.QueryParameters.Add("YEAR", model.Year.Trim());
            }
           
            if (!string.IsNullOrWhiteSpace(model.Panel))
            {
                whereQ += "\r\n\t" + "AND SV.UUID = :PANEL_TYPE_ID";
                model.QueryParameters.Add("PANEL_TYPE_ID", model.Panel.Trim());
            }
            return whereQ;

        }
        public Fn06CMM_CPSearchModel LoadfCP(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                Fn06CMM_CPSearchModel model = new Fn06CMM_CPSearchModel();

                model.C_COMMITTEE_PANEL = db.C_COMMITTEE_PANEL
                                        .Where(x => x.UUID == id).FirstOrDefault();

                var q = from cp in db.C_COMMITTEE_PANEL
                        join cpm in db.C_COMMITTEE_PANEL_MEMBER on cp.UUID equals cpm.COMMITTEE_PANEL_ID
                        join cm in db.C_COMMITTEE_MEMBER on cpm.MEMBER_ID equals cm.UUID
                        where cp.UUID == id
                        select cpm;
                if(q.Any())
                    model.ExpiryDateTime = q.FirstOrDefault().EXPIRY_DATE;


                //model.C_COMMITTEE = db.C_COMMITTEE
                //    .Where(o => o.UUID == model.C_COMMITTEE.UUID)
                //    .Include(o => o.C_COMMITTEE_PANEL.C_COMMITTEE_PANEL_MEMBER.Select(o2 => o2.C_COMMITTEE_MEMBER))
                //    .Include(o => o.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                //    .Include(o => o.C_S_SYSTEM_VALUE)
                //    .FirstOrDefault();
                model.SearchType = 1;
                return model;
            }
        }
        public void SearchPanelMember(Fn06CMM_CPSearchModel model)
        {
            if (model.SearchType == 1)
            {
                getCommitteeMembers(model, "PANEL", null, model.Year, model.Panel, null, null, null, null, null);
            }
            else
            {

                getCommitteeMembers(model, "MEMBER", null, null, null, model.Surname, model.GivenName, null, null, null);
            }

        }
        public void SearchCommitteeMember(Fn06CMM_CPSearchModel model)
        {
            //getCommitteeMembers(model, "COMMITTEE", null, null, model.C_COMMITTEE.UUID, null, null);
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //model.C_COMMITTEE_PANEL = db.C_COMMITTEE_PANEL
                //                      .Where(x => x.UUID == model.C_COMMITTEE_PANEL.UUID)
                //                      .Include(x=>x.C_COMMITTEE)
                //                      .Include(x=>x.C_COMMITTEE.Select(o=>o.C_COMMITTEE_COMMITTEE_MEMBER))
                                    

                //                      .Include(x=>x.C_COMMITTEE_PANEL_MEMBER)
                //                      .Include(x => x.C_COMMITTEE_PANEL_MEMBER.Select(y=>y.C_COMMITTEE_MEMBER))
                //                      .FirstOrDefault();

            }
            string PanelUUID = model.C_COMMITTEE_PANEL.UUID;
            if (string.IsNullOrWhiteSpace(PanelUUID))
                PanelUUID = "-1";

            getCommitteeMembers(model, "PANEL", null, model.C_COMMITTEE_PANEL.YEAR.ToString(), PanelUUID, null, null, null, null, null);
            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            for (int i = 0; i < draftMember.Count; i++)
            {
                Dictionary<string, object> draftEach = draftMember[i];
                if (model.Data.Where(o => o["UUID"] as string == draftEach["UUID"] as string).ToList().Count == 0)
                    model.Data.Add(draftEach);
            }
            model.Data.RemoveAll(o => deleteMember.Contains(o["UUID"]));
            model.Total = model.Data.Count;
        }
        public ServiceResult AddToMember(Fn06CMM_CPSearchModel model)
        {
            getCommitteeMembers(model, "MEMBER", model.CheckMembers, null, null, null, null, null, null, null);
            //GetDBMember(model);
            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);


            if (model.Data != null)
            {
                for (int i = 0; i < model.Data.Count; i++)
                {
                    var currentData = model.Data[i];
                    if ((draftMember.Select(o => o["UUID"] as string)).Equals(currentData["UUID"] as string)) continue;
                    draftMember.Add(currentData);
                }
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public ServiceResult Save(Fn06CMM_CPSearchModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                bool result = true;

                var Query = db.C_COMMITTEE_PANEL.Where(x => x.YEAR == model.C_COMMITTEE_PANEL.YEAR && x.PANEL_TYPE_ID == model.C_COMMITTEE_PANEL.PANEL_TYPE_ID);
                if (Query.Any() && model.C_COMMITTEE_PANEL.UUID == null)
                {
                    result = false;
                }
                if (!result)
                {

                    Dictionary<string, List<string>> errormsg = new Dictionary<string, List<string>>();
                   
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "Sorry! Inputted Panel Type and Year are existing in the system " } };

                }
                // return new ValidationResult("Please enter " + v[VAR_PROP.LABEL] + ".", new List<string> { propName });


                C_COMMITTEE_PANEL cp = new C_COMMITTEE_PANEL();
                Dictionary<string, string> tempMemberStatus = new Dictionary<string, string>();
                foreach (var i in db.C_COMMITTEE_PANEL_MEMBER.Where(x => x.COMMITTEE_PANEL_ID == model.C_COMMITTEE_PANEL.UUID))
                {
                    tempMemberStatus.Add(i.MEMBER_ID, i.MEMBER_STATUS_ID);
                    db.C_COMMITTEE_PANEL_MEMBER.Remove(i);

                }
                if (model.C_COMMITTEE_PANEL.UUID == null)
                {
                 
                    cp.UUID = Guid.NewGuid().ToString().Replace("-", "");

                    cp.YEAR = model.C_COMMITTEE_PANEL.YEAR;
                    cp.PANEL_TYPE_ID = model.C_COMMITTEE_PANEL.PANEL_TYPE_ID;
                    cp.MODIFIED_BY = SystemParameterConstant.UserName;
                    cp.MODIFIED_DATE = DateTime.Now;
                    cp.CREATED_BY = SystemParameterConstant.UserName;
                    cp.CREATED_DATE = DateTime.Now;
                    db.C_COMMITTEE_PANEL.Add(cp);

                }
              // .RemoveRange(db.C_COMMITTEE_PANEL_MEMBER.Where(x => x.COMMITTEE_PANEL_ID == model.C_COMMITTEE_PANEL.UUID));
                foreach (var item in model.PanelRole)
                {
                    C_COMMITTEE_PANEL_MEMBER cpm = new C_COMMITTEE_PANEL_MEMBER();
                    // cpm.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    if (string.IsNullOrWhiteSpace(model.C_COMMITTEE_PANEL.UUID))
                    {
                        cpm.COMMITTEE_PANEL_ID = cp.UUID;
                    }
                    else
                    {
                        cpm.COMMITTEE_PANEL_ID = model.C_COMMITTEE_PANEL.UUID;
                    }
            
                    cpm.MEMBER_ID = item.Key;
                    cpm.PANEL_ROLE_ID = item.Value;
                    cpm.EXPIRY_DATE = model.ExpiryDateTime;
                    foreach (var k in tempMemberStatus)
                    {
                        if (k.Key == item.Key)
                        {
                            cpm.MEMBER_STATUS_ID = k.Value;
                        }
                    }
                    db.C_COMMITTEE_PANEL_MEMBER.Add(cpm);

                }

                db.SaveChanges();

            }

            //model.PanelRole
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult Delete(Fn06CMM_CPSearchModel model)
        {
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            if (model.PanelRole.Count == 0)
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            foreach (string key in model.PanelRole.Keys)
            {
                if (!deleteMember.Contains(key)) deleteMember.Add(key);
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public void SearchCG(Fn06CMM_CGSearchModel model)
        {
            //model.Query = SearchCA_q;
            //model.Query = SearchCMM_q;  //need to change query after all
            //   model.QueryWhere =

            model.Query = Search_CG_q;
            model.QueryWhere = SearchCG_whereQ(model);
            model.Sort = "YEAR DESC ,CODE ASC ,  NAME";
            model.Search();
        }
        
        public string ExportCG(Fn06CMM_CGSearchModel model)
        {
            model.Query = Search_CG_q;
            model.QueryWhere = SearchCG_whereQ(model);
            model.Sort = "YEAR DESC ,CODE ASC ,  NAME";
            return model.Export("ExportData");
        }

        private string SearchCG_whereQ(Fn06CMM_CGSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Year))
            {
                whereQ += "\r\n\t" + "AND CG.YEAR = :YEAR";
                model.QueryParameters.Add("YEAR", model.Year.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.Panel))
            {
                whereQ += "\r\n\t" + "AND cp.PANEL_TYPE_ID = :Panel";
                model.QueryParameters.Add("Panel", model.Panel.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.Month))
            {
                whereQ += "\r\n\t" + "AND CG.MONTH = :MONTH";
                model.QueryParameters.Add("MONTH", model.Month.ToString());
            }
            if (!string.IsNullOrWhiteSpace(model.CommitteeGroup))
            {
                whereQ += "\r\n\t" + "AND CG.NAME = :CommitteeGroup";
                model.QueryParameters.Add("CommitteeGroup", model.CommitteeGroup);
            }
            if (!string.IsNullOrWhiteSpace(model.Committee))
            {
                whereQ += "\r\n\t" + "AND CG.COMMITTEE_TYPE_ID = :Committee";
                model.QueryParameters.Add("Committee", model.Committee);
            }
            return whereQ;

        }

        public Fn06CMM_CGSearchModel LoadfCG(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                Fn06CMM_CGSearchModel model = new Fn06CMM_CGSearchModel();

                model.C_COMMITTEE_GROUP = db.C_COMMITTEE_GROUP.Where(x => x.UUID == id)
                                            .Include(x=>x.C_COMMITTEE)
                                            .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL)              
                                            .FirstOrDefault();


                model.SearchType = 1;
                return model;
            }
        }
        public void SearchCommitteeMember(Fn06CMM_CGSearchModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var q  = db.C_COMMITTEE_GROUP.Where(x => x.UUID == model.C_COMMITTEE_GROUP.UUID)
                                            .Include(x => x.C_COMMITTEE)
                                            .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL)
                                            .FirstOrDefault();
                if (q == null)
                {
                    getCommitteeMembers(model, "GROUP", null, null,"-1", null, null, null, null, null);
                }
                else
                {
                    getCommitteeMembers(model, "GROUP", null, q.YEAR.ToString(), q.C_COMMITTEE.C_COMMITTEE_PANEL.UUID, null, null, q.COMMITTEE_TYPE_ID, q.NAME, q.MONTH.ToString());
                }

            }

            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            for (int i = 0; i < draftMember.Count; i++)
            {
                Dictionary<string, object> draftEach = draftMember[i];
                if (model.Data.Where(o => o["UUID"] as string == draftEach["UUID"] as string).ToList().Count == 0)
                    model.Data.Add(draftEach);
            }
            model.Data.RemoveAll(o => deleteMember.Contains(o["UUID"]));
            model.Total = model.Data.Count;
        }

        public void SearchPanelMember(Fn06CMM_CGSearchModel model)
        {
            //getCommitteeMembers(model, "PANEL", null, model.Year, model.Panel, model.Surname, model.GivenName, null, null, null);
            if (model.SearchType == 1)
            {
                getCommitteeMembers(model, "PANEL", null, model.Year, model.Panel, null, null, null, null, null);
            }
            else
            {

                getCommitteeMembers(model, "PANEL", null, null, null, model.Surname, model.GivenName, null, null, null);
            }

        }
        public ServiceResult AddToMember(Fn06CMM_CGSearchModel model)
        {
            getCommitteeMembers(model, "MEMBER", model.CheckMembers, null, null, null, null, null, null, null);
            //GetDBMember(model);
            List<Dictionary<string, object>> draftMember = SessionUtil.DraftList<Dictionary<string, object>>(ApplicationConstant.DRAFT_KEY_MEMBER);


            if (model.Data != null)
            {
                for (int i = 0; i < model.Data.Count; i++)
                {
                    var currentData = model.Data[i];
                    if ((draftMember.Select(o => o["UUID"] as string)).Equals(currentData["UUID"] as string)) continue;
                    draftMember.Add(currentData);
                }
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult Delete(Fn06CMM_CGSearchModel model)
        {
            List<string> deleteMember = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_MEMBER);
            if (model.PanelRole.Count == 0)
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            foreach (string key in model.PanelRole.Keys)
            {
                if (!deleteMember.Contains(key)) deleteMember.Add(key);
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult Save(Fn06CMM_CGSearchModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                bool result = true;

                //var Query = db.C_COMMITTEE_PANEL.Where(x => x.YEAR == model.C_COMMITTEE_PANEL.YEAR && x.PANEL_TYPE_ID == model.C_COMMITTEE_PANEL.PANEL_TYPE_ID);
                //if (Query.Any() && model.C_COMMITTEE_PANEL.UUID == null)
                //{
                //    result = false;
                //}
                //    if (!result)
                //    {

                //        Dictionary<string, List<string>> errormsg = new Dictionary<string, List<string>>();

                //        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "Sorry! Inputted Panel Type and Year are existing in the system " } };

                //    }
                //    // return new ValidationResult("Please enter " + v[VAR_PROP.LABEL] + ".", new List<string> { propName });

                var Query = db.C_COMMITTEE_GROUP.Where(
                    x => x.C_COMMITTEE.C_COMMITTEE_PANEL.PANEL_TYPE_ID == model.C_COMMITTEE_GROUP.C_COMMITTEE.C_COMMITTEE_PANEL.PANEL_TYPE_ID
                    && x.C_COMMITTEE.COMMITTEE_TYPE_ID ==model.C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID
                    && x.NAME == model.C_COMMITTEE_GROUP.NAME
                    && x.YEAR ==model.C_COMMITTEE_GROUP.YEAR
                    && x.MONTH == model.C_COMMITTEE_GROUP.MONTH

                    );
                        
                if (Query.Any() && model.C_COMMITTEE_GROUP.UUID == null)
                {
                    result = false;
                }
                if (!result)
                {

                    Dictionary<string, List<string>> errormsg = new Dictionary<string, List<string>>();

                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "Sorry! Inputted Panel Type, Committee Type , CommitteeGroup, Year and Month are existing in the system" } };

                }

                result = true;
                var comQ = db.C_COMMITTEE.
                    Where(x => x.COMMITTEE_TYPE_ID == model.C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID
                    && x.YEAR == model.C_COMMITTEE_GROUP.YEAR);
                if (!comQ.Any() && model.C_COMMITTEE_GROUP.UUID == null)
                {
                    result = false;
                }
                if (!result)
                {

                    Dictionary<string, List<string>> errormsg = new Dictionary<string, List<string>>();

                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "Sorry! Inputted Panel Type, Commitee Type and Year are NOT existing in the system" } };

                }
                C_COMMITTEE_GROUP cg = new C_COMMITTEE_GROUP();
                if (model.C_COMMITTEE_GROUP.UUID == null)
                {
                 
                    cg.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    cg.NAME = model.C_COMMITTEE_GROUP.NAME;
                    cg.YEAR = model.C_COMMITTEE_GROUP.YEAR;
                    cg.COMMITTEE_TYPE_ID = model.C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID;
                    cg.MONTH = model.C_COMMITTEE_GROUP.MONTH;
                    var c = db.C_COMMITTEE.Where(x => x.COMMITTEE_TYPE_ID == model.C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID && x.YEAR == model.C_COMMITTEE_GROUP.YEAR);
                    cg.COMMITTEE_ID = c.FirstOrDefault().UUID;
                    cg.MODIFIED_BY = SystemParameterConstant.UserName;
                    cg.MODIFIED_DATE = DateTime.Now;
                    cg.CREATED_BY = SystemParameterConstant.UserName;
                    cg.CREATED_DATE = DateTime.Now;
                    db.C_COMMITTEE_GROUP.Add(cg);
                }




                db.C_COMMITTEE_GROUP_MEMBER.
                    RemoveRange(
                      db.C_COMMITTEE_GROUP_MEMBER
                      .Where(x => x.COMMITTEE_GROUP_ID == model.C_COMMITTEE_GROUP.UUID));

                foreach (var item in model.PanelRole)
                {
                    C_COMMITTEE_GROUP_MEMBER cgm = new C_COMMITTEE_GROUP_MEMBER();

                    // cpm.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    if (string.IsNullOrWhiteSpace(model.C_COMMITTEE_GROUP.UUID))
                    {
                        cgm.COMMITTEE_GROUP_ID = cg.UUID;
                    }
                    else
                    {
                        cgm.COMMITTEE_GROUP_ID = model.C_COMMITTEE_GROUP.UUID;

                    }

                    cgm.MEMBER_ID = item.Key;
                    cgm.COMMITTEE_ROLE_ID = item.Value;
         
                    db.C_COMMITTEE_GROUP_MEMBER.Add(cgm);

                }
                
                db.SaveChanges();

                //    C_COMMITTEE_PANEL cp = new C_COMMITTEE_PANEL();
                //    Dictionary<string, string> tempMemberStatus = new Dictionary<string, string>();
                //    foreach (var i in db.C_COMMITTEE_PANEL_MEMBER.Where(x => x.COMMITTEE_PANEL_ID == model.C_COMMITTEE_PANEL.UUID))
                //    {
                //        tempMemberStatus.Add(i.MEMBER_ID, i.MEMBER_STATUS_ID);
                //        db.C_COMMITTEE_PANEL_MEMBER.Remove(i);

                //    }
                //    if (model.C_COMMITTEE_PANEL.UUID == null)
                //    {

                //        cp.UUID = Guid.NewGuid().ToString().Replace("-", "");

                //        cp.YEAR = model.C_COMMITTEE_PANEL.YEAR;
                //        cp.PANEL_TYPE_ID = model.C_COMMITTEE_PANEL.PANEL_TYPE_ID;
                //        cp.MODIFIED_BY = SystemParameterConstant.UserName;
                //        cp.MODIFIED_DATE = DateTime.Now;
                //        cp.CREATED_BY = SystemParameterConstant.UserName;
                //        cp.CREATED_DATE = DateTime.Now;
                //        db.C_COMMITTEE_PANEL.Add(cp);

                //    }
                //    // .RemoveRange(db.C_COMMITTEE_PANEL_MEMBER.Where(x => x.COMMITTEE_PANEL_ID == model.C_COMMITTEE_PANEL.UUID));
                //    foreach (var item in model.PanelRole)
                //    {
                //        C_COMMITTEE_PANEL_MEMBER cpm = new C_COMMITTEE_PANEL_MEMBER();
                //        // cpm.UUID = Guid.NewGuid().ToString().Replace("-", "");
                //        if (string.IsNullOrWhiteSpace(model.C_COMMITTEE_PANEL.UUID))
                //        {
                //            cpm.COMMITTEE_PANEL_ID = cp.UUID;
                //        }
                //        else
                //        {
                //            cpm.COMMITTEE_PANEL_ID = model.C_COMMITTEE_PANEL.UUID;
                //        }

                //        cpm.MEMBER_ID = item.Key;
                //        cpm.PANEL_ROLE_ID = item.Value;
                //        cpm.EXPIRY_DATE = model.ExpiryDateTime;
                //        foreach (var k in tempMemberStatus)
                //        {
                //            if (k.Key == item.Key)
                //            {
                //                cpm.MEMBER_STATUS_ID = k.Value;
                //            }
                //        }
                //        db.C_COMMITTEE_PANEL_MEMBER.Add(cpm);

                //    }

                //    db.SaveChanges();

            }

            //model.PanelRole
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public string GetPAbyHKID(string hkid)
        {
            string uuid = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var enHKID = EncryptDecryptUtil.getEncrypt(hkid);
                var query = db.C_IND_APPLICATION.Where(x => x.C_APPLICANT.HKID == enHKID);


                if (!query.Any())
                {
                    return uuid;
                }
                else
                {
                    uuid = query.FirstOrDefault().UUID;
                }
               

            }

            return uuid;

        }

    }
}