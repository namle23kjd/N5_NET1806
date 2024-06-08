
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Customer;
using PetSpa.Models.DTO.Manager;

namespace PetSpa.Models.DTO.Admin
{
    public class AdminDTO
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string UserName { get; set; }

    }
}
