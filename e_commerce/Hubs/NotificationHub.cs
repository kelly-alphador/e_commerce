using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Hubs
{
    public class NotificationHub : Hub
    {
        public static void SendNotification(string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.broadcastNotification(message);
        }
    }
}