using System;
using System.Text.Json;
using System.Threading.Tasks;
using IceCreamProject.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace IceCreamProject.Services
{
    public interface IRabbitMqService
    {
        Task PublishIceCreamUpdated(IceCreamUpdatedMessage message);
        Task PublishLogAsync(object message);
    }

    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private readonly string filePath;

        public RabbitMqService(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            filePath = Path.Combine(dataDir, "rabbit_messages.jsonl");
        }

        public async Task PublishIceCreamUpdated(IceCreamUpdatedMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            await File.AppendAllTextAsync(filePath, json + Environment.NewLine);
        }

        public async Task PublishLogAsync(object message)
        {
            var json = JsonSerializer.Serialize(message);
            await File.AppendAllTextAsync(filePath, json + Environment.NewLine);
        }

        public void Dispose()
        {
            // nothing to dispose for file-based impl
        }
    }

    public static partial class UserExtension
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddHostedService<IceCreamUpdateWorker>();
            return services;
        }
    }
}
