using System.Threading.Tasks;
using IceCreamNamespace.Models;
using System.Collections.Generic;


namespace IceCreamService.interfaces;


    public interface IUserService
    {
     List<IceCream> GetAll();

     IceCream Get(int id);
     void Add(IceCream newIceCream);

     void Update(int id, IceCream newIceCream);

     void Delete(int id);
     int Count {get;}
    }


