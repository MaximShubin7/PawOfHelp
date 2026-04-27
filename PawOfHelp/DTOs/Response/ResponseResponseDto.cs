// DTOs/Response/ResponseResponseDto.cs
using PawOfHelp.DTOs.Public;

namespace PawOfHelp.DTOs.Response;

public class ResponseResponseDto
{
    public Guid Id { get; set; }
    public PublicProfileDto Sender { get; set; } = new();
    public Guid TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}