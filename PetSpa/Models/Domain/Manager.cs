using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSpa.Models.Domain;

public partial class Manager
{
    public Guid ManaId { get; set; }
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public Guid AdminId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Admin Admins { get; set; } = null!;
    public virtual List<Staff> Staffs { get; set; } = new List<Staff>();
    public virtual List<Voucher> Vouchers { get; set; } = new List<Voucher>();
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}
