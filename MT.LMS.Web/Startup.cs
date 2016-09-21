using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MT.LMS.Web.Startup))]
namespace MT.LMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
