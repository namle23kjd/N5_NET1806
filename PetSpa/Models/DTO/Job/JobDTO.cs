using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO.Job
{
    public class JobDTO
    {
        public Guid JobId { get; set; }

        public Guid StaffId { get; set; }

        public Guid ManaId { get; set; }

        public Guid BookingDetailId { get; set; }

    }
}
