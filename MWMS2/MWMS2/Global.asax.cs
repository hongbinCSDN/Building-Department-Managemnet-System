using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Threading;
using MWMS2.Constant;
using MWMS2.Filter;
using System.Data.Entity.Infrastructure.Interception;

namespace MWMS2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //It's important to check whether session object is ready
            if (HttpContext.Current.Session != null)
            {

                if (HttpContext.Current.Request.Url.Segments.Length > 2 && HttpContext.Current.Request.Url.Segments[2].ToLower() == "infopage")
                {
                    HttpContext.Current.Response.Redirect("Interface/InfoPage/InfoPage" + HttpContext.Current.Request.Url.Query);
                    return;
                }
                CultureInfo ci = (CultureInfo)this.Session["Culturee"];
                //Checking first if there is no value in session 
                //and set default language 
                //this can happen for first user's request
                if (ci == null)
                {
                    //Sets default culture to english invariant
                    string langName = "en";

                    //Try to get values from Accept lang HTTP header
                    if (HttpContext.Current.Request.UserLanguages != null &&
                    HttpContext.Current.Request.UserLanguages.Length != 0)
                    {
                        //Gets accepted list 
                        langName = HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);
                    }
                    ci = new CultureInfo(langName);
                    this.Session["Culturee"] = ci;
                }

                // ci.DateTimeFormat.LongDatePattern = "dd/MM/yyyy";
                ci.DateTimeFormat.ShortDatePattern = ApplicationConstant.DISPLAY_DATE_FORMAT;
                ci.DateTimeFormat.ShortTimePattern = "";
                ci.DateTimeFormat.LongDatePattern = ApplicationConstant.DISPLAY_DATE_FORMAT;
                ci.DateTimeFormat.LongTimePattern = "";
               
                //  ci.DateTimeFormat.SetAllDateTimePatterns(new string[] { "dd/MM/yyyy" }, 'Y');
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = ci;




            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalFilters.Filters.Add(new InterfaceFilter());
            GlobalFilters.Filters.Add(new SystemFilter());
            GlobalFilters.Filters.Add(new LoggingFilter());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbInterception.Add(new SqlCommandInterceptor());


            //   log4net.GlobalContext.Properties["LogFileName"] = "C:\\MWMS_REVAMP\\Log\\"+"MWMS_REVAMP_"+ DateTime.Now.Date.ToString("ddMMyyyy") + ".txt";
            //  log4net.Config.XmlConfigurator.Configure();
        }

       protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {

            //Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            //Response.Cache.SetValidUntilExpires(false);
            //Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();


            //Response.Cache.SetNoStore();
            //Response.Cache.AppendCacheExtension("no-cache");
            //Response.Expires = 0;
            //   Response.Cache.SetCacheability(HttpCacheability.Private);  // HTTP 1.1.
            //Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            //Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            //Response.AppendHeader("Expires", "0"); // Proxies.
        }
    }
}
