using System.Collections.Generic;
using System.Linq;
using IceCreamProject.Hubs;
using IceCreamService.interfaces;
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

        public IceCreamService(IIceCreamRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            var user = activeUser.ActiveUser;
            if (user is null)
                throw new System.InvalidOperationException("Active user is required");
            this.activeUserId = user.Id;
            this.activeUsername = user.Username;
        }

        public List<IceCream> GetAll()
            => repository
                .GetAll()
                .Where(i => i.UserId == activeUserId)
                .ToList();

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

        public void Delete(int id)
        {
            var IceCream = Get(id);
            if (IceCream is null)
                return;

            if (IceCream.UserId != activeUserId)
                return;

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
            hubContext.Clients.All.SendAsync("ReceiveActivity", activeUsername, action, IceCream.Name);
        }

        public int Count => GetAll().Count;
    }

    public static partial class IceCreamExtensions
    {
        public static IServiceCollection AddIceCream(this IServiceCollection services)
        {
            services.AddSingleton<IIceCreamRepository, IceCreamRepository>();
            services.AddScoped<IIceCreamService, IceCreamService>();
            return services;
        }
    }
