using System;

namespace FintcsApi.Models
{
    public class SocietyApproval
    {
        public int Id { get; set; }

        // Foreign Keys
        public int SocietyId { get; set; }
        public string UserId { get; set; } = string.Empty;  // FK to your Users table (string if IdentityUser)

        // Approval status
        public bool Approved { get; set; }
        public DateTime ApprovedAt { get; set; }

        // Navigation
        public Society Society { get; set; }
    }
}
