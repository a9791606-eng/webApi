using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using IceCreamNamespace.Services;
using Serilog;

namespace IceCreamNamespace.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggingQueue? _queue;

        public RequestLoggingMiddleware(RequestDelegate next, LoggingQueue? queue = null)
        {
            _next = next;
            _queue = queue;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var start = DateTime.UtcNow;


            await _next(context);

           
            sw.Stop();

          
            string controller = "-";
            string action = "-";
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var routeValues = context.Request.RouteValues;
                if (routeValues.TryGetValue("controller", out var c)) controller = c?.ToString() ?? "-";
                if (routeValues.TryGetValue("action", out var a)) action = a?.ToString() ?? "-";
            }

           
            string username = "Anonymous";
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                username = context.User.FindFirst("username")?.Value 
                           ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                           ?? context.User.Identity?.Name 
                           ?? "AuthenticatedUser";
            }

            var entry = new IceCreamNamespace.Models.LogEntry
            {
                Start = start,
                Controller = controller,
                Action = action,
                Username = username,
                DurationMs = sw.ElapsedMilliseconds,
                Path = context.Request.Path,
                StatusCode = context.Response?.StatusCode ?? 0
            };

            try
            {
               
                Log.Information("{Start} {Controller}/{Action} {Username} {StatusCode} {DurationMs}ms {Path}", 
                    entry.Start, entry.Controller, entry.Action, entry.Username, entry.StatusCode, entry.DurationMs, entry.Path);

             
                _queue?.Enqueue(entry);
            }
            catch { /* שמירה על יציבות השרת במקרה של תקלה בלוג */ }
        }
    }
}