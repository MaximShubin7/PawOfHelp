// DTOs/HelpTask/HelpTaskListResponseDto.cs
namespace PawOfHelp.DTOs.HelpTask;

public class HelpTaskListResponseDto
{
    public List<HelpTaskResponseDto> Tasks { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}