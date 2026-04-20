// Services/Interfaces/IPublicUserService.cs
using PawOfHelp.DTOs.User;
using PawOfHelp.DTOs.Organization;

namespace PawOfHelp.Services.Interfaces;

public interface IPublicUserService
{
    Task<UserProfileResponseDto> GetPublicUserProfileAsync(Guid userId);
    Task<OrganizationResponseDto> GetPublicOrganizationProfileAsync(Guid userId);
}