// DTOs/Response/ResponseListResponseDto.cs
namespace PawOfHelp.DTOs.Response;

public class ResponseListResponseDto
{
    public List<ResponseResponseDto> Responses { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}