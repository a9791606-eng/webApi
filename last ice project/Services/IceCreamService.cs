using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using IceCreamNamespace.Models;
using IceCreamService.interfaces;

namespace IceCreamNamespace.Services;

public class IceCreamService : IICService
{
    private readonly List<IceCream> list;

    public IceCreamService()
    {
        list = new List<IceCream>
        {
            new IceCream { Id = 1, Name = "vanilla", IsGlutenFree = true },
            new IceCream { Id = 2, Name = "strawberry", IsGlutenFree = true },
            new IceCream { Id = 3, Name = "chocolate", IsGlutenFree = true },
            new IceCream { Id = 4, Name = "Pistachio", IsGlutenFree = false }
        };
    }

    public List<IceCream> GetAll()
        => list.ToList();

    public IceCream Get(int id)
        => list.FirstOrDefault(p => p.Id == id);

    public void Add(IceCream newIceCream)
    {
        var maxId = list.Any() ? list.Max(p => p.Id) : 0;
        newIceCream.Id = maxId + 1;
        list.Add(newIceCream);
    }

    public void Update(int id, IceCream newIceCream)
    {
        var index = list.FindIndex(p => p.Id == id);
        if (index == -1) return;
        newIceCream.Id = id;
        list[index] = newIceCream;
    }

    public void Update(IceCream newIceCream)
    {
        var id = newIceCream?.Id ?? 0;
        var index = list.FindIndex(p => p.Id == id);
        if (index == -1) return;
        // Ensure the id remains the same
        newIceCream.Id = id;
        list[index] = newIceCream;
    }

    public void Delete(int id)
    {
        var item = Get(id);
        if (item != null) list.Remove(item);
    }

    public int Count => list.Count;
}

public static class IceCreamExtension
{
    public static void AddIceCreamService(this IServiceCollection services)
    {
        services.AddSingleton<IICService, IceCreamService>();
    }

    public int Count => list.Count;
}



