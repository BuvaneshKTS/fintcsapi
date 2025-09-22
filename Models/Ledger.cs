// Models/Ledger.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintcsApi.Models
{
    public class Ledger
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // If linked to a member
        public int? MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        public string Under { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public string BalanceType { get; set; } = "Dr"; // Dr / Cr
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
