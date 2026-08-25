using Moonatna.Models;
using Moonatna.Repositories.Users;

namespace Moonatna.Services.Users
{
    public class UsersService:IUsersService
    {
        private readonly IUsersRepository _users;

        public UsersService(IUsersRepository users) => _users = users;

        public async Task<User> GetOrCreateAsync(string firebaseUid, string displayName)
        {
            var existing = await _users.GetByFirebaseUidAsync(firebaseUid);
            if (existing is not null)
                return existing;

            var user = new User { FirebaseUid = firebaseUid, DisplayName = displayName };
            user.Id = await _users.CreateAsync(user);
            return user;
        }
    }
}
