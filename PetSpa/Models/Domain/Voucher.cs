using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Voucher
{
    public Guid VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Discount { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public Guid CusId { get; set; }

    public Guid ManaId { get; set; }

    public Guid BookingId { get; set; }

    public bool Status { get; set; }

    public virtual Booking Bookings { get; set; } = null!;

    public virtual Customer Customers { get; set; } = null!;

    public virtual Manager Managers { get; set; } = null!;
}
