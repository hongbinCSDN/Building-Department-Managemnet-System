using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;

namespace MWMS2.Services.AdminService.DAO
{
    public class Sys_UMDAOService
    {
        private const string SearchUsers_q = @"SELECT  DISTINCT T1.UUID,
                                                       T1.BD_PORTAL_LOGIN,
                                                       T1.CODE          CODE,
                                                       T1.SUPERVISOR_ID SUPERVISOR_ID,
                                                       T1.EMAIL         EMAIL,
                                                       T1.FAX_NO        FAX_NO,
                                                       T1.PHONE         PHONE,
                                                       T1.DSMS_USERNAME DSMS_USERNAME,
                                                       T1.RECEIVE_CASE  RECEIVE_CASE,
                                                       T1.IS_ACTIVE     IS_ACTIVE,
                                                       T1.SYS_UNIT_ID   SYS_UNIT_ID,
                                                       T2.CODE          SYS_RANK_ID,
                                                       T4.CODE          SYS_ROLE,
                                                       T5.CODE          UNITCODE
                                                FROM   SYS_POST T1
                                                       LEFT JOIN SYS_RANK T2
                                                         ON T1.SYS_RANK_ID = T2.UUID
                                                       LEFT JOIN SYS_POST_ROLE T3
                                                         ON T1.UUID = T3.SYS_POST_ID
                                                       LEFT JOIN SYS_ROLE T4
                                                         ON T4.UUID=t3.sys_role_id
                                                       LEFT JOIN SYS_UNIT T5
                                                         ON T1.SYS_UNIT_ID = T5.UUID
                                                WHERE  1 = 1 ";


        #region Search
        public Sys_UMModel SearchUsers(Sys_UMModel model)
        {
            model.Query = SearchUsers_q;
            model.Search();
            return model;
        }
        public void AjaxPostRole(Sys_UMModel model)
        {
            model.Query = ""
             + "\r\n\t" + " SELECT DISTINCT                                                             "
             + "\r\n\t" + " T1.UUID                                                                     "
             + "\r\n\t" + " , T1.CODE, T1.DESCRIPTION                                                   "
             + "\r\n\t" + " , CASE WHEN T2.SYS_POST_ID IS NULL THEN 'N' ELSE 'Y' END AS CHECKED         "
             + "\r\n\t" + " FROM SYS_ROLE T1                                                            "
             + "\r\n\t" + " LEFT JOIN SYS_POST_ROLE T2 ON T2.SYS_ROLE_ID = T1.UUID                      "
             + "\r\n\t" + " AND T2.SYS_POST_ID = :sysPost                                               ";
            model.QueryParameters.Add("sysPost", model.SYS_POST.UUID);
            model.Rpp = -1;
            model.Sort = "CODE";
            model.Search();
        }
        public void AjaxScuSubordinateMember(Sys_UMModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List<B_S_SCU_TEAM> l = db.B_S_SCU_TEAM.Where(o => o.SYS_POST_ID == model.SYS_POST.UUID).ToList();
                model.Data = l.Select(o => new Dictionary<string, object>() { ["UUID"] = o.CHILD_SYS_POST_ID }).ToList();
            }
        }
        public void AjaxPemSubordinateMember(Sys_UMModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_S_SCU_TEAM> l = db.P_S_SCU_TEAM.Where(o => o.SYS_POST_ID == model.SYS_POST.UUID).ToList();
                model.Data = l.Select(o => new Dictionary<string, object>() { ["UUID"] = o.CHILD_SYS_POST_ID }).ToList();
            }
        }
        
        public void AjaxScuResponsibleArea(Sys_UMModel model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                List<SYS_POST_AREA> l = db.SYS_POST_AREA.Where(o => o.SYS_POST_ID == model.SYS_POST.UUID).OrderBy(o => o.AREA_CODE).ToList();
                model.Data = l.Select(o => new Dictionary<string, object>() { ["AREA_CODE"] = o.AREA_CODE }).ToList();
            }
        }

        #endregion

        #region Add
        public ServiceResult SaveUser(Sys_UMModel model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (EntitiesAuth db = new EntitiesAuth())
                {
                    if (!string.IsNullOrWhiteSpace(model.SYS_POST.UUID))
                    {
                        bool existPortal = db.SYS_POST.Where(o => o.BD_PORTAL_LOGIN == model.SYS_POST.BD_PORTAL_LOGIN)
                              .Where(o => o.UUID != model.SYS_POST.UUID).Count() > 0;
                        bool existPostCode = db.SYS_POST.Where(o => o.CODE == model.SYS_POST.CODE)
                              .Where(o => o.UUID != model.SYS_POST.UUID).Count() > 0;
                        if (existPortal || existPostCode)
                        {
                            Dictionary<string, List<string>> v = new Dictionary<string, List<string>>();
                            if(existPortal) v["SYS_POST.BD_PORTAL_LOGIN"] = new List<string>() { "Portal ID already exist." };
                            if(existPostCode) v["SYS_POST.CODE"] = new List<string>() { "Post Code already exist." };
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = v };
                        }
                    }
                    else
                    {
                        bool existPortal = db.SYS_POST.Where(o => o.BD_PORTAL_LOGIN == model.SYS_POST.BD_PORTAL_LOGIN).Count() > 0;
                        bool existPostCode = db.SYS_POST.Where(o => o.CODE == model.SYS_POST.CODE).Count() > 0;
                        if (existPortal || existPostCode)
                        {
                            Dictionary<string, List<string>> v = new Dictionary<string, List<string>>();
                            if (existPortal) v["SYS_POST.BD_PORTAL_LOGIN"] = new List<string>() { "Portal ID already exist." };
                            if (existPostCode) v["SYS_POST.CODE"] = new List<string>() { "Post Code already exist." };
                            return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = v };
                        }
                    }
                }

                    Dictionary<string, SYS_POST> smmSYS_POSTs = new Dictionary<string, SYS_POST>();
                Dictionary<string, SYS_POST> pemSYS_POSTs = new Dictionary<string, SYS_POST>();
                Dictionary<string, MWMS_ADDRESS_META_DATA> MWMS_ADDRESS_META_DATAs = new Dictionary<string, MWMS_ADDRESS_META_DATA>();

                using (EntitiesAddress db = new EntitiesAddress())
                {
                    for (int i = 0; i < model.ScuResponsibleAreas.Count; i++)
                    {
                        if (MWMS_ADDRESS_META_DATAs.ContainsKey(model.ScuResponsibleAreas[i])) continue;
                        string searchArea = model.ScuResponsibleAreas[i];
                        if (string.IsNullOrWhiteSpace(searchArea)) continue;
                        MWMS_ADDRESS_META_DATAs[model.ScuResponsibleAreas[i]] = db.MWMS_ADDRESS_META_DATA.Where(o => o.CODE == searchArea).FirstOrDefault();
                    }
                }

                using (EntitiesAuth db = new EntitiesAuth())
                {
                    try
                    {
                        for (int i = 0; i < model.PemSubordinateMembers.Count; i++)
                        {
                            if (pemSYS_POSTs.ContainsKey(model.PemSubordinateMembers[i])) continue;
                            string searchPost = model.PemSubordinateMembers[i];
                            if (string.IsNullOrWhiteSpace(searchPost)) continue;
                            pemSYS_POSTs[model.PemSubordinateMembers[i]] = db.SYS_POST.Where(o => o.CODE == searchPost).FirstOrDefault();
                        }

                        for (int i = 0; i < model.ScuSubordinateMembers.Count; i++)
                        {
                            if (smmSYS_POSTs.ContainsKey(model.ScuSubordinateMembers[i])) continue;
                            string searchPost = model.ScuSubordinateMembers[i];
                            if (string.IsNullOrWhiteSpace(searchPost)) continue;
                            smmSYS_POSTs[model.ScuSubordinateMembers[i]] = db.SYS_POST.Where(o => o.CODE == searchPost).FirstOrDefault();
                        }
                        
                        if (model.SYS_POST.PW != null) model.SYS_POST.PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.PW);
                        //else model.SYS_POST.PW = dbPOST.PW;
                        if (model.SYS_POST.DSMS_PW != null) model.SYS_POST.DSMS_PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.DSMS_PW);
                        //else model.SYS_POST.DSMS_PW = dbPOST.DSMS_PW;

                        SYS_POST dbPOST = null;

                        if (string.IsNullOrWhiteSpace(model.SYS_POST.UUID))
                        {
                            dbPOST = model.SYS_POST;
                            db.SYS_POST.Add(dbPOST);
                        }
                        else
                        {
                            dbPOST = db.SYS_POST.Find(model.SYS_POST.UUID);
                            foreach (PropertyInfo propertyInfo in typeof(SYS_POST).GetProperties())
                            {
                                if (propertyInfo.Name == "UUID") continue;
                                var toValue = propertyInfo.GetValue(model.SYS_POST, null);
                                if (toValue != null)
                                {
                                    propertyInfo.SetValue(dbPOST, toValue);
                                }
                            }
                            db.SYS_POST_ROLE.RemoveRange(db.SYS_POST_ROLE.Where(o => o.SYS_POST.UUID == model.SYS_POST.UUID));
                        }
                        if (model.SelectedRoles != null)
                            for (int i = 0; i < model.SelectedRoles.Length; i++)
                            {
                                db.SYS_POST_ROLE.Add(new SYS_POST_ROLE()
                                {
                                    SYS_POST = dbPOST,
                                    SYS_ROLE_ID = model.SelectedRoles[i]
                                });
                            }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error:" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                }
                //SMM child post
                using (EntitiesSignboard db = new EntitiesSignboard())
                {
                    string postid = model.SYS_POST.UUID;

                    db.B_S_SCU_TEAM.RemoveRange(db.B_S_SCU_TEAM.Where(o => o.SYS_POST_ID == postid));
                    foreach (string o in smmSYS_POSTs.Keys)
                    {
                        db.B_S_SCU_TEAM.Add(new B_S_SCU_TEAM() { SYS_POST_ID = postid, CHILD_SYS_POST_ID = o });
                    }

                    db.SaveChanges();

                }
                //SMM child post
                using (EntitiesMWProcessing db = new EntitiesMWProcessing())
                {
                    string postid = model.SYS_POST.UUID;

                    db.P_S_SCU_TEAM.RemoveRange(db.P_S_SCU_TEAM.Where(o => o.SYS_POST_ID == postid));
                    foreach (string o in pemSYS_POSTs.Keys)
                    {
                        db.P_S_SCU_TEAM.Add(new P_S_SCU_TEAM() { SYS_POST_ID = postid, CHILD_SYS_POST_ID = o });
                    }
                    db.SaveChanges();
                }

                //SMM  area
                using (EntitiesAuth db = new EntitiesAuth())
                {
                    string postid = model.SYS_POST.UUID;

                    db.SYS_POST_AREA.RemoveRange(db.SYS_POST_AREA.Where(o => o.SYS_POST_ID == postid));
                    foreach (string o in MWMS_ADDRESS_META_DATAs.Keys)
                    {
                        db.SYS_POST_AREA.Add(new SYS_POST_AREA() { SYS_POST_ID = postid, AREA_CODE = o });
                    }
                    db.SaveChanges();
                }


                scope.Complete();
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            
        }
        #endregion
       

        #region Edit
        public Sys_UMModel GetPostByUUID(string id)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                Sys_UMModel model = new Sys_UMModel();
                model.SYS_POST = db.SYS_POST.Where(m => m.UUID == id).Include(o=>o.SYS_RANK).FirstOrDefault();
                //model.SysPostRole = db.SYS_POST_ROLE.Where(m => m.SYS_POST_ID == id).ToList();
                return model;
            }
        }

        public ServiceResult UpdateUserPostNew(Sys_UMModel model)
        {
            using(EntitiesAuth db = new EntitiesAuth())
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var sysPost = db.SYS_POST.Where(m => m.UUID == model.SYS_POST.UUID).FirstOrDefault();
                    sysPost.BD_PORTAL_LOGIN = model.SYS_POST.BD_PORTAL_LOGIN;
                    sysPost.CODE = model.SYS_POST.CODE;
                    sysPost.EMAIL = model.SYS_POST.EMAIL;
                    sysPost.FAX_NO = model.SYS_POST.FAX_NO;
                    sysPost.DSMS_USERNAME = model.SYS_POST.DSMS_USERNAME;
                    sysPost.SYS_RANK_ID = model.SYS_POST.SYS_RANK_ID;
                    sysPost.SYS_UNIT_ID = model.SYS_POST.SYS_UNIT_ID;
                    sysPost.PHONE = model.SYS_POST.PHONE;
                    if(!string.IsNullOrEmpty(model.SYS_POST.DSMS_PW))
                        sysPost.DSMS_PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.DSMS_PW);
                    if (!String.IsNullOrEmpty(model.SYS_POST.PW))
                        sysPost.PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.PW);
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
                        Result = ServiceResult.RESULT_SUCCESS,Data = sysPost
                    };

                }
            }
        }
        
        public ServiceResult UpdateUserPost(SYS_POST sysPost)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    //SYS_POST originPost = db.SYS_POST.Where(m => m.UUID == model.SysPost.UUID).FirstOrDefault();
                    //originPost = model.SysPost;
                    db.Entry(sysPost).State = EntityState.Modified;

                    try
                    {
                        int result = db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }

        #endregion
    }
}