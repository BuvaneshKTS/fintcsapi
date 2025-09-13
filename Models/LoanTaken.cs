// Models/LoanTaken.cs
using System.ComponentModel.DataAnnotations;

namespace FintcsApi.Models
{
    public class LoanTaken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LoanNo { get; set; } = string.Empty;

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public string LoanType { get; set; } = string.Empty;

        public string? CustomType { get; set; }

        [Required]
        public string MemberNo { get; set; } = string.Empty;

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

        // Auto-calculated fields
        public decimal NetLoan { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal NewLoanShare { get; set; }
        public decimal PayAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Always UTC
    }

    // ---------------- DTOs ----------------
    // DTOs/LoanTakenCreateDto.cs
public class LoanTakenCreateDto
{
    public string? LoanNo { get; set; }
    public DateTime LoanDate { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public string? CustomType { get; set; }
    public string MemberNo { get; set; } = string.Empty;
    public decimal LoanAmount { get; set; }
    public decimal PreviousLoan { get; set; }
    public int Installments { get; set; }
    public string? Purpose { get; set; } = string.Empty;
    public string? AuthorizedBy { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = "Cash";
    public string? Bank { get; set; }
    public string? ChequeNo { get; set; }
    public DateTime? ChequeDate { get; set; }

    // ✅ Frontend-calculated values
    public decimal NetLoan { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal NewLoanShare { get; set; }
    public decimal PayAmount { get; set; }
}


    public class LoanTakenResponseDto
{
    public int Id { get; set; }
    public string LoanNo { get; set; }
    public DateTime LoanDate { get; set; }
    public string LoanType { get; set; }
    public string? CustomType { get; set; }
    public string MemberNo { get; set; }
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

    // Auto-calculated
    public decimal NetLoan { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal NewLoanShare { get; set; }
    public decimal PayAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}

}
