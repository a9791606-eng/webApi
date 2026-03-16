using System.Collections.Generic;
using System.Linq;
using IceCreamNamespace.Models;
using IceCreamNamespace.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IceCreamNamespace.Services;

public class UsersService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IActiveUser activeUser;
    private readonly IIceCreamRepository? iceCreamRepository;

    public UsersService(IUserRepository userRepository, IActiveUser activeUser, IIceCreamRepository? iceCreamRepository = null)
    {
        this.userRepository = userRepository;
        this.activeUser = activeUser;
        this.iceCreamRepository = iceCreamRepository;
    }

    public List<User> Get()
    {
        var user = activeUser.ActiveUser;
        if (user != null && user.IsAdmin)
            return userRepository.GetAll();

        if (user == null)
            return new List<User>();

        var me = userRepository.Get(user.Id);
        return me == null ? new List<User>() : new List<User> { me };
    }

    public User? Get(int id) => userRepository.Get(id);

    public User? Create(User newUser)
    {
        var user = activeUser.ActiveUser;
        if (user == null || !user.IsAdmin)
            return null;

        userRepository.Add(newUser);
        return newUser;
    }

    public int Update(int id, User newUser)
    {
        var existing = userRepository.Get(id);
        if (existing == null) return 1;
        if (existing.Id != newUser.Id) return 2;

        var current = activeUser.ActiveUser;
        if (current == null) return 4;
        if (!current.IsAdmin && current.Id != id) return 4;
        if (!current.IsAdmin && newUser.IsAdmin) newUser.IsAdmin = false;

        userRepository.Update(newUser);
        return 3;
    }

    public bool Delete(int id)
    {
        var current = activeUser.ActiveUser;
        if (current == null || !current.IsAdmin) return false;
        
        if (iceCreamRepository != null)
        {
            var items = iceCreamRepository.GetAll().Where(i => i.UserId == id).ToList();
            foreach (var it in items) iceCreamRepository.Delete(it.Id);
        }
        
        userRepository.Delete(id);
        return true;
    }

    public int Count => userRepository.Count;
}

public static class UserServiceExtensions
{
    public static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UsersService>();
        return services;
    }
}