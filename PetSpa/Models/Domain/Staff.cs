using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Staff
{
    public Guid AccId { get; set; }

    public Guid StaffId { get; set; }

    public string FullName { get; set; } 

    public string Gender { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
