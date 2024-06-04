using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Admin
{
    public Guid Id { get; set; }

    public Guid AdminId { get; set; }

    public IdentityUser User { get; set; }
    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

}
