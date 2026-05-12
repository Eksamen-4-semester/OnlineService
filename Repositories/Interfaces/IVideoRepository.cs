using OnlineService.Models;

namespace OnlineService.Repositories.Interfaces;

public interface IVideoRepository
{
    Task<List<Video>> GetAllVideos();

    Task<Video?> GetVideoById(string id);

    Task<bool> UploadVideo(VideoDto dto);

    Task<bool> DeleteVideo(string id);
}