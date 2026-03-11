
using IceCreamNamespace.Models;
using IceCreamService.interfaces;
using IceCreamProject.Interfaces;
using Microsoft.AspNetCore.SignalR;
using IceCreamProject.Hubs;
using IceCreamProject.Interfaces;
using IceCreamProject.Services;

namespace IceCreamNamespace.Services;

    public  class IceCreamService : IICService
    {
      
     private List<IceCream> list;

    public IceCreamService()
    {
        this.list = new List<IceCream>{
             new IceCream { Id = 1, Name = "vanilla",isGloutenFree=true},
             new IceCream { Id = 2, Name = "strawberry",isGloutenFree=true},
             new IceCream { Id = 3, Name = "chocolate",isGloutenFree=true},
             new IceCream { Id = 4, Name = "Pistachio",isGloutenFree=false} 
        };
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
            this.activeUserId = user.Id;
            this.activeUsername = user.Username;
        }
   

    public List<IceCream> Get()
    {
        return list;
    }

    private IceCream find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public IceCream Get(int id) => find(id);

    public IceCream Create(IceCream newIceCream)
    {
        var maxId = list.Max(p => p.Id);
        newIceCream.Id = maxId + 1;
        list.Add(newIceCream);
            return newIceCream;
    }

    public int Update(int id, IceCream newIceCream)
    {
        var Ice = find(id);
        if(Ice == null)
          return 1;

        if(Ice.Id != newIceCream.Id)
           return 2;

        var index = list.IndexOf(Ice);
        list[index] = newIceCream;

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

public interface IActiveUser
{
    object ActiveUser { get; }
}

public static class IceCreamExtension{
      public static void AddIceCreamService(this IServiceCollection services)
        {
            services.AddSingleton<IICService, IceCreamService>();
            //services.AddScope<IOrderManager, OrderManager>();
            //services.AddTransient<IOrderSender, OrderSenderHttp>();            
        }




}

