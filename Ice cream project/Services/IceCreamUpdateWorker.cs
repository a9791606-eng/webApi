using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using IceCreamNamespace.Hubs;
using IceCreamNamespace.Models;
using RabbitMQ.Client; 

namespace IceCreamNamespace.Services;

public class IceCreamUpdateWorker : BackgroundService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private IConnection? connection;
    
  
    private IChannel? channel; 

    public IceCreamUpdateWorker(IHubContext<ActivityHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
     
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
        
        var filePath = Path.Combine(dataDir, "rabbit_messages.jsonl");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (File.Exists(filePath))
            {
                try {
                    var lines = await File.ReadAllLinesAsync(filePath, stoppingToken);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        
                        var message = JsonSerializer.Deserialize<IceCreamUpdatedMessage>(line);
                        if (message != null)
                        {
                           
                            await hubContext.Clients.All.SendAsync(
                                "ReceiveActivity", 
                                message.Username, 
                                "updated", 
                                message.IceCreamName, 
                                stoppingToken
                            );
                        }
                    }
                 
                    await File.WriteAllTextAsync(filePath, string.Empty, stoppingToken);
                } catch {
               
                }
            }
            await Task.Delay(2000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
       
        if (channel != null) await channel.CloseAsync(cancellationToken: cancellationToken);
        if (connection != null) await connection.CloseAsync(cancellationToken: cancellationToken);
        
        await base.StopAsync(cancellationToken);
    }
}