using System.Text;
using FidoDino.API.Extensions;
using FidoDino.API.Middleware;
using FidoDino.Application.Interfaces;
using FidoDino.Infrastructure.Data;
using FidoDino.Infrastructure.Data.Seeders;
using SmtpEmailService = FidoDino.Application.Interfaces.SmtpEmailService;

// Builder & Configuration
var builder = WebApplication.CreateBuilder(args);

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

// SMTP
var smtpSection = builder.Configuration.GetSection("SMTP");
var smtpUser = smtpSection["User"];
var smtpPass = smtpSection["Pass"];

// Service Registration (DI)

// CORS
builder.Services.AddCorsConfig();

// Authentication & Authorization
builder.Services.AddAuth(builder.Configuration);

// Database & Redis
builder.Services.AddDatabase(builder.Configuration);

// Cache
builder.Services.AddCacheServices();

// Controllers & Validation
builder.Services.AddControllersWithValidation();

// SignalR
builder.Services.AddSignalR();

// Repositories
builder.Services.AddRepositories();

// Application Services
builder.Services.AddApplicationServices();

// OAuth Clients
builder.Services.AddOAuth(builder.Configuration);

// Email Service
builder.Services.AddSingleton<IEmailService>(
    new SmtpEmailService(
        smtpUser: smtpUser!,
        smtpPass: smtpPass!
    )
);

// Seeders
builder.Services.AddScoped<ISeeder, UserDataSeeder>();
builder.Services.AddScoped<ISeeder, PermissionSeeder>();
builder.Services.AddScoped<ISeeder, IceRewardSeeder>();
builder.Services.AddScoped<DatabaseSeeder>();

// DbContext
builder.Services.AddScoped<FidoDinoDbContext>();

// Swagger
builder.Services.AddSwaggerDocs();

// Hangfire
builder.Services.AddHangfireJobs(builder.Configuration);

// Build App
var app = builder.Build();

// Middleware & Pipeline
// Swagger (Dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FidoDino API V1");
        options.DocumentTitle = "FidoDino API Documentation";
    });
}

app.UseCors("AllowFrontend");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHub<FidoDino.API.Hubs.LeaderboardHub>("/leaderboardHub");

// Startup Jobs
// Database seed
await app.UseDatabaseSeederAsync();

// Load platform data to Redis
using (var scope = app.Services.CreateScope())
{
    var startupService = scope.ServiceProvider.GetRequiredService<IStartupService>();
    await startupService.LoadPlatformDataToRedisAsync();
}

// Hangfire recurring jobs
app.UseLeaderboardSummaryJobs(builder.Configuration);

app.Run();
