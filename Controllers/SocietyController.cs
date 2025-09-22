// Controllers/SocietyController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
                var existingSocietyCount = await _context.Societies.CountAsync();
                if (existingSocietyCount > 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Society already exists. Only one society is allowed in the system."
                    });
                }

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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    chBounceCharge = createDto.chBounceCharge,
                    targetDropdown = createDto.targetDropdown,
                    LoanTypes = createDto.LoanTypes.Select(l => new LoanType
                    {
                        LoanTypeName = l.LoanTypeName,
                        CompulsoryDeposit = l.CompulsoryDeposit,
                        OptionalDeposit = l.OptionalDeposit,
                        Share = l.Share,
                        LimitAmount = l.LimitAmount,
                        Interest = l.Interest,
                        XTimes = l.XTimes
                    }).ToList()
                };

                _context.Societies.Add(society);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<SocietyDto>
                {
                    Success = true,
                    Message = "Society created successfully.",
                    Data = MapToSocietyDto(society)
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
                var society = await _context.Societies
                    .Include(s => s.LoanTypes)
                    .FirstOrDefaultAsync();

                if (society == null)
                {
                    return Ok(new ApiResponse<SocietyDto>
                    {
                        Success = true,
                        Data = null,
                        Message = "No society configuration found. Using default values."
                    });
                }

                return Ok(new ApiResponse<SocietyDto>
                {
                    Success = true,
                    Data = MapToSocietyDto(society)
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
                var society = await _context.Societies
                    .Include(s => s.LoanTypes)
                    .FirstOrDefaultAsync(s => s.Id == updateDto.Id);

                if (society == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Society with Id {updateDto.Id} not found."
                    });
                }

                // Update Society fields
                society.SocietyName = updateDto.SocietyName;
                society.Address = updateDto.Address;
                society.City = updateDto.City;
                society.Phone = updateDto.Phone;
                society.Fax = updateDto.Fax;
                society.Email = updateDto.Email;
                society.Website = updateDto.Website;
                society.RegistrationNumber = updateDto.RegistrationNumber;
                society.UpdatedAt = DateTime.UtcNow;
                society.chBounceCharge = updateDto.chBounceCharge;
                society.targetDropdown = updateDto.targetDropdown;

                // Replace LoanTypes (delete all old → insert new)
                _context.LoanTypes.RemoveRange(society.LoanTypes);
                society.LoanTypes = updateDto.LoanTypes.Select(l => new LoanType
                {
                    LoanTypeName = l.LoanTypeName,
                    CompulsoryDeposit = l.CompulsoryDeposit,
                    OptionalDeposit = l.OptionalDeposit,
                    Share = l.Share,
                    LimitAmount = l.LimitAmount,
                    Interest = l.Interest,
                    XTimes = l.XTimes,
                    SocietyId = society.Id
                }).ToList();

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<SocietyDto>
                {
                    Success = true,
                    Message = "Society updated successfully.",
                    Data = MapToSocietyDto(society)
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

        // 🔹 Helper method to keep mapping clean
        private SocietyDto MapToSocietyDto(Society society)
        {
            return new SocietyDto
            {
                Id = society.Id,
                SocietyName = society.SocietyName,
                Address = society.Address,
                City = society.City,
                Phone = society.Phone,
                Fax = society.Fax,
                Email = society.Email,
                Website = society.Website,
                RegistrationNumber = society.RegistrationNumber,
                chBounceCharge = society.chBounceCharge,
                targetDropdown = society.targetDropdown,
                LoanTypes = society.LoanTypes.Select(l => new LoanTypeDto
                {
                    LoanTypeName = l.LoanTypeName,
                    CompulsoryDeposit = l.CompulsoryDeposit,
                    OptionalDeposit = l.OptionalDeposit,
                    Share = l.Share,
                    LimitAmount = l.LimitAmount,
                    Interest = l.Interest,
                    XTimes = l.XTimes
                }).ToList()
            };
        }
    }
}
