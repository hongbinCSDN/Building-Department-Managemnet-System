using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using System.Data.Entity;
using MWMS2.Controllers;
using MWMS2.Constant;

namespace MWMS2.Services
{
    public class AuditLogService
    {
        public static log4net.ILog LogFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string AUDIT_LOG_IN_DP = "Portal Login";
        public static string AUDIT_LOG_IN_LOGIN = "Password Login";
        //public static string AUDIT_LOG_IN_DP = "Login-dp";
        //public static string AUDIT_LOG_IN_LOGIN = "Login-login";
        public static string AUDIT_LOG_OUT = "Logout";

        public static void logDebug(object e)
        {   AuditLogService.LogFile.Debug(e);
        }
        public static void logError(object e)
        {
            AuditLogService.LogFile.Error(e);
        }
        public static String GetIP()
        {
            String ip =
                HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

     


        public static void Log(string action, string description)
        {
            saveAuditLog(action, description, GetIP());

        }
        private static void saveAuditLog(string action, string description, string ipAddress)
        {
            try { 
                SYS_LOG log = new SYS_LOG();
                log.ACTION = action;
                log.DESCRIPTION = description;
                log.IPADDRESS = ipAddress;
                log.LOG_DATE = DateTime.Now;



                log.LOGIN = SessionUtil.LoginPost == null ? null :

                    ( String.IsNullOrEmpty(SessionUtil.LoginPost.CODE) ? SessionUtil.LoginPost.BD_PORTAL_LOGIN : 
                    SessionUtil.LoginPost.CODE) +" ("+ SessionUtil.LoginPost.BD_PORTAL_LOGIN + ")" ;

                using (EntitiesAuth db = new EntitiesAuth())
                {
                    db.SYS_LOG.Add(log);
                    db.SaveChanges();
                }
            }catch(Exception e)
            {
                AuditLogService.logDebug(e);
            }

        }


    }
}