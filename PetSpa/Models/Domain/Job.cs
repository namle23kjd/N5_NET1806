using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Job
{
    public int JobId { get; set; }

    public Guid StaffId { get; set; }

    public int? ManaId { get; set; }

    public Guid BookingDetailId { get; set; }

    public virtual BookingDetail BookingDetail { get; set; } = null!;

    public virtual Manager? Mana { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
