namespace TrainingAPI.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Expertise { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 