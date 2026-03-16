using IceCreamNamespace.Hubs;
using IceCreamNamespace.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IceCreamNamespace
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
            // register repositories and services
            services.AddAppRepositories();
            services.AddIceCream();
            services.AddUserService();
            services.AddSingleton<LoggingQueue>();
            services.AddHostedService<LoggingWorker>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddRabbitMq();

            // ...existing code...

            services.AddAuthorization(cfg =>
            {
                cfg.AddPolicy("AdminOnly", policy => policy.RequireClaim("type", "Admin"));
            });

            services.AddHttpContextAccessor();
            services.AddControllers();
            // Use basic Swagger generation; detailed OpenAPI types removed to avoid package conflicts
            services.AddSwaggerGen();

            services.AddActiveUser();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IceCream v1"));
            }

            app.UseHttpsRedirection();
            /*js*/
            app.UseDefaultFiles();
            app.UseStaticFiles();
            /*js (remove "launchUrl" from Properties\launchSettings.json*/

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ActivityHub>("/activityHub");
            });
        }
    }
}
