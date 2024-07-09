using PetSpa.Models.DTO.Booking;

namespace PetSpa.Models.DTO.Staff
{
    public class StaffBookingDTO
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public string PetName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; }
    }
}
