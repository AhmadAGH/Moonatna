using Moonatna.Models;
using Moonatna.Repositories.Families;
using System.Security.Cryptography;

namespace Moonatna.Services.Families
{
    public class FamiliesService : IFamiliesService
    {
        private readonly IFamiliesRepository _families;

        public FamiliesService(IFamiliesRepository families) => _families = families;

        public async Task<Family> CreateFamilyAsync(string name, int userId)
        {
            var family = new Family
            {
                Name = name.Trim(),
                JoinCode = await GenerateUniqueJoinCodeAsync(),
                CreatedByUserId = userId
            };

            family.Id = await _families.CreateAsync(family);

            await _families.AddMemberAsync(new FamilyMember
            {
                FamilyId = family.Id,
                UserId = userId,
                Role = FamilyRole.Owner
            });

            return family;
        }

        public async Task<Family?> JoinFamilyAsync(string joinCode, int userId)
        {
            var family = await _families.GetByJoinCodeAsync(joinCode.Trim().ToUpperInvariant());
            if (family is null)
                return null;

            var existing = await _families.GetMembershipAsync(family.Id, userId);
            if (existing is not null)
                return family; // already a member — joining twice is a no-op, not an error

            await _families.AddMemberAsync(new FamilyMember
            {
                FamilyId = family.Id,
                UserId = userId,
                Role = FamilyRole.Member
            });

            return family;
        }

        public async Task<IEnumerable<Family>> GetMyFamiliesAsync(int userId)
            => await _families.GetFamiliesByUserIdAsync(userId);

        private async Task<string> GenerateUniqueJoinCodeAsync()
        {
            const string alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789"; // no 0/O, 1/I/L confusion

            for (var attempt = 0; attempt < 10; attempt++)
            {
                var chars = new char[6];
                for (var i = 0; i < chars.Length; i++)
                    chars[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];

                var code = new string(chars);
                if (await _families.GetByJoinCodeAsync(code) is null)
                    return code;
            }

            throw new InvalidOperationException("Could not generate a unique join code.");
        }

        public async Task<FamilyMember?> GetMembershipAsync(int familyId, int userId)
            => await _families.GetMembershipAsync(familyId, userId);

        public async Task<IEnumerable<FamilyMemberInfo>> GetMembersAsync(int familyId)
            => await _families.GetMembersAsync(familyId);

        public async Task<bool> SetAutoPromoteAsync(int familyId, bool enabled, int userId)
        {
            var membership = await _families.GetMembershipAsync(familyId, userId);
            if (membership?.Role != FamilyRole.Owner) return false;   // the switch is the owner's
            await _families.UpdateAutoPromoteAsync(familyId, enabled);
            return true;
        }
    }
}
