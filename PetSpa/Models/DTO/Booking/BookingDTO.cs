using PetSpa.Models.Domain;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Customer;
using PetSpa.Models.DTO.Invoice;
using PetSpa.Models.DTO.Staff;
using PetSpa.Models.DTO.Voucher;

namespace PetSpa.Models.DTO.Booking
{
    public class BookingDTO
    {
        public Guid BookingId { get; set; }
        public Guid CusId { get; set; }

        public Guid StaffId { get; set; }

        public bool Status { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime? BookingSchedule { get; set; }

        public string? Feedback { get; set; }

        public List<BookingDetailDTO>? BookingDetails { get; set; }

        public virtual CustomerDTO Cus { get; set; } 

        public virtual InvoiceDTO? Invoice { get; set; }

        public virtual StaffDTO Staff { get; set; } = null!;

        public virtual VoucherDTO? Voucher { get; set; }
    }
}
