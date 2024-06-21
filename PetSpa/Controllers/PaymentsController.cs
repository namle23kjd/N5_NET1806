using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.PaymentDTO;
using PetSpa.Repositories.PaymentRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly PetSpaContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IVnPayService vnPayService, PetSpaContext context, IConfiguration configuration, ILogger<PaymentsController> logger)
        {
            _vnPayService = vnPayService;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentInformationModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var transactionId = DateTime.Now.Ticks.ToString();

                    // Tạo Payment entity
                    var payment = new Payment
                    {
                        TransactionId = transactionId, // Sử dụng tick làm TransactionId
                        CreatedDate = DateTime.UtcNow,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(3),
                        PaymentMethod = "VNPay"
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync(); // Lưu Payment trước để lấy PaymentId

                    // Cập nhật trạng thái của các booking
                    foreach (var bookingId in model.BookingIds)
                    {
                        var booking = await _context.Bookings.Include(b => b.BookingDetails)
                                                             .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                        if (booking == null)
                        {
                            return NotFound($"Booking with ID {bookingId} not found");
                        }

                        booking.PaymentId = payment.PaymentId;
                        booking.PaymentStatus = false;
                    }

                    await _context.SaveChangesAsync(); // Lưu Booking sau khi cập nhật PaymentId

                    var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext, transactionId);

                    // Commit transaction
                    await transaction.CommitAsync();

                    return Ok(new { PaymentUrl = paymentUrl, PaymentId = payment.PaymentId });
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }


        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            _logger.LogInformation("Payment callback received with response: {@Response}", response);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Xử lý cập nhật trạng thái thanh toán
                        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);
                        if (payment != null)
                        {
                            payment.PaymentMethod = response.PaymentMethod;

                            var bookings = await _context.Bookings.Include(b => b.BookingDetails)
                                                                  .Where(b => b.PaymentId == payment.PaymentId)
                                                                  .ToListAsync();
                            if (bookings.Any())
                            {
                                foreach (var booking in bookings)
                                {
                                    booking.PaymentStatus = true; // Đặt trạng thái là true khi thanh toán thành công
                                }
                                await _context.SaveChangesAsync();
                            }

                            await transaction.CommitAsync();
                            return Ok(new { PaymentId = payment.PaymentId, TransactionId = payment.TransactionId });
                        }
                        else
                        {
                            _logger.LogError("Payment not found for TransactionId: {TransactionId}", response.TransactionId);
                            return BadRequest("Payment not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error processing successful payment callback");
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

            //using (var transaction = await _context.Database.BeginTransactionAsync())
            //{
            //    try
            //    {
            //        var failedPayment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);
            //        if (failedPayment != null)
            //        {
            //            var failedBookings = await _context.Bookings.Include(b => b.BookingDetails)
            //                                                        .Where(b => b.PaymentId == failedPayment.PaymentId)
            //                                                        .ToListAsync();
            //            if (failedBookings.Any())
            //            {
            //                foreach (var failedBooking in failedBookings)
            //                {
            //                    _context.BookingDetails.RemoveRange(failedBooking.BookingDetails);
            //                    _context.Bookings.Remove(failedBooking);
            //                }
            //            }
            //            _context.Payments.Remove(failedPayment);
            //            await _context.SaveChangesAsync();
            //        }

            //        await transaction.CommitAsync();
            //        return BadRequest(response);
            //    }
            //    catch (Exception ex)
            //    {
            //        await transaction.RollbackAsync();
            //        return StatusCode(500, $"Internal server error: {ex.Message}");
            //    }
            //}
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var failedPayment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);
                    if (failedPayment != null)
                    {
                        var failedBookings = await _context.Bookings.Include(b => b.BookingDetails)
                                                                    .Where(b => b.PaymentId == failedPayment.PaymentId)
                                                                    .ToListAsync();
                        if (failedBookings.Any())
                        {
                            foreach (var failedBooking in failedBookings)
                            {
                                _context.BookingDetails.RemoveRange(failedBooking.BookingDetails);
                                _context.Bookings.Remove(failedBooking);
                            }
                        }
                        _context.Payments.Remove(failedPayment);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return BadRequest(new { PaymentId = failedPayment.PaymentId, TransactionId = failedPayment.TransactionId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
        [HttpDelete("{paymentId:int}")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
            {
                return NotFound($"Payment with ID {paymentId} not found");
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok("Payment and related bookings deleted successfully");
        }

    }
}

