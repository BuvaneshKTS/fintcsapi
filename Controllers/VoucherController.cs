// Controllers/VoucherController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;

namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VoucherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _context.Vouchers.Include(v => v.Ledger).ToListAsync();
            return Ok(new { success = true, data = vouchers });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Voucher created", data = voucher });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, [FromBody] Voucher updated)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            voucher.VoucherType = updated.VoucherType;
            voucher.Date = updated.Date;
            voucher.Particulars = updated.Particulars;
            voucher.Debit = updated.Debit;
            voucher.Credit = updated.Credit;
            voucher.LedgerId = updated.LedgerId;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Voucher updated", data = voucher });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Voucher deleted" });
        }
    }
}
