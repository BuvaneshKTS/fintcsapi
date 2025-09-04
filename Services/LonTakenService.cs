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
            loan.NetLoan = loan.LoanAmount + loan.PreviousLoan;
            loan.InstallmentAmount = Math.Round(((loan.LoanAmount * 0.07m) + loan.LoanAmount) / loan.Installments, 2);
            var requiredShare = loan.LoanAmount * 0.1m;
            loan.NewLoanShare = requiredShare;
            loan.PayAmount = (loan.LoanAmount - loan.PreviousLoan) - loan.NewLoanShare;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<List<LoanTaken>> GetLoansAsync()
        {
            return await _context.Loans.ToListAsync();
        }
    }
}
