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
                .Select(m => new { m.Id, m.MemNo, m.Name })
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.MemNo == dto.MemberNo);
            if (member == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid member selected",
                    Errors = new[] { "Member does not exist" }
                });
            }

            // Backend validations
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

            // Auto calculations
            var loan = new LoanTaken
            {
                LoanNo = dto.LoanNo,
                LoanDate = dto.LoanDate,
                LoanType = dto.LoanType,
                CustomType = dto.CustomType,
                MemberNo = dto.MemberNo,
                LoanAmount = dto.LoanAmount,
                PreviousLoan = dto.PreviousLoan,
                Installments = dto.Installments,
                Purpose = dto.Purpose,
                AuthorizedBy = dto.AuthorizedBy,
                PaymentMode = dto.PaymentMode,
                Bank = dto.Bank,
                ChequeNo = dto.ChequeNo,
                ChequeDate = dto.ChequeDate
            };

            loan.NetLoan = loan.LoanAmount + loan.PreviousLoan;
            loan.InstallmentAmount = Math.Round(((loan.LoanAmount * 0.07m) + loan.LoanAmount) / loan.Installments, 2);
            var requiredShare = loan.LoanAmount * 0.1m;
            loan.NewLoanShare = requiredShare; // TODO: compare with member.Share if you add that
            loan.PayAmount = (loan.LoanAmount - loan.PreviousLoan) - loan.NewLoanShare;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            var response = new LoanTakenResponseDto
            {
                Id = loan.Id,
                LoanNo = loan.LoanNo,
                LoanDate = loan.LoanDate,
                LoanType = loan.LoanType,
                MemberNo = loan.MemberNo,
                LoanAmount = loan.LoanAmount,
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
                    LoanNo = l.LoanNo,
                    LoanDate = l.LoanDate,
                    LoanType = l.LoanType,
                    MemberNo = l.MemberNo,
                    LoanAmount = l.LoanAmount,
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
