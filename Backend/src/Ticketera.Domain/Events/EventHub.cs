using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace Ticketera.Events
{
    public class EventHub: AbpHub
    {
        public override Task OnConnectedAsync()
        {
            var conId = this.Context.ConnectionId;

            if (conId != null)
            {
                ConnectionClient.Connections.Add(conId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var conId = this.Context.ConnectionId;

            if (conId != null)
            {
                ConnectionClient.Connections.Remove(conId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
