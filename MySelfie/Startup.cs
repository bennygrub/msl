using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MySelfie.Startup))]
namespace MySelfie
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
