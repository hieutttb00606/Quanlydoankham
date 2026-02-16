using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanKham.API.Data;
using QuanLyDoanKham.API.DTOs;
using QuanLyDoanKham.API.Models;
using QuanLyDoanKham.API.Helpers;

namespace QuanLyDoanKham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Staffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaffs()
        {
            return await _context.Staffs
                .Where(s => s.IsActive)
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    EmployeeCode = s.EmployeeCode,
                    FullName = s.FullName,
                    FullNameUnsigned = s.FullNameUnsigned,
                    BirthYear = s.BirthYear,
                    Gender = s.Gender,
                    IDCardNumber = s.IDCardNumber,
                    TaxCode = s.TaxCode,
                    BankAccountNumber = s.BankAccountNumber,
                    BankAccountName = s.BankAccountName,
                    BankName = s.BankName,
                    PhoneNumber = s.PhoneNumber,
                    JobTitle = s.JobTitle,
                    Department = s.Department,
                    EmployeeType = s.EmployeeType,
                    IDCardFrontPath = s.IDCardFrontPath,
                    IDCardBackPath = s.IDCardBackPath,
                    PracticeCertificatePath = s.PracticeCertificatePath,
                    BaseSalary = s.BaseSalary,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        // GET: api/Staffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);

            if (staff == null || !staff.IsActive)
            {
                return NotFound();
            }

            return staff;
        }

        // POST: api/Staffs
        [HttpPost]
        public async Task<ActionResult<Staff>> PostStaff(StaffDto dto)
        {
            var staff = new Staff
            {
                EmployeeCode = string.IsNullOrEmpty(dto.EmployeeCode) ? $"NV{DateTime.Now:yyyyMMddHHmmss}" : dto.EmployeeCode,
                FullName = dto.FullName,
                FullNameUnsigned = StringHelper.RemoveVietnameseAccents(dto.FullName).ToUpper(),
                BirthYear = dto.BirthYear,
                Gender = dto.Gender,
                IDCardNumber = dto.IDCardNumber,
                TaxCode = dto.TaxCode,
                BankAccountNumber = dto.BankAccountNumber,
                BankAccountName = dto.BankAccountName,
                BankName = dto.BankName,
                PhoneNumber = dto.PhoneNumber,
                JobTitle = dto.JobTitle,
                Department = dto.Department,
                EmployeeType = dto.EmployeeType,
                IDCardFrontPath = dto.IDCardFrontPath,
                IDCardBackPath = dto.IDCardBackPath,
                PracticeCertificatePath = dto.PracticeCertificatePath,
                BaseSalary = dto.BaseSalary,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaff), new { id = staff.StaffId }, staff);
        }

        // PUT: api/Staffs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, StaffDto dto)
        {
            if (id != dto.StaffId)
            {
                return BadRequest();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            staff.EmployeeCode = dto.EmployeeCode;
            staff.FullName = dto.FullName;
            staff.FullNameUnsigned = StringHelper.RemoveVietnameseAccents(dto.FullName).ToUpper();
            staff.BirthYear = dto.BirthYear;
            staff.Gender = dto.Gender;
            staff.IDCardNumber = dto.IDCardNumber;
            staff.TaxCode = dto.TaxCode;
            staff.BankAccountNumber = dto.BankAccountNumber;
            staff.BankAccountName = dto.BankAccountName;
            staff.BankName = dto.BankName;
            staff.PhoneNumber = dto.PhoneNumber;
            staff.JobTitle = dto.JobTitle;
            staff.Department = dto.Department;
            staff.EmployeeType = dto.EmployeeType;
            staff.BaseSalary = dto.BaseSalary;
            staff.ModifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Staffs/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            staff.IsActive = false;
            staff.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(e => e.StaffId == id);
        }
    }
}
