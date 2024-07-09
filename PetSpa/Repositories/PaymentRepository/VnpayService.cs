using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.DTO.PaymentDTO;

namespace PetSpa.Repositories.PaymentRepository
{
    public class VnpayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly PetSpaContext _context;

        public VnpayService(IConfiguration configuration, PetSpaContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string transactionId)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", string.IsNullOrEmpty(model.ReturnUrl) ? urlCallBack : model.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", transactionId);
            if (_configuration.GetValue<int>("Vnpay:TransactionTimeout") > 0)
            {
                var timeoutMinutes = _configuration.GetValue<int>("Vnpay:TransactionTimeout");
                pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(timeoutMinutes).ToString("yyyyMMddHHmmss"));
            }
            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            var payment = _context.Payments.FirstOrDefault(p => p.TransactionId == response.TransactionId);

            if (payment != null)
            {
                response.PaymentId = payment.PaymentId.ToString();
            }

            return response;
        }
    }
}
