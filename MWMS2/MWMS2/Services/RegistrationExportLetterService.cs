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
using System.Globalization;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.IO.Compression;



namespace MWMS2.Services
{

    public class RegistrationExportLetterService: RegistrationCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String SEPARATOR = ",";
        private String DOUBLEQUOTE = "\"";


        String SearchCompany = ""
                          + "\r\n" + "\t" + "SELECT                                                               "
                          + "\r\n" + "\t" + "T1.FILE_REFERENCE_NO                                                 "
                          + "\r\n" + "\t" + "FROM C_comp_APPLICATION T1                                           "
                          + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
                          + "\r\n" + "\t" + "WHERE 1=1                                                            ";
        //  + "\r\n" + "\t" + "AND T1.REGISTRATION_TYPE = 'CGC'                                     ";

        String SearchIP = ""
                           + "\r\n" + "\t" + "SELECT                                                                 "
                           + "\r\n" + "\t" + "T1.FILE_REFERENCE_NO                                                   "
                           + "\r\n" + "\t" + "FROM C_IND_APPLICATION T1 Where T1.REGISTRATION_TYPE = 'IP'            ";

        String SearchIMW = ""
                         + "\r\n" + "\t" + "SELECT                                                                 "
                         + "\r\n" + "\t" + "T1.FILE_REFERENCE_NO                                                   "
                         + "\r\n" + "\t" + "FROM C_IND_APPLICATION T1 Where T1.REGISTRATION_TYPE = 'IMW'            ";

        private String Query_CRCMinutes =
             " SELECT meeting.MEETING_GROUP, meeting.MEETING_NO, to_char(cis.INTERVIEW_DATE, 'dd Month yyyy') as INTERVIEW_DATE  " +
             " , (case when sv2.CODE = 'AM' then '09:15' else '14:15' end) as codeTime " +
             " , room.ROOM, appli.file_reference_no, cand.INTERVIEW_NUMBER, appli.ENGLISH_COMPANY_NAME " +
             " ,(case when cand.INTERVIEW_TYPE = 'I' then "+
             " (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cand.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') "+
             " else '' end) as InterviewName  " +
             " ,(case when cand.INTERVIEW_TYPE = 'A' then " +
             " (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cand.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') " +
             " else '' end) as AssessmentName,  " +
             " to_char(cand.start_date, 'HH24:MI') as fullTime, code.ENGLISH_DESCRIPTION " +
             " from C_COMP_APPLICANT_INFO info " +
             " inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID " +
             " inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID " +
             " inner join C_S_CATEGORY_CODE code on code.UUID = appli.CATEGORY_ID " +
             " inner join C_S_SYSTEM_VALUE sv on sv.UUID = info.APPLICANT_ROLE_ID " +
             " , C_INTERVIEW_CANDIDATES cand  " +
             " inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cand.INTERVIEW_SCHEDULE_ID  " +
             " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = cis.TIME_SESSION_ID  " +
             " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID  " +
             " inner join C_S_ROOM room on room.UUID = cis.ROOM_ID  " +
             " where cand.CANDIDATE_NUMBER = info.CANDIDATE_NUMBER  " +
             " and cis.MEETING_NUMBER = :MeetingNumber  "+
             " order by cand.START_DATE ";

        private String Query_CRCMinutesProf =
            "SELECT meeting.MEETING_GROUP, meeting.MEETING_NO, to_char(cis.INTERVIEW_DATE, 'dd Month yyyy') as INTERVIEW_DATE  " +
            " , (case when sv2.CODE = 'AM' then '09:15' else '14:15' end) as codeTime , room.ROOM, indApl.file_reference_no, cic.INTERVIEW_NUMBER, ' ' " +
            " , (case when cic.INTERVIEW_TYPE = 'I' then (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cic.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') else '' end) as InterviewName " +
            " , (case when cic.INTERVIEW_TYPE = 'A' then (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cic.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') else '' end) as AssessmentName " +
            " , to_char(cic.start_date, 'HH24:MI') as fullTime, sCode.ENGLISH_DESCRIPTION  " +
            " from C_INTERVIEW_CANDIDATES cic  " +
            " inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cic.INTERVIEW_SCHEDULE_ID " +
            " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = cis.TIME_SESSION_ID  " +
            " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID   " +
            " inner join C_S_ROOM room on room.UUID = cis.ROOM_ID " +
            " , C_IND_CERTIFICATE cert " +
            " inner join C_IND_APPLICATION indApl on indApl.UUID = cert.MASTER_ID " +
            " inner join C_APPLICANT apnt on apnt.UUID = indapl.applicant_id " +
            " inner join C_S_CATEGORY_CODE sCode on sCode.UUID = cert.category_id " +
            " inner join C_S_SYSTEM_VALUE sv on sv.UUID = cert.application_status_id " +
            " where cic.candidate_number = cert.CANDIDATE_NUMBER  " +
            " order by cic.start_date  ";

        private String Query_MeetingMember =
            " SELECT CM.UUID, (TITLE.ENGLISH_DESCRIPTION||' '|| upper(APPLN.SURNAME) ||' '|| APPLN.GIVEN_NAME_ON_ID) AS NAME  " +
            " , SOC.ENGLISH_DESCRIPTION AS SOCI, ROLE.CODE AS ROLE, CM.POST AS POST, CM.RANK AS RANK  " +
            " , FROM C_COMMITTEE_MEMBER CM " +
            " INNER JOIN C_APPLICANT APPLN ON CM.APPLICANT_ID = APPLN.UUID  " +
            " INNER JOIN C_MEETING_MEMBER MM ON CM.UUID=MM.MEMBER_ID  " +
            " LEFT OUTER JOIN C_COMMITTEE_MEMBER_INSTITUTE CMI ON CMI.MEMBER_ID = CM.UUID   " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE TITLE ON APPLN.TITLE_ID = TITLE.UUID  " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE SOC ON CMI.SOCIETY_ID = SOC.UUID   " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE ROLE ON MM.COMMITTEE_ROLE_ID = ROLE.UUID   " +
            " WHERE MM.meeting_id = :meeting " +
            " ORDER BY APPLN.SURNAME, APPLN.GIVEN_NAME_ON_ID ";
            

        private String Query_MeetingGrpCommittee =
            " SELECT DISTINCT stitle.ENGLISH_DESCRIPTION AS TITLE ,a.SURNAME ,a.GIVEN_NAME_ON_ID  " +
            " , (a.SURNAME ||' '|| a.GIVEN_NAME_ON_ID) as FullName " +
             " , (addr.ADDRESS_LINE1 || addr.ADDRESS_LINE2 || addr.ADDRESS_LINE3 || addr.ADDRESS_LINE4 || addr.ADDRESS_LINE5) as cAddress " +
            " , TO_CHAR(ivs.INTERVIEW_DATE, 'dd.MM.yyyy') as FullDate " +
            " , TO_CHAR(ivs.INTERVIEW_DATE, 'dd') AS dDate, TO_CHAR(ivs.INTERVIEW_DATE, 'Month') AS dMonth " +
            " ,TO_CHAR(ivs.INTERVIEW_DATE, 'yyyy') AS dYear, TO_CHAR(ivs.INTERVIEW_DATE, 'Day') AS dWeekDay " +
            " ,(CASE WHEN stime.ENGLISH_DESCRIPTION = 'AM' THEN '9:15 am' WHEN stime.ENGLISH_DESCRIPTION = 'PM' THEN '2:15 pm' ELSE '' END) as dayTime  " +
            " , r.ROOM, srole.ENGLISH_DESCRIPTION AS ROLE, ssoc.ENGLISH_DESCRIPTION AS SOC, fa.SURNAME AS SCCRETARY_SURNAME " +
            " , fa.GIVEN_NAME_ON_ID AS SCCRETARY_GIVEN_NAME, (fa.SURNAME || ' ' || fa.GIVEN_NAME_ON_ID ) AS SCCRETARY_FULLNAME " +
            " , m.MEETING_GROUP || m.MEETING_NO || '/' || substr(m.YEAR, 3) as MEETING_NO  " +
            " FROM  C_APPLICANT a " +
            " LEFT JOIN C_COMMITTEE_MEMBER cm ON a.UUID = cm.APPLICANT_ID " +
            " LEFT JOIN C_MEETING_MEMBER mm ON cm.UUID = mm.MEMBER_ID " +
            " LEFT JOIN C_MEETING m ON m.UUID = mm.MEETING_ID " +
            " LEFT JOIN C_INTERVIEW_SCHEDULE ivs ON m.UUID = ivs.MEETING_ID " +
            " LEFT JOIN C_INTERVIEW_CANDIDATES ivc ON ivs.UUID = ivc.INTERVIEW_SCHEDULE_ID " +
            " LEFT JOIN C_ADDRESS addr ON addr.UUID = cm.ENGLISH_ADDRESS_ID  " +
            " LEFT JOIN C_S_SYSTEM_VALUE stitle ON stitle.UUID = a.TITLE_ID  " +
            " LEFT JOIN C_S_SYSTEM_VALUE stime ON stime.UUID = ivs.TIME_SESSION_ID  " +
            " LEFT JOIN C_S_ROOM r ON r.UUID = ivs.ROOM_ID  " +
            " LEFT JOIN C_S_SYSTEM_VALUE srole ON srole.UUID = mm.COMMITTEE_ROLE_ID  " +
            " LEFT JOIN C_COMMITTEE_MEMBER_INSTITUTE  cmi ON cm.UUID = cmi.MEMBER_ID  " +
            " LEFT JOIN C_S_SYSTEM_VALUE ssoc ON ssoc.UUID = cmi.SOCIETY_ID  " +
            " LEFT JOIN C_MEETING_MEMBER fmm ON fmm.MEETING_ID = m.UUID  " +
            " LEFT JOIN C_S_SYSTEM_VALUE findsec ON findsec.UUID = fmm.COMMITTEE_ROLE_ID  " +
            " LEFT JOIN C_COMMITTEE_MEMBER fcm ON fcm.UUID = fmm.MEMBER_ID  " +
            " LEFT JOIN C_APPLICANT fa ON fa.UUID = fcm.APPLICANT_ID  " +
            " WHERE 1 = 1  " +
            " AND findsec.ENGLISH_DESCRIPTION = 'SECRETARY'  " +
            " AND ivs.MEETING_NUMBER  = :MeetingNumber ";

        private String Query_RenewalRestoration =
             " SELECT appli.FILE_REFERENCE_NO, cic.INTERVIEW_NUMBER, appli.ENGLISH_COMPANY_NAME, upper(apnt.Surname)  " +
             " , (upper(apnt.surname) || ' ' || apnt.given_Name_On_Id) as fullname " +
             " , to_char(cic.Start_date, 'HH24:MI') as TImeCIC, to_char(cic.Start_date, 'dd Month YYYY') as DateCIC " +
             " , addrE.address_line1, addrE.address_line2, addrE.address_line3, addrE.address_line4, addrE.address_line5 " +
             " , addrC.address_line1, addrC.address_line2, addrC.address_line3, addrC.address_line4, addrC.address_line5 " +
             " , appli.fax_no1, sv.ENGLISH_DESCRIPTION, room.ROOM   " +
             " , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5 " +
             " , cis.report_to " +
             " , (select (ca2.surname || ' ' || ca2.given_name_on_id ) as sName from C_MEETING_MEMBER mm2  " +
             " inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
             " inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
             " inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID  " +
             " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id  " +
             " where meeting2.UUID = '8a8591a22590a121012591576d4800aa'  " +
             " and sv2.code = '2' and cic.INTERVIEW_TYPE = 'A' )  as interviewFullName " +
             " , sv2.CODE " +
             " from C_COMP_APPLICANT_INFO info  " +
             " inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID " +
             " inner join C_ADDRESS addrE on addrE.UUID = appli.ENGLISH_ADDRESS_ID " +
             " inner join C_ADDRESS addrC on addrC.UUID = appli.CHINESE_ADDRESS_ID " +
             " inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID " +
             " inner join C_S_SYSTEM_VALUE sv on sv.UUID = apnt.TITLE_ID " +
             " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = info.APPLICANT_ROLE_ID " +
             " , C_INTERVIEW_CANDIDATES cic   " +
             " inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cic.INTERVIEW_SCHEDULE_ID  " +
             " inner join C_S_ROOM room on room.UUID = cis.ROOM_ID  " +
             " inner join C_ADDRESS addr2 on addr2.UUID = room.english_address_id  " +
             " where info.candidate_Number = cic.candidate_number  " +
            // " and cis.uuid = '8a8591a22590a121012591576d4800ab' " +
             " and cis.uuid = :interviewScheduleId   " + 
             " order by cic.start_Date  ";

        private String Query_ExportAssessment =
            " SELECT appli.FILE_REFERENCE_NO, cic.INTERVIEW_NUMBER, appli.ENGLISH_COMPANY_NAME, upper(apnt.Surname)  " +
            " , (upper(apnt.surname) || ' ' || apnt.given_Name_On_Id) as fullname " +
            " , to_char(cic.Start_date, 'HH24:MI') as TImeCIC, to_char(cic.Start_date, 'dd Month YYYY') as DateCIC " +
            " , addrE.address_line1, addrE.address_line2, addrE.address_line3, addrE.address_line4, addrE.address_line5 " +
            " , addrC.address_line1, addrC.address_line2, addrC.address_line3, addrC.address_line4, addrC.address_line5 " +
            " , appli.fax_no1, sv.ENGLISH_DESCRIPTION, room.ROOM   " +
            " , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5 " +
             " , cis.report_to " +
             " , (select (ca2.surname || ' ' || ca2.given_name_on_id ) as sName from C_MEETING_MEMBER mm2  " +
             " inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
             " inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
             " inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID  " +
             " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id  " +
             " where meeting2.UUID = '8a8591a22590a121012591576d4800aa'  " +
             " and sv2.code = '2' and cic.INTERVIEW_TYPE = 'A' )  as interviewFullName " +
             " , sv2.CODE " +
            " from C_COMP_APPLICANT_INFO info  " +
            " inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID " +
            " inner join C_ADDRESS addrE on addrE.UUID = appli.ENGLISH_ADDRESS_ID " +
            " inner join C_ADDRESS addrC on addrC.UUID = appli.CHINESE_ADDRESS_ID " +
            " inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID " +
            " inner join C_S_SYSTEM_VALUE sv on sv.UUID = apnt.TITLE_ID " +
            " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = info.APPLICANT_ROLE_ID " +
            " , C_INTERVIEW_CANDIDATES cic   " +
            " inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cic.INTERVIEW_SCHEDULE_ID  " +
            " inner join C_S_ROOM room on room.UUID = cis.ROOM_ID  " +
            " inner join C_ADDRESS addr2 on addr2.UUID = room.english_address_id  " +
            " where info.candidate_Number = cic.candidate_number  " +
            " and cic.Interview_type = 'A' " +
            //" and cis.uuid = '8a8591a22590a121012591576d4800ab' " +
            " and cis.uuid = :interviewScheduleId   " +  
            " order by cic.start_Date  ";

        private String Query_ExportInterview =
           " SELECT appli.FILE_REFERENCE_NO, cic.INTERVIEW_NUMBER, appli.ENGLISH_COMPANY_NAME, upper(apnt.Surname)  " +
           " , (upper(apnt.surname) || ' ' || apnt.given_Name_On_Id) as fullname " +
           " , to_char(cic.Start_date, 'HH24:MI') as TImeCIC, to_char(cic.Start_date, 'dd Month YYYY') as DateCIC " +
           " , addrE.address_line1, addrE.address_line2, addrE.address_line3, addrE.address_line4, addrE.address_line5 " +
           " , addrC.address_line1, addrC.address_line2, addrC.address_line3, addrC.address_line4, addrC.address_line5 " +
           " , appli.fax_no1, sv.ENGLISH_DESCRIPTION, room.ROOM   " +
           " , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5 " +
           " , cis.report_to,'', '', '', '', '', '', '', '', '', '', '', '', '', '', ''  " +
           " , (select (ca2.surname || ' ' || ca2.given_name_on_id ) as sName from C_MEETING_MEMBER mm2  " +
           " inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
           " inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
           " inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID  " +
           " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id  " +
           " where meeting2.UUID = '8a8591a22590a121012591576d4800aa'  " +
           " and sv2.code = '2' and cic.INTERVIEW_TYPE = 'A' ) as interviewFullName " +
           " , sv2.CODE, ('Assessment No. for:' || cic.interview_Number) as secretary " +
           " from C_COMP_APPLICANT_INFO info  " +
           " inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID " +
           " inner join C_ADDRESS addrE on addrE.UUID = appli.ENGLISH_ADDRESS_ID " +
           " inner join C_ADDRESS addrC on addrC.UUID = appli.CHINESE_ADDRESS_ID " +
           " inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID " +
           " inner join C_S_SYSTEM_VALUE sv on sv.UUID = apnt.TITLE_ID " +
           " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = info.APPLICANT_ROLE_ID " +
           " , C_INTERVIEW_CANDIDATES cic   " +
           " inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cic.INTERVIEW_SCHEDULE_ID  " +
           " inner join C_S_ROOM room on room.UUID = cis.ROOM_ID  " +
           " inner join C_ADDRESS addr2 on addr2.UUID = room.english_address_id  " +
           " where info.candidate_Number = cic.candidate_number  " +
           " and cic.Interview_type = 'I' " +
           //" and cis.uuid = '8a8591a22590a121012591576d4800ab' " +
           " and cis.uuid = :interviewScheduleId   " +
           " order by cic.start_Date  ";

        public ServiceResult CheckFileRef(Fn02GCA_ExportLetterSearchModel model)
        {
            if (model.RegType == "CGC" || model.RegType == "CMW")
            {
                model.Query = SearchCompany;
            }
            else if (model.RegType == "IP")
            {
                model.Query = SearchIP;
            }
            else {
                model.Query = SearchIMW;
            }

            string whereQ = "AND T1.FILE_REFERENCE_NO = :FileRef";
            model.QueryWhere = whereQ;
            //model.QueryParameters.Add("RegType", model.RegType);
            model.QueryParameters.Add("FileRef", model.FileRef);
            //model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            if (model.Data.Count > 0)
            {
                //Dictionary<string, List<string>> err = new Dictionary<string, List<string>>();
                //err.Add("FileRef", new List<string>() { "OK" });
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                };
            } else
            {
                Dictionary<string, List<string>> err = new Dictionary<string, List<string>>();
                err.Add("FileRef", new List<string>() { "No Record is found" });
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE
                    ,
                    ErrorMessages = err
                };

            }
            // new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = v.ModelState.Where(o => o.Value.Errors.Count > 0).ToDictionary(o => o.Key, o => o.Value.Errors.Select(o2 => (string.IsNullOrWhiteSpace(o2.ErrorMessage) ? o2.Exception.ToString() : o2.ErrorMessage)).ToList()) };



            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
            };




        }



        public ServiceResult Save(CompICModel model, string regType)
        {
            List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                C_INTERVIEW_SCHEDULE sch = db.C_INTERVIEW_SCHEDULE.Find(model.C_INTERVIEW_SCHEDULE.UUID);
                string[] meet = sch.MEETING_NUMBER.Split('-');
                for (int i = 0; i < draftList.Count; i++)
                {
                    C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES.Find(draftList[i].C_INTERVIEW_CANDIDATES.UUID);
                    if(cand == null)
                    {
                        cand = new C_INTERVIEW_CANDIDATES();
                        cand.REGISTRATION_TYPE = regType;
                        cand.INTERVIEW_SCHEDULE_ID = model.C_INTERVIEW_SCHEDULE.UUID;
                        db.C_INTERVIEW_CANDIDATES.Add(cand);
                        cand.CANDIDATE_NUMBER = draftList[i].CANDIDATE_NUMBER.Value;
                    }
                    cand.ALIAS_NUMBER = draftList[i].C_INTERVIEW_CANDIDATES.ALIAS_NUMBER;
                    cand.INTERVIEW_TYPE = draftList[i].C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE;
                    cand.START_DATE = draftList[i].C_INTERVIEW_CANDIDATES.START_DATE;
                    cand.END_DATE = draftList[i].C_INTERVIEW_CANDIDATES.START_DATE.Value.AddMinutes(draftList[i].TIMEDURATION);

                    cand.INTERVIEW_NUMBER = meet[0] + "-" + cand.CANDIDATE_NUMBER + "-" + meet[1] + "-" + meet[2];
                    if (cand.INTERVIEW_TYPE == "A") cand.INTERVIEW_NUMBER += "-A";
                    cand.ASSESSMENT_NUMBER = meet[0] + "-" + cand.CANDIDATE_NUMBER + "-" + meet[1];
                }
                db.SaveChanges();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }



        public FileStreamResult ExportTemplate(/*String MeetingNumber*/)
        {

            List<string> headerList = new List<string>() {

               "Title", "Last_Name", "First_Name", "English_Name", "Correspondence_Address_for_General_Deliv",
               "Date", "DD", "MMMM", "YYYY", "weekDay", "Time", "Room_Number", "Role", "Society",
               "Secretary_Last_Name", "Secretary_First_Name", "Secretary_English_Name", "Meeting_No"

            };

            List<List<object>> data = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.Query_MeetingGrpCommittee;
                    //Query_CancelMeeting.Append("MEETINGNUM", meetingNum);
                    //Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    //dataInput.Add("MeetingNumber", MeetingNumber);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, null);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("MeetingGroupCommittee", headerList, data);

        }

        public FileStreamResult ExportRenewalRestoration(String UUID)
        {

            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "secretary", "role"

            };

           
            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.Query_RenewalRestoration;
                    Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    dataInput.Add("interviewScheduleId", UUID);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, dataInput);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("Renewal_Restoration", headerList, data);
 
        }

        public FileStreamResult ExportInterview(String UUID)
        {

            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "cc_n1", "cc_n2", "cc_n3", "cc_n4", "cc_n5", "cc_n6", "cc_n7",
               "cc_n8", "cc_n9", "cc_n10", "cc_n11", "cc_n12", "cc_n13", "cc_n14", "cc_n15", "secretary", "role", "Assessment"

            };


            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    // sql 1
                    String sql = this.Query_ExportInterview;
                    Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    dataInput.Add("interviewScheduleId", UUID);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, dataInput);
                    data = CommonUtil.convertToList(dr);

                    // sql 2
                    String sql2 = this.Query_ExportInterview;
                    Dictionary<string, object> dataInput2 = new Dictionary<string, object>();
                    dataInput2.Add("interviewScheduleId", UUID);
                    DbDataReader dr2 = CommonUtil.GetDataReader(conn, sql, dataInput);
                    //data.Add(dr2);

                    conn.Close();
                }
            }
            return this.exportCSVFile("Register_for_Interview", headerList, data);

        }

        public FileStreamResult ExportAssessment(String UUID)
        {

            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "secretary", "role"

            };


            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.Query_ExportAssessment;
                    Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    dataInput.Add("interviewScheduleId", UUID);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, dataInput);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("Register_for_Assessment", headerList, data);

        }

        //public string CheckFileRef(Fn02GCA_ExportLetterSearchModel model, String fileRef) {
        //    model.Query

        //    return null;
        //}
        //FileRef, RegType, cExportLetterUuid, signature
        //SelectAsUuid, SelectTdUuid, SelectPrbUuid, SelectCommitteeUuid, SelectCertUuid, SelectIvCandUuid
        public String PopulateExportLetter(String FileRef, String RegType, String cExportLetterUuid, String SelectAsUuid, String SelectTdUuid
            , String SelectPrbUuid, String SelectCommitteeUuid, String SelectCertUuid, String SelectIvCandUuid, String SelectAuthUuid)
        {

            String content = "";
            String tabChar = "\t";
            String nextLineChar = "\n";
            String indPMUuid = "";
            String interviewCandUuid = "";
            String ApplicantSelectUuid = "";
            
            C_COMP_APPLICATION compApp = getCompApplicationByFileRef(FileRef);
            C_IND_APPLICATION cIndApp = getIndApplicationByFileRef(FileRef);
           
            // exception handling
            if (compApp == null && cIndApp == null) {
                return "";
            }

            // identify Indi or comp
            Boolean isComp = true;
            if (null != cIndApp)
            {
                isComp = false;
            }


            C_S_AUTHORITY sAuthority = getAuthorityByUuid(SelectAuthUuid);
            C_S_EXPORT_LETTER exportLetter = getExportLetterByUuid(cExportLetterUuid);
            C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);  
          
           
            C_INTERVIEW_CANDIDATES interviewCand = getInterviewCandidatesByUuid(interviewCandUuid);

            if (compApp != null)
            {
                List<C_COMP_APPLICANT_INFO> applicantList = getCompApplicantInfo(compApp.UUID);  //compApp.UUID}
            }
            
            List<C_COMP_APPLICANT_MW_ITEM> companyMwItemList = new List<C_COMP_APPLICANT_MW_ITEM>();

            List<String> mwClass1TypeList = new List<String>();
            List<String> mwClass2TypeList = new List<String>();
            List<String> mwClass3TypeList = new List<String>();

            // certificate common colunn header
            List<string> certHeaderList1 = new List<string>()  //change name to : companyColumnCommonHeader
            {
                "file_ref", "reg_no", "br_no"
            };

            List<string> certHeaderList2 = new List<string>() //change name to : authorityColumnCommonHeader
            {
                "test"
            };

            List<string> companyColumnCommon = new List<string>()
            {
                 "file_ref", "reg_no", "br_no",
                 "ename_co", "cname_co", "cat_code", "cat_grp_code",
                 "cat_grp_desc", "reg_dt", "ereg_dt", "creg_dt", "gazet_dt", "egazet_dt", "cgazet_dt",
                 "restore_app_dt", "erestore_app_dt", "crestore_app_dt", "restore_dt", "erestore_dt", "crestore_dt",
                 "retent_app_dt", "eretent_app_dt", "cretent_app_dt", "removal_dt", "eremoval_dt", "cremoval_dt",
                 "approval_dt", "eapproval_dt", "capproval_dt","expiry_dt", "eexpiry_dt", "cexpiry_dt",
                 "addr1","addr2", "addr3", "addr4", "addr5", "caddr1", "caddr2",
                 "caddr3", "caddr4", "caddr5", "tel_no1", "fax_no1",
                 "sub_reg", "sub_creg", "app_name"
            };

            List<string> authorityColumnCommon = new List<string>()
            {
                 "auth_name", "auth_cname", "auth_rank", "auth_crank", "auth_tel", "auth_fax", "auth_action_name"
            };

            List<string> generalColumnCommon = new List<string>()
            {
                 "issue_dt", "eissue_dt", "eissue_dt_wo_day", "cissue_dt"
            };

            List<string> companyItemColumnCommon = new List<string>()
            {
                 "typeline1","classline1", "typeline2", "classline2","typeline3","classline3","typeline4",
                 "classline4","typeline5", "classline5", "typeline6","classline6", "typeline7","classline7"
            };

            List<string> asColumnCommon = new List<string>()
            {
                 "AS_surname", "AS_givenname","AS_ename", "AS_cname", "AS_removal_dt",  "AS_eremoval_dt",
                 "AS_cremoval_dt", "AS_deletion_dt", "AS_edeletion_dt", "AS_cdeletion_dt",
                 "AS_withdraw_dt", "AS_ewithdraw_dt", "AS_cwithdraw_dt", "AS_salutation", "AS_csalutation", "AS_title", "AS_ctitle"
            };

            List<string> tdColumnCommon = new List<string>()
            {
                 "TD_surname", "TD_givenname", "TD_ename", "TD_cname", "TD_removal_dt", "TD_eremoval_dt",
                 "TD_cremoval_dt", "TD_deletion_dt", "TD_edeletion_dt", "TD_cdeletion_dt",
                 "TD_withdraw_dt", "TD_ewithdraw_dt", "TD_cwithdraw_dt","TD_salutation", "TD_csalutation", "TD_title", "TD_ctitle"
            };

            List<string> applicantColumnCommon = new List<string>()
            {
                 "surname", "givenname", "ename", "cname", "title", "ctitle", "salutation", "csalutation", "file_ref",
                  "addr1", "addr2", "addr3", "addr4", "addr5", "caddr1", "caddr2",  "caddr3", "caddr4", "caddr5"
            };

            List<string> indCertificateColumnCommon = new List<string>()
            {
                "retent_app_dt", "eretent_app_dt", "cretent_app_dt", "restore_app_dt", "erestore_app_dt", "crestore_app_dt",
                "removal_dt", "eremoval_dt", "cremoval_dt", "expiry_dt", "eexpiry_dt", "cexpiry_dt"
            };

            List<string> applicantItemColumnCommon = new List<string>()
            {
                 "itemline1", "itemline2","itemline3","itemline4","itemline5","itemline6"
            };

            List<string> prbColumnCommon = new List<string>()
            {
                 "prb", "prb_cat_grp", "prb_reg_no"
            };

            List<string> certColumnCommon = new List<string>()
            {
                "cert_app_dt",  "cert_eapp_dt", "cert_capp_dt", "cert_cat_grp", "cert_sub_title"
            };

            List<string> processMonitorColumnCommon = new List<string>()
            {
               "pm_notify_dt","pm_enotify_dt", "pm_enotify_dt_wo_day", "pm_cnotify_dt","pm_interview_dt","pm_einterview_dt", "pm_einterview_dt_dow","pm_cinterview_dt"
            };

            List<string> interviewCandidatesColumnCommon = new List<string>()
            {
               "ic_start_dt", "ic_estart_dt", "ic_cstart_dt"
            };

            List<string> committeeColumnCommon = new List<string>()
            {
               "committee_name"
            };

            List<string> approvedLetterColumnCommon = new List<string>()
            {
                "file_ref", "ename_co", "cname_co","app_name", "addr1","addr2", "addr3", "addr4", "addr5",
                "caddr1", "caddr2", "caddr3", "caddr4", "caddr5","restore_app_dt", "erestore_app_dt", "crestore_app_dt",
                "retent_app_dt", "eretent_app_dt", "cretent_app_dt", "co_name", "co_class","co_type",
                "as_name", "as_class","as_type","td_name","td_class","td_type"
            };
            List<string> certHeaderList = new List<string>() { };

            //foreach (String certHeader in certHeaderList)
            //{
            //    content = appendCertContent(content, certHeader);
            //}
            //content += "\r\n";

            if (exportLetter.LETTER_SPECIAL_REMARK != null && exportLetter.LETTER_SPECIAL_REMARK.Equals(RegistrationConstant.LETTER_REMARK_APPROVED_LETTER))
            {
                for (int i = 0; i < companyColumnCommon.Count; i++)
                {
                    certHeaderList.Add(companyColumnCommon.ElementAt(i));
                }

                for (int i = 0; i < authorityColumnCommon.Count; i++)
                {
                    certHeaderList.Add(authorityColumnCommon.ElementAt(i));
                }

                for (int i = 0; i < generalColumnCommon.Count; i++)
                {
                    certHeaderList.Add(generalColumnCommon.ElementAt(i));
                }
            }
            else
            {
                if (exportLetter.COMPANY_SELECT == "Y")
                {
                    for (int i = 0; i < companyColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(companyColumnCommon.ElementAt(i));
                    }
                    if (exportLetter.REGISTRATION_TYPE.Equals(RegistrationConstant.REGISTRATION_TYPE_MWCA))
                    {
                        for (int i = 0; i < companyItemColumnCommon.Count; i++)
                        {
                            certHeaderList.Add(companyItemColumnCommon.ElementAt(i));
                        }
                    }
                }
               if(exportLetter.AS_SELECT=="Y")
               // if (exportLetter.AS_SELECT.Equals("Y"))
                {
                    for (int i = 0; i < asColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(asColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.TD_SELECT == "Y")
                {
                    for (int i = 0; i < tdColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(tdColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.APPLICANT_SELECT == "Y")
                {
                    for (int i = 0; i < applicantColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(applicantColumnCommon.ElementAt(i));
                    }
                    if (exportLetter.REGISTRATION_TYPE.Equals(RegistrationConstant.REGISTRATION_TYPE_MWIA))
                    {
                        for (int i = 0; i < indCertificateColumnCommon.Count; i++)
                        {
                            certHeaderList.Add(indCertificateColumnCommon.ElementAt(i));
                        }
                        for (int i = 0; i < applicantItemColumnCommon.Count; i++)
                        {
                            certHeaderList.Add(applicantItemColumnCommon.ElementAt(i));
                        }
                    }
                }
                if (exportLetter.PRB_SELECT == "Y")
                {
                    for (int i = 0; i < prbColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(prbColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.CERT_SELECT == "Y")
                {
                    for (int i = 0; i < certColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(certColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.PROCESS_MONITOR_SELECT == "Y")
                {
                    for (int i = 0; i < processMonitorColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(processMonitorColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.INTERVIEW_CANDIDATES_SELECT == "Y")
                {
                    for (int i = 0; i < interviewCandidatesColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(interviewCandidatesColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.COMMITTEE_SELECT == "Y")
                {
                    for (int i = 0; i < committeeColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(committeeColumnCommon.ElementAt(i));
                    }
                }
                if (exportLetter.AUTHORITY_NAME_SELECT == "Y")
                {
                    for (int i = 0; i < authorityColumnCommon.Count; i++)
                    {
                        certHeaderList.Add(authorityColumnCommon.ElementAt(i));
                    }
                }
                for (int i = 0; i < generalColumnCommon.Count; i++)
                {
                    certHeaderList.Add(generalColumnCommon.ElementAt(i));
                }
            }

            foreach (String certHeader in certHeaderList)
            {
                content = appendCertContent(content, certHeader);
            }
            content += "\r\n";

            if (exportLetter.LETTER_SPECIAL_REMARK != null && exportLetter.LETTER_SPECIAL_REMARK.Equals(RegistrationConstant.LETTER_REMARK_APPROVED_LETTER))
            {
              
                String[] MW_CLASS_DESCRIPTION = { " I, II & III ", " II & III ", " III " };

                if (compApp != null)
                {
                    
                    List<C_COMP_APPLICANT_INFO> applicantList = getCompApplicantInfo(compApp.UUID);  //compApp.UUID}
                    
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

                    String companyTypeString = "";
                    String companyClassString = "";
                    if (mwClass1TypeList != null)
                    {
                        companyClassString += MW_CLASS_DESCRIPTION[0];

                        for (int i = 0; i < mwClass1TypeList.Count(); i++)
                        {
                            companyTypeString += mwClass1TypeList[i].Substring(5);
                            if (i != mwClass1TypeList.Count() - 1)
                            {
                                companyTypeString = companyTypeString + ",";
                            }
                            else
                            {
                                if (mwClass3TypeList != null || mwClass2TypeList != null)
                                {
                                    companyClassString += nextLineChar;
                                    companyTypeString += nextLineChar;
                                }

                            }
                        }
                    }

                    if (mwClass2TypeList != null)
                    {
                        companyClassString += MW_CLASS_DESCRIPTION[1];
                        for (int i = 0; i < mwClass2TypeList.Count(); i++)
                        {
                            companyTypeString += mwClass2TypeList[i].Substring(5);
                            if (i != mwClass2TypeList.Count() - 1)
                            {
                                companyTypeString = companyTypeString + ",";
                            }
                            else
                            {
                                if (mwClass3TypeList != null)
                                {
                                    companyClassString += nextLineChar;
                                    companyTypeString += nextLineChar;
                                }
                            }
                        }
                    }

                    if (mwClass3TypeList != null)
                    {
                        companyClassString += MW_CLASS_DESCRIPTION[2];
                        for (int i = 0; i < mwClass3TypeList.Count(); i++)
                        {
                            companyTypeString += mwClass3TypeList[i].Substring(5);
                            if (i != mwClass3TypeList.Count() - 1)
                            {
                                companyTypeString = companyTypeString + ",";
                            }
                        }
                    }

                    String asClassStr = "";
                    String asTypeStr = "";
                    String asNameStr = "";
                    String tdClassStr = "";
                    String tdTypeStr = "";
                    String tdNameStr = "";

                    foreach (C_COMP_APPLICANT_INFO compAppInfo in applicantList)
                    {
                        applicantList = getCompApplicantInfo(compApp.UUID);

                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        C_S_SYSTEM_VALUE svActive = getSSystemValueByUuid(compAppInfo.APPLICANT_STATUS_ID);

                        List<C_COMP_APPLICANT_MW_ITEM> compApplicantMwItemList = getCompApplicantMwItem(compAppInfo.UUID);
                        if (compApplicantMwItemList != null && compApplicantMwItemList.Count > 0)
                        {
                            if (RegistrationConstant.STATUS_ACTIVE.Equals(svActive))
                            {
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

                                String appNameStr = "";
                                String appClassStr = "";
                                String appTypeStr = "";

                                C_APPLICANT cApnt = getApplicantByUuid(compAppInfo.UUID);
                                if (string.IsNullOrWhiteSpace(cApnt.CHINESE_NAME))
                                {
                                    appNameStr = cApnt.FULL_NAME_DISPLAY; 
                                }
                                else
                                {
                                    appNameStr = cApnt.CHINESE_NAME;
                                }
                                if (mwClass1TypeList != null)
                                {
                                    appClassStr += MW_CLASS_DESCRIPTION[0];
                                    for (int j = 0; j < mwClass1TypeList.Count(); j++)
                                    {
                                        appTypeStr += mwClass1TypeList[j].Substring(5);
                                        if (j != mwClass1TypeList.Count() - 1)
                                        {
                                            appTypeStr += ",";
                                        }
                                        else
                                        {
                                            if (mwClass3TypeList != null || mwClass2TypeList != null)
                                            {
                                                appClassStr += nextLineChar;
                                                appTypeStr += nextLineChar;
                                                appNameStr += nextLineChar;
                                            }
                                        }

                                    }
                                }
                                if (mwClass2TypeList != null)
                                {
                                    appClassStr += MW_CLASS_DESCRIPTION[1];
                                    for (int j = 0; j < mwClass2TypeList.Count(); j++)
                                    {
                                        appTypeStr += mwClass2TypeList[j].Substring(5);
                                        if (j != mwClass1TypeList.Count() - 1)
                                        {
                                            appTypeStr += ",";
                                        }
                                        else
                                        {
                                            if (mwClass3TypeList != null)
                                            {
                                                appClassStr += nextLineChar;
                                                appTypeStr += nextLineChar;
                                                appNameStr += nextLineChar;
                                            }
                                        }
                                    }
                                }
                                if (mwClass3TypeList != null)
                                {
                                    appClassStr += MW_CLASS_DESCRIPTION[2];
                                    for (int j = 0; j < mwClass3TypeList.Count(); j++)
                                    {
                                        appTypeStr += mwClass3TypeList[j].Substring(5);
                                        if (j != mwClass3TypeList.Count() - 1)
                                        {
                                            appTypeStr += ",";
                                        }
                                    }
                                }

                                appClassStr += nextLineChar;
                                appTypeStr += nextLineChar;
                                appNameStr += nextLineChar;

                                if (svRole.Equals("AS"))
                                {
                                    asClassStr += appClassStr;
                                    asTypeStr += appTypeStr;
                                    asNameStr += appNameStr;
                                }
                                else
                                {
                                    tdClassStr += appClassStr;
                                    tdTypeStr += appTypeStr;
                                    tdNameStr += appNameStr;
                                }
                            }
                        }
                    }

                    content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                    content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                    content = appendCertContent(content, appendDoubleQuote(compApp.APPLICANT_NAME));

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

                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE.ToString()));
                    DateTime? regDt = compApp.RESTORATION_APPLICATION_DATE;
                    content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(regDt)));
                    content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(regDt)));

                    content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE.ToString()));
                    DateTime? RetentAppDt = compApp.RETENTION_APPLICATION_DATE;
                    content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RetentAppDt)));
                    content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RetentAppDt)));

                    if (string.IsNullOrWhiteSpace(compApp.CHINESE_COMPANY_NAME))
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                    }
                    content = appendCertContent(content, appendDoubleQuote(companyClassString));
                    content = appendCertContent(content, appendDoubleQuote(companyTypeString));
                    content = appendCertContent(content, appendDoubleQuote(asNameStr));
                    content = appendCertContent(content, appendDoubleQuote(asClassStr));
                    content = appendCertContent(content, appendDoubleQuote(asTypeStr));
                    content = appendCertContent(content, appendDoubleQuote(tdNameStr));
                    content = appendCertContent(content, appendDoubleQuote(tdClassStr));
                    content = appendCertContent(content, appendDoubleQuote(tdTypeStr));



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
                        for (int j = 0; j < authorityColumnCommon.Count(); j++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }

                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getCurrentDate()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDate()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDateWithoutDay()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getChineseCurrentDate()));
                }
                else
                {
                    for (int i = 0; i < approvedLetterColumnCommon.Count; i++)
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
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
                        for (int i = 0; i < authorityColumnCommon.Count; i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }

                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getCurrentDate()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDate()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDateWithoutDay()));
                    content = appendCertContent(content, appendDoubleQuote(DateUtil.getChineseCurrentDate()));

                }

            }
            else   //remark = null
            {
                if (exportLetter.COMPANY_SELECT == "Y")
                {
                    List<String> MW_TYPE_DESCRIPTION = new List<string>()
                    {
                        "A (Alteration and Addition Works)(改動及加建工程)",
                        "B (Repair Works)(修葺工程)",
                        "C (Works relating to Signboards)(關乎招牌的工程)",
                        "D (Drainage Works)(排水工程)",
                        "E (Works relating to Structures for Amenities)(關乎適意設施的工程) ",
                        "F (Finishes Works)(飾面工程)",
                        "G (Demolition Works)(拆卸工程)"
                    };

                    List<string> MW_CLASS_DESCRIPTION = new List<string>()
                    {
                        " I, II & III "," II & III "," III "
                    };
                    List<C_COMP_APPLICANT_INFO> applicantList = getCompApplicantInfo(compApp.UUID);

                    applicantList = getCompApplicantInfo(compApp.UUID);

                    foreach (C_COMP_APPLICANT_INFO compAppInfo in applicantList)
                    {
                        List<C_COMP_APPLICANT_MW_ITEM> compApplicantMwItemList = getCompApplicantMwItem(compAppInfo.UUID);
                        List<String> mwItemId = new List<string>();
                        if (compApplicantMwItemList != null && compApplicantMwItemList.Count > 0)
                        {
                            C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                            C_S_SYSTEM_VALUE svActive = getSSystemValueByUuid(compAppInfo.APPLICANT_STATUS_ID);

                            if (RegistrationConstant.STATUS_ACTIVE.Equals(svActive.CODE)
                                && svRole.CODE.StartsWith("A"))
                            {
                                companyMwItemList.AddRange(compApplicantMwItemList);
                            }

                        }

                    }

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

                    int printLineCoutner = 0;
                    int MAX_LINE = MW_TYPE_DESCRIPTION.Count();
                    if (compApp != null)
                    {
                        content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                        content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO));
                        content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO));
                        content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME.ToUpper()));
                        content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                        content = appendCertContent(content, sCatCode != null ? appendDoubleQuote(sCatCode.CODE) : "");
                        C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                        content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                        content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                        content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE.ToString()));
                        DateTime? RegDate = compApp.REGISTRATION_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RegDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RegDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE.ToString()));
                        DateTime? GazetteDate = compApp.GAZETTE_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(GazetteDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(GazetteDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE.ToString()));
                        DateTime? RestoreAppliDate = compApp.RESTORATION_APPLICATION_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RestoreAppliDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RestoreAppliDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE.ToString()));
                        DateTime? RestoreDate = compApp.RESTORE_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RestoreDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RestoreDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE.ToString()));
                        DateTime? RetentAppliDate = compApp.RETENTION_APPLICATION_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RetentAppliDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RetentAppliDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE.ToString()));
                        DateTime? RemovalDate = compApp.REMOVAL_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE.ToString()));
                        DateTime? ApprovalDate = compApp.APPROVAL_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(ApprovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(ApprovalDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE.ToString()));
                        DateTime? ExpiryDate = compApp.EXPIRY_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(ExpiryDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(ExpiryDate)));

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
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(compApp.APPLICANT_NAME));

                        if (exportLetter.REGISTRATION_TYPE.Equals(RegistrationConstant.REGISTRATION_TYPE_MWCA))
                        {
                            for (int i = 0; i < RegistrationConstant.MW_TYPE.Count(); i++)
                            {
                                if (mwClass1TypeList.Contains(RegistrationConstant.MW_TYPE[i]))
                                {
                                    content = appendCertContent(content, appendDoubleQuote(MW_TYPE_DESCRIPTION[i]));
                                    content = appendCertContent(content, appendDoubleQuote(MW_CLASS_DESCRIPTION[0]));
                                    printLineCoutner++;
                                }
                                else if (mwClass2TypeList.Contains(RegistrationConstant.MW_TYPE[i]))
                                {
                                    content = appendCertContent(content, appendDoubleQuote(MW_TYPE_DESCRIPTION[i]));
                                    content = appendCertContent(content, appendDoubleQuote(MW_CLASS_DESCRIPTION[1]));
                                    printLineCoutner++;
                                }
                                else if (mwClass3TypeList.Contains(RegistrationConstant.MW_TYPE[i]))
                                {
                                    content = appendCertContent(content, appendDoubleQuote(MW_TYPE_DESCRIPTION[i]));
                                    content = appendCertContent(content, appendDoubleQuote(MW_CLASS_DESCRIPTION[2]));
                                    printLineCoutner++;
                                }

                            }

                            for (int i = printLineCoutner; i < MAX_LINE; i++)
                            {
                                content = appendCertContent(content, appendDoubleQuote(""));
                                content = appendCertContent(content, appendDoubleQuote(""));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < companyColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                        for (int i = 0; i < companyItemColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }


                }

                if (exportLetter.AS_SELECT == "Y")
                {
                    
                    C_COMP_APPLICANT_INFO compApplicantInfoAs = getCompApplicantInfoByApplicantId(SelectAsUuid); //SelectAsUuid
                    if (compApplicantInfoAs != null)
                    {
                        C_APPLICANT app = getApplicantByUuid(cIndApp.APPLICANT_ID);

                        content = appendCertContent(content, appendDoubleQuote(app.SURNAME));
                        content = appendCertContent(content, appendDoubleQuote(app.GIVEN_NAME_ON_ID));
                        String dummySearchResult = "";
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID;
                        content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                        content = appendCertContent(content, appendDoubleQuote(app.CHINESE_NAME));

                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoAs.REMOVAL_DATE.ToString()));
                        DateTime? RemovalDate = compApplicantInfoAs.REMOVAL_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDate)));

                        //no idea why duplicated
                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoAs.REMOVAL_DATE.ToString()));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoAs.INTERVIEW_WITHDRAWN_DATE.ToString()));
                        DateTime? IvWithdrawnDate = compApplicantInfoAs.INTERVIEW_WITHDRAWN_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(IvWithdrawnDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(IvWithdrawnDate)));

                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                        }

                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("女士"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                        }
                        C_S_SYSTEM_VALUE svDescriptionId = getSSystemValueByUuid(SelectAsUuid);
                        if (svDescriptionId != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svDescriptionId.ENGLISH_DESCRIPTION));
                            content = appendCertContent(content, appendDoubleQuote(svDescriptionId.CHINESE_DESCRIPTION));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < asColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                }

                if (exportLetter.TD_SELECT == "Y")
                {
                    C_COMP_APPLICATION compApplicationTd = getCompApplicationByUuid(SelectTdUuid);
                    C_COMP_APPLICANT_INFO compApplicantInfoTd = null;
                    if (compApplicationTd != null)
                    {
                         compApplicantInfoTd = getCompApplicantInfoByMasterId(compApplicationTd.UUID);
                    }
                    

                    if (compApplicantInfoTd != null)
                    {
                        C_APPLICANT app = getApplicantByUuid(compApplicantInfoTd.UUID);

                        content = appendCertContent(content, appendDoubleQuote(app.SURNAME));
                        content = appendCertContent(content, appendDoubleQuote(app.GIVEN_NAME_ON_ID));
                        String dummySearchResult = "";
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID;
                        content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                        content = appendCertContent(content, appendDoubleQuote(app.CHINESE_NAME));

                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoTd.REMOVAL_DATE.ToString()));
                        DateTime? RemovalDate = compApplicantInfoTd.REMOVAL_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDate)));

                        //no idea why duplicated
                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoTd.REMOVAL_DATE.ToString()));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDate)));

                        content = appendCertContent(content, appendDoubleQuote(compApplicantInfoTd.INTERVIEW_WITHDRAWN_DATE.ToString()));
                        DateTime? IvWithdrawnDate = compApplicantInfoTd.INTERVIEW_WITHDRAWN_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(IvWithdrawnDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(IvWithdrawnDate)));

                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                        }

                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("女士"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                        }
                        if (app.C_S_SYSTEM_VALUE != null)
                        {
                            C_S_SYSTEM_VALUE svDESC = getSSystemValueByUuid(app.TITLE_ID);
                            content = appendCertContent(content, appendDoubleQuote(svDESC.ENGLISH_DESCRIPTION));
                            content = appendCertContent(content, appendDoubleQuote(svDESC.CHINESE_DESCRIPTION));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }

                    }
                    else
                    {
                        for (int i = 0; i < tdColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }


                }

                if (exportLetter.APPLICANT_SELECT == "Y")
                {
                    C_IND_APPLICATION indApplication = null; 

                    if (cIndApp != null) {
                       indApplication = getIndApplicationByUuid(cIndApp.UUID);
                    }

                    //C_IND_APPLICATION indApplication = getIndApplicationByUuid(cIndApp.UUID);
                    //C_COMP_APPLICATION compApplicationApnt = getCompApplicationByFileRef(FileRef);
                    //C_COMP_APPLICANT_INFO compApplicantInfoApnt = getCompApplicantInfoByMasterId(compApplicationApnt.UUID);
                    if (indApplication != null)
                    {
                        C_APPLICANT app = getApplicantByUuid(cIndApp.APPLICANT_ID);

                        content = appendCertContent(content, appendDoubleQuote(app.SURNAME));
                        content = appendCertContent(content, appendDoubleQuote(app.GIVEN_NAME_ON_ID));
                        String dummySearchResult = "";
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID;
                        content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                        content = appendCertContent(content, appendDoubleQuote(app.CHINESE_NAME));

                        C_S_SYSTEM_VALUE svUuid = getSSystemValueByUuid(app.TITLE_ID);
                        if (svUuid != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svUuid.ENGLISH_DESCRIPTION));
                            content = appendCertContent(content, appendDoubleQuote(svUuid.CHINESE_DESCRIPTION));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }


                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("Sir/Madam"));
                        }

                        if (app.GENDER != null)
                        {
                            if (app.GENDER == "M")
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生"));
                            }
                            else if (app.GENDER == "F")
                            {
                                content = appendCertContent(content, appendDoubleQuote("女士"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                            }
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("先生/女士"));
                        }

                        content = appendCertContent(content, appendDoubleQuote(cIndApp.FILE_REFERENCE_NO));

                        if (cIndApp.POST_TO == "Y")
                        {
                            C_ADDRESS engAddress = getAddressByUuid(cIndApp.ENGLISH_HOME_ADDRESS_ID);
                            C_ADDRESS chiAddress = getAddressByUuid(cIndApp.CHINESE_HOME_ADDRESS_ID);

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


                        }
                        else if (cIndApp.POST_TO == "Y")
                        {
                            C_ADDRESS engAddress = getAddressByUuid(cIndApp.ENGLISH_OFFICE_ADDRESS_ID);
                            C_ADDRESS chiAddress = getAddressByUuid(cIndApp.CHINESE_OFFICE_ADDRESS_ID);

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
                        }

                        if (exportLetter.REGISTRATION_TYPE == "IMW")
                        {
                            C_IND_CERTIFICATE cIndCert = null;
                            List<C_IND_CERTIFICATE> indCert = getIndApplication(cIndApp.UUID);
                            if (!indCert.Equals(null))
                            {
                                cIndCert = (C_IND_CERTIFICATE)indCert[0];
                            }
                            if (!cIndCert.Equals(null)) {
                                List<C_IND_APPLICATION_MW_ITEM> indApplicationMwItemList = getIndApplicationMwItem(cIndCert.UUID);
                            }
                                
                              

                            if (cIndCert != null)
                            {
                                content = appendCertContent(content, appendDoubleQuote(cIndCert.RETENTION_APPLICATION_DATE.ToString()));
                                DateTime? RetentAppliDt = cIndCert.RETENTION_APPLICATION_DATE;
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RetentAppliDt)));
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RetentAppliDt)));

                                content = appendCertContent(content, appendDoubleQuote(cIndCert.RESTORATION_APPLICATION_DATE.ToString()));
                                DateTime? RestoreAppliDt = cIndCert.RESTORATION_APPLICATION_DATE;
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RestoreAppliDt)));
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RestoreAppliDt)));

                                content = appendCertContent(content, appendDoubleQuote(cIndCert.REMOVAL_DATE.ToString()));
                                DateTime? RemovalDt = cIndCert.REMOVAL_DATE;
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(RemovalDt)));
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(RemovalDt)));

                                content = appendCertContent(content, appendDoubleQuote(cIndCert.EXPIRY_DATE.ToString()));
                                DateTime? ExpiryDt = cIndCert.EXPIRY_DATE;
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(ExpiryDt)));
                                content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(ExpiryDt)));
                            }
                            else
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    content = appendCertContent(content, appendDoubleQuote(""));
                                }
                            }


                            String mwItemsStr = "";
                            List<C_S_SYSTEM_VALUE> mwItems = getIndApplicationMwItemByIndAppUuid(cIndApp.UUID);
                            if (mwItems.Count() > 0)
                            {

                                for (int i = 0; i < mwItems.Count(); i++)
                                {
                                    if (i == 0) { mwItemsStr += SEPARATOR; }
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
                            }
                            else
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    content = appendCertContent(content, appendDoubleQuote(""));
                                }
                            }

                        }
                    }

                    else
                    {
                        for (int i = 0; i < applicantColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                        if (exportLetter.REGISTRATION_TYPE.Equals("IMW"))
                        {
                            for (int i = 0; i < indCertificateColumnCommon.Count(); i++)
                            {
                                content = appendCertContent(content, appendDoubleQuote(""));
                            }
                            for (int i = 0; i < applicantItemColumnCommon.Count(); i++)
                            {
                                content = appendCertContent(content, appendDoubleQuote(""));
                            }
                        }
                    }

                }
                if (exportLetter.PRB_SELECT == "Y")
                {
                    C_IND_QUALIFICATION indQualification = getIndQualificationByPrbUuid(SelectPrbUuid);
                    //C_COMP_APPLICATION compApplicationPrb = getCompApplicationByUuid(SelectPrbUuid);
                    //C_COMP_APPLICANT_INFO compApplicantInfoPrb = getCompApplicantInfoByMasterId(compApplicationPrb.UUID);

                    if (indQualification != null)
                    {
                        C_S_CATEGORY_CODE catCode = getSCategoryCodeByUuid(indQualification.CATEGORY_ID);
                        C_S_SYSTEM_VALUE svEngDesc = getSSystemValueByUuid(catCode.CATEGORY_GROUP_ID);
                        C_S_SYSTEM_VALUE svEngDesc1 = getSSystemValueByUuid(indQualification.PRB_ID);

                        content = appendCertContent(content, appendDoubleQuote(svEngDesc1.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svEngDesc.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(indQualification.REGISTRATION_NUMBER));
                    }
                    else
                    {
                        for (int i = 0; i < prbColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }


                }
                if (exportLetter.CERT_SELECT == "Y")
                {
                    C_IND_CERTIFICATE indCertificate = getIndCertificateByCatId(SelectCertUuid);
                    



                    if (indCertificate != null)
                    {
                        C_S_CATEGORY_CODE catCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                        C_S_SYSTEM_VALUE svEngDesc = getSSystemValueByUuid(catCode.CATEGORY_GROUP_ID);


                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPLICATION_DATE.ToString()));
                        DateTime? StartDate = indCertificate.APPLICATION_DATE;
                        content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishFormatDate(StartDate)));
                        content = appendCertContent(content, appendDoubleQuote(DateUtil.getChineseFormatDate(StartDate)));
                        content = appendCertContent(content, appendDoubleQuote(svEngDesc.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(catCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                    }
                    else
                    {
                        for (int i = 0; i < certColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                }
                if (exportLetter.PROCESS_MONITOR_SELECT == "Y")
                {
                    C_IND_PROCESS_MONITOR indProcessMonitor = getProcessMonitorByUuid(indPMUuid);

                    if (indProcessMonitor != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(indProcessMonitor.INTERVIEW_NOTIFY_DATE.ToString()));
                        DateTime? IvNotifyDate = indProcessMonitor.INTERVIEW_NOTIFY_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(IvNotifyDate)));
                        content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishDateWithoutDay(IvNotifyDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(IvNotifyDate)));

                        content = appendCertContent(content, appendDoubleQuote(indProcessMonitor.INTERVIEW_DATE.ToString()));
                        DateTime? IvDate = indProcessMonitor.INTERVIEW_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(IvDate)));
                        content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishDateWithoutDay(IvDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(IvDate)));
                    }
                    else
                    {
                        for (int i = 0; i < processMonitorColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                }
                if (exportLetter.INTERVIEW_CANDIDATES_SELECT == "Y")
                {
                    C_INTERVIEW_CANDIDATES interviewCandidates = getInterviewCandidatesByUuid(SelectIvCandUuid);

                    if (interviewCandidates != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(interviewCand.START_DATE.ToString()));
                        DateTime? StartDate = interviewCand.START_DATE;
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getEnglishFormatDate(StartDate)));
                        content = appendCertContent(content, appendDoubleQuote(OldDateUtil.getChineseFormatDate(StartDate)));
                    }
                    else
                    {
                        for (int i = 0; i < interviewCandidatesColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                }
                if (exportLetter.COMMITTEE_SELECT == "Y")
                {
                   
                    C_S_SYSTEM_VALUE committeePanel = getSSystemValueByUuid(exportLetter.UUID);
                    if (committeePanel != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(committeePanel.ENGLISH_DESCRIPTION));
                    }
                    else
                    {
                        for (int i = 0; i < committeeColumnCommon.Count(); i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }

                }
                if (exportLetter.AUTHORITY_NAME_SELECT == "Y") {
                    C_S_AUTHORITY sAuthority2 = getAuthorityByUuid(SelectAuthUuid);
                    if (sAuthority2 != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.ENGLISH_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.ENGLISH_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.CHINESE_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.ENGLISH_ACTION_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.TELEPHONE_NO));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority2.FAX_NO));
                    }
                    else
                    {
                        for (int i = 0; i < authorityColumnCommon.Count(); i++) {
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }

                }
                content = appendCertContent(content, appendDoubleQuote(DateUtil.getCurrentDate()));
                content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDate()));
                content = appendCertContent(content, appendDoubleQuote(DateUtil.getEnglishCurrentDateWithoutDay()));
                content = appendCertContent(content, appendDoubleQuote(DateUtil.getChineseCurrentDate()));

            }

          
            return content;
        }

        public void LookUpSelection(Fn02GCA_ExportLetterSearchModel model) {
            String Regtype = model.RegType;
            String ExportLetterUuid = model.selectedLetterUuid;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                    String FileRef = model.FileRef;
                C_S_EXPORT_LETTER C_S_EXPORT_LETTER = db.C_S_EXPORT_LETTER.Find(model.selectedLetterUuid);
                if (C_S_EXPORT_LETTER != null)
                {
                    if ("Y".Equals(C_S_EXPORT_LETTER.AS_SELECT))
                    {
                        model.AS_Select = true;
                        List<C_APPLICANT> r = db.C_COMP_APPLICANT_INFO
                            .Where(o => o.C_COMP_APPLICATION.FILE_REFERENCE_NO == FileRef)
                            .Where(o => o.C_S_SYSTEM_VALUE.CODE == "AS")
                            .Select(o => o.C_APPLICANT).ToList();
                        model.SelectASList = r.Select(o => new SelectListItem() { Text = o.FULL_NAME_DISPLAY, Value = o.UUID }).ToList();
                    }
                    if ("Y".Equals(C_S_EXPORT_LETTER.TD_SELECT))
                    {
                        model.TD_Select = true;
                        List<C_APPLICANT> r = db.C_COMP_APPLICANT_INFO
                            .Where(o => o.C_COMP_APPLICATION.FILE_REFERENCE_NO == FileRef)
                            .Where(o => o.C_S_SYSTEM_VALUE.CODE == "TD")
                            .Select(o => o.C_APPLICANT).ToList();
                        model.SelectTDList = r.Select(o => new SelectListItem() { Text = o.FULL_NAME_DISPLAY, Value = o.UUID }).ToList();
                    }
                    if ("Y".Equals(C_S_EXPORT_LETTER.PRB_SELECT))
                    {
                        model.PRB_Select = true;
                        List<C_S_SYSTEM_VALUE> r = db.C_IND_QUALIFICATION
                            .Where(o => o.C_IND_APPLICATION.FILE_REFERENCE_NO == FileRef)
                            .Select(o => o.C_S_SYSTEM_VALUE1).ToList();
                        model.SelectPRBList = r.Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }).ToList();
                    }
                    if ("Y".Equals(C_S_EXPORT_LETTER.INTERVIEW_CANDIDATES_SELECT))
                    {

                    }
                    if ("Y".Equals(C_S_EXPORT_LETTER.CERT_SELECT))
                    {
                        model.CERT_Select = true;
                        List<C_S_CATEGORY_CODE> r = db.C_IND_CERTIFICATE
                            .Where(o => o.C_IND_APPLICATION.FILE_REFERENCE_NO == FileRef)
                            .Select(o => o.C_S_CATEGORY_CODE).ToList();
                        model.SelectCERTList = r.Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }).ToList();
                    }
                    if (("Y".Equals(C_S_EXPORT_LETTER.PROCESS_MONITOR_SELECT) && Regtype == "IP") || ("Y".Equals(C_S_EXPORT_LETTER.INTERVIEW_CANDIDATES_SELECT) && Regtype != "IP"))
                    {
                        model.DIA_Select = true;
                        List<C_INTERVIEW_CANDIDATES> r = db.C_INTERVIEW_CANDIDATES
                            .Where(o => o.UUID == ExportLetterUuid)
                             .Where(o => o.REGISTRATION_TYPE == Regtype)
                           .ToList();
                        model.SelectDIAList = r.Select(o => new SelectListItem() { Text = o.START_DATE.ToString(), Value = o.UUID }).ToList();
                    }

                    if ("Y".Equals(C_S_EXPORT_LETTER.COMMITTEE_SELECT))
                    {
                        model.COMMITTEE_Select = true;
                        List<C_S_SYSTEM_VALUE> r = db.C_S_SYSTEM_VALUE
                            .Where(o => o.C_S_SYSTEM_TYPE.TYPE == "COMMITTEE_TYPE")
                             .Where(o => o.REGISTRATION_TYPE == Regtype)
                           .ToList();
                        //List<C_S_SYSTEM_VALUE> r = db.C_COMMITTEE
                        //     .Where(o => o.C_S_SYSTEM_VALUE.REGISTRATION_TYPE == Regtype)
                        //   .Select(o => o.C_S_SYSTEM_VALUE).ToList();
                        model.SelectCOMMITTEEList = r.Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }).ToList();
                    }
                }
         
             

                //if ("Y".Equals(C_S_EXPORT_LETTER.APPLICANT_SELECT))
                //{
                   

                //}
                
            }
        }
        
        public Fn02GCA_ExportLetterSearchModel getLetterByUUID(Fn02GCA_ExportLetterSearchModel dataExport)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string UUID = dataExport.selectedLetterUuid.Trim();
                var query = db.C_S_EXPORT_LETTER.Find(UUID);
                dataExport.FilePath = ApplicationConstant.CRMFilePath + RegistrationConstant.LETTER_TEMPLATE_PATH;
                dataExport.FileName = query.FILE_NAME;

                return dataExport;
            }
        }

    }
}