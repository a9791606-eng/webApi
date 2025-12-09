using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;


namespace IceCreamNamespace.Services;

    public  class UsersService : IUserService
    {
      
     private List<User> list;

    public UsersService()
    {
        this.list = new List<User>{
             new User { Id = 1, FirstName = "Dan",LastName="aaa"},
             new User { Id = 2, FirstName = "Avi",LastName="bbb"},
             new User { Id = 3, FirstName = "Yehuda",LastName="ccc"},
             new User { Id = 4, FirstName = "Roni",LastName="ddd"} 
        };
    }
   
   

    public List<User> Get()
    {
        return list;
    }

    private User find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public User Get(int id) => find(id);

    public User Create(User newUser)
    {
        var maxId = list.Max(p => p.Id);
        newUser.Id = maxId + 1;
        list.Add(newUser);
            return newUser;
    }

    public int Update(int id, User newUser)
    {
        var Ice = find(id);
        if(Ice == null)
          return 1;

        if(Ice.Id != newUser.Id)
           return 2;

        var index = list.IndexOf(Ice);
        list[index] = newUser;

        return 3;
    }

   
    public bool Delete(int id)
    {
         var Ice= find(id);
        if(Ice==null)
            return false;
        list.Remove(Ice);
        return true;
    }
}
    public static class UserExtension{
      public static void AddUserService(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UsersService>();          
        }

//  public static class IceCreamExtension{
//       public static void AddIceCreamService(this IServiceCollection services)
//         {
//             services.AddSingleton<IICService, IceCreamService>();
//             //services.AddScope<IOrderManager, OrderManager>();
//             //services.AddTransient<IOrderSender, OrderSenderHttp>();            
//         }


}

