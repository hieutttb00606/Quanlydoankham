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
    public class GroupDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === STAFF LOGIC (1 CỦ CÒN 500K) ===

        // GET: api/GroupDetails/staff/{groupId}
        [HttpGet("staff/{groupId}")]
        public async Task<ActionResult<IEnumerable<GroupStaffItemDto>>> GetStaffByGroup(int groupId)
        {
            var list = await _context.GroupStaffDetails
                .Include(d => d.Staff)
                .Where(d => d.GroupId == groupId)
                .Select(d => new GroupStaffItemDto
                {
                    Id = d.Id,
                    StaffId = d.StaffId,
                    FullName = d.Staff.FullName,
                    JobTitle = d.Staff.JobTitle,
                    ShiftType = d.ShiftType,
                    CalculatedSalary = d.CalculatedSalary
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: api/GroupDetails/staff
        [HttpPost("staff")]
        public async Task<ActionResult<GroupStaffDetail>> AddStaffToGroup(AddStaffToGroupDto dto)
        {
            // Check if staff already allocated to this group
            var exists = await _context.GroupStaffDetails
                .AnyAsync(d => d.GroupId == dto.GroupId && d.StaffId == dto.StaffId);
            
            if (exists)
                return BadRequest("Staff already allocated to this group");

            // 1. Get Base Salary
            var staff = await _context.Staffs.FindAsync(dto.StaffId);
            if (staff == null) return NotFound("Staff not found");

            // 2. Calculate Logic "1 củ còn 500k"
            // ShiftType: 0.5 (Sáng/Chiều) -> Salary * 0.5
            // ShiftType: 1.0 (Cả ngày) -> Salary * 1.0
            decimal finalSalary = staff.BaseSalary * (decimal)dto.ShiftType;

            var detail = new GroupStaffDetail
            {
                GroupId = dto.GroupId,
                StaffId = dto.StaffId,
                ShiftType = dto.ShiftType,
                CalculatedSalary = finalSalary
            };

            _context.GroupStaffDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(detail);
        }

        // DELETE: api/GroupDetails/staff/{id}
        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> RemoveStaffFromGroup(int id)
        {
            var detail = await _context.GroupStaffDetails.FindAsync(id);
            if (detail == null) return NotFound();

            _context.GroupStaffDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // === SUPPLY LOGIC (KHO) ===

        // GET: api/GroupDetails/supply/{groupId}
        [HttpGet("supply/{groupId}")]
        public async Task<ActionResult<IEnumerable<GroupSupplyItemDto>>> GetSupplyByGroup(int groupId)
        {
            var list = await _context.GroupSupplyDetails
                .Include(d => d.Supply)
                .Where(d => d.GroupId == groupId)
                .Select(d => new GroupSupplyItemDto
                {
                    Id = d.Id,
                    SupplyId = d.SupplyId,
                    SupplyName = d.Supply.SupplyName,
                    QuantityUsed = d.QuantityUsed,
                    ReturnQuantity = d.ReturnQuantity,
                    // Chi phí = (Dùng - Trả) * Đơn giá. Nếu là TS Cố Định (FixedAsset) thì chi phí = 0 (chỉ mượn).
                    TotalCost = d.Supply.IsFixedAsset ? 0 : (d.QuantityUsed - d.ReturnQuantity) * d.Supply.UnitPrice
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: api/GroupDetails/supply (Xuất kho cho đoàn)
        [HttpPost("supply")]
        public async Task<ActionResult> AddSupplyToGroup(AddSupplyToGroupDto dto)
        {
            var supply = await _context.Supplies.FindAsync(dto.SupplyId);
            if (supply == null) return NotFound("Supply not found");

            // Check tồn kho
            if (supply.StockQuantity < dto.QuantityUsed)
                return BadRequest($"Not enough stock. Available: {supply.StockQuantity}");

            // Trừ kho
            supply.StockQuantity -= dto.QuantityUsed;

            var detail = new GroupSupplyDetail
            {
                GroupId = dto.GroupId,
                SupplyId = dto.SupplyId,
                QuantityUsed = dto.QuantityUsed,
                ReturnQuantity = 0
            };

            _context.GroupSupplyDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(detail);
        }

        // PUT: api/GroupDetails/supply/{id}/return (Nhập lại vật tư thừa)
        [HttpPut("supply/{id}/return")]
        public async Task<ActionResult> ReturnSupply(int id, [FromBody] ReturnSupplyDto dto)
        {
            var detail = await _context.GroupSupplyDetails
                .Include(d => d.Supply)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null) return NotFound();

            // Validate: cannot return more than (used - already returned)
            var maxReturn = detail.QuantityUsed - detail.ReturnQuantity;
            if (dto.ReturnQuantity > maxReturn)
                return BadRequest($"Cannot return more than {maxReturn} items");

            // Update số lượng trả (add to existing returns)
            detail.ReturnQuantity += dto.ReturnQuantity;

            // Cộng lại vào kho tổng
            detail.Supply.StockQuantity += dto.ReturnQuantity;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Returned successfully", NewStock = detail.Supply.StockQuantity });
        }
    }
}
