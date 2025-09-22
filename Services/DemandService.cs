// // Services/DemandService.cs
// using Microsoft.EntityFrameworkCore;
// using FintcsApi.Data;
// using FintcsApi.Models;
// using System.Text.Json;

// namespace FintcsApi.Services
// {
//     public class DemandService
//     {
//         private readonly AppDbContext _context;

//         public DemandService(AppDbContext context)
//         {
//             _context = context;
//         }

//         // ✅ Your existing method (unchanged)
//         public async Task<object?> GetFormattedSocietyAndMembersAsync()
//         {
//             var society = await _context.Societies.FirstOrDefaultAsync();
//             if (society == null) return null;

//             // 🔹 Deserialize loan types
//             var loanTypes = new List<LoanTypeDto>();
//             try
//             {
//                 if (!string.IsNullOrEmpty(society.LoanTypes))
//                     loanTypes = JsonSerializer.Deserialize<List<LoanTypeDto>>(society.LoanTypes) ?? new List<LoanTypeDto>();
//             }
//             catch
//             {
//                 loanTypes = new List<LoanTypeDto>();
//             }

//             // 🔹 Get all active members
//             var members = await _context.Members
//                 .Where(m => m.Status == "Active")
//                 .OrderBy(m => m.MemNo)
//                 .ToListAsync();

//             var memberResponses = members.Select(m => new MemberResponseDto
//             {
//                 Id = m.Id,
//                 MemNo = m.MemNo,
//                 Name = m.Name,
//                 FHName = m.FHName,
//                 OfficeAddress = m.OfficeAddress,
//                 City = m.City,
//                 cdAmount = m.cdAmount,
//                 Email2 = m.Email2,
//                 Pincode = m.Pincode,
//                 Mobile2 = m.Mobile2,
//                 PhoneOffice = m.PhoneOffice,
//                 Branch = m.Branch,
//                 PhoneRes = m.PhoneRes,
//                 Mobile = m.Mobile,
//                 Designation = m.Designation,
//                 ResidenceAddress = m.ResidenceAddress,
//                 DOB = m.DOB,
//                 DOJSociety = m.DOJSociety,
//                 Email = m.Email,
//                 DOJOrg = m.DOJOrg,
//                 DOR = m.DOR,
//                 Nominee = m.Nominee,
//                 NomineeRelation = m.NomineeRelation,
//                 BankingDetails = JsonSerializer.Deserialize<BankingDetailsDto>(m.BankingDetails) ?? new BankingDetailsDto(),
//                 IsPendingApproval = m.IsPendingApproval,
//                 CreatedAt = m.CreatedAt,
//                 UpdatedAt = m.UpdatedAt,
//                 Status = m.Status
//             }).ToList();

//             // 🔹 Get active loans
//             var activeLoans = await _context.Loans
//                 .Where(l => l.Status == "Active")
//                 .OrderBy(l => l.LoanNo)
//                 .ToListAsync();

//             var loanResponses = activeLoans.Select(l => new
//             {
//                 l.Id,
//                 l.LoanNo,
//                 l.LoanDate,
//                 l.LoanType,
//                 l.MemberNo,
//                 l.LoanAmount,
//                 l.Installments,
//                 l.Purpose,
//                 l.Status,
//                 l.NetLoan,
//                 l.InstallmentAmount,
//                 l.PayAmount,
//                 l.CreatedAt
//             }).ToList();

//             // 🔹 Build monthly demand table
//             var demandTable = members.Select(m =>
//             {
//                 var loansForMember = activeLoans.Where(l => l.MemberNo == m.MemNo).ToList();

//                 var row = new Dictionary<string, object?>
//                 {
//                     ["MemberNo"] = m.MemNo,
//                     ["MemberName"] = m.Name,
//                     ["CDAmount"] = m.cdAmount ?? "0"
//                 };

//                 decimal totalInstallments = 0;
//                 decimal totalInterest = 0;
//                 decimal overallLoanTotal = 0;

//                 foreach (var lt in loanTypes)
//                 {
//                     var loan = loansForMember.FirstOrDefault(l => l.LoanType == lt.LoanType);

//                     decimal loanAmount = loan?.LoanAmount ?? 0;
//                     decimal installment = (loan != null && loan.Installments > 0)
//                         ? loan.LoanAmount / loan.Installments
//                         : 0;
//                     decimal interest = installment * (lt.Interest / 100m);

//                     row[$"{lt.LoanType}"] = loanAmount;
//                     row[$"{lt.LoanType}Installment"] = Math.Round(installment, 2);
//                     row[$"{lt.LoanType}Interest"] = Math.Round(interest, 2);

//                     totalInstallments += installment;
//                     totalInterest += interest;
//                     overallLoanTotal += loanAmount;
//                 }

//                 var cd = Convert.ToDecimal(m.cdAmount ?? "0");
//                 var netDeduction = cd + totalInstallments + totalInterest;

//                 row["OverallLoanTotal"] = overallLoanTotal;
//                 row["InstallmentSum"] = Math.Round(totalInstallments, 2);
//                 row["InterestSum"] = Math.Round(totalInterest, 2);
//                 row["NetDeduction"] = Math.Round(netDeduction, 2);
//                 row["PenalInterest"] = 0m;
//                 row["PenalDeduction"] = 0m;
//                 row["TotalPayable"] = Math.Round(netDeduction, 2);
//                 row["DueDate"] = loansForMember.FirstOrDefault()?.LoanDate.Day + "th (monthly)";

//                 return row;
//             }).ToList();

//             // 🔹 Final response includes Society, Members, Loans, Demand
//             return new
//             {
//                 Demand = demandTable,
//                 Society = new
//                 {
//                     society.Id,
//                     society.SocietyName,
//                     society.Address,
//                     society.City,
//                     society.Phone,
//                     society.Fax,
//                     society.Email,
//                     society.Website,
//                     society.RegistrationNumber,
//                     society.chBounceCharge,
//                     society.targetDropdown,
//                     LoanTypes = loanTypes,
//                     society.IsPendingApproval,
//                     society.PendingChanges,
//                     society.CreatedAt,
//                     society.UpdatedAt
//                 },
//                 Members = memberResponses,
//                 Loans = loanResponses                
//             };
//         }

//         // ✅ New: Fetch demand by month/year
//         public async Task<List<object>> GetDemandByMonthAsync(int month, int year)
//         {
//             var demands = await _context.Demands
//                 .Where(d => d.Month == month && d.Year == year)
//                 .ToListAsync();

//             return demands.Select(d => new
//             {
//                 d.Id,
//                 d.Month,
//                 d.Year,
//                 Data = JsonSerializer.Deserialize<object>(d.Data),
//                 d.CreatedAt
//             }).Cast<object>().ToList();
//         }


//         // ✅ New: Create demand if not exists and month is consecutive
//         public async Task<object> CreateDemandAsync(int month, int year, string jsonData)
//         {
//             var exists = await _context.Demands.AnyAsync(d => d.Month == month && d.Year == year);
//             if (exists)
//             {
//                 return new { Success = false, Message = $"Demand already exists for {month}/{year}" };
//             }

//             var lastDemand = await _context.Demands
//                 .OrderByDescending(d => d.Year)
//                 .ThenByDescending(d => d.Month)
//                 .FirstOrDefaultAsync();

//             if (lastDemand != null)
//             {
//                 var expectedNextMonth = lastDemand.Month == 12 ? 1 : lastDemand.Month + 1;
//                 var expectedNextYear = lastDemand.Month == 12 ? lastDemand.Year + 1 : lastDemand.Year;

//                 if (month != expectedNextMonth || year != expectedNextYear)
//                 {
//                     return new { Success = false, Message = $"Next demand should be {expectedNextMonth}/{expectedNextYear}" };
//                 }
//             }

//             var newDemand = new Demand
//             {
//                 Month = month,
//                 Year = year,
//                 Data = jsonData,
//                 CreatedAt = DateTime.UtcNow
//             };

//             _context.Demands.Add(newDemand);
//             await _context.SaveChangesAsync();

//             return new { Success = true, Message = $"Demand generated for {month}/{year}", Data = newDemand };
//         }

//         // ✅ New: Delete demand
//         public async Task<object> DeleteDemandAsync(int id)
//         {
//             var demand = await _context.Demands.FindAsync(id);
//             if (demand == null)
//             {
//                 return new { Success = false, Message = "Demand not found" };
//             }

//             _context.Demands.Remove(demand);
//             await _context.SaveChangesAsync();

//             return new { Success = true, Message = "Demand deleted successfully" };
//         }

//         // ✅ New: Fetch all demands
//         public async Task<List<object>> GetAllDemandsAsync()
//         {
//             var demands = await _context.Demands
//                 .OrderByDescending(d => d.Year)
//                 .ThenByDescending(d => d.Month)
//                 .ToListAsync();

//             return demands.Select(d => new
//             {
//                 d.Id,
//                 d.Month,
//                 d.Year,
//                 Data = JsonSerializer.Deserialize<object>(d.Data),
//                 d.CreatedAt
//             }).Cast<object>().ToList();
//         }

//     }
// }
