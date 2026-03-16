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
        private readonly int activeUserId;
        private readonly string activeUsername;
        private readonly bool isAdmin;

        public IceCreamService(IIceCreamRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            var user = activeUser?.ActiveUser;
            // allow unauthenticated construction; methods will check for user
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

        public IceCream Get(int id)
        {
            var IceCream = repository.Get(id);
            return IceCream?.UserId == activeUserId ? IceCream : null;
        }

        public void Add(IceCream IceCream)
        {
            IceCream.UserId = activeUserId;
            repository.Add(IceCream);
            BroadcastActivity("added", IceCream);
        }

        // Implement interface Create by delegating to Add
        public void Create(IceCream newIceCream) => Add(newIceCream);

        public void Delete(int id)
        {
            var IceCream = repository.Get(id);
            if (IceCream is null) return;
            var isOwner = IceCream.UserId == activeUserId;
            if (!isOwner && !isAdmin) return;
            repository.Delete(id);
            BroadcastActivity("deleted", IceCream);
        }

        public void Update(IceCream IceCream)
        {
            var existing = repository.Get(IceCream.Id);
            if (existing?.UserId != activeUserId)
                return;

            repository.Update(IceCream);
            BroadcastActivity("updated", IceCream);
        }

        private void BroadcastActivity(string action, IceCream IceCream)
        {
            // שלח הודעה רק למשתמש הנוכחי
            var connections = ActivityHub.GetConnectionsForUser(activeUserId.ToString());
            if (connections != null && connections.Count > 0)
                hubContext.Clients.Clients(connections).SendAsync("ReceiveActivity", activeUsername, action, IceCream.Name);
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
