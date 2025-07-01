using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password, string role);
    }
}
