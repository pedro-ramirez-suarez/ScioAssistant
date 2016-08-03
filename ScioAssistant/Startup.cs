using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeepThought.Startup))]
namespace DeepThought
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
