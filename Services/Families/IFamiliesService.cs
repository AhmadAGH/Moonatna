using Moonatna.Models;

namespace Moonatna.Services.Families
{
    public interface IFamiliesService
    {
        Task<Family> CreateFamilyAsync(string name, int userId);
        Task<Family?> JoinFamilyAsync(string joinCode, int userId);
        Task<IEnumerable<Family>> GetMyFamiliesAsync(int userId);
        Task<FamilyMember?> GetMembershipAsync(int familyId, int userId);
        Task<IEnumerable<FamilyMemberInfo>> GetMembersAsync(int familyId);
        Task<bool> SetAutoPromoteAsync(int familyId, bool enabled, int userId);
    }
}
