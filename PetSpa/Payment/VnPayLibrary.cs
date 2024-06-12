using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PetSpa.Payment
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (data.Length > 0)
                {
                    data.Append('&');
                }
                data.Append(HttpUtility.UrlEncode(kv.Key));
                data.Append('=');
                data.Append(HttpUtility.UrlEncode(kv.Value));
            }

            var queryString = data.ToString();
            var signData = $"{queryString}{hashSecret}";
            var vnp_SecureHash = ComputeHash(signData);
            var paymentUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";

            return paymentUrl;
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
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
