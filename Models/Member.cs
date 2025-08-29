// Models/Member.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintcsApi.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }
        
        // Auto-generated member number (MEM_001, MEM_002, etc.)
        public string MemNo { get; set; } = "";
        
        public string Name { get; set; } = "";
        public string FHName { get; set; } = "";
        public string OfficeAddress { get; set; } = "";
        public string City { get; set; } = "";
        public string PhoneOffice { get; set; } = "";
        public string Branch { get; set; } = "";
        public string PhoneRes { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Designation { get; set; } = "";
        public string ResidenceAddress { get; set; } = "";
        
        public DateTime DOB { get; set; }
        public DateTime DOJSociety { get; set; }
        public string Email { get; set; } = "";
        public DateTime DOJOrg { get; set; }
        public DateTime? DOR { get; set; } // Nullable for active members
        
        public string Nominee { get; set; } = "";
        public string NomineeRelation { get; set; } = "";
        
        // JSON string for banking details
        public string BankingDetails { get; set; } = "{}";
        
        public bool IsPendingApproval { get; set; } = false;
        
        // JSON string to store pending changes until approved
        public string PendingChanges { get; set; } = "{}";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTO classes for banking details
    public class BankingDetailsDto
    {
        public string BankName { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string IFSCCode { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string AccountHolderName { get; set; } = "";
    }

    public class MemberCreateDto
    {
        [Required]
        public string Name { get; set; } = "";
        public string FHName { get; set; } = "";
        public string OfficeAddress { get; set; } = "";
        public string City { get; set; } = "";
        public string PhoneOffice { get; set; } = "";
        public string Branch { get; set; } = "";
        public string PhoneRes { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Designation { get; set; } = "";
        public string ResidenceAddress { get; set; } = "";
        
        public DateTime DOB { get; set; }
        public DateTime DOJSociety { get; set; }
        public string Email { get; set; } = "";
        public DateTime DOJOrg { get; set; }
        public DateTime? DOR { get; set; }
        
        public string Nominee { get; set; } = "";
        public string NomineeRelation { get; set; } = "";
        public BankingDetailsDto BankingDetails { get; set; } = new();
    }

    public class MemberUpdateDto
    {
        public string Name { get; set; } = "";
        public string FHName { get; set; } = "";
        public string OfficeAddress { get; set; } = "";
        public string City { get; set; } = "";
        public string PhoneOffice { get; set; } = "";
        public string Branch { get; set; } = "";
        public string PhoneRes { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Designation { get; set; } = "";
        public string ResidenceAddress { get; set; } = "";
        
        public DateTime DOB { get; set; }
        public DateTime DOJSociety { get; set; }
        public string Email { get; set; } = "";
        public DateTime DOJOrg { get; set; }
        public DateTime? DOR { get; set; }
        
        public string Nominee { get; set; } = "";
        public string NomineeRelation { get; set; } = "";
        public BankingDetailsDto BankingDetails { get; set; } = new();
    }

    public class MemberResponseDto
    {
        public int Id { get; set; }
        public string MemNo { get; set; } = "";
        public string Name { get; set; } = "";
        public string FHName { get; set; } = "";
        public string OfficeAddress { get; set; } = "";
        public string City { get; set; } = "";
        public string PhoneOffice { get; set; } = "";
        public string Branch { get; set; } = "";
        public string PhoneRes { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Designation { get; set; } = "";
        public string ResidenceAddress { get; set; } = "";
        public DateTime DOB { get; set; }
        public DateTime DOJSociety { get; set; }
        public string Email { get; set; } = "";
        public DateTime DOJOrg { get; set; }
        public DateTime? DOR { get; set; }
        public string Nominee { get; set; } = "";
        public string NomineeRelation { get; set; } = "";
        public BankingDetailsDto BankingDetails { get; set; } = new();
        public bool IsPendingApproval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}