using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.Customer;

namespace PetSpa.Models.DTO.PaymentDTO
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public Guid CusId { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpirationTime { get; set; }
        public string PaymentMethod { get; set; }

        public string CustomerName { get; set; }
    }
}
