using Microsoft.AspNetCore.Mvc;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Linq;

namespace PetSpa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly PetSpaContext _context;

        public InvoiceController(PetSpaContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetInvoice(Guid id)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] InvoiceRequest request)
        {
            if (_context.Bookings.Any(b => b.BookingId == request.BookingId) == false)
            {
                return BadRequest(new { Message = "Invalid BookingId" });
            }

            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                BookingId = request.BookingId,
                Price = request.Price
            };

            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(Guid id, [FromBody] InvoiceRequest request)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.BookingId = request.BookingId;
            invoice.Price = request.Price;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(Guid id)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class InvoiceRequest
    {
        public Guid BookingId { get; set; }
        public decimal Price { get; set; }
    }
}
