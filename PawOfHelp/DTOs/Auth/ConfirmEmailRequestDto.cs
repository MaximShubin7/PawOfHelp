// DTOs/Auth/ConfirmEmailRequestDto.cs
namespace PawOfHelp.DTOs.Auth;

public class ConfirmEmailRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}