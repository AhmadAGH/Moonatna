using Moonatna.Models;

namespace Moonatna.ViewModels.Families;

public class FamilySettingsViewModel
{
    public int FamilyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; }    // displayed for sharing
    public bool AutoPromoteAdHoc { get; set; }
    public bool IsOwner { get; set; }       // only the owner sees the switch
    public List<FamilyMemberViewModel> Members { get; set; } = new();
}

public class FamilyMemberViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public FamilyRole Role { get; set; }
    public string RoleKey => $"FamilyRole.{Role}";
}
