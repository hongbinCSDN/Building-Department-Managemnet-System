
using MWMS2.Entity;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MWMS2.Filter
{
    public class SystemFilter : ActionFilterAttribute
    {
        //ZZZ is admin
        private const string test_PortalID = "KWTAM";

        public static string URL_AREA = "area";
        public static string URL_ACTION = "action";
        public static string URL_CONTROLLER = "controller";

       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {   
            SessionUtil.LastAction = getDataTokens(filterContext, SystemFilter.URL_ACTION);
            
            /*string header = "";
            foreach (string k in HttpContext.Current.Request.Headers.AllKeys)
            {
                header = header + k+"===" + 
            }*/

            AuditLogService.Log(AuditLogService.AUDIT_LOG_IN_DP, "");


            if (!string.IsNullOrWhiteSpace(test_PortalID))
            {
                HttpContext.Current.Request.Headers.Add(AuthManager.HEADER_KEY_DEPT, "BD");
                HttpContext.Current.Request.Headers.Add(AuthManager.HEADER_KEY_POST, test_PortalID);
            }

            try
            {
                if (isBDPortalLoginPage(filterContext))
                {
                    AuthManager.Auth();
                }
                if (isExcludedPathForLoginPage(filterContext)) {
                    return;
                }


                if (SessionUtil.LoginPost is null) {
                    goToSessionTimeOut(filterContext);
                    return;
                }
                string pageCode = filterContext.RequestContext.HttpContext.Request.Form["pageCode"];
                MenuManager MenuManager = new MenuManager(SessionUtil.LoginPost, filterContext);
                MenuManager.LoadMenu(filterContext);
            }
            catch (Exception ex){
                AuditLogService.logDebug("Error OnActionExecuting"+ex);
                AuditLogService.logDebug(ex.Message);
                goToLogin(filterContext);
                return;
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (filterContext.Exception!= null){
                    AuditLogService.logDebug("Error: " + filterContext.Exception);
            }
        
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            String ApplicationServerNode = ConfigurationManager.AppSettings["ApplicationServerNode"];
            if(ApplicationServerNode != null) filterContext.HttpContext.Response.AppendHeader("ApplicationServerNode", ApplicationServerNode);
            base.OnResultExecuted(filterContext);


        }

        public bool isBDPortalLoginPage(ControllerContext filterContext)
        {   String area = getDataTokens(filterContext, SystemFilter.URL_AREA);
            String lastAction = getDataTokens(filterContext, SystemFilter.URL_ACTION);
            String controllerName = getDataTokens(filterContext, SystemFilter.URL_CONTROLLER);
            bool result = false;
            if ((String.IsNullOrEmpty(area) && "CMN".Equals(controllerName) && "CMN01".Equals(lastAction))) {
                result= true;
            }
            return result;


        }


        public bool isExcludedPathForLoginPage(ControllerContext filterContext)
        {
            String lastAction = getDataTokens(filterContext, SystemFilter.URL_ACTION);
            String controllerName = getDataTokens(filterContext, SystemFilter.URL_CONTROLLER);
            String area = getDataTokens(filterContext, SystemFilter.URL_AREA);
            if ( ( "CMN".Equals(area) && "LOGIN".Equals(controllerName) && "LOGIN".Equals(lastAction)) ||
                 ("CMN".Equals(area) && "LOGIN".Equals(controllerName) && "CHECKLOGIN".Equals(lastAction))||
                ("CMN".Equals(area) && "LOGIN".Equals(controllerName) && "DOLOGIN".Equals(lastAction)) ||
                ("CMN".Equals(area) && "LOGIN".Equals(controllerName) && "TIMEOUT".Equals(lastAction)) ||
                ("DOCSERVICE".Equals(controllerName)) || ("DOC".Equals(controllerName)) || ("SMMDOC".Equals(controllerName)))
            {
                return true;
            }
            return false;
        }



        private void SimAD(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary{
                            {"controller", "Login"},
                            {"action", "login"},
                            {"area", "CMN"},
                        }
            );
        }

        private void goToSessionTimeOut(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary{
                            {"controller", "Login"},
                            {"action", "timeOut"},
                            {"area", "CMN"},
                        }
            );
        }

        private void goToLogin(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary{
                            {"controller", "Login"},
                            {"action", "login"},
                            {"area", "CMN"},
                        }
            );
        }
        public String getDataTokens(ControllerContext filterContext, String key)
        {
            String result = "";
            if (URL_AREA.Equals(key))
            {
                if (filterContext.RequestContext.RouteData.DataTokens[key] is null)
                {
                    return result;
                }
                else
                {
                    return filterContext.RequestContext.RouteData.DataTokens[key].ToString().ToUpper();
                }

            }
            else
            {
                if (filterContext.RequestContext.RouteData.Values[key] is null)
                {
                    return result;
                }
                else
                {
                    return filterContext.RequestContext.RouteData.Values[key].ToString().ToUpper();



                }
            }
        }

    }
    class MenuManager
    {
        

        private readonly SYS_POST SYS_POST;
        private  string currentPageCode;
        private readonly ActionExecutingContext filterContext;
        public MenuManager(SYS_POST post, ActionExecutingContext filterContext)
        {
            this.SYS_POST = post;
            this.filterContext = filterContext;
            this.currentPageCode = filterContext == null ? null : filterContext.RequestContext.HttpContext.Request.Form["pageCode"];
        }
        public void LoadMenu(ActionExecutingContext filterContext){


            AuditLogService.logDebug("LoadMenu");
            if (SYS_POST is null || string.IsNullOrWhiteSpace(SYS_POST.UUID))
            {
                return;
            }

            using (EntitiesAuth db = new EntitiesAuth())
            {

                List<SYS_ROLE_FUNC> SYS_ROLE_FUNCs;
                if (SessionUtil.AccessableFunctions is null){
                    AuditLogService.logDebug("start");
                    string q = ""
                        + "\r\n\t" + " SELECT DISTINCT T5.* FROM SYS_POST T1                             "
                        + "\r\n\t" + " INNER JOIN SYS_POST_ROLE T2 ON T2.SYS_POST_ID = T1.UUID           "
                        + "\r\n\t" + " INNER JOIN SYS_ROLE T3 ON T3.UUID = T2.SYS_ROLE_ID                "
                        + "\r\n\t" + " INNER JOIN SYS_ROLE_FUNC T4 ON T4.SYS_ROLE_ID = T3.UUID           "
                        + "\r\n\t" + " INNER JOIN SYS_FUNC T5 ON T5.UUID = T4.SYS_FUNC_ID                "
                        + "\r\n\t" + " WHERE T1.UUID = :SYS_POST_ID ORDER BY SEQ                         ";
                    //List<SYS_FUNC> SYS_FUNCs = db.SYS_FUNC.SqlQuery(q, new OracleParameter("SYS_POST_ID", SYS_POST.SYS_POST_ID)).ToList<SYS_FUNC>();
                    SYS_ROLE_FUNCs = db.SYS_POST_ROLE.Where(o => o.SYS_POST_ID == SYS_POST.UUID).SelectMany(o => o.SYS_ROLE.SYS_ROLE_FUNC).Where(o=>o.SYS_FUNC.IS_ACTIVE == "Y").Include(o => o.SYS_FUNC).ToList();
                    AuditLogService.logDebug("SYS_ROLE_FUNCs Number:" + SYS_ROLE_FUNCs.Count().ToString());
                    AuditLogService.logDebug(" SYS_POST.UUID:" + SYS_POST.UUID);

                    SessionUtil.AccessableFunctions = SYS_ROLE_FUNCs;
                    SessionUtil.IconFunctions = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.USE_TYPE.Equals("ICON")).Select(o => o.SYS_FUNC).Distinct().OrderByDescending(O => O.SEQ).ToList();
                    SessionUtil.ModuleFunctions = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.USE_TYPE.Equals("MODULE")).Select(o => o.SYS_FUNC).Distinct().OrderByDescending(O => O.SEQ).ToList();


                    SessionUtil.LoginPostRoleList  = db.SYS_POST_ROLE.Where(x => x.SYS_POST.BD_PORTAL_LOGIN == SYS_POST.BD_PORTAL_LOGIN).ToList();
                     
                 
                  



                    for (int i = 0; i < SYS_ROLE_FUNCs.Count; i++)
                    {
                        SYS_FUNC f = SYS_ROLE_FUNCs[i].SYS_FUNC;
                        f.Childs = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.PARENT_ID == f.UUID).OrderBy(o => o.SYS_FUNC.SEQ).Select(o => o.SYS_FUNC).Distinct().ToList();

                        for (int j = 0; j < f.Childs.Count; j++)
                        {
                            SYS_FUNC sf = f.Childs[j];
                            sf.Childs = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.PARENT_ID == sf.UUID).OrderBy(o => o.SYS_FUNC.SEQ).Select(o => o.SYS_FUNC).Distinct().ToList();
                        }
                    }

                    #region get search level
                    List<C_S_SYSTEM_VALUE> acessRight = new List<C_S_SYSTEM_VALUE>();
                    using (DbConnection conn = db.Database.Connection)
                        {

                        String sql  = " SELECT " +
                               " DISTINCT V.REGISTRATION_TYPE, V.CODE FROM SYS_POST T, SYS_POST_ROLE R, C_S_SEARCH_LEVEL L, C_S_SYSTEM_VALUE V  " +
                               " WHERE T.UUID = R.SYS_POST_ID AND R.SYS_ROLE_ID = L.SYS_ROLE_ID AND L.SEARCHING_LEVEL_ID = V.UUID  " +
                               " AND T.UUID = :SYS_POST_ID ";
                        Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                        queryParameters.Add("SYS_POST_ID", SYS_POST.UUID);

                        DbDataReader dr = CommonUtil.GetDataReader(conn, sql, queryParameters);
                            while (dr.Read())
                            { acessRight.Add(new C_S_SYSTEM_VALUE
                                   {   REGISTRATION_TYPE = (String)dr.GetValue(0),
                                       CODE = (String)dr.GetValue(1)
                                   });
                            }
                            dr.Close();
                            conn.Close();
                        }
                    SessionUtil.SearchLevelRights = acessRight;
                    #endregion


                  

                    //setModuleFirstPage/

                    SYS_FUNC pemModule = SessionUtil.ModuleFunctions.Where(o => o.CODE == "2000").FirstOrDefault();
                    SYS_FUNC smmModule = SessionUtil.ModuleFunctions.Where(o => o.CODE == "3000").FirstOrDefault();
                    SYS_FUNC crmModule = SessionUtil.ModuleFunctions.Where(o => o.CODE == "4000").FirstOrDefault();
                    RedirectDefaultUrl(pemModule, "2201", true);
                    RedirectDefaultUrl(smmModule, "3201", true);
                    RedirectDefaultUrl(crmModule, null, true);



                }
                else
                {
                    SYS_ROLE_FUNCs = SessionUtil.AccessableFunctions;
                }
                SYS_ROLE_FUNC loadRoleFunc = null;
                if (string.IsNullOrWhiteSpace(currentPageCode))
                {
                    if (filterContext != null)
                    {
                        string[] split = filterContext.Controller.GetType().FullName.Split('.');
                        if (split.Length >= 5)
                        {
                            string split4 = split[4].Replace("Controller", "");
                            if (!split4.Equals(split[4]))
                            {
                                string split2 = split[2];
                                loadRoleFunc = 
                                SYS_ROLE_FUNCs
                                    .Where(o => o.SYS_FUNC.FunctionSplit != null
                                && o.SYS_FUNC.FunctionSplit.Length >= 2
                                && o.SYS_FUNC.FunctionSplit[0] == split2
                                && o.SYS_FUNC.FunctionSplit[1] == split4).FirstOrDefault();
                            }
                        }
                    }
                }
                SessionUtil.CurrentFunction = loadRoleFunc != null ? loadRoleFunc.SYS_FUNC : SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.CODE == currentPageCode).Select(o => o.SYS_FUNC).FirstOrDefault();

                if (SessionUtil.CurrentFunction != null)
                {

                    if (filterContext != null && !filterContext.HttpContext.Request.Form.AllKeys.Contains("Rpp"))
                        if (!string.IsNullOrWhiteSpace(SessionUtil.CurrentFunction.DESCRIPTION))
                        {
                            Dictionary<string, object> aaa = new Dictionary<string, object>();
                                foreach (string item in filterContext.HttpContext.Request.Form.AllKeys)
                            {
                                if (item == "__RequestVerificationToken") continue;
                                if (item =="pageCode") continue;
                                aaa.Add(item, filterContext.HttpContext.Request.Form[item]);
                            }
                           string abc = JsonConvert.SerializeObject(aaa, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                            string ll = filterContext.RouteData.Values["action"].ToString();
                            AuditLogService.Log("View " + SessionUtil.CurrentFunction.DESCRIPTION +(string.IsNullOrWhiteSpace(ll)?"":" - " + ll),string.IsNullOrWhiteSpace(abc)||aaa.Count==0?"":"Action Detail : " +abc);
                        }
                    
                    SYS_FUNC currentModule = null;
                    string findUuid = SessionUtil.CurrentFunction.UUID;
                    do
                    {
                        if (string.IsNullOrWhiteSpace(findUuid)) break;
                        SYS_FUNC currentPage = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.UUID == findUuid).Select(o => o.SYS_FUNC).FirstOrDefault();
                        if(currentPage == null)
                        {
                            break;
                        }
                        else if ("MODULE".Equals(currentPage.USE_TYPE))
                        {
                            currentModule = currentPage; break;
                        }
                        else
                        {
                            findUuid = currentPage.PARENT_ID;
                        }
                    }
                    while (true);
                    SessionUtil.CurrentModule = currentModule;

                    if (currentModule != null)
                    {
                        currentModule.Childs = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.PARENT_ID == findUuid).OrderBy(o => o.SYS_FUNC.SEQ).Select(o => o.SYS_FUNC).Distinct().ToList();
                        for (int i = 0; i < currentModule.Childs.Count; i++)
                        {
                            currentModule.Childs[i].Childs = SYS_ROLE_FUNCs.Where(o => o.SYS_FUNC.PARENT_ID == currentModule.Childs[i].CODE).OrderBy(o => o.SYS_FUNC.SEQ).Select(o => o.SYS_FUNC).Distinct().ToList();
                        }
                    }


                   




                }
            }
        }
        private void RedirectDefaultUrl(SYS_FUNC func, string defaultCode, bool setFirstItem)
        {
            SYS_ROLE_FUNC tmp;
            if (func == null) return;
            if (!string.IsNullOrWhiteSpace(defaultCode))
            {
                tmp = SessionUtil.AccessableFunctions.Where(o => o.SYS_FUNC.CODE == defaultCode).FirstOrDefault();
                if (tmp != null)
                {
                    func.URL = tmp.SYS_FUNC.URL;
                    func.CODE = tmp.SYS_FUNC.CODE;
                    return;
                }
            }
            if (setFirstItem)
            {
                List<SYS_FUNC> tmp2 = func.Childs;
                while (true)
                {
                    if (tmp2 == null) break;
                    if (tmp2.Count == 0) break;
                    if (tmp2[0].URL == null) tmp2 = tmp2[0].Childs;
                    else { func.URL = tmp2[0].URL; func.CODE = tmp2[0].CODE; break; }
                }
            }
        }
    }
    class AuthManager
    {
        private static readonly bool superAccess = false;
        private static Dictionary<string, SYS_POST> _loginPosts = new Dictionary<string, SYS_POST>();
        
        public const string HEADER_KEY_POST = "x-uid";
        public const string HEADER_KEY_DEPT = "x-dpdeptid";
        public const string HEADER_BD_DEPT = "BD";
        public static void Auth()
        {
            //MenuManager.AuditLogService.logDebug("Auth:");

            lock (_loginPosts)
            {
                if (SessionUtil.LoginPost != null)
                {
                    //MenuManager.AuditLogService.logDebug("SessionUtil.LoginPost: != null");
                    //MenuManager.AuditLogService.logDebug("SessionUtil.LoginPost:" + SessionUtil.LoginPost.BD_PORTAL_LOGIN);
                    return;
                }

                string portalId = DoHeaderLogin();
                if (string.IsNullOrWhiteSpace(portalId)) throw new Exception("Connnot find portal ID information from header.");
                SYS_POST post = DoDbLogin(portalId.ToUpper());


                if (post != null && !"Y".Equals(post.IS_ACTIVE)) throw new Exception("Inactive User.");
                SessionUtil.LoginPost = post;

                if (_loginPosts.ContainsKey(post.BD_PORTAL_LOGIN))
                {
                    _loginPosts.Remove(post.BD_PORTAL_LOGIN);
                    _loginPosts.Add(post.BD_PORTAL_LOGIN, post);
                }
                else
                {
                    _loginPosts.Add(post.BD_PORTAL_LOGIN, post);
                }
                AuditLogService.Log(AuditLogService.AUDIT_LOG_IN_DP, "");

            }


            AuditLogService.logDebug("Auth END");


        }
        private static string DoHeaderLogin()
        {
            string r = null;
            if (superAccess) r = "ZZZ";
            else{

                string[] headerPost = HttpContext.Current.Request.Headers.GetValues(HEADER_KEY_DEPT.ToUpper());
                if(headerPost == null)
                headerPost = HttpContext.Current.Request.Headers.GetValues(HEADER_KEY_DEPT.ToLower());

                String dept = null;
                if (headerPost != null && headerPost.Length > 0)
                {
                    dept = headerPost[0];
                }
                if (!HEADER_BD_DEPT.Equals(dept)) {
                    return null;
                }
                headerPost = HttpContext.Current.Request.Headers.GetValues(HEADER_KEY_POST);

                if (headerPost != null && headerPost.Length > 0)
                {
                    r = headerPost[0];
                }
            }

            AuditLogService.logDebug("DoHeaderLogin:"+ r+"-");
            return r;
        }
        private static SYS_POST DoDbLogin(string portalID)
        {
            portalID = portalID.ToUpper();
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_POST sysPost;
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    sysPost = db.SYS_POST
                        .Where(o => o.BD_PORTAL_LOGIN.ToUpper() ==  portalID)
                        .Include(o => o.SYS_POST_ROLE)
                       .FirstOrDefault();
                    if (sysPost == null)
                    {
                        sysPost = new SYS_POST() { BD_PORTAL_LOGIN = portalID, IS_ACTIVE = "Y" };
                        SYS_ROLE sysRole = db.SYS_ROLE.Where(o => o.CODE == "GUEST").FirstOrDefault();
                        SYS_POST_ROLE sysPostRole = new SYS_POST_ROLE();
                        sysPostRole.SYS_ROLE = sysRole;
                        sysPostRole.SYS_POST = sysPost;
                        //sysPost.SYS_POST_ROLE.Add(new SYS_POST_ROLE() { SYS_POST = sysPost, SYS_ROLE = db.SYS_ROLE.Where(o => o.CODE == "GUEST").FirstOrDefault() });

                        db.SYS_POST.Add(sysPost);
                        db.SYS_POST_ROLE.Add(sysPostRole);
                        // db.SYS_POST_ROLE.Add(new SYS_POST_ROLE() { SYS_POST = sysPost, SYS_ROLE = db.SYS_ROLE.Where(o => o.CODE == "GUEST").FirstOrDefault() });
                        db.SystemSave();
                        tran.Commit();
                    }
                    SessionUtil.LoginPost = sysPost;
                }
            }
            return SessionUtil.LoginPost;
        }
    public static SYS_POST getPOST(string portalID, string password)
    {
        using (EntitiesAuth db = new EntitiesAuth())
        {
            SYS_POST SYS_POST = db.SYS_POST
                .Where(o => o.BD_PORTAL_LOGIN == portalID && o.PW == password)
               .FirstOrDefault();
            SessionUtil.LoginPost = SYS_POST;
            AuditLogService.Log(AuditLogService.AUDIT_LOG_IN_LOGIN, "");
        }
        return SessionUtil.LoginPost;
    }
    
    }
}