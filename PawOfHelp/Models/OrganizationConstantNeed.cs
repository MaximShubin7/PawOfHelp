// Models/OrganizationConstantNeed.cs
namespace PawOfHelp.Models;

public class OrganizationConstantNeed
{
    public Guid OrganizationId { get; set; }
    public short ConstantNeedId { get; set; }

    public OrganizationDetails Organization { get; set; } = null!;
    public ConstantNeed ConstantNeed { get; set; } = null!;
}