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

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Society update submitted for approval. All users must approve before changes become permanent."
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

                var userId = User.Identity?.Name; // Or however you store logged-in user
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Prevent double approval
                if (await _context.SocietyApprovals
                    .AnyAsync(a => a.SocietyId == society.Id && a.UserId == userId && a.Approved))
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
                    UserId = userId,
                    Approved = true,
                    ApprovedAt = DateTime.UtcNow
                };

                _context.SocietyApprovals.Add(approval);
                await _context.SaveChangesAsync();

                // ✅ Check if all users approved
                var totalUsers = await _context.Users.CountAsync(); // Adjust if users are tied to society
                var approvedUsers = await _context.SocietyApprovals
                    .CountAsync(a => a.SocietyId == society.Id && a.Approved);

                if (approvedUsers >= totalUsers)
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

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Your approval is recorded. Waiting for other users."
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
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Data = null,
                        Message = "No pending changes"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new { 
                        HasPendingChanges = society.IsPendingApproval,
                        PendingChanges = society.PendingChanges 
                    }
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
    }
}