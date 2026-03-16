using System.Text.Json;
using Microsoft.Extensions.Hosting;
using IceCreamNamespace.Models;
using Serilog;

namespace IceCreamNamespace.Services
{
    public class LoggingWorker : BackgroundService
    {
        private readonly LoggingQueue _queue;
        private readonly IRabbitMqService? _rabbit;

        public LoggingWorker(LoggingQueue queue, IRabbitMqService? rabbit = null)
        {
            _queue = queue;
            _rabbit = rabbit;

            // הגדרת Serilog עם Rotation של 50MB כפי שנדרש באתגר
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Data", "logs.txt"), 
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 50 * 1024 * 1024, // 50MB
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var entry))
                {
                    try
                    {
                        // כתיבה ללוג דרך Serilog
                        Log.Information("Request: {Controller}/{Action} | User: {Username} | Duration: {Duration}ms", 
                            entry.Controller, entry.Action, entry.Username, entry.DurationMs);

                        // שליחה ל-RabbitMQ אם קיים
                        if (_rabbit != null)
                        {
                            var msg = new IceCreamUpdatedMessage
                            {
                                Username = entry.Username,
                                IceCreamName = entry.Path,
                                Timestamp = entry.Start
                            };
                            await _rabbit.PublishIceCreamUpdated(msg);
                        }
                    }
                    catch { /* robustness */ }
                }
                else
                {
                    await Task.Delay(200, stoppingToken);
                }
            }
        }
    }
}