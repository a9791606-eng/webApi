using System.Threading.Tasks;
using IceCreamNamespace.Models;


namespace IceCreamService.interfaces;


    public interface IUserService
    {
     List<User> Get();
   
     User Get(int id);
     User Create(User newUser);

     int Update(int id, User newUser);
   
     bool Delete(int id);
    }


