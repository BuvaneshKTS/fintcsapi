// Models/Voucher.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintcsApi.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoucherType { get; set; } = string.Empty;

        public DateTime Date { get; set; }
        public string Particulars { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        // Links to Ledger
        public int LedgerId { get; set; }
        [ForeignKey("LedgerId")]
        public Ledger Ledger { get; set; } = null!;
    }
}
