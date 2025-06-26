namespace TrainingAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int TrainingSessionId { get; set; }
        public int UserId { get; set; }
        public DateTime? AttendedAt { get; set; }
        public string Status { get; set; } = "absent"; // present, absent
        
        // Navigation properties
        public TrainingSession? TrainingSession { get; set; }
        public User? User { get; set; }
    }
} 