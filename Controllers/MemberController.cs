// Controllers/MemberController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FintcsApi.Data;
using FintcsApi.Models;
using System.Text;           // ✅ add this
using System.IO;  

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

                // here i need console
                Console.WriteLine(JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true }));

                var memberResponses = members.Select(m => new MemberResponseDto
                {
                    Id = m.Id,
                    MemNo = m.MemNo,
                    Name = m.Name,
                    FHName = m.FHName,
                    OfficeAddress = m.OfficeAddress,
                    City = m.City,
                    cdAmount = m.cdAmount,
                    Email2 = m.Email2,
                    Pincode = m.Pincode,
                    Mobile2 = m.Mobile2,    
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

                // here i need console
                Console.WriteLine(JsonSerializer.Serialize(memberResponses, new JsonSerializerOptions { WriteIndented = true }));
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
                    cdAmount = member.cdAmount,
                    Email2 = member.Email2,
                    Pincode = member.Pincode,
                    Mobile2 = member.Mobile2,
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
        // [HttpPost]
        // public async Task<IActionResult> CreateMember([FromBody] MemberCreateDto createDto)
        // {
        //     // =============================
        //     // 1. Log the raw request body
        //     // =============================
        //     string rawRequestBody;
        //     try
        //     {
        //         Request.EnableBuffering();
        //         Request.Body.Position = 0;
        //         using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        //         rawRequestBody = await reader.ReadToEndAsync();
        //         Request.Body.Position = 0; // reset for model binding
        //     }
        //     catch (Exception ex)
        //     {
        //         rawRequestBody = "[Failed to read raw body]";
        //         Console.WriteLine($"[RequestBody] Error reading body: {ex.Message}");
        //     }

        //     Console.WriteLine($"[RequestBody] {DateTime.UtcNow:o} => {rawRequestBody}");

        //     // =============================
        //     // 2. Log the bound DTO (post-model binding)
        //     // =============================
        //     try
        //     {
        //         var dtoAsJson = JsonSerializer.Serialize(createDto);
        //         Console.WriteLine($"[BoundDTO] {DateTime.UtcNow:o} => {dtoAsJson}");
        //     }
        //     catch (Exception dtoEx)
        //     {
        //         Console.WriteLine($"[BoundDTO] Failed to serialize DTO: {dtoEx.Message}");
        //     }

        //     // =============================
        //     // 3. Main try/catch for logic
        //     // =============================
        //     try
        //     {
        //         Console.WriteLine($"[CreateMember] {DateTime.UtcNow:o} - Start CreateMember");

        //         // 1. Validate model state
        //         if (!ModelState.IsValid)
        //         {
        //             var errors = ModelState.Values
        //                 .SelectMany(v => v.Errors)
        //                 .Select(e => e.ErrorMessage)
        //                 .ToArray();

        //             Console.WriteLine("[CreateMember] ModelState invalid: " + string.Join(" | ", errors));
        //             return BadRequest(new ApiResponse<object>
        //             {
        //                 Success = false,
        //                 Message = "Validation failed",
        //                 Errors = errors
        //             });
        //         }
        //         Console.WriteLine("[CreateMember] ModelState valid");

        //         // 2. Find last member
        //         Console.WriteLine("[CreateMember] Querying last member from DB...");
        //         var lastMember = await _context.Members
        //             .OrderByDescending(m => m.Id)
        //             .FirstOrDefaultAsync();

        //         Console.WriteLine($"[CreateMember] Last member found: {(lastMember != null ? lastMember.MemNo : "NULL")}");

        //         // 3. Compute next MemNo
        //         int nextNumber = 1;
        //         if (lastMember != null && !string.IsNullOrEmpty(lastMember.MemNo))
        //         {
        //             var lastNumberStr = lastMember.MemNo.Replace("MEM_", "");
        //             Console.WriteLine($"[CreateMember] Extracted lastNumberStr: '{lastNumberStr}'");

        //             if (int.TryParse(lastNumberStr, out int lastNumber))
        //             {
        //                 nextNumber = lastNumber + 1;
        //                 Console.WriteLine($"[CreateMember] Parsed lastNumber: {lastNumber}, nextNumber: {nextNumber}");
        //             }
        //             else
        //             {
        //                 Console.WriteLine($"[CreateMember] Failed to parse '{lastNumberStr}', using nextNumber = {nextNumber}");
        //             }
        //         }

        //         var memNo = $"MEM_{nextNumber:D3}";
        //         Console.WriteLine($"[CreateMember] Generated MemNo: {memNo}");

        //         // 4. Create Member entity
        //         Console.WriteLine("[CreateMember] Creating Member entity...");
        //         var member = new Member
        //         {
        //             MemNo = memNo,
        //             Name = createDto.Name,
        //             FHName = createDto.FHName,
        //             OfficeAddress = createDto.OfficeAddress,
        //             City = createDto.City,
        //             cdAmount = createDto.cdAmount,
        //             Email2 = createDto.Email2,
        //             Pincode = createDto.Pincode,
        //             Mobile2 = createDto.Mobile2,
        //             PhoneOffice = createDto.PhoneOffice,
        //             Branch = createDto.Branch,
        //             PhoneRes = createDto.PhoneRes,
        //             Mobile = createDto.Mobile,
        //             Designation = createDto.Designation,
        //             ResidenceAddress = createDto.ResidenceAddress,
        //             Email = createDto.Email,
        //             DOB = createDto.DOB,
        //             DOJSociety = createDto.DOJSociety,
        //             DOJOrg = createDto.DOJOrg,
        //             DOR = createDto.DOR,
        //             Nominee = createDto.Nominee,
        //             NomineeRelation = createDto.NomineeRelation,
        //             BankingDetails = JsonSerializer.Serialize(createDto.BankingDetails) // store as JSON string
        //         };
        //         Console.WriteLine("[CreateMember] Member object prepared. (BankingDetails serialized)");

        //         // 5. Add & save
        //         _context.Members.Add(member);
        //         Console.WriteLine("[CreateMember] Member added to DbContext. Calling SaveChangesAsync...");
        //         await _context.SaveChangesAsync();
        //         Console.WriteLine($"[CreateMember] SaveChangesAsync completed. New Member.Id = {member.Id}");

        //         // 6. Prepare response DTO
        //         var memberResponse = new MemberResponseDto
        //         {
        //             Id = member.Id,
        //             MemNo = member.MemNo,
        //             Name = member.Name,
        //             FHName = member.FHName,
        //             OfficeAddress = member.OfficeAddress,
        //             City = member.City,
        //             cdAmount = member.cdAmount,
        //             Email2 = member.Email2,
        //             Pincode = member.Pincode,
        //             Mobile2 = member.Mobile2,
        //             PhoneOffice = member.PhoneOffice,
        //             Branch = member.Branch,
        //             PhoneRes = member.PhoneRes,
        //             Mobile = member.Mobile,
        //             Designation = member.Designation,
        //             ResidenceAddress = member.ResidenceAddress,
        //             DOB = member.DOB,
        //             DOJSociety = member.DOJSociety,
        //             Email = member.Email,
        //             DOJOrg = member.DOJOrg,
        //             DOR = member.DOR,
        //             Nominee = member.Nominee,
        //             NomineeRelation = member.NomineeRelation,
        //             BankingDetails = createDto.BankingDetails,
        //             IsPendingApproval = member.IsPendingApproval,
        //             CreatedAt = member.CreatedAt,
        //             UpdatedAt = member.UpdatedAt
        //         };
        //         Console.WriteLine("[CreateMember] MemberResponseDto prepared");

        //         // 7. Return Created response
        //         Console.WriteLine("[CreateMember] Returning CreatedAtAction result");
        //         return CreatedAtAction(nameof(GetMember), new { id = member.Id }, new ApiResponse<MemberResponseDto>
        //         {
        //             Success = true,
        //             Data = memberResponse,
        //             Message = "Member created successfully"
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"[CreateMember] Exception: {ex}");
        //         return StatusCode(500, new ApiResponse<object>
        //         {
        //             Success = false,
        //             Message = "Error creating member",
        //             Errors = new[] { ex.Message }
        //         });
        //     }
        // }

        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberCreateDto createDto)
        {
            // =============================
            // 1. Log the raw request body
            // =============================
            string rawRequestBody;
            try
            {
                Request.EnableBuffering();
                Request.Body.Position = 0;
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                rawRequestBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0; // reset for model binding
            }
            catch (Exception ex)
            {
                rawRequestBody = "[Failed to read raw body]";
                Console.WriteLine($"[RequestBody] Error reading body: {ex.Message}");
            }

            Console.WriteLine($"[RequestBody] {DateTime.UtcNow:o} => {rawRequestBody}");

            // =============================
            // 2. Log the bound DTO (post-model binding)
            // =============================
            try
            {
                var dtoAsJson = JsonSerializer.Serialize(createDto);
                Console.WriteLine($"[BoundDTO] {DateTime.UtcNow:o} => {dtoAsJson}");
            }
            catch (Exception dtoEx)
            {
                Console.WriteLine($"[BoundDTO] Failed to serialize DTO: {dtoEx.Message}");
            }

            // =============================
            // 3. Main try/catch for logic
            // =============================
            try
            {
                Console.WriteLine($"[CreateMember] {DateTime.UtcNow:o} - Start CreateMember");

                // 1. Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    Console.WriteLine("[CreateMember] ModelState invalid: " + string.Join(" | ", errors));
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }
                Console.WriteLine("[CreateMember] ModelState valid");

                // 2. Uniqueness checks
                Console.WriteLine("[CreateMember] Checking unique fields...");

                if (!string.IsNullOrEmpty(createDto.Email))
                {
                    bool emailExists = await _context.Members.AnyAsync(m => m.Email == createDto.Email);
                    if (emailExists)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Email already exists",
                            Errors = new[] { "The provided Email is already owned by another member." }
                        });
                    }
                }

                if (!string.IsNullOrEmpty(createDto.Mobile))
                {
                    bool mobileExists = await _context.Members.AnyAsync(m => m.Mobile == createDto.Mobile);
                    if (mobileExists)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Mobile number already exists",
                            Errors = new[] { "The provided Mobile number is already owned by another member." }
                        });
                    }
                }

                if (!string.IsNullOrEmpty(createDto.Email2))
                {
                    bool email2Exists = await _context.Members.AnyAsync(m => m.Email2 == createDto.Email2);
                    if (email2Exists)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Secondary Email already exists",
                            Errors = new[] { "The provided Email2 is already owned by another member." }
                        });
                    }
                }

                // 3. Find last member
                Console.WriteLine("[CreateMember] Querying last member from DB...");
                var lastMember = await _context.Members
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefaultAsync();

                Console.WriteLine($"[CreateMember] Last member found: {(lastMember != null ? lastMember.MemNo : "NULL")}");

                // 4. Compute next MemNo
                int nextNumber = 1;
                if (lastMember != null && !string.IsNullOrEmpty(lastMember.MemNo))
                {
                    var lastNumberStr = lastMember.MemNo.Replace("MEM_", "");
                    Console.WriteLine($"[CreateMember] Extracted lastNumberStr: '{lastNumberStr}'");

                    if (int.TryParse(lastNumberStr, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                        Console.WriteLine($"[CreateMember] Parsed lastNumber: {lastNumber}, nextNumber: {nextNumber}");
                    }
                    else
                    {
                        Console.WriteLine($"[CreateMember] Failed to parse '{lastNumberStr}', using nextNumber = {nextNumber}");
                    }
                }

                var memNo = $"MEM_{nextNumber:D3}";
                Console.WriteLine($"[CreateMember] Generated MemNo: {memNo}");

                // 5. Create Member entity
                Console.WriteLine("[CreateMember] Creating Member entity...");
                var member = new Member
                {
                    MemNo = memNo,
                    Name = createDto.Name,
                    FHName = createDto.FHName,
                    OfficeAddress = createDto.OfficeAddress,
                    City = createDto.City,
                    cdAmount = createDto.cdAmount,
                    Email2 = createDto.Email2,
                    Pincode = createDto.Pincode,
                    Mobile2 = createDto.Mobile2,
                    PhoneOffice = createDto.PhoneOffice,
                    Branch = createDto.Branch,
                    PhoneRes = createDto.PhoneRes,
                    Mobile = createDto.Mobile,
                    Designation = createDto.Designation,
                    ResidenceAddress = createDto.ResidenceAddress,
                    Email = createDto.Email,
                    DOB = createDto.DOB,
                    DOJSociety = createDto.DOJSociety,
                    DOJOrg = createDto.DOJOrg,
                    DOR = createDto.DOR,
                    Nominee = createDto.Nominee,
                    NomineeRelation = createDto.NomineeRelation,
                    BankingDetails = JsonSerializer.Serialize(createDto.BankingDetails)
                };
                Console.WriteLine("[CreateMember] Member object prepared. (BankingDetails serialized)");

                // 6. Add & save
                _context.Members.Add(member);
                Console.WriteLine("[CreateMember] Member added to DbContext. Calling SaveChangesAsync...");
                await _context.SaveChangesAsync();
                Console.WriteLine($"[CreateMember] SaveChangesAsync completed. New Member.Id = {member.Id}");

                // 7. Prepare response DTO
                var memberResponse = new MemberResponseDto
                {
                    Id = member.Id,
                    MemNo = member.MemNo,
                    Name = member.Name,
                    FHName = member.FHName,
                    OfficeAddress = member.OfficeAddress,
                    City = member.City,
                    cdAmount = member.cdAmount,
                    Email2 = member.Email2,
                    Pincode = member.Pincode,
                    Mobile2 = member.Mobile2,
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
                Console.WriteLine("[CreateMember] MemberResponseDto prepared");

                // 8. Return Created response
                Console.WriteLine("[CreateMember] Returning CreatedAtAction result");
                return CreatedAtAction(nameof(GetMember), new { id = member.Id }, new ApiResponse<MemberResponseDto>
                {
                    Success = true,
                    Data = memberResponse,
                    Message = "Member created successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateMember] Exception: {ex}");
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
            Request.EnableBuffering();

            // Read raw request body
            string rawBody;
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0; // reset stream for model binding
            }

            // Log the raw request body BEFORE try block
            Console.WriteLine("=========== Raw Request Body BEFORE try ===========");
            Console.WriteLine(rawBody);
            try
            {
                Console.WriteLine("=========== UpdateMember API Called ===========");
                Console.WriteLine($"Member ID (Route Param): {id}");

                // Print full incoming payload
                Console.WriteLine("Incoming DTO:");
                Console.WriteLine(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions { WriteIndented = true }));

                var member = await _context.Members.FindAsync(id);

                if (member == null)
                {
                    Console.WriteLine($"❌ Member with ID {id} not found.");
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Member not found"
                    });
                }

                Console.WriteLine("✅ Existing Member found. Updating fields...");

                // Directly update fields
                member.Name = updateDto.Name;
                member.FHName = updateDto.FHName;
                member.OfficeAddress = updateDto.OfficeAddress;
                member.City = updateDto.City;
                member.cdAmount = updateDto.cdAmount;
                member.Email2 = updateDto.Email2;
                member.Pincode = updateDto.Pincode;
                member.Mobile2 = updateDto.Mobile2;
                member.PhoneOffice = updateDto.PhoneOffice;
                member.Branch = updateDto.Branch;
                member.PhoneRes = updateDto.PhoneRes;
                member.Mobile = updateDto.Mobile;
                member.Designation = updateDto.Designation;
                member.ResidenceAddress = updateDto.ResidenceAddress;
                member.DOB = updateDto.DOB;
                member.DOJSociety = updateDto.DOJSociety;
                member.Email = updateDto.Email;
                member.DOJOrg = updateDto.DOJOrg;
                member.DOR = updateDto.DOR;
                member.Nominee = updateDto.Nominee;
                member.NomineeRelation = updateDto.NomineeRelation;

                // Debugging banking details
                Console.WriteLine("Banking Details (From DTO):");
                Console.WriteLine(JsonSerializer.Serialize(updateDto.BankingDetails, new JsonSerializerOptions { WriteIndented = true }));

                // Assuming stored as JSON in DB
                member.BankingDetails = JsonSerializer.Serialize(updateDto.BankingDetails);

                Console.WriteLine("✅ Member object after mapping:");
                Console.WriteLine(JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true }));

                await _context.SaveChangesAsync();

                Console.WriteLine("💾 Database save successful!");

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Member updated successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception occurred during UpdateMember:");
                Console.WriteLine(ex.ToString());

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
                    member.cdAmount = pendingChanges.cdAmount;
                    member.Email2 = pendingChanges.Email2;
                    member.Pincode = pendingChanges.Pincode;
                    member.Mobile2 = pendingChanges.Mobile2;
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
                    cdAmount = member.cdAmount,
                    Email2 = member.Email2,
                    Pincode = member.Pincode,
                    Mobile2 = member.Mobile2,
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