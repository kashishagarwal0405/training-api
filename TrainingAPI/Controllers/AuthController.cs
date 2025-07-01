using Microsoft.AspNetCore.Mvc;
using TrainingAPI.Services;

namespace TrainingAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
                return BadRequest(new { message = "Email, password, and role are required" });

            var user = await _authService.AuthenticateAsync(request.Email, request.Password, request.Role);
            if (user == null)
                return Unauthorized(new { message = "Invalid email, password, or role" });

            // For demo, just return user info (no JWT)
            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = user.Role
                },
                token = "demo-token"
            });
        }

        public class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "";
        }
    }
} 