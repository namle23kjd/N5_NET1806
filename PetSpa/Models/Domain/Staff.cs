using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSpa.Models.Domain;

public partial class Staff
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public Guid ManagerManaId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual List<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual Manager Manager { get; set; } = null!;
}
