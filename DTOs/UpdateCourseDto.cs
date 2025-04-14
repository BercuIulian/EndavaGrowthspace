using EndavaGrowthspace.Enums;

namespace EndavaGrowthspace.DTOs
{
    public class UpdateCourseDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public CourseDiscipline Discipline { get; set; }
        public string Description { get; set; } = string.Empty;
        public CourseDifficulty Difficulty { get; set; }
    }
}
