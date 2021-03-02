using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(xggameplan.Hubs.Startup))]
namespace xggameplan.Hubs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration();

                try
                {
                    map.RunSignalR(hubConfiguration);
                }
                catch
                {
                    // Sometimes fails to add a performance counter to Windows and throws an exception.
                    // Nothing we can log so just carry on.
                }
            });
        }
    }
}
