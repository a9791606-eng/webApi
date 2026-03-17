using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
using IceCreamNamespace.Middleware; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


var logFilePath = builder.Configuration["Serilog:LogFilePath"] ?? "Data/logs.txt";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(logFilePath,
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 50 * 1024 * 1024, // 50MB
        rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppRepositories();
builder.Services.AddIceCream();
builder.Services.AddUserService();
builder.Services.AddActiveUser();
builder.Services.AddSignalR();

builder.Services.AddSingleton<LoggingQueue>();
builder.Services.AddHostedService<LoggingWorker>();
builder.Services.AddHostedService<IceCreamUpdateWorker>();

var googleConfig = builder.Configuration.GetSection("Google");
var clientId = googleConfig["ClientId"];
var clientSecret = googleConfig["ClientSecret"];

var authBuilder = builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options => {
        options.Cookie.SameSite = SameSiteMode.Lax; // Use Lax for localhost, None requires Secure and is not allowed for localhost in some browsers
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters = IceCreamTokenService.GetTokenValidationParameters();
    });


if (!string.IsNullOrEmpty(clientId) && !clientId.Contains("YOUR_") && !string.IsNullOrEmpty(clientSecret) && !clientSecret.Contains("YOUR_"))
{
    authBuilder.AddGoogle(options => {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        // Must match the Redirect URI configured in Google Cloud Console
        options.CallbackPath = new PathString("/external/google-callback");
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.SaveTokens = true;

        // Required so the OAuth state/correlation cookies are sent correctly in cross-site redirects
        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    });
}

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("type", "Admin"));
});

var app = builder.Build();


app.UseDefaultFiles(); 
app.UseStaticFiles();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ice Cream API V1");

    c.RoutePrefix = "swagger"; 
});

app.UseRouting();


app.UseMiddleware<RequestLoggingMiddleware>(); 

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<IceCreamNamespace.Hubs.ActivityHub>("/activityHub");

try
{
    Log.Information("Starting Ice Cream Web API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}