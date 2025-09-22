// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using FintcsApi.Services;
// using System.Text.Json;

// namespace FintcsApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     [Authorize]
//     public class DemandController : ControllerBase
//     {
//         private readonly DemandService _demandService;

//         public DemandController(DemandService demandService)
//         {
//             _demandService = demandService;
//         }

//         // ✅ Get all saved demands
//         [HttpGet("all")]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> GetAllDemands()
//         {
//             var demands = await _demandService.GetAllDemandsAsync();
//             if (demands == null || !demands.Any())
//             {
//                 return NotFound(new
//                 {
//                     Success = false,
//                     Message = "No demands found"
//                 });
//             }

//             return Ok(new
//             {
//                 Success = true,
//                 Message = "All demands fetched successfully",
//                 Data = demands
//             });
//         }


//         // ✅ Existing endpoint (kept as-is)
//         [HttpGet("preview")]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> GetSocietyAndMembers()
//         {
//             var result = await _demandService.GetFormattedSocietyAndMembersAsync();
//             if (result == null)
//             {
//                 return NotFound(new
//                 {
//                     Success = false,
//                     Message = "Society details not found"
//                 });
//             }

//             return Ok(new
//             {
//                 Success = true,
//                 Message = "Society details fetched successfully",
//                 Data = result
//             });
//         }

//         // ✅ Get saved demand by month/year
//         [HttpGet]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> GetDemand([FromQuery] int month, [FromQuery] int year)
//         {
//             var demands = await _demandService.GetDemandByMonthAsync(month, year);
//             if (demands == null || !demands.Any())
//             {
//                 return NotFound(new
//                 {
//                     Success = false,
//                     Message = $"No demand found for {month}/{year}"
//                 });
//             }

//             return Ok(new
//             {
//                 Success = true,
//                 Message = $"Demand for {month}/{year} fetched successfully",
//                 Data = demands
//             });
//         }

//         // ✅ Create demand for a month/year
//         [HttpPost]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> CreateDemand([FromBody] DemandRequestDto request)
//         {
//             // generate preview demand first
//             var preview = await _demandService.GetFormattedSocietyAndMembersAsync();
//             var jsonData = JsonSerializer.Serialize(preview);

//             var result = await _demandService.CreateDemandAsync(request.Month, request.Year, jsonData);
//             return Ok(result);
//         }

//         // ✅ Delete a demand by id
//         [HttpDelete("{id}")]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> DeleteDemand(int id)
//         {
//             var result = await _demandService.DeleteDemandAsync(id);
//             return Ok(result);
//         }
//     }

//     // DTO for demand request
//     public class DemandRequestDto
//     {
//         public int Month { get; set; }
//         public int Year { get; set; }
//     }
// }
