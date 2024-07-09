namespace PetSpa.Models.DTO.Manager
{
    public class UpdateBookingWithStaffRequest
    {
        public Guid BookingId { get; set; }
        public Guid StaffId { get; set; }
    }
}
