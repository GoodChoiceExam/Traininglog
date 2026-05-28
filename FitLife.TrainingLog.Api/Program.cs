using System.Text;
using System.Text.Json.Serialization;
using FitLife.TrainingLog.Api.Repositories;
using FitLife.TrainingLog.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NLog;
using NLog.Web;

// Logger sættes op tidligt så vi kan fange fejl allerede under startup
var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();

try
{
    // Guid skal serialiseres som standard UUID-streng i MongoDB, ikke som legacy BSON Binary
    BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

    var builder = WebApplication.CreateBuilder(args);

    // Fjerner de standard ASP.NET log-providers og sætter NLog op i stedet
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "fitlife-identity";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "fitlife";
    var secret = builder.Configuration["Jwt:Secret"] ?? "dev-secret-change-in-production";

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    logger.Warn(context.Exception, "JWT authentication failed");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    logger.Warn("Unauthorized request to {Path}", context.Request.Path);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
            policy.WithOrigins("http://localhost:5271")
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    // MongoDB-klienten er singleton fordi MongoClient er trådsikker og dyr at oprette
    var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
    var database = mongoClient.GetDatabase(builder.Configuration["MongoDB:DatabaseName"]);
    builder.Services.AddSingleton(database);

    builder.Services.AddSingleton<IWorkoutRepository, WorkoutRepository>();
    builder.Services.AddSingleton<IWorkoutService, WorkoutService>();
    builder.Services.AddHostedService<HeartbeatService>();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a valid JWT Bearer token from the Identity service."
        });

        options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", null, null),
                []
            }
        });
    });

    var app = builder.Build();
    logger.Info("FitLife TrainingLog API starting");

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Frontend");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    LogManager.Shutdown();
}
