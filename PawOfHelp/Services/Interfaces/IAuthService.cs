// Services/Interfaces/IAuthService.cs
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestDto request);
    Task<VolunteerAuthResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request);
    Task<VolunteerAuthResponseDto> LoginAsync(LoginRequestDto request);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task ResetPasswordWithCodeAsync(ResetPasswordWithCodeRequestDto request);
}