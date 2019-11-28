using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MWMS2.Services
{
    public class SysDAOService : BaseDAOService
    {
        public void LoadRoles(DisplayGrid displayGrid, string roleCode, string roleDescription)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                displayGrid.Data = db.SYS_ROLE
                    .Where(o => o.CODE.ToUpper().Contains(roleCode.ToUpper()) || roleCode == null)
                    .Where(o => o.DESCRIPTION.ToUpper().Contains(roleDescription.ToUpper()) || roleDescription == null)
                    .OrderBy(o => o.CODE)
                    .Skip((displayGrid.Page - 1) * displayGrid.Rpp)
                    .Take(displayGrid.Rpp)
                    .ToList()
                    .Select(o => o.ToDictionary())
                    .ToList();
                int total = db.SYS_ROLE
                    .Where(o => o.CODE.ToUpper().Contains(roleCode.ToUpper()) || roleCode == null)
                    .Where(o => o.DESCRIPTION.ToUpper().Contains(roleDescription.ToUpper()) || roleDescription == null)
                    .Count();
                displayGrid.Total = total;
            }
        }

        public void SearchRoleModule(DisplayGrid displayGrid)
        {
            displayGrid.Query = @"SELECT *
                                    FROM   Sys_Func
                                    WHERE  USE_TYPE != 'ICON' ";
            displayGrid.Search();


        }

        public void GetRoleInfo(SysSearchRoleModuleModel model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                model.SYS_ROLE = db.SYS_ROLE.Where(o => o.UUID == model.SYS_ROLE.UUID).FirstOrDefault();
            }
        }
        public IEnumerable<RoleFunc> GetRoleFunc()
        {
            string sSql = @"SELECT SF.UUID As SYS_FUNC_ID,
                                   SF.PARENT_ID,
                                   SF.CODE,
                                   SF.DESCRIPTION,
                                   SF.USE_TYPE,
                                   SF.ABLE_SHOW
                            FROM   SYS_FUNC SF                         
                            WHERE  SF.USE_TYPE NOT IN ( 'ICON' )
                            ORDER  BY SF.UUID,
                                      SF.PARENT_ID ASC ";

   

            return GetObjectData<RoleFunc>(sSql);
        }
        public IEnumerable<RoleFunc> GetRoleFunc(string SysRoleID)
        {
            string sSql = @"SELECT SF.UUID As SYS_FUNC_ID,
                                   SF.PARENT_ID,
                                   SF.CODE,
                                   SF.DESCRIPTION,
                                   SF.USE_TYPE,
                                   SF.ABLE_SHOW,
                                   (Case When USE_TYPE = 'MODULE' Or USE_TYPE = 'TOP_MENU' Then 'Y' Else SRF.CAN_LIST End) CAN_LIST,
                                   (Case When USE_TYPE = 'MODULE' Or USE_TYPE = 'TOP_MENU' Then 'Y' Else SRF.CAN_VIEW_DETAILS End) CAN_VIEW_DETAILS,
                                   (Case When USE_TYPE = 'MODULE' Or USE_TYPE = 'TOP_MENU' Then 'Y' Else SRF.CAN_CREATE End) CAN_CREATE,
                                   (Case When USE_TYPE = 'MODULE' Or USE_TYPE = 'TOP_MENU' Then 'Y' Else SRF.CAN_EDIT End) CAN_EDIT,
                                   (Case When USE_TYPE = 'MODULE' Or USE_TYPE = 'TOP_MENU' Then 'Y' Else SRF.CAN_DELETE End) CAN_DELETE,
                                   SRF.UUID As SYS_ROLE_FUNC_ID,
                                   ( CASE
                                       WHEN SRF.UUID IS NULL THEN 'N'
                                       ELSE 'Y'
                                     END ) AS IS_CHECKED
                            FROM   SYS_FUNC SF
                                   LEFT JOIN SYS_ROLE_FUNC SRF
                                          ON SF.UUID = SRF.SYS_FUNC_ID
                                             AND SRF.SYS_ROLE_ID = :SYS_ROLE_ID
                            WHERE  SF.USE_TYPE NOT IN ( 'ICON' ) AND SF.IS_ACTIVE = 'Y'
                            ORDER  BY SF.UUID,
                                      SF.PARENT_ID ASC ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SYS_ROLE_ID",SysRoleID)
            };

            return GetObjectData<RoleFunc>(sSql, oracleParameters);
        }
        public IEnumerable<UserGroup> GetUserGroup()
        {
            string sSql = @"SELECT DISTINCT T1.UUID,
                                            T1.ENGLISH_DESCRIPTION
                                  FROM   C_S_SYSTEM_VALUE T1
                                   INNER JOIN C_S_SYSTEM_TYPE T2
                                           ON T1.SYSTEM_TYPE_ID = T2.UUID
                                              AND T2.TYPE = 'CONVICTION_SOURCE'
                                   LEFT JOIN (SELECT DISTINCT UGCI.CONVICTION_ID,
                                                              UGCI.SYS_ROLE_ID
                                              FROM   C_S_USER_GROUP_CONV_INFO UGCI
                                                     JOIN SYS_ROLE R
                                                       ON UGCI.SYS_ROLE_ID = R.UUID) T3
                                          ON T3.CONVICTION_ID = T1.UUID
                                 
                            ORDER  BY T1.ENGLISH_DESCRIPTION ";

      
            return GetObjectData<UserGroup>(sSql);
        }
        public IEnumerable<UserGroup> GetUserGroup(string SysRoleID)
        {
            string sSql = @"SELECT DISTINCT T1.UUID,
                                            T1.ENGLISH_DESCRIPTION,
                                            CASE
                                              WHEN T3.SYS_ROLE_ID IS NULL THEN 'N'
                                              ELSE 'Y'
                                            END AS IS_CHECKED
                            FROM   C_S_SYSTEM_VALUE T1
                                   INNER JOIN C_S_SYSTEM_TYPE T2
                                           ON T1.SYSTEM_TYPE_ID = T2.UUID
                                              AND T2.TYPE = 'CONVICTION_SOURCE'
                                   LEFT JOIN (SELECT DISTINCT UGCI.CONVICTION_ID,
                                                              UGCI.SYS_ROLE_ID
                                              FROM   C_S_USER_GROUP_CONV_INFO UGCI
                                                     JOIN SYS_ROLE R
                                                       ON UGCI.SYS_ROLE_ID = R.UUID) T3
                                          ON T3.CONVICTION_ID = T1.UUID
                                             AND T3.SYS_ROLE_ID = :SYS_ROLE_ID
                            ORDER  BY T1.ENGLISH_DESCRIPTION ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SYS_ROLE_ID",SysRoleID)
            };
            return GetObjectData<UserGroup>(sSql, oracleParameters);
        }
        public IEnumerable<Level> GetLevel()
        {
            string sSql = @"SELECT T1.REGISTRATION_TYPE,
                                   T1.UUID,
                                   T1.ENGLISH_DESCRIPTION
                                   FROM   C_S_SYSTEM_VALUE T1
                                   INNER JOIN C_S_SYSTEM_TYPE T2
                                           ON T1.SYSTEM_TYPE_ID = T2.UUID
                                              AND T2.TYPE = 'SEARCHING_LEVEL'
                                   LEFT JOIN (SELECT DISTINCT SL.SEARCHING_LEVEL_ID,
                                                              SL.SYS_ROLE_ID
                                              FROM   C_S_SEARCH_LEVEL SL
                                                     JOIN SYS_ROLE R
                                                       ON SL.SYS_ROLE_ID = R.UUID) T3
                                          ON T3.SEARCHING_LEVEL_ID = T1.UUID
                                            
                            ORDER  BY T1.REGISTRATION_TYPE,
                                      T1.ENGLISH_DESCRIPTION ";

        
            return GetObjectData<Level>(sSql);
        }
        public IEnumerable<Level> GetLevel(string SysRoleID)
        {
            string sSql = @"SELECT T1.REGISTRATION_TYPE,
                                   T1.UUID,
                                   T1.ENGLISH_DESCRIPTION,
                                   CASE
                                     WHEN T3.SYS_ROLE_ID IS NULL THEN 'N'
                                     ELSE 'Y'
                                   END AS IS_CHECKED
                            FROM   C_S_SYSTEM_VALUE T1
                                   INNER JOIN C_S_SYSTEM_TYPE T2
                                           ON T1.SYSTEM_TYPE_ID = T2.UUID
                                              AND T2.TYPE = 'SEARCHING_LEVEL'
                                   LEFT JOIN (SELECT DISTINCT SL.SEARCHING_LEVEL_ID,
                                                              SL.SYS_ROLE_ID
                                              FROM   C_S_SEARCH_LEVEL SL
                                                     JOIN SYS_ROLE R
                                                       ON SL.SYS_ROLE_ID = R.UUID) T3
                                          ON T3.SEARCHING_LEVEL_ID = T1.UUID
                                             AND T3.SYS_ROLE_ID = :SYS_ROLE_ID
                            ORDER  BY T1.REGISTRATION_TYPE,
                                      T1.ENGLISH_DESCRIPTION ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SYS_ROLE_ID",SysRoleID)
            };
            return GetObjectData<Level>(sSql, oracleParameters);
        }

        public IEnumerable<C_S_SYSTEM_VALUE> GetConviction()
        {
            string sSql = @"SELECT SV.*
                            FROM   C_S_SYSTEM_TYPE ST
                                   JOIN C_S_SYSTEM_VALUE SV
                                     ON ST.UUID = SV.SYSTEM_TYPE_ID
                                        AND ST.TYPE = 'CONVICTION_SOURCE' ";
            return GetObjectData<C_S_SYSTEM_VALUE>(sSql);
        }

        public void UpdateRoleInfo(SysSearchRoleModuleModel model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                if (string.IsNullOrWhiteSpace(model.SYS_ROLE.UUID))
                {
                    db.SYS_ROLE.Add(model.SYS_ROLE);

                }
                else
                {
                    SYS_ROLE record = db.SYS_ROLE.Where(o => o.UUID == model.SYS_ROLE.UUID).FirstOrDefault();

                    record.DESCRIPTION = model.SYS_ROLE.DESCRIPTION;

                }


                db.SaveChanges();
            }
        }

        public void UpdateRoleFunc(List<SYS_ROLE_FUNC> models, string SYS_ROLE_ID)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                //Delete Role Func
                db.SYS_ROLE_FUNC.RemoveRange(db.SYS_ROLE_FUNC.Where(d => d.SYS_ROLE_ID == SYS_ROLE_ID &&
                (d.SYS_FUNC.USE_TYPE == "MENU_ITEM" || d.SYS_FUNC.USE_TYPE == "MODULE" || d.SYS_FUNC.USE_TYPE == "TOP_MENU")));


                //Add Role Func
                foreach (var item in models)
                {
                    db.SYS_ROLE_FUNC.Add(item);
                }

                db.SaveChanges();
            }

        }

        public void UpdateUserGroup(List<C_S_USER_GROUP_CONV_INFO> models, string SYS_ROLE_ID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //Delete User Group
                db.C_S_USER_GROUP_CONV_INFO.RemoveRange(db.C_S_USER_GROUP_CONV_INFO.Where(d => d.SYS_ROLE_ID == SYS_ROLE_ID));


                //Add User Group
                foreach (var item in models)
                {
                    db.C_S_USER_GROUP_CONV_INFO.Add(item);
                }

                db.SaveChanges();
            }
        }

        public void UpdateLevel(List<C_S_SEARCH_LEVEL> models, string SYS_ROLE_ID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                //Delete Level
                db.C_S_SEARCH_LEVEL.RemoveRange(db.C_S_SEARCH_LEVEL.Where(d => d.SYS_ROLE_ID == SYS_ROLE_ID));


                //Add Level
                foreach (var item in models)
                {
                  
                    db.C_S_SEARCH_LEVEL.Add(item);
                }

                db.SaveChanges();
            }
        }

        public SYS_RANK getSysRankByRankUuid(String uuid)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_RANK sysRank = db.SYS_RANK.Where(o => o.UUID == uuid).FirstOrDefault();
                return sysRank;
            }
        }

        public SYS_POST getSysPostByCode(String code)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_POST sysPost = db.SYS_POST.Where(o => o.CODE == code).FirstOrDefault();
                return sysPost;
            }
        }

    }
}