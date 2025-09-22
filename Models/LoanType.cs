using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FintcsApi.Models
{
    public class LoanType
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Society")]
        public int SocietyId { get; set; }

        [JsonIgnore] // 🔹 prevent circular reference in JSON
        public Society Society { get; set; }

        public string LoanTypeName { get; set; } = "";
        public decimal CompulsoryDeposit { get; set; }
        public decimal OptionalDeposit { get; set; }
        public decimal Share { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal XTimes { get; set; }
    }

    // DTO used for API responses/updates
    public class LoanTypeDto
    {
        public string LoanTypeName { get; set; } = "";
        public decimal CompulsoryDeposit { get; set; }
        public decimal OptionalDeposit { get; set; }
        public decimal Share { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal XTimes { get; set; }
    }
}
