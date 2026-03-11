using System.Collections.Generic;
using IceCreamNamespace.Models;
using IceCreamProject.Models;

namespace IceCreamProject.Interfaces
{
    public interface IIceCreamRepository
    {
        List<IceCream> GetAll();
        IceCream Get(int id);
        void Add(IceCream iceCream);
        void Delete(int id);
        void Update(IceCream iceCream);
        int Count { get; }
    }
}
