using MWMS2.Entity;
using MWMS2.DaoController;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Admin.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SysController : ValidationController
    {
        private static volatile SysBLService _BL;
        private static readonly object locker = new object();
        private static SysBLService BL { get { if (_BL == null) lock (locker) if (_BL == null) _BL = new SysBLService(); return _BL; } }

        public ActionResult RoleIndex()
        {
            return View();
        }
        public ActionResult SearchRoles(SysSearchModel model)
        {
            BL.LoadRoles(model);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ActionResult RoleDetail(string SYS_ROLE_ID)
        {
            SysSearchRoleModuleModel model = new SysSearchRoleModuleModel()
            {
                SYS_ROLE = new SYS_ROLE() { UUID = SYS_ROLE_ID }
            };
            BL.GetRoleFunc(model);
            return View(model);
        }
        public ActionResult CreateRole()
        {
            //planning
            SysSearchRoleModuleModel model = new SysSearchRoleModuleModel()
            {
                SYS_ROLE = new SYS_ROLE() { UUID = "" }
            };
            BL.GetRoleFunc(model);
            return View("RoleDetail",model);
        }
        public ActionResult SaveRoleFunc(SysSearchRoleModuleModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.SaveRoleFunc(model);
        }

        public ActionResult ExportRoles(SysSearchModel model)
        {

            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.ExportRoles(model) });
        }

        /*
        public ActionResult PostIndex()
        {
            return View();
        }*/
        /*
        // GET: Admin/UM
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserManagementRefresh()
        {
            return RedirectToAction("UserManagement");
        }*/
        //public ActionResult UserManagement(string UserName, string Status, string Rank, string Email)
        //{
        //    DaoUserManagement daoUserManagement = new DaoUserManagement();
        //    var query = daoUserManagement.GetUserByCriteria(UserName, Status, Rank, Email);

        //    List<SelectListItem> StatusList = new List<SelectListItem>();
        //    StatusList.Add(new SelectListItem
        //    {
        //        Text = "-- Please Select --",
        //        Value = "",
        //        Selected = true
        //    }

        //        );

        //    foreach (var item in SystemParameterConstant.StatusLevel)
        //    {

        //        StatusList.Add(new SelectListItem
        //        {
        //            Text = item,
        //            Value = item,

        //        });

        //    }
        //    ViewBag.StatusList = StatusList;








        //    return View(query.ToList());
        //}
        //public ActionResult UserManagementEdit(string uuid)
        //{
        //    DaoUserManagement daoUserManagement = new DaoUserManagement();
        //    bool existingRecord = false;
        //    var query = daoUserManagement.GetUserByUUID(uuid);
        //    if (query.Count() != 0)
        //        existingRecord = true;



        //    List<SelectListItem> SecurityLevelList = new List<SelectListItem>();
        //    foreach (var item in SystemParameterConstant.SecurityLevel)
        //    {
        //        bool selected = false;
        //        if (existingRecord)
        //        {
        //            if (item == query.FirstOrDefault().SECURITY_LEVEL)
        //            {
        //                selected = true;
        //            }
        //        }
        //        SecurityLevelList.Add(new SelectListItem
        //        {
        //            Text = item,
        //            Value = item,
        //            Selected = selected
        //        });
        //        selected = false;
        //    }
        //    ViewBag.SecurityLevelList = SecurityLevelList;


        //    List<SelectListItem> RankWFList = new List<SelectListItem>();

        //    foreach (var item in SystemParameterConstant.RankWF)
        //    {
        //        bool selected = false;
        //        if (existingRecord)
        //        {
        //            if (item.Key == query.FirstOrDefault().RANK)
        //            {
        //                selected = true;
        //            }
        //        }
        //        RankWFList.Add(new SelectListItem
        //        {
        //            Text = item.Key,
        //            Value = item.Key,
        //            Selected = selected
        //        });

        //    }
        //    ViewBag.RankWFList = RankWFList;

        //    ViewBag.UserGroup = daoUserManagement.GetUserGroup();
        //   //db fk pk missing  ViewBag.SelectedUserGroup = query.SelectMany(x => x.B_S_USER_ACCOUNT_GROUP_INFO);

        //    return View(query.FirstOrDefault());
        //}
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult UserManagementSave([Bind(Exclude = "")]  B_S_USER_ACCOUNT userAcouunt, string[] v_UserGroup_CheckBox)
        //    {
        //        DaoUserManagement daoUserManagement = new DaoUserManagement();
        //        daoUserManagement.UserSave(userAcouunt, v_UserGroup_CheckBox);
        //        return RedirectToAction("UserManagement");
        //    }
    }
}
