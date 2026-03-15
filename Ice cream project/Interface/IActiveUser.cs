using IceCreamNamespace.Models;

namespace IceCreamNamespace.Interfaces
{
    public interface IActiveUser
    {
        User? ActiveUser { get; }
    }
}