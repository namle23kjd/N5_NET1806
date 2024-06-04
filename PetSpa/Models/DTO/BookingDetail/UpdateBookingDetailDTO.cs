namespace PetSpa.Models.DTO.BookingDetail
{
    public class UpdateBookingDetailDTO
    {
        public Guid? ServiceId { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? ComboId { get; set; }

        public Guid BookingId { get; set; }

        public Guid PetId { get; set; }

        public string ComboType { get; set; } = null!;
    }
}
