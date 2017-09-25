using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

[assembly: OwinStartup(typeof(RemoteEye.SignalrServer.Startup))]

namespace RemoteEye.SignalrServer
{
    public class Global : System.Web.HttpApplication
    {
        private  IDisposable webApp; // Declared in your class fields

        protected void Application_Start(object sender, EventArgs e)
        {
            string url = "http://192.168.31.73:5301/";
            //if (webApp == null)
            //{
                webApp = WebApp.Start(url);
            //}
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
        }
    }

    public class NotificationHub : Hub
    {
        public void JobStatusNotification(string camId, int interval)
        {
            Clients.All.GetJobStatus(camId, interval);
        }
    }
}