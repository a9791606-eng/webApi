using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;
using IceCreamProject.Hubs;
using IceCreamProject.Interfaces;


namespace IceCreamNamespace.Services;

    public  class UsersService : IUserService
    {
      
     private readonly IUserRepository userRepository;
     private readonly IActiveUser activeUser;

    public UsersService(IUserRepository userRepository, IActiveUser activeUser)
    {
        this.userRepository = userRepository;
        this.activeUser = activeUser;
    }

    public List<User> Get()
    {
        var user = activeUser.User;
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
        var user = activeUser.User;
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

        var current = activeUser.User;
        if (current == null) return 4;

        if (!current.IsAdmin && current.Id != id) return 4;

        if (!current.IsAdmin && newUser.IsAdmin) newUser.IsAdmin = false;

        userRepository.Update(newUser);
        return 3;
    }

    public bool Delete(int id)
    {
        var current = activeUser.User;
        if (current == null || !current.IsAdmin) return false;
        var u = userRepository.Get(id);
        if (u == null) return false;

        // delete user's items as well
        // Use IIceCreamRepository to remove items belonging to this user
        userRepository.Delete(id);
        return true;
    }

    public int Count => userRepository.Count;
}
    public static class UserExtension{
      public static void AddUserService(this IServiceCollection services)
        {
            // Register HttpContextAccessor and ActiveUser so services can check ownership/roles
            services.AddHttpContextAccessor();
            services.AddScoped<IActiveUser, ActiveUser>();

            services.AddScoped<IUserService, UsersService>();          
        }



}

