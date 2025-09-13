// Services/LoanTakenService.cs
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;

namespace FintcsApi.Services
{
    public class LoanTakenService
    {
        private readonly AppDbContext _context;

        public LoanTakenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetMembersAsync()
        {
            return await _context.Members
                .Select(m => new { m.Id, m.MemNo, m.Name })
                .ToListAsync<object>();
        }

        public async Task<LoanTaken> CreateLoanAsync(LoanTaken loan)
        {
            if (string.IsNullOrEmpty(loan.LoanNo))
            {
                loan.LoanNo = await GenerateLoanNoAsync();
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<List<LoanTaken>> GetLoansAsync()
        {
            return await _context.Loans.ToListAsync();
        }

        public async Task<string> GenerateLoanNoAsync()
        {
            var lastLoan = await _context.Loans
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();

            if (lastLoan == null || string.IsNullOrEmpty(lastLoan.LoanNo))
            {
                return "Loan_001";
            }

            var lastNumberStr = lastLoan.LoanNo.Split('_').Last();
            if (!int.TryParse(lastNumberStr, out int lastNumber))
            {
                lastNumber = 0;
            }

            return $"Loan_{(lastNumber + 1):D3}";
        }
    }
}
