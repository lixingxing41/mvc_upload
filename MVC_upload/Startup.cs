using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCBASE.Startup))]
namespace MVCBASE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
