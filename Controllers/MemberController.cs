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
                    UpdatedAt = m.UpdatedAt,
                    Status = m.Status  // ✅ map Status field
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
                    UpdatedAt = member.UpdatedAt,
                    Status = member.Status  // ✅ map Status field
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
                    Status = createDto.Status,  // ✅ set Status from DTO
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
                    UpdatedAt = member.UpdatedAt,
                    Status = member.Status  // ✅ map Status field
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

            // --- Read raw request body ---
            string rawBody = string.Empty;
            try
            {
                Request.Body.Position = 0;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    rawBody = await reader.ReadToEndAsync();
                }
                Request.Body.Position = 0;

                Console.WriteLine($"Raw request body length: {rawBody?.Length ?? 0}");
                var preview = rawBody != null && rawBody.Length > 1000
                    ? rawBody.Substring(0, 1000) + " ...[truncated]"
                    : rawBody;
                Console.WriteLine("Raw request body preview:");
                Console.WriteLine(preview ?? "(empty)");
            }
            catch (Exception exRead)
            {
                Console.WriteLine("❌ Exception while reading raw request body:");
                Console.WriteLine(exRead.ToString());
            }

            // --- Model binding check ---
            if (updateDto == null)
            {
                Console.WriteLine("❌ updateDto is NULL after model binding.");
                Console.WriteLine("ModelState errors (if any):");
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        Console.WriteLine($"  ModelState[{kv.Key}]: {err.ErrorMessage}");
                    }
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Model binding failed or payload empty",
                    Errors = new[] { "updateDto is null or model state invalid" }
                });
            }

            Console.WriteLine("Deserialized updateDto:");
            Console.WriteLine(JsonSerializer.Serialize(updateDto, new JsonSerializerOptions { WriteIndented = true }));

            try
            {
                var member = await _context.Members.FindAsync(id);

                if (member == null)
                {
                    Console.WriteLine($"❌ Member with ID {id} NOT FOUND.");
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Member not found"
                    });
                }

                Console.WriteLine("✅ Existing member found. Current state:");
                Console.WriteLine(JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true }));

                // --- Update fields one by one with debug logs ---
                Console.WriteLine("---- Field: Name ----");
                Console.WriteLine($"Current: '{member.Name}' | Incoming: '{updateDto.Name}'");
                if (!string.IsNullOrWhiteSpace(updateDto.Name) && updateDto.Name != member.Name)
                    member.Name = updateDto.Name;

                Console.WriteLine("---- Field: FHName ----");
                Console.WriteLine($"Current: '{member.FHName}' | Incoming: '{updateDto.FHName}'");
                if (!string.IsNullOrWhiteSpace(updateDto.FHName) && updateDto.FHName != member.FHName)
                    member.FHName = updateDto.FHName;

                Console.WriteLine("---- Field: OfficeAddress ----");
                if (!string.IsNullOrWhiteSpace(updateDto.OfficeAddress) && updateDto.OfficeAddress != member.OfficeAddress)
                    member.OfficeAddress = updateDto.OfficeAddress;

                Console.WriteLine("---- Field: City ----");
                if (!string.IsNullOrWhiteSpace(updateDto.City) && updateDto.City != member.City)
                    member.City = updateDto.City;

                Console.WriteLine("---- Field: cdAmount ----");
                if (!string.IsNullOrWhiteSpace(updateDto.cdAmount) && updateDto.cdAmount != member.cdAmount)
                    member.cdAmount = updateDto.cdAmount;
                Console.WriteLine($"Current: '{member.cdAmount?.ToString() ?? "null"}' | Incoming: '{updateDto.cdAmount?.ToString() ?? "null"}'");
                

                Console.WriteLine("---- Field: Email2 ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Email2) && updateDto.Email2 != member.Email2)
                    member.Email2 = updateDto.Email2;

                Console.WriteLine("---- Field: Pincode ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Pincode) && updateDto.Pincode != member.Pincode)
                    member.Pincode = updateDto.Pincode;

                Console.WriteLine("---- Field: Mobile2 ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Mobile2) && updateDto.Mobile2 != member.Mobile2)
                    member.Mobile2 = updateDto.Mobile2;

                Console.WriteLine("---- Field: PhoneOffice ----");
                if (!string.IsNullOrWhiteSpace(updateDto.PhoneOffice) && updateDto.PhoneOffice != member.PhoneOffice)
                    member.PhoneOffice = updateDto.PhoneOffice;

                Console.WriteLine("---- Field: Branch ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Branch) && updateDto.Branch != member.Branch)
                    member.Branch = updateDto.Branch;

                Console.WriteLine("---- Field: PhoneRes ----");
                if (!string.IsNullOrWhiteSpace(updateDto.PhoneRes) && updateDto.PhoneRes != member.PhoneRes)
                    member.PhoneRes = updateDto.PhoneRes;

                Console.WriteLine("---- Field: Mobile ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Mobile) && updateDto.Mobile != member.Mobile)
                    member.Mobile = updateDto.Mobile;

                Console.WriteLine("---- Field: Designation ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Designation) && updateDto.Designation != member.Designation)
                    member.Designation = updateDto.Designation;

                Console.WriteLine("---- Field: ResidenceAddress ----");
                if (!string.IsNullOrWhiteSpace(updateDto.ResidenceAddress) && updateDto.ResidenceAddress != member.ResidenceAddress)
                    member.ResidenceAddress = updateDto.ResidenceAddress;

                Console.WriteLine("---- Field: DOB ----");
                if (updateDto.DOB != null && updateDto.DOB != member.DOB)
                    member.DOB = updateDto.DOB;

                Console.WriteLine("---- Field: DOJSociety ----");
                if (updateDto.DOJSociety != null && updateDto.DOJSociety != member.DOJSociety)
                    member.DOJSociety = updateDto.DOJSociety;

                Console.WriteLine("---- Field: Email ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != member.Email)
                    member.Email = updateDto.Email;

                Console.WriteLine("---- Field: DOJOrg ----");
                if (updateDto.DOJOrg != null && updateDto.DOJOrg != member.DOJOrg)
                    member.DOJOrg = updateDto.DOJOrg;

                Console.WriteLine("---- Field: DOR ----");
                if (updateDto.DOR != null && updateDto.DOR != member.DOR)
                    member.DOR = updateDto.DOR;

                Console.WriteLine("---- Field: Nominee ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Nominee) && updateDto.Nominee != member.Nominee)
                    member.Nominee = updateDto.Nominee;

                Console.WriteLine("---- Field: NomineeRelation ----");
                if (!string.IsNullOrWhiteSpace(updateDto.NomineeRelation) && updateDto.NomineeRelation != member.NomineeRelation)
                    member.NomineeRelation = updateDto.NomineeRelation;
                
                Console.WriteLine("---- Field: Status ----");
                if (!string.IsNullOrWhiteSpace(updateDto.Status) && updateDto.Status != member.Status)
                    member.Status = updateDto.Status;

                Console.WriteLine("---- Field: BankingDetails ----");
                if (updateDto.BankingDetails != null)
                {
                    var bankJson = JsonSerializer.Serialize(updateDto.BankingDetails, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine("Incoming BankingDetails JSON:");
                    Console.WriteLine(bankJson);
                    member.BankingDetails = bankJson;
                }

                Console.WriteLine("Member object BEFORE SaveChanges:");
                Console.WriteLine(JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true }));

                await _context.SaveChangesAsync();

                Console.WriteLine("✅ SaveChangesAsync successful.");
                Console.WriteLine("==================================================");

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

    }
}