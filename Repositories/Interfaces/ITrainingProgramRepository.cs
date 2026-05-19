using OnlineService.Models;

namespace OnlineService.Repositories.Interfaces;

public interface ITrainingProgramRepository
{
    Task<List<TrainingProgram>> GetAllTrainingPrograms();

    Task<TrainingProgram?> GetTrainingProgramById(string id);

    Task<bool> CreateTrainingProgram(TrainingProgramDto dto, string trainerId);
    Task<bool> DeleteTrainingProgram(string id);
}