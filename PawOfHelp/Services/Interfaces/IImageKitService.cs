// Services/Interfaces/IImageKitService.cs
namespace PawOfHelp.Services.Interfaces;

public interface IImageKitService
{
    Task<string?> UploadImageAsync(IFormFile file, string folder, string entityId);
    Task<string?> UploadImageFromUrlAsync(string imageUrl, string folder, string entityId);
}