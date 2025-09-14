using PastryShop.Data.DTO;

namespace PastryShop.Service
{
    public interface IUserService
    {
        Task<LoginAuthenticatedUserDTO?> AuthenticateUser(LoginCredentialUserDTO loginCredentialUserDTO);
        Task<bool> UsernameExists(string username);
        Task<List<InfoUserBasicDTO>> GetAllUsersBasicInfo();
        Task<InfoUserDetailsDTO?> GetUserDetailsById(int userId);
        Task<EditUserManagementDTO?> GetEditUserById(int userId);
        Task<EditUserProfileDTO?> GetEditUserProfileById(int userId);
        Task CreateUser(AddUserDTO user);
        Task EditUserLastLogin(int userId);
        Task EditUser(EditUserManagementDTO userDto);
        Task EditUserProfile(EditUserProfileDTO userDto);
        Task ActivateUser(int userId);
        Task DeactivateUser(int userId);
        Task UpdateUserPassword(EditResetOrChangePasswordDTO dto);
        Task ChangeUserType(EditChangeUserTypeDTO dto);
        Task ChangeTheme(int userId, string theme);
        Task ChangeLanguage(int userId, string language);

    }
}