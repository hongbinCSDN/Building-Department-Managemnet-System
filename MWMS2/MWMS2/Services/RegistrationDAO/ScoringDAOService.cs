using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services
{
    public class ScoringDAOService : BaseDAOService
    {

        public Fn09SC_SCModel SearchWarningLetterRecord(Fn09SC_SCModel model)
        {
            //StringBuilder SearchQueryString = new StringBuilder();
            //            SearchQueryString.Append(@" select  distinct  appl.UUID,case when appl.HKID is null then " + EncryptDecryptUtil.getDecryptSQL("appl.PASSPORT_NO"));
            //            SearchQueryString.Append(@" when appl.PASSPORT_NO is null then " + EncryptDecryptUtil.getDecryptSQL("appl.HKID") +  " end as ID_NO,");
            //            SearchQueryString.Append(@" Appl.SURNAME ||' '||Appl.GIVEN_NAME_ON_ID AS ENG_FULL_NAME,appl.CHINESE_NAME as CHI_NAME from 

            //(select c_applicant.UUID,c_applicant.HKID,c_applicant.PASSPORT_NO,c_applicant.SURNAME,
            // c_applicant.GIVEN_NAME_ON_ID,c_applicant.CHINESE_NAME
            // from C_ind_application
            // inner join c_applicant on C_ind_application.applicant_id =c_applicant.uuid
            // UNION
            //  select c_applicant.UUID,c_applicant.HKID,c_applicant.PASSPORT_NO,c_applicant.SURNAME,
            // c_applicant.GIVEN_NAME_ON_ID,c_applicant.CHINESE_NAME
            // from c_comp_applicant_info
            // inner join c_applicant on c_comp_applicant_info.applicant_id =c_applicant.uuid
            // ) 

            //appl");
            string queryStr = " select distinct list.FILE_REF, list.ENG_NAME, list.CHIN_NAME, list.UUID, list.TYPE "
                + " \r\n\t , case when wl.UUID is not null then wl.SUBJECT else '-' end as TITLE "
                + " \r\n\t , case when wl.UUID is not null then TO_CHAR(wl.LETTER_ISSUE_DATE, 'DD-MM-YYYY') else '-' end as ISSUE_DATE "
                + " \r\n\t , case when wl.UUID is not null then '60' else '0' end as SCORE "
                + " \r\n\t , case when wl.UUID is not null and wl.AS_UUID is not null then app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID else '-' end as AS_ENG_NAME "
                + " \r\n\t , case when wl.UUID is not null and wl.AS_UUID is not null then TO_CHAR(app.CHINESE_NAME) else '-' end as AS_CHIN_NAME "
                + " \r\n\t , '15' as LETTER_SCORE "
                + " \r\n\t from "
                + " \r\n\t (select distinct ind_app.FILE_REFERENCE_NO as FILE_REF, app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as ENG_NAME "
                + " \r\n\t , app.CHINESE_NAME as CHIN_NAME "
                + " \r\n\t , ind_app.UUID as UUID, 'Individual' as TYPE "
                + " \r\n\t from c_ind_application ind_app "
                + " \r\n\t left "
                + " \r\n\t join c_applicant app on ind_app.applicant_id = app.UUID "
                + " \r\n\t union "
                + " \r\n\t select distinct comp_app.FILE_REFERENCE_NO as FILE_REF, comp_app.ENGLISH_COMPANY_NAME as ENG_NAME "
                + " \r\n\t , CHINESE_COMPANY_NAME as CHIN_NAME "
                + " \r\n\t , comp_app.UUID as UUID, 'Company' as TYPE "
                + " \r\n\t from c_comp_application comp_app "
                + " \r\n\t ) list "
                + " \r\n\t left join W_WL wl on list.FILE_REF = wl.REGISTRATION_NO "
                + " \r\n\t left join c_applicant app on wl.AS_UUID = app.UUID "
                + " \r\n\t where 1 = 1 ";

            //model.Query = SearchQueryString.ToString();
            model.Query = queryStr;
            model.Search();
            return model;
        }

        public Fn09SC_SCModel getScoringDetail(Fn09SC_SCModel model)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                using(EntitiesWarningLetter wl = new EntitiesWarningLetter())
                {
                    if("Company".Equals(model.FormType))
                    {
                        model.C_COMP_APPLICATION = db.C_COMP_APPLICATION.Find(model.FormUuid);

                        model.WL_List = wl.W_WL.Where(x => x.REGISTRATION_NO == model.FormFileRef).OrderBy(x => x.LETTER_ISSUE_DATE).ToList();

                        List<string> AS_List = model.WL_List.Select(x => x.AS_UUID).Distinct().ToList(); // List of all AS's id
                        model.AS_List = new List<C_APPLICANT>();
                        foreach(var AS in AS_List)
                        {
                            var query = db.C_APPLICANT.Find(AS);
                            model.AS_List.Add(query);
                        }

                        model.WL_List_by_AL = new List<List<W_WL>>();
                        for (int i = 0; i < model.AS_List.Count(); i++)
                        {
                            List<W_WL> letterList =  model.WL_List.Where(x => x.AS_UUID == AS_List[i]).ToList();
                            model.WL_List_by_AL.Add(letterList);
                        }

                        model.NumOfPeriod = 3;
                    }
                    else if("Individual".Equals(model.FormType))
                    {
                        model.C_IND_APPLICATION = db.C_IND_APPLICATION.Find(model.FormUuid);
                        model.C_APPLICANT = db.C_APPLICANT.Find(model.C_IND_APPLICATION.APPLICANT_ID);

                        model.WL_List = wl.W_WL.Where(x => x.REGISTRATION_NO == model.FormFileRef).OrderBy(x => x.LETTER_ISSUE_DATE).ToList();

                    }

                    return model;
                }
            }
        }

        //public Fn09SC_SCModel GetAppCompInfo(Fn09SC_SCModel model)
        //{
        //    using(EntitiesRegistration db = new EntitiesRegistration())
        //    {
        //        var result = (from ca in db.C_COMP_APPLICATION
        //               join cai in db.C_COMP_APPLICANT_INFO on ca.UUID equals cai.MASTER_ID
        //               join appl in db.C_APPLICANT on cai.APPLICANT_ID equals appl.UUID
        //               where appl.UUID == model.UUID
        //               select new Fn09SC_SCModel
        //               {
        //                   UUID = model.UUID
        //                   ,
        //                   File_Reference_No = ca.FILE_REFERENCE_NO
        //                   ,
        //                   Eng_Comp_Name = ca.ENGLISH_COMPANY_NAME
        //                   ,
        //                   Chi_Comp_Name = ca.CHINESE_COMPANY_NAME
        //                   ,
        //                   Expiry_Date = ca.EXPIRY_DATE
        //                   ,
        //                   Removal_Date = ca.REMOVAL_DATE
        //                   ,Chinese_Name = model.Chinese_Name
        //                   ,Eng_Full_Name = model.Eng_Full_Name
        //               }).FirstOrDefault();
        //        return result != null ? result : new Fn09SC_SCModel(); 
        //    }
        //}


        public Fn09SC_SCModel SearchCompanyInfo(Fn09SC_SCModel model)
        {
            model.Query = @"select 
                            ca.FILE_REFERENCE_NO,ca.CHINESE_COMPANY_NAME,
                            ca.ENGLISH_COMPANY_NAME,ca.EXPIRY_DATE,ca.REMOVAL_DATE 
                            from c_comp_application ca
                            inner join c_comp_applicant_info cai on ca.uuid = cai.master_id
                            inner join C_applicant Appl on Appl.UUID = cai.APPLICANT_ID ";
            model.QueryWhere = @"Where 1=1 And Appl.UUID= :UUID";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Search();
            return model;
        }


        public Fn09SC_SCModel SearchIndInfo(Fn09SC_SCModel model)
        {
            model.Query = @" select iapp.file_reference_no , sv.english_description
                                       ,ic.EXPIRY_DATE         from C_ind_application iapp, C_applicant app, C_ind_certificate ic, C_s_system_value sv
                                       where iapp.APPLICANT_ID = app.UUID
                                       AND ic.master_id = iapp.uuid
                                       AND sv.uuid = ic.application_status_id ";
            model.QueryWhere = @" And app.UUID= :UUID";
            model.QueryParameters.Add("UUID", model.UUID);
            model.Search();
            return model;
        }


        public Fn09SC_SCModel SearchOffenceList(Fn09SC_SCModel model)
        {
           
            if (CheckUUIDIsCompOrInd(model.UUID) == 1)
            {
                model.Query = @"select wof.WL_TYPE_OF_OFFENSE_ENG ,wof.score from w_wl wl
                                inner join W_WL_TYPE_OF_OFFENSE wof on wl.uuid = wof.WL_UUID";
                model.QueryWhere = @" Where AS_UUID = :UUID";
                model.QueryParameters.Add("UUID", model.UUID);
                model.Search();
            }
            else if (CheckUUIDIsCompOrInd(model.UUID) == 2) 
            {
                model.Query = @"select wof.WL_TYPE_OF_OFFENSE_ENG ,wof.score
                                from w_wl wl
                                inner join W_WL_TYPE_OF_OFFENSE wof on wl.uuid = wof.WL_UUID
                                ";
                model.QueryWhere = @" where 1=1
                                and wl.REGISTRATION_NO= (select indApp.FILE_REFERENCE_NO 
                                FROM C_APPLICANT Appl
                                inner join C_ind_application indApp on indApp.APPLICANT_ID = Appl.UUID                                                       
                                where 1=1 
                                and Appl.uuid= :UUID
                                )";
                model.QueryParameters.Add("UUID", model.UUID);
                model.Search();
            }
            
            return model;
            
        }

        public Fn09SC_SCModel SearchCourseList(Fn09SC_SCModel model)
        {
           
            if (CheckUUIDIsCompOrInd(model.UUID) == 1)
            {
                model.Query = @"select casc.UUID,casc.COURSE_NAME,casc.COURSE_ISSUE_DT,casc.COURSE_SCORE
                                from C_APPLICANT_SCORING cas 
                                inner join C_APPLICANT_SCORING_COURSE casc on cas.UUID = casc.C_APPLICANT_SCORING_ID
                                ";
                model.QueryWhere = @" where 1=1 and cas.AS_UUID=:UUID";
                model.QueryParameters.Add("UUID", model.UUID);
                model.Search();
            }
            else if (CheckUUIDIsCompOrInd(model.UUID) == 2)
            {
                model.Query = @"select casc.UUID,casc.COURSE_NAME,casc.COURSE_ISSUE_DT,casc.COURSE_SCORE 
                                from C_APPLICANT_SCORING cas 
                                inner join C_APPLICANT_SCORING_COURSE casc on cas.UUID = casc.C_APPLICANT_SCORING_ID ";
                model.QueryWhere = @" Where 1=1 And cas.REGISTRATION_NO = (select indApp.FILE_REFERENCE_NO 
                                    FROM C_APPLICANT Appl
                                    inner join C_ind_application indApp on indApp.APPLICANT_ID = Appl.UUID                                                    
                                    where 1=1 
                                    and Appl.uuid= :UUID
                                    )";
                model.QueryParameters.Add("UUID", model.UUID);
                model.Search();
            }
            
                return model;
           
       }

        public ServiceResult UpdateCourseSource(UpdateScoreModel model)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    C_APPLICANT_SCORING_COURSE course = db.C_APPLICANT_SCORING_COURSE.Where(m => m.UUID == model.UUID).FirstOrDefault();
                    if(course != null)
                    {
                        course.COURSE_NAME = model.CourseName;
                        course.COURSE_ISSUE_DT = model.CourseDate;
                        course.COURSE_SCORE = model.CourseScore;
                    }
                    else
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE };
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
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

                }
            }
        }

        public ServiceResult DeleteCourse(string UUID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    C_APPLICANT_SCORING_COURSE course = db.C_APPLICANT_SCORING_COURSE.Where(m => m.UUID == UUID).FirstOrDefault();
                    if(course != null)
                    {
                        db.C_APPLICANT_SCORING_COURSE.Remove(course);
                    }
                    else
                    {
                        return new ServiceResult { Result = ServiceResult.RESULT_FAILURE };
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
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }

        public int CheckUUIDIsCompOrInd(string UUID)
        {
            using(EntitiesRegistration db =new EntitiesRegistration())
            {
                if ((from cai in db.C_COMP_APPLICANT_INFO where cai.APPLICANT_ID == UUID select cai.UUID).ToList().Count > 0)
                    return 1;
                else if ((from cia in db.C_IND_APPLICATION where cia.APPLICANT_ID == UUID select cia.UUID).ToList().Count > 0)
                    return 2;
                else return 0;
            }
        }


        public ServiceResult CalculateTotalScore(string uuid)
        {
            string queryString = @"Select C_APPLICANT_OFFENSE_COURSE_TOTALSCORE(:uuid) from dual";
            OracleParameter[] oracleParameter = new OracleParameter[]
            {
                new OracleParameter(":uuid",uuid)
            };
            return new ServiceResult { Result = ServiceResult.RESULT_SUCCESS, Data = GetObjectData<float>(queryString, oracleParameter).FirstOrDefault().ToString() };

        }

        public ServiceResult AddNewCourse(Fn09SC_SCModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (CheckUUIDIsCompOrInd(model.UUID) == 1)
                {
                    using(DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        string scoringUUID = (from cas in db.C_APPLICANT_SCORING where cas.AS_UUID == model.UUID select cas.UUID).FirstOrDefault();
                        if (scoringUUID == null)
                        {
                            C_APPLICANT_SCORING cas = new C_APPLICANT_SCORING()
                            {
                                UUID = Guid.NewGuid().ToString("N")
                                ,
                                CATEGORY = "Company"
                                ,
                                AS_UUID = model.UUID
                            };
                            db.C_APPLICANT_SCORING.Add(cas);
                        }
                        try
                        {
                            db.SaveChanges();

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

                        for (int i = 0; i < model.CourseNameList.Count; i++)
                        {

                            C_APPLICANT_SCORING_COURSE apsc = new C_APPLICANT_SCORING_COURSE
                            {
                                UUID = Guid.NewGuid().ToString("N")
                                ,
                                C_APPLICANT_SCORING_ID = (from cas in db.C_APPLICANT_SCORING
                                                          where cas.AS_UUID == model.UUID
                                                          select cas.UUID
                                                            ).FirstOrDefault()
                                ,
                                COURSE_ISSUE_DT = Convert.ToDateTime(model.CourseIssueDateList[i])
                                ,
                                COURSE_NAME = model.CourseNameList[i]
                                ,
                                COURSE_SCORE = Convert.ToDecimal(model.CourseScoreList[i])
                            };
                            db.C_APPLICANT_SCORING_COURSE.Add(apsc);
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Added Successfully." } };

                    }
                }
                else if(CheckUUIDIsCompOrInd(model.UUID) == 2)
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        string indAppRegNo = (from appl in db.C_APPLICANT
                                              join indApp in db.C_IND_APPLICATION on appl.UUID equals indApp.APPLICANT_ID
                                              where appl.UUID == model.UUID
                                              select indApp.FILE_REFERENCE_NO).FirstOrDefault();
                        string scoringRegNo = (from cas in db.C_APPLICANT_SCORING where cas.REGISTRATION_NO == indAppRegNo
                                               select cas.REGISTRATION_NO).FirstOrDefault();
                        if (scoringRegNo == null)
                        {
                            C_APPLICANT_SCORING cas = new C_APPLICANT_SCORING()
                            {
                                UUID = Guid.NewGuid().ToString("N")
                               ,
                                CATEGORY = "Individual"
                               ,
                                REGISTRATION_NO = indAppRegNo
                            };
                            db.C_APPLICANT_SCORING.Add(cas);
                        }
                        try
                        {
                            db.SaveChanges();

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

                        for (int i = 0; i < model.CourseNameList.Count; i++)
                        {

                            C_APPLICANT_SCORING_COURSE apsc = new C_APPLICANT_SCORING_COURSE
                            {
                                UUID = Guid.NewGuid().ToString("N")
                                ,
                                C_APPLICANT_SCORING_ID = (from cas in db.C_APPLICANT_SCORING
                                                          where cas.REGISTRATION_NO == indAppRegNo
                                                          select cas.UUID
                                                            ).FirstOrDefault()
                                ,
                                COURSE_ISSUE_DT = Convert.ToDateTime(model.CourseIssueDateList[i])
                                ,
                                COURSE_NAME = model.CourseNameList[i]
                                ,
                                COURSE_SCORE = Convert.ToDecimal(model.CourseScoreList[i])
                            };
                            db.C_APPLICANT_SCORING_COURSE.Add(apsc);
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
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Added Successfully." } };
                    }
                }
                else
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "Added Failed." } };
            }
           
        }

        public string Excel(Fn09SC_SCModel model)
        {
            StringBuilder SearchQueryString = new StringBuilder();
            SearchQueryString.Append(@" select  distinct  appl.UUID,case when appl.HKID is null then " + EncryptDecryptUtil.getDecryptSQL("appl.PASSPORT_NO"));
            SearchQueryString.Append(@" when appl.PASSPORT_NO is null then " + EncryptDecryptUtil.getDecryptSQL("appl.HKID") + " end as ID_NO,");
            SearchQueryString.Append(@" Appl.SURNAME ||' '||Appl.GIVEN_NAME_ON_ID AS ENG_FULL_NAME,appl.CHINESE_NAME as CHI_NAME from C_APPLICANT appl");
            model.Query = SearchQueryString.ToString();
            return model.Export("Scoring");
        }
    }
}