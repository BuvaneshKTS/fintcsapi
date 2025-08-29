// Models/DTOs.cs
using System.ComponentModel.DataAnnotations;

namespace FintcsApi.Models
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        // Extended user details
        public string EDPNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AddressOffice { get; set; } = string.Empty;
        public string AddressResidential { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string PhoneOffice { get; set; } = string.Empty;
        public string PhoneResidential { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Roles { get; set; } = "user";
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
        public DateTime CreatedAt { get; set; }
    }

    public class UserDetailsDto
    {
        public string EDPNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AddressOffice { get; set; } = string.Empty;
        public string AddressResidential { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string PhoneOffice { get; set; } = string.Empty;
        public string PhoneResidential { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Roles { get; set; } = "user";
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
        public DateTime ExpiresAt { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}
