using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Controllers;
using MWMS2.Entity;
using MWMS2.Models;
namespace MWMS2.DaoController
{
    public class DaoRoleManagement
    {/*
        private EntitiesSignboard db = new EntitiesSignboard();
        CommonFunction cf = new CommonFunction();
        public IQueryable<B_S_USER_GROUP> GetUserGroup()
        {
            var query = db.B_S_USER_GROUP.OrderBy(x => x.CODE);

            return query;
        }
        public IQueryable<B_S_USER_GROUP> GetUserGroupByUUID(string uuid)
        {
            var query = db.B_S_USER_GROUP.Where(x=>x.UUID==uuid);

            return query;
        }
        public void SaveUserGroup()
        {


        }
        public List<ModelModuleFunction> GetModuleFunction()
        {//after this
               var l = (from mod in db.B_S_MODULE
                        join func in db.B_S_FUNCTION on mod.UUID equals func.MODULE_ID
                        orderby mod.CODE, func.ORDERING
                        select new ModelModuleFunction
                        { 
                            module = mod,
                            function = func
                        }).ToList();
            return l;
        


        }
        public IQueryable<B_S_MODULE> GetModule()
        {
            // var query2 = db.B_S_MODULE.OrderBy(x=>x.B_S_FUNCTION.OrderBy(y => y.ORDERING).Select(y=>y.ORDERING).FirstOrDefault());
            var query = db.B_S_MODULE.OrderBy(x => x.ORDERING);
     //if i comment this !!!!!       var test = GetModuleFunction();

            return query;
        }

       
   
        public IQueryable<B_S_USER_GROUP_FUNCTION_INFO> GetSelectFunction(string uuid)
        {
            var query = db.B_S_USER_GROUP_FUNCTION_INFO.Where(x=>x.USERGROUP_ID==uuid);
            return query;
        }*/
    }
}