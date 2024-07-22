using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
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
                    // Kiểm tra xem CusId có hợp lệ không
                    var customer = await _context.Customers.FindAsync(model.CusId);
                    if (customer == null)
                    {
                        return NotFound($"Customer with ID {model.CusId} not found");
                    }

                    // Tạo Payment entity
                    var payment = new Payment
                    {
                        TransactionId = transactionId, // Sử dụng tick làm TransactionId
                        CreatedDate = DateTime.UtcNow,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                        PaymentMethod = "VNPay",
                        CusId = model.CusId, // Gán CusId vào Payment
                        TotalPayment = Convert.ToDecimal(model.Amount) // Khởi tạo TotalPayment bằng 0
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync(); // Lưu Payment trước để lấy PaymentId

                    //decimal totalAmount = 0m;

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
                        _context.Bookings.Update(booking); // Cập nhật booking
                    }

                    customer.TotalSpent += payment.TotalPayment ?? 0m; // Cập nhật TotalSpent của khách hàng
                    customer.UpdateCusRank(); // Cập nhật CusRank của khách hàng nếu cần
                    _context.Customers.Update(customer); // Cập nhật khách hàng trong cơ sở dữ liệu

                    await _context.SaveChangesAsync(); // Lưu Booking sau khi cập nhật PaymentId và Payment sau khi cập nhật TotalPayment

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
                        _logger.LogInformation("Searching for payment with TransactionId: {TransactionId}", response.TransactionId);
                        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);
                        if (payment != null)
                        {
                            _logger.LogInformation("Payment found: {PaymentId}", payment.PaymentId);
                            payment.PaymentMethod = response.PaymentMethod;

                            var bookings = await _context.Bookings.Include(b => b.BookingDetails)
                                                                  .Where(b => b.PaymentId == payment.PaymentId)
                                                                  .ToListAsync();
                            if (bookings.Any())
                            {
                                _logger.LogInformation("Bookings found for PaymentId: {PaymentId}", payment.PaymentId);

                                // Tính tổng số tiền đã chi tiêu và cập nhật vào TotalSpent
                                var totalSpent = bookings.Sum(b => b.TotalAmount ?? 0);
                                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CusId == payment.CusId);
                                if (customer != null)
                                {
                                    _logger.LogInformation("Customer found: {CusId}", customer.CusId);
                                    customer.TotalSpent += totalSpent;
                                    customer.UpdateCusRank(); // Cập nhật CusRank nếu cần thiết
                                    _context.Customers.Update(customer);
                                }
                                await _context.SaveChangesAsync();
                            }

                            await transaction.CommitAsync();
                            var returnUrl = Request.Query["vnp_ReturnUrl"];
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                _logger.LogInformation("Redirecting to ReturnUrl: {ReturnUrl}", returnUrl);
                                return Redirect(returnUrl);
                            }
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
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error processing successful payment callback");
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

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

                        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CusId == failedPayment.CusId);
                        if (customer != null)
                        {
                            // Trừ tổng số tiền của failed payment khỏi TotalSpent của customer
                            customer.TotalSpent -= failedPayment.TotalPayment ?? 0;
                            customer.UpdateCusRank(); // Cập nhật CusRank nếu cần thiết
                            _context.Customers.Update(customer);
                        }

                        _context.Payments.Remove(failedPayment);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return BadRequest(new { PaymentId = failedPayment?.PaymentId, TransactionId = failedPayment?.TransactionId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error processing failed payment callback");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        [HttpDelete("transaction/{transactionId}")]
        public async Task<IActionResult> DeletePayment(string transactionId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
                    if (payment == null)
                    {
                        return NotFound($"Payment with ID {transactionId} not found");
                    }

                    // Tìm khách hàng tương ứng với payment này
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CusId == payment.CusId);
                    if (customer != null)
                    {
                        // Trừ số tiền của payment khỏi TotalSpent của customer
                        customer.TotalSpent -= payment.TotalPayment ?? 0;

                        // Cập nhật CusRank nếu cần thiết
                        customer.UpdateCusRank();

                        // Cập nhật customer trong context
                        _context.Customers.Update(customer);
                    }

                    // Xóa các bookings liên quan đến payment này
                    var relatedBookings = await _context.Bookings.Where(b => b.PaymentId == payment.PaymentId).ToListAsync();
                    if (relatedBookings.Any())
                    {
                        _context.Bookings.RemoveRange(relatedBookings);
                    }

                    _context.Payments.Remove(payment);

                    // Lưu các thay đổi vào database
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return Ok("Payment and related bookings deleted successfully");
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }


        [HttpGet("history-customerId")]
        public async Task<IActionResult> GetPaymentHistory(Guid CustomerId)
        {
            var customer = await _context.Customers.Include(c => c.Payments)
                                    .ThenInclude(p => p.Bookings)
                                        .ThenInclude(b => b.BookingDetails)
                                            .ThenInclude(bd => bd.Service)
                                    .Include(c => c.Payments)
                                        .ThenInclude(p => p.Bookings)
                                            .ThenInclude(b => b.BookingDetails)
                                                .ThenInclude(bd => bd.Combo)
                                    .FirstOrDefaultAsync(c => c.CusId == CustomerId);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            var paymentHistory = customer.Payments.Select(p => new PaymentHistoryDTO
            {
                CustomerName = customer.FullName,
                PaymentMethod = p.PaymentMethod,
                CreatedDate = p.CreatedDate,
                ExpirationTime = p.ExpirationTime,
                TotalAmount = p.Bookings.Sum(b => b.TotalAmount ?? 0),
                BookingDetails = p.Bookings.SelectMany(b => b.BookingDetails).Select(bd => new BookingDetailHistoryDTO
                {
                    BookingId = bd.BookingId,
                    PetId = bd.PetId,
                    ScheduleDate = bd.Booking.StartDate,
                    ComboId = bd.ComboId,
                    ServiceId = bd.ServiceId,
                    StaffId = bd.StaffId,
                    ServicePrice = bd.ServiceId.HasValue ? bd.Service.Price : bd.Combo.Price,
                    CheckAccept = bd.Booking.CheckAccept,
                    Status = bd.Booking.Status ?? BookingStatus.NotStarted,
                    Feedback = bd.Booking.Feedback,
                    BookingSchedule = bd.Booking.BookingSchedule
                }).ToList()
            }).ToList();

            return Ok(paymentHistory);
        }

        [HttpGet("get-all-payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Service)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Combo)
                .Select(p => new
                {
                    CustomerName = p.Customer.FullName,
                    PaymentId = p.PaymentId,
                    CreatedDate = p.CreatedDate,
                    TotalPayment = p.TotalPayment,
                    Bookings = p.Bookings.Select(b => new
                    {
                        BookingId = b.BookingId,
                        CheckAccept = b.CheckAccept,
                        Status = b.Status,
                        BookingDetails = b.BookingDetails.Select(bd => new
                        {
                            ServiceName = bd.ServiceId != null ? bd.Service.ServiceName : null,
                            ComboName = bd.ComboId != null ? bd.Combo.ComboType : null
                        })
                    })
                })
                .ToListAsync();

            if (payments == null || payments.Count == 0)
            {
                return NotFound("No payments found.");
            }

            return Ok(payments);
        }
    }
}
