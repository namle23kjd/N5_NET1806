namespace PetSpa.Payment
{
    public class VNPayConfig
    {
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string VnpUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}
