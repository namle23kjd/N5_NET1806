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

        public BookingStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? ComboId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime BookingSchedule { get; set; }
        public string? Feedback { get; set; }
        public bool CheckAccept { get; set; }
        public string CustomerName { get; set; }
        public List<BookingDetailDTO>? BookingDetails { get; set; } = new List<BookingDetailDTO>();

        public virtual CustomerDTO Cus { get; set; } 

        public virtual InvoiceDTO? Invoice { get; set; }

        public virtual StaffDTO Staff { get; set; } = null!;

        public virtual VoucherDTO? Voucher { get; set; }
    }
}
