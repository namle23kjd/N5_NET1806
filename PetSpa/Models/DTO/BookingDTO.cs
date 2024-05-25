using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO
{
    public class BookingDTO
    {
        public Guid CusId { get; set; }

        public Guid BookingId { get; set; }

        public Guid StaffId { get; set; }

        public bool Status { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime? BookingSchedule { get; set; }

        public string? Feedback { get; set; }
    }
}
