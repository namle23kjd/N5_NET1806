using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Invoice
{
    public Guid BookingId { get; set; }

    public int InvoiceId { get; set; }

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
