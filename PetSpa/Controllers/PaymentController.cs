using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Payment;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly VNPayService _vnPayService;
        private readonly PetSpaContext _context;

        public PaymentController(VNPayService vnPayService, PetSpaContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentRequest request)
        {
            var paymentUrl = _vnPayService.CreatePaymentUrl(request.OrderId, request.Amount, request.OrderDescription);

            // Tạo một bản ghi Payment trong cơ sở dữ liệu
            var payment = new PaymentT
            {
                Id = Guid.NewGuid(),
                InvoiceId = Guid.Parse(request.OrderId),
                RequiredAmount = request.Amount,
                PaidAmount = 0, // Ban đầu chưa thanh toán nên là 0
                CreatedAt = DateTime.Now
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            return Ok(new { PaymentUrl = paymentUrl });
        }

        [HttpGet("payment-return")]
        public IActionResult PaymentReturn()
        {
            var vnpayData = Request.Query;
            var isValid = _vnPayService.ValidateSignature(vnpayData);
            if (isValid)
            {
                // Xử lý logic khi thanh toán thành công
                var orderId = vnpayData["vnp_TxnRef"].ToString();
                var payment = _context.Payments.FirstOrDefault(p => p.InvoiceId.ToString() == orderId);
                if (payment != null)
                {
                    payment.PaidAmount = decimal.Parse(vnpayData["vnp_Amount"]) / 100; // Chuyển đổi số tiền từ đơn vị VNĐ về đơn vị nhỏ nhất
                    payment.Status = "Success"; // Cập nhật trạng thái thanh toán
                    _context.SaveChanges();
                }
                return Ok(new { Message = "Payment successful" });
            }
            else
            {
                // Xử lý logic khi thanh toán thất bại
                return BadRequest(new { Message = "Payment failed" });
            }
        }
    }
}
