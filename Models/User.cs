// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintcsApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Store user details as JSON string (includes email, phone, roles)
        public string Details { get; set; } = "{}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Helper method to get role from Details JSON
        [NotMapped]
        public string Role
        {
            get
            {
                try
                {
                    var details = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(Details);
                    return details?.role ?? "user";
                }
                catch
                {
                    return "user";
                }
            }
        }

        // Helper method to get email from Details JSON
        [NotMapped]
        public string Email
        {
            get
            {
                try
                {
                    var details = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(Details);
                    return details?.email ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        // Helper method to get phone from Details JSON
        [NotMapped]
        public string Phone
        {
            get
            {
                try
                {
                    var details = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(Details);
                    return details?.phone ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

    public class UserDetails
    {
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string role { get; set; } = "user";
        public string EDPNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AddressOffice { get; set; } = string.Empty;
        public string AddressResidential { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string PhoneOffice { get; set; } = string.Empty;
        public string PhoneResidential { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }
}
