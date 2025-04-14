using EndavaGrowthspace.Enums;

namespace EndavaGrowthspace.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; } //user Id
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public CourseDiscipline Discipline { get; set; }
        public string Description { get; set; } = string.Empty;
        public CourseDifficulty Difficulty {  get; set; }
        public List<Guid> Contributors { get; set; } = new();
        public List<CourseModules> Modules { get; set; } = new();
        public List<Guid> Enrollments { get; set; } = new();
        

        public class CourseModules
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public int Order { get; set; }
        }

    }
}
