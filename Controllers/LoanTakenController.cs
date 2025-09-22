// Controllers/LoanTakenController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;

namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanTakenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoanTakenController(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- GET MEMBERS ----------------
        [HttpGet("members")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _context.Members
                .Select(m => new { m.Id, m.Name })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Members fetched successfully",
                Data = members
            });
        }

        // ---------------- CREATE LOAN ----------------
        [HttpPost]
        public async Task<IActionResult> CreateLoan([FromBody] LoanTakenCreateDto dto)
        {
            // ✅ Restructure MemberNo if provided (e.g., "MEM_001" → 1)
            if (!string.IsNullOrWhiteSpace(dto.MemberNo))
            {
                if (dto.MemberNo.StartsWith("MEM_"))
                {
                    if (int.TryParse(dto.MemberNo.Replace("MEM_", ""), out var memberId))
                    {
                        dto.MemberId = memberId;
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid MemberNo format",
                            Errors = new[] { "MemberNo must be in format MEM_###" }
                        });
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == dto.MemberId);
            if (member == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid member selected",
                    Errors = new[] { "Member does not exist" }
                });
            }

            if (dto.Installments <= 0 || dto.Installments > 60)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Installments must be between 1 and 60",
                    Errors = new[] { "Invalid installment count" }
                });
            }

            if (dto.PaymentMode == "Cheque" && dto.ChequeDate == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cheque date required for cheque payment",
                    Errors = new[] { "Cheque date missing" }
                });
            }

            // ✅ Map DTO → Entity
            var loan = new LoanTaken
            {
                LoanDate = dto.LoanDate,
                LoanType = dto.LoanType,
                MemberId = dto.MemberId,
                LoanAmount = dto.LoanAmount,
                PreviousLoan = dto.PreviousLoan,
                Installments = dto.Installments,
                Purpose = dto.Purpose ?? string.Empty,
                AuthorizedBy = dto.AuthorizedBy ?? string.Empty,
                PaymentMode = dto.PaymentMode,
                Bank = dto.Bank,
                ChequeNo = dto.ChequeNo,
                ChequeDate = dto.ChequeDate,
                NetLoan = dto.NetLoan,
                InstallmentAmount = dto.InstallmentAmount,
                NewLoanShare = dto.NewLoanShare,
                PayAmount = dto.PayAmount,
                CreatedAt = DateTime.UtcNow,
                Status = "Active"
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            // ✅ Map Entity → Response DTO
            var response = new LoanTakenResponseDto
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                LoanType = loan.LoanType,
                MemberId = loan.MemberId,
                LoanAmount = loan.LoanAmount,
                PreviousLoan = loan.PreviousLoan,
                Installments = loan.Installments,
                Purpose = loan.Purpose,
                AuthorizedBy = loan.AuthorizedBy,
                PaymentMode = loan.PaymentMode,
                Bank = loan.Bank,
                ChequeNo = loan.ChequeNo,
                ChequeDate = loan.ChequeDate,
                Status = loan.Status,
                NetLoan = loan.NetLoan,
                InstallmentAmount = loan.InstallmentAmount,
                NewLoanShare = loan.NewLoanShare,
                PayAmount = loan.PayAmount,
                CreatedAt = loan.CreatedAt
            };

            return Ok(new ApiResponse<LoanTakenResponseDto>
            {
                Success = true,
                Message = "Loan created successfully",
                Data = response
            });
        }

        // ---------------- GET ALL LOANS ----------------
        [HttpGet]
        public async Task<IActionResult> GetLoans()
        {
            var loans = await _context.Loans
                .Select(l => new LoanTakenResponseDto
                {
                    Id = l.Id,
                    LoanDate = l.LoanDate,
                    LoanType = l.LoanType,
                    MemberId = l.MemberId,
                    LoanAmount = l.LoanAmount,
                    PreviousLoan = l.PreviousLoan,
                    Installments = l.Installments,
                    Purpose = l.Purpose,
                    AuthorizedBy = l.AuthorizedBy,
                    PaymentMode = l.PaymentMode,
                    Bank = l.Bank,
                    ChequeNo = l.ChequeNo,
                    ChequeDate = l.ChequeDate,
                    Status = l.Status,
                    NetLoan = l.NetLoan,
                    InstallmentAmount = l.InstallmentAmount,
                    NewLoanShare = l.NewLoanShare,
                    PayAmount = l.PayAmount,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<LoanTakenResponseDto>>
            {
                Success = true,
                Message = "Loans fetched successfully",
                Data = loans
            });
        }
    }
}
