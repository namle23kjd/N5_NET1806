using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO
{
    public class JobDTO
    {
        public int JobId { get; set; }

        public Guid StaffId { get; set; }

        public int? ManaId { get; set; }

        public Guid BookingDetailId { get; set; }

    }
}
