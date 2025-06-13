using System.ComponentModel.DataAnnotations;

namespace PhoneStoreAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password hash is required.")]
        [MaxLength(255, ErrorMessage = "Password hash cannot exceed 255 characters.")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } // Loại bỏ ?
        public byte? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required(ErrorMessage = "You must agree to the terms.")]
        public bool IsAgree { get; set; }
        [MaxLength(50, ErrorMessage = "Photo path cannot exceed 50 characters.")]
        public string? Photo { get; set; }
        public bool Activated { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

