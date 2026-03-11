using System.Threading.Tasks;
using IceCreamNamespace.Models;
using System.Collections.Generic;


namespace IceCreamService.interfaces;


    public interface IICService
    {
     List<IceCream> Get();
     IceCream Get(int id);
     void Create(IceCream newIceCream);
     void Update(int id, IceCream newIceCream);
     bool Delete(int id);
     int Count { get; }
    }


