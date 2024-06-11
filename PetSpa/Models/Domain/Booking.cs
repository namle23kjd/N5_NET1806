using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Booking
{
    public Guid BookingId { get; set; }
    public Guid CusId { get; set; }
    public Guid ManaId { get; set; } // Thêm cột ManaId
    public bool Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? BookingSchedule { get; set; }
    public string? Feedback { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool CheckAccept { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual Manager Manager { get; set; } = null!; // Quan hệ với bảng Manager
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Voucher Voucher { get; set; } = null!;
}
