using Microsoft.AspNetCore.Mvc;
using TrainingAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TrainingAPI.Services;

namespace TrainingAPI.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly string _coursesFile = "MockData/courses.json";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourseById(int id)
        {
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            course.Id = courses.Any() ? courses.Max(c => c.Id) + 1 : 1;
            course.IsActive = true;
            courses.Add(course);
            await JsonFileHelper.WriteListAsync(_coursesFile, courses);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Course>> UpdateCourse(int id, Course course)
        {
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            var idx = courses.FindIndex(c => c.Id == id);
            if (idx == -1) return NotFound();
            course.Id = id;
            courses[idx] = course;
            await JsonFileHelper.WriteListAsync(_coursesFile, courses);
            return Ok(course);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var courses = await JsonFileHelper.ReadListAsync<Course>(_coursesFile);
            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            courses.Remove(course);
            await JsonFileHelper.WriteListAsync(_coursesFile, courses);
            return NoContent();
        }
    }
}
