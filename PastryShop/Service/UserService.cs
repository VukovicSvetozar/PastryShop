using System.Text;
using System.Security.Cryptography;
using PastryShop.Data.DTO;
using PastryShop.Data.DAO;
using PastryShop.Enum;
using PastryShop.Model;
using PastryShop.Utility;

namespace PastryShop.Service
{
    public class UserService(IUserDao userDao) : IUserService
    {
        private readonly IUserDao _userDao = userDao;

        public async Task<LoginAuthenticatedUserDTO?> AuthenticateUser(LoginCredentialUserDTO loginCredentialUserDTO)
        {
            var user = await _userDao.GetUserByUsername(loginCredentialUserDTO.Username);

            if (user != null && VerifyPassword(loginCredentialUserDTO.Password, user.Password))
            {
                return new LoginAuthenticatedUserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    ImagePath = user.ImagePath,
                    UserType = user.UserType,
                    Theme = user.Theme,
                    Language = user.Language,
                    IsActive = user.IsActive,
                    ForcePasswordChange = user.ForcePasswordChange
                };
            }

            return null;
        }

        public async Task<bool> UsernameExists(string username)
        {
            return await _userDao.UsernameExists(username);
        }

        public async Task<List<InfoUserBasicDTO>> GetAllUsersBasicInfo()
        {
            var users = await _userDao.GetAllUsers();

            var basicInfoUserDTOs = users.Select(user => new InfoUserBasicDTO
            {
                Id = user.Id,
                Username = user.Username,
                UserType = user.UserType,
                IsActive = user.IsActive,
            }).ToList();

            return basicInfoUserDTOs;
        }

        public async Task<InfoUserDetailsDTO?> GetUserDetailsById(int userId)
        {
            var user = await _userDao.GetUserById(userId);

            if (user == null)
                return null;

            var dto = new InfoUserDetailsDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImagePath = user.ImagePath,
                HireDate = user.HireDate,
                Salary = user.Salary,
                LastLogin = user.LastLogin,
                UserType = user.UserType
            };

            if (user is Manager manager)
            {
                dto.Department = manager.Department;
            }

            if (user is Cashier cashier)
            {
                dto.CashRegisterId = cashier.CashRegisterId;
                dto.ShiftStart = cashier.ShiftStart;
                dto.ShiftEnd = cashier.ShiftEnd;
            }

            return dto;
        }

        public async Task<EditUserManagementDTO?> GetEditUserById(int userId)
        {
            var user = await _userDao.GetUserById(userId);

            if (user == null)
                return null;

            var dto = new EditUserManagementDTO
            {
                Id = user.Id,
                Username = user.Username,
                UserType = user.UserType,
                Salary = user.Salary
            };

            if (user is Manager manager)
            {
                dto.Department = manager.Department;
            }

            if (user is Cashier cashier)
            {
                dto.CashRegisterId = cashier.CashRegisterId;
                dto.ShiftStart = cashier.ShiftStart;
                dto.ShiftEnd = cashier.ShiftEnd;
            }

            return dto;
        }

        public async Task<EditUserProfileDTO?> GetEditUserProfileById(int userId)
        {
            var user = await _userDao.GetUserById(userId);

            if (user == null)
                return null;

            var dto = new EditUserProfileDTO
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImagePath = user.ImagePath
            };
            return dto;
        }

        public async Task CreateUser(AddUserDTO userDto)
        {
            User user;

            if (userDto is AddManagerDTO managerDto)
            {
                user = new Manager
                {
                    Username = managerDto.Username,
                    Password = UserService.HashPassword(managerDto.Password),
                    FirstName = managerDto.FirstName,
                    LastName = managerDto.LastName,
                    PhoneNumber = managerDto.PhoneNumber,
                    Address = managerDto.Address,
                    ImagePath = managerDto.ImagePath,
                    Salary = managerDto.Salary,
                    UserType = UserType.Manager,
                    Department = managerDto.Department
                };
            }
            else if (userDto is AddCashierDTO cashierDto)
            {
                user = new Cashier
                {
                    Username = cashierDto.Username,
                    Password = UserService.HashPassword(cashierDto.Password),
                    FirstName = cashierDto.FirstName,
                    LastName = cashierDto.LastName,
                    PhoneNumber = cashierDto.PhoneNumber,
                    Address = cashierDto.Address,
                    ImagePath = cashierDto.ImagePath,
                    Salary = cashierDto.Salary,
                    UserType = UserType.Cashier,
                    CashRegisterId = cashierDto.CashRegisterId,
                    ShiftStart = cashierDto.ShiftStart,
                    ShiftEnd = cashierDto.ShiftEnd
                };
            }
            else
            {
                Logger.LogError("Nepoznat tip korisnika.", new ArgumentException("Nepoznat tip korisnika"));
                return;
            }

            user.HireDate = DateOnly.FromDateTime(DateTime.Now);
            user.LastLogin ??= null;
            user.Theme = Theme.Light;
            user.Language = Language.Serbian;
            user.IsActive = true;
            user.ForcePasswordChange = false;

            await _userDao.AddUser(user);
        }

        public async Task EditUserLastLogin(int userId)
        {
            await _userDao.UpdateUserLastLogin(userId);
        }

        public async Task EditUser(EditUserManagementDTO userDto)
        {
            var existingUser = await _userDao.GetUserById(userDto.Id);

            if (existingUser == null)
            {
                Logger.LogError($"Korisnik sa ID-jem {userDto.Id} nije pronađen.", new KeyNotFoundException());
                return;
            }

            existingUser.Salary = userDto.Salary;

            if (existingUser is Manager manager)
            {
                if (!string.IsNullOrEmpty(userDto.Department))
                {
                    manager.Department = userDto.Department;
                }
            }
            else if (existingUser is Cashier cashier)
            {
                if (userDto.CashRegisterId.HasValue)
                {
                    cashier.CashRegisterId = userDto.CashRegisterId.Value;
                }
                if (userDto.ShiftStart.HasValue)
                {
                    cashier.ShiftStart = userDto.ShiftStart.Value;
                }
                if (userDto.ShiftEnd.HasValue)
                {
                    cashier.ShiftEnd = userDto.ShiftEnd.Value;
                }
            }
            else
            {
                Logger.LogError("Nepodržan tip korisnika.", new ArgumentException("Nepodržan tip korisnika"));
            }

            await _userDao.UpdateUser(existingUser);
        }

        public async Task EditUserProfile(EditUserProfileDTO userDto)
        {
            var existingUserProfile = await _userDao.GetUserById(userDto.Id) ?? throw new KeyNotFoundException($"Profil korisnika sa ID-jem {userDto.Id} nije pronađen.");

            existingUserProfile.Username = userDto.Username;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                existingUserProfile.Password = HashPassword(userDto.Password);
            }

            existingUserProfile.FirstName = userDto.FirstName;
            existingUserProfile.LastName = userDto.LastName;
            existingUserProfile.PhoneNumber = userDto.PhoneNumber;
            if (!string.IsNullOrEmpty(userDto.Address))
            {
                existingUserProfile.Address = userDto.Address;
            }
            if (!string.IsNullOrEmpty(userDto.ImagePath))
            {
                existingUserProfile.ImagePath = userDto.ImagePath;
            }

            await _userDao.UpdateUserProfile(existingUserProfile);
        }

        public async Task ActivateUser(int userId)
        {
            await _userDao.ActivateUser(userId);
        }

        public async Task DeactivateUser(int userId)
        {
            await _userDao.DeactivateUser(userId);
        }

        public async Task UpdateUserPassword(EditResetOrChangePasswordDTO dto)
        {
            if (dto == null)
            {
                Logger.LogError("EditResetOrChangePasswordDTO je null.", new ArgumentNullException(nameof(dto)));
                return;
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                Logger.LogError("Nova lozinka ne može biti prazna.", new ArgumentException("Nova lozinka ne može biti prazna."));
                return;
            }

            string hashedPassword = HashPassword(dto.NewPassword);

            await _userDao.UpdateUserPassword(dto.UserId, hashedPassword, dto.ForcePasswordChange);
        }

        public async Task ChangeUserType(EditChangeUserTypeDTO dto)
        {
            if (dto == null)
            {
                Logger.LogError("EditChangeUserTypeDTO je null.", new ArgumentNullException(nameof(dto)));
                return;
            }
            await _userDao.UpdateUserType(
                dto.UserId,
                dto.NewUserType,
                dto.Department,
                dto.CashRegisterId,
                dto.ShiftStart,
                dto.ShiftEnd
            );
        }

        public async Task ChangeTheme(int userId, string theme)
        {
            await _userDao.UpdateUserTheme(userId, theme);

            var currentUser = UserSession.GetCurrentUser();
            currentUser.Theme = System.Enum.Parse<Theme>(theme);
            UserSession.SetCurrentUser(currentUser);
        }

        public async Task ChangeLanguage(int userId, string language)
        {
            await _userDao.UpdateUserLanguage(userId, language);

            var currentUser = UserSession.GetCurrentUser();
            currentUser.Language = System.Enum.Parse<Language>(language);
            UserSession.SetCurrentUser(currentUser);
        }

        private static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedHashedPassword;
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

    }
}