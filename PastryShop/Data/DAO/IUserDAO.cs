using PastryShop.Enum;
using PastryShop.Model;

namespace PastryShop.Data.DAO
{
    public interface IUserDao
    {
        Task<bool> UsernameExists(string username);
        Task<User?> GetUserByUsername(string username);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(int userId);
        Task AddUser(User user);
        Task UpdateUserLastLogin(int userId);
        Task UpdateUser(User user);
        Task ActivateUser(int userId);
        Task DeactivateUser(int userId);
        Task UpdateUserPassword(int userId, string hashedPassword, bool forcePasswordChange);
        Task UpdateUserType(int userId, UserType newUserType, string? department = null, int? cashRegisterId = null, DateTime? shiftStart = null, DateTime? shiftEnd = null);
        Task UpdateUserProfile(User user);
        Task UpdateUserTheme(int userId, string theme);
        Task UpdateUserLanguage(int userId, string language);

    }
}