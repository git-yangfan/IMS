using System.Web.Mvc;

namespace IMS.Web.Areas.Repair
{
    public class RepairAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Repair";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Repair_default",
                "Repair/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}