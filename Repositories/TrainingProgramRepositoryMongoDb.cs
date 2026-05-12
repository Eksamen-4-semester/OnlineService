using MongoDB.Driver;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;

namespace OnlineService.Repositories;

public class TrainingProgramRepositoryMongoDb : ITrainingProgramRepository
{
    private readonly IMongoCollection<TrainingProgram> _trainingPrograms;

    public TrainingProgramRepositoryMongoDb(IMongoDatabase database)
    {
        _trainingPrograms = database.GetCollection<TrainingProgram>("TrainingPrograms");
    }

    public async Task<List<TrainingProgram>> GetAllTrainingPrograms()
    {
        return await _trainingPrograms.Find(_ => true).ToListAsync();
    }

    public async Task<TrainingProgram?> GetTrainingProgramById(string id)
    {
        return await _trainingPrograms.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateTrainingProgram(TrainingProgramDto dto)
    {
        var trainingProgram = new TrainingProgram
        {
            Title = dto.Title,
            Description = dto.Description,
            DifficultyLevel = dto.DifficultyLevel,
            DurationWeeks = dto.DurationWeeks,
            Exercises = dto.Exercises
        };

        await _trainingPrograms.InsertOneAsync(trainingProgram);

        return true;
    }

    public async Task<bool> DeleteTrainingProgram(string id)
    {
        var result = await _trainingPrograms.DeleteOneAsync(p => p.Id == id);

        return result.DeletedCount > 0;
    }
}