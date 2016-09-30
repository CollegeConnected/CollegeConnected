using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CollegeConnected.Startup))]
namespace CollegeConnected
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
