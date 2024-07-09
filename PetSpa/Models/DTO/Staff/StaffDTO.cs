using Microsoft.AspNetCore.Identity;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Manager;

namespace PetSpa.Models.DTO.Staff
{
    public class StaffDTO
    {
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }

        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public virtual IdentityUser User { get; set; } = null!;
        public virtual ICollection<BookingDetailDTO> BookingDetails { get; set; } = new List<BookingDetailDTO>();

    }
}
