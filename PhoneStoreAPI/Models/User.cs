namespace PhoneStoreAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public byte? Gender { get; set; } // 0 = Nam, 1 = Nữ, 2 = Khác
        public DateTime? BirthDate { get; set; }
        public bool IsAgree { get; set; }
        public string? Photo { get; set; }
        public bool Activated { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
