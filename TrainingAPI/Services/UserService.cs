using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        private readonly ILogger<UserService> _logger;

        public UserService(IConfiguration configuration, ILogger<UserService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT u.Id, u.Name, u.Email, u.Department, u.RoleId, u.CreatedAt, u.IsActive,
                       r.Id as Role_Id, r.Name as Role_Name
                FROM Users u 
                INNER JOIN Roles r ON u.RoleId = r.Id 
                ORDER BY u.Name";
            return await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
            {
                user.Role = role;
                return user;
            }, splitOn: "Role_Id");
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT u.Id, u.Name, u.Email, u.Department, u.RoleId, u.CreatedAt, u.IsActive,
                       r.Id as Role_Id, r.Name as Role_Name
                FROM Users u 
                INNER JOIN Roles r ON u.RoleId = r.Id 
                WHERE u.Id = @Id";
            var result = await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
            {
                user.Role = role;
                return user;
            }, new { Id = id }, splitOn: "Role_Id");
            return result.FirstOrDefault();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT u.Id, u.Name, u.Email, u.Department, u.RoleId, u.CreatedAt, u.IsActive,
                           r.Id as Role_Id, r.Name as Role_Name
                    FROM Users u 
                    INNER JOIN Roles r ON u.RoleId = r.Id 
                    WHERE u.Email = @Email";
                var result = await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
                {
                    user.Role = role;
                    return user;
                }, new { Email = email }, splitOn: "Role_Id");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT u.Id, u.Name, u.Email, u.Department, u.RoleId, u.CreatedAt, u.IsActive,
                       r.Id as Role_Id, r.Name as Role_Name
                FROM Users u 
                INNER JOIN Roles r ON u.RoleId = r.Id 
                WHERE r.Name = @RoleName AND u.IsActive = 1 
                ORDER BY u.Name";
            return await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
            {
                user.Role = role;
                return user;
            }, new { RoleName = roleName }, splitOn: "Role_Id");
        }

        public async Task<User> CreateUserAsync(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                INSERT INTO Users (Name, Email, Department, RoleId, CreatedAt, IsActive) 
                VALUES (@Name, @Email, @Department, @RoleId, @CreatedAt, @IsActive);
                SELECT LAST_INSERT_ID()";

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                user.Name,
                user.Email,
                user.Department,
                user.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            user.Id = id;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                UPDATE Users 
                SET Name = @Name, Email = @Email, Department = @Department, RoleId = @RoleId, IsActive = @IsActive 
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Department,
                user.RoleId,
                user.IsActive
            });

            if (rowsAffected == 0)
                throw new ArgumentException("User not found");

            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM Users WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "UPDATE Users SET IsActive = 0 WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        // New method to get all roles
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT Id, Name FROM Roles ORDER BY Name";
            return await connection.QueryAsync<Role>(sql);
        }
    }
} 