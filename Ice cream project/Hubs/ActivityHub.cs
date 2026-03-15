using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.Linq;


namespace IceCreamProject.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    // mapping from user id to list of connection ids
    private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> _userConnections = new();

    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var bag = _userConnections.GetOrAdd(userId, _ => new ConcurrentBag<string>());
            bag.Add(Context.ConnectionId);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(System.Exception? exception)
    {
        var userId = Context.User?.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(userId) && _userConnections.TryGetValue(userId, out var bag))
        {
            // remove connection id from bag
            var remaining = new ConcurrentBag<string>(bag.Where(cid => cid != Context.ConnectionId));
            _userConnections[userId] = remaining;
        }
        return base.OnDisconnectedAsync(exception);
    }

    public Task BroadcastToUser(string userId, string username, string action, string iceCreamName)
    {
        if (string.IsNullOrEmpty(userId)) return Task.CompletedTask;
        if (!_userConnections.TryGetValue(userId, out var bag)) return Task.CompletedTask;
        var connectionIds = bag.ToArray();
        return Clients.Clients(connectionIds).SendAsync("ReceiveActivity", username, action, iceCreamName);
    }

    // helper for other services to access connection ids (optional)
    public static string[] GetConnectionsForUser(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return new string[0];
        return _userConnections.TryGetValue(userId, out var bag) ? bag.ToArray() : new string[0];
    }
}
