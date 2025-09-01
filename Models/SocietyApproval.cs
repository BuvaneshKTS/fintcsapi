using System;

namespace FintcsApi.Models
{
    public class SocietyApproval
    {
        public int Id { get; set; }

        // Foreign Keys
        public int SocietyId { get; set; }
        public string UserId { get; set; } = string.Empty;  // FK to Users table (matching existing migration)

        // Approval status
        public bool Approved { get; set; }
        public DateTime ApprovedAt { get; set; }  // Not nullable in existing migration

        // Navigation properties
        public Society Society { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    // DTO for approval status view
    public class ApprovalStatusDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool HasApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Status => HasApproved ? "Approved" : "Pending";
    }

    // DTO for pending changes with approval info
    public class PendingChangesWithApprovalsDto
    {
        public bool HasPendingChanges { get; set; }
        public string PendingChanges { get; set; } = string.Empty;
        public List<ApprovalStatusDto> ApprovalStatus { get; set; } = new();
        public int TotalUsers { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public string ChangeRequestId { get; set; } = string.Empty;
    }
}
