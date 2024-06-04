using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.Combo;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.Service;
using PetSpa.Models.DTO.Staff;

namespace PetSpa.Models.DTO.BookingDetail
{
    public class BookingDetailDTO
    {
        public Guid? ServiceId { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? ComboId { get; set; }

        public Guid BookingDetailId { get; set; }

        public Guid BookingId { get; set; }

        public Guid PetId { get; set; }

        public string ComboType { get; set; } = null!;

        public virtual BookingDTO Booking { get; set; } = null!;

        public virtual ComboDTO? Combo { get; set; }

        public virtual PetDTO Pet { get; set; } = null!;

        public virtual ServiceDTO? Service { get; set; }

        public virtual StaffDTO? Staff { get; set; }

    }
}
