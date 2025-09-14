using PastryShop.Data.DTO;

namespace PastryShop.Utility
{
    public class UserSession
    {
        private static readonly object _lock = new();
        private static LoginAuthenticatedUserDTO? _currentUser;
        public static event EventHandler? CurrentUserChanged;

        public static LoginAuthenticatedUserDTO GetCurrentUser()
        {
            lock (_lock)
            {
                if (_currentUser != null)
                    return _currentUser;

                Logger.LogError("Nema prijavljenog korisnika.", new InvalidOperationException("Nema prijavljenog korisnika."));
                return new LoginAuthenticatedUserDTO();
            }
        }

        public static void SetCurrentUser(LoginAuthenticatedUserDTO user)
        {
            lock (_lock)
            {
                if (user == null)
                {
                    Logger.LogError("Korisnik ne može biti null.", new ArgumentNullException(nameof(user)));
                    return;
                }

                _currentUser = user;
                CurrentUserChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool IsUserLoggedIn()
        {
            lock (_lock)
            {
                return _currentUser != null;
            }
        }

        public static void Logout()
        {
            lock (_lock)
            {
                _currentUser = null;
            }
        }

    }
}