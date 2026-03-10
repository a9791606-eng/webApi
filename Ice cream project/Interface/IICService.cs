using System.Threading.Tasks;
using IceCreamNamespace.Models;


namespace IceCreamService.interfaces;


    public interface IICService
    {
     List<IceCream> Get();
   
     IceCream Get(int id);
     IceCream Create(IceCream newIceCream);

     int Update(int id, IceCream newIceCream);
   
     bool Delete(int id);
    }


