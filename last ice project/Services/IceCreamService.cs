using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;

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



    public List<IceCream> GetAll()
    {
        return list;
    }

    private IceCream find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public IceCream GetAll(int id) => find(id);

    public void Add(IceCream newIceCream)
    {
        var maxId = list.Max(p => p.Id);
        newIceCream.Id = maxId + 1;
        list.Add(newIceCream);
          //  return newIceCream;
    }

    public void Update(int id, IceCream newIceCream)
    {
        var Ice = find(id);
        if(Ice == null)
       // return 1;

        if(Ice.Id != newIceCream.Id)
       // return 2;

        var index = list.IndexOf(Ice);
        list[index] = newIceCream;

       // return 3;
    }


    public void Delete(int id)
    {
        var Ice= find(id);
        if(Ice!=null)
            list.Remove(Ice);
             //return false;
        //return true;
    }

    public int Count => list.Count;
}
public static class IceCreamExtension{
    public static void AddIceCreamService(this IServiceCollection services)
    {
        services.AddSingleton<IICService, IceCreamService>();
        //services.AddScope<IOrderManager, OrderManager>();
        //services.AddTransient<IOrderSender, OrderSenderHttp>();            
    }

}

