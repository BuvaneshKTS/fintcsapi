// SocietyController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FintcsApi.Data;
using FintcsApi.Models;

namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SocietyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SocietyController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/society (Admin only - only works when table is empty)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateSociety([FromBody] SocietyUpdateDto createDto)
        {
            try
            {
                // Check if society table is empty
                var existingSocietyCount = await _context.Societies.CountAsync();
                
                if (existingSocietyCount > 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Society already exists. Only one society is allowed in the system."
                    });
                }

                // Create new society
                var society = new Society
                {
                    SocietyName = createDto.SocietyName,
                    Address = createDto.Address,
                    City = createDto.City,
                    Phone = createDto.Phone,
                    Fax = createDto.Fax,
                    Email = createDto.Email,
                    Website = createDto.Website,
                    RegistrationNumber = createDto.RegistrationNumber,
                    Tabs = JsonSerializer.Serialize(createDto.Tabs),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsPendingApproval = false,
                    PendingChanges = "{}"
                };

                _context.Societies.Add(society);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Society>
                {
                    Success = true,
                    Data = society,
                    Message = "Society created successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error creating society",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/society
        [HttpGet]
        public async Task<IActionResult> GetSociety()
        {
            try
            {
                var society = await _context.Societies.FirstOrDefaultAsync();
                
                // If no society exists, return default values
                if (society == null)
                {
                    return Ok(new ApiResponse<Society>
                    {
                        Success = true,
                        Data = new Society(),
                        Message = "No society configuration found. Using default values."
                    });
                }

                return Ok(new ApiResponse<Society>
                {
                    Success = true,
                    Data = society
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving society information",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // PUT: api/society (Admin only)
        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSociety([FromBody] SocietyUpdateDto updateDto)
        {
            try
            {
                var society = await _context.Societies.FirstOrDefaultAsync();
                
                // If no society exists, create one
                if (society == null)
                {
                    society = new Society();
                    _context.Societies.Add(society);
                }

                // Check if there are any non-admin users in the system
                var nonAdminUsers = await _context.Users.Where(u => !u.Details.Contains("\"role\":\"admin\"")).ToListAsync();
                
                if (nonAdminUsers.Count == 0)
                {
                    // No users to approve, apply changes immediately
                    society.SocietyName = updateDto.SocietyName;
                    society.Address = updateDto.Address;
                    society.City = updateDto.City;
                    society.Phone = updateDto.Phone;
                    society.Fax = updateDto.Fax;
                    society.Email = updateDto.Email;
                    society.Website = updateDto.Website;
                    society.RegistrationNumber = updateDto.RegistrationNumber;
                    society.Tabs = JsonSerializer.Serialize(updateDto.Tabs);
                    society.UpdatedAt = DateTime.UtcNow;

                    // Clear any pending state
                    society.PendingChanges = "{}";
                    society.IsPendingApproval = false;

                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Society updated successfully (no users to approve)."
                    });
                }

                // Store the changes in PendingChanges
                var pendingChanges = new
                {
                    SocietyName = updateDto.SocietyName,
                    Address = updateDto.Address,
                    City = updateDto.City,
                    Phone = updateDto.Phone,
                    Fax = updateDto.Fax,
                    Email = updateDto.Email,
                    Website = updateDto.Website,
                    RegistrationNumber = updateDto.RegistrationNumber,
                    Tabs = updateDto.Tabs
                };

                society.PendingChanges = JsonSerializer.Serialize(pendingChanges);
                society.IsPendingApproval = true;

                // Clear any existing approvals for this society (for new change request)
                var existingApprovals = await _context.SocietyApprovals
                    .Where(a => a.SocietyId == society.Id)
                    .ToListAsync();
                _context.SocietyApprovals.RemoveRange(existingApprovals);

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Society update submitted for approval. All {nonAdminUsers.Count} users must approve before changes become permanent.",
                    Data = new { 
                        RequiredApprovals = nonAdminUsers.Count 
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error updating society information",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // POST: api/society/approve-changes
        // [HttpPost("approve-changes")]
        // public async Task<IActionResult> ApprovePendingChanges()
        // {
        //     try
        //     {
        //         var society = await _context.Societies.FirstOrDefaultAsync();
                
        //         if (society == null || !society.IsPendingApproval)
        //         {
        //             return BadRequest(new ApiResponse<object>
        //             {
        //                 Success = false,
        //                 Message = "No pending changes to approve"
        //             });
        //         }

        //         // Parse pending changes and apply them
        //         var pendingChanges = JsonSerializer.Deserialize<SocietyUpdateDto>(society.PendingChanges);
                
        //         if (pendingChanges != null)
        //         {
        //             society.SocietyName = pendingChanges.SocietyName;
        //             society.Address = pendingChanges.Address;
        //             society.City = pendingChanges.City;
        //             society.Phone = pendingChanges.Phone;
        //             society.Fax = pendingChanges.Fax;
        //             society.Email = pendingChanges.Email;
        //             society.Website = pendingChanges.Website;
        //             society.RegistrationNumber = pendingChanges.RegistrationNumber;
        //             society.Tabs = JsonSerializer.Serialize(pendingChanges.Tabs);
        //         }

        //         // Clear pending changes
        //         society.PendingChanges = "{}";
        //         society.IsPendingApproval = false;

        //         await _context.SaveChangesAsync();

        //         return Ok(new ApiResponse<Society>
        //         {
        //             Success = true,
        //             Data = society,
        //             Message = "Society changes approved and applied successfully"
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new ApiResponse<object>
        //         {
        //             Success = false,
        //             Message = "Error approving society changes",
        //             Errors = new[] { ex.Message }
        //         });
        //     }
        // }

        // POST: api/society/approve-changes
        [HttpPost("approve-changes")]
        public async Task<IActionResult> ApprovePendingChanges()
        {
            try
            {
                var society = await _context.Societies.FirstOrDefaultAsync();
                if (society == null || !society.IsPendingApproval)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No pending changes to approve"
                    });
                }

                // Get current user ID from JWT token
                var currentUsername = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUsername))
                    return Unauthorized();

                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUsername);
                if (currentUser == null)
                    return Unauthorized();

                // Check if user is admin - admins can't approve their own changes
                if (currentUser.Role == "admin")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Administrators cannot approve their own changes."
                    });
                }

                // Prevent double approval by checking if user already approved this society changes
                if (await _context.SocietyApprovals
                    .AnyAsync(a => a.SocietyId == society.Id && a.UserId == currentUser.Id.ToString() && a.Approved))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "You have already approved these changes."
                    });
                }

                // Save approval
                var approval = new SocietyApproval
                {
                    SocietyId = society.Id,
                    UserId = currentUser.Id.ToString(),
                    Approved = true,
                    ApprovedAt = DateTime.UtcNow
                };

                _context.SocietyApprovals.Add(approval);
                await _context.SaveChangesAsync();

                // Check if all non-admin users approved
                var nonAdminUsers = await _context.Users
                    .Where(u => !u.Details.Contains("\"role\":\"admin\""))
                    .ToListAsync();
                    
                var approvedUsers = await _context.SocietyApprovals
                    .CountAsync(a => a.SocietyId == society.Id && a.Approved);

                if (approvedUsers >= nonAdminUsers.Count)
                {
                    // Apply pending changes now
                    var pendingChanges = JsonSerializer.Deserialize<SocietyUpdateDto>(society.PendingChanges);
                    if (pendingChanges != null)
                    {
                        society.SocietyName = pendingChanges.SocietyName;
                        society.Address = pendingChanges.Address;
                        society.City = pendingChanges.City;
                        society.Phone = pendingChanges.Phone;
                        society.Fax = pendingChanges.Fax;
                        society.Email = pendingChanges.Email;
                        society.Website = pendingChanges.Website;
                        society.RegistrationNumber = pendingChanges.RegistrationNumber;
                        society.Tabs = JsonSerializer.Serialize(pendingChanges.Tabs);
                        society.UpdatedAt = DateTime.UtcNow;
                    }

                    // Clear pending changes
                    society.PendingChanges = "{}";
                    society.IsPendingApproval = false;

                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "✅ All users approved. Changes applied successfully!"
                    });
                }

                var pendingCount = nonAdminUsers.Count - approvedUsers;
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Your approval is recorded. Waiting for {pendingCount} more approvals."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error approving society changes",
                    Errors = new[] { ex.Message }
                });
            }
        }


        // GET: api/society/pending-changes
        [HttpGet("pending-changes")]
        public async Task<IActionResult> GetPendingChanges()
        {
            try
            {
                var society = await _context.Societies.FirstOrDefaultAsync();
                
                if (society == null || !society.IsPendingApproval)
                {
                    return Ok(new ApiResponse<PendingChangesWithApprovalsDto>
                    {
                        Success = true,
                        Data = new PendingChangesWithApprovalsDto { HasPendingChanges = false },
                        Message = "No pending changes"
                    });
                }

                // Get all non-admin users and their approval status
                var nonAdminUsers = await _context.Users
                    .Where(u => !u.Details.Contains("\"role\":\"admin\""))
                    .ToListAsync();

                var approvals = await _context.SocietyApprovals
                    .Where(a => a.SocietyId == society.Id)
                    .ToListAsync();

                var approvalStatus = nonAdminUsers.Select(user =>
                {
                    var approval = approvals.FirstOrDefault(a => a.UserId == user.Id.ToString());
                    var userDetails = JsonSerializer.Deserialize<UserDetails>(user.Details);
                    
                    return new ApprovalStatusDto
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Name = userDetails?.Name ?? string.Empty,
                        Email = userDetails?.email ?? string.Empty,
                        HasApproved = approval != null && approval.Approved,
                        ApprovedAt = approval?.ApprovedAt
                    };
                }).ToList();

                var result = new PendingChangesWithApprovalsDto
                {
                    HasPendingChanges = true,
                    PendingChanges = society.PendingChanges,
                    ApprovalStatus = approvalStatus,
                    TotalUsers = nonAdminUsers.Count,
                    ApprovedCount = approvals.Count(a => a.Approved),
                    PendingCount = nonAdminUsers.Count - approvals.Count(a => a.Approved),
                    ChangeRequestId = string.Empty
                };

                return Ok(new ApiResponse<PendingChangesWithApprovalsDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving pending changes",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/society/approval-status (Admin only)
        [HttpGet("approval-status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetApprovalStatus()
        {
            try
            {
                var society = await _context.Societies.FirstOrDefaultAsync();
                
                if (society == null || !society.IsPendingApproval)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Data = new { 
                            HasPendingChanges = false,
                            Message = "No pending changes requiring approval"
                        }
                    });
                }

                // Get detailed approval status
                var nonAdminUsers = await _context.Users
                    .Where(u => !u.Details.Contains("\"role\":\"admin\""))
                    .ToListAsync();

                var approvals = await _context.SocietyApprovals
                    .Where(a => a.SocietyId == society.Id)
                    .ToListAsync();

                var detailedStatus = nonAdminUsers.Select(user =>
                {
                    var approval = approvals.FirstOrDefault(a => a.UserId == user.Id.ToString());
                    var userDetails = JsonSerializer.Deserialize<UserDetails>(user.Details);
                    
                    return new
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Name = userDetails?.Name ?? string.Empty,
                        Email = userDetails?.email ?? string.Empty,
                        Phone = userDetails?.phone ?? string.Empty,
                        EDPNo = userDetails?.EDPNo ?? string.Empty,
                        HasApproved = approval != null && approval.Approved,
                        ApprovedAt = approval?.ApprovedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        Status = approval != null && approval.Approved ? "Approved" : "Pending"
                    };
                }).ToList();

                var pendingUsers = detailedStatus.Where(u => !u.HasApproved).ToList();
                var approvedUsers = detailedStatus.Where(u => u.HasApproved).ToList();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new
                    {
                        HasPendingChanges = true,
                        ChangeRequestId = string.Empty,
                        TotalUsers = nonAdminUsers.Count,
                        ApprovedCount = approvedUsers.Count,
                        PendingCount = pendingUsers.Count,
                        ApprovedUsers = approvedUsers,
                        PendingUsers = pendingUsers,
                        AllUsers = detailedStatus,
                        PendingChanges = society.PendingChanges
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving approval status",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}