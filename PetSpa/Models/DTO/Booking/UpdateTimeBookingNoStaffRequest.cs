namespace PetSpa.Models.DTO.Booking
{
    public class UpdateTimeBookingNoStaffRequest
    {
        public Guid BookingId { get; set; }
        public DateTime NewBookingSchedule { get; set; }
    }
}
