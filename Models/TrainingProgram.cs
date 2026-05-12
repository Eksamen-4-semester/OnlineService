using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineService.Models;

public class TrainingProgram
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DifficultyLevel { get; set; } = string.Empty;

    public int DurationWeeks { get; set; }

    public List<string> Exercises { get; set; } = new();
}