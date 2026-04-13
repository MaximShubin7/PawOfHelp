// Services/Interfaces/IImageKitService.cs
namespace PawOfHelp.Services.Interfaces;

public interface IImageKitService
{
    Task<string?> UploadImageAsync(IFormFile file, string userId);
    Task<string?> UploadImageFromUrlAsync(string imageUrl, string userId);
    Task<bool> DeleteImageAsync(string fileId);
    Task<string?> GetFileIdFromUrlAsync(string photoUrl);
}