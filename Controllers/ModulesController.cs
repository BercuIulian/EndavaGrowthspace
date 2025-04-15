using EndavaGrowthspace.Data;
using EndavaGrowthspace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EndavaGrowthspace.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EndavaGrowthspace.Controllers
{
    [Authorize]
    [ApiController]
    [Route ("api/[controller]")]
    public class ModulesController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        public ModulesController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet] // get all modules
        public async Task<ActionResult<IEnumerable<Module>>> GetModules()
        {
            return await _context.Modules.ToListAsync();
        }

        [HttpGet("{id}")] // get module by id
        public async Task<ActionResult<Module>> GetModule(Guid id, [FromQuery] Guid userId)
        {
            var module = await _context.Modules.FindAsync(id);

            if (module == null)
            {
                return NotFound("Module not found");
            }

            var course = await _context.Courses.FindAsync(module.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            bool isCreator = course.CreatedBy == userId || module.CreatedBy == userId;

            bool isEnrolled = course.Enrollments != null && course.Enrollments.Contains(userId);

            if (!isCreator && !isEnrolled)
            {
                return Forbid("User must be enrolled in the course or be the creator to view module details");
            }

            return module;
        }
        
        [HttpGet("Course/{courseId}")] // get course modules
        public async Task<ActionResult<IEnumerable<Module>>> GetModulesByCourse(Guid courseId, [FromQuery] Guid userId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Order)
                .ToListAsync();

            return modules;
        }

        [HttpPost] // add a new module
        public async Task<ActionResult<Module>> CreateModule(CreateModuleDto moduleDto)
        {
            var user = await _context.Users.FindAsync(moduleDto.CreatedBy);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var course = await _context.Courses.FindAsync(moduleDto.CourseId);
            if (course == null)
            {
                return BadRequest("Course not found");
            }

            int highestOrder = 0;
            if (await _context.Modules.AnyAsync(m => m.CourseId == moduleDto.CourseId))
            {
                highestOrder = await _context.Modules
                    .Where(m => m.CourseId == moduleDto.CourseId)
                    .MaxAsync(m => m.Order);
            }

            var module = new Module
            {
                Id = Guid.NewGuid(),
                Title = moduleDto.Title,
                CourseId = moduleDto.CourseId,
                CreatedBy = moduleDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Description = moduleDto.Description,
                Content = moduleDto.Content,
                Order = moduleDto.Order ?? (highestOrder + 1)
            };

            _context.Modules.Add(module);

            course.Modules.Add(new Course.CourseModules
            {
                Id = module.Id,
                Title = module.Title,
                Order = module.Order
            });

            var updatedContributors = course.Contributors.ToList();
            if (!updatedContributors.Contains(moduleDto.CreatedBy))
            {
                updatedContributors.Add(moduleDto.CreatedBy);
                course.Contributors = updatedContributors;              
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModule), new { id = module.Id }, module);
        }

        [HttpPut("{id}")] // update a module
        public async Task<IActionResult> UpdateModule(Guid id, UpdateModuleDto moduleDto)
        {
            var module = await _context.Modules.FindAsync(id);

            if (module == null)
            {
                return NotFound();
            }

            if (module.CreatedBy != moduleDto.UserId)
            {
                return Forbid("Only the creator can update the module");                
            }

            module.Title = moduleDto.Title;
            module.Description = moduleDto.Description;
            module.Content = moduleDto.Content;
            module.UpdatedAt = DateTime.UtcNow;

            if (moduleDto.Order.HasValue)
            {
                module.Order = moduleDto.Order.Value;
            }

            try
            {
                if (module.Title != moduleDto.Title)
                {
                    var course = await _context.Courses.FindAsync(module.CourseId);
                    if (course != null)
                    {
                        var courseModule = course.Modules.FirstOrDefault(m => m.Id == module.Id);
                        if(courseModule != null)
                        {
                            courseModule.Title = moduleDto.Title;

                            if (moduleDto.Order.HasValue)
                            {
                                courseModule.Order = moduleDto.Order.Value;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!ModuleExists(id))
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

        [HttpDelete("{id}")] // delete a module
        public async Task<IActionResult> DeleteModules(Guid id, [FromQuery] Guid userId)
        {
            var module = await _context.Modules.FindAsync(id);
            if(module == null)
            {
                return NotFound();
            }

            if (module.CreatedBy != userId)
            {
                return Forbid("Only the creator can delete a module");
            }

            var course = await _context.Courses.FindAsync(module.CourseId);
            if(course != null)
            {
                course.Modules.RemoveAll(m => m.Id == id);
            }

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool ModuleExists(Guid id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}