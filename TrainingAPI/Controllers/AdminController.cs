using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TrainingAPI.Models;
using TrainingAPI.Services;

namespace TrainingAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly string _usersFile = "MockData/users.json";
        private readonly string _coursesFile = "MockData/courses.json";
        private readonly string _activitiesFile = "MockData/activities.json";

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            // For demo, hardcode system health/configs
            return Ok(new
            {
                TotalUsers = users.Count,
                ActiveCourses = courses.Count(c => c.IsActive),
                SystemHealth = "99.9%",
                Configurations = 12
            });
        }

        [HttpGet("activities")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentActivities()
        {
            var activities = await JsonFileHelper.ReadListAsync<object>(_activitiesFile);
            return Ok(activities.OrderByDescending(a => ((dynamic)a).Timestamp).Take(10));
        }
    }
}
