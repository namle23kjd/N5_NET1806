using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Payment
{
    public int PaymentId { get; set; }
    public Guid CusId { get; set; }
    public string TransactionId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ExpirationTime { get; set; }
    public string PaymentMethod { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual List<Invoice> Invoices { get; set; } = new List<Invoice>();
}