using System.ComponentModel.DataAnnotations;

namespace TrainingAPI.Models
{
    public class TrainingRequest
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string TrainingType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "pending"; // pending, approved, rejected, completed
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Foreign keys
        public int RequesterId { get; set; }
        public int? TrainingSessionId { get; set; }
        
        // Navigation properties
        public User? Requester { get; set; }
        public TrainingSession? TrainingSession { get; set; }
    }
} 