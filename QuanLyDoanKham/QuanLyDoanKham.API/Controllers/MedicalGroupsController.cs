using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanKham.API.Data;
using QuanLyDoanKham.API.DTOs;
using QuanLyDoanKham.API.Models;

namespace QuanLyDoanKham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicalGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MedicalGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalGroupDto>>> GetMedicalGroups()
        {
            // Simple logic: Get all. Filter by Customer normally implemented here too.
            var groups = await _context.MedicalGroups
                .Include(g => g.Contract)
                .ThenInclude(c => c.Company)
                .Select(g => new MedicalGroupDto
                {
                    GroupId = g.GroupId,
                    GroupName = g.GroupName,
                    ExamDate = g.ExamDate,
                    HealthContractId = g.HealthContractId,
                    CompanyName = g.Contract.Company.CompanyName
                })
                .ToListAsync();

            return Ok(groups);
        }

        // POST: api/MedicalGroups
        [HttpPost]
        public async Task<ActionResult<MedicalGroup>> PostMedicalGroup(MedicalGroupDto dto)
        {
            var entity = new MedicalGroup
            {
                GroupName = dto.GroupName,
                ExamDate = dto.ExamDate,
                HealthContractId = dto.HealthContractId
            };

            _context.MedicalGroups.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }
    }
}
