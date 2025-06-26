using System.ComponentModel.DataAnnotations;

namespace TrainingAPI.Models
{
    public class TrainingParticipant
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainingSessionId { get; set; }
        public string Status { get; set; } = "registered"; // registered, attended, no-show, cancelled
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? AttendedAt { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        public TrainingSession? TrainingSession { get; set; }
    }
} 