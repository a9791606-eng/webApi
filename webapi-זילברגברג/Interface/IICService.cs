using System.Threading.Tasks;
using angular-אנגולר.Models;


namespace angular-אנגולר.interface IICService
{
    public interface IICService
    {
        Task<string> Transmit(Order order);
    }
};

   