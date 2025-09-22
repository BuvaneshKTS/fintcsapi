using System.ComponentModel.DataAnnotations;

namespace FintcsApi.Models
{
    public class Society
    {
        [Key]
        public int Id { get; set; }

        public string SocietyName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Email { get; set; } = "";
        public string Website { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public string chBounceCharge { get; set; } = "";
        public string targetDropdown { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Navigation property
        public ICollection<LoanType> LoanTypes { get; set; } = new List<LoanType>();
    }

    // DTO for updating society
    public class SocietyUpdateDto
    {
        public int Id { get; set; }
        public string SocietyName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Email { get; set; } = "";
        public string Website { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public string chBounceCharge { get; set; } = "";
        public string targetDropdown { get; set; } = "";

        public List<LoanTypeDto> LoanTypes { get; set; } = new();
    }

    // DTO for reading society (GET response)
    public class SocietyDto
    {
        public int Id { get; set; }
        public string SocietyName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Email { get; set; } = "";
        public string Website { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public string chBounceCharge { get; set; } = "";
        public string targetDropdown { get; set; } = "";

        public List<LoanTypeDto> LoanTypes { get; set; } = new();
    }
}
