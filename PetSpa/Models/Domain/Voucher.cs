using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Discount { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public Guid CusId { get; set; }

    public int ManaId { get; set; }

    public Guid BookingId { get; set; }

    public bool Status { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Customer Cus { get; set; } = null!;

    public virtual Manager Mana { get; set; } = null!;
}
