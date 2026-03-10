using IceCreamNamespace.Models;
using Microsoft.AspNetCore.Http;


namespace IceCreamNamespace.Interfaces
{
    public interface IActiveUser
    {
        User ActiveUser { get; }
    }
}