using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebApiCacheEx1.Hubs
{
    public class CacheNotificationHub : Hub
    {
        public void SendCacheNotifyMessage(string pushNotificationMessage)
        {
            Clients.All.cacheNotification(Context.ConnectionId, pushNotificationMessage);
        }
    }
}