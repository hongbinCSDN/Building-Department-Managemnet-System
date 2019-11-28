
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
    public class InterfaceFilter : ActionFilterAttribute
    {
        //ZZZ is admin
        private const string test_PortalID = "";

        public static string URL_AREA = "area";
        public static string URL_ACTION = "action";
        public static string URL_CONTROLLER = "controller";

       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {   
         
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
          
          
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
          
        }




        
    
    }
}