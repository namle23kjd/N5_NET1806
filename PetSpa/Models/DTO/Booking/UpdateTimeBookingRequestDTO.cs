namespace PetSpa.Models.DTO.Booking
{
    public class UpdateTimeBookingRequestDTO
    {
        public Guid BookingId { get; set; }
        public DateTime NewBookingSchedule { get; set; }
        public Guid? NewStaffId { get; set; }
    }
}
