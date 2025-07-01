using System.ComponentModel.DataAnnotations;

namespace TrainingAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;
        
        public int RoleId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<TrainingRequest> TrainingRequests { get; set; } = new List<TrainingRequest>();
        public string? Role { get; set; }
    }
} 