using MWMS2.DaoController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class RMController : Controller
    {
        /*
        // GET: Admin/RM
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RoleManagement()
        {
            DaoRoleManagement daoRoleManagement = new DaoRoleManagement();
            var query = daoRoleManagement.GetUserGroup();
            return View(query.ToList());
        }
        public ActionResult RoleManagementEdit(string uuid)
        {
            DaoRoleManagement daoRoleManagement = new DaoRoleManagement();
            var query = daoRoleManagement.GetUserGroupByUUID(uuid);
            var SelectedFunctionQuery = daoRoleManagement.GetSelectFunction(uuid);
            var ModuleQuery = daoRoleManagement.GetModule();
            // var ModuleFunctionQuery = daoRoleManagement.GetModuleFunction();
            //ViewBag.ModuleFunction = ModuleFunctionQuery;
            ViewBag.Module = ModuleQuery;
            ViewBag.SelectedFunction = SelectedFunctionQuery;
            ///foreach (var item in ModuleQuery)
            ///{
            ///    foreach (var function in item.B_S_FUNCTION.OrderBy(x=>x.ORDERING))
            ///    {
            ///
            ///    }
            ///
            ///}
            return View(query.FirstOrDefault());
        }
        */
    }
}