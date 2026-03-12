using KsPizza.Models;
using Microsoft.AspNetCore.Http;


namespace IceCreamService.Interfaces
{
    public interface IActiveUser
    {
        User ActiveUser { get; }
    }
}