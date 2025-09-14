using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using PastryShop.Enum;
using PastryShop.Model;
using PastryShop.Utility;

namespace PastryShop.Data.DAO
{
    public class UserDao(string connectionString) : IUserDao
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<bool> UsernameExists(string username)
        {
            const string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom provere postojanja korisničkog imena: {username}", ex);
                return false;
            }
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            const string query = @"
                SELECT u.*, 
                    m.Department, 
                    c.CashRegisterId, c.ShiftStart, c.ShiftEnd
                FROM Users u
                LEFT JOIN Managers m ON u.Id = m.UserId
                LEFT JOIN Cashiers c ON u.Id = c.UserId
                WHERE u.Username = @Username";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Username", username);

                await using var reader = await command.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja korisnika koristeći korisničko ime: {username}", ex);
                return null;
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            const string query = @"
                SELECT u.*, 
                       m.Department, 
                       c.CashRegisterId, c.ShiftStart, c.ShiftEnd
                FROM Users u
                LEFT JOIN Managers m ON u.Id = m.UserId
                LEFT JOIN Cashiers c ON u.Id = c.UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);
                await using var reader = await command.ExecuteReaderAsync();

                var users = await MapReaderToUsers(reader);
                return users;
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja svih korisnika iz baze podataka.", ex);
                return [];
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            const string query = @"
                SELECT u.*, 
                    m.Department, 
                    c.CashRegisterId, c.ShiftStart, c.ShiftEnd
                FROM Users u
                LEFT JOIN Managers m ON u.Id = m.UserId
                LEFT JOIN Cashiers c ON u.Id = c.UserId
                WHERE u.Id = @Id";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Id", userId);

                await using var reader = await command.ExecuteReaderAsync();

                return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja korisnika koristeći ID: {userId}", ex);
                return null;
            }
        }

        public async Task AddUser(User user)
        {
            try
            {
                await using var connection = await OpenConnectionAsync();
                const string userQuery = @"
                    INSERT INTO Users 
                    (Username, Password, FirstName, LastName, PhoneNumber, Address, ImagePath, HireDate, Salary, LastLogin, UserType, Theme, Language, IsActive, ForcePasswordChange)
                    VALUES 
                    (@Username, @Password, @FirstName, @LastName, @PhoneNumber, @Address, @ImagePath, @HireDate, @Salary, @LastLogin, @UserType, @Theme, @Language, @IsActive, @ForcePasswordChange);
                    SELECT LAST_INSERT_ID();";

                await using var userCommand = new MySqlCommand(userQuery, connection);

                user.HireDate = DateOnly.FromDateTime(DateTime.Now);
                user.LastLogin ??= null;
                user.Theme = Theme.Light;
                user.Language = Language.Serbian;
                user.IsActive = true;
                user.ForcePasswordChange = false;

                BindUserToCommand(userCommand, user);
                var userId = Convert.ToInt32(await userCommand.ExecuteScalarAsync());

                if (user is Manager manager)
                {
                    await AddManager(manager, userId, connection);
                }
                else if (user is Cashier cashier)
                {
                    await AddCashier(cashier, userId, connection);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dodavanja korisnika {user.Username}.", ex);
            }
        }

        private static async Task AddManager(Manager manager, int userId, MySqlConnection connection)
        {
            const string query = @"
                INSERT INTO Managers (UserId, Department)
                VALUES (@UserId, @Department);";

            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Department", manager.Department);

            await command.ExecuteNonQueryAsync();
        }

        private static async Task AddCashier(Cashier cashier, int userId, MySqlConnection connection)
        {
            const string query = @"
                INSERT INTO Cashiers 
                (UserId, CashRegisterId, ShiftStart, ShiftEnd)
                VALUES 
                (@UserId, @CashRegisterId, @ShiftStart, @ShiftEnd);";

            await using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CashRegisterId", cashier.CashRegisterId);
            command.Parameters.AddWithValue("@ShiftStart", cashier.ShiftStart.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@ShiftEnd", cashier.ShiftEnd.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateUserLastLogin(int userId)
        {
            const string query = @"
                UPDATE Users
                SET LastLogin = NOW()
                WHERE Id = @UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja vremena poslednje prijave korisnika sa ID: {userId}", ex);
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var transaction = await connection.BeginTransactionAsync();

                const string userQuery = @"
                    UPDATE Users
                    SET Salary = @Salary
                    WHERE Id = @Id";

                await using var userCommand = new MySqlCommand(userQuery, connection, (MySqlTransaction)transaction);
                userCommand.Parameters.AddWithValue("@Id", user.Id);
                userCommand.Parameters.AddWithValue("@Salary", user.Salary);

                await userCommand.ExecuteNonQueryAsync();

                if (user is Manager manager)
                {
                    const string managerQuery = @"
                        UPDATE Managers
                        SET Department = @Department
                        WHERE UserId = @Id";

                    await using var managerCommand = new MySqlCommand(managerQuery, connection, (MySqlTransaction)transaction);
                    managerCommand.Parameters.AddWithValue("@Id", manager.Id);
                    managerCommand.Parameters.AddWithValue("@Department", manager.Department);

                    await managerCommand.ExecuteNonQueryAsync();
                }
                else if (user is Cashier cashier)
                {
                    const string cashierQuery = @"
                        UPDATE Cashiers
                        SET CashRegisterId = @CashRegisterId, 
                            ShiftStart = @ShiftStart, 
                            ShiftEnd = @ShiftEnd
                        WHERE UserId = @Id";

                    await using var cashierCommand = new MySqlCommand(cashierQuery, connection, (MySqlTransaction)transaction);
                    cashierCommand.Parameters.AddWithValue("@Id", cashier.Id);
                    cashierCommand.Parameters.AddWithValue("@CashRegisterId", cashier.CashRegisterId);
                    cashierCommand.Parameters.AddWithValue("@ShiftStart", cashier.ShiftStart.ToString("yyyy-MM-dd HH:mm:ss"));
                    cashierCommand.Parameters.AddWithValue("@ShiftEnd", cashier.ShiftEnd.ToString("yyyy-MM-dd HH:mm:ss"));

                    await cashierCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja korisnika ID: {user.Id}", ex);
            }
        }

        public async Task ActivateUser(int userId)
        {
            const string query = @"
                UPDATE Users
                SET IsActive = TRUE
                WHERE Id = @UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom aktivacije korisnika sa ID: {userId}", ex);
            }
        }

        public async Task DeactivateUser(int userId)
        {
            const string query = @"
                UPDATE Users
                SET IsActive = FALSE
                WHERE Id = @UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom deaktivacije korisnika sa ID: {userId}", ex);
            }
        }

        public async Task UpdateUserPassword(int userId, string hashedPassword, bool forcePasswordChange)
        {
            const string query = @"
                UPDATE Users
                SET Password = @Password,
                    ForcePasswordChange = @ForcePasswordChange
                WHERE Id = @UserId";
            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@ForcePasswordChange", forcePasswordChange);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom resetovanja korisnika sa ID: {userId}", ex);
            }
        }

        public async Task UpdateUserType(int userId, UserType newUserType, string? department = null, int? cashRegisterId = null, DateTime? shiftStart = null, DateTime? shiftEnd = null)
        {
            using var connection = await OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string updateUserQuery = @"
                    UPDATE Users
                    SET UserType = @UserType
                    WHERE Id = @UserId";

                using var updateUserCommand = new MySqlCommand(updateUserQuery, connection, transaction);
                updateUserCommand.Parameters.AddWithValue("@UserId", userId);
                updateUserCommand.Parameters.AddWithValue("@UserType", newUserType.ToString());
                await updateUserCommand.ExecuteNonQueryAsync();

                if (newUserType != UserType.Manager)
                {
                    const string deleteManagerQuery = @"
                        DELETE FROM Managers
                        WHERE UserId = @UserId";

                    using var deleteManagerCommand = new MySqlCommand(deleteManagerQuery, connection, transaction);
                    deleteManagerCommand.Parameters.AddWithValue("@UserId", userId);
                    await deleteManagerCommand.ExecuteNonQueryAsync();
                }

                if (newUserType != UserType.Cashier)
                {
                    const string deleteCashierQuery = @"
                        DELETE FROM Cashiers
                        WHERE UserId = @UserId";
                    using var deleteCashierCommand = new MySqlCommand(deleteCashierQuery, connection, transaction);
                    deleteCashierCommand.Parameters.AddWithValue("@UserId", userId);
                    await deleteCashierCommand.ExecuteNonQueryAsync();
                }

                if (newUserType == UserType.Manager)
                {
                    const string upsertManagerQuery = @"
                        INSERT INTO Managers (UserId, Department)
                        VALUES (@UserId, @Department)
                        ON DUPLICATE KEY UPDATE
                        Department = @Department";

                    using var upsertManagerCommand = new MySqlCommand(upsertManagerQuery, connection, transaction);
                    upsertManagerCommand.Parameters.AddWithValue("@UserId", userId);
                    upsertManagerCommand.Parameters.AddWithValue("@Department", department ?? string.Empty);
                    await upsertManagerCommand.ExecuteNonQueryAsync();
                }

                if (newUserType == UserType.Cashier)
                {
                    const string upsertCashierQuery = @"
                        INSERT INTO Cashiers (UserId, CashRegisterId, ShiftStart, ShiftEnd)
                        VALUES (@UserId, @CashRegisterId, @ShiftStart, @ShiftEnd)
                        ON DUPLICATE KEY UPDATE
                        CashRegisterId = @CashRegisterId,
                        ShiftStart = @ShiftStart,
                        ShiftEnd = @ShiftEnd";

                    using var upsertCashierCommand = new MySqlCommand(upsertCashierQuery, connection, transaction);
                    upsertCashierCommand.Parameters.AddWithValue("@UserId", userId);
                    upsertCashierCommand.Parameters.AddWithValue("@CashRegisterId", cashRegisterId ?? (object)DBNull.Value);
                    upsertCashierCommand.Parameters.AddWithValue("@ShiftStart", shiftStart ?? (object)DBNull.Value);
                    upsertCashierCommand.Parameters.AddWithValue("@ShiftEnd", shiftEnd ?? (object)DBNull.Value);
                    await upsertCashierCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger.LogError($"Greška prilikom promjene tipa korisnika sa ID: {userId}", ex);
            }
        }

        public async Task UpdateUserProfile(User user)
        {
            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var transaction = await connection.BeginTransactionAsync();

                const string userQuery = @"
                    UPDATE Users
                    SET 
                        Username = @Username,
                        Password = @Password,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        PhoneNumber = @PhoneNumber,
                        Address = @Address,
                        ImagePath = @ImagePath
                    WHERE Id = @Id";

                await using var userCommand = new MySqlCommand(userQuery, connection, (MySqlTransaction)transaction);

                userCommand.Parameters.AddWithValue("@Id", user.Id);
                userCommand.Parameters.AddWithValue("@Username", user.Username);
                userCommand.Parameters.AddWithValue("@Password", user.Password);
                userCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                userCommand.Parameters.AddWithValue("@LastName", user.LastName);
                userCommand.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                userCommand.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                userCommand.Parameters.AddWithValue("@ImagePath", user.ImagePath ?? (object)DBNull.Value);

                await userCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja profila korisnika ID: {user.Id}", ex);
            }
        }

        public async Task UpdateUserTheme(int userId, string theme)
        {
            const string query = @"
                UPDATE Users
                SET Theme = @Theme
                WHERE Id = @UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Theme", theme);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja teme korisnika sa ID: {userId}", ex);
            }
        }

        public async Task UpdateUserLanguage(int userId, string language)
        {
            const string query = @"
                UPDATE Users
                SET Language = @Language
                WHERE Id = @UserId";

            try
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Language", language);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja jezika korisnika sa ID: {userId}", ex);
            }
        }

        private async Task<MySqlConnection> OpenConnectionAsync()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                return connection;
            }
            catch (Exception ex)
            {
                Logger.LogError("Neuspješno povezivanje sa bazom podataka.", ex);
                throw new ApplicationException("Neuspješno povezivanje sa bazom podataka.", ex);
            }
        }

        private static User MapReaderToUser(DbDataReader reader)
        {
            var userType = (UserType)System.Enum.Parse(typeof(UserType), reader.GetString("UserType"));

            if (userType == UserType.Manager)
            {
                var manager = new Manager();
                MapBaseUser(reader, manager);
                manager.UserType = userType;
                manager.Department = reader.GetString("Department");
                return manager;
            }
            else if (userType == UserType.Cashier)
            {
                var cashier = new Cashier();
                MapBaseUser(reader, cashier);
                cashier.UserType = userType;
                cashier.CashRegisterId = reader.GetInt32("CashRegisterId");
                cashier.ShiftStart = reader.GetDateTime("ShiftStart");
                cashier.ShiftEnd = reader.GetDateTime("ShiftEnd");
                return cashier;
            }
            Logger.LogError("Nepoznat tip korisnika.", new ApplicationException("Nepoznat tip korisnika"));
            return null!;
        }

        private static async Task<List<User>> MapReaderToUsers(DbDataReader reader)
        {
            var users = new List<User>();
            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }
            return users;
        }

        private static void MapBaseUser(DbDataReader reader, User user)
        {
            user.Id = reader.GetInt32("Id");
            user.Username = reader.GetString("Username");
            user.Password = reader.GetString("Password");
            user.FirstName = reader.GetString("FirstName");
            user.LastName = reader.GetString("LastName");
            user.PhoneNumber = reader.GetString("PhoneNumber");
            user.Address = reader.IsDBNull("Address") ? null : reader.GetString("Address");
            user.ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath");
            user.HireDate = DateOnly.FromDateTime(reader.GetDateTime("HireDate"));
            user.Salary = reader.GetDecimal("Salary");
            user.LastLogin = reader.IsDBNull("LastLogin") ? null : reader.GetDateTime("LastLogin");
            user.Theme = (Theme)System.Enum.Parse(typeof(Theme), reader.GetString("Theme"));
            user.Language = (Language)System.Enum.Parse(typeof(Language), reader.GetString("Language"));
            user.IsActive = reader.GetBoolean("IsActive");
            user.ForcePasswordChange = reader.GetBoolean("ForcePasswordChange");
        }

        private static void BindUserToCommand(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath", user.ImagePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@HireDate", user.HireDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Salary", user.Salary);
            command.Parameters.AddWithValue("@LastLogin", user.LastLogin ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UserType", user.UserType.ToString());
            command.Parameters.AddWithValue("@Theme", user.Theme.ToString());
            command.Parameters.AddWithValue("@Language", user.Language.ToString());
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@ForcePasswordChange", user.ForcePasswordChange);
        }

    }
}