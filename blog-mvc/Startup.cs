using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(blog_mvc.Startup))]
namespace blog_mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
