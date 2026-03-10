using IceCreamNamespace.Services;
using IceCreamService.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using MyMiddleware;
var builder = WebApplication.CreateBuilder(args);

// Ensure Kestrel listens on the development HTTPS and HTTP ports configured in launchSettings
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7274, listenOptions => listenOptions.UseHttps()); // HTTPS
    options.ListenLocalhost(5089); // HTTP
});

builder.Services.AddIceCreamService();
builder.Services.AddUserService();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthorization();

// app.UseMyLogMiddleware();
// app.UseMy1stMiddleware();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
app.MapControllers();

app.Run();

