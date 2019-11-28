using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.DaoController;
using MWMS2.Models;
using MWMS2.Dao;
namespace MWMS2.Controllers
{
    public class UserManController : Controller
    {
        // GET: UserMan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserManagementRefresh()
        {
           return RedirectToAction("UserManagement");
        }
        public ActionResult UserManagement(string UserName,string Status, string Rank, string Email)
        {
            DaoUserManagement daoUserManagement = new DaoUserManagement();
            var query = daoUserManagement.GetUserByCriteria( UserName,  Status,  Rank,  Email);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList.Add(new SelectListItem
            {
                Text = "-- Please Select --",
                Value = "",
                Selected = true
            }

                );

            foreach (var item in SystemParameterConstant.StatusLevel)
            {

                StatusList.Add(new SelectListItem
                {
                    Text = item,
                    Value = item,
                   
                });
              
            }
            ViewBag.StatusList = StatusList;








            return View(query.ToList());
        }
        public ActionResult UserManagementEdit(string uuid)
        {
            DaoUserManagement daoUserManagement = new DaoUserManagement();
            bool existingRecord = false;
            var query = daoUserManagement.GetUserByUUID(uuid);
            if (query.Count() != 0)
                existingRecord = true;



            List<SelectListItem> SecurityLevelList = new List<SelectListItem>();               
            foreach (var item in SystemParameterConstant.SecurityLevel)
            {
                bool selected = false;
                if (existingRecord)
                {
                    if (item == query.FirstOrDefault().SECURITY_LEVEL)
                    {
                        selected = true;
                    }
                }
                SecurityLevelList.Add(new SelectListItem
                {
                    Text = item,
                    Value = item,
                    Selected = selected
                });
                selected = false;
            }
            ViewBag.SecurityLevelList = SecurityLevelList;


            List<SelectListItem> RankWFList = new List<SelectListItem>();

            foreach (var item in SystemParameterConstant.RankWF)
            {
                bool selected = false;
                if (existingRecord)
                {
                    if (item.Key == query.FirstOrDefault().RANK)
                    {
                        selected = true;
                    }
                }
                RankWFList.Add(new SelectListItem
                {
                    Text = item.Key,
                    Value = item.Key,
                    Selected = selected
                });
            
            }
            ViewBag.RankWFList = RankWFList;

            ViewBag.UserGroup = daoUserManagement.GetUserGroup();
            ViewBag.SelectedUserGroup = query.SelectMany(x => x.B_S_USER_ACCOUNT_GROUP_INFO);

            return View(query.FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserManagementSave( [Bind(Exclude = "")]  B_S_USER_ACCOUNT userAcouunt, string[] v_UserGroup_CheckBox)
        {
            DaoUserManagement daoUserManagement = new DaoUserManagement();
            daoUserManagement.UserSave(userAcouunt, v_UserGroup_CheckBox);
           return RedirectToAction("UserManagement");
        }
        }
}