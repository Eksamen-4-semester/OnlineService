using MongoDB.Driver;
using OnlineService.Repositories;
using OnlineService.Repositories.Interfaces;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddAuthorization();

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        Environment.GetEnvironmentVariable("JWT_SECRET")
                        ?? "THIS_IS_A_DEVELOPMENT_SECRET_KEY_CHANGE_ME_12345"
                    )
                ),
            ValidIssuer = "AuthService",
            ValidAudience = "User",
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString =
        Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
        ?? "mongodb://localhost:27017";
    
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();

    var databaseName =
        Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME")
        ?? "OnlineServiceDb";

    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<IVideoRepository, VideoRepositoryMongoDb>();
builder.Services.AddScoped<ITrainingProgramRepository, TrainingProgramRepositoryMongoDb>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("OnlineService started");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();