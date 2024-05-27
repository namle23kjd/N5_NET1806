using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Customer
{
    public Guid AccId { get; set; }

    public Guid CusId { get; set; }

    public string FullName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? CusRank { get; set; } = null;

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
