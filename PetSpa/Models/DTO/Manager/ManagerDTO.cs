using Microsoft.AspNetCore.Identity;
using PetSpa.Models.DTO.Admin;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.Staff;
using PetSpa.Models.DTO.Voucher;

namespace PetSpa.Models.DTO.Manager
{
    public class ManagerDTO
    {
        public Guid ManaId { get; set; }
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public Guid AdminId { get; set; }

        public virtual IdentityUser User { get; set; } = null!;
        public virtual AdminDTO Admins { get; set; } = null!;
        public virtual ICollection<StaffDTO> Staffs { get; set; } = new List<StaffDTO>();
        public virtual ICollection<VoucherDTO> Vouchers { get; set; } = new List<VoucherDTO>();
        public virtual ICollection<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
    }
}
