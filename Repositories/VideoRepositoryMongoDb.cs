using MongoDB.Driver;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;

namespace OnlineService.Repositories;

public class VideoRepositoryMongoDb : IVideoRepository
{
    private readonly IMongoCollection<Video> _videos;

    public VideoRepositoryMongoDb(IMongoDatabase database)
    {
        _videos = database.GetCollection<Video>("Videos");
    }

    public async Task<List<Video>> GetAllVideos()
    {
        return await _videos.Find(_ => true).ToListAsync();
    }

    public async Task<Video?> GetVideoById(string id)
    {
        return await _videos.Find(v => v.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> UploadVideo(VideoDto dto, string trainerId)    
    {
        var video = new Video
        {
            Title = dto.Title,
            Description = dto.Description,
            FileName = dto.VideoFile?.FileName ?? string.Empty,
            TrainerId = trainerId,
            UploadDate = DateTime.UtcNow
        };

        await _videos.InsertOneAsync(video);

        return true;
    }

    public async Task<bool> DeleteVideo(string id)
    {
        var result = await _videos.DeleteOneAsync(v => v.Id == id);

        return result.DeletedCount > 0;
    }
}