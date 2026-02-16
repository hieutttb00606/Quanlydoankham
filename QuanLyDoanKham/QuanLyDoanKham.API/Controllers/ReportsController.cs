using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanKham.API.Data;

namespace QuanLyDoanKham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ Admin/Manager được xem báo cáo
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try {
                var totalContracts = await _context.Contracts.CountAsync();
                var totalRevenue = await _context.Contracts.SumAsync(c => c.TotalAmount);
                var activeGroups = await _context.MedicalGroups.CountAsync();
                
                // 1. Staff participating (Unique IDs)
                var staffParticipating = await _context.GroupStaffDetails.Select(d => d.StaffId).Distinct().CountAsync();

                // 2. Supplies Exported (Total Qty)
                var suppliesExported = await _context.GroupSupplyDetails.SumAsync(d => d.QuantityUsed);

                // 3. Costs
                var totalSalaryCost = await _context.GroupStaffDetails.SumAsync(d => d.CalculatedSalary);
                var supplyCost = await _context.GroupSupplyDetails
                    .Include(d => d.Supply)
                    .Where(d => !d.Supply.IsFixedAsset)
                    .SumAsync(d => (d.QuantityUsed - d.ReturnQuantity) * d.Supply.UnitPrice);

                var profit = totalRevenue - totalSalaryCost - supplyCost;

                // 4. Detailed data for tables
                var topSupplies = await _context.GroupSupplyDetails
                    .Include(d => d.Supply)
                    .GroupBy(d => d.Supply.SupplyName)
                    .Select(g => new { 
                        Name = g.Key, 
                        Qty = g.Sum(x => x.QuantityUsed),
                        Cost = g.Sum(x => x.QuantityUsed * x.Supply.UnitPrice)
                    })
                    .OrderByDescending(x => x.Qty)
                    .Take(5)
                    .ToListAsync();

                var groupStats = await _context.MedicalGroups
                    .Include(g => g.Contract)
                    .ThenInclude(c => c.Company)
                    .Select(g => new {
                        g.GroupName,
                        CompanyName = g.Contract.Company.CompanyName,
                        StaffCount = _context.GroupStaffDetails.Count(d => d.GroupId == g.GroupId),
                        TotalCost = _context.GroupStaffDetails.Where(d => d.GroupId == g.GroupId).Sum(d => d.CalculatedSalary) +
                                    _context.GroupSupplyDetails.Where(d => d.GroupId == g.GroupId && !d.Supply.IsFixedAsset)
                                        .Sum(d => (d.QuantityUsed - d.ReturnQuantity) * d.Supply.UnitPrice)
                    })
                    .OrderByDescending(g => g.TotalCost)
                    .ToListAsync();

                return Ok(new
                {
                    totalContracts,
                    totalRevenue,
                    totalSalaryCost,
                    totalSupplyCost = supplyCost,
                    netProfit = profit,
                    activeGroups,
                    staffParticipating,
                    suppliesExported,
                    topSupplies,
                    groupStats
                });
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
