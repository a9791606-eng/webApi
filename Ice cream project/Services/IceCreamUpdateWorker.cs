using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IceCreamNamespace.Hubs;
using IceCreamNamespace.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IceCreamNamespace.Services
{
    public class IceCreamUpdateWorker : BackgroundService
    {
        private readonly IHubContext<ActivityHub> hubContext;
        private dynamic connection;
        private dynamic channel;
        private const string QueueName = "ice-cream-updates";

        public IceCreamUpdateWorker(IHubContext<ActivityHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var dataDir = Path.Combine(AppContext.BaseDirectory, "..", "Data");
            var filePath = Path.Combine(dataDir, "rabbit_messages.jsonl");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        try
                        {
                            var message = JsonSerializer.Deserialize<IceCreamUpdatedMessage>(line);
                            await Task.Delay(1000, stoppingToken);
                            await hubContext.Clients.All.SendAsync("ReceiveActivity", message.Username, "updated", message.IceCreamName, stoppingToken);
                        }
                        catch { }
                    }

                    // truncate file after processing
                    File.WriteAllText(filePath, string.Empty);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            try { channel?.Close(); } catch { }
            try { connection?.Close(); } catch { }
            return base.StopAsync(cancellationToken);
        }
    }
}
