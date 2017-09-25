using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
[assembly: OwinStartup(typeof(RemoteEye.SignalrHost.Startup))]

namespace RemoteEye.SignalrHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://192.168.31.73:5555/";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
            app.UseCors(CorsOptions.AllowAll);
        }
    }
    //[HubName("MyHub")]

    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }
}