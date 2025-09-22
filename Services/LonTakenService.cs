// Services/LoanTakenService.cs
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintcsApi.Services
{
    public class LoanTakenService
    {
        private readonly AppDbContext _context;

        public LoanTakenService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of members (Id, Name only)
        /// </summary>
        public async Task<List<MemberListDto>> GetMembersAsync()
        {
            return await _context.Members
                .Select(m => new MemberListDto 
                { 
                    Id = m.Id, 
                    Name = m.Name 
                })
                .ToListAsync();
        }

        /// <summary>
        /// Create a new loan entry
        /// </summary>
        public async Task<LoanTaken> CreateLoanAsync(LoanTaken loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        /// <summary>
        /// Get all loans
        /// </summary>
        public async Task<List<LoanTaken>> GetLoansAsync()
        {
            return await _context.Loans.ToListAsync();
        }
    }

    /// <summary>
    /// DTO for lightweight member listing
    /// </summary>
    public class MemberListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
