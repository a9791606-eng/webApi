using IceCreamService.Interfaces;
using IceCreamNamespace.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace IceCreamNamespace.Services
{
    public class ActiveUserService : IActiveUser
    {
        public User ActiveUser { get; private set; }
        public ActiveUserService(IHttpContextAccessor context)
        {
            var user = context?.HttpContext?.User;
            if (user == null) return;

            var idClaim = user.FindFirst("id") ?? user.FindFirst("Id");
            var nameClaim = user.FindFirst("username") ?? user.FindFirst("name");
            var typeClaim = user.FindFirst("type");

            if (idClaim != null && int.TryParse(idClaim.Value, out var id))
            {
                ActiveUser = new User
                {
                    Id = id,
                    Username = nameClaim?.Value ?? string.Empty,
                    IsAdmin = string.Equals(typeClaim?.Value, "Admin", System.StringComparison.OrdinalIgnoreCase)
                };
            }
        }

    }

    public static partial class IceCreamExtensions
    {
        public static IServiceCollection AddActiveUser(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IActiveUser, ActiveUserService>();
            return services;
        }
    }
}