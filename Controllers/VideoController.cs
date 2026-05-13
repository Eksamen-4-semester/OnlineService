using Microsoft.AspNetCore.Mvc;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadVideo([FromForm] VideoDto dto)
    {
        _logger.LogInformation("Upload video request received");

        if (dto.VideoFile == null || dto.VideoFile.Length == 0)
        {
            _logger.LogWarning("Upload failed: no video file provided");
            return BadRequest("Video file is required");
        }

        await _videoRepository.UploadVideo(dto);

        _logger.LogInformation("Video uploaded: {Title}", dto.Title);

        return Created();
    }

    [HttpDelete("{videoId}")]
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