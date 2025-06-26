namespace TrainingAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 