using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly PetSpaContext _context;

        public VouchersController(PetSpaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _context.Vouchers.ToListAsync();
            return Ok(vouchers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return Ok(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            if (voucher == null)
            {
                return BadRequest();
            }

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.VoucherId }, voucher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] Voucher updatedVoucher)
        {
            if (id != updatedVoucher.VoucherId || updatedVoucher == null)
            {
                return BadRequest();
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            voucher.Code = updatedVoucher.Code;
            voucher.Discount = updatedVoucher.Discount;
            voucher.ExpiryDate = updatedVoucher.ExpiryDate;
            voucher.IssueDate = updatedVoucher.IssueDate;
            voucher.Status = updatedVoucher.Status;

            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
