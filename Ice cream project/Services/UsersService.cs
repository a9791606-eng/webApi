using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using IceCreamNamespace.Models;
using IceCreamNamespace.Interfaces;
using IceCreamNamespace.Hubs;


namespace IceCreamNamespace.Services;

    public  class UsersService : IUserService
    {
      
     private readonly IUserRepository userRepository;
     private readonly IActiveUser activeUser;
     private readonly IIceCreamRepository iceCreamRepository;
 
     public UsersService(IUserRepository userRepository, IActiveUser activeUser)
     {
         this.userRepository = userRepository;
         this.activeUser = activeUser;
     }

     public UsersService(IUserRepository userRepository, IActiveUser activeUser, IIceCreamRepository iceCreamRepository)
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

    public User Get(int id) => userRepository.Get(id);

    public User Create(User newUser)
    {
        var user = activeUser.ActiveUser;
        if (user == null || !user.IsAdmin)
            return null; // only admin can create users

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
        var u = userRepository.Get(id);
        if (u == null) return false;

        // delete user's items as well
        // Use IIceCreamRepository to remove items belonging to this user
        // ensure we have the repo via DI
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
    public static partial class UserExtension{
      public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UsersService>();
            return services;
        }



 }

