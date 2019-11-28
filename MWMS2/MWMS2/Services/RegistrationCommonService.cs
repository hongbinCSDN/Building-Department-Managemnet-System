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
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Web.Configuration;

namespace MWMS2.Services
{
    public class RegistrationCommonService: BaseCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String SEPARATOR = ",";
        private String DOUBLEQUOTE =	"\"";


        string SearchBU_q = ""
               + "\r\n" + "\t" + "select UUID,FIlE_PATH,FILE_NAME,CREATED_DATE "
               + "\r\n" + "\t" + "from C_BATCH_UPLOAD_QP_EXPERIENCE";

        string SearchNoDayCompLeaveFormRecord_q = ""
                + "\r\n" + "\t" + " select "
                + "\r\n" + "\t" + " compAppInfo.master_id as MasterId, "
                + "\r\n" + "\t" + " compAppInfo.UUID as compUUID, "
                + "\r\n" + "\t" + " compApp.CERTIFICATION_NO as compCert, "
                + "\r\n" + "\t" + " compApp.ENGLISH_COMPANY_NAME as EngComp, "
                + "\r\n" + "\t" + " concat(concat( appl.SURNAME,' '), appl.GIVEN_NAME_ON_ID) as EngFullName, "
                + "\r\n" + "\t" + " sv.code as Role "
                + "\r\n" + "\t" + " FROM C_Comp_Applicant_Info compAppInfo "
                + "\r\n" + "\t" + " inner join C_comp_application compApp on compAppInfo.MASTER_ID = compApp.uuid "
                + "\r\n" + "\t" + " inner join c_applicant appl on appl.uuid = compAppInfo.APPLICANT_ID "
                + "\r\n" + "\t" + " inner join C_s_system_value sv on sv.uuid = compAppInfo.APPLICANT_ROLE_ID "
                + "\r\n" + "\t" + " where 1=1 "
                + "\r\n" + "\t" + " AND (compApp.CERTIFICATION_NO is not null) "
                + "\r\n" + "\t" + " AND compApp.EXPIRY_DATE>=sysdate "
                + "\r\n" + "\t" + " AND ((compApp.REMOVAL_DATE is null) or (compApp.REMOVAL_DATE>sysdate)) "
                + "\r\n" + "\t" + " AND ((compAppInfo.REMOVAL_DATE is null) or (compAppInfo.REMOVAL_DATE>sysdate)) "
                + "\r\n" + "\t" + " AND (sv.code like 'A%') "
                ;

        string SearchCompLeaveFormRecord_q =""
                + "\r\n" + "\t" + " select "
                + "\r\n" + "\t" + " leaveForm.MASTER_ID as leaveMasterId, "
                + "\r\n" + "\t" + " leaveForm.COMPANY_APPLICANT_ID as leaveCompAppID, "
                + "\r\n" + "\t" + " compAppInfo.master_id as masterId, "
                + "\r\n" + "\t" + " compAppInfo.UUID as compUUID, "
                + "\r\n" + "\t" + " compApp.CERTIFICATION_NO as compCert, "
                + "\r\n" + "\t" + " compApp.ENGLISH_COMPANY_NAME as EngComp, "
                + "\r\n" + "\t" + " concat(concat( appl.SURNAME,' '), appl.GIVEN_NAME_ON_ID) as EngFullName, "
                + "\r\n" + "\t" + " sv.code as Role "
                + "\r\n" + "\t" + " FROM  C_LEAVE_FORM leaveForm "
                + "\r\n" + "\t" + " inner join C_Comp_Applicant_Info compAppInfo on compAppInfo.uuid = leaveForm.COMPANY_APPLICANT_ID "
                + "\r\n" + "\t" + " inner join C_comp_application compApp on compAppInfo.MASTER_ID = compApp.UUID "
                + "\r\n" + "\t" + " inner join c_applicant appl on appl.uuid = compAppInfo.APPLICANT_ID "
                + "\r\n" + "\t" + " inner join C_s_system_value sv on sv.uuid = compAppInfo.APPLICANT_ROLE_ID "
                + "\r\n" + "\t" + " where 1=1 "

            ;

        string SearchNoDayIndLeaveFormRecord_q = ""
                + "\r\n" + "\t" + " select "
                + "\r\n" + "\t" + " indApp.UUID as indAppUUID, "
                + "\r\n" + "\t" + " indCert.UUID as indCertUUID, "
                + "\r\n" + "\t" + " indCert.CERTIFICATION_NO as indCertNo, "
                + "\r\n" + "\t" + " concat(concat( appl.SURNAME,' '), appl.GIVEN_NAME_ON_ID) as EngFullName, "
                + "\r\n" + "\t" + " appl.CHINESE_NAME as CHINAME "
                + "\r\n" + "\t" + " FROM C_IND_APPLICATION indApp "
                + "\r\n" + "\t" + " inner join C_IND_CERTIFICATE indCert on indApp.UUID = indCert.MASTER_ID "
                + "\r\n" + "\t" + " inner join C_APPLICANT appl on appl.uuid = indApp.APPLICANT_ID "
                + "\r\n" + "\t" + " where 1=1 "
                + "\r\n" + "\t" + " AND indCert.CERTIFICATION_NO is not null "
                + "\r\n" + "\t" + " AND indCert.EXPIRY_DATE >=sysdate "
                + "\r\n" + "\t" + " AND ((indCert.REMOVAL_DATE is null ) or (indCert.REMOVAL_DATE > sysdate)) "
            ;

        string SearchIndLeaveFormRecord_q = ""
                + "\r\n" + "\t" + " select "
                + "\r\n" + "\t" + " indApp.UUID as indAppUUID, "
                + "\r\n" + "\t" + " indCert.UUID as indCertUUID, "
                + "\r\n" + "\t" + " indCert.CERTIFICATION_NO as indCertNo, "
                + "\r\n" + "\t" + " concat(concat( appl.SURNAME,' '), appl.GIVEN_NAME_ON_ID) as EngFullName, "
                + "\r\n" + "\t" + " appl.CHINESE_NAME as CHINAME "
                + "\r\n" + "\t" + " FROM C_LEAVE_FORM leaveForm "
                + "\r\n" + "\t" + " inner join C_IND_CERTIFICATE indCert on leaveForm.CERTIFICATION_NO = indCert.CERTIFICATION_NO "
                + "\r\n" + "\t" + " inner join C_IND_APPLICATION indApp on indApp.UUID = indCert.MASTER_ID"
                + "\r\n" + "\t" + " inner join C_APPLICANT appl on appl.uuid = indApp.APPLICANT_ID "
                + "\r\n" + "\t" + " where 1=1 "
            ;

        public string SearchPAandMWIA_PM_q(ProcessMonitorSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND APP.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.FILE_REFERENCE_NO) LIKE :FileRef)";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + " AND (upper(P.SURNAME) LIKE :SurName)";
                model.QueryParameters.Add("SurName", "%" + model.SurName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(P.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + " AND (upper(P.CHINESE_NAME) LIKE :ChiName)";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("P.HKID") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("P.PASSPORT_NO") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            return ""
                + "\r\n" + "\t" + " SELECT A.RECORD_TYPE as RECORD_TYPE, "
                + "\r\n" + "\t" + " A.PM_UUID as PM_UUID, A.CERT_UUID as CERT_UUID, "
                + "\r\n" + "\t" + " CASE WHEN A.PM_UUID is null THEN 'No' ELSE 'Yes' END as UM, "
                + "\r\n" + "\t" + " concat(concat( A.SURNAME,' '), A.GIVEN_NAME_ON_ID) as NAME, "
                + "\r\n" + "\t" + " A.FILE_REFERENCE_NO as FILE_REFERENCE_NO, "
                + "\r\n" + "\t" + " A.CAT_CODE_DESC as CAT_CODE_DESC, A.RECEIVED_DATE as RECEIVED_DATE, "
                //+ "\r\n" + "\t" + " A.DUE_DATE,'" + string.IsNullOrWhiteSpace(model.FileRef) + "' as VETTING_OFFICER, '" + model.FileRef+"' AS TYPE_OF_APPLICATION, "
                + "\r\n" + "\t" + " A.DUE_DATE,A.VETTING_OFFICER,A.CODE AS TYPE_OF_APPLICATION, "
                + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL(" NVL(A.HKID,A.PASSPORT_NO ) ") +" as HKID "
                + "\r\n" + "\t" + " FROM( "
                + "\r\n" + "\t" + " SELECT 'EDIT' AS RECORD_TYPE,  IND.UUID AS PM_UUID,  '' AS CERT_UUID, "
                + "\r\n" + "\t" + " APP.FILE_REFERENCE_NO,  P.SURNAME,  P.GIVEN_NAME_ON_ID,  CAT_CODE.CODE AS CAT_CODE_DESC, IND.RECEIVED_DATE,  "
                + "\r\n" + "\t" + " IND.DUE_DATE,IND.VETTING_OFFICER,S.CODE , P.HKID ,P.PASSPORT_NO "
                + "\r\n" + "\t" + " FROM "
                + "\r\n" + "\t" + " C_IND_PROCESS_MONITOR IND LEFT JOIN C_S_SYSTEM_VALUE S "
                + "\r\n" + "\t" + " ON IND.UUID = S.UUID , C_IND_APPLICATION APP, "
                + "\r\n" + "\t" + " C_APPLICANT P, C_S_CATEGORY_CODE CAT_CODE  WHERE  IND.MASTER_ID=APP.UUID "
                + "\r\n" + "\t" + " AND APP.APPLICANT_ID=P.UUID  AND IND.CATEGORY_ID=CAT_CODE.UUID "
                + "\r\n" + "\t" + whereQ
                + "\r\n" + "\t" + " UNION ALL  "
                + "\r\n" + "\t" + " SELECT  'NEW' AS RECORD_TYPE,  '' AS PM_UUID,  CERT.UUID AS CERT_UUID, "
                + "\r\n" + "\t" + " APP.FILE_REFERENCE_NO,  P.SURNAME,  P.GIVEN_NAME_ON_ID,  CAT_CODE.CODE AS CAT_CODE_DESC, "
                + "\r\n" + "\t" + " NULL AS RECEIVED_DATE, NULL AS DUE_DATE,NULL AS VETTING_OFFICER,NULL AS CODE,P.HKID,P.PASSPORT_NO   FROM C_IND_APPLICATION APP, C_APPLICANT P, C_IND_CERTIFICATE CERT, "
                + "\r\n" + "\t" + " C_S_CATEGORY_CODE CAT_CODE  WHERE APP.APPLICANT_ID=P.UUID  AND APP.UUID=CERT.MASTER_ID "
                + "\r\n" + "\t" + "AND CERT.CATEGORY_ID = CAT_CODE.UUID "
                + "\r\n" + "\t" + whereQ
                + "\r\n" + "\t" + ") A ";
        }
        public string SearchGCAandMWCA_PM_q(ProcessMonitorSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND COM.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);

            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND upper(COM.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + " AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE :ComName OR COM.CHINESE_COMPANY_NAME LIKE:ComName)";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
            return ""
                + "\r\n" + "\t" + " SELECT  " 
                + "\r\n" + "\t" + " A.PM_UUID as PM_UUID, "
                + "\r\n" + "\t" + " CASE WHEN A.PM_UUID is null THEN 'No' ELSE 'Yes' END as UM, "
                + "\r\n" + "\t" + " A.COMP_UUID as COMP_UUID, A.COMP_APPL_UUID as COMP_APPL_UUID, "
                + "\r\n" + "\t" + " A.RECORD_TYPE as RECORD_TYPE,  A.FILE_REFERENCE_NO as FILE_REFERENCE_NO, "
                + "\r\n" + "\t" + " concat(concat( A.SURNAME,' '), A.GIVEN_NAME_ON_ID) AS NAME, A.CODE as CODE, "
                + "\r\n" + "\t" + " A.APPLICATION_TYPE as APPLICATION_TYPE, "
                + "\r\n" + "\t" + " A.RECEIVED_DATE as RECEIVED_DATE, A.ENGLISH_COMPANY_NAME as ENGLISH_COMPANY_NAME, "
                //For UR
                + "\r\n" + "\t" + " A.VETTING_OFFICER AS VETTING_OFFICER "

                + "\r\n" + "\t" + " FROM (SELECT  '' AS PM_UUID,  COM.UUID AS COMP_UUID, "
                + "\r\n" + "\t" + " APPINFO.UUID AS COMP_APPL_UUID,  'NEW' AS RECORD_TYPE, "
                + "\r\n" + "\t" + " COM.FILE_REFERENCE_NO,   APP.SURNAME,   APP.GIVEN_NAME_ON_ID, "
                + "\r\n" + "\t" + " APP.CHINESE_NAME,   COM.ENGLISH_COMPANY_NAME,  V.CODE, V2.CODE AS APPLICATION_TYPE, "
                + "\r\n" + "\t" + " NULL AS RECEIVED_DATE, NULL AS VETTING_OFFICER "
                + "\r\n" + "\t" + " FROM C_COMP_APPLICATION COM,C_COMP_APPLICANT_INFO APPINFO, "
                + "\r\n" + "\t" + " C_APPLICANT APP,C_S_SYSTEM_VALUE V,C_S_SYSTEM_VALUE V2 "
                + "\r\n" + "\t" + " WHERE COM.UUID=APPINFO.MASTER_ID "
                + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ID =APP.UUID AND APPINFO.APPLICANT_ROLE_ID=V.UUID "
                + "\r\n" + "\t" + " AND COM.APPLICATION_FORM_ID=V2.UUID "
                + whereQ
                //+ "\r\n" + "\t" + " AND APPINFO.APPLICANT_ID =APP.UUID AND APPINFO.APPLICANT_ROLE_ID=V.UUID "
                + "\r\n" + "\t" + " UNION ALL "
                + "\r\n" + "\t" + " SELECT  PRM.UUID AS PM_UUID,  PRM.MASTER_ID AS COMP_UUID, "
                + "\r\n" + "\t" + " PRM.COMPANY_APPLICANTS_ID AS COMP_APPL_UUID, 'EDIT', "
                + "\r\n" + "\t" + " COM.FILE_REFERENCE_NO, APP.SURNAME, APP.GIVEN_NAME_ON_ID, "
                + "\r\n" + "\t" + " APP.CHINESE_NAME,  COM.ENGLISH_COMPANY_NAME, "
                + "\r\n" + "\t" + " V.CODE,  V2.CODE AS APPLICATION_TYPE,  PRM.RECEIVED_DATE  AS RECEIVED_DATE, "
                + "\r\n" + "\t" + " PRM.VETTING_OFFICER AS VETTING_OFFICER "
                + "\r\n" + "\t" + " FROM C_COMP_PROCESS_MONITOR PRM, C_COMP_APPLICATION COM, "
                + "\r\n" + "\t" + " C_COMP_APPLICANT_INFO APPINFO,  C_APPLICANT APP,  C_S_SYSTEM_VALUE V,  C_S_SYSTEM_VALUE V2 "
                + "\r\n" + "\t" + " WHERE  PRM.MONITOR_TYPE ='UPM' "
                + "\r\n" + "\t" + " AND PRM.MASTER_ID = COM.UUID  AND PRM.COMPANY_APPLICANTS_ID = APPINFO.UUID "
                + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ID =APP.UUID AND APPINFO.APPLICANT_ROLE_ID=V.UUID "
                + "\r\n" + "\t" + " AND PRM.APPLICATION_FORM_ID=V2.UUID "
                + whereQ
                + "\r\n" + "\t" + " ) A "
                ;
        }
        public string SearchGCA_PM10PAndFaskTrack_q(ProcessMonitorSearchModel model, string RegType, string MonitorType)
        {
            //string whereMonitorQ = "";
            //string whereQ = "";


            //whereQ += "\r\n\t" + " AND COM.REGISTRATION_TYPE = :RegType";
            //model.QueryParameters.Add("RegType", RegType);

            //whereMonitorQ += "\r\n\t" + " AND PRM.MONITOR_TYPE = :MonitorType";
            //model.QueryParameters.Add("MonitorType", MonitorType);

            //if (!string.IsNullOrWhiteSpace(model.FileRef))
            //{
            //    whereQ += "\r\n\t" + " AND upper(COM.FILE_REFERENCE_NO) LIKE :FileRef";
            //    model.QueryParameters.Add("FileRef", "%" + model.FileRef.ToUpper() + "%");
            //}
            //if (!string.IsNullOrWhiteSpace(model.ComName))
            //{
            //    whereQ += "\r\n\t" + " AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE :ComName1 OR COM.CHINESE_COMPANY_NAME LIKE :ComName2)";
            //    model.QueryParameters.Add("ComName1", "%" + model.ComName.Trim().ToUpper() + "%");
            //    model.QueryParameters.Add("ComName2", "%" + model.ComName.Trim().ToUpper() + "%");
            //}   
            string whereQ = "";
            string whereMonitorQ = "";
            whereQ += "\r\n\t" + "AND COM.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);

            //whereMonitorQ += "\r\n\t" + "  AND PRM.MONITOR_TYPE = :abc";
            //model.QueryParameters.Add("abc", "123");

            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND upper(COM.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + " AND (upper(COM.ENGLISH_COMPANY_NAME) LIKE :ComName1 OR COM.CHINESE_COMPANY_NAME LIKE :ComName2)";
                model.QueryParameters.Add("ComName1", "%" + model.ComName.Trim().ToUpper() + "%");
                model.QueryParameters.Add("ComName2", "%" + model.ComName.Trim().ToUpper() + "%");
            }





            return ""
                + "\r\n" + "\t" + " SELECT A.PM_UUID as PM_UUID, "
                + "\r\n" + "\t" + " CASE WHEN A.PM_UUID is null THEN 'No' ELSE 'Yes' END as UM, "
                + "\r\n" + "\t" + " A.COMP_UUID as COMP_UUID,  A.RECORD_TYPE as RECORD_TYPE, A.FILE_REFERENCE_NO as FILE_REFERENCE_NO, "
                + "\r\n" + "\t" + " A.RECEIVED_DATE as RECEIVED_DATE, A.ENGLISH_COMPANY_NAME as ENGLISH_COMPANY_NAME "
                + "\r\n" + "\t" + " FROM ( "
                + "\r\n" + "\t" + " SELECT  '' AS PM_UUID, COM.UUID AS COMP_UUID, "
                + "\r\n" + "\t" + " 'NEW' AS RECORD_TYPE, COM.FILE_REFERENCE_NO, COM.ENGLISH_COMPANY_NAME, "
                + "\r\n" + "\t" + " NULL AS RECEIVED_DATE "
                + "\r\n" + "\t" + " FROM C_COMP_APPLICATION COM "
                + "\r\n" + "\t" + " WHERE 1=1 "
                + whereQ

                + "\r\n" + "\t" + " UNION ALL "
                + "\r\n" + "\t" + " SELECT  PRM.UUID AS PM_UUID,  PRM.MASTER_ID AS COMP_UUID, "
                + "\r\n" + "\t" + " 'EDIT',  COM.FILE_REFERENCE_NO, "
                + "\r\n" + "\t" + " COM.ENGLISH_COMPANY_NAME, PRM.RECEIVED_DATE AS RECEIVED_DATE "
                + "\r\n" + "\t" + " FROM C_COMP_PROCESS_MONITOR PRM, C_COMP_APPLICATION COM "
                + "\r\n" + "\t" + " WHERE 1=1 "
                + "\r\n" + "\t" + " AND PRM.MASTER_ID = COM.UUID "
                 ///!!!TO BE FIX (SQL INJECTION)
                 + "\r\n" + "\t" + "  AND PRM.MONITOR_TYPE = '" + MonitorType + "'"
                + whereQ
                 + whereMonitorQ
                + "\r\n" + "\t" + " ) A "
                ;
        }




        String SearchUAS_q = ""
        + "\r\n" + "\t" + " select ind_cert.MISS_DOCUMENT_TYPE as MissItem,ind_app.UUID as UUID,ind_app.FILE_REFERENCE_NO AS FILE_REFERENCE_NO,cat.code AS code, "
        + "\r\n" + "\t" + " concat(concat( APP.SURNAME,' '), APP.GIVEN_NAME_ON_ID) AS NAME ,app.CHINESE_NAME AS CHINAME, "
        + "\r\n" + "\t" + " ind_cert.GAZETTE_DATE AS GAZETTE_DATE,ind_cert.expiry_date AS expiry_date, "
        + "\r\n" + "\t" + " ind_cert.retention_date AS retention_date,ind_cert.RESTORE_DATE AS RESTORE_DATE, "
        + "\r\n" + "\t" + " sv.english_description AS english_description FROM C_Ind_Certificate ind_cert "
        + "\r\n" + "\t" + " inner join C_Ind_application ind_app on ind_app.uuid =ind_cert.master_id "
        + "\r\n" + "\t" + " inner join C_applicant app on app.uuid = ind_app.APPLICANT_ID "
        + "\r\n" + "\t" + " inner join C_S_CATEGORY_CODE cat on cat.uuid = ind_cert.category_id "
        + "\r\n" + "\t" + " inner join C_S_system_value sv on sv.uuid = ind_cert.application_status_id "
        + "\r\n" + "\t" + " where 1=1 ";


        String SearchUAS = ""
        + "\r\n" + "\t" + "SELECT                                                               "
        + "\r\n" + "\t" + "T1.*,T2.ENGLISH_DESCRIPTION                                          "
        + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
        + "\r\n" + "\t" + "WHERE 1=1                                                            ";

        string SearchImg_q = ""
        + "\r\n" + "\t" + " select distinct ind_cert.UUID as UUID, ind_cert.FILE_PATH_NONRESTRICTED as FILEPATH, "
        + "\r\n" + "\t" + " concat(concat( appl.SURNAME,' '), appl.GIVEN_NAME_ON_ID) AS FULLNAME, "
        + "\r\n" + "\t" + " ind_app.FILE_REFERENCE_NO AS FILEREF, "
        + "\r\n" + "\t" + " ind_cert.APPLICATION_STATUS_ID as APPSTATID, "
        + "\r\n" + "\t" + " ind_cert.CATEGORY_ID as CATCODE, "
        + "\r\n" + "\t" + " ind_cert.CERTIFICATION_NO as CERNO "
        + "\r\n" + "\t" + " from c_ind_certificate ind_cert "
        + "\r\n" + "\t" + " inner join c_ind_application ind_app on ind_cert.master_id = ind_app.uuid "
        + "\r\n" + "\t" + " inner join c_applicant appl on appl.uuid = ind_app.applicant_id "
        + "\r\n" + "\t" + " where 1=1 ";
        public InterviewResultSearchModel SearchInterviewResultForCompany(InterviewResultSearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT interviewCan.UUID as INTERVIEW_CANDIDATES_UUID "
             + "\r\n" + "\t" + ", interviewCan.START_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", to_char(interviewCan.START_DATE, 'DD/MM/YYYY HH:MI AM') as INTERVIEW_DATE_DISPLAY "
             + "\r\n" + "\t" + ", interviewCan.INTERVIEW_NUMBER as INTERVIEW_NUMBER "
             + "\r\n" + "\t" + ", (case when interviewCan.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end) as INTERVIEW_TYPE "
             + "\r\n" + "\t" + ", interviewCan.RESULT_DATE as RESULT_DATE "
             + "\r\n" + "\t" + ", (case when interviewCan.IS_ABSENT = 'N' then 'No' else '' end) as IS_ABSENT "
             + "\r\n" + "\t" + ", svResult.ENGLISH_DESCRIPTION as RESULT "
             + "\r\n" + "\t" + ", app.SURNAME as SURNAME "
             + "\r\n" + "\t" + ", app.GIVEN_NAME_ON_ID as GIVEN_NAME_ON_ID "
             + "\r\n" + "\t" + ", app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as FULL_NAME "
             + "\r\n" + "\t" + ", svRole.CODE as ROLE "
             + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES interviewCan "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE svResult on svResult.uuid = interviewCan.RESULT_ID "
             + "\r\n" + "\t" + "left join C_Comp_Applicant_Info cCompAppInfo on cCompAppInfo.CANDIDATE_NUMBER = interviewCan.CANDIDATE_NUMBER "
             + "\r\n" + "\t" + "left join C_comp_Application compApp on compApp.uuid = cCompAppInfo.MASTER_ID"
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE svRole on svRole.uuid = cCompAppInfo.APPLICANT_ROLE_ID "
             + "\r\n" + "\t" + "left join C_APPLICANT app on app.uuid = cCompAppInfo.APPLICANT_ID "
             + "\r\n" + "\t" + "left join C_interview_Schedule interviewSchedule on interviewSchedule.uuid = interviewCan.INTERVIEW_SCHEDULE_ID "
             + "\r\n" + "\t" + "left join c_meeting meeting on meeting.uuid = interviewSchedule.meeting_id "
             + "\r\n" + "\t" + "left join C_committee_Group commGrp on commGrp.uuid = meeting.COMMITTEE_GROUP_ID "
             + "\r\n" + "\t" + "left join C_S_system_value svType on svType.uuid = meeting.COMMITTEE_TYPE_ID "
             + "\r\n" + "\t" + "where 1=1 ";

            // search parameter
            String year = model.Year;
            DateTime? dateFrom = model.DateFrom;
            DateTime? dateTo = model.DateTo;
            String group = model.Group;
            String type = model.Type;
            String surnName = model.SurnName;
            String givenName = model.GivenName;
            String fileRef = model.FileRef;
            String interviewNo = model.InterviewNo;
            String HKID = model.HKID;
            String passportNo = model.PassportNo;

            if (!String.IsNullOrEmpty(year))
            {
                q += "\r\n" + "\t" + " AND TO_CHAR(interviewCan.START_DATE, 'YYYY') = :year";
                model.QueryParameters.Add("year", year);
            }
            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND interviewCan.START_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND interviewCan.START_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }
            if (!String.IsNullOrEmpty(group))
            {
                q += "\r\n" + "\t" + " AND commGrp.NAME = :grpName";
                model.QueryParameters.Add("grpName", group);
            }
            if (!String.IsNullOrEmpty(type))
            {
                q += "\r\n" + "\t" + " AND svType.uuid = :type";
                model.QueryParameters.Add("type", type);
            }
            if (!String.IsNullOrEmpty(givenName))
            {
                q += "\r\n" + "\t" + " AND app.GIVEN_NAME_ON_ID like :givenName";
                model.QueryParameters.Add("givenName", "%" + givenName + "%");
            }
            if (!String.IsNullOrEmpty(surnName))
            {
                q += "\r\n" + "\t" + " AND app.SURNAME like :surnName";
                model.QueryParameters.Add("surnName", "%" + surnName + "%");
            }
            if (!String.IsNullOrEmpty(fileRef))
            {
                q += "\r\n" + "\t" + " AND UPPER(compApp.file_Reference_No) like :fileRef";
                model.QueryParameters.Add("fileRef", "%" + fileRef.ToUpper() + "%");
            }
            if (!String.IsNullOrEmpty(interviewNo))
            {
                q += "\r\n" + "\t" + " AND interviewCan.interview_Number like :interviewNo";
                model.QueryParameters.Add("interviewNo", "%" + interviewNo + "%");
            }
            if (!String.IsNullOrEmpty(HKID))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("app.HKID") + " like :HKID";
                model.QueryParameters.Add("HKID", "%" + HKID + "%");
            }
            if (!String.IsNullOrEmpty(passportNo))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("app.PASSPORT_NO") + " like :passportNo";
                model.QueryParameters.Add("HKID", "%" + passportNo + "%");
            }

            model.Query = q;
            model.Search();

            return model;
        }
        public string ExpoortInterviewResultForCompany(InterviewResultSearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT interviewCan.UUID as INTERVIEW_CANDIDATES_UUID "
             + "\r\n" + "\t" + ", interviewCan.START_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", to_char(interviewCan.START_DATE, 'DD/MM/YYYY HH:MI AM') as INTERVIEW_DATE_DISPLAY "
             + "\r\n" + "\t" + ", interviewCan.INTERVIEW_NUMBER as INTERVIEW_NUMBER "
             + "\r\n" + "\t" + ", (case when interviewCan.INTERVIEW_TYPE = 'I' then 'Interview' else '' end) as INTERVIEW_TYPE "
             + "\r\n" + "\t" + ", interviewCan.RESULT_DATE as RESULT_DATE "
             + "\r\n" + "\t" + ", (case when interviewCan.IS_ABSENT = 'N' then 'No' else '' end) as IS_ABSENT "
             + "\r\n" + "\t" + ", svResult.ENGLISH_DESCRIPTION as RESULT "
             + "\r\n" + "\t" + ", app.SURNAME as SURNAME "
             + "\r\n" + "\t" + ", app.GIVEN_NAME_ON_ID as GIVEN_NAME_ON_ID "
             + "\r\n" + "\t" + ", app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as FULL_NAME "
             + "\r\n" + "\t" + ", svRole.CODE as ROLE "
             + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES interviewCan "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE svResult on svResult.uuid = interviewCan.RESULT_ID "
             + "\r\n" + "\t" + "left join C_Comp_Applicant_Info cCompAppInfo on cCompAppInfo.CANDIDATE_NUMBER = interviewCan.CANDIDATE_NUMBER "
             + "\r\n" + "\t" + "left join C_comp_Application compApp on compApp.uuid = cCompAppInfo.MASTER_ID"
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE svRole on svRole.uuid = cCompAppInfo.APPLICANT_ROLE_ID "
             + "\r\n" + "\t" + "left join C_APPLICANT app on app.uuid = cCompAppInfo.APPLICANT_ID "
             + "\r\n" + "\t" + "left join C_interview_Schedule interviewSchedule on interviewSchedule.uuid = interviewCan.INTERVIEW_SCHEDULE_ID "
             + "\r\n" + "\t" + "left join c_meeting meeting on meeting.uuid = interviewSchedule.meeting_id "
             + "\r\n" + "\t" + "left join C_committee_Group commGrp on commGrp.uuid = meeting.COMMITTEE_GROUP_ID "
             + "\r\n" + "\t" + "left join C_S_system_value svType on svType.uuid = meeting.COMMITTEE_TYPE_ID "
             + "\r\n" + "\t" + "where 1=1 ";

            // search parameter
            String year = model.Year;
            DateTime? dateFrom = model.DateFrom;
            DateTime? dateTo = model.DateTo;
            String group = model.Group;
            String type = model.Type;
            String surnName = model.SurnName;
            String givenName = model.GivenName;
            String fileRef = model.FileRef;
            String interviewNo = model.InterviewNo;
            String HKID = model.HKID;
            String passportNo = model.PassportNo;

            if (!String.IsNullOrEmpty(year))
            {
                q += "\r\n" + "\t" + " AND TO_CHAR(interviewCan.START_DATE, 'YYYY') = :year";
                model.QueryParameters.Add("year", year);
            }
            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND interviewCan.START_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND interviewCan.START_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }
            if (!String.IsNullOrEmpty(group))
            {
                q += "\r\n" + "\t" + " AND commGrp.NAME = :grpName";
                model.QueryParameters.Add("grpName", group);
            }
            if (!String.IsNullOrEmpty(type))
            {
                q += "\r\n" + "\t" + " AND svType.uuid = :type";
                model.QueryParameters.Add("type", type);
            }
            if (!String.IsNullOrEmpty(givenName))
            {
                q += "\r\n" + "\t" + " AND app.GIVEN_NAME_ON_ID like :givenName";
                model.QueryParameters.Add("givenName", "%" + givenName + "%");
            }
            if (!String.IsNullOrEmpty(surnName))
            {
                q += "\r\n" + "\t" + " AND app.SURNAME like :surnName";
                model.QueryParameters.Add("surnName", "%" + surnName + "%");
            }
            if (!String.IsNullOrEmpty(fileRef))
            {
                q += "\r\n" + "\t" + " AND compApp.file_Reference_No like :fileRef";
                model.QueryParameters.Add("fileRef", "%" + fileRef + "%");
            }
            if (!String.IsNullOrEmpty(interviewNo))
            {
                q += "\r\n" + "\t" + " AND interviewCan.interview_Number like :interviewNo";
                model.QueryParameters.Add("interviewNo", "%" + interviewNo + "%");
            }
            if (!String.IsNullOrEmpty(HKID))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("app.HKID") + " like :HKID";
                model.QueryParameters.Add("HKID", "%" + HKID + "%");
            }
            if (!String.IsNullOrEmpty(passportNo))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("app.PASSPORT_NO") + " like :passportNo";
                model.QueryParameters.Add("HKID", "%" + passportNo + "%");
            }

            model.Query = q;
            

            return model.Export("ExportData");
        }

        public InterviewResultSearchModel SearchInterviewResultForIndividual(InterviewResultSearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT indCan.UUID as UUID, appli.FILE_REFERENCE_NO as FILE_REFERENCE_NO "
             + "\r\n" + "\t" + ", indCan.START_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", to_char(indCan.START_DATE, 'DD/MM/YYYY HH:MI AM') as INTERVIEW_DATE_DISPLAY "
             + "\r\n" + "\t" + ", indCan.INTERVIEW_NUMBER as INTERVIEW_NUMBER "
             + "\r\n" + "\t" + ", (case when indCan.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end) as INTERVIEW_TYPE "
             + "\r\n" + "\t" + ", indCan.RESULT_DATE as RESULT_DATE "
             + "\r\n" + "\t" + ", (case when indCan.IS_ABSENT = 'N' then 'No' else '' end) as IS_ABSENT "
             + "\r\n" + "\t" + ", sv.ENGLISH_DESCRIPTION as RESULT, apnt.SURNAME, apnt.GIVEN_NAME_ON_ID "
             + "\r\n" + "\t" + ", (apnt.SURNAME ||' '||apnt.GIVEN_NAME_ON_ID) as FULL_NAME , sCode.CODE as CATEGORY_CODE, indCan.* "
             + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES indCan "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.uuid = indCan.RESULT_ID "
             + "\r\n" + "\t" + ", C_IND_CERTIFICATE indCert "
             + "\r\n" + "\t" + "left join C_INd_APPLICATION appli on appli.UUID = indCert.MASTER_ID "
             + "\r\n" + "\t" + "left join C_APPLICANT apnt on apnt.uuid = appli.APPLICANT_ID "
             + "\r\n" + "\t" + "left join C_S_CATEGORY_CODE sCode on sCode.uuid = indCert.CATEGORY_ID "
             + "\r\n" + "\t" + "where 1=1 "
             + "\r\n" + "\t" + "and indCan.CANDIDATE_NUMBER = indCert.CANDIDATE_NUMBER ";

            //// search parameter
            //String year = model.Year;
            //DateTime? dateFrom = model.DateFrom;
            //DateTime? dateTo = model.DateTo;
            //String group = model.Group;
            //String type = model.Type;
            //String surnName = model.SurnName;
            //String givenName = model.GivenName;
            //String fileRef = model.FileRef;
            //String interviewNo = model.InterviewNo;
            //String HKID = model.HKID;
            //String passportNo = model.PassportNo;

            //if (!String.IsNullOrEmpty(year))
            //{
            //    q += "\r\n" + "\t" + " AND TO_CHAR(indCan.START_DATE, 'YYYY') = :year";
            //    model.QueryParameters.Add("year", year);
            //}
            //if (dateFrom != null)
            //{
            //    q += "\r\n" + "\t" + " AND indCan.START_DATE >= :dateFrom";
            //    model.QueryParameters.Add("dateFrom", dateFrom);
            //}
            //if (dateTo != null)
            //{
            //    q += "\r\n" + "\t" + " AND indCan.START_DATE <= :dateTo";
            //    model.QueryParameters.Add("dateTo", dateTo);
            //}
            ////if (!String.IsNullOrEmpty(group))
            ////{
            ////    q += "\r\n" + "\t" + " AND commGrp.NAME = :grpName";
            ////    model.QueryParameters.Add("grpName", group);
            ////}
            //if (!String.IsNullOrEmpty(type))
            //{
            //    q += "\r\n" + "\t" + " AND sv.uuid = :type";
            //    model.QueryParameters.Add("type", type);
            //}
            //if (!String.IsNullOrEmpty(givenName))
            //{
            //    q += "\r\n" + "\t" + " AND apnt.GIVEN_NAME_ON_ID like :givenName";
            //    model.QueryParameters.Add("givenName", "%" + givenName + "%");
            //}
            //if (!String.IsNullOrEmpty(surnName))
            //{
            //    q += "\r\n" + "\t" + " AND apnt.SURNAME like :surnName";
            //    model.QueryParameters.Add("surnName", "%" + surnName + "%");
            //}
            //if (!String.IsNullOrEmpty(fileRef))
            //{
            //    q += "\r\n" + "\t" + " AND appli.file_Reference_No like :fileRef";
            //    model.QueryParameters.Add("fileRef", "%" + fileRef + "%");
            //}
            //if (!String.IsNullOrEmpty(interviewNo))
            //{
            //    q += "\r\n" + "\t" + " AND indCan.interview_Number like :interviewNo";
            //    model.QueryParameters.Add("interviewNo", "%" + interviewNo + "%");
            //}
            //if (!String.IsNullOrEmpty(HKID))
            //{
            //    q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.HKID") + " like :HKID";
            //    model.QueryParameters.Add("HKID", "%" + HKID + "%");
            //}
            //if (!String.IsNullOrEmpty(passportNo))
            //{
            //    q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.PASSPORT_NO") + " like :passportNo";
            //    model.QueryParameters.Add("HKID", "%" + passportNo + "%");
            //}

            model.Query = q;
            model.Search();

            return model;
        }

        //MWCA-IC
        public InterviewCandidatesSearchModel SearchInterviewCandidatesForCompany(InterviewCandidatesSearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT t1.UUID as UUID "
             + "\r\n" + "\t" + ", t1.MEETING_NUMBER as MEETING_NUMBER "
             + "\r\n" + "\t" + ", t1.INTERVIEW_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", t5.CODE as CODE "
             + "\r\n" + "\t" + ", t4.ROOM as ROOM "
             + "\r\n" + "\t" + ", t1.IS_CANCEL as IS_CANCEL "
             + "\r\n" + "\t" + ", t2.YEAR as YEAR "
             + "\r\n" + "\t" + ", t2.MEETING_GROUP as MEETING_GROUP "
             + "\r\n" + "\t" + ", t3.UUID as searchType "
             + "\r\n" + "\t" + " From C_INTERVIEW_SCHEDULE t1 "
             + "\r\n" + "\t" + "left join C_MEETING t2 on t2.uuid = t1.MEETING_ID "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE t3 on t3.UUID = t2.COMMITTEE_TYPE_ID "
             + "\r\n" + "\t" + "left join C_S_ROOM t4 on t4.uuid = t1.ROOM_ID "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE t5 on t5.UUID = t1.TIME_SESSION_ID "
             + "\r\n" + "\t" + "where t3.code = 'MWCRC' ";
            //+ "\r\n" + "\t" + "ORDER BY INTERVIEW_DATE "

            // search parameter
            String year = model.Year;
            DateTime? interviewDate = model.InterviewDate;
            String group = model.Group;
            String type = model.Type;


            if (!String.IsNullOrEmpty(year))
            {
                q += "\r\n" + "\t" + " AND t2.YEAR >= :year";
                model.QueryParameters.Add("year", year);
            }
            if (interviewDate != null)
            {
                q += "\r\n" + "\t" + " AND t1.INTERVIEW_DATE >= :interviewDate";
                model.QueryParameters.Add("interviewDate", interviewDate);

            }

            if (!String.IsNullOrEmpty(group))
            {
                q += "\r\n" + "\t" + " AND t2.MEETING_GROUP = :grpName";
                model.QueryParameters.Add("grpName", group);
            }
            if (!String.IsNullOrEmpty(type))
            {
                q += "\r\n" + "\t" + " AND t3.UUID = :type";
                model.QueryParameters.Add("type", type);
            }


            model.Query = q;
            model.Search();

            return model;
        }

        //MWCA-IC
        public Fn02GCA_GCNSearchModel SearchCompApplicationByFileRefAndCompNameA(Fn02GCA_GCNSearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT ca.UUID as UUID "
             + "\r\n" + "\t" + ", ca.FILE_REFERENCE_NO as FILE_REFERENCE_NO "
             + "\r\n" + "\t" + ", ca.ENGLISH_COMPANY_NAME as ENGLISH_COMPANY_NAME "
             + "\r\n" + "\t" + ", sv.ENGLISH_DESCRIPTION as ENGLISH_DESCRIPTION "
             + "\r\n" + "\t" + ", ca.EXPIRY_DATE as EXPIRY_DATE "
             + "\r\n" + "\t" + " From C_COMP_APPLICATION ca "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.uuid = ca.APPLICATION_STATUS_ID "
             + "\r\n" + "\t" + "where ca.registration_type ='CGC' ";
            //+ "\r\n" + "\t" + "ORDER BY INTERVIEW_DATE "

            // search parameter
            String fileRef = model.FileRef;
            String comName = model.ComName;


            if (!String.IsNullOrEmpty(fileRef))
            {
                q += "\r\n" + "\t" + " AND ca.FILE_REFERENCE_NO like :fileRef";
                model.QueryParameters.Add("fileRef", "%" + fileRef + "%");
            }

            if (!String.IsNullOrEmpty(comName))
            {

                q += "\r\n" + "\t" + " AND ca.ENGLISH_COMPANY_NAME like :comName";
                model.QueryParameters.Add("comName", "%" + comName + "%");

            }

            //q += "\r\n" + "\t" + " ORDER BY ca.FILE_REFERENCE_NO ";

            model.Query = q;
            model.Search();

            return model;
        }
        public LeaveFormDisplayModel ViewCompLF(string compAppInfoMasterId, string compAppInfoUUID, string RegType, string filePath)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_COMP_APPLICANT_INFO compAppInfo = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == compAppInfoUUID).FirstOrDefault();

                C_COMP_APPLICATION compApp = (from t1 in db.C_COMP_APPLICATION
                                              join t2 in db.C_COMP_APPLICANT_INFO on t1.UUID equals t2.MASTER_ID
                                              where t2.UUID == compAppInfoUUID
                                              select t1).FirstOrDefault();
                C_APPLICANT applicant = (from app in db.C_APPLICANT
                                         join compA in db.C_COMP_APPLICANT_INFO on app.UUID equals compA.APPLICANT_ID
                                         where compA.UUID == compAppInfo.UUID
                                         select app).FirstOrDefault();

                string Role = (from sv1 in db.C_S_SYSTEM_VALUE
                               join ca2 in db.C_COMP_APPLICANT_INFO on sv1.UUID equals ca2.APPLICANT_ROLE_ID
                               where sv1.UUID == compAppInfo.APPLICANT_ROLE_ID
                               select sv1.CODE).FirstOrDefault();

                List<C_LEAVE_FORM> C_LEAVE_FORM = db.C_LEAVE_FORM.Where(o => o.COMPANY_APPLICANT_ID == compAppInfoUUID).ToList();

                C_LEAVE_FORM C_LEAVE_FORM_PATH = db.C_LEAVE_FORM.Where(o => o.COMPANY_APPLICANT_ID == compAppInfoUUID).FirstOrDefault();

                LeaveFormDisplayModel model = new LeaveFormDisplayModel()
                  {
                    LeaveFormList = C_LEAVE_FORM
                    ,
                    RegType = RegType
                    ,
                    C_COMP_APPLICANT_INFO = compAppInfo
                    ,
                    C_COMP_APPLICATION = compApp
                    ,
                    C_APPLICANT = applicant
                    ,
                    ASName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                    ,
                    RoleType = Role
                };
                return model;
            }
        }
        public LeaveFormDisplayModel ViewIndLF(string indAppUUID, string indCertUUID, string RegType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION IndApp = db.C_IND_APPLICATION.Where(o => o.UUID == indAppUUID).FirstOrDefault();

                C_IND_CERTIFICATE IndCert = db.C_IND_CERTIFICATE.Where(o => o.UUID == indCertUUID).FirstOrDefault();

                C_APPLICANT applicant = (from t1 in db.C_APPLICANT
                                         join t2 in db.C_IND_APPLICATION on t1.UUID equals t2.APPLICANT_ID
                                         where t2.UUID == IndApp.UUID
                                         select t1).FirstOrDefault();

                List<C_LEAVE_FORM> C_LEAVE_FORM = db.C_LEAVE_FORM.Where(o => o.MASTER_ID == indAppUUID).ToList();

                C_LEAVE_FORM C_LEAVE_FORM_PATH = db.C_LEAVE_FORM.Where(o => o.MASTER_ID == indAppUUID).FirstOrDefault();

                return new LeaveFormDisplayModel()
                {
                    LeaveFormList = C_LEAVE_FORM
                    ,
                    RegType = RegType
                    ,
                    C_IND_APPLICATION = IndApp
                    ,
                    C_IND_CERTIFICATE = IndCert
                    ,
                    C_APPLICANT = applicant
                    ,
                    ASName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                    ,
                    ChiName = applicant.CHINESE_NAME
                };
            }
        }
        public LeaveFormSearchModel SearchComp_LF(LeaveFormSearchModel model, string RegType)
        {
            if (model.StartDate == null && model.EndDate == null)
            {
                model.Query = SearchNoDayCompLeaveFormRecord_q;
                model.QueryWhere = SearchCompLF_whereQ(model, RegType);
                model.Search();
                return model;
            }
            else
            {
                model.Query = SearchCompLeaveFormRecord_q;
                model.QueryWhere = SearchCompLF_whereQ(model, RegType);
                model.Search();
                return model;
            }
        }
        public LeaveFormSearchModel SearchInd_LF(LeaveFormSearchModel model, string RegType)
        {
            if (model.StartDate == null && model.EndDate == null)
            {
                model.Query = SearchNoDayIndLeaveFormRecord_q;
                model.QueryWhere = SearchIndLF_whereQ(model, RegType);
                model.Search();
                return model;
            }
            else
            {
                model.Query = SearchIndLeaveFormRecord_q;
                model.QueryWhere = SearchIndLF_whereQ(model, RegType);
                model.Search();
                return model;
            }
        }

        public string ExportInd_LF(LeaveFormSearchModel model, string RegType)
        {
            if (model.StartDate == null && model.EndDate == null)
            {
                model.Query = SearchNoDayIndLeaveFormRecord_q;
                model.QueryWhere = SearchIndLF_whereQ(model, RegType);
                return model.Export("ExportData");
            }
            else
            {
                model.Query = SearchIndLeaveFormRecord_q;
                model.QueryWhere = SearchIndLF_whereQ(model, RegType);
                return model.Export("ExportData");
            }
        }
        public string ExportComp_LF(LeaveFormSearchModel model, string RegType)
        {
            if (model.StartDate == null && model.EndDate == null)
            {
                model.Query = SearchNoDayCompLeaveFormRecord_q;
                model.QueryWhere = SearchCompLF_whereQ(model, RegType);
                model.Search();
                return model.Export("ExportData");
            }
            else
            {
                model.Query = SearchCompLeaveFormRecord_q;
                model.QueryWhere = SearchCompLF_whereQ(model, RegType);
                model.Search();
                return model.Export("ExportData");
            }
        }

        public string SearchCompLF_whereQ(LeaveFormSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND compApp.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + " AND (upper(compApp.CERTIFICATION_NO) LIKE :RegNo)";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.SURNAME) LIKE :SurName)";
                model.QueryParameters.Add("SurName", "%" + model.SurName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.CHINESE_NAME) LIKE :ChiName)";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CompName))
            {
                whereQ += "\r\n\t" + " AND (upper(compApp.ENGLISH_COMPANY_NAME) LIKE :ComName OR compApp.CHINESE_COMPANY_NAME LIKE :ComName)";
                model.QueryParameters.Add("ComName", "%" + model.CompName.Trim().ToUpper() + "%");
            }
            if (model.StartDate!=null)
            {
                whereQ += "\r\n\t" + " AND leaveForm.LEAVE_START_DATE = :StartDate";
                model.QueryParameters.Add("StartDate", model.StartDate);
            }
            if (model.EndDate!=null)
            {
                whereQ += "\r\n\t" + " AND leaveForm.LEAVE_END_DATE = :EndDate";
                model.QueryParameters.Add("EndDate", model.EndDate);
            }
            return whereQ;
        }
        public string SearchIndLF_whereQ(LeaveFormSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND indApp.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + " AND (upper(indCert.CERTIFICATION_NO) LIKE :RegNo)";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.SURNAME) LIKE :SurName)";
                model.QueryParameters.Add("SurName", "%" + model.SurName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + " AND (upper(appl.CHINESE_NAME) LIKE :ChiName)";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CompName))
            {
                whereQ += "\r\n\t" + " AND (upper(indApp.ENGLISH_COMPANY_NAME) LIKE :ComName OR indApp.CHINESE_COMPANY_NAME LIKE:ComName)";
                model.QueryParameters.Add("ComName", "%" + model.CompName.Trim().ToUpper() + "%");
            }
            if (model.StartDate!=null)
            {
                whereQ += "\r\n\t" + " AND leaveForm.LEAVE_START_DATE =:StartDate";
                model.QueryParameters.Add("StartDate", model.StartDate);
            }
            if (model.EndDate!=null)
            {
                whereQ += "\r\n\t" + " AND leaveForm.LEAVE_END_DATE =:EndDate";
                model.QueryParameters.Add("EndDate", model.EndDate);
            }
            return whereQ;
        }

        public ProcessMonitorSearchModel SearchGCAandMWCA_PM(ProcessMonitorSearchModel model, string RegType)
        {
            model.Query = SearchGCAandMWCA_PM_q(model, RegType);
            model.Search();
            return model;
        }
        public string ExportGCAandMWCA_PM(ProcessMonitorSearchModel model, string RegType)
        {
            if (RegType=="GCA")
            {
                model.Query = SearchGCAandMWCA_PM_q(model, RegType);
                return model.Export("ExportData");
            }
            else
            {
                model.Query = SearchGCAandMWCA_PM_q(model, RegType);
                return model.Export("ExportData");
            }
        }
        public ProcessMonitorSearchModel SearchGCA_PM10P(ProcessMonitorSearchModel model, string RegType, string MonitorType)
        {
            model.Query = SearchGCA_PM10PAndFaskTrack_q(model, RegType, MonitorType);
            model.Search();
            return model;
        }
        public string ExportGCA_PM10P_PM(ProcessMonitorSearchModel model, string RegType, string MonitorType)
        {
            model.Query = SearchGCA_PM10PAndFaskTrack_q(model, RegType, MonitorType);
            return model.Export("ExportData");
        }
        public ProcessMonitorSearchModel SearchPAandMWIA_PM(ProcessMonitorSearchModel model, string RegType)
        {
            model.Query = SearchPAandMWIA_PM_q(model, RegType);
            model.Search();
            return model;
        }
        public string ExportPAandMWIA_PM(ProcessMonitorSearchModel model, string RegType)
        {
            model.Query = SearchPAandMWIA_PM_q(model, RegType);
            return model.Export("ExportData");
        }
        //FN02_MRA
        public Fn02GCA_MRASearchModel ViewReservedRoom(Fn02GCA_MRASearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT t1.UUID as UUID "
             + "\r\n" + "\t" + ", t2.ROOM as ROOM "
             + "\r\n" + "\t" + ", t1.MEETING_NUMBER as MEETING_NUMBER "
             + "\r\n" + "\t" + ", t1.INTERVIEW_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", t3.CODE as CODE "
             + "\r\n" + "\t" + ", t1.IS_CANCEL as IS_CANCEL "
             + "\r\n" + "\t" + " From C_INTERVIEW_SCHEDULE t1 "
             + "\r\n" + "\t" + "left join C_S_ROOM t2 on t2.uuid = t1.ROOM_ID "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE t3 on t3.uuid = t1.TIME_SESSION_ID "
             + "\r\n" + "\t" + "where 1=1 ";



            // search parameter
            DateTime? dateFrom = model.DateFrom;
            DateTime? dateTo = model.DateTo;
            String room = model.Room;

            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND t1.INTERVIEW_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND t1.INTERVIEW_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }

            if (!String.IsNullOrEmpty(room))
            {
                q += "\r\n" + "\t" + " AND t2.ROOM = :room";
                model.QueryParameters.Add("room", room);
            }

            //q += "\r\n" + "\t" + " ORDER BY ca.FILE_REFERENCE_NO ";

            model.Query = q;
            model.Search();

            return model;
        }
        public Fn02GCA_MRASearchModel ExportReservedRoom(Fn02GCA_MRASearchModel model)
        {
            String q = ""
             + "\r\n" + "\t" + "SELECT t1.UUID as UUID "
             + "\r\n" + "\t" + ", t2.ROOM as ROOM "
             + "\r\n" + "\t" + ", t1.MEETING_NUMBER as MEETING_NUMBER "
             + "\r\n" + "\t" + ", t1.INTERVIEW_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", t3.CODE as CODE "
             + "\r\n" + "\t" + ", t1.IS_CANCEL as IS_CANCEL "
             + "\r\n" + "\t" + " From C_INTERVIEW_SCHEDULE t1 "
             + "\r\n" + "\t" + "left join C_S_ROOM t2 on t2.uuid = t1.ROOM_ID "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE t3 on t3.uuid = t1.TIME_SESSION_ID "
             + "\r\n" + "\t" + "where 1=1 ";



            // search parameter
            DateTime? dateFrom = model.DateFrom;
            DateTime? dateTo = model.DateTo;
            String room = model.Room;

            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND t1.INTERVIEW_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND t1.INTERVIEW_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }

            if (!String.IsNullOrEmpty(room))
            {
                q += "\r\n" + "\t" + " AND t2.ROOM = :room";
                model.QueryParameters.Add("room", room);
            }

            //q += "\r\n" + "\t" + " ORDER BY ca.FILE_REFERENCE_NO ";

            model.Query = q;
            //  model.Search();
            model.Export("ExportFile");
            return model;
        }

        public Fn02GCA_MRASearchModel ViewAvailableRoom(Fn02GCA_MRASearchModel model)
        {
            //List<C_INTERVIEW_SCHEDULE> cisList = new List<C_INTERVIEW_SCHEDULE>();


            //List<C_S_ROOM> roomList =SystemListUtil.GetRoomLists();
            //List<C_S_SYSTEM_VALUE> TimeSession = SystemListUtil.GetSVListByType(
            //                    RegistrationConstant.SYSTEM_TYPE_TIME_SESSION
            //                   );
            //int tempInt = 0;

            //for (int r = 0; r < roomList.Count(); r++)
            //{

            //    for (var dt = model.DateFrom.Value.Date; dt.Date <= model.DateTo.Value.Date; dt = dt.AddDays(1))
            //    {

            //        for (int ts = 0; ts < TimeSession.Count(); ts++)
            //        {

            //            C_INTERVIEW_SCHEDULE cis = new C_INTERVIEW_SCHEDULE();
            //            cis.UUID = tempInt.ToString();
            //            cis.ROOM_ID = roomList[r].UUID;
            //            cis.TIME_SESSION_ID = TimeSession[ts].UUID;

            //            cis.INTERVIEW_DATE = dt;
            //            cisList.Add(cis);
            //            tempInt++;
            //            if (tempInt != 0 && tempInt % 10 == 0)
            //            {

            //                break;
            //            }
            //        }

            //    }

            //}

            //List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
            //for (int i = 0; i < cisList.Count; i++)
            //{
            //    data.Add(new Dictionary<string, object>() {
            //        { "V1" ,SystemListUtil.GetRoomByUUID(cisList[i].ROOM_ID).ROOM }
            //        , { "V2" ,cisList[i].INTERVIEW_DATE }
            //        , { "V3" ,SystemListUtil.GetSVByUUID(cisList[i].TIME_SESSION_ID).CODE }
            //        ,  { "V4" ,cisList[i].UUID }
            //        });

            //}
            //model.Data = data;

            //model.Total = model.Data.Count;

            String q = ""
          + "\r\n" + "\t" + "  SELECT "  
          + "\r\n" + "\t" + "  T1.ROOM AS V1 "
          + "\r\n" + "\t" + "  , DATE_BASE.D AS V2"
          + "\r\n" + "\t" + " , SYSV.CODE AS V3"
              + "\r\n" + "\t" + " , T1.UUID  AS V4"
            + "\r\n" + "\t" + " , SYSV.UUID AS V5"

          + "\r\n" + "\t" + " FROM"
        + "\r\n" + "\t" + "  (  SELECT TO_DATE('" + model.DateFrom.ToString().Replace("/", "-") + "','dd-MM-yyyy') + ROWNUM -1 AS D"

          //+ "\r\n" + "\t" + "  (  SELECT :dateFrom + ROWNUM -1 AS D"
          + "\r\n" + "\t" + "  FROM ALL_OBJECTS "
          // + "\r\n" + "\t" + " WHERE ROWNUM <=  ((:dateTo) - (:dateFrom ) ) +1"
          + "\r\n" + "\t" + " WHERE ROWNUM <= TO_DATE('" + model.DateTo.ToString().Replace("/", "-") + "','dd-MM-yyyy') - TO_DATE('" + model.DateFrom.ToString().Replace("/", "-") + "','dd-MM-yyyy') +1"
          + "\r\n" + "\t" + "  ) DATE_BASE"
          + "\r\n" + "\t" + "    LEFT JOIN C_S_ROOM T1 ON 1=1"
          + "\r\n" + "\t" + " LEFT JOIN ("
          + "\r\n" + "\t" + " SELECT J1.* FROM C_S_SYSTEM_VALUE J1, C_S_SYSTEM_TYPE J2 WHERE J1.SYSTEM_TYPE_ID = J2.UUID AND J2.TYPE = 'TIME_SESSION'"
          + "\r\n" + "\t" + "    ) SYSV ON 1=1"
            + "\r\n" + "\t" + " where 1=1"
          + "\r\n" + "\t" + " And not exists (  select 1 from  C_INTERVIEW_SCHEDULE jj1 where t1.uuid = jj1.ROOM_ID  and jj1.TIME_SESSION_ID =SYSV.uuid and jj1.INTERVIEW_DATE= DATE_BASE.D  )";



            // search parameter
            DateTime? dateFrom = model.DateFrom;
            DateTime? dateTo = model.DateTo;
            String room = model.Room;

            //if (dateFrom != null)
            //{
            //    ///q += "\r\n" + "\t" + "  :dateFrom";
            //    model.QueryParameters.Add("dateFrom", dateFrom);
            //}
            //if (dateTo != null)
            //{
            //   // q += "\r\n" + "\t" + " :dateTo";
            //    model.QueryParameters.Add("dateTo", dateTo);
            //}

            if (!String.IsNullOrEmpty(room))
            {
                q += "\r\n" + "\t" + " AND  T1.ROOM = :room";
                model.QueryParameters.Add("room", room);
            }

            //q += "\r\n" + "\t" + " ORDER BY ca.FILE_REFERENCE_NO ";

            model.Query = q;
            model.Search();


            return model;
        }
        // common function to Export Cert/Letter/QpCode
        /*
        public String exportCompUpdateAppStatus(String exportType, String process, String certificateNo, String compAppUuid, String rptIssueDt, String rptRcvdDt, String signature
            , String missingItemNullChk, String missingItemChequeChk, String missingItemProfRegCertChk
            , String missingItemOthersChk, String missingItemIncompleteFormChk)
        {
            String certificateContent = "";
            if (RegistrationConstant.CERT.Equals(exportType))
            {
                certificateContent = populateCompExportCert(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            else if (RegistrationConstant.LETTER.Equals(exportType))
            {
                certificateContent = populateCompExportLetter(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            else if (RegistrationConstant.LETTER_WITH_CODE.Equals(exportType))
            {
                certificateContent = populateCompExportLetterWithCode(certificateNo, process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            return certificateContent;
        }
        */

        public String populateProfExportCert(String process, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;

            String catCode = "";

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            if (indCertificate != null)
            {
                //define header
                List<string> certHeaderList = null;

                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                    C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (catGrp != null) { catCode = catGrp.CODE; }
                }

                if (catCode != null)
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        certHeaderList = new List<string>()
                        {
                            "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref", "sn","itemline1","itemline2","itemline3","itemline4","itemline5","itemline6"
                        };

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                    else
                    {
                        certHeaderList = new List<string>()
                        {
                            "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref"
                        };

                        if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                        {
                            certHeaderList = new List<string>()
                            {
                                "file_ref","reg_no",
                                "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                                "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                                "addr1","addr2","addr3","addr4","addr5",
                                "caddr1","caddr2","caddr3","caddr4","caddr5",
                                "c_o",
                                "tel_no1","fax_no1",
                                "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                                "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                                "issue_dt","cert_cname","letter_file_ref", "prb_cat_code_qualification", "sn"
                            };
                        }

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                }
            }

            if (indCertificate != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
                if (indApplication != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));

                    applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
                    if (applicant != null)
                    {
                        String decryptHKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID);
                        if (applicant.TITLE_ID != null)
                        {
                            // get sv Title
                            C_S_SYSTEM_VALUE svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
                            if (svTitle != null)
                            {
                                content = appendCertContent(content, appendDoubleQuote(svTitle.ENGLISH_DESCRIPTION));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(applicant.SURNAME != null ? applicant.SURNAME.ToUpper() : ""));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(decryptHKID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GENDER));

                        if (applicant.GENDER != null)
                        {
                            if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));

                        if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXTENDED_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                        else
                        {
                            if (RegistrationConstant.NEW_REG.Equals(intProcess))
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                        }

                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));
                    }

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }

                    C_S_CATEGORY_CODE sCatCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                }

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }

                if (!(RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)))
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)");
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)");
                    }
                    else if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));

                        String prbQuali = "";
                        List<C_IND_QUALIFICATION> indQualList = getIndQualificationListByMasterId(indApplication.UUID);
                        if (indQualList != null)
                        {
                            for (int i = 0; i < indQualList.Count(); i++)
                            {
                                C_IND_QUALIFICATION indQual = indQualList.ElementAt(i);

                                if (i != 0) { prbQuali += ", "; }

                                C_S_SYSTEM_VALUE svPrb = getSSystemValueByUuid(indQual.PRB_ID);
                                if (svPrb != null)
                                {
                                    prbQuali += svPrb.CODE;
                                    prbQuali += RegistrationConstant.COLON;
                                }

                                C_S_CATEGORY_CODE sCatCodeTmp = getSCategoryCodeByUuid(indQual.CATEGORY_ID);
                                if (sCatCodeTmp != null)
                                {
                                    prbQuali += sCatCodeTmp.CODE;
                                }

                                // get qualification details
                                List<C_IND_QUALIFICATION_DETAIL> indQualDetailsList = getIndQualificationDetailsListByIndQualUuid(indQual.UUID);

                                for (int k = 0; k < indQualDetailsList.Count(); k++)
                                {
                                    C_IND_QUALIFICATION_DETAIL indQualDetail = indQualDetailsList.ElementAt(k);
                                    if (k == 0)
                                    {
                                        prbQuali += RegistrationConstant.ARROW;
                                    }
                                    else
                                    {
                                        prbQuali += "/";
                                    }

                                    C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = getSCategoryCodeDetaiByUuid(indQualDetail.S_CATEGORY_CODE_DETAIL_ID);
                                    prbQuali += sCategoryCodeDetail != null ? sCategoryCodeDetail.ENGLISH_DESCRIPTION : "";
                                }
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(prbQuali.Trim()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
                else
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode)) {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode)) {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
            }

            // Generate new sequence number;
            if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
            }

            if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode)) {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content += SEPARATOR + appendDoubleQuote(newSequenceNo.ToString());
            }

            // get list of mwItems
            String tabChar = "\t";
            String mwItemsStr = "";
            List<C_S_SYSTEM_VALUE> mwItems = getIndApplicationMwItemByIndAppUuid(indApplication.UUID);
            for (int i = 0; i < mwItems.Count(); i++)
            {
                if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE + mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE + SEPARATOR;
                }
                if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE;
                }
                if (i == (mwItems.Count() - 1))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE;
                }
            }

            if (mwItems.Count() <= 35)
            {
                mwItemsStr += SEPARATOR;
                //add blank items
                int groups = (mwItems.Count()) / 7;
                int lastGroupSize = (mwItems.Count()) % 7;
                if (groups == 5)
                {
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
                else if (groups < 5)
                {
                    if (lastGroupSize == 0)
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                    }
                    int emptyGroups = 1;
                    while (emptyGroups < (5 - groups))
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        emptyGroups++;
                    }
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
            }
            content += mwItemsStr;

            return content;
        }

        // 
        public String populateProfExportQPCard(String process, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;

            String catCode = "";

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            if (indCertificate != null)
            {
                //define header
                List<string> certHeaderList = null;

                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                    C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (catGrp != null) { catCode = catGrp.CODE; }
                }

                if (catCode != null)
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        certHeaderList = new List<string>()
                        { "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                            "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                            "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref", "sn","itemline1","itemline2","itemline3","itemline4","itemline5","itemline6"
                        };

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                    else
                    {
                        certHeaderList = new List<string>()
                        { "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                            "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                            "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref"
                        };

                        if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                        {
                            certHeaderList = new List<string>()
                            {   "file_ref","reg_no",
                                "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                                "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                                "addr1","addr2","addr3","addr4","addr5",
                                "caddr1","caddr2","caddr3","caddr4","caddr5",
                                "c_o",
                                "tel_no1","fax_no1",
                                "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                                "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                                "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                                "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                                "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                                "issue_dt","cert_cname","letter_file_ref", "prb_cat_code_qualification", "sn"
                            };
                        }

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                }
            }

            if (indCertificate != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
                if (indApplication != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));

                    applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
                    if (applicant != null)
                    {
                        String decryptHKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID);
                        if (applicant.TITLE_ID != null)
                        {
                            // get sv Title
                            C_S_SYSTEM_VALUE svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
                            if (svTitle != null)
                            {
                                content = appendCertContent(content, appendDoubleQuote(svTitle.ENGLISH_DESCRIPTION));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(applicant.SURNAME != null ? applicant.SURNAME.ToUpper() : ""));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(decryptHKID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GENDER));

                        if (applicant.GENDER != null)
                        {
                            if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));

                        if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXTENDED_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                        else
                        {
                            if (RegistrationConstant.NEW_REG.Equals(intProcess))
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                        }

                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));
                    }

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }

                    C_S_CATEGORY_CODE sCatCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                }

                // Qp Card information


                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }

                if (!(RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)))
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)");
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)");
                    }
                    else if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));

                        String prbQuali = "";
                        List<C_IND_QUALIFICATION> indQualList = getIndQualificationListByMasterId(indApplication.UUID);
                        if (indQualList != null)
                        {
                            for (int i = 0; i < indQualList.Count(); i++)
                            {
                                C_IND_QUALIFICATION indQual = indQualList.ElementAt(i);

                                if (i != 0) { prbQuali += ", "; }

                                C_S_SYSTEM_VALUE svPrb = getSSystemValueByUuid(indQual.PRB_ID);
                                if (svPrb != null)
                                {
                                    prbQuali += svPrb.CODE;
                                    prbQuali += RegistrationConstant.COLON;
                                }

                                C_S_CATEGORY_CODE sCatCodeTmp = getSCategoryCodeByUuid(indQual.CATEGORY_ID);
                                if (sCatCodeTmp != null)
                                {
                                    prbQuali += sCatCodeTmp.CODE;
                                }

                                // get qualification details
                                List<C_IND_QUALIFICATION_DETAIL> indQualDetailsList = getIndQualificationDetailsListByIndQualUuid(indQual.UUID);

                                for (int k = 0; k < indQualDetailsList.Count(); k++)
                                {
                                    C_IND_QUALIFICATION_DETAIL indQualDetail = indQualDetailsList.ElementAt(k);
                                    if (k == 0)
                                    {
                                        prbQuali += RegistrationConstant.ARROW;
                                    }
                                    else
                                    {
                                        prbQuali += "/";
                                    }

                                    C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = getSCategoryCodeDetaiByUuid(indQualDetail.S_CATEGORY_CODE_DETAIL_ID);
                                    prbQuali += sCategoryCodeDetail != null ? sCategoryCodeDetail.ENGLISH_DESCRIPTION : "";
                                }
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(prbQuali.Trim()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
                else
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
            }

            // Generate new sequence number;
            if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
            }

            if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content += SEPARATOR + appendDoubleQuote(newSequenceNo.ToString());
            }

            // get list of mwItems
            String tabChar = "\t";
            String mwItemsStr = "";
            List<C_S_SYSTEM_VALUE> mwItems = getIndApplicationMwItemByIndAppUuid(indApplication.UUID);
            for (int i = 0; i < mwItems.Count(); i++)
            {
                if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE + mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE + SEPARATOR;
                }
                if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE;
                }
                if (i == (mwItems.Count() - 1))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE;
                }
            }

            if (mwItems.Count() <= 35)
            {
                mwItemsStr += SEPARATOR;
                //add blank items
                int groups = (mwItems.Count()) / 7;
                int lastGroupSize = (mwItems.Count()) % 7;
                if (groups == 5)
                {
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
                else if (groups < 5)
                {
                    if (lastGroupSize == 0)
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                    }
                    int emptyGroups = 1;
                    while (emptyGroups < (5 - groups))
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        emptyGroups++;
                    }
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
            }
            content += mwItemsStr;

            return content;
        }

        public String populateCompExportCert(String process, String compAppUuid, String issueDt, String lastDocRecDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";

            // certificate common colunn header
            List<string> certHeaderList1 = new List<string>()
            {
            "file_ref", "reg_no", "br_no", "ename", "cname",
            "cat_code", "cat_grp_code", "cat_grp_desc",
            "reg_dt", "gazet_dt", "expiry_dt",
            "restore_app_dt", "restore_dt",
            "retent_app_dt",
            "removal_dt",
            "approval_dt",
            "addr1", "addr2", "addr3",
            "addr4", "addr5",
            "caddr1", "caddr2", "caddr3",
            "caddr4", "caddr5",
            "tel_no1",  "fax_no1",
            "auth_name", "auth_cname",
            "auth_rank", "auth_crank",
            "acting", "auth_tel",
            "auth_fax",
            "sub_reg", "sub_creg", "issue_dt", "doc_received_dt", "as_td",
            "cert_cname", "sn",
            "typeline1", "classline1", "typeline2", "classline2",
            "typeline3", "classline3", "typeline4", "classline4",
            "typeline5", "classline5", "typeline6", "classline6",
            "typeline7", "classline7"
            };

            List<string> certHeaderList2 = new List<string>()
            {
            "file_ref", "reg_no", "br_no", "ename", "cname",
            "cat_code", "cat_grp_code", "cat_grp_desc",
            "reg_dt",
            "gazet_dt",
            "expiry_dt",
            "restore_app_dt",
            "restore_dt",
            "retent_app_dt",
            "removal_dt",
            "approval_dt",
            "addr1", "addr2", "addr3",
            "addr4", "addr5",
            "caddr1", "caddr2", "caddr3",
            "caddr4", "caddr5",
            "tel_no1",  "fax_no1",
            "auth_name", "auth_cname",
            "auth_rank", "auth_crank",
            "acting", "auth_tel",
            "auth_fax",
            "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
            "as_td",
            "cert_cname"
            };

            // by default
            List<string> certHeaderList = certHeaderList1;

            // get compApp by uuid
            C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

            if (compApp != null)
            {
                // get sCatCode
                C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if ("MWC".Equals(catCodeStr) || "MWI".Equals(catCodeStr) || "MWC(P)".Equals(catCodeStr))
                    {
                        certHeaderList = certHeaderList1;
                    }
                    else
                    {
                        certHeaderList = certHeaderList2;
                    }
                }

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME.ToUpper()));
                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                content = appendCertContent(content, sCatCode != null ? appendDoubleQuote(sCatCode.CODE) : "");

                // get svCatGrp
                C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE.ToString()));

                if (RegistrationConstant.RETENTION == intProcess || RegistrationConstant.RESTORATION == intProcess)
                {
                    content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE.ToString()));
                }
                else
                {
                    if (compApp.EXPIRY_DATE == null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE.ToString()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE.ToString()));
                    }
                }

                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE.ToString()));

                // get address
                C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                if (sCatCode != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(issueDt));
                content = appendCertContent(content, appendDoubleQuote(lastDocRecDt));

                // get comp applicant infor list
                List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                String dummySearchResult = "";
                foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                {
                    C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                    C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                    dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                        + " (" + svRole.CODE + ")@ ";
                }
                content = appendCertContent(content, appendDoubleQuote(dummySearchResult));

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if ("MWC".Equals(catCodeStr) || "MWI".Equals(catCodeStr) || "MWC(P)".Equals(catCodeStr))
                    {
                        // update/print sequence no
                        String newSequenceNo = getCertSeqNo(catCodeStr, "Y");
                        content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
                        AuditLogService.logDebug("SequenceNumber: " + newSequenceNo + ", Code:" + catCodeStr);

                        // MwItem List
                        List<C_COMP_APPLICANT_MW_ITEM> companyMwItemList = new List<C_COMP_APPLICANT_MW_ITEM>();
                        List<String> mwClass1TypeList = new List<String>();
                        List<String> mwClass2TypeList = new List<String>();
                        List<String> mwClass3TypeList = new List<String>();

                        // get mw items
                        List<C_COMP_APPLICANT_INFO> applicantList = getCompApplicantInfo(compApp.UUID);
                        foreach (C_COMP_APPLICANT_INFO compAppInfo in applicantList)
                        {
                            List<C_COMP_APPLICANT_MW_ITEM> compApplicantMwItemList = getCompApplicantMwItem(compAppInfo.UUID);
                            if (compApplicantMwItemList != null && compApplicantMwItemList.Count > 0)
                            {
                                C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                                C_S_SYSTEM_VALUE svStatus = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);

                                if (RegistrationConstant.STATUS_ACTIVE.Equals(svStatus.CODE)
                                    && svRole.CODE.StartsWith("A"))
                                {
                                    companyMwItemList.AddRange(compApplicantMwItemList);
                                }
                            }
                        }

                        // get company Capability by Applicant
                        // add to class 1 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_I.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass1TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        // add to class 2 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_II.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE) && !mwClass2TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass2TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        // add to class 3 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_III.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE)
                                    && !mwClass2TypeList.Contains(svMwType.CODE)
                                    && !mwClass3TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass3TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        mwClass1TypeList.Sort();
                        mwClass2TypeList.Sort();
                        mwClass3TypeList.Sort();

                        String[] MW_CLASS_DESCRIPTION = { " I, II & III ", " II & III ", " III " };
                        int printLineCoutner = 0;
                        int MAX_LINE = RegistrationConstant.MW_TYPE_DESCRIPTION.Length;
                        for (int i = 0; i < RegistrationConstant.MW_TYPE.Length; i++)
                        {
                            String mwType = RegistrationConstant.MW_TYPE[i];
                            String mwTypeDesc = RegistrationConstant.MW_TYPE_DESCRIPTION[i];
                            if (mwClass1TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[0];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            } else if (mwClass2TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[1];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            }
                            else if (mwClass3TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[2];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            }
                        }

                        for (int i = printLineCoutner; i < MAX_LINE; i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                }
            }

            return content;
        }

        public String populateCompExportLetter(String process, String compAppUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";

            // certificate common colunn header
            List<string> certHeaderList = new List<string>()
            {
                "file_ref", "reg_no", "br_no", "ename", "cname",
                "cat_code", "cat_grp_code", "cat_grp_desc",
                "reg_dt",
                "gazet_dt",
                "expiry_dt",
                "restore_app_dt",
                "restore_dt",
                "retent_app_dt",
                "removal_dt",
                "approval_dt",
                "addr1", "addr2", "addr3",
                "addr4", "addr5",
                "caddr1", "caddr2", "caddr3",
                "caddr4", "caddr5",
                "tel_no1",  "fax_no1",

                "auth_name", "auth_cname",
                "auth_rank", "auth_crank",
                "acting", "auth_tel",
                "auth_fax",

                "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                "as_td",
                "cert_cname"
            };

            // append header
            foreach (String certHeader in certHeaderList)
            {
                content = appendCertContent(content, certHeader);
            }
            content += "\r\n";


            // get compApp by uuid
            C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

            if (compApp != null)
            {
                // get sCatCode
                C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME));
                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                content = appendCertContent(content, appendDoubleQuote(sCatCode.CODE));

                // get svCatGrp
                C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                content = appendCertContent(content, compApp.REGISTRATION_DATE != null ? appendDoubleQuote(compApp.REGISTRATION_DATE.ToString()) : "");
                content = appendCertContent(content, compApp.GAZETTE_DATE != null ? appendDoubleQuote(compApp.GAZETTE_DATE.ToString()) : "");

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content += appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : "");
                    }
                    else
                    {
                        if (compApp.EXPIRY_DATE != null)
                        {
                            content += appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : "");
                        }
                        else
                        {
                            content += appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : "");
                        }
                    }
                }

                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));

                // get address
                C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));

                // parameters from UI
                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));

                // get comp applicant info list
                List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                String dummySearchResult = "";
                foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                {
                    C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                    C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                    dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                        + " (" + svRole.CODE + ")@ ";
                }
                content = appendCertContent(content, appendDoubleQuote(dummySearchResult));

                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
            }

            return content;
        }

        public String populateCompExportLetterWithCode(String certificateNo, String process, String compAppUuid,
            String rptIssueDt, String rptRcvdDt, String signature
            , String missingItemRetensionNullChk, String missingItemRetensionChequeChk, String missingItemRetensionProfRegCertChk
            , String missingItemRetensionOthersChk, String missingItemRetensionIncompleteFormChk
            , String missingItemRestorationNullChk, String missingItemRestorationChequeChk, String missingItemRestorationProfRegCertChk
            , String missingItemRestorationOthersChk, String missingItemRestorationIncompleteFormChk
            )
        {
            int intProcess = int.Parse(process);
            String content = "";

            if (RegistrationConstant.LETTER_MMD0008B_1.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0008B_2.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0009B.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0007B_1.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0007B_2.Equals(certificateNo))
            {
                // header
                List<string> certHeaderList = new List<string>()
                {   "file_ref", "reg_no", "br_no", "ename", "cname",
                    "cat_code", "cat_grp_code", "cat_grp_desc",
                    "reg_dt",
                    "gazet_dt",
                    "expiry_dt",
                    "restore_app_dt",
                    "restore_dt",
                    "retent_app_dt",
                    "removal_dt",
                    "approval_dt",
                    "addr1", "addr2", "addr3",
                    "addr4", "addr5",
                    "caddr1", "caddr2", "caddr3",
                    "caddr4", "caddr5",
                    "tel_no1",  "fax_no1",

                    "auth_name", "auth_cname",
                    "auth_rank", "auth_crank",
                    "acting", "auth_tel",
                    "auth_fax",

                    "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                        "as_td",
                        "cert_cname"
                };

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                // get compApp by uuid
                C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

                if (compApp != null)
                {
                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                    String catCodeStr = "";
                    if (sCatCode != null)
                    {
                        catCodeStr = sCatCode.CODE;
                        catCodeStr = catCodeStr.Trim();
                    }

                    content += appendDoubleQuote(compApp.FILE_REFERENCE_NO != null ? compApp.FILE_REFERENCE_NO.ToString() : "");
                    content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO != null ? compApp.CERTIFICATION_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO != null ? compApp.BR_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME != null ? compApp.ENGLISH_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(catCodeStr));

                    // get svCatGrp
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                    content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE != null ? compApp.REGISTRATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE != null ? compApp.GAZETTE_DATE.ToString() : ""));

                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                    }
                    else
                    {
                        if (compApp.EXPIRY_DATE != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : ""));
                        }
                    }

                    if (RegistrationConstant.LETTER_MMD0009B.Equals(certificateNo))
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));

                    // get address
                    C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                    C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                    if (engAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                    }
                    if (chiAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                    // get authority
                    C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                    if (sAuthority != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));

                    if (RegistrationConstant.LETTER_MMD0007B_2.Equals(certificateNo))
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                    }
                    content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                }
            }
            else if (RegistrationConstant.FAX_TEMPLATE_RETENTION.Equals(certificateNo) || RegistrationConstant.FAX_TEMPLATE_RESTORATION.Equals(certificateNo))
            {
                // header
                List<string> certHeaderList = new List<string>()
                {
                    "file_ref", "reg_no", "br_no", "ename", "cname",
                    "cat_code", "cat_grp_code", "cat_grp_desc",
                    "reg_dt",
                    "gazet_dt",
                    "expiry_dt",
                    "restore_app_dt",
                    "restore_dt",
                    "retent_app_dt",
                    "removal_dt",
                    "approval_dt",
                    "addr1", "addr2", "addr3",
                    "addr4", "addr5",
                    "caddr1", "caddr2", "caddr3",
                    "caddr4", "caddr5",
                    "tel_no1",  "fax_no1",

                    "auth_name", "auth_cname",
                    "auth_rank", "auth_crank",
                    "acting", "auth_tel",
                    "auth_fax",

                    "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                    "missing_item",
                    "as_td",
                    "cert_cname",
                    "form_used","miss_1","miss_2","miss_3","miss_4","reg_name"
                };

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                // get compApp by uuid
                C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

                if (compApp != null)
                {
                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                    String catCodeStr = "";
                    if (sCatCode != null)
                    {
                        catCodeStr = sCatCode.CODE;
                        catCodeStr = catCodeStr.Trim();
                    }

                    content += appendDoubleQuote(compApp.FILE_REFERENCE_NO != null ? compApp.FILE_REFERENCE_NO.ToString() : "");
                    content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO != null ? compApp.CERTIFICATION_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO != null ? compApp.BR_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME != null ? compApp.ENGLISH_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(catCodeStr));

                    // get svCatGrp
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                    content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE != null ? compApp.REGISTRATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE != null ? compApp.GAZETTE_DATE.ToString() : ""));

                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : ""));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE != null ? compApp.RETENTION_APPLICATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));


                    // get address
                    C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                    C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                    if (engAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                    }
                    if (chiAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                    // get authority
                    C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                    if (sAuthority != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // print parameters from UI
                    content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                    content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));
                }

                if (RegistrationConstant.FAX_TEMPLATE_RETENTION.Equals(certificateNo)) {
                    if ("CHECKED".Equals(missingItemRetensionNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("NULL"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));

                    // get systemValue by form_ID
                    if (compApp.APPLICATION_FORM_ID != null)
                    {
                        C_S_SYSTEM_VALUE svForm = getSSystemValueByUuid(compApp.APPLICATION_FORM_ID);
                        if (svForm != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svForm.CODE));
                        }
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // handle parameters from UI
                    if ("CHECKED".Equals(missingItemRetensionNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionChequeChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionProfRegCertChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionOthersChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionIncompleteFormChk) ? "Y" : "N"));
                    }

                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);
                    if (sCatCode != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_DESCRIPTION));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                }
                else if (RegistrationConstant.FAX_TEMPLATE_RESTORATION.Equals(certificateNo))
                {
                    if ("CHECKED".Equals(missingItemRestorationNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("NULL"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));

                    // get systemValue by form_ID
                    if (compApp.APPLICATION_FORM_ID != null)
                    {
                        C_S_SYSTEM_VALUE svForm = getSSystemValueByUuid(compApp.APPLICATION_FORM_ID);
                        if (svForm != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svForm.CODE));
                        }
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // handle parameters from UI
                    if ("CHECKED".Equals(missingItemRestorationNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationChequeChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationProfRegCertChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationOthersChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationIncompleteFormChk) ? "Y" : "N"));
                    }

                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);
                    if (sCatCode != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_DESCRIPTION));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                }
            }
            return content;
        }

        // get cert seq no from oracle function
        public String getCertSeqNo(String type, String isUpdateSeq)
        {
            type = "CERTIFICATE_" + type;

            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            QueryParameters.Add("type", type);
            QueryParameters.Add("isUpdateSeq", isUpdateSeq);

            String q = "select C_GET_CERT_SEQ_NO(':type', ':isUpdateSeq') as CERT_SEQ_NO from dual";
            q = q.Replace(":type", type);
            q = q.Replace(":isUpdateSeq", isUpdateSeq);

            String resultSeqNo = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        resultSeqNo = Data[0]["CERT_SEQ_NO"].ToString();
                    }
                    conn.Close();
                }
            }

            return resultSeqNo;
        }

        public C_IND_CERTIFICATE getIndCertificateByUuid(String indCertUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_CERTIFICATE indCertificate = db.C_IND_CERTIFICATE.Where(o => o.UUID == indCertUuid).FirstOrDefault();
                return indCertificate;
            }
        }

        public C_IND_CERTIFICATE getIndCertificateByCatId(String indCertUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_CERTIFICATE indCertificate = db.C_IND_CERTIFICATE.Where(o => o.CATEGORY_ID == indCertUuid).FirstOrDefault();
                return indCertificate;
            }
        }


        public C_IND_APPLICATION getIndApplicationByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION indApplication = db.C_IND_APPLICATION.Where(o => o.UUID == uuid).FirstOrDefault();
                return indApplication;
            }
        }

        public C_IND_APPLICATION getIndApplicationByFileRef(String FileRef)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION indApplication = db.C_IND_APPLICATION.Where(o => o.FILE_REFERENCE_NO == FileRef).FirstOrDefault();
                return indApplication;
            }
        }

        public C_COMP_APPLICATION getCompApplicationByUuid(String compAppUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.UUID == compAppUuid).FirstOrDefault();
                return application;
            }
        }
        

        public C_IND_QUALIFICATION getIndQualificationByPrbUuid(String PrbUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_QUALIFICATION application = db.C_IND_QUALIFICATION.Where(o => o.UUID == PrbUuid).FirstOrDefault();
                return application;
            }
        }



        public C_COMP_APPLICATION getCompApplicationByFileRef(String FileRef)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.FILE_REFERENCE_NO == FileRef).FirstOrDefault();
                return application;
            }
        }

        public C_INTERVIEW_CANDIDATES getInterviewCandidatesByUuid(String ivCandUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_CANDIDATES application = db.C_INTERVIEW_CANDIDATES.Where(o => o.UUID == ivCandUuid).FirstOrDefault();
                return application;
            }
        }

        public C_INTERVIEW_CANDIDATES getInterviewCandidatesByIvSchId(String ivSchid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_CANDIDATES application = db.C_INTERVIEW_CANDIDATES.Where(o => o.INTERVIEW_SCHEDULE_ID== ivSchid).FirstOrDefault();
                return application;
            }
        }

        public C_IND_PROCESS_MONITOR getProcessMonitorByUuid(String PMUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_PROCESS_MONITOR application = db.C_IND_PROCESS_MONITOR.Where(o => o.UUID == PMUuid).FirstOrDefault();
                return application;
            }
        }


        public C_COMP_APPLICANT_INFO getCompApplicantInfoByUuid(String compAppUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICANT_INFO application = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == compAppUuid).FirstOrDefault();
                return application;
            }
        }

        public C_COMP_APPLICANT_INFO getCompApplicantInfoByMasterId(String compAppUuid)
        {
            if(compAppUuid != null) {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_COMP_APPLICANT_INFO application = db.C_COMP_APPLICANT_INFO.Where(o => o.MASTER_ID == compAppUuid).FirstOrDefault();
                    return application;
                }
            }
            return null;
           
        }

        public C_COMP_APPLICANT_INFO getCompApplicantInfoByApplicantId(String compAppUuid)
        {
            if (compAppUuid != null)
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_COMP_APPLICANT_INFO application = db.C_COMP_APPLICANT_INFO.Where(o => o.APPLICANT_ID == compAppUuid).FirstOrDefault();
                    return application;
                }
            }
            return null;

        }

        //new add
        public C_S_EXPORT_LETTER getExportLetterByUuid(String exportLetterUUID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_EXPORT_LETTER application = db.C_S_EXPORT_LETTER.Where(o => o.UUID == exportLetterUUID).FirstOrDefault();
                return application;
            }
        }

        public C_S_SYSTEM_VALUE getSSystemValueByUuid(String svUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE sv = db.C_S_SYSTEM_VALUE.Where(o => o.UUID == svUuid).FirstOrDefault();
                return sv;
            }
        }



 
        public C_COMP_APPLICANT_INFO getCCompApplicantByUuid(C_APPLICANT C_APPLICANT)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICANT_INFO cai = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == C_APPLICANT.UUID).FirstOrDefault();
                return cai;
            }
        }

        public C_S_CATEGORY_CODE getSCategoryCodeByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_CATEGORY_CODE sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.UUID == uuid).FirstOrDefault();
                return sCategoryCode;
            }
        }

        public C_S_CATEGORY_CODE_DETAIL getSCategoryCodeDetaiByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = db.C_S_CATEGORY_CODE_DETAIL.Where(o => o.UUID == uuid).FirstOrDefault();
                return sCategoryCodeDetail;
            }
        }

        public C_S_CATEGORY_CODE getCompSCategoryCode(C_COMP_APPLICATION C_COMP_APPLICATION)
        {
            if(C_COMP_APPLICATION != null) {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_S_CATEGORY_CODE sCatCode = db.C_S_CATEGORY_CODE.Where(o => o.UUID == C_COMP_APPLICATION.CATEGORY_ID).FirstOrDefault();
                    return sCatCode;
                }
            }
            return null ;
           
        }

        public C_ADDRESS getAddressByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_ADDRESS address = db.C_ADDRESS.Where(o => o.UUID == uuid).FirstOrDefault();
                return address;
            }
        }

        public C_S_AUTHORITY getAuthorityByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_AUTHORITY sAuthority = db.C_S_AUTHORITY.Where(o => o.UUID == uuid).FirstOrDefault();
                return sAuthority;
            }
        }

        public C_APPLICANT getApplicantByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_APPLICANT app = db.C_APPLICANT.Where(o => o.UUID == uuid).FirstOrDefault();
                return app;
            }
        }

        public List<C_COMP_APPLICANT_MW_ITEM> getCompApplicantMwItem(String compAppInfoUuid)
        {
            List<C_COMP_APPLICANT_MW_ITEM> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppMwItem in db.C_COMP_APPLICANT_MW_ITEM
                              where 1 == 1
                              && compAppMwItem.COMPANY_APPLICANTS_ID == compAppInfoUuid
                              select compAppMwItem).ToList();
            }

            return resultList;
        }

        public List<C_COMP_APPLICANT_INFO> getCompApplicantInfo(String compAppUuid)
        {
            List<C_COMP_APPLICANT_INFO> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                              where 1 == 1
                              && compAppInfo.MASTER_ID == compAppUuid
                              select compAppInfo).ToList();
            }

            return resultList;
        }

        public List<C_IND_APPLICATION_MW_ITEM> getIndApplicationMwItem(String MwItemUuid)
        {
            List<C_IND_APPLICATION_MW_ITEM> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from mwItem in db.C_IND_APPLICATION_MW_ITEM
                              join indApp in db.C_IND_APPLICATION on mwItem.MASTER_ID equals indApp.UUID
                              where 1 == 1
                              && mwItem.UUID == MwItemUuid
                              select mwItem).ToList();
            }

            return resultList;
        }

        public List<C_IND_CERTIFICATE> getIndApplication(String IndCertUuid)
        {
            List<C_IND_CERTIFICATE> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indCert in db.C_IND_CERTIFICATE
                              join indApp in db.C_IND_APPLICATION on indCert.MASTER_ID equals indApp.UUID
                              where 1 == 1
                              && indApp.UUID == IndCertUuid
                              select indCert).ToList();
            }

            return resultList;
        }

        public C_COMP_APPLICANT_INFO getCompApplicantInfoAs(String selectAsUuid)
        {
            C_COMP_APPLICANT_INFO result = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                result = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                              where 1 == 1
                              && compAppInfo.MASTER_ID == selectAsUuid
                               select compAppInfo).FirstOrDefault();
            }

            return result;
        }

        public C_COMP_APPLICANT_INFO getCompApplicantInfoTd(String selectTdUuid)
        {
            C_COMP_APPLICANT_INFO result = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                result = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                          where 1 == 1
                          && compAppInfo.MASTER_ID == selectTdUuid
                          select compAppInfo).FirstOrDefault();
            }

            return result;
        }

        public C_IND_APPLICATION getIndApplicationApplicant(String selectTdUuid)
        {
            C_IND_APPLICATION result = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                result = (from indApp in db.C_IND_APPLICATION
                          where 1 == 1
                          && indApp.UUID == selectTdUuid
                          select indApp).FirstOrDefault();
            }

            return result;
        }


        public List<C_IND_QUALIFICATION_DETAIL> getIndQualificationDetailsListByIndQualUuid(String indQualUuid)
        {
            List<C_IND_QUALIFICATION_DETAIL> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indQualDetail in db.C_IND_QUALIFICATION_DETAIL
                              where 1 == 1
                              && indQualDetail.IND_QUALIFICATION_ID == indQualUuid
                              select indQualDetail).ToList();
            }
            return resultList;
        }

        public List<C_IND_APPLICATION_MW_ITEM> getIndApplicationMwItemByMasterId(String masterId)
        {
            List<C_IND_APPLICATION_MW_ITEM> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indAppMwItem in db.C_IND_APPLICATION_MW_ITEM
                              where 1 == 1
                              && indAppMwItem.MASTER_ID == masterId
                              select indAppMwItem).ToList();
            }
            return resultList;
        }

        public List<C_S_SYSTEM_VALUE> getIndApplicationMwItemByIndAppUuid(String IndAppUuid)
        {
            List<C_S_SYSTEM_VALUE> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indAppMwItem in db.C_IND_APPLICATION_MW_ITEM
                              join svMwItem in db.C_S_SYSTEM_VALUE on indAppMwItem.ITEM_DETAILS_ID equals svMwItem.UUID
                              where 1 == 1
                              && indAppMwItem.MASTER_ID == IndAppUuid
                              select svMwItem).ToList();
            }
            return resultList;
        }

        public List<C_IND_QUALIFICATION> getIndQualificationListByMasterId(String indAppUuid)
        {
            List<C_IND_QUALIFICATION> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indQualification in db.C_IND_QUALIFICATION
                              where 1 == 1
                              && indQualification.MASTER_ID == indAppUuid
                              select indQualification).ToList();
            }
            return resultList;
        }

        public List<C_COMP_APPLICANT_INFO> getCompApplicantInfoForExportData(String compAppUuid, DateTime date)
        {
            List<C_COMP_APPLICANT_INFO> resultList = null;


            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                              join svRole in db.C_S_SYSTEM_VALUE on compAppInfo.APPLICANT_ROLE_ID equals svRole.UUID
                              join svStatus in db.C_S_SYSTEM_VALUE on compAppInfo.APPLICANT_STATUS_ID equals svStatus.UUID
                              join app in db.C_APPLICANT on compAppInfo.APPLICANT_ID equals app.UUID
                              where 1 == 1
                              && compAppInfo.MASTER_ID == compAppUuid
                              && (svRole.CODE == "AS" || svRole.CODE == "TD")
                              && svStatus.CODE == "1"
                              && compAppInfo.ACCEPT_DATE <= date
                              && (compAppInfo.REMOVAL_DATE == null || compAppInfo.REMOVAL_DATE >= date)
                              orderby app.SURNAME, app.GIVEN_NAME_ON_ID, svRole.CODE
                              select compAppInfo).ToList();

            }

            return resultList;
        }

        public string appendCertContent(String certContent, string appendStr)
        {
            certContent += String.IsNullOrEmpty(certContent) ? appendStr : "," + appendStr;
            return certContent;
        }

        public string appendICContent(String ICContent, string appendStr)
        {
            ICContent += String.IsNullOrEmpty(ICContent) ? appendStr : "\t" + appendStr;
            return ICContent;
        }

        public string ExportUASforGCAandMWCAReport(UpdateAppStatusSearchModel model, string RegType)
        {
            model.Query = SearchUAS;
            model.QueryWhere = SearchUAS_whereQ(model, RegType);
            return model.Export("DataExport");
        }
        public UpdateAppStatusSearchModel SearchUASforGCAandMWCA(UpdateAppStatusSearchModel model, string RegType)
        {
            model.Query = SearchUAS;  //need to change query after all
            model.QueryWhere = SearchUAS_whereQ(model, RegType);
            model.Search();
            return model;
        }
        //add on 20190429
        private string SearchUAS_whereQ(UpdateAppStatusSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND T1.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND upper(T1.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + " AND (upper(T1.ENGLISH_COMPANY_NAME) LIKE :ComName OR T1.CHINESE_COMPANY_NAME LIKE:ComName)";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
            return whereQ;
        }
        //add on 20190514
        public ProcessMonitorDisplayModel ViewCompPM(string compUUID, string pmUUID, string compAppUUID, string RegType, string MonitorType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (pmUUID != "null" && pmUUID != null)
                {
                    C_COMP_PROCESS_MONITOR compPM = db.C_COMP_PROCESS_MONITOR.Where(o => o.UUID == pmUUID).FirstOrDefault();
                    //C_COMP_APPLICANT_INFO compAppInfo = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == compAppUUID).FirstOrDefault();
                    C_COMP_APPLICATION compApp = db.C_COMP_APPLICATION.Where(o => o.UUID == compUUID).FirstOrDefault();
                    C_COMP_APPLICANT_INFO compAppInfo = new C_COMP_APPLICANT_INFO();
                    C_APPLICANT applicant = new C_APPLICANT();
                    List<C_INTERVIEW_SCHEDULE> InterviewDateByCompApplicantInfo = new List<C_INTERVIEW_SCHEDULE>();
                    string Role = "";
                    if (!string.IsNullOrWhiteSpace(compAppUUID))
                    {
                         compAppInfo = (from t1 in db.C_COMP_APPLICANT_INFO
                                                             join t2 in db.C_COMP_APPLICATION on t1.MASTER_ID equals t2.UUID
                                                             where t2.UUID == compUUID && t1.UUID == compAppUUID
                                                             select t1).FirstOrDefault();
                         applicant = (from app in db.C_APPLICANT
                                                 join compA in db.C_COMP_APPLICANT_INFO on app.UUID equals compA.APPLICANT_ID
                                                 where compA.UUID == compAppInfo.UUID
                                                 select app).FirstOrDefault();

                         Role = (from sv1 in db.C_S_SYSTEM_VALUE
                                       join ca2 in db.C_COMP_APPLICANT_INFO on sv1.UUID equals ca2.APPLICANT_ROLE_ID
                                       where sv1.UUID == compAppInfo.APPLICANT_ROLE_ID
                                       select sv1.CODE).FirstOrDefault();

                        InterviewDateByCompApplicantInfo = (from INS in db.C_INTERVIEW_SCHEDULE
                                                                                       join INC in db.C_INTERVIEW_CANDIDATES on INS.UUID equals INC.INTERVIEW_SCHEDULE_ID
                                                                                       join CAI in db.C_COMP_APPLICANT_INFO on INC.CANDIDATE_NUMBER equals CAI.CANDIDATE_NUMBER
                                                                                       where CAI.CANDIDATE_NUMBER == compAppInfo.CANDIDATE_NUMBER
                                                                                       select INS).ToList();



                    }



                    List<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems.Add(new SelectListItem
                    {
                        Text = " ",
                        Value = "",
                        Selected = true
                    });
                    selectListItems.AddRange((
                                                from sv in InterviewDateByCompApplicantInfo
                                                select new SelectListItem()
                                                {
                                                    Text = sv.INTERVIEW_DATE.ToString(),
                                                    Value = sv.INTERVIEW_DATE.ToString(),
                                                }
                                     ).ToList());

                    return new ProcessMonitorDisplayModel()
                    {
                        interviewDateList = selectListItems
                        ,
                        InterResultID = compPM.INTERVIEW_RESULT_ID
                        ,
                        RegType = RegType
                        ,
                        C_COMP_PROCESS_MONITOR = compPM
                        ,
                        C_COMP_APPLICANT_INFO = compAppInfo
                        ,
                        C_COMP_APPLICATION = compApp
                        ,
                        pmUUID = compPM.UUID
                        ,
                        C_APPLICANT = applicant
                        ,
                        CandidateName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                        ,
                        RoleType = Role
                        ,
                        MonitorType = MonitorType
                        ,
                        CGCAppList = compPM.APPLICATION_FORM_ID
                        ,
                        MWCAppList = compPM.APPLICATION_FORM_ID
                    };
                }
                else {
                    if (MonitorType == RegistrationConstant.PROCESS_MONITOR_UPM)
                    {
                        //C_COMP_APPLICANT_INFO compAppInfo = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == compAppUUID).FirstOrDefault();
                        C_COMP_APPLICATION compApp = db.C_COMP_APPLICATION.Where(o => o.UUID == compUUID).FirstOrDefault();
                        C_COMP_APPLICANT_INFO compAppInfo = (from t1 in db.C_COMP_APPLICANT_INFO
                                                             join t2 in db.C_COMP_APPLICATION on t1.MASTER_ID equals t2.UUID
                                                             where t2.UUID == compUUID && t1.UUID == compAppUUID
                                                             select t1).FirstOrDefault();
                        C_APPLICANT applicant = (from app in db.C_APPLICANT
                                                 join compA in db.C_COMP_APPLICANT_INFO on app.UUID equals compA.APPLICANT_ID
                                                 where compA.UUID == compAppInfo.UUID
                                                 select app).FirstOrDefault();

                        string Role = (from sv1 in db.C_S_SYSTEM_VALUE
                                       join ca2 in db.C_COMP_APPLICANT_INFO on sv1.UUID equals ca2.APPLICANT_ROLE_ID
                                       where sv1.UUID == compAppInfo.APPLICANT_ROLE_ID
                                       select sv1.CODE).FirstOrDefault();

                        List<C_INTERVIEW_SCHEDULE> InterviewDateByCompApplicantInfo = (from INS in db.C_INTERVIEW_SCHEDULE
                                                                                       join INC in db.C_INTERVIEW_CANDIDATES on INS.UUID equals INC.INTERVIEW_SCHEDULE_ID
                                                                                       join CAI in db.C_COMP_APPLICANT_INFO on INC.CANDIDATE_NUMBER equals CAI.CANDIDATE_NUMBER
                                                                                       where CAI.CANDIDATE_NUMBER == compAppInfo.CANDIDATE_NUMBER
                                                                                       select INS).ToList();

                        List<SelectListItem> selectListItems = new List<SelectListItem>();
                        selectListItems.Add(new SelectListItem
                        {
                            Text = " ",
                            Value = "",
                            Selected = true
                        });
                        selectListItems.AddRange((from sv in InterviewDateByCompApplicantInfo
                                                  select new SelectListItem()
                                                  {
                                                      Text = sv.INTERVIEW_DATE.ToString(),
                                                      Value = sv.INTERVIEW_DATE.ToString(),
                                                  }
                                         ).ToList());

                        return new ProcessMonitorDisplayModel()
                        {
                            interviewDateList = selectListItems
                            //InterviewDateByCompApplicantInfo = InterviewDateByCompApplicantInfo
                            ,
                            RegType = RegType
                            ,
                            C_COMP_APPLICANT_INFO = compAppInfo
                            ,
                            C_COMP_APPLICATION = compApp
                            ,
                            C_APPLICANT = applicant
                            ,
                            CandidateName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                            ,
                            RoleType = Role
                            ,
                            MonitorType = MonitorType
                        };
                    }
                    else
                    {
                        C_COMP_APPLICATION compApp = db.C_COMP_APPLICATION.Where(o => o.UUID == compUUID).FirstOrDefault();

                        C_COMP_APPLICANT_INFO compAppInfo = (from t1 in db.C_COMP_APPLICANT_INFO
                                                             join t2 in db.C_COMP_APPLICATION on t1.MASTER_ID equals t2.UUID
                                                             where t2.UUID == compUUID
                                                             select t1).FirstOrDefault();


                        return new ProcessMonitorDisplayModel()
                        {
                            C_COMP_APPLICANT_INFO = compAppInfo
                            ,
                            MonitorType = MonitorType
                            ,
                            C_COMP_APPLICATION = compApp
                        };
                    }

                }
            }
        }

        public ProcessMonitorDisplayModel ViewIndPM(string certUUID, string pmUUID, string RegType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (pmUUID != "null" && pmUUID != null) {
                    C_IND_PROCESS_MONITOR IndPM = db.C_IND_PROCESS_MONITOR.Where(o => o.UUID == pmUUID).FirstOrDefault();
                    C_IND_APPLICATION IndApp = db.C_IND_APPLICATION.Where(o => o.UUID == IndPM.MASTER_ID).FirstOrDefault();
                    C_APPLICANT applicant = (from t1 in db.C_APPLICANT
                                             join t2 in db.C_IND_APPLICATION on t1.UUID equals t2.APPLICANT_ID
                                             where t2.UUID == IndApp.UUID
                                             select t1).FirstOrDefault();

                    C_S_CATEGORY_CODE SCategoryCode = (from t1 in db.C_S_CATEGORY_CODE
                                                       join t2 in db.C_IND_CERTIFICATE on t1.UUID equals t2.CATEGORY_ID
                                                       join t3 in db.C_IND_APPLICATION on t2.MASTER_ID equals t3.UUID
                                                       where t3.UUID == IndApp.UUID
                                                       select t1).FirstOrDefault();
                    return new ProcessMonitorDisplayModel()
                    {
                        SuppleDocumentDate = IndPM.SUPPLE_DOCUMENT_DATE
                        ,
                        RegType = RegType
                        ,
                        C_IND_APPLICATION = IndApp
                        ,
                        C_IND_PROCESS_MONITOR = IndPM
                        ,
                        C_S_CATEGORY_CODE = SCategoryCode
                        ,
                        pmUUID = IndPM.UUID
                        ,
                        C_APPLICANT = applicant
                        ,
                        CandidateName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                        ,
                        IPAppList = IndPM.APPLICATION_FORM_ID
                        ,
                        MWIAAppList = IndPM.APPLICATION_FORM_ID
                        ,
                        HKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID)
                        ,
                        PASSPORTNO = EncryptDecryptUtil.getDecryptHKID(applicant.PASSPORT_NO)
                    };
                }
                else
                {
                    C_IND_CERTIFICATE IndCert = db.C_IND_CERTIFICATE.Where(o => o.UUID == certUUID).FirstOrDefault();
                    C_IND_APPLICATION IndApp = db.C_IND_APPLICATION.Where(o => o.UUID == IndCert.MASTER_ID).FirstOrDefault();
                    C_APPLICANT applicant = (from t1 in db.C_APPLICANT
                                             join t2 in db.C_IND_APPLICATION on t1.UUID equals t2.APPLICANT_ID
                                             where t2.UUID == IndApp.UUID
                                             select t1).FirstOrDefault();
                    C_S_CATEGORY_CODE SCategoryCode = (from t1 in db.C_S_CATEGORY_CODE
                                                       join t2 in db.C_IND_CERTIFICATE on t1.UUID equals t2.CATEGORY_ID
                                                       join t3 in db.C_IND_APPLICATION on t2.MASTER_ID equals t3.UUID
                                                       where t3.UUID == IndApp.UUID
                                                       select t1).FirstOrDefault();
                    return new ProcessMonitorDisplayModel()
                    {
                        RegType = RegType
                        ,
                        C_IND_CERTIFICATE = IndCert
                        ,
                        C_IND_APPLICATION = IndApp
                        ,
                        C_S_CATEGORY_CODE = SCategoryCode
                        ,
                        certUUID = IndCert.UUID
                        ,
                        C_APPLICANT = applicant
                        ,
                        CandidateName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID
                        ,
                        HKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID)
                        ,
                        PASSPORTNO = EncryptDecryptUtil.getDecryptHKID(applicant.PASSPORT_NO)
                    };
                }
            }
        }

        public UpdateAppStatusDisplayModel ViewCompUAS(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.UUID == id).FirstOrDefault();

                List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                             join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                             where t2.MASTER_ID == id
                                             select t1).ToList();

                C_ADDRESS chinessAddess = null;
                C_ADDRESS englishAddress = null;
                if (application.CHINESE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.CHINESE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_ADDRESS_ID).FirstOrDefault();
                }

                if (application.ENGLISH_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.ENGLISH_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_ADDRESS_ID).FirstOrDefault();
                }
                return new UpdateAppStatusDisplayModel()
                {
                    DateVetting = application.VETTING_DATE
                    ,
                    C_COMP_APPLICATION = application
                    ,
                    C_ADDRESS_Chinese = chinessAddess
                    ,
                    C_ADDRESS_English = englishAddress
                    ,
                    dateRetentionApplcationRet = application.RETENTION_APPLICATION_DATE
                    ,
                    dateRestorationApplicationRes = application.RESTORATION_APPLICATION_DATE
                    ,
                    NewRegExtDate = application.EXTEND_DATE
                    ,
                    RetExtDate = application.EXTEND_DATE
                    ,
                    ResExtDate = application.EXTEND_DATE

                };
            }
        }
        public UpdateAppStatusDisplayModel ViewIndUAS(string id ,string code)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION application = db.C_IND_APPLICATION.Where(o => o.UUID == id).FirstOrDefault();
                C_APPLICANT IndApplcation = db.C_APPLICANT.Where(o => o.UUID == application.APPLICANT_ID).FirstOrDefault();
                //C_APPLICANT IndApplcation = (from t1 in db.C_APPLICANT
                //                             join t2 in db.C_IND_APPLICATION on t1.UUID equals t2.APPLICANT_ID
                //                             where t2.UUID == id
                //                             select t1).FirstOrDefault();

                //C_IND_CERTIFICATE IndCertificate = db.C_IND_CERTIFICATE.Where(o => o.MASTER_ID == application.UUID).FirstOrDefault();

                C_IND_CERTIFICATE IndCertificate = (from t1 in db.C_IND_CERTIFICATE
                                                    join t2 in db.C_IND_APPLICATION on t1.MASTER_ID equals t2.UUID
                                                    join cat in db.C_S_CATEGORY_CODE on t1.CATEGORY_ID equals cat.UUID
                                                    where t2.UUID == id && cat.CODE == code
                                                    select t1).FirstOrDefault();

                C_S_CATEGORY_CODE SCategoryCode = (from t1 in db.C_S_CATEGORY_CODE
                                                   join t2 in db.C_IND_CERTIFICATE on t1.UUID equals t2.CATEGORY_ID
                                                   join t3 in db.C_IND_APPLICATION on t2.MASTER_ID equals t3.UUID
                                                   where t3.UUID == id && t1.CODE == code
                                                   select t1).FirstOrDefault();

                var CertiList = (from indcert in db.C_IND_CERTIFICATE
                                 join svperiod in db.C_S_SYSTEM_VALUE on indcert.PERIOD_OF_VALIDITY_ID equals svperiod.UUID
                                 join svappform in db.C_S_SYSTEM_VALUE on indcert.APPLICATION_FORM_ID equals svappform.UUID
                                 join catcode in db.C_S_CATEGORY_CODE on indcert.CATEGORY_ID equals catcode.UUID
                                 where indcert.MASTER_ID == id
                                 select new UpdateAppStatusDisplayModel.CERTLIST { CERT_c_IND_CERTIFICATE = indcert, CERT_APPFORM_c_S_SYSTEM_VALUE = svappform, CERT_PERIODVAD_c_S_SYSTEM_VALUE = svperiod, CERT_c_S_CATEGORY_CODE = catcode }).ToList();

                C_ADDRESS chinessAddess = null;
                C_ADDRESS englishAddress = null;

                if (application.CHINESE_HOME_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_HOME_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.CHINESE_OFFICE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_OFFICE_ADDRESS_ID).FirstOrDefault();
                }
                if (application.ENGLISH_HOME_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_HOME_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.ENGLISH_OFFICE_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_OFFICE_ADDRESS_ID).FirstOrDefault();
                }

                return new UpdateAppStatusDisplayModel()
                {
                    CatCode=code
                    ,
                    IndDateVetting = IndCertificate.VETTING_DATE
                    ,
                    C_IND_APPLICATION = application
                    ,
                    C_APPLICANT = IndApplcation
                    ,
                    C_IND_CERTIFICATE = IndCertificate
                    ,
                    C_S_CATEGORY_CODE = SCategoryCode
                    ,
                    C_IND_ADDRESS_Chinese = chinessAddess
                    ,
                    C_IND_ADDRESS_English = englishAddress
                    ,
                    dateIndRetentionApplcationRet = IndCertificate.RETENTION_APPLICATION_DATE
                    ,
                    dateIndRestorationApplicationRes = IndCertificate.RESTORATION_APPLICATION_DATE
                    ,
                    IndNewRegExtDate = IndCertificate.EXTENDED_DATE
                    ,
                    IndRetExtDate = IndCertificate.EXTENDED_DATE
                    ,
                    IndResExtDate = IndCertificate.EXTENDED_DATE
                    ,
                    CERTLISTs = CertiList
                    ,
                    BS_ADDRESS_ENG = chinessAddess
                    ,
                    BS_ADDRESS_CHI = englishAddress
                    ,
                    SelectedCertiUUID = IndCertificate.UUID
                    ,
                    SelectedCertificate = IndCertificate
                };
            }
        }
        public UpdateAppStatusDisplayModel CalDate(string selectedType,string registrationDate, string NewRegValPerList, string RetValPerList, string ResValPerList, UpdateAppStatusDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (selectedType == "New Registration") {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_COMP_APPLICATION.EXTEND_DATE = null;
                    }
                    if ((model.C_COMP_APPLICATION.REGISTRATION_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (NewRegValPerList == "3")
                        //{
                        //    model.NewRegExtDate = model.C_COMP_APPLICATION.REGISTRATION_DATE.Value.AddYears(3);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.NewRegExtDate;
                        //}
                        //if (NewRegValPerList == "5")
                        //{
                        //    model.NewRegExtDate = model.C_COMP_APPLICATION.REGISTRATION_DATE.Value.AddYears(5);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.NewRegExtDate;
                        //}

                        if(!string.IsNullOrEmpty(registrationDate))
                        {
                            model.NewRegExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.NewRegExtDate;
                        }
                        else
                        {
                            model.NewRegExtDate = model.C_COMP_APPLICATION.REGISTRATION_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.NewRegExtDate;
                        }

                    }
                } else if (selectedType == "Retention")
                {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_COMP_APPLICATION.EXTEND_DATE = null;
                        if (model.C_COMP_APPLICATION.RESTORATION_APPLICATION_DATE != null)
                        {
                            model.dateRestorationApplicationRes = model.C_COMP_APPLICATION.RESTORATION_APPLICATION_DATE;
                        }
                        else
                        {
                            model.dateRestorationApplicationRes = model.C_COMP_APPLICATION.APPLICATION_DATE;
                        }

                    }
                    if ((model.C_COMP_APPLICATION.RETENTION_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (RetValPerList == "3")
                        //{
                        //    model.RetExtDate = model.C_COMP_APPLICATION.RETENTION_DATE.Value.AddYears(3);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.RetExtDate;
                        //}
                        //if (RetValPerList == "5")
                        //{
                        //    model.RetExtDate = model.C_COMP_APPLICATION.RETENTION_DATE.Value.AddYears(5);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.RetExtDate;
                        //}
                        if (!string.IsNullOrEmpty(registrationDate))
                        {
                            model.RetExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.RetExtDate;
                        }
                        else
                        {
                            model.RetExtDate = model.C_COMP_APPLICATION.RETENTION_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.RetExtDate;
                        }
                    }
                    if (model.C_COMP_APPLICATION.RETENTION_APPLICATION_DATE != null)
                    {
                        model.dateRetentionApplcationRet = model.C_COMP_APPLICATION.RETENTION_APPLICATION_DATE;
                    }
                    else
                    {
                        model.dateRetentionApplcationRet = model.C_COMP_APPLICATION.APPLICATION_DATE;
                    }
                }
                else if (selectedType == "Restoration")
                {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_COMP_APPLICATION.EXTEND_DATE = null;
                        if (model.C_COMP_APPLICATION.RETENTION_APPLICATION_DATE != null)
                        {
                            model.dateRetentionApplcationRet = model.C_COMP_APPLICATION.RETENTION_APPLICATION_DATE;
                        }
                        else
                        {
                            model.dateRetentionApplcationRet = model.C_COMP_APPLICATION.APPLICATION_DATE;
                        }
                    }
                    if ((model.C_COMP_APPLICATION.RESTORE_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (ResValPerList == "3")
                        //{
                        //    model.ResExtDate = model.C_COMP_APPLICATION.RESTORE_DATE.Value.AddYears(3);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.ResExtDate;
                        //}
                        //if (ResValPerList == "5")
                        //{
                        //    model.ResExtDate = model.C_COMP_APPLICATION.RESTORE_DATE.Value.AddYears(5);
                        //    model.C_COMP_APPLICATION.EXTEND_DATE = model.ResExtDate;
                        //}
                        if (!string.IsNullOrEmpty(registrationDate))
                        {
                            model.ResExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.ResExtDate;
                        }
                        else
                        {
                            model.ResExtDate = model.C_COMP_APPLICATION.RESTORE_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_COMP_APPLICATION.EXTEND_DATE = model.ResExtDate;
                        }
                    }
                    if (model.C_COMP_APPLICATION.RESTORATION_APPLICATION_DATE != null)
                    {
                        model.dateRestorationApplicationRes = model.C_COMP_APPLICATION.RESTORATION_APPLICATION_DATE;
                    }
                    else
                    {
                        model.dateRestorationApplicationRes = model.C_COMP_APPLICATION.APPLICATION_DATE;
                    }
                }
                return model;
            }
        }
        //Ind Cal
        public UpdateAppStatusDisplayModel CalIndDate(string selectedType,string registrationDate , string NewRegValPerList, string RetValPerList, string ResValPerList, UpdateAppStatusDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (selectedType == "New Registration")
                {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_IND_CERTIFICATE.EXTENDED_DATE = null;
                    }
                    if ((model.C_IND_CERTIFICATE.REGISTRATION_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (NewRegValPerList == "3")
                        //{
                        //    model.IndNewRegExtDate = model.C_IND_CERTIFICATE.REGISTRATION_DATE.Value.AddYears(3);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndNewRegExtDate;
                        //}
                        //if (NewRegValPerList == "5")
                        //{
                        //    model.IndNewRegExtDate = model.C_IND_CERTIFICATE.REGISTRATION_DATE.Value.AddYears(5);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndNewRegExtDate;
                        //}
                        if(!string.IsNullOrEmpty(registrationDate))
                        {
                            model.IndNewRegExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndNewRegExtDate;
                        }
                        else
                        {
                            model.IndNewRegExtDate = model.C_IND_CERTIFICATE.REGISTRATION_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndNewRegExtDate;
                        }
                       
                    }
                }
                else if (selectedType == "Retention")
                {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_IND_CERTIFICATE.EXTENDED_DATE = null;
                        if (model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE != null)
                        {
                            model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE;
                        }
                        else
                        {
                            model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                        }

                    }
                    if ((model.C_IND_CERTIFICATE.RETENTION_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (NewRegValPerList == "3")
                        //{
                        //    model.IndRetExtDate = model.C_IND_CERTIFICATE.RETENTION_DATE.Value.AddYears(3);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndRetExtDate;
                        //}
                        //if (NewRegValPerList == "5")
                        //{
                        //    model.IndRetExtDate = model.C_IND_CERTIFICATE.RETENTION_DATE.Value.AddYears(5);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndRetExtDate;
                        //}
                        if (!string.IsNullOrEmpty(registrationDate))
                        {
                            model.IndRetExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndRetExtDate;
                        }
                        else
                        {
                            model.IndRetExtDate = model.C_IND_CERTIFICATE.RETENTION_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndRetExtDate;
                        }
                    }
                    if (model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE != null)
                    {
                        model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE;
                    }
                    else
                    {
                        model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                    }
                }
                else if (selectedType == "Restoration")
                {
                    if (NewRegValPerList == "- Select -")
                    {
                        model.C_IND_CERTIFICATE.EXTENDED_DATE = null;
                        if (model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE != null)
                        {
                            model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE;
                        }
                        else
                        {
                            model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                        }
                    }
                    if ((model.C_IND_CERTIFICATE.RESTORE_DATE != null || !string.IsNullOrEmpty(registrationDate)) && NewRegValPerList != "- Select -")
                    {
                        //if (ResValPerList == "3")
                        //{
                        //    model.IndResExtDate = model.C_IND_CERTIFICATE.RESTORE_DATE.Value.AddYears(3);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndResExtDate;
                        //}
                        //if (ResValPerList == "5")
                        //{
                        //    model.IndResExtDate = model.C_IND_CERTIFICATE.RESTORE_DATE.Value.AddYears(5);
                        //    model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndResExtDate;
                        //}
                        if (!string.IsNullOrEmpty(registrationDate))
                        {
                            model.IndResExtDate = Convert.ToDateTime(registrationDate).AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndResExtDate;
                        }
                        else
                        {
                            model.IndResExtDate = model.C_IND_CERTIFICATE.RESTORE_DATE.Value.AddYears(Convert.ToInt32(NewRegValPerList));
                            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.IndResExtDate;
                        }
                    }
                    if (model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE != null)
                    {
                        model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE;
                    }
                    else
                    {
                        model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                    }
                }
                //else if (selectedType == "QP Card Application")
                //{
                //    if (ResValPerList == "- Select -")
                //    {
                //        model.C_IND_CERTIFICATE.EXTENDED_DATE = null;
                //        if (model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE != null)
                //        {
                //            model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.RETENTION_APPLICATION_DATE;
                //        }
                //        else
                //        {
                //            model.dateIndRetentionApplcationRet = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                //        }
                //    }
                //    if (model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE != null && ResValPerList != "- Select -")
                //    {
                //        if (ResValPerList == "3")
                //        {
                //            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE.Value.AddYears(3);
                //        }
                //        if (ResValPerList == "5")
                //        {
                //            model.C_IND_CERTIFICATE.EXTENDED_DATE = model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE.Value.AddYears(5);
                //        }
                //    }
                //    if (model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE != null)
                //    {
                //        model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.RESTORATION_APPLICATION_DATE;
                //    }
                //    else
                //    {
                //        model.dateIndRestorationApplicationRes = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                //    }
                //}

                return model;
            }
        }
        public string ExportUASforPAandMWIAReport(UpdateAppStatusSearchModel model, string RegType)
        {
            model.Query = SearchUAS_q;
            model.QueryWhere = SearchUASforPAandMWIA_whereQ(model, RegType);
            return model.Export("DataExport");
        }
        public UpdateAppStatusSearchModel SearchUASforPAandMWIA(UpdateAppStatusSearchModel model, string RegType)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchUAS_q;  //need to change query after all
            model.QueryWhere = SearchUASforPAandMWIA_whereQ(model, RegType);
            model.Search();
            return model;
        }
        private string SearchUASforPAandMWIA_whereQ(UpdateAppStatusSearchModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND ind_app.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND (upper(IND_APP.FILE_REFERENCE_NO) LIKE :FileRef)";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.SURNAME) LIKE :SurnName)";
                model.QueryParameters.Add("SurnName", "%" + model.SurName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.CHINESE_NAME) LIKE :ChiName)";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APP.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APP.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            return whereQ;
        }
        public void SaveComp_status(UpdateAppStatusDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var comp = (from comp_app in db.C_COMP_APPLICATION
                            where comp_app.UUID == model.C_COMP_APPLICATION.UUID
                            select comp_app).FirstOrDefault();
                comp.APPLICATION_DATE = model.C_COMP_APPLICATION.APPLICATION_DATE;
                comp.APPLICATION_FORM_ID = model.C_COMP_APPLICATION.APPLICATION_FORM_ID;
                comp.PERIOD_OF_VALIDITY_ID = model.C_COMP_APPLICATION.PERIOD_OF_VALIDITY_ID;
                comp.REGISTRATION_DATE = model.C_COMP_APPLICATION.REGISTRATION_DATE;
                comp.GAZETTE_DATE = model.C_COMP_APPLICATION.GAZETTE_DATE;
                comp.APPROVAL_DATE = model.C_COMP_APPLICATION.APPROVAL_DATE;
                //comp.EXTEND_DATE = model.C_COMP_APPLICATION.EXTEND_DATE;

                if (model.DateType == "1")
                {
                    comp.EXTEND_DATE = model.NewRegExtDate;
                }
                else if (model.DateType == "2")
                {
                    comp.EXTEND_DATE = model.RetExtDate;
                }
                else if (model.DateType == "3")
                {
                    comp.EXTEND_DATE = model.ResExtDate;
                }
                if (model.isConfirmMail is true)
                {
                    comp.DOCUMENT_MAIL_DATE = model.C_COMP_APPLICATION.DOCUMENT_MAIL_DATE;
                }
                comp.RETENTION_DATE = model.C_COMP_APPLICATION.RETENTION_DATE;
                comp.VETTING_DATE = model.DateVetting;
                comp.RESTORE_DATE = model.C_COMP_APPLICATION.RESTORE_DATE;
                comp.CERTIFICATION_NO = model.C_COMP_APPLICATION.CERTIFICATION_NO;
                comp.REMOVAL_DATE = model.C_COMP_APPLICATION.REMOVAL_DATE;
                comp.RETENTION_APPLICATION_DATE = model.dateRetentionApplcationRet;
                comp.RESTORATION_APPLICATION_DATE = model.dateRestorationApplicationRes;
                String result = "";
                if (model.ProcessPage == "2")
                {
                    if (model.missingItemRet is null)
                    {
                        comp.MISS_DOCUMENT_TYPE = null;
                    }
                    else if (model.missingItemRet.Count >= 1)
                    {
                        if (model.missingItemRet.Contains("0"))
                        {

                            result = "0";
                        }
                        else
                        {
                            foreach (String smi in model.missingItemRet)
                            {
                                if (String.IsNullOrEmpty(result))
                                {
                                    result += smi;
                                }
                                else
                                {
                                    result += "," + smi;
                                }
                            }
                        }
              
                        comp.MISS_DOCUMENT_TYPE = result;
                    }

                }
                else
                {

                    if (model.missingItem is null)
                    {
                        comp.MISS_DOCUMENT_TYPE = null;
                    }
                    else if (model.missingItem.Count >= 1)
                    {
                        if (model.missingItem.Contains("0"))
                        {

                            result = "0";
                        }
                        else
                        {
                            foreach (String smi in model.missingItem)
                            {
                                if (String.IsNullOrEmpty(result))
                                {
                                    result += smi;
                                }
                                else
                                {
                                    result += "," + smi;
                                }
                            }
                        }
                     
                        comp.MISS_DOCUMENT_TYPE = result;
                    }

                    

                }
                db.SaveChanges();
            }
        }
        public void SaveInd_status(UpdateAppStatusDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //var ind = (from ind_cert in db.C_IND_CERTIFICATE
                //           join ind_app in db.C_IND_APPLICATION on ind_cert.MASTER_ID equals ind_app.UUID
                //           where ind_app.UUID == model.C_IND_APPLICATION.UUID
                //           select ind_cert).FirstOrDefault();

                var ind = db.C_IND_CERTIFICATE
                    .Where(x => x.C_IND_APPLICATION.UUID == model.C_IND_APPLICATION.UUID && x.C_S_CATEGORY_CODE.CODE == model.CatCode)
                    .Include(x => x.C_IND_APPLICATION)
                    .Include(x => x.C_S_CATEGORY_CODE)
                    .FirstOrDefault();

                ind.APPLICATION_DATE = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                ind.APPLICATION_FORM_ID = model.C_IND_CERTIFICATE.APPLICATION_FORM_ID;
                ind.PERIOD_OF_VALIDITY_ID = model.C_IND_CERTIFICATE.PERIOD_OF_VALIDITY_ID;
                ind.REGISTRATION_DATE = model.C_IND_CERTIFICATE.REGISTRATION_DATE;
                ind.GAZETTE_DATE = model.C_IND_CERTIFICATE.GAZETTE_DATE;
                ind.APPROVAL_DATE = model.C_IND_CERTIFICATE.APPROVAL_DATE;
                ind.REMOVAL_DATE = model.C_IND_CERTIFICATE.REMOVAL_DATE;
                ind.RETENTION_APPLICATION_DATE = model.dateIndRetentionApplcationRet;
                ind.RESTORATION_APPLICATION_DATE = model.dateIndRestorationApplicationRes;
                if (model.isConfirmMail is true)
                {
                    ind.DOCUMENT_MAIL_DATE = model.C_IND_CERTIFICATE.DOCUMENT_MAIL_DATE;
                }
                ind.RETENTION_DATE = model.C_IND_CERTIFICATE.RETENTION_DATE;
                ind.VETTING_DATE = model.IndDateVetting;
                ind.RESTORE_DATE = model.C_IND_CERTIFICATE.RESTORE_DATE;
                ind.CERTIFICATION_NO = model.C_IND_CERTIFICATE.CERTIFICATION_NO;
                if (model.DateType == "1")
                {
                    ind.EXTENDED_DATE = model.IndNewRegExtDate;
                }
                else if (model.DateType == "2")
                {
                    ind.EXTENDED_DATE = model.IndRetExtDate;
                }
                else if (model.DateType == "3")
                {
                    ind.EXTENDED_DATE = model.IndResExtDate;
                }
                String result = "";
                if (model.missingItem is null)
                {
                    ind.MISS_DOCUMENT_TYPE = null;
                }
                else if (model.missingItem.Count >= 1)
                {
                    foreach (String smi in model.missingItem)
                    {
                        if (String.IsNullOrEmpty(result))
                        {
                            result += smi;
                        }
                        else
                        {
                            result += "," + smi;
                        }
                    }
                    ind.MISS_DOCUMENT_TYPE = result;
                }
                if (model.SelectedCertiUUID != null)
                {
                    var queryCert = db.C_IND_CERTIFICATE.Where(x => x.UUID == model.SelectedCertiUUID).FirstOrDefault();
                    queryCert.CARD_APP_DATE = model.SelectedCertificate.CARD_APP_DATE;
                    queryCert.CARD_EXPIRY_DATE = model.SelectedCertificate.CARD_EXPIRY_DATE;
                    queryCert.CARD_ISSUE_DATE = model.SelectedCertificate.CARD_ISSUE_DATE;
                    queryCert.CARD_RETURN_DATE = model.SelectedCertificate.CARD_RETURN_DATE;
                    queryCert.CARD_SERIAL_NO = model.SelectedCertificate.CARD_SERIAL_NO;
                }
            
                ind.C_IND_APPLICATION.CHINESE_BS_CARE_OF = model.C_IND_APPLICATION.CHINESE_BS_CARE_OF;
                ind.C_IND_APPLICATION.ENGLISH_BS_CARE_OF = model.C_IND_APPLICATION.ENGLISH_BS_CARE_OF;
              
                ind.QP_ADDRESS_E1 = model.BS_ADDRESS_ENG.ADDRESS_LINE1;
                ind.QP_ADDRESS_E2 = model.BS_ADDRESS_ENG.ADDRESS_LINE2;
                ind.QP_ADDRESS_E3 = model.BS_ADDRESS_ENG.ADDRESS_LINE3;
                ind.QP_ADDRESS_E4 = model.BS_ADDRESS_ENG.ADDRESS_LINE4;
                ind.QP_ADDRESS_E5 = model.BS_ADDRESS_ENG.ADDRESS_LINE5;

                ind.QP_ADDRESS_C1 = model.BS_ADDRESS_CHI.ADDRESS_LINE1;
                ind.QP_ADDRESS_C2 = model.BS_ADDRESS_CHI.ADDRESS_LINE2;
                ind.QP_ADDRESS_C3 = model.BS_ADDRESS_CHI.ADDRESS_LINE3;
                ind.QP_ADDRESS_C4 = model.BS_ADDRESS_CHI.ADDRESS_LINE4;
                ind.QP_ADDRESS_C5 = model.BS_ADDRESS_CHI.ADDRESS_LINE5;

                db.SaveChanges();
            }
        }
        #region Qualification
        public List<QualifcationDisplayListModel> GetQualificationByIndUuid(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                List<QualifcationDisplayListModel> list = (from Quali in db.C_IND_QUALIFICATION
                                                           join cat in db.C_S_CATEGORY_CODE on Quali.CATEGORY_ID equals cat.UUID
                                                           join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
                                                           join svprbDescrption in db.C_S_SYSTEM_VALUE on Quali.PRB_ID equals svprbDescrption.UUID
                                                           where Quali.MASTER_ID == uuid
                                                           select new QualifcationDisplayListModel()
                                                           {
                                                               UUID = Quali.UUID,
                                                               PRB = svprbDescrption.ENGLISH_DESCRIPTION,
                                                               QUALIFICATIONCODE = cat.CODE,
                                                               REGISTRATIONNO = Quali.REGISTRATION_NUMBER,
                                                               EXPIRYDATE = Quali.EXPIRY_DATE,
                                                               
                                                           }).ToList();

                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
                {
                    List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;



                    for (int i = 0; i < v.Count; i++)
                    {

                        list.Remove(list.Find(x => x.UUID == v[i].UUID));
                        
                    
                        list.Add(new QualifcationDisplayListModel()
                        {
                            UUID = v[i].UUID == null ? null : v[i].UUID,
                            PRB = v[i].PRB == null ? null : SystemListUtil.GetSVByUUID(v[i].PRB).ENGLISH_DESCRIPTION,
                            QUALIFICATIONCODE = v[i].QUALIFICATIONCODE == null ? null : SystemListUtil.GetCatCodeByUUID(v[i].QUALIFICATIONCODE).CODE,
                            QUALIFICATIONCODETYPE = v[i].QUALIFICATIONCODETYPE == null ? null : v[i].QUALIFICATIONCODETYPE,
                            REGISTRATIONNO = v[i].REGISTRATIONNO == null ? null : v[i].REGISTRATIONNO,
                            EXPIRYDATE = v[i].EXPIRYDATE == null ? null : v[i].EXPIRYDATE,
                        });
                    }
                }
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_QUALIFICATION))
                {
                    List<string> deleteList = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_QUALIFICATION] as List<string>;

                    list = list.Where(o => !deleteList.Contains(o.UUID)).ToList();
                }

                foreach (var item in list)
                {
                    if (item.EXPIRYDATE != null)
                    {
                        item.EXPIRYDATESTRING = item.EXPIRYDATE.Value.ToShortDateString();
                    }

                }

                return list;
            }
        }
        //public ServiceResult AddNewDraftMWItem(IndMWItemDisplayModel model)
        //{

        //    if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_NEWMWITEM))
        //    {
        //        SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_NEWMWITEM, new List<MWItemDetailListModel>());
        //    }
        //    List<string> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_NEWMWITEM] as List<string>;
        //    Dictionary<string,string> x = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_NEWMWITEM] as Dictionary<string, string>;
        //    v.AddRange(model.NewSelectedMWitem);
        //    foreach (var item in model.NewSelectedMWitemSupportedBy)
        //    {
        //        x.Add(item.Key,item.Value);
        //    }



        //    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

        //}

        public ServiceResult AddDraftMWItemTest(MWItemDetailListModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                {
                    SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_MWITEM, new List<MWItemDetailListModel>());
                }

                List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;

                if (model.m_NewSelectedMWitem != null && model.m_NewSelectedMWitemSupportedBy != null)
                {
                    if (model.m_NewSelectedMWitem.Count() != model.m_NewSelectedMWitemSupportedBy.Count())
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The Tray Item is not valid" } };

                    }
                    foreach (var item in model.m_NewSelectedMWitem)
                    {
                        var result = model.m_NewSelectedMWitemSupportedBy.ContainsKey(item);
                        if (result == false)
                        {
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The Tray Item is not valid" } };

                        }
                    }
                }
                else
                {
                    if (model.m_MWItemSaveVersion == "New")
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The Tray Item is not valid" } };

                    }
                    else {

                       
                    }
                }

                if (model.m_UUID == null)
                {
                    model.m_UUID = Guid.NewGuid().ToString().Replace("-", "");
                    model.m_Created_Date = DateTime.Now;

                    if (model.m_MWItemSaveVersion == "New")
                    {

                        model.isTray = true;
                        var ApplyTemp = model.m_MWItemDeleteByApplicant;
                        model.m_Apply_Item = ApplyTemp;
                        List<string> mwitemList = new List<string>() ;
                        foreach (var item in model.m_NewSelectedMWitem)
                        {
                            var m = db.C_S_MW_IND_CAPA_MAIN.Where(x => x.UUID == item).Include(x=>x.C_S_MW_IND_CAPA).FirstOrDefault();
                            var mwitem = m.C_S_MW_IND_CAPA.ITEM_VALUE ;
                            mwitem = mwitem.Replace(" ","" );
                            var temp = mwitem.Split(',');
                            
                            mwitemList.AddRange(temp);

                        }
                       
                       mwitemList =  mwitemList.Distinct().OrderBy(x=> int.Parse(x.Substring(2))).ToList();
                        if (ApplyTemp != null)
                        {
                            ApplyTemp = ApplyTemp.Replace(" ", "");
                            var ToBeRemove = ApplyTemp.Split(',');

                            foreach (var item in ToBeRemove)
                            {
                               bool result =  mwitemList.Remove(item);
                                if (result == false)
                                {
                                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The target delete Item is not valid, please check." } };
                                }
                            }
                        }

                        model.NewVerApprovedItems = new List<string>();
                        model.NewVerApprovedItems.AddRange(mwitemList);
                        //if (!string.IsNullOrWhiteSpace(model.m_MWItemDeleteByApplicant))
                        //{
                        //    var splitedMWitem = model.m_MWItemDeleteByApplicant.Split(',');
                        //    foreach (var item in splitedMWitem)
                        //    {
                        //        model.NewVerApprovedItems.Remove(item);
                        //    }
                       
                        //}
                        foreach (var item in mwitemList)
                        {

                          
                           model.m_Approved_Item += item + ", ";

                        }
                        model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 2);

                       

                    }
                    else
                    {
                        if (model.ApplyItems != null)
                        {
                            foreach (var item in model.ApplyItems)
                            {
                                model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";

                            }
                            model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                        }

                        if (model.ApprovedItems != null)
                        {
                            foreach (var item in model.ApprovedItems)
                            {
                                model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                            }
                            model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                        }

                    }
                    v.Add(model);

                }
                else
                {
                    bool inDraftSession = false;
                    for (int i = 0; i < v.Count(); i++)
                    {
                        if (v[i].m_UUID == model.m_UUID)
                        {
                            if (model.ApplyItems != null)
                            {
                                foreach (var item in model.ApplyItems)
                                {
                                    model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                                }
                                model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                            }

                            if (model.ApprovedItems != null)
                            {
                                foreach (var item in model.ApprovedItems)
                                {
                                    model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                                }
                                model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                            }
                            
                            //model.m_Created_Date = DateTime.Now;
                            model.NewVerApprovedItems = v[i].NewVerApprovedItems;
                            v[i] = model;
                            if (model.m_MWItemSaveVersion == "New")
                            {
                                model.isTray = true;
                              

                            }
                                inDraftSession = true;
                           
                        }

                    }
                    if (inDraftSession == false)
                    {
                        if (model.m_MWItemSaveVersion == "New")
                        {
                            
                               
                            
                            model.isTray = true;
                            var ApplyTemp = model.m_MWItemDeleteByApplicant;
                            model.m_Apply_Item = ApplyTemp;
                            List<string> mwitemList = new List<string>();
                            foreach (var item in model.m_NewSelectedMWitem)
                            {
                                var mwitem = db.C_S_MW_IND_CAPA_MAIN.Where(x => x.UUID == item).FirstOrDefault().C_S_MW_IND_CAPA.ITEM_VALUE;
                                if (string.IsNullOrWhiteSpace(mwitem))
                                {
                                }
                                else
                                {
                                    mwitem = mwitem.Replace(" ", "");
                                    var temp = mwitem.Split(',');

                                    mwitemList.AddRange(temp);
                                }
                          

                            }

                            mwitemList = mwitemList.Distinct().OrderBy(x => int.Parse(x.Substring(2))).ToList();
                            if (ApplyTemp != null)
                            {
                                ApplyTemp = ApplyTemp.Replace(" ", "");
                                var ToBeRemove = ApplyTemp.Split(',');

                                foreach (var item in ToBeRemove)
                                {
                                    //mwitemList.Remove(item);
                                    bool result = mwitemList.Remove(item);
                                    if (result == false)
                                    {
                                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "The target delete Item is not valid, please check." } };
                                    }
                                }
                            }
                       
                            model.NewVerApprovedItems = new List<string>();
                            model.NewVerApprovedItems.AddRange(mwitemList);
                            foreach (var item in mwitemList)
                            {


                                model.m_Approved_Item += item + ", ";

                            }
                            model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 2);



                        }
                        else
                        {
                            if (model.ApplyItems != null)
                        {
                            foreach (var item in model.ApplyItems)
                            {
                                model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                            }
                            model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                        }

                        if (model.ApprovedItems != null)
                        {
                            foreach (var item in model.ApprovedItems)
                            {
                                model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                            }
                            model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                        }
                    }
                        v.Add(model);
                    }
                }

            }

            




            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }



        public ServiceResult AddDraftMWItem(MWItemDetailListModel model)
        {
            

            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_MWITEM, new List<MWItemDetailListModel>());
            }

            List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;
            if (model.m_UUID == null)
            {
                model.m_UUID = Guid.NewGuid().ToString().Replace("-", "");
                model.m_Created_Date = DateTime.Now;
         
                if (model.ApplyItems != null)
                {
                    foreach (var item in model.ApplyItems)
                    {
                        model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                 
                    }
                    model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                }

                if (model.ApprovedItems != null)
                {
                    foreach (var item in model.ApprovedItems)
                    {
                        model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                    }
                    model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                }
                v.Add(model);
            }
            else
            {
                bool inDraftSession = false;
                for (int i = 0; i < v.Count(); i++)
                {
                    if (v[i].m_UUID == model.m_UUID)
                    {
                        if (model.ApplyItems != null)
                        {
                            foreach (var item in model.ApplyItems)
                            {
                                model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                            }
                            model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                        }

                        if (model.ApprovedItems != null)
                        {
                            foreach (var item in model.ApprovedItems)
                            {
                                model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                            }
                            model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                        }
                        v[i] = model;

                        inDraftSession = true;
                    }

                }
                if (inDraftSession == false)
                {

                    if (model.ApplyItems != null)
                    {
                        foreach (var item in model.ApplyItems)
                        {
                            model.m_Apply_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                        }
                        model.m_Apply_Item = model.m_Apply_Item.Substring(0, model.m_Apply_Item.Length - 1);

                    }

                    if (model.ApprovedItems != null)
                    {
                        foreach (var item in model.ApprovedItems)
                        {
                            model.m_Approved_Item += SystemListUtil.GetSVByUUID(item.Key).CODE.Substring(4) + ",";
                        }
                        model.m_Approved_Item = model.m_Approved_Item.Substring(0, model.m_Approved_Item.Length - 1);
                    }
                    v.Add(model);
                }
            }




            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult AddDraftQualification(QualifcationDisplayListModel model)
        {
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_QUALIFICATION, new List<QualifcationDisplayListModel>());
            }

            List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
            if (model.UUID == null)
            {
                model.UUID = Guid.NewGuid().ToString().Replace("-", "");
                v.Add(model);
            }
            else
            {


                bool inDraftSession = false;
                for (int i = 0; i < v.Count(); i++)
                {
                    if (v[i].UUID == model.UUID)
                    {
                        v[i] = model;
                        //v[i].PRB = model.PRB;
                        //v[i].QUALIFICATIONCODE = model.QUALIFICATIONCODE;
                        //v[i].QUALIFICATIONCODETYPE = model.QUALIFICATIONCODETYPE;
                        //v[i].SelectedCatCodeDetail = model.SelectedCatCodeDetail;
                        //v[i].REGISTRATIONNO = model.REGISTRATIONNO;
                        //v[i].EXPIRYDATE = model.EXPIRYDATE;
                        inDraftSession = true;
                    }

                }
                if (inDraftSession == false)
                {
                    v.Add(model);
                }


            }

            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public ServiceResult DeleteQualification(string id)
        {
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_QUALIFICATION))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DELETE_KEY_QUALIFICATION, new List<string>());
            }
            List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_QUALIFICATION] as List<string>;
            v.Add(id);
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_QUALIFICATION, new List<QualifcationDisplayListModel>());
            }

            List<QualifcationDisplayListModel> x = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;

            var ToRemoveDraft = x.Where(y => y.UUID == id);
            if (ToRemoveDraft != null)
                x.Remove(ToRemoveDraft.FirstOrDefault());


            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult DeleteMWItem(string id)
        {
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_MWITEM))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DELETE_KEY_MWITEM, new List<string>());
            }
            List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_MWITEM] as List<string>;
            v.Add(id);
            //if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
            //{
            //    SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_MWITEM, new List<MWItemDetailListModel>());
            //}
            if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
            {
                List<MWItemDetailListModel> x = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;

                var ToRemoveDraft = x.Where(y => y.m_UUID == id);
                if (ToRemoveDraft != null)
                    x.Remove(ToRemoveDraft.FirstOrDefault());
            }



            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public QualifcationDisplayListModel GetQualificationByUUID(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                QualifcationDisplayListModel qc = new QualifcationDisplayListModel();
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
                {
                    List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
                    qc = (from temp in v
                          where temp.UUID == uuid
                          select temp).FirstOrDefault();

                }
                if (qc.UUID == null)
                {
                    qc = (from Quali in db.C_IND_QUALIFICATION

                          join cat in db.C_S_CATEGORY_CODE on Quali.CATEGORY_ID equals cat.UUID
                          join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
                          join svprbDescrption in db.C_S_SYSTEM_VALUE on Quali.PRB_ID equals svprbDescrption.UUID
                          where Quali.UUID == uuid
                          select new QualifcationDisplayListModel()
                          {
                              UUID = Quali.UUID,
                              PRB = Quali.PRB_ID,
                              QUALIFICATIONCODE = Quali.CATEGORY_ID,
                              QUALIFICATIONCODETYPE = Quali.QUALIFICATION_TYPE,
                              REGISTRATIONNO = Quali.REGISTRATION_NUMBER,
                              EXPIRYDATE = Quali.EXPIRY_DATE
                          }).FirstOrDefault();

                    var qcd = from Quali in db.C_IND_QUALIFICATION
                              join QualiDetail in db.C_IND_QUALIFICATION_DETAIL on Quali.UUID equals QualiDetail.IND_QUALIFICATION_ID
                              where Quali.UUID == uuid
                              select QualiDetail.S_CATEGORY_CODE_DETAIL_ID;
                    qc.SelectedCatCodeDetail = qcd.ToList();
                }

                return qc;
            }
        }

        #endregion

        #region MWItemList
        public IndMWItemDisplayModel GetMWItemListByIndUUID(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                bool m_TrayDisplay = false;
                var queryMWItemMaster = db.C_IND_APPLICATION_MW_ITEM_MASTER.Where(x => x.MASTER_ID == id).OrderBy(x => x.CREATED_DATE);
                
                List<MWItemDetailListModel> mlist = new List<MWItemDetailListModel>();

                foreach (var item in queryMWItemMaster)
                {
                    MWItemDetailListModel m = new MWItemDetailListModel();
                    m.m_UUID = item.UUID;
                    m.m_Master_ID = item.MASTER_ID;
                    m.m_STATUS_CODE = item.STATUS_CODE;
                    m.m_APPROVED_DATE = item.APPROVED_DATE;
                    m.m_APPROVED_BY = item.APPROVED_BY;
                    m.m_APPLICATION_FORM_ID = item.APPLICATION_FORM_ID;
                    m.m_APPLICATION_DATE = item.APPLICATION_DATE;
                    m.m_Created_Date = item.CREATED_DATE;
                    
                    if (item.SV_MWITEM_VERSION_ID == null)
                    {
                        m_TrayDisplay = m.isTray = false;
                    }
                    else
                    {
                         m.isTray = db.C_S_SYSTEM_VALUE.Where(x => x.UUID == item.SV_MWITEM_VERSION_ID).FirstOrDefault().CODE == "3" ? true : false;
                        string formCode = SystemListUtil.GetSVByUUID(item.APPLICATION_FORM_ID).CODE;
                        if (m.isTray == false && (formCode == "BA26" || formCode == "BA26A" || formCode == "BA26B"))
                        {
                            m_TrayDisplay = false;
                        }
                        else
                        {
                            m_TrayDisplay = true;
                        }
                    }
                    m.m_Apply_Item = item.ITEM_REMOVE;
                    m.m_MWItemDeleteByApplicant = item.ITEM_REMOVE;
                    mlist.Add(m);
                }



                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                {
                    List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;



                    for (int i = 0; i < v.Count; i++)
                    {
                        bool masterExist = false;
                        var existingMasterList = mlist.Find(x => x.m_UUID == v[i].m_UUID);
                        if (existingMasterList != null)
                            masterExist = true;

                        mlist.Add(new MWItemDetailListModel()
                        {
                            m_UUID = v[i].m_UUID == null ? null : v[i].m_UUID,
                            m_Master_ID = v[i].m_Master_ID == null ? null : v[i].m_Master_ID,
                            m_APPLICATION_DATE = v[i].m_APPLICATION_DATE == null ? null : v[i].m_APPLICATION_DATE,
                            m_APPROVED_BY = v[i].m_APPROVED_BY == null ? null : v[i].m_APPROVED_BY,
                            m_APPLICATION_FORM_ID = v[i].m_APPLICATION_FORM_ID == null ? null : v[i].m_APPLICATION_FORM_ID,
                            m_APPROVED_DATE = v[i].m_APPROVED_DATE == null ? null : v[i].m_APPROVED_DATE,
                            m_Created_Date = masterExist ? existingMasterList.m_Created_Date : v[i].m_Created_Date,
                            m_STATUS_CODE = v[i].m_STATUS_CODE == null ? null : v[i].m_STATUS_CODE,
                            m_Apply_Item = v[i].m_Apply_Item == null ? null : v[i].m_Apply_Item,
                            m_Approved_Item = v[i].m_Approved_Item == null ? null : v[i].m_Approved_Item,
                            isTray = v[i].isTray,

                        });
                        mlist.Remove(existingMasterList);


                    }
                }
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_MWITEM))
                {
                    List<string> deleteList = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_MWITEM] as List<string>;

                    mlist = mlist.Where(o => !deleteList.Contains(o.m_UUID)).ToList();
                }

                mlist = mlist.OrderBy(x => x.m_Created_Date).ToList();


            
                 








                return new IndMWItemDisplayModel
                {
                    IndApplication_UUID = id,
                    // C_IND_APPLICATION_MW_ITEM_MASTER_LIST = queryMWItemMaster.ToList(),
                    MWItemDetailList = mlist,
                  TrayDisplay = m_TrayDisplay,
                };
            }
        }
        public List<C_IND_APPLICATION_MW_ITEM_MASTER> GetMWItemMaster(string Master_ID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_IND_APPLICATION_MW_ITEM_MASTER.Where(x => x.MASTER_ID == Master_ID).OrderBy(x => x.CREATED_DATE).ToList();
            }
        }
        public IndMWItemDisplayModel GetMWItemListByUUID(string id, string masterID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_IND_APPLICATION_MW_ITEM_MASTER.
                    Where(y => y.UUID == id).FirstOrDefault();

                List<MWItemDetailListModel> mlist = new List<MWItemDetailListModel>();

                foreach (var item in GetMWItemMaster(masterID))
                {
                    MWItemDetailListModel m = new MWItemDetailListModel();
                    m.m_UUID = item.UUID;
                    m.m_STATUS_CODE = item.STATUS_CODE;
                    m.m_APPROVED_DATE = item.APPROVED_DATE;
                    m.m_APPROVED_BY = item.APPROVED_BY;
                    m.m_APPLICATION_FORM_ID = item.APPLICATION_FORM_ID;
                    m.m_APPLICATION_DATE = item.APPLICATION_DATE;
                    m.m_Created_Date = item.CREATED_DATE;
                    mlist.Add(m);
                }

                IndMWItemDisplayModel model = new IndMWItemDisplayModel();


                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                {
                    List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;
                    var q = v.Find(x => x.m_UUID == id);
                    model.SelectedMWItemDetail.m_UUID = q.m_UUID;
                    model.SelectedMWItemDetail.m_Master_ID = q.m_Master_ID;
                    model.SelectedMWItemDetail.m_APPLICATION_DATE = q.m_APPLICATION_DATE;
                    model.SelectedMWItemDetail.m_APPLICATION_FORM_ID = q.m_APPLICATION_FORM_ID;
                    model.SelectedMWItemDetail.m_APPROVED_BY = q.m_APPROVED_BY;
                    model.SelectedMWItemDetail.m_APPROVED_DATE = q.m_APPROVED_DATE;
                    model.SelectedMWItemDetail.m_STATUS_CODE = q.m_STATUS_CODE;
                    model.SelectedMWItemDetail.m_Created_Date = q.m_Created_Date;
                    model.SelectedMWItemDetail.SelectedApplyMWItem = new List<string>();
                    model.SelectedMWItemDetail.SelectedApprovedMWItem = new List<string>();
                    model.SelectedMWItemDetail.NewVerApprovedItems = q.NewVerApprovedItems;
                    model.SelectedMWItemDetail.m_MWItemDeleteByApplicant = q.m_MWItemDeleteByApplicant;
                    model.SelectedMWItemDetail.m_Approved_Item = q.m_Approved_Item;
                    if (q.ApplyItems != null)
                    {
                        foreach (var item in q.ApplyItems)
                        {
                            model.SelectedMWItemDetail.SelectedApplyMWItem.Add(item.Key + item.Value);
                        }
                    }
                    if (q.ApprovedItems != null)
                    {
                        foreach (var item in q.ApprovedItems)
                        {
                            model.SelectedMWItemDetail.SelectedApprovedMWItem.Add(item.Key + item.Value);
                        }

                    }
                

                    model.SelectedMWItemDetail.ApplyItems = q.ApplyItems;
                    model.SelectedMWItemDetail.ApprovedItems = q.ApprovedItems;

                }
                else if (query != null)
                {
                    model.SelectedMWItemDetail.m_UUID = query.UUID;
                    model.SelectedMWItemDetail.m_Master_ID = query.MASTER_ID;
                    model.SelectedMWItemDetail.m_APPLICATION_DATE = query.APPLICATION_DATE;
                    model.SelectedMWItemDetail.m_APPLICATION_FORM_ID = query.APPLICATION_FORM_ID;
                    model.SelectedMWItemDetail.m_APPROVED_BY = query.APPROVED_BY;
                    model.SelectedMWItemDetail.m_APPROVED_DATE = query.APPROVED_DATE;
                    model.SelectedMWItemDetail.m_STATUS_CODE = query.STATUS_CODE;
                    model.SelectedMWItemDetail.m_Created_Date = query.CREATED_DATE;
                    model.SelectedMWItemDetail.m_MWItemDeleteByApplicant = query.ITEM_REMOVE;
                }

                //if (query != null )
                //{
                //    model.SelectedMWItemDetail.m_UUID = query.UUID;
                //    model.SelectedMWItemDetail.m_Master_ID = query.MASTER_ID;
                //    model.SelectedMWItemDetail.m_APPLICATION_DATE = query.APPLICATION_DATE;
                //    model.SelectedMWItemDetail.m_APPLICATION_FORM_ID = query.APPLICATION_FORM_ID;
                //    model.SelectedMWItemDetail.m_APPROVED_BY = query.APPROVED_BY;
                //    model.SelectedMWItemDetail.m_APPROVED_DATE = query.APPROVED_DATE;
                //    model.SelectedMWItemDetail.m_STATUS_CODE = query.STATUS_CODE;
                //    model.SelectedMWItemDetail.m_Created_Date = query.CREATED_DATE;
                //}
                //else
                //{
                //    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                //    {
                //      List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;
                //      var q =   v.Find(x => x.m_UUID == id);
                //        model.SelectedMWItemDetail.m_UUID = q.m_UUID;
                //        model.SelectedMWItemDetail.m_Master_ID = q.m_Master_ID;
                //        model.SelectedMWItemDetail.m_APPLICATION_DATE = q.m_APPLICATION_DATE;
                //        model.SelectedMWItemDetail.m_APPLICATION_FORM_ID = q.m_APPLICATION_FORM_ID;
                //        model.SelectedMWItemDetail.m_APPROVED_BY = q.m_APPROVED_BY;
                //        model.SelectedMWItemDetail.m_APPROVED_DATE = q.m_APPROVED_DATE;
                //        model.SelectedMWItemDetail.m_STATUS_CODE = q.m_STATUS_CODE;
                //        model.SelectedMWItemDetail.m_Created_Date = q.m_Created_Date;
                //        model.SelectedMWItemDetail.SelectedApplyMWItem = new List<string>();
                //        model.SelectedMWItemDetail.SelectedApprovedMWItem = new List<string>();
                //        foreach (var item in q.ApplyItems)
                //        {
                //            model.SelectedMWItemDetail.SelectedApplyMWItem.Add(item.Key + item.Value);
                //        }
                //        foreach (var item in q.ApprovedItems)
                //        {
                //            model.SelectedMWItemDetail.SelectedApprovedMWItem.Add(item.Key + item.Value);
                //        }

                //        model.SelectedMWItemDetail.ApplyItems = q.ApplyItems;
                //        model.SelectedMWItemDetail.ApprovedItems = q.ApprovedItems;

                //    }

                //}
                return new IndMWItemDisplayModel
                {
                    // C_IND_APPLICATION_MW_ITEM_MASTER_LIST = queryMWItemMaster.ToList()
                    IndApplication_UUID = masterID,
                    SelectedMWItemDetail = model.SelectedMWItemDetail,
                    // SelectedMasterUUID          = query.MASTER_ID,
                    // SelectedApplicationDate     = query.APPLICATION_DATE,
                    // SelectedApplicationForm     = query.APPLICATION_FORM_ID,
                    // SelectedApprovedBy          =  query.APPROVED_BY,
                    // SelectedApproveDate         = query.APPROVED_DATE,


                    MWItemDetailList = mlist,
                };
            }

        }
        public void DeleteCompPMRecord(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_PROCESS_MONITOR compPM = db.C_COMP_PROCESS_MONITOR.Where(o => o.UUID == id).FirstOrDefault();
                db.C_COMP_PROCESS_MONITOR.Remove(compPM);
                db.SaveChanges();
            }
        }
        public void DeleteIndPMRecord(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_PROCESS_MONITOR compPM = db.C_IND_PROCESS_MONITOR.Where(o => o.UUID == id).FirstOrDefault();
                db.C_IND_PROCESS_MONITOR.Remove(compPM);
                db.SaveChanges();
            }
        }
        //Save
        public void SaveComp_PM(ProcessMonitorDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if(model.C_COMP_PROCESS_MONITOR!=null)
                if (model.C_COMP_PROCESS_MONITOR.UUID != null)
                {
                    C_COMP_PROCESS_MONITOR CompPM = db.C_COMP_PROCESS_MONITOR.Where(o => o.UUID == model.C_COMP_PROCESS_MONITOR.UUID).FirstOrDefault();
                    //APPLICATION_FORM_ID
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_CGA)
                    {
                        CompPM.APPLICATION_FORM_ID = model.CGCAppList;
                    }
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWCA)
                    {
                        CompPM.APPLICATION_FORM_ID = model.MWCAppList;
                    }

                    if (model.SelectedAppApplyType != null) {
                        string result = "";
                        foreach (string smi in model.SelectedAppApplyType)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result += smi;
                            }
                        }
                        CompPM.APPLY_STATUS = result;
                    }
                    //NEW UR
                    CompPM.PROCESS_MONITOR_TYPE = model.C_COMP_PROCESS_MONITOR.PROCESS_MONITOR_TYPE;
                    //CMW
                    if (model.AdditionASorTD == true)
                    {
                        CompPM.ADDITION_AUTH_SIGN = "on";
                    }
                    if (model.AdditionClass == true)
                    {
                        CompPM.ADDITION_CLASS = "on";
                    }
                    if (model.AdditionType == true)
                    {
                        CompPM.ADDITION_TYPE = "on";
                    }
                    CompPM.RESULT_LETTER_DATE = model.C_COMP_PROCESS_MONITOR.RESULT_LETTER_DATE;
                    //INTERVIEW_DATE
                    CompPM.INTERVIEW_DATE = model.C_COMP_PROCESS_MONITOR.INTERVIEW_DATE;

                    CompPM.INTERVIEW_RESULT_ID = model.InterResultID;
                    //Fast-Track
                    CompPM.ASSISTANT = model.C_COMP_PROCESS_MONITOR.ASSISTANT;

                    if (model.MonitorType == RegistrationConstant.PROCESS_MONITOR_FASK_TRACK) {
                        DateTime? FastTrack = model.C_COMP_PROCESS_MONITOR.RECEIVED_DATE;
                        CompPM.FAST_TRACK_DUE_28_DAYS_DATE = FastTrack.Value.AddDays(28);
                    }
                    //UPM
                    CompPM.PLEDGE_DUE_10_DAYS_DATE = model.C_COMP_PROCESS_MONITOR.PLEDGE_DUE_10_DAYS_DATE;
                    CompPM.PLEDGE_INITIAL_DATE = model.C_COMP_PROCESS_MONITOR.PLEDGE_INITIAL_DATE;
                    //Fast-Track and 10-Day
                    CompPM.NO_WORKING_DAYS_DATE = model.C_COMP_PROCESS_MONITOR.NO_WORKING_DAYS_DATE;
                    CompPM.CARD_ISSUE_DATE = model.C_COMP_PROCESS_MONITOR.CARD_ISSUE_DATE;
                    CompPM.CARD_EXPIRY_DATE = model.C_COMP_PROCESS_MONITOR.CARD_EXPIRY_DATE;
                    CompPM.CARD_RETURN_DATE = model.C_COMP_PROCESS_MONITOR.CARD_RETURN_DATE;
                    CompPM.CARD_SERIAL_NO = model.C_COMP_PROCESS_MONITOR.CARD_SERIAL_NO;
                    CompPM.CRC_NAME = model.C_COMP_PROCESS_MONITOR.CRC_NAME;
                    CompPM.SECRETARY = model.C_COMP_PROCESS_MONITOR.SECRETARY;
                    CompPM.VETTING_OFFICER = model.C_COMP_PROCESS_MONITOR.VETTING_OFFICER;
                    CompPM.NATURE = model.C_COMP_PROCESS_MONITOR.NATURE;
                    CompPM.RECEIVED_DATE = model.C_COMP_PROCESS_MONITOR.RECEIVED_DATE;
                    CompPM.PRELIMINARY_LETTER_DATE = model.C_COMP_PROCESS_MONITOR.PRELIMINARY_LETTER_DATE;
                    CompPM.INITIAL_REPLY = model.C_COMP_PROCESS_MONITOR.INITIAL_REPLY;
                    CompPM.RESULT_LETTER = model.C_COMP_PROCESS_MONITOR.RESULT_LETTER;
                    CompPM.INTERVIEW = model.C_COMP_PROCESS_MONITOR.INTERVIEW;
                    CompPM.DEFER_DATE = model.C_COMP_PROCESS_MONITOR.DEFER_DATE;
                    CompPM.CERTIFICATE_ISSUED_DATE = model.C_COMP_PROCESS_MONITOR.CERTIFICATE_ISSUED_DATE;
                    CompPM.REMARKS = model.C_COMP_PROCESS_MONITOR.REMARKS;
                    CompPM.WITHDRAW_DATE = model.C_COMP_PROCESS_MONITOR.WITHDRAW_DATE;
                    CompPM.DUE_DATE = model.C_COMP_PROCESS_MONITOR.DUE_DATE;
                    CompPM.TWO_MONTH_CASE = model.C_COMP_PROCESS_MONITOR.TWO_MONTH_CASE;
                    CompPM.DATE_OF_LETTER_6 = model.C_COMP_PROCESS_MONITOR.DATE_OF_LETTER_6;
                    CompPM.DATE_OF_LETTER_8 = model.C_COMP_PROCESS_MONITOR.DATE_OF_LETTER_8;
                    CompPM.REG_MAIL_NO = model.C_COMP_PROCESS_MONITOR.REG_MAIL_NO;
                    if (model.MonitorType == RegistrationConstant.PROCESS_MONITOR_UPM_10DAYS)
                    {
                        //COUNT Public_Holiday 10days without HD
                        List<C_S_PUBLIC_HOLIDAY> PUBLIC_HOLIDAY = db.C_S_PUBLIC_HOLIDAY.ToList();
                        List<DateTime> PublicHolidayList = new List<DateTime>();
                        foreach (var item in PUBLIC_HOLIDAY)
                        {
                            PublicHolidayList.Add(item.HOLIDAY);
                        }
                        DateTime? recievedDate = CompPM.RECEIVED_DATE;
                        if (recievedDate != null)
                        {
                            DateTime? dueDate = recievedDate;
                            for (int iDayCount = 0; iDayCount < 10;)
                            {
                                dueDate = dueDate.Value.AddDays(1);
                                if ((!PublicHolidayList.Contains(dueDate.Value)) && (CommonUtil.isWeekDay(dueDate.Value)))
                                {
                                    iDayCount++;
                                }
                            }
                            CompPM.PLEDGE_DUE_10_DAYS_DATE = dueDate;

                            //count No. of Working Days without HD
                            DateTime? pledgeInitialDaye = CompPM.PLEDGE_INITIAL_DATE;
                            if (pledgeInitialDaye != null)
                            {
                                DateTime? countingDate = recievedDate;
                                int counter = -1;
                                while (countingDate.Value.CompareTo(pledgeInitialDaye) <= 0)
                                {
                                    countingDate = countingDate.Value.AddDays(1);
                                    if ((!PublicHolidayList.Contains(countingDate.Value)) && (CommonUtil.isWeekDay(countingDate.Value)))
                                    {
                                        counter++;
                                    }
                                }
                                if (counter != -1)
                                {
                                    CompPM.NO_WORKING_DAYS_DATE = (counter);
                                }
                            }
                        }
                    }
                }
                else {
                    C_COMP_PROCESS_MONITOR CompPM = new C_COMP_PROCESS_MONITOR();
                    CompPM.UUID = Guid.NewGuid().ToString();
                    CompPM.MASTER_ID = model.C_COMP_APPLICATION.UUID;
                    CompPM.COMPANY_APPLICANTS_ID = model.C_COMP_APPLICANT_INFO.UUID;
                    CompPM.MONITOR_TYPE = model.MonitorType;
                    CompPM.RESULT_LETTER_DATE = model.C_COMP_PROCESS_MONITOR.RESULT_LETTER_DATE;
                    CompPM.REG_MAIL_NO = model.C_COMP_PROCESS_MONITOR.REG_MAIL_NO;
                    //APPLICATION_FORM_ID
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_CGA)
                    {
                        CompPM.APPLICATION_FORM_ID = model.CGCAppList;
                    }
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWCA)
                    {
                        CompPM.APPLICATION_FORM_ID = model.MWCAppList;
                    }
                    //Selected App Apply Type
                    if (model.SelectedAppApplyType != null)
                    {
                        string result = "";
                        foreach (string smi in model.SelectedAppApplyType)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result += smi;
                            }
                        }
                        CompPM.APPLY_STATUS = result;
                    }
                    //CMW
                    if (model.AdditionASorTD == true)
                    {
                        CompPM.ADDITION_AUTH_SIGN = "on";
                    }
                    if (model.AdditionClass == true)
                    {
                        CompPM.ADDITION_CLASS = "on";
                    }
                    if (model.AdditionType == true)
                    {
                        CompPM.ADDITION_TYPE = "on";
                    }
                    //Inter Result
                    CompPM.INTERVIEW_RESULT_ID = model.InterResultID;

                    CompPM.ASSISTANT = model.C_COMP_PROCESS_MONITOR.ASSISTANT;
                    //Fast-Track count add 28 days
                    if (model.MonitorType == RegistrationConstant.PROCESS_MONITOR_FASK_TRACK)
                    {
                        DateTime? FastTrack = model.C_COMP_PROCESS_MONITOR.RECEIVED_DATE;
                        CompPM.FAST_TRACK_DUE_28_DAYS_DATE = FastTrack.Value.AddDays(28);
                    }
                    //10-Days
                    CompPM.RECEIVED_DATE = model.C_COMP_PROCESS_MONITOR.RECEIVED_DATE;
                    CompPM.PLEDGE_DUE_10_DAYS_DATE = model.C_COMP_PROCESS_MONITOR.PLEDGE_DUE_10_DAYS_DATE;
                    //INTERVIEW DATE
                    CompPM.INTERVIEW_DATE = model.C_COMP_PROCESS_MONITOR.INTERVIEW_DATE;
                    CompPM.RESULT_LETTER = model.C_COMP_PROCESS_MONITOR.RESULT_LETTER;

                    CompPM.PLEDGE_INITIAL_DATE = model.C_COMP_PROCESS_MONITOR.PLEDGE_INITIAL_DATE;
                    //Fast-Track and 10-Day
                    CompPM.NO_WORKING_DAYS_DATE = model.C_COMP_PROCESS_MONITOR.NO_WORKING_DAYS_DATE;
                    //Card
                    CompPM.CARD_ISSUE_DATE = model.C_COMP_PROCESS_MONITOR.CARD_ISSUE_DATE;
                    CompPM.CARD_EXPIRY_DATE = model.C_COMP_PROCESS_MONITOR.CARD_EXPIRY_DATE;
                    CompPM.CARD_RETURN_DATE = model.C_COMP_PROCESS_MONITOR.CARD_RETURN_DATE;
                    CompPM.CARD_SERIAL_NO = model.C_COMP_PROCESS_MONITOR.CARD_SERIAL_NO;


                    CompPM.CRC_NAME = model.C_COMP_PROCESS_MONITOR.CRC_NAME;
                    CompPM.SECRETARY = model.C_COMP_PROCESS_MONITOR.SECRETARY;
                    CompPM.VETTING_OFFICER = model.C_COMP_PROCESS_MONITOR.VETTING_OFFICER;
                    CompPM.NATURE = model.C_COMP_PROCESS_MONITOR.NATURE;

                    CompPM.PRELIMINARY_LETTER_DATE = model.C_COMP_PROCESS_MONITOR.PRELIMINARY_LETTER_DATE;
                    CompPM.INITIAL_REPLY = model.C_COMP_PROCESS_MONITOR.INITIAL_REPLY;
                    CompPM.INTERVIEW = model.C_COMP_PROCESS_MONITOR.INTERVIEW;
                    CompPM.DEFER_DATE = model.C_COMP_PROCESS_MONITOR.DEFER_DATE;
                    CompPM.CERTIFICATE_ISSUED_DATE = model.C_COMP_PROCESS_MONITOR.CERTIFICATE_ISSUED_DATE;

                    CompPM.REMARKS = model.C_COMP_PROCESS_MONITOR.REMARKS;
                    CompPM.WITHDRAW_DATE = model.C_COMP_PROCESS_MONITOR.WITHDRAW_DATE;

                    CompPM.DUE_DATE = model.C_COMP_PROCESS_MONITOR.DUE_DATE;
                    CompPM.TWO_MONTH_CASE = model.C_COMP_PROCESS_MONITOR.TWO_MONTH_CASE;
                    CompPM.DATE_OF_LETTER_6 = model.C_COMP_PROCESS_MONITOR.DATE_OF_LETTER_6;
                    CompPM.DATE_OF_LETTER_8 = model.C_COMP_PROCESS_MONITOR.DATE_OF_LETTER_8;
                    //new
                    CompPM.PROCESS_MONITOR_TYPE = model.C_COMP_PROCESS_MONITOR.PROCESS_MONITOR_TYPE;
                    if (model.MonitorType == RegistrationConstant.PROCESS_MONITOR_UPM_10DAYS)
                    {
                        //COUNT Public_Holiday 10days without HD
                        List<C_S_PUBLIC_HOLIDAY> PUBLIC_HOLIDAY = db.C_S_PUBLIC_HOLIDAY.ToList();
                        List<DateTime> PublicHolidayList = new List<DateTime>();
                        foreach (var item in PUBLIC_HOLIDAY)
                        {
                            PublicHolidayList.Add(item.HOLIDAY);
                        }
                        DateTime? recievedDate = CompPM.RECEIVED_DATE;
                        if (recievedDate != null)
                        {
                            DateTime? dueDate = recievedDate;
                            for (int iDayCount = 0; iDayCount < 10;)
                            {
                                dueDate = dueDate.Value.AddDays(1);
                                if ((!PublicHolidayList.Contains(dueDate.Value)) && (CommonUtil.isWeekDay(dueDate.Value)))
                                {
                                    iDayCount++;
                                }
                            }
                            CompPM.PLEDGE_DUE_10_DAYS_DATE = dueDate;

                            //count No. of Working Days without HD
                            DateTime? pledgeInitialDaye = CompPM.PLEDGE_INITIAL_DATE;
                            if (pledgeInitialDaye != null)
                            {
                                DateTime? countingDate = recievedDate;
                                int counter = -1;
                                while (countingDate.Value.CompareTo(pledgeInitialDaye) <= 0)
                                {
                                    countingDate = countingDate.Value.AddDays(1);
                                    if ((!PublicHolidayList.Contains(countingDate.Value)) && (CommonUtil.isWeekDay(countingDate.Value)))
                                    {
                                        counter++;
                                    }
                                }
                                if (counter != -1)
                                {
                                    CompPM.NO_WORKING_DAYS_DATE = (counter);
                                }
                            }
                        }
                    }
                    //Four
                    CompPM.MODIFIED_DATE = System.DateTime.Now;
                    CompPM.MODIFIED_BY = SystemParameterConstant.UserName;
                    CompPM.CREATED_DATE = System.DateTime.Now;
                    CompPM.CREATED_BY = SystemParameterConstant.UserName;
                    db.C_COMP_PROCESS_MONITOR.Add(CompPM);
                }
                db.SaveChanges();
            }
        }
        public void SaveInd_PM(ProcessMonitorDisplayModel model, string RegType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (model.C_IND_PROCESS_MONITOR.UUID != null)
                {
                    C_IND_PROCESS_MONITOR IndPM = db.C_IND_PROCESS_MONITOR.Where(o => o.UUID == model.C_IND_PROCESS_MONITOR.UUID).FirstOrDefault();
                    IndPM.AUDIT_TEXT = model.AUDIT;
                    IndPM.RECEIVED_DATE = model.C_IND_PROCESS_MONITOR.RECEIVED_DATE;
                    IndPM.REFERENCE_ASK_DATE = model.C_IND_PROCESS_MONITOR.REFERENCE_ASK_DATE;
                    IndPM.REFERENCE_REPLY_DATE = model.C_IND_PROCESS_MONITOR.REFERENCE_REPLY_DATE;
                    IndPM.REGISTRATION_REPLY_DATE = model.C_IND_PROCESS_MONITOR.REGISTRATION_REPLY_DATE;
                    IndPM.INTERVIEW_NOTIFY_DATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_NOTIFY_DATE;
                    IndPM.INTERVIEW_DATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_DATE;
                    IndPM.RESULT_ACCEPT_DATE = model.C_IND_PROCESS_MONITOR.RESULT_ACCEPT_DATE;
                    IndPM.RESULT_REFUSE_DATE = model.C_IND_PROCESS_MONITOR.RESULT_REFUSE_DATE;
                    IndPM.RESULT_DEFER_DATE = model.C_IND_PROCESS_MONITOR.RESULT_DEFER_DATE;
                    IndPM.SUPPLE_DOCUMENT_DATE = model.C_IND_PROCESS_MONITOR.SUPPLE_DOCUMENT_DATE;
                    IndPM.DEFER_DATE = model.C_IND_PROCESS_MONITOR.DEFER_DATE;
                    IndPM.GAZETTE_DATE = model.C_IND_PROCESS_MONITOR.GAZETTE_DATE;
                    IndPM.VETTING_OFFICER = model.C_IND_PROCESS_MONITOR.VETTING_OFFICER;
                    IndPM.WITHDRAWAL_DATE = model.C_IND_PROCESS_MONITOR.WITHDRAWAL_DATE;
                    IndPM.REGISTRATION_ASK_DATE = model.C_IND_PROCESS_MONITOR.REGISTRATION_ASK_DATE;
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_IP)
                    {
                        IndPM.APPLICATION_FORM_ID = model.IPAppList;
                    }
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                    {
                        IndPM.APPLICATION_FORM_ID = model.MWIAAppList;
                    }
                    if (RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                    {
                        string result = "";
                        foreach (string smi in model.SelectedAppApplyType)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result += smi;
                            }
                        }
                        IndPM.APPLY_STATUS = result;
                    }
                    IndPM.NOTIFICATION_LETTER_DUEDATE = model.C_IND_PROCESS_MONITOR.NOTIFICATION_LETTER_DUEDATE;
                    IndPM.INTERVIEW_DUEDATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_DUEDATE;
                    IndPM.REMOVAL_DATE = model.C_IND_PROCESS_MONITOR.REMOVAL_DATE;
                    IndPM.OS_DATE = model.C_IND_PROCESS_MONITOR.OS_DATE;
                    IndPM.DUE_DATE = model.C_IND_PROCESS_MONITOR.DUE_DATE;
                    //IndPM.ApplicationForm = model.C_COMP_PROCESS_MONITOR.DUE_DATE;
                }
                else
                {
                    C_IND_PROCESS_MONITOR IndPM = new C_IND_PROCESS_MONITOR();
                    IndPM.UUID = Guid.NewGuid().ToString();
                    IndPM.MASTER_ID = model.C_IND_APPLICATION.UUID;
                    IndPM.CATEGORY_ID = model.C_S_CATEGORY_CODE.UUID;
                    IndPM.CATEGORY_GROUP_ID = model.C_S_CATEGORY_CODE.CATEGORY_GROUP_ID;
                    IndPM.AUDIT_TEXT = model.AUDIT;
                    IndPM.RECEIVED_DATE = model.C_IND_PROCESS_MONITOR.RECEIVED_DATE;
                    IndPM.REFERENCE_ASK_DATE = model.C_IND_PROCESS_MONITOR.REFERENCE_ASK_DATE;
                    IndPM.REFERENCE_REPLY_DATE = model.C_IND_PROCESS_MONITOR.REFERENCE_REPLY_DATE;
                    IndPM.REGISTRATION_REPLY_DATE = model.C_IND_PROCESS_MONITOR.REGISTRATION_REPLY_DATE;
                    IndPM.INTERVIEW_NOTIFY_DATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_NOTIFY_DATE;
                    IndPM.INTERVIEW_DATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_DATE;
                    IndPM.RESULT_ACCEPT_DATE = model.C_IND_PROCESS_MONITOR.RESULT_ACCEPT_DATE;
                    IndPM.RESULT_REFUSE_DATE = model.C_IND_PROCESS_MONITOR.RESULT_REFUSE_DATE;
                    IndPM.RESULT_DEFER_DATE = model.C_IND_PROCESS_MONITOR.RESULT_DEFER_DATE;
                    IndPM.SUPPLE_DOCUMENT_DATE = model.C_IND_PROCESS_MONITOR.SUPPLE_DOCUMENT_DATE;
                    IndPM.DEFER_DATE = model.C_IND_PROCESS_MONITOR.DEFER_DATE;
                    IndPM.GAZETTE_DATE = model.C_IND_PROCESS_MONITOR.GAZETTE_DATE;
                    IndPM.VETTING_OFFICER = model.C_IND_PROCESS_MONITOR.VETTING_OFFICER;
                    IndPM.WITHDRAWAL_DATE = model.C_IND_PROCESS_MONITOR.WITHDRAWAL_DATE;
                    IndPM.REGISTRATION_ASK_DATE = model.C_IND_PROCESS_MONITOR.REGISTRATION_ASK_DATE;
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_IP)
                    {
                        IndPM.APPLICATION_FORM_ID = model.IPAppList;
                    }
                    if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                    {
                        IndPM.APPLICATION_FORM_ID = model.MWIAAppList;
                    }
                    if (RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                    {
                        string result = "";
                        foreach (string smi in model.SelectedAppApplyType)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result += smi;
                            }
                        }
                        IndPM.APPLY_STATUS = result;
                    }
                    IndPM.NOTIFICATION_LETTER_DUEDATE = model.C_IND_PROCESS_MONITOR.NOTIFICATION_LETTER_DUEDATE;
                    IndPM.INTERVIEW_DUEDATE = model.C_IND_PROCESS_MONITOR.INTERVIEW_DUEDATE;
                    IndPM.REMOVAL_DATE = model.C_IND_PROCESS_MONITOR.REMOVAL_DATE;
                    IndPM.OS_DATE = model.C_IND_PROCESS_MONITOR.OS_DATE;
                    IndPM.DUE_DATE = model.C_IND_PROCESS_MONITOR.DUE_DATE;
                    IndPM.MODIFIED_DATE = System.DateTime.Now;
                    IndPM.MODIFIED_BY = SystemParameterConstant.UserName;
                    IndPM.CREATED_DATE = System.DateTime.Now;
                    IndPM.CREATED_BY = SystemParameterConstant.UserName;
                    //IndPM.ApplicationForm = model.C_COMP_PROCESS_MONITOR.DUE_DATE;
                    db.C_IND_PROCESS_MONITOR.Add(IndPM);
                }
                db.SaveChanges();
            }
        }
        #endregion
        public ServiceResult SaveComp_LF(LeaveFormDisplayModel model, string path, IEnumerable<HttpPostedFileBase> file)
        {
            string fileExt = "";
            if (file.ElementAt(0) != null) { fileExt = "." + System.IO.Path.GetExtension(file.ElementAt(0).FileName).Substring(1); }
            if (file.ElementAt(1) != null) { fileExt = "." + System.IO.Path.GetExtension(file.ElementAt(1).FileName).Substring(1); }
            //string fileExt = "." + System.IO.Path.GetExtension(file.FileName).Substring(1);
            string uuid = CommonUtil.NewUuid();
            try
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_LEAVE_FORM LeaveForm = new C_LEAVE_FORM();
                    LeaveForm.UUID = uuid;
                    LeaveForm.MASTER_ID = model.C_COMP_APPLICATION.UUID;
                    LeaveForm.COMPANY_APPLICANT_ID = model.C_COMP_APPLICANT_INFO.UUID;
                    LeaveForm.CERTIFICATION_NO = model.C_COMP_APPLICATION.CERTIFICATION_NO;
                    if (model.mode == "C")
                    {
                        LeaveForm.IS_CANCEL = "Y";
                        LeaveForm.RECORD_TYPE = "C";
                        LeaveForm.FILE_PATH_CANCELLATION = LeaveForm.UUID + fileExt;
                        LeaveForm.LEAVE_START_DATE = model.CancelStartDate;
                        LeaveForm.LEAVE_END_DATE = model.CancelEndDate;
                        LeaveForm.REMARK = model.CancelRemark;
                    }
                    else
                    {
                        LeaveForm.IS_CANCEL = "N";
                        LeaveForm.RECORD_TYPE = model.SelectedNatureType;

                        if (model.SelectedNatureType == "L")
                        {
                            LeaveForm.FILE_PATH_LEAVE = LeaveForm.UUID + fileExt;
                        }
                        else
                        {
                            LeaveForm.FILE_PATH_REPLACEMENT = LeaveForm.UUID + fileExt;
                        }
                        LeaveForm.LEAVE_START_DATE = model.StartDate;
                        LeaveForm.LEAVE_END_DATE = model.EndDate;
                        LeaveForm.REMARK = model.Remark;
                    }
                    LeaveForm.REGISTRATION_TYPE = model.RegType;


                    LeaveForm.MODIFIED_DATE = System.DateTime.Now;
                    LeaveForm.MODIFIED_BY = SystemParameterConstant.UserName;
                    LeaveForm.CREATED_DATE = System.DateTime.Now;
                    LeaveForm.CREATED_BY = SystemParameterConstant.UserName;
                    db.C_LEAVE_FORM.Add(LeaveForm);
                    db.SaveChanges();
                    //file.SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + fileExt)));
                    if (file.ElementAt(0) != null)
                    {
                        file.ElementAt(0).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + fileExt)));
                    }
                    else
                    {
                        file.ElementAt(1).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + fileExt)));
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception ex)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            }
        }
        public ServiceResult SaveInd_LF(LeaveFormDisplayModel model, string path, IEnumerable<HttpPostedFileBase> file)
        {
            string fileExt0 = "";
            string fileExt1 = "";
            string fileExt2 = "";
            string fileExt3 = "";
            if (file.ElementAt(0) != null) { fileExt0 = "." + System.IO.Path.GetExtension(file.ElementAt(0).FileName).Substring(1); }
            if (file.ElementAt(1) != null) { fileExt1 = "." + System.IO.Path.GetExtension(file.ElementAt(1).FileName).Substring(1); }
            if (file.ElementAt(2) != null) { fileExt2 = "." + System.IO.Path.GetExtension(file.ElementAt(2).FileName).Substring(1); }
            if (file.ElementAt(3) != null) { fileExt3 = "." + System.IO.Path.GetExtension(file.ElementAt(3).FileName).Substring(1); }
            //string fileExt = "." + System.IO.Path.GetExtension(file.FileName).Substring(1);
            string uuid = CommonUtil.NewUuid();
            try
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_LEAVE_FORM LeaveForm = new C_LEAVE_FORM();
                    LeaveForm.UUID = Guid.NewGuid().ToString();
                    LeaveForm.MASTER_ID = model.C_IND_APPLICATION.UUID;
                    LeaveForm.COMPANY_APPLICANT_ID = null;
                    LeaveForm.CERTIFICATION_NO = model.C_IND_CERTIFICATE.CERTIFICATION_NO;

                    if (model.mode == "C")
                    {
                        LeaveForm.IS_CANCEL = "Y";
                        LeaveForm.RECORD_TYPE = "C";
                        if (file.ElementAt(2) != null)
                        {
                            LeaveForm.FILE_PATH_BA21 = LeaveForm.UUID+"2" + fileExt2;
                        }
                        if (file.ElementAt(3) != null)
                        {
                            LeaveForm.FILE_PATH_LEAVE =LeaveForm.UUID+"3" + fileExt3;
                        }

                        LeaveForm.LEAVE_START_DATE = model.CancelIndStartDate;
                        LeaveForm.LEAVE_END_DATE = model.CancelIndEndDate;
                        LeaveForm.REMARK = model.CancelIndRemark;
                    }
                    else
                    {
                        LeaveForm.IS_CANCEL = "N";
                        LeaveForm.RECORD_TYPE = "L";
                        if (file.ElementAt(0) != null)
                        {
                            LeaveForm.FILE_PATH_BA21 = LeaveForm.UUID+"0" + fileExt0;
                        }
                        if (file.ElementAt(1) != null)
                        {
                            LeaveForm.FILE_PATH_LEAVE = LeaveForm.UUID+"1" + fileExt1;
                        }
                        LeaveForm.LEAVE_START_DATE = model.IndStartDate;
                        LeaveForm.LEAVE_END_DATE = model.IndEndDate;
                        LeaveForm.REMARK = model.IndRemark;
                    }
                    LeaveForm.REGISTRATION_TYPE = model.RegType;
                    LeaveForm.MODIFIED_DATE = System.DateTime.Now;
                    LeaveForm.MODIFIED_BY = SystemParameterConstant.UserName;
                    LeaveForm.CREATED_DATE = System.DateTime.Now;
                    LeaveForm.CREATED_BY = SystemParameterConstant.UserName;
                    db.C_LEAVE_FORM.Add(LeaveForm);
                    db.SaveChanges();

                    //file.SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + fileExt)));
                    if (file.ElementAt(0) != null)
                    {
                        file.ElementAt(0).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID+"0" + fileExt0)));
                    }
                    if(file.ElementAt(1) != null)
                    {
                        file.ElementAt(1).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID+"1" + fileExt1)));
                    }
                    if (file.ElementAt(2) != null)
                    {
                        file.ElementAt(2).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + "2" + fileExt2)));
                    }
                    if (file.ElementAt(3) != null)
                    {
                        file.ElementAt(3).SaveAs(Path.Combine(path, Path.GetFileName(LeaveForm.UUID + "3" + fileExt3)));
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception ex)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            }
        }
        public string getCurrentCompExpiryDateMwInd(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == id).FirstOrDefault();

                string result = "";
                if (query != null)
                {
                    if (query.EXPIRY_DATE != null)
                    {
                        result = query.EXPIRY_DATE.Value.ToShortDateString();
                    }
                }
                return result;
                //return db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == id).FirstOrDefault().EXPIRY_DATE.Value.ToShortDateString();

            }

        }


        public string getCurrentCompExpiryDateComp(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_COMP_APPLICATION.Where(x => x.UUID == id).FirstOrDefault();

                string result = "";
                if (query != null)
                {
                    if (query.EXPIRY_DATE != null)
                    {
                        result = query.EXPIRY_DATE.Value.ToShortDateString();
                    }
                }
                return result;
                //return db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == id).FirstOrDefault().EXPIRY_DATE.Value.ToShortDateString();

            }

        }

        public string GetCRCByInterviewDate(ProcessMonitorDisplayModel model)
        {
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();


            string q = ""
                     + "\r\n" + "\t" + " Select mt.MEETING_NO "
                     + "\r\n" + "\t" + " From c_meeting mt "
                     + "\r\n" + "\t" + " inner join C_Meeting_Member MM on MM.MEETING_ID = mt.uuid "
                     + "\r\n" + "\t" + " inner join c_Interview_Schedule INS on INS.MEETING_ID = mt.uuid "
                     + "\r\n" + "\t" + " inner join c_Interview_Candidates INC on INC.INTERVIEW_SCHEDULE_ID = INS.UUID "
                     + "\r\n" + "\t" + " inner join c_S_System_Value SSV on MM.COMMITTEE_ROLE_ID = SSV.uuid "
                     + "\r\n" + "\t" + " inner join c_Comp_Applicant_Info CAI on CAI.candidate_Number = INC.candidate_Number "
                     + "\r\n" + "\t" + " inner join c_S_System_Value SROLE on CAI.APPLICANT_ROLE_ID = SROLE.uuid "
                     + "\r\n" + "\t" + " inner join c_Committee_Member CM on MM.member_id = CM.uuid "
                     + "\r\n" + "\t" + " inner join C_Applicant A on CM.APPLICANT_ID = A.uuid "
                     + "\r\n" + "\t" + " WHERE 1 = 1 "
                     + "\r\n" + "\t" + " AND INS.interview_Date = to_date('" + model.C_COMP_PROCESS_MONITOR.INTERVIEW_DATE.ToString()+ "','dd-mm-yyyy') "
                     + "\r\n" + "\t" + " AND CAI.uuid = '"+ model.C_COMP_APPLICANT_INFO.UUID+"' "
                     + "\r\n" + "\t" + " AND SSV.ENGLISH_DESCRIPTION = 'SECRETARY' "
                     ;
            //q = q.Replace(":compApplicantInfo", model.C_COMP_APPLICANT_INFO.UUID);
            //q = q.Replace(":interviewDate", model.C_COMP_PROCESS_MONITOR.INTERVIEW_DATE.ToString());

            string CRCNo = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        CRCNo = Data[0]["MEETING_NO"].ToString();
                    }
                    conn.Close();
                }
            }
            return CRCNo;
        }
        public string GetCRCPostByInterviewDate(ProcessMonitorDisplayModel model)
        {
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            string q = ""
                     + "\r\n" + "\t" + " Select CM.post "
                     + "\r\n" + "\t" + " From c_meeting mt "
                     + "\r\n" + "\t" + " inner join C_Meeting_Member MM on MM.MEETING_ID = mt.uuid "
                     + "\r\n" + "\t" + " inner join c_Interview_Schedule INS on INS.MEETING_ID = mt.uuid "
                     + "\r\n" + "\t" + " inner join c_Interview_Candidates INC on INC.INTERVIEW_SCHEDULE_ID = INS.UUID "
                     + "\r\n" + "\t" + " inner join c_S_System_Value SSV on MM.COMMITTEE_ROLE_ID = SSV.uuid "
                     + "\r\n" + "\t" + " inner join c_Comp_Applicant_Info CAI on CAI.candidate_Number = INC.candidate_Number "
                     + "\r\n" + "\t" + " inner join c_S_System_Value SROLE on CAI.APPLICANT_ROLE_ID = SROLE.uuid "
                     + "\r\n" + "\t" + " inner join c_Committee_Member CM on MM.member_id = CM.uuid "
                     + "\r\n" + "\t" + " inner join C_Applicant A on CM.APPLICANT_ID = A.uuid "
                     + "\r\n" + "\t" + " WHERE 1 = 1 "
                     + "\r\n" + "\t" + " AND INS.interview_Date = to_date('" + model.C_COMP_PROCESS_MONITOR.INTERVIEW_DATE.ToString() + "','dd-mm-yyyy') "
                     + "\r\n" + "\t" + " AND CAI.uuid = '" + model.C_COMP_APPLICANT_INFO.UUID + "' "
                     + "\r\n" + "\t" + " AND SSV.ENGLISH_DESCRIPTION = 'SECRETARY' "
                     ;

            string CRCNo = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        CRCNo = Data[0]["POST"].ToString();
                    }
                    conn.Close();
                }
            }
            return CRCNo;
        }
        public string GetProcDueDate(string sDate, string rType)
        {
            string eDate;
            if (rType == "Y")
            {
                eDate = sDate;
            }
            else
            {
                eDate = getProcDueDate(sDate);
            }
            return eDate;
        }

        public string getProcDueDate(string sDate)
        {
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();

            string mask = "dd/MM/yyyy";

            QueryParameters.Add("sDate", sDate);
            QueryParameters.Add("mask", mask);

            string q = "select C_GET_IND_PROC_DUEDATE(to_date(':sDate', ':mask')) as DUE_DATE from dual";

            q = q.Replace(":sDate", sDate);
            q = q.Replace(":mask", mask);

            string eDate = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        eDate = Data[0]["DUE_DATE"].ToString();
                    }
                    conn.Close();
                }
            }
            return eDate;
        }
        public C_IND_APPLICATION getIndApplicationByUUID(string indApp)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION application = db.C_IND_APPLICATION.Where(o => o.UUID == indApp).FirstOrDefault();
                return application;
            }
        }
        public List<C_IND_APPLICATION_MW_ITEM> getIndApplicationMwItemByIndApplication(C_IND_APPLICATION app)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_IND_APPLICATION_MW_ITEM> MWItem = db.C_IND_APPLICATION_MW_ITEM.Where(x=>x.UUID==app.UUID).Include(x=>x.C_S_SYSTEM_VALUE).ToList();
                return MWItem;
            }
        }
        
        #region PA/MWIA

        public SurnameDetailModel SearchSurname(SurnameDetailModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_APPLICANT_HISTORY> r = new List<C_APPLICANT_HISTORY>();
                if (model.UUID != null)
                {
                      r = db.C_APPLICANT_HISTORY
                     .Include(o => o.C_APPLICANT)
                     .Where(o => o.APPLICANT_ID == model.UUID)
                     .OrderBy(o => o.CREATED_DATE)
                     .ToList();
                }
                string temp = "";
                List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                for (int i = 0; i < r.Count; i++)
                {
                    if (temp.CompareTo(r[i].SURNAME + r[i].GIVEN_NAME_ON_ID + r[i].CHINESE_NAME) != 0)
                        data.Add(new Dictionary<string, object>() {
                    { "V1" ,r[i].SURNAME }
                    , { "V2" ,r[i].GIVEN_NAME_ON_ID }
                    , { "V3" ,r[i].CHINESE_NAME }
                    , { "V4" ,r[i].CREATED_DATE}});

                    temp = r[i].SURNAME + r[i].GIVEN_NAME_ON_ID + r[i].CHINESE_NAME;
                }
                model.Data = data;
                model.Total = model.Data.Count;

                return model;
            }

        }
        public HKIDPASSPORTDetailModel SearchHKIDComp(HKIDPASSPORTDetailModel model)
        {
            if (model.HKID == RegistrationConstant.VARIABLE_NULL || model.HKID == "undefined")
            {
                model.HKID = "";
            }
            if (model.PASSPORT == RegistrationConstant.VARIABLE_NULL || model.PASSPORT == "undefined")
            {
                model.PASSPORT = "";
            }
        
            String sqlQuery1 = "SELECT distinct capp.file_reference_no,( app.surname || ' ' || app.given_name_on_id) as FULLNAME," +
                            " app.chinese_name, srole.code, capp.REGISTRATION_TYPE, capp.ENGLISH_COMPANY_NAME, capp.CHINESE_COMPANY_NAME,sv.ENGLISH_DESCRIPTION as comp_status, sv2.ENGLISH_DESCRIPTION as app_status, " +
                            " capp.EXPIRY_DATE, cinfo.CARD_SERIAL_NO, cinfo.CARD_ISSUE_DATE, cinfo.CARD_EXPIRY_DATE, cinfo.CARD_RETURN_DATE,  " +
                            " capp.UUID" +
                            " from C_comp_application capp, C_comp_applicant_info cinfo, C_applicant app, C_s_system_value srole, C_s_system_value sv, C_s_system_value sv2 " +
                            " where cinfo.APPLICANT_ID = app.UUID" +
                            " AND cinfo.MASTER_ID = capp.UUID" +
                            " AND sv.UUID = capp.APPLICATION_STATUS_ID " +
                            " AND srole.uuid = cinfo.applicant_role_id " +
                            " AND sv2.UUID = cinfo.APPLICANT_STATUS_ID ";
            if (string.IsNullOrWhiteSpace(model.HKID) && string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery1 += " AND 1=2";
            }
            if (!string.IsNullOrWhiteSpace(model.HKID) && !string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery1 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid  ";
                sqlQuery1 += " or upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no ) ";

            }
            else if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                sqlQuery1 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid )  ";
                model.QueryParameters.Add("hkid", model.HKID.Trim().ToUpper());

            }
            else if (!string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery1 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no )  ";
                model.QueryParameters.Add("passportNo", model.HKID.Trim().ToUpper());

            }
            model.Query = sqlQuery1;
            model.Search();





            return model;
        }
        public HKIDPASSPORTDetailModel SearchHKIDQP(HKIDPASSPORTDetailModel model)
        {
            //if (model.HKID == RegistrationConstant.VARIABLE_NULL)
            //{
            //    model.HKID = "";
            //}
            //if (model.PASSPORT == RegistrationConstant.VARIABLE_NULL)
            //{
            //    model.PASSPORT = "";
            //}
            String sqlQuery3 = " SELECT * FROM ( " +
                               " SELECT distinct  cinfo.CARD_SERIAL_NO, cinfo.CARD_ISSUE_DATE, " +
                               " cinfo.CARD_EXPIRY_DATE,capp.file_reference_no, cinfo.CARD_RETURN_DATE , app.hkid as hkid, app.passport_no as passport_no" +
                               " from C_comp_application capp, C_comp_applicant_info cinfo, C_applicant app, C_s_system_value srole, C_s_system_value sv, C_s_system_value sv2 " +
                               " where cinfo.APPLICANT_ID = app.UUID" +
                               " AND cinfo.MASTER_ID = capp.UUID" +
                               " AND sv.UUID = capp.APPLICATION_STATUS_ID " +
                               " AND srole.uuid = cinfo.applicant_role_id " +
                               " AND sv2.UUID = cinfo.APPLICANT_STATUS_ID " +
                               " UNION ALL " +
                               " (select distinct ic.CARD_SERIAL_NO,ic.CARD_ISSUE_DATE,ic.CARD_EXPIRY_DATE, ic.CERTIFICATION_NO, ic.CARD_RETURN_DATE, app.hkid as hkid, app.passport_no as passport_no " +
                               " from C_ind_application iapp, C_applicant app, C_ind_certificate ic, C_s_system_value sv" +
                               " where iapp.APPLICANT_ID = app.UUID" +
                               " AND ic.master_id = iapp.uuid" +
                               " AND sv.uuid = ic.application_status_id )) WHERE 1 = 1 " +
                               " AND CARD_SERIAL_NO is not null ";
            if (string.IsNullOrWhiteSpace(model.HKID) && string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                model.ErrorMessage = "No Record Found.";
                sqlQuery3 += " AND 1=2";
            }
            if (!string.IsNullOrWhiteSpace(model.HKID) && !string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                

                sqlQuery3 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid  ";
                sqlQuery3 += " or upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no ) ";

                model.QueryParameters.Add("hkid", model.HKID.Trim().ToUpper());
                model.QueryParameters.Add("passport_no", model.PASSPORT.Trim().ToUpper());


            }
            else if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                sqlQuery3 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid )  ";
                model.QueryParameters.Add("hkid", model.HKID.Trim().ToUpper());

            }
            else if (!string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery3 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no )  ";
                model.QueryParameters.Add("passportNo", model.HKID.Trim().ToUpper());

            }


            model.Query = sqlQuery3;

            model.Search();



            return model;

        }
        public HKIDPASSPORTDetailModel SearchHKIDInd(HKIDPASSPORTDetailModel model)
        {

            //if (model.HKID == RegistrationConstant.VARIABLE_NULL)
            //{
            //    model.HKID = "";
            //}
            //if (model.PASSPORT == RegistrationConstant.VARIABLE_NULL)
            //{
            //    model.PASSPORT = "";
            //}

            //String sqlQuery2 = "select iapp.file_reference_no, app.surname || ' ' || app.given_name_on_id as FULLNAME" +
            //                         " ,app.chinese_name, '' as role, iapp.REGISTRATION_TYPE, sv.english_description " +
            //                          " ,ic.EXPIRY_DATE, ic.CERTIFICATION_NO, ic.uuid " +
            //                          " , ic.CARD_SERIAL_NO, ic.CARD_ISSUE_DATE" +
            //                          ", ic.CARD_EXPIRY_DATE, ic.CARD_RETURN_DATE, iapp.uuid as iapp_uuid" +
            //                          " from C_ind_application iapp, C_applicant app, C_ind_certificate ic, C_s_system_value sv" +
            //                          " where iapp.APPLICANT_ID = app.UUID" +
            //                          " AND ic.master_id = iapp.uuid" +
            //                          " AND sv.uuid = ic.application_status_id ";


            String sqlQuery2 = @"select iapp.file_reference_no, app.surname || ' ' || app.given_name_on_id as FULLNAME
,app.chinese_name, '' as role, iapp.REGISTRATION_TYPE, sv.english_description 
,ic.EXPIRY_DATE, ic.CERTIFICATION_NO, ic.uuid
, ic.CARD_SERIAL_NO, ic.CARD_ISSUE_DATE, ic.CARD_EXPIRY_DATE, 
ic.CARD_RETURN_DATE, iapp.uuid as iapp_uuid 
from C_ind_application iapp
left join C_applicant app on iapp.APPLICANT_ID = app.UUID 
left join  C_ind_certificate ic on  ic.master_id = iapp.uuid 
 left join  C_s_system_value sv  on sv.uuid = ic.application_status_id  where 1=1 ";

            if (string.IsNullOrWhiteSpace(model.HKID) && string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery2 += " AND 1=2";
            }

            if (!string.IsNullOrWhiteSpace(model.HKID) && !string.IsNullOrWhiteSpace(model.PASSPORT))
            {

                sqlQuery2 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid  ";
                sqlQuery2 += " or upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no ) ";


                model.QueryParameters.Add("hkid", model.HKID.Trim().ToUpper());
                model.QueryParameters.Add("passport_no", model.PASSPORT.Trim().ToUpper());


            }
            else if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                sqlQuery2 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("hkid") + ") = :hkid )  ";
                model.QueryParameters.Add("hkid", model.HKID.Trim().ToUpper());

            }
            else if (!string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                sqlQuery2 += " AND ( upper(" + EncryptDecryptUtil.getDecryptSQL("passport_no") + ") = :passport_no )  ";
                model.QueryParameters.Add("passportNo", model.HKID.Trim().ToUpper());

            }
            List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();

            model.Query = sqlQuery2;
            model.Search();





            return model;
        }
        public CRMReportModel SearchIPImg(CRMReportModel model, string RegType)
        {
            model.Query = SearchImg_q;
            model.QueryWhere = SearchImg_whereQ(model, RegType);
            model.Search();
            return model;
        }
        public string ExportIPImg(CRMReportModel model, string RegType)
        {
            model.Query = SearchImg_q;
            model.QueryWhere = SearchImg_whereQ(model, RegType);
        
            return model.Export("ExportData");
        }
        private string SearchImg_whereQ(CRMReportModel model, string RegType)
        {
            string whereQ = "";
            whereQ += "\r\n\t" + "AND ind_app.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", RegType);
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND (upper(ind_app.FILE_REFERENCE_NO) LIKE :FileRef)";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CheckSurname))
            {
                whereQ += "\r\n\t" + " AND (upper(APPL.SURNAME) LIKE :SurnName)";
                model.QueryParameters.Add("SurnName", "%" + model.CheckSurname.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CheckGivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(APPL.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.CheckGivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APPL.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PASSPORT))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APPL.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PASSPORT.Trim().ToUpper() + "%");
            }

            return whereQ;
        }

        public bool CanDeleteApplication(string id)
        {



            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            QueryParameters.Add("UUID", id);


            String q = "SELECT  C_CAN_IND_APPLICATION_DELETE( :UUID ) as result  FROM DUAL";



            bool result = false;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        int s = Int32.Parse(Data[0]["RESULT"].ToString());
                        if (s > 0)
                        {
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }

                    }
                    conn.Close();
                }
            }

            return result;
        }

        public bool DeleteApplication(string id)
        {

            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            QueryParameters.Add("UUID", id);
            QueryParameters.Add("USERNAME", SystemParameterConstant.UserName);

            String q = " call C_IND_APPLICATION_DELETE( :UUID , :USERNAME ) ";
            //q = q.Replace(":UUID", id);
            //q = q.Replace(":user", SystemParameterConstant.UserName);

            bool result = false;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    // CommonUtil.CallStoredProc(conn, q, QueryParameters);


                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);

                    conn.Close();
                }
            }

            return result;


        }

        public List<C_APPLICANT_HISTORY> displayApplicantHistory(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_APPLICANT_HISTORY> alist = new List<C_APPLICANT_HISTORY>();
                string temp = "";
                foreach (var item in db.C_APPLICANT_HISTORY.Where(x => x.APPLICANT_ID == uuid))
                {
                    if (temp != item.GIVEN_NAME_ON_ID + item.SURNAME + item.CHINESE_NAME)
                    {
                        alist.Add(item);
                    }


                    temp = item.GIVEN_NAME_ON_ID + item.SURNAME + item.CHINESE_NAME;


                }
                return alist.OrderBy(x => x.CREATED_DATE).ToList();
            }


        }
        #endregion
        #region Meeting Room Arrangement
        public Fn02GCA_MRASearchModel SearchMRAAvailable(Fn02GCA_MRASearchModel model)
        {

            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            return registrationCommonService.ViewAvailableRoom(model);

        }
        public MeetingRoomMemberListDisplayModel AjaxDeleteDrafttoMemberList(MeetingRoomDisplayModel model)
        {
            List<string> v = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_COMMITTEE_MEMBER);
            if (model.TargetMeetingMemberToDelete != null)
                v.Add(model.TargetMeetingMemberToDelete);
            MeetingRoomMemberListDisplayModel m = new MeetingRoomMemberListDisplayModel();

            if (string.IsNullOrWhiteSpace(model.MemberlistChanged))
            {

                m.MEETING_UUID = model.C_MEETING.UUID;
                m.init();
            }
            else
            {
                m.Committee_Group_Id = model.C_MEETING.COMMITTEE_GROUP_ID;
                m.loadMember();

            }
            return m;
        }
        public MeetingRoomMemberListDisplayModel AjaxAddDrafttoMemberList(MeetingRoomDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                     //  if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER))
                 //  {
               //     SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER, new List<string>());
               // }
                List<string> v = SessionUtil.DraftList<string>(ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER);
               // List<string> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_COMMITTEE_MEMBER] as List<string>;
               if(model.NewMemberUUID !=null)
                    v.AddRange(model.NewMemberUUID);

                MeetingRoomMemberListDisplayModel m = new MeetingRoomMemberListDisplayModel() ;

                // List<string> ToNewMemeberList = new List<string>();



                ////for filtering existing member 
                //foreach (var u in model.NewMemberUUID)
                //{
                //    if (m.MemberList.Exists(x => x.UUID == u))
                //    {

                //    }
                //    else
                //    {
                //        v.Add(u);
                //        //ToNewMemeberList.Add(u);
                //    }
                //}
                // m.Committee_Group_Id = model.C_MEETING.COMMITTEE_GROUP_ID;
                // m.loadMember();
                if (string.IsNullOrWhiteSpace(model.MemberlistChanged))
                {

                    m.MEETING_UUID = model.C_MEETING.UUID;
                    m.init();
                }
                else
                {
                    m.Committee_Group_Id = model.C_MEETING.COMMITTEE_GROUP_ID;
                    m.loadMember();

                }




                //foreach (var item in ToNewMemeberList)
                //{
                //    MeetingRoomMemberModel mrm = new MeetingRoomMemberModel();
                //    //var t = (from cm in db.C_COMMITTEE_MEMBER
                //    //        join app in db.C_APPLICANT on cm.APPLICANT_ID equals app.UUID
                //    //        where cm.UUID == item
                //    //        select new  cm ,).FirstOrDefault();
                //    //mrm.Name = t.SURNAME + " " + app.GIVEN_NAME_ON_ID;
                //    //mrm.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                //    var query =( from cm in db.C_COMMITTEE_MEMBER
                //                 join cgm in db.C_COMMITTEE_GROUP_MEMBER on cm.UUID equals cgm.MEMBER_ID  into gj
                //                 from x in gj.DefaultIfEmpty()
                //                 join app in db.C_APPLICANT on cm.APPLICANT_ID equals app.UUID
                //                where cm.UUID == item
                //                select new MeetingRoomMemberModel
                //                {
                //                    Role = x.COMMITTEE_ROLE_ID,
                //                    HKID = app.HKID ,
                //                    PassportNo = app.PASSPORT_NO,
                //                   // Name = app.SURNAME + " " + app.GIVEN_NAME_ON_ID ,
                //                   sName =  app.SURNAME ,
                //                   gName = app.GIVEN_NAME_ON_ID,
                //                    Rank = cm.RANK,
                //                    Post = cm.POST,
                //                    Career = cm.CAREER,

                //                }).FirstOrDefault();
                //    //query.UUID = Guid.NewGuid().ToString().Replace("-", "");
                //    query.UUID = item;
                //    query.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                //    query.Name = query.sName + " " + query.gName;
                //    query.PassportNo = EncryptDecryptUtil.getDecryptHKID(query.PassportNo);

                //    m.MemberList.Add(query);



                //}



                return m;

            }


        }


        public MeetingRoomDisplayModel SearchMemberByCommitteeGroup(MeetingRoomDisplayModel model)
        {
            String q = ""
       + "\r\n" + "\t" + "select cm.uuid as uuid,app.surname,app.given_name_on_id , (app.surname||' '||app.given_name_on_id) as fullname,"
       + "\r\n" + "\t" + " cm.rank as rank , cm.post , cm.career"
       + "\r\n" + "\t" + "from C_Committee_Group_Member cgm"
       + "\r\n" + "\t" + "inner join C_committee_Member cm on cgm.MEMBER_ID = cm.uuid"
        + "\r\n" + "\t" + "inner join C_applicant app on app.uuid =cm.applicant_id"
          + "\r\n" + "\t" + "inner join C_committee_group cg on cg.uuid = cgm.COMMITTEE_GROUP_ID"
             + "\r\n" + "\t" + "inner join C_committee c on c.uuid = cg.COMMITTEE_ID"
         + "\r\n" + "\t" + "inner join C_committee_Panel cp on cp.uuid = c.COMMITTEE_PANEL_ID"

       + "\r\n" + "\t" + "where 1=1 ";



            // search parameter


            if (model.CSearchCommitteePanel != null)
            {
                q += "\r\n" + "\t" + " AND cp.PANEL_TYPE_ID = :CSearchCommitteePanel";
                model.QueryParameters.Add("CSearchCommitteePanel", model.CSearchCommitteePanel);
            }
            if (model.CSearchYear != null)
            {
                q += "\r\n" + "\t" + " AND cp.year = :CSearchYear";
                model.QueryParameters.Add("CSearchYear", model.CSearchYear );
            }
            if (model.CSearchCommittee != null)
            {
                q += "\r\n" + "\t" + " AND c.COMMITTEE_TYPE_ID = :CSearchCommittee";
                model.QueryParameters.Add("CSearchCommittee", model.CSearchCommittee );
            }
            if (model.CSearchCommitteeGroup != null)
            {
                q += "\r\n" + "\t" + " AND cg.name = :CSearchCommitteeGroup";
                model.QueryParameters.Add("CSearchCommitteeGroup", model.CSearchCommitteeGroup );
            }
            if (model.CSearchMonth != null)
            {
                q += "\r\n" + "\t" + " AND cg.month like :CSearchMonth";
                model.QueryParameters.Add("CSearchMonth", "%" + model.CSearchMonth.Trim().ToUpper() + "%");
            }
            model.Query = q;
            model.Search();

            return model;
        }
        public MeetingRoomDisplayModel SearchMemberBySurnameAndGivenName(MeetingRoomDisplayModel model)
        {
            String q = ""
          + "\r\n" + "\t" + "select t1.uuid as uuid,t2.surname,t2.given_name_on_id , (t2.surname||' '||t2.given_name_on_id) as fullname,"
          + "\r\n" + "\t" + " t1.rank as rank , t1.post , t1.career"
          + "\r\n" + "\t" + "from C_Committee_Member t1"
          + "\r\n" + "\t" + "inner join C_applicant t2 on t1.applicant_id =t2.uuid "
          + "\r\n" + "\t" + "where 1=1 ";



            // search parameter
            

            if (model.OSearchSurname != null)
            {
                q += "\r\n" + "\t" + " AND upper(t2.SURNAME) like :surname";
                model.QueryParameters.Add("surname","%"+ model.OSearchSurname.Trim().ToUpper()+"%");
            }
            if (model.OSearchGivenname != null)
            {
                q += "\r\n" + "\t" + " AND upper(t2.given_name_on_id) like :givenname";
                model.QueryParameters.Add("givenname", "%" + model.OSearchGivenname.Trim().ToUpper() + "%");
            }
            
            model.Query = q;
            model.Search();

            return model;

        }

        public ServiceResult SaveMRA(MeetingRoomDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
             


                C_INTERVIEW_SCHEDULE c_INTERVIEW_SCHEDULE = new C_INTERVIEW_SCHEDULE();
                C_MEETING c_MEETING = new C_MEETING();

                var IS = db.C_INTERVIEW_SCHEDULE.Find(model.C_INTERVIEW_SCHEDULE.UUID);
                var cm = db.C_MEETING.Find(model.C_MEETING.UUID);
                if (IS == null)
                {
                    var isRoomReversedQuery = db.C_INTERVIEW_SCHEDULE
                                             .Where(x => x.ROOM_ID == model.C_INTERVIEW_SCHEDULE.ROOM_ID
                                             && x.INTERVIEW_DATE == model.C_INTERVIEW_SCHEDULE.INTERVIEW_DATE
                                             && x.TIME_SESSION_ID == model.C_INTERVIEW_SCHEDULE.TIME_SESSION_ID
                                             );


                    if (isRoomReversedQuery.Any())
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE, Message = { "Selected room in the date and session is reserved " } };
                    }

                    c_INTERVIEW_SCHEDULE.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    c_INTERVIEW_SCHEDULE.CREATED_DATE = DateTime.Now;
                    c_INTERVIEW_SCHEDULE.CREATED_BY = SystemParameterConstant.UserName;
                    c_INTERVIEW_SCHEDULE.MEETING_ID = Guid.NewGuid().ToString().Replace("-", "");
                }
                else
                { c_INTERVIEW_SCHEDULE = IS; }

                c_INTERVIEW_SCHEDULE.MODIFIED_DATE = DateTime.Now;
                c_INTERVIEW_SCHEDULE.MODIFIED_BY = SystemParameterConstant.UserName;
                if (c_INTERVIEW_SCHEDULE.INTERVIEW_DATE !=null)
                     c_INTERVIEW_SCHEDULE.INTERVIEW_DATE = (DateTime)model.InterviewSchDateTime;
               //  c_INTERVIEW_SCHEDULE.INTERVIEW_DATE = model.C_INTERVIEW_SCHEDULE.INTERVIEW_DATE;
               c_INTERVIEW_SCHEDULE.TIME_SESSION_ID = model.C_INTERVIEW_SCHEDULE.TIME_SESSION_ID;
                c_INTERVIEW_SCHEDULE.ROOM_ID = model.C_INTERVIEW_SCHEDULE.ROOM_ID;
                c_INTERVIEW_SCHEDULE.IS_CANCEL = model.C_INTERVIEW_SCHEDULE.IS_CANCEL;

        
                if (cm == null || cm.COMMITTEE_GROUP_ID != model.C_MEETING.COMMITTEE_GROUP_ID)
                {
                    //for new 
                    if (cm != null)
                    {
                        c_INTERVIEW_SCHEDULE.MEETING_ID = Guid.NewGuid().ToString().Replace("-", "");
                    }

                    c_MEETING.UUID = c_INTERVIEW_SCHEDULE.MEETING_ID;
                    c_MEETING.CREATED_DATE = DateTime.Now;
                    c_MEETING.CREATED_BY = SystemParameterConstant.UserName;
                    var temp = SystemListUtil.GetCommitteeGroupByUUID(model.C_MEETING.COMMITTEE_GROUP_ID);
                    if (temp != null)
                    {
                        c_MEETING.MEETING_GROUP = temp.NAME;
                        c_MEETING.YEAR = temp.YEAR;
                        c_MEETING.COMMITTEE_TYPE_ID = temp.COMMITTEE_TYPE_ID;


                 
                    }
               


                    var meetingNo = from m in db.C_MEETING
                                    where m.COMMITTEE_GROUP_ID == model.C_MEETING.COMMITTEE_GROUP_ID
                                    
                                    orderby m.MEETING_NO descending
                                    orderby m.MEETING_NO == null
                                    select m.MEETING_NO;
                
                    if (meetingNo == null || !meetingNo.Any())
                    {
                        c_MEETING.MEETING_NO = "01";
                    }
                    else
                    {
                        int tempMeetingNo = int.Parse(meetingNo.FirstOrDefault());
                        tempMeetingNo++;
                        c_MEETING.MEETING_NO = tempMeetingNo.ToString("D2");
                    }
                }
                else
                {
                    c_MEETING = cm;
                }

                c_MEETING.COMMITTEE_GROUP_ID = model.C_MEETING.COMMITTEE_GROUP_ID;

                c_MEETING.MODIFIED_DATE = DateTime.Now;
                c_MEETING.MODIFIED_BY = SystemParameterConstant.UserName;

                c_INTERVIEW_SCHEDULE.MEETING_NUMBER = c_MEETING.YEAR + "-" +
                                             getSSystemValueByUuid(c_MEETING.COMMITTEE_TYPE_ID).CODE
                                               + "-" + c_MEETING.MEETING_GROUP + c_MEETING.MEETING_NO;



                //c_INTERVIEW_SCHEDULE.MEETING_NUMBER = c_MEETING.YEAR + "-" +
                //                                    getSSystemValueByUuid(c_MEETING.C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID).CODE
                //                                    + "-" + c_MEETING.MEETING_GROUP + "-" + c_MEETING.MEETING_NO;


                var memberlist = db.C_MEETING_MEMBER.Where(x => x.MEETING_ID == c_MEETING.UUID);
                db.C_MEETING_MEMBER.RemoveRange(memberlist);

                foreach (var item in model.memberListRole)
                {
                    C_MEETING_MEMBER c = new C_MEETING_MEMBER();
                    c.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    c.MEETING_ID = c_MEETING.UUID;
                    c.MEMBER_ID = item.Key;
                    c.COMMITTEE_ROLE_ID = item.Value;
                    if (model.IsAbsentList == null)
                    { c.IS_ABSENT = "N"; }
                    else
                    { c.IS_ABSENT = model.IsAbsentList.ContainsKey(item.Key) ? "Y" : "N"; }
                  
                    c.MODIFIED_BY = SystemParameterConstant.UserName;
                    c.MODIFIED_DATE = DateTime.Now;
                    c.CREATED_BY = SystemParameterConstant.UserName;
                    c.CREATED_DATE = DateTime.Now;
                    db.C_MEETING_MEMBER.Add(c);


                }

                if (IS == null)
                {
                    db.C_INTERVIEW_SCHEDULE.Add(c_INTERVIEW_SCHEDULE);

                }
                if (cm == null || cm.COMMITTEE_GROUP_ID != model.C_MEETING.COMMITTEE_GROUP_ID)
                {

                
                   
                   db.C_MEETING.Add(c_MEETING);
                   
                }

                db.SaveChanges();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }

        }
        public MeetingRoomDisplayModel GetMRADisplayDetail(string id, MeetingRoomDisplayModel model,string MEETING_NUMBER)
        {
            if (model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWCA || model.RegType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
            {
                model.RegType = RegistrationConstant.REGISTRATION_TYPE_MW;
            }

            if (string.IsNullOrWhiteSpace(id))
            {

                return model;
 //               return new MeetingRoomDisplayModel();
                

           }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
             var query =  (db.C_INTERVIEW_SCHEDULE.Where(x=>x.UUID == id)
                    .Include(x=>x.C_INTERVIEW_CANDIDATES)
                     .Include(x => x.C_MEETING)
                     .Include(x=>x.C_S_ROOM)
                     .Include(x=>x.C_S_SYSTEM_VALUE)
                     .Include(x=>x.C_MEETING.C_COMMITTEE_GROUP)
                     .Include(x => x.C_MEETING.C_S_SYSTEM_VALUE)
                     ).FirstOrDefault();


                ////var mml = GetMemberList(query.C_MEETING.UUID);
                //List<C_MEETING_MEMBER> mml = new List<C_MEETING_MEMBER>();
                //var q = from mm in db.C_MEETING_MEMBER
                //        join cm in db.C_COMMITTEE_MEMBER on mm.MEMBER_ID equals cm.UUID
                //        join cmapplicant in db.C_APPLICANT on cm.APPLICANT_ID equals cmapplicant.UUID
                //        where mm.MEETING_ID == query.C_MEETING.UUID
                //        select mm;
                //mml.AddRange(q);
                //// return list;

                //MeetingRoomMemberListDisplayModel ml = new MeetingRoomMemberListDisplayModel();
                //ml.MemberList = new List<MeetingRoomMemberModel>();
                ////<MeetingRoomMemberListDisplayModel> memberList = new List<MeetingRoomMemberListDisplayModel>();
                //foreach (var item in mml)
                //{
                //    MeetingRoomMemberModel member = new MeetingRoomMemberModel();

                //    member.UUID = item.C_COMMITTEE_MEMBER.UUID;
                //    member.Name = item.C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME + " " + item.C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID;
                //    member.HKID =EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.HKID);
                //    member.PassportNo = EncryptDecryptUtil.getDecryptHKID(item.C_COMMITTEE_MEMBER.C_APPLICANT.PASSPORT_NO);
                //    member.Rank = item.C_COMMITTEE_MEMBER.RANK;
                //    member.Post = item.C_COMMITTEE_MEMBER.POST;
                //    member.Career = item.C_COMMITTEE_MEMBER.CAREER;
                //    member.Role = item.COMMITTEE_ROLE_ID;
                //    member.Absent = item.IS_ABSENT;
                //    ml.MemberList.Add(member);
                //    //var mm = item.C_COMMITTEE_MEMBER;

                //    //C_COMMITTEE_GROUP_MEMBER c = new C_COMMITTEE_GROUP_MEMBER();
                //    //c.C_COMMITTEE_MEMBER = mm;

                //    //if (item.C_S_SYSTEM_VALUE != null)
                //    //{ c.C_S_SYSTEM_VALUE = item.C_S_SYSTEM_VALUE; }

                //    //memberList.Add(c);
                //}
                //ml.MemberList= ml.MemberList.OrderBy(x => x.Name).ToList();

                MeetingRoomMemberListDisplayModel memberModel = new MeetingRoomMemberListDisplayModel() { MEETING_UUID = query.C_MEETING.UUID};
                memberModel.init();



                return new MeetingRoomDisplayModel()
                {
                    RegType = model.RegType,
                    InterviewSchDateTime = query.INTERVIEW_DATE,
                    C_INTERVIEW_SCHEDULE = query,
                    C_MEETING = query.C_MEETING,
                    C_S_ROOM = query.C_S_ROOM,
                    INT_SCH_C_S_SYSTEM_VALUE = query.C_S_SYSTEM_VALUE,
                    MEET_C_S_SYSTEM_VALUE = query.C_MEETING.C_S_SYSTEM_VALUE,
                   // MeetingMemberList = mml,
                    MemberList = memberModel,
                    MEETING_NUMBER = MEETING_NUMBER,
                };
            }

               

        }
        public byte[] DownloadFile(string fullPath)
        {
            try { 
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return fileBytes;
            }catch(Exception e)
            {
                return new byte[] { };
            }
        }
        public byte[] DownloadDocBySubpath(string Path)
        {
            try
            {
                string fullPath = ApplicationConstant.CRMFilePath +
                ApplicationConstant.FileSeparator + Path;
                return System.IO.File.ReadAllBytes(@fullPath);
            }
            catch
            {
                return new byte[0];
            }


        }

        public MeetingRoomMemberListDisplayModel LoadMember(string id)
        {
            MeetingRoomMemberListDisplayModel memberModel = new MeetingRoomMemberListDisplayModel() { Committee_Group_Id = id };
            memberModel.loadMember();


            return memberModel;

        }

        public List<C_MEETING_MEMBER> GetMemberList(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                List<C_MEETING_MEMBER> list = new List<C_MEETING_MEMBER>();
                var q = from mm in db.C_MEETING_MEMBER
                        join cm in db.C_COMMITTEE_MEMBER on mm.MEMBER_ID equals cm.UUID
                        join cmapplicant in db.C_APPLICANT on cm.APPLICANT_ID equals cmapplicant.UUID
                        where mm.MEETING_ID == id
                        select mm;
                list.AddRange(q);
                return list;
                    //return  db.C_MEETING_MEMBER
                    //          .Include(x => x.C_COMMITTEE_MEMBER)
                    //          .Include(x => x.C_COMMITTEE_MEMBER.C_APPLICANT)
                    //          .Include(x=>x.C_S_SYSTEM_VALUE)
                    //         // .Include(x=>x.C_COMMITTEE_MEMBER.C_COMMITTEE_GROUP_MEMBER)
                    //          .Where(x=>x.C_MEETING.UUID==id)
                    //          .OrderBy(x=> x.C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME)
                    //          .ThenBy(x => x.C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID).ToList();

            }

        }
        #endregion


        #region Batch Uplaod
  
   
        public Fn03PA_BUSearchModel SearchBU(Fn03PA_BUSearchModel model)
        {
          
            model.Query = SearchBU_q;  
  
            model.Search();
            return model;
        }
        public string ExportBU(Fn03PA_BUSearchModel model)
        {
            model.Query = SearchBU_q;
            return model.Export("ExportData");
        }
        #endregion

        public static string getUUIDbyCODE(string code)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_CATEGORY_CODE.Where(x => x.CODE == code).FirstOrDefault();
                return result != null ? result.UUID : code;
            }
        }

        public static string getUUIDbyTypeEDesc(string type, string EDesc)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Where(x => x.REGISTRATION_TYPE == type && x.ENGLISH_DESCRIPTION == EDesc).FirstOrDefault();
                return result != null ? result.UUID : "";
            }
        }

        public static string getUUIDbyTypeAndCode(string type, string code)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Where(x => x.C_S_SYSTEM_TYPE.TYPE == type && x.CODE == code).FirstOrDefault();
                return result != null ? result.UUID : "";
            }
        }

        public static string getUUIDbyParentTypeCode(string parentId, string code)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Where(x => x.UUID == parentId && x.CODE == code).FirstOrDefault();
                return result != null ? result.UUID : "";
            }
        }

        public static string getUUIDbyFormTypeCode(string CODE, string registrationType)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Where(x => x.CODE == CODE && x.REGISTRATION_TYPE == registrationType).FirstOrDefault();
                return result != null ? result.UUID : "";
            }
        }

        public static string getUUIDbyCompanyType(string systemType, string eDesc)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Where(x => x.C_S_SYSTEM_TYPE.TYPE == systemType && x.ENGLISH_DESCRIPTION == eDesc).FirstOrDefault();
                return result != null ? result.UUID : "";
            }
        }

        public static string getCodebyUUID(string uuid)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_S_SYSTEM_VALUE.Find(uuid);
                return result != null ? result.CODE : "";
            }
        }

        public byte[] getFileByte(String fullPath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(@fullPath);
            }
            catch (Exception e)
            {
                return new byte[] { };
            }
        }

        public FileResult getRIASigByUUID(string uuid)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_IND_CERTIFICATE.Find(uuid);

                byte[] fileBytes = new byte[] { };
                if (query != null)
                {
                    string filePath = ApplicationConstant.CRMFilePath + query.FILE_PATH_NONRESTRICTED;
                    if (File.Exists(filePath))
                    {
                        fileBytes = getFileByte(filePath);
                        if (query.FILE_PATH_NONRESTRICTED.Contains("jpg"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);

                        }
                        else if (query.FILE_PATH_NONRESTRICTED.Contains("gif"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);

                        }
                    }
                }
                return null;
            }
        } 
    }
}
