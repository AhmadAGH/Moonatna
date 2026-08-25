using Moonatna.Models;

namespace Moonatna.Repositories.Families
{
    public interface IFamiliesRepository
    {
        Task<Family?> GetByIdAsync(int id);
        Task<Family?> GetByJoinCodeAsync(string joinCode);
        Task<int> CreateAsync(Family family);
        Task UpdateAsync(Family family); // rename + AutoPromoteAdHoc switch

        //Members
        Task<FamilyMember?> GetMembershipAsync(int familyId, int userId);
        Task<IEnumerable<Family>> GetFamiliesByUserIdAsync(int userId);
        Task AddMemberAsync(FamilyMember member);
        Task RemoveMemberAsync(int familyId, int userId);

        Task<IEnumerable<FamilyMemberInfo>> GetMembersAsync(int familyId);
        Task UpdateAutoPromoteAsync(int id, bool autoPromoteAdHoc);
    }
}
