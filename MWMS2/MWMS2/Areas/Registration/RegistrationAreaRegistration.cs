using System.Web.Mvc;

namespace MWMS2.Areas.Registration
{
    public class RegistrationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Registration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Registration_default",
                "Registration/{controller}/{action}/{id}",
                new { action = "{controller}", id = UrlParameter.Optional }
            );
        }
    }
}