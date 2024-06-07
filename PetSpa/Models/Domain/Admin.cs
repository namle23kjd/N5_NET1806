using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSpa.Models.Domain;

public partial class Admin
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public ApplicationUser User { get; set; }
    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

}
