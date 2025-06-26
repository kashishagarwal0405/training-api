/*
 * CONSOLIDATED .NET API TEMPLATE WITH DAPPER
 * 
 * This template provides a complete, reusable structure for building .NET APIs with Dapper.
 * Copy this entire structure to your new API project and modify as needed.
 * 
 * FEATURES:
 * - Dapper for data access (lightweight, fast)
 * - Consolidated SQL queries in one place
 * - Base service class for common operations
 * - Database initialization and seeding
 * - Proper error handling and logging
 * - Clean architecture with separation of concerns
 * 
 * USAGE:
 * 1. Copy the entire Infrastructure/Database folder
 * 2. Copy the Services folder with BaseService
 * 3. Copy the Models folder
 * 4. Copy the Controllers folder
 * 5. Update Program.cs with the service registrations
 * 6. Modify connection string in appsettings.json
 * 7. Customize SQL queries in SqlQueries.cs
 * 8. Add your own models, services, and controllers
 */

// REQUIRED NUGET PACKAGES:
// Microsoft.Data.SqlClient
// Dapper
// Microsoft.AspNetCore.Cors
// Microsoft.Extensions.Configuration
// Microsoft.Extensions.Logging

// FOLDER STRUCTURE:
/*
YourAPI/
├── Infrastructure/
│   └── Database/
│       ├── DatabaseContext.cs          // Dapper connection management
│       ├── DatabaseInitializer.cs      // Table creation and seeding
│       └── SqlQueries.cs               // All SQL queries in one place
├── Models/
│   ├── User.cs                         // Your entity models
│   ├── TrainingRequest.cs
│   └── ...
├── Services/
│   ├── BaseService.cs                  // Common CRUD operations
│   ├── IUserService.cs                 // Service interfaces
│   ├── UserService.cs                  // Service implementations
│   └── ...
├── Controllers/
│   ├── UsersController.cs              // API endpoints
│   └── ...
├── Program.cs                          // Service registration
└── appsettings.json                    // Connection string
*/

// SAMPLE appsettings.json:
/*
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabase;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
*/

// SAMPLE Program.cs:
/*
using YourAPI.Infrastructure.Database;
using YourAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
// Add your other services here

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.Run();
*/

// SAMPLE MODEL:
/*
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = "employee";
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
*/

// SAMPLE SERVICE INTERFACE:
/*
public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}
*/

// SAMPLE SERVICE IMPLEMENTATION:
/*
public class UserService : BaseService, IUserService
{
    public UserService(IDatabaseContext dbContext, ILogger<UserService> logger) 
        : base(dbContext, logger)
    {
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await GetAllAsync<User>(SqlQueries.GetAllUsers);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await GetByIdAsync<User>(SqlQueries.GetUserById, id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var id = await CreateAsync(SqlQueries.CreateUser, new
        {
            user.Name,
            user.Email,
            user.Department,
            user.Role,
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
        var success = await UpdateAsync(SqlQueries.UpdateUser, new
        {
            user.Id,
            user.Name,
            user.Email,
            user.Department,
            user.Role,
            user.IsActive
        });

        if (!success)
            throw new InvalidOperationException("Failed to update user");

        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await DeleteAsync(SqlQueries.DeleteUser, new { Id = id });
    }
}
*/

// SAMPLE CONTROLLER:
/*
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        try
        {
            if (id != user.Id)
                return BadRequest();

            var updatedUser = await _userService.UpdateUserAsync(user);
            return Ok(updatedUser);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
*/

// KEY BENEFITS OF THIS TEMPLATE:
// 1. EASY TO COPY: All code is in separate files that can be copied directly
// 2. REUSABLE: BaseService provides common CRUD operations
// 3. MAINTAINABLE: SQL queries are centralized in SqlQueries.cs
// 4. SCALABLE: Easy to add new models, services, and controllers
// 5. PERFORMANT: Dapper is lightweight and fast
// 6. RELIABLE: Proper error handling and logging throughout
// 7. CLEAN: Separation of concerns with clear interfaces
// 8. FLEXIBLE: Easy to modify for different database types

// QUICK START STEPS:
// 1. Create new .NET Web API project
// 2. Install required NuGet packages
// 3. Copy Infrastructure/Database folder
// 4. Copy Services folder
// 5. Copy Models folder
// 6. Copy Controllers folder
// 7. Update Program.cs
// 8. Update appsettings.json with your connection string
// 9. Run the application - database will be created automatically 