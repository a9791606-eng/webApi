using IceCreamNamespace.Models;
using System.Collections.Generic;

namespace IceCreamService.Interfaces
{
    public interface IICService
    {
     List<IceCream> GetAll();
     IceCream Get(int id);
     void Create(IceCream newIceCream);
     void Delete(int id);
     void Update(IceCream newIceCream);
     int Count { get; }
    }
}

