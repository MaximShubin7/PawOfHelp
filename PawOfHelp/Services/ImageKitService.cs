// Services/ImageKitService.cs
using System.Text;
using System.Text.Json;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class ImageKitService : IImageKitService
{
    private readonly HttpClient _httpClient;
    private readonly string _privateKey;

    public ImageKitService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _privateKey = configuration["ImageKit:PrivateKey"];

        if (string.IsNullOrEmpty(_privateKey))
            throw new Exception("ImageKit Private Key не найден в настройках");
    }

    public async Task<string?> UploadImageAsync(IFormFile file, string folder, string entityId)
    {
        if (file == null || file.Length == 0)
            return null;

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            throw new Exception("Только JPEG, PNG, WEBP");

        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("Файл не должен превышать 5 MB");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var fileName = $"{entityId}_{DateTime.UtcNow.Ticks}_{file.FileName}";

        using var formData = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        formData.Add(fileContent, "file", fileName);

        formData.Add(new StringContent(fileName), "fileName");
        formData.Add(new StringContent($"/{folder}"), "folder");
        formData.Add(new StringContent("true"), "useUniqueFileName");

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var response = await _httpClient.PostAsync("https://upload.imagekit.io/api/v1/files/upload", formData);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ошибка загрузки в ImageKit: {responseJson}");

        using var document = JsonDocument.Parse(responseJson);
        var url = document.RootElement.GetProperty("url").GetString();

        return url;
    }

    public async Task<string?> UploadImageFromUrlAsync(string imageUrl, string folder, string entityId)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        var fileName = $"{entityId}_{DateTime.UtcNow.Ticks}_from_web.jpg";

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(imageUrl), "file");
        formData.Add(new StringContent(fileName), "fileName");
        formData.Add(new StringContent($"/{folder}"), "folder");
        formData.Add(new StringContent("true"), "useUniqueFileName");

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var response = await _httpClient.PostAsync("https://upload.imagekit.io/api/v1/files/upload", formData);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ошибка загрузки в ImageKit: {responseJson}");

        using var document = JsonDocument.Parse(responseJson);
        var url = document.RootElement.GetProperty("url").GetString();

        return url;
    }
}