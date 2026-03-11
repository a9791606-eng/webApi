using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using IceCreamProject.Models;
using System;

namespace IceCreamProject.Services
{
    public class LoggingWorker : BackgroundService
    {
        private readonly LoggingQueue _queue;
        private readonly string logPath;
        private readonly IRabbitMqService rabbit;

        public LoggingWorker(LoggingQueue queue, IWebHostEnvironment env, IRabbitMqService rabbit = null)
        {
            _queue = queue;
            rabbit = rabbit;
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            logPath = Path.Combine(dataDir, "logs.jsonl");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var entry))
                {
                    try
                    {
                        var line = JsonSerializer.Serialize(entry);
                        await File.AppendAllTextAsync(logPath, line + Environment.NewLine, stoppingToken);

                        // Optionally publish to RabbitMQ for other consumers
                        if (rabbit != null)
                        {
                            var msg = new IceCreamUpdatedMessage
                            {
                                UserId = 0,
                                Username = entry.Username,
                                IceCreamName = entry.Path,
                                Timestamp = entry.Start
                            };
                            await rabbit.PublishIceCreamUpdated(msg);
                        }
                    }
                    catch { /* swallow for robustness */ }
                }
                else
                {
                    await Task.Delay(200, stoppingToken);
                }
            }
        }
    }
}
