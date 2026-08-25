using Moonatna.Models;

namespace Moonatna.Services.Users
{
    public interface IUsersService
    {
        Task<User> GetOrCreateAsync(string firebaseUid, string displayName);
    }

}
