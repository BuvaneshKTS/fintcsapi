using System.ComponentModel.DataAnnotations;

namespace FintcsApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Basic info
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "user";

        // Extra details
        [StringLength(50)]
        public string EDPNo { get; set; } = string.Empty;

        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string AddressOffice { get; set; } = string.Empty;

        [StringLength(250)]
        public string AddressResidential { get; set; } = string.Empty;

        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneOffice { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneResidential { get; set; } = string.Empty;

        [StringLength(20)]
        public string Mobile { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
