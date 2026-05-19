using Microsoft.AspNetCore.Mvc;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineService.Controllers;

[Authorize]
[ApiController]
[Route("api/trainingprograms")]
public class TrainingProgramController : ControllerBase
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly ILogger<TrainingProgramController> _logger;

    public TrainingProgramController(
        ITrainingProgramRepository trainingProgramRepository,
        ILogger<TrainingProgramController> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrainingPrograms()
    {
        _logger.LogInformation("Fetching all training programs");

        var programs = await _trainingProgramRepository.GetAllTrainingPrograms();

        return Ok(programs);
    }

    [HttpGet("{programId}")]
    public async Task<IActionResult> GetTrainingProgram(string programId)
    {
        _logger.LogInformation("Fetching training program: {ProgramId}", programId);

        var program = await _trainingProgramRepository.GetTrainingProgramById(programId);

        if (program == null)
        {
            _logger.LogWarning("Training program not found: {ProgramId}", programId);
            return NotFound();
        }

        return Ok(program);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PersonalTrainer")]
    public async Task<IActionResult> CreateTrainingProgram([FromBody] TrainingProgramDto dto)
    {
        _logger.LogInformation("Create training program request received: {Title}", dto.Title);

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            _logger.LogWarning("Training program creation failed: title missing");
            return BadRequest("Title is required");
        }

        var trainerId = User.FindFirst("userId")?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(trainerId))
        {
            _logger.LogWarning("Training program creation failed: trainer id missing from token");
            return Unauthorized("TrainerId missing from token");
        }

        await _trainingProgramRepository.CreateTrainingProgram(dto, trainerId);

        _logger.LogInformation(
            "Training program created: {Title} by trainer {TrainerId}",
            dto.Title,
            trainerId
        );

        return Created();
    }
    
    [HttpDelete("{programId}")]
    [Authorize(Roles = "Admin,PersonalTrainer")]
    public async Task<IActionResult> DeleteTrainingProgram(string programId)
    {
        _logger.LogInformation("Delete request received for training program: {ProgramId}", programId);

        var deleted = await _trainingProgramRepository.DeleteTrainingProgram(programId);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Training program not found: {ProgramId}", programId);
            return NotFound();
        }

        _logger.LogInformation("Training program deleted: {ProgramId}", programId);

        return NoContent();
    }
}