// Models/LoanTaken.cs
using System.ComponentModel.DataAnnotations;

namespace FintcsApi.Models
{
    public class LoanTaken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public string LoanType { get; set; } = string.Empty;

        [Required]
        public int MemberId { get; set; }  // 🔄 Changed from MemberNo to MemberId

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public decimal PreviousLoan { get; set; }

        [Required]
        public int Installments { get; set; }

        public string Purpose { get; set; } = string.Empty;
        public string AuthorizedBy { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = "Cash";

        public string? Bank { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? Status { get; set; }

        [Required]
        public decimal NetLoan { get; set; }

        [Required]
        public decimal InstallmentAmount { get; set; }

        [Required]
        public decimal NewLoanShare { get; set; }

        [Required]
        public decimal PayAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTOs
    public class LoanTakenCreateDto
    {
        public DateTime LoanDate { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public int MemberId { get; set; }   // 🔄 Changed
        public string? MemberNo { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal PreviousLoan { get; set; }
        public int Installments { get; set; }
        public string? Purpose { get; set; } = string.Empty;
        public string? AuthorizedBy { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = "Cash";
        public string? Bank { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }

        public decimal NetLoan { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal NewLoanShare { get; set; }
        public decimal PayAmount { get; set; }
    }

    public class LoanTakenResponseDto
    {
        public int Id { get; set; }
        public DateTime LoanDate { get; set; }
        public string LoanType { get; set; }
        public int MemberId { get; set; }   // 🔄 Changed
        public decimal LoanAmount { get; set; }
        public decimal PreviousLoan { get; set; }
        public int Installments { get; set; }
        public string Purpose { get; set; }
        public string AuthorizedBy { get; set; }
        public string PaymentMode { get; set; }
        public string? Bank { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? Status { get; set; }
        public decimal NetLoan { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal NewLoanShare { get; set; }
        public decimal PayAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
