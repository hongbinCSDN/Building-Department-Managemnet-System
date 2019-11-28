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

namespace MWMS2.Services
{
    
    public class RegistrationCompIRService : RegistrationCommonService
    {
        String SearchIC_q = ""
                       + "\r\n" + "\t" + "SELECT sch.UUID, meeting.YEAR as YEAR, meeting.MEETING_GROUP as MEETING_GROUP,  "
                       + "\r\n" + "\t" + "sch.MEETING_NUMBER as MEETING_NUMBER, sch.INTERVIEW_DATE as INTERVIEW_DATE,    "
                       + "\r\n" + "\t" + "sv2.CODE as CODE, room.ROOM as ROOM, sch.IS_CANCEL as IS_CANCELED,              "
                       + "\r\n" + "\t" + "sv1.Code as TYPECGC             "
                       + "\r\n" + "\t" + "FROM C_INTERVIEW_SCHEDULE sch                                                  "
                       + "\r\n" + "\t" + "LEFT JOIN C_MEETING meeting on meeting.UUID = sch.MEETING_ID                   "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv1 on sv1.UUID = meeting.COMMITTEE_TYPE_ID         "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_ROOM room on room.UUID = sch.ROOM_ID                             "
                       + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE sv2 on sv2.UUID = sch.TIME_SESSION_ID               "
                       + "\r\n" + "\t" + "WHERE 1=1                                                                      "
                       + "\r\n" + "\t" + "AND sv1.REGISTRATION_TYPE = :RegType                                            ";

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

        public InterviewResultSearchModel SearchInterviewResultForIndividual(InterviewResultSearchModel model)
        {

            String q = ""
             + "\r\n" + "\t" + "SELECT indCan.UUID as INTERVIEW_CANDIDATES_UUID, appli.FILE_REFERENCE_NO as FILE_REFERENCE_NO "
             + "\r\n" + "\t" + ", indCan.START_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", to_char(indCan.START_DATE, 'DD/MM/YYYY HH:MI AM') as INTERVIEW_DATE_DISPLAY "
             + "\r\n" + "\t" + ", indCan.INTERVIEW_NUMBER as INTERVIEW_NUMBER "
             + "\r\n" + "\t" + ", (case when indCan.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end) as INTERVIEW_TYPE "
             + "\r\n" + "\t" + ", indCan.RESULT_DATE as RESULT_DATE "
             + "\r\n" + "\t" + ", (case when indCan.IS_ABSENT = 'N' then 'No' else '' end) as IS_ABSENT "
             + "\r\n" + "\t" + ", sv.ENGLISH_DESCRIPTION as RESULT, apnt.SURNAME, apnt.GIVEN_NAME_ON_ID "
             + "\r\n" + "\t" + ", (apnt.SURNAME ||' '||apnt.GIVEN_NAME_ON_ID) as FULL_NAME , sCode.CODE as ROLE "
             + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES indCan "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.uuid = indCan.RESULT_ID "
             + "\r\n" + "\t" + ", C_IND_CERTIFICATE indCert "
             + "\r\n" + "\t" + "left join C_INd_APPLICATION appli on appli.UUID = indCert.MASTER_ID "
             + "\r\n" + "\t" + "left join C_APPLICANT apnt on apnt.uuid = appli.APPLICANT_ID "
             + "\r\n" + "\t" + "left join C_S_CATEGORY_CODE sCode on sCode.uuid = indCert.CATEGORY_ID "
             + "\r\n" + "\t" + "where 1=1 "
             + "\r\n" + "\t" + "and indCan.CANDIDATE_NUMBER = indCert.CANDIDATE_NUMBER ";

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
                q += "\r\n" + "\t" + " AND TO_CHAR(indCan.START_DATE, 'YYYY') = :year";
                model.QueryParameters.Add("year", year);
            }
            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND indCan.START_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND indCan.START_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }
            //if (!String.IsNullOrEmpty(group))
            //{
            //    q += "\r\n" + "\t" + " AND commGrp.NAME = :grpName";
            //    model.QueryParameters.Add("grpName", group);
            //}
            if (!String.IsNullOrEmpty(type))
            {
                q += "\r\n" + "\t" + " AND sv.uuid = :type";
                model.QueryParameters.Add("type", type);
            }
            if (!String.IsNullOrEmpty(givenName))
            {
                q += "\r\n" + "\t" + " AND apnt.GIVEN_NAME_ON_ID like :givenName";
                model.QueryParameters.Add("givenName", "%" + givenName + "%");
            }
            if (!String.IsNullOrEmpty(surnName))
            {
                q += "\r\n" + "\t" + " AND apnt.SURNAME like :surnName";
                model.QueryParameters.Add("surnName", "%" + surnName + "%");
            }
            if (!String.IsNullOrEmpty(fileRef))
            {
                q += "\r\n" + "\t" + " AND appli.file_Reference_No like :fileRef";
                model.QueryParameters.Add("fileRef", "%" + fileRef + "%");
            }
            if (!String.IsNullOrEmpty(interviewNo))
            {
                q += "\r\n" + "\t" + " AND indCan.interview_Number like :interviewNo";
                model.QueryParameters.Add("interviewNo", "%" + interviewNo + "%");
            }
            if (!String.IsNullOrEmpty(HKID))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.HKID") + " like :HKID";
                model.QueryParameters.Add("HKID", "%" + HKID + "%");
            }
            if (!String.IsNullOrEmpty(passportNo))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.PASSPORT_NO") + " like :passportNo";
                model.QueryParameters.Add("HKID", "%" + passportNo + "%");
            }

            model.Query = q;
            model.Search();

            return model;
        }
        public string ExportInterviewResultForIndividual(InterviewResultSearchModel model)
        {

            String q = ""
             + "\r\n" + "\t" + "SELECT indCan.UUID as INTERVIEW_CANDIDATES_UUID, appli.FILE_REFERENCE_NO as FILE_REFERENCE_NO "
             + "\r\n" + "\t" + ", indCan.START_DATE as INTERVIEW_DATE "
             + "\r\n" + "\t" + ", to_char(indCan.START_DATE, 'DD/MM/YYYY HH:MI AM') as INTERVIEW_DATE_DISPLAY "
             + "\r\n" + "\t" + ", indCan.INTERVIEW_NUMBER as INTERVIEW_NUMBER "
             + "\r\n" + "\t" + ", (case when indCan.INTERVIEW_TYPE = 'I' then 'Interview' else '' end) as INTERVIEW_TYPE "
             + "\r\n" + "\t" + ", indCan.RESULT_DATE as RESULT_DATE "
             + "\r\n" + "\t" + ", (case when indCan.IS_ABSENT = 'N' then 'No' else '' end) as IS_ABSENT "
             + "\r\n" + "\t" + ", sv.ENGLISH_DESCRIPTION as RESULT, apnt.SURNAME, apnt.GIVEN_NAME_ON_ID "
             + "\r\n" + "\t" + ", (apnt.SURNAME ||' '||apnt.GIVEN_NAME_ON_ID) as FULL_NAME , sCode.CODE as CATEGORY_CODE "
             + "\r\n" + "\t" + "FROM C_INTERVIEW_CANDIDATES indCan "
             + "\r\n" + "\t" + "left join C_S_SYSTEM_VALUE sv on sv.uuid = indCan.RESULT_ID "
             + "\r\n" + "\t" + ", C_IND_CERTIFICATE indCert "
             + "\r\n" + "\t" + "left join C_INd_APPLICATION appli on appli.UUID = indCert.MASTER_ID "
             + "\r\n" + "\t" + "left join C_APPLICANT apnt on apnt.uuid = appli.APPLICANT_ID "
             + "\r\n" + "\t" + "left join C_S_CATEGORY_CODE sCode on sCode.uuid = indCert.CATEGORY_ID "
             + "\r\n" + "\t" + "where 1=1 "
             + "\r\n" + "\t" + "and indCan.CANDIDATE_NUMBER = indCert.CANDIDATE_NUMBER ";

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
                q += "\r\n" + "\t" + " AND TO_CHAR(indCan.START_DATE, 'YYYY') = :year";
                model.QueryParameters.Add("year", year);
            }
            if (dateFrom != null)
            {
                q += "\r\n" + "\t" + " AND indCan.START_DATE >= :dateFrom";
                model.QueryParameters.Add("dateFrom", dateFrom);
            }
            if (dateTo != null)
            {
                q += "\r\n" + "\t" + " AND indCan.START_DATE <= :dateTo";
                model.QueryParameters.Add("dateTo", dateTo);
            }
            //if (!String.IsNullOrEmpty(group))
            //{
            //    q += "\r\n" + "\t" + " AND commGrp.NAME = :grpName";
            //    model.QueryParameters.Add("grpName", group);
            //}
            if (!String.IsNullOrEmpty(type))
            {
                q += "\r\n" + "\t" + " AND sv.uuid = :type";
                model.QueryParameters.Add("type", type);
            }
            if (!String.IsNullOrEmpty(givenName))
            {
                q += "\r\n" + "\t" + " AND apnt.GIVEN_NAME_ON_ID like :givenName";
                model.QueryParameters.Add("givenName", "%" + givenName + "%");
            }
            if (!String.IsNullOrEmpty(surnName))
            {
                q += "\r\n" + "\t" + " AND apnt.SURNAME like :surnName";
                model.QueryParameters.Add("surnName", "%" + surnName + "%");
            }
            if (!String.IsNullOrEmpty(fileRef))
            {
                q += "\r\n" + "\t" + " AND appli.file_Reference_No like :fileRef";
                model.QueryParameters.Add("fileRef", "%" + fileRef + "%");
            }
            if (!String.IsNullOrEmpty(interviewNo))
            {
                q += "\r\n" + "\t" + " AND indCan.interview_Number like :interviewNo";
                model.QueryParameters.Add("interviewNo", "%" + interviewNo + "%");
            }
            if (!String.IsNullOrEmpty(HKID))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.HKID") + " like :HKID";
                model.QueryParameters.Add("HKID", "%" + HKID + "%");
            }
            if (!String.IsNullOrEmpty(passportNo))
            {
                q += "\r\n" + "\t" + " and " + EncryptDecryptUtil.getDecryptSQL("apnt.PASSPORT_NO") + " like :passportNo";
                model.QueryParameters.Add("HKID", "%" + passportNo + "%");
            }

            model.Query = q;
            

            return model.Export("ExportData");
        }

        public ServiceResult AddtoList(int candidateNumber, string regType, string UUID)
        {
            ServiceResult result = new ServiceResult();
            DisplayGrid grid = new DisplayGrid();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICANT_INFO app = db.C_COMP_APPLICANT_INFO.Where(o => o.CANDIDATE_NUMBER == candidateNumber)
                    .Include(o => o.C_S_SYSTEM_VALUE).Include(O=>O.C_COMP_APPLICATION).Include(o => o.C_APPLICANT).FirstOrDefault();

                if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA) || regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
                {
                    grid.Query = ""
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
                     + "\r\n\t" + "WHERE T0.UUID = :UUID                                                                       "
                     + "\r\n\t" + "AND T1.CANDIDATE_NUMBER = :candidateNumber                                                  ";
                }
                else if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_IP))
                {
                    grid.Query = ""
                    + "\r\n\t" + "SELECT T3.FILE_REFERENCE_NO                                                                "
                    + "\r\n\t" + ", T4.SURNAME || ' ' || T4.GIVEN_NAME_ON_ID AS FUL_NAME                                     "
                    + "\r\n\t" + ", T5.CODE , T1.*                                                                   "
                    + "\r\n\t" + ", case when T1.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end as IVType         "
                    + "\r\n\t" + "FROM C_INTERVIEW_SCHEDULE T0                                                               "
                    + "\r\n\t" + "INNER JOIN  C_INTERVIEW_CANDIDATES T1 ON T0.UUID = T1.INTERVIEW_SCHEDULE_ID                "
                    + "\r\n\t" + "LEFT JOIN C_IND_CERTIFICATE T2 ON T1.CANDIDATE_NUMBER = T2.CANDIDATE_NUMBER                "
                    + "\r\n\t" + "LEFT JOIN C_IND_APPLICATION T3 ON T2.MASTER_ID = T3.UUID                                   "
                    + "\r\n\t" + "LEFT JOIN C_APPLICANT T4 ON T4.UUID = T3.APPLICANT_ID                                      "
                    + "\r\n\t" + "LEFT JOIN C_S_CATEGORY_CODE T5 ON T5.UUID = T2.CATEGORY_ID                                 "
                    + "\r\n\t" + "WHERE T0.UUID = :UUID                                                                      "
                    + "\r\n\t" + "AND T1.CANDIDATE_NUMBER = :candidateNumber                                                 ";
                }


                // else 

                grid.QueryParameters.Add("UUID", UUID);
                grid.QueryParameters.Add("candidateNumber  ", candidateNumber);
                grid.Search();

                if (grid.Data.Count > 0)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE,
                        Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                    };
                }
                //else
                //{
                //    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                //}


                List<C_COMP_APPLICANT_INFO> draftList =
                     SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);

                int cnt = draftList.Where(o => o.CANDIDATE_NUMBER == candidateNumber).ToList().Count;
                if (cnt > 0)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE,
                        Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                    };
                }

                if (app == null)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE,
                        Message = new List<string>() { "No Candidate exists with the Candidate Number" }
                    };
                }

                draftList.Add(app);

                //try {
                //    if (draftList == null)
                //    {
                //        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                //    }
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}


                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };


            }
           // return new ServiceResult();
        }

        //new add !!!!
        public void SearchIR(InterviewResultSearchModel model, string type)
        {
            //model.Query = SearchCA_q;
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();

            if (type == "CGC" || type == "CMW")
            {
                model = registrationCommonService.SearchInterviewResultForCompany(model);
            }
            else
            {
                model = SearchInterviewResultForIndividual(model);
            }

        }

        public string ExportExcel(InterviewResultSearchModel model, string type)
        {
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            if (type == "CGC" || type == "CMW") // company
            {
                model = registrationCommonService.SearchInterviewResultForCompany(model);
            }
            else
            {
                model = SearchInterviewResultForIndividual(model);
            }

            // customized excel columns
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string> { ["columnName"] = "INTERVIEW_DATE_DISPLAY", ["displayName"] = "Interview Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "INTERVIEW_NUMBER", ["displayName"] = "Interview/Assessment No" })
                .Append(new Dictionary<string, string> { ["columnName"] = "INTERVIEW_TYPE", ["displayName"] = "Type" })
                .Append(new Dictionary<string, string> { ["columnName"] = "FULL_NAME", ["displayName"] = "Name" })
                .Append(new Dictionary<string, string> { ["columnName"] = "ROLE", ["displayName"] = "Role" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RESULT_DATE", ["displayName"] = "Result Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RESULT", ["displayName"] = "Result" })
                .Append(new Dictionary<string, string> { ["columnName"] = "IS_ABSENT", ["displayName"] = "Absent" }).ToArray();


            return model.Export("Interview Result");
        }

        public string ExportIR(InterviewResultSearchModel model, string type)
        {
            //model.Query = SearchCA_q;
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();

            if (type == "CGC" || type == "CMW")
            {
               return  registrationCommonService.ExpoortInterviewResultForCompany(model);
            }
            else
            {
               return  ExportInterviewResultForIndividual(model);
            }

        }
        public InterviewResultDisplayModel ViewCompIR(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = (from interviewCan in db.C_INTERVIEW_CANDIDATES
                             join svResult in db.C_S_SYSTEM_VALUE on interviewCan.RESULT_ID equals svResult.UUID into gy
                             from x in gy.DefaultIfEmpty()
                             join cCompAppInfo in db.C_COMP_APPLICANT_INFO on interviewCan.CANDIDATE_NUMBER equals cCompAppInfo.CANDIDATE_NUMBER into gy3
                             from x3 in gy3.DefaultIfEmpty()
                             //join compApp in db.C_COMP_APPLICATION on cCompAppInfo.MASTER_ID equals compApp.UUID
                             join svRole in db.C_S_SYSTEM_VALUE on x3.APPLICANT_ROLE_ID equals svRole.UUID into gy4
                             from x4 in gy4.DefaultIfEmpty()
                             join appli in db.C_APPLICANT on x3.APPLICANT_ID equals appli.UUID into gy1
                             from x1 in gy1.DefaultIfEmpty()
                             //join interviewSchedule in db.C_INTERVIEW_SCHEDULE on interviewCan.INTERVIEW_SCHEDULE_ID equals interviewSchedule.UUID
                             //join meeting in db.C_MEETING on interviewSchedule.MEETING_ID equals meeting.UUID
                            // join commGrp in db.C_COMMITTEE_GROUP on meeting.COMMITTEE_GROUP_ID equals commGrp.UUID
                             //join svType in db.C_S_SYSTEM_VALUE on meeting.COMMITTEE_TYPE_ID equals svType.UUID into gy2
                             //from x2 in gy2.DefaultIfEmpty()
                             where interviewCan.UUID == id
                             //select interviewCan).FirstOrDefault(); 
                             select new
                             {
                                 inte = interviewCan,
                                 InterviewType = interviewCan.INTERVIEW_TYPE,
                                 InterviewNumber = interviewCan.INTERVIEW_NUMBER,
                                 InterviewDate = interviewCan.START_DATE,
                                 //ApplicantName = x1.FULL_NAME_DISPLAY,
                                 appl =x1
                                 
                             }).FirstOrDefault();


                //var test = db.C_INTERVIEW_CANDIDATES.Where(x=>x.UUID==id)
                //           .Include(x=>x.C_S_SYSTEM_VALUE)
                //           .Include(x=>x.)

                return new InterviewResultDisplayModel()
                {
                    C_APPLICANT = query.appl,
                    C_INTERVIEW_CANDIDATES = query.inte

                };
               
            }

        }

        public InterviewResultDisplayModel ViewIndIR(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = (from indCan in db.C_INTERVIEW_CANDIDATES
                             join indCert in db.C_IND_CERTIFICATE on indCan.CANDIDATE_NUMBER equals indCert.CANDIDATE_NUMBER
                             join indApcn in db.C_IND_APPLICATION on indCert.MASTER_ID equals indApcn.UUID
                             join cApnt in db.C_APPLICANT on indApcn.APPLICANT_ID equals cApnt.UUID into gy
                             from x1 in gy.DefaultIfEmpty()
                             //join sCode in db.C_S_CATEGORY_CODE on indCert.CATEGORY_ID equals sCode.CODE 
                             //join sv in db.C_S_SYSTEM_VALUE on indCan.RESULT_ID equals sv.UUID
                             where indCan.UUID == id
                             //select interviewCan).FirstOrDefault(); 
                             select new
                             {
                                 inte = indCan,
                                 InterviewType = indCan.INTERVIEW_TYPE,
                                 InterviewNumber = indCan.INTERVIEW_NUMBER,
                                 InterviewDate = indCan.START_DATE,
                                 //ApplicantName = x1.FULL_NAME_DISPLAY,
                                 appl = x1

                             }).FirstOrDefault();


                //var test = db.C_INTERVIEW_CANDIDATES.Where(x=>x.UUID==id)
                //           .Include(x=>x.C_S_SYSTEM_VALUE)
                //           .Include(x=>x.)

                return new InterviewResultDisplayModel()
                {
                    C_APPLICANT = query.appl,
                    C_INTERVIEW_CANDIDATES = query.inte

                };

            }

        }

        public ServiceResult SaveComp(InterviewResultDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_INTERVIEW_CANDIDATES.Find(model.C_INTERVIEW_CANDIDATES.UUID);

                query.RESULT_ID = model.C_INTERVIEW_CANDIDATES.RESULT_ID;
                query.RESULT_DATE = model.C_INTERVIEW_CANDIDATES.RESULT_DATE;
                query.IS_ABSENT = model.C_INTERVIEW_CANDIDATES.IS_ABSENT;
                query.MW_APPLY = model.C_INTERVIEW_CANDIDATES.MW_APPLY;
                query.MW_APPROVED = model.C_INTERVIEW_CANDIDATES.MW_APPROVED;
                query.REMARKS = model.C_INTERVIEW_CANDIDATES.REMARKS;
               
                query.ASSESSMENT_RES_PATH = model.C_INTERVIEW_CANDIDATES.ASSESSMENT_RES_PATH; //upload --- not sure

                query.MODIFIED_DATE = System.DateTime.Now;
                query.MODIFIED_BY = SystemParameterConstant.UserName; //"Admin";

                db.SaveChanges();
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
            if (!string.IsNullOrWhiteSpace(model.Type))
            {
                whereQ += "\r\n\t" + "AND sv1.Code = :InterviewType";
                model.QueryParameters.Add("InterviewType", model.Type);
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

        public CompICModel Form(string id)
        {
            CompICModel model = new CompICModel();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                model.C_INTERVIEW_SCHEDULE = db.C_INTERVIEW_SCHEDULE.Where(o => o.UUID == id)
                    .Include(o => o.C_S_SYSTEM_VALUE).Include(o => o.C_S_ROOM)
                    .Include(o => o.C_MEETING)
                    .FirstOrDefault();
            }
            return model;
        }

        public DisplayGrid AjaxCandidates(string C_INTERVIEW_SCHEDULE_UUID, string regType)
        {
            DisplayGrid grid = new DisplayGrid();
            
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                //need to get regType
                
                //string regType = RegistrationConstant.REGISTRATION_TYPE_CGA; //CGC
                if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA)|| regType.Equals(RegistrationConstant.REGISTRATION_TYPE_MW))
                {
                    grid.Query = ""
                     + "\r\n\t" + "SELECT T3.FILE_REFERENCE_NO                                                                 "
                     + "\r\n\t" + ", T4.SURNAME || ' ' || T4.GIVEN_NAME_ON_ID AS FUL_NAME                                      "
                     + "\r\n\t" + ", T5.CODE, T1.*                                                                       "
                     + "\r\n\t" + ", case when T1.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end as IVType        "
                     + "\r\n\t" + "FROM C_INTERVIEW_SCHEDULE T0                                                                "
                     + "\r\n\t" + "INNER JOIN  C_INTERVIEW_CANDIDATES T1 ON T0.UUID = T1.INTERVIEW_SCHEDULE_ID                 "
                     + "\r\n\t" + "LEFT JOIN C_COMP_APPLICANT_INFO T2 ON T1.CANDIDATE_NUMBER = T2.CANDIDATE_NUMBER             "
                     + "\r\n\t" + "LEFT JOIN C_COMP_APPLICATION T3 ON T3.UUID = T2.MASTER_ID                                   "
                     + "\r\n\t" + "LEFT JOIN C_APPLICANT T4 ON T4.UUID = T2.APPLICANT_ID                                       "
                     + "\r\n\t" + "LEFT JOIN C_S_SYSTEM_VALUE T5 ON T5.UUID = T2.APPLICANT_ROLE_ID                             "
                     + "\r\n\t" + "WHERE T0.UUID = :C_INTERVIEW_SCHEDULE_UUID                                                  ";
                }
                else  if (regType.Equals(RegistrationConstant.REGISTRATION_TYPE_IP))
                {
                    grid.Query = ""
                    + "\r\n\t" + "SELECT T3.FILE_REFERENCE_NO                                                                "
                    + "\r\n\t" + ", T4.SURNAME || ' ' || T4.GIVEN_NAME_ON_ID AS FUL_NAME                                     "
                    + "\r\n\t" + ", T5.CODE , T1.*                                                                          "
                    + "\r\n\t" + ", case when T1.INTERVIEW_TYPE = 'I' then 'Interview' else 'Assessment' end as IVType         "
                    + "\r\n\t" + "FROM C_INTERVIEW_SCHEDULE T0                                                               "
                    + "\r\n\t" + "INNER JOIN  C_INTERVIEW_CANDIDATES T1 ON T0.UUID = T1.INTERVIEW_SCHEDULE_ID                "
                    + "\r\n\t" + "LEFT JOIN C_IND_CERTIFICATE T2 ON T1.CANDIDATE_NUMBER = T2.CANDIDATE_NUMBER                "
                    + "\r\n\t" + "LEFT JOIN C_IND_APPLICATION T3 ON T2.MASTER_ID = T3.UUID                                   "
                    + "\r\n\t" + "LEFT JOIN C_APPLICANT T4 ON T4.UUID = T3.APPLICANT_ID                                      "
                    + "\r\n\t" + "LEFT JOIN C_S_CATEGORY_CODE T5 ON T5.UUID = T2.CATEGORY_ID                                 "
                    + "\r\n\t" + "WHERE T0.UUID = :C_INTERVIEW_SCHEDULE_UUID                                                 ";
                }
               // else 

                grid.QueryParameters.Add("C_INTERVIEW_SCHEDULE_UUID", C_INTERVIEW_SCHEDULE_UUID);
                grid.Search();



                List<C_COMP_APPLICANT_INFO> draftList =
                     SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);

                int listCount = draftList.Count -1 ;
                if (draftList != null  ) for (int i = 0; i < draftList.Count; i++)
                {
                        grid.Data.Add(new Dictionary<string, object>() {
                            
                             {"FILE_REFERENCE_NO",draftList[i].C_COMP_APPLICATION?.FILE_REFERENCE_NO   }
                           , {"CANDIDATE_NUMBER",draftList[i]?.CANDIDATE_NUMBER  }
                         //, {"INTERVIEW_TYPE",draftList[i]. }
                         //, {"ALIAS_NUMBER",draftList[i].   }
                           , {"FUL_NAME",draftList[i].C_APPLICANT?.FULL_NAME_DISPLAY   }
                           , {"CODE",draftList[i].C_S_SYSTEM_VALUE?.CODE   }
                           
                        });
                }
               
                return grid;
            }
        }//super useful

        

    }
}