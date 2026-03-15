using System.Threading.Tasks;
using IceCreamNamespace.Models;
using System.Collections.Generic;


namespace IceCreamNamespace.Interfaces
{
    public interface IUserService
    {
     List<User> Get();
     User Get(int id);
     User Create(User newUser);
     int Update(int id, User newUser);
     bool Delete(int id);
     int Count { get; }
    }
}

