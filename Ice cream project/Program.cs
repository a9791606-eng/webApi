using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
using IceCreamNamespace.Middleware; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- 1. הגדרת Serilog (תיעוד לוגים לקובץ) ---
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Data/logs.txt", 
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 50 * 1024 * 1024, // 50MB
        rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Host.UseSerilog();

// --- 2. רישום שירותים (Dependency Injection) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppRepositories();
builder.Services.AddIceCream();
builder.Services.AddUserService();
builder.Services.AddActiveUser();
builder.Services.AddSignalR();

// רישום ה-Queue וה-Workers
builder.Services.AddSingleton<LoggingQueue>();
builder.Services.AddHostedService<LoggingWorker>();
builder.Services.AddHostedService<IceCreamUpdateWorker>();

// הגדרת אימות JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = IceCreamTokenService.GetTokenValidationParameters();
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("type", "Admin"));
});

var app = builder.Build();

// --- 3. הגדרת ה-Pipeline (סדר הפעולות קריטי!) ---

// א. תמיכה בקבצים סטטיים (חייב להופיע לפני הכל כדי שהאתר יעלה)
app.UseDefaultFiles(); 
app.UseStaticFiles();

// ב. הגדרת Swagger - מוגדר לעבוד בכתובת /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ice Cream API V1");
    // שינינו מ-string.Empty ל-"swagger" כדי שדף הבית יהיה פנוי לאתר שלך
    c.RoutePrefix = "swagger"; 
});

app.UseRouting();

// ג. מידלוור הלוגים המותאם שלך
app.UseMiddleware<RequestLoggingMiddleware>(); 

app.UseAuthentication();
app.UseAuthorization();

// ד. מיפוי ה-Endpoints
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