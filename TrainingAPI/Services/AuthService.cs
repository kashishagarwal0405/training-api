using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class AuthService : IAuthService
    {
        // Hardcoded demo users
        private static readonly List<User> DemoUsers = new()
        {
            new User { Id = 1, Name = "John Employee", Email = "employee@demo.com", PasswordHash = "password123", Role = "employee" },
            new User { Id = 2, Name = "Sarah L&D Manager", Email = "ld@demo.com", PasswordHash = "password123", Role = "ld" },
            new User { Id = 3, Name = "Admin User", Email = "admin@demo.com", PasswordHash = "password123", Role = "admin" }
        };

        public Task<User?> AuthenticateAsync(string email, string password, string role)
        {
            var user = DemoUsers.FirstOrDefault(u =>
                u.Email == email && u.PasswordHash == password && u.Role == role);

            return Task.FromResult(user);
        }
    }
}