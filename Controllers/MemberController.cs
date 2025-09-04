// Controllers/MemberController.cs
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
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/member
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            try
            {
                var members = await _context.Members
                    .OrderBy(m => m.MemNo)
                    .ToListAsync();

                var memberResponses = members.Select(m => new MemberResponseDto
                {
                    Id = m.Id,
                    MemNo = m.MemNo,
                    Name = m.Name,
                    FHName = m.FHName,
                    OfficeAddress = m.OfficeAddress,
                    City = m.City,
                    PhoneOffice = m.PhoneOffice,
                    Branch = m.Branch,
                    PhoneRes = m.PhoneRes,
                    Mobile = m.Mobile,
                    Designation = m.Designation,
                    ResidenceAddress = m.ResidenceAddress,
                    DOB = m.DOB,
                    DOJSociety = m.DOJSociety,
                    Email = m.Email,
                    DOJOrg = m.DOJOrg,
                    DOR = m.DOR,
                    Nominee = m.Nominee,
                    NomineeRelation = m.NomineeRelation,
                    BankingDetails = JsonSerializer.Deserialize<BankingDetailsDto>(m.BankingDetails) ?? new BankingDetailsDto(),
                    IsPendingApproval = m.IsPendingApproval,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(new ApiResponse<List<MemberResponseDto>>
                {
                    Success = true,
                    Data = memberResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving members",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/member/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                
                if (member == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Member not found"
                    });
                }

                var memberResponse = new MemberResponseDto
                {
                    Id = member.Id,
                    MemNo = member.MemNo,
                    Name = member.Name,
                    FHName = member.FHName,
                    OfficeAddress = member.OfficeAddress,
                    City = member.City,
                    PhoneOffice = member.PhoneOffice,
                    Branch = member.Branch,
                    PhoneRes = member.PhoneRes,
                    Mobile = member.Mobile,
                    Designation = member.Designation,
                    ResidenceAddress = member.ResidenceAddress,
                    DOB = member.DOB,
                    DOJSociety = member.DOJSociety,
                    Email = member.Email,
                    DOJOrg = member.DOJOrg,
                    DOR = member.DOR,
                    Nominee = member.Nominee,
                    NomineeRelation = member.NomineeRelation,
                    BankingDetails = JsonSerializer.Deserialize<BankingDetailsDto>(member.BankingDetails) ?? new BankingDetailsDto(),
                    IsPendingApproval = member.IsPendingApproval,
                    CreatedAt = member.CreatedAt,
                    UpdatedAt = member.UpdatedAt
                };

                return Ok(new ApiResponse<MemberResponseDto>
                {
                    Success = true,
                    Data = memberResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving member",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // POST: api/member
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberCreateDto createDto)
        {
            try
            {
                // Auto-generate MemNo
                var lastMember = await _context.Members
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (lastMember != null && !string.IsNullOrEmpty(lastMember.MemNo))
                {
                    var lastNumberStr = lastMember.MemNo.Replace("MEM_", "");
                    if (int.TryParse(lastNumberStr, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                var memNo = $"MEM_{nextNumber:D3}"; // MEM_001, MEM_002, etc.

                var member = new Member
                {
                    MemNo = memNo,
                    Name = createDto.Name,
                    FHName = createDto.FHName,
                    OfficeAddress = createDto.OfficeAddress,
                    City = createDto.City,
                    PhoneOffice = createDto.PhoneOffice,
                    Branch = createDto.Branch,
                    PhoneRes = createDto.PhoneRes,
                    Mobile = createDto.Mobile,
                    Designation = createDto.Designation,
                    ResidenceAddress = createDto.ResidenceAddress,
                    DOB = createDto.DOB,
                    DOJSociety = createDto.DOJSociety,
                    Email = createDto.Email,
                    DOJOrg = createDto.DOJOrg,
                    DOR = createDto.DOR,
                    Nominee = createDto.Nominee,
                    NomineeRelation = createDto.NomineeRelation,
                    BankingDetails = JsonSerializer.Serialize(createDto.BankingDetails)
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                var memberResponse = new MemberResponseDto
                {
                    Id = member.Id,
                    MemNo = member.MemNo,
                    Name = member.Name,
                    FHName = member.FHName,
                    OfficeAddress = member.OfficeAddress,
                    City = member.City,
                    PhoneOffice = member.PhoneOffice,
                    Branch = member.Branch,
                    PhoneRes = member.PhoneRes,
                    Mobile = member.Mobile,
                    Designation = member.Designation,
                    ResidenceAddress = member.ResidenceAddress,
                    DOB = member.DOB,
                    DOJSociety = member.DOJSociety,
                    Email = member.Email,
                    DOJOrg = member.DOJOrg,
                    DOR = member.DOR,
                    Nominee = member.Nominee,
                    NomineeRelation = member.NomineeRelation,
                    BankingDetails = createDto.BankingDetails,
                    IsPendingApproval = member.IsPendingApproval,
                    CreatedAt = member.CreatedAt,
                    UpdatedAt = member.UpdatedAt
                };

                return CreatedAtAction(nameof(GetMember), new { id = member.Id }, new ApiResponse<MemberResponseDto>
                {
                    Success = true,
                    Data = memberResponse,
                    Message = "Member created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error creating member",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // PUT: api/member/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] MemberUpdateDto updateDto)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                
                if (member == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Member not found"
                    });
                }

                // Store the changes in PendingChanges
                var pendingChanges = new
                {
                    Name = updateDto.Name,
                    FHName = updateDto.FHName,
                    OfficeAddress = updateDto.OfficeAddress,
                    City = updateDto.City,
                    PhoneOffice = updateDto.PhoneOffice,
                    Branch = updateDto.Branch,
                    PhoneRes = updateDto.PhoneRes,
                    Mobile = updateDto.Mobile,
                    Designation = updateDto.Designation,
                    ResidenceAddress = updateDto.ResidenceAddress,
                    DOB = updateDto.DOB,
                    DOJSociety = updateDto.DOJSociety,
                    Email = updateDto.Email,
                    DOJOrg = updateDto.DOJOrg,
                    DOR = updateDto.DOR,
                    Nominee = updateDto.Nominee,
                    NomineeRelation = updateDto.NomineeRelation,
                    BankingDetails = updateDto.BankingDetails
                };

                member.PendingChanges = JsonSerializer.Serialize(pendingChanges);
                member.IsPendingApproval = true;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Member update submitted for approval. All users must approve before changes become permanent."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error updating member",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // POST: api/member/{id}/approve-changes
        [HttpPost("{id}/approve-changes")]
        public async Task<IActionResult> ApprovePendingChanges(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                
                if (member == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Member not found"
                    });
                }

                if (!member.IsPendingApproval)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No pending changes to approve for this member"
                    });
                }

                // Parse pending changes and apply them
                var pendingChanges = JsonSerializer.Deserialize<MemberUpdateDto>(member.PendingChanges);
                
                if (pendingChanges != null)
                {
                    member.Name = pendingChanges.Name;
                    member.FHName = pendingChanges.FHName;
                    member.OfficeAddress = pendingChanges.OfficeAddress;
                    member.City = pendingChanges.City;
                    member.PhoneOffice = pendingChanges.PhoneOffice;
                    member.Branch = pendingChanges.Branch;
                    member.PhoneRes = pendingChanges.PhoneRes;
                    member.Mobile = pendingChanges.Mobile;
                    member.Designation = pendingChanges.Designation;
                    member.ResidenceAddress = pendingChanges.ResidenceAddress;
                    member.DOB = pendingChanges.DOB;
                    member.DOJSociety = pendingChanges.DOJSociety;
                    member.Email = pendingChanges.Email;
                    member.DOJOrg = pendingChanges.DOJOrg;
                    member.DOR = pendingChanges.DOR;
                    member.Nominee = pendingChanges.Nominee;
                    member.NomineeRelation = pendingChanges.NomineeRelation;
                    member.BankingDetails = JsonSerializer.Serialize(pendingChanges.BankingDetails);
                }

                // Clear pending changes
                member.PendingChanges = "{}";
                member.IsPendingApproval = false;

                await _context.SaveChangesAsync();

                var memberResponse = new MemberResponseDto
                {
                    Id = member.Id,
                    MemNo = member.MemNo,
                    Name = member.Name,
                    FHName = member.FHName,
                    OfficeAddress = member.OfficeAddress,
                    City = member.City,
                    PhoneOffice = member.PhoneOffice,
                    Branch = member.Branch,
                    PhoneRes = member.PhoneRes,
                    Mobile = member.Mobile,
                    Designation = member.Designation,
                    ResidenceAddress = member.ResidenceAddress,
                    DOB = member.DOB,
                    DOJSociety = member.DOJSociety,
                    Email = member.Email,
                    DOJOrg = member.DOJOrg,
                    DOR = member.DOR,
                    Nominee = member.Nominee,
                    NomineeRelation = member.NomineeRelation,
                    BankingDetails = JsonSerializer.Deserialize<BankingDetailsDto>(member.BankingDetails) ?? new BankingDetailsDto(),
                    IsPendingApproval = member.IsPendingApproval,
                    CreatedAt = member.CreatedAt,
                    UpdatedAt = member.UpdatedAt
                };

                return Ok(new ApiResponse<MemberResponseDto>
                {
                    Success = true,
                    Data = memberResponse,
                    Message = "Member changes approved and applied successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error approving member changes",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/member/pending-changes
        [HttpGet("pending-changes")]
        public async Task<IActionResult> GetMembersWithPendingChanges()
        {
            try
            {
                var membersWithPendingChanges = await _context.Members
                    .Where(m => m.IsPendingApproval)
                    .Select(m => new { 
                        Id = m.Id, 
                        MemNo = m.MemNo, 
                        Name = m.Name,
                        PendingChanges = m.PendingChanges 
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = membersWithPendingChanges
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving members with pending changes",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}