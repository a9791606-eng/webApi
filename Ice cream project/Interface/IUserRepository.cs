using System.Collections.Generic;
using IceCreamNamespace.Models;

namespace IceCreamNamespace.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Get(int id);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        int Count { get; }
    }
}