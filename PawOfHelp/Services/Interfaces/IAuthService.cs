// Services/Interfaces/IAuthService.cs
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}