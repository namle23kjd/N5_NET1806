using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace PetSpa.Payment
{
    public class VNPayService
    {
        private readonly VNPayConfig _config;

        public VNPayService(IOptions<VNPayConfig> config)
        {
            _config = config.Value;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string orderDescription)
        {
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderDescription);
            vnpay.AddRequestData("vnp_OrderType", "billpayment");
            vnpay.AddRequestData("vnp_ReturnUrl", _config.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId);

            var paymentUrl = vnpay.CreateRequestUrl(_config.VnpUrl, _config.HashSecret);
            return paymentUrl;
        }

        public bool ValidateSignature(IQueryCollection vnpayData)
        {
            var vnp_SecureHash = vnpayData["vnp_SecureHash"].ToString();
            var hashSecret = _config.HashSecret;
            var data = new StringBuilder();

            foreach (var key in vnpayData.Keys.OrderBy(key => key))
            {
                if (!key.Equals("vnp_SecureHash", StringComparison.InvariantCultureIgnoreCase))
                {
                    data.Append(key + "=" + vnpayData[key] + "&");
                }
            }

            var queryString = data.ToString().TrimEnd('&');
            var signData = queryString + hashSecret;
            var computedHash = ComputeHash(signData);

            return computedHash.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string ComputeHash(string data)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
