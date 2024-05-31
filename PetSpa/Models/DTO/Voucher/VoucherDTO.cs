using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO.Voucher
{
    public class VoucherDTO
    {
        public Guid VoucherId { get; set; }

        public string Code { get; set; } = null!;

        public decimal Discount { get; set; }

        public DateOnly IssueDate { get; set; }

        public DateOnly ExpiryDate { get; set; }

        public Guid CusId { get; set; }

        public Guid ManaId { get; set; }

        public Guid BookingId { get; set; }

        public bool Status { get; set; }

    }
}
