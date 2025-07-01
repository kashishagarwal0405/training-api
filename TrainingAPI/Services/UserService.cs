using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly string _usersFile = "MockData/users.json";
        private readonly string _rolesFile = "MockData/roles.json";
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var roles = await JsonFileHelper.ReadListAsync<Role>(_rolesFile);
            foreach (var user in users)
            {
                user.Role = roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
            }
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var roles = await JsonFileHelper.ReadListAsync<Role>(_rolesFile);
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
                user.Role = roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
                var roles = await JsonFileHelper.ReadListAsync<Role>(_rolesFile);
                var user = users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                    user.Role = roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var roles = await JsonFileHelper.ReadListAsync<Role>(_rolesFile);
            var role = roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null) return new List<User>();
            var filtered = users.Where(u => u.RoleId == role.Id && u.IsActive).ToList();
            foreach (var user in filtered)
                user.Role = role.Name;
            return filtered;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            users.Add(user);
            await JsonFileHelper.WriteListAsync(_usersFile, users);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var idx = users.FindIndex(u => u.Id == user.Id);
            if (idx == -1) throw new ArgumentException("User not found");
            users[idx] = user;
            await JsonFileHelper.WriteListAsync(_usersFile, users);
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            users.Remove(user);
            await JsonFileHelper.WriteListAsync(_usersFile, users);
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            user.IsActive = false;
            await JsonFileHelper.WriteListAsync(_usersFile, users);
            return true;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await JsonFileHelper.ReadListAsync<Role>(_rolesFile);
        }
    }
} 