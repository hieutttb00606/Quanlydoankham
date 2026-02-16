using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoanKham.API.Data;
using QuanLyDoanKham.API.DTOs;
using QuanLyDoanKham.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyDoanKham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Patients/by-contract/5
        [HttpGet("by-contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatientsByContract(int contractId)
        {
            var patients = await _context.Patients
                .Where(p => p.HealthContractId == contractId)
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,
                    HealthContractId = p.HealthContractId,
                    FullName = p.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    IDCardNumber = p.IDCardNumber,
                    PhoneNumber = p.PhoneNumber,
                    Department = p.Department,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();

            return Ok(patients);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            return Ok(new PatientDto
            {
                PatientId = patient.PatientId,
                HealthContractId = patient.HealthContractId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                IDCardNumber = patient.IDCardNumber,
                PhoneNumber = patient.PhoneNumber,
                Department = patient.Department,
                CreatedDate = patient.CreatedDate
            });
        }

        // POST: api/Patients
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Patient>> CreatePatient(PatientDto dto)
        {
            var patient = new Patient
            {
                HealthContractId = dto.HealthContractId,
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                IDCardNumber = dto.IDCardNumber,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                CreatedDate = DateTime.Now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        // POST: api/Patients/5/exam-result
        [HttpPost("{id}/exam-result")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExamResult>> AddExamResult(int id, ExamResultDto dto)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound("Patient not found");

            var examResult = new ExamResult
            {
                PatientId = id,
                GroupId = dto.GroupId,
                ExamType = dto.ExamType,
                Result = dto.Result,
                Diagnosis = dto.Diagnosis,
                DoctorStaffId = dto.DoctorStaffId,
                ExamDate = DateTime.Now
            };

            _context.ExamResults.Add(examResult);
            await _context.SaveChangesAsync();

            return Ok(examResult);
        }

        // GET: api/Patients/5/exam-results
        [HttpGet("{id}/exam-results")]
        public async Task<ActionResult<IEnumerable<ExamResultDto>>> GetExamResults(int id)
        {
            var results = await _context.ExamResults
                .Where(e => e.PatientId == id)
                .Include(e => e.DoctorStaff)
                .Select(e => new ExamResultDto
                {
                    ExamResultId = e.ExamResultId,
                    PatientId = e.PatientId,
                    GroupId = e.GroupId,
                    ExamType = e.ExamType,
                    Result = e.Result,
                    Diagnosis = e.Diagnosis,
                    DoctorStaffId = e.DoctorStaffId,
                    DoctorName = e.DoctorStaff != null ? e.DoctorStaff.FullName : null,
                    ExamDate = e.ExamDate
                })
                .ToListAsync();

            return Ok(results);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
