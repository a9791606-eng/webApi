using IceCreamProject.Hubs;
using IceCreamProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IceCreamNamespace.Services;

namespace IceCreamProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Minimal setup: controllers and required app services
            services.AddHttpContextAccessor();
            services.AddControllers();

            // Custom project services
            services.AddAppRepositories();
            services.AddIceCreamService();
            services.AddUserService();
            services.AddSingleton<LoggingQueue>();
            services.AddHostedService<LoggingWorker>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddRabbitMq();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            // request logging middleware
            app.UseMiddleware<IceCreamProject.Middleware.RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ActivityHub>("/activityHub");
            });
        }
    }
}
