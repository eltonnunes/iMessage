using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Server.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(String nome, String message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(nome, message);
        }

        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();
            context.Clients.All.displayStatus();
        }
    }
}