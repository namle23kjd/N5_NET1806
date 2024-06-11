using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Invoice
{
    public Guid BookingId { get; set; }

    public Guid InvoiceId { get; set; }

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; }
}
