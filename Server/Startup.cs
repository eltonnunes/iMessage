using Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var config = new HubConfiguration(); 
            config.EnableJSONP = true;

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR(config);
        }
    }
}