using System.Collections;
using System.Text;
using System.Text.Json;
using IceCreamProject.Hubs;
using IceCreamProject.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;


namespace IceCreamProject.Services
{
    public class IceCreamUpdateWorker : BackgroundService
    {
        private readonly IHubContext<ActivityHub> hubContext;
        private IConnection connection;
        private IModel channel;
        private const string QueueName = "ice-cream-updates";

        public IceCreamUpdateWorker(IHubContext<ActivityHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = (IConnection)factory.CreateConnectionAsync();
            channel =  connection.CreateModel();

             channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<IceCreamUpdatedMessage>(json);

                // HEAVY OPERATIONS HAPPEN HERE (not in HTTP request thread!)
                Thread.Sleep(5000);  // Simulate invoice generation, analytics, etc.
                //await Task.Delay(5000);

                // Broadcast to SignalR after heavy work completes
                await hubContext.Clients.All.SendAsync(
                    "ReceiveActivity",
                    message.Username,
                    "updated",
                    message.IceCreamName,
                    stoppingToken);

                // Acknowledge message
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

             channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,  // Manual acknowledgment for reliability
                consumer: consumer);

            // Keep the worker running
             Task.Delay(Timeout.Infinite, stoppingToken);
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await channel?.CloseAsync();
            await connection?.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }

    internal class AsyncEventingBasicConsumer
    {
        private IModel channel;

        public AsyncEventingBasicConsumer(IModel channel)
        {
            this.channel = channel;
        }

        public Func<object, object, Task> ReceivedAsync { get; internal set; }
    }

    internal interface IModel
    {
        Task BasicAckAsync(object deliveryTag, bool multiple);
        Task BasicConsumeAsync(string queue, bool autoAck, AsyncEventingBasicConsumer consumer);
        Task CloseAsync();
        Task QueueDeclareAsync(string queue, bool durable, bool exclusive, bool autoDelete, object arguments);
    }
}
