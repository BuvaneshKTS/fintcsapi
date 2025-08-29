// Models/Society.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        // JSON string for tabs configuration
        public string Tabs { get; set; } = "{}";
        
        public bool IsPendingApproval { get; set; } = false;
        
        // JSON string to store pending changes until approved
        public string PendingChanges { get; set; } = "{}";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTO classes for the tabs structure
    public class SocietyTabsDto
    {
        public InterestRatesDto Interest { get; set; } = new();
        public LimitsDto Limit { get; set; } = new();
    }

    public class InterestRatesDto
    {
        public decimal Dividend { get; set; }
        public decimal OD { get; set; }
        public decimal CD { get; set; }
        public decimal Loan { get; set; }
        public decimal EmergencyLoan { get; set; }
        public decimal LAS { get; set; }
    }

    public class LimitsDto
    {
        public decimal Share { get; set; }
        public decimal Loan { get; set; }
        public decimal EmergencyLoan { get; set; }
    }

    public class SocietyUpdateDto
    {
        public string SocietyName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Email { get; set; } = "";
        public string Website { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public SocietyTabsDto Tabs { get; set; } = new();
    }
}