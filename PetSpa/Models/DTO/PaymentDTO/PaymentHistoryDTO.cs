namespace PetSpa.Models.DTO.PaymentDTO
{
    public class PaymentHistoryDTO
    {
        public string CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationTime { get; set; }
        public List<string> ServicesOrCombos { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
