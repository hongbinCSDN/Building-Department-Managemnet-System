
using MWMS2.Entity;
using MWMS2.Services;
using MWMS2.Utility;
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
    public class LoggingFilter : ActionFilterAttribute
    {
        //ZZZ is admin
        private const string test_PortalID = "";

        public static string URL_AREA = "area";
        public static string URL_ACTION = "action";
        public static string URL_CONTROLLER = "controller";


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try { 
                base.OnActionExecuting(filterContext);
            }catch(Exception e)
            {
                AuditLogService.logDebug("Error OnActionExecuting" + e);
                AuditLogService.logDebug(e.Message);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            try
            {
                base.OnResultExecuting(filterContext);
            }
            catch (Exception e)
            {
                AuditLogService.logDebug("Error OnResultExecuting" + e);
                AuditLogService.logDebug(e.Message);
            }

        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                base.OnActionExecuted(filterContext);
            }
            catch (Exception e)
            {
                AuditLogService.logDebug("Error OnActionExecuted" + e);
                AuditLogService.logDebug(e.Message);
            }


        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
             try
            {
                base.OnResultExecuted(filterContext);
            }
            catch (Exception e)
            {
                AuditLogService.logDebug("Error OnResultExecuted" + e);
                AuditLogService.logDebug(e.Message);
            }

        }






    }
}