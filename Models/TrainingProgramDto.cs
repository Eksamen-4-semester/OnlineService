namespace OnlineService.Models;

public class TrainingProgramDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DifficultyLevel { get; set; } = string.Empty;

    public int DurationWeeks { get; set; }

    public List<string> Exercises { get; set; } = new();
}