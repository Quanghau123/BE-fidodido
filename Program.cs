using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

using FidoDino.Infrastructure.Redis;
using FidoDino.Domain.Interfaces.Auth;
using FidoDino.Persistence.Repositories.Auth;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Persistence.Repositories.Game;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Persistence.Repositories.Leaderboard;
using FidoDino.Application.Services;
using FidoDino.Infrastructure.Data;
using System.Text;
using FidoDino.Common.Authorization;
using FidoDino.Infrastructure.Auth;
using FidoDino.Application.Interfaces;
using FidoDino.Infrastructure.Security;
using FidoDino.Infrastructure.Data.Seeders;
using Microsoft.AspNetCore.Authorization;
using SmtpEmailService = FidoDino.Application.Interfaces.SmtpEmailService;
using Microsoft.OpenApi.Models;
using System.Reflection;
using FidoDino.API.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using FidoDino.Application.Validation.User;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json.Serialization;
using FidoDino.Domain.Interfaces.System;
using FidoDino.Persistence.Repositories.System;
using StackExchange.Redis;
using FidoDino.Infrastructure.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Entities.Game;

// <summary>
// Khởi tạo builder cho ứng dụng web ASP.NET Core.
// </summary>
var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

var smtpSection = builder.Configuration.GetSection("SMTP");
var smtpUser = smtpSection["User"];
var smtpPass = smtpSection["Pass"];

//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // FE domain
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),

            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

//Authorization Policy
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permissions.All)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});


// EF Core DbContext
builder.Services.AddDbContext<FidoDinoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<RedisConnectionFactory>(sp =>
    new RedisConnectionFactory(builder.Configuration.GetConnectionString("Redis")!));

// Đăng ký IConnectionMultiplexer và IDatabase cho StackExchange.Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddScoped<IDatabase>(sp =>
    sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// SignalR
builder.Services.AddSignalR();

// BackgroundService cập nhật BXH
builder.Services.AddHostedService<FidoDino.API.BackgroundServices.LeaderboardUpdateService>();

//Controllers & Validation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

//OAuth Options
builder.Services.Configure<GoogleOAuthOptions>(
    builder.Configuration.GetSection("Google"));

builder.Services.Configure<FacebookOAuthOptions>(
    builder.Configuration.GetSection("Facebook"));

//DI Seeders
builder.Services.AddScoped<ISeeder, UserDataSeeder>();
builder.Services.AddScoped<ISeeder, PermissionSeeder>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<ISeeder, IceRewardSeeder>();

//DI Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
builder.Services.AddScoped<IGamePlayService, GamePlayService>();
builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<ILeaderboardAppService, LeaderboardAppService>();
builder.Services.AddScoped<ISnapshotService, SnapshotService>();
builder.Services.AddScoped<IPlayTurnService, PlayTurnService>();
builder.Services.AddScoped<IEffectService, EffectService>();
builder.Services.AddScoped<IStartupService, StartupService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ISystemStatusAppService, SystemStatusAppService>();
builder.Services.AddScoped<ILeaderboardSummaryService, LeaderboardSummaryService>();

//DI Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
builder.Services.AddScoped<IPlayTurnRepository, PlayTurnRepository>();
builder.Services.AddScoped<IIceRepository, IceRepository>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>();
builder.Services.AddScoped<ILeaderboardStateRepository, LeaderboardStateRepository>();
builder.Services.AddScoped<IActiveEffectRepository, ActiveEffectRepository>();
builder.Services.AddScoped<ISystemStatusRepository, SystemStatusRepository>();
builder.Services.AddScoped<IIceRewardRepository, IceRewardRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<ILeaderboardSnapshotRepository, LeaderboardSnapshotRepository>();

//DI Email Service
builder.Services.AddSingleton<IEmailService>(
    new SmtpEmailService(
        smtpUser: smtpUser!,
        smtpPass: smtpPass!
    )
);

//DI OAuth Clients
builder.Services.AddHttpClient();
builder.Services.AddScoped<IOAuthClient, GoogleOAuthClient>();
builder.Services.AddScoped<IOAuthClient, FacebookOAuthClient>();
builder.Services.AddScoped<IOAuthService, OAuthService>();

// EffectCacheService DI
builder.Services.AddScoped<EffectCacheService>();
builder.Services.AddScoped<IceCacheService>();
builder.Services.AddScoped<IceRewardCacheService>();
builder.Services.AddScoped<RewardCacheService>();

// FidoDinoDbContext DI
builder.Services.AddScoped<FidoDinoDbContext>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Authentication vào Swagger
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header sử dụng Bearer scheme.'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // Include XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Hangfire configuration and recurring job registration
#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddHangfire(x =>
    x.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
#pragma warning restore CS0618 // Type or member is obsolete
builder.Services.AddHangfireServer();

// <summary>
// Build ứng dụng web từ cấu hình đã thiết lập.
// </summary>
var app = builder.Build();
// Map SignalR Hub endpoint
app.MapHub<FidoDino.API.Hubs.LeaderboardHub>("/leaderboardHub");

// Register Hangfire recurring job for leaderboard summary (after app build)
using (var scope = app.Services.CreateScope())
{
    var configValue = builder.Configuration["Leaderboard:DefaultTimeRange"];
    if (!Enum.TryParse<TimeRangeType>(configValue, true, out var timeRange))
        timeRange = TimeRangeType.Day;

    var recurringJobManager =
        scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    switch (timeRange)
    {
        case TimeRangeType.Day:
            recurringJobManager.AddOrUpdate<LeaderboardSummaryService>(
                "summary-day",
                s => s.SummarizeAndResetAsync(TimeRangeType.Day, 100),
                Cron.Daily(23, 59));
            // Cron.Minutely());
            break;

        case TimeRangeType.Week:
            recurringJobManager.AddOrUpdate<LeaderboardSummaryService>(
                "summary-week",
                s => s.SummarizeAndResetAsync(TimeRangeType.Week, 100),
                Cron.Weekly(DayOfWeek.Sunday, 23, 59));
            // Cron.Minutely());
            break;

        case TimeRangeType.Month:
            recurringJobManager.AddOrUpdate<LeaderboardSummaryService>(
                "summary-month",
                s => s.SummarizeAndResetAsync(TimeRangeType.Month, 100),
                Cron.Monthly(23, 59));
            break;
    }
}

// <summary>
// Thiết lập Swagger UI khi chạy ở môi trường Development.
// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FidoDino API V1");
        options.DocumentTitle = "FidoDino API Documentation";
    });
}

// <summary>
// Thực hiện seed dữ liệu ban đầu cho database khi khởi động ứng dụng.
// </summary>
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FidoDinoDbContext>();
    var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await databaseSeeder.SeedAsync(db);
}

// Load dữ liệu nền tảng lên Redis khi server khởi động
using (var scope = app.Services.CreateScope())
{
    var startupService = scope.ServiceProvider.GetRequiredService<IStartupService>();
    await startupService.LoadPlatformDataToRedisAsync();
}

// <summary>
// Thiết lập các middleware và chạy ứng dụng web.
// </summary>
app.UseCors("AllowFrontend");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
