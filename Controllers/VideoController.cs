using Microsoft.AspNetCore.Mvc;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineService.Controllers;


[Authorize]
[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly IVideoRepository _videoRepository;
    private readonly ILogger<VideoController> _logger;

    public VideoController(
        IVideoRepository videoRepository,
        ILogger<VideoController> logger)
    {
        _videoRepository = videoRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVideos()
    {
        _logger.LogInformation("Fetching all videos");

        var videos = await _videoRepository.GetAllVideos();

        return Ok(videos);
    }

    [HttpGet("{videoId}")]
    public async Task<IActionResult> GetVideo(string videoId)
    {
        _logger.LogInformation("Fetching video: {VideoId}", videoId);

        var video = await _videoRepository.GetVideoById(videoId);

        if (video == null)
        {
            _logger.LogWarning("Video not found: {VideoId}", videoId);
            return NotFound();
        }

        return Ok(video);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PersonalTrainer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadVideo([FromForm] VideoDto dto)
    {
        _logger.LogInformation("Upload video request received");

        if (dto.VideoFile == null || dto.VideoFile.Length == 0)
        {
            _logger.LogWarning("Upload failed: no video file provided");
            return BadRequest("Video file is required");
        }

        var trainerId = User.FindFirst("userId")?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(trainerId))
        {
            _logger.LogWarning("Upload failed: trainer id missing from token");
            return Unauthorized("TrainerId missing from token");
        }

        await _videoRepository.UploadVideo(dto, trainerId);

        _logger.LogInformation("Video uploaded: {Title} by trainer {TrainerId}", dto.Title, trainerId);

        return Created();
    }

    [HttpDelete("{videoId}")]
    [Authorize(Roles = "Admin,PersonalTrainer")]
    public async Task<IActionResult> DeleteVideo(string videoId)
    {
        _logger.LogInformation("Delete request received for video: {VideoId}", videoId);

        var deleted = await _videoRepository.DeleteVideo(videoId);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Video not found: {VideoId}", videoId);
            return NotFound();
        }

        _logger.LogInformation("Video deleted: {VideoId}", videoId);

        return NoContent();
    }
}