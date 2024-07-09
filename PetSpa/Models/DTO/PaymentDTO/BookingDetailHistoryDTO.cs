using PetSpa.Models.DTO.Booking;

namespace PetSpa.Models.DTO.PaymentDTO
{
    public class BookingDetailHistoryDTO
    {
        public Guid BookingId { get; set; }
        public Guid PetId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public Guid? ComboId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? StaffId { get; set; }
        public decimal? ServicePrice { get; set; }
        public CheckAccpectStatus CheckAccept { get; set; }
        public BookingStatus Status { get; set; }
        public string Feedback { get; set; }
        public DateTime BookingSchedule { get; set; }
    }
}
