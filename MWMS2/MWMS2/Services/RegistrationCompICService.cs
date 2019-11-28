using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Data.Entity;
using System.Globalization;


namespace MWMS2.Services
{

    public class RegistrationCompICService: RegistrationCommonService
    {
        String SearchIC_q = ""
                       + "\r\n" + "\t" + "SELECT distinct sch.UUID,  meeting.YEAR as YEAR, meeting.MEETING_GROUP as MEETING_GROUP,   "
                       + "\r\n" + "\t" + "sch.MEETING_NUMBER as MEETING_NUMBER, sch.INTERVIEW_DATE as INTERVIEW_DATE,                "
                       + "\r\n" + "\t" + "sv2.CODE as CODE, room.ROOM as ROOM, sch.IS_CANCEL as IS_CANCELED,                         "
                       + "\r\n" + "\t" + "sv1.Code as TYPECGC                                                                        "
                       + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES cand                                                           "
                       + "\r\n" + "\t" + "LEFT JOIN C_INTERVIEW_SCHEDULE sch on sch.UUID = cand.INTERVIEW_SCHEDULE_ID                "
                       + "\r\n" + "\t" + "LEFT JOIN C_MEETING meeting on meeting.UUID = sch.MEETING_ID                               "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv1 on sv1.UUID = meeting.COMMITTEE_TYPE_ID                     "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_ROOM room on room.UUID = sch.ROOM_ID                                         "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv2 on sv2.UUID = sch.TIME_SESSION_ID                           "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv3 on sv3.UUID = cand.RESULT_ID                                "
                       + "\r\n" + "\t" + "WHERE 1=1                                                                                  "
                       + "\r\n" + "\t" + "AND sv1.REGISTRATION_TYPE = :RegType                                                       ";

        String SearchCandidateNo_q = ""
                     + "\r\n\t" + "SELECT T3.FILE_REFERENCE_NO                                                                 "
                     + "\r\n\t" + ", T4.SURNAME || ' ' || T4.GIVEN_NAME_ON_ID AS FUL_NAME                                      "
                     + "\r\n\t" + ", T5.CODE, T1.*                                                                             "
                     + "\r\n\t" + ", case when T1.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end as IVType        "
                     + "\r\n\t" + "FROM C_INTERVIEW_SCHEDULE T0                                                                "
                     + "\r\n\t" + "INNER JOIN  C_INTERVIEW_CANDIDATES T1 ON T0.UUID = T1.INTERVIEW_SCHEDULE_ID                 "
                     + "\r\n\t" + "LEFT JOIN C_COMP_APPLICANT_INFO T2 ON T1.CANDIDATE_NUMBER = T2.CANDIDATE_NUMBER             "
                     + "\r\n\t" + "LEFT JOIN C_COMP_APPLICATION T3 ON T3.UUID = T2.MASTER_ID                                   "
                     + "\r\n\t" + "LEFT JOIN C_APPLICANT T4 ON T4.UUID = T2.APPLICANT_ID                                       "
                     + "\r\n\t" + "LEFT JOIN C_S_SYSTEM_VALUE T5 ON T5.UUID = T2.APPLICANT_ROLE_ID                             "
                     + "\r\n\t" + "WHERE 1=1                                                                                   "
                     + "\r\n\t" + "And T3.registration_type = 'CGC'                                                            "
                     + "\r\n\t" + "And T1.candidate_number = :CandidateNo                                                      ";

        // 1. ExportCRCMinute (COMP)
        private string Query_CRCMinutes =
            " \r\n\t SELECT meeting.MEETING_GROUP, meeting.MEETING_NO, to_char(cis.INTERVIEW_DATE, 'dd Month yyyy') as INTERVIEW_DATE, "
            + " \r\n\t (case when sv2.CODE = 'AM' then '09:15' else '14:15' end) as codeTime, room.ROOM, "
            + " \r\n\t sv.code, cand.interview_type, appli.file_reference_no, cand.INTERVIEW_NUMBER,appli.ENGLISH_COMPANY_NAME, "
            + " \r\n\t (case when cand.INTERVIEW_TYPE = 'I' then "
            + " \r\n\t (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cand.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') "
            + " \r\n\t else '' end) as InterviewName  "
            + " \r\n\t ,(case when cand.INTERVIEW_TYPE = 'A' then "
            + " \r\n\t (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cand.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')')"
            + " \r\n\t else '' end) as AssessmentName, "
            + " \r\n\t to_char(cand.start_date, 'HH24:MI') as fullTime, "
            + " \r\n\t code.ENGLISH_DESCRIPTION "
            + " \r\n from C_COMP_APPLICANT_INFO info "
            + " \r\n\t inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID "
            + " \r\n\t inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID "
            + " \r\n\t inner join C_S_CATEGORY_CODE code on code.UUID = appli.CATEGORY_ID "
            + " \r\n\t inner join C_S_SYSTEM_VALUE sv on sv.UUID = info.APPLICANT_ROLE_ID "
            + " \r\n\t , C_INTERVIEW_CANDIDATES cand "
            + " \r\n\t inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cand.INTERVIEW_SCHEDULE_ID "
            + " \r\n\t inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = cis.TIME_SESSION_ID "
            + " \r\n\t inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID "
            + " \r\n\t inner join C_S_ROOM room on room.UUID = cis.ROOM_ID "
            + " \r\n where 1=1 and cand.CANDIDATE_NUMBER = info.CANDIDATE_NUMBER "
            + " \r\n\t and cis.MEETING_NUMBER = :MeetingNumber ";

        // 1. ExportCRCMinute (Prof?)
        private string Query_CRCMinutesProf =
            "SELECT meeting.MEETING_GROUP, meeting.MEETING_NO, to_char(cis.INTERVIEW_DATE, 'dd Month yyyy') as INTERVIEW_DATE  " +
            " , (case when sv2.CODE = 'AM' then '09:15' else '14:15' end) as codeTime , room.ROOM, indApl.file_reference_no, cic.INTERVIEW_NUMBER, ' ' " +
            " , (case when cic.INTERVIEW_TYPE = 'I' then (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cic.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') else '' end) as InterviewName " +
            " , (case when cic.INTERVIEW_TYPE = 'A' then (apnt.Surname || ' ' || apnt.GIVEN_NAME_ON_ID || ' (' || case when cic.INTERVIEW_TYPE = 'I' then 'Interview of ' else 'Assessment of ' end || sv.CODE || ')') else '' end) as AssessmentName " +
            " , to_char(cic.start_date, 'HH24:MI') as fullTime, sCode.ENGLISH_DESCRIPTION, cert.REGISTRATION_DATE  " +
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
            " order by cic.start_date ";

        // 2. ExportMeetingGroupCommittee
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

        // 3. ExportRenewalRestoration
        private String Query_RenewalRestoration =
             " SELECT appli.FILE_REFERENCE_NO, cic.INTERVIEW_NUMBER, appli.ENGLISH_COMPANY_NAME, upper(apnt.Surname)  " +
             " \r\n\t , (upper(apnt.surname) || ' ' || apnt.given_Name_On_Id) as fullname " +
             " \r\n\t , to_char(cic.Start_date, 'HH24:MI') as TImeCIC, to_char(cic.Start_date, 'dd Month YYYY') as DateCIC " +
             " \r\n\t , addrE.address_line1, addrE.address_line2, addrE.address_line3, addrE.address_line4, addrE.address_line5 " +
             " \r\n\t , addrC.address_line1, addrC.address_line2, addrC.address_line3, addrC.address_line4, addrC.address_line5 " +
             " \r\n\t , appli.fax_no1, sv.ENGLISH_DESCRIPTION, room.ROOM   " +
             " \r\n\t , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5 " +
             " \r\n\t , cis.report_to " +
             " \r\n\t , (select (ca2.surname || ' ' || ca2.given_name_on_id ) as sName from C_MEETING_MEMBER mm2  " +
             " \r\n\t inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
             " \r\n\t inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
             " \r\n\t inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID  " +
             " \r\n\t inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id  " +
             " \r\n\t where meeting2.UUID = '8a8591a22590a121012591576d4800aa'  " +
             " \r\n\t and sv2.code = '2' and cic.INTERVIEW_TYPE = 'A' )  as interviewFullName " +
             " \r\n\t , sv2.CODE " +
             " \r\n\t from C_COMP_APPLICANT_INFO info  " +
             " \r\n\t inner join C_COMP_APPLICATION appli on appli.UUID = info.MASTER_ID " +
             " \r\n\t inner join C_ADDRESS addrE on addrE.UUID = appli.ENGLISH_ADDRESS_ID " +
             " \r\n\t inner join C_ADDRESS addrC on addrC.UUID = appli.CHINESE_ADDRESS_ID " +
             " \r\n\t inner join C_APPLICANT apnt on apnt.UUID = info.APPLICANT_ID " +
             " \r\n\t inner join C_S_SYSTEM_VALUE sv on sv.UUID = apnt.TITLE_ID " +
             " \r\n\t inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = info.APPLICANT_ROLE_ID " +
             " \r\n\t , C_INTERVIEW_CANDIDATES cic   " +
             " \r\n\t inner join C_INTERVIEW_SCHEDULE cis on cis.UUID = cic.INTERVIEW_SCHEDULE_ID  " +
             " \r\n\t inner join C_S_ROOM room on room.UUID = cis.ROOM_ID  " +
             " \r\n\t inner join C_ADDRESS addr2 on addr2.UUID = room.english_address_id  " +
             " \r\n\t where info.candidate_Number = cic.candidate_number  " +
            // " and cis.uuid = '8a8591a22590a121012591576d4800ab' " +
             " \r\n\t and cis.uuid = :interviewScheduleId   " +
             " \r\n\t order by cic.start_Date  ";

        // 5. ExportAssessment
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

        // 4. ExportInterview
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


        public ServiceResult AddtoList(string durationStr, string candidateNumberStr, string regType, string UUID)
        {

            int candidateNumber;
            int duration;
            try
            {
                candidateNumber = int.Parse(candidateNumberStr);
                duration = int.Parse(durationStr);

            }
            catch (Exception ex)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE,
                    Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                };
            }


            List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            bool exist = draftList.Where(o => o.CANDIDATE_NUMBER == candidateNumber).FirstOrDefault() != null;
            if (exist)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE,
                    Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                };
            }

            List<C_IND_CERTIFICATE> draftListIp = SessionUtil.DraftList<C_IND_CERTIFICATE>(ApplicationConstant.DRAFT_KEY_C_IND_CERTIFICATE);
            bool existIp = draftListIp.Where(o => o.CANDIDATE_NUMBER == candidateNumber).FirstOrDefault() != null;
            if (existIp)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE,
                    Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                };
            }


            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES.Where(k => k.CANDIDATE_NUMBER == candidateNumber).FirstOrDefault();
                if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
                {

                    C_COMP_APPLICANT_INFO app = db.C_COMP_APPLICANT_INFO
                        .Where(o => o.CANDIDATE_NUMBER == candidateNumber)
                       .Include(o => o.C_S_SYSTEM_VALUE)
                       .Include(O => O.C_COMP_APPLICATION)
                       .Include(o => o.C_APPLICANT).FirstOrDefault();

                    

                    if (app == null)
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE,
                            Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                        };
                    app.TIMEDURATION = duration;
                    //app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = "Interview" };
                    if (cand != null)
                        app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = cand.INTERVIEW_TYPE == "I" ? "Interview" : "Assessment" };
                    else
                        app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = "Interview" };


                    draftList.Add(app);
                }
                else
                {
                    C_IND_CERTIFICATE app = db.C_IND_CERTIFICATE
                       .Where(o => o.CANDIDATE_NUMBER == candidateNumber)
                      .Include(o => o.C_S_SYSTEM_VALUE)
                      .Include(O => O.C_IND_APPLICATION)
                      .Include(o => o.C_IND_APPLICATION.C_APPLICANT).FirstOrDefault();
                    if (app == null)
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE,
                            Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                        };
                    app.TIMEDURATION = duration;
                    //app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = "Interview" };
                    if (cand != null)
                        app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = cand.INTERVIEW_TYPE == "I" ? "Interview" : "Assessment" };
                    else
                        app.C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES() { INTERVIEW_TYPE_DISPLAY = "Interview" };

                    draftListIp.Add(app);
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

            }

        }

        public CompICModel SearchIC_CGC(CompICModel model)
        {
            model.Query = SearchIC_q;
            model.QueryParameters.Add("RegType", RegistrationConstant.REGISTRATION_TYPE_CGA); //learn
            model.QueryWhere = SearchIC_whereQ(model);
            model.Search();
            return model;
        }

        public CompICModel ExportIC_CGC(CompICModel model)
        {
            model.Query = SearchIC_q;
            model.QueryParameters.Add("RegType", RegistrationConstant.REGISTRATION_TYPE_CGA); //learn
            model.QueryWhere = SearchIC_whereQ(model);
            model.Export("ExportFile");
            return model;
        }
        public CompICModel SearchIC_MW(CompICModel model)
        {
            model.Query = SearchIC_q;
            model.QueryParameters.Add("regType", RegistrationConstant.REGISTRATION_TYPE_MW); //learn
            model.QueryWhere = SearchIC_whereQ(model);
            model.Search();
            return model;
        }

        public CompICModel SearchIC_IP(CompICModel model)
        {
            model.Query = SearchIC_q;
            model.QueryParameters.Add("RegType", RegistrationConstant.REGISTRATION_TYPE_IP); //learn
            model.QueryWhere = SearchIC_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchIC_whereQ(CompICModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Year))
            {
                whereQ += "\r\n\t" + "AND UPPER(YEAR) LIKE :Year";
                model.QueryParameters.Add("Year", "%" + model.Year.Trim().ToUpper() + "%");
            }
            if (model.InterviewDate != null)
            {
                whereQ += "\r\n\t" + "AND sch.INTERVIEW_DATE >= :InterviewDate";
                model.QueryParameters.Add("InterviewDate", model.InterviewDate);
            }
            if (!string.IsNullOrWhiteSpace(model.Group))
            {
                whereQ += "\r\n\t" + "AND meeting.MEETING_GROUP = :InterviewGroup";
                model.QueryParameters.Add("InterviewGroup", model.Group);
            }
            if (!string.IsNullOrWhiteSpace(model.InterviewType))
            {
                whereQ += "\r\n\t" + "AND sv3.Code = :InterviewType";
                model.QueryParameters.Add("InterviewType", model.InterviewType);
            }
            if (!string.IsNullOrWhiteSpace(model.Type))
            {
                whereQ += "\r\n\t" + "AND sv1.Code = :Type";
                model.QueryParameters.Add("Type", model.Type);
            }

            return whereQ;
        }

        //Add to List
        public CompICModel SearchCandidateNoAddRecord(CompICModel model)
        {
            model.Query = SearchCandidateNo_q;
            model.QueryParameters.Add("CandidateNo", "10100"); //learn

            model.Search();
            return model;
        }

        public CompICModel Form(string id, string RegType)
        {
            CompICModel model = new CompICModel();
            model.RegType = RegType;
            C_INTERVIEW_CANDIDATES cand = getInterviewCandidatesByIvSchId(id);
            if(cand != null) {
                if (cand.RESULT_ID != null)
                {
                    model.CannotEditFlag = true;
                }
                else
                {
                    model.CannotEditFlag = false;
                }
            }
           
            

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                model.C_INTERVIEW_SCHEDULE = db.C_INTERVIEW_SCHEDULE.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE).Include(o => o.C_S_ROOM)
                    .Include(o => o.C_MEETING)
                    .FirstOrDefault();

                if(model.C_INTERVIEW_SCHEDULE.C_S_SYSTEM_VALUE != null)
                {
                    if (model.C_INTERVIEW_SCHEDULE.C_S_SYSTEM_VALUE.CODE == "PM")
                        model.StartTime = "02:15 PM";
                    else
                        model.StartTime = "09:15 AM";
                }
            }

            InitCandidates(id, RegType);
            return model;
        }

        public DisplayGrid AjaxCandidates(string C_INTERVIEW_SCHEDULE_UUID, string regType)
        {
            DisplayGrid grid = new DisplayGrid() { Data = new List<Dictionary<string, object>>() };
            if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
            {
                List<C_COMP_APPLICANT_INFO> draftList0 = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
                List<int?> delList = SessionUtil.DraftList<int?>(ApplicationConstant.DELETE_KEY_C_COMP_APPLICANT_INFO);
                List<C_COMP_APPLICANT_INFO> draftList = draftList0.Where(o => !delList.Contains(o.CANDIDATE_NUMBER)).ToList();

                if (draftList != null) for (int i = 0; i < draftList.Count; i++)
                    {
                        grid.Data.Add(new Dictionary<string, object>() {
                        {"FILE_REFERENCE_NO",draftList[i].C_COMP_APPLICATION?.FILE_REFERENCE_NO   },
                        {"CANDIDATE_NUMBER",draftList[i]?.CANDIDATE_NUMBER  },
                        {"INTERVIEW_TYPE",draftList[i].C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE_DISPLAY },
                        {"ALIAS_NUMBER",draftList[i].C_INTERVIEW_CANDIDATES.ALIAS_NUMBER   },
                        {"FUL_NAME",draftList[i].C_APPLICANT?.FULL_NAME_DISPLAY   },
                        {"CODE",draftList[i].C_S_SYSTEM_VALUE?.CODE   },
                        {"START_TIME",draftList[i].C_INTERVIEW_CANDIDATES.START_DATE_DISPLAY_TIME },
                        {"TIMEDURATION", draftList[i].TIMEDURATION }
                    });
                    }
            }
            else {
                List<C_IND_CERTIFICATE> draftList0 = SessionUtil.DraftList<C_IND_CERTIFICATE>(ApplicationConstant.DRAFT_KEY_C_IND_CERTIFICATE);
                List<long?> delList = SessionUtil.DraftList<long?>(ApplicationConstant.DELETE_KEY_C_IND_CERTIFICATE);
                List<C_IND_CERTIFICATE> draftList = draftList0.Where(o => !delList.Contains(o.CANDIDATE_NUMBER)).ToList();

                if (draftList != null) for (int i = 0; i < draftList.Count; i++)
                    {
                        grid.Data.Add(new Dictionary<string, object>() {
                        {"FILE_REFERENCE_NO",draftList[i].C_IND_APPLICATION?.FILE_REFERENCE_NO   },
                        {"CANDIDATE_NUMBER",draftList[i]?.CANDIDATE_NUMBER  },
                        {"INTERVIEW_TYPE",draftList[i].C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE_DISPLAY },
                        {"ALIAS_NUMBER",draftList[i].C_INTERVIEW_CANDIDATES.ALIAS_NUMBER   },
                        {"FUL_NAME",draftList[i].C_IND_APPLICATION.C_APPLICANT?.FULL_NAME_DISPLAY   },
                        {"CODE",draftList[i].C_S_SYSTEM_VALUE?.CODE   },
                        {"START_TIME",draftList[i].C_INTERVIEW_CANDIDATES.START_DATE_DISPLAY_TIME },
                        {"TIMEDURATION", draftList[i].TIMEDURATION }
                    });
                }
            }
               
            return grid;
        }
        
        public ServiceResult GenCandidates(CompICModel model, string regType)
        {
            //List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);

            List<C_COMP_APPLICANT_INFO> draftList0 = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            List<int?> delList = SessionUtil.DraftList<int?>(ApplicationConstant.DELETE_KEY_C_COMP_APPLICANT_INFO);
            List<C_COMP_APPLICANT_INFO> draftList = draftList0.Where(o => !delList.Contains(o.CANDIDATE_NUMBER)).ToList();

            List<C_IND_CERTIFICATE> draftList0Ip = SessionUtil.DraftList<C_IND_CERTIFICATE>(ApplicationConstant.DRAFT_KEY_C_IND_CERTIFICATE);
            List<long?> delListIp = SessionUtil.DraftList<long?>(ApplicationConstant.DELETE_KEY_C_IND_CERTIFICATE);
            List<C_IND_CERTIFICATE> draftListIp = draftList0Ip.Where(o => !delListIp.Contains(o.CANDIDATE_NUMBER)).ToList();

            DateTime time = DateTime.ParseExact(model.StartTime, "hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));

            if (model.GenData != null) foreach (string key in model.GenData.Keys)
            {
                if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW)) {
                        C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = draftList.Where(o => o.CANDIDATE_NUMBER.ToString() == key).FirstOrDefault();
                        if (C_COMP_APPLICANT_INFO != null)
                        {
                            C_COMP_APPLICANT_INFO.TIMEDURATION = int.Parse(model.GenData[key]["DURATION"]);
                            C_COMP_APPLICANT_INFO.C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE = model.GenData[key]["TYPE"];
                            C_COMP_APPLICANT_INFO.C_INTERVIEW_CANDIDATES.ALIAS_NUMBER = model.GenData[key]["ALIAS"];
                        }
                        for (int i = 0; i < draftList.Count; i++)
                        {
                            draftList[i].C_INTERVIEW_CANDIDATES.START_DATE = time;
                            time = time.AddMinutes(draftList[i].TIMEDURATION);
                        }
                    }
                else {
                        C_IND_CERTIFICATE C_IND_CERTIFICATE = draftListIp.Where(o => o.CANDIDATE_NUMBER.ToString() == key).FirstOrDefault();
                        if (C_IND_CERTIFICATE != null)
                        {
                            C_IND_CERTIFICATE.TIMEDURATION = int.Parse(model.GenData[key]["DURATION"]);
                            C_IND_CERTIFICATE.C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE = model.GenData[key]["TYPE"];
                            C_IND_CERTIFICATE.C_INTERVIEW_CANDIDATES.ALIAS_NUMBER = model.GenData[key]["ALIAS"];
                        }
                        for (int i = 0; i < draftListIp.Count; i++)
                        {
                            draftListIp[i].C_INTERVIEW_CANDIDATES.START_DATE = time;
                            time = time.AddMinutes(draftListIp[i].TIMEDURATION);
                        }

                    }
              
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult Save(CompICModel model, string RegType)
        {
            string meetingNo = model.C_INTERVIEW_SCHEDULE.MEETING_NUMBER;

            List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            List<int?> delList = SessionUtil.DraftList<int?>(ApplicationConstant.DELETE_KEY_C_COMP_APPLICANT_INFO);

            List<C_IND_CERTIFICATE> draftListIp = SessionUtil.DraftList<C_IND_CERTIFICATE>(ApplicationConstant.DRAFT_KEY_C_IND_CERTIFICATE);
            List<long?> delListIp = SessionUtil.DraftList<long?>(ApplicationConstant.DELETE_KEY_C_IND_CERTIFICATE);

            

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_SCHEDULE sch = db.C_INTERVIEW_SCHEDULE.Find(model.C_INTERVIEW_SCHEDULE.UUID);
                string[] meet = sch.MEETING_NUMBER.Split('-');

                if (RegType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || RegType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
                {
                    for (int i = 0; i < delList.Count; i++)
                    {
                        int j = delList[i].Value;
                        C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES
                            .Where(o => o.CANDIDATE_NUMBER == j)
                            .Where(o => o.INTERVIEW_SCHEDULE_ID == model.C_INTERVIEW_SCHEDULE.UUID || o.MEETING_NUMBER == model.C_INTERVIEW_SCHEDULE.MEETING_NUMBER)
                            .FirstOrDefault();
                        db.C_INTERVIEW_CANDIDATES.Remove(cand);
                    }

                    for (int i = 0; i < draftList.Count; i++)
                    {
                        C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES.Find(draftList[i].C_INTERVIEW_CANDIDATES.UUID);
                        if (cand == null)
                        {
                            cand = new C_INTERVIEW_CANDIDATES();
                            cand.REGISTRATION_TYPE = RegType;
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
                }
                else
                {
                    for (int i = 0; i < delListIp.Count; i++)
                    {
                        long j = delListIp[i].Value;
                        C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES
                            .Where(o => o.CANDIDATE_NUMBER == j)
                            .Where(o => o.INTERVIEW_SCHEDULE_ID == model.C_INTERVIEW_SCHEDULE.UUID || o.MEETING_NUMBER == model.C_INTERVIEW_SCHEDULE.MEETING_NUMBER)
                            .FirstOrDefault();
                        db.C_INTERVIEW_CANDIDATES.Remove(cand);
                    }

                    for (int i = 0; i < draftListIp.Count; i++)
                    {
                        C_INTERVIEW_CANDIDATES cand = db.C_INTERVIEW_CANDIDATES.Find(draftListIp[i].C_INTERVIEW_CANDIDATES.UUID);
                        if (cand == null)
                        {
                            cand = new C_INTERVIEW_CANDIDATES();
                            cand.REGISTRATION_TYPE = RegType;
                            cand.INTERVIEW_SCHEDULE_ID = model.C_INTERVIEW_SCHEDULE.UUID;
                            db.C_INTERVIEW_CANDIDATES.Add(cand);
                            cand.CANDIDATE_NUMBER = (int)draftListIp[i].CANDIDATE_NUMBER.Value;
                        }
                        cand.ALIAS_NUMBER = draftListIp[i].C_INTERVIEW_CANDIDATES.ALIAS_NUMBER;
                        cand.INTERVIEW_TYPE = draftListIp[i].C_INTERVIEW_CANDIDATES.INTERVIEW_TYPE;
                        cand.START_DATE = draftListIp[i].C_INTERVIEW_CANDIDATES.START_DATE;
                        cand.END_DATE = draftListIp[i].C_INTERVIEW_CANDIDATES.START_DATE.Value.AddMinutes(draftListIp[i].TIMEDURATION);

                        cand.INTERVIEW_NUMBER = meet[0] + "-" + cand.CANDIDATE_NUMBER + "-" + meet[1] + "-" + meet[2];
                        if (cand.INTERVIEW_TYPE == "A") cand.INTERVIEW_NUMBER += "-A";
                        cand.ASSESSMENT_NUMBER = meet[0] + "-" + cand.CANDIDATE_NUMBER + "-" + meet[1];
                    }
                }
                
                db.SaveChanges();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

        public void DeleteRecord(int CandidateNo)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_CANDIDATES record = db.C_INTERVIEW_CANDIDATES.Where(o => o.CANDIDATE_NUMBER == CandidateNo).FirstOrDefault();
                db.C_INTERVIEW_CANDIDATES.Remove(record);
                db.SaveChanges();

            }
        }

        public ServiceResult DeleteForm(string meetingNo)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_INTERVIEW_SCHEDULE record = db.C_INTERVIEW_SCHEDULE.Where(o => o.MEETING_NUMBER == meetingNo).FirstOrDefault();
                var candidates = db.C_INTERVIEW_CANDIDATES.Where(x => x.INTERVIEW_SCHEDULE_ID == record.UUID);
                db.C_INTERVIEW_CANDIDATES.RemoveRange(candidates);
                db.C_INTERVIEW_SCHEDULE.Remove(record);
                db.SaveChanges();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

       
        public ServiceResult DeteSession(int CandNo, string RegType)
        {

           // List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            List<int?> deleteList = SessionUtil.DraftList<int?>(ApplicationConstant.DELETE_KEY_C_COMP_APPLICANT_INFO);
            List<long?> deleteListIp = SessionUtil.DraftList<long?>(ApplicationConstant.DELETE_KEY_C_IND_CERTIFICATE);

            if (RegType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || RegType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
            {
                deleteList.Add(CandNo);
            }
            else
            {
                deleteListIp.Add(CandNo);
            }
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

        }

        public ServiceResult InitCandidates(string C_INTERVIEW_SCHEDULE_UUID, string regType)
        {
            List<C_COMP_APPLICANT_INFO> draftList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
            List<C_IND_CERTIFICATE> draftListIp = SessionUtil.DraftList<C_IND_CERTIFICATE>(ApplicationConstant.DRAFT_KEY_C_IND_CERTIFICATE);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
                {
                    if (draftList.Count == 0)
                    {
                        List<C_INTERVIEW_CANDIDATES> C_INTERVIEW_CANDIDATESs =
                        db.C_INTERVIEW_CANDIDATES
                        .Where(o => o.INTERVIEW_SCHEDULE_ID == C_INTERVIEW_SCHEDULE_UUID)
                        .OrderBy(o => o.START_DATE)
                        .ToList();
                        for (int i = 0; i < C_INTERVIEW_CANDIDATESs.Count; i++)
                        {
                            int canNo = C_INTERVIEW_CANDIDATESs[i].CANDIDATE_NUMBER;
                            C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = db.C_COMP_APPLICANT_INFO
                                .Where(o => o.CANDIDATE_NUMBER == canNo)
                                .Include(o => o.C_COMP_APPLICATION)
                                .Include(o => o.C_APPLICANT)
                                .Include(o => o.C_S_SYSTEM_VALUE).FirstOrDefault();
                            C_COMP_APPLICANT_INFO.C_INTERVIEW_CANDIDATES = C_INTERVIEW_CANDIDATESs[i];
                            draftList.Add(C_COMP_APPLICANT_INFO);
                        }
                    }
                }
                else if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_IP))
                {
                    if (draftListIp.Count == 0)
                    {
                        List<C_INTERVIEW_CANDIDATES> C_INTERVIEW_CANDIDATESs =
                        db.C_INTERVIEW_CANDIDATES
                        .Where(o => o.INTERVIEW_SCHEDULE_ID == C_INTERVIEW_SCHEDULE_UUID)
                        .OrderBy(o => o.START_DATE)
                        .ToList();
                        for (int i = 0; i < C_INTERVIEW_CANDIDATESs.Count; i++)
                        {
                            long canNo = C_INTERVIEW_CANDIDATESs[i].CANDIDATE_NUMBER;
                            C_IND_CERTIFICATE C_IND_CERTIFICATE = db.C_IND_CERTIFICATE
                                .Where(o => o.CANDIDATE_NUMBER == canNo)
                                .Include(o => o.C_IND_APPLICATION)
                                .Include(o => o.C_IND_APPLICATION.C_APPLICANT)
                                .Include(o => o.C_S_SYSTEM_VALUE).FirstOrDefault();
                            C_IND_CERTIFICATE.C_INTERVIEW_CANDIDATES = C_INTERVIEW_CANDIDATESs[i];
                            draftListIp.Add(C_IND_CERTIFICATE);
                        }
                    }
                   
                }
            }
            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
            };
        }

        public List<List<object>> getExportInterviewCandidatesByInterviewScheduleComp(string MeetingNumber)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();
                    string query = Query_CRCMinutes; 
                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    queryParameters.Add("MeetingNumber", MeetingNumber);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, query, queryParameters);
                    data = CommonUtil.convertToList(dr);

                    return data;
                }
            }
        }
        public List<List<object>> getExportInterviewCandidatesByInterviewScheduleProf(string MeetingNumber)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();
                    string query = Query_CRCMinutesProf;
                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    queryParameters.Add("MeetingNumber", MeetingNumber);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, query, queryParameters);
                    data = CommonUtil.convertToList(dr);

                    return data;
                }
            }
        }

        // FileStreamResult
        public string ExportCRCMinute(CompICModel model, string MeetingNumber)
        {
            RegistrationMeetingMemberService mms = new RegistrationMeetingMemberService();

            string content = "";
            List<List<object>> data = new List<List<object>>();
            List<List<object>> members = null;
            List<string> headercolumn = new List<string>();
            List<string> contentcolumn = new List<string>();

            if (RegistrationConstant.REGISTRATION_TYPE_CGA.Equals(model.RegType) || RegistrationConstant.REGISTRATION_TYPE_MWCA.Equals(model.RegType))
            {
                data = getExportInterviewCandidatesByInterviewScheduleComp(MeetingNumber);

                List<string> headerList1 = new List<string>() {

                    "crc", "crc_no", "inter_date", "inter_time", "room_no"
                };
                headercolumn.AddRange(headerList1);
                if(data != null && data.Count() > 0)
                {
                    if(data[0] != null)
                    {
                        for(int i = 0; i < 5; i++) // 0-4
                        {
                            contentcolumn.Add(appendDoubleQuote(data[0][i].ToString()));
                        }
                    }
                }

                List<string> headerList2 = new List<string>() {
                    "file_ref", "interv_no", "comp_name", "name","nameTD", "time", "edesc"
                };
                for (int x = 1; x <= 30; x++)
                {
                    for (int i = 0; i < headerList2.Count; i++)
                    {
                        headercolumn.Add(headerList2.ElementAt(i) + x);
                    }
                }
                if (data != null && data.Count() > 0)
                {
                    int j = 0;
                    for(; j < data.Count(); j++)
                    {
                        for (int i = 7; i < 14; i++) // 7-13
                        {
                            contentcolumn.Add(appendDoubleQuote(data[j][i].ToString()));
                        }
                    }
                    for(int k = j; k < 30; k++)
                    {
                        string temp = appendDoubleQuote("");
                        List<string> emptyStrings = new List<string>() { temp, temp, temp, temp, temp, temp, temp };
                        contentcolumn.AddRange(emptyStrings);
                    }
                }

                List<string> headerList3 = new List<string>()
                {
                    "Chairman", "ViceChairman1", "ViceChairman2", "member1",
                    "member2", "member3", "member4", "member5", "BAPost", "BARank",
                    "BA", "Secretary"
                };
                headercolumn.AddRange(headerList3);

                //role code = 4
                string chairman = "";
                //role code = 7
                //string viceChairman = "";
                List<string> viceChairmanList = new List<string>();
                //role code = 3,6
                //string member = "";
                List<string> memberList = new List<string>();
                //role code = 1
                string baPost = "";
                string baRank = "";
                string ba = "";
                //role code = 2
                string secretary = "";

                string comma = ", ";

                members = mms.getMeetingMemberForExport(getMeetingIdByMeetingNumber(MeetingNumber));
                if (members != null && members.Count() > 0)
                {
                    string tempUuid = "";
                    for(int i = 0; i < members.Count(); i++)
                    {
                        List<object> objects = members[i];
                        string uuid = objects[0].ToString();
                        string name = objects[1].ToString();
                        string soc = objects[2].ToString();
                        string role = objects[3].ToString();
                        string post = objects[4].ToString();
                        string rank = objects[5].ToString();

                        if(!tempUuid.Equals(uuid))
                        {
                            tempUuid = uuid;
                            if("1".Equals(role))
                            {
                                if(!string.IsNullOrWhiteSpace(rank))
                                {
                                    baRank = name + comma + rank;
                                }
                                if(!string.IsNullOrWhiteSpace(post))
                                {
                                    baPost = name + comma + post;
                                }
                                ba = name;
                            }
                            else if("2".Equals(role)) {
                                secretary = name + (soc == null ? "" : (comma + soc));
                            }
                            else if("3".Equals(role) || "6".Equals(role))
                            {
                                memberList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                            else if("4".Equals(role))
                            {
                                chairman = name + (soc == null ? "" : (comma + soc));
                            }
                            else if("7".Equals(role))
                            {
                                viceChairmanList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                        }
                        else
                        {
                            if("2".Equals(role))
                            {
                                secretary = secretary + comma + soc;
                            }
                            else if("3".Equals(role) || "6".Equals(role))
                            {
                                memberList.Add(name + comma + soc);
                            }
                            else if("4".Equals(role))
                            {
                                chairman = chairman + comma + soc;
                            }
                            else if("7".Equals(role))
                            {
                                viceChairmanList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                        }
                    }
                }

                int mi = 1;
                if(string.IsNullOrWhiteSpace(chairman)) // no chairman
                {
                    if(memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi-1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                else
                {
                    contentcolumn.Add(appendDoubleQuote(chairman));
                }
                if(viceChairmanList.Count() >= 1)
                {
                    contentcolumn.Add(appendDoubleQuote(viceChairmanList[0]));                   
                }
                else
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi-1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                if(viceChairmanList.Count() >= 2)
                {
                    contentcolumn.Add(appendDoubleQuote(viceChairmanList[1]));
                }
                else
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi-1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                for(int i = 0; i < 5; i++)
                {
                    if(memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi-1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                contentcolumn.Add(appendDoubleQuote(baPost));
                contentcolumn.Add(appendDoubleQuote(baRank));
                contentcolumn.Add(appendDoubleQuote(ba));
                contentcolumn.Add(appendDoubleQuote(secretary));

                foreach(var column in headercolumn)
                {
                    content = appendICContent(content, column);
                }
                content += "\r\n";
                var c = 0;
                foreach(var column in contentcolumn)
                {
                    if (c == 0)
                    {
                        content += column;
                    }
                    else
                    {
                        content = appendICContent(content, column);
                    }
                    c++;
                }
                //string content = headercolumn + "\r\n" + contentcolumn;
            }
            else // RegistrationConstant.REGISTRATION_TYPE_IP || RegistrationConstant.REGISTRATION_TYPE_MWIA
            {
                data = getExportInterviewCandidatesByInterviewScheduleProf(MeetingNumber);

                List<string> headerList1 = new List<string>() {

                    "crc", "crc_no", "inter_date", "inter_time", "room_no"
                };
                headercolumn.AddRange(headerList1);
                if (data != null && data.Count() > 0)
                {
                    if (data[0] != null)
                    {
                        for (int i = 0; i < 5; i++) // 0-4
                        {
                            contentcolumn.Add(appendDoubleQuote(data[0][i].ToString()));
                        }
                    }
                }

                List<string> headerList2 = new List<string>() {
                    "file_ref", "interv_no", "comp_name", "name","nameTD", "time", "edesc", "date_of_reg"
                };
                for (int x = 1; x <= 30; x++)
                {
                    for (int i = 0; i < headerList2.Count; i++)
                    {
                        headercolumn.Add(headerList2.ElementAt(i) + x);
                    }
                }
                if (data != null && data.Count() > 0)
                {
                    int j = 0;
                    for (; j < data.Count(); j++)
                    {
                        for (int i = 5; i < 12; i++) // 5-12
                        {
                            contentcolumn.Add(appendDoubleQuote(data[j][i].ToString()));
                        }
                    }
                    for (int k = j; k < 30; k++)
                    {
                        string temp = appendDoubleQuote("");
                        List<string> emptyStrings = new List<string>() { temp, temp, temp, temp, temp, temp, temp };
                        contentcolumn.AddRange(emptyStrings);
                    }
                }

                List<string> headerList3 = new List<string>()
                {
                    "Chairman", "ViceChairman1", "ViceChairman2", "member1",
                    "member2", "member3", "member4", "member5", "BAPost", "BARank",
                    "BA", "Secretary"
                };
                headercolumn.AddRange(headerList3);

                //role code = 4
                string chairman = "";
                //role code = 7
                //string viceChairman = "";
                List<string> viceChairmanList = new List<string>();
                //role code = 3,6
                //string member = "";
                List<string> memberList = new List<string>();
                //role code = 1
                string baPost = "";
                string baRank = "";
                string ba = "";
                //role code = 2
                string secretary = "";

                string comma = ", ";

                members = mms.getMeetingMemberForExport(getMeetingIdByMeetingNumber(MeetingNumber));
                if (members != null && members.Count() > 0)
                {
                    string tempUuid = "";
                    for (int i = 0; i < members.Count(); i++)
                    {
                        List<object> objects = members[i];
                        string uuid = objects[0].ToString();
                        string name = objects[1].ToString();
                        string soc = objects[2].ToString();
                        string role = objects[3].ToString();
                        string post = objects[4].ToString();
                        string rank = objects[5].ToString();

                        if (!tempUuid.Equals(uuid))
                        {
                            tempUuid = uuid;
                            if ("1".Equals(role))
                            {
                                if (!string.IsNullOrWhiteSpace(rank))
                                {
                                    baRank = name + comma + rank;
                                }
                                if (!string.IsNullOrWhiteSpace(post))
                                {
                                    baPost = name + comma + post;
                                }
                                ba = name;
                            }
                            else if ("2".Equals(role))
                            {
                                secretary = name + (soc == null ? "" : (comma + soc));
                            }
                            else if ("3".Equals(role) || "6".Equals(role))
                            {
                                memberList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                            else if ("4".Equals(role))
                            {
                                chairman = name + (soc == null ? "" : (comma + soc));
                            }
                            else if ("7".Equals(role))
                            {
                                viceChairmanList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                        }
                        else
                        {
                            if ("2".Equals(role))
                            {
                                secretary = secretary + comma + soc;
                            }
                            else if ("3".Equals(role) || "6".Equals(role))
                            {
                                memberList.Add(name + comma + soc);
                            }
                            else if ("4".Equals(role))
                            {
                                chairman = chairman + comma + soc;
                            }
                            else if ("7".Equals(role))
                            {
                                viceChairmanList.Add(name + (soc == null ? "" : (comma + soc)));
                            }
                        }
                    }
                }

                int mi = 1;
                if (string.IsNullOrWhiteSpace(chairman)) // no chairman
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi - 1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                else
                {
                    contentcolumn.Add(appendDoubleQuote(chairman));
                }
                if (viceChairmanList.Count() >= 1)
                {
                    contentcolumn.Add(appendDoubleQuote(viceChairmanList[0]));
                }
                else
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi - 1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                if (viceChairmanList.Count() >= 2)
                {
                    contentcolumn.Add(appendDoubleQuote(viceChairmanList[1]));
                }
                else
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi - 1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                for (int i = 0; i < memberList.Count(); i++)
                {
                    if (memberList.Count() > mi)
                    {
                        contentcolumn.Add(appendDoubleQuote(memberList[mi - 1]));
                        mi++;
                    }
                    else
                    {
                        contentcolumn.Add(appendDoubleQuote(""));
                    }
                }
                //for (int i = 0; i < 5; i++)
                //{
                //    if (memberList.Count() > mi)
                //    {
                //        contentcolumn.Add(appendDoubleQuote(memberList[i]));
                //    }
                //    else
                //    {
                //        contentcolumn.Add(appendDoubleQuote(""));
                //    }
                //}
                contentcolumn.Add(appendDoubleQuote(baPost));
                contentcolumn.Add(appendDoubleQuote(baRank));
                contentcolumn.Add(appendDoubleQuote(ba));
                contentcolumn.Add(appendDoubleQuote(secretary));

                foreach (var column in headercolumn)
                {
                    content = appendICContent(content, column);
                }
                content += "\r\n";
                var c = 0;
                foreach (var column in contentcolumn)
                {
                    if (c == 0)
                    {
                        content += column;
                    }
                    else
                    {
                        content = appendICContent(content, column);
                    }
                    c++;
                }
            }
            return content;
        }

        public List<List<object>> getApplicantsByMeetingNumber(string MeetingNumber)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();

                    string sql = Query_MeetingGrpCommittee;
                    Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    dataInput.Add("MeetingNumber", MeetingNumber);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, dataInput);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();

                    return data;
                }
            }
        }

        public string ExportMeetingGroupCommittee(CompICModel model, string MeetingNumber)
        {
            List<string> headerList = new List<string>() {

               "Title", "Last_Name", "First_Name", "English_Name", "Correspondence_Address_for_General_Deliv",
               "Date", "DD", "MMMM", "YYYY", "weekDay", "Time", "Room_Number", "Role", "Society",
               "Secretary_Last_Name", "Secretary_First_Name", "Secretary_English_Name", "Meeting_No"
            };

            List<List<object>> data = getApplicantsByMeetingNumber(MeetingNumber);
            return Exportfor_MeetingGroupCommittee(headerList, data);
        }

        public List<List<object>> getExportRenewalRestoration(string UUID)
        {
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

                    return data;
                }
            }

        }

        public string ExportRenewalRestoration(CompICModel model, string UUID)
        {
            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "secretary", "role"

            };

            List<List<object>> data = getExportRenewalRestoration(UUID);
            return Exportfor_RenewalRestoration_Interview_Assessment(headerList, data);

        }

        public List<List<object>> getExportInterview(string UUID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();

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

                    return data;
                }
            }
        }

        public string ExportInterview(CompICModel dataExport, string UUID)
        {
            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "cc_n1", "cc_n2", "cc_n3", "cc_n4", "cc_n5", "cc_n6", "cc_n7",
               "cc_n8", "cc_n9", "cc_n10", "cc_n11", "cc_n12", "cc_n13", "cc_n14", "cc_n15", "secretary", "role", "Assessment"

            };

            List<List<object>> data = getExportInterview(UUID);
            return Exportfor_RenewalRestoration_Interview_Assessment(headerList, data);
        }

        public List<List<object>> getAssessment(string UUID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();

                    String sql = this.Query_ExportAssessment;
                    Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    dataInput.Add("interviewScheduleId", UUID);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, dataInput);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();

                    return data;
                }
            }
        }

        public string ExportAssessment(CompICModel dataExport, string UUID)
        {
            List<string> headerList = new List<string>() {

               "file_ref","inter_no", "company_name", "surname", "officer","inter_time","inter_date",
               "address1", "address2", "address3", "address4", "address5","caddress1", "caddress2",
               "caddress3", "caddress4","caddress5","fax_no", "title", "room_no", "bd_address",
               "report_to", "secretary", "role"

            };

            List<List<object>> data = getAssessment(UUID);
            return Exportfor_RenewalRestoration_Interview_Assessment(headerList, data);
        }

        public string Exportfor_MeetingGroupCommittee(List<string> headerList, List<List<object>> data)
        {
            string content = "";

            List<string> contentcolumn = new List<string>();
            List<string> headercolumn = headerList;

            if (data != null && data.Count() > 0)
            {
                for (int j = 0; j < data.Count(); j++)
                {
                    List<object> temp = data[j];
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        contentcolumn.Add(temp[i].ToString());
                    }
                    contentcolumn.Add("\r\n");
                }
            }

            foreach (var column in headercolumn)
            {
                content = appendICContent(content, column);
            }
            content += "\r\n";
            bool isNewLine = true;
            foreach (var column in contentcolumn)
            {
                if ("\r\n".Equals(column))
                {
                    content += "\r\n";
                    isNewLine = true;
                    continue;
                }
                if (isNewLine)
                {
                    content += appendDoubleQuote(column);
                }
                else
                {
                    content = appendICContent(content, appendDoubleQuote(column));
                }
                isNewLine = false;
            }

            return content;
        }

        public string Exportfor_RenewalRestoration_Interview_Assessment(List<string> headerList, List<List<object>> data)
        {
            string content = "";

            List<string> headercolumn = headerList;
            List<string> contentcolumn = new List<string>();

            if (data != null && data.Count() > 0)
            {
                for (int j = 0; j < data.Count(); j++)
                {
                    List<object> temp = data[j];
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        contentcolumn.Add(temp[i].ToString());
                    }
                    contentcolumn.Add("\r\n");
                }
            }

            foreach (var column in headercolumn)
            {
                content = appendCertContent(content, column);
            }
            content += "\r\n";
            bool isNewLine = true;
            foreach (var column in contentcolumn)
            {
                if ("\r\n".Equals(column))
                {
                    content += "\r\n";
                    isNewLine = true;
                    continue;
                }
                if (isNewLine)
                {
                    content += appendDoubleQuote(column);
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(column));
                }
                isNewLine = false;
            }

            return content;
        }

       public string getMeetingIdByMeetingNumber(string meetingNumber)
       {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_INTERVIEW_SCHEDULE.Where(x => x.MEETING_NUMBER == meetingNumber).FirstOrDefault();

                return query != null ? query.MEETING_ID : null;
            }
       }
    }
}