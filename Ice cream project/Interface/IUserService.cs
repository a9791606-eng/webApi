using System.Collections.Generic;
using IceCreamNamespace.Models;

namespace IceCreamNamespace.Interfaces;

public interface IUserService
{
    List<User> Get();
    User? Get(int id); // מאפשר Null
    User? Create(User newUser); // מאפשר Null
    int Update(int id, User newUser);
    bool Delete(int id);
    int Count { get; }
}