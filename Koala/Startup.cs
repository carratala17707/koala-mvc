using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Koala.Startup))]
namespace Koala
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
