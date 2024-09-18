using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(e_commerce.Startup))]
namespace e_commerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
