using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;

public partial class Booking
{
    public Guid BookingId { get; set; }
    public Guid CusId { get; set; }
    public Guid ManaId { get; set; }
    public BookingStatus? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime BookingSchedule { get; set; }
    public string? Feedback { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool CheckAccept { get; set; }
    public int? PaymentId { get; set; }
    public Guid? InvoiceId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual Manager Manager { get; set; } = null!;
    public virtual List<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual Payment Payments { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Voucher Voucher { get; set; } = null!;
}
