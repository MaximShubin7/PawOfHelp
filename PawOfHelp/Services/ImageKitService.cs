// Services/ImageKitService.cs
using System.Text;
using System.Text.Json;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class ImageKitService : IImageKitService
{
    private readonly HttpClient _httpClient;
    private readonly string _privateKey;
    private readonly string _urlEndpoint;

    public ImageKitService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _privateKey = configuration["ImageKit:PrivateKey"];
        _urlEndpoint = configuration["ImageKit:UrlEndpoint"];

        if (string.IsNullOrEmpty(_privateKey))
            throw new Exception("ImageKit Private Key не найден в настройках");
    }

    public async Task<string?> UploadImageAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0)
            return null;

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            throw new Exception("Только JPEG, PNG, WEBP");

        if (file.Length > 25 * 1024 * 1024)
            throw new Exception("Файл не должен превышать 25 MB");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_{file.FileName}";

        return await UploadToImageKitAsync(fileBytes, fileName, file.ContentType);
    }

    public async Task<string?> UploadImageFromUrlAsync(string imageUrl, string userId)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_from_web.jpg";

        return await UploadToImageKitAsync(imageUrl, fileName);
    }

    private async Task<string?> UploadToImageKitAsync(object fileSource, string fileName, string? contentType = null)
    {
        using var formData = new MultipartFormDataContent();

        if (fileSource is byte[] bytes)
        {
            var fileContent = new ByteArrayContent(bytes);
            if (contentType != null)
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            formData.Add(fileContent, "file", fileName);
        }
        else if (fileSource is string url)
        {
            formData.Add(new StringContent(url), "file");
        }

        formData.Add(new StringContent(fileName), "fileName");
        formData.Add(new StringContent("/users"), "folder");
        formData.Add(new StringContent("true"), "useUniqueFileName");

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var response = await _httpClient.PostAsync("https://upload.imagekit.io/api/v1/files/upload", formData);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ошибка загрузки в ImageKit: {responseJson}");

        using var document = JsonDocument.Parse(responseJson);
        var uploadedUrl = document.RootElement.GetProperty("url").GetString();

        return uploadedUrl;
    }

    public async Task<bool> DeleteImageAsync(string fileId)
    {
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var response = await _httpClient.DeleteAsync($"https://upload.imagekit.io/api/v1/files/{fileId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetFileIdFromUrlAsync(string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl))
            return null;

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var encodedUrl = Uri.EscapeDataString(photoUrl);
        var response = await _httpClient.GetAsync($"https://upload.imagekit.io/api/v1/files?url={encodedUrl}");
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return null;

        using var document = JsonDocument.Parse(responseJson);
        if (document.RootElement.GetArrayLength() > 0)
        {
            var fileId = document.RootElement[0].GetProperty("fileId").GetString();
            return fileId;
        }

        return null;
    }
}