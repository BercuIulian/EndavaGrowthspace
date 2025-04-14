using EndavaGrowthspace.Enums;

namespace EndavaGrowthspace.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public CourseDiscipline Discipline { get; set; }
        public string Description { get; set; } = string.Empty;
        public CourseDifficulty Difficulty { get; set; }
    }
}
