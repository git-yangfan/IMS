using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IMS.Web.Startup))]
namespace IMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
