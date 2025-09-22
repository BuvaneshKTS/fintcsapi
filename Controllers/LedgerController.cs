// Controllers/LedgerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;

namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LedgerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ledgers = await _context.Ledgers.Include(l => l.Member).ToListAsync();
            return Ok(new { success = true, data = ledgers });
        }

        [HttpPost]
        public async Task<IActionResult> CreateLedger([FromBody] Ledger ledger)
        {
            _context.Ledgers.Add(ledger);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Ledger created", data = ledger });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLedger(int id, [FromBody] Ledger updated)
        {
            var ledger = await _context.Ledgers.FindAsync(id);
            if (ledger == null)
                return NotFound();

            ledger.Name = updated.Name;
            ledger.Under = updated.Under;
            ledger.OpeningBalance = updated.OpeningBalance;
            ledger.BalanceType = updated.BalanceType;
            ledger.MemberId = updated.MemberId;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Ledger updated", data = ledger });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLedger(int id)
        {
            var ledger = await _context.Ledgers.FindAsync(id);
            if (ledger == null) return NotFound();

            _context.Ledgers.Remove(ledger);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Ledger deleted" });
        }
    }
}
