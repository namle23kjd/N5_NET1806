using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Booking
{
    public Guid CusId { get; set; }

    public Guid BookingId { get; set; }

    public Guid StaffId { get; set; }

    public bool Status { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? BookingSchedule { get; set; }

    public string? Feedback { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Customer Cus { get; set; } = null!;

    public virtual Invoice? Invoice { get; set; }

    public virtual Staff Staff { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
