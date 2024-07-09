using Microsoft.AspNetCore.Identity;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.Voucher;


namespace PetSpa.Models.DTO.Customer
{
    public class CustomerDTO
    {

        public Guid Id { get; set; }
        public Guid CusId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string CusRank { get; set; } = null!;
        public Guid AdminId { get; set; } // Thêm thuộc tính này để phản ánh quan hệ với Admin
        public decimal TotalSpent { get; set; } = 0;
        public string? Banking { get; set; }

        public virtual IdentityUser User { get; set; } = null!; // Quan hệ với bảng AspNetUsers
        public virtual AdminDTO Admins { get; set; } = null!; // Quan hệ với bảng Admin
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
        public virtual ICollection<PetDTO> Pets { get; set; } = new List<PetDTO>();
        public virtual ICollection<VoucherDTO> Vouchers { get; set; } = new List<VoucherDTO>();
    }
}
