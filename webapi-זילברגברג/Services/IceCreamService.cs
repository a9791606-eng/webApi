using System.Threading.Tasks;
using angular_אנגולר.Models;
using angular_אנגולר.Intrface;
using System;
using System.Net.HTTP;
using System.Text;
using System.Text.Json;


namespace angular_אנגולר.Services;

    public  class IceCreamService : Intrface
    {
       IICService Services;
        public IceCreamService(IICService Services)
        {
           this.Services=services;
        }
        public async Task<string> Transmit(Order order)
        {
            //var orderSender = new OrderSenderHttp();

            return await Services.Send(order);
        }
        public static class ServicesExtension{
            public static void AddServices(this IServiceCollection services)
            {
                services.AddSingleton<IICService,ServicesHttp>();        
            }
        }
    }
        
    
    public class ServicesHttp
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> Send(Order order)
        {
            var jsonOrder = JsonSerializer.Serialize<Order>(order);
            var stringContent = new StringContent(jsonOrder, UnicodeEncoding.UTF8, "application/json");

            // This statement calls a not existing URL. This is just an example...
            var response = await httpClient.PostAsync("https://mymicroservice/myendpoint", stringContent);

            return response.Content.ReadAsStringAsync().Result;

        }
    }

