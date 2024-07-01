using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSpa.Models.Domain;

public partial class Manager
{
    public Guid ManaId { get; set; }
    public Guid Id { get; set; }
    public string? FullName { get; set; } 
    public string? Gender { get; set; } 
    public string? PhoneNumber { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual List<Staff> Staffs { get; set; } = new List<Staff>();
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}
