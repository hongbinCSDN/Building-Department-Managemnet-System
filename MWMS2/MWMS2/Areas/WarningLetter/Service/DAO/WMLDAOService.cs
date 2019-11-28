using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.WarningLetter.Service.DAO
{
    public class WMLDAOService
    {
        public WLM_OffenceModel SearchAllOffence(WLM_OffenceModel model)
        {
            model.Query = @"Select sv.uuid,sv.TYPE,'Offence :' Name,sv.DESCRIPTION_ENG from W_S_SYSTEM_VALUE sv
                        inner join W_S_SYSTEM_TYPE st on st.UUID=sv.S_SYSTEM_TYPE_ID";

            model.QueryWhere = @"where st.TYPE='Type_Of_Offense'";
            model.Search();
            return model;
        }

        public W_S_SYSTEM_VALUE CheckExistOffense(WLM_OffenceModel model)
        {
            using(EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                W_S_SYSTEM_VALUE OffenseType = new W_S_SYSTEM_VALUE();
                for (int i = 0; i < model.DESCRIPTION_ENG.Count; i++)
                {
                    string description_eng = model.DESCRIPTION_ENG[i];
                     OffenseType = (from st in db.W_S_SYSTEM_TYPE
                                       join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                       where st.TYPE == "Type_Of_Offense" && sv.DESCRIPTION_ENG == description_eng
                                    select sv).FirstOrDefault();
                    if (OffenseType != null)
                        break;
                }

                return OffenseType;
            }
          
        }

        public ServiceResult UpdateOffenseName(ScoreListModel model)
        {
            using (EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var sv = db.W_S_SYSTEM_VALUE.Where(x => x.UUID == model.Offense_Id).FirstOrDefault();
                    sv.DESCRIPTION_ENG = model.Offense_Name;
                    sv.TYPE = model.Type;
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
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Edit Successfully." },Data = model.Offense_Name };

                }
            }
        }

        public ServiceResult DeleteOffenseName(string id)
        {
            using (EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var toBeDel = db.W_S_SYSTEM_VALUE.Where(x => x.UUID == id).FirstOrDefault();

                    db.W_S_OFFENSE_SCORE.RemoveRange(
                           db.W_S_OFFENSE_SCORE.Where(x => x.OFFENSE_ID == id)
                        );
                 

                    var sv = db.W_S_SYSTEM_VALUE.Remove(toBeDel);
                   
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
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Delete Successfully." } };

                }
            }
        }
        public ServiceResult AddNewOffense(WLM_OffenceModel model)
        {
            using(EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    for(int i=0;i < model.DESCRIPTION_ENG.Count; i++)
                    {
                        W_S_SYSTEM_VALUE sv = new W_S_SYSTEM_VALUE
                        {
                            UUID = Guid.NewGuid().ToString("N")
                       ,
                            DESCRIPTION_ENG = model.DESCRIPTION_ENG[i]
                       ,
                            S_SYSTEM_TYPE_ID = (from st in db.W_S_SYSTEM_TYPE where st.TYPE == "Type_Of_Offense" select st.UUID).FirstOrDefault()
                       ,
                            CREATED_DATE = Convert.ToDateTime(DateTime.Now.ToString("d"))

                       ,   TYPE = model.TYPE[i]
                       ,
                            CREATED_BY = "admin"
                       ,
                            MODIFIED_DATE = Convert.ToDateTime(DateTime.Now.ToString("d"))
                       ,
                            MODIFIED_BY = "admin"
                        };
                        db.W_S_SYSTEM_VALUE.Add(sv);
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
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS,Message = new List<string>() { "Added Successfully." } };
                }
            }
        } 


        public ScoreListModel SearchDetailScoreList(ScoreListModel model)
        {
            model.Query = @"select sv.DESCRIPTION_ENG,wsos.effective_dt,wsos.SCORE from W_S_OFFENSE_SCORE wsos
                            inner join W_S_SYSTEM_VALUE sv on sv.UUID = wsos.OFFENSE_ID
                            inner join W_S_SYSTEM_TYPE st on st.UUID=sv.S_SYSTEM_TYPE_ID
                           
                            ";
            model.QueryWhere = @" where 1=1 and st.TYPE='Type_Of_Offense' and offense_id = :Offense_Id";
            model.QueryParameters.Add("Offense_Id", model.Offense_Id);
            model.Search();
            return model;
        }

        public W_S_OFFENSE_SCORE CheckIsExistEffectDate(ScoreListModel model)
        {
            using (EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                W_S_OFFENSE_SCORE offenseScore = new W_S_OFFENSE_SCORE();
                for (int i = 0; i < model.Effect_Date.Count; i++)
                {
                    DateTime effect_date = Convert.ToDateTime(model.Effect_Date[i]);
                    offenseScore = (from wsos in db.W_S_OFFENSE_SCORE
                                        where wsos.OFFENSE_ID == model.Offense_Id
                                        && wsos.EFFECTIVE_DT == effect_date
                                    select wsos).FirstOrDefault();
                    if (offenseScore != null)
                        break;
                }
              
                return offenseScore;
            }
        }

     

        public ServiceResult AddNewScore(ScoreListModel model)
        {
            using(EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    for(int i=0;i < model.Effect_Date.Count; i++)
                    {

                        W_S_OFFENSE_SCORE wsos = new W_S_OFFENSE_SCORE
                        {
                            UUID = Guid.NewGuid().ToString("N")
                            ,
                            OFFENSE_ID = model.Offense_Id
                            ,
                            SCORE = Convert.ToDecimal(model.Score[i])
                            ,
                            EFFECTIVE_DT = Convert.ToDateTime(model.Effect_Date[i])
                            ,
                            CREATED_BY = "admin"
                            ,
                            CREATED_DATE = Convert.ToDateTime(DateTime.Now.ToString("d"))
                            ,
                            MODIFIED_BY = "admin"
                            ,
                            MODIFIED_DATE = Convert.ToDateTime(DateTime.Now.ToString("d"))
                        };
                        db.W_S_OFFENSE_SCORE.Add(wsos);
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
        }

    }
}