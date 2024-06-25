using PetSpa.Models.Domain;

public partial class BookingDetail
{
    public Guid BookingDetailId { get; set; }
    public Guid BookingId { get; set; }
    public Guid PetId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? StaffId { get; set; }
    public Guid? ComboId { get; set; }
    public bool Status { get; set; }
    public TimeSpan Duration { get; set; }
    public string ComboType { get; set; } = null!;
    public virtual Booking Booking { get; set; } = null!;
    public virtual Combo? Combo { get; set; }
    public virtual Pet Pet { get; set; } = null!;
    public virtual Service? Service { get; set; }
    public virtual Staff? Staff { get; set; }
}
