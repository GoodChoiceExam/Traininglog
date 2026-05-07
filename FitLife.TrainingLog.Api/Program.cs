using FitLife.TrainingLog.Api.Repositories;
using FitLife.TrainingLog.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NLog;
using NLog.Web;

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));


var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
    var database = mongoClient.GetDatabase(builder.Configuration["MongoDB:DatabaseName"]);
    builder.Services.AddSingleton(database);

    builder.Services.AddSingleton<IWorkoutRepository, WorkoutRepository>();
    builder.Services.AddSingleton<IWorkoutService, WorkoutService>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = "fitlife-identity",
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                IssuerSigningKeyResolver = (_, _, kid, _) =>
                {
                    var client = new HttpClient();
                    var json = client.GetStringAsync(
                        builder.Configuration["Jwt:Authority"] + "/.well-known/jwks.json").Result;
                    var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(json);
                    return jwks.GetSigningKeys();
                }
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

    app.UseAuthentication();
    app.UseAuthorization();

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