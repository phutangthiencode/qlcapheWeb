using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(qlCaPhe.Startup))]
namespace qlCaPhe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
