using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO
{
    public class StaffRequestDTO
    {
        public Guid AccId { get; set; }

        public Guid StaffId { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; } = null!;

    }
}
