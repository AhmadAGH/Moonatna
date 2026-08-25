using Moonatna.Models;

namespace Moonatna.Repositories.Users
{
    public interface IUsersRepository
    {
        Task<User?> GetByFirebaseUidAsync(string firebaseUid);
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
    }
}
