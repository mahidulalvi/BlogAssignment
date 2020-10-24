using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PocketGraphs.Startup))]
namespace PocketGraphs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
