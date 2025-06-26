using System.ComponentModel.DataAnnotations;

namespace TrainingAPI.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Trainer { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "scheduled"; // scheduled, in-progress, completed, cancelled
        
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<TrainingParticipant> Participants { get; set; } = new List<TrainingParticipant>();
        public ICollection<TrainingRequest> TrainingRequests { get; set; } = new List<TrainingRequest>();
    }
} 