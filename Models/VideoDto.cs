using Microsoft.AspNetCore.Http;

namespace OnlineService.Models;

public class VideoDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IFormFile? VideoFile { get; set; }
}