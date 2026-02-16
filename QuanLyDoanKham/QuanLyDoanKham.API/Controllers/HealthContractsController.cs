using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanKham.API.Data;
using QuanLyDoanKham.API.DTOs;
using QuanLyDoanKham.API.Models;
using System.Security.Claims;

namespace QuanLyDoanKham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealthContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HealthContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HealthContracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthContractDto>>> GetContracts()
        {
            // RBAC Logic: 
            // - If Role = Customer -> Only show their contracts
            // - If Role = Staff/Admin -> Show all

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;

            IQueryable<HealthContract> query = _context.Contracts.Include(c => c.Company);

            if (role == "Customer" && !string.IsNullOrEmpty(companyIdClaim))
            {
                int companyId = int.Parse(companyIdClaim);
                query = query.Where(c => c.CompanyId == companyId);
            }

            var list = await query.Select(c => new HealthContractDto
            {
                HealthContractId = c.HealthContractId,
                CompanyId = c.CompanyId,
                CompanyName = c.Company != null ? c.Company.CompanyName : "Không xác định",
                TotalAmount = c.TotalAmount,
                PatientCount = c.PatientCount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsFinished = c.IsFinished
            }).ToListAsync();

            return Ok(list);
        }

        // POST: api/HealthContracts
        [HttpPost]
        public async Task<ActionResult<HealthContract>> PostContract(HealthContractDto dto)
        {
            // Validate: Only Staff/Admin can create contracts?
            // For now, allow all authenticated users for simplicity or check role
            
            var contract = new HealthContract
            {
                CompanyId = dto.CompanyId,
                TotalAmount = dto.TotalAmount,
                PatientCount = dto.PatientCount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsFinished = false
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return Ok(contract);
        }
    }
}
