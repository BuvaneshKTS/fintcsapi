// ================================
// File: Controllers/MemberController.cs
// ================================
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
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all members
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _context.Members.ToListAsync();
            return Ok(new { success = true, data = members });
        }

        // ✅ Get member by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound(new { success = false, message = "Member not found" });

            return Ok(new { success = true, data = member });
        }

        // ✅ Create new member
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] Member member)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            member.CreatedAt = DateTime.UtcNow;
            member.UpdatedAt = DateTime.UtcNow;

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Member created successfully", data = member });
        }

        // ✅ Update member
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] Member updatedMember)
        {
            // i want to see the updatedMember object in console here
            Console.WriteLine($"Received update for member {id}: {System.Text.Json.JsonSerializer.Serialize(updatedMember)}");
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound(new { success = false, message = "Member not found" });

            // Update fields one by one (except Id, CreatedAt)
            member.Name = updatedMember.Name;
            member.FHName = updatedMember.FHName;
            member.Mobile = updatedMember.Mobile;
            member.Email = updatedMember.Email;
            member.Status = updatedMember.Status;
            member.OfficeAddress = updatedMember.OfficeAddress;
            member.City = updatedMember.City;
            member.PhoneOffice = updatedMember.PhoneOffice;
            member.Branch = updatedMember.Branch;
            member.PhoneRes = updatedMember.PhoneRes;
            member.Designation = updatedMember.Designation;
            member.ResidenceAddress = updatedMember.ResidenceAddress;
            member.DOB = updatedMember.DOB;
            member.DOJSociety = updatedMember.DOJSociety;
            member.DOR = updatedMember.DOR;
            member.Nominee = updatedMember.Nominee;
            member.NomineeRelation = updatedMember.NomineeRelation;
            member.cdAmount = updatedMember.cdAmount;
            member.Email2 = updatedMember.Email2;
            member.Mobile2 = updatedMember.Mobile2;
            member.Pincode = updatedMember.Pincode;
            member.BankName = updatedMember.BankName;
            member.AccountNumber = updatedMember.AccountNumber;
            member.PayableAt = updatedMember.PayableAt;
            member.SocietyId = updatedMember.SocietyId;
            member.Share = updatedMember.Share;

            // Only UpdatedAt changes
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Member updated successfully", data = member });
        }


        // ✅ Delete member
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound(new { success = false, message = "Member not found" });

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Member deleted successfully" });
        }
    }
}