using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Utility
{
    public class SessionUtil
    {
        private const string SESSION_KEY_LAST_ACTION = "LAST_ACTION";
        private const string SESSION_KEY_POSTCODE = "POSTCODE";
        private const string SESSION_KEY_ROLECODE = "ROLECODE";
        private const string SESSION_KEY_ICON_FUNCTIONS = "ICON_FUNCTIONS";
        private const string SESSION_KEY_ACCESSABLE_FUNCTIONS = "ACCESSABLE_FUNCTIONS";
        private const string SESSION_KEY_SEARCH_LEVELS = "SESSION_KEY_SEARCH_LEVELS";

        private const string SESSION_KEY_MODULE_FUNCTIONS = "MODULE_FUNCTIONS";
        private const string SESSION_KEY_CURRENT_FUNCTION = "CURRENT_FUNCTION";
        private const string SESSION_KEY_CURRENT_MODULE = "CURRENT_MODULE";
        private const string SESSION_KEY_DRAFT_OBJECT = "DRAFT_OBJECT";
        public static SYS_POST LoginPost
        {
            get { return HttpContext.Current.Session[SESSION_KEY_POSTCODE] as SYS_POST; }
            set { HttpContext.Current.Session[SESSION_KEY_POSTCODE] = value; }
        }
        public static List<SYS_POST_ROLE> LoginPostRoleList
        {
            get { return HttpContext.Current.Session[SESSION_KEY_ROLECODE] == null ? null : HttpContext.Current.Session[SESSION_KEY_ROLECODE] as List<SYS_POST_ROLE>; }
            set { HttpContext.Current.Session[SESSION_KEY_ROLECODE] = value; }
        }
        public static List<SYS_FUNC> IconFunctions
        {
            get { return HttpContext.Current.Session[SESSION_KEY_ICON_FUNCTIONS] == null ? null : HttpContext.Current.Session[SESSION_KEY_ICON_FUNCTIONS] as List<SYS_FUNC>; }
            set { HttpContext.Current.Session[SESSION_KEY_ICON_FUNCTIONS] = value; }
        }
        public static List<SYS_ROLE_FUNC> AccessableFunctions
        {
            get { return HttpContext.Current.Session[SESSION_KEY_ACCESSABLE_FUNCTIONS] == null ? null : HttpContext.Current.Session[SESSION_KEY_ACCESSABLE_FUNCTIONS] as List<SYS_ROLE_FUNC>; }
            set { HttpContext.Current.Session[SESSION_KEY_ACCESSABLE_FUNCTIONS] = value; }
        }

        public static List<C_S_SYSTEM_VALUE> SearchLevelRights
        {
            get { return HttpContext.Current.Session[SESSION_KEY_SEARCH_LEVELS] == null ? null : HttpContext.Current.Session[SESSION_KEY_SEARCH_LEVELS] as List<C_S_SYSTEM_VALUE>; }
            set { HttpContext.Current.Session[SESSION_KEY_SEARCH_LEVELS] = value; }
        }



        public static SYS_FUNC CurrentFunction
        {
            get { return HttpContext.Current.Session[SESSION_KEY_CURRENT_FUNCTION] == null ? new SYS_FUNC() : HttpContext.Current.Session[SESSION_KEY_CURRENT_FUNCTION] as SYS_FUNC; }
            set { HttpContext.Current.Session[SESSION_KEY_CURRENT_FUNCTION] = value; }
        }
        public static SYS_FUNC CurrentModule
        {
            get { return HttpContext.Current.Session[SESSION_KEY_CURRENT_MODULE] == null ? new SYS_FUNC() : HttpContext.Current.Session[SESSION_KEY_CURRENT_MODULE] as SYS_FUNC; }
            set { HttpContext.Current.Session[SESSION_KEY_CURRENT_MODULE] = value; }
        }


        public static List<SYS_FUNC> ModuleFunctions
        {
            get { return HttpContext.Current.Session[SESSION_KEY_MODULE_FUNCTIONS] == null ? null : HttpContext.Current.Session[SESSION_KEY_MODULE_FUNCTIONS] as List<SYS_FUNC>; }
            set { HttpContext.Current.Session[SESSION_KEY_MODULE_FUNCTIONS] = value; }
        }

        public static Dictionary<string, object> DraftObject
        {
            get {
                if (HttpContext.Current.Session[SESSION_KEY_DRAFT_OBJECT] == null)
                    HttpContext.Current.Session[SESSION_KEY_DRAFT_OBJECT] = new Dictionary<string, object>();
                return HttpContext.Current.Session[SESSION_KEY_DRAFT_OBJECT] as Dictionary<string, object>; }
            set { HttpContext.Current.Session[SESSION_KEY_DRAFT_OBJECT] = value; }
        }
        public static List<T> DraftList<T>(string draftKey)
        {
            if (!SessionUtil.DraftObject.ContainsKey(draftKey)) SessionUtil.DraftObject[draftKey] = new List<T>();
            return SessionUtil.DraftObject[draftKey] as List<T>;
        }
        public static string DRAFT_NEXT_ID
        {
            get{
                if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_TEMP_ID))
                    SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_TEMP_ID] = 1;
                int r = (int)SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_TEMP_ID];
                SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_TEMP_ID] = r + 1;
                return r.ToString();
            }
        }

        public static string LastAction
        {
            get
            {
                if (HttpContext.Current.Session[SESSION_KEY_LAST_ACTION] == null) return "";
                return HttpContext.Current.Session[SESSION_KEY_LAST_ACTION] as string;
            }
            set { HttpContext.Current.Session[SESSION_KEY_LAST_ACTION] = value; }
        }

        public static void Logout()
        {
            AuditLogService.Log(AuditLogService.AUDIT_LOG_OUT, "");

            //HttpContext.Current.Session[SESSION_KEY_POSTCODE] = null;

            HttpContext.Current.Session.RemoveAll();

                
            SessionUtil.LoginPost = null;
        //    get { return HttpContext.Current.Session[SESSION_KEY_POSTCODE] as string; }
        //   set { HttpContext.Current.Session[SESSION_KEY_POSTCODE] = value; }
        }

    }
}