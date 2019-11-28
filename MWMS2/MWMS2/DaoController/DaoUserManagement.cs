using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MWMS2.Controllers;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.DaoController
{
    public class DaoUserManagement
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();
//        internal static readonly Dictionary<string, int> enumOrder =
//new Dictionary<string, int>() {
//      {"SPO", 1},
//      {"PO", 2},
//      {"TO", 3},
//      {"TC",4 },
//      { "others",5 },

//};
        //public IOrderedEnumerable<B_S_USER_ACCOUNT> GetUserByCriteria(string UserName, string Status, string Rank, string Email)
        //{
        //    var query = from user in db.B_S_USER_ACCOUNT
        //                select user;
        //    if (!string.IsNullOrEmpty(UserName))
        //    {
        //        query = query.Where(x => x.USERNAME.ToUpper().Contains(UserName.ToUpper()));
        //    }
        //    if (!string.IsNullOrEmpty(Status))
        //    {
        //        query = query.Where(x => x.STATUS== Status);
        //    }
        //    if (!string.IsNullOrEmpty(Rank))
        //    {
        //        query = query.Where(x => x.RANK.Contains(Rank));
        //    }
        //    if (!string.IsNullOrEmpty(Email))
        //    {
        //        query = query.Where(x => x.EMAIL.ToUpper().Contains(Email.ToUpper()));
        //    }

        //    var result = query.AsEnumerable().OrderBy(x => SystemParameterConstant.RankWF[x.RANK == null ? "others" : x.RANK]);
         


        //    return result;
        //}
        //public IQueryable<B_S_USER_ACCOUNT> GetUserByUUID(string uuid)
        //{
        //    var query = db.B_S_USER_ACCOUNT.Where(x => x.UUID == uuid);
        //    return query;
        //}

        //public IQueryable<B_S_USER_GROUP> GetUserGroup()
        //{
        //    var query = db.B_S_USER_GROUP.Where(x=>x.STATUS== "ACTIVE").OrderBy(x=>x.DESCRIPTION);
        //    return query;
        //}
        //public void UserSave(B_S_USER_ACCOUNT userAccount, string[] UserGroup_checkbox) 
        //{
        //    bool existingRecord = userAccount.UUID == null ? false : true;
        //    if (!existingRecord)
        //    {
        //        userAccount.UUID = Guid.NewGuid().ToString();
             
        //        userAccount.CREATED_DATE = System.DateTime.Now;
        //        userAccount.CREATED_BY = SystemParameterConstant.UserName;
        //    }
        //    userAccount.MODIFIED_DATE = System.DateTime.Now;
        //    userAccount.MODIFIED_BY = SystemParameterConstant.UserName;
        //    db.B_S_USER_ACCOUNT_GROUP_INFO.RemoveRange(db.B_S_USER_ACCOUNT_GROUP_INFO.Where(x => x.USER_ACCOUNT_ID == userAccount.UUID));
        //    foreach (var item in UserGroup_checkbox)
        //    {
        //        B_S_USER_ACCOUNT_GROUP_INFO UAG = new B_S_USER_ACCOUNT_GROUP_INFO();
        //        UAG.UUID= Guid.NewGuid().ToString();
        //        UAG.USER_ACCOUNT_ID = userAccount.UUID;
        //        UAG.USER_GROUP_ID = item;
        //        UAG.CREATED_DATE = System.DateTime.Now;
        //        UAG.CREATED_BY = SystemParameterConstant.UserName;
        //        UAG.MODIFIED_DATE = System.DateTime.Now;
        //        UAG.MODIFIED_BY = SystemParameterConstant.UserName;
        //        db.B_S_USER_ACCOUNT_GROUP_INFO.Add(UAG);

        //    }
          
            


        //    if (existingRecord)
        //    {
        //        db.Entry(userAccount).State = EntityState.Modified;
           
        //    }
        //    else
        //    {
        //        db.B_S_USER_ACCOUNT.Add(userAccount);              
        //    }
        //    db.SaveChanges();
        //}




        private List<SYS_FUNC> GetFuncTree()
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                List<SYS_FUNC> funcs = db.SYS_FUNC.Where(o => o.PARENT_ID == null).Include(o => o.SYS_FUNC2.SYS_FUNC2).ToList();
                return funcs;
            }

        }

    }
}