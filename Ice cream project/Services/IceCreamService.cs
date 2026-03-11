using System;
using System.Collections.Generic;
using System.Linq;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;
using IceCreamProject.Interfaces;
using Microsoft.AspNetCore.SignalR;
using IceCreamProject.Hubs;
using IceCreamProject.Interfaces;
using IceCreamProject.Models;
using IceCreamProject.Services;





namespace IceCreamNamespace.Services;

public class IceCreamService : IICService
{
    private readonly IIceCreamRepository repository;
    private readonly IActiveUser activeUser;

    public IceCreamService(IIceCreamRepository repository, IActiveUser activeUser)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.activeUser = activeUser ?? throw new ArgumentNullException(nameof(activeUser));
    }
   private readonly IHubContext<ActivityHub> hubContext;
        private readonly IIceCreamRepository repository;
        private readonly IRabbitMqService rabbitMqService;
        private readonly int activeUserId;
        private readonly string activeUsername;

        public IceCreamService(IIceCreamRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext, IRabbitMqService rabbitMqService)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            this.rabbitMqService = rabbitMqService;
            var user = activeUser.ActiveUser;
            if (user is null)
                throw new System.InvalidOperationException("Active user is required");
          //  this.activeUserId = user.Id;
         //   this.activeUsername = user.Username;
        }
   

    public List<IceCream> Get()
    {
        var user = activeUser.User;
        if (user == null) return new List<IceCream>();
        if (user.IsAdmin) return repository.GetAll();
        return repository.GetAll().Where(i => i.UserId == user.Id).ToList();
    }

    public IceCream Get(int id) => repository.Get(id);

    public void Create(IceCream newIceCream)
    {
        if (newIceCream == null) throw new ArgumentNullException(nameof(newIceCream));
        var user = activeUser.User ?? throw new InvalidOperationException("Active user is required");

        if (!user.IsAdmin)
            newIceCream.UserId = user.Id;
        else if (newIceCream.UserId == 0)
            newIceCream.UserId = user.Id;

        repository.Add(newIceCream);

        // Notify via SignalR (if repository or hub context is available)
        // TODO: publish message to logging queue
    }

    public void Update(int id, IceCream newIceCream)
    {
        var existing = repository.Get(id);
        if (existing is null) throw new KeyNotFoundException("Item not found");

        var user = activeUser.User;
        if (user == null) throw new InvalidOperationException("No active user");

        if (!user.IsAdmin && existing.UserId != user.Id) throw new UnauthorizedAccessException("Forbidden");

        if (!user.IsAdmin) newIceCream.UserId = user.Id;

        if (newIceCream.Id != id) throw new ArgumentException("Id mismatch");

        repository.Update(newIceCream);

        // SignalR / logging notifications will be implemented by injecting IHubContext and a logging queue elsewhere.
    }

    public bool Delete(int id)
    {
        var existing = repository.Get(id);
        if (existing is null) return false;

        var user = activeUser.User;
        if (user == null) return false;

        if (!user.IsAdmin && existing.UserId != user.Id) return false;

        repository.Delete(id);
        return true;
    }

    public int Count => repository.Count;
}

// Keep IActiveUser and ActiveUser in separate file if already present; leave interface here if not
public interface IActiveUser
{
    User User { get; }
}

public class ActiveUser : IActiveUser
{
    private readonly IHttpContextAccessor accessor;

    public ActiveUser(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    public User User
    {
        get
        {
            var ctx = accessor.HttpContext;
            if (ctx == null) return null;
            var user = ctx.User;
            if (user?.Identity?.IsAuthenticated != true) return null;
            var idClaim = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var name = user.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var isAdminClaim = user.Claims.FirstOrDefault(c => c.Type == "isAdmin")?.Value;
            if (!int.TryParse(idClaim, out var id)) return null;
            var isAdmin = false;
            if (!string.IsNullOrEmpty(isAdminClaim)) bool.TryParse(isAdminClaim, out isAdmin);
            return new User { Id = id, Username = name ?? string.Empty, IsAdmin = isAdmin };
        }
    }
}

public static class IceCreamExtension
{
    public static void AddIceCreamService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IActiveUser, ActiveUser>();
        services.AddScoped<IIceCreamRepository, IceCreamRepository>();
        services.AddScoped<IICService, IceCreamService>();
    }
}

