using FitLife.TrainingLog.Api.Repositories;
using FitLife.TrainingLog.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "fitlife-identity";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "fitlife";
    var jwksUrl = builder.Configuration["Jwt:JwksUrl"] ?? "http://localhost:5244/.well-known/jwks.json";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (_, _, _, _) => JwksSigningKeyResolver.GetSigningKeys(jwksUrl)
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
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Frontend");

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

static class JwksSigningKeyResolver
{
    private static readonly HttpClient Client = new();
    private static DateTime _expiresAt = DateTime.MinValue;
    private static IReadOnlyCollection<SecurityKey> _cachedKeys = [];

    public static IEnumerable<SecurityKey> GetSigningKeys(string jwksUrl)
    {
        if (_cachedKeys.Count > 0 && DateTime.UtcNow < _expiresAt)
            return _cachedKeys;

        var json = Client.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
        var jwks = new JsonWebKeySet(json);

        _cachedKeys = jwks.Keys.Cast<SecurityKey>().ToArray();
        _expiresAt = DateTime.UtcNow.AddMinutes(5);
        return _cachedKeys;
    }
}