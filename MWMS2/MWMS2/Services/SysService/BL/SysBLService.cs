using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class SysBLService
    {
        private static volatile SysDAOService _DAO;
        private static readonly object locker = new object();
        private static SysDAOService DAO { get { if (_DAO == null) lock (locker) if (_DAO == null) _DAO = new SysDAOService(); return _DAO; } }


        public void LoadRoles(SysSearchModel model)
        {
            DAO.LoadRoles(model, model.SearchRoleCode, model.SearchRoleDesc);
        }


        public void GetRoleFunc(SysSearchRoleModuleModel model)
        {
            DAO.GetRoleInfo(model);
            model.RoleFuncList = new List<RoleFunc>();
            model.LevelList = new List<Level>();
            model.UserGoupList = new List<UserGroup>();
            if (model.SYS_ROLE != null)
            {
                model.RoleFuncList = DAO.GetRoleFunc(model.SYS_ROLE.UUID).ToList();
                model.LevelList = DAO.GetLevel(model.SYS_ROLE.UUID).ToList();
                model.UserGoupList = DAO.GetUserGroup(model.SYS_ROLE.UUID).ToList();
            }
            else
            {
                model.RoleFuncList = DAO.GetRoleFunc().ToList();
                model.LevelList = DAO.GetLevel().ToList();
                model.UserGoupList = DAO.GetUserGroup().ToList();
            }


        }

        public JsonResult SaveRoleFunc(SysSearchRoleModuleModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {

                //Update Role Info
                DAO.UpdateRoleInfo(model);

                //Update Role Func
                List<SYS_ROLE_FUNC> RoleFuncs = new List<SYS_ROLE_FUNC>();

                foreach (var item in model.RoleFuncList)
                {

                    if (item.IsChecked)
                    {
                        if (item.USE_TYPE == "TOP_MENU")
                        {
                            item.CAN_EDIT = "Y";

                        }
                        RoleFuncs.Add(new SYS_ROLE_FUNC()
                        {
                            SYS_ROLE_ID = model.SYS_ROLE.UUID,
                            SYS_FUNC_ID = item.SYS_FUNC_ID,
                            CAN_LIST = item.CAN_LIST,
                            CAN_VIEW_DETAILS = item.CAN_VIEW_DETAILS,
                            CAN_CREATE = item.CAN_CREATE,
                            CAN_EDIT = item.CAN_EDIT,
                            CAN_DELETE = item.CAN_DELETE
                        });
                    }
                }

                //Update User Group
                List<C_S_USER_GROUP_CONV_INFO> UserGroupList = new List<C_S_USER_GROUP_CONV_INFO>();

                foreach (var item in model.UserGoupList)
                {
                    if (item.IsChecked)
                    {

                        UserGroupList.Add(new C_S_USER_GROUP_CONV_INFO()
                        {
                            CONVICTION_ID = item.UUID,
                            SYS_ROLE_ID = model.SYS_ROLE.UUID
                        });

                    }
                }

                //Update Level
                List<C_S_SEARCH_LEVEL> LevelList = new List<C_S_SEARCH_LEVEL>();
                foreach (var item in model.LevelList)
                {
                    if (item.IsChecked)
                    {

                        LevelList.Add(new C_S_SEARCH_LEVEL()
                        {
                            SEARCHING_LEVEL_ID = item.UUID,
                            SYS_ROLE_ID = model.SYS_ROLE.UUID
                        });

                    }
                }

                DAO.UpdateRoleFunc(RoleFuncs, model.SYS_ROLE.UUID);

                DAO.UpdateUserGroup(UserGroupList, model.SYS_ROLE.UUID);

                DAO.UpdateLevel(LevelList, model.SYS_ROLE.UUID);


                serviceResult.Result = ServiceResult.RESULT_SUCCESS;

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };

        }

        public string ExportRoles(SysSearchModel model)
        {
            LoadRoles(model);
            return model.ExportCurrentData("Excel_" + DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}