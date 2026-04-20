// Services/Interfaces/IOrganizationService.cs
using PawOfHelp.DTOs.Organization;

namespace PawOfHelp.Services.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationResponseDto> GetOrganizationAsync(Guid userId);
    Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid userId, UpdateOrganizationDto dto);
}