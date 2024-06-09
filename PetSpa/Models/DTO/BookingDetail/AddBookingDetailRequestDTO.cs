using System;

namespace PetSpa.Models.DTO.BookingDetail
{
    public class AddBookingDetailRequestDTO
    {
        public Guid PetId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? ComboId { get; set; }
        public Guid? StaffId { get; set; }
        public bool Status { get; set; }
        public string ComboType { get; set; } = null!;
    }
}
