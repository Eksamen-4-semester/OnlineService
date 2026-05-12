using MongoDB.Driver;
using OnlineService.Repositories;
using OnlineService.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

app.UseAuthorization();

app.MapControllers();

app.Run();