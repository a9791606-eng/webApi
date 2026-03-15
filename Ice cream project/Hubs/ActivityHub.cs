using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace IceCreamNamespace.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    public async Task BroadcastActivity(string username, string action, string iceCreamName)
    {
        await Clients.All.SendAsync("ReceiveActivity", username, action, iceCreamName);
    }
}
