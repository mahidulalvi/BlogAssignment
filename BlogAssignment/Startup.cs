using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogAssignment.Startup))]
namespace BlogAssignment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
