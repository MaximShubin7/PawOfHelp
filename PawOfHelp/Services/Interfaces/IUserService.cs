// Services/Interfaces/IUserService.cs
using PawOfHelp.DTOs.User;

namespace PawOfHelp.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileResponseDto> GetProfileAsync(Guid userId);
    Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    Task DeleteProfileAsync(Guid userId);
}