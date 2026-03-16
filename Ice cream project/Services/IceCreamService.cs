using System;
using System.Collections.Generic;
using System.Linq;
using IceCreamNamespace.Hubs;
using IceCreamNamespace.Interfaces;
using IceCreamNamespace.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace IceCreamNamespace.Services;

public class IceCreamService : IICService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IIceCreamRepository repository;
    private readonly IActiveUser _activeUserProvider;
    private readonly int activeUserId;
    private readonly string activeUsername;
    private readonly bool isAdmin;

    public IceCreamService(IIceCreamRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext)
    {
        this.repository = repository;
        this.hubContext = hubContext;
        this._activeUserProvider = activeUser;
        
        var user = activeUser?.ActiveUser;
        
        this.activeUserId = user?.Id ?? 0;
        this.activeUsername = user?.Username ?? "anonymous";
        this.isAdmin = user?.IsAdmin ?? false;
    }

    public List<IceCream> GetAll()
    {
        var all = repository.GetAll();
        if (activeUserId == 0) return new List<IceCream>();
        if (isAdmin) return all;
        return all.Where(i => i.UserId == activeUserId).ToList();
    }

    public IceCream? Get(int id)
    {
        var iceCream = repository.Get(id);
        if (iceCream == null) return null;
        
      
        if (isAdmin || iceCream.UserId == activeUserId)
            return iceCream;
            
        return null;
    }

    public void Add(IceCream iceCream)
    {
        iceCream.UserId = activeUserId;
        repository.Add(iceCream);
        BroadcastActivity("added", iceCream);
    }

    public void Create(IceCream newIceCream) => Add(newIceCream);

    public void Delete(int id)
    {
        var iceCream = repository.Get(id);
        if (iceCream is null) return;
        
       
        if (!isAdmin && iceCream.UserId != activeUserId)
        {
            throw new UnauthorizedAccessException("אינך רשאי למחוק פריט זה");
        }
        
        repository.Delete(id);
        BroadcastActivity("deleted", iceCream);
    }

    public void Update(IceCream item)
    {

        var existing = repository.Get(item.Id);

        if (existing == null) throw new Exception("Not found");

       
        if (!isAdmin && existing.UserId != activeUserId)
        {
            throw new UnauthorizedAccessException("You can only update your own items!");
        }

        item.UserId = existing.UserId; 
        
        repository.Update(item);
        BroadcastActivity("updated", item);
    }

    private void BroadcastActivity(string action, IceCream iceCream)
    {
       
        var connections = ActivityHub.GetConnectionsForUser(activeUserId.ToString());
        
        if (connections != null && connections.Any())
        {
            var connectionList = connections.ToList();
            hubContext.Clients.Clients(connectionList).SendAsync("ReceiveActivity", activeUsername, action, iceCream.Name);
        }
    }

    public int Count => GetAll().Count;
}

public static partial class IceCreamExtensions
{
    public static IServiceCollection AddIceCream(this IServiceCollection services)
    {
        
        services.AddSingleton<IIceCreamRepository, IceCreamRepository>();
        services.AddScoped<IICService, IceCreamService>();
        return services;
    }
}