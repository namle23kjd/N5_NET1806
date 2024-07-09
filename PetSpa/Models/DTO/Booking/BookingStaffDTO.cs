namespace PetSpa.Models.DTO.Booking
{
    public class BookingStaffDTO
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public Guid ServiceId { get; set; }
        public string PetName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; }
        public bool CheckAccept { get; set; }
    }
}
