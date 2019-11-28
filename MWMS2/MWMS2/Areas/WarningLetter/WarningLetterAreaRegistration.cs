using System.Web.Mvc;

namespace MWMS2.Areas.WarningLetter
{
    public class WarningLetterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WarningLetter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WarningLetter_default",
                "WarningLetter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}