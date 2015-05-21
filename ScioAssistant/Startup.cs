using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ScioAssistant.Startup))]
namespace ScioAssistant
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
