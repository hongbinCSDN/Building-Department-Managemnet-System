using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.AdminService.DAO;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.AdminService.BL
{
    public class Sys_UMBLService
    {

        private static volatile Sys_UMDAOService _DA;
        private static readonly object locker = new object();
        private static Sys_UMDAOService DA { get { if (_DA == null) lock (locker) if (_DA == null) _DA = new Sys_UMDAOService(); return _DA; } }


        #region Search
        private string SearchUsers_whereQ(Sys_UMModel model)
        {
            string whereq = "";

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.DSMS_USERNAME))
            {
                whereq += @" and UPPER(T1.dsms_username) like :UserName ";
                model.QueryParameters.Add("UserName", "%" + model.SYS_POST.DSMS_USERNAME.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.IS_ACTIVE))
            {
                whereq += @" and UPPER(T1.is_active)= :Status ";
                model.QueryParameters.Add("Status", model.SYS_POST.IS_ACTIVE.ToUpper().Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.SYS_RANK_ID))
            {
                whereq += @" and UPPER(T1.sys_rank_id)= :Rank ";
                model.QueryParameters.Add("Rank", model.SYS_POST.SYS_RANK_ID.ToUpper().Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.BD_PORTAL_LOGIN))
            {
                whereq += @" and UPPER(T1.bd_portal_login) like :PortalLogin ";
                model.QueryParameters.Add("PortalLogin", "%" + model.SYS_POST.BD_PORTAL_LOGIN.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.SUPERVISOR_ID))
            {
                whereq += @" and UPPER(T1.supervisor_id)= :SupervisorID ";
                model.QueryParameters.Add("SupervisorID", model.SYS_POST.SUPERVISOR_ID.ToUpper().Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.PHONE))
            {
                whereq += @" and T1.phone like :Phone ";
                model.QueryParameters.Add("Phone", "%" + model.SYS_POST.PHONE + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.Role))
            {
                whereq += @" and UPPER(T1.CODE) = :Role ";
                model.QueryParameters.Add("Role", model.Role.ToUpper().Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.EMAIL))
            {
                whereq += @" and UPPER(T1.email) like :Email ";
                model.QueryParameters.Add("Email", "%" + model.SYS_POST.EMAIL.ToUpper().Trim() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.CODE))
            {
                whereq += @" and UPPER(T1.code) like :Code ";
                model.QueryParameters.Add("Code", "%" + model.SYS_POST.CODE.ToUpper().Trim() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.FAX_NO))
            {
                whereq += @" and T1.fax_no like :Fax ";
                model.QueryParameters.Add("Fax", "%" + model.SYS_POST.FAX_NO + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.RECEIVE_CASE))
            {
                whereq += @" and UPPER(T1.receive_case)= :ReceiveCase ";
                model.QueryParameters.Add("ReceiveCase", model.SYS_POST.RECEIVE_CASE.ToUpper().Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.SYS_POST.SYS_UNIT_ID))
            {
                whereq += @" and UPPER(T1.sys_unit_id)= :Unit ";
                model.QueryParameters.Add("Unit", model.SYS_POST.SYS_UNIT_ID.ToUpper());
            }

            return whereq;
        }

        public Sys_UMModel SearchUsers(Sys_UMModel model)
        {
            model.QueryWhere = SearchUsers_whereQ(model);
            return DA.SearchUsers(model);
        }
        public void AjaxPostRole(Sys_UMModel model)
        {
            DA.AjaxPostRole(model);
        }
        public void AjaxScuSubordinateMember(Sys_UMModel model)
        {
            DA.AjaxScuSubordinateMember(model);
            model.Data.Add(new Dictionary<string, object>() { { "UUID", "" } });
            model.Total = model.Data.Count;
        }
        public void AjaxPemSubordinateMember(Sys_UMModel model)
        {
            DA.AjaxPemSubordinateMember(model);
            model.Data.Add(new Dictionary<string, object>() { { "UUID", "" } });
            model.Total = model.Data.Count;
        }


        
        public void AjaxScuResponsibleArea(Sys_UMModel model)
        {
            DA.AjaxScuResponsibleArea(model);
            model.Data.Add(new Dictionary<string, object>() { { "AREA_CODE", "" } });
            model.Total = model.Data.Count;
        }
        

        #endregion

        #region Add
        public ServiceResult SaveUser(Sys_UMModel model)
        {
            return DA.SaveUser(model);
        }
        #endregion
        /*
       #region Common
       public Sys_UMModel GetRolesList(Sys_UMModel model)
       {
           List<SYS_ROLE> roles = DA.GetRolesList();
           foreach (SYS_ROLE item in roles)
           {
               if (model.SysPostRole != null && model.SysPostRole.Count() > 0)
               {
                   if (model.SysPostRole.Where(m => m.SYS_ROLE_ID == item.UUID).FirstOrDefault() != null)
                   {
                       model.SysRoleCheckedBoxes.Add(new Sys_Role_CheckedBox() { SysRoleID = item.UUID, SysRoleCode = item.CODE, SysRoleDes = item.DESCRIPTION, IsChecked = true });
                   }
                   else
                   {
                       model.SysRoleCheckedBoxes.Add(new Sys_Role_CheckedBox() { SysRoleID = item.UUID, SysRoleCode = item.CODE, SysRoleDes = item.DESCRIPTION });
                   }
               }
               else
               {
                   model.SysRoleCheckedBoxes.Add(new Sys_Role_CheckedBox() { SysRoleID = item.UUID, SysRoleCode = item.CODE, SysRoleDes = item.DESCRIPTION });
               }
           }
           return model;
       }

       #endregion
    */
        #region Edit
        public Sys_UMModel GetPostByUUID(string id)
       {
           Sys_UMModel model = DA.GetPostByUUID(id);
          // model.SysRoleCheckedBoxes = new List<Sys_Role_CheckedBox>();
           //model = GetRolesList(model);
           return model;
       }
        /*
        public ServiceResult UpdateUser(Sys_UMModel model)
        {
            return DA.UpdateUser(model);
        }*/
        
        public ServiceResult UpdateUserPost(Sys_UMModel model)
        {
            using (EntitiesAuth auth = new EntitiesAuth())
            {
                // check unique portal login name & post code
                if (!string.IsNullOrWhiteSpace(model.SYS_POST.UUID))
                {
                    bool existPortal = auth.SYS_POST.Where(o => o.BD_PORTAL_LOGIN == model.SYS_POST.BD_PORTAL_LOGIN)
                          .Where(o => o.UUID != model.SYS_POST.UUID).Count() > 0;
                    bool existPostCode = auth.SYS_POST.Where(o => o.CODE == model.SYS_POST.CODE)
                          .Where(o => o.UUID != model.SYS_POST.UUID).Count() > 0;
                    if (existPortal || existPostCode)
                    {
                        Dictionary<string, List<string>> v = new Dictionary<string, List<string>>();
                        if (existPortal) v["SYS_POST.BD_PORTAL_LOGIN"] = new List<string>() { "Portal ID already exist." };
                        if (existPostCode) v["SYS_POST.CODE"] = new List<string>() { "Post Code already exist." };
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = v };
                    }

                }

                //Sys_UMModel sysUMModel = DA.GetPostByUUID(model.SYS_POST.UUID);
                //SYS_POST sysPost = sysUMModel.SYS_POST;
                //sysPost.BD_PORTAL_LOGIN = model.SYS_POST.BD_PORTAL_LOGIN;
                //sysPost.CODE = model.SYS_POST.CODE;
                //sysPost.EMAIL = model.SYS_POST.EMAIL;
                //sysPost.FAX_NO = model.SYS_POST.FAX_NO;
                //sysPost.DSMS_USERNAME = model.SYS_POST.DSMS_USERNAME;
                //sysPost.SYS_RANK_ID = model.SYS_POST.SYS_RANK_ID;

                //if (!String.IsNullOrEmpty(model.SYS_POST.DSMS_PW))
                //{
                //    sysPost.DSMS_PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.DSMS_PW);
                //}

                //if (!String.IsNullOrEmpty(model.SYS_POST.PW))
                //{
                //    sysPost.PW = EncryptDecryptUtil.getEncrypt(model.SYS_POST.PW);
                //}

                //ServiceResult result = DA.UpdateUserPost(sysPost);
                //if (result.Result.Equals(ServiceResult.RESULT_SUCCESS))
                //{
                //    Utility.SessionUtil.LoginPost = sysPost;
                //}

                ServiceResult result = DA.UpdateUserPostNew(model);
                if (result.Result.Equals(ServiceResult.RESULT_SUCCESS))
                    SessionUtil.LoginPost = (SYS_POST)result.Data;


                return result;
            }
        }
        
        #endregion
    }
}