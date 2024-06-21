using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Invoice
{
    public Guid BookingId { get; set; }
    public Guid InvoiceId { get; set; }
    public int PaymentId { get; set; }
    public decimal Price { get; set; }
    public virtual Booking Bookings { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;
}

