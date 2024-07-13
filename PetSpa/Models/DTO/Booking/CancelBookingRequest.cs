namespace PetSpa.Models.DTO.Booking
{
    public class CancelBookingRequest
    {
        public Guid BookingId { get; set; }
        public string BankingInfo { get; set; }= string.Empty;
    }
}
