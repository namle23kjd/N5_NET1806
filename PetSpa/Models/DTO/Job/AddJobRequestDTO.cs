using System.Reflection;
using System.Text;

namespace PetSpa.Models.DTO.Job
{
    public class AddJobRequestDTO
    {
        public Guid JobId { get; set; }

        public Guid StaffId { get; set; }

        public Guid? ManaId { get; set; }

        public Guid BookingDetailId { get; set; }
    }
}
