using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAjax.Startup))]
namespace WebAjax
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
