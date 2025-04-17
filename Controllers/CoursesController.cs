using EndavaGrowthspace.Data;
using EndavaGrowthspace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EndavaGrowthspace.DTOs;
using Microsoft.AspNetCore.Authorization;
using EndavaGrowthspace.Constants;

namespace EndavaGrowthspace.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public CourseController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet] // get all courses
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Courses.ToListAsync();
        }

        [HttpGet("{id}")] // get course by id
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if(course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpPost] // add a course
        public async Task<ActionResult<Course>> CreateCourse(CreateCourseDto courseDto)
        {
            var user = await _context.Users.FindAsync(courseDto.CreatedBy);
            if(user == null)
            {
                return BadRequest("User not found :(");
            }

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = courseDto.Title,
                CreatedBy = courseDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Discipline = courseDto.Discipline,
                Description = courseDto.Description,
                Difficulty = courseDto.Difficulty,
                Contributors = new List<Guid> { courseDto.CreatedBy },
                Modules = new List<Course.CourseModules>(),
                Enrollments = new List<Guid>()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new {id = course.Id}, course);
        }

        [HttpPut("{id}")] // update course
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            bool isAdmin = User.IsInRole(AuthorizationConstants.Roles.Admin) || User.IsInRole(AuthorizationConstants.Roles.Administrator);

            if (!isAdmin && course.CreatedBy != courseDto.UserId)
            {
                return Forbid("Only the creator or the admin can update the course :)");
            }

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            course.Discipline = courseDto.Discipline;
            course.Difficulty = courseDto.Difficulty;
            course.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")] // delete a course
        public async Task<IActionResult> DeleteCourses(Guid id, [FromQuery] Guid userId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            bool isAdmin = User.IsInRole(AuthorizationConstants.Roles.Admin) || User.IsInRole(AuthorizationConstants.Roles.Administrator);

            if (!isAdmin && course.CreatedBy != userId)
            {
                return Forbid("Only the creator or the admin can delete a course ;)");
            }

            var modules = await _context.Modules.Where(m => m.CourseId == id).ToListAsync();
            _context.Modules.RemoveRange(modules);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> EnrollInCourse(Guid id, EnrollmentDto enrollmentDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(enrollmentDto.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (course.Enrollments == null)
            {
                course.Enrollments = new List<Guid>();
            }

            if (course.Enrollments.Contains(enrollmentDto.UserId))
            {
                return BadRequest("User is already enrolled in this course");
            }

            var updatedEnrollments = course.Enrollments.ToList();
            updatedEnrollments.Add(enrollmentDto.UserId);
            course.Enrollments = updatedEnrollments;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e=> e.Id == id);
        }
    }
}