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
using MWMS2.Constant;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class RegistrationGCAService
    {

        String SearchTemp_q = ""
                         + "\r\n" + "\t" + "SELECT                                                               "
                         + "\r\n" + "\t" + "T1.*,T2.ENGLISH_DESCRIPTION                                          "
                         + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
                         + "\r\n" + "\t" + "WHERE T1.REGISTRATION_TYPE in ('CGC' ,'CMW')                         "
                        ;

        String SearchGCN_q = ""
                         + "\r\n" + "\t" + "SELECT                                                               "
                         + "\r\n" + "\t" + "T1.*,T2.ENGLISH_DESCRIPTION                                          "
                         + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
                         + "\r\n" + "\t" + "WHERE 1=1                                                            "
                         + "\r\n" + "\t" + "AND T1.REGISTRATION_TYPE ='CGC'                                      "
                        ;

        String SearchIC_q = ""
                         + "\r\n" + "\t" + "SELECT sch.UUID,                                                              "
                         + "\r\n" + "\t" + "sch.MEETING_NUMBER as MEETING_NUMBER, sch.INTERVIEW_DATE as INTERVIEW_DATE,   "
                         + "\r\n" + "\t" + "sv2.CODE as CODE, room.ROOM as ROOM, sch.IS_CANCEL as IS_CANCELED             "
                         + "\r\n" + "\t" + "FROM C_INTERVIEW_SCHEDULE sch                                                 "
                         + "\r\n" + "\t" + "LEFT JOIN C_MEETING meeting on meeting.UUID = sch.MEETING_ID                  "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv1 on sv1.UUID = meeting.COMMITTEE_TYPE_ID        "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_ROOM room on room.UUID = sch.ROOM_ID                            "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv2 on sv2.UUID = sch.TIME_SESSION_ID              "
                         + "\r\n" + "\t" + "WHERE 1=1                                                                     "
                         + "\r\n" + "\t" + "AND sv1.REGISTRATION_TYPE = 'CGC'                                             " ;

        //the last col under monitor depends on PM UUID is null or not  -> null = NO , have UUID = YES
        String SearchPM_q = ""
                      + "\r\n" + "\t" + "SELECT A.PM_UUID, A.COMP_UUID, A.COMP_APPL_UUID, A.RECORD_TYPE,  A.FILE_REFERENCE_NO,                                "
                       + "\r\n" + "\t" + " concat(concat( A.SURNAME,' '), A.GIVEN_NAME_ON_ID) AS NAME,  A.CODE, A.APPLICATION_TYPE,                                                         "
                       + "\r\n" + "\t" + " A.RECEIVED_DATE, A.ENGLISH_COMPANY_NAME                                                                             "
                       + "\r\n" + "\t" + " FROM  (  SELECT  '' AS PM_UUID,  COM.UUID AS COMP_UUID,  APPINFO.UUID AS COMP_APPL_UUID,                            "
                       + "\r\n" + "\t" + " 'NEW' AS RECORD_TYPE,  COM.FILE_REFERENCE_NO,   APP.SURNAME,                                                        "
                       + "\r\n" + "\t" + " APP.GIVEN_NAME_ON_ID,  APP.CHINESE_NAME,   COM.ENGLISH_COMPANY_NAME,                                                "
                       + "\r\n" + "\t" + " V.CODE, V2.CODE AS APPLICATION_TYPE, NULL AS RECEIVED_DATE  FROM C_COMP_APPLICATION COM,                            "
                       + "\r\n" + "\t" + " C_COMP_APPLICANT_INFO APPINFO,  C_APPLICANT APP, C_S_SYSTEM_VALUE V, C_S_SYSTEM_VALUE V2                            "
                       + "\r\n" + "\t" + " WHERE COM.UUID=APPINFO.MASTER_ID   AND COM.REGISTRATION_TYPE= 'CGC'  AND APPINFO.APPLICANT_ID =APP.UUID             "
                       + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ROLE_ID=V.UUID  AND COM.APPLICATION_FORM_ID=V2.UUID                                           "
                       + "\r\n" + "\t" + " AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE  upper('%a%')  OR COM.CHINESE_COMPANY_NAME LIKE  upper('a') )             "
                       + "\r\n" + "\t" + " UNION ALL  SELECT  PRM.UUID AS PM_UUID,  PRM.MASTER_ID AS COMP_UUID,                                                "
                       + "\r\n" + "\t" + " PRM.COMPANY_APPLICANTS_ID AS COMP_APPL_UUID,  'EDIT',  COM.FILE_REFERENCE_NO,                                       "
                       + "\r\n" + "\t" + " APP.SURNAME,  APP.GIVEN_NAME_ON_ID,  APP.CHINESE_NAME,  COM.ENGLISH_COMPANY_NAME,                                   "
                       + "\r\n" + "\t" + " V.CODE, V2.CODE AS APPLICATION_TYPE, PRM.RECEIVED_DATE  AS RECEIVED_DATE                                            "
                       + "\r\n" + "\t" + " FROM C_COMP_PROCESS_MONITOR PRM, C_COMP_APPLICATION COM,  C_COMP_APPLICANT_INFO APPINFO,  C_APPLICANT APP,          "
                       + "\r\n" + "\t" + " C_S_SYSTEM_VALUE V, C_S_SYSTEM_VALUE V2  WHERE  PRM.MONITOR_TYPE= 'UPM'  AND COM.REGISTRATION_TYPE= 'CGC'           "
                       + "\r\n" + "\t" + " AND PRM.MASTER_ID = COM.UUID  AND PRM.COMPANY_APPLICANTS_ID = APPINFO.UUID  AND APPINFO.APPLICANT_ID =APP.UUID      "
                       + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ROLE_ID=V.UUID  AND PRM.APPLICATION_FORM_ID=V2.UUID                                           "
                       + "\r\n" + "\t" + " AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE  upper('%a%')  OR COM.CHINESE_COMPANY_NAME LIKE  '%a%' ) ) A              ";
        //+ "\r\n" + "\t" + " ORDER BY A.FILE_REFERENCE_NO asc, A.RECORD_TYPE asc, A.RECEIVED_DATE desc,  A.CODE asc                              ";          

        String SearchPM_10Days_q = ""
                                    + "\r\n" + "\t" + "SELECT A.PM_UUID, A.COMP_UUID, A.RECORD_TYPE,  A.FILE_REFERENCE_NO,   A.RECEIVED_DATE, A.ENGLISH_COMPANY_NAME                                                            "
                                    + "\r\n" + "\t" + "FROM  (                                                                                                                                                                  "
                                    + "\r\n" + "\t" + "SELECT  '' AS PM_UUID,  COM.UUID AS COMP_UUID,  'NEW' AS RECORD_TYPE,  COM.FILE_REFERENCE_NO,   COM.ENGLISH_COMPANY_NAME,  NULL AS RECEIVED_DATE                         "
                                    + "\r\n" + "\t" + "FROM C_COMP_APPLICATION COM                                                                                                                                              "
                                    + "\r\n" + "\t" + "WHERE COM.REGISTRATION_TYPE= 'CGC'                                                                                                                                       "
                                    + "\r\n" + "\t" + "AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE  '%A%'                                                                                                                         "
                                    + "\r\n" + "\t" + "OR COM.CHINESE_COMPANY_NAME LIKE  '%a%' )                                                                                                                                "
                                    + "\r\n" + "\t" + "UNION ALL  SELECT  PRM.UUID AS PM_UUID,  PRM.MASTER_ID AS COMP_UUID,  'EDIT',  COM.FILE_REFERENCE_NO,  COM.ENGLISH_COMPANY_NAME,  PRM.RECEIVED_DATE  AS RECEIVED_DATE    "
                                    + "\r\n" + "\t" + "FROM C_COMP_PROCESS_MONITOR PRM, C_COMP_APPLICATION COM                                                                                                                  "
                                    + "\r\n" + "\t" + "WHERE  PRM.MONITOR_TYPE= 'UPM_10DAYS'                                                                                                                                    "
                                    + "\r\n" + "\t" + "AND COM.REGISTRATION_TYPE= 'CGC'  AND PRM.MASTER_ID = COM.UUID  AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE  'A'                                                           "
                                    + "\r\n" + "\t" + "OR COM.CHINESE_COMPANY_NAME LIKE  'a' ) ) A                                                                                                                              ";
        // + "\r\n" + "\t" + "ORDER BY A.FILE_REFERENCE_NO asc, A.RECORD_TYPE asc, A.RECEIVED_DATE desc                                                                                                ";




        public DisplayGrid SearchTemp(FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchTemp_q, Parameters = post };
            dlr.Search();
            return dlr;
        }
        public string ExportTemp(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchTemp_q, Columns = Columns , Parameters = post };
            return dlr.Export("ExportData");
        }

        public void SaveApplicant(C_COMP_APPLICATION C_COMP_APPLICATION)
        {

        }
       

        public DisplayGrid SearchPM(FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchPM_q, Parameters = post };
            dlr.Search();
            return dlr;
        }
        public string ExportPM(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchPM_q, Columns = Columns, Parameters = post };
            return dlr.Export("ExportData");
        }

        public DisplayGrid SearchPM_10Days(FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchPM_10Days_q, Parameters = post };
            dlr.Search();
            return dlr;
        }
        public string ExportPM_10Days(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchPM_10Days_q, Columns = Columns, Parameters = post };
            return dlr.Export("ExportData");
        }


        //FN02
        public void SearchCA(Fn02GCA_CASearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp_q;  //need to change query after all
            model.Search();
        }
        
        //edit on 20190426
        //public Fn02GCA_PMSearchModel SearchPM(Fn02GCA_PMSearchModel model)
        //{
        //    model.Query = SearchCA_q;
        //    model.Query = SearchGCA_q;  //need to change query after all
        //    model.Query = SearchGCA_PM(model);
        //    model.Search();
        //    return model;
        //}
        public void SearchPM10P(Fn02GCA_PM10PSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp_q;  //need to change query after all
            model.Search();
        }

        public void SearchMFT(Fn02GCA_MFTSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp_q;  //need to change query after all
            model.Search();
        }

        public Fn02GCA_GCNSearchModel SearchGCN(Fn02GCA_GCNSearchModel model)
        {
            //model.Query = SearchCA_q;
            // RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            // model = registrationCommonService.SearchCompApplicationByFileRefAndCompNameA(model);
            model.Query = SearchGCN_q;  //need to change query after all
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchGCN_whereQ(Fn02GCA_GCNSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND UPPER(FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + "AND UPPER(ENGLISH_COMPANY_NAME) LIKE :ComName";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
           
            return whereQ;
        }

        public Fn02GCA_MRASearchModel SearchMRA(Fn02GCA_MRASearchModel model)
        {

            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
           return  registrationCommonService.ViewReservedRoom(model);
            
            //model.Query = SearchCA_q;
            //  RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            //  model = registrationCommonService.ViewReservedRoom(model);
        }

        public Fn02GCA_MRASearchModel ExportMRA(Fn02GCA_MRASearchModel model)
        {

            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            return registrationCommonService.ExportReservedRoom(model);

            //model.Query = SearchCA_q;
            //  RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            //  model = registrationCommonService.ViewReservedRoom(model);
        }
        //public Fn02GCA_MRASearchModel SearchMRAAvailable(Fn02GCA_MRASearchModel model)
        //{

        //    RegistrationCommonService registrationCommonService = new RegistrationCommonService();
        //    return registrationCommonService.ViewAvailableRoom(model);

        //}


        public void SearchIC(Fn02GCA_ICSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchIC_q;  //need to change query after all
            model.Search();
        }


        // Andy modified on 30/04/2019, search interview candidate
        public void SearchIR(InterviewResultSearchModel model)
        {
            //model.Query = SearchCA_q;
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            model = registrationCommonService.SearchInterviewResultForCompany(model);

        }

        public Fn07CNV_CNVDisplayModel ViewCompGCN(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = from CompCNV in db.C_COMP_CONVICTION
                            where CompCNV.UUID == id
                            select CompCNV;
                return new Fn07CNV_CNVDisplayModel()
                { C_COMP_CONVICTION = query.FirstOrDefault() };
            }

        }


    }
}