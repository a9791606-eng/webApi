using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;
using System.IO;
using System;
using System.Net;
using System.Text.Json;

namespace IceCreamNamespace.Services;

    public  class IceCreamService : IICService
    {
      
     private List<IceCream> list;
     private string filePath;
    public IceCreamService()
    {
        this.list = new List<IceCream>(){
             new IceCream { Id = 1, Name = "vanilla",isGloutenFree=true},
             new IceCream { Id = 2, Name = "strawberry",isGloutenFree=true},
             new IceCream { Id = 3, Name = "chocolate",isGloutenFree=true},
             new IceCream { Id = 4, Name = "Pistachio",isGloutenFree=false} 
        };
        this.filePath = Path.Combine("Data", "IceCream.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                var content = jsonFile.ReadToEnd();
                list = JsonSerializer.Deserialize<List<IceCream>>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
    }
   
   private void saveToFile()
        {
            var text = JsonSerializer.Serialize(list);
            File.WriteAllText(filePath, text);
        }
        public List<IceCream> GetAll() => list;


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
         saveToFile();
            return newIceCream;
           
    }

    public int Update(int id, IceCream newIceCream)
    {
        var Ice = find(id);
        if(Ice == null)
          return 1;

        Ice.Name=newIceCream.Name;
        Ice.isGloutenFree=newIceCream.isGloutenFree;
        saveToFile();
        return 3;
        
    }

   
    public bool Delete(int id)
    {
         var Ice= find(id);
        if(Ice==null)
            return false;
        list.Remove(Ice);
         saveToFile();
        return true;
       
    }
}
    public static class IceCreamExtension{
      public static void AddIceCreamService(this IServiceCollection services)
        {
            services.AddSingleton<IICService, IceCreamService>();
            //services.AddScope<IOrderManager, OrderManager>();
            //services.AddTransient<IOrderSender, OrderSenderHttp>();            
        }




}

