using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// configure Serilog early
var logFilePath = builder.Configuration.GetValue<string>("Logging:FilePath")
    ?? Path.Combine(builder.Environment.ContentRootPath, "Data", "logs-.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 50_000_000, retainedFileCountLimit: 7)
    .CreateLogger();
builder.Host.UseSerilog();

// Core registrations required for DI
builder.Services.AddIceCream();
builder.Services.AddUserService();
builder.Services.AddAppRepositories();
// ensure repositories are available for services that depend on them
// AddIceCream registers IceCreamRepository; AddAppRepositories registers UserRepository
builder.Services.AddActiveUser();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddRabbitMq();

// Configure authentication: JWT for API + Cookie/Google for external interactive login
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.TokenValidationParameters = IceCreamTokenService.GetTokenValidationParameters();
    })
    .AddCookie("External")
    .AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.SignInScheme = "External";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("type", "Admin"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// serve login.html as default
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("login.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// request logging middleware (now writes to Serilog)
app.UseMiddleware<IceCreamNamespace.Middleware.RequestLoggingMiddleware>();

app.MapHub<IceCreamNamespace.Hubs.ActivityHub>("/activityHub");

// map external auth callback route for Google (signin-google)
// the ExternalAuthController handles issuance of JWT after Google signin
app.MapControllers();

app.Run();

