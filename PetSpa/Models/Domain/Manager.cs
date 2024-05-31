using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Manager
{
    public Guid AccId { get; set; }

    public Guid ManaId { get; set; }

    public string FullName { get; set; } 

    public string Gender { get; set; } 

    public string PhoneNumber { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
