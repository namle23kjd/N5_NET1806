using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Account;
using PetSpa.Models.DTO.Job;
using PetSpa.Models.DTO.Voucher;

namespace PetSpa.Models.DTO.Manager
{
    public class ManagerDTO
    {
        public Guid AccId { get; set; }

        public Guid ManaId { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public virtual AccountDTO Acc { get; set; } = null!;

        public virtual ICollection<JobDTO> Jobs { get; set; } = new List<JobDTO>();

        public virtual ICollection<VoucherDTO> Vouchers { get; set; } = new List<VoucherDTO>();
    }
}
